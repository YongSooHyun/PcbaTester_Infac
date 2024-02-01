namespace EOL_GND.View
{
    partial class RunOptionsForm
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
            this.components = new System.ComponentModel.Container();
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(RunOptionsForm));
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.disabledCheckBox = new System.Windows.Forms.CheckBox();
            this.variantComboBox = new System.Windows.Forms.ComboBox();
            this.disabledGroupBox = new System.Windows.Forms.GroupBox();
            this.variantGroupBox = new System.Windows.Forms.GroupBox();
            this.disabledGroupBox.SuspendLayout();
            this.variantGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(285, 175);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 32);
            this.cancelButton.TabIndex = 101;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(167, 175);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 32);
            this.okButton.TabIndex = 100;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // disabledCheckBox
            // 
            this.disabledCheckBox.AutoSize = true;
            this.disabledCheckBox.Location = new System.Drawing.Point(9, 23);
            this.disabledCheckBox.Name = "disabledCheckBox";
            this.disabledCheckBox.Size = new System.Drawing.Size(200, 19);
            this.disabledCheckBox.TabIndex = 0;
            this.disabledCheckBox.Text = "Disabled된 스텝들을 실행합니다.";
            this.disabledCheckBox.UseVisualStyleBackColor = true;
            // 
            // variantComboBox
            // 
            this.variantComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variantComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.variantComboBox.FormattingEnabled = true;
            this.variantComboBox.Location = new System.Drawing.Point(9, 22);
            this.variantComboBox.Name = "variantComboBox";
            this.variantComboBox.Size = new System.Drawing.Size(367, 23);
            this.variantComboBox.TabIndex = 0;
            // 
            // disabledGroupBox
            // 
            this.disabledGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.disabledGroupBox.Controls.Add(this.disabledCheckBox);
            this.disabledGroupBox.Location = new System.Drawing.Point(12, 12);
            this.disabledGroupBox.Name = "disabledGroupBox";
            this.disabledGroupBox.Size = new System.Drawing.Size(385, 53);
            this.disabledGroupBox.TabIndex = 0;
            this.disabledGroupBox.TabStop = false;
            this.disabledGroupBox.Text = "Disabled 스텝 실행 여부";
            // 
            // variantGroupBox
            // 
            this.variantGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variantGroupBox.Controls.Add(this.variantComboBox);
            this.variantGroupBox.Location = new System.Drawing.Point(12, 71);
            this.variantGroupBox.Name = "variantGroupBox";
            this.variantGroupBox.Size = new System.Drawing.Size(385, 57);
            this.variantGroupBox.TabIndex = 1;
            this.variantGroupBox.TabStop = false;
            this.variantGroupBox.Text = "Variant";
            // 
            // RunOptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(409, 219);
            this.Controls.Add(this.variantGroupBox);
            this.Controls.Add(this.disabledGroupBox);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "RunOptionsForm";
            this.ShowInTaskbar = false;
            this.Text = "Run Options";
            this.disabledGroupBox.ResumeLayout(false);
            this.disabledGroupBox.PerformLayout();
            this.variantGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.CheckBox disabledCheckBox;
        private System.Windows.Forms.ComboBox variantComboBox;
        private System.Windows.Forms.GroupBox disabledGroupBox;
        private System.Windows.Forms.GroupBox variantGroupBox;
    }
}