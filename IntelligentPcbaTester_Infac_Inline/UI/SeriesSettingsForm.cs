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
    public partial class SeriesSettingsForm : Form
    {
        public SeriesSettingsForm()
        {
            InitializeComponent();

            seriesOptionComboBox.SelectedIndex = (int)AppSettings.SeriesConnOption;
            serverNameTextBox.Text = AppSettings.SeriesServer;
            portNumericUpDown.Value = AppSettings.SeriesServerPort;
            timeoutNumericUpDown.Value = AppSettings.SeriesCommTimeout;

            requestComboBox.DataSource = Enum.GetValues(typeof(SeriesMessage.RequestCommand));

            UpdateServerStatus();
            UpdateConnectionStatus();
        }

        private void seriesOptionComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            AppSettings.SeriesConnOption = (SeriesOption)seriesOptionComboBox.SelectedIndex;
            switch (AppSettings.SeriesConnOption)
            {
                case SeriesOption.SeriesNext:
                    commGroupBox.Enabled = true;
                    serverLabel.Enabled = false;
                    serverNameTextBox.Enabled = false;
                    debugGroupBox.Enabled = true;
                    serverGroupBox.Enabled = true;
                    clientGroupBox.Enabled = false;
                    break;
                case SeriesOption.SeriesPrev:
                    commGroupBox.Enabled = true;
                    serverLabel.Enabled = true;
                    serverNameTextBox.Enabled = true;
                    debugGroupBox.Enabled = true;
                    serverGroupBox.Enabled = false;
                    clientGroupBox.Enabled = true;
                    break;
                case SeriesOption.None:
                default:
                    commGroupBox.Enabled = false;
                    debugGroupBox.Enabled = false;
                    break;
            }
        }

        private void serverNameTextBox_TextChanged(object sender, EventArgs e)
        {
            AppSettings.SeriesServer = serverNameTextBox.Text;
        }

        private void portNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.SeriesServerPort = (int)portNumericUpDown.Value;
        }

        private void timeoutNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            AppSettings.SeriesCommTimeout = (int)timeoutNumericUpDown.Value;
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            try
            {
                int port = (int)portNumericUpDown.Value;
                SeriesServer.SharedServer.Start(port);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Series Server: {ex.Message}");
            }
            finally
            {
                Thread.Sleep(50);
                UpdateServerStatus();
            }
        }

        private void UpdateServerStatus()
        {
            serverStatusLabel.Text = SeriesServer.SharedServer.Running ? "실행 중" : "중지됨";
            startButton.Enabled = !SeriesServer.SharedServer.Running;
            stopButton.Enabled = SeriesServer.SharedServer.Running;
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            try
            {
                SeriesServer.SharedServer.Stop();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Series Server: {ex.Message}");
            }
            finally
            {
                Thread.Sleep(50);
                UpdateServerStatus();
            }
        }

        private async void connectButton_Click(object sender, EventArgs e)
        {
            try
            {
                clientGroupBox.Enabled = false;
                await Task.Run(() => SeriesClient.SharedClient.Connect(serverNameTextBox.Text, (int)portNumericUpDown.Value));
            }
            catch (Exception ex)
            {
                Logger.LogError($"Series Client: {ex.Message}");
            }
            finally
            {
                UpdateConnectionStatus();
                clientGroupBox.Enabled = true;
            }
        }

        private void UpdateConnectionStatus()
        {
            connectionStatusLabel.Text = SeriesClient.SharedClient.Connected ? "연결됨" : "끊어짐";
            connectButton.Enabled = !SeriesClient.SharedClient.Connected;
            disconnectButton.Enabled = SeriesClient.SharedClient.Connected;
        }

        private void disconnectButton_Click(object sender, EventArgs e)
        {
            try
            {
                SeriesClient.SharedClient.Disconnect();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Series Client: {ex.Message}");
            }
            finally
            {
                UpdateConnectionStatus();
            }
        }

        private void sendButton_Click(object sender, EventArgs e)
        {
            try
            {
                var request = new SeriesRequestMessage((SeriesMessage.RequestCommand)requestComboBox.SelectedItem);
                var response = SeriesClient.SharedClient.SendRequest(request);
                Logger.LogTimedMessage($"Series Response={{{nameof(SeriesMessage.Command)}={response.Command}, " +
                    $"{nameof(SeriesResponseMessage.Capacity)}={response.Capacity}}}");
            }
            catch (Exception ex)
            {
                Logger.LogError($"Series Client: {ex.Message}");
            }
            finally
            {
                UpdateConnectionStatus();
            }
        }
    }
}
