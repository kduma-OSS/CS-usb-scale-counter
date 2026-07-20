using ScaleLib;
using ScaleCounter.Core;
using System;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Windows.Forms;
using UnitsNet;

namespace USBScaleCounter
{
    public partial class MainForm : Form
    {
        private readonly Scale _scale = new Scale();
        private readonly ItemCounter _counter = new ItemCounter();
        private readonly PresetStore _presets = new PresetStore();

        private MultiCountControl _multiControl;
        private SplitContainer _split;
        private Form _popoutForm;
        private bool _poppedOut;
        private bool _splitInitialized;
        private bool _suppressComboEvent;
        private bool _isStable;
        private CountState? _previousStableState;
        private readonly System.Windows.Forms.Timer _settleTimer = new System.Windows.Forms.Timer { Interval = 500 };

        public MainForm()
        {
            InitializeComponent();

            BuildMultiPanel();

            presetComboBox.SelectedIndexChanged += PresetComboBox_SelectedIndexChanged;
            configureButton.Click += ConfigureButton_Click;
            multiButton.CheckedChanged += MultiButton_CheckedChanged;

            soundButton.Checked = _presets.GetSoundEnabled();
            soundButton.CheckedChanged += (s, e) => _presets.SetSoundEnabled(soundButton.Checked);

            _settleTimer.Tick += SettleTimer_Tick;

            RefreshPresetCombo();
            ApplyActivePreset();

            _scale.IsConnectedChanged += IsConnectedChanged;
            _scale.WeightChanged += WeightChanged;

            UpdateInterface();
        }

        private void BuildMultiPanel()
        {
            _multiControl = new MultiCountControl(_presets, _scale) { Dock = DockStyle.Fill };
            _multiControl.PopOutClicked += MultiControl_PopOutClicked;

            // Host the dashboard (left) and the multi-count panel (right) in a resizable split,
            // so the multi panel never overlaps the main content.
            mainToolStripContainer.ContentPanel.Controls.Remove(statusPanel);
            _split = new SplitContainer
            {
                Dock = DockStyle.Fill,
                Orientation = Orientation.Vertical,
                Panel2Collapsed = true,
                FixedPanel = FixedPanel.Panel2
            };
            statusPanel.Dock = DockStyle.Fill;
            _split.Panel1.Controls.Add(statusPanel);
            _split.Panel2.Controls.Add(_multiControl);
            mainToolStripContainer.ContentPanel.Controls.Add(_split);
        }

        // Positions the splitter so the multi panel is ~360px wide. Must be called before
        // un-collapsing Panel2; clamps to the valid range and never throws.
        private void EnsureSplitterDistance()
        {
            if (_splitInitialized) return;
            try
            {
                int min = _split.Panel1MinSize;
                int max = _split.Width - _split.Panel2MinSize - _split.SplitterWidth;
                if (max <= min) return; // container too small; keep default
                int desired = _split.Width - 360;
                _split.SplitterDistance = Math.Min(Math.Max(desired, min), max);
                _splitInitialized = true;
            }
            catch
            {
                // Not sized yet; the default splitter position is used.
            }
        }

        private void ShowMultiPanel()
        {
            EnsureSplitterDistance();   // set a valid distance BEFORE un-collapsing
            _split.Panel2Collapsed = false;
        }

        private void ApplyActivePreset()
        {
            var preset = _presets.Active;
            if (preset != null) _counter.Apply(preset);
        }

        private void RefreshPresetCombo()
        {
            _suppressComboEvent = true;
            presetComboBox.Items.Clear();
            foreach (var p in _presets.Presets)
                presetComboBox.Items.Add(p.Name);

            var active = _presets.Active;
            if (active != null)
            {
                var index = _presets.Presets.ToList().FindIndex(p => p.Id == active.Id);
                if (index >= 0) presetComboBox.SelectedIndex = index;
            }
            _suppressComboEvent = false;
        }

        private void PresetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (_suppressComboEvent) return;

            var index = presetComboBox.SelectedIndex;
            var presets = _presets.Presets;
            if (index >= 0 && index < presets.Count)
            {
                _presets.SetActive(presets[index].Id);
                ApplyActivePreset();
                UpdateInterface();
            }
        }

        private void ConfigureButton_Click(object sender, EventArgs e)
        {
            using (var form = new PresetsForm(_presets, _scale))
            {
                form.ShowDialog(this);
            }

            RefreshPresetCombo();
            ApplyActivePreset();
            _multiControl.Reload();
            UpdateInterface();
        }

        private void MultiButton_CheckedChanged(object sender, EventArgs e)
        {
            if (multiButton.Checked)
            {
                if (!_poppedOut)
                    ShowMultiPanel();
                _multiControl.Reload();
            }
            else
            {
                if (_poppedOut && _popoutForm != null)
                    _popoutForm.Close();
                else
                    _split.Panel2Collapsed = true;
            }
        }

