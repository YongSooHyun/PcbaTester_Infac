namespace EOL_GND.View
{
    partial class StepCreationForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepCreationForm));
            this.label1 = new System.Windows.Forms.Label();
            this.templatesListView = new BrightIdeasSoftware.ObjectListView();
            this.nameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.templateSmallImageList = new System.Windows.Forms.ImageList(this.components);
            this.label2 = new System.Windows.Forms.Label();
            this.countNUDown = new System.Windows.Forms.NumericUpDown();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.templatesListView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.countNUDown)).BeginInit();
            this.SuspendLayout();
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(12, 14);
            this.label1.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(214, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "추가할 테스트 스텝 유형을 선택하세요.";
            // 
            // templatesListView
            // 
            this.templatesListView.AllColumns.Add(this.nameColumn);
            this.templatesListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.templatesListView.CellEditUseWholeCell = false;
            this.templatesListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.nameColumn});
            this.templatesListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.templatesListView.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.templatesListView.FullRowSelect = true;
            this.templatesListView.GridLines = true;
            this.templatesListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.templatesListView.HideSelection = false;
            this.templatesListView.Location = new System.Drawing.Point(12, 35);
            this.templatesListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.templatesListView.MultiSelect = false;
            this.templatesListView.Name = "templatesListView";
            this.templatesListView.ShowGroups = false;
            this.templatesListView.Size = new System.Drawing.Size(381, 505);
            this.templatesListView.SmallImageList = this.templateSmallImageList;
            this.templatesListView.TabIndex = 1;
            this.templatesListView.UseCompatibleStateImageBehavior = false;
            this.templatesListView.View = System.Windows.Forms.View.Details;
            this.templatesListView.ItemActivate += new System.EventHandler(this.templatesListView_ItemActivate);
            // 
            // nameColumn
            // 
            this.nameColumn.AspectName = "CategoryName";
            this.nameColumn.FillsFreeSpace = true;
            this.nameColumn.Text = "Template";
            this.nameColumn.Width = 100;
            // 
            // templateSmallImageList
            // 
            this.templateSmallImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("templateSmallImageList.ImageStream")));
            this.templateSmallImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.templateSmallImageList.Images.SetKeyName(0, "dmm-32.png");
            this.templateSmallImageList.Images.SetKeyName(1, "oscilloscope-32.png");
            this.templateSmallImageList.Images.SetKeyName(2, "power-32.png");
            this.templateSmallImageList.Images.SetKeyName(3, "relay-32.png");
            this.templateSmallImageList.Images.SetKeyName(4, "waveform-32.png");
            this.templateSmallImageList.Images.SetKeyName(5, "microchip-32.png");
            this.templateSmallImageList.Images.SetKeyName(6, "can-32.png");
            this.templateSmallImageList.Images.SetKeyName(7, "stimulus-32.png");
            this.templateSmallImageList.Images.SetKeyName(8, "abort-32.png");
            this.templateSmallImageList.Images.SetKeyName(9, "null-32.png");
            this.templateSmallImageList.Images.SetKeyName(10, "lin-32.png");
            this.templateSmallImageList.Images.SetKeyName(11, "voltmeter-32.png");
            this.templateSmallImageList.Images.SetKeyName(12, "amplifier-32.png");
            this.templateSmallImageList.Images.SetKeyName(13, "secc-32.png");
            this.templateSmallImageList.Images.SetKeyName(14, "serial_port-32.png");
            this.templateSmallImageList.Images.SetKeyName(15, "servo-32.png");
            this.templateSmallImageList.Images.SetKeyName(16, "mighty_zap-32.png");
            // 
            // label2
            // 
            this.label2.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(12, 555);
            this.label2.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(100, 15);
            this.label2.TabIndex = 2;
            this.label2.Text = "테스트 스텝 개수:";
            // 
            // countNUDown
            // 
            this.countNUDown.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left)));
            this.countNUDown.Location = new System.Drawing.Point(118, 552);
            this.countNUDown.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.countNUDown.Maximum = new decimal(new int[] {
            1000,
            0,
            0,
            0});
            this.countNUDown.Minimum = new decimal(new int[] {
            1,
            0,
            0,
            0});
            this.countNUDown.Name = "countNUDown";
            this.countNUDown.Size = new System.Drawing.Size(55, 23);
            this.countNUDown.TabIndex = 3;
            this.countNUDown.Value = new decimal(new int[] {
            1,
            0,
            0,
            0});
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(223, 549);
            this.okButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(82, 29);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(311, 549);
            this.cancelButton.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(82, 29);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // StepCreationForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(405, 590);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.countNUDown);
            this.Controls.Add(this.label2);
            this.Controls.Add(this.templatesListView);
            this.Controls.Add(this.label1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "StepCreationForm";
            this.Text = "Add New Test Step";
            ((System.ComponentModel.ISupportInitialize)(this.templatesListView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.countNUDown)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.ObjectListView templatesListView;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown countNUDown;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private BrightIdeasSoftware.OLVColumn nameColumn;
        private System.Windows.Forms.ImageList templateSmallImageList;
    }
}