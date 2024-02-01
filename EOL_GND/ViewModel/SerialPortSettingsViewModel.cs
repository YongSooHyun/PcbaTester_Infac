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
    internal class SerialPortSettingsViewModel
    {
        internal List<SerialPortSetting> GetDeviceSettings()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            return manager.GetSerialPortSettings();
        }

        internal void ConfigureEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(SerialPortSetting.DataBits):
                    if (editControl is NumericUpDown nuDown)
                    {
                        nuDown.Minimum = 5;
                        nuDown.Maximum = 8;
                    }
                    break;
                case nameof(SerialPortSetting.BaudRate):
                    if (editControl is NumericUpDown baudRateNuDown)
                    {
                        baudRateNuDown.ThousandsSeparator = true;
                    }
                    break;
                case nameof(SerialPortSetting.Timeout):
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
                case nameof(SerialPortSetting.DeviceType):
                    if (propertyValue is DeviceType type)
                    {
                        if (type.GetCategory() != DeviceCategory.SerialPort)
                        {
                            errorMessage = $"{propertyValue}은(는) SerialPort 디바이스 타입이 아닙니다.";
                            return false;
                        }
                    }
                    break;
            }

            errorMessage = string.Empty;
            return true;
        }

        internal void Save()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            manager.Save();
        }

        internal SerialPortSetting CreateSetting()
        {
            var createdSetting = new SerialPortSetting();

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
