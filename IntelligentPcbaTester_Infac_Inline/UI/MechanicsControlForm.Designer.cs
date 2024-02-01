namespace IntelligentPcbaTester
{
    partial class MechanicsControlForm
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
            this.stateGroupBox = new System.Windows.Forms.GroupBox();
            this.topClampControl = new IntelligentPcbaTester.OnOffControl();
            this.bottomClampControl = new IntelligentPcbaTester.OnOffControl();
            this.emergencyControl = new IntelligentPcbaTester.OnOffControl();
            this.conveyorControl = new IntelligentPcbaTester.OnOffControl();
            this.safetyControl = new IntelligentPcbaTester.OnOffControl();
            this.pneumaticControl = new IntelligentPcbaTester.OnOffControl();
            this.cylinderBottomControl = new IntelligentPcbaTester.OnOffControl();
            this.cylinderFctControl = new IntelligentPcbaTester.OnOffControl();
            this.cylinderTopControl = new IntelligentPcbaTester.OnOffControl();
            this.pcbControl = new IntelligentPcbaTester.OnOffControl();
            this.topFixtureControl = new IntelligentPcbaTester.OnOffControl();
            this.bottomFixtureControl = new IntelligentPcbaTester.OnOffControl();
            this.rearDoorControl = new IntelligentPcbaTester.OnOffControl();
            this.frontDoorControl = new IntelligentPcbaTester.OnOffControl();
            this.modeControl = new IntelligentPcbaTester.OnOffControl();
            this.modeButton = new System.Windows.Forms.Button();
            this.closeButton = new System.Windows.Forms.Button();
            this.cylInitButton = new System.Windows.Forms.Button();
            this.cylMidUpButton = new System.Windows.Forms.Button();
            this.cylFctUpButton = new System.Windows.Forms.Button();
            this.cylUpButton = new System.Windows.Forms.Button();
            this.cylDownButton = new System.Windows.Forms.Button();
            this.stateGroupBox.SuspendLayout();
            this.SuspendLayout();
            // 
            // stateGroupBox
            // 
            this.stateGroupBox.Anchor = ((System.Windows.Forms.AnchorStyles)(((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.stateGroupBox.Controls.Add(this.topClampControl);
            this.stateGroupBox.Controls.Add(this.bottomClampControl);
            this.stateGroupBox.Controls.Add(this.emergencyControl);
            this.stateGroupBox.Controls.Add(this.conveyorControl);
            this.stateGroupBox.Controls.Add(this.safetyControl);
            this.stateGroupBox.Controls.Add(this.pneumaticControl);
            this.stateGroupBox.Controls.Add(this.cylinderBottomControl);
            this.stateGroupBox.Controls.Add(this.cylinderFctControl);
            this.stateGroupBox.Controls.Add(this.cylinderTopControl);
            this.stateGroupBox.Controls.Add(this.pcbControl);
            this.stateGroupBox.Controls.Add(this.topFixtureControl);
            this.stateGroupBox.Controls.Add(this.bottomFixtureControl);
            this.stateGroupBox.Controls.Add(this.rearDoorControl);
            this.stateGroupBox.Controls.Add(this.frontDoorControl);
            this.stateGroupBox.Controls.Add(this.modeControl);
            this.stateGroupBox.Location = new System.Drawing.Point(12, 12);
            this.stateGroupBox.Name = "stateGroupBox";
            this.stateGroupBox.Size = new System.Drawing.Size(530, 130);
            this.stateGroupBox.TabIndex = 0;
            this.stateGroupBox.TabStop = false;
            this.stateGroupBox.Text = "상태";
            // 
            // topClampControl
            // 
            this.topClampControl.Location = new System.Drawing.Point(424, 90);
            this.topClampControl.Margin = new System.Windows.Forms.Padding(35, 3, 35, 3);
            this.topClampControl.Name = "topClampControl";
            this.topClampControl.OffColor = System.Drawing.Color.Red;
            this.topClampControl.OffTitle = "상부 클램프";
            this.topClampControl.ON = true;
            this.topClampControl.OnTitle = "상부 클램프";
            this.topClampControl.Size = new System.Drawing.Size(91, 28);
            this.topClampControl.TabIndex = 14;
            this.topClampControl.TabStop = false;
            // 
            // bottomClampControl
            // 
            this.bottomClampControl.Location = new System.Drawing.Point(316, 90);
            this.bottomClampControl.Margin = new System.Windows.Forms.Padding(35, 3, 35, 3);
            this.bottomClampControl.Name = "bottomClampControl";
            this.bottomClampControl.OffColor = System.Drawing.Color.Red;
            this.bottomClampControl.OffTitle = "하부 클램프";
            this.bottomClampControl.ON = true;
            this.bottomClampControl.OnTitle = "하부 클램프";
            this.bottomClampControl.Size = new System.Drawing.Size(91, 28);
            this.bottomClampControl.TabIndex = 13;
            this.bottomClampControl.TabStop = false;
            // 
            // emergencyControl
            // 
            this.emergencyControl.Location = new System.Drawing.Point(211, 90);
            this.emergencyControl.Margin = new System.Windows.Forms.Padding(30, 3, 30, 3);
            this.emergencyControl.Name = "emergencyControl";
            this.emergencyControl.OffColor = System.Drawing.Color.LimeGreen;
            this.emergencyControl.OffTitle = "긴급-정상";
            this.emergencyControl.OnColor = System.Drawing.Color.Red;
            this.emergencyControl.OnTitle = "긴급-비정상";
            this.emergencyControl.Size = new System.Drawing.Size(91, 28);
            this.emergencyControl.TabIndex = 12;
            this.emergencyControl.TabStop = false;
            // 
            // conveyorControl
            // 
            this.conveyorControl.Location = new System.Drawing.Point(109, 90);
            this.conveyorControl.Margin = new System.Windows.Forms.Padding(26, 3, 26, 3);
            this.conveyorControl.Name = "conveyorControl";
            this.conveyorControl.OffColor = System.Drawing.Color.Red;
            this.conveyorControl.OffTitle = "C/V 정지";
            this.conveyorControl.ON = true;
            this.conveyorControl.OnTitle = "C/V 작동";
            this.conveyorControl.Size = new System.Drawing.Size(91, 28);
            this.conveyorControl.TabIndex = 11;
            this.conveyorControl.TabStop = false;
            // 
            // safetyControl
            // 
            this.safetyControl.Location = new System.Drawing.Point(9, 90);
            this.safetyControl.Margin = new System.Windows.Forms.Padding(22, 3, 22, 3);
            this.safetyControl.Name = "safetyControl";
            this.safetyControl.OffColor = System.Drawing.Color.Red;
            this.safetyControl.OffTitle = "안전센서";
            this.safetyControl.ON = true;
            this.safetyControl.OnTitle = "안전센서";
            this.safetyControl.Size = new System.Drawing.Size(91, 28);
            this.safetyControl.TabIndex = 10;
            this.safetyControl.TabStop = false;
            // 
            // pneumaticControl
            // 
            this.pneumaticControl.Location = new System.Drawing.Point(424, 56);
            this.pneumaticControl.Margin = new System.Windows.Forms.Padding(19, 3, 19, 3);
            this.pneumaticControl.Name = "pneumaticControl";
            this.pneumaticControl.OffColor = System.Drawing.Color.Red;
            this.pneumaticControl.OffTitle = "공압 비정상";
            this.pneumaticControl.ON = true;
            this.pneumaticControl.OnTitle = "공압 정상";
            this.pneumaticControl.Size = new System.Drawing.Size(91, 28);
            this.pneumaticControl.TabIndex = 9;
            this.pneumaticControl.TabStop = false;
            // 
            // cylinderBottomControl
            // 
            this.cylinderBottomControl.Location = new System.Drawing.Point(316, 56);
            this.cylinderBottomControl.Margin = new System.Windows.Forms.Padding(16, 3, 16, 3);
            this.cylinderBottomControl.Name = "cylinderBottomControl";
            this.cylinderBottomControl.OffColor = System.Drawing.Color.Red;
            this.cylinderBottomControl.OffTitle = "실린더 BOT";
            this.cylinderBottomControl.ON = true;
            this.cylinderBottomControl.OnTitle = "실린더 BOT";
            this.cylinderBottomControl.Size = new System.Drawing.Size(91, 28);
            this.cylinderBottomControl.TabIndex = 8;
            this.cylinderBottomControl.TabStop = false;
            // 
            // cylinderFctControl
            // 
            this.cylinderFctControl.Location = new System.Drawing.Point(211, 56);
            this.cylinderFctControl.Margin = new System.Windows.Forms.Padding(14, 3, 14, 3);
            this.cylinderFctControl.Name = "cylinderFctControl";
            this.cylinderFctControl.OffColor = System.Drawing.Color.Red;
            this.cylinderFctControl.OffTitle = "실린더 FCT";
            this.cylinderFctControl.ON = true;
            this.cylinderFctControl.OnTitle = "실린더 FCT";
            this.cylinderFctControl.Size = new System.Drawing.Size(91, 28);
            this.cylinderFctControl.TabIndex = 7;
            this.cylinderFctControl.TabStop = false;
            // 
            // cylinderTopControl
            // 
            this.cylinderTopControl.Location = new System.Drawing.Point(109, 56);
            this.cylinderTopControl.Margin = new System.Windows.Forms.Padding(12, 3, 12, 3);
            this.cylinderTopControl.Name = "cylinderTopControl";
            this.cylinderTopControl.OffColor = System.Drawing.Color.Red;
            this.cylinderTopControl.OffTitle = "실린더 TOP";
            this.cylinderTopControl.ON = true;
            this.cylinderTopControl.OnTitle = "실린더 TOP";
            this.cylinderTopControl.Size = new System.Drawing.Size(91, 28);
            this.cylinderTopControl.TabIndex = 6;
            this.cylinderTopControl.TabStop = false;
            // 
            // pcbControl
            // 
            this.pcbControl.Location = new System.Drawing.Point(9, 56);
            this.pcbControl.Margin = new System.Windows.Forms.Padding(10, 3, 10, 3);
            this.pcbControl.Name = "pcbControl";
            this.pcbControl.OffColor = System.Drawing.Color.Red;
            this.pcbControl.OffTitle = "PCB 없음";
            this.pcbControl.ON = true;
            this.pcbControl.OnTitle = "PCB 있음";
            this.pcbControl.Size = new System.Drawing.Size(91, 28);
            this.pcbControl.TabIndex = 5;
            this.pcbControl.TabStop = false;
            // 
            // topFixtureControl
            // 
            this.topFixtureControl.Location = new System.Drawing.Point(424, 22);
            this.topFixtureControl.Margin = new System.Windows.Forms.Padding(9, 3, 9, 3);
            this.topFixtureControl.Name = "topFixtureControl";
            this.topFixtureControl.OffColor = System.Drawing.Color.Red;
            this.topFixtureControl.OffTitle = "상부픽스처";
            this.topFixtureControl.ON = true;
            this.topFixtureControl.OnTitle = "상부픽스처";
            this.topFixtureControl.Size = new System.Drawing.Size(91, 28);
            this.topFixtureControl.TabIndex = 4;
            this.topFixtureControl.TabStop = false;
            // 
            // bottomFixtureControl
            // 
            this.bottomFixtureControl.Location = new System.Drawing.Point(316, 22);
            this.bottomFixtureControl.Margin = new System.Windows.Forms.Padding(8, 3, 8, 3);
            this.bottomFixtureControl.Name = "bottomFixtureControl";
            this.bottomFixtureControl.OffColor = System.Drawing.Color.Red;
            this.bottomFixtureControl.OffTitle = "하부픽스처";
            this.bottomFixtureControl.ON = true;
            this.bottomFixtureControl.OnTitle = "하부픽스처";
            this.bottomFixtureControl.Size = new System.Drawing.Size(91, 28);
            this.bottomFixtureControl.TabIndex = 3;
            this.bottomFixtureControl.TabStop = false;
            // 
            // rearDoorControl
            // 
            this.rearDoorControl.Location = new System.Drawing.Point(211, 22);
            this.rearDoorControl.Margin = new System.Windows.Forms.Padding(6, 3, 6, 3);
            this.rearDoorControl.Name = "rearDoorControl";
            this.rearDoorControl.OffColor = System.Drawing.Color.Red;
            this.rearDoorControl.OffTitle = "후면도어";
            this.rearDoorControl.ON = true;
            this.rearDoorControl.OnTitle = "후면도어";
            this.rearDoorControl.Size = new System.Drawing.Size(91, 28);
            this.rearDoorControl.TabIndex = 2;
            this.rearDoorControl.TabStop = false;
            // 
            // frontDoorControl
            // 
            this.frontDoorControl.Location = new System.Drawing.Point(109, 22);
            this.frontDoorControl.Margin = new System.Windows.Forms.Padding(5, 3, 5, 3);
            this.frontDoorControl.Name = "frontDoorControl";
            this.frontDoorControl.OffColor = System.Drawing.Color.Red;
            this.frontDoorControl.OffTitle = "전면도어";
            this.frontDoorControl.ON = true;
            this.frontDoorControl.OnTitle = "전면도어";
            this.frontDoorControl.Size = new System.Drawing.Size(91, 28);
            this.frontDoorControl.TabIndex = 1;
            this.frontDoorControl.TabStop = false;
            // 
            // modeControl
            // 
            this.modeControl.Location = new System.Drawing.Point(9, 22);
            this.modeControl.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.modeControl.Name = "modeControl";
            this.modeControl.OffColor = System.Drawing.Color.Coral;
            this.modeControl.OffTitle = "수동모드";
            this.modeControl.OnTitle = "자동모드";
            this.modeControl.Size = new System.Drawing.Size(91, 28);
            this.modeControl.TabIndex = 0;
            this.modeControl.TabStop = false;
            // 
            // modeButton
            // 
            this.modeButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.modeButton.Location = new System.Drawing.Point(12, 156);
            this.modeButton.Name = "modeButton";
            this.modeButton.Size = new System.Drawing.Size(158, 48);
            this.modeButton.TabIndex = 1;
            this.modeButton.Text = "자동 / 수동 전환";
            this.modeButton.UseVisualStyleBackColor = true;
            this.modeButton.Click += new System.EventHandler(this.modeButton_Click);
            // 
            // closeButton
            // 
            this.closeButton.Anchor = ((System.Windows.Forms.AnchorStyles)((System.Windows.Forms.AnchorStyles.Bottom | System.Windows.Forms.AnchorStyles.Right)));
            this.closeButton.DialogResult = System.Windows.Forms.DialogResult.Cancel;
            this.closeButton.Location = new System.Drawing.Point(542, 331);
            this.closeButton.Name = "closeButton";
            this.closeButton.Size = new System.Drawing.Size(0, 0);
            this.closeButton.TabIndex = 7;
            this.closeButton.TabStop = false;
            this.closeButton.Text = "Close";
            this.closeButton.UseVisualStyleBackColor = true;
            // 
            // cylInitButton
            // 
            this.cylInitButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cylInitButton.Image = global::IntelligentPcbaTester.Properties.Resources.arrow_refresh_32;
            this.cylInitButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cylInitButton.Location = new System.Drawing.Point(198, 283);
            this.cylInitButton.Name = "cylInitButton";
            this.cylInitButton.Size = new System.Drawing.Size(158, 48);
            this.cylInitButton.TabIndex = 6;
            this.cylInitButton.Text = "          실린더 초기화";
            this.cylInitButton.UseVisualStyleBackColor = true;
            this.cylInitButton.Click += new System.EventHandler(this.cylInitButton_Click);
            // 
            // cylMidUpButton
            // 
            this.cylMidUpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cylMidUpButton.Image = global::IntelligentPcbaTester.Properties.Resources.arrow_up_mid_32;
            this.cylMidUpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cylMidUpButton.Location = new System.Drawing.Point(12, 283);
            this.cylMidUpButton.Name = "cylMidUpButton";
            this.cylMidUpButton.Size = new System.Drawing.Size(158, 48);
            this.cylMidUpButton.TabIndex = 5;
            this.cylMidUpButton.Text = "          실린더 MID 상승";
            this.cylMidUpButton.UseVisualStyleBackColor = true;
            this.cylMidUpButton.Click += new System.EventHandler(this.cylMidUpButton_Click);
            // 
            // cylFctUpButton
            // 
            this.cylFctUpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cylFctUpButton.Image = global::IntelligentPcbaTester.Properties.Resources.arrow_up_step_32;
            this.cylFctUpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cylFctUpButton.Location = new System.Drawing.Point(384, 223);
            this.cylFctUpButton.Name = "cylFctUpButton";
            this.cylFctUpButton.Size = new System.Drawing.Size(158, 48);
            this.cylFctUpButton.TabIndex = 4;
            this.cylFctUpButton.Text = "          실린더 FCT 상승";
            this.cylFctUpButton.UseVisualStyleBackColor = true;
            this.cylFctUpButton.Click += new System.EventHandler(this.cylFctUpButton_Click);
            // 
            // cylUpButton
            // 
            this.cylUpButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cylUpButton.Image = global::IntelligentPcbaTester.Properties.Resources.arrow_up_32;
            this.cylUpButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cylUpButton.Location = new System.Drawing.Point(198, 223);
            this.cylUpButton.Name = "cylUpButton";
            this.cylUpButton.Size = new System.Drawing.Size(158, 48);
            this.cylUpButton.TabIndex = 3;
            this.cylUpButton.Text = "          실린더 상승";
            this.cylUpButton.UseVisualStyleBackColor = true;
            this.cylUpButton.Click += new System.EventHandler(this.cylUpButton_Click);
            // 
            // cylDownButton
            // 
            this.cylDownButton.Cursor = System.Windows.Forms.Cursors.Hand;
            this.cylDownButton.Image = global::IntelligentPcbaTester.Properties.Resources.arrow_down_upperline_32;
            this.cylDownButton.ImageAlign = System.Drawing.ContentAlignment.MiddleLeft;
            this.cylDownButton.Location = new System.Drawing.Point(12, 223);
            this.cylDownButton.Name = "cylDownButton";
            this.cylDownButton.Size = new System.Drawing.Size(158, 48);
            this.cylDownButton.TabIndex = 2;
            this.cylDownButton.Text = "          실린더 && C/V 하강";
            this.cylDownButton.UseVisualStyleBackColor = true;
            this.cylDownButton.Click += new System.EventHandler(this.cylDownButton_Click);
            // 
            // MechanicsControlForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.CancelButton = this.closeButton;
            this.ClientSize = new System.Drawing.Size(554, 343);
            this.Controls.Add(this.closeButton);
            this.Controls.Add(this.cylInitButton);
            this.Controls.Add(this.cylMidUpButton);
            this.Controls.Add(this.cylFctUpButton);
            this.Controls.Add(this.cylUpButton);
            this.Controls.Add(this.cylDownButton);
            this.Controls.Add(this.modeButton);
            this.Controls.Add(this.stateGroupBox);
            this.Font = new System.Drawing.Font("Segoe UI", 9F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point, ((byte)(0)));
            this.FormBorderStyle = System.Windows.Forms.FormBorderStyle.FixedDialog;
            this.Margin = new System.Windows.Forms.Padding(4, 3, 4, 3);
            this.MaximizeBox = false;
            this.MinimizeBox = false;
            this.Name = "MechanicsControlForm";
            this.ShowIcon = false;
            this.ShowInTaskbar = false;
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
            this.Text = "기구부 제어";
            this.stateGroupBox.ResumeLayout(false);
            this.ResumeLayout(false);

        }

        #endregion

        private System.Windows.Forms.GroupBox stateGroupBox;
        private OnOffControl modeControl;
        private OnOffControl frontDoorControl;
        private OnOffControl rearDoorControl;
        private OnOffControl bottomFixtureControl;
        private OnOffControl topFixtureControl;
        private OnOffControl pcbControl;
        private OnOffControl cylinderTopControl;
        private OnOffControl cylinderFctControl;
        private OnOffControl cylinderBottomControl;
        private OnOffControl pneumaticControl;
        private OnOffControl safetyControl;
        private OnOffControl conveyorControl;
        private OnOffControl emergencyControl;
        private System.Windows.Forms.Button modeButton;
        private System.Windows.Forms.Button cylDownButton;
        private System.Windows.Forms.Button cylUpButton;
        private System.Windows.Forms.Button cylFctUpButton;
        private System.Windows.Forms.Button cylMidUpButton;
        private System.Windows.Forms.Button cylInitButton;
        private System.Windows.Forms.Button closeButton;
        private OnOffControl topClampControl;
        private OnOffControl bottomClampControl;
    }
}