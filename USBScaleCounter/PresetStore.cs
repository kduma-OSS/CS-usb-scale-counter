using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Web.Script.Serialization;
using ScaleCounter.Core;

namespace USBScaleCounter
{
    /// <summary>
    /// Persists weighed-item presets as indented JSON in %AppData%\USBScaleCounter\presets.json,
    /// and exports/imports selected presets as portable ".uscpreset" files. Uses the shared
    /// <see cref="WeighedItemPreset"/> model and seeds a default preset on first run.
    /// </summary>
    internal sealed class PresetStore
    {
        private sealed class Document
        {
            public string ActiveId { get; set; }
            public List<WeighedItemPreset> Presets { get; set; } = new List<WeighedItemPreset>();

            // App-level multi-count selection (preset ids). Null = not configured (show all).
            public List<string> MultiCountIds { get; set; }

            // Whether audible count signals are enabled.
            public bool SoundEnabled { get; set; } = true;
        }

        private readonly string _path;
        private readonly JavaScriptSerializer _serializer = new JavaScriptSerializer();
        private Document _doc = new Document();

        public PresetStore()
        {
            var dir = Path.Combine(
                Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
                "USBScaleCounter");
            Directory.CreateDirectory(dir);
            _path = Path.Combine(dir, "presets.json");
            Reload();
        }

        public IReadOnlyList<WeighedItemPreset> Presets => _doc.Presets;
        public string ActiveId => _doc.ActiveId;

        public WeighedItemPreset Active =>
            _doc.Presets.FirstOrDefault(p => p.Id == _doc.ActiveId) ?? _doc.Presets.FirstOrDefault();

        public void Reload()
        {
            try
            {
                if (File.Exists(_path))
                    _doc = _serializer.Deserialize<Document>(File.ReadAllText(_path)) ?? new Document();
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
        }

        public void SetActive(string id)
        {
            if (!_doc.Presets.Any(p => p.Id == id)) return;
            _doc.ActiveId = id;
            Persist();
        }

        public void Save(WeighedItemPreset preset)
        {
            var index = _doc.Presets.FindIndex(p => p.Id == preset.Id);
            if (index >= 0)
                _doc.Presets[index] = preset;
            else
            {
                _doc.Presets.Add(preset);
                if (string.IsNullOrEmpty(_doc.ActiveId)) _doc.ActiveId = preset.Id;
            }
            Persist();
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
        }

        /// <summary>Serializes the given presets to indented JSON (for a .uscpreset file).</summary>
        public string ExportJson(IEnumerable<WeighedItemPreset> presets) =>
            Indent(_serializer.Serialize(presets.ToList()));

        /// <summary>Merges presets from a .uscpreset file's JSON; returns how many were merged.</summary>
        public int ImportJson(string json)
        {
            var imported = _serializer.Deserialize<List<WeighedItemPreset>>(json);
            var count = PresetCollection.Merge(_doc.Presets, imported);
            if (count > 0) Persist();
            return count;
        }

        /// <summary>Imports presets from a file on disk; returns how many were merged.</summary>
        public int ImportFile(string path) => ImportJson(File.ReadAllText(path));

        /// <summary>The bundled default presets (parsed from the embedded resource).</summary>
        private List<WeighedItemPreset> DefaultPresets() =>
            _serializer.Deserialize<List<WeighedItemPreset>>(PresetDefaults.Json) ?? new List<WeighedItemPreset>();

        /// <summary>Merges the bundled default presets into the store; returns how many were merged.</summary>
        public int LoadDefaults()
        {
            var count = PresetCollection.Merge(_doc.Presets, DefaultPresets());
            if (count > 0) Persist();
            return count;
        }

        /// <summary>Preset ids shown in the multi-count panel (defaults to all presets).</summary>
        public List<string> GetMultiCountIds() =>
            _doc.MultiCountIds ?? _doc.Presets.Select(p => p.Id).ToList();

        public void SetMultiCountIds(IEnumerable<string> ids)
        {
            _doc.MultiCountIds = ids.ToList();
            Persist();
        }

        public bool GetSoundEnabled() => _doc.SoundEnabled;

        public void SetSoundEnabled(bool value)
        {
            _doc.SoundEnabled = value;
            Persist();
        }

        private void Persist()
        {
            try
            {
                File.WriteAllText(_path, Indent(_serializer.Serialize(_doc)));
            }
            catch
            {
                // Best-effort: a failed write shouldn't crash the app.
            }
        }

        /// <summary>Pretty-prints compact JSON with two-space indentation.</summary>
        private static string Indent(string json)
        {
            var sb = new StringBuilder(json.Length * 2);
            int depth = 0;
            bool inString = false, escape = false;

            for (int i = 0; i < json.Length; i++)
            {
                char c = json[i];

                if (inString)
                {
                    sb.Append(c);
                    if (escape) escape = false;
                    else if (c == '\\') escape = true;
                    else if (c == '"') inString = false;
                    continue;
                }

                switch (c)
                {
                    case '"':
                        sb.Append(c);
                        inString = true;
                        break;
                    case '{':
                    case '[':
                        sb.Append(c);
                        char close = c == '{' ? '}' : ']';
                        if (i + 1 < json.Length && json[i + 1] == close)
                        {
                            sb.Append(close); // keep empty {} / [] on one line
                            i++;
                        }
                        else
                        {
                            depth++;
                            NewLine(sb, depth);
                        }
                        break;
                    case '}':
                    case ']':
                        depth--;
                        NewLine(sb, depth);
                        sb.Append(c);
                        break;
                    case ',':
                        sb.Append(c);
                        NewLine(sb, depth);
                        break;
                    case ':':
                        sb.Append(": ");
                        break;
                    default:
                        sb.Append(c);
                        break;
                }
            }

            return sb.ToString();
        }

        private static void NewLine(StringBuilder sb, int depth)
        {
            sb.Append('\n');
            sb.Append(' ', depth * 2);
        }
    }
}
