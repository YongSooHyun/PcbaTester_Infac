namespace EOL_GND.View
{
    partial class ByteArrayEditorForm
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
            this.hexBox = new Be.Windows.Forms.HexBox();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.exportFileButton = new System.Windows.Forms.Button();
            this.importFileButton = new System.Windows.Forms.Button();
            this.SuspendLayout();
            // 
            // hexBox
            // 
            this.hexBox.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.hexBox.ColumnInfoVisible = true;
            this.hexBox.Font = new System.Drawing.Font("Consolas", 10F);
            this.hexBox.GroupSeparatorVisible = true;
            this.hexBox.GroupSize = 8;
            this.hexBox.LineInfoVisible = true;
            this.hexBox.Location = new System.Drawing.Point(13, 12);
            this.hexBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.hexBox.Name = "hexBox";
            this.hexBox.ShadowSelectionColor = System.Drawing.Color.FromArgb(((int)(((byte)(100)))), ((int)(((byte)(60)))), ((int)(((byte)(188)))), ((int)(((byte)(255)))));
            this.hexBox.Size = new System.Drawing.Size(642, 382);
            this.hexBox.StringViewVisible = true;
            this.hexBox.TabIndex = 0;
            this.hexBox.VScrollBarVisible = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(435, 403);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(107, 29);
            this.okButton.TabIndex = 3;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(548, 403);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(107, 29);
            this.cancelButton.TabIndex = 4;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // exportFileButton
            // 
            this.exportFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.exportFileButton.Location = new System.Drawing.Point(126, 403);
            this.exportFileButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.exportFileButton.Name = "exportFileButton";
            this.exportFileButton.Size = new System.Drawing.Size(107, 29);
            this.exportFileButton.TabIndex = 2;
            this.exportFileButton.Text = "&Export to File...";
            this.exportFileButton.UseVisualStyleBackColor = true;
            this.exportFileButton.Click += new System.EventHandler(this.exportFileButton_Click);
            // 
            // importFileButton
            // 
            this.importFileButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.importFileButton.Location = new System.Drawing.Point(13, 403);
            this.importFileButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.importFileButton.Name = "importFileButton";
            this.importFileButton.Size = new System.Drawing.Size(107, 29);
            this.importFileButton.TabIndex = 1;
            this.importFileButton.Text = "&Import File...";
            this.importFileButton.UseVisualStyleBackColor = true;
            this.importFileButton.Click += new System.EventHandler(this.importFileButton_Click);
            // 
            // ByteArrayEditForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(668, 441);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.exportFileButton);
            this.Controls.Add(this.importFileButton);
            this.Controls.Add(this.hexBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ByteArrayEditForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.Text = "Hex Editor";
            this.ResumeLayout(false);

        }

        #endregion

        private Be.Windows.Forms.HexBox hexBox;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.Button exportFileButton;
        private System.Windows.Forms.Button importFileButton;
    }
}