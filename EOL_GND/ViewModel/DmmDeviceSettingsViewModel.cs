using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class DmmDeviceSettingsViewModel
    {
        internal List<DmmDeviceSetting> GetDeviceSettings()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            return manager.GetDmmSettings();
        }

        internal void ConfigureEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(DmmDeviceSetting.IOTimeout):
                    if (editControl is NumericUpDown timeoutNuDown)
                    {
                        timeoutNuDown.ThousandsSeparator = true;
                    }
                    break;
            }
        }

        internal bool ValidateValue(string propertyName, object propertyValue, out string errorMessage)
        {
            switch (propertyName)
            {
                case nameof(DmmDeviceSetting.DeviceType):
                    if (propertyValue is DeviceType type)
                    {
                        if (type.GetCategory() != DeviceCategory.DMM)
                        {
                            errorMessage = $"{propertyValue}은(는) DMM 디바이스 타입이 아닙니다.";
                            return false;
                        }
                    }
                    break;
            }

            errorMessage = string.Empty;
            return true;
        }

        internal bool EditFinished(object modelObject, string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(DmmDeviceSetting.ChannelHighInput):
                    if (DeviceSettingsManager.SharedInstance.AutoFillChannels && value is int channelHighInput && modelObject is DmmDeviceSetting setting)
                    {
                        if (channelHighInput == 0)
                        {
                            setting.ChannelLowInput = 0;
                            setting.ChannelHighSense = 0;
                            setting.ChannelLowSense = 0;
                        }
                        else
                        {
                            setting.ChannelLowInput = channelHighInput + 1;
                            setting.ChannelHighSense = channelHighInput + 2;
                            setting.ChannelLowSense = channelHighInput + 3;
                        }

                        return true;
                    }
                    break;
            }

            return false;
        }

        internal void Save()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            manager.Save();
        }

        internal DmmDeviceSetting CreateSetting()
        {
            var createdSetting = new DmmDeviceSetting();

            // 이름은 이전 디바이스 이름에 숫자를 붙이거나 증가시켜 만든다.
            string lastName = null;
            var settings = GetDeviceSettings();
            if (settings.Count > 0)
            {
                lastName = settings.Last().DeviceName;
            }
            createdSetting.DeviceName = DeviceSetting.CreateNewName(lastName, createdSetting.DeviceType.GetCategory().ToString());

            var manager = DeviceSettingsManager.SharedInstance;
            manager.DeviceSettings.Add(createdSetting);
            return createdSetting;
        }

        internal void DeleteSettings(System.Collections.IList settings)
        {
            var manager = DeviceSettingsManager.SharedInstance;
            foreach (var setting in settings)
            {
                manager.DeviceSettings.Remove(setting as DeviceSetting);
            }
        }
    }
}
