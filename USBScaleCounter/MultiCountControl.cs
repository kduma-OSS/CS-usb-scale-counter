using ScaleLib;
using ScaleCounter.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Windows.Forms;

namespace USBScaleCounter
{
    /// <summary>
    /// Shows, for the current scale weight, the item count for several presets at once — one
    /// mini colored tile per preset. Which presets appear is an app-level choice (Configure…),
    /// not a property of the preset. Hosted docked in the main window; can be popped out.
    /// </summary>
    internal sealed class MultiCountControl : UserControl
    {
        private readonly PresetStore _store;
        private readonly Scale _scale;
        private readonly ItemCounter _counter = new ItemCounter();
        private readonly List<Tile> _tiles = new List<Tile>();

        private readonly Panel _body = new Panel { Dock = DockStyle.Fill };
        private readonly Button _popOutButton = new Button { Text = "Pop out", AutoSize = true };

        public event EventHandler PopOutClicked;

        public MultiCountControl(PresetStore store, Scale scale)
        {
            _store = store;
            _scale = scale;

            BuildUi();
            Reload();

            _scale.WeightChanged += OnWeightChanged;
            _scale.IsConnectedChanged += OnConnectedChanged;
        }

        private void BuildUi()
        {
            var header = new TableLayoutPanel
            {
                Dock = DockStyle.Top,
                Height = 40,
                ColumnCount = 3,
                RowCount = 1,
                Padding = new Padding(0, 2, 2, 2)
            };
            header.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));
            header.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            var title = new Label
            {
                Text = "Multi-count",
                Dock = DockStyle.Fill,
                TextAlign = ContentAlignment.MiddleLeft,
                Padding = new Padding(6, 0, 0, 0),
                Font = new Font(Font, FontStyle.Bold)
            };

            var configureButton = new Button { Text = "Configure…", AutoSize = true, Anchor = AnchorStyles.Right, Margin = new Padding(3) };
            configureButton.Click += (s, e) => ConfigureMulti();

            _popOutButton.AutoSize = true;
            _popOutButton.Anchor = AnchorStyles.Right;
            _popOutButton.Margin = new Padding(3);
            _popOutButton.Click += (s, e) => PopOutClicked?.Invoke(this, EventArgs.Empty);

            header.Controls.Add(title, 0, 0);
            header.Controls.Add(configureButton, 1, 0);
            header.Controls.Add(_popOutButton, 2, 0);

