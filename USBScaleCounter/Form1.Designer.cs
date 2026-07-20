namespace USBScaleCounter
{
    partial class MainForm
    {
        /// <summary>
        /// Required designer variable.
        /// </summary>
        private System.ComponentModel.IContainer components = null;

        /// <summary>
        /// Clean up any resources being used.
        /// </summary>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private void InitializeComponent()
        {
            this.mainToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectionStatusValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.scaleStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.scaleStatusValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.topToolStrip = new System.Windows.Forms.ToolStrip();
            this.presetLabel = new System.Windows.Forms.ToolStripLabel();
            this.presetComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.sep1 = new System.Windows.Forms.ToolStripSeparator();
            this.targetLabel = new System.Windows.Forms.ToolStripLabel();
            this.targetValue = new System.Windows.Forms.ToolStripLabel();
            this.sep2 = new System.Windows.Forms.ToolStripSeparator();
            this.configureButton = new System.Windows.Forms.ToolStripButton();
            this.multiButton = new System.Windows.Forms.ToolStripButton();
            this.soundButton = new System.Windows.Forms.ToolStripButton();
            this.statusPanel = new System.Windows.Forms.Panel();
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.progressText = new System.Windows.Forms.Label();
            this.statusText = new System.Windows.Forms.Label();
            this.weightText = new System.Windows.Forms.Label();
            this.diffText = new System.Windows.Forms.Label();
            this.mainToolStripContainer.BottomToolStripPanel.SuspendLayout();
            this.mainToolStripContainer.ContentPanel.SuspendLayout();
            this.mainToolStripContainer.TopToolStripPanel.SuspendLayout();
            this.mainToolStripContainer.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.topToolStrip.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            //
            // mainToolStripContainer
            //
            this.mainToolStripContainer.BottomToolStripPanel.Controls.Add(this.mainStatusStrip);
            this.mainToolStripContainer.ContentPanel.Controls.Add(this.statusPanel);
            this.mainToolStripContainer.ContentPanel.Size = new System.Drawing.Size(1017, 597);
            this.mainToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.mainToolStripContainer.Name = "mainToolStripContainer";
            this.mainToolStripContainer.Size = new System.Drawing.Size(1017, 668);
            this.mainToolStripContainer.TabIndex = 1;
            this.mainToolStripContainer.TopToolStripPanel.Controls.Add(this.topToolStrip);
            //
            // mainStatusStrip
            //
            this.mainStatusStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.connectionStatusLabel,
            this.connectionStatusValue,
            this.scaleStatusLabel,
            this.scaleStatusValue});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 0);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(1017, 22);
            this.mainStatusStrip.TabIndex = 0;
            //
            // connectionStatusLabel
            //
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Text = "Connection:";
            //
            // connectionStatusValue
            //
            this.connectionStatusValue.Name = "connectionStatusValue";
            this.connectionStatusValue.Text = "-";
            //
            // scaleStatusLabel
            //
            this.scaleStatusLabel.Name = "scaleStatusLabel";
            this.scaleStatusLabel.Text = "Status:";
            //
            // scaleStatusValue
            //
            this.scaleStatusValue.Name = "scaleStatusValue";
            this.scaleStatusValue.Text = "-";
            //
            // topToolStrip
            //
            this.topToolStrip.Dock = System.Windows.Forms.DockStyle.None;
            this.topToolStrip.GripStyle = System.Windows.Forms.ToolStripGripStyle.Hidden;
            this.topToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.presetLabel,
            this.presetComboBox,
            this.sep1,
            this.targetLabel,
            this.targetValue,
            this.sep2,
            this.configureButton,
            this.multiButton,
            this.soundButton});
            this.topToolStrip.Location = new System.Drawing.Point(0, 0);
            this.topToolStrip.Name = "topToolStrip";
            this.topToolStrip.Size = new System.Drawing.Size(1017, 25);
            this.topToolStrip.TabIndex = 0;
            //
            // presetLabel
            //
            this.presetLabel.Name = "presetLabel";
            this.presetLabel.Text = "Preset:";
            //
            // presetComboBox
            //
            this.presetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.presetComboBox.Name = "presetComboBox";
            this.presetComboBox.Size = new System.Drawing.Size(220, 25);
            //
            // sep1
            //
            this.sep1.Name = "sep1";
            //
            // targetLabel
            //
            this.targetLabel.Name = "targetLabel";
            this.targetLabel.Text = "Target:";
            //
            // targetValue
            //
            this.targetValue.Name = "targetValue";
            this.targetValue.Text = "-";
            //
            // sep2
            //
            this.sep2.Name = "sep2";
            //
            // configureButton
            //
            this.configureButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.configureButton.Name = "configureButton";
            this.configureButton.Text = "Configure…";
            //
            // multiButton
            //
            this.multiButton.CheckOnClick = true;
            this.multiButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.multiButton.Name = "multiButton";
            this.multiButton.Text = "Multi";
            //
            // soundButton
            //
            this.soundButton.CheckOnClick = true;
            this.soundButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Text;
            this.soundButton.Name = "soundButton";
            this.soundButton.Text = "Sound";
            //
            // statusPanel
            //
            this.statusPanel.Controls.Add(this.tableLayoutPanel1);
            this.statusPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusPanel.Location = new System.Drawing.Point(0, 0);
            this.statusPanel.Name = "statusPanel";
            this.statusPanel.Size = new System.Drawing.Size(1017, 597);
            this.statusPanel.TabIndex = 0;
            //
            // tableLayoutPanel1
            //
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Controls.Add(this.diffText, 1, 1);
            this.tableLayoutPanel1.Controls.Add(this.statusText, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.progressText, 2, 2);
            this.tableLayoutPanel1.Controls.Add(this.weightText, 0, 2);
            this.tableLayoutPanel1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tableLayoutPanel1.Location = new System.Drawing.Point(0, 0);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 3;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 25F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(1017, 597);
            this.tableLayoutPanel1.TabIndex = 0;
            //
            // progressText
            //
            this.progressText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.progressText.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.progressText.Name = "progressText";
            this.progressText.TabIndex = 0;
            this.progressText.Text = "- / -";
            this.progressText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // statusText
            //
            this.tableLayoutPanel1.SetColumnSpan(this.statusText, 3);
            this.statusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.Name = "statusText";
            this.statusText.TabIndex = 0;
            this.statusText.Text = "Connect the scale!";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // weightText
            //
            this.weightText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightText.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weightText.Name = "weightText";
            this.weightText.TabIndex = 1;
            this.weightText.Text = "- g";
            this.weightText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // diffText
            //
            this.diffText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffText.Font = new System.Drawing.Font("Microsoft Sans Serif", 90F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diffText.Name = "diffText";
            this.diffText.TabIndex = 2;
            this.diffText.Text = "-";
            this.diffText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            //
            // MainForm
            //
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 668);
            this.Controls.Add(this.mainToolStripContainer);
            this.Name = "MainForm";
            this.Text = "USB Scale Counter";
            this.FormClosing += new System.Windows.Forms.FormClosingEventHandler(this.MainForm_FormClosing);
            this.Load += new System.EventHandler(this.MainForm_Load);
            this.mainToolStripContainer.BottomToolStripPanel.ResumeLayout(false);
            this.mainToolStripContainer.BottomToolStripPanel.PerformLayout();
            this.mainToolStripContainer.ContentPanel.ResumeLayout(false);
            this.mainToolStripContainer.TopToolStripPanel.ResumeLayout(false);
            this.mainToolStripContainer.TopToolStripPanel.PerformLayout();
            this.mainToolStripContainer.ResumeLayout(false);
            this.mainToolStripContainer.PerformLayout();
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.topToolStrip.ResumeLayout(false);
            this.topToolStrip.PerformLayout();
            this.statusPanel.ResumeLayout(false);
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        #endregion

        private System.Windows.Forms.ToolStripContainer mainToolStripContainer;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel connectionStatusValue;
        private System.Windows.Forms.ToolStripStatusLabel scaleStatusLabel;
        private System.Windows.Forms.ToolStripStatusLabel scaleStatusValue;
        private System.Windows.Forms.ToolStrip topToolStrip;
        private System.Windows.Forms.ToolStripLabel presetLabel;
        private System.Windows.Forms.ToolStripComboBox presetComboBox;
        private System.Windows.Forms.ToolStripSeparator sep1;
        private System.Windows.Forms.ToolStripLabel targetLabel;
        private System.Windows.Forms.ToolStripLabel targetValue;
        private System.Windows.Forms.ToolStripSeparator sep2;
        private System.Windows.Forms.ToolStripButton configureButton;
        private System.Windows.Forms.ToolStripButton multiButton;
        private System.Windows.Forms.ToolStripButton soundButton;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label progressText;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.Label weightText;
        private System.Windows.Forms.Label diffText;
    }
}
