using ScaleLib;
using ScaleCounter.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Windows.Forms;

namespace USBScaleCounter
{
    /// <summary>Manages the saved presets: pick the active one, add, edit, delete, export and import.</summary>
    internal sealed class PresetsForm : Form
    {
        private readonly PresetStore _store;
        private readonly Scale _scale;
        private readonly ListBox _list = new ListBox();
        private readonly Label _activeLabel = new Label();

        public PresetsForm(PresetStore store, Scale scale)
        {
            _store = store;
            _scale = scale;

            Text = "Presets";
            ClientSize = new Size(470, 380);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            MaximizeBox = false;
            ShowIcon = false;

            _activeLabel.Dock = DockStyle.Top;
            _activeLabel.Height = 26;
            _activeLabel.TextAlign = ContentAlignment.MiddleLeft;
            _activeLabel.Padding = new Padding(8, 0, 0, 0);

            _list.Dock = DockStyle.Fill;
            _list.IntegralHeight = false;
            _list.DisplayMember = "Name";
            _list.SelectionMode = SelectionMode.MultiExtended;

            var buttons = new FlowLayoutPanel
            {
                Dock = DockStyle.Bottom,
                Height = 80,
                Padding = new Padding(6),
                FlowDirection = FlowDirection.LeftToRight,
                WrapContents = true
            };
            AddButton(buttons, "Set active", (s, e) => { var p = First(); if (p != null) { _store.SetActive(p.Id); RefreshList(); } });
            AddButton(buttons, "Add", (s, e) => EditPreset(new WeighedItemPreset()));
            AddButton(buttons, "Edit", (s, e) => { var p = First(); if (p != null) EditPreset(p); });
            AddButton(buttons, "Delete", (s, e) => { var p = First(); if (p != null) { _store.Delete(p.Id); RefreshList(); } });
            AddButton(buttons, "Export selected...", (s, e) => ExportSelected());
            AddButton(buttons, "Import...", (s, e) => ImportPresets());
            AddButton(buttons, "Load defaults", (s, e) => LoadDefaults());
            AddButton(buttons, "Close", (s, e) => Close());

            Controls.Add(_list);
            Controls.Add(buttons);
            Controls.Add(_activeLabel);

            RefreshList();
        }

        private static void AddButton(Control parent, string text, EventHandler onClick)
        {
            var button = new Button { Text = text, AutoSize = true, Margin = new Padding(3) };
            button.Click += onClick;
            parent.Controls.Add(button);
        }

        private WeighedItemPreset First() =>
            _list.SelectedItems.Count > 0 ? _list.SelectedItems[0] as WeighedItemPreset : null;

        private List<WeighedItemPreset> SelectedPresets() =>
            _list.SelectedItems.Cast<WeighedItemPreset>().ToList();

        private void EditPreset(WeighedItemPreset preset)
        {
            using (var form = new CalibrationForm(preset, _scale))
            {
                if (form.ShowDialog(this) == DialogResult.OK)
                    _store.Save(preset);
            }

            RefreshList();
        }

        private void ExportSelected()
        {
            var selected = SelectedPresets();
            if (selected.Count == 0)
            {
                MessageBox.Show(this, "Select one or more presets to export.", "Export");
                return;
            }

            var filter = PresetFile.Description + " (*" + PresetFile.Extension + ")|*" + PresetFile.Extension + "|All files (*.*)|*.*";
            using (var dialog = new SaveFileDialog
            {
                Filter = filter,
                DefaultExt = PresetFile.Extension.TrimStart('.'),
                FileName = SafeFileName(selected.Count == 1 ? selected[0].Name : "presets") + PresetFile.Extension
            })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    File.WriteAllText(dialog.FileName, _store.ExportJson(selected));
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Export failed");
                }
            }
        }

        private void ImportPresets()
        {
            var filter = PresetFile.Description + " (*" + PresetFile.Extension + ")|*" + PresetFile.Extension + "|All files (*.*)|*.*";
            using (var dialog = new OpenFileDialog { Filter = filter })
            {
                if (dialog.ShowDialog(this) != DialogResult.OK) return;
                try
                {
                    var count = _store.ImportFile(dialog.FileName);
                    RefreshList();
                    MessageBox.Show(this,
                        count > 0 ? $"Imported {count} preset(s)." : "No presets found in the file.",
                        "Import");
                }
                catch (Exception ex)
                {
                    MessageBox.Show(this, ex.Message, "Import failed");
                }
            }
        }

        private void LoadDefaults()
        {
            if (MessageBox.Show(this,
                    "Add the built-in default presets? Existing presets with the same id are updated.",
                    "Load defaults", MessageBoxButtons.OKCancel) != DialogResult.OK)
                return;

            var count = _store.LoadDefaults();
            RefreshList();
            MessageBox.Show(this, $"Loaded {count} preset(s).", "Load defaults");
        }

        private static string SafeFileName(string name)
        {
            foreach (var c in Path.GetInvalidFileNameChars())
                name = name.Replace(c, '_');
            return string.IsNullOrWhiteSpace(name) ? "preset" : name;
        }

        private void RefreshList()
        {
            _list.DataSource = null;
            _list.DataSource = _store.Presets.ToList();
            _list.DisplayMember = "Name";

            var active = _store.Active;
            _activeLabel.Text = "Active preset: " + (active != null ? active.Name : "-");
        }
    }
}
