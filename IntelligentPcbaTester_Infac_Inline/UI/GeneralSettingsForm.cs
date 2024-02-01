using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;
using TestFramework.PluginTestCell;

namespace IntelligentPcbaTester
{
    public partial class GeneralSettingsForm : Form
    {
        private bool isLoading = true;

        public GeneralSettingsForm()
        {
            InitializeComponent();

            probeCountNumericUpDown.Minimum = 0;
            probeCountNumericUpDown.Maximum = int.MaxValue;
            probeCountNumericUpDown.Value = AppSettings.MaxProbeCount;
            if (AppSettings.ProbeCountSyncMode)
            {
                syncRadioButton.Checked = true;
            }
            else
            {
                asyncRadioButton.Checked = true;
            }

            barcodeCheckBox.Checked = AppSettings.ShouldScanBarcode;
            barcodeRetryCountNuDown.Value = AppSettings.BarcodeRetryCount;
            barcodeRetryIntervalNuDown.Value = AppSettings.BarcodeRetryInterval;
            autoRestartCheckBox.Checked = AppSettings.AutoRestartTest;
            pneumaticCheckBox.Checked = AppSettings.CheckPneumatic;
            doorCheckBox.Checked = AppSettings.CheckDoorOpenStatus;
            autoModeCheckBox.Checked = AppSettings.CheckAutoMode;
            fidScanCheckBox.Checked = AppSettings.ShouldCheckFid;

            // 관리자만 설정을 허용.
            if (AppSettings.LoggedUser?.Role == UserRole.관리자)
            {
            }
            else
            {
            }

            testRetryCountNumericUpDown.Value = AppSettings.TestRetryCount;
            testRetryIntervalNumericUpDown.Value = AppSettings.TestRetryInterval;
            ngMasterRetryCountNUDown.Value = AppSettings.NgMasterRetryCount;
            pcbViewCheckBox.Checked = AppSettings.ShowPcbViewer;
            failInfoNotepadCheckBox.Checked = AppSettings.ShowFailInfoNotepad;
            elozLogModeComboBox.DataSource = Enum.GetValues(typeof(Project.LogMode));
            failActionComboBox.SelectedIndex = (int)AppSettings.TestFailAction;
            sectionFailActionComboBox.SelectedIndex = (int)AppSettings.SectionFailAction;

            // Print Options.
            printingOptionsComboBox.DataSource = Enum.GetValues(typeof(PrintingOptions));
            loggingCountNumericUpDown.Value = AppSettings.LoggingStepCount;
            fontSizeNumericUpDown.Value = AppSettings.PrintFontSize;

            // Log options.
            limitAsPercentCheckBox.Checked = AppSettings.ResultValueLimitAsPercent;
            logModeComboBox.DataSource = Enum.GetValues(typeof(LogFileMode));

            // 테스트 결과 사운드.
            soundModeComboBox.SelectedIndex = AppSettings.ResultSoundPlayMode;
            passFileTextBox.Text = AppSettings.ResultPassSoundFile;
            failFileTextBox.Text = AppSettings.ResultFailSoundFile;

            // 차종코드 설정.
            fgcodePosNUDown.Value = AppSettings.CarTypeCodeStartPosition;
            fgcodeLengthNUDown.Value = AppSettings.CarTypeCodeLength;

            // 테스트 이력 보관 기간.
            historyPeriodNUDown.Value = AppSettings.HistoryKeepingPeriod;
            periodUnitComboBox.DataSource = Enum.GetValues(typeof(TimespanUnit));

            // DIO 수동 모드.
            dioManualModeCheckBox.Checked = AppSettings.DioManualMode;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            elozLogModeComboBox.SelectedItem = AppSettings.ElozLogMode;
            printingOptionsComboBox.SelectedItem = AppSettings.PrintingMode;
            logModeComboBox.SelectedItem = AppSettings.LogFileMode;
            periodUnitComboBox.SelectedItem = AppSettings.HistoryKeepingUnit;
            isLoading = false;
        }

