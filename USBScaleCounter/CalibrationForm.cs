using ScaleLib;
using ScaleCounter.Core;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Globalization;
using System.Windows.Forms;

namespace USBScaleCounter
{
    /// <summary>
    /// Edits and (re-)calibrates a preset: capture several (quantity, weight) points, fit the
    /// per-item weight + tare live via <see cref="Calibration.Fit"/>, then save. A point with
    /// quantity 0 measures the tare (empty container).
    /// </summary>
    internal sealed class CalibrationForm : Form
    {
        private readonly WeighedItemPreset _preset;
        private readonly Scale _scale;
        private readonly List<CalibrationSample> _samples;

        private readonly TextBox _name = new TextBox();
        private readonly TextBox _target = new TextBox();
        private readonly TextBox _quantity = new TextBox();
        private readonly Label _currentWeight = new Label();
        private readonly DataGridView _grid = new DataGridView();
        private readonly Label _result = new Label();
        private readonly Button _saveButton = new Button();

        public CalibrationForm(WeighedItemPreset preset, Scale scale)
        {
            _preset = preset;
            _scale = scale;
            _samples = new List<CalibrationSample>(preset.Samples);

            BuildUi();

            _name.Text = preset.Name;
            _target.Text = preset.TargetQuantity.ToString(CultureInfo.InvariantCulture);
            RefreshGrid();
            Recompute();
            UpdateCurrentWeight();

            _scale.WeightChanged += OnWeightChanged;
            FormClosed += (s, e) => _scale.WeightChanged -= OnWeightChanged;
        }

        private void BuildUi()
        {
            Text = "Calibrate preset";
            ClientSize = new Size(520, 560);
            MinimumSize = new Size(470, 520);
            StartPosition = FormStartPosition.CenterParent;
            MinimizeBox = false;
            ShowIcon = false;

            var root = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 1, Padding = new Padding(10) };
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // preset
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // add measurement
            root.RowStyles.Add(new RowStyle(SizeType.Percent, 100));   // measurements grid
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // result
            root.RowStyles.Add(new RowStyle(SizeType.AutoSize));       // buttons

            root.Controls.Add(BuildPresetGroup(), 0, 0);
            root.Controls.Add(BuildAddGroup(), 0, 1);
            root.Controls.Add(BuildMeasurementsGroup(), 0, 2);

            _result.AutoSize = true;
            _result.Font = new Font(Font, FontStyle.Bold);
            _result.Margin = new Padding(3, 8, 3, 8);
            root.Controls.Add(_result, 0, 3);

            var buttons = new FlowLayoutPanel { AutoSize = true, Dock = DockStyle.Fill, FlowDirection = FlowDirection.RightToLeft };
            _saveButton.Text = "Save";
            _saveButton.Click += (s, e) => Save();
            var cancel = new Button { Text = "Cancel" };
            cancel.Click += (s, e) => { DialogResult = DialogResult.Cancel; Close(); };
            buttons.Controls.Add(_saveButton);
            buttons.Controls.Add(cancel);
            root.Controls.Add(buttons, 0, 4);

            Controls.Add(root);
            AcceptButton = _saveButton;
        }

