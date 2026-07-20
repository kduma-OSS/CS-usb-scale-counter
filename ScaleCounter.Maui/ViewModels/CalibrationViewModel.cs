using System.Collections.ObjectModel;
using System.Globalization;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;

namespace ScaleCounter.Maui.ViewModels;

/// <summary>One editable calibration point (known quantity + captured weight).</summary>
public partial class SampleRow : ObservableObject
{
	[ObservableProperty] private int _quantity;
	[ObservableProperty] private double _weightGrams;

	public string Display => $"{Quantity} pcs  →  {WeightGrams.ToString("0.##", CultureInfo.InvariantCulture)} g";

	public CalibrationSample ToSample() => new(Quantity, WeightGrams);
}

/// <summary>
/// Edits (and re-calibrates) a preset: capture several (quantity, weight) points, fit the
/// per-item weight + tare live, then save. A point with quantity 0 measures the tare.
/// </summary>
public partial class CalibrationViewModel : ObservableObject
{
	private readonly IPresetStore _store;
	private readonly IScale _scale;
	private WeighedItemPreset _preset = new();

	[ObservableProperty] private string _name = "";
	[ObservableProperty] private string _targetQuantity = "25";
	[ObservableProperty] private string _newQuantity = "";
	[ObservableProperty] private string _currentWeightText = "- g";
	[ObservableProperty] private string _resultText = "";
	[ObservableProperty] private bool _canSave;

	public ObservableCollection<SampleRow> Samples { get; } = new();

	/// <summary>Raised after Save or Cancel so the page can navigate back.</summary>
	public event EventHandler? Finished;

	public CalibrationViewModel(IPresetStore store, IScale scale)
	{
		_store = store;
		_scale = scale;
		_scale.WeightChanged += OnWeightChanged;
		UpdateCurrentWeight();
	}

	public void Load(WeighedItemPreset preset)
	{
		_preset = preset;
		Name = preset.Name;
		TargetQuantity = preset.TargetQuantity.ToString(CultureInfo.InvariantCulture);

		Samples.Clear();
		foreach (var sample in preset.Samples)
			Samples.Add(new SampleRow { Quantity = sample.Quantity, WeightGrams = sample.WeightGrams });

		Recompute();
	}

	private void OnWeightChanged(object? sender, UnitsNet.Mass e) => UpdateCurrentWeight();

	private void UpdateCurrentWeight() =>
		CurrentWeightText = _scale.IsConnected
			? _scale.Weight.Grams.ToString("0.##", CultureInfo.InvariantCulture) + " g"
			: "- g";

	[RelayCommand]
	private void Capture()
	{
		if (!int.TryParse(NewQuantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity < 0)
			return;

		Samples.Add(new SampleRow { Quantity = quantity, WeightGrams = _scale.Weight.Grams });
		NewQuantity = "";
		Recompute();
	}

	[RelayCommand]
	private void RemoveSample(SampleRow row)
	{
		Samples.Remove(row);
		Recompute();
	}

	private void Recompute()
	{
		var result = Calibration.Fit(Samples.Select(s => s.ToSample()).ToList());
		if (result.IsValid)
		{
			ResultText =
				$"Per item: {result.PerItemWeightGrams.ToString("0.##", CultureInfo.InvariantCulture)} g" +
				$"  ·  Tare: {result.TareGrams.ToString("0.##", CultureInfo.InvariantCulture)} g" +
				$"  ·  R²: {result.RSquared.ToString("0.###", CultureInfo.InvariantCulture)}";
			CanSave = true;
		}
		else
		{
			ResultText = result.Error ?? "Add measurements to calibrate.";
			CanSave = false;
		}
	}

	[RelayCommand]
	private void Save()
	{
		var result = Calibration.Fit(Samples.Select(s => s.ToSample()).ToList());
		if (!result.IsValid)
			return;

		_preset.Name = string.IsNullOrWhiteSpace(Name) ? "Preset" : Name.Trim();
		_preset.PerItemWeightGrams = result.PerItemWeightGrams;
		_preset.TareGrams = result.TareGrams;
		_preset.TargetQuantity =
			int.TryParse(TargetQuantity, NumberStyles.Integer, CultureInfo.InvariantCulture, out var target) && target >= 1
				? target
				: 1;
		_preset.Samples = Samples.Select(s => s.ToSample()).ToList();

		_store.Save(_preset);
		Finish();
	}

	[RelayCommand]
	private void Cancel() => Finish();

	private void Finish()
	{
		_scale.WeightChanged -= OnWeightChanged;
		Finished?.Invoke(this, EventArgs.Empty);
	}
}
