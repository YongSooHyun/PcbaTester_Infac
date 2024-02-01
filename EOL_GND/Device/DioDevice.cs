using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public class DioDevice : TestDevice
    {
        // 시리얼 통신 클래스 인스턴스.
        private readonly SerialDevice serialDevice = new SerialDevice();

        // Dispose패턴 필드.
        private bool disposedValue = false;

        /// <summary>
        /// 지정한 이름을 가진 DIO 디바이스를 리턴한다.
        /// 해당 이름을 가진 DIO 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static DioDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.DIO, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is DioDevice dioDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return dioDevice;
            }

            DioDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.DIO:
                default:
                    device = new DioDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        private DioDevice(string name) : base(DeviceType.DIO, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            serialDevice.Open(Setting as Model.SerialDeviceSetting, token);
        }

        public override void Disconnect()
        {
            serialDevice.Close();
        }

        // Serial Port로 명령을 보내고 그 응답을 받아서 리턴한다.
        private string SendCommand(string command, int readTimeout, bool checkResponse, CancellationToken token)
        {
            return serialDevice.SendCommand(command, readTimeout, checkResponse, token);
        }

        // 에러를 파싱하여 예외를 던진다.
        private void ParseError(string response, string command)
        {
            if (response.StartsWith($">{command}", StringComparison.OrdinalIgnoreCase))
            {
                // Do nothing;
            }
            else
            {
                // 에러 메시지 출력.
                throw new Exception($"DIO 명령 오류: {response}");
            }
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return SendCommand(command, readTimeout, read, token);
        }

        /// <summary>
        /// 지정한 명령을 실행한다.
        /// </summary>
        /// <param name="cmdInfo"></param>
        /// <param name="token"></param>
        public string RunCommand(CommandInfo cmdInfo, CancellationToken token)
        {
            int timeout = cmdInfo.Timeout <= 0 ? SerialPort.InfiniteTimeout : cmdInfo.Timeout;
            string response = SendCommand($"@{cmdInfo.Command}", timeout, true, token);
            ParseError(response, cmdInfo.Command);
            return response;
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

        /// <summary>
        /// DIO 명령 정보.
        /// </summary>
        public class CommandInfo : ICloneable, INotifyPropertyChanged
        {
            /// <summary>
            /// 명령 문자열.
            /// </summary>
            public string Command
            {
                get => _command;
                set
                {
                    if (_command != value)
                    {
                        _command = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _command;

            /// <summary>
            /// 명령 응답 타임아웃(ms).
            /// </summary>
            public int Timeout
            {
                get => _timeout;
                set
                {
                    if (_timeout != value)
                    {
                        _timeout = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private int _timeout = 500;

            /// <summary>
            /// 설명.
            /// </summary>
            public string Description
            {
                get => _description;
                set
                {
                    if (_description != value)
                    {
                        _description = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _description;

            // INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;

            public CommandInfo()
            {
                Command = "NewCommand";
            }

            public override string ToString()
            {
                return Command;
            }

            public object Clone()
            {
                var newInstance = new CommandInfo();
                newInstance.Command = Command;
                newInstance.Timeout = Timeout;
                newInstance.Description = Description;

                return newInstance;
            }

            // This method is called by the Set accessor of each property.  
            // The CallerMemberName attribute that is applied to the optional propertyName  
            //   parameter causes the property name of the caller to be substituted as an argument.
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }
}
