using ScaleCounter.Core;

namespace ScaleCounter.Maui.Services;

/// <summary>Stores the weighed-item presets and tracks which one is active.</summary>
public interface IPresetStore
{
	IReadOnlyList<WeighedItemPreset> Presets { get; }
	WeighedItemPreset? Active { get; }
	string? ActiveId { get; }

	/// <summary>Raised whenever the preset list or the active preset changes.</summary>
	event EventHandler? Changed;

	void SetActive(string id);
	void Save(WeighedItemPreset preset); // add or update by Id
	void Delete(string id);
	void Reload();

	/// <summary>Serializes the given presets to indented, portable JSON (for export/sharing).</summary>
	string ExportJson(IEnumerable<WeighedItemPreset> presets);

	/// <summary>Merges presets from exported JSON into the store; returns how many were merged.</summary>
	int ImportJson(string json);

	/// <summary>Merges the bundled default presets into the store; returns how many were merged.</summary>
	int LoadDefaults();
}
