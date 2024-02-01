using EOL_GND.Common;
using EOL_GND.ViewModel;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class DeviceSettingsForm : Form
    {
        public DeviceSettingsForm()
        {
            InitializeComponent();

            deviceListView.CellEditUseWholeCell = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = deviceListView.Font;
            Controls.Add(dummyComboBox);
            deviceListView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);

            autoFillCheckBox.Checked = DeviceSettingsViewModel.AutoFill;
            deviceListView.SetObjects(DeviceSettingsViewModel.RelayDevices);
            UpdateButtons();

            cardNumberNUDown.Value = DeviceSettingsViewModel.CardNumber;
            cardChannelsNUDown.Value = DeviceSettingsViewModel.CardChannelCount;
            startNumberNUDown.Value = DeviceSettingsViewModel.CardStartNumber;

            // 사용자 권한 설정.
            var permission = GeneralSettingsViewModel.GetUserPermission();
            var editAllowed = permission?.CanEditDeviceSettings?? false;
            Enabled = editAllowed;
        }

        private void deviceListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateButtons();
        }

        private void UpdateButtons()
        {
            removeButton.Enabled = deviceListView.SelectedObjects.Count > 0;
            upButton.Enabled = deviceListView.SelectedIndex > 0;
            downButton.Enabled = deviceListView.SelectedIndex >= 0 && deviceListView.SelectedIndex < deviceListView.Items.Count - 1;
        }

        private void deviceListView_CellEditValidating(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (!DeviceSettingsViewModel.ValidateValue(e.Column.AspectName, e.NewValue, out string errorMessage))
            {
                InformationBox.Show(errorMessage, "Validation Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void deviceListView_CellEditFinished(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            UpdateChannelNumbers();
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var newRecord = DeviceSettingsViewModel.CreateDeviceInfo();
            deviceListView.AddObject(newRecord);
            deviceListView.SelectObject(newRecord);
            UpdateChannelNumbers();
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            var selectedObjects = deviceListView.SelectedObjects;
            if (selectedObjects.Count > 0)
            {
                DeviceSettingsViewModel.RemoveInfos(selectedObjects);
                deviceListView.RemoveObjects(selectedObjects);
                UpdateChannelNumbers();
            }
        }

        private void upButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = deviceListView.SelectedIndex;
            if (selectedIndex > 0)
            {
                var modelObject = deviceListView.SelectedObject;
                DeviceSettingsViewModel.MoveUp(modelObject);
                deviceListView.MoveObjects(selectedIndex - 1, new object[] { modelObject });
                deviceListView.SelectObject(modelObject);
                UpdateChannelNumbers();
            }
        }

        private void downButton_Click(object sender, EventArgs e)
        {
            int selectedIndex = deviceListView.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < deviceListView.Items.Count - 1)
            {
                var modelObject = deviceListView.SelectedObject;
                DeviceSettingsViewModel.MoveDown(modelObject);
                deviceListView.MoveObjects(selectedIndex + 2, new object[] { modelObject });
                deviceListView.SelectObject(modelObject);
                UpdateChannelNumbers();
            }
        }

        private void autoFillCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            DeviceSettingsViewModel.AutoFill = autoFillCheckBox.Checked;
        }

        private void cardNumberNUDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateChannelNumbers();
            DeviceSettingsViewModel.CardNumber = (int)cardNumberNUDown.Value;
        }

        private void cardChannelsNUDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateChannelNumbers();
            DeviceSettingsViewModel.CardChannelCount = (int)cardChannelsNUDown.Value;
        }

        private void startNumberNUDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateChannelNumbers();
            DeviceSettingsViewModel.CardStartNumber = (int)startNumberNUDown.Value;
        }

        private void UpdateChannelNumbers()
        {
            int cardNumber = (int)cardNumberNUDown.Value;
            int channelsPerCard = (int)cardChannelsNUDown.Value;
            int startNumber = (int)startNumberNUDown.Value;
            DeviceSettingsViewModel.UpdateChannelNumbers(cardNumber, channelsPerCard, startNumber);
            deviceListView.RefreshObjects(DeviceSettingsViewModel.RelayDevices);
        }

        private void channelResetButton_Click(object sender, EventArgs e)
        {
            try
            {
                int cardNumber = (int)cardNumberNUDown.Value;
                int channelsPerCard = (int)cardChannelsNUDown.Value;
                int startNumber = (int)startNumberNUDown.Value;
                DeviceSettingsViewModel.ResetDeviceChannels(cardNumber, channelsPerCard, startNumber);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Device channels reset error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }
    }
}
