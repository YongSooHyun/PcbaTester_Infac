using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.Properties;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EOL_GND.Device.WaveformGeneratorDevice;

namespace EOL_GND.Device
{
    /// <summary>
    /// InfiniiVision 3000T X Series 오실로스코프 기능들을 제공한다.
    /// </summary>
    public class KeysightInfiniiVision3000TXDevice : OscopeDevice
    {
        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        // Download 시 사용되는 변수.
        internal readonly ManualResetEvent DownloadEvent = new ManualResetEvent(true);

        public KeysightInfiniiVision3000TXDevice(string name) : base(DeviceType.Keysight_InfiniiVision_3000T_X, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            var oscopeSetting = Setting as OscopeDeviceSetting;
            visaDevice.Connect(oscopeSetting, token);
        }

        public override void Disconnect()
        {
            visaDevice.Disconnect();
        }

        // 명령을 보내고 파라미터에 따라 응답을 읽어 리턴한다.
        private string SendCommand(string command, bool readResponse, CancellationToken token)
        {
            // 다운로드가 종료되기를 기다린다.
            DownloadEvent.WaitOne();

            visaDevice.WriteLine(command, token);

            if (readResponse)
            {
                return visaDevice.ReadLine(token);
            }

            return null;
        }

        private double SendAndReadLineDouble(string command, CancellationToken token)
        {
            // 다운로드가 종료되기를 기다린다.
            DownloadEvent.WaitOne();

            visaDevice.WriteLine(command, token);
            return visaDevice.ReadLineDouble(token);
        }

        public override string RunCommand(string command, bool read,  int readTimeout, CancellationToken token)
        {
            return SendCommand(command, read, token);
        }

        public override string ReadIDN(CancellationToken token)
        {
            return SendCommand("*IDN?", true, token);
        }

        public override void Recall(OscopeStateLocation location, CancellationToken token)
        {
            SendCommand($"*RCL {(int)location}", false, token);
        }

        public override void Reset(CancellationToken token)
        {
            SendCommand("*RST", false, token);
        }

        public override void Save(OscopeStateLocation location, CancellationToken token)
        {
            SendCommand($"*SAV {(int)location}", false, token);
        }

