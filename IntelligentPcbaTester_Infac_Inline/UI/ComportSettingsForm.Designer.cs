namespace IntelligentPcbaTester
{
    partial class ComportSettingsForm
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
            this.portSettingsDataGridView = new System.Windows.Forms.DataGridView();
            this.deviceNameColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.enabledColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.portColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.baudRateColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.dataBitsColumn = new System.Windows.Forms.DataGridViewTextBoxColumn();
            this.parityColumn = new System.Windows.Forms.DataGridViewComboBoxColumn();
            this.crColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.lfColumn = new System.Windows.Forms.DataGridViewCheckBoxColumn();
            this.comportSettingsBindingSource = new System.Windows.Forms.BindingSource(this.components);
            this.tableLayoutPanel1 = new System.Windows.Forms.TableLayoutPanel();
            this.saveButton = new System.Windows.Forms.Button();
            ((System.ComponentModel.ISupportInitialize)(this.portSettingsDataGridView)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.comportSettingsBindingSource)).BeginInit();
            this.tableLayoutPanel1.SuspendLayout();
            this.SuspendLayout();
            // 
            // portSettingsDataGridView
            // 
            this.portSettingsDataGridView.AllowUserToAddRows = false;
            this.portSettingsDataGridView.AllowUserToDeleteRows = false;
            this.portSettingsDataGridView.AllowUserToOrderColumns = true;
            this.portSettingsDataGridView.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.portSettingsDataGridView.AutoGenerateColumns = false;
            this.portSettingsDataGridView.AutoSizeColumnsMode = System.Windows.Forms.DataGridViewAutoSizeColumnsMode.Fill;
            dataGridViewCellStyle1.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            dataGridViewCellStyle1.BackColor = System.Drawing.Color.Khaki;
            dataGridViewCellStyle1.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            dataGridViewCellStyle1.ForeColor = System.Drawing.SystemColors.WindowText;
            dataGridViewCellStyle1.SelectionBackColor = System.Drawing.SystemColors.Highlight;
            dataGridViewCellStyle1.SelectionForeColor = System.Drawing.SystemColors.HighlightText;
            dataGridViewCellStyle1.WrapMode = System.Windows.Forms.DataGridViewTriState.True;
            this.portSettingsDataGridView.ColumnHeadersDefaultCellStyle = dataGridViewCellStyle1;
            this.portSettingsDataGridView.ColumnHeadersHeight = 40;
            this.portSettingsDataGridView.Columns.AddRange(new System.Windows.Forms.DataGridViewColumn[] {
            this.deviceNameColumn,
            this.enabledColumn,
            this.portColumn,
            this.baudRateColumn,
            this.dataBitsColumn,
            this.parityColumn,
            this.crColumn,
            this.lfColumn});
            this.portSettingsDataGridView.DataSource = this.comportSettingsBindingSource;
            this.portSettingsDataGridView.EnableHeadersVisualStyles = false;
            this.portSettingsDataGridView.Location = new System.Drawing.Point(0, 0);
            this.portSettingsDataGridView.Name = "portSettingsDataGridView";
            this.portSettingsDataGridView.RowHeadersVisible = false;
            this.portSettingsDataGridView.RowTemplate.DefaultCellStyle.Alignment = System.Windows.Forms.DataGridViewContentAlignment.MiddleCenter;
            this.portSettingsDataGridView.RowTemplate.Height = 40;
            this.portSettingsDataGridView.Size = new System.Drawing.Size(808, 503);
            this.portSettingsDataGridView.TabIndex = 0;
            this.portSettingsDataGridView.CellEndEdit += new System.Windows.Forms.DataGridViewCellEventHandler(this.portSettingsDataGridView_CellEndEdit);
            this.portSettingsDataGridView.CellValidating += new System.Windows.Forms.DataGridViewCellValidatingEventHandler(this.portSettingsDataGridView_CellValidating);
            // 
            // deviceNameColumn
            // 
            this.deviceNameColumn.DataPropertyName = "DeviceName";
            this.deviceNameColumn.FillWeight = 83.13624F;
            this.deviceNameColumn.HeaderText = "Device 명";
            this.deviceNameColumn.Name = "deviceNameColumn";
            this.deviceNameColumn.ReadOnly = true;
            // 
            // enabledColumn
            // 
            this.enabledColumn.DataPropertyName = "IsEnabled";
            this.enabledColumn.FillWeight = 42F;
            this.enabledColumn.HeaderText = "Enabled";
            this.enabledColumn.Name = "enabledColumn";
            // 
            // portColumn
            // 
            this.portColumn.DataPropertyName = "Port";
            this.portColumn.FillWeight = 58.19537F;
            this.portColumn.HeaderText = "COM Port";
            this.portColumn.Name = "portColumn";
            this.portColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.portColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // baudRateColumn
            // 
            this.baudRateColumn.DataPropertyName = "BaudRate";
            this.baudRateColumn.FillWeight = 83.13624F;
            this.baudRateColumn.HeaderText = "Baudrate";
            this.baudRateColumn.Name = "baudRateColumn";
            // 
            // dataBitsColumn
            // 
            this.dataBitsColumn.DataPropertyName = "DataBits";
            this.dataBitsColumn.FillWeight = 58.19537F;
            this.dataBitsColumn.HeaderText = "Data Bits";
            this.dataBitsColumn.Name = "dataBitsColumn";
            this.dataBitsColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            // 
            // parityColumn
            // 
            this.parityColumn.DataPropertyName = "Parity";
            this.parityColumn.FillWeight = 58.19537F;
            this.parityColumn.HeaderText = "Parity";
            this.parityColumn.Name = "parityColumn";
            this.parityColumn.Resizable = System.Windows.Forms.DataGridViewTriState.True;
            this.parityColumn.SortMode = System.Windows.Forms.DataGridViewColumnSortMode.Automatic;
            // 
            // crColumn
            // 
            this.crColumn.DataPropertyName = "CR";
            this.crColumn.FillWeight = 41.56812F;
            this.crColumn.HeaderText = "CR";
            this.crColumn.Name = "crColumn";
            // 
            // lfColumn
            // 
            this.lfColumn.DataPropertyName = "LF";
            this.lfColumn.FillWeight = 41.56812F;
            this.lfColumn.HeaderText = "LF";
            this.lfColumn.Name = "lfColumn";
            // 
            // comportSettingsBindingSource
            // 
            this.comportSettingsBindingSource.DataSource = typeof(IntelligentPcbaTester.ComportSettings);
            // 
            // tableLayoutPanel1
            // 
            this.tableLayoutPanel1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.tableLayoutPanel1.ColumnCount = 1;
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.ColumnStyles.Add(new System.Windows.Forms.ColumnStyle(System.Windows.Forms.SizeType.Absolute, 20F));
            this.tableLayoutPanel1.Controls.Add(this.saveButton, 0, 0);
            this.tableLayoutPanel1.Location = new System.Drawing.Point(26, 504);
            this.tableLayoutPanel1.Name = "tableLayoutPanel1";
            this.tableLayoutPanel1.RowCount = 1;
            this.tableLayoutPanel1.RowStyles.Add(new System.Windows.Forms.RowStyle(System.Windows.Forms.SizeType.Percent, 100F));
            this.tableLayoutPanel1.Size = new System.Drawing.Size(756, 57);
            this.tableLayoutPanel1.TabIndex = 2;
            // 
            // saveButton
            // 
            this.saveButton.Anchor = System.Windows.Forms.AnchorStyles.None;
            this.saveButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.saveButton.Font = new System.Drawing.Font("Gulim", 12F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point, ((byte)(129)));
            this.saveButton.Location = new System.Drawing.Point(322, 10);
            this.saveButton.Name = "saveButton";
            this.saveButton.Size = new System.Drawing.Size(111, 36);
            this.saveButton.TabIndex = 0;
            this.saveButton.Text = "저장(&S)";
            this.saveButton.UseVisualStyleBackColor = false;
            this.saveButton.Click += new System.EventHandler(this.saveButton_Click);
            // 
            // ComportSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(808, 561);
            this.Controls.Add(this.tableLayoutPanel1);
            this.Controls.Add(this.portSettingsDataGridView);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "ComportSettingsForm";
            this.Text = "COM 포트";
            this.Shown += new System.EventHandler(this.ComportSettingsForm_Shown);
            ((System.ComponentModel.ISupportInitialize)(this.portSettingsDataGridView)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.comportSettingsBindingSource)).EndInit();
            this.tableLayoutPanel1.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion
        private System.Windows.Forms.DataGridView portSettingsDataGridView;
        private System.Windows.Forms.BindingSource comportSettingsBindingSource;
        private System.Windows.Forms.TableLayoutPanel tableLayoutPanel1;
        private System.Windows.Forms.Button saveButton;
        private System.Windows.Forms.DataGridViewTextBoxColumn deviceNameColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn enabledColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn portColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn baudRateColumn;
        private System.Windows.Forms.DataGridViewTextBoxColumn dataBitsColumn;
        private System.Windows.Forms.DataGridViewComboBoxColumn parityColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn crColumn;
        private System.Windows.Forms.DataGridViewCheckBoxColumn lfColumn;
    }
}