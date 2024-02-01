namespace EOL_GND.View
{
    partial class UserEditorForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(UserEditorForm));
            this.mainToolStrip = new System.Windows.Forms.ToolStrip();
            this.saveButton = new System.Windows.Forms.ToolStripButton();
            this.separator1 = new System.Windows.Forms.ToolStripSeparator();
            this.addButton = new System.Windows.Forms.ToolStripButton();
            this.deleteButton = new System.Windows.Forms.ToolStripButton();
            this.userListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.userNameColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.passwordColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.roleColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.noteColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.mainToolStrip.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userListView)).BeginInit();
            this.SuspendLayout();
            // 
            // mainToolStrip
            // 
            this.mainToolStrip.ImageScalingSize = new System.Drawing.Size(24, 24);
            this.mainToolStrip.Items.AddRange(new System.Windows.Forms.ToolStripItem[] {
            this.saveButton,
            this.separator1,
            this.addButton,
            this.deleteButton});
            this.mainToolStrip.Location = new System.Drawing.Point(0, 0);
            this.mainToolStrip.Name = "mainToolStrip";
            this.mainToolStrip.Size = new System.Drawing.Size(782, 31);
            this.mainToolStrip.TabIndex = 1;
            this.mainToolStrip.Text = "messageToolStrip";
            // 
            // saveButton
            // 
            this.saveButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.saveButton.Image = global::EOL_GND.Properties.Resources.save_32;
            this.saveButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(28, 28);
            this.saveButton.Text = "Save (Ctrl + S)";
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // separator1
            // 
            this.separator1.Name = "separator1";
            this.separator1.Size = new System.Drawing.Size(6, 31);
            // 
            // addButton
            // 
            this.addButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.addButton.Image = global::EOL_GND.Properties.Resources.plus_32;
            this.addButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(28, 28);
            this.addButton.Text = "Add Message (Ctrl + N)";
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deleteButton
            // 
            this.deleteButton.DisplayStyle = System.Windows.Forms.ToolStripItemDisplayStyle.Image;
            this.deleteButton.Image = global::EOL_GND.Properties.Resources.minus_32;
            this.deleteButton.ImageTransparentColor = System.Drawing.Color.Magenta;
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(28, 28);
            this.deleteButton.Text = "Delete Messages (Del)";
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // userListView
            // 
            this.userListView.AllColumns.Add(this.noColumn);
            this.userListView.AllColumns.Add(this.userNameColumn);
            this.userListView.AllColumns.Add(this.passwordColumn);
            this.userListView.AllColumns.Add(this.roleColumn);
            this.userListView.AllColumns.Add(this.noteColumn);
            this.userListView.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(230)))), ((int)(((byte)(255)))), ((int)(((byte)(230)))));
            this.userListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.userListView.CellEditUseWholeCell = false;
            this.userListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.userNameColumn,
            this.passwordColumn,
            this.roleColumn,
            this.noteColumn});
            this.userListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.userListView.Dock = System.Windows.Forms.DockStyle.Fill;
            this.userListView.FullRowSelect = true;
            this.userListView.GridLines = true;
            this.userListView.HideSelection = false;
            this.userListView.Location = new System.Drawing.Point(0, 31);
            this.userListView.Name = "userListView";
            this.userListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.userListView.ShowCommandMenuOnRightClick = true;
            this.userListView.ShowGroups = false;
            this.userListView.ShowItemCountOnGroups = true;
            this.userListView.ShowItemToolTips = true;
            this.userListView.Size = new System.Drawing.Size(782, 492);
            this.userListView.TabIndex = 2;
            this.userListView.UseCompatibleStateImageBehavior = false;
            this.userListView.UseFilterIndicator = true;
            this.userListView.UseFiltering = true;
            this.userListView.View = System.Windows.Forms.View.Details;
            this.userListView.VirtualMode = true;
            this.userListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.userListView_CellEditStarting);
            this.userListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.userListView_FormatRow);
            this.userListView.SelectedIndexChanged += new System.EventHandler(this.userListView_SelectionChanged);
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
            // userNameColumn
            // 
            this.userNameColumn.AspectName = "UserName";
            this.userNameColumn.AspectToStringFormat = "";
            this.userNameColumn.Text = "User Name";
            this.userNameColumn.Width = 120;
            // 
            // passwordColumn
            // 
            this.passwordColumn.AspectName = "PlainPassword";
            this.passwordColumn.Text = "Password";
            this.passwordColumn.Width = 200;
            // 
            // roleColumn
            // 
            this.roleColumn.AspectName = "Role";
            this.roleColumn.Text = "Level";
            this.roleColumn.Width = 120;
            // 
            // noteColumn
            // 
            this.noteColumn.AspectName = "Note";
            this.noteColumn.Text = "Note";
            this.noteColumn.Width = 240;
            // 
            // UserEditorForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(782, 523);
            this.Controls.Add(this.userListView);
            this.Controls.Add(this.mainToolStrip);
            this.Font = new System.Drawing.Font("Segoe UI", 9F);
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "UserEditorForm";
            this.Text = "User Management";
            this.mainToolStrip.ResumeLayout(false);
            this.mainToolStrip.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.userListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.ToolStrip mainToolStrip;
        private System.Windows.Forms.ToolStripButton saveButton;
        private System.Windows.Forms.ToolStripSeparator separator1;
        private System.Windows.Forms.ToolStripButton addButton;
        private System.Windows.Forms.ToolStripButton deleteButton;
        private BrightIdeasSoftware.FastObjectListView userListView;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn userNameColumn;
        private BrightIdeasSoftware.OLVColumn passwordColumn;
        private BrightIdeasSoftware.OLVColumn roleColumn;
        private BrightIdeasSoftware.OLVColumn noteColumn;
    }
}