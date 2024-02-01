using System;
using System.Diagnostics;
using System.IO.Ports;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntelligentPcbaTester
{
    enum DioRelayNumber
    {
        All = 0,
        Power1Plus = 1,
        Power1Minus = 2,
        Power2Plus = 3,
        Power2Minus = 4,

        /// <summary>
        /// 실린더 하강 및 중지
        /// </summary>
        CylinderDown = 13,

        /// <summary>
        /// 실린더 상승 및 중지
        /// </summary>
        CylinderUp = 14,

        /// <summary>
        /// Fixture 배출 및 중지
        /// </summary>
        FixtureOut = 15,

        /// <summary>
        /// Fixture 장착 및 중지
        /// </summary>
        FixtureIn = 16,
    }

    enum DioErrorCode
    {
        /// <summary>
        /// 실린더 DOWN 상태에서 UNLOCK 명령시 에러메세지
        /// </summary>
        UnlockSensorError = 2140,

        /// <summary>
        /// MODE를 MANUAL로 변경하세요
        /// </summary>
        NeedManualMode = 9010,

        /// <summary>
        /// SW MODE를 LOCK/UNLOCK 으로 변경하세요
        /// </summary>
        NeedSwLockMode = 9020,

        /// <summary>
        /// 앞 센서 작동시 에러메세지(센서 작동 후 실린더 올라감)
        /// </summary>
        SafetySensorError = 9991,

        /// <summary>
        /// 실린더 내려오는 중 이거나 뒷문이 열린 상태에서 에러메세지
        /// </summary>
        DoorOpenError = 9992,

        /// <summary>
        /// 픽스처를 LOCK/UNLOCK MODE로 변경하세요
        /// </summary>
        NeedLockMode = 9993,
    }

    /// <summary>
    /// GNDKR DIO 디바이스와 통신하는 클래스.
    /// </summary>
    class DioOut : IDisposable
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "DIO(Out)";

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = false;

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        private readonly SerialPort serialPort = new SerialPort();

        // 응답 문자열 체크에 사용되는 문자열 비교 방법.
        private readonly StringComparison responseComparison = StringComparison.OrdinalIgnoreCase;

        // 버전 확인 Command.
        private const string VersionCommand = "@VER";

        // 버전 응답 시작 문자열.
        private const string VersionResponse = ">Version:";

        // 명령 전송, 응답, 파라미터 구분 문자열.
        private const string SendCommandHeader = "@";
        private const string RecvCommandHeader = ">";
        private const string ParamDelimiter = ":";

        // 릴레이 Command.
        private const string RelayCommand = "RLY";

        // 릴레이 ON/OFF 문자열.
        private const string RelayOn = "ON";
        private const string RelayOff = "OFF";

        // 디스플레이 Command.
        private const string DisplayCommand = "DISP";

        // 디스플레이 Pass/Fail 문자열.
        private const string DisplayPass = "PASS";
        private const string DisplayFail = "FAIL";

        // 버저 Command.
        private const string BuzzerCommand = "BUZ";

        // 실린더 다운 Command.
        private const string CylinderDownCommand = "DOWN";

        // 실린더 업 Command.
        private const string CylinderUpCommand = "UP";

        // 실린더 초기화.
        private const string CylinderInitCommand = "CYLINIT";

        // 초기화.
        private const string InitCommand = "INIT";

        // Bottom Fixture Lock Command.
        private const string BotFixtureLockCommand = "LOCK";

        // Bottom Fixture Unlock Command.
        private const string BotFixtureUnlockCommand = "UNLOCK";

        // Top Fixture Lock Command.
        private const string TopFixtureLockCommand = "TFLOCK";

        // Top Fixture Unlock Command.
        private const string TopFixtureUnlockCommand = "TFUNLOCK";

        // Fixture Change Command.
        private const string FixtureChangeCommand = "CHANGE";

        // Cylinder Step Down Command.
        private const string CylinderStepDownCommand = "STEPDOWN";

        // Cylinder Step Up Command.
        private const string CylinderStepUpCommand = "STEPUP";

        // Cylinder FCT Down Command.
        private const string CylinderFctDownCommand = "FCTDOWN";

        // Cylinder FCT Up Command.
        private const string CylinderFctUpCommand = "FCTUP";

        // Bottom Fixture Load = Clamp / Lock
        private const string BotFixtureLoadCommand = "BFLOAD";

        // Bottom Fixture Unload = Unlock / Unclamp
        private const string BotFixtureUnloadCommand = "BFUNLOAD";

        // Top Fixture Load = Clamp / Lock
        private const string TopFixtureLoadCommand = "TFLOAD";

        // Top Fixture Unload = Unlock / Unclamp
        private const string TopFixtureUnloadCommand = "TFUNLOAD";

        // Power Off 명령.
        private const string PowerOffCommand = "PWROFF";

        // Discharge relay 제어 명령.<방전모듈을 500ms 동안 방전(기본)>
        private const string DischargeCommand = "DISC";

        // 경광등 표시 명령들.
        private const string OutIdleCommand = "OUT_IDLE";

        // 이오나이저 명령.
        private const string IonizerCommand = "ION";

        // 응답이 지정한 문자열로 시작하지 않는 경우의 오류 메시지.
        private const string ResponseHeaderErrorMessage = "응답이 지정한 문자열로 시작하지 않습니다.";

        // 에러 코드 시작 문자열.
        private const string ErrorHeader = "$";
        private const string RetryError = "$0000, Command Error";

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new object();

        // @VER, @READ 등 짧은 명령 타임아웃.
        private const int ShortCommandTimeout = 12000;

        // 실린더 다운, 픽스처 락 등 긴 명령 타임아웃.
        private const int LongCommandTimeout = 12000;

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
        private string SendCommand(string command, int readTimeout, bool showLog = true)
        {
            lock (commLockObj)
            {
                int retryCount = 2;
                int retryInterval = 100;
                string response = "";
                do
                {
                    Thread.Sleep(100);

                    serialPort.DiscardInBuffer();
                    serialPort.DiscardOutBuffer();

                    if (showLog)
                    {
                        Logger.LogCommMessage(DeviceName, command, true);
                    }

                    serialPort.WriteLine(command);
                    serialPort.ReadTimeout = readTimeout;

                    response = Utils.ReadLine(serialPort, CR || LF);

                    if (showLog)
                    {
                        Logger.LogCommMessage(DeviceName, response, false);
                    }

                    // Retry할 것인지 체크.
                    if (!response.StartsWith(RetryError, responseComparison) || retryCount <= 0)
                    {
                        break;
                    }

                    retryCount--;
                    if (retryInterval > 0)
                    {
                        Thread.Sleep(retryInterval);
                    }
                } while (retryCount >= 0);

                return response;
            }
        }

        /// <summary>
        /// 디바이스 버전을 체크한다. 연결을 확인하는데 쓸 수도 있다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 읽기 타임아웃. 읽는데 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>버전 문자열. 응답 형식이 맞지 않으면 null을 리턴한다.</returns>
        internal string CheckVersion(int readTimeout = ShortCommandTimeout)
        {
            string response = SendCommand(VersionCommand, readTimeout);
            if (response.StartsWith(VersionResponse, responseComparison))
            {
                return response.Substring(VersionResponse.Length);
            }
            else
            {
                throw new Exception(ResponseHeaderErrorMessage);
            }
        }

        ///// <summary>
        ///// 상태를 읽는다.
        ///// </summary>
        ///// <param name="readTimeout">ms단위의 Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        ///// <returns>상태 코드 조합.</returns>
        //internal DioRead2Flags ReadStatus(bool showLog, int readTimeout = ShortCommandTimeout)
        //{
        //    string response = SendCommand($"{SendCommandHeader}{ReadCommand}", readTimeout, showLog);
        //    string responseStart = $"{RecvCommandHeader}{ReadCommand}{ParamDelimiter}";
        //    if (response.StartsWith(responseStart, responseComparison))
        //    {
        //        var bitString = response.Substring(responseStart.Length);

        //        // 공백을 모두 제거한다.
        //        bitString = Regex.Replace(bitString, "\\s+", "");

        //        // 2진 문자열을 파싱한다.
        //        ushort parsed = Convert.ToUInt16(bitString, 2);
        //        return (DioRead2Flags)parsed;
        //    }
        //    else
        //    {
        //        throw new Exception(ResponseHeaderErrorMessage);
        //    }
        //}

        /// <summary>
        /// Relay 명령을 전송한다.
        /// </summary>
        /// <param name="number"> 명령 번호.</param>
        /// <param name="on">On/Off 스위치.</param>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void SendRelayCommand(DioRelayNumber number, bool on, int readTimeout = ShortCommandTimeout)
        {
            string numberString = number == DioRelayNumber.All ? "ALL" : $"{(int)number}";
            string relayCmd = $"{SendCommandHeader}{RelayCommand}{numberString}";
            string command = $"{relayCmd}{(on ? RelayOn : RelayOff)}";

            string response = SendCommand(command, readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Display 명령을 전송한다.
        /// </summary>
        /// <param name="passed">합격/불량 여부.</param>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void SendDisplayCommand(bool passed, int readTimeout = ShortCommandTimeout)
        {
            string command = $"{SendCommandHeader}{DisplayCommand}{ParamDelimiter}{(passed ? DisplayPass : DisplayFail)}";
            string response = SendCommand(command, readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Buzzer 음을 울리는 명령을 전송한다.
        /// </summary>
        /// <param name="number">버저음 개수.</param>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void SendBuzzerCommand(int number, int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{BuzzerCommand}{number}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// 실린더를 하강시킨다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderDown(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderDownCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        // 에러를 파싱하여 예외를 던진다.
        private void ParseError(string response)
        {
            if (response.StartsWith(ErrorHeader, responseComparison))
            {
                //int delimiterIndex = response.IndexOf(ParamDelimiter);
                //string errorNumberString;
                //if (delimiterIndex >= ErrorHeader.Length)
                //{
                //    errorNumberString = response.Substring(ErrorHeader.Length, delimiterIndex - ErrorHeader.Length);
                //}
                //else
                //{
                //    errorNumberString = response.Substring(ErrorHeader.Length);
                //}

                //if (int.TryParse(errorNumberString, out int errorNumber))
                //{
                //    string errorMessage;
                //    switch (errorNumber)
                //    {
                //        case (int)DioErrorCode.DoorOpenError:
                //            errorMessage = "도어 센서 오류입니다. 문이 열려있는지 확인하세요.";
                //            break;
                //        case (int)DioErrorCode.NeedLockMode:
                //            errorMessage = "Fixture를 Lock 또는 Unlock 모드로 변경하세요.";
                //            break;
                //        case (int)DioErrorCode.NeedManualMode:
                //            errorMessage = "모드를 수동으로 변경하세요.";
                //            break;
                //        case (int)DioErrorCode.NeedSwLockMode:
                //            errorMessage = "SW 모드를 Lock 또는 Unlock 으로 변경하세요.";
                //            break;
                //        case (int)DioErrorCode.SafetySensorError:
                //            errorMessage = "안전 센서 오류입니다.";
                //            break;
                //        case (int)DioErrorCode.UnlockSensorError:
                //            errorMessage = "Unlock 센서 오류입니다.";
                //            break;
                //        default:
                //            errorMessage = $"알 수 없는 오류: {errorNumber}";
                //            break;
                //    }

                //    throw new Exception(errorMessage);
                //}
                //else
                //{
                //    throw new Exception(response);
                //}

                // 에러 메시지 출력.
                throw new Exception(response);
            }
            else
            {
                // Do nothing;
            }
        }

        /// <summary>
        /// 실린더를 상승시킨다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderUp(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderUpCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// 실린더를 초기화한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderInit(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderInitCommand}", readTimeout);

            ParseError(response);
        }

        /// <summary>
        /// DIO 디바이스를 초기화한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void Init(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{InitCommand}", readTimeout);

            ParseError(response);
        }

        /// <summary>
        /// Bottom Fixture를 Lock한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void BottomFixtureLock(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{BotFixtureLockCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Bottom Fixture를 Unlock한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void BottomFixtureUnlock(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{BotFixtureUnlockCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Top Fixture를 Lock한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void TopFixtureLock(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{TopFixtureLockCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Top Fixture를 Unlock한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void TopFixtureUnlock(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{TopFixtureUnlockCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Fixture를 Change한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void FixtureChange(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{FixtureChangeCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Cylinder를 조금 내린다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderStepDown(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderStepDownCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Cylinder를 조금 올린다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderStepUp(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderStepUpCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Cylinder를 올린다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderFctUp(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderFctUpCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Cylinder를 내린다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void CylinderFctDown(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{CylinderFctDownCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Bottom Fixture를 장착한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void BottomFixtureLoad(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{BotFixtureLoadCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Bottom Fixture를 분리한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void BottomFixtureUnload(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{BotFixtureUnloadCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Top Fixture를 장착한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void TopFixtureLoad(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{TopFixtureLoadCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Top Fixture를 분리한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        internal void TopFixtureUnload(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{TopFixtureUnloadCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Power Off.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void PowerOff(int readTimeout = ShortCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{PowerOffCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// Discharge.
        /// </summary>
        /// <param name="readTimeout"></param>
        internal void Discharge(int readTimeout = LongCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{DischargeCommand}", readTimeout);

            // 오류 코드 파싱 및 예외 던지기.
            ParseError(response);
        }

        /// <summary>
        /// 경광등에  idle 표시를 한다.
        /// </summary>
        /// <param name="showLog"></param>
        /// <param name="readTimeout"></param>
        internal void OutIdle(bool showLog, int readTimeout = ShortCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{OutIdleCommand}", readTimeout, showLog);
            ParseError(response);
        }

        /// <summary>
        /// 이오나이저 On/Off 명령을 보낸다.
        /// </summary>
        /// <param name="on"></param>
        /// <param name="readTimeout"></param>
        internal void IonizerControl(bool on, int readTimeout = ShortCommandTimeout)
        {
            string control = on ? "ON" : "OFF";
            string response = SendCommand($"{SendCommandHeader}{IonizerCommand}:{control}", readTimeout);
            ParseError(response);
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DioDevice()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
