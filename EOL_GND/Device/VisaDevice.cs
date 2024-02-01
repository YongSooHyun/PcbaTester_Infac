using EOL_GND.Common;
using EOL_GND.Model;
using Ivi.Visa;
using Keysight.Visa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// VISA 디바이스 통신을 위한 기능들을 제공한다.
    /// </summary>
    public class VisaDevice : IDisposable
    {
        /// <summary>
        /// 디바이스 연결 상태.
        /// </summary>
        public bool Connected => mbSession != null;

        // 연결 설정.
        public VisaDeviceSetting Setting { get; private set; }

        // 디바이스 연결을 관리.
        private MessageBasedSession mbSession = null;

        // 연결이 안된 상태에서 명령을 수행할 때 오류 메시지.
        private const string NotConnectedMessage = "VISA 디바이스에 연결되지 않았습니다.";

        // Dispose패턴 필드.
        private bool disposedValue = false;

        /// <summary>
        /// 해당 VISA 주소를 가진 디바이스에 연결한다.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public void Connect(VisaDeviceSetting setting, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (Connected)
            {
                return;
            }

            if (string.IsNullOrEmpty(setting.Address))
            {
                throw new Exception("VISA Address가 비어 있습니다.");
            }

            using (var resManager = new ResourceManager())
            {
                var registration = token.Register(() => resManager.Dispose());

                // Debugging.
                Logger.LogVerbose($"Connecting to VISA device {setting.DeviceName}({setting.Address})");

                mbSession?.Dispose();
                mbSession = null;
                var session = resManager.Open(setting.Address);

                Logger.LogInfo($"Connected to {session.ResourceName}");
                registration.Dispose();

                if (session is MessageBasedSession mbs)
                {
                    mbs.TimeoutMilliseconds = setting.IOTimeout;
                    mbSession = mbs;
                    Setting = setting;

                    // Fluke 8845A/8846A DMM 인 경우 필요.
                    if (mbs.SessionType == SessionType.TcpipSocketSession)
                    {
                        mbs.TerminationCharacterEnabled = true;
                    }
                }
                else
                {
                    session?.Dispose();
                    throw new Exception($"{setting.Address}는 지원되지 않는 VISA디바이스입니다.");
                }
            }
        }

        /// <summary>
        /// 디바이스에 명령을 전송한다.
        /// </summary>
        /// <param name="command"></param>
        /// <exception cref="Exception"></exception>
        public void WriteLine(string command, CancellationToken token, bool showLog = true)
        {
            if (!Connected && Setting != null)
            {
                // 설정이 비어있지 않으면 연결 시도.
                Connect(Setting, CancellationToken.None);
            }

            if (!Connected)
            {
                throw new Exception(NotConnectedMessage);
            }

            token.ThrowIfCancellationRequested();
            var registration = token.Register(Disconnect);

            try
            {
                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, command, true);
                }
                mbSession.FormattedIO.WriteLine(command);
            }
            catch (NativeVisaException nativeVisaEx)
            {
                if (nativeVisaEx.ErrorCode == NativeErrorCode.ConnectionLost && Setting != null)
                {
                    // 연결이 끊어진 경우 다시 시도.
                    Disconnect();
                    Connect(Setting, token);
                    WriteLine(command, token, showLog);
                }
                else
                {
                    throw;
                }
            }
            finally
            {
                registration.Dispose();
            }
        }

        /// <summary>
        /// 디바이스 응답을 읽는다.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public string ReadLine(CancellationToken token, bool showLog = true)
        {
            if (!Connected)
            {
                throw new Exception(NotConnectedMessage);
            }

            token.ThrowIfCancellationRequested();
            var registration = token.Register(Disconnect);

            try
            {
                var response = mbSession.FormattedIO.ReadLine();
                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, response, false);
                }
                return response;
            }
            finally
            {
                registration.Dispose();
            }
        }

        /// <summary>
        /// double 값을 계측기로부터 읽는다.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double ReadLineDouble(CancellationToken token, bool showLog = true)
        {
            if (!Connected)
            {
                throw new Exception(NotConnectedMessage);
            }

            token.ThrowIfCancellationRequested();
            var registration = token.Register(Disconnect);

            try
            {
                double value = mbSession.FormattedIO.ReadLineDouble();
                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, value.ToString(), false);
                }
                return value;
            }
            finally
            {
                registration.Dispose();
            }
        }

        /// <summary>
        /// 콤마로 구분된 double 리스트를 응답 라인에서 읽는다.
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public double[] ReadLineListOfDouble(CancellationToken token, bool showLog = true)
        {
            if (!Connected)
            {
                throw new Exception(NotConnectedMessage);
            }

            token.ThrowIfCancellationRequested();
            var registration = token.Register(Disconnect);

            try
            {
                double[] values = mbSession.FormattedIO.ReadLineListOfDouble();
                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, string.Join(", ", values), false);
                }
                return values;
            }
            finally
            {
                registration.Dispose();
            }
        }

        /// <summary>
        /// 바이너리 데이터를 읽는다.
        /// </summary>
        /// <param name="token"></param>
        /// <param name="showLog"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public byte[] ReadBinaryBlockOfByte(CancellationToken token, bool showLog = true)
        {
            if (!Connected)
            {
                throw new Exception(NotConnectedMessage);
            }

            token.ThrowIfCancellationRequested();
            token.Register(Disconnect);

            // Binary 데이터 전송은 시간이 오래 걸리므로 타임아웃을 20초로 한다.
            var prevTimeout = mbSession.TimeoutMilliseconds;
            try
            {
                mbSession.TimeoutMilliseconds = 20_000;
                var bytes = mbSession.FormattedIO.ReadBinaryBlockOfByte();
                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, $"Binary block {bytes?.Length} bytes", false);
                }
                return bytes;
            }
            finally
            {
                mbSession.TimeoutMilliseconds = prevTimeout;
            }
        }

        /// <summary>
        /// SYST:ERR? 명령 응답 문자열을 파싱해 예외를 던진다.
        /// </summary>
        /// <exception cref="Exception"></exception>
        public static void ScpiSystemError(string response)
        {
            var words = response.Split(',');
            if (words.Length != 2 || !int.TryParse(words[0], out int errorCode))
            {
                throw new Exception($"SCPI 명령 형식 오류: {response}");
            }
            if (errorCode != 0)
            {
                throw new Exception($"SCPI Error: {response}");
            }
        }

        /// <summary>
        /// VISA 디바이스 연결을 해제한다.
        /// </summary>
        public void Disconnect()
        {
            if (mbSession != null)
            {
                mbSession.Dispose();
                mbSession = null;
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    mbSession?.Dispose();
                    mbSession = null;
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                disposedValue = true;
            }
        }

        // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~VisaDevice()
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
