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
    /// ODA EX-Series 파워관련 기능을 제공한다.
    /// </summary>
    public class OdaExSeriesDevice : PowerDevice
    {
        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        // 통신 프로토콜에 따른 예약된 문자열들.
        private const string ValueSeparator = ",";

        // Dispose 패턴에 사용하는 변수.
        private bool disposedValue = false;

        public OdaExSeriesDevice(string name)
            : base(DeviceType.ODA_EX_Series, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            visaDevice.Connect(Setting as VisaDeviceSetting, token);
        }

        public override void Disconnect()
        {
            visaDevice.Disconnect();
        }

        private string SendCommand(string command, bool checkResponse, CancellationToken token)
        {
            visaDevice.WriteLine(command, token);
            if (checkResponse)
            {
                return visaDevice.ReadLine(token);
            }

            return null;
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return SendCommand(command, read, token);
        }

        public override double MeasureCurrent(int channel, CancellationToken token)
        {
            string response = SendCommand("MEAS:CURR?", true, token);
            string escaped = response.Replace("\"", "");
            return double.Parse(escaped);
        }

        public override double MeasureVoltage(int channel, CancellationToken token)
        {
            string response = SendCommand("MEAS:VOLT?", true, token);
            string escaped = response.Replace("\"", "");
            return double.Parse(escaped);
        }

        /// <summary>
        /// 파워 출력을 허용하거나 차단한다.
        /// </summary>
        /// <param name="on">true이면 출력을 허용, false이면 출력을 차단한다.</param>
        private void SetOutput(bool on, CancellationToken token)
        {
            const string outputCommand = "OUTP";
            SendCommand($"{outputCommand} {(on ? "ON" : "OFF")}", false, token);
        }

        public override void PowerOff(CancellationToken token)
        {
            SetOutput(false, token);
        }

        public override void PowerOn(CancellationToken token)
        {
            SetOutput(true, token);
        }

        /// <summary>
        /// 파워 출력상태를 읽어들인다.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        private bool ReadOutput(CancellationToken token)
        {
            string response = SendCommand($"OUTP?", true, token);
            string trimmed = response.Trim();
            if (trimmed == "0")
            {
                return false;
            }
            else if (trimmed == "1")
            {
                return true;
            }
            else
            {
                throw new Exception($"{Setting.DeviceName}의 출력상태 응답형식이 옳지 않습니다.");
            }
        }

        public override bool GetPowerState(CancellationToken token)
        {
            return ReadOutput(token);
        }

        public override string ReadIDN(CancellationToken token)
        {
            return SendCommand("*IDN?", true, token);
        }

        /// <summary>
        /// 전압, 전류를 설정한다.
        /// </summary>
        /// <param name="voltage">설정하려는 전압.(소수점 아래 4자리)</param>
        /// <param name="current">설정하려는 전류.(소수점 아래 4자리) null이면 설정하지 않는다.</param>
        public override void SetPower(int channel, double voltage, double? current, int queryDelay, CancellationToken token)
        {
            string currentStr = current == null ? "" : $"{ValueSeparator}{current}";
            string cmdLine = $"APPL {voltage}{currentStr}";
            SendCommand(cmdLine, false, token);

            // 제대로 설정되었는지 체크.
            if (queryDelay > 0)
            {
                Task.Delay(queryDelay).Wait(token);
            }
            ReadPower(channel, out double settedVoltage, out double settedCurrent, token);
            if (voltage != settedVoltage)
            {
                throw new Exception($"파워 설정에 실패하였습니다(설정하려는 전압: {voltage}V, 설정된 전압: {settedVoltage}V).");
            }
            if (current != null && current != settedCurrent)
            {
                throw new Exception($"파워 설정에 실패하였습니다(설정하려는 전류: {current}A, 설정된 전류: {settedCurrent}A).");
            }
        }

        public override void ReadPower(int channel, out double voltage, out double current, CancellationToken token)
        {
            string response = SendCommand($"APPL?", true, token);
            string escaped = response.Replace("\"", "");
            string[] valueStrArray = escaped.Split(new string[] { ValueSeparator }, StringSplitOptions.None);
            if (valueStrArray.Length == 2)
            {
                voltage = double.Parse(valueStrArray[0]);
                current = double.Parse(valueStrArray[1]);
            }
            else
            {
                throw new Exception($"{Setting.DeviceName}의 전압 전류 응답형식이 옳지 않습니다.");
            }
        }

        public override string ReadSN(CancellationToken token)
        {
            return SendCommand("*SN?", true, token);
        }

        public override void Reset(CancellationToken token)
        {
            SendCommand("*RST", false, token);
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    visaDevice.Dispose();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
