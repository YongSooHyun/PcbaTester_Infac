namespace EOL_GND.View
{
    partial class FindReplaceForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(FindReplaceForm));
            this.mainTabControl = new System.Windows.Forms.TabControl();
            this.findPage = new System.Windows.Forms.TabPage();
            this.label1 = new System.Windows.Forms.Label();
            this.findNextButton = new System.Windows.Forms.Button();
            this.findPrevButton = new System.Windows.Forms.Button();
            this.matchCaseCheckBox = new System.Windows.Forms.CheckBox();
            this.patternComboBox = new System.Windows.Forms.ComboBox();
            this.closeButton = new System.Windows.Forms.Button();
            this.mainTabControl.SuspendLayout();
            this.findPage.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainTabControl
            // 
            this.mainTabControl.Controls.Add(this.findPage);
            this.mainTabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainTabControl.Location = new System.Drawing.Point(0, 0);
            this.mainTabControl.Name = "mainTabControl";
            this.mainTabControl.SelectedIndex = 0;
            this.mainTabControl.Size = new System.Drawing.Size(551, 409);
            this.mainTabControl.TabIndex = 0;
            // 
            // findPage
            // 
            this.findPage.BackColor = System.Drawing.SystemColors.Control;
            this.findPage.Controls.Add(this.label1);
            this.findPage.Controls.Add(this.findNextButton);
            this.findPage.Controls.Add(this.findPrevButton);
            this.findPage.Controls.Add(this.matchCaseCheckBox);
            this.findPage.Controls.Add(this.patternComboBox);
            this.findPage.Location = new System.Drawing.Point(4, 24);
            this.findPage.Name = "findPage";
            this.findPage.Padding = new System.Windows.Forms.Padding(3);
            this.findPage.Size = new System.Drawing.Size(543, 381);
            this.findPage.TabIndex = 0;
            this.findPage.Text = "Find";
            // 
            // label1
            // 
            this.label1.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.label1.Font = new System.Drawing.Font("Consolas", 8.25F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.label1.Location = new System.Drawing.Point(15, 82);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(513, 247);
            this.label1.TabIndex = 4;
            this.label1.Text = resources.GetString("label1.Text");
            // 
            // findNextButton
            // 
            this.findNextButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.findNextButton.Location = new System.Drawing.Point(423, 340);
            this.findNextButton.Name = "findNextButton";
            this.findNextButton.Size = new System.Drawing.Size(105, 29);
            this.findNextButton.TabIndex = 3;
            this.findNextButton.Text = "&Find Next";
            this.findNextButton.UseVisualStyleBackColor = true;
            this.findNextButton.Click += new System.EventHandler(this.findNextButton_Click);
            // 
            // findPrevButton
            // 
            this.findPrevButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.findPrevButton.Location = new System.Drawing.Point(312, 340);
            this.findPrevButton.Name = "findPrevButton";
            this.findPrevButton.Size = new System.Drawing.Size(105, 29);
            this.findPrevButton.TabIndex = 2;
            this.findPrevButton.Text = "Find &Previous";
            this.findPrevButton.UseVisualStyleBackColor = true;
            this.findPrevButton.Click += new System.EventHandler(this.findPrevButton_Click);
            // 
            // matchCaseCheckBox
            // 
            this.matchCaseCheckBox.AutoSize = true;
            this.matchCaseCheckBox.Location = new System.Drawing.Point(15, 49);
            this.matchCaseCheckBox.Name = "matchCaseCheckBox";
            this.matchCaseCheckBox.Size = new System.Drawing.Size(86, 19);
            this.matchCaseCheckBox.TabIndex = 1;
            this.matchCaseCheckBox.Text = "Match &case";
            this.matchCaseCheckBox.UseVisualStyleBackColor = true;
            this.matchCaseCheckBox.CheckedChanged += new System.EventHandler(this.matchCaseCheckBox_CheckedChanged);
            // 
            // patternComboBox
            // 
            this.patternComboBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.patternComboBox.FormattingEnabled = true;
            this.patternComboBox.Location = new System.Drawing.Point(15, 17);
            this.patternComboBox.Name = "patternComboBox";
            this.patternComboBox.Size = new System.Drawing.Size(513, 23);
            this.patternComboBox.TabIndex = 0;
            this.patternComboBox.TextChanged += new System.EventHandler(this.patternComboBox_TextChanged);
            // 
            // closeButton
            // 
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.closeButton.Location = new System.Drawing.Point(0, 409);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(551, 0);
            this.closeButton.TabIndex = 1;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            this.closeButton.Click += new System.EventHandler(this.closeButton_Click);
            // 
            // FindReplaceForm
            // 
            this.AcceptButton = this.findNextButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(551, 409);
            this.Controls.Add(this.mainTabControl);
            this.Controls.Add(this.closeButton);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "FindReplaceForm";
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "Find and Replace";
            this.mainTabControl.ResumeLayout(false);
            this.findPage.ResumeLayout(false);
            this.findPage.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.TabControl mainTabControl;
        private System.Windows.Forms.TabPage findPage;
        private System.Windows.Forms.ComboBox patternComboBox;
        private System.Windows.Forms.CheckBox matchCaseCheckBox;
        private System.Windows.Forms.Button findNextButton;
        private System.Windows.Forms.Button findPrevButton;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Label label1;
    }
}