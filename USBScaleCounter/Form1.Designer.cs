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
        /// <param name="disposing">true if managed resources should be disposed; otherwise, false.</param>
        protected override void Dispose(bool disposing)
        {
            if (disposing && (components != null))
            {
                components.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        /// <summary>
        /// Required method for Designer support - do not modify
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.mainToolStripContainer = new System.Windows.Forms.ToolStripContainer();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.connectionStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.connectionStatusValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.scaleStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.scaleStatusValue = new System.Windows.Forms.ToolStripStatusLabel();
            this.menuStrip1 = new System.Windows.Forms.MenuStrip();
            this.configureToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStrip1 = new System.Windows.Forms.ToolStrip();
            this.expectedWeightLabel = new System.Windows.Forms.ToolStripLabel();
            this.expectedWeightValue = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.expectedQuantityLabel = new System.Windows.Forms.ToolStripLabel();
            this.expectedQuantityValue = new System.Windows.Forms.ToolStripLabel();
            this.toolStrip2 = new System.Windows.Forms.ToolStrip();
            this.weightLabel = new System.Windows.Forms.ToolStripLabel();
            this.weightValue = new System.Windows.Forms.ToolStripLabel();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.quantityLabel = new System.Windows.Forms.ToolStripLabel();
            this.quantityValue = new System.Windows.Forms.ToolStripLabel();
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
            this.menuStrip1.SuspendLayout();
            this.toolStrip1.SuspendLayout();
            this.toolStrip2.SuspendLayout();
            this.statusPanel.SuspendLayout();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStripContainer
            // 
            // 
            // mainToolStripContainer.BottomToolStripPanel
            // 
            this.mainToolStripContainer.BottomToolStripPanel.Controls.Add(this.mainStatusStrip);
            // 
            // mainToolStripContainer.ContentPanel
            // 
            this.mainToolStripContainer.ContentPanel.Controls.Add(this.statusPanel);
            this.mainToolStripContainer.ContentPanel.Size = new System.Drawing.Size(1017, 597);
            this.mainToolStripContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainToolStripContainer.Location = new System.Drawing.Point(0, 0);
            this.mainToolStripContainer.Name = "mainToolStripContainer";
            this.mainToolStripContainer.Size = new System.Drawing.Size(1017, 668);
            this.mainToolStripContainer.TabIndex = 1;
            this.mainToolStripContainer.Text = "toolStripContainer1";
            // 
            // mainToolStripContainer.TopToolStripPanel
            // 
            this.mainToolStripContainer.TopToolStripPanel.Controls.Add(this.menuStrip1);
            this.mainToolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip1);
            this.mainToolStripContainer.TopToolStripPanel.Controls.Add(this.toolStrip2);
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
            this.connectionStatusLabel.Size = new System.Drawing.Size(72, 17);
            this.connectionStatusLabel.Text = "Connection:";
            // 
            // connectionStatusValue
            // 
            this.connectionStatusValue.Name = "connectionStatusValue";
            this.connectionStatusValue.Size = new System.Drawing.Size(127, 17);
            this.connectionStatusValue.Text = "connectionStatusValue";
            // 
            // scaleStatusLabel
            // 
            this.scaleStatusLabel.Name = "scaleStatusLabel";
            this.scaleStatusLabel.Size = new System.Drawing.Size(42, 17);
            this.scaleStatusLabel.Text = "Status:";
            // 
            // scaleStatusValue
            // 
            this.scaleStatusValue.Name = "scaleStatusValue";
            this.scaleStatusValue.Size = new System.Drawing.Size(93, 17);
            this.scaleStatusValue.Text = "scaleStatusValue";
            // 
            // menuStrip1
            // 
            this.menuStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.menuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.configureToolStripMenuItem});
            this.menuStrip1.Location = new System.Drawing.Point(0, 0);
            this.menuStrip1.Name = "menuStrip1";
            this.menuStrip1.Size = new System.Drawing.Size(1017, 24);
            this.menuStrip1.TabIndex = 0;
            this.menuStrip1.Text = "menuStrip1";
            // 
            // configureToolStripMenuItem
            // 
            this.configureToolStripMenuItem.Name = "configureToolStripMenuItem";
            this.configureToolStripMenuItem.Size = new System.Drawing.Size(81, 20);
            this.configureToolStripMenuItem.Text = "Configure...";
            this.configureToolStripMenuItem.Click += new System.EventHandler(this.configureToolStripMenuItem_Click);
            // 
            // toolStrip1
            // 
            this.toolStrip1.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.expectedWeightLabel,
            this.expectedWeightValue,
            this.toolStripSeparator1,
            this.expectedQuantityLabel,
            this.expectedQuantityValue});
            this.toolStrip1.Location = new System.Drawing.Point(38, 24);
            this.toolStrip1.Name = "toolStrip1";
            this.toolStrip1.Size = new System.Drawing.Size(266, 25);
            this.toolStrip1.TabIndex = 1;
            // 
            // expectedWeightLabel
            // 
            this.expectedWeightLabel.Name = "expectedWeightLabel";
            this.expectedWeightLabel.Size = new System.Drawing.Size(98, 22);
            this.expectedWeightLabel.Text = "Expected Weight:";
            // 
            // expectedWeightValue
            // 
            this.expectedWeightValue.Name = "expectedWeightValue";
            this.expectedWeightValue.Size = new System.Drawing.Size(25, 22);
            this.expectedWeightValue.Text = "123";
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 25);
            // 
            // expectedQuantityLabel
            // 
            this.expectedQuantityLabel.Name = "expectedQuantityLabel";
            this.expectedQuantityLabel.Size = new System.Drawing.Size(106, 22);
            this.expectedQuantityLabel.Text = "Expected Quantity:";
            // 
            // expectedQuantityValue
            // 
            this.expectedQuantityValue.Name = "expectedQuantityValue";
            this.expectedQuantityValue.Size = new System.Drawing.Size(19, 22);
            this.expectedQuantityValue.Text = "45";
            // 
            // toolStrip2
            // 
            this.toolStrip2.Dock = System.Windows.Forms.DockStyle.None;
            this.toolStrip2.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.weightLabel,
            this.weightValue,
            this.toolStripSeparator2,
            this.quantityLabel,
            this.quantityValue});
            this.toolStrip2.Location = new System.Drawing.Point(468, 24);
            this.toolStrip2.Name = "toolStrip2";
            this.toolStrip2.Size = new System.Drawing.Size(156, 25);
            this.toolStrip2.TabIndex = 2;
            // 
            // weightLabel
            // 
            this.weightLabel.Name = "weightLabel";
            this.weightLabel.Size = new System.Drawing.Size(48, 22);
            this.weightLabel.Text = "Weight:";
            // 
            // weightValue
            // 
            this.weightValue.Name = "weightValue";
            this.weightValue.Size = new System.Drawing.Size(22, 22);
            this.weightValue.Text = "- g";
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 25);
            // 
            // quantityLabel
            // 
            this.quantityLabel.Name = "quantityLabel";
            this.quantityLabel.Size = new System.Drawing.Size(56, 22);
            this.quantityLabel.Text = "Quantity:";
            // 
            // quantityValue
            // 
            this.quantityValue.Name = "quantityValue";
            this.quantityValue.Size = new System.Drawing.Size(12, 22);
            this.quantityValue.Text = "-";
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
            this.progressText.Location = new System.Drawing.Point(765, 447);
            this.progressText.Name = "progressText";
            this.progressText.Size = new System.Drawing.Size(249, 150);
            this.progressText.TabIndex = 0;
            this.progressText.Text = "1/2";
            this.progressText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // statusText
            // 
            this.tableLayoutPanel1.SetColumnSpan(this.statusText, 3);
            this.statusText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.statusText.Font = new System.Drawing.Font("Microsoft Sans Serif", 27.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.statusText.Location = new System.Drawing.Point(3, 0);
            this.statusText.Name = "statusText";
            this.statusText.Size = new System.Drawing.Size(1011, 149);
            this.statusText.TabIndex = 0;
            this.statusText.Text = "label1";
            this.statusText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // weightText
            // 
            this.weightText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.weightText.Font = new System.Drawing.Font("Microsoft Sans Serif", 48F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.weightText.Location = new System.Drawing.Point(3, 447);
            this.weightText.Name = "weightText";
            this.weightText.Size = new System.Drawing.Size(248, 150);
            this.weightText.TabIndex = 1;
            this.weightText.Text = "555 g";
            this.weightText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // diffText
            // 
            this.diffText.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffText.Font = new System.Drawing.Font("Microsoft Sans Serif", 90F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.diffText.Location = new System.Drawing.Point(257, 149);
            this.diffText.Name = "diffText";
            this.diffText.Size = new System.Drawing.Size(502, 298);
            this.diffText.TabIndex = 2;
            this.diffText.Text = "+ 5";
            this.diffText.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // MainForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1017, 668);
            this.Controls.Add(this.mainToolStripContainer);
            this.MainMenuStrip = this.menuStrip1;
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
            this.menuStrip1.ResumeLayout(false);
            this.menuStrip1.PerformLayout();
            this.toolStrip1.ResumeLayout(false);
            this.toolStrip1.PerformLayout();
            this.toolStrip2.ResumeLayout(false);
            this.toolStrip2.PerformLayout();
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
        private System.Windows.Forms.MenuStrip menuStrip1;
        private System.Windows.Forms.ToolStripMenuItem configureToolStripMenuItem;
        private System.Windows.Forms.ToolStrip toolStrip1;
        private System.Windows.Forms.ToolStripLabel expectedWeightLabel;
        private System.Windows.Forms.ToolStripLabel expectedWeightValue;
        private System.Windows.Forms.ToolStripLabel expectedQuantityLabel;
        private System.Windows.Forms.ToolStripLabel expectedQuantityValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStrip toolStrip2;
        private System.Windows.Forms.ToolStripLabel weightLabel;
        private System.Windows.Forms.ToolStripLabel weightValue;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripLabel quantityLabel;
        private System.Windows.Forms.ToolStripLabel quantityValue;
        private System.Windows.Forms.Panel statusPanel;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Label progressText;
        private System.Windows.Forms.Label statusText;
        private System.Windows.Forms.Label diffText;
        private System.Windows.Forms.Label weightText;
    }
}

