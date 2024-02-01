using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Scope 디바이스와 통신한다.
    /// </summary>
    class Scope
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "Scope";

        // IDN 확인을 위한 Command 문자열.
        private const string IdnCommand = "*IDN?";

        /// <summary>
        /// IDN을 확인하는 명령을 전송하고 그 응답을 리턴한다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 읽기 타임아웃. 읽는데 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>IDN 응답 문자열.</returns>
        internal string GetIdn(int readTimeout = 100)
        {
            using (var serialPort = new SerialPort())
            {
                var settingsManager = ComportSettingsManager.Load();
                var portSettings = settingsManager.FindSettings(DeviceName);
                if (portSettings != null)
                {
                    serialPort.PortName = portSettings.Port.ToString();
                    serialPort.BaudRate = portSettings.BaudRate;
                    serialPort.DataBits = portSettings.DataBits;
                    serialPort.Parity = portSettings.Parity;
                    if (!string.IsNullOrEmpty(portSettings.NewLine))
                    {
                        serialPort.NewLine = portSettings.NewLine;
                    }
                }

                serialPort.Open();

                Logger.LogCommMessage(DeviceName, IdnCommand, true);

                serialPort.WriteLine(IdnCommand);
                serialPort.ReadTimeout = readTimeout;
                string response = Utils.ReadLine(serialPort, (portSettings?.CR ?? false) || (portSettings?.LF ?? false));

                Logger.LogCommMessage(DeviceName, response, false);

                return response;
            }
        }
    }
}
