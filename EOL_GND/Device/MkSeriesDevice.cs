using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// MKPOWER DC Power MK3003P/3005P/3010P/6003P/6005P 디바이스 통신 기능을 수행한다.
    /// </summary>
    public class MkSeriesDevice : PowerDevice
    {
        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        // Dispose 패턴에 사용하는 변수.
        private bool disposedValue = false;

        public MkSeriesDevice(string name) : base(DeviceType.MK_P_Series, name)
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

        private double SendAndReadDouble(string command, CancellationToken token)
        {
            string response = SendCommand(command, true, token);
            return double.Parse(response);
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return SendCommand(command, read, token);
        }

        public override double MeasureCurrent(int channel, CancellationToken token)
        {
            return SendAndReadDouble($"IOUT{channel}?", token);
        }

        public override double MeasureVoltage(int channel, CancellationToken token)
        {
            return SendAndReadDouble($"VOUT{channel}?", token);
        }

        /// <summary>
        /// 파워 출력을 허용하거나 차단한다.
        /// </summary>
        /// <param name="on">true이면 출력을 허용, false이면 출력을 차단한다.</param>
        private void SetOutput(bool on, CancellationToken token)
        {
            SendCommand($"OUT{(on ? "1" : "0")}", false, token);
        }

        public override void PowerOff(CancellationToken token)
        {
            SetOutput(false, token);
        }

        public override void PowerOn(CancellationToken token)
        {
            SetOutput(true, token);
        }

        public override bool GetPowerState(CancellationToken token)
        {
            //var response = SendCommand("STATUS?", true, token);
            //byte flags = Convert.ToByte(response.Trim(), 2);
            //return (flags & 0b0100_0000) != 0;
            throw new NotSupportedException();
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
            // 전압 설정.
            SendCommand($"VSET{channel}:{voltage}", false, token);
            if (queryDelay > 0)
            {
                Task.Delay(queryDelay).Wait(token);
            }
            var settedVoltage = SendAndReadDouble($"VSET{channel}?", token);
            if (voltage != settedVoltage)
            {
                throw new Exception($"파워 설정에 실패하였습니다(설정하려는 전압: {voltage}V, 설정된 전압: {settedVoltage}V).");
            }

            // 전류 설정.
            if (current != null)
            {
                SendCommand($"ISET{channel}:{current}", false, token);
                if (queryDelay > 0)
                {
                    Task.Delay(queryDelay).Wait();
                }
                var settedCurrent = SendAndReadDouble($"ISET{channel}?", token);
                if (current != settedCurrent)
                {
                    throw new Exception($"파워 설정에 실패하였습니다(설정하려는 전류: {current}A, 설정된 전류: {settedCurrent}A).");
                }
            }
        }

        public override void ReadPower(int channel, out double voltage, out double current, CancellationToken token)
        {
            voltage = SendAndReadDouble($"VSET{channel}?", token);
            current = SendAndReadDouble($"ISET{channel}?", token);
        }

        public override string ReadSN(CancellationToken token)
        {
            throw new NotSupportedException();
        }

        public override void Reset(CancellationToken token)
        {
            throw new NotSupportedException();
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
