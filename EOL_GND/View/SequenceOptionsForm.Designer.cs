namespace EOL_GND.View
{
    partial class SequenceOptionsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(SequenceOptionsForm));
            this.cleanupGroupBox = new System.Windows.Forms.GroupBox();
            this.cleanupTextBox = new System.Windows.Forms.TextBox();
            this.cleanupCheckBox = new System.Windows.Forms.CheckBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.cleanupGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // cleanupGroupBox
            // 
            this.cleanupGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanupGroupBox.Controls.Add(this.cleanupTextBox);
            this.cleanupGroupBox.Controls.Add(this.cleanupCheckBox);
            this.cleanupGroupBox.Location = new System.Drawing.Point(12, 12);
            this.cleanupGroupBox.Name = "cleanupGroupBox";
            this.cleanupGroupBox.Size = new System.Drawing.Size(470, 81);
            this.cleanupGroupBox.TabIndex = 0;
            this.cleanupGroupBox.TabStop = false;
            this.cleanupGroupBox.Text = "C&leanup";
            // 
            // cleanupTextBox
            // 
            this.cleanupTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.cleanupTextBox.Location = new System.Drawing.Point(10, 45);
            this.cleanupTextBox.Name = "cleanupTextBox";
            this.cleanupTextBox.Size = new System.Drawing.Size(449, 23);
            this.cleanupTextBox.TabIndex = 1;
            // 
            // cleanupCheckBox
            // 
            this.cleanupCheckBox.AutoSize = true;
            this.cleanupCheckBox.Location = new System.Drawing.Point(10, 22);
            this.cleanupCheckBox.Name = "cleanupCheckBox";
            this.cleanupCheckBox.Size = new System.Drawing.Size(409, 19);
            this.cleanupCheckBox.TabIndex = 0;
            this.cleanupCheckBox.Text = "테스트 중단 시 다음 ID를 가진 스텝들을 실행합니다(예: 1~3, 5, 7, 12~17):";
            this.toolTip1.SetToolTip(this.cleanupCheckBox, "테스트가 마지막까지 실행되면 이 스텝들은 실행되지 않습니다.\r\n또한, 지정한 ID를 찾을 수 없으면 해당 스텝은 실행되지 않습니다.");
            this.cleanupCheckBox.UseVisualStyleBackColor = true;
            this.cleanupCheckBox.CheckedChanged += new System.EventHandler(this.cleanupCheckBox_CheckedChanged);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(370, 216);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(112, 32);
            this.cancelButton.TabIndex = 101;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(252, 216);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 32);
            this.okButton.TabIndex = 100;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // SequenceOptionsForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(494, 260);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.cleanupGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "SequenceOptionsForm";
            this.ShowInTaskbar = false;
            this.Text = "Sequence Options";
            this.cleanupGroupBox.ResumeLayout(false);
            this.cleanupGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox cleanupGroupBox;
        private System.Windows.Forms.CheckBox cleanupCheckBox;
        private System.Windows.Forms.TextBox cleanupTextBox;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.ToolTip toolTip1;
    }
}