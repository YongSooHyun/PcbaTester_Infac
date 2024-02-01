namespace EOL_GND.View
{
    partial class StepEditForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(StepEditForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.saveTSButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.prevStepTSButton = new System.Windows.Forms.ToolStripButton();
            this.nextStepTSButton = new System.Windows.Forms.ToolStripButton();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.runCountComboBox = new System.Windows.Forms.ToolStripComboBox();
            this.runTSButton = new System.Windows.Forms.ToolStripButton();
            this.stopTSButton = new System.Windows.Forms.ToolStripButton();
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.stepCategoryLabel = new System.Windows.Forms.Label();
            this.stepPictureBox = new System.Windows.Forms.PictureBox();
            this.stepPropertyGrid = new System.Windows.Forms.PropertyGrid();
            this.gridContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.testResultListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.resultColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.startTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.finishTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.totalColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.measuredColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.valueStateColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.resultInfoColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.resultContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.clearToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mainStatusStrip = new System.Windows.Forms.StatusStrip();
            this.resultStatusLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.elapsedLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.infoLabel = new System.Windows.Forms.ToolStripStatusLabel();
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.stepPictureBox)).BeginInit();
            this.gridContextMenu.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.testResultListView)).BeginInit();
            this.resultContextMenu.SuspendLayout();
            this.mainStatusStrip.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(32, 32);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveTSButton,
            this.toolStripSeparator1,
            this.prevStepTSButton,
            this.nextStepTSButton,
            this.toolStripSeparator2,
            this.runCountComboBox,
            this.runTSButton,
            this.stopTSButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(1009, 39);
            this.mainToolStrip.TabIndex = 0;
            this.mainToolStrip.Text = "mainToolStrip";
            // 
            // saveTSButton
            // 
            this.saveTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveTSButton.Image = global::EOL_GND.Properties.Resources.save;
            this.saveTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveTSButton.Name = "saveTSButton";
            this.saveTSButton.Size = new System.Drawing.Size(36, 36);
            this.saveTSButton.Text = "Save (Ctrl+S)";
            this.saveTSButton.Click += new System.EventHandler(this.saveTSButton_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(6, 39);
            // 
            // prevStepTSButton
            // 
            this.prevStepTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.prevStepTSButton.Image = global::EOL_GND.Properties.Resources.arrow_left;
            this.prevStepTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.prevStepTSButton.Name = "prevStepTSButton";
            this.prevStepTSButton.Size = new System.Drawing.Size(36, 36);
            this.prevStepTSButton.Text = "Previous Test Step (Alt+Left Arrow)";
            this.prevStepTSButton.Click += new System.EventHandler(this.prevStepTSButton_Click);
            // 
            // nextStepTSButton
            // 
            this.nextStepTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.nextStepTSButton.Image = global::EOL_GND.Properties.Resources.arrow_right;
            this.nextStepTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.nextStepTSButton.Name = "nextStepTSButton";
            this.nextStepTSButton.Size = new System.Drawing.Size(36, 36);
            this.nextStepTSButton.Text = "Next Test Step (Alt+Right Arrow)";
            this.nextStepTSButton.Click += new System.EventHandler(this.nextStepTSButton_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(6, 39);
            // 
            // runCountComboBox
            // 
            this.runCountComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.runCountComboBox.FlatStyle = System.Windows.Forms.FlatStyle.Standard;
            this.runCountComboBox.Items.AddRange(new object[] {
            "1",
            "5",
            "10",
            "20",
            "30",
            "40",
            "50",
            "60",
            "70",
            "80",
            "90",
            "100"});
            this.runCountComboBox.Name = "runCountComboBox";
            this.runCountComboBox.Size = new System.Drawing.Size(75, 39);
            this.runCountComboBox.ToolTipText = "Number of Times to Run";
            // 
            // runTSButton
            // 
            this.runTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.runTSButton.Image = global::EOL_GND.Properties.Resources.play;
            this.runTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.runTSButton.Name = "runTSButton";
            this.runTSButton.Size = new System.Drawing.Size(36, 36);
            this.runTSButton.Text = "Run Test Step (F5)";
            this.runTSButton.Click += new System.EventHandler(this.runTSButton_Click);
            // 
            // stopTSButton
            // 
            this.stopTSButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.stopTSButton.Enabled = false;
            this.stopTSButton.Image = global::EOL_GND.Properties.Resources.stop;
            this.stopTSButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.stopTSButton.Name = "stopTSButton";
            this.stopTSButton.Size = new System.Drawing.Size(36, 36);
            this.stopTSButton.Text = "Stop Running Test (F6)";
            this.stopTSButton.Click += new System.EventHandler(this.stopTSButton_Click);
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 39);
            this.mainSplitContainer.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.mainSplitContainer.Panel1.Controls.Add(this.stepCategoryLabel);
            this.mainSplitContainer.Panel1.Controls.Add(this.stepPictureBox);
            this.mainSplitContainer.Panel1.Controls.Add(this.stepPropertyGrid);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.testResultListView);
            this.mainSplitContainer.Size = new System.Drawing.Size(1009, 614);
            this.mainSplitContainer.SplitterDistance = 487;
            this.mainSplitContainer.SplitterWidth = 3;
            this.mainSplitContainer.TabIndex = 1;
            // 
            // stepCategoryLabel
            // 
            this.stepCategoryLabel.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stepCategoryLabel.Font = new System.Drawing.Font("Segoe UI", 14F, System.Drawing.FontStyle.Bold);
            this.stepCategoryLabel.Location = new System.Drawing.Point(741, 297);
            this.stepCategoryLabel.Margin = new System.Windows.Forms.Padding(4, 0, 4, 0);
            this.stepCategoryLabel.Name = "stepCategoryLabel";
            this.stepCategoryLabel.Size = new System.Drawing.Size(255, 81);
            this.stepCategoryLabel.TabIndex = 2;
            this.stepCategoryLabel.TextAlign = System.Drawing.ContentAlignment.TopCenter;
            // 
            // stepPictureBox
            // 
            this.stepPictureBox.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.stepPictureBox.Location = new System.Drawing.Point(741, 37);
            this.stepPictureBox.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepPictureBox.Name = "stepPictureBox";
            this.stepPictureBox.Size = new System.Drawing.Size(255, 256);
            this.stepPictureBox.SizeMode = System.Windows.Forms.PictureBoxSizeMode.Zoom;
            this.stepPictureBox.TabIndex = 1;
            this.stepPictureBox.TabStop = false;
            this.stepPictureBox.DoubleClick += new System.EventHandler(this.stepPictureBox_DoubleClick);
            // 
            // stepPropertyGrid
            // 
            this.stepPropertyGrid.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stepPropertyGrid.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.stepPropertyGrid.ContextMenuStrip = this.gridContextMenu;
            this.stepPropertyGrid.Font = new System.Drawing.Font("Courier New", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.stepPropertyGrid.LineColor = System.Drawing.Color.LightSteelBlue;
            this.stepPropertyGrid.Location = new System.Drawing.Point(0, 0);
            this.stepPropertyGrid.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.stepPropertyGrid.Name = "stepPropertyGrid";
            this.stepPropertyGrid.PropertySort = System.Windows.Forms.PropertySort.Categorized;
            this.stepPropertyGrid.Size = new System.Drawing.Size(726, 487);
            this.stepPropertyGrid.TabIndex = 0;
            this.stepPropertyGrid.ToolbarVisible = false;
            // 
            // gridContextMenu
            // 
            this.gridContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resetToolStripMenuItem});
            this.gridContextMenu.Name = "contextMenu";
            this.gridContextMenu.Size = new System.Drawing.Size(103, 26);
            this.gridContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenu_Opening);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(102, 22);
            this.resetToolStripMenuItem.Text = "&Reset";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // testResultListView
            // 
            this.testResultListView.AllColumns.Add(this.noColumn);
            this.testResultListView.AllColumns.Add(this.resultColumn);
            this.testResultListView.AllColumns.Add(this.startTimeColumn);
            this.testResultListView.AllColumns.Add(this.finishTimeColumn);
            this.testResultListView.AllColumns.Add(this.totalColumn);
            this.testResultListView.AllColumns.Add(this.measuredColumn);
            this.testResultListView.AllColumns.Add(this.valueStateColumn);
            this.testResultListView.AllColumns.Add(this.resultInfoColumn);
            this.testResultListView.AllowColumnReorder = true;
            this.testResultListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.testResultListView.CellEditUseWholeCell = false;
            this.testResultListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.resultColumn,
            this.startTimeColumn,
            this.finishTimeColumn,
            this.totalColumn,
            this.measuredColumn,
            this.valueStateColumn,
            this.resultInfoColumn});
            this.testResultListView.ContextMenuStrip = this.resultContextMenu;
            this.testResultListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.testResultListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.testResultListView.FullRowSelect = true;
            this.testResultListView.GridLines = true;
            this.testResultListView.HideSelection = false;
            this.testResultListView.Location = new System.Drawing.Point(0, 0);
            this.testResultListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.testResultListView.MultiSelect = false;
            this.testResultListView.Name = "testResultListView";
            this.testResultListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.testResultListView.ShowCommandMenuOnRightClick = true;
            this.testResultListView.ShowGroups = false;
            this.testResultListView.ShowItemToolTips = true;
            this.testResultListView.Size = new System.Drawing.Size(1009, 124);
            this.testResultListView.TabIndex = 0;
            this.testResultListView.UseCellFormatEvents = true;
            this.testResultListView.UseCompatibleStateImageBehavior = false;
            this.testResultListView.UseFilterIndicator = true;
            this.testResultListView.UseFiltering = true;
            this.testResultListView.UseNotifyPropertyChanged = true;
            this.testResultListView.View = System.Windows.Forms.View.Details;
            this.testResultListView.VirtualMode = true;
            this.testResultListView.FormatCell += new System.EventHandler<BrightIdeasSoftware.FormatCellEventArgs>(this.testResultListView_FormatCell);
            this.testResultListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.testResultListView_FormatRow);
            // 
            // noColumn
            // 
            this.noColumn.Groupable = false;
            this.noColumn.Sortable = false;
            this.noColumn.Text = "NO";
            this.noColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.noColumn.UseFiltering = false;
            this.noColumn.Width = 40;
            // 
            // resultColumn
            // 
            this.resultColumn.AspectName = "ResultStateDesc";
            this.resultColumn.Text = "Result";
            this.resultColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.resultColumn.Width = 100;
            // 
            // startTimeColumn
            // 
            this.startTimeColumn.AspectName = "StartTime";
            this.startTimeColumn.AspectToStringFormat = "{0:HH\\:mm\\:ss.fff}";
            this.startTimeColumn.Text = "Start Time";
            this.startTimeColumn.Width = 100;
            // 
            // finishTimeColumn
            // 
            this.finishTimeColumn.AspectName = "FinishTime";
            this.finishTimeColumn.AspectToStringFormat = "{0:HH\\:mm\\:ss.fff}";
            this.finishTimeColumn.Text = "Finish Time";
            this.finishTimeColumn.Width = 100;
            // 
            // totalColumn
            // 
            this.totalColumn.AspectName = "TotalMilliseconds";
            this.totalColumn.Text = "Total (ms)";
            this.totalColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.totalColumn.Width = 75;
            // 
            // measuredColumn
            // 
            this.measuredColumn.AspectName = "ResultValueDesc";
            this.measuredColumn.Text = "Measured";
            this.measuredColumn.Width = 150;
            // 
            // valueStateColumn
            // 
            this.valueStateColumn.AspectName = "ResultValueStateDesc";
            this.valueStateColumn.Text = "Value State";
            this.valueStateColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.valueStateColumn.Width = 75;
            // 
            // resultInfoColumn
            // 
            this.resultInfoColumn.AspectName = "ResultInfo";
            this.resultInfoColumn.Text = "Result Info";
            this.resultInfoColumn.Width = 340;
            // 
            // resultContextMenu
            // 
            this.resultContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.clearToolStripMenuItem});
            this.resultContextMenu.Name = "resultContextMenu";
            this.resultContextMenu.Size = new System.Drawing.Size(102, 26);
            this.resultContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.resultContextMenu_Opening);
            // 
            // clearToolStripMenuItem
            // 
            this.clearToolStripMenuItem.Name = "clearToolStripMenuItem";
            this.clearToolStripMenuItem.Size = new System.Drawing.Size(101, 22);
            this.clearToolStripMenuItem.Text = "&Clear";
            this.clearToolStripMenuItem.Click += new System.EventHandler(this.clearToolStripMenuItem_Click);
            // 
            // mainStatusStrip
            // 
            this.mainStatusStrip.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(240)))), ((int)(((byte)(240)))), ((int)(((byte)(240)))));
            this.mainStatusStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.resultStatusLabel,
            this.elapsedLabel,
            this.infoLabel});
            this.mainStatusStrip.Location = new System.Drawing.Point(0, 653);
            this.mainStatusStrip.Name = "mainStatusStrip";
            this.mainStatusStrip.Size = new System.Drawing.Size(1009, 22);
            this.mainStatusStrip.TabIndex = 2;
            this.mainStatusStrip.Text = "mainStatusStrip";
            // 
            // resultStatusLabel
            // 
            this.resultStatusLabel.AutoSize = false;
            this.resultStatusLabel.BackColor = System.Drawing.Color.Silver;
            this.resultStatusLabel.Margin = new System.Windows.Forms.Padding(0, 1, 0, 1);
            this.resultStatusLabel.Name = "resultStatusLabel";
            this.resultStatusLabel.Size = new System.Drawing.Size(100, 20);
            // 
            // elapsedLabel
            // 
            this.elapsedLabel.Font = new System.Drawing.Font("Courier New", 9F);
            this.elapsedLabel.Margin = new System.Windows.Forms.Padding(6, 1, 0, 1);
            this.elapsedLabel.Name = "elapsedLabel";
            this.elapsedLabel.Size = new System.Drawing.Size(0, 20);
            // 
            // infoLabel
            // 
            this.infoLabel.Margin = new System.Windows.Forms.Padding(6, 1, 0, 1);
            this.infoLabel.Name = "infoLabel";
            this.infoLabel.Size = new System.Drawing.Size(0, 20);
            // 
            // StepEditForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1009, 675);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.mainToolStrip);
            this.Controls.Add(this.mainStatusStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "StepEditForm";
            this.Text = "Edit Test Step";
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.stepPictureBox)).EndInit();
            this.gridContextMenu.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.testResultListView)).EndInit();
            this.resultContextMenu.ResumeLayout(false);
            this.mainStatusStrip.ResumeLayout(false);
            this.mainStatusStrip.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private System.Windows.Forms.StatusStrip mainStatusStrip;
        private System.Windows.Forms.PropertyGrid stepPropertyGrid;
        private System.Windows.Forms.PictureBox stepPictureBox;
        private System.Windows.Forms.Label stepCategoryLabel;
        private BrightIdeasSoftware.FastObjectListView testResultListView;
        private System.Windows.Forms.ToolStripButton saveTSButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripButton prevStepTSButton;
        private System.Windows.Forms.ToolStripButton nextStepTSButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripButton runTSButton;
        private System.Windows.Forms.ToolStripButton stopTSButton;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn startTimeColumn;
        private BrightIdeasSoftware.OLVColumn finishTimeColumn;
        private BrightIdeasSoftware.OLVColumn totalColumn;
        private BrightIdeasSoftware.OLVColumn resultColumn;
        private BrightIdeasSoftware.OLVColumn measuredColumn;
        private BrightIdeasSoftware.OLVColumn valueStateColumn;
        private BrightIdeasSoftware.OLVColumn resultInfoColumn;
        private System.Windows.Forms.ToolStripStatusLabel resultStatusLabel;
        private System.Windows.Forms.ContextMenuStrip gridContextMenu;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip resultContextMenu;
        private System.Windows.Forms.ToolStripMenuItem clearToolStripMenuItem;
        private System.Windows.Forms.ToolStripStatusLabel infoLabel;
        private System.Windows.Forms.ToolStripComboBox runCountComboBox;
        private System.Windows.Forms.ToolStripStatusLabel elapsedLabel;
    }
}