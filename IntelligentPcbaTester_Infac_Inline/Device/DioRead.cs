using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// DIO 디바이스에 상태 읽기 명령을 보냈을 때 응답문자열에 포함된 비트들의 의미.
    /// </summary>
    [Flags]
    public enum DioReadFlags : uint
    {
        /// <summary>
        /// 0=자동, 1=수동
        /// </summary>
        Mode = 0x00_00_00_01,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        Start1 = 0x00_00_00_02,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        Start2 = 0x00_00_00_04,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        PneumaticSensorTop = 0x00_00_00_08,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        PneumaticSensorFct = 0x00_00_00_10,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        PneumaticSensorBot = 0x00_00_00_20,

        /// <summary>
        /// 0=OPEN, 1=CLOSE
        /// </summary>
        DoorSensor = 0x00_00_00_40,

        /// <summary>
        /// 0=비정상, 1=정상
        /// </summary>
        SafetySensor = 0x00_00_00_80,

        /// <summary>
        /// Unused Bit #8
        /// </summary>
        Unused_8 = 0x00_00_01_00,

        /// <summary>
        /// Unused Bit #9
        /// </summary>
        Unused_9 = 0x00_00_02_00,

        /// <summary>
        /// Unused Bit #10
        /// </summary>
        Unused_10 = 0x00_00_04_00,

        /// <summary>
        /// Unused Bit #11
        /// </summary>
        Unused_11 = 0x00_00_08_00,

        /// <summary>
        /// 0=INLINE, 1=OFFLINE
        /// </summary>
        EnableSw = 0x00_00_10_00,

        /// <summary>
        /// 0=Abnormal, 1=Normal
        /// </summary>
        AirSensorOut = 0x00_00_20_00,

        /// <summary>
        /// Unused Bit #14
        /// </summary>
        Unused_14 = 0x00_00_40_00,

        /// <summary>
        /// Unused Bit #15
        /// </summary>
        Unused_15 = 0x00_00_80_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureUnlockSw = 0x00_01_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureLockSw = 0x00_02_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureUnlockSensor = 0x00_04_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureLockSensor = 0x00_08_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureClampSensor = 0x00_10_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureUnclampSensor = 0x00_20_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        TopFixtureSensor = 0x00_40_00_00,

        /// <summary>
        /// Unused Bit #23
        /// </summary>
        Unused_23 = 0x00_80_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureUnlockSw = 0x01_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureLockSw = 0x02_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureUnlockSensor = 0x04_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureLockSensor = 0x08_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureClampSensor = 0x10_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureUnclampSensor = 0x20_00_00_00,

        /// <summary>
        /// 0=OFF, 1=ON
        /// </summary>
        BotFixtureSensor = 0x40_00_00_00,

        /// <summary>
        /// Unused Bit #31
        /// </summary>
        Unused_31 = 0x80_00_00_00,
    }

    class DioRead : IDisposable
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "DIO(Read)";

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

        // 상태 읽기 Command.
        private const string ReadCommand = "READ32";

        // FID 읽기 Command.
        private const string FidCommand = "FID";

        // 응답이 지정한 문자열로 시작하지 않는 경우의 오류 메시지.
        private const string ResponseHeaderErrorMessage = "응답이 지정한 문자열로 시작하지 않습니다.";

        // 에러 코드 시작 문자열.
        private const string ErrorHeader = "$";
        private const string RetryError = "$0000, Command Error";

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new object();

        // @VER, @READ 등 짧은 명령 타임아웃.
        private const int ShortCommandTimeout = 6000;

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

        /// <summary>
        /// 상태를 읽는다.
        /// </summary>
        /// <param name="readTimeout">ms단위의 Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>상태 코드 조합.</returns>
        internal DioReadFlags ReadStatus(bool showLog, int readTimeout = ShortCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{ReadCommand}", readTimeout, showLog);
            string responseStart = $"{RecvCommandHeader}READ{ParamDelimiter}";
            if (response.StartsWith(responseStart, responseComparison))
            {
                var bitString = response.Substring(responseStart.Length);

                // 공백을 모두 제거한다.
                bitString = Regex.Replace(bitString, "\\s+", "");

                // 2진 문자열을 파싱한다.
                uint parsed = Convert.ToUInt32(bitString, 2);
                return (DioReadFlags)parsed;
            }
            else
            {
                throw new Exception(ResponseHeaderErrorMessage);
            }
        }

        /// <summary>
        /// FID를 읽는 명령을 전송한다.
        /// </summary>
        /// <param name="readTimeout">Serial Port 읽기 타임아웃. 이 시간을 초과하면 <see cref="TimeoutException"/>이 발생한다.</param>
        /// <returns>응답을 파싱하여 얻은 FID를 리턴한다.</returns>
        internal int ReadFid(bool showLog = true, int readTimeout = ShortCommandTimeout)
        {
            string response = SendCommand($"{SendCommandHeader}{FidCommand}", readTimeout, showLog);
            string responseStart = $"{RecvCommandHeader}{FidCommand}{ParamDelimiter}";
            if (response.StartsWith(responseStart, responseComparison))
            {
                // FID 파싱.
                string fidString = response.Substring(responseStart.Length);
                if (int.TryParse(fidString, out int parsed))
                {
                    return parsed;
                }
                else
                {
                    throw new Exception($"FID 문자열 {fidString} 을 파싱할 수 없습니다.");
                }
            }
            else
            {
                throw new Exception(ResponseHeaderErrorMessage);
            }
        }

        // 에러를 파싱하여 예외를 던진다.
        private void ParseError(string response)
        {
            if (response.StartsWith(ErrorHeader, responseComparison))
            {
                int delimiterIndex = response.IndexOf(ParamDelimiter);
                string errorNumberString;
                if (delimiterIndex >= ErrorHeader.Length)
                {
                    errorNumberString = response.Substring(ErrorHeader.Length, delimiterIndex - ErrorHeader.Length);
                }
                else
                {
                    errorNumberString = response.Substring(ErrorHeader.Length);
                }

                if (int.TryParse(errorNumberString, out int errorNumber))
                {
                    string errorMessage;
                    switch (errorNumber)
                    {
                        case (int)DioErrorCode.DoorOpenError:
                            errorMessage = "도어 센서 오류입니다. 문이 열려있는지 확인하세요.";
                            break;
                        case (int)DioErrorCode.NeedLockMode:
                            errorMessage = "Fixture를 Lock 또는 Unlock 모드로 변경하세요.";
                            break;
                        case (int)DioErrorCode.NeedManualMode:
                            errorMessage = "모드를 수동으로 변경하세요.";
                            break;
                        case (int)DioErrorCode.NeedSwLockMode:
                            errorMessage = "SW 모드를 Lock 또는 Unlock 으로 변경하세요.";
                            break;
                        case (int)DioErrorCode.SafetySensorError:
                            errorMessage = "안전 센서 오류입니다.";
                            break;
                        case (int)DioErrorCode.UnlockSensorError:
                            errorMessage = "Unlock 센서 오류입니다.";
                            break;
                        default:
                            errorMessage = $"알 수 없는 오류: {errorNumber}";
                            break;
                    }

                    throw new Exception(errorMessage);
                }
                else
                {
                    throw new Exception(response);
                }
            }
            else
            {
                // Do nothing;
            }
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
