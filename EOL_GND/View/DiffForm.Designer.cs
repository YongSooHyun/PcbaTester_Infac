namespace EOL_GND.View
{
    partial class DiffForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DiffForm));
            this.diffControl1 = new Menees.Diffs.Windows.Forms.DiffControl();
            this.SuspendLayout();
            // 
            // diffControl1
            // 
            this.diffControl1.Dock = System.Windows.Forms.DockStyle.Fill;
            this.diffControl1.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.diffControl1.Location = new System.Drawing.Point(0, 0);
            this.diffControl1.Name = "diffControl1";
            this.diffControl1.ShowWhiteSpaceInLineDiff = true;
            this.diffControl1.Size = new System.Drawing.Size(1099, 624);
            this.diffControl1.TabIndex = 0;
            this.diffControl1.ViewFont = new System.Drawing.Font("Courier New", 9F);
            // 
            // DiffForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1099, 624);
            this.Controls.Add(this.diffControl1);
            this.Font = new System.Drawing.Font("Courier New", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DiffForm";
            this.Text = "Difference Viewer";
            this.ResumeLayout(false);

        }

        #endregion

        private Menees.Diffs.Windows.Forms.DiffControl diffControl1;
    }
}