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
    internal class LinDeviceSettingsViewModel
    {
        internal List<LinDeviceSetting> GetDeviceSettings()
        {
            var manager = DeviceSettingsManager.SharedInstance;
            return manager.GetLinSettings();
        }

        internal void ConfigureEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(LinDeviceSetting.DeviceNumber):
                    if (editControl is NumericUpDown nuDown)
                    {
                        nuDown.Minimum = 1;
                        nuDown.Maximum = 16;
                    }
                    break;
                case nameof(LinDeviceSetting.Channel):
                    if (editControl is NumericUpDown chNUDown)
                    {
                        chNUDown.Minimum = 1;
                        chNUDown.Maximum = 16;
                    }
                    break;
            }
        }

        internal bool ValidateValue(string propertyName, object propertyValue, out string errorMessage)
        {
            switch (propertyName)
            {
                case nameof(LinDeviceSetting.DeviceType):
                    if (propertyValue is DeviceType type)
                    {
                        if (type.GetCategory() != DeviceCategory.LIN)
                        {
                            errorMessage = $"{propertyValue}은(는) LIN 디바이스 타입이 아닙니다.";
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

        internal LinDeviceSetting CreateSetting()
        {
            var createdSetting = new LinDeviceSetting();

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