        private void MultiControl_PopOutClicked(object sender, EventArgs e)
        {
            if (_poppedOut)
            {
                if (_popoutForm != null) _popoutForm.Close(); // FormClosed re-docks
            }
            else
            {
                PopOutMulti();
            }
        }

        private void PopOutMulti()
        {
            _popoutForm = new Form
            {
                Text = "Multi-count",
                ClientSize = new Size(440, 380),
                StartPosition = FormStartPosition.CenterParent,
                ShowIcon = false,
                MinimizeBox = false
            };

            _split.Panel2.Controls.Remove(_multiControl);
            _multiControl.Dock = DockStyle.Fill;
            _popoutForm.Controls.Add(_multiControl);
            _multiControl.SetPoppedOut(true);
            _poppedOut = true;
            _split.Panel2Collapsed = true;

            _popoutForm.FormClosed += PopoutForm_FormClosed;
            _popoutForm.Show(this);
        }

        private void PopoutForm_FormClosed(object sender, FormClosedEventArgs e)
        {
            if (!_poppedOut) return;

            if (_popoutForm != null) _popoutForm.Controls.Remove(_multiControl);
            _multiControl.Dock = DockStyle.Fill;
            _split.Panel2.Controls.Add(_multiControl);
            _multiControl.SetPoppedOut(false);
            _poppedOut = false;
            _popoutForm = null;
            if (multiButton.Checked)
                ShowMultiPanel();
            else
                _split.Panel2Collapsed = true;
        }

        private void IsConnectedChanged(object sender, bool e)
        {
            UpdateInterface();
        }

        private void WeightChanged(object sender, Mass weight)
        {
            UpdateInterface();
        }

        private void DoCalculations()
        {
            var result = _counter.Count(_scale.Weight);

            progressText.Text = result.State == CountState.Uncalibrated
                ? "- / -"
                : $"{result.Count} / {result.Expected}";

            switch (result.State)
            {
                case CountState.Exact:
                    statusPanel.BackColor = Color.Green;
                    statusPanel.ForeColor = Color.White;
                    break;
                case CountState.NotEnough:
                    statusPanel.BackColor = Color.Orange;
                    statusPanel.ForeColor = Color.White;
                    break;
                case CountState.TooMany:
                    statusPanel.BackColor = Color.Red;
                    statusPanel.ForeColor = Color.White;
                    break;
                default: // Empty / Uncalibrated
                    statusPanel.BackColor = SystemColors.Control;
                    statusPanel.ForeColor = SystemColors.ControlText;
                    break;
            }

            statusText.Text = result.Message;
            diffText.Text = result.Diff;
        }

        private void UpdateInterface()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateInterface));
                return;
            }

            connectionStatusValue.Text = _scale.IsConnected ? "Connected" : "Disconnected";

            var preset = _presets.Active;
            targetValue.Text = preset != null
                ? preset.TargetQuantity.ToString(CultureInfo.InvariantCulture)
                : "-";

            if (_scale.IsConnected)
            {
                weightText.Text = _scale.Weight.Grams.ToString(CultureInfo.InvariantCulture) + " g";
                DoCalculations();

                // The scale's "Stable" flag is unreliable (always Stable), so debounce stability
                // ourselves: restart a settle timer on every (value-debounced) reading.
                _isStable = false;
                _settleTimer.Stop();
                _settleTimer.Start();
            }
            else
            {
                statusPanel.BackColor = SystemColors.Control;
                statusPanel.ForeColor = SystemColors.ControlText;
                statusText.Text = "Connect the scale!";
                diffText.Text = "-";
                progressText.Text = "- / -";
                weightText.Text = "- g";
                _settleTimer.Stop();
                _isStable = false;
                _previousStableState = null;
            }

            UpdateStabilityLabel();
        }

        private void UpdateStabilityLabel()
        {
            scaleStatusValue.Text = !_scale.IsConnected ? "-" : (_isStable ? "Stable" : "Measuring…");
        }

        // Fires once the weight has been quiet for the settle interval — the reading is "stable".
        private void SettleTimer_Tick(object sender, EventArgs e)
        {
            _settleTimer.Stop();
            _isStable = true;
            UpdateStabilityLabel();

            if (!_scale.IsConnected)
                return;

            var result = _counter.Count(_scale.Weight);
            if (soundButton.Checked && _previousStableState.HasValue)
            {
                var signal = CountSignals.ForTransition(_previousStableState.Value, result.State);
                if (signal != CountSignal.None)
                    SignalSounds.Play(signal);
            }
            _previousStableState = result.State;
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            var version = Assembly.GetExecutingAssembly().GetName().Version;
            Text += $" ({version})";

            // Use the embedded application icon for the title bar / taskbar too.
            try { this.Icon = System.Drawing.Icon.ExtractAssociatedIcon(Application.ExecutablePath); }
            catch { /* fall back to the default form icon */ }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scale.WeightChanged -= WeightChanged;
            _scale.IsConnectedChanged -= IsConnectedChanged;
        }
    }
}
