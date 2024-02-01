namespace EOL_GND.View
{
    partial class DbcEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DbcEditorForm));
            this.mainSplitContainer = new System.Windows.Forms.SplitContainer();
            this.messageListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.idColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.nameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.dlcColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.cycleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.messageContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addMessageToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.copyMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteMessagesToolStripMenuItem = new System.Windows.Forms.ToolStripMenuItem();
            this.messageToolStrip = new System.Windows.Forms.ToolStrip();
            this.messageTitleLabel = new System.Windows.Forms.ToolStripLabel();
            this.messageSaveButton = new System.Windows.Forms.ToolStripButton();
            this.messageSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.messageAddButton = new System.Windows.Forms.ToolStripButton();
            this.messageDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.messageSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.messageCopyButton = new System.Windows.Forms.ToolStripButton();
            this.messagePasteButton = new System.Windows.Forms.ToolStripButton();
            this.signalListView = new BrightIdeasSoftware.FastObjectListView();
            this.sigNoColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.sigNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.startBitColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.lengthColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.byteOrderColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.signedColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.factorColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.offsetColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.minimumColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.maximumColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.unitColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.errorIDColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.signalContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSignalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSignalsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator2 = new System.Windows.Forms.ToolStripSeparator();
            this.copySignalsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.pasteSignalsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signalToolStrip = new System.Windows.Forms.ToolStrip();
            this.signalTitleLabel = new System.Windows.Forms.ToolStripLabel();
            this.signalAddButton = new System.Windows.Forms.ToolStripButton();
            this.signalDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.signalSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.signalCopyButton = new System.Windows.Forms.ToolStripButton();
            this.signalPasteButton = new System.Windows.Forms.ToolStripButton();
            this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).BeginInit();
            this.mainSplitContainer.Panel1.SuspendLayout();
            this.mainSplitContainer.Panel2.SuspendLayout();
            this.mainSplitContainer.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.messageListView)).BeginInit();
            this.messageContextMenu.SuspendLayout();
            this.messageToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalListView)).BeginInit();
            this.signalContextMenu.SuspendLayout();
            this.signalToolStrip.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // mainSplitContainer
            // 
            this.mainSplitContainer.Dock = System.Windows.Forms.DockStyle.Fill;
            this.mainSplitContainer.Location = new System.Drawing.Point(0, 0);
            this.mainSplitContainer.Name = "mainSplitContainer";
            this.mainSplitContainer.Orientation = System.Windows.Forms.Orientation.Horizontal;
            // 
            // mainSplitContainer.Panel1
            // 
            this.mainSplitContainer.Panel1.Controls.Add(this.messageListView);
            this.mainSplitContainer.Panel1.Controls.Add(this.messageToolStrip);
            // 
            // mainSplitContainer.Panel2
            // 
            this.mainSplitContainer.Panel2.Controls.Add(this.signalListView);
            this.mainSplitContainer.Panel2.Controls.Add(this.signalToolStrip);
            this.mainSplitContainer.Size = new System.Drawing.Size(882, 617);
            this.mainSplitContainer.SplitterDistance = 293;
            this.mainSplitContainer.TabIndex = 0;
            this.mainSplitContainer.TabStop = false;
            // 
            // messageListView
            // 
            this.messageListView.AllColumns.Add(this.noColumn);
            this.messageListView.AllColumns.Add(this.idColumn);
            this.messageListView.AllColumns.Add(this.nameColumn);
            this.messageListView.AllColumns.Add(this.dlcColumn);
            this.messageListView.AllColumns.Add(this.cycleColumn);
            this.messageListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.messageListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.messageListView.CellEditUseWholeCell = false;
            this.messageListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.idColumn,
            this.nameColumn,
            this.dlcColumn,
            this.cycleColumn});
            this.messageListView.ContextMenuStrip = this.messageContextMenu;
            this.messageListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.messageListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.messageListView.FullRowSelect = true;
            this.messageListView.GridLines = true;
            this.messageListView.HideSelection = false;
            this.messageListView.Location = new System.Drawing.Point(0, 31);
            this.messageListView.Name = "messageListView";
            this.messageListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.messageListView.ShowCommandMenuOnRightClick = true;
            this.messageListView.ShowGroups = false;
            this.messageListView.ShowItemCountOnGroups = true;
            this.messageListView.ShowItemToolTips = true;
            this.messageListView.Size = new System.Drawing.Size(882, 262);
            this.messageListView.TabIndex = 1;
            this.messageListView.UseCompatibleStateImageBehavior = false;
            this.messageListView.UseFilterIndicator = true;
            this.messageListView.UseFiltering = true;
            this.messageListView.View = System.Windows.Forms.View.Details;
            this.messageListView.VirtualMode = true;
            this.messageListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.messageListView_CellEditFinishing);
            this.messageListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.messageListView_CellEditStarting);
            this.messageListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.messageListView_FormatRow);
            this.messageListView.SelectionChanged += new System.EventHandler(this.messageListView_SelectionChanged);
            this.messageListView.ItemActivate += new System.EventHandler(this.messageListView_ItemActivate);
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
            // idColumn
            // 
            this.idColumn.AspectName = "ID";
            this.idColumn.AspectToStringFormat = "0x{0:X}";
            this.idColumn.Text = "ID";
            this.idColumn.Width = 90;
            // 
            // nameColumn
            // 
            this.nameColumn.AspectName = "Name";
            this.nameColumn.Text = "Name";
            this.nameColumn.Width = 180;
            // 
            // dlcColumn
            // 
            this.dlcColumn.AspectName = "DLC";
            this.dlcColumn.Text = "DLC";
            this.dlcColumn.Width = 65;
            // 
            // cycleColumn
            // 
            this.cycleColumn.AspectName = "CycleTime";
            this.cycleColumn.Text = "CycleTime(ms)";
            this.cycleColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.cycleColumn.Width = 100;
            // 
            // messageContextMenu
            // 
            this.messageContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addMessageToolStripMenuItem,
            this.deleteMessagesToolStripMenuItem,
            this.toolStripSeparator1,
            this.copyMessagesToolStripMenuItem,
            this.pasteMessagesToolStripMenuItem});
            this.messageContextMenu.Name = "messageContextMenu";
            this.messageContextMenu.Size = new System.Drawing.Size(199, 98);
            this.messageContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.messageContextMenu_Opening);
            // 
            // addMessageToolStripMenuItem
            // 
            this.addMessageToolStripMenuItem.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.addMessageToolStripMenuItem.Name = "addMessageToolStripMenuItem";
            this.addMessageToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addMessageToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.addMessageToolStripMenuItem.Text = "&Add Message";
            this.addMessageToolStripMenuItem.Click += new System.EventHandler(this.addMessageToolStripMenuItem_Click);
            // 
            // deleteMessagesToolStripMenuItem
            // 
            this.deleteMessagesToolStripMenuItem.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.deleteMessagesToolStripMenuItem.Name = "deleteMessagesToolStripMenuItem";
            this.deleteMessagesToolStripMenuItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteMessagesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.deleteMessagesToolStripMenuItem.Text = "&Delete Messages";
            this.deleteMessagesToolStripMenuItem.Click += new System.EventHandler(this.deleteMessagesToolStripMenuItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(195, 6);
            // 
            // copyMessagesToolStripMenuItem
            // 
            this.copyMessagesToolStripMenuItem.Image = global::EOL_GND.Properties.Resources.copy_32;
            this.copyMessagesToolStripMenuItem.Name = "copyMessagesToolStripMenuItem";
            this.copyMessagesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copyMessagesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.copyMessagesToolStripMenuItem.Text = "&Copy Messages";
            this.copyMessagesToolStripMenuItem.Click += new System.EventHandler(this.copyMessagesToolStripMenuItem_Click);
            // 
            // pasteMessagesToolStripMenuItem
            // 
            this.pasteMessagesToolStripMenuItem.Image = global::EOL_GND.Properties.Resources.paste_32;
            this.pasteMessagesToolStripMenuItem.Name = "pasteMessagesToolStripMenuItem";
            this.pasteMessagesToolStripMenuItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteMessagesToolStripMenuItem.Size = new System.Drawing.Size(198, 22);
            this.pasteMessagesToolStripMenuItem.Text = "&Paste Messages";
            this.pasteMessagesToolStripMenuItem.Click += new System.EventHandler(this.pasteMessagesToolStripMenuItem_Click);
            // 
            // messageToolStrip
            // 
            this.messageToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.messageToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.messageTitleLabel,
            this.messageSaveButton,
            this.messageSeparator1,
            this.messageAddButton,
            this.messageDeleteButton,
            this.messageSeparator2,
            this.messageCopyButton,
            this.messagePasteButton});
            this.messageToolStrip.Location = new System.Drawing.Point(0, 0);
            this.messageToolStrip.Name = "messageToolStrip";
            this.messageToolStrip.Size = new System.Drawing.Size(882, 31);
            this.messageToolStrip.TabIndex = 0;
            this.messageToolStrip.Text = "messageToolStrip";
            // 
            // messageTitleLabel
            // 
            this.messageTitleLabel.Name = "messageTitleLabel";
            this.messageTitleLabel.Size = new System.Drawing.Size(61, 28);
            this.messageTitleLabel.Text = "Messages:";
            // 
            // messageSaveButton
            // 
            this.messageSaveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messageSaveButton.Image = global::EOL_GND.Properties.Resources.save_32;
            this.messageSaveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messageSaveButton.Name = "messageSaveButton";
            this.messageSaveButton.Size = new System.Drawing.Size(28, 28);
            this.messageSaveButton.Text = "Save (Ctrl + S)";
            this.messageSaveButton.Click += new System.EventHandler(this.messageSaveButton_Click);
            // 
            // messageSeparator1
            // 
            this.messageSeparator1.Name = "messageSeparator1";
            this.messageSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // messageAddButton
            // 
            this.messageAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messageAddButton.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.messageAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messageAddButton.Name = "messageAddButton";
            this.messageAddButton.Size = new System.Drawing.Size(28, 28);
            this.messageAddButton.Text = "Add Message (Ctrl + N)";
            this.messageAddButton.Click += new System.EventHandler(this.messageAddButton_Click);
            // 
            // messageDeleteButton
            // 
            this.messageDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messageDeleteButton.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.messageDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messageDeleteButton.Name = "messageDeleteButton";
            this.messageDeleteButton.Size = new System.Drawing.Size(28, 28);
            this.messageDeleteButton.Text = "Delete Messages (Del)";
            this.messageDeleteButton.Click += new System.EventHandler(this.messageDeleteButton_Click);
            // 
            // messageSeparator2
            // 
            this.messageSeparator2.Name = "messageSeparator2";
            this.messageSeparator2.Size = new System.Drawing.Size(6, 31);
            // 
            // messageCopyButton
            // 
            this.messageCopyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messageCopyButton.Image = global::EOL_GND.Properties.Resources.copy_32;
            this.messageCopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messageCopyButton.Name = "messageCopyButton";
            this.messageCopyButton.Size = new System.Drawing.Size(28, 28);
            this.messageCopyButton.Text = "Copy Messages (Ctrl + C)";
            this.messageCopyButton.Click += new System.EventHandler(this.messageCopyButton_Click);
            // 
            // messagePasteButton
            // 
            this.messagePasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.messagePasteButton.Image = global::EOL_GND.Properties.Resources.paste_32;
            this.messagePasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.messagePasteButton.Name = "messagePasteButton";
            this.messagePasteButton.Size = new System.Drawing.Size(28, 28);
            this.messagePasteButton.Text = "Paste Messages (Ctrl + V)";
            this.messagePasteButton.Click += new System.EventHandler(this.messagePasteButton_Click);
            // 
            // signalListView
            // 
            this.signalListView.AllColumns.Add(this.sigNoColumn);
            this.signalListView.AllColumns.Add(this.sigNameColumn);
            this.signalListView.AllColumns.Add(this.startBitColumn);
            this.signalListView.AllColumns.Add(this.lengthColumn);
            this.signalListView.AllColumns.Add(this.byteOrderColumn);
            this.signalListView.AllColumns.Add(this.signedColumn);
            this.signalListView.AllColumns.Add(this.factorColumn);
            this.signalListView.AllColumns.Add(this.offsetColumn);
            this.signalListView.AllColumns.Add(this.minimumColumn);
            this.signalListView.AllColumns.Add(this.maximumColumn);
            this.signalListView.AllColumns.Add(this.unitColumn);
            this.signalListView.AllColumns.Add(this.errorIDColumn);
            this.signalListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.signalListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.signalListView.CellEditUseWholeCell = false;
            this.signalListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sigNoColumn,
            this.sigNameColumn,
            this.startBitColumn,
            this.lengthColumn,
            this.byteOrderColumn,
            this.signedColumn,
            this.factorColumn,
            this.offsetColumn,
            this.minimumColumn,
            this.maximumColumn,
            this.unitColumn,
            this.errorIDColumn});
            this.signalListView.ContextMenuStrip = this.signalContextMenu;
            this.signalListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.signalListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.signalListView.FullRowSelect = true;
            this.signalListView.GridLines = true;
            this.signalListView.HideSelection = false;
            this.signalListView.Location = new System.Drawing.Point(0, 31);
            this.signalListView.Name = "signalListView";
            this.signalListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.signalListView.ShowCommandMenuOnRightClick = true;
            this.signalListView.ShowGroups = false;
            this.signalListView.ShowItemCountOnGroups = true;
            this.signalListView.ShowItemToolTips = true;
            this.signalListView.Size = new System.Drawing.Size(882, 289);
            this.signalListView.TabIndex = 1;
            this.signalListView.UseCompatibleStateImageBehavior = false;
            this.signalListView.UseFilterIndicator = true;
            this.signalListView.UseFiltering = true;
            this.signalListView.View = System.Windows.Forms.View.Details;
            this.signalListView.VirtualMode = true;
            this.signalListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.signalListView_CellEditFinishing);
            this.signalListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.signalListView_CellEditStarting);
            this.signalListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.signalListView_FormatRow);
            this.signalListView.SelectionChanged += new System.EventHandler(this.signalListView_SelectionChanged);
            this.signalListView.ItemActivate += new System.EventHandler(this.signalListView_ItemActivate);
            // 
            // sigNoColumn
            // 
            this.sigNoColumn.Groupable = false;
            this.sigNoColumn.IsEditable = false;
            this.sigNoColumn.Sortable = false;
            this.sigNoColumn.Text = "NO";
            this.sigNoColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.sigNoColumn.UseFiltering = false;
            this.sigNoColumn.Width = 50;
            // 
            // sigNameColumn
            // 
            this.sigNameColumn.AspectName = "Name";
            this.sigNameColumn.Text = "Name";
            this.sigNameColumn.Width = 180;
            // 
            // startBitColumn
            // 
            this.startBitColumn.AspectName = "StartBit";
            this.startBitColumn.Text = "StartBit";
            this.startBitColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // lengthColumn
            // 
            this.lengthColumn.AspectName = "Length";
            this.lengthColumn.Text = "Length";
            this.lengthColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // byteOrderColumn
            // 
            this.byteOrderColumn.AspectName = "IntelByteOrder";
            this.byteOrderColumn.CheckBoxes = true;
            this.byteOrderColumn.Text = "IntelBO";
            this.byteOrderColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.byteOrderColumn.ToolTipText = "Intel Byte Order";
            // 
            // signedColumn
            // 
            this.signedColumn.AspectName = "Signed";
            this.signedColumn.CheckBoxes = true;
            this.signedColumn.Text = "Signed";
            this.signedColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // factorColumn
            // 
            this.factorColumn.AspectName = "Factor";
            this.factorColumn.Text = "Factor";
            this.factorColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // offsetColumn
            // 
            this.offsetColumn.AspectName = "Offset";
            this.offsetColumn.Text = "Offset";
            this.offsetColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            // 
            // minimumColumn
            // 
            this.minimumColumn.AspectName = "Minimum";
            this.minimumColumn.Text = "Minimum";
            this.minimumColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.minimumColumn.Width = 70;
            // 
            // maximumColumn
            // 
            this.maximumColumn.AspectName = "Maximum";
            this.maximumColumn.Text = "Maximum";
            this.maximumColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.maximumColumn.Width = 70;
            // 
            // unitColumn
            // 
            this.unitColumn.AspectName = "Unit";
            this.unitColumn.Text = "Unit";
            this.unitColumn.Width = 70;
            // 
            // errorIDColumn
            // 
            this.errorIDColumn.AspectName = "ErrorID";
            this.errorIDColumn.AspectToStringFormat = "0x{0:X}";
            this.errorIDColumn.Text = "ErrorID";
            // 
            // signalContextMenu
            // 
            this.signalContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSignalItem,
            this.deleteSignalsItem,
            this.toolStripSeparator2,
            this.copySignalsItem,
            this.pasteSignalsItem});
            this.signalContextMenu.Name = "messageContextMenu";
            this.signalContextMenu.Size = new System.Drawing.Size(185, 98);
            this.signalContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.signalContextMenu_Opening);
            // 
            // addSignalItem
            // 
            this.addSignalItem.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.addSignalItem.Name = "addSignalItem";
            this.addSignalItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addSignalItem.Size = new System.Drawing.Size(184, 22);
            this.addSignalItem.Text = "&Add Signal";
            this.addSignalItem.Click += new System.EventHandler(this.addSignalItem_Click);
            // 
            // deleteSignalsItem
            // 
            this.deleteSignalsItem.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.deleteSignalsItem.Name = "deleteSignalsItem";
            this.deleteSignalsItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteSignalsItem.Size = new System.Drawing.Size(184, 22);
            this.deleteSignalsItem.Text = "&Delete Signals";
            this.deleteSignalsItem.Click += new System.EventHandler(this.deleteSignalsItem_Click);
            // 
            // toolStripSeparator2
            // 
            this.toolStripSeparator2.Name = "toolStripSeparator2";
            this.toolStripSeparator2.Size = new System.Drawing.Size(181, 6);
            // 
            // copySignalsItem
            // 
            this.copySignalsItem.Image = global::EOL_GND.Properties.Resources.copy_32;
            this.copySignalsItem.Name = "copySignalsItem";
            this.copySignalsItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.C)));
            this.copySignalsItem.Size = new System.Drawing.Size(184, 22);
            this.copySignalsItem.Text = "&Copy Signals";
            this.copySignalsItem.Click += new System.EventHandler(this.copySignalsItem_Click);
            // 
            // pasteSignalsItem
            // 
            this.pasteSignalsItem.Image = global::EOL_GND.Properties.Resources.paste_32;
            this.pasteSignalsItem.Name = "pasteSignalsItem";
            this.pasteSignalsItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.V)));
            this.pasteSignalsItem.Size = new System.Drawing.Size(184, 22);
            this.pasteSignalsItem.Text = "&Paste Signals";
            this.pasteSignalsItem.Click += new System.EventHandler(this.pasteSignalsItem_Click);
            // 
            // signalToolStrip
            // 
            this.signalToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.signalToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signalTitleLabel,
            this.signalAddButton,
            this.signalDeleteButton,
            this.signalSeparator1,
            this.signalCopyButton,
            this.signalPasteButton});
            this.signalToolStrip.Location = new System.Drawing.Point(0, 0);
            this.signalToolStrip.Name = "signalToolStrip";
            this.signalToolStrip.Size = new System.Drawing.Size(882, 31);
            this.signalToolStrip.TabIndex = 0;
            this.signalToolStrip.Text = "signalToolStrip";
            // 
            // signalTitleLabel
            // 
            this.signalTitleLabel.Name = "signalTitleLabel";
            this.signalTitleLabel.Size = new System.Drawing.Size(47, 28);
            this.signalTitleLabel.Text = "Signals:";
            // 
            // signalAddButton
            // 
            this.signalAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalAddButton.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.signalAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalAddButton.Name = "signalAddButton";
            this.signalAddButton.Size = new System.Drawing.Size(28, 28);
            this.signalAddButton.Text = "Add Signal (Ctrl + N)";
            this.signalAddButton.Click += new System.EventHandler(this.signalAddButton_Click);
            // 
            // signalDeleteButton
            // 
            this.signalDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalDeleteButton.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.signalDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalDeleteButton.Name = "signalDeleteButton";
            this.signalDeleteButton.Size = new System.Drawing.Size(28, 28);
            this.signalDeleteButton.Text = "Delete Signals (Del)";
            this.signalDeleteButton.Click += new System.EventHandler(this.signalDeleteButton_Click);
            // 
            // signalSeparator1
            // 
            this.signalSeparator1.Name = "signalSeparator1";
            this.signalSeparator1.Size = new System.Drawing.Size(6, 31);
            // 
            // signalCopyButton
            // 
            this.signalCopyButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalCopyButton.Image = global::EOL_GND.Properties.Resources.copy_32;
            this.signalCopyButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalCopyButton.Name = "signalCopyButton";
            this.signalCopyButton.Size = new System.Drawing.Size(28, 28);
            this.signalCopyButton.Text = "Copy Signals (Ctrl + C)";
            this.signalCopyButton.Click += new System.EventHandler(this.signalCopyButton_Click);
            // 
            // signalPasteButton
            // 
            this.signalPasteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalPasteButton.Image = global::EOL_GND.Properties.Resources.paste_32;
            this.signalPasteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalPasteButton.Name = "signalPasteButton";
            this.signalPasteButton.Size = new System.Drawing.Size(28, 28);
            this.signalPasteButton.Text = "Paste Signals (Ctrl + V)";
            this.signalPasteButton.Click += new System.EventHandler(this.signalPasteButton_Click);
            // 
            // buttonPanel
            // 
            this.buttonPanel.ColumnCount = 4;
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 120F));
            this.buttonPanel.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.buttonPanel.Controls.Add(this.okButton, 1, 0);
            this.buttonPanel.Controls.Add(this.cancelButton, 2, 0);
            this.buttonPanel.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.buttonPanel.Location = new System.Drawing.Point(0, 617);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.RowCount = 1;
            this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonPanel.Size = new System.Drawing.Size(882, 46);
            this.buttonPanel.TabIndex = 1;
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(647, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(92, 31);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(767, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(92, 31);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // DbcEditorForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(882, 663);
            this.Controls.Add(this.mainSplitContainer);
            this.Controls.Add(this.buttonPanel);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DbcEditorForm";
            this.ShowInTaskbar = false;
            this.Text = "CAN Messages & Signals";
            this.mainSplitContainer.Panel1.ResumeLayout(false);
            this.mainSplitContainer.Panel1.PerformLayout();
            this.mainSplitContainer.Panel2.ResumeLayout(false);
            this.mainSplitContainer.Panel2.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.mainSplitContainer)).EndInit();
            this.mainSplitContainer.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.messageListView)).EndInit();
            this.messageContextMenu.ResumeLayout(false);
            this.messageToolStrip.ResumeLayout(false);
            this.messageToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.signalListView)).EndInit();
            this.signalContextMenu.ResumeLayout(false);
            this.signalToolStrip.ResumeLayout(false);
            this.signalToolStrip.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.SplitContainer mainSplitContainer;
        private BrightIdeasSoftware.FastObjectListView messageListView;
        private System.Windows.Forms.ToolStrip messageToolStrip;
        private BrightIdeasSoftware.FastObjectListView signalListView;
        private System.Windows.Forms.ToolStrip signalToolStrip;
        private System.Windows.Forms.ToolStripLabel messageTitleLabel;
        private System.Windows.Forms.ToolStripLabel signalTitleLabel;
        private BrightIdeasSoftware.OLVColumn idColumn;
        private BrightIdeasSoftware.OLVColumn nameColumn;
        private BrightIdeasSoftware.OLVColumn dlcColumn;
        private BrightIdeasSoftware.OLVColumn cycleColumn;
        private BrightIdeasSoftware.OLVColumn sigNameColumn;
        private BrightIdeasSoftware.OLVColumn startBitColumn;
        private BrightIdeasSoftware.OLVColumn lengthColumn;
        private BrightIdeasSoftware.OLVColumn signedColumn;
        private BrightIdeasSoftware.OLVColumn byteOrderColumn;
        private BrightIdeasSoftware.OLVColumn factorColumn;
        private BrightIdeasSoftware.OLVColumn offsetColumn;
        private BrightIdeasSoftware.OLVColumn minimumColumn;
        private BrightIdeasSoftware.OLVColumn maximumColumn;
        private BrightIdeasSoftware.OLVColumn unitColumn;
        private BrightIdeasSoftware.OLVColumn errorIDColumn;
        private System.Windows.Forms.ToolStripButton messageAddButton;
        private System.Windows.Forms.ToolStripButton messageDeleteButton;
        private System.Windows.Forms.ToolStripButton signalAddButton;
        private System.Windows.Forms.ToolStripButton signalDeleteButton;
        private System.Windows.Forms.TableLayoutPanel buttonPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.ToolStripButton messageSaveButton;
        private System.Windows.Forms.ToolStripSeparator messageSeparator1;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn sigNoColumn;
        private System.Windows.Forms.ToolStripSeparator messageSeparator2;
        private System.Windows.Forms.ToolStripButton messageCopyButton;
        private System.Windows.Forms.ToolStripButton messagePasteButton;
        private System.Windows.Forms.ToolStripSeparator signalSeparator1;
        private System.Windows.Forms.ToolStripButton signalCopyButton;
        private System.Windows.Forms.ToolStripButton signalPasteButton;
        private System.Windows.Forms.ContextMenuStrip messageContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addMessageToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem deleteMessagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem copyMessagesToolStripMenuItem;
        private System.Windows.Forms.ToolStripMenuItem pasteMessagesToolStripMenuItem;
        private System.Windows.Forms.ContextMenuStrip signalContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addSignalItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSignalsItem;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator2;
        private System.Windows.Forms.ToolStripMenuItem copySignalsItem;
        private System.Windows.Forms.ToolStripMenuItem pasteSignalsItem;
    }
}