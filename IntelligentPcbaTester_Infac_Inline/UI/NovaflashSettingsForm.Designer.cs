namespace IntelligentPcbaTester
{
    partial class NovaflashSettingsForm
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
            this.lanRadioButton = new System.Windows.Forms.RadioButton();
            this.serialRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.lanGroupBox = new System.Windows.Forms.GroupBox();
            this.ipTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.serialGroupBox = new System.Windows.Forms.GroupBox();
            this.baudRateNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.comPortTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.groupBox1.SuspendLayout();
            this.lanGroupBox.SuspendLayout();
            this.serialGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baudRateNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // lanRadioButton
            // 
            this.lanRadioButton.AutoSize = true;
            this.lanRadioButton.Checked = true;
            this.lanRadioButton.Location = new System.Drawing.Point(11, 24);
            this.lanRadioButton.Name = "lanRadioButton";
            this.lanRadioButton.Size = new System.Drawing.Size(48, 19);
            this.lanRadioButton.TabIndex = 0;
            this.lanRadioButton.TabStop = true;
            this.lanRadioButton.Text = "LAN";
            this.lanRadioButton.UseVisualStyleBackColor = true;
            this.lanRadioButton.CheckedChanged += new System.EventHandler(this.lanRadioButton_CheckedChanged);
            // 
            // serialRadioButton
            // 
            this.serialRadioButton.AutoSize = true;
            this.serialRadioButton.Location = new System.Drawing.Point(65, 24);
            this.serialRadioButton.Name = "serialRadioButton";
            this.serialRadioButton.Size = new System.Drawing.Size(53, 19);
            this.serialRadioButton.TabIndex = 1;
            this.serialRadioButton.Text = "Serial";
            this.serialRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.serialRadioButton);
            this.groupBox1.Controls.Add(this.lanRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(573, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "연결 형태";
            // 
            // lanGroupBox
            // 
            this.lanGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.lanGroupBox.Controls.Add(this.ipTextBox);
            this.lanGroupBox.Controls.Add(this.label1);
            this.lanGroupBox.Location = new System.Drawing.Point(3, 69);
            this.lanGroupBox.Name = "lanGroupBox";
            this.lanGroupBox.Size = new System.Drawing.Size(573, 66);
            this.lanGroupBox.TabIndex = 1;
            this.lanGroupBox.TabStop = false;
            this.lanGroupBox.Text = "LAN";
            // 
            // ipTextBox
            // 
            this.ipTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.ipTextBox.Location = new System.Drawing.Point(35, 26);
            this.ipTextBox.Name = "ipTextBox";
            this.ipTextBox.Size = new System.Drawing.Size(529, 23);
            this.ipTextBox.TabIndex = 1;
            this.ipTextBox.TextChanged += new System.EventHandler(this.ipTextBox_TextChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(8, 29);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(20, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "IP:";
            // 
            // serialGroupBox
            // 
            this.serialGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serialGroupBox.Controls.Add(this.baudRateNumericUpDown);
            this.serialGroupBox.Controls.Add(this.label3);
            this.serialGroupBox.Controls.Add(this.comPortTextBox);
            this.serialGroupBox.Controls.Add(this.label2);
            this.serialGroupBox.Enabled = false;
            this.serialGroupBox.Location = new System.Drawing.Point(3, 141);
            this.serialGroupBox.Name = "serialGroupBox";
            this.serialGroupBox.Size = new System.Drawing.Size(573, 98);
            this.serialGroupBox.TabIndex = 2;
            this.serialGroupBox.TabStop = false;
            this.serialGroupBox.Text = "Serial";
            // 
            // baudRateNumericUpDown
            // 
            this.baudRateNumericUpDown.Location = new System.Drawing.Point(79, 60);
            this.baudRateNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.baudRateNumericUpDown.Name = "baudRateNumericUpDown";
            this.baudRateNumericUpDown.Size = new System.Drawing.Size(120, 23);
            this.baudRateNumericUpDown.TabIndex = 3;
            this.baudRateNumericUpDown.ThousandsSeparator = true;
            this.baudRateNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(8, 62);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(60, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "BaudRate:";
            // 
            // comPortTextBox
            // 
            this.comPortTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.comPortTextBox.Location = new System.Drawing.Point(79, 26);
            this.comPortTextBox.Name = "comPortTextBox";
            this.comPortTextBox.Size = new System.Drawing.Size(485, 23);
            this.comPortTextBox.TabIndex = 1;
            this.comPortTextBox.TextChanged += new System.EventHandler(this.comPortTextBox_TextChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(8, 29);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(32, 15);
            this.label2.TabIndex = 0;
            this.label2.Text = "Port:";
            // 
            // NovaflashSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 425);
            this.Controls.Add(this.serialGroupBox);
            this.Controls.Add(this.lanGroupBox);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "NovaflashSettingsForm";
            this.Text = "NovaFlash";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.lanGroupBox.ResumeLayout(false);
            this.lanGroupBox.PerformLayout();
            this.serialGroupBox.ResumeLayout(false);
            this.serialGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.baudRateNumericUpDown)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton lanRadioButton;
        private System.Windows.Forms.RadioButton serialRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox lanGroupBox;
        private System.Windows.Forms.TextBox ipTextBox;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.GroupBox serialGroupBox;
        private System.Windows.Forms.TextBox comPortTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown baudRateNumericUpDown;
        private System.Windows.Forms.Label label3;
    }
}