        private void probeCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MaxProbeCount = (int)probeCountNumericUpDown.Value;
        }

        private void barcodeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ShouldScanBarcode = barcodeCheckBox.Checked;
        }

        private void autoRestartCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.AutoRestartTest = autoRestartCheckBox.Checked;
        }

        private void autoModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.CheckAutoMode = autoModeCheckBox.Checked;
        }

        private void elozLogModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                AppSettings.ElozLogMode = (Project.LogMode)elozLogModeComboBox.SelectedItem;
            }
        }

        private void printingOptionsComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                AppSettings.PrintingMode = (PrintingOptions)printingOptionsComboBox.SelectedItem;
            }
        }

        private void loggingCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.LoggingStepCount = (int)loggingCountNumericUpDown.Value;
        }

        private void fontSizeNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.PrintFontSize = (int)fontSizeNumericUpDown.Value;
        }

        private void testRetryCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.TestRetryCount = (int)testRetryCountNumericUpDown.Value;
        }

        private void testRetryIntervalNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.TestRetryInterval = (int)testRetryIntervalNumericUpDown.Value;
        }

        private void ngMasterRetryCountNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.NgMasterRetryCount = (int)ngMasterRetryCountNUDown.Value;
        }

        private void pneumaticCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.CheckPneumatic = pneumaticCheckBox.Checked;
        }

        private void doorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.CheckDoorOpenStatus = doorCheckBox.Checked;
        }

        private void syncRadioButton_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ProbeCountSyncMode = syncRadioButton.Checked;
        }

        private void limitAsPercentCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ResultValueLimitAsPercent = limitAsPercentCheckBox.Checked;
        }

        private void soundModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            soundFileGroupBox.Enabled = soundModeComboBox.SelectedIndex == 0;
            AppSettings.ResultSoundPlayMode = soundModeComboBox.SelectedIndex;
        }

        private void passFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                passFileTextBox.Text = path;
                AppSettings.ResultPassSoundFile = path;
            }
        }

        private void failFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                failFileTextBox.Text = path;
                AppSettings.ResultFailSoundFile = path;
            }
        }

        private void barcodeRetryCountNuDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.BarcodeRetryCount = (int)barcodeRetryCountNuDown.Value;
        }

        private void barcodeRetryIntervalNuDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.BarcodeRetryInterval = (int)barcodeRetryIntervalNuDown.Value;
        }

        private void fidScanCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ShouldCheckFid = fidScanCheckBox.Checked;
        }

        private void logModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                AppSettings.LogFileMode = (LogFileMode)logModeComboBox.SelectedItem;
            }
        }

        private void fgcodePosNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.CarTypeCodeStartPosition = (int)fgcodePosNUDown.Value;
        }

        private void fgcodeLengthNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.CarTypeCodeLength = (int)fgcodeLengthNUDown.Value;
        }

        private void pcbViewCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ShowPcbViewer = pcbViewCheckBox.Checked;
        }

        private void failInfoNotepadCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.ShowFailInfoNotepad = failInfoNotepadCheckBox.Checked;
        }

        private void periodUnitComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                AppSettings.HistoryKeepingUnit = (TimespanUnit)periodUnitComboBox.SelectedItem;
            }
        }

        private void historyPeriodNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.HistoryKeepingPeriod = (int)historyPeriodNUDown.Value;
        }

        private void failActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = failActionComboBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                AppSettings.TestFailAction = (TestFailAction)selectedIndex;
            }
        }

        private void sectionFailActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            int selectedIndex = sectionFailActionComboBox.SelectedIndex;
            if (selectedIndex >= 0)
            {
                AppSettings.SectionFailAction = (SectionFailAction)selectedIndex;
            }
        }

        private void dioManualModeCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.DioManualMode = dioManualModeCheckBox.Checked;
        }
    }
}
