using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Fixture의 Probe Counter와 통신한다.
    /// </summary>
    class ProbeCounter
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "Probe Counter";

        /// <summary>
        /// COM 포트.
        /// </summary>
        internal const string PortName = "COM8";

        /// <summary>
        /// 현재값, 최대값을 읽는다.
        /// </summary>
        /// <param name="currentValue"></param>
        /// <param name="maxValue"></param>
        internal static void ReadCount(out uint currentValue, out uint maxValue)
        {
            SerialPort serialPort = new SerialPort();
            try
            {
                var settingsManager = ComportSettingsManager.Load();
                var portSettings = settingsManager.FindSettings(DeviceName);
                if (portSettings != null)
                {
                    serialPort.PortName = portSettings.Port.ToString();
                    serialPort.BaudRate = portSettings.BaudRate;
                    serialPort.DataBits = portSettings.DataBits;
                    serialPort.Parity = portSettings.Parity;
                }
                else
                {
                    serialPort.PortName = PortName;
                    serialPort.BaudRate = 9600;
                }

                serialPort.Open();

                // Query 만들기.
                byte[] queryData = new byte[8];

                // Slave Address.
                queryData[0] = 1;

                // Function.
                queryData[1] = 4;

                // Start Address.
                queryData[2] = 3;
                queryData[3] = 0xEB;

                // Number of Points.
                queryData[4] = 0;
                queryData[5] = 5;

                // Error Check(CRC16)
                queryData[6] = 0x40;
                queryData[7] = 0x79;

                // Query 전송.
                serialPort.Write(queryData, 0, queryData.Length);

                string message = "";
                foreach (byte data in queryData)
                {
                    if (message.Length > 0)
                    {
                        message += " ";
                    }
                    message += $"{data:X2}";
                }
                Logger.LogCommMessage(DeviceName, message, true);

                // 응답 읽기.
                serialPort.ReadTimeout = 500;
                int readLength = 15;
                byte[] responseData = new byte[readLength];
                int readBytes = 0;
                while (readBytes < readLength)
                {
                    readBytes += serialPort.Read(responseData, readBytes, readLength - readBytes);
                }

                message = "";
                for (int i = 0; i < readBytes; i++)
                {
                    if (message.Length > 0)
                    {
                        message += " ";
                    }
                    message += $"{responseData[i]:X2}";
                }
                Logger.LogCommMessage(DeviceName, message, false);

                if (readBytes != responseData.Length)
                {
                    throw new Exception($"Probe Counter: 응답 바이트 수({readBytes})가 잘못되었습니다.");
                }

                // 응답 파싱.

                // 현재 값.
                currentValue = (uint)(responseData[5] << 24) + (uint)(responseData[6] << 16) +
                    (uint)(responseData[3] << 8) + responseData[4];
                // 최대값.
                maxValue = (uint)(responseData[11] << 24) + (uint)(responseData[12] << 16) +
                    (uint)(responseData[9] << 8) + responseData[10];
            }
            finally
            {
                serialPort.Close();
            }
        }
    }
}
