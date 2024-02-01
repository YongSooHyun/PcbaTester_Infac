using System;
using System.IO.Ports;
using System.Threading;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Power Supply 디바이스와 통신하는 클래스.
    /// </summary>
    class PowerSupply : IDisposable
    {
        internal const string Power1Name = "Power-1";
        internal const string Power2Name = "Power-2";

        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal string DeviceName { get; set; } = "Power";

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        private readonly SerialPort serialPort = new SerialPort();

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = true;

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new object();

        // 각종 명령 문자열 및 응답.
        private const string IdnCommand = "*SN?";
        private const string IdnStartStr = "oda";
        private const string SetCommand = "APPL";
        private const string QueryStr = "?";
        private const string ResetCommand = "*RST";
        private const string OutputCommand = "OUTP";
        private const string OutputOn = "ON";
        private const string OutputOff = "OFF";
        private const string ValueSeparator = ",";

        // 응답 문자열 체크에 사용되는 문자열 비교 방법.
        private readonly StringComparison responseComparison = StringComparison.OrdinalIgnoreCase;

        // 명령 타임아웃.
        private const int ShortCommandTimeout = 3000;
        private const int CommandDelayTime = 500;

        // Dispose 패턴에 사용하는 변수.
        private bool disposedValue;

        internal PowerSupply(string name)
        {
            DeviceName = name;
        }

        /// <summary>
        /// 디바이스와 통신하기 위한 Open 을 진행한다.
        /// </summary>
        internal void Open()
        {
            if (serialPort.IsOpen)
            {
                return;
            }

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

                CR = portSettings.CR;
                LF = portSettings.LF;
            }

            serialPort.Open();
        }

        /// <summary>
        /// 디바이스와의 통신을 닫는다.
        /// </summary>
        internal void Close()
        {
            serialPort.Close();
        }

        // Serial Port로 명령을 보내고 그 응답을 받아서 리턴한다.
        private string SendCommand(string command, int readTimeout, bool checkResponse = true, bool showLog = true)
        {
            lock (commLockObj)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                if (showLog)
                {
                    Logger.LogCommMessage(DeviceName, command, true);
                }

                serialPort.WriteLine(command);

                if (checkResponse)
                {
                    serialPort.ReadTimeout = readTimeout;
                    string response = Utils.ReadLine(serialPort, CR || LF);

                    if (showLog)
                    {
                        Logger.LogCommMessage(DeviceName, response, false);
                    }

                    return response;
                }
                else
                {
                    return null;
                }
            }
        }

        /// <summary>
        /// IDN을 확인하는 명령을 전송하고 그 응답을 리턴한다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 읽기 타임아웃. 읽는데 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>IDN 응답 문자열.</returns>
        internal string GetIdn(int timeout)
        {
            return SendCommand(IdnCommand, timeout);
        }

        /// <summary>
        /// 전압, 전류를 설정한다.
        /// </summary>
        /// <param name="voltage">설정하려는 전압.(소수점 아래 4자리)</param>
        /// <param name="current">설정하려는 전류.(소수점 아래 4자리) null이면 설정하지 않는다.</param>
        internal void SetPower(float voltage, float? current)
        {
            string currentStr = current == null ? "" : $"{ValueSeparator}{current:F4}";
            string cmdLine = $"{SetCommand} {voltage:F4}{currentStr}";
            SendCommand(cmdLine, ShortCommandTimeout, false);

            // 설정된 값을 얻어 제대로 됐는지 비교한다.
            Thread.Sleep(CommandDelayTime);
            string response = SendCommand($"{SetCommand}{QueryStr}", ShortCommandTimeout);
            string escaped = response.Replace("\"", "");
            string[] valueStrArray = escaped.Split(new string[] { ValueSeparator }, StringSplitOptions.None);
            if (valueStrArray.Length == 2)
            {
                float parsedVoltage = float.Parse(valueStrArray[0]);
                float parsedCurrent = float.Parse(valueStrArray[1]);
                if (voltage != parsedVoltage)
                {
                    throw new Exception($"전압이 제대로 설정되지 않았습니다. 설정값={voltage:F4}, 리턴값={parsedVoltage:F4}");
                }
                if (current != null && current != parsedCurrent)
                {
                    throw new Exception($"전류가 제대로 설정되지 않았습니다. 설정값={current:F4}, 리턴값={parsedCurrent:F4}");
                }
            }
            else
            {
                throw new Exception("파워의 전압 전류 응답 형식이 옳지 않습니다.");
            }
        }

        /// <summary>
        /// 파워를 초기화한다.
        /// </summary>
        internal void Reset()
        {
            SendCommand(ResetCommand, ShortCommandTimeout, false);
        }

        /// <summary>
        /// 파워 출력을 허용하거나 차단한다.
        /// </summary>
        /// <param name="on">true이면 출력을 허용, false이면 출력을 차단한다.</param>
        internal void SetOutput(bool on)
        {
            SendCommand($"{OutputCommand} {(on ? OutputOn : OutputOff)}", ShortCommandTimeout, false);

            // 제대로 설정됐는지 체크한다.
            Thread.Sleep(CommandDelayTime);
            string response = SendCommand($"{OutputCommand}{QueryStr}", ShortCommandTimeout);
            string trimmed = response.Trim();
            if ((on && trimmed != "1") || (!on && trimmed == "1"))
            {
                throw new Exception($"파워 출력이 {(on ? "허용" : "차단")}되지 않았습니다.");
            }
        }

        /// <summary>
        /// 디바이스의 연결상태를 체크한다.
        /// </summary>
        /// <returns></returns>
        internal bool CheckConnected(int timeout = ShortCommandTimeout)
        {
            string sn = GetIdn(timeout);
            return sn.StartsWith(IdnStartStr, responseComparison);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    serialPort.Close();
                }

                disposedValue = true;
            }
        }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
