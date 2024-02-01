namespace IntelligentPcbaTester
{
    partial class OnOffControl
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

        #region Component Designer generated code

        /// <summary> 
        /// Required method for Designer support - do not modify 
        /// the contents of this method with the code editor.
        /// </summary>
        private void InitializeComponent()
        {
            this.colorLabel = new System.Windows.Forms.Label();
            this.titleLabel = new System.Windows.Forms.Label();
            this.SuspendLayout();
            // 
            // colorLabel
            // 
            this.colorLabel.Anchor = System.Windows.Forms.AnchorStyles.Left;
            this.colorLabel.BackColor = System.Drawing.Color.Silver;
            this.colorLabel.BorderStyle = System.Windows.Forms.BorderStyle.Fixed3D;
            this.colorLabel.Location = new System.Drawing.Point(3, 8);
            this.colorLabel.Name = "colorLabel";
            this.colorLabel.Size = new System.Drawing.Size(13, 13);
            this.colorLabel.TabIndex = 1;
            // 
            // titleLabel
            // 
            this.titleLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.titleLabel.Location = new System.Drawing.Point(18, 0);
            this.titleLabel.Margin = new System.Windows.Forms.Padding(0);
            this.titleLabel.Name = "titleLabel";
            this.titleLabel.Size = new System.Drawing.Size(62, 28);
            this.titleLabel.TabIndex = 2;
            this.titleLabel.Text = "OFF";
            this.titleLabel.TextAlign = System.Drawing.ContentAlignment.MiddleLeft;
            // 
            // OnOffControl
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(6F, 13F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.Controls.Add(this.titleLabel);
            this.Controls.Add(this.colorLabel);
            this.Name = "OnOffControl";
            this.Size = new System.Drawing.Size(80, 28);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.Label colorLabel;
        private System.Windows.Forms.Label titleLabel;
    }
}
