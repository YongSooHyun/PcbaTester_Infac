using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public class SerialPortDevice : TestDevice
    {
        internal StringBuilder LogBuilder { get; } = new StringBuilder();

        // 시리얼 통신 클래스 인스턴스.
        private readonly SerialDevice serialDevice = new SerialDevice();

        // Dispose패턴 필드.
        private bool disposedValue = false;

        /// <summary>
        /// 지정한 이름을 가진 SerialPort 디바이스를 리턴한다.
        /// 해당 이름을 가진 SerialPort 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static SerialPortDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.SerialPort, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is SerialPortDevice serialPortDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return serialPortDevice;
            }

            SerialPortDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.SerialPort:
                default:
                    device = new SerialPortDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        private SerialPortDevice(string name) : base(DeviceType.SerialPort, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            serialDevice.Open(Setting as Model.SerialDeviceSetting, token);
        }

        public override void Disconnect()
        {
            serialDevice.SerialPort.DataReceived -= SerialPort_DataReceived;
            serialDevice.Close();
        }

        public void LogStart()
        {
            serialDevice.SerialPort.DataReceived += SerialPort_DataReceived;
        }

        public void LogStop()
        {
            serialDevice.SerialPort.DataReceived -= SerialPort_DataReceived;
        }

        private void SerialPort_DataReceived(object sender, System.IO.Ports.SerialDataReceivedEventArgs e)
        {
            try
            {
                var bytesToRead = serialDevice.SerialPort.BytesToRead;
                if (bytesToRead > 0)
                {
                    var buffer = new byte[bytesToRead];
                    var readBytes = serialDevice.SerialPort.Read(buffer, 0, bytesToRead);
                    if (readBytes > 0)
                    {
                        var readText = serialDevice.SerialPort.Encoding.GetString(buffer, 0, readBytes);
                        LogBuilder.Append(readText);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"{(Setting as SerialDeviceSetting).Port} DataReceived error: {ex.Message}");
            }
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return serialDevice.SendCommand(command, readTimeout, read, token);
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serialDevice.Dispose();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
