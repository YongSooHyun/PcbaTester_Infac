namespace EOL_GND.View
{
    partial class ChangeHistoryForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(ChangeHistoryForm));
            this.historyListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.timeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.userColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.versionColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.remarksColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.historyContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.differenceItem = new System.Windows.Forms.ToolStripMenuItem();
            this.initialVersionItem = new System.Windows.Forms.ToolStripMenuItem();
            this.mergeItem = new System.Windows.Forms.ToolStripMenuItem();
            this.newestItem = new System.Windows.Forms.ToolStripMenuItem();
            this.oldestItem = new System.Windows.Forms.ToolStripMenuItem();
            ((System.ComponentModel.ISupportInitialize)(this.historyListView)).BeginInit();
            this.historyContextMenu.SuspendLayout();
            this.SuspendLayout();
            // 
            // historyListView
            // 
            this.historyListView.AllColumns.Add(this.noColumn);
            this.historyListView.AllColumns.Add(this.timeColumn);
            this.historyListView.AllColumns.Add(this.userColumn);
            this.historyListView.AllColumns.Add(this.versionColumn);
            this.historyListView.AllColumns.Add(this.remarksColumn);
            this.historyListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.historyListView.CellEditUseWholeCell = false;
            this.historyListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.timeColumn,
            this.userColumn,
            this.versionColumn,
            this.remarksColumn});
            this.historyListView.ContextMenuStrip = this.historyContextMenu;
            this.historyListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.historyListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.historyListView.FullRowSelect = true;
            this.historyListView.GridLines = true;
            this.historyListView.HideSelection = false;
            this.historyListView.Location = new System.Drawing.Point(0, 0);
            this.historyListView.Name = "historyListView";
            this.historyListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.historyListView.ShowCommandMenuOnRightClick = true;
            this.historyListView.ShowGroups = false;
            this.historyListView.ShowItemCountOnGroups = true;
            this.historyListView.ShowItemToolTips = true;
            this.historyListView.Size = new System.Drawing.Size(735, 592);
            this.historyListView.TabIndex = 3;
            this.historyListView.UseCompatibleStateImageBehavior = false;
            this.historyListView.UseFilterIndicator = true;
            this.historyListView.UseFiltering = true;
            this.historyListView.View = System.Windows.Forms.View.Details;
            this.historyListView.VirtualMode = true;
            this.historyListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.historyListView_FormatRow);
            this.historyListView.ItemActivate += new System.EventHandler(this.historyListView_ItemActivate);
            // 
            // noColumn
            // 
            this.noColumn.Groupable = false;
            this.noColumn.IsEditable = false;
            this.noColumn.Sortable = false;
            this.noColumn.Text = "NO";
            this.noColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.noColumn.UseFiltering = false;
            this.noColumn.Width = 50;
            // 
            // timeColumn
            // 
            this.timeColumn.AspectName = "ModificationTime";
            this.timeColumn.Text = "변경 시간";
            this.timeColumn.Width = 150;
            // 
            // userColumn
            // 
            this.userColumn.AspectName = "UserInfo";
            this.userColumn.AspectToStringFormat = "";
            this.userColumn.Text = "사용자";
            this.userColumn.Width = 200;
            // 
            // versionColumn
            // 
            this.versionColumn.AspectName = "EditorVersion";
            this.versionColumn.Text = "프로그램 버전";
            this.versionColumn.Width = 120;
            // 
            // remarksColumn
            // 
            this.remarksColumn.AspectName = "Remarks";
            this.remarksColumn.Text = "설명";
            this.remarksColumn.Width = 200;
            // 
            // historyContextMenu
            // 
            this.historyContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.differenceItem,
            this.initialVersionItem,
            this.mergeItem});
            this.historyContextMenu.Name = "historyContextMenu";
            this.historyContextMenu.Size = new System.Drawing.Size(212, 70);
            this.historyContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.historyContextMenu_Opening);
            // 
            // differenceItem
            // 
            this.differenceItem.Name = "differenceItem";
            this.differenceItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.D)));
            this.differenceItem.Size = new System.Drawing.Size(211, 22);
            this.differenceItem.Text = "Display &Difference";
            this.differenceItem.ToolTipText = "선택한 2개의 버전의 차이를 보여줍니다.\r\n하나만 선택한 경우 초기 버전과 선택한 버전의 차이를 보여줍니다.";
            this.differenceItem.Click += new System.EventHandler(this.differenceItem_Click);
            // 
            // initialVersionItem
            // 
            this.initialVersionItem.Name = "initialVersionItem";
            this.initialVersionItem.Size = new System.Drawing.Size(211, 22);
            this.initialVersionItem.Text = "Display &Initial Version";
            this.initialVersionItem.ToolTipText = "초기 버전을 보여줍니다.";
            this.initialVersionItem.Click += new System.EventHandler(this.initialVersionItem_Click);
            // 
            // mergeItem
            // 
            this.mergeItem.DropDownItems.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.newestItem,
            this.oldestItem});
            this.mergeItem.Name = "mergeItem";
            this.mergeItem.Size = new System.Drawing.Size(211, 22);
            this.mergeItem.Text = "&Merge Selected Records";
            this.mergeItem.ToolTipText = "선택한 연속되는 레코드들을 하나로 병합합니다.";
            // 
            // newestItem
            // 
            this.newestItem.Name = "newestItem";
            this.newestItem.Size = new System.Drawing.Size(180, 22);
            this.newestItem.Text = "Into &Newest";
            this.newestItem.Click += new System.EventHandler(this.newestItem_Click);
            // 
            // oldestItem
            // 
            this.oldestItem.Name = "oldestItem";
            this.oldestItem.Size = new System.Drawing.Size(180, 22);
            this.oldestItem.Text = "Into &Oldest";
            this.oldestItem.Click += new System.EventHandler(this.oldestItem_Click);
            // 
            // ChangeHistoryForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(735, 592);
            this.Controls.Add(this.historyListView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "ChangeHistoryForm";
            this.Text = "Change History";
            ((System.ComponentModel.ISupportInitialize)(this.historyListView)).EndInit();
            this.historyContextMenu.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView historyListView;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn userColumn;
        private BrightIdeasSoftware.OLVColumn timeColumn;
        private BrightIdeasSoftware.OLVColumn versionColumn;
        private System.Windows.Forms.ContextMenuStrip historyContextMenu;
        private System.Windows.Forms.ToolStripMenuItem differenceItem;
        private System.Windows.Forms.ToolStripMenuItem initialVersionItem;
        private BrightIdeasSoftware.OLVColumn remarksColumn;
        private System.Windows.Forms.ToolStripMenuItem mergeItem;
        private System.Windows.Forms.ToolStripMenuItem newestItem;
        private System.Windows.Forms.ToolStripMenuItem oldestItem;
    }
}