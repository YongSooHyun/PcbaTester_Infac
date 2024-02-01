namespace EOL_GND.View
{
    partial class DeviceSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(DeviceSettingsForm));
            this.autoFillCheckBox = new System.Windows.Forms.CheckBox();
            this.relayChannelsGroupBox = new System.Windows.Forms.GroupBox();
            this.channelResetButton = new System.Windows.Forms.Button();
            this.startNumberNUDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.cardChannelsNUDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.cardNumberNUDown = new System.Windows.Forms.NumericUpDown();
            this.label2 = new System.Windows.Forms.Label();
            this.downButton = new System.Windows.Forms.Button();
            this.upButton = new System.Windows.Forms.Button();
            this.removeButton = new System.Windows.Forms.Button();
            this.addButton = new System.Windows.Forms.Button();
            this.deviceListView = new BrightIdeasSoftware.FastObjectListView();
            this.categoryColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.channelColumn = ((BrightIdeasSoftware.OLVColumn)(new BrightIdeasSoftware.OLVColumn()));
            this.label1 = new System.Windows.Forms.Label();
            this.toolTip1 = new System.Windows.Forms.ToolTip(this.components);
            this.label5 = new System.Windows.Forms.Label();
            this.relayChannelsGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startNumberNUDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardChannelsNUDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardNumberNUDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceListView)).BeginInit();
            this.SuspendLayout();
            // 
            // autoFillCheckBox
            // 
            this.autoFillCheckBox.AutoSize = true;
            this.autoFillCheckBox.Location = new System.Drawing.Point(6, 3);
            this.autoFillCheckBox.Name = "autoFillCheckBox";
            this.autoFillCheckBox.Size = new System.Drawing.Size(568, 19);
            this.autoFillCheckBox.TabIndex = 0;
            this.autoFillCheckBox.Text = "계측기의 1번 채널 High 릴레이 채널 번호를 변경하면 다른 릴레이 채널 번호들을 자동으로 입력합니다.";
            this.autoFillCheckBox.UseVisualStyleBackColor = true;
            this.autoFillCheckBox.CheckedChanged += new System.EventHandler(this.autoFillCheckBox_CheckedChanged);
            // 
            // relayChannelsGroupBox
            // 
            this.relayChannelsGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.relayChannelsGroupBox.Controls.Add(this.label5);
            this.relayChannelsGroupBox.Controls.Add(this.channelResetButton);
            this.relayChannelsGroupBox.Controls.Add(this.startNumberNUDown);
            this.relayChannelsGroupBox.Controls.Add(this.label4);
            this.relayChannelsGroupBox.Controls.Add(this.cardChannelsNUDown);
            this.relayChannelsGroupBox.Controls.Add(this.label3);
            this.relayChannelsGroupBox.Controls.Add(this.cardNumberNUDown);
            this.relayChannelsGroupBox.Controls.Add(this.label2);
            this.relayChannelsGroupBox.Controls.Add(this.downButton);
            this.relayChannelsGroupBox.Controls.Add(this.upButton);
            this.relayChannelsGroupBox.Controls.Add(this.removeButton);
            this.relayChannelsGroupBox.Controls.Add(this.addButton);
            this.relayChannelsGroupBox.Controls.Add(this.deviceListView);
            this.relayChannelsGroupBox.Controls.Add(this.label1);
            this.relayChannelsGroupBox.Location = new System.Drawing.Point(6, 28);
            this.relayChannelsGroupBox.Name = "relayChannelsGroupBox";
            this.relayChannelsGroupBox.Size = new System.Drawing.Size(813, 245);
            this.relayChannelsGroupBox.TabIndex = 1;
            this.relayChannelsGroupBox.TabStop = false;
            this.relayChannelsGroupBox.Text = "릴레이 채널 할당";
            // 
            // channelResetButton
            // 
            this.channelResetButton.Location = new System.Drawing.Point(256, 164);
            this.channelResetButton.Name = "channelResetButton";
            this.channelResetButton.Size = new System.Drawing.Size(160, 33);
            this.channelResetButton.TabIndex = 12;
            this.channelResetButton.Text = "릴레이 채널 번호 재설정";
            this.toolTip1.SetToolTip(this.channelResetButton, "디바이스들의 릴레이 채널 번호들을 릴레이 카드 설정에 따라 모두 재설정합니다.");
            this.channelResetButton.UseVisualStyleBackColor = true;
            this.channelResetButton.Click += new System.EventHandler(this.channelResetButton_Click);
            // 
            // startNumberNUDown
            // 
            this.startNumberNUDown.Location = new System.Drawing.Point(330, 113);
            this.startNumberNUDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.startNumberNUDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.startNumberNUDown.Name = "startNumberNUDown";
            this.startNumberNUDown.Size = new System.Drawing.Size(86, 23);
            this.startNumberNUDown.TabIndex = 11;
            this.startNumberNUDown.ThousandsSeparator = true;
            this.startNumberNUDown.ValueChanged += new System.EventHandler(this.startNumberNUDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(263, 116);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(61, 15);
            this.label4.TabIndex = 10;
            this.label4.Text = "시작 번호:";
            // 
            // cardChannelsNUDown
            // 
            this.cardChannelsNUDown.Location = new System.Drawing.Point(330, 81);
            this.cardChannelsNUDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.cardChannelsNUDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.cardChannelsNUDown.Name = "cardChannelsNUDown";
            this.cardChannelsNUDown.Size = new System.Drawing.Size(86, 23);
            this.cardChannelsNUDown.TabIndex = 9;
            this.cardChannelsNUDown.ThousandsSeparator = true;
            this.cardChannelsNUDown.ValueChanged += new System.EventHandler(this.cardChannelsNUDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(239, 84);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(85, 15);
            this.label3.TabIndex = 8;
            this.label3.Text = "카드당 채널수:";
            // 
            // cardNumberNUDown
            // 
            this.cardNumberNUDown.Location = new System.Drawing.Point(330, 49);
            this.cardNumberNUDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.cardNumberNUDown.Minimum = new decimal(new int[] {
            -2147483648,
            0,
            0,
            -2147483648});
            this.cardNumberNUDown.Name = "cardNumberNUDown";
            this.cardNumberNUDown.Size = new System.Drawing.Size(86, 23);
            this.cardNumberNUDown.TabIndex = 7;
            this.cardNumberNUDown.ThousandsSeparator = true;
            this.cardNumberNUDown.ValueChanged += new System.EventHandler(this.cardNumberNUDown_ValueChanged);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(263, 52);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(61, 15);
            this.label2.TabIndex = 6;
            this.label2.Text = "카드 번호:";
            // 
            // downButton
            // 
            this.downButton.Image = global::EOL_GND.Properties.Resources.down_16;
            this.downButton.Location = new System.Drawing.Point(114, 206);
            this.downButton.Name = "downButton";
            this.downButton.Size = new System.Drawing.Size(29, 29);
            this.downButton.TabIndex = 5;
            this.toolTip1.SetToolTip(this.downButton, "아래로");
            this.downButton.UseVisualStyleBackColor = true;
            this.downButton.Click += new System.EventHandler(this.downButton_Click);
            // 
            // upButton
            // 
            this.upButton.Image = global::EOL_GND.Properties.Resources.up_16;
            this.upButton.Location = new System.Drawing.Point(79, 206);
            this.upButton.Name = "upButton";
            this.upButton.Size = new System.Drawing.Size(29, 29);
            this.upButton.TabIndex = 4;
            this.toolTip1.SetToolTip(this.upButton, "위로");
            this.upButton.UseVisualStyleBackColor = true;
            this.upButton.Click += new System.EventHandler(this.upButton_Click);
            // 
            // removeButton
            // 
            this.removeButton.Image = global::EOL_GND.Properties.Resources.minus_16;
            this.removeButton.Location = new System.Drawing.Point(44, 206);
            this.removeButton.Name = "removeButton";
            this.removeButton.Size = new System.Drawing.Size(29, 29);
            this.removeButton.TabIndex = 3;
            this.toolTip1.SetToolTip(this.removeButton, "삭제");
            this.removeButton.UseVisualStyleBackColor = true;
            this.removeButton.Click += new System.EventHandler(this.removeButton_Click);
            // 
            // addButton
            // 
            this.addButton.Image = global::EOL_GND.Properties.Resources.plus_16;
            this.addButton.Location = new System.Drawing.Point(9, 206);
            this.addButton.Name = "addButton";
            this.addButton.Size = new System.Drawing.Size(29, 29);
            this.addButton.TabIndex = 2;
            this.toolTip1.SetToolTip(this.addButton, "추가");
            this.addButton.UseVisualStyleBackColor = true;
            this.addButton.Click += new System.EventHandler(this.addButton_Click);
            // 
            // deviceListView
            // 
            this.deviceListView.AllColumns.Add(this.categoryColumn);
            this.deviceListView.AllColumns.Add(this.channelColumn);
            this.deviceListView.CellEditActivation = BrightIdeasSoftware.ObjectListView.CellEditActivateMode.DoubleClick;
            this.deviceListView.CellEditUseWholeCell = false;
            this.deviceListView.Columns.AddRange(new System.Windows.Forms.ColumnHeader[] {
            this.categoryColumn,
            this.channelColumn});
            this.deviceListView.FullRowSelect = true;
            this.deviceListView.GridLines = true;
            this.deviceListView.HeaderStyle = System.Windows.Forms.ColumnHeaderStyle.None;
            this.deviceListView.HideSelection = false;
            this.deviceListView.Location = new System.Drawing.Point(9, 40);
            this.deviceListView.Name = "deviceListView";
            this.deviceListView.SelectColumnsOnRightClick = false;
            this.deviceListView.SelectColumnsOnRightClickBehaviour = BrightIdeasSoftware.ObjectListView.ColumnSelectBehaviour.None;
            this.deviceListView.ShowFilterMenuOnRightClick = false;
            this.deviceListView.ShowGroups = false;
            this.deviceListView.ShowItemToolTips = true;
            this.deviceListView.Size = new System.Drawing.Size(223, 163);
            this.deviceListView.TabIndex = 1;
            this.deviceListView.UseCompatibleStateImageBehavior = false;
            this.deviceListView.View = System.Windows.Forms.View.Details;
            this.deviceListView.VirtualMode = true;
            this.deviceListView.CellEditFinished += new BrightIdeasSoftware.CellEditEventHandler(this.deviceListView_CellEditFinished);
            this.deviceListView.CellEditValidating += new BrightIdeasSoftware.CellEditEventHandler(this.deviceListView_CellEditValidating);
            this.deviceListView.SelectionChanged += new System.EventHandler(this.deviceListView_SelectionChanged);
            // 
            // categoryColumn
            // 
            this.categoryColumn.AspectName = "Category";
            this.categoryColumn.Groupable = false;
            this.categoryColumn.Sortable = false;
            this.categoryColumn.Text = "Category";
            this.categoryColumn.UseFiltering = false;
            this.categoryColumn.Width = 130;
            // 
            // channelColumn
            // 
            this.channelColumn.AspectName = "ChannelDesc";
            this.channelColumn.Groupable = false;
            this.channelColumn.IsEditable = false;
            this.channelColumn.Sortable = false;
            this.channelColumn.Text = "Channel";
            this.channelColumn.TextAlign = System.Windows.Forms.HorizontalAlignment.Center;
            this.channelColumn.UseFiltering = false;
            this.channelColumn.Width = 87;
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(6, 22);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(698, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "아래의 계측기 순서에 따라 다음과 같이 릴레이 채널 번호가 할당됩니다:  채널 번호 = (카드 번호 - 1) * 카드당 채널 개수 + 시작 번호";
            // 
            // label5
            // 
            this.label5.Location = new System.Drawing.Point(435, 61);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(232, 59);
            this.label5.TabIndex = 13;
            this.label5.Text = "* \'릴레이 채널 번호 재설정\' 버튼을 눌러야 릴레이 카드 설정 내용이 실제로 디바이스 채널들에 적용됩니다.";
            // 
            // DeviceSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(822, 473);
            this.Controls.Add(this.relayChannelsGroupBox);
            this.Controls.Add(this.autoFillCheckBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "DeviceSettingsForm";
            this.Text = "Device";
            this.relayChannelsGroupBox.ResumeLayout(false);
            this.relayChannelsGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.startNumberNUDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardChannelsNUDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.cardNumberNUDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.deviceListView)).EndInit();
            this.ResumeLayout(false);
            this.PerformLayout();

        }

        #endregion

        private System.Windows.Forms.CheckBox autoFillCheckBox;
        private System.Windows.Forms.GroupBox relayChannelsGroupBox;
        private System.Windows.Forms.Label label1;
        private BrightIdeasSoftware.FastObjectListView deviceListView;
        private BrightIdeasSoftware.OLVColumn categoryColumn;
        private System.Windows.Forms.Button removeButton;
        private System.Windows.Forms.Button addButton;
        private System.Windows.Forms.Button downButton;
        private System.Windows.Forms.Button upButton;
        private System.Windows.Forms.ToolTip toolTip1;
        private System.Windows.Forms.NumericUpDown cardNumberNUDown;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.NumericUpDown startNumberNUDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.NumericUpDown cardChannelsNUDown;
        private System.Windows.Forms.Label label3;
        private BrightIdeasSoftware.OLVColumn channelColumn;
        private System.Windows.Forms.Button channelResetButton;
        private System.Windows.Forms.Label label5;
    }
}