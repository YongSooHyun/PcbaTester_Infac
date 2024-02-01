namespace EOL_GND.View
{
    partial class GeneralSettingsForm
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
            System.ComponentModel.ComponentResourceManager resources = new System.ComponentModel.ComponentResourceManager(typeof(GeneralSettingsForm));
            this.groupBox1 = new System.Windows.Forms.GroupBox();
            this.editFontBrowseButton = new System.Windows.Forms.Button();
            this.editFontTextBox = new System.Windows.Forms.TextBox();
            this.label2 = new System.Windows.Forms.Label();
            this.sequenceFontBrowseButton = new System.Windows.Forms.Button();
            this.sequenceFontTextBox = new System.Windows.Forms.TextBox();
            this.label1 = new System.Windows.Forms.Label();
            this.groupBox2 = new System.Windows.Forms.GroupBox();
            this.stepEditorCheckBox = new System.Windows.Forms.CheckBox();
            this.sequenceEditorCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox3 = new System.Windows.Forms.GroupBox();
            this.logFileNameTextBox = new System.Windows.Forms.TextBox();
            this.label4 = new System.Windows.Forms.Label();
            this.logFolderBrowseButton = new System.Windows.Forms.Button();
            this.logFolderTextBox = new System.Windows.Forms.TextBox();
            this.label3 = new System.Windows.Forms.Label();
            this.logFilesCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox4 = new System.Windows.Forms.GroupBox();
            this.logLevelComboBox = new System.Windows.Forms.ComboBox();
            this.label8 = new System.Windows.Forms.Label();
            this.logTargetComboBox = new System.Windows.Forms.ComboBox();
            this.label7 = new System.Windows.Forms.Label();
            this.appLogFileNameTextBox = new System.Windows.Forms.TextBox();
            this.label5 = new System.Windows.Forms.Label();
            this.appLogFolderBrowseButton = new System.Windows.Forms.Button();
            this.appLogFolderTextBox = new System.Windows.Forms.TextBox();
            this.label6 = new System.Windows.Forms.Label();
            this.expertGroupBox = new System.Windows.Forms.GroupBox();
            this.invokeAsyncCheckBox = new System.Windows.Forms.CheckBox();
            this.changeHistoryGroupBox = new System.Windows.Forms.GroupBox();
            this.historyRemarksCheckBox = new System.Windows.Forms.CheckBox();
            this.groupBox1.SuspendLayout();
            this.groupBox2.SuspendLayout();
            this.groupBox3.SuspendLayout();
            this.groupBox4.SuspendLayout();
            this.expertGroupBox.SuspendLayout();
            this.changeHistoryGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // groupBox1
            // 
            this.groupBox1.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox1.Controls.Add(this.editFontBrowseButton);
            this.groupBox1.Controls.Add(this.editFontTextBox);
            this.groupBox1.Controls.Add(this.label2);
            this.groupBox1.Controls.Add(this.sequenceFontBrowseButton);
            this.groupBox1.Controls.Add(this.sequenceFontTextBox);
            this.groupBox1.Controls.Add(this.label1);
            this.groupBox1.Location = new System.Drawing.Point(6, 3);
            this.groupBox1.Name = "groupBox1";
            this.groupBox1.Size = new System.Drawing.Size(824, 67);
            this.groupBox1.TabIndex = 0;
            this.groupBox1.TabStop = false;
            this.groupBox1.Text = "&Font";
            // 
            // editFontBrowseButton
            // 
            this.editFontBrowseButton.Location = new System.Drawing.Point(719, 21);
            this.editFontBrowseButton.Name = "editFontBrowseButton";
            this.editFontBrowseButton.Size = new System.Drawing.Size(27, 25);
            this.editFontBrowseButton.TabIndex = 5;
            this.editFontBrowseButton.Text = "...";
            this.editFontBrowseButton.UseVisualStyleBackColor = true;
            this.editFontBrowseButton.Click += new System.EventHandler(this.editFontBrowseButton_Click);
            // 
            // editFontTextBox
            // 
            this.editFontTextBox.Location = new System.Drawing.Point(468, 22);
            this.editFontTextBox.Name = "editFontTextBox";
            this.editFontTextBox.Size = new System.Drawing.Size(251, 23);
            this.editFontTextBox.TabIndex = 4;
            this.editFontTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.editFontTextBox_KeyUp);
            // 
            // label2
            // 
            this.label2.AutoSize = true;
            this.label2.Location = new System.Drawing.Point(395, 26);
            this.label2.Name = "label2";
            this.label2.Size = new System.Drawing.Size(67, 15);
            this.label2.TabIndex = 3;
            this.label2.Text = "Step &Editor:";
            // 
            // sequenceFontBrowseButton
            // 
            this.sequenceFontBrowseButton.Location = new System.Drawing.Point(359, 21);
            this.sequenceFontBrowseButton.Name = "sequenceFontBrowseButton";
            this.sequenceFontBrowseButton.Size = new System.Drawing.Size(27, 25);
            this.sequenceFontBrowseButton.TabIndex = 2;
            this.sequenceFontBrowseButton.Text = "...";
            this.sequenceFontBrowseButton.UseVisualStyleBackColor = true;
            this.sequenceFontBrowseButton.Click += new System.EventHandler(this.sequenceFontBrowseButton_Click);
            // 
            // sequenceFontTextBox
            // 
            this.sequenceFontTextBox.Location = new System.Drawing.Point(108, 22);
            this.sequenceFontTextBox.Name = "sequenceFontTextBox";
            this.sequenceFontTextBox.Size = new System.Drawing.Size(251, 23);
            this.sequenceFontTextBox.TabIndex = 1;
            this.sequenceFontTextBox.KeyUp += new System.Windows.Forms.KeyEventHandler(this.sequenceFontTextBox_KeyUp);
            // 
            // label1
            // 
            this.label1.AutoSize = true;
            this.label1.Location = new System.Drawing.Point(7, 25);
            this.label1.Name = "label1";
            this.label1.Size = new System.Drawing.Size(95, 15);
            this.label1.TabIndex = 0;
            this.label1.Text = "&Sequence Editor:";
            // 
            // groupBox2
            // 
            this.groupBox2.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox2.Controls.Add(this.stepEditorCheckBox);
            this.groupBox2.Controls.Add(this.sequenceEditorCheckBox);
            this.groupBox2.Location = new System.Drawing.Point(6, 76);
            this.groupBox2.Name = "groupBox2";
            this.groupBox2.Size = new System.Drawing.Size(824, 61);
            this.groupBox2.TabIndex = 1;
            this.groupBox2.TabStop = false;
            this.groupBox2.Text = "&Restore last window state(location and size)";
            // 
            // stepEditorCheckBox
            // 
            this.stepEditorCheckBox.AutoSize = true;
            this.stepEditorCheckBox.Location = new System.Drawing.Point(127, 26);
            this.stepEditorCheckBox.Name = "stepEditorCheckBox";
            this.stepEditorCheckBox.Size = new System.Drawing.Size(83, 19);
            this.stepEditorCheckBox.TabIndex = 1;
            this.stepEditorCheckBox.Text = "Step Editor";
            this.stepEditorCheckBox.UseVisualStyleBackColor = true;
            this.stepEditorCheckBox.CheckedChanged += new System.EventHandler(this.StepEditorCheckBox_CheckedChanged);
            // 
            // sequenceEditorCheckBox
            // 
            this.sequenceEditorCheckBox.AutoSize = true;
            this.sequenceEditorCheckBox.Location = new System.Drawing.Point(10, 26);
            this.sequenceEditorCheckBox.Name = "sequenceEditorCheckBox";
            this.sequenceEditorCheckBox.Size = new System.Drawing.Size(111, 19);
            this.sequenceEditorCheckBox.TabIndex = 0;
            this.sequenceEditorCheckBox.Text = "Sequence Editor";
            this.sequenceEditorCheckBox.UseVisualStyleBackColor = true;
            this.sequenceEditorCheckBox.CheckedChanged += new System.EventHandler(this.SequenceEditorCheckBox_CheckedChanged);
            // 
            // groupBox3
            // 
            this.groupBox3.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox3.Controls.Add(this.logFileNameTextBox);
            this.groupBox3.Controls.Add(this.label4);
            this.groupBox3.Controls.Add(this.logFolderBrowseButton);
            this.groupBox3.Controls.Add(this.logFolderTextBox);
            this.groupBox3.Controls.Add(this.label3);
            this.groupBox3.Controls.Add(this.logFilesCheckBox);
            this.groupBox3.Location = new System.Drawing.Point(6, 143);
            this.groupBox3.Name = "groupBox3";
            this.groupBox3.Size = new System.Drawing.Size(824, 125);
            this.groupBox3.TabIndex = 2;
            this.groupBox3.TabStop = false;
            this.groupBox3.Text = "&Test Log";
            // 
            // logFileNameTextBox
            // 
            this.logFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFileNameTextBox.Location = new System.Drawing.Point(100, 83);
            this.logFileNameTextBox.Name = "logFileNameTextBox";
            this.logFileNameTextBox.Size = new System.Drawing.Size(688, 23);
            this.logFileNameTextBox.TabIndex = 5;
            this.logFileNameTextBox.TextChanged += new System.EventHandler(this.logFileNameTextBox_TextChanged);
            // 
            // label4
            // 
            this.label4.AutoSize = true;
            this.label4.Location = new System.Drawing.Point(7, 86);
            this.label4.Name = "label4";
            this.label4.Size = new System.Drawing.Size(86, 15);
            this.label4.TabIndex = 4;
            this.label4.Text = "Log File Name:";
            // 
            // logFolderBrowseButton
            // 
            this.logFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.logFolderBrowseButton.Location = new System.Drawing.Point(788, 50);
            this.logFolderBrowseButton.Name = "logFolderBrowseButton";
            this.logFolderBrowseButton.Size = new System.Drawing.Size(27, 25);
            this.logFolderBrowseButton.TabIndex = 3;
            this.logFolderBrowseButton.Text = "...";
            this.logFolderBrowseButton.UseVisualStyleBackColor = true;
            this.logFolderBrowseButton.Click += new System.EventHandler(this.logFolderBrowseButton_Click);
            // 
            // logFolderTextBox
            // 
            this.logFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.logFolderTextBox.Location = new System.Drawing.Point(100, 51);
            this.logFolderTextBox.Name = "logFolderTextBox";
            this.logFolderTextBox.Size = new System.Drawing.Size(688, 23);
            this.logFolderTextBox.TabIndex = 2;
            this.logFolderTextBox.TextChanged += new System.EventHandler(this.logFolderTextBox_TextChanged);
            // 
            // label3
            // 
            this.label3.AutoSize = true;
            this.label3.Location = new System.Drawing.Point(7, 54);
            this.label3.Name = "label3";
            this.label3.Size = new System.Drawing.Size(87, 15);
            this.label3.TabIndex = 1;
            this.label3.Text = "Log File Folder:";
            // 
            // logFilesCheckBox
            // 
            this.logFilesCheckBox.AutoSize = true;
            this.logFilesCheckBox.Location = new System.Drawing.Point(10, 26);
            this.logFilesCheckBox.Name = "logFilesCheckBox";
            this.logFilesCheckBox.Size = new System.Drawing.Size(109, 19);
            this.logFilesCheckBox.TabIndex = 0;
            this.logFilesCheckBox.Text = "Create Log Files";
            this.logFilesCheckBox.UseVisualStyleBackColor = true;
            this.logFilesCheckBox.CheckedChanged += new System.EventHandler(this.logFilesCheckBox_CheckedChanged);
            // 
            // groupBox4
            // 
            this.groupBox4.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.groupBox4.Controls.Add(this.logLevelComboBox);
            this.groupBox4.Controls.Add(this.label8);
            this.groupBox4.Controls.Add(this.logTargetComboBox);
            this.groupBox4.Controls.Add(this.label7);
            this.groupBox4.Controls.Add(this.appLogFileNameTextBox);
            this.groupBox4.Controls.Add(this.label5);
            this.groupBox4.Controls.Add(this.appLogFolderBrowseButton);
            this.groupBox4.Controls.Add(this.appLogFolderTextBox);
            this.groupBox4.Controls.Add(this.label6);
            this.groupBox4.Location = new System.Drawing.Point(6, 274);
            this.groupBox4.Name = "groupBox4";
            this.groupBox4.Size = new System.Drawing.Size(824, 125);
            this.groupBox4.TabIndex = 3;
            this.groupBox4.TabStop = false;
            this.groupBox4.Text = "&Application Log";
            // 
            // logLevelComboBox
            // 
            this.logLevelComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logLevelComboBox.FormattingEnabled = true;
            this.logLevelComboBox.Location = new System.Drawing.Point(348, 22);
            this.logLevelComboBox.Name = "logLevelComboBox";
            this.logLevelComboBox.Size = new System.Drawing.Size(121, 23);
            this.logLevelComboBox.TabIndex = 3;
            this.logLevelComboBox.SelectedIndexChanged += new System.EventHandler(this.logLevelComboBox_SelectedIndexChanged);
            // 
            // label8
            // 
            this.label8.AutoSize = true;
            this.label8.Location = new System.Drawing.Point(282, 25);
            this.label8.Name = "label8";
            this.label8.Size = new System.Drawing.Size(60, 15);
            this.label8.TabIndex = 2;
            this.label8.Text = "Log Level:";
            // 
            // logTargetComboBox
            // 
            this.logTargetComboBox.DropDownStyle = System.Windows.Forms.ComboBoxStyle.DropDownList;
            this.logTargetComboBox.FormattingEnabled = true;
            this.logTargetComboBox.Location = new System.Drawing.Point(100, 22);
            this.logTargetComboBox.Name = "logTargetComboBox";
            this.logTargetComboBox.Size = new System.Drawing.Size(121, 23);
            this.logTargetComboBox.TabIndex = 1;
            this.logTargetComboBox.SelectedIndexChanged += new System.EventHandler(this.logTargetComboBox_SelectedIndexChanged);
            // 
            // label7
            // 
            this.label7.AutoSize = true;
            this.label7.Location = new System.Drawing.Point(7, 25);
            this.label7.Name = "label7";
            this.label7.Size = new System.Drawing.Size(65, 15);
            this.label7.TabIndex = 0;
            this.label7.Text = "Log Target:";
            // 
            // appLogFileNameTextBox
            // 
            this.appLogFileNameTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogFileNameTextBox.Location = new System.Drawing.Point(100, 86);
            this.appLogFileNameTextBox.Name = "appLogFileNameTextBox";
            this.appLogFileNameTextBox.Size = new System.Drawing.Size(688, 23);
            this.appLogFileNameTextBox.TabIndex = 8;
            this.appLogFileNameTextBox.TextChanged += new System.EventHandler(this.appLogFileNameTextBox_TextChanged);
            // 
            // label5
            // 
            this.label5.AutoSize = true;
            this.label5.Location = new System.Drawing.Point(7, 89);
            this.label5.Name = "label5";
            this.label5.Size = new System.Drawing.Size(86, 15);
            this.label5.TabIndex = 7;
            this.label5.Text = "Log File Name:";
            // 
            // appLogFolderBrowseButton
            // 
            this.appLogFolderBrowseButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogFolderBrowseButton.Location = new System.Drawing.Point(788, 53);
            this.appLogFolderBrowseButton.Name = "appLogFolderBrowseButton";
            this.appLogFolderBrowseButton.Size = new System.Drawing.Size(27, 25);
            this.appLogFolderBrowseButton.TabIndex = 6;
            this.appLogFolderBrowseButton.Text = "...";
            this.appLogFolderBrowseButton.UseVisualStyleBackColor = true;
            this.appLogFolderBrowseButton.Click += new System.EventHandler(this.appLogFolderBrowseButton_Click);
            // 
            // appLogFolderTextBox
            // 
            this.appLogFolderTextBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.appLogFolderTextBox.Location = new System.Drawing.Point(100, 54);
            this.appLogFolderTextBox.Name = "appLogFolderTextBox";
            this.appLogFolderTextBox.Size = new System.Drawing.Size(688, 23);
            this.appLogFolderTextBox.TabIndex = 5;
            this.appLogFolderTextBox.TextChanged += new System.EventHandler(this.appLogFolderTextBox_TextChanged);
            // 
            // label6
            // 
            this.label6.AutoSize = true;
            this.label6.Location = new System.Drawing.Point(7, 57);
            this.label6.Name = "label6";
            this.label6.Size = new System.Drawing.Size(87, 15);
            this.label6.TabIndex = 4;
            this.label6.Text = "Log File Folder:";
            // 
            // expertGroupBox
            // 
            this.expertGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.expertGroupBox.Controls.Add(this.invokeAsyncCheckBox);
            this.expertGroupBox.Location = new System.Drawing.Point(6, 466);
            this.expertGroupBox.Name = "expertGroupBox";
            this.expertGroupBox.Size = new System.Drawing.Size(824, 55);
            this.expertGroupBox.TabIndex = 5;
            this.expertGroupBox.TabStop = false;
            this.expertGroupBox.Text = "Expert";
            // 
            // invokeAsyncCheckBox
            // 
            this.invokeAsyncCheckBox.AutoSize = true;
            this.invokeAsyncCheckBox.Location = new System.Drawing.Point(10, 24);
            this.invokeAsyncCheckBox.Name = "invokeAsyncCheckBox";
            this.invokeAsyncCheckBox.Size = new System.Drawing.Size(110, 19);
            this.invokeAsyncCheckBox.TabIndex = 0;
            this.invokeAsyncCheckBox.Text = "UI Invoke Async";
            this.invokeAsyncCheckBox.UseVisualStyleBackColor = true;
            this.invokeAsyncCheckBox.CheckedChanged += new System.EventHandler(this.invokeAsyncCheckBox_CheckedChanged);
            // 
            // changeHistoryGroupBox
            // 
            this.changeHistoryGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.changeHistoryGroupBox.Controls.Add(this.historyRemarksCheckBox);
            this.changeHistoryGroupBox.Location = new System.Drawing.Point(6, 405);
            this.changeHistoryGroupBox.Name = "changeHistoryGroupBox";
            this.changeHistoryGroupBox.Size = new System.Drawing.Size(824, 55);
            this.changeHistoryGroupBox.TabIndex = 4;
            this.changeHistoryGroupBox.TabStop = false;
            this.changeHistoryGroupBox.Text = "Change History";
            // 
            // historyRemarksCheckBox
            // 
            this.historyRemarksCheckBox.AutoSize = true;
            this.historyRemarksCheckBox.Location = new System.Drawing.Point(10, 24);
            this.historyRemarksCheckBox.Name = "historyRemarksCheckBox";
            this.historyRemarksCheckBox.Size = new System.Drawing.Size(218, 19);
            this.historyRemarksCheckBox.TabIndex = 0;
            this.historyRemarksCheckBox.Text = "Ask the user to enter history remarks";
            this.historyRemarksCheckBox.UseVisualStyleBackColor = true;
            this.historyRemarksCheckBox.CheckedChanged += new System.EventHandler(this.historyRemarksCheckBox_CheckedChanged);
            // 
            // GeneralSettingsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(833, 535);
            this.Controls.Add(this.changeHistoryGroupBox);
            this.Controls.Add(this.expertGroupBox);
            this.Controls.Add(this.groupBox4);
            this.Controls.Add(this.groupBox3);
            this.Controls.Add(this.groupBox2);
            this.Controls.Add(this.groupBox1);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.Icon = ((System.Drawing.Icon)(resources.GetObject("$this.Icon")));
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.Name = "GeneralSettingsForm";
            this.Text = "General";
            this.groupBox1.ResumeLayout(false);
            this.groupBox1.PerformLayout();
            this.groupBox2.ResumeLayout(false);
            this.groupBox2.PerformLayout();
            this.groupBox3.ResumeLayout(false);
            this.groupBox3.PerformLayout();
            this.groupBox4.ResumeLayout(false);
            this.groupBox4.PerformLayout();
            this.expertGroupBox.ResumeLayout(false);
            this.expertGroupBox.PerformLayout();
            this.changeHistoryGroupBox.ResumeLayout(false);
            this.changeHistoryGroupBox.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox groupBox1;
        private System.Windows.Forms.Label label1;
        private System.Windows.Forms.Button sequenceFontBrowseButton;
        private System.Windows.Forms.TextBox sequenceFontTextBox;
        private System.Windows.Forms.Button editFontBrowseButton;
        private System.Windows.Forms.TextBox editFontTextBox;
        private System.Windows.Forms.Label label2;
        private System.Windows.Forms.GroupBox groupBox2;
        private System.Windows.Forms.CheckBox stepEditorCheckBox;
        private System.Windows.Forms.CheckBox sequenceEditorCheckBox;
        private System.Windows.Forms.GroupBox groupBox3;
        private System.Windows.Forms.CheckBox logFilesCheckBox;
        private System.Windows.Forms.Button logFolderBrowseButton;
        private System.Windows.Forms.TextBox logFolderTextBox;
        private System.Windows.Forms.Label label3;
        private System.Windows.Forms.TextBox logFileNameTextBox;
        private System.Windows.Forms.Label label4;
        private System.Windows.Forms.GroupBox groupBox4;
        private System.Windows.Forms.TextBox appLogFileNameTextBox;
        private System.Windows.Forms.Label label5;
        private System.Windows.Forms.Button appLogFolderBrowseButton;
        private System.Windows.Forms.TextBox appLogFolderTextBox;
        private System.Windows.Forms.Label label6;
        private System.Windows.Forms.ComboBox logTargetComboBox;
        private System.Windows.Forms.Label label7;
        private System.Windows.Forms.ComboBox logLevelComboBox;
        private System.Windows.Forms.Label label8;
        private System.Windows.Forms.GroupBox expertGroupBox;
        private System.Windows.Forms.CheckBox invokeAsyncCheckBox;
        private System.Windows.Forms.GroupBox changeHistoryGroupBox;
        private System.Windows.Forms.CheckBox historyRemarksCheckBox;
    }
}