
namespace IntelligentPcbaTester
{
    partial class CrcSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(CrcSettingsForm));
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.deleteButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.saveButton = new System.Windows.Forms.Button();
            this.crcListView = new BrightIdeasSoftware.FastObjectListView();
            this.noColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.mesCrcColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.novaCrcColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.tableLayoutPanel1.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.crcListView)).BeginInit();
            this.SuspendLayout();
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 3;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 33.33333F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.deleteButton, 1, 0);
            this.tableLayoutPanel1.Controls.Add(this.addButton, 0, 0);
            this.tableLayoutPanel1.Controls.Add(this.saveButton, 2, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(12, 501);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(776, 57);
            this.tableLayoutPanel1.TabIndex = 1;
            // 
            // deleteButton
            // 
            this.deleteButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.deleteButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(255)))), ((int)(((byte)(192)))), ((int)(((byte)(128)))));
            this.deleteButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.deleteButton.Font = new System.Drawing.Font("Malgun Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.deleteButton.Image = ((System.Drawing.Image)(resources.GetObject("deleteButton.Image")));
            this.deleteButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.deleteButton.Location = new System.Drawing.Point(321, 10);
            this.deleteButton.Name = "deleteButton";
            this.deleteButton.Size = new System.Drawing.Size(132, 36);
            this.deleteButton.TabIndex = 1;
            this.deleteButton.Text = "     삭제(&D)";
            this.deleteButton.UseVisualStyleBackColor = false;
            this.deleteButton.Click += new System.EventHandler(this.deleteButton_Click);
            // 
            // addButton
            // 
            this.addButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.addButton.BackColor = System.Drawing.Color.Yellow;
            this.addButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.addButton.Font = new System.Drawing.Font("Malgun Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.addButton.Image = ((System.Drawing.Image)(resources.GetObject("addButton.Image")));
            this.addButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.addButton.Location = new System.Drawing.Point(63, 10);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(131, 36);
            this.addButton.TabIndex = 0;
            this.addButton.Text = "      추가(&A)";
            this.addButton.UseVisualStyleBackColor = false;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // saveButton
            // 
            this.saveButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.saveButton.BackColor = System.Drawing.Color.FromArgb(((int)(((byte)(128)))), ((int)(((byte)(255)))), ((int)(((byte)(128)))));
            this.saveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveButton.Font = new System.Drawing.Font("Malgun Gothic", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.saveButton.Image = ((System.Drawing.Image)(resources.GetObject("saveButton.Image")));
            this.saveButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.saveButton.Location = new System.Drawing.Point(579, 10);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(134, 36);
            this.saveButton.TabIndex = 3;
            this.saveButton.Text = "     저장(&S)";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // crcListView
            // 
            this.crcListView.AllColumns.Add(this.noColumn);
            this.crcListView.AllColumns.Add(this.mesCrcColumn);
            this.crcListView.AllColumns.Add(this.novaCrcColumn);
            this.crcListView.AllowColumnReorder = true;
            this.crcListView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.crcListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.crcListView.CellEditUseWholeCell = false;
            this.crcListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.noColumn,
            this.mesCrcColumn,
            this.novaCrcColumn});
            this.crcListView.Cursor = System.Windows.Forms.Cursors.Default;
            this.crcListView.FullRowSelect = true;
            this.crcListView.GridLines = true;
            this.crcListView.HideSelection = false;
            this.crcListView.Location = new System.Drawing.Point(0, 0);
            this.crcListView.Name = "crcListView";
            this.crcListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.Submenu;
            this.crcListView.ShowCommandMenuOnRightClick = true;
            this.crcListView.ShowGroups = false;
            this.crcListView.ShowItemToolTips = true;
            this.crcListView.Size = new System.Drawing.Size(800, 495);
            this.crcListView.TabIndex = 0;
            this.crcListView.UseCompatibleStateImageBehavior = false;
            this.crcListView.UseFilterIndicator = true;
            this.crcListView.UseFiltering = true;
            this.crcListView.View = System.Windows.Forms.View.Details;
            this.crcListView.VirtualMode = true;
            this.crcListView.CellEditFinishing += new BrightIdeasSoftware.CellEditEventHandler(this.crcListView_CellEditFinishing);
            this.crcListView.CellEditStarting += new BrightIdeasSoftware.CellEditEventHandler(this.crcListView_CellEditStarting);
            this.crcListView.FormatRow += new System.EventHandler<BrightIdeasSoftware.FormatRowEventArgs>(this.crcListView_FormatRow);
            this.crcListView.SelectionChanged += new System.EventHandler(this.crcListView_SelectionChanged);
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
            // mesCrcColumn
            // 
            this.mesCrcColumn.AspectName = "MesCrc";
            this.mesCrcColumn.AspectToStringFormat = "{0:X}";
            this.mesCrcColumn.CellEditUseWholeCell = true;
            this.mesCrcColumn.Groupable = false;
            this.mesCrcColumn.Text = "MES CRC (Hex)";
            this.mesCrcColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.mesCrcColumn.Width = 240;
            // 
            // novaCrcColumn
            // 
            this.novaCrcColumn.AspectName = "NovaCrc";
            this.novaCrcColumn.AspectToStringFormat = "{0:X}";
            this.novaCrcColumn.Groupable = false;
            this.novaCrcColumn.Text = "NovaFlash CRC (Hex)";
            this.novaCrcColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.novaCrcColumn.Width = 240;
            // 
            // CrcSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(800, 558);
            this.Controls.Add(this.crcListView);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "CrcSettingsForm";
            this.Text = "CRC 테이블";
            this.tableLayoutPanel1.ResumeLayout(false);
            ((System.ComponentModel.ISupportInitialize)(this.crcListView)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button deleteButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button saveButton;
        private BrightIdeasSoftware.FastObjectListView crcListView;
        private BrightIdeasSoftware.OLVColumn noColumn;
        private BrightIdeasSoftware.OLVColumn mesCrcColumn;
        private BrightIdeasSoftware.OLVColumn novaCrcColumn;
    }
}