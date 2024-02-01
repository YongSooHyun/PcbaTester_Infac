using System;
using System.IO.Ports;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Barcode 리더기와 통신하는 클래스. 
    /// </summary>
    class BarcodeScanner
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        internal const string DeviceName = "BarcodeScanner";

        private readonly SerialPort serialPort = new SerialPort();

        // CR, LF 사용 여부.
        private bool CR = true;
        private bool LF = true;

        /// <summary>
        /// 바코드를 읽는다.
        /// </summary>
        /// <returns>읽은 바코드.</returns>
        internal string ReadBarcode(int timeout)
        {
            serialPort.PortName = "COM4";
            serialPort.BaudRate = 9600;
            serialPort.ReadTimeout = timeout;
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

            try
            {
                serialPort.Open();
                serialPort.WriteLine("||>SET COM.DMCC-RESPONSE 0");
                serialPort.WriteLine("||>TRIGGER ON");
                string barcode = Utils.ReadLine(serialPort, CR || LF);
                serialPort.WriteLine("||>TRIGGER OFF");
                return barcode;
            }
            finally
            {
                serialPort.Close();
            }
        }
    }
}
