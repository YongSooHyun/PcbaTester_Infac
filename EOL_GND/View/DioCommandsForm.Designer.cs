namespace EOL_GND.View
{
    partial class DioCommandsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DioCommandsForm));
            this.categoryImageList = new System.Windows.Forms.ImageList(this.components);
            this.commandsListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.commandColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timeoutColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.removeButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.commandsTextBox = new System.Windows.Forms.TextBox();
            this.cancelButton = new System.Windows.Forms.Button();
            this.descColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            ((System.ComponentModel.ISupportInitialize)(this.commandsListView)).BeginInit();
            this.SuspendLayout();
            // 
            // categoryImageList
            // 
            this.categoryImageList.ImageStream = ((System.Windows.Forms.ImageListStreamer)(resources.GetObject("categoryImageList.ImageStream")));
            this.categoryImageList.TransparentColor = System.Drawing.Color.Transparent;
            this.categoryImageList.Images.SetKeyName(0, "empty-32.png");
            this.categoryImageList.Images.SetKeyName(1, "power-32.png");
            this.categoryImageList.Images.SetKeyName(2, "dmm-32.png");
            this.categoryImageList.Images.SetKeyName(3, "oscilloscope-32.png");
            this.categoryImageList.Images.SetKeyName(4, "waveform-32.png");
            // 
            // commandsListView
            // 
            this.commandsListView.AllColumns.Add(this.noColumn);
            this.commandsListView.AllColumns.Add(this.commandColumn);
            this.commandsListView.AllColumns.Add(this.timeoutColumn);
            this.commandsListView.AllColumns.Add(this.descColumn);
            this.commandsListView.AllowColumnReorder = true;
            this.commandsListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandsListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.commandsListView.CellEditUseWholeCell = false;
            this.commandsListView.CheckBoxes = true;
            this.commandsListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.commandColumn,
            this.timeoutColumn,
            this.descColumn});
            this.commandsListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.commandsListView.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.commandsListView.FullRowSelect = true;
            this.commandsListView.GridLines = true;
            this.commandsListView.HideSelection = false;
            this.commandsListView.Location = new System.Drawing.Point(12, 12);
            this.commandsListView.Name = "commandsListView";
            this.commandsListView.ShowCommandMenuOnRightClick = true;
            this.commandsListView.ShowGroups = false;
            this.commandsListView.ShowImagesOnSubItems = true;
            this.commandsListView.ShowItemToolTips = true;
            this.commandsListView.Size = new System.Drawing.Size(448, 387);
            this.commandsListView.TabIndex = 0;
            this.commandsListView.UseCompatibleStateImageBehavior = false;
            this.commandsListView.UseFilterIndicator = true;
            this.commandsListView.View = System.Windows.Forms.View.Details;
            this.commandsListView.VirtualMode = true;
            this.commandsListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.commandsListView_CellEditFinishing);
            this.commandsListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.commandsListView_CellEditStarting);
            this.commandsListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.commandsListView_FormatRow);
            this.commandsListView.SelectionChanged += new System.EventHandler(this.commandsListView_SelectionChanged);
            this.commandsListView.ItemActivate += new System.EventHandler(this.commandsListView_ItemActivate);
            this.commandsListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.commandsListView_ItemChecked);
            // 
            // noColumn
            // 
            this.noColumn.IsEditable = false;
            this.noColumn.Sortable = false;
            this.noColumn.Text = "NO";
            this.noColumn.UseFiltering = false;
            this.noColumn.Width = 55;
            // 
            // commandColumn
            // 
            this.commandColumn.AspectName = "Command";
            this.commandColumn.Text = "Command";
            this.commandColumn.Width = 125;
            // 
            // timeoutColumn
            // 
            this.timeoutColumn.AspectName = "Timeout";
            this.timeoutColumn.Text = "Timeout(ms)";
            this.timeoutColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.timeoutColumn.Width = 95;
            // 
            // removeButton
            // 
            this.removeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.removeButton.Location = new System.Drawing.Point(353, 408);
            this.removeButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(107, 29);
            this.removeButton.TabIndex = 3;
            this.removeButton.Text = "&Remove";
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.Location = new System.Drawing.Point(353, 408);
            this.okButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(107, 29);
            this.okButton.TabIndex = 4;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.addButton.Location = new System.Drawing.Point(240, 408);
            this.addButton.Margin = new System.Windows.Forms.Padding(3, 0, 3, 0);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(107, 29);
            this.addButton.TabIndex = 2;
            this.addButton.Text = "&Add";
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // commandsTextBox
            // 
            this.commandsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commandsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.commandsTextBox.Location = new System.Drawing.Point(12, 415);
            this.commandsTextBox.Name = "commandsTextBox";
            this.commandsTextBox.ReadOnly = true;
            this.commandsTextBox.Size = new System.Drawing.Size(335, 16);
            this.commandsTextBox.TabIndex = 1;
            // 
            // cancelButton
            // 
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(97, 381);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(0, 0);
            this.cancelButton.TabIndex = 5;
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // descColumn
            // 
            this.descColumn.AspectName = "Description";
            this.descColumn.Text = "Description";
            this.descColumn.Width = 150;
            // 
            // DioCommandsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(472, 446);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.removeButton);
            this.Controls.Add(this.addButton);
            this.Controls.Add(this.okButton);
            this.Controls.Add(this.commandsListView);
            this.Controls.Add(this.commandsTextBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DioCommandsForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "DIO 명령";
            ((System.ComponentModel.ISupportInitialize)(this.commandsListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion
        private BrightIdeasSoftware.FastObjectListView commandsListView;
        private System.Windows.Forms.Button removeButton;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn commandColumn;
        private BrightIdeasSoftware.OLVColumn timeoutColumn;
        private System.Windows.Forms.ImageList categoryImageList;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.TextBox commandsTextBox;
        private System.Windows.Forms.Button cancelButton;
        private BrightIdeasSoftware.OLVColumn descColumn;
    }
}