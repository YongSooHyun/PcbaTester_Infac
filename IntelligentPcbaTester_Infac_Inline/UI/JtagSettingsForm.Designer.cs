namespace IntelligentPcbaTester
{
    partial class JtagSettingsForm
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
            this.useRadioButton = new System.Windows.Forms.RadioButton();
            this.notUseRadioButton = new System.Windows.Forms.RadioButton();
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.groupBox1.SuspendLayout();
            this.SuspendLayout();
            // 
            // useRadioButton
            // 
            this.useRadioButton.AutoSize = true;
            this.useRadioButton.Location = new System.Drawing.Point(11, 24);
            this.useRadioButton.Name = "useRadioButton";
            this.useRadioButton.Size = new System.Drawing.Size(49, 19);
            this.useRadioButton.TabIndex = 0;
            this.useRadioButton.Text = "사용";
            this.useRadioButton.UseVisualStyleBackColor = true;
            this.useRadioButton.CheckedChanged += new System.EventHandler(this.useRadioButton_CheckedChanged);
            // 
            // notUseRadioButton
            // 
            this.notUseRadioButton.AutoSize = true;
            this.notUseRadioButton.Location = new System.Drawing.Point(66, 24);
            this.notUseRadioButton.Name = "notUseRadioButton";
            this.notUseRadioButton.Size = new System.Drawing.Size(79, 19);
            this.notUseRadioButton.TabIndex = 1;
            this.notUseRadioButton.Text = "사용 안 함";
            this.notUseRadioButton.UseVisualStyleBackColor = true;
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.notUseRadioButton);
            this.groupBox1.Controls.Add(this.useRadioButton);
            this.groupBox1.Location = new System.Drawing.Point(3, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(573, 60);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "사용 여부";
            // 
            // JtagSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(579, 425);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "JtagSettingsForm";
            this.Text = "JTAG";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.RadioButton useRadioButton;
        private System.Windows.Forms.RadioButton notUseRadioButton;
        private System.Windows.Forms.GroupBox groupBox1;
    }
}