using EOL_GND.Common;
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
    /// 파형 발생기.
    /// </summary>
    public abstract class WaveformGeneratorDevice : TestDevice
    {
        /// <summary>
        /// 식별문자열 읽기.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadIDN(CancellationToken token);

        /// <summary>
        /// 지정한 비휘발성 메모리로부터 계측기상태를 불러들인다.
        /// </summary>
        /// <param name="location">0|1|2|3|4 : 0은 계측기의 power down 상태를 보관한다.</param>
        public abstract void Recall(WgStateLocation location, CancellationToken token);

        /// <summary>
        /// 지정한 비휘발성 메모리에 계측기상태를 보관한다.
        /// </summary>
        /// <param name="location">0|1|2|3|4 : 0은 계측기의 power down 상태를 보관한다.</param>
        public abstract void Save(WgStateLocation location, CancellationToken token);

        /// <summary>
        /// 계측기를 공장 출고 상태로 초기화한다.
        /// </summary>
        public abstract void Reset(CancellationToken token);

        /// <summary>
        /// Allows you to add amplitude modulation (AM) to a carrier waveform.
        /// </summary>
        /// <param name="channel">output channel.</param>
        /// <param name="carrierFunction">carrier waveform's function.</param>
        /// <param name="carrierFrequency">carrier waveform's frequency, 1 μHz to 10 MHz.</param>
        /// <param name="voltageUnit">Selects the units for output amplitude.</param>
        /// <param name="carrierAmplitude">carrier waveform's amplitude(voltage), 1 mVpp to 5 V for a 50 Ω load or 10 V for a high-impedance load.</param>
        /// <param name="dcOffsetVoltage">carrier waveform's voltage offset, ± 5 VDC into 50 Ω.</param>
        /// <param name="phaseAngle">Sets waveform's phase offset angle.</param>
        /// <param name="dssc">Selects Amplitude Modulation mode − Double Sideband Suppressed Carrier (ON) or AM modulated carrier with sidebands(OFF).</param>
        /// <param name="modulationSource">the source of the modulating signal.</param>
        /// <param name="modulationWaveform">shape of the internal modulating waveform.</param>
        /// <param name="modulationFrequency">frequency at which output pulse width shifts through its pulse width deviation.</param>
        /// <param name="modulationDepth">Sets internal modulation depth ("percent modulation") in percent.</param>
        /// <param name="outputImpedance"></param>
        /// <param name="token"></param>
        public abstract void AmplitudeModulation(Channel channel, FunctionWaveform carrierFunction, double? carrierFrequency, 
            VoltageUnit? voltageUnit, double carrierAmplitude, double? dcOffsetVoltage, double? phaseAngle, bool? dssc, 
            SignalSource? modulationSource, InternalFuncWaveform? modulationWaveform, double? modulationFrequency,
            double? modulationDepth, double? outputImpedance, CancellationToken token);

        /// <summary>
        /// PWM(Pulse Width Modulation).
        /// </summary>
        /// <param name="channel">output channel.</param>
        /// <param name="carrierFrequency">carrier waveform's frequency, 1 μHz to 10 MHz.</param>
        /// <param name="carrierAmplitude">carrier waveform's amplitude(voltage), 1 mVpp to 5 V for a 50 Ω load or 10 V for a high-impedance load.</param>
        /// <param name="dcOffsetVoltage">carrier waveform's voltage offset, ± 5 VDC into 50 Ω.</param>
        /// <param name="pulseWidthDevication">pulse width deviation; the ± variation in width (in seconds) 
        /// from the pulse width of the carrier pulse waveform, 0 to 500,000 (seconds).</param>
        /// <param name="dutyCycleDeviation">duty cycle deviation in percent of period, from 0 to 50.</param>
        /// <param name="modulationSource">the source of the modulating signal.</param>
        /// <param name="modulationWaveform">shape of the internal modulating waveform.</param>
        /// <param name="modulationFrequency">frequency at which output pulse width shifts through its pulse width deviation.</param>
        public abstract void PwmOn(Channel channel, double? carrierFrequency, VoltageUnit? voltageUnit, double carrierAmplitude, 
            double? dcOffsetVoltage, double? phaseAngle, double? pulseDutyCycle, double? pulseLeadingTime, double? pulseTrailingTime, 
            double? pulseWidthDevication, double? dutyCycleDeviation, SignalSource? modulationSource, 
            InternalFuncWaveform? modulationWaveform, double? modulationFrequency, double? outputImpedance, CancellationToken token);

        /// <summary>
        /// PWM off.
        /// </summary>
        public abstract void PwmOff(Channel channel, CancellationToken token);

        /// <summary>
        /// Generates waveform.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="frequency"></param>
        /// <param name="amplitude"></param>
        /// <param name="dcOffsetVoltage"></param>
        public abstract void WaveformOn(Channel channel, FunctionWaveform function, double? pulseDutyCycle, double? pulseLeadingTime, 
            double? pulseTrailingTime, double? frequency, VoltageUnit? voltageUnit, double amplitude, double? dcOffsetVoltage, 
            double? phaseAngle, double? outputImpedance, CancellationToken token);

        /// <summary>
        /// Output off.
        /// </summary>
        public abstract void OutputOff(Channel channel, CancellationToken token);

        /// <summary>
        /// 두 채널 동기화.
        /// </summary>
        public abstract void Synchronize(CancellationToken token);

        /// <summary>
        /// 스크린 이미지를 다운로드합니다.
        /// </summary>
        /// <param name="imageType"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract byte[] Download(EolWaveformGeneratorStep.ImageType? imageType, CancellationToken token);

        /// <summary>
        /// 지정한 이름을 가진 디바이스를 리턴한다.
        /// 해당 이름을 가진 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static WaveformGeneratorDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.WaveformGenerator, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is WaveformGeneratorDevice wgenDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return wgenDevice;
            }

            WaveformGeneratorDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.Keysight_EDU33210_Series:
                default:
                    device = new KeysightEdu33210SeriesDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected WaveformGeneratorDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        /// <summary>
        /// Location where the instrument states are saved and recalled.
        /// </summary>
        public enum WgStateLocation
        {
            Location0 = 0,
            Location1,
            Location2,
            Location3,
            Location4,
        }

        /// <summary>
        /// 출력 파형.
        /// </summary>
        public enum FunctionWaveform
        {
            Sinusoid,
            Square,
            Triangle,
            Ramp,
            Pulse,
            PRBS,
            Noise,
            ARB,
            DC,
        }

        /// <summary>
        /// 내부발생파형, PWM 변조파형 지정에 쓰임.
        /// </summary>
        public enum InternalFuncWaveform
        {
            /// <summary>
            /// SINusoid: a sine wave, no phase shift.
            /// </summary>
            Sinusoid,

            /// <summary>
            /// SQUare: a square wave, 50% duty cycle.
            /// </summary>
            Square,

            /// <summary>
            /// RAMP: ramp, 100% symmetry.
            /// </summary>
            Ramp,

            /// <summary>
            /// NRAMp: negative ramp, 0% symmetry.
            /// </summary>
            NRamp,

            /// <summary>
            /// TRIangle: ramp, 50% symmetry.
            /// </summary>
            Triangle,

            /// <summary>
            /// NOISe: gaussian noise; if NOISe is the internal function, it cannot also be the carrier.
            /// </summary>
            Noise,

            /// <summary>
            /// PRBS: pseudo-random binary sequence modulation; if PRBS is the internal function, it cannot also be the carrier.
            /// </summary>
            PRBS,

            /// <summary>
            /// ARBitrary: arbitrary waveform; default is exponential rise; if ARB is the internal function, it cannot also be the carrier.
            /// </summary>
            Arbitrary,
        }

        public enum SignalSource
        {
            /// <summary>
            /// Internal.
            /// </summary>
            Internal,

            /// <summary>
            /// Channel 1.
            /// </summary>
            CH1,

            /// <summary>
            /// Channel 2.
            /// </summary>
            CH2,
        }

        public enum Channel
        {
            CH1 = 1,
            CH2,
        }

        /// <summary>
        /// The units for output amplitude.
        /// </summary>
        public enum VoltageUnit
        {
            Vpp,
            Vrms,
            DBM,
        }
    }
}
