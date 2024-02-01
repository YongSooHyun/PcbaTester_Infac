using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class MesSettingsForm : Form
    {
        // DataSource가 있는 ComboBox의 SelectedItem을 설정할 때 이벤트 처리를 막기 위한 변수.
        private bool isLoading = true;

        public MesSettingsForm()
        {
            InitializeComponent();

            enabledCheckBox.Checked = AppSettings.MesEnabled;
            prevFailActionComboBox.SelectedIndex = (int)AppSettings.PrevFailAction;
            testFailActionComboBox.SelectedIndex = (int)AppSettings.MesReportAction;
            testFailActionLabel.Enabled = enabledCheckBox.Checked;
            testFailActionComboBox.Enabled = enabledCheckBox.Checked;

            ictPortNUDown.Value = AppSettings.MesIctServerPort;
            eolPortNUDown.Value = AppSettings.MesEolServerPort;
            keepAliveTimeoutNumericUpDown.Value = AppSettings.MesKeepAliveTimeout;
            t3TimeoutNumericUpDown.Value = AppSettings.MesT3Timeout;
            UpdateServerStatus();

            messageIdTextBox.Text = AppSettings.MesMessageId;
            pcIdTextBox.Text = AppSettings.MesPcId;
            factoryIdTextBox.Text = AppSettings.MesFactoryId;
            lineIdTextBox.Text = AppSettings.MesLineId;
            operIdTextBox.Text = AppSettings.MesOperId;
            equipmentIdTextBox.Text = AppSettings.MesEquipmentId;
            barcodeTypeComboBox.DataSource = Enum.GetValues(typeof(MesMessage.Barcode));

            m1RetryCountNumericUpDown.Value = AppSettings.MesM1RetryCount;
            m1RetryIntervalNumericUpDown.Value = AppSettings.MesM1RetryInterval;
            m3RetryCountNumericUpDown.Value = AppSettings.MesM3RetryCount;
            m3RetryIntervalNumericUpDown.Value = AppSettings.MesM3RetryInterval;

            fgcodeCheckComboBox.SelectedIndex = (int)AppSettings.FgcodeCheckMethod;

            // MES 결과 사운드.
            soundModeComboBox.SelectedIndex = AppSettings.MesSoundPlayMode;
            m2OkFileTextBox.Text = AppSettings.MesM2OkSoundFile;
            m2NgFileTextBox.Text = AppSettings.MesM2NgSoundFile;
            m4OkFileTextBox.Text = AppSettings.MesM4OkSoundFile;
            m4NgFileTextBox.Text = AppSettings.MesM4NgSoundFile;
        }

        private void UpdateServerStatus()
        {
            ictServerStatusLabel.Text = MesServer.SharedIctServer.IsRunning ? "실행 중" : "중지됨";
            ictServerStartButton.Enabled = !MesServer.SharedIctServer.IsRunning;
            ictServerStopButton.Enabled = MesServer.SharedIctServer.IsRunning;

            eolServerStatusLabel.Text = MesServer.SharedEolServer.IsRunning ? "실행 중" : "중지됨";
            eolServerStartButton.Enabled = !MesServer.SharedEolServer.IsRunning;
            eolServerStopButton.Enabled = MesServer.SharedEolServer.IsRunning;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            barcodeTypeComboBox.SelectedItem = AppSettings.MesBarcodeType;
            isLoading = false;
        }

        private void enabledCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            AppSettings.MesEnabled = enabledCheckBox.Checked;
            prevFailActionLabel.Enabled = enabledCheckBox.Checked;
            prevFailActionComboBox.Enabled = enabledCheckBox.Checked;
            testFailActionLabel.Enabled = enabledCheckBox.Checked;
            testFailActionComboBox.Enabled = enabledCheckBox.Checked;
        }

        private void ictPortNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesIctServerPort = (int)ictPortNUDown.Value;
        }

        private void eolPortNUDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesEolServerPort = (int)eolPortNUDown.Value;
        }

        private void ictServerStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                MesServer.SharedIctServer.StartServer(AppSettings.MesIctServerPort);

                // 서버가 시작될 때까지 기다렸다가 업데이트 한다.
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES ICT Server Error: {ex.Message}");
                Logger.LogDebugInfo(ex.StackTrace);
                InformationBox.Show(ex.Message, "MES ICT 서버 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }

            UpdateServerStatus();
        }

        private void ictServerStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                MesServer.SharedIctServer.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES ICT Server Error: {ex.Message}");
                Logger.LogDebugInfo(ex.StackTrace);
                InformationBox.Show(ex.Message, "MES ICT 서버 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }

            UpdateServerStatus();
        }

        private void eolServerStartButton_Click(object sender, EventArgs e)
        {
            try
            {
                MesServer.SharedEolServer.StartServer(AppSettings.MesEolServerPort);

                // 서버가 시작될 때까지 기다렸다가 업데이트 한다.
                Thread.Sleep(100);
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES EOL Server Error: {ex.Message}");
                Logger.LogDebugInfo(ex.StackTrace);
                InformationBox.Show(ex.Message, "MES EOL 서버 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }

            UpdateServerStatus();
        }

        private void eolServerStopButton_Click(object sender, EventArgs e)
        {
            try
            {
                MesServer.SharedEolServer.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES EOL Server Error: {ex.Message}");
                Logger.LogDebugInfo(ex.StackTrace);
                InformationBox.Show(ex.Message, "MES EOL 서버 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }

            UpdateServerStatus();
        }

        private void keepAliveTimeoutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesKeepAliveTimeout = (int)keepAliveTimeoutNumericUpDown.Value;
        }

        private void t3TimeoutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesT3Timeout = (int)t3TimeoutNumericUpDown.Value;
        }

        private void messageIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesMessageId = messageIdTextBox.Text;
        }

        private void pcIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesPcId = pcIdTextBox.Text;
        }

        private void factoryIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesFactoryId = factoryIdTextBox.Text;
        }

        private void lineIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesLineId = lineIdTextBox.Text;
        }

        private void operIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesOperId = operIdTextBox.Text;
        }

        private void equipmentIdTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.MesEquipmentId = equipmentIdTextBox.Text;
        }

        private void barcodeTypeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                AppSettings.MesBarcodeType = (MesMessage.Barcode)barcodeTypeComboBox.SelectedItem;
            }
        }

        private void m1RetryCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesM1RetryCount = (int)m1RetryCountNumericUpDown.Value;
        }

        private void m3RetryCountNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesM3RetryCount = (int)m3RetryCountNumericUpDown.Value;
        }

        private void m1RetryIntervalNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesM1RetryInterval = (int)m1RetryIntervalNumericUpDown.Value;
        }

        private void m3RetryIntervalNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.MesM3RetryInterval = (int)m3RetryIntervalNumericUpDown.Value;
        }

        private void prevFailActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.PrevFailAction = (PrevProcFailAction)prevFailActionComboBox.SelectedIndex;
        }

        private void testFailActionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.MesReportAction = (MesResultAction)testFailActionComboBox.SelectedIndex;
        }

        private async void ictM1SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string barcode = barcodeTextBox.Text;
                if (string.IsNullOrEmpty(barcode))
                {
                    InformationBox.Show("바코드를 입력하세요.", "바코드 입력 오류", icon: InformationBoxIcon.Error);
                    return;
                }

                ictM1SendButton.Enabled = false;
                ictM3SendButton.Enabled = false;

                await Task.Run(() =>
                {
                    // M1 메시지 전송.
                    MainViewModel.MesIctSendM1Message(barcode, out MesResponseMessage response);

                    // 받은 메시지 출력.
                    if (response != null)
                    {
                        Logger.LogTimedMessage($"{response.MessageId}/{response.PcId}/{response.ProcessFlag}/{response.FactoryId}/{response.LineId}/{response.OperId}/" +
                            $"{response.TransactionTime:yyyyMMddHHmmss}/{response.Step}/{response.EquipmentId}/{response.BarcodeType}/{response.BarcodeNo}/{response.TagId}/" +
                            $"{response.Status}/{response.InspSeq}/{response.InspData}/{response.Prod}/{response.MesResult}/{response.UserDefined1}/{response.UserDefined2}/" +
                            $"{response.UserDefined3}/{response.UserDefined4}/{response.UserDefined5}/{response.MesMessageCode}/{response.MesMessageContent}");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES ICT M1 전송 오류: {ex}");
            }
            finally
            {
                ictM1SendButton.Enabled = true;
                ictM3SendButton.Enabled = true;
            }
        }

        private async void ictM3SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string barcode = barcodeTextBox.Text;
                if (string.IsNullOrEmpty(barcode))
                {
                    InformationBox.Show("바코드를 입력하세요.", "바코드 입력 오류", icon: InformationBoxIcon.Error);
                    return;
                }

                ictM1SendButton.Enabled = false;
                ictM3SendButton.Enabled = false;

                await Task.Run(() =>
                {
                    // M1 메시지 전송.
                    MainViewModel.MesIctSendM3Message(barcode, passedRadioButton.Checked, out MesResponseMessage response);

                    // 받은 메시지 출력.
                    if (response != null)
                    {
                        Logger.LogTimedMessage($"{response.MessageId}/{response.PcId}/{response.ProcessFlag}/{response.FactoryId}/{response.LineId}/{response.OperId}/" +
                            $"{response.TransactionTime:yyyyMMddHHmmss}/{response.Step}/{response.EquipmentId}/{response.BarcodeType}/{response.BarcodeNo}/{response.TagId}/" +
                            $"{response.Status}/{response.InspSeq}/{response.InspData}/{response.Prod}/{response.MesResult}/{response.UserDefined1}/{response.UserDefined2}/" +
                            $"{response.UserDefined3}/{response.UserDefined4}/{response.UserDefined5}/{response.MesMessageCode}/{response.MesMessageContent}");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES ICT M3 전송 오류: {ex}");
            }
            finally
            {
                ictM1SendButton.Enabled = true;
                ictM3SendButton.Enabled = true;
            }
        }

        private async void eolM1SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string barcode = barcodeTextBox.Text;
                if (string.IsNullOrEmpty(barcode))
                {
                    InformationBox.Show("바코드를 입력하세요.", "바코드 입력 오류", icon: InformationBoxIcon.Error);
                    return;
                }

                eolM1SendButton.Enabled = false;
                eolM3SendButton.Enabled = false;

                await Task.Run(() =>
                {
                    // M1 메시지 전송.
                    MainViewModel.MesEolSendM1Message(barcode, out MesResponseMessage response);

                    // 받은 메시지 출력.
                    if (response != null)
                    {
                        Logger.LogTimedMessage($"{response.MessageId}/{response.PcId}/{response.ProcessFlag}/{response.FactoryId}/{response.LineId}/{response.OperId}/" +
                            $"{response.TransactionTime:yyyyMMddHHmmss}/{response.Step}/{response.EquipmentId}/{response.BarcodeType}/{response.BarcodeNo}/{response.TagId}/" +
                            $"{response.Status}/{response.InspSeq}/{response.InspData}/{response.Prod}/{response.MesResult}/{response.UserDefined1}/{response.UserDefined2}/" +
                            $"{response.UserDefined3}/{response.UserDefined4}/{response.UserDefined5}/{response.MesMessageCode}/{response.MesMessageContent}");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES EOL M1 전송 오류: {ex}");
            }
            finally
            {
                eolM1SendButton.Enabled = true;
                eolM3SendButton.Enabled = true;
            }
        }

        private async void eolM3SendButton_Click(object sender, EventArgs e)
        {
            try
            {
                string barcode = barcodeTextBox.Text;
                if (string.IsNullOrEmpty(barcode))
                {
                    InformationBox.Show("바코드를 입력하세요.", "바코드 입력 오류", icon: InformationBoxIcon.Error);
                    return;
                }

                eolM1SendButton.Enabled = false;
                eolM3SendButton.Enabled = false;

                await Task.Run(() =>
                {
                    // M1 메시지 전송.
                    MainViewModel.MesEolSendM3Message(barcode, passedRadioButton.Checked, out MesResponseMessage response);

                    // 받은 메시지 출력.
                    if (response != null)
                    {
                        Logger.LogTimedMessage($"{response.MessageId}/{response.PcId}/{response.ProcessFlag}/{response.FactoryId}/{response.LineId}/{response.OperId}/" +
                            $"{response.TransactionTime:yyyyMMddHHmmss}/{response.Step}/{response.EquipmentId}/{response.BarcodeType}/{response.BarcodeNo}/{response.TagId}/" +
                            $"{response.Status}/{response.InspSeq}/{response.InspData}/{response.Prod}/{response.MesResult}/{response.UserDefined1}/{response.UserDefined2}/" +
                            $"{response.UserDefined3}/{response.UserDefined4}/{response.UserDefined5}/{response.MesMessageCode}/{response.MesMessageContent}");
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"MES EOL M3 전송 오류: {ex}");
            }
            finally
            {
                eolM1SendButton.Enabled = true;
                eolM3SendButton.Enabled = true;
            }
        }

        private void fgcodeCheckComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            var fgcodeCheckMode = (FgcodeCheckMode)fgcodeCheckComboBox.SelectedIndex;
            AppSettings.FgcodeCheckMethod = fgcodeCheckMode;
        }

        private void m2OkFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                m2OkFileTextBox.Text = path;
                AppSettings.MesM2OkSoundFile = path;
            }
        }

        private void m2NgFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                m2NgFileTextBox.Text = path;
                AppSettings.MesM2NgSoundFile = path;
            }
        }

        private void m4OkFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                m4OkFileTextBox.Text = path;
                AppSettings.MesM4OkSoundFile = path;
            }
        }

        private void m4NgFileBrowseButton_Click(object sender, EventArgs e)
        {
            var path = Utils.SelectWavFile();
            if (path != null)
            {
                m4NgFileTextBox.Text = path;
                AppSettings.MesM4NgSoundFile = path;
            }
        }

        private void soundModeComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            soundFileGroupBox.Enabled = soundModeComboBox.SelectedIndex == 0;
            AppSettings.MesSoundPlayMode = soundModeComboBox.SelectedIndex;
        }
    }
}
