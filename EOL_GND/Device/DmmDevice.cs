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
    /// DMM 디바이스.
    /// </summary>
    public abstract class DmmDevice : TestDevice
    {
        public enum DownloadImageFormat
        {
            PNG,
            BMP,
        }

        /// <summary>
        /// 식별문자열 읽기.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadIDN(CancellationToken token);

        /// <summary>
        /// Resets the Meter to its power-up configuration.
        /// </summary>
        public abstract void Reset(CancellationToken token);

        /// <summary>
        /// Configure function, range, and resolution.
        /// </summary>
        /// <param name="function"></param>
        /// <param name="range"></param>
        /// <param name="rtdType"></param>
        /// <param name="lowCurrent"></param>
        /// <param name="resolution"></param>
        /// <param name="highVoltage"></param>
        public abstract void Configure(DmmFunction function, double? range, RtdType? rtdType, bool lowCurrent, double? resolution, 
            bool highVoltage, double? nplCycles, double? aperture, double? bandWidth, bool? analogFilter, bool? digitalFilter, 
            bool? autoZero, bool? autoImpedance, int commandDelay, CancellationToken token);

        /// <summary>
        /// 측정을 진행합니다.
        /// </summary>
        /// <returns></returns>
        public abstract double[] Read(DmmFunction function, int? sampleCount, CancellationToken token);

        /// <summary>
        /// 스크린 이미지를 다운로드합니다.
        /// </summary>
        /// <param name="imageFormat"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract byte[] Download(DownloadImageFormat? imageFormat, CancellationToken token);

        /// <summary>
        /// 지정한 이름을 가진 DMM 디바이스를 리턴한다.
        /// 해당 이름을 가진 DMM 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static DmmDevice CreateInstance(string deviceName, bool reconnect)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.DMM, deviceName);

            var oldDevice = FindDevice(deviceSetting, reconnect);
            if (oldDevice is DmmDevice dmmDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return dmmDevice;
            }

            DmmDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.Keysight_Truevolt_Series:
                    device = new KeysightTruevoltSeriesDevice(deviceName);
                    break;
                case DeviceType.Fluke_8845A_8846A:
                default:
                    device = new Fluke884xADevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        /// <summary>
        /// DMM function의 측정범위 default값을 리턴한다.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static double GetDefaultRange(DmmFunction function)
        {
            switch (function)
            {
                case DmmFunction.Voltage_DC:
                    return 0.1;
                case DmmFunction.Voltage_AC:
                    return 0.1;
                case DmmFunction.Capacitance:
                    return 1E-9;
                case DmmFunction.Current_AC:
                    return 1E-2;
                case DmmFunction.Current_DC:
                    return 1E-4;
                case DmmFunction.Frequency:
                    return 1;
                case DmmFunction.Period:
                    return 1;
                case DmmFunction.Resistance_2_Wire:
                    return 100;
                case DmmFunction.Resistance_4_Wire:
                    return 100;
                default:
                    return 0;
            }
        }

        /// <summary>
        /// DMM function의 측정 정확도 default값을 얻는다.
        /// </summary>
        /// <param name="function"></param>
        /// <returns></returns>
        public static double GetDefaultResolution(DmmFunction function)
        {
            switch (function)
            {
                case DmmFunction.Capacitance:
                    return 1E-12;
                case DmmFunction.Current_AC:
                    return 1E-10;
                case DmmFunction.Current_DC:
                    return 1E-10;
                case DmmFunction.Resistance_2_Wire:
                    return 1E-5;
                case DmmFunction.Resistance_4_Wire:
                    return 1E-5;
                case DmmFunction.Voltage_AC:
                    return 1E-7;
                case DmmFunction.Voltage_DC:
                    return 1E-7;
                default:
                    return 0;
            }
        }

        protected DmmDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        public enum DmmFunction
        {
            Capacitance,
            Current_AC,
            Current_DC,
            Voltage_AC,
            Voltage_DC,
            Voltage_DC_Ratio,
            Resistance_2_Wire,
            Resistance_4_Wire,
            Frequency,
            Period,
            Temperature_2_Wire,
            Temperature_4_Wire,
            Diode,
            Continuity,
        }

        public enum RtdType
        {
            PT100_385,      // PT100 385 (R0 to 100Ω and Alpha to 0.00385055)
            PT100_392,      // PT100 392 (R0 to 100Ω and Alpha to 0.00391600)
        }

        public enum GateTime
        {
            _10ms,      // 4.5 digits
            _100ms,     // 5.5 digits
            _1s,        // 6.5 digits
        }

        public enum AcFilter
        {
            Slow = 3,
            Medium = 20,
            Fast = 200,
        }

        public enum TemperatureUnit
        {
            Cel,    // Celsius
            Far,    // Fahrenheit
            Kel,    // Kelvin
        }

        public enum AutoZeroMode
        {
            ON,
            OFF,
            ONCE,
        }
    }
}
