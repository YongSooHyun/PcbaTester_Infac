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
    internal class OscopeDeviceSettingsViewModel
    {
        internal List<OscopeDeviceSetting> GetDeviceSettings()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            return manager.GetOscopeSettings();
        }

        internal void ConfigureEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(OscopeDeviceSetting.IOTimeout):
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
                case nameof(OscopeDeviceSetting.DeviceType):
                    if (propertyValue is DeviceType type)
                    {
                        if (type.GetCategory() != DeviceCategory.Oscilloscope)
                        {
                            errorMessage = $"{propertyValue}은(는) Oscilloscope 디바이스 타입이 아닙니다.";
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
                case nameof(OscopeDeviceSetting.Channel1High):
                    if (DeviceSettingsManager.SharedInstance.AutoFillChannels && value is int channel1High && modelObject is OscopeDeviceSetting setting)
                    {
                        if (channel1High == 0)
                        {
                            setting.Channel1Low = 0;
                            setting.Channel2High = 0;
                            setting.Channel2Low = 0;
                            setting.Channel3High = 0;
                            setting.Channel3Low = 0;
                            setting.Channel4High = 0;
                            setting.Channel4Low = 0;
                        }
                        else
                        {
                            setting.Channel1Low = channel1High + 1;
                            setting.Channel2High = channel1High + 2;
                            setting.Channel2Low = channel1High + 3;
                            setting.Channel3High = channel1High + 4;
                            setting.Channel3Low = channel1High + 5;
                            setting.Channel4High = channel1High + 6;
                            setting.Channel4Low = channel1High + 7;
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

        internal OscopeDeviceSetting CreateSetting()
        {
            var createdSetting = new OscopeDeviceSetting();

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
