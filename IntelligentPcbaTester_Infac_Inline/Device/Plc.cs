using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// PLC로부터 상태를 읽을 때 응답문자열에 포함된 비트들의 의미.
    /// </summary>
    [Flags]
    public enum PlcReadFlags : uint
    {
        /// <summary>
        /// 0=자동, 1=수동
        /// </summary>
        Mode = 0x00_00_00_01,

        /// <summary>
        /// 0=OPEN, 1=CLOSE
        /// </summary>
        FrontDoor = 0x00_00_00_02,

        /// <summary>
        /// 0=OPEN, 1=CLOSE
        /// </summary>
        RearDoor = 0x00_00_00_04,

        /// <summary>
        /// 0=미장착, 1=장착
        /// </summary>
        BottomFixtureSensor = 0x00_00_00_08,

        /// <summary>
        /// 0=미장착, 1=장착
        /// </summary>
        TopFixtureSensor = 0x00_00_00_10,

        /// <summary>
        /// 0=없음, 1=있음
        /// </summary>
        PcbSensor = 0x00_00_00_20,

        /// <summary>
        /// 0=NG, 1=OK
        /// </summary>
        CylinderBottomSensor = 0x00_00_00_40,

        /// <summary>
        /// 0=NG, 1=OK
        /// </summary>
        CylinderFctSensor = 0x00_00_00_80,

        /// <summary>
        /// 0=NG, 1=OK
        /// </summary>
        CylinderTopSensor = 0x00_00_01_00,

        /// <summary>
        /// 0=Abnormal, 1=Normal
        /// </summary>
        AirPressure = 0x00_00_02_00,

        /// <summary>
        /// 0=Abnormal, 1=Normal
        /// </summary>
        SafetySensor = 0x00_00_04_00,

        /// <summary>
        /// 0=NotReady, 1=Ready
        /// </summary>
        IctReady = 0x00_00_08_00,

        /// <summary>
        /// 0=Stop, 1=Working
        /// </summary>
        ConveyorSensor = 0x00_00_10_00,

        /// <summary>
        /// 0=Abnormal, 1=Normal
        /// </summary>
        Emergency = 0x00_00_20_00,

        /// <summary>
        /// 0=미장착, 1=장착
        /// </summary>
        BottomClamp = 0x00_00_40_00,

        /// <summary>
        /// 0=미장착, 1=장착
        /// </summary>
        TopClamp = 0x00_00_80_00,
    }

    /// <summary>
    /// PCB 테스트 결과, PCB이동 시 배출동작 지시.
    /// </summary>
    public enum PcbTestResult
    {
        NoPcb = 0,
        Pass,
        Fail,
        NotTested,
    }

    /// <summary>
    /// MES 통신 결과, PCB이동 시 ICT존 투입 지시.
    /// </summary>
    public enum PcbMesResult
    {
        NoPcb = 0,
        OK,
        NG,
    }

    /// <summary>
    /// PCB이동 시 SCAN존 투입 지시.
    /// </summary>
    public enum PcbScanZone
    {
        NoPcb = 0,
        Optional,
        Necessary,
    }

    internal class Plc : IDisposable
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "PLC";

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = true;

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        private readonly SerialPort serialPort = new SerialPort();

        // 문자열 비교에 사용되는 비교 방법.
        private static readonly StringComparison textComparison = StringComparison.OrdinalIgnoreCase;

        // 명령 전송, 응답, 오류 구분 문자열.
        private const string SendCommandHeader = "@";
        private const string RecvCommandHeader = ">";
        private const string ErrorCommandHeader = "$";

        // 버전 확인 Command.
        private const string VersionCommand = "VER";

        // 상태 읽기 Command.
        private const string ReadCommand = "READ";

        // PCB 이동 명령.
        private const string PcbMoveCommand = "PCB";

        // Fixture ID 전송 Command.
        private const string FidCommand = "FID";

        // 실린더 다운 명령.
        private const string CylinderDownCommand = "DOWN";

        // 실린더 상승 명령.
        private const string CylinderUpCommand = "UP";

        // 실린더를 FCT 위치로 상승 명령.
        private const string CylinderFctUpCommand = "FCTUP";

        // 실린더를 대기모드 위치로 상승 명령.
        private const string CylinderMidUpCommand = "MIDUP";

        // 자동/수동 모드 변경 명령.
        private const string ModeCommand = "MODE";

        // 실린더 초기화 명령.
        private const string CylinderInitCommand = "CYLINIT";

        // 오류정보 전송 명령.
        private const string ErrorCommand = "ERROR";

        // 응답이 약속된 형식이 아닐 때 에러 메시지.
        private const string FormatErrorMsg = "기구 응답형식 오류입니다.";

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new object();

        // Dispose 패턴에 사용하는 변수.
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
        private string SendCommand(string command, int readTimeout, int retryCount, bool showLog = true)
        {
            lock (commLockObj)
            {
                int retriedCount = 0;
                do
                {
                    try
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

                        // PLC통신에서 뒤에 \0 이 따라들어오면 서식이 파괴되므로 이를 제거.
                        response = response.Replace("\0", "");

                        if (showLog)
                        {
                            Logger.LogCommMessage(DeviceName, response, false);
                        }

                        return response;
                    }
                    catch (TimeoutException)
                    {
                        //Logger.LogError($"PLC command timeout: CMD={command}, Timeout={readTimeout}, RetryCount={retryCount - retriedCount}");
                        if (retryCount > retriedCount)
                        {
                            // Retry, do nothing.
                        }
                        else
                        {
                            throw;
                        }
                        retriedCount++;
                    }
                } while (retriedCount <= retryCount);

                return "";
            }
        }

        // 오류 응답을 파싱하여 에러정보를 리턴한다.
        internal static void ParseError(string response, string command)
        {
            if (response.StartsWith(ErrorCommandHeader + "EM", textComparison))
            {
                throw new PlcException("기구 비상 상황이 발생했습니다.");
            }
            else if (response.StartsWith(ErrorCommandHeader, textComparison))
            {
                throw new PlcException("기구 오류가 발생했습니다.");
            }
            else if (response.StartsWith(RecvCommandHeader + command, textComparison))
            {
                // No error.
            }
            else
            {
                // Format error.
                throw new PlcException(FormatErrorMsg);
            }
        }

        /// <summary>
        /// 디바이스 버전을 체크한다. 연결을 확인하는데 쓸 수도 있다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 읽기 타임아웃. 읽는데 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>버전 문자열. 응답 형식이 맞지 않으면 null을 리턴한다.</returns>
        internal string CheckVersion(int readTimeout = 500)
        {
            string response = SendCommand(VersionCommand, readTimeout, 2);
            ParseError(response, VersionCommand);
            return response;
        }

        /// <summary>
        /// 상태를 읽는다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>상태 코드 조합.</returns>
        internal PlcReadFlags ReadStatus(bool showLog, int readTimeout = 500)
        {
            string response = SendCommand(ReadCommand, readTimeout, 2, showLog);
            ParseError(response, ReadCommand);

            var header = RecvCommandHeader + ReadCommand;
            if (response.StartsWith(header))
            {
                // 상태 코드(정수).
                string statusText = response.Substring(header.Length);

                // 공백을 모두 제거한다.
                string uintString = Regex.Replace(statusText, "\\s+", "");

                // 정수 문자열을 파싱한다.
                if (uint.TryParse(uintString, out uint parsed))
                {
                    return (PlcReadFlags)parsed;
                }
            }

            throw new PlcException(FormatErrorMsg);
        }

        /// <summary>
        /// 자동/수동 모드 설정.
        /// </summary>
        /// <param name="auto">true이면 자동, false이면 수동.</param>
        /// <param name="readTimeout"></param>
        internal void SetMode(bool auto, int readTimeout = 500)
        {
            string response = SendCommand($"{ModeCommand}{(auto ? "1" : "0")}", readTimeout, 2);
            ParseError(response, ModeCommand);
        }

        /// <summary>
        /// Fixture ID를 PLC에 보낸다.
        /// </summary>
        /// <param name="fid">Fixture ID.</param>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void SendFid(int fid, int readTimeout = 20_000)
        {
            string response = SendCommand($"{FidCommand}{fid:D3}", readTimeout, 0);
            ParseError(response, FidCommand);
        }

        /// <summary>
        /// PCB를 이동시킨다.
        /// </summary>
        /// <param name="scanZone">스캔존에 새 PCB를 투입할지 여부.</param>
        /// <param name="mesResult">스캔존에 있는 PCB의 이전공정 체크 결과.</param>
        /// <param name="testResult">테스트존에 있는 PCB의 테스트 결과.</param>
        /// <param name="readTimeout">응답 대기 시간.</param>
        /// <returns>스캔존에 새 PCB 투입 여부.</returns>
        internal PcbTestResult MovePcb(PcbScanZone scanZone, PcbMesResult mesResult, PcbTestResult testResult, int readTimeout = SerialPort.InfiniteTimeout)
        {
            string response = SendCommand($"{PcbMoveCommand}{(int)scanZone}{(int)mesResult}{(int)testResult}", readTimeout, 0);
            ParseError(response, PcbMoveCommand);

            var header = RecvCommandHeader + PcbMoveCommand;
            if (response.StartsWith(header) && response.Length > (header.Length + 1))
            {
                // Scan존 PCB 투입 여부.
                if (int.TryParse(response.Substring(header.Length, 1), out int scan))
                {
                    if (scan == 0)
                    {
                        return PcbTestResult.NoPcb;
                    }
                    else if (scan == 1)
                    {
                        // 이전 공정 결과.
                        if (int.TryParse(response.Substring(header.Length + 1, 1), out int prev))
                        {
                            if (prev == (int)PcbTestResult.Pass)
                            {
                                return PcbTestResult.Pass;
                            }
                            else if (prev == (int)PcbTestResult.Fail)
                            {
                                return PcbTestResult.Fail;
                            }
                            else if (prev == (int)PcbTestResult.NotTested)
                            {
                                return PcbTestResult.NotTested;
                            }
                        }
                    }
                }
            }

            throw new PlcException(FormatErrorMsg);
        }

        /// <summary>
        /// 실린더를 하강시킨다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void CylinderDown(int readTimeout = 5_000)
        {
            string response = SendCommand(CylinderDownCommand, readTimeout, 1);
            ParseError(response, CylinderDownCommand);
        }

        /// <summary>
        /// 실린더를 상승시킨다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void CylinderUp(int readTimeout = 5_000)
        {
            string response = SendCommand(CylinderUpCommand, readTimeout, 1);
            ParseError(response, CylinderUpCommand);
        }

        /// <summary>
        /// 실린더를 조금 상승시킨다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void CylinderFctUp(int readTimeout = 5_000)
        {
            string response = SendCommand(CylinderFctUpCommand, readTimeout, 1);
            ParseError(response, CylinderFctUpCommand);
        }

        /// <summary>
        /// 실린더를 중간위치로 상승시킨다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void CylinderMidUp(int readTimeout = 5_000)
        {
            string response = SendCommand(CylinderMidUpCommand, readTimeout, 1);
            ParseError(response, CylinderMidUpCommand);
        }

        /// <summary>
        /// 실린더를 초기화한다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void CylinderInit(int readTimeout = 5_000)
        {
            string response = SendCommand(CylinderInitCommand, readTimeout, 1);
            ParseError(response, CylinderInitCommand);
        }

        /// <summary>
        /// 오류 정보를 PLC에 전송한다.
        /// </summary>
        /// <param name="readTimeout">응답 대기 시간.</param>
        internal void SendError(int readTimeout = 1_000)
        {
            string response = SendCommand($"{ErrorCommand}", readTimeout, 1);
            ParseError(response, ErrorCommand);
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

    internal class PlcException : Exception
    {
        internal PlcException() : base()
        {
        }

        internal PlcException(string message) : base(message)
        {
        }
    }
}
