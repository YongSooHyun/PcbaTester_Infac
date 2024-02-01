namespace IntelligentPcbaTester
{
    partial class MessageDialog
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
            this.contentLabel = new System.Windows.Forms.Label();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.mainTLPanel = new System.Windows.Forms.TableLayoutPanel();
            this.buttonTLPanel = new System.Windows.Forms.TableLayoutPanel();
            this.mainTLPanel.SuspendLayout();
            this.buttonTLPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // contentLabel
            // 
            this.contentLabel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.contentLabel.Font = new System.Drawing.Font("Segoe UI", 20.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.contentLabel.Location = new System.Drawing.Point(0, 0);
            this.contentLabel.Margin = new System.Windows.Forms.Padding(0, 0, 0, 5);
            this.contentLabel.Name = "contentLabel";
            this.contentLabel.Size = new System.Drawing.Size(388, 202);
            this.contentLabel.TabIndex = 0;
            this.contentLabel.Text = "Dialog message goes here...";
            this.contentLabel.TextAlign = System.Drawing.ContentAlignment.MiddleCenter;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.okButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.okButton.Location = new System.Drawing.Point(26, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(114, 36);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.cancelButton.Location = new System.Drawing.Point(194, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(114, 36);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // mainTLPanel
            // 
            this.mainTLPanel.BackColor = System.Drawing.SystemColors.Control;
            this.mainTLPanel.ColumnCount = 1;
            this.mainTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTLPanel.Controls.Add(this.contentLabel, 0, 0);
            this.mainTLPanel.Controls.Add(this.buttonTLPanel, 0, 1);
            this.mainTLPanel.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTLPanel.Location = new System.Drawing.Point(3, 3);
            this.mainTLPanel.Name = "mainTLPanel";
            this.mainTLPanel.RowCount = 2;
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.mainTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 54F));
            this.mainTLPanel.Size = new System.Drawing.Size(388, 261);
            this.mainTLPanel.TabIndex = 0;
            // 
            // buttonTLPanel
            // 
            this.buttonTLPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom)));
            this.buttonTLPanel.ColumnCount = 2;
            this.buttonTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonTLPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 50F));
            this.buttonTLPanel.Controls.Add(this.cancelButton, 1, 0);
            this.buttonTLPanel.Controls.Add(this.okButton, 0, 0);
            this.buttonTLPanel.Location = new System.Drawing.Point(26, 213);
            this.buttonTLPanel.Margin = new System.Windows.Forms.Padding(3, 6, 3, 6);
            this.buttonTLPanel.Name = "buttonTLPanel";
            this.buttonTLPanel.RowCount = 1;
            this.buttonTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonTLPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.buttonTLPanel.Size = new System.Drawing.Size(335, 42);
            this.buttonTLPanel.TabIndex = 1;
            // 
            // MessageDialog
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 17F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.Blue;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(394, 267);
            this.Controls.Add(this.mainTLPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MessageDialog";
            this.Padding = new System.Windows.Forms.Padding(3);
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.TopMost = true;
            this.mainTLPanel.ResumeLayout(false);
            this.buttonTLPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.Label contentLabel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.TableLayoutPanel mainTLPanel;
        private System.Windows.Forms.TableLayoutPanel buttonTLPanel;
    }
}