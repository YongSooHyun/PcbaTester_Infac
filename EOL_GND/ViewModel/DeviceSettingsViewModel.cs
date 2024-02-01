using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class DeviceSettingsViewModel
    {
        internal static bool AutoFill
        {
            get => DeviceSettingsManager.SharedInstance.AutoFillChannels;
            set => DeviceSettingsManager.SharedInstance.AutoFillChannels = value;
        }

        internal static List<StepCategoryInfo> RelayDevices
        {
            get
            {
                if (DeviceSettingsManager.SharedInstance.RelayDevices == null)
                {
                    DeviceSettingsManager.SharedInstance.RelayDevices = new List<StepCategoryInfo>()
                    {
                        new StepCategoryInfo(StepCategory.Oscilloscope),
                        new StepCategoryInfo(StepCategory.DMM),
                        new StepCategoryInfo(StepCategory.WaveformGenerator),
                        new StepCategoryInfo(StepCategory.Oscilloscope),
                    };
                }
                return DeviceSettingsManager.SharedInstance.RelayDevices;
            }
        }

        internal static int CardNumber
        {
            get => DeviceSettingsManager.SharedInstance.RelayCardNumber;
            set => DeviceSettingsManager.SharedInstance.RelayCardNumber = value;
        }

        internal static int CardChannelCount
        {
            get => DeviceSettingsManager.SharedInstance.RelayChannelsPerCard;
            set => DeviceSettingsManager.SharedInstance.RelayChannelsPerCard = value;
        }

        internal static int CardStartNumber
        {
            get => DeviceSettingsManager.SharedInstance.RelayChannelOffset;
            set => DeviceSettingsManager.SharedInstance.RelayChannelOffset = value;
        }

        internal static StepCategoryInfo CreateDeviceInfo()
        {
            var device = new StepCategoryInfo(StepCategory.DMM);
            RelayDevices.Add(device);
            return device;
        }

        internal static void RemoveInfos(IEnumerable modelObjects)
        {
            foreach (var modelObject in modelObjects)
            {
                RelayDevices.Remove(modelObject as StepCategoryInfo);
            }
        }

        internal static void MoveUp(object modelObject)
        {
            var info = modelObject as StepCategoryInfo;
            int index = RelayDevices.IndexOf(info);
            if (index > 0)
            {
                RelayDevices.RemoveAt(index);
                RelayDevices.Insert(index - 1, info);
            }
        }

        internal static void MoveDown(object modelObj)
        {
            var info = modelObj as StepCategoryInfo;
            int index = RelayDevices.IndexOf(info);
            if (index < RelayDevices.Count - 1)
            {
                RelayDevices.RemoveAt(index);
                RelayDevices.Insert(index + 1, info);
            }
        }

        internal static bool ValidateValue(string propertyName, object propertyValue, out string errorMessage)
        {
            switch (propertyName)
            {
                case nameof(StepCategoryInfo.Category):
                    if (propertyValue is StepCategory category)
                    {
                        if (category != StepCategory.DMM && category != StepCategory.Oscilloscope && category != StepCategory.WaveformGenerator)
                        {
                            errorMessage = $"DMM, Oscilloscope, WaveformGenerator 중 하나만 입력할 수 있습니다.";
                            return false;
                        }
                    }
                    break;
            }

            errorMessage = string.Empty;
            return true;
        }

        internal static void UpdateChannelNumbers(int cardNumber, int channelsPerCard, int startNumber)
        {
            int startChannel = (cardNumber - 1) * channelsPerCard + startNumber;
            for (int i = 0; i < RelayDevices.Count; i++)
            {
                var info = RelayDevices[i];
                switch (info.Category)
                {
                    case StepCategory.DMM:
                        info.StartChannel = startChannel;
                        info.EndChannel = startChannel + 3;
                        startChannel += 4;
                        break;
                    case StepCategory.Oscilloscope:
                        info.StartChannel = startChannel;
                        info.EndChannel = startChannel + 7;
                        startChannel += 8;
                        break;
                    case StepCategory.WaveformGenerator:
                        info.StartChannel = startChannel;
                        info.EndChannel = startChannel + 3;
                        startChannel += 4;
                        break;
                }
            }
        }

        internal static void ResetDeviceChannels(int cardNumber, int channelsPerCard, int startNumber)
        {
            UpdateChannelNumbers(cardNumber, channelsPerCard, startNumber);

            var settingsManager = DeviceSettingsManager.SharedInstance;

            // DMM 채널 리셋.
            var dmmSettings = settingsManager.GetDmmSettings();
            var oscopeSettings = settingsManager.GetOscopeSettings();
            var wgenSettings = settingsManager.GetWaveformGeneratorSettings();
            int dmmIndex = 0, oscopeIndex = 0, wgenIndex = 0;
            for (int deviceIndex = 0; deviceIndex < RelayDevices.Count; deviceIndex++)
            {
                var info = RelayDevices[deviceIndex];
                switch (info.Category)
                {
                    case StepCategory.DMM:
                        if (dmmSettings.Count > dmmIndex)
                        {
                            dmmSettings[dmmIndex].ChannelHighInput = info.StartChannel;
                            dmmSettings[dmmIndex].ChannelLowInput = info.StartChannel + 1;
                            dmmSettings[dmmIndex].ChannelHighSense = info.StartChannel + 2;
                            dmmSettings[dmmIndex].ChannelLowSense = info.StartChannel + 3;
                            dmmIndex++;
                        }
                        break;
                    case StepCategory.Oscilloscope:
                        if (oscopeSettings.Count > oscopeIndex)
                        {
                            oscopeSettings[oscopeIndex].Channel1High = info.StartChannel;
                            oscopeSettings[oscopeIndex].Channel1Low = info.StartChannel + 1;
                            oscopeSettings[oscopeIndex].Channel2High = info.StartChannel + 2;
                            oscopeSettings[oscopeIndex].Channel2Low = info.StartChannel + 3;
                            oscopeSettings[oscopeIndex].Channel3High = info.StartChannel + 4;
                            oscopeSettings[oscopeIndex].Channel3Low = info.StartChannel + 5;
                            oscopeSettings[oscopeIndex].Channel4High = info.StartChannel + 6;
                            oscopeSettings[oscopeIndex].Channel4Low = info.StartChannel + 7;
                            oscopeIndex++;
                        }
                        break;
                    case StepCategory.WaveformGenerator:
                        if (wgenSettings.Count > wgenIndex)
                        {
                            wgenSettings[wgenIndex].Channel1High = info.StartChannel;
                            wgenSettings[wgenIndex].Channel1Low = info.StartChannel + 1;
                            wgenSettings[wgenIndex].Channel2High = info.StartChannel + 2;
                            wgenSettings[wgenIndex].Channel2Low = info.StartChannel + 3;
                            wgenIndex++;
                        }
                        break;

                }
            }

            settingsManager.Save();
        }
    }
}
