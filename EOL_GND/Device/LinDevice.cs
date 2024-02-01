using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// LIN 디바이스.
    /// </summary>
    public abstract class LinDevice : TestDevice
    {
        /// <summary>
        /// 지정한 이름을 가진 LIN 디바이스를 리턴한다.
        /// 해당 이름을 가진 LIN 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static LinDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.LIN, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is LinDevice linDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return linDevice;
            }

            LinDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.PeakLIN:
                default:
                    device = new PeakLinDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected LinDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        /// <summary>
        /// LIN 디바이스에 연결합니다. 연결한 다음 필터를 설정할 때까지 메시지를 받지 못한다.
        /// </summary>
        /// <param name="masterMode"></param>
        /// <param name="baudrate"></param>
        public abstract void Open(bool masterMode, ushort baudrate);

        /// <summary>
        /// LIN 디바이스 연결을 해제합니다.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// 필터를 설정한다. 연결한 다음 필터를 설정할 때까지 메시지를 받지 못한다.
        /// </summary>
        /// <param name="receiveMask"></param>
        public abstract void SetReceiveFilter(ulong receiveMask);

        /// <summary>
        /// Frame Entry를 설정한다.
        /// </summary>
        /// <param name="frameEntry"></param>
        public abstract void SetFrameEntry(LinFrameEntry frameEntry);

        /// <summary>
        /// 큐에서 다음 메시지를 읽는다. 큐가 비었으면 false를 리턴한다.
        /// </summary>
        /// <param name="message"></param>
        /// <returns></returns>
        public abstract bool Read(out LinMessage message);

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            // Do nothing.
            return null;
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            // Do nothing.
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            // Do nothing.
            return null;
        }
    }
}