        private Control BuildPresetGroup()
        {
            var group = new GroupBox { Text = "Preset", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(8) };
            var layout = TwoColumnLayout();

            _name.Dock = DockStyle.Fill;
            _target.Dock = DockStyle.Fill;
            layout.Controls.Add(FieldLabel("Name:"), 0, 0);
            layout.Controls.Add(_name, 1, 0);
            layout.Controls.Add(FieldLabel("Target quantity:"), 0, 1);
            layout.Controls.Add(_target, 1, 1);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildAddGroup()
        {
            var group = new GroupBox { Text = "Add measurement", Dock = DockStyle.Fill, AutoSize = true, Padding = new Padding(8) };
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 3, AutoSize = true };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.AutoSize));

            _currentWeight.AutoSize = true;
            _currentWeight.Font = new Font(Font.FontFamily, 14f, FontStyle.Bold);
            _currentWeight.Text = "- g";

            _quantity.Dock = DockStyle.Fill;
            var capture = new Button { Text = "Capture", AutoSize = true };
            capture.Click += (s, e) => Capture();

            layout.Controls.Add(FieldLabel("Current weight:"), 0, 0);
            layout.Controls.Add(_currentWeight, 1, 0);
            layout.SetColumnSpan(_currentWeight, 2);
            layout.Controls.Add(FieldLabel("Quantity:"), 0, 1);
            layout.Controls.Add(_quantity, 1, 1);
            layout.Controls.Add(capture, 2, 1);
            var hint = new Label
            {
                Text = "Tip: quantity 0 = empty container (measures the tare).",
                AutoSize = true,
                ForeColor = SystemColors.GrayText,
                Margin = new Padding(3)
            };
            layout.Controls.Add(hint, 1, 2);
            layout.SetColumnSpan(hint, 2);

            group.Controls.Add(layout);
            return group;
        }

        private Control BuildMeasurementsGroup()
        {
            var group = new GroupBox { Text = "Measurements", Dock = DockStyle.Fill, Padding = new Padding(8) };

            _grid.Dock = DockStyle.Fill;
            _grid.AllowUserToAddRows = false;
            _grid.AllowUserToDeleteRows = false;
            _grid.AllowUserToResizeRows = false;
            _grid.RowHeadersVisible = false;
            _grid.ReadOnly = true;
            _grid.MultiSelect = false;
            _grid.SelectionMode = DataGridViewSelectionMode.FullRowSelect;
            _grid.AutoGenerateColumns = false;
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Quantity", Name = "Quantity", Width = 110 });
            _grid.Columns.Add(new DataGridViewTextBoxColumn { HeaderText = "Weight (g)", Name = "Weight", AutoSizeMode = DataGridViewAutoSizeColumnMode.Fill });

            var bottom = new FlowLayoutPanel { Dock = DockStyle.Bottom, AutoSize = true, FlowDirection = FlowDirection.LeftToRight };
            var remove = new Button { Text = "Remove selected", AutoSize = true };
            remove.Click += (s, e) => RemoveSelected();
            bottom.Controls.Add(remove);

            group.Controls.Add(_grid);
            group.Controls.Add(bottom);
            return group;
        }

        private static TableLayoutPanel TwoColumnLayout()
        {
            var layout = new TableLayoutPanel { Dock = DockStyle.Fill, ColumnCount = 2, AutoSize = true };
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Absolute, 130));
            layout.ColumnStyles.Add(new ColumnStyle(SizeType.Percent, 100));
            return layout;
        }

        private static Label FieldLabel(string text) =>
            new Label { Text = text, Anchor = AnchorStyles.Left, AutoSize = true, Margin = new Padding(3, 6, 3, 3) };

        private void OnWeightChanged(object sender, UnitsNet.Mass e) => UpdateCurrentWeight();

        private void UpdateCurrentWeight()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateCurrentWeight));
                return;
            }

            _currentWeight.Text = _scale.IsConnected
                ? _scale.Weight.Grams.ToString("0.##", CultureInfo.InvariantCulture) + " g"
                : "- g";
        }

        private void Capture()
        {
            if (!int.TryParse(_quantity.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var quantity) || quantity < 0)
            {
                MessageBox.Show(this, "Enter a valid quantity (0 or more).", "Calibration");
                return;
            }

            _samples.Add(new CalibrationSample(quantity, _scale.Weight.Grams));
            _quantity.Text = "";
            RefreshGrid();
            Recompute();
        }

        private void RemoveSelected()
        {
            var row = _grid.CurrentRow;
            if (row == null) return;

            int index = row.Index;
            if (index >= 0 && index < _samples.Count)
            {
                _samples.RemoveAt(index);
                RefreshGrid();
                Recompute();
            }
        }

        private void RefreshGrid()
        {
            _grid.Rows.Clear();
            foreach (var sample in _samples)
                _grid.Rows.Add(
                    sample.Quantity.ToString(CultureInfo.InvariantCulture),
                    sample.WeightGrams.ToString("0.##", CultureInfo.InvariantCulture));
        }

        private void Recompute()
        {
            var result = Calibration.Fit(_samples);
            if (result.IsValid)
            {
                _result.Text =
                    $"Per item: {result.PerItemWeightGrams.ToString("0.##", CultureInfo.InvariantCulture)} g" +
                    $"   •   Tare: {result.TareGrams.ToString("0.##", CultureInfo.InvariantCulture)} g" +
                    $"   •   R²: {result.RSquared.ToString("0.###", CultureInfo.InvariantCulture)}";
                _saveButton.Enabled = true;
            }
            else
            {
                _result.Text = result.Error ?? "Add measurements to calibrate.";
                _saveButton.Enabled = false;
            }
        }

        private void Save()
        {
            var result = Calibration.Fit(_samples);
            if (!result.IsValid)
            {
                MessageBox.Show(this, result.Error ?? "Cannot calibrate from these measurements.", "Calibration");
                return;
            }

            _preset.Name = string.IsNullOrWhiteSpace(_name.Text) ? "Preset" : _name.Text.Trim();
            _preset.PerItemWeightGrams = result.PerItemWeightGrams;
            _preset.TareGrams = result.TareGrams;
            _preset.TargetQuantity =
                int.TryParse(_target.Text, NumberStyles.Integer, CultureInfo.InvariantCulture, out var target) && target >= 1
                    ? target
                    : 1;
            _preset.Samples = new List<CalibrationSample>(_samples);

            DialogResult = DialogResult.OK;
            Close();
        }
    }
}
