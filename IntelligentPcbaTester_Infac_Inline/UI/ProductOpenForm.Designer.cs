namespace IntelligentPcbaTester
{
    partial class ProductOpenForm
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
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle1 = new System.Windows.Forms.DataGridViewCellStyle();
            System.Windows.Forms.DataGridViewCellStyle dataGridViewCellStyle2 = new System.Windows.Forms.DataGridViewCellStyle();
            this.productDataGridView = new System.Windows.Forms.DataGridView();
            this.productBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.openButton = new System.Windows.Forms.Button();
            this.cancelButton = new System.Windows.Forms.Button();
            this.fGCodeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.carTypeDataGridViewTextBoxColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.projectPathColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            ((System.ComponentModel.ISupportInitialize)(this.productDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBindingSource)).BeginInit();
            this.SuspendLayout();
            // 
            // productDataGridView
            // 
            this.productDataGridView.AllowUserToAddRows = false;
            this.productDataGridView.AllowUserToDeleteRows = false;
            this.productDataGridView.AllowUserToOrderColumns = true;
            this.productDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.productDataGridView.AutoGenerateColumns = false;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Khaki;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.productDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.productDataGridView.ColumnHeadersHeight = 30;
            this.productDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.fGCodeDataGridViewTextBoxColumn,
            this.carTypeDataGridViewTextBoxColumn,
            this.projectPathColumn});
            this.productDataGridView.DataSource = this.productBindingSource;
            this.productDataGridView.EnableHeadersVisualStyles = false;
            this.productDataGridView.Location = new System.Drawing.Point(0, 0);
            this.productDataGridView.MultiSelect = false;
            this.productDataGridView.Name = "productDataGridView";
            this.productDataGridView.ReadOnly = true;
            this.productDataGridView.RowTemplate.Height = 30;
            this.productDataGridView.Size = new System.Drawing.Size(800, 501);
            this.productDataGridView.TabIndex = 0;
            this.productDataGridView.CellDoubleClick += new System.Windows.Forms.DataGridViewCellEventHandler(this.productDataGridView_CellDoubleClick);
            this.productDataGridView.CurrentCellChanged += new System.EventHandler(this.productDataGridView_CurrentCellChanged);
            // 
            // productBindingSource
            // 
            this.productBindingSource.DataSource = typeof(IntelligentPcbaTester.Product);
            // 
            // openButton
            // 
            this.openButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.openButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.openButton.DialogResult = System.Windows.Forms.DialogResult.OK;
            this.openButton.Image = global::IntelligentPcbaTester.Properties.Resources.open_folder_32;
            this.openButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.openButton.Location = new System.Drawing.Point(238, 509);
            this.openButton.Name = "openButton";
            this.openButton.Size = new System.Drawing.Size(142, 39);
            this.openButton.TabIndex = 1;
            this.openButton.Text = "   &Open";
            this.openButton.UseVisualStyleBackColor = true;
            this.openButton.Click += new System.EventHandler(this.openButton_Click);
            // 
            // cancelButton
            // 
            this.cancelButton.Anchor = System.Windows.Forms.AnchorStyles.Bottom;
            this.cancelButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cancelButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.cancelButton.Image = global::IntelligentPcbaTester.Properties.Resources.cancel_32;
            this.cancelButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cancelButton.Location = new System.Drawing.Point(420, 509);
            this.cancelButton.Name = "cancelButton";
            this.cancelButton.Size = new System.Drawing.Size(142, 39);
            this.cancelButton.TabIndex = 2;
            this.cancelButton.Text = "   &Cancel";
            this.cancelButton.UseVisualStyleBackColor = true;
            // 
            // fGCodeDataGridViewTextBoxColumn
            // 
            this.fGCodeDataGridViewTextBoxColumn.DataPropertyName = "FGCode";
            this.fGCodeDataGridViewTextBoxColumn.FillWeight = 20F;
            this.fGCodeDataGridViewTextBoxColumn.HeaderText = "FGCODE";
            this.fGCodeDataGridViewTextBoxColumn.Name = "fGCodeDataGridViewTextBoxColumn";
            this.fGCodeDataGridViewTextBoxColumn.ReadOnly = true;
            this.fGCodeDataGridViewTextBoxColumn.Width = 151;
            // 
            // carTypeDataGridViewTextBoxColumn
            // 
            this.carTypeDataGridViewTextBoxColumn.DataPropertyName = "CarType";
            this.carTypeDataGridViewTextBoxColumn.FillWeight = 30F;
            this.carTypeDataGridViewTextBoxColumn.HeaderText = "차종";
            this.carTypeDataGridViewTextBoxColumn.Name = "carTypeDataGridViewTextBoxColumn";
            this.carTypeDataGridViewTextBoxColumn.ReadOnly = true;
            this.carTypeDataGridViewTextBoxColumn.Width = 240;
            // 
            // projectPathColumn
            // 
            this.projectPathColumn.AutoSizeMode = System.Windows.Forms.DataGridViewAutoSizeColumnMode.Fill;
            this.projectPathColumn.DataPropertyName = "ProjectPath";
            dataGridViewCellStyle2.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleLeft;
            this.projectPathColumn.DefaultCellStyle = dataGridViewCellStyle2;
            this.projectPathColumn.FillWeight = 60F;
            this.projectPathColumn.HeaderText = "Project";
            this.projectPathColumn.Name = "projectPathColumn";
            this.projectPathColumn.ReadOnly = true;
            // 
            // ProductOpenForm
            // 
            this.AcceptButton = this.openButton;
            this.AutoScaleDimensions = new System.Drawing.SizeF(96F, 96F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Dpi;
            this.CancelButton = this.cancelButton;
            this.ClientSize = new System.Drawing.Size(800, 558);
            this.Controls.Add(this.cancelButton);
            this.Controls.Add(this.openButton);
            this.Controls.Add(this.productDataGridView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ProductOpenForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "프로젝트 열기";
            ((System.ComponentModel.ISupportInitialize)(this.productDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.productBindingSource)).EndInit();
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView productDataGridView;
        private System.Windows.Forms.BindingSource productBindingSource;
        private System.Windows.Forms.Button openButton;
        private System.Windows.Forms.Button cancelButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn fGCodeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn carTypeDataGridViewTextBoxColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn projectPathColumn;
    }
}