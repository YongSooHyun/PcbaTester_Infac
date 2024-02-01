using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// 오실로스코프.
    /// </summary>
    public abstract class OscopeDevice : TestDevice
    {
        // 설명이 해당되는 디바이스 이름.
        public const string KeysightInfiniiVision3000T_X_Series = "[Keysight InfiniiVision 3000T X-Series]";

        /// <summary>
        /// 식별문자열 읽기.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadIDN(CancellationToken token);

        /// <summary>
        /// 지정한 비휘발성 메모리로부터 계측기상태를 불러들인다.
        /// </summary>
        /// <param name="location">0|1|2|3|4|5|6|7|8|9 : 0은 계측기의 power down 상태를 보관한다.</param>
        public abstract void Recall(OscopeStateLocation location, CancellationToken token);

        /// <summary>
        /// 지정한 비휘발성 메모리에 계측기상태를 보관한다.
        /// </summary>
        /// <param name="location">0|1|2|3|4|5|6|7|8|9 : 0은 계측기의 power down 상태를 보관한다.</param>
        public abstract void Save(OscopeStateLocation location, CancellationToken token);

        /// <summary>
        /// 계측기를 공장 출고 상태로 초기화한다.
        /// </summary>
        public abstract void Reset(CancellationToken token);

        /// <summary>
        /// 스코프를 설정합니다.
        /// </summary>
        /// <param name="channelSettings"></param>
        /// <param name="triggerSettings"></param>
        /// <param name="token"></param>
        public abstract void Configure(List<OscopeChannelSettings> channelSettings, OscopeTriggerSettings triggerSettings,
            OscopeAcquireSettings acquireSettings, OscopeTimebaseSettings timebaseSettings, CancellationToken token);

        /// <summary>
        /// 신호를 캡쳐합니다.
        /// 캡처가 완료될 때까지 블록됩니다.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="token"></param>
        public abstract void Capture(bool autoScale, List<OscopeChannel> sources, CancellationToken token);

        /// <summary>
        /// 신호를 캡쳐합니다.
        /// 캡처 명령만 전송하고 바로 리턴합니다.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="token"></param>
        public abstract void BeginCapture(bool autoScale, List<OscopeChannel> sources, CancellationToken token);

        /// <summary>
        /// 신호를 캡쳐합니다.
        /// 앞서 전송한 캡처 명령이 완료될 때까지 기다립니다.
        /// <see cref="BeginCapture(bool, List{OscopeChannel}, CancellationToken)"/> 호출 후 사용됩니다.
        /// </summary>
        /// <param name="sources"></param>
        /// <param name="token"></param>
        public abstract void EndCapture(CancellationToken token);

        /// <summary>
        /// 캡처한 신호에 대한 측정을 진행합니다.
        /// </summary>
        /// <param name="measurementSettings"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract double? Measure(OscopeMeasurementSettings measurementSettings, CancellationToken token);

        /// <summary>
        /// DVM을 이용하여 전압을 측정합니다.
        /// </summary>
        /// <param name="dvmSettings"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract double DigitalVoltmeter(OscopeDvmSettings dvmSettings, CancellationToken token);

        /// <summary>
        /// 스크린 이미지를 다운로드합니다.
        /// </summary>
        /// <param name="displaySettings"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract byte[] Download(OscopeDisplaySettings displaySettings, CancellationToken token);

        /// <summary>
        /// 지정한 이름을 가진 오실로스코프 디바이스를 리턴한다.
        /// 해당 이름을 가진 오실로스코프 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static OscopeDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var oscopeSetting = settingsManager.FindSetting(DeviceCategory.Oscilloscope, deviceName);

            var oldDevice = FindDevice(oscopeSetting);
            if (oldDevice is OscopeDevice scopeDevice)
            {
                Logger.LogVerbose($"Using old device: {oscopeSetting.DeviceType}, {oscopeSetting.DeviceName}");
                return scopeDevice;
            }

            OscopeDevice device;
            switch (oscopeSetting.DeviceType)
            {
                case DeviceType.Keysight_InfiniiVision_3000T_X:
                default:
                    device = new KeysightInfiniiVision3000TXDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected OscopeDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        /// <summary>
        /// Location where the instrument states are saved and recalled.
        /// </summary>
        public enum OscopeStateLocation
        {
            Location0 = 0,
            Location1,
            Location2,
            Location3,
            Location4,
            Location5,
            Location6,
            Location7,
            Location8,
            Location9,
        }

        [Flags]
        public enum OscopeChannels
        {
            None = 0,
            CH1 = 0x1,
            CH2 = 0x2,
            CH3 = 0x4,
            CH4 = 0x8,
            All = CH1 | CH2 | CH3 | CH4,
        }

        public enum OscopeChannel
        {
            Channel1,
            Channel2,
            Channel3,
            Channel4,
        }
    }
}
