using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Maui.Dispatching;
using Microsoft.Maui.Storage;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;
using ScaleCounter.Maui.Views;
using UnitsNet;

namespace ScaleCounter.Maui.ViewModels;

/// <summary>
/// Dashboard view-model. Reads the scale and counts against the active preset
/// (calibrated per-item weight + tare + target) via the shared <see cref="ItemCounter"/>.
/// </summary>
public partial class MainViewModel : ObservableObject
{
	private readonly IScale _scale;
	private readonly ItemCounter _counter;
	private readonly IPresetStore _presets;
	private readonly ISoundPlayer _sound;

	// The scale reports "Stable" almost continuously, so we debounce stability ourselves:
	// every new reading restarts this timer, and the value is only treated as settled
	// (and sound signals fire) once the weight has been quiet for the whole interval.
	private readonly IDispatcherTimer _settleTimer;
	private bool _isStable;
	private CountState? _previousStableState;
	private bool _soundOn;

	[ObservableProperty] private string _soundToggleText = "🔔";
	[ObservableProperty] private string _statusText = "Connect the scale!";
	[ObservableProperty] private string _diffText = "-";
	[ObservableProperty] private string _weightText = "- g";
	[ObservableProperty] private string _progressText = "- / -";
	[ObservableProperty] private string _presetText = "-";
	[ObservableProperty] private string _connectionText = "Disconnected";
	[ObservableProperty] private string _scaleStatusText = "-";
	[ObservableProperty] private string _footerText = "";
	[ObservableProperty] private double _progress;
	[ObservableProperty] private Color _backgroundColor = Colors.LightGray;
	[ObservableProperty] private Color _foregroundColor = Colors.Black;

	public MainViewModel(IScale scale, ItemCounter counter, IPresetStore presets, ISoundPlayer sound, IDispatcher dispatcher)
	{
		_scale = scale;
		_counter = counter;
		_presets = presets;
		_sound = sound;

		_settleTimer = dispatcher.CreateTimer();
		_settleTimer.Interval = TimeSpan.FromMilliseconds(500);
		_settleTimer.IsRepeating = false;
		_settleTimer.Tick += (_, _) => OnStable();

		_soundOn = Preferences.Get("SoundEnabled", true);
		UpdateSoundText();

		ApplyActivePreset();

		_scale.IsConnectedChanged += OnConnectionChanged;
		_scale.WeightChanged += OnWeightChanged;
		_scale.StatusChanged += OnStatusChanged;
		_presets.Changed += OnPresetsChanged;

		_scale.Start();
		UpdateInterface();
	}

	/// <summary>Re-applies the active preset (called when returning to the page).</summary>
	public void ReloadPreset()
	{
		ApplyActivePreset();
		UpdateInterface();
	}

	private void ApplyActivePreset()
	{
		var preset = _presets.Active;
		if (preset != null)
		{
			_counter.Apply(preset);
			PresetText = preset.Name;
		}
	}

	private void OnPresetsChanged(object? sender, EventArgs e) => ReloadPreset();
	private void OnConnectionChanged(object? sender, bool e) => UpdateInterface();
	private void OnWeightChanged(object? sender, Mass e) => UpdateInterface();
	private void OnStatusChanged(object? sender, StatusEnum e) => UpdateInterface();

	[RelayCommand]
	private async Task GoToPresets()
	{
		var page = IPlatformApplication.Current!.Services.GetRequiredService<PresetsPage>();
		await Shell.Current.Navigation.PushAsync(page);
	}

	[RelayCommand]
	private void ToggleSound()
	{
		_soundOn = !_soundOn;
		Preferences.Set("SoundEnabled", _soundOn);
		UpdateSoundText();
	}

	private void UpdateSoundText() => SoundToggleText = _soundOn ? "🔔" : "🔕";

	private void UpdateInterface()
	{
		ConnectionText = _scale.IsConnected ? "Connected" : "Disconnected";

		if (!_scale.IsConnected)
		{
			_settleTimer.Stop();
			_isStable = false;
			_previousStableState = null;
			SetNeutral("Connect the scale!");
			RefreshFooter();
			return;
		}

		WeightText = _scale.Weight.Grams.ToString(CultureInfo.InvariantCulture) + " g";

		var result = _counter.Count(_scale.Weight);
		DiffText = result.Diff;
		StatusText = result.Message;
		ProgressText = result.State == CountState.Uncalibrated ? "- / -" : $"{result.Count} / {result.Expected}";
		Progress = result.Expected > 0 && result.State != CountState.Uncalibrated
			? Math.Clamp((double)result.Count / result.Expected, 0, 1)
			: 0;

		(BackgroundColor, ForegroundColor) = result.State switch
		{
			CountState.Exact => (Colors.Green, Colors.White),
			CountState.NotEnough => (Colors.Orange, Colors.White),
			CountState.TooMany => (Colors.Red, Colors.White),
			_ => (Colors.LightGray, Colors.Black) // Empty / Uncalibrated
		};

		// Restart the settle timer on every reading; the sound signal only fires from OnStable().
		_isStable = false;
		_settleTimer.Stop();
		_settleTimer.Start();
		RefreshFooter();
	}

	// Fires once the weight has been quiet for the settle interval — the reading is "stable".
	private void OnStable()
	{
		_settleTimer.Stop();
		_isStable = true;
		RefreshFooter();

		if (!_scale.IsConnected)
			return;

		var result = _counter.Count(_scale.Weight);
		if (_soundOn && _previousStableState.HasValue)
		{
			var signal = CountSignals.ForTransition(_previousStableState.Value, result.State);
			if (signal != CountSignal.None)
				_sound.Play(signal);
		}
		_previousStableState = result.State;
	}

	private void RefreshFooter()
	{
		ScaleStatusText = !_scale.IsConnected ? "-" : (_isStable ? "Stable" : "Measuring…");
		FooterText = $"{PresetText}  ·  {ConnectionText}  ·  {ScaleStatusText}";
	}

	private void SetNeutral(string message)
	{
		BackgroundColor = Colors.LightGray;
		ForegroundColor = Colors.Black;
		StatusText = message;
		DiffText = "-";
		ProgressText = "- / -";
		WeightText = "- g";
		Progress = 0;
	}
}
