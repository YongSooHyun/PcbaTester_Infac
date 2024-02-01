
namespace IntelligentPcbaTester
{
    partial class SeriesSettingsForm
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
            this.generalGroupBox = new System.Windows.Forms.GroupBox();
            this.seriesOptionComboBox = new System.Windows.Forms.ComboBox();
            this.label1 = new System.Windows.Forms.Label();
            this.commGroupBox = new System.Windows.Forms.GroupBox();
            this.serverNameTextBox = new System.Windows.Forms.TextBox();
            this.serverLabel = new System.Windows.Forms.Label();
            this.label5 = new System.Windows.Forms.Label();
            this.timeoutNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label4 = new System.Windows.Forms.Label();
            this.portNumericUpDown = new System.Windows.Forms.NumericUpDown();
            this.label3 = new System.Windows.Forms.Label();
            this.debugGroupBox = new System.Windows.Forms.GroupBox();
            this.serverGroupBox = new System.Windows.Forms.GroupBox();
            this.startButton = new System.Windows.Forms.Button();
            this.stopButton = new System.Windows.Forms.Button();
            this.clientGroupBox = new System.Windows.Forms.GroupBox();
            this.connectButton = new System.Windows.Forms.Button();
            this.disconnectButton = new System.Windows.Forms.Button();
            this.label2 = new System.Windows.Forms.Label();
            this.requestComboBox = new System.Windows.Forms.ComboBox();
            this.sendButton = new System.Windows.Forms.Button();
            this.label6 = new System.Windows.Forms.Label();
            this.serverStatusLabel = new System.Windows.Forms.Label();
            this.connectionStatusLabel = new System.Windows.Forms.Label();
            this.label8 = new System.Windows.Forms.Label();
            this.generalGroupBox.SuspendLayout();
            this.commGroupBox.SuspendLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).BeginInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).BeginInit();
            this.debugGroupBox.SuspendLayout();
            this.serverGroupBox.SuspendLayout();
            this.clientGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // generalGroupBox
            // 
            this.generalGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.generalGroupBox.Controls.Add(this.seriesOptionComboBox);
            this.generalGroupBox.Controls.Add(this.label1);
            this.generalGroupBox.Location = new System.Drawing.Point(3, 3);
            this.generalGroupBox.Name = "generalGroupBox";
            this.generalGroupBox.Size = new System.Drawing.Size(926, 61);
            this.generalGroupBox.TabIndex = 0;
            this.generalGroupBox.TabStop = false;
            this.generalGroupBox.Text = "일반";
            // 
            // seriesOptionComboBox
            // 
            this.seriesOptionComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.seriesOptionComboBox.FormattingEnabled = true;
            this.seriesOptionComboBox.Items.AddRange(new object[] {
            "직렬연결 안 함",
            "직렬연결 앞설비",
            "직렬연결 뒷설비"});
            this.seriesOptionComboBox.Location = new System.Drawing.Point(105, 24);
            this.seriesOptionComboBox.Name = "seriesOptionComboBox";
            this.seriesOptionComboBox.Size = new System.Drawing.Size(174, 23);
            this.seriesOptionComboBox.TabIndex = 1;
            this.seriesOptionComboBox.SelectedIndexChanged += new System.EventHandler(this.seriesOptionComboBox_SelectedIndexChanged);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(11, 28);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(88, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "직렬 연결 설정:";
            // 
            // commGroupBox
            // 
            this.commGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.commGroupBox.Controls.Add(this.serverNameTextBox);
            this.commGroupBox.Controls.Add(this.serverLabel);
            this.commGroupBox.Controls.Add(this.label5);
            this.commGroupBox.Controls.Add(this.timeoutNumericUpDown);
            this.commGroupBox.Controls.Add(this.label4);
            this.commGroupBox.Controls.Add(this.portNumericUpDown);
            this.commGroupBox.Controls.Add(this.label3);
            this.commGroupBox.Location = new System.Drawing.Point(3, 70);
            this.commGroupBox.Name = "commGroupBox";
            this.commGroupBox.Size = new System.Drawing.Size(926, 64);
            this.commGroupBox.TabIndex = 1;
            this.commGroupBox.TabStop = false;
            this.commGroupBox.Text = "통신";
            // 
            // serverNameTextBox
            // 
            this.serverNameTextBox.Location = new System.Drawing.Point(78, 24);
            this.serverNameTextBox.Name = "serverNameTextBox";
            this.serverNameTextBox.Size = new System.Drawing.Size(128, 23);
            this.serverNameTextBox.TabIndex = 1;
            this.serverNameTextBox.TextChanged += new System.EventHandler(this.serverNameTextBox_TextChanged);
            // 
            // serverLabel
            // 
            this.serverLabel.AutoSize = true;
            this.serverLabel.Location = new System.Drawing.Point(11, 28);
            this.serverLabel.Name = "serverLabel";
            this.serverLabel.Size = new System.Drawing.Size(61, 15);
            this.serverLabel.TabIndex = 0;
            this.serverLabel.Text = "서버 주소:";
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(563, 27);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(23, 15);
            this.label5.TabIndex = 6;
            this.label5.Text = "ms";
            // 
            // timeoutNumericUpDown
            // 
            this.timeoutNumericUpDown.Location = new System.Drawing.Point(466, 24);
            this.timeoutNumericUpDown.Maximum = new decimal(new int[] {
            2147483647,
            0,
            0,
            0});
            this.timeoutNumericUpDown.Name = "timeoutNumericUpDown";
            this.timeoutNumericUpDown.Size = new System.Drawing.Size(94, 23);
            this.timeoutNumericUpDown.TabIndex = 5;
            this.timeoutNumericUpDown.TextAlign = System.Windows.Forms.HorizontalAlignment.Right;
            this.timeoutNumericUpDown.ThousandsSeparator = true;
            this.timeoutNumericUpDown.ValueChanged += new System.EventHandler(this.timeoutNumericUpDown_ValueChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(375, 28);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(85, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "통신 타임아웃:";
            // 
            // portNumericUpDown
            // 
            this.portNumericUpDown.Location = new System.Drawing.Point(273, 24);
            this.portNumericUpDown.Maximum = new decimal(new int[] {
            65535,
            0,
            0,
            0});
            this.portNumericUpDown.Name = "portNumericUpDown";
            this.portNumericUpDown.Size = new System.Drawing.Size(75, 23);
            this.portNumericUpDown.TabIndex = 3;
            this.portNumericUpDown.ThousandsSeparator = true;
            this.portNumericUpDown.ValueChanged += new System.EventHandler(this.portNumericUpDown_ValueChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(233, 28);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(34, 15);
            this.label3.TabIndex = 2;
            this.label3.Text = "포트:";
            // 
            // debugGroupBox
            // 
            this.debugGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.debugGroupBox.Controls.Add(this.clientGroupBox);
            this.debugGroupBox.Controls.Add(this.serverGroupBox);
            this.debugGroupBox.Location = new System.Drawing.Point(3, 140);
            this.debugGroupBox.Name = "debugGroupBox";
            this.debugGroupBox.Size = new System.Drawing.Size(926, 191);
            this.debugGroupBox.TabIndex = 2;
            this.debugGroupBox.TabStop = false;
            this.debugGroupBox.Text = "디버깅";
            // 
            // serverGroupBox
            // 
            this.serverGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.serverGroupBox.Controls.Add(this.serverStatusLabel);
            this.serverGroupBox.Controls.Add(this.label6);
            this.serverGroupBox.Controls.Add(this.stopButton);
            this.serverGroupBox.Controls.Add(this.startButton);
            this.serverGroupBox.Location = new System.Drawing.Point(14, 22);
            this.serverGroupBox.Name = "serverGroupBox";
            this.serverGroupBox.Size = new System.Drawing.Size(898, 75);
            this.serverGroupBox.TabIndex = 0;
            this.serverGroupBox.TabStop = false;
            this.serverGroupBox.Text = "서버(뒷설비)";
            // 
            // startButton
            // 
            this.startButton.Location = new System.Drawing.Point(15, 29);
            this.startButton.Name = "startButton";
            this.startButton.Size = new System.Drawing.Size(110, 29);
            this.startButton.TabIndex = 0;
            this.startButton.Text = "서버 시작";
            this.startButton.UseVisualStyleBackColor = true;
            this.startButton.Click += new System.EventHandler(this.startButton_Click);
            // 
            // stopButton
            // 
            this.stopButton.Location = new System.Drawing.Point(131, 29);
            this.stopButton.Name = "stopButton";
            this.stopButton.Size = new System.Drawing.Size(110, 29);
            this.stopButton.TabIndex = 1;
            this.stopButton.Text = "서버 종료";
            this.stopButton.UseVisualStyleBackColor = true;
            this.stopButton.Click += new System.EventHandler(this.stopButton_Click);
            // 
            // clientGroupBox
            // 
            this.clientGroupBox.Controls.Add(this.connectionStatusLabel);
            this.clientGroupBox.Controls.Add(this.label8);
            this.clientGroupBox.Controls.Add(this.sendButton);
            this.clientGroupBox.Controls.Add(this.requestComboBox);
            this.clientGroupBox.Controls.Add(this.label2);
            this.clientGroupBox.Controls.Add(this.disconnectButton);
            this.clientGroupBox.Controls.Add(this.connectButton);
            this.clientGroupBox.Location = new System.Drawing.Point(14, 103);
            this.clientGroupBox.Name = "clientGroupBox";
            this.clientGroupBox.Size = new System.Drawing.Size(898, 75);
            this.clientGroupBox.TabIndex = 1;
            this.clientGroupBox.TabStop = false;
            this.clientGroupBox.Text = "클라이언트(앞설비)";
            // 
            // connectButton
            // 
            this.connectButton.Location = new System.Drawing.Point(15, 29);
            this.connectButton.Name = "connectButton";
            this.connectButton.Size = new System.Drawing.Size(110, 29);
            this.connectButton.TabIndex = 0;
            this.connectButton.Text = "서버 연결";
            this.connectButton.UseVisualStyleBackColor = true;
            this.connectButton.Click += new System.EventHandler(this.connectButton_Click);
            // 
            // disconnectButton
            // 
            this.disconnectButton.Location = new System.Drawing.Point(131, 29);
            this.disconnectButton.Name = "disconnectButton";
            this.disconnectButton.Size = new System.Drawing.Size(110, 29);
            this.disconnectButton.TabIndex = 1;
            this.disconnectButton.Text = "연결 종료";
            this.disconnectButton.UseVisualStyleBackColor = true;
            this.disconnectButton.Click += new System.EventHandler(this.disconnectButton_Click);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(408, 36);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(34, 15);
            this.label2.TabIndex = 4;
            this.label2.Text = "요청:";
            // 
            // requestComboBox
            // 
            this.requestComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.requestComboBox.FormattingEnabled = true;
            this.requestComboBox.Location = new System.Drawing.Point(448, 32);
            this.requestComboBox.Name = "requestComboBox";
            this.requestComboBox.Size = new System.Drawing.Size(136, 23);
            this.requestComboBox.TabIndex = 5;
            // 
            // sendButton
            // 
            this.sendButton.Location = new System.Drawing.Point(590, 29);
            this.sendButton.Name = "sendButton";
            this.sendButton.Size = new System.Drawing.Size(110, 29);
            this.sendButton.TabIndex = 6;
            this.sendButton.Text = "요청 전송";
            this.sendButton.UseVisualStyleBackColor = true;
            this.sendButton.Click += new System.EventHandler(this.sendButton_Click);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(253, 36);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(61, 15);
            this.label6.TabIndex = 2;
            this.label6.Text = "서버 상태:";
            // 
            // serverStatusLabel
            // 
            this.serverStatusLabel.AutoSize = true;
            this.serverStatusLabel.Location = new System.Drawing.Point(320, 36);
            this.serverStatusLabel.Name = "serverStatusLabel";
            this.serverStatusLabel.Size = new System.Drawing.Size(43, 15);
            this.serverStatusLabel.TabIndex = 3;
            this.serverStatusLabel.Text = "중지됨";
            // 
            // connectionStatusLabel
            // 
            this.connectionStatusLabel.AutoSize = true;
            this.connectionStatusLabel.Location = new System.Drawing.Point(320, 36);
            this.connectionStatusLabel.Name = "connectionStatusLabel";
            this.connectionStatusLabel.Size = new System.Drawing.Size(43, 15);
            this.connectionStatusLabel.TabIndex = 3;
            this.connectionStatusLabel.Text = "끊어짐";
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(253, 36);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(61, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "연결 상태:";
            // 
            // SeriesSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.AutoScroll = true;
            this.ClientSize = new System.Drawing.Size(933, 831);
            this.Controls.Add(this.debugGroupBox);
            this.Controls.Add(this.commGroupBox);
            this.Controls.Add(this.generalGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Name = "SeriesSettingsForm";
            this.Text = "직렬 연결";
            this.generalGroupBox.ResumeLayout(false);
            this.generalGroupBox.PerformLayout();
            this.commGroupBox.ResumeLayout(false);
            this.commGroupBox.PerformLayout();
            ((System.ComponentModel.ISupportInitialize)(this.timeoutNumericUpDown)).EndInit();
            ((System.ComponentModel.ISupportInitialize)(this.portNumericUpDown)).EndInit();
            this.debugGroupBox.ResumeLayout(false);
            this.serverGroupBox.ResumeLayout(false);
            this.serverGroupBox.PerformLayout();
            this.clientGroupBox.ResumeLayout(false);
            this.clientGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox generalGroupBox;
        private System.Windows.Forms.GroupBox commGroupBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.NumericUpDown portNumericUpDown;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.NumericUpDown timeoutNumericUpDown;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox debugGroupBox;
        private System.Windows.Forms.TextBox serverNameTextBox;
        private System.Windows.Forms.Label serverLabel;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.ComboBox seriesOptionComboBox;
        private System.Windows.Forms.GroupBox serverGroupBox;
        private System.Windows.Forms.Button stopButton;
        private System.Windows.Forms.Button startButton;
        private System.Windows.Forms.GroupBox clientGroupBox;
        private System.Windows.Forms.Button disconnectButton;
        private System.Windows.Forms.Button connectButton;
        private System.Windows.Forms.ComboBox requestComboBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.Button sendButton;
        private System.Windows.Forms.Label serverStatusLabel;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.Label connectionStatusLabel;
        private System.Windows.Forms.Label label8;
    }
}