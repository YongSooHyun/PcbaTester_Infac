namespace EOL_GND.View
{
    partial class CanSignalsEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CanSignalsEditorForm));
            this.signalListView = new BrightIdeasSoftware.FastObjectListView();
            this.sigNoColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.sigNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.valueTypeColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.valueColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.signalContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addSignalItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteSignalsItem = new System.Windows.Forms.ToolStripMenuItem();
            this.toolStripSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.moveUpItem = new System.Windows.Forms.ToolStripMenuItem();
            this.moveDownItem = new System.Windows.Forms.ToolStripMenuItem();
            this.signalToolStrip = new System.Windows.Forms.ToolStrip();
            this.signalAddButton = new System.Windows.Forms.ToolStripButton();
            this.signalDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.signalSeparator1 = new System.Windows.Forms.ToolStripSeparator();
            this.signalMoveUpButton = new System.Windows.Forms.ToolStripButton();
            this.signalMoveDownButton = new System.Windows.Forms.ToolStripButton();
            this.buttonPanel = new System.Windows.Forms.TableLayoutPanel();
            this.okButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.signalListView)).BeginInit();
            this.signalContextMenu.SuspendLayout();
            this.signalToolStrip.SuspendLayout();
            this.buttonPanel.SuspendLayout();
            this.SuspendLayout();
            // 
            // signalListView
            // 
            this.signalListView.AllColumns.Add(this.sigNoColumn);
            this.signalListView.AllColumns.Add(this.sigNameColumn);
            this.signalListView.AllColumns.Add(this.valueTypeColumn);
            this.signalListView.AllColumns.Add(this.valueColumn);
            this.signalListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.signalListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.signalListView.CellEditUseWholeCell = false;
            this.signalListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.sigNoColumn,
            this.sigNameColumn,
            this.valueTypeColumn,
            this.valueColumn});
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
            this.signalListView.Size = new System.Drawing.Size(535, 281);
            this.signalListView.TabIndex = 1;
            this.signalListView.UseCellFormatEvents = true;
            this.signalListView.UseCompatibleStateImageBehavior = false;
            this.signalListView.UseFilterIndicator = true;
            this.signalListView.UseFiltering = true;
            this.signalListView.View = System.Windows.Forms.View.Details;
            this.signalListView.VirtualMode = true;
            this.signalListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.signalListView_CellEditStarting);
            this.signalListView.CellToolTipShowing += new System.EventHandler<BrightIdeasSoftware.ToolTipShowingEventArgs>(this.signalListView_CellToolTipShowing);
            this.signalListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.signalListView_FormatRow);
            this.signalListView.SelectionChanged += new System.EventHandler(this.signalListView_SelectionChanged);
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
            this.sigNameColumn.AspectName = "SignalName";
            this.sigNameColumn.Text = "Signal Name";
            this.sigNameColumn.Width = 220;
            // 
            // valueTypeColumn
            // 
            this.valueTypeColumn.AspectName = "ValueType";
            this.valueTypeColumn.Text = "Value Type";
            this.valueTypeColumn.Width = 120;
            // 
            // valueColumn
            // 
            this.valueColumn.AspectName = "Value";
            this.valueColumn.Text = "Value";
            this.valueColumn.Width = 120;
            // 
            // signalContextMenu
            // 
            this.signalContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addSignalItem,
            this.deleteSignalsItem,
            this.toolStripSeparator1,
            this.moveUpItem,
            this.moveDownItem});
            this.signalContextMenu.Name = "messageContextMenu";
            this.signalContextMenu.Size = new System.Drawing.Size(204, 98);
            this.signalContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.signalContextMenu_Opening);
            // 
            // addSignalItem
            // 
            this.addSignalItem.Image = global::EOL_GND.Properties.Resources.plus_16;
            this.addSignalItem.Name = "addSignalItem";
            this.addSignalItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addSignalItem.Size = new System.Drawing.Size(203, 22);
            this.addSignalItem.Text = "&Add Signal";
            this.addSignalItem.Click += new System.EventHandler(this.addSignalItem_Click);
            // 
            // deleteSignalsItem
            // 
            this.deleteSignalsItem.Image = global::EOL_GND.Properties.Resources.minus_16;
            this.deleteSignalsItem.Name = "deleteSignalsItem";
            this.deleteSignalsItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteSignalsItem.Size = new System.Drawing.Size(203, 22);
            this.deleteSignalsItem.Text = "&Delete Signals";
            this.deleteSignalsItem.Click += new System.EventHandler(this.deleteSignalsItem_Click);
            // 
            // toolStripSeparator1
            // 
            this.toolStripSeparator1.Name = "toolStripSeparator1";
            this.toolStripSeparator1.Size = new System.Drawing.Size(200, 6);
            // 
            // moveUpItem
            // 
            this.moveUpItem.Image = global::EOL_GND.Properties.Resources.up_16;
            this.moveUpItem.Name = "moveUpItem";
            this.moveUpItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Up)));
            this.moveUpItem.Size = new System.Drawing.Size(203, 22);
            this.moveUpItem.Text = "Move &Up";
            this.moveUpItem.Click += new System.EventHandler(this.moveUpItem_Click);
            // 
            // moveDownItem
            // 
            this.moveDownItem.Image = global::EOL_GND.Properties.Resources.down_16;
            this.moveDownItem.Name = "moveDownItem";
            this.moveDownItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.Down)));
            this.moveDownItem.Size = new System.Drawing.Size(203, 22);
            this.moveDownItem.Text = "Move &Down";
            this.moveDownItem.Click += new System.EventHandler(this.moveDownItem_Click);
            // 
            // signalToolStrip
            // 
            this.signalToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.signalToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.signalAddButton,
            this.signalDeleteButton,
            this.signalSeparator1,
            this.signalMoveUpButton,
            this.signalMoveDownButton});
            this.signalToolStrip.Location = new System.Drawing.Point(0, 0);
            this.signalToolStrip.Name = "signalToolStrip";
            this.signalToolStrip.Size = new System.Drawing.Size(535, 31);
            this.signalToolStrip.TabIndex = 0;
            this.signalToolStrip.Text = "signalToolStrip";
            // 
            // signalAddButton
            // 
            this.signalAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalAddButton.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.signalAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalAddButton.Name = "signalAddButton";
            this.signalAddButton.Size = new System.Drawing.Size(28, 28);
            this.signalAddButton.Text = "Add Signal (Ctrl+N)";
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
            // signalMoveUpButton
            // 
            this.signalMoveUpButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalMoveUpButton.Image = global::EOL_GND.Properties.Resources.up_32;
            this.signalMoveUpButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalMoveUpButton.Name = "signalMoveUpButton";
            this.signalMoveUpButton.Size = new System.Drawing.Size(28, 28);
            this.signalMoveUpButton.Text = "Move Up (Ctrl+Up)";
            this.signalMoveUpButton.Click += new System.EventHandler(this.signalMoveUpButton_Click);
            // 
            // signalMoveDownButton
            // 
            this.signalMoveDownButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.signalMoveDownButton.Image = global::EOL_GND.Properties.Resources.down_32;
            this.signalMoveDownButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.signalMoveDownButton.Name = "signalMoveDownButton";
            this.signalMoveDownButton.Size = new System.Drawing.Size(28, 28);
            this.signalMoveDownButton.Text = "Move Down (Ctrl+Down)";
            this.signalMoveDownButton.Click += new System.EventHandler(this.signalMoveDownButton_Click);
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
            this.buttonPanel.Location = new System.Drawing.Point(0, 312);
            this.buttonPanel.Name = "buttonPanel";
            this.buttonPanel.RowCount = 1;
            this.buttonPanel.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.buttonPanel.Size = new System.Drawing.Size(535, 46);
            this.buttonPanel.TabIndex = 2;
            // 
            // okButton
            // 
            this.okButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(290, 7);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(102, 31);
            this.okButton.TabIndex = 0;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Click += new System.EventHandler(this.okButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Right;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Location = new System.Drawing.Point(410, 7);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(102, 31);
            this.cancelButton.TabIndex = 1;
            this.cancelButton.Text = "&Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            this.cancelButton.Click += new System.EventHandler(this.cancelButton_Click);
            // 
            // CanSignalsEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(535, 358);
            this.Controls.Add(this.signalListView);
            this.Controls.Add(this.buttonPanel);
            this.Controls.Add(this.signalToolStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "CanSignalsEditorForm";
            this.ShowInTaskbar = false;
            this.Text = "CAN Signals";
            ((System.ComponentModel.ISupportInitialize)(this.signalListView)).EndInit();
            this.signalContextMenu.ResumeLayout(false);
            this.signalToolStrip.ResumeLayout(false);
            this.signalToolStrip.PerformLayout();
            this.buttonPanel.ResumeLayout(false);
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView signalListView;
        private BrightIdeasSoftware.OLVColumn sigNoColumn;
        private BrightIdeasSoftware.OLVColumn sigNameColumn;
        private System.Windows.Forms.ToolStrip signalToolStrip;
        private System.Windows.Forms.ToolStripButton signalAddButton;
        private System.Windows.Forms.ToolStripButton signalDeleteButton;
        private System.Windows.Forms.ToolStripSeparator signalSeparator1;
        private System.Windows.Forms.TableLayoutPanel buttonPanel;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.Button cancelButton;
        private BrightIdeasSoftware.OLVColumn valueTypeColumn;
        private BrightIdeasSoftware.OLVColumn valueColumn;
        private System.Windows.Forms.ContextMenuStrip signalContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addSignalItem;
        private System.Windows.Forms.ToolStripMenuItem deleteSignalsItem;
        private System.Windows.Forms.ToolStripButton signalMoveUpButton;
        private System.Windows.Forms.ToolStripButton signalMoveDownButton;
        private System.Windows.Forms.ToolStripSeparator toolStripSeparator1;
        private System.Windows.Forms.ToolStripMenuItem moveUpItem;
        private System.Windows.Forms.ToolStripMenuItem moveDownItem;
    }
}