        public override void Configure(List<OscopeChannelSettings> channelSettings, OscopeTriggerSettings triggerSettings, 
            OscopeAcquireSettings acquireSettings, OscopeTimebaseSettings timebaseSettings, CancellationToken token)
        {
            // 계측기 초기화.
            SendCommand("*CLS", false, token);

            // 자동 설정.
            //if (autoScale)
            //{
            //    SendCommand(":AUTOSCALE", false, token);
            //}

            // Timebase 설정.
            if (timebaseSettings.Mode != null)
            {
                SendCommand($":TIMEBASE:MODE {timebaseSettings.Mode}", false, token);
            }
            if (timebaseSettings.Position != null)
            {
                SendCommand($":TIMEBASE:POSITION {timebaseSettings.Position}", false, token);
            }
            if (timebaseSettings.Range != null)
            {
                SendCommand($":TIMEBASE:RANGE {timebaseSettings.Range}", false, token);
            }
            if (timebaseSettings.Reference != null)
            {
                SendCommand($":TIMEBASE:REFERENCE {timebaseSettings.Reference}", false, token);
            }
            if (timebaseSettings.Scale != null)
            {
                SendCommand($":TIMEBASE:SCALE {timebaseSettings.Scale}", false, token);
            }
            if (timebaseSettings.Vernier != null)
            {
                SendCommand($":TIMEBASE:VERNIER {(timebaseSettings.Vernier == true ? "ON" : "OFF")}", false, token);
            }

            // 채널 설정.
            for (int i = 0; i < channelSettings.Count; i++)
            {
                int channel = i + 1;
                var settings = channelSettings[i];
                if (settings == null)
                {
                    continue;
                }

                if (settings.BandwidthLimit != null)
                {
                    SendCommand($":CHANNEL{channel}:BWLIMIT {(settings.BandwidthLimit == true ? "ON" : "OFF")}", false, token);
                }
                if (settings.Coupling != null)
                {
                    SendCommand($":CHANNEL{channel}:COUPLING {settings.Coupling}", false, token);
                }
                if (settings.Impedance != null)
                {
                    string impedance;
                    if (settings.Impedance == OscopeChannelSettings.ImpedanceMode._50_Ω)
                    {
                        impedance = "FIFTY";
                    }
                    else
                    {
                        impedance = "ONEMEG";
                    }
                    SendCommand($":CHANNEL{channel}:IMPEDANCE {impedance}", false, token);
                }
                if (settings.Invert != null)
                {
                    SendCommand($":CHANNEL{channel}:INVERT {(settings.Invert == true ? "ON" : "OFF")}", false, token);
                }
                if (settings.Label != null)
                {
                    SendCommand($":CHANNEL{channel}:LABEL \"{settings.Label}\"", false, token);
                }
                if (settings.Offset != null)
                {
                    SendCommand($":CHANNEL{channel}:OFFSET {settings.Offset}", false, token);
                }
                if (settings.ProbeAttenuationFactor != null)
                {
                    SendCommand($":CHANNEL{channel}:PROBE {settings.ProbeAttenuationFactor}", false, token);
                }
                if (settings.ProbeExternalGain != null)
                {
                    SendCommand($":CHANNEL{channel}:PROBE:EXTERNAL:GAIN {settings.ProbeExternalGain}", false, token);
                }
                if (settings.ProbeExternalUnit != null)
                {
                    SendCommand($":CHANNEL{channel}:PROBE:EXTERNAL:UNIT {settings.ProbeExternalUnit}", false, token);
                }
                if (settings.ProbeExternal != null)
                {
                    SendCommand($":CHANNEL{channel}:PROBE:EXTERNAL {(settings.ProbeExternal == true ? "ON" : "OFF")}", false, token);
                }
                if (settings.ProbeSkew != null)
                {
                    SendCommand($":CHANNEL{channel}:PROBE:SKEW {settings.ProbeSkew}", false, token);
                }
                if (settings.Range != null)
                {
                    SendCommand($":CHANNEL{channel}:RANGE {settings.Range}", false, token);
                }
                if (settings.Scale != null)
                {
                    SendCommand($":CHANNEL{channel}:SCALE {settings.Scale}", false, token);
                }
                if (settings.Unit != null)
                {
                    SendCommand($":CHANNEL{channel}:UNIT {settings.Unit}", false, token);
                }
                if (settings.Vernier != null)
                {
                    SendCommand($":CHANNEL{channel}:VERNIER {(settings.Vernier == true ? "ON" : "OFF")}", false, token);
                }
                if (settings.Display != null)
                {
                    SendCommand($":CHANNEL{channel}:DISPLAY {(settings.Display == true ? "ON" : "OFF")}", false, token);
                }
            }

            // 트리거 Edge 설정.
            if (triggerSettings.EdgeModeSettings.Source != null)
            {
                SendCommand($":TRIGGER:EDGE:SOURCE {triggerSettings.EdgeModeSettings.Source}", false, token);
            }

            if (triggerSettings.EdgeModeSettings.Coupling != null)
            {
                string coupling;
                switch (triggerSettings.EdgeModeSettings.Coupling)
                {
                    case OscopeTriggerSettings.EdgeSettings.InputCouplingMode.AC:
                        coupling = "AC";
                        break;
                    case OscopeTriggerSettings.EdgeSettings.InputCouplingMode.LowFrequencyReject:
                        coupling = "LFREJECT";
                        break;
                    case OscopeTriggerSettings.EdgeSettings.InputCouplingMode.DC:
                    default:
                        coupling = "DC";
                        break;
                }
                SendCommand($":TRIGGER:EDGE:COUPLING {coupling}", false, token);
            }
            if (triggerSettings.EdgeModeSettings.Level != null)
            {
                SendCommand($":TRIGGER:EDGE:LEVEL {triggerSettings.EdgeModeSettings.Level}", false, token);
            }

            if (triggerSettings.EdgeModeSettings.Reject != null)
            {
                string reject;
                switch (triggerSettings.EdgeModeSettings.Reject)
                {
                    case OscopeTriggerSettings.EdgeSettings.RejectMode.LowFrequencyReject:
                        reject = "LFREJECT";
                        break;
                    case OscopeTriggerSettings.EdgeSettings.RejectMode.HighFrequencyReject:
                        reject = "HFREJECT";
                        break;
                    case OscopeTriggerSettings.EdgeSettings.RejectMode.OFF:
                    default:
                        reject = "OFF";
                        break;
                }
                SendCommand($":TRIGGER:EDGE:REJECT {reject}", false, token);
            }
            if (triggerSettings.EdgeModeSettings.Slope != null)
            {
                SendCommand($":TRIGGER:EDGE:SLOPE {triggerSettings.EdgeModeSettings.Slope}", false, token);
            }

            // 트리거 설정.
            if (triggerSettings.HighFrequencyRejectFilter != null)
            {
                SendCommand($":TRIGGER:HFREJECT {(triggerSettings.HighFrequencyRejectFilter == true ? "ON" : "OFF")}", false, token);
            }
            if (triggerSettings.HoldoffTime != null)
            {
                SendCommand($":TRIGGER:HOLDOFF {triggerSettings.HoldoffTime}", false, token);
            }
            if (triggerSettings.NoiseRejectFilter != null)
            {
                SendCommand($":TRIGGER:NREJECT {(triggerSettings.NoiseRejectFilter == true ? "ON" : "OFF")}", false, token);
            }
            if (triggerSettings.Sweep != null)
            {
                SendCommand($":TRIGGER:SWEEP {triggerSettings.Sweep}", false, token);
            }
            if (triggerSettings.LevelAutoSetup)
            {
                SendCommand(":TRIGGER:LEVEL:ASETUP", false, token);
            }
            if (triggerSettings.Force)
            {
                SendCommand(":TRIGGER:FORCE", false, token);
            }

            // Acquire 설정.
            if (acquireSettings.DigitizerMode != null)
            {
                SendCommand(":ACQUIRE:DIGITIZER " + (acquireSettings.DigitizerMode.GetValueOrDefault() ? "ON" : "OFF"), false, token);
                if (acquireSettings.DigitizerMode.GetValueOrDefault())
                {
                    if (acquireSettings.Points != null)
                    {
                        SendCommand(":ACQUIRE:POINTS " + acquireSettings.Points.ToString(), false, token);
                    }
                    if (acquireSettings.SampleRate != null)
                    {
                        SendCommand(":ACQUIRE:SRATE " + acquireSettings.SampleRate, false, token);
                    }
                }
            }

            // 앞의 설정들에 오류가 없는지 체크.
            string response = SendCommand(":SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void Capture(bool autoScale, List<OscopeChannel> sources, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            var sourceList = string.Join(",", sources);
            if (autoScale)
            {
                SendCommand($":AUTOSCALE {sourceList}", false, token);
            }

            // 캡처 시작.
            var registration = token.Register(() => SendCommand("*CLS", false, CancellationToken.None));

            SendCommand($":DIGITIZE {sourceList}", false, CancellationToken.None);
            var response = SendCommand("*OPC?", true, token);
            registration.Dispose();
        }

        public override void BeginCapture(bool autoScale, List<OscopeChannel> sources, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            var sourceList = string.Join(",", sources);
            if (autoScale)
            {
                SendCommand($":AUTOSCALE {sourceList}", false, token);
            }

            // 캡처 시작.
            var registration = token.Register(() => SendCommand("*CLS", false, CancellationToken.None));

            SendCommand($":DIGITIZE {sourceList}", false, CancellationToken.None);

            registration.Dispose();
        }

        public override void EndCapture(CancellationToken token)
        {
            // 캡처 종료 대기.
            var registration = token.Register(() => SendCommand("*CLS", false, CancellationToken.None));
            var response = SendCommand("*OPC?", true, token);
            registration.Dispose();
        }

        public override double? Measure(OscopeMeasurementSettings measurementSettings, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            // 측정 소스 설정.
            //SendCommand($":{measurementSettings.Source1}:DISPLAY ON", false, token);
            string command;
            if (measurementSettings.Source2 != null)
            {
                command = $":MEASURE:SOURCE {measurementSettings.Source1},{measurementSettings.Source2}";
            }
            else
            {
                command = $":MEASURE:SOURCE {measurementSettings.Source1}";
            }
            SendCommand(command, false, token);

            double? measuredValue = null;
            switch (measurementSettings.Method)
            {
                case OscopeMeasurementSettings.MeasureMethod.Clear:
                    SendCommand(":MEASURE:CLEAR", false, token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.BitRate:
                    SendCommand(":MEASURE:BRATE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:BRATE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.BurstWidth:
                    SendCommand(":MEASURE:BWIDTH", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:BWIDTH?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Counter:
                    SendCommand(":MEASURE:COUNTER", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:COUNTER?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Delay:
                    if (measurementSettings.DelayEdgeSelectMode == OscopeMeasurementSettings.EdgeSelectMode.Manual)
                    {
                        SendCommand($":MEASURE:DELAY:DEFINE {measurementSettings.Source1EdgeSlope}, {measurementSettings.Source1EdgeNumber}, " +
                            $"{measurementSettings.Source1EdgeThreshold}, {measurementSettings.Source2EdgeSlope}, " +
                            $"{measurementSettings.Source2EdgeNumber}, {measurementSettings.Source2EdgeThreshold}", false, token);
                    }
                    SendCommand($":MEASURE:DELAY {measurementSettings.DelayEdgeSelectMode}", false, token);
                    measuredValue = SendAndReadLineDouble($":MEASURE:DELAY? {measurementSettings.DelayEdgeSelectMode}", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.PositiveDutyCycle:
                    SendCommand(":MEASURE:DUTYCYCLE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:DUTYCYCLE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.FallTime:
                    SendCommand(":MEASURE:FALLTIME", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:FALLTIME?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Frequency:
                    SendCommand(":MEASURE:FREQUENCY", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:FREQUENCY?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.NegativeDutyCycle:
                    SendCommand(":MEASURE:NDUTY", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:NDUTY?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.FallingEdgeCount:
                    SendCommand(":MEASURE:NEDGES", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:NEDGES?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.FallingPulseCount:
                    SendCommand(":MEASURE:NPULSES", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:NPULSES?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.NegativePulseWidth:
                    SendCommand(":MEASURE:NWIDTH", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:NWIDTH?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Overshoot:
                    SendCommand(":MEASURE:OVERSHOOT", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:OVERSHOOT?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.RisingEdgeCount:
                    SendCommand(":MEASURE:PEDGES", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PEDGES?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Period:
                    SendCommand(":MEASURE:PERIOD", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PERIOD?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Phase:
                    if (measurementSettings.DelayEdgeSelectMode == OscopeMeasurementSettings.EdgeSelectMode.Manual)
                    {
                        SendCommand($":MEASURE:DELAY:DEFINE {measurementSettings.Source1EdgeSlope}, {measurementSettings.Source1EdgeNumber}, " +
                            $"{measurementSettings.Source1EdgeThreshold}, {measurementSettings.Source2EdgeSlope}, " +
                            $"{measurementSettings.Source2EdgeNumber}, {measurementSettings.Source2EdgeThreshold}", false, token);
                    }
                    SendCommand(":MEASURE:PHASE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PHASE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.RisingPulseCount:
                    SendCommand(":MEASURE:PPULSES", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PPULSES?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.Preshoot:
                    SendCommand(":MEASURE:PRESHOOT", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PRESHOOT?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.PositivePulseWidth:
                    SendCommand(":MEASURE:PWIDTH", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:PWIDTH?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.RiseTime:
                    SendCommand(":MEASURE:RISETIME", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:RISETIME?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.StanardDeviation:
                    SendCommand(":MEASURE:SDEVIATION", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:SDEVIATION?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VAmplitude:
                    SendCommand(":MEASURE:VAMPLITUDE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VAMPLITUDE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VAverage:
                    SendCommand(":MEASURE:VAVERAGE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VAVERAGE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VBase:
                    SendCommand(":MEASURE:VBASE", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VBASE?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VMax:
                    SendCommand(":MEASURE:VMAX", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VMAX?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VMin:
                    SendCommand(":MEASURE:VMIN", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VMIN?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VPP:
                    SendCommand(":MEASURE:VPP", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VPP?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VRatio:
                    SendCommand(":MEASURE:VRATIO", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VRATIO?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VRMS:
                    string sendCommand = $":MEASure:VRMS Display, {measurementSettings.VrmsType}";
                    string queryCommand = $":MEASure:VRMS? Display, {measurementSettings.VrmsType}";
                    SendCommand(sendCommand, false, token);
                    measuredValue = SendAndReadLineDouble(queryCommand, token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.VTop:
                    SendCommand(":MEASURE:VTOP", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:VTOP?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.XMax:
                    SendCommand(":MEASURE:XMAX", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:XMAX?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.XMin:
                    SendCommand(":MEASURE:XMIN", false, token);
                    measuredValue = SendAndReadLineDouble(":MEASURE:XMIN?", token);
                    break;
                case OscopeMeasurementSettings.MeasureMethod.YatX:
                    SendCommand($":MEASURE:YATX {measurementSettings.HorizontalLocation}", false, token);
                    measuredValue = SendAndReadLineDouble($":MEASURE:YATX? {measurementSettings.HorizontalLocation}", token);
                    break;
            }

            return measuredValue;
        }

        public override double DigitalVoltmeter(OscopeDvmSettings dvmSettings, CancellationToken token)
        {
            SendCommand("*CLS", false, token);

            if (dvmSettings.AutoRange != null)
            {
                SendCommand($":DVM:ARANGE {(dvmSettings.AutoRange == true ? "ON" : "OFF")}", false, token);
            }
            if (dvmSettings.Mode != null)
            {
                SendCommand($":DVM:MODE {dvmSettings.Mode}", false, token);
            }
            SendCommand($":DVM:SOURCE {dvmSettings.Source}", false, token);
            if (dvmSettings.Enable != null)
            {
                SendCommand($":DVM:ENABLE {(dvmSettings.Enable == true ? "ON" : "OFF")}", false, token);
            }

            // 앞의 설정들에 오류가 없는지 체크.
            string response = SendCommand(":SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);

            double invalidValue = 9.9E+37;
            double measuredValue;
            while (true)
            {
                measuredValue = SendAndReadLineDouble(":DVM:CURRENT?", token);
                if (measuredValue < invalidValue)
                {
                    break;
                }
                MultimediaTimer.Delay(20, token).Wait(token);
            }
            return measuredValue;
        }

        public override byte[] Download(OscopeDisplaySettings displaySettings, CancellationToken token)
        {
            try
            {
                SendCommand("*CLS", false, token);
                SendCommand(":HARDCOPY:INKSAVER OFF", false, token);

                if (displaySettings.Annotation != null)
                {
                    if (string.IsNullOrEmpty(displaySettings.Annotation))
                    {
                        SendCommand(":DISPLAY:ANNOTATION1 OFF", false, token);
                    }
                    else
                    {
                        SendCommand($":DISPLAY:ANNOTATION1:TEXT \"{displaySettings.Annotation}\"", false, token);
                        SendCommand(":DISPLAY:ANNOTATION1 ON", false, token);
                    }
                }
                if (displaySettings.Label != null)
                {
                    SendCommand($":DISPLAY:LABEL {(displaySettings.Label == true ? "ON" : "OFF")}", false, token);
                }

                // 이미지 데이터 다운로드.
                SendCommand("*OPC?", true, token);
                visaDevice.WriteLine($":DISPLAY:DATA? {displaySettings.DownloadImageFormat}, {displaySettings.DownloadImagePalette}", token);

                // 바이너리 블록을 다 읽을 때까지 다른 명령 실행 못하도록 함.
                DownloadEvent.Reset();

                var imageData = visaDevice.ReadBinaryBlockOfByte(token);

                // 다운로드한 이미지 리턴.
                return imageData;
            }
            finally
            {
                DownloadEvent.Set();
            }
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            // do nothing.
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            // do nothing.
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
