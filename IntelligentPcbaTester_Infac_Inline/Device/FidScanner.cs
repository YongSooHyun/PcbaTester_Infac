using System;
using System.IO.Ports;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// FID Barcode 리더기와 통신하는 클래스. 
    /// </summary>
    class FidScanner : IDisposable
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "FIDScanner";

        private readonly SerialPort serialPort = new SerialPort();

        // CR, LF 사용 여부.
        private bool CR = false;
        private bool LF = false;

        private bool disposedValue = false;

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
        /// 바코드를 읽을 때까지 블록된다.
        /// 타임아웃을 지정한 경우 그 시간동안 읽지 못하면 <see cref="TimeoutException"/>이 발생한다.
        /// </summary>
        /// <param name="timeout">실패할 때까지 타임아웃 시간을 ms 단위로 지정한다.</param>
        /// <returns>바코드 문자열.</returns>
        internal string ReadBarcode(int timeout = SerialPort.InfiniteTimeout)
        {
            serialPort.ReadTimeout = timeout;
            return Utils.ReadLine(serialPort, CR || LF);
        }

        internal void Close()
        {
            serialPort.Close();
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
