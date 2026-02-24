using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace USBScaleCounter
{
    public partial class ConfigForm : Form
    {
        public int ExpectedQuantity
        {
            get => int.TryParse(expectedQuantityTextBox.Text, out var value) ? value : 0;
            set => expectedQuantityTextBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        public double ExpectedWeight
        {
            get => double.TryParse(expectedWeightTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var value) ? value : 0;
            set => expectedWeightTextBox.Text = value.ToString(CultureInfo.InvariantCulture);
        }

        public ConfigForm()
        {
            InitializeComponent();
        }

        private bool ValidateForm()
        {
            var isValid = true;

            if (!int.TryParse(expectedQuantityTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var expectedCount))
            {
                errorProvider1.SetError(expectedQuantityTextBox, "Please enter a valid integer value.");
                if (isValid) expectedQuantityTextBox.Focus();
                isValid = false;
            }
            else if (expectedCount < 1)
            {
                errorProvider1.SetError(expectedQuantityTextBox, "Please enter a value greater than 0.");
                if (isValid) expectedQuantityTextBox.Focus();
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(expectedQuantityTextBox, null);
            }

            if (!double.TryParse(expectedWeightTextBox.Text, NumberStyles.Float, CultureInfo.InvariantCulture, out var expectedWeight))
            {
                errorProvider1.SetError(expectedWeightTextBox, "Please enter a valid double value.");
                if (isValid) expectedWeightTextBox.Focus();
                isValid = false;
            }
            else if (expectedWeight <= 0)
            {
                errorProvider1.SetError(expectedWeightTextBox, "Please enter a value greater than 0.");
                if (isValid) expectedWeightTextBox.Focus();
                isValid = false;
            }
            else
            {
                errorProvider1.SetError(expectedWeightTextBox, null);
            }

            return isValid;
        }

        private void TextBoxValidating(object sender, CancelEventArgs e)
        {
            ValidateForm();
        }

        private void ConfigForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            // Only validate on OK, not on Cancel or X button
            if (DialogResult != DialogResult.OK) return;

            if (ValidateForm()) return;

            e.Cancel = true;

            // Reset DialogResult so the form stays interactive
            DialogResult = DialogResult.None;
        }
    }
}
