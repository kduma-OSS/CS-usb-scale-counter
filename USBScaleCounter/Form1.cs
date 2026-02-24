using ScaleLib;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using UnitsNet;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.ProgressBar;

namespace USBScaleCounter
{
    public partial class MainForm : Form
    {
        private readonly Scale _scale = new Scale();

        public int ExpectedQuantity { get; set; } = 25;
        public double ExpectedWeight { get; set; } = 580;


        public MainForm()
        {
            InitializeComponent();

            _scale.IsConnectedChanged += IsConnectedChanged;
            _scale.WeightChanged += WeightChanged;

            UpdateInterface();
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
            var expectedMass = Mass.FromGrams(ExpectedWeight);
            var singleMass = expectedMass / ExpectedQuantity;
            var delta = Mass.FromGrams(5);
            var count = (int)((_scale.Weight + delta) / singleMass);
            quantityValue.Text = count.ToString();
            progressText.Text = $"{count} / {ExpectedQuantity}";

            if (count == ExpectedQuantity)
            {
                statusPanel.BackColor = Color.Green;
                statusPanel.ForeColor = Color.White;
                statusText.Text = $"There is {count} items on scale!";
                diffText.Text = "OK";
            }
            else if(count == 0)
            {
                statusPanel.BackColor = SystemColors.Control;
                statusPanel.ForeColor = SystemColors.ControlText;
                statusText.Text = $"Place something on scale!";
                diffText.Text = "Empty";
            }
            else if(count < ExpectedQuantity)
            {
                statusPanel.BackColor = Color.Orange;
                statusPanel.ForeColor = Color.White;
                statusText.Text = $"Not Enough!";
                diffText.Text = $"+ {ExpectedQuantity-count}";
            }
            else
            {
                statusPanel.BackColor = Color.Red;
                statusPanel.ForeColor = Color.White;
                statusText.Text = $"Too Much!";
                diffText.Text = $"- {count - ExpectedQuantity}";

            }
        }

        private void UpdateInterface()
        {
            if (InvokeRequired)
            {
                Invoke(new Action(UpdateInterface));
                return;
            }

            connectionStatusValue.Text = _scale.IsConnected ? "Connected" : "Disconnected";
            scaleStatusValue.Text = _scale.Status.ToString();

            expectedQuantityValue.Text = ExpectedQuantity.ToString(CultureInfo.InvariantCulture);
            expectedWeightValue.Text = ExpectedWeight.ToString(CultureInfo.InvariantCulture);


            if (_scale.IsConnected)
            {
                weightText.Text = weightValue.Text = _scale.Weight.Grams.ToString(CultureInfo.InvariantCulture)+" g";
                DoCalculations();
            }
            else
            {
                statusPanel.BackColor = SystemColors.Control;
                statusPanel.ForeColor = SystemColors.ControlText;
                statusText.Text = $"Connect the scale!";
                diffText.Text = "-";
                progressText.Text = "- / -";
                quantityValue.Text = "-";
                weightText.Text = weightValue.Text = "- g";
            }
        }

        private void button1_Click(object sender, EventArgs e)
        {
            var scale = _scale;

        }

        private void MainForm_Load(object sender, EventArgs e)
        {

        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            _scale.WeightChanged -= WeightChanged;
            _scale.IsConnectedChanged -= IsConnectedChanged;
        }

        private void configureToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var configForm = new ConfigForm();
            configForm.ExpectedQuantity = ExpectedQuantity;
            configForm.ExpectedWeight = ExpectedWeight;

            var result = configForm.ShowDialog(this);

            if (result == DialogResult.Cancel) return;

            ExpectedQuantity = configForm.ExpectedQuantity;
            ExpectedWeight = configForm.ExpectedWeight;

            UpdateInterface();
        }
    }
}