            Controls.Add(_body);
            Controls.Add(header);
        }

        /// <summary>Rebuilds the tiles from the configured multi-count presets.</summary>
        public void Reload()
        {
            _tiles.Clear();
            _body.Controls.Clear();

            var ids = new HashSet<string>(_store.GetMultiCountIds());
            var presets = _store.Presets.Where(p => ids.Contains(p.Id)).ToList();

            if (presets.Count == 0)
            {
                _body.Controls.Add(new Label
                {
                    Text = "No presets selected.\nUse “Configure…” to choose which presets to count.",
                    Dock = DockStyle.Fill,
                    TextAlign = ContentAlignment.MiddleCenter,
                    ForeColor = SystemColors.GrayText
                });
                return;
            }

            var table = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, RowCount = presets.Count };
            for (int i = 0; i < presets.Count; i++)
            {
                table.RowStyles.Add(new RowStyle(SizeType.Percent, 100f / presets.Count));
                var tile = new Tile(presets[i]);
                _tiles.Add(tile);
                table.Controls.Add(tile.Panel, 0, i);
            }
            _body.Controls.Add(table);

            UpdateCounts();
        }

        public void SetPoppedOut(bool poppedOut) => _popOutButton.Text = poppedOut ? "Dock" : "Pop out";

        private void ConfigureMulti()
        {
            var presets = _store.Presets.ToList();
            var selected = new HashSet<string>(_store.GetMultiCountIds());

            using (var dialog = new Form
            {
                Text = "Multi-count presets",
                ClientSize = new Size(300, 360),
                StartPosition = FormStartPosition.CenterParent,
                MinimizeBox = false,
                MaximizeBox = false,
                ShowIcon = false
            })
            {
                var list = new CheckedListBox { Dock = DockStyle.Fill, CheckOnClick = true, IntegralHeight = false };
                foreach (var preset in presets)
                    list.Items.Add(preset.Name, selected.Contains(preset.Id));

                var buttons = new FlowLayoutPanel { Dock = DockStyle.Bottom, Height = 44, Padding = new Padding(6), FlowDirection = FlowDirection.RightToLeft };
                var ok = new Button { Text = "OK", DialogResult = DialogResult.OK };
                var cancel = new Button { Text = "Cancel", DialogResult = DialogResult.Cancel };
                buttons.Controls.Add(ok);
                buttons.Controls.Add(cancel);

                dialog.AcceptButton = ok;
                dialog.CancelButton = cancel;
                dialog.Controls.Add(list);
                dialog.Controls.Add(buttons);

                if (dialog.ShowDialog(this) != DialogResult.OK)
                    return;

                var ids = new List<string>();
                for (int i = 0; i < presets.Count; i++)
                    if (list.GetItemChecked(i))
                        ids.Add(presets[i].Id);

                _store.SetMultiCountIds(ids);
                Reload();
            }
        }

        private void OnWeightChanged(object sender, UnitsNet.Mass e) => UpdateCounts();
        private void OnConnectedChanged(object sender, bool e) => UpdateCounts();

        private void UpdateCounts()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateCounts));
                return;
            }

            bool connected = _scale.IsConnected;
            foreach (var tile in _tiles)
                tile.Update(connected, _counter, _scale.Weight);
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                _scale.WeightChanged -= OnWeightChanged;
                _scale.IsConnectedChanged -= OnConnectedChanged;
            }
            base.Dispose(disposing);
        }

        /// <summary>A single colored tile: preset name, big count, and "count / target · diff".</summary>
        private sealed class Tile
        {
            private readonly WeighedItemPreset _preset;
            private readonly Label _name;
            private readonly Label _count;
            private readonly Label _footer;

            public Panel Panel { get; }

            public Tile(WeighedItemPreset preset)
            {
                _preset = preset;

                Panel = new Panel { Dock = DockStyle.Fill, Margin = new Padding(2), BorderStyle = BorderStyle.FixedSingle };
                _count = new Label { Dock = DockStyle.Fill, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Microsoft Sans Serif", 28f, FontStyle.Bold), Text = "-" };
                _name = new Label { Dock = DockStyle.Top, Height = 22, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Microsoft Sans Serif", 9f, FontStyle.Bold), Text = preset.Name, AutoEllipsis = true };
                _footer = new Label { Dock = DockStyle.Bottom, Height = 22, TextAlign = ContentAlignment.MiddleCenter, Font = new Font("Microsoft Sans Serif", 9f), Text = "-" };

                Panel.Controls.Add(_count);
                Panel.Controls.Add(_name);
                Panel.Controls.Add(_footer);
            }

            public void Update(bool connected, ItemCounter counter, UnitsNet.Mass weight)
            {
                _name.Text = _preset.Name;

                if (!connected)
                {
                    _count.Text = "-";
                    _footer.Text = "-";
                    Apply(SystemColors.Control, SystemColors.ControlText);
                    return;
                }

                counter.Apply(_preset);
                var result = counter.Count(weight);

                _count.Text = result.State == CountState.Uncalibrated
                    ? "-"
                    : result.Count.ToString(CultureInfo.InvariantCulture);
                _footer.Text = result.State == CountState.Uncalibrated
                    ? "-"
                    : $"{result.Count} / {result.Expected}   {result.Diff}";

                Color back;
                Color fore = Color.White;
                switch (result.State)
                {
                    case CountState.Exact: back = Color.Green; break;
                    case CountState.NotEnough: back = Color.Orange; break;
                    case CountState.TooMany: back = Color.Red; break;
                    default: back = SystemColors.Control; fore = SystemColors.ControlText; break;
                }
                Apply(back, fore);
            }

            private void Apply(Color back, Color fore)
            {
                Panel.BackColor = back;
                _name.ForeColor = fore;
                _count.ForeColor = fore;
                _footer.ForeColor = fore;
            }
        }
    }
}
