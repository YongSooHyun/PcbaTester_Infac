namespace EOL_GND.View
{
    partial class AmplifierDeviceSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(AmplifierDeviceSettingsForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.saveTSButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addTSButton = new System.Windows.Forms.ToolStripButton();
            this.deleteTSButton = new System.Windows.Forms.ToolStripButton();
            this.listView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.deviceTypeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.nameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.addressColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timeoutColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveTSButton,
            this.toolStripSeparator1,
            this.addTSButton,
            this.deleteTSButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(809, 39);
            this.mainToolStrip.TabIndex = 0;
            this.mainToolStrip.Text = "toolStrip1";
            // 
            // saveTSButton
            // 
            this.saveTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveTSButton.Image = global::EOL_GND.Properties.Resources.save;
            this.saveTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveTSButton.Name = "saveTSButton";
            this.saveTSButton.Size = new System.Drawing.Size(36, 36);
            this.saveTSButton.Text = "Save";
            this.saveTSButton.Click += new System.EventHandler(this.saveTSButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // addTSButton
            // 
            this.addTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addTSButton.Image = global::EOL_GND.Properties.Resources.step_add;
            this.addTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addTSButton.Name = "addTSButton";
            this.addTSButton.Size = new System.Drawing.Size(36, 36);
            this.addTSButton.Text = "Add";
            this.addTSButton.Click += new System.EventHandler(this.addTSButton_Click);
            // 
            // deleteTSButton
            // 
            this.deleteTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteTSButton.Enabled = false;
            this.deleteTSButton.Image = global::EOL_GND.Properties.Resources.step_remove;
            this.deleteTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteTSButton.Name = "deleteTSButton";
            this.deleteTSButton.Size = new System.Drawing.Size(36, 36);
            this.deleteTSButton.Text = "Delete";
            this.deleteTSButton.Click += new System.EventHandler(this.deleteTSButton_Click);
            // 
            // listView
            // 
            this.listView.AllColumns.Add(this.noColumn);
            this.listView.AllColumns.Add(this.deviceTypeColumn);
            this.listView.AllColumns.Add(this.nameColumn);
            this.listView.AllColumns.Add(this.addressColumn);
            this.listView.AllColumns.Add(this.timeoutColumn);
            this.listView.AllowColumnReorder = true;
            this.listView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.listView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.listView.CellEditUseWholeCell = false;
            this.listView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.deviceTypeColumn,
            this.nameColumn,
            this.addressColumn,
            this.timeoutColumn});
            this.listView.Cursor = System.Windows.Forms.Cursors.Default;
            this.listView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.listView.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.listView.FullRowSelect = true;
            this.listView.GridLines = true;
            this.listView.HideSelection = false;
            this.listView.Location = new System.Drawing.Point(0, 39);
            this.listView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.listView.Name = "listView";
            this.listView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.listView.ShowCommandMenuOnRightClick = true;
            this.listView.ShowGroups = false;
            this.listView.ShowImagesOnSubItems = true;
            this.listView.ShowItemToolTips = true;
            this.listView.Size = new System.Drawing.Size(809, 519);
            this.listView.TabIndex = 1;
            this.listView.UseCompatibleStateImageBehavior = false;
            this.listView.UseFilterIndicator = true;
            this.listView.UseFiltering = true;
            this.listView.UseSubItemCheckBoxes = true;
            this.listView.View = System.Windows.Forms.View.Details;
            this.listView.VirtualMode = true;
            this.listView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.listView_CellEditStarting);
            this.listView.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(this.listView_CellEditValidating);
            this.listView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.ListView_FormatRow);
            this.listView.SelectionChanged += new System.EventHandler(this.listView_SelectionChanged);
            // 
            // noColumn
            // 
            this.noColumn.Groupable = false;
            this.noColumn.IsEditable = false;
            this.noColumn.Sortable = false;
            this.noColumn.Text = "NO";
            this.noColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.noColumn.ToolTipText = "";
            this.noColumn.UseFiltering = false;
            this.noColumn.Width = 40;
            // 
            // deviceTypeColumn
            // 
            this.deviceTypeColumn.AspectName = "DeviceType";
            this.deviceTypeColumn.Text = "Type";
            this.deviceTypeColumn.Width = 180;
            // 
            // nameColumn
            // 
            this.nameColumn.AspectName = "DeviceName";
            this.nameColumn.Text = "Name";
            this.nameColumn.ToolTipText = "이름은 중복되지 않아야 합니다.";
            this.nameColumn.Width = 120;
            // 
            // addressColumn
            // 
            this.addressColumn.AspectName = "Address";
            this.addressColumn.Text = "VISA Address";
            this.addressColumn.Width = 150;
            // 
            // timeoutColumn
            // 
            this.timeoutColumn.AspectName = "IOTimeout";
            this.timeoutColumn.AspectToStringFormat = "{0:N0}";
            this.timeoutColumn.Text = "Timeout";
            this.timeoutColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.timeoutColumn.ToolTipText = "I/O Timeout(ms)";
            this.timeoutColumn.Width = 75;
            // 
            // AmplifierDeviceSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(809, 558);
            this.Controls.Add(this.listView);
            this.Controls.Add(this.mainToolStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "AmplifierDeviceSettingsForm";
            this.Text = "Amplifier";
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.listView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private BrightIdeasSoftware.FastObjectListView listView;
        private System.Windows.Forms.ToolStripButton saveTSButton;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn deviceTypeColumn;
        private BrightIdeasSoftware.OLVColumn nameColumn;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton addTSButton;
        private System.Windows.Forms.ToolStripButton deleteTSButton;
        private BrightIdeasSoftware.OLVColumn timeoutColumn;
        private BrightIdeasSoftware.OLVColumn addressColumn;
    }
}