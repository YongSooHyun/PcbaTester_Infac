namespace EOL_GND.View
{
    partial class VariantEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(VariantEditorForm));
            this.variantListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.variantColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.remarksColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.variantContextMenu = new System.Windows.Forms.ContextMenuStrip(this.components);
            this.addVariantItem = new System.Windows.Forms.ToolStripMenuItem();
            this.deleteVariantItem = new System.Windows.Forms.ToolStripMenuItem();
            this.variantToolStrip = new System.Windows.Forms.ToolStrip();
            this.variantAddButton = new System.Windows.Forms.ToolStripButton();
            this.variantDeleteButton = new System.Windows.Forms.ToolStripButton();
            this.closeButton = new System.Windows.Forms.Button();
            this.okButton = new System.Windows.Forms.Button();
            this.variantsTextBox = new System.Windows.Forms.TextBox();
            this.panel1 = new System.Windows.Forms.Panel();
            ((System.ComponentModel.ISupportInitialize)(this.variantListView)).BeginInit();
            this.variantContextMenu.SuspendLayout();
            this.variantToolStrip.SuspendLayout();
            this.panel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // variantListView
            // 
            this.variantListView.AllColumns.Add(this.noColumn);
            this.variantListView.AllColumns.Add(this.variantColumn);
            this.variantListView.AllColumns.Add(this.remarksColumn);
            this.variantListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.variantListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.variantListView.CellEditUseWholeCell = false;
            this.variantListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.variantColumn,
            this.remarksColumn});
            this.variantListView.ContextMenuStrip = this.variantContextMenu;
            this.variantListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.variantListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.variantListView.FullRowSelect = true;
            this.variantListView.GridLines = true;
            this.variantListView.HideSelection = false;
            this.variantListView.Location = new System.Drawing.Point(0, 31);
            this.variantListView.MultiSelect = false;
            this.variantListView.Name = "variantListView";
            this.variantListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.variantListView.ShowCommandMenuOnRightClick = true;
            this.variantListView.ShowGroups = false;
            this.variantListView.ShowItemCountOnGroups = true;
            this.variantListView.ShowItemToolTips = true;
            this.variantListView.Size = new System.Drawing.Size(412, 337);
            this.variantListView.TabIndex = 1;
            this.variantListView.UseCompatibleStateImageBehavior = false;
            this.variantListView.UseFilterIndicator = true;
            this.variantListView.UseFiltering = true;
            this.variantListView.View = System.Windows.Forms.View.Details;
            this.variantListView.VirtualMode = true;
            this.variantListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.variantListView_FormatRow);
            this.variantListView.SelectionChanged += new System.EventHandler(this.variantListView_SelectionChanged);
            this.variantListView.ItemChecked += new System.Windows.Forms.ItemCheckedEventHandler(this.variantListView_ItemChecked);
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
            // variantColumn
            // 
            this.variantColumn.AspectName = "Name";
            this.variantColumn.Text = "Variant";
            this.variantColumn.Width = 120;
            // 
            // remarksColumn
            // 
            this.remarksColumn.AspectName = "Remarks";
            this.remarksColumn.Text = "Description";
            this.remarksColumn.Width = 198;
            // 
            // variantContextMenu
            // 
            this.variantContextMenu.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.addVariantItem,
            this.deleteVariantItem});
            this.variantContextMenu.Name = "messageContextMenu";
            this.variantContextMenu.Size = new System.Drawing.Size(179, 48);
            this.variantContextMenu.Opening += new System.ComponentModel.CancelEventHandler(this.variantContextMenu_Opening);
            // 
            // addVariantItem
            // 
            this.addVariantItem.Image = global::EOL_GND.Properties.Resources.plus_16;
            this.addVariantItem.Name = "addVariantItem";
            this.addVariantItem.ShortcutKeys = ((System.Windows.Forms.Keys)((System.Windows.Forms.Keys.Control | System.Windows.Forms.Keys.N)));
            this.addVariantItem.Size = new System.Drawing.Size(178, 22);
            this.addVariantItem.Text = "&Add Variant";
            this.addVariantItem.Click += new System.EventHandler(this.addVariantItem_Click);
            // 
            // deleteVariantItem
            // 
            this.deleteVariantItem.Image = global::EOL_GND.Properties.Resources.minus_16;
            this.deleteVariantItem.Name = "deleteVariantItem";
            this.deleteVariantItem.ShortcutKeys = System.Windows.Forms.Keys.Delete;
            this.deleteVariantItem.Size = new System.Drawing.Size(178, 22);
            this.deleteVariantItem.Text = "&Delete Variant";
            this.deleteVariantItem.Click += new System.EventHandler(this.deleteVariantItem_Click);
            // 
            // variantToolStrip
            // 
            this.variantToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.variantToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.variantAddButton,
            this.variantDeleteButton});
            this.variantToolStrip.Location = new System.Drawing.Point(0, 0);
            this.variantToolStrip.Name = "variantToolStrip";
            this.variantToolStrip.Size = new System.Drawing.Size(412, 31);
            this.variantToolStrip.TabIndex = 0;
            this.variantToolStrip.Text = "variantToolStrip";
            // 
            // variantAddButton
            // 
            this.variantAddButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.variantAddButton.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.variantAddButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.variantAddButton.Name = "variantAddButton";
            this.variantAddButton.Size = new System.Drawing.Size(28, 28);
            this.variantAddButton.Text = "Add Variant (Ctrl+N)";
            this.variantAddButton.Click += new System.EventHandler(this.variantAddButton_Click);
            // 
            // variantDeleteButton
            // 
            this.variantDeleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.variantDeleteButton.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.variantDeleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.variantDeleteButton.Name = "variantDeleteButton";
            this.variantDeleteButton.Size = new System.Drawing.Size(28, 28);
            this.variantDeleteButton.Text = "Delete Variant (Del)";
            this.variantDeleteButton.Click += new System.EventHandler(this.variantDeleteButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(288, 13);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(112, 32);
            this.closeButton.TabIndex = 102;
            this.closeButton.Text = "&Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // okButton
            // 
            this.okButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.okButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.okButton.Location = new System.Drawing.Point(170, 13);
            this.okButton.Name = "okButton";
            this.okButton.Size = new System.Drawing.Size(112, 32);
            this.okButton.TabIndex = 101;
            this.okButton.Text = "&OK";
            this.okButton.UseVisualStyleBackColor = true;
            this.okButton.Visible = false;
            // 
            // variantsTextBox
            // 
            this.variantsTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.variantsTextBox.BorderStyle = System.Windows.Forms.BorderStyle.None;
            this.variantsTextBox.Location = new System.Drawing.Point(12, 22);
            this.variantsTextBox.Name = "variantsTextBox";
            this.variantsTextBox.ReadOnly = true;
            this.variantsTextBox.Size = new System.Drawing.Size(152, 16);
            this.variantsTextBox.TabIndex = 100;
            this.variantsTextBox.Visible = false;
            // 
            // panel1
            // 
            this.panel1.Controls.Add(this.okButton);
            this.panel1.Controls.Add(this.closeButton);
            this.panel1.Controls.Add(this.variantsTextBox);
            this.panel1.Dock = System.Windows.Forms.DockStyle.Bottom;
            this.panel1.Location = new System.Drawing.Point(0, 368);
            this.panel1.Name = "panel1";
            this.panel1.Size = new System.Drawing.Size(412, 57);
            this.panel1.TabIndex = 103;
            // 
            // VariantEditorForm
            // 
            this.AcceptButton = this.okButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(412, 425);
            this.Controls.Add(this.variantListView);
            this.Controls.Add(this.variantToolStrip);
            this.Controls.Add(this.panel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "VariantEditorForm";
            this.ShowInTaskbar = false;
            this.Text = "Edit Variants";
            ((System.ComponentModel.ISupportInitialize)(this.variantListView)).EndInit();
            this.variantContextMenu.ResumeLayout(false);
            this.variantToolStrip.ResumeLayout(false);
            this.variantToolStrip.PerformLayout();
            this.panel1.ResumeLayout(false);
            this.panel1.PerformLayout();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private BrightIdeasSoftware.FastObjectListView variantListView;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn variantColumn;
        private System.Windows.Forms.ToolStrip variantToolStrip;
        private System.Windows.Forms.ToolStripButton variantAddButton;
        private System.Windows.Forms.ToolStripButton variantDeleteButton;
        private BrightIdeasSoftware.OLVColumn remarksColumn;
        private System.Windows.Forms.ContextMenuStrip variantContextMenu;
        private System.Windows.Forms.ToolStripMenuItem addVariantItem;
        private System.Windows.Forms.ToolStripMenuItem deleteVariantItem;
        private System.Windows.Forms.Button closeButton;
        private System.Windows.Forms.Button okButton;
        private System.Windows.Forms.TextBox variantsTextBox;
        private System.Windows.Forms.Panel panel1;
    }
}