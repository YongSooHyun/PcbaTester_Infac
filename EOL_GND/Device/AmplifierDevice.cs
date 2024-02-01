using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// 증폭기.
    /// </summary>
    public abstract class AmplifierDevice : TestDevice
    {
        /// <summary>
        /// 식별문자열 읽기.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadIDN(CancellationToken token);

        /// <summary>
        /// 계측기를 공장 출고 상태로 초기화한다.
        /// </summary>
        public abstract void Reset(CancellationToken token);

        /// <summary>
        /// Amplifier 설정을 진행한다.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="inputCoupling"></param>
        /// <param name="inputImpedance"></param>
        /// <param name="gain"></param>
        public abstract void Configure(AmplifierChannels channels, AmplifierCoupling? inputCoupling, AmplifierImpedance? inputImpedance, 
            AmplifierGain? gain, CancellationToken token);

        /// <summary>
        /// Amplifier 채널 출력을 켜거나 끈다.
        /// </summary>
        /// <param name="channels"></param>
        /// <param name="on"></param>
        public abstract void SetOutput(AmplifierChannels channels, bool on, CancellationToken token);

        /// <summary>
        /// 지정한 이름을 가진 디바이스를 리턴한다.
        /// 해당 이름을 가진 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static AmplifierDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.Amplifier, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is AmplifierDevice ampDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return ampDevice;
            }

            AmplifierDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.Keysight_33502A:
                default:
                    device = new Keysight33502ADevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected AmplifierDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        [Flags]
        public enum AmplifierChannels
        {
            CH1 = 1,
            CH2 = 2,
        }

        public enum AmplifierCoupling
        {
            AC,
            DC,
        }

        [TypeConverter(typeof(DescEnumConverter))]
        public enum AmplifierImpedance
        {
            [Description("1 MΩ")]
            _1_MΩ,

            [Description("50 Ω")]
            _50_Ω,
        }

        public enum AmplifierGain
        {
            Direct,
            Amplifier,
        }
    }
}
