using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// Serial 디바이스 통신을 위한 기능들을 제공한다.
    /// </summary>
    public class SerialDevice : IDisposable
    {
        // 시리얼 포트 설정.
        public SerialDeviceSetting Setting { get; private set; }

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        internal readonly SerialPort SerialPort = new SerialPort() { NewLine = "\n" };

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = true;

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly SemaphoreSlim semaphoreSlim = new SemaphoreSlim(1, 1);

        private bool disposedValue;

        /// <summary>
        /// 시리얼 포트를 오픈한다.
        /// </summary>
        /// <param name="setting"></param>
        /// <param name="token"></param>
        public void Open(SerialDeviceSetting setting, CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            if (SerialPort.IsOpen)
            {
                return;
            }

            SerialPort.PortName = setting.Port.ToString();
            SerialPort.BaudRate = setting.BaudRate;
            SerialPort.DataBits = setting.DataBits;
            SerialPort.Parity = setting.Parity;
            SerialPort.ReadTimeout = (setting.Timeout <= 0) ? SerialPort.InfiniteTimeout : setting.Timeout;
            SerialPort.WriteTimeout = (setting.Timeout <= 0) ? SerialPort.InfiniteTimeout : setting.Timeout;
            if (!string.IsNullOrEmpty(setting.NewLine))
            {
                SerialPort.NewLine = setting.NewLine;
            }

            CR = setting.CR;
            LF = setting.LF;

            SerialPort.Open();
            Setting = setting;
        }

        public void Close()
        {
            SerialPort.Close();
        }

        // Serial Port로 명령을 보내고 그 응답을 받아서 리턴한다.
        public string SendCommand(string command, int readTimeout, bool checkResponse, CancellationToken token, bool showLog = true)
        {
            token.ThrowIfCancellationRequested();

            bool semaphoreNotEntered = false;
            try
            {
                try
                {
                    semaphoreSlim.Wait(token);
                }
                catch (OperationCanceledException)
                {
                    semaphoreNotEntered = true;
                    throw;
                }

                // Cancel기능을 위한 처리.
                var registration = token.Register(() => SerialPort.Close());

                SerialPort.DiscardInBuffer();
                SerialPort.DiscardOutBuffer();

                if (showLog)
                {
                    Logger.LogCommMessage(Setting.DeviceName, command + ", ReadTimeout = " + readTimeout, true);
                }

                SerialPort.WriteLine(command);

                if (checkResponse)
                {
                    SerialPort.ReadTimeout = readTimeout <= 0 ? SerialPort.InfiniteTimeout : readTimeout;
                    string response = Utils.ReadLine(SerialPort, CR || LF);

                    if (showLog)
                    {
                        Logger.LogCommMessage(Setting.DeviceName, response, false);
                    }

                    registration.Dispose();
                    return response;
                }
                else
                {
                    registration.Dispose();
                    return null;
                }
            }
            finally
            {
                if (!semaphoreNotEntered)
                {
                    semaphoreSlim.Release();
                }
            }
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                    semaphoreSlim.Dispose();
                    SerialPort.Dispose();
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                disposedValue = true;
            }
        }

        // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~SerialDevice()
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
