using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public class Keysight33502ADevice : AmplifierDevice
    {
        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        public Keysight33502ADevice(string name) : base(DeviceType.Keysight_33502A, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            var ampSetting = Setting as AmplifierDeviceSetting;
            visaDevice.Connect(ampSetting, token);
        }

        public override void Disconnect()
        {
            visaDevice.Disconnect();
        }

        // 명령을 보내고 파라미터에 따라 응답을 읽어 리턴한다.
        private string SendCommand(string command, bool readResponse, CancellationToken token)
        {
            visaDevice.WriteLine(command, token);

            if (readResponse)
            {
                return visaDevice.ReadLine(token);
            }

            return null;
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return SendCommand(command, read, token);
        }

        public override string ReadIDN(CancellationToken token)
        {
            return SendCommand("*IDN?", true, token);
        }

        public override void Reset(CancellationToken token)
        {
            SendCommand("*RST", false, token);
        }

        public override void Configure(AmplifierChannels channels, AmplifierCoupling? inputCoupling, AmplifierImpedance? inputImpedance, 
            AmplifierGain? gain, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            // Input coupling.
            if (inputCoupling != null)
            {
                if (channels.HasFlag(AmplifierChannels.CH1))
                {
                    SendCommand($"INPUT1:COUPLING {inputCoupling}", false, token);
                }
                if (channels.HasFlag(AmplifierChannels.CH2))
                {
                    SendCommand($"INPUT2:COUPLING {inputCoupling}", false, token);
                }
            }

            // Input impedance.
            if (inputImpedance != null)
            {
                string impedance = inputImpedance == AmplifierImpedance._50_Ω ? "50" : "1E6";
                if (channels.HasFlag(AmplifierChannels.CH1))
                {
                    SendCommand($"INPUT1:IMPEDANCE {impedance}", false, token);
                }
                if (channels.HasFlag(AmplifierChannels.CH2))
                {
                    SendCommand($"INPUT2:IMPEDANCE {impedance}", false, token);
                }
            }

            // Gain.
            if (gain != null)
            {
                string gainStr = gain == AmplifierGain.Direct ? "DIRECT" : "AMPLIFIER";
                if (channels.HasFlag(AmplifierChannels.CH1))
                {
                    SendCommand($"ROUTE1 {gainStr}", false, token);
                }
                if (channels.HasFlag(AmplifierChannels.CH2))
                {
                    SendCommand($"ROUTE2 {gainStr}", false, token);
                }
            }

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void SetOutput(AmplifierChannels channels, bool on, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            string onStr = on ? "ON" : "OFF";
            if (channels.HasFlag(AmplifierChannels.CH1))
            {
                SendCommand($"OUTPUT1 {onStr}", false, token);
            }
            if (channels.HasFlag(AmplifierChannels.CH2))
            {
                SendCommand($"OUTPUT2 {onStr}", false, token);
            }

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
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
