
namespace IntelligentPcbaTester
{
    partial class ProbeCountEditForm
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
            this.label1 = new System.Windows.Forms.Label();
            this.todayNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.todayPassedNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.label3 = new System.Windows.Forms.Label();
            this.todayRatioLabel = new System.Windows.Forms.Label();
            this.fidNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.label5 = new System.Windows.Forms.Label();
            this.totalNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label6 = new System.Windows.Forms.Label();
            this.totalPassedNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label7 = new System.Windows.Forms.Label();
            this.totalRatioLabel = new System.Windows.Forms.Label();
            ((System.ComponentModel.ISupportInitialize)(this.todayNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayPassedNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.fidNumericUpDown)).BeginInit();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalPassedNumericUpDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 26);
            this.label1.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(43, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Tested:";
            // 
            // todayNumericUpDown
            // 
            this.todayNumericUpDown.Location = new System.Drawing.Point(59, 23);
            this.todayNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.todayNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.todayNumericUpDown.Name = "todayNumericUpDown";
            this.todayNumericUpDown.Size = new System.Drawing.Size(99, 23);
            this.todayNumericUpDown.TabIndex = 1;
            this.todayNumericUpDown.ThousandsSeparator = true;
            this.todayNumericUpDown.ValueChanged += new System.EventHandler(this.totalNumericUpDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(9, 58);
            this.label2.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(46, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "&Passed:";
            // 
            // todayPassedNumericUpDown
            // 
            this.todayPassedNumericUpDown.Location = new System.Drawing.Point(59, 55);
            this.todayPassedNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.todayPassedNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.todayPassedNumericUpDown.Name = "todayPassedNumericUpDown";
            this.todayPassedNumericUpDown.Size = new System.Drawing.Size(99, 23);
            this.todayPassedNumericUpDown.TabIndex = 3;
            this.todayPassedNumericUpDown.ThousandsSeparator = true;
            this.todayPassedNumericUpDown.ValueChanged += new System.EventHandler(this.passedNumericUpDown_ValueChanged);
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(75, 190);
            this.okButton.Margin = new System.Windows.Forms.Padding(2);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(96, 28);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(205, 190);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(2);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(96, 28);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(18, 89);
            this.label3.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(37, 15);
            this.label3.TabIndex = 4;
            this.label3.Text = "Ratio:";
            // 
            // todayRatioLabel
            // 
            this.todayRatioLabel.AutoSize = true;
            this.todayRatioLabel.Location = new System.Drawing.Point(59, 89);
            this.todayRatioLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.todayRatioLabel.Name = "todayRatioLabel";
            this.todayRatioLabel.Size = new System.Drawing.Size(35, 15);
            this.todayRatioLabel.TabIndex = 5;
            this.todayRatioLabel.Text = "0.0 %";
            // 
            // fidNumericUpDown
            // 
            this.fidNumericUpDown.Location = new System.Drawing.Point(142, 20);
            this.fidNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.fidNumericUpDown.Maximum = new decimal(new int[] {
            255,
            0,
            0,
            0});
            this.fidNumericUpDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fidNumericUpDown.Name = "fidNumericUpDown";
            this.fidNumericUpDown.Size = new System.Drawing.Size(120, 23);
            this.fidNumericUpDown.TabIndex = 1;
            this.fidNumericUpDown.ThousandsSeparator = true;
            this.fidNumericUpDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.fidNumericUpDown.ValueChanged += new System.EventHandler(this.fidNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(111, 23);
            this.label4.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(27, 15);
            this.label4.TabIndex = 0;
            this.label4.Text = "&FID:";
            // 
            // groupBox1
            // 
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Controls.Add(this.todayNumericUpDown);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.todayPassedNumericUpDown);
            this.groupBox1.Controls.Add(this.label3);
            this.groupBox1.Controls.Add(this.todayRatioLabel);
            this.groupBox1.Location = new System.Drawing.Point(12, 58);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(169, 116);
            this.groupBox1.TabIndex = 2;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "To&day";
            // 
            // groupBox2
            // 
            this.groupBox2.Controls.Add(this.label5);
            this.groupBox2.Controls.Add(this.totalNumericUpDown);
            this.groupBox2.Controls.Add(this.label6);
            this.groupBox2.Controls.Add(this.totalPassedNumericUpDown);
            this.groupBox2.Controls.Add(this.label7);
            this.groupBox2.Controls.Add(this.totalRatioLabel);
            this.groupBox2.Location = new System.Drawing.Point(196, 58);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(169, 116);
            this.groupBox2.TabIndex = 3;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "Tot&al";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(12, 26);
            this.label5.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(43, 15);
            this.label5.TabIndex = 0;
            this.label5.Text = "&Tested:";
            // 
            // totalNumericUpDown
            // 
            this.totalNumericUpDown.Location = new System.Drawing.Point(59, 23);
            this.totalNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.totalNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.totalNumericUpDown.Name = "totalNumericUpDown";
            this.totalNumericUpDown.Size = new System.Drawing.Size(99, 23);
            this.totalNumericUpDown.TabIndex = 1;
            this.totalNumericUpDown.ThousandsSeparator = true;
            this.totalNumericUpDown.ValueChanged += new System.EventHandler(this.totalNumericUpDown_ValueChanged_1);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(9, 58);
            this.label6.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(46, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "&Passed:";
            // 
            // totalPassedNumericUpDown
            // 
            this.totalPassedNumericUpDown.Location = new System.Drawing.Point(59, 55);
            this.totalPassedNumericUpDown.Margin = new System.Windows.Forms.Padding(2);
            this.totalPassedNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.totalPassedNumericUpDown.Name = "totalPassedNumericUpDown";
            this.totalPassedNumericUpDown.Size = new System.Drawing.Size(99, 23);
            this.totalPassedNumericUpDown.TabIndex = 3;
            this.totalPassedNumericUpDown.ThousandsSeparator = true;
            this.totalPassedNumericUpDown.ValueChanged += new System.EventHandler(this.totalPassedNumericUpDown_ValueChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(18, 89);
            this.label7.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(37, 15);
            this.label7.TabIndex = 4;
            this.label7.Text = "Ratio:";
            // 
            // totalRatioLabel
            // 
            this.totalRatioLabel.AutoSize = true;
            this.totalRatioLabel.Location = new System.Drawing.Point(59, 89);
            this.totalRatioLabel.Margin = new System.Windows.Forms.Padding(2, 0, 2, 0);
            this.totalRatioLabel.Name = "totalRatioLabel";
            this.totalRatioLabel.Size = new System.Drawing.Size(35, 15);
            this.totalRatioLabel.TabIndex = 5;
            this.totalRatioLabel.Text = "0.0 %";
            // 
            // ProbeCountEditForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(377, 232);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Controls.Add(this.fidNumericUpDown);
            this.Controls.Add(this.label4);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(2);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "ProbeCountEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "검사 수 편집";
            ((System.ComponentModel.ISupportInitialize)(this.todayNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.todayPassedNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.fidNumericUpDown)).EndInit();
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.totalNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.totalPassedNumericUpDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.NumericUpDown todayNumericUpDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown todayPassedNumericUpDown;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.Label todayRatioLabel;
        private System.Windows.Forms.NumericUpDown fidNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown totalNumericUpDown;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.NumericUpDown totalPassedNumericUpDown;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.Label totalRatioLabel;
    }
}