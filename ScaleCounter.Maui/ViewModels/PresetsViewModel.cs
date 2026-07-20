using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using ScaleCounter.Core;
using ScaleCounter.Maui.Services;

namespace ScaleCounter.Maui.ViewModels;

/// <summary>Lists the saved presets: select the active one, add, edit or delete.</summary>
public partial class PresetsViewModel : ObservableObject
{
	private readonly IPresetStore _store;

	[ObservableProperty] private string? _activeId;
	[ObservableProperty] private string _activeName = "";

	public ObservableCollection<WeighedItemPreset> Presets { get; } = new();

	/// <summary>Raised to ask the view to open the calibration editor for a preset.</summary>
	public event EventHandler<WeighedItemPreset>? EditRequested;

	/// <summary>Raised to ask the view to export/share a single preset to a file.</summary>
	public event EventHandler<WeighedItemPreset>? ShareRequested;

	public PresetsViewModel(IPresetStore store)
	{
		_store = store;
		Refresh();
	}

	public void Refresh()
	{
		Presets.Clear();
		foreach (var preset in _store.Presets)
			Presets.Add(preset);
		ActiveId = _store.ActiveId;
		ActiveName = _store.Active?.Name ?? "-";
	}

	[RelayCommand]
	private void Select(WeighedItemPreset preset)
	{
		_store.SetActive(preset.Id);
		ActiveId = preset.Id;
		ActiveName = preset.Name;
	}

	[RelayCommand]
	private void Edit(WeighedItemPreset preset) => EditRequested?.Invoke(this, preset);

	[RelayCommand]
	private void Share(WeighedItemPreset preset) => ShareRequested?.Invoke(this, preset);

	[RelayCommand]
	private void Add() => EditRequested?.Invoke(this, new WeighedItemPreset());

	[RelayCommand]
	private void Delete(WeighedItemPreset preset)
	{
		_store.Delete(preset.Id);
		Refresh();
	}

	public string ExportJson(IEnumerable<WeighedItemPreset> presets) => _store.ExportJson(presets);

	public int ImportJson(string json)
	{
		int count = _store.ImportJson(json);
		Refresh();
		return count;
	}

	public int LoadDefaults()
	{
		int count = _store.LoadDefaults();
		Refresh();
		return count;
	}
}
