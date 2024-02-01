using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;

namespace IntelligentPcbaTester
{
    internal class Dio : IDisposable
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "DIO";

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = true;

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        private readonly SerialPort serialPort = new SerialPort();

        // 문자열 비교에 사용되는 비교 방법.
        private readonly StringComparison textComparison = StringComparison.OrdinalIgnoreCase;

        // 명령 전송, 응답, 오류 구분 문자열.
        private const string SendCommandHeader = "@";
        private const string RecvCommandHeader = ">";
        private const string ErrorCommandHeader = "$";

        // 버전 확인 Command.
        private const string VersionCommand = "VER";

        // 하단 픽스처 전원 켜기.
        private const string FixturePowerOnCommand = "FIXPWRON";

        // 하단 픽스처 전원 끄기.
        private const string FixturePowerOffCommand = "FIXPWROFF";

        // 전원 끄기.
        private const string PowerOffCommand = "ALLOFF";

        // 방전회로 동작.
        private const string DischargeOnCommand = "DISCON";

        // 방전회로 해제.
        private const string DischargeOffCommand = "DISCOFF";

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new object();

        // Dispose 패턴에 사용되는 변수.
        private bool disposedValue = false;

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
        private string SendCommand(string command, int readTimeout, bool showLog = true)
        {
            lock (commLockObj)
            {
                string response = "";

                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                string commandLine = SendCommandHeader + command;
                if (showLog)
                {
                    Logger.LogCommMessage(DeviceName, commandLine, true);
                }

                serialPort.WriteLine(commandLine);
                serialPort.ReadTimeout = readTimeout;

                response = Utils.ReadLine(serialPort, CR || LF);

                if (showLog)
                {
                    Logger.LogCommMessage(DeviceName, response, false);
                }

                return response;
            }
        }

        // 오류 응답을 파싱하여 에러정보를 리턴한다.
        private void ParseError(string response, string command)
        {
            if (response.StartsWith(ErrorCommandHeader, textComparison))
            {
                throw new Exception(DeviceName + " Error: " + response);
            }
            else if (response.StartsWith(RecvCommandHeader, textComparison))
            {
                // No error.
            }
            else
            {
                // Format error.
                throw new Exception(DeviceName + " Unknown Error: " + response);
            }
        }

        /// <summary>
        /// 디바이스 버전을 체크한다. 연결을 확인하는데 쓸 수도 있다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 읽기 타임아웃. 읽는데 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>버전 문자열. 응답 형식이 맞지 않으면 null을 리턴한다.</returns>
        internal string CheckVersion(int readTimeout = 500)
        {
            string response = SendCommand(VersionCommand, readTimeout);
            ParseError(response, VersionCommand);
            return response;
        }

        /// <summary>
        /// 픽스처 파워 On.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void FixturePowerOn(bool showLog, int readTimeout = 1000)
        {
            string response = SendCommand(FixturePowerOnCommand, readTimeout, showLog);
            ParseError(response, FixturePowerOnCommand);
        }

        /// <summary>
        /// 픽스처 파워 Off.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void FixturePowerOff(bool showLog, int readTimeout = 1000)
        {
            string response = SendCommand(FixturePowerOffCommand, readTimeout, showLog);
            ParseError(response, FixturePowerOffCommand);
        }

        /// <summary>
        /// 파워 Off.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void PowerOff(int readTimeout = 1000)
        {
            string response = SendCommand(PowerOffCommand, readTimeout);
            ParseError(response, PowerOffCommand);
        }

        /// <summary>
        /// Discharge On.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void DischargeOn(int readTimeout = 1000)
        {
            string response = SendCommand(DischargeOnCommand, readTimeout);
            ParseError(response, DischargeOnCommand);
        }

        /// <summary>
        /// Discharge Off.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void DischargeOff(int readTimeout = 1000)
        {
            string response = SendCommand(DischargeOffCommand, readTimeout);
            ParseError(response, DischargeOffCommand);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
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
