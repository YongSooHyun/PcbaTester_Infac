using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EOL_GND.Device.WaveformGeneratorDevice;

namespace EOL_GND.Device
{
    public class KeysightEdu33210SeriesDevice : WaveformGeneratorDevice
    {
        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        public KeysightEdu33210SeriesDevice(string name) : base(DeviceType.Keysight_EDU33210_Series, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            var waveformGenSetting = Setting as WaveformGeneratorDeviceSetting;
            visaDevice.Connect(waveformGenSetting, token);
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

        // 명령을 보내고 double 값을 읽는다.
        private double SendAndReadDouble(string command, CancellationToken token, bool showLog = false)
        {
            visaDevice.WriteLine(command, token);
            return visaDevice.ReadLineDouble(token);
        }

        public override void PwmOff(Channel channel, CancellationToken token)
        {
            // To minimize glitching.
            SendCommand("*CLS", false, token);
            SendCommand($"SOURCE{(int)channel}:PWM:STATE OFF", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE MIN", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET 0", false, token);
            SendCommand($"OUTPUT{(int)channel} OFF", false, token);

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void PwmOn(Channel channel, double? carrierFrequency, VoltageUnit? voltageUnit,
            double carrierAmplitude, double? dcOffsetVoltage, double? phaseAngle, double? pulseDutyCycle, 
            double? pulseLeadingTime, double? pulseTrailingTime, double? pulseWidthDevication, 
            double? dutyCycleDeviation, SignalSource? modulationSource, InternalFuncWaveform? modulationWaveform, 
            double? modulationFrequency, double? outputImpedance, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            SendCommand($"SOURCE{(int)channel}:FUNCTION PULSE", false, token);
            if (carrierFrequency != null)
            {
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:LEADING MIN", false, token);
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:TRAILING MIN", false, token);
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:WIDTH MIN", false, token);
                SendCommand($"SOURCE{(int)channel}:FREQUENCY {carrierFrequency}", false, token);
            }
            if (pulseLeadingTime != null)
            {
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:LEADING {pulseLeadingTime}", false, token);
            }
            if (pulseTrailingTime != null)
            {
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:TRAILING {pulseTrailingTime}", false, token);
            }
            if (pulseDutyCycle != null)
            {
                SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:DCYCLE {pulseDutyCycle}", false, token);
            }
            if (voltageUnit != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:UNIT {voltageUnit}", false, token);
            }
            if (phaseAngle != null)
            {
                SendCommand($"SOURCE{(int)channel}:PHASE {phaseAngle}", false, token);
            }
            if (modulationSource != null)
            {
                SendCommand($"SOURCE{(int)channel}:PWM:SOURCE {modulationSource}", false, token);
                if (modulationSource == SignalSource.Internal)
                {
                    if (modulationWaveform != null)
                    {
                        SendCommand($"SOURCE{(int)channel}:PWM:INTERNAL:FUNCTION {modulationWaveform}", false, token);
                    }
                    if (modulationFrequency != null)
                    {
                        SendCommand($"SOURCE{(int)channel}:PWM:INTERNAL:FREQUENCY {modulationFrequency}", false, token);
                    }
                }
            }
            if (pulseWidthDevication != null)
            {
                SendCommand($"SOURCE{(int)channel}:PWM:DEVIATION {pulseWidthDevication}", false, token);
            }
            if (dutyCycleDeviation != null)
            {
                SendCommand($"SOURCE{(int)channel}:PWM:DEVIATION:DCYCLE {dutyCycleDeviation}", false, token);
            }
            if (outputImpedance != null)
            {
                string impedance = outputImpedance > 10_000 ? "INFINITY" : $"{outputImpedance}";
                SendCommand($"OUTPUT{(int)channel}:LOAD {impedance}", false, token);
            }

            // To minimize glitching.
            SendCommand($"SOURCE{(int)channel}:VOLTAGE MIN", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET 0", false, token);

            SendCommand($"SOURCE{(int)channel}:PWM:STATE ON", false, token);
            SendCommand($"OUTPUT{(int)channel} ON", false, token);

            SendCommand($"SOURCE{(int)channel}:VOLTAGE {carrierAmplitude}", false, token);
            if (dcOffsetVoltage != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET {dcOffsetVoltage}", false, token);
            }

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void AmplitudeModulation(Channel channel, FunctionWaveform carrierFunction, double? carrierFrequency, 
            VoltageUnit? voltageUnit, double carrierAmplitude, double? dcOffsetVoltage, double? phaseAngle, bool? dssc, 
            SignalSource? modulationSource, InternalFuncWaveform? modulationWaveform, double? modulationFrequency, double? modulationDepth, 
            double? outputImpedance, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            SendCommand($"SOURCE{(int)channel}:FUNCTION {carrierFunction}", false, token);
            if (carrierFrequency != null)
            {
                SendCommand($"SOURCE{(int)channel}:FREQUENCY {carrierFrequency}", false, token);
            }
            if (voltageUnit != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:UNIT {voltageUnit}", false, token);
            }
            if (phaseAngle != null)
            {
                SendCommand($"SOURCE{(int)channel}:PHASE {phaseAngle}", false, token);
            }
            if (dssc != null)
            {
                SendCommand($"SOURCE{(int)channel}:AM:DSSC {(dssc == true ? "ON" : "OFF")}", false, token);
            }
            if (modulationSource != null)
            {
                SendCommand($"SOURCE{(int)channel}:AM:SOURCE {modulationSource}", false, token);
                if (modulationSource == SignalSource.Internal)
                {
                    if (modulationWaveform != null)
                    {
                        SendCommand($"SOURCE{(int)channel}:AM:INTERNAL:FUNCTION {modulationWaveform}", false, token);
                    }
                    if (modulationFrequency != null)
                    {
                        SendCommand($"SOURCE{(int)channel}:AM:INTERNAL:FREQUENCY {modulationFrequency}", false, token);
                    }
                }
            }
            if (modulationDepth != null)
            {
                SendCommand($"SOURCE{(int)channel}:AM:DEPTH {modulationDepth}", false, token);
            }
            if (outputImpedance != null)
            {
                string impedance = outputImpedance > 10_000 ? "INFINITY" : $"{outputImpedance}";
                SendCommand($"OUTPUT{(int)channel}:LOAD {impedance}", false, token);
            }

            // To minimize glitching.
            SendCommand($"SOURCE{(int)channel}:VOLTAGE MIN", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET 0", false, token);

            SendCommand($"SOURCE{(int)channel}:AM:STATE ON", false, token);
            SendCommand($"OUTPUT{(int)channel} ON", false, token);

            SendCommand($"SOURCE{(int)channel}:VOLTAGE {carrierAmplitude}", false, token);
            if (dcOffsetVoltage != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET {dcOffsetVoltage}", false, token);
            }

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override string ReadIDN(CancellationToken token)
        {
            return SendCommand("*IDN?", true, token);
        }

        public override void Recall(WgStateLocation location, CancellationToken token)
        {
            SendCommand($"*RCL {(int)location}", false, token);
        }

        public override void Reset(CancellationToken token)
        {
            SendCommand("*RST", false, token);
        }

        public override void Save(WgStateLocation location, CancellationToken token)
        {
            SendCommand($"*SAV {(int)location}", false, token);
        }

        public override void OutputOff(Channel channel, CancellationToken token)
        {
            // To minimize glitching.
            SendCommand("*CLS", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE MIN", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET 0", false, token);
            SendCommand($"OUTPUT{(int)channel} OFF", false, token);

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void WaveformOn(Channel channel, FunctionWaveform function, double? pulseDutyCycle, double? pulseLeadingTime, 
            double? pulseTrailingTime, double? frequency, VoltageUnit? voltageUnit, double amplitude, double? dcOffsetVoltage, 
            double? phaseAngle, double? outputImpedance, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            SendCommand($"SOURCE{(int)channel}:FUNCTION {function}", false, token);
            if (frequency != null)
            {
                if (function == FunctionWaveform.Pulse)
                {
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:LEADING MIN", false, token);
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:TRAILING MIN", false, token);
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:WIDTH MIN", false, token);
                }
                SendCommand($"SOURCE{(int)channel}:FREQUENCY {frequency}", false, token);
            }
            if (function == FunctionWaveform.Pulse)
            {
                if (pulseLeadingTime != null)
                {
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:LEADING {pulseLeadingTime}", false, token);
                }
                if (pulseTrailingTime != null)
                {
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:TRANSITION:TRAILING {pulseTrailingTime}", false, token);
                }
                if (pulseDutyCycle != null)
                {
                    SendCommand($"SOURCE{(int)channel}:FUNCTION:PULSE:DCYCLE {pulseDutyCycle}", false, token);
                }
            }
            if (voltageUnit != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:UNIT {voltageUnit}", false, token);
            }
            if (phaseAngle != null)
            {
                SendCommand($"SOURCE{(int)channel}:PHASE {phaseAngle}", false, token);
            }
            if (outputImpedance != null)
            {
                string impedance = outputImpedance > 10_000 ? "INFINITY" : $"{outputImpedance}";
                SendCommand($"OUTPUT{(int)channel}:LOAD {impedance}", false, token);
            }

            // To minimize glitching.
            SendCommand($"SOURCE{(int)channel}:VOLTAGE MIN", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET 0", false, token);
            SendCommand($"OUTPUT{(int)channel} ON", false, token);
            SendCommand($"SOURCE{(int)channel}:VOLTAGE {amplitude}", false, token);
            if (dcOffsetVoltage != null)
            {
                SendCommand($"SOURCE{(int)channel}:VOLTAGE:OFFSET {dcOffsetVoltage}", false, token);
            }

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override void Synchronize(CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            SendCommand($"PHASE:SYNCHRONIZE", false, token);

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override byte[] Download(EolWaveformGeneratorStep.ImageType? imageType, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            if (imageType != null)
            {
                SendCommand($"HCOPY:SDUMP:DATA:FORMAT {imageType}", false, token);
            }

            // 이미지 데이터 다운로드.
            visaDevice.WriteLine("HCOPY:SDUMP:DATA?", token);
            var imageData = visaDevice.ReadBinaryBlockOfByte(token);

            // 다운로드한 이미지 리턴.
            return imageData;
        }

        private object GetPredefinedValue(object step, string paramName, string predefined, CancellationToken token)
        {
            object value = null;
            switch (paramName)
            {
                case nameof(EolWaveformGeneratorStep.OutputFrequency):
                    if (step is EolWaveformGeneratorStep wgStep)
                    {
                        SendCommand($"FUNCTION {wgStep.OutputFunction}", false, token);
                    }
                    value = SendAndReadDouble($"FREQUENCY? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.OutputVoltage):
                    value = SendAndReadDouble($"VOLTAGE? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.OutputDCVoltageOffset):
                    value = SendAndReadDouble($"VOLTAGE:OFFSET? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.OutputImpedance):
                    value = SendAndReadDouble($"OUTPUT:LOAD? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.PWMPulseWidthDeviation):
                    value = SendAndReadDouble($"PWM:DEVIATION? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.PWMDutyCycleDeviation):
                    value = SendAndReadDouble($"PWM:DEVIATION:DCYCLE? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.PWMModulationFrequency):
                    value = SendAndReadDouble($"PWM:INTERNAL:FREQUENCY? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.ModulationDepth):
                    value = SendAndReadDouble($"AM:DEPTH? {predefined}", token);
                    break;
                case nameof(EolWaveformGeneratorStep.AMModulationFrequency):
                    value = SendAndReadDouble($"AM:INTERNAL:FREQUENCY? {predefined}", token);
                    break;
            }

            return value;
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            return GetPredefinedValue(step, paramName, "MIN", token);
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            return GetPredefinedValue(step, paramName, "MAX", token);
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
