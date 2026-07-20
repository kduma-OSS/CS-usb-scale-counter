using System.Text.Json;
using ScaleCounter.Core;

namespace ScaleCounter.Maui.Services;

/// <summary>
/// Persists presets as JSON in the app data directory. Seeds a default preset on first run
/// so the counter is usable out of the box (matching the desktop's old 25 / 580 g default).
/// </summary>
public sealed class JsonPresetStore : IPresetStore
{
	private sealed class Document
	{
		public string? ActiveId { get; set; }
		public List<WeighedItemPreset> Presets { get; set; } = new();
	}

	private static readonly JsonSerializerOptions JsonOptions = new() { WriteIndented = true };

	private readonly string _path;
	private Document _doc = new();

	public event EventHandler? Changed;

	public JsonPresetStore()
	{
		_path = Path.Combine(FileSystem.AppDataDirectory, "presets.json");
		Reload();
	}

	public IReadOnlyList<WeighedItemPreset> Presets => _doc.Presets;
	public string? ActiveId => _doc.ActiveId;

	public WeighedItemPreset? Active =>
		_doc.Presets.FirstOrDefault(p => p.Id == _doc.ActiveId) ?? _doc.Presets.FirstOrDefault();

	public void Reload()
	{
		try
		{
			if (File.Exists(_path))
				_doc = JsonSerializer.Deserialize<Document>(File.ReadAllText(_path)) ?? new Document();
		}
		catch
		{
			_doc = new Document();
		}

		if (_doc.Presets.Count == 0)
		{
			_doc.Presets.AddRange(DefaultPresets());
			_doc.ActiveId = _doc.Presets[0].Id;
			Persist();
		}
		else if (string.IsNullOrEmpty(_doc.ActiveId) || _doc.Presets.All(p => p.Id != _doc.ActiveId))
		{
			_doc.ActiveId = _doc.Presets[0].Id;
		}

		Changed?.Invoke(this, EventArgs.Empty);
	}

	public void SetActive(string id)
	{
		if (!_doc.Presets.Any(p => p.Id == id))
			return;

		_doc.ActiveId = id;
		Persist();
		Changed?.Invoke(this, EventArgs.Empty);
	}

	public void Save(WeighedItemPreset preset)
	{
		int index = _doc.Presets.FindIndex(p => p.Id == preset.Id);
		if (index >= 0)
			_doc.Presets[index] = preset;
		else
		{
			_doc.Presets.Add(preset);
			_doc.ActiveId ??= preset.Id;
		}

		Persist();
		Changed?.Invoke(this, EventArgs.Empty);
	}

	public void Delete(string id)
	{
		_doc.Presets.RemoveAll(p => p.Id == id);

		if (_doc.Presets.Count == 0)
		{
			_doc.Presets.AddRange(DefaultPresets());
			_doc.ActiveId = _doc.Presets[0].Id;
		}
		else if (_doc.ActiveId == id)
		{
			_doc.ActiveId = _doc.Presets[0].Id;
		}

		Persist();
		Changed?.Invoke(this, EventArgs.Empty);
	}

	private static List<WeighedItemPreset> DefaultPresets() =>
		JsonSerializer.Deserialize<List<WeighedItemPreset>>(PresetDefaults.Json) ?? new List<WeighedItemPreset>();

	public int LoadDefaults()
	{
		int count = PresetCollection.Merge(_doc.Presets, DefaultPresets());
		if (count > 0)
		{
			Persist();
			Changed?.Invoke(this, EventArgs.Empty);
		}
		return count;
	}

	public string ExportJson(IEnumerable<WeighedItemPreset> presets) =>
		JsonSerializer.Serialize(presets.ToList(), JsonOptions); // JsonOptions has WriteIndented = true

	public int ImportJson(string json)
	{
		var imported = JsonSerializer.Deserialize<List<WeighedItemPreset>>(json);
		int count = PresetCollection.Merge(_doc.Presets, imported);
		if (count > 0)
		{
			Persist();
			Changed?.Invoke(this, EventArgs.Empty);
		}
		return count;
	}

	private void Persist()
	{
		try
		{
			File.WriteAllText(_path, JsonSerializer.Serialize(_doc, JsonOptions));
		}
		catch
		{
			// Best-effort: a failed write shouldn't crash the app.
		}
	}
}
