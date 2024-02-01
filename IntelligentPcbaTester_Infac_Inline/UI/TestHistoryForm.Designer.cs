namespace IntelligentPcbaTester
{
    partial class TestHistoryForm
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
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle1 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle2 = new BrightIdeasSoftware.HeaderStateStyle();
            BrightIdeasSoftware.HeaderStateStyle headerStateStyle3 = new BrightIdeasSoftware.HeaderStateStyle();
            this.historyListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.groupDateColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.projectColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.fidColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.ictProjectColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.modelColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.snColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.resultColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.startTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.finishTimeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.testDurationColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.elozDurationColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.totalDurationColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.board1LogFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.board2LogFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.board3LogFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.board4LogFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.printLogFileColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.contextMenuStrip1 = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.deleteToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.resetToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.showGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.collapseGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.expandGroupsToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.headerFormatStyle1 = new BrightIdeasSoftware.HeaderFormatStyle();
            ((System.ComponentModel.ISupportInitialize)(this.historyListView)).BeginInit();
            this.contextMenuStrip1.SuspendLayout();
            this.SuspendLayout();
            // 
            // historyListView
            // 
            this.historyListView.AllColumns.Add(this.noColumn);
            this.historyListView.AllColumns.Add(this.groupDateColumn);
            this.historyListView.AllColumns.Add(this.projectColumn);
            this.historyListView.AllColumns.Add(this.fidColumn);
            this.historyListView.AllColumns.Add(this.ictProjectColumn);
            this.historyListView.AllColumns.Add(this.modelColumn);
            this.historyListView.AllColumns.Add(this.snColumn);
            this.historyListView.AllColumns.Add(this.resultColumn);
            this.historyListView.AllColumns.Add(this.startTimeColumn);
            this.historyListView.AllColumns.Add(this.finishTimeColumn);
            this.historyListView.AllColumns.Add(this.testDurationColumn);
            this.historyListView.AllColumns.Add(this.elozDurationColumn);
            this.historyListView.AllColumns.Add(this.totalDurationColumn);
            this.historyListView.AllColumns.Add(this.board1LogFileColumn);
            this.historyListView.AllColumns.Add(this.board2LogFileColumn);
            this.historyListView.AllColumns.Add(this.board3LogFileColumn);
            this.historyListView.AllColumns.Add(this.board4LogFileColumn);
            this.historyListView.AllColumns.Add(this.printLogFileColumn);
            this.historyListView.AllowColumnReorder = true;
            this.historyListView.CellEditUseWholeCell = false;
            this.historyListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.groupDateColumn,
            this.projectColumn,
            this.fidColumn,
            this.ictProjectColumn,
            this.modelColumn,
            this.snColumn,
            this.resultColumn,
            this.startTimeColumn,
            this.finishTimeColumn,
            this.testDurationColumn,
            this.elozDurationColumn,
            this.totalDurationColumn,
            this.board1LogFileColumn,
            this.board2LogFileColumn,
            this.board3LogFileColumn,
            this.board4LogFileColumn,
            this.printLogFileColumn});
            this.historyListView.ContextMenuStrip = this.contextMenuStrip1;
            this.historyListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.historyListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyListView.FullRowSelect = true;
            this.historyListView.GridLines = true;
            this.historyListView.HeaderFormatStyle = this.headerFormatStyle1;
            this.historyListView.HideSelection = false;
            this.historyListView.Location = new System.Drawing.Point(0, 0);
            this.historyListView.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.historyListView.Name = "historyListView";
            this.historyListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.historyListView.ShowCommandMenuOnRightClick = true;
            this.historyListView.ShowGroups = false;
            this.historyListView.ShowItemToolTips = true;
            this.historyListView.Size = new System.Drawing.Size(1444, 724);
            this.historyListView.TabIndex = 0;
            this.historyListView.UseCompatibleStateImageBehavior = false;
            this.historyListView.UseFilterIndicator = true;
            this.historyListView.UseFiltering = true;
            this.historyListView.View = System.Windows.Forms.View.Details;
            this.historyListView.VirtualMode = true;
            this.historyListView.CellClick += new System.EventHandler<BrightIdeasSoftware.CellClickEventArgs>(this.historyListView_CellClick);
            // 
            // noColumn
            // 
            this.noColumn.AspectName = "RowIndex";
            this.noColumn.Groupable = false;
            this.noColumn.Text = "NO";
            this.noColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.noColumn.UseFiltering = false;
            this.noColumn.Width = 50;
            // 
            // groupDateColumn
            // 
            this.groupDateColumn.AspectName = "GroupDate";
            this.groupDateColumn.AspectToStringFormat = "";
            this.groupDateColumn.Text = "날짜";
            this.groupDateColumn.Width = 140;
            // 
            // projectColumn
            // 
            this.projectColumn.AspectName = "ProjectName";
            this.projectColumn.Text = "프로젝트";
            this.projectColumn.Width = 135;
            // 
            // fidColumn
            // 
            this.fidColumn.AspectName = "FixtureId";
            this.fidColumn.Text = "FID";
            this.fidColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.fidColumn.Width = 30;
            // 
            // ictProjectColumn
            // 
            this.ictProjectColumn.AspectName = "IctProject";
            this.ictProjectColumn.Text = "ICT 프로젝트";
            this.ictProjectColumn.Width = 135;
            // 
            // modelColumn
            // 
            this.modelColumn.AspectName = "Model";
            this.modelColumn.Text = "MODEL";
            this.modelColumn.Width = 80;
            // 
            // snColumn
            // 
            this.snColumn.AspectName = "SerialNumber";
            this.snColumn.Text = "S/N";
            this.snColumn.Width = 80;
            // 
            // resultColumn
            // 
            this.resultColumn.AspectName = "ResultText";
            this.resultColumn.Text = "결과";
            this.resultColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.resultColumn.Width = 40;
            // 
            // startTimeColumn
            // 
            this.startTimeColumn.AspectName = "StartTime";
            this.startTimeColumn.Text = "시작시간";
            this.startTimeColumn.Width = 140;
            // 
            // finishTimeColumn
            // 
            this.finishTimeColumn.AspectName = "FinishTime";
            this.finishTimeColumn.Text = "종료시간";
            this.finishTimeColumn.Width = 140;
            // 
            // testDurationColumn
            // 
            this.testDurationColumn.AspectName = "TestDuration";
            this.testDurationColumn.Text = "전체시간";
            this.testDurationColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.testDurationColumn.Width = 65;
            // 
            // elozDurationColumn
            // 
            this.elozDurationColumn.AspectName = "ElozDuration";
            this.elozDurationColumn.Text = "eloZ시간";
            this.elozDurationColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.elozDurationColumn.Width = 65;
            // 
            // totalDurationColumn
            // 
            this.totalDurationColumn.AspectName = "TotalDuration";
            this.totalDurationColumn.Text = "테스트시간";
            this.totalDurationColumn.Width = 75;
            // 
            // board1LogFileColumn
            // 
            this.board1LogFileColumn.AspectName = "Board1LogFile";
            this.board1LogFileColumn.Text = "Board1 로그";
            this.board1LogFileColumn.Width = 120;
            // 
            // board2LogFileColumn
            // 
            this.board2LogFileColumn.AspectName = "Board2LogFile";
            this.board2LogFileColumn.Text = "Board2 로그";
            this.board2LogFileColumn.Width = 80;
            // 
            // board3LogFileColumn
            // 
            this.board3LogFileColumn.AspectName = "Board3LogFile";
            this.board3LogFileColumn.Text = "Board3 로그";
            this.board3LogFileColumn.Width = 80;
            // 
            // board4LogFileColumn
            // 
            this.board4LogFileColumn.AspectName = "Board4LogFile";
            this.board4LogFileColumn.Text = "Board4 로그";
            this.board4LogFileColumn.Width = 80;
            // 
            // printLogFileColumn
            // 
            this.printLogFileColumn.AspectName = "PrintLogFile";
            this.printLogFileColumn.Text = "Print 로그";
            this.printLogFileColumn.Width = 120;
            // 
            // contextMenuStrip1
            // 
            this.contextMenuStrip1.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.deleteToolStripMenuItem,
            this.resetToolStripMenuItem,
            this.toolStripSeparator1,
            this.showGroupsToolStripMenuItem,
            this.collapseGroupsToolStripMenuItem,
            this.expandGroupsToolStripMenuItem});
            this.contextMenuStrip1.Name = "contextMenuStrip1";
            this.contextMenuStrip1.Size = new System.Drawing.Size(187, 120);
            this.contextMenuStrip1.Opening += new System.ComponentModel.CancelEventHandler(this.contextMenuStrip1_Opening);
            // 
            // deleteToolStripMenuItem
            // 
            this.deleteToolStripMenuItem.Name = "deleteToolStripMenuItem";
            this.deleteToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.deleteToolStripMenuItem.Text = "&Delete...";
            this.deleteToolStripMenuItem.Click += new System.EventHandler(this.deleteToolStripMenuItem_Click);
            // 
            // resetToolStripMenuItem
            // 
            this.resetToolStripMenuItem.Name = "resetToolStripMenuItem";
            this.resetToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.R)));
            this.resetToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.resetToolStripMenuItem.Text = "&Reset...";
            this.resetToolStripMenuItem.Click += new System.EventHandler(this.resetToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(183, 6);
            // 
            // showGroupsToolStripMenuItem
            // 
            this.showGroupsToolStripMenuItem.Name = "showGroupsToolStripMenuItem";
            this.showGroupsToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.G)));
            this.showGroupsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.showGroupsToolStripMenuItem.Text = "Show &Groups";
            this.showGroupsToolStripMenuItem.Click += new System.EventHandler(this.showGroupsToolStripMenuItem_Click);
            // 
            // collapseGroupsToolStripMenuItem
            // 
            this.collapseGroupsToolStripMenuItem.Name = "collapseGroupsToolStripMenuItem";
            this.collapseGroupsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.collapseGroupsToolStripMenuItem.Text = "&Collapse Groups";
            this.collapseGroupsToolStripMenuItem.Click += new System.EventHandler(this.collapseGroupsToolStripMenuItem_Click);
            // 
            // expandGroupsToolStripMenuItem
            // 
            this.expandGroupsToolStripMenuItem.Name = "expandGroupsToolStripMenuItem";
            this.expandGroupsToolStripMenuItem.Size = new System.Drawing.Size(186, 22);
            this.expandGroupsToolStripMenuItem.Text = "&Expand Groups";
            this.expandGroupsToolStripMenuItem.Click += new System.EventHandler(this.expandGroupsToolStripMenuItem_Click);
            // 
            // headerFormatStyle1
            // 
            this.headerFormatStyle1.Hot = headerStateStyle1;
            this.headerFormatStyle1.Normal = headerStateStyle2;
            this.headerFormatStyle1.Pressed = headerStateStyle3;
            // 
            // TestHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(1444, 724);
            this.Controls.Add(this.historyListView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "TestHistoryForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "검사 이력";
            ((System.ComponentModel.ISupportInitialize)(this.historyListView)).EndInit();
            this.contextMenuStrip1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView historyListView;
        private BrightIdeasSoftware.OLVColumn projectColumn;
        private BrightIdeasSoftware.OLVColumn ictProjectColumn;
        private BrightIdeasSoftware.OLVColumn modelColumn;
        private BrightIdeasSoftware.OLVColumn snColumn;
        private BrightIdeasSoftware.OLVColumn resultColumn;
        private BrightIdeasSoftware.OLVColumn startTimeColumn;
        private BrightIdeasSoftware.OLVColumn finishTimeColumn;
        private BrightIdeasSoftware.OLVColumn board1LogFileColumn;
        private BrightIdeasSoftware.OLVColumn printLogFileColumn;
        private BrightIdeasSoftware.HeaderFormatStyle headerFormatStyle1;
        private BrightIdeasSoftware.OLVColumn testDurationColumn;
        private BrightIdeasSoftware.OLVColumn elozDurationColumn;
        private System.Windows.Forms.ContextMenuStrip contextMenuStrip1;
        private System.Windows.Forms.ToolStripMenuItem deleteToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem showGroupsToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn groupDateColumn;
        private BrightIdeasSoftware.OLVColumn fidColumn;
        private System.Windows.Forms.ToolStripMenuItem resetToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn board2LogFileColumn;
        private BrightIdeasSoftware.OLVColumn board3LogFileColumn;
        private BrightIdeasSoftware.OLVColumn board4LogFileColumn;
        private BrightIdeasSoftware.OLVColumn totalDurationColumn;
        private System.Windows.Forms.ToolStripMenuItem collapseGroupsToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem expandGroupsToolStripMenuItem;
        private BrightIdeasSoftware.OLVColumn noColumn;
    }
}