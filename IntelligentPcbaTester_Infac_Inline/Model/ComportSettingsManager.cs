using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 각 디바이스의 COM Port 설정을 관리(저장, 로딩) 한다.
    /// </summary>
    public class ComportSettingsManager
    {
        /// <summary>
        /// COM 포트 설정 보관 파일 이름.
        /// </summary>
        private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "comport.cfg");

        private static ComportSettingsManager sharedObject = null;

        /// <summary>
        /// COM 포트 설정 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<ComportSettings> ComSettings { get; set; } = new List<ComportSettings>();

        private ComportSettingsManager()
        {
        }

        /// <summary>
        /// 이 클래스의 인스턴스를 파일에 보관한다.
        /// </summary>
        internal void Save()
        {
            using (var writer = new StreamWriter(FileName))
            {
                var xmlSerializer = new XmlSerializer(GetType(), GetType().Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 이 클래스를 XML파일로부터 로딩한다.
        /// </summary>
        /// <returns>로딩한 오브젝트.</returns>
        internal static ComportSettingsManager Load()
        {
            if (sharedObject != null)
            {
                return sharedObject;
            }

            FileStream stream = null;
            try
            {
                stream = new FileStream(FileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(ComportSettingsManager), typeof(ComportSettingsManager).Namespace);
                var obj = xmlSerializer.Deserialize(stream) as ComportSettingsManager;
                sharedObject = obj;
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(ComportSettingsManager)}.{nameof(Load)}(): {e.Message}");
                sharedObject = CreateDefaultSettings();
                return sharedObject;
            }
            finally
            {
                stream?.Close();
            }
        }

        private static ComportSettingsManager CreateDefaultSettings()
        {
            var manager = new ComportSettingsManager();

            // COM 포트 디폴트 설정.
            manager.ComSettings.Clear();

            // Power1 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = PowerSupply.Power1Name,
                Port = ComPort.COM1
            });

            // Power2 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = PowerSupply.Power2Name,
                Port = ComPort.COM2,
                Mandatory = false
            });

            // DIO 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = DioRead.DeviceName,
                Port = ComPort.COM4,
                BaudRate = 115200,
                CR = true,
                LF = false
            });
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = DioOut.DeviceName,
                Port = ComPort.COM5,
                BaudRate = 115200,
                CR = true,
                LF = false
            });
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = "DIO(GP)",
                Port = ComPort.COM6,
                BaudRate = 115200,
                CR = true,
                LF = false
            });
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = "DIO(EXT-GP)",
                Port = ComPort.COM7,
                BaudRate = 115200,
                CR = true,
                LF = false
            });
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = Dio.DeviceName,
                Port = ComPort.COM10,
                BaudRate = 115200,
                CR = true,
                LF = false
            });

            // PLC 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = Plc.DeviceName,
                Port = ComPort.COM9,
                BaudRate = 115200,
                CR = false,
                LF = true
            });

            // Probe Counter 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = ProbeCounter.DeviceName,
                Port = ComPort.COM11,
            });

            // Barcode Scanner 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = BarcodeScanner.DeviceName,
                Port = ComPort.COM12,
                BaudRate = 115200,
                CR = true,
                LF = true
            });

            // FID Scanner 설정.
            manager.ComSettings.Add(new ComportSettings
            {
                DeviceName = FidScanner.DeviceName,
                Port = ComPort.COM13,
                BaudRate = 9600,
                CR = false,
                LF = false
            });

            return manager;
        }

        /// <summary>
        /// 디바이스의 설정을 찾아서 리턴한다.
        /// </summary>
        /// <param name="deviceName">찾으려는 디바이스 이름.</param>
        /// <returns>찾은 설정 오브젝트. 없으면 null을 리턴한다.</returns>
        internal ComportSettings FindSettings(string deviceName)
        {
            foreach (var settings in ComSettings)
            {
                if (settings.DeviceName.Equals(deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return settings;
                }
            }

            return null;
        }

        internal static bool ValidateValue(string propertyName, string propertyValue, StringBuilder errorMessage)
        {
            const int minDataBits = 5;
            const int maxDataBits = 8;
            switch (propertyName)
            {
                //case nameof(ComportSettings.Port):
                //    if (int.TryParse(propertyValue, out int parsed) && parsed > 0)
                //    {
                //        return true;
                //    }

                //    errorMessage?.Append("COM포트 번호는 0보다 큰 정수이어야 합니다.");
                //    return false;
                case nameof(ComportSettings.BaudRate):
                    if (int.TryParse(propertyValue, out int parsed) && parsed > 0)
                    {
                        return true;
                    }

                    errorMessage?.Append("Baudrate는 0보다 큰 정수이어야 합니다.");
                    return false;
                case nameof(ComportSettings.DataBits):
                    if (int.TryParse(propertyValue, out parsed) && parsed >= minDataBits && parsed <= maxDataBits)
                    {
                        return true;
                    }

                    errorMessage?.Append($"DataBits는 {minDataBits} 부터 {maxDataBits} 까지의 정수이어야 합니다.");
                    return false;
            }

            return true;
        }
    }
}
