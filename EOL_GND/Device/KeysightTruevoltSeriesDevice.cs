using EOL_GND.Model;
using EOL_GND.Properties;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// Keysight Truvolt Series(34460A/34461A/34465A/34470A) DMM 관련 기능을 제공한다.
    /// </summary>
    public class KeysightTruevoltSeriesDevice : DmmDevice
    {
        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        // VISA 디바이스.
        private readonly VisaDevice visaDevice = new VisaDevice();

        public KeysightTruevoltSeriesDevice(string name) : base(DeviceType.Keysight_Truevolt_Series, name)
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

        // 디바이스로 명령을 보내고 그 응답을 받아서 리턴한다.
        private string SendCommand(string command, bool checkResponse, CancellationToken token)
        {
            visaDevice.WriteLine(command, token);
            if (checkResponse)
            {
                return visaDevice.ReadLine(token);
            }

            return null;
        }

        // 명령을 보내고 double 값을 읽는다.
        private double SendAndReadDouble(string command, CancellationToken token)
        {
            string response = SendCommand(command, true, token);
            return double.Parse(response);
        }

        // 명령을 보내고 double 값을 읽는다.
        private double[] SendAndReadLineListOfDouble(string command, CancellationToken token)
        {
            string response = SendCommand(command, true, token);
            string[] values = response.Split(',');
            var parsedValues = new double[values.Length];
            for (int i = 0; i < values.Length; i++)
            {
                parsedValues[i] = double.Parse(values[i]);
            }
            return parsedValues;
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            return SendCommand(command, read, token);
        }

        // DMM 연결이 자주 끊기는 것에 대비하기 위해 모든 명령 실행 전 ReadIDN 호출.
        public override string ReadIDN(CancellationToken token)
        {
            return SendCommand("*IDN?", true, token);
        }

        public override void Reset(CancellationToken token)
        {
            SendCommand("*RST", false, token);
        }

        public override void Configure(DmmFunction function, double? range, RtdType? rtdType, bool lowCurrent, double? resolution,
            bool highVoltage, double? nplCycles, double? aperture, double? bandWidth, bool? analogFilter, bool? digitalFilter,
            bool? autoZero, bool? autoImpedance, int commandDelay, CancellationToken token)
        {
            string func;
            var commands = new List<string>();
            switch (function)
            {
                case DmmFunction.Capacitance:
                    // 34465A and 34470A only.
                    func = "CAPACITANCE";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    // Fixed at 4½ digits.
                    //if (resolution != null)
                    //{
                    //    commands.Add($"{func}:RESOLUTION {resolution}");
                    //}
                    break;
                case DmmFunction.Current_AC:
                    func = "CURRENT:AC";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    // Fixed at 6½ digits.
                    //if (resolution != null)
                    //{
                    //    commands.Add($"{func}:RESOLUTION {resolution}");
                    //}

                    if (bandWidth != null)
                    {
                        commands.Add($"{func}:BANDWIDTH {bandWidth}");
                    }
                    break;
                case DmmFunction.Current_DC:
                    func = "CURRENT:DC";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    if (resolution != null)
                    {
                        commands.Add($"{func}:RESOLUTION {resolution}");
                    }

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    //if (analogFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:STATE {(analogFilter == true ? "ON" : "OFF")}");
                    //}

                    //if (digitalFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:DIGITAL:STATE {(digitalFilter == true ? "ON" : "OFF")}");
                    //}

                    if (autoZero != null)
                    {
                        commands.Add($"{func}:ZERO:AUTO {(autoZero == true ? "ON" : "OFF")}");
                    }
                    break;
                case DmmFunction.Voltage_AC:
                    func = "VOLTAGE:AC";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    //if (resolution != null)
                    //{
                    //    commands.Add($"{func}:RESOLUTION {resolution}");
                    //}

                    if (bandWidth != null)
                    {
                        commands.Add($"{func}:BANDWIDTH {bandWidth}");
                    }
                    break;
                case DmmFunction.Voltage_DC:
                    func = "VOLTAGE:DC";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    if (resolution != null)
                    {
                        commands.Add($"{func}:RESOLUTION {resolution}");
                    }

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    //if (analogFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:STATE {(analogFilter == true ? "ON" : "OFF")}");
                    //}

                    //if (digitalFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:DIGITAL:STATE {(digitalFilter == true ? "ON" : "OFF")}");
                    //}

                    if (autoImpedance != null)
                    {
                        commands.Add($"{func}:IMPEDANCE:AUTO {(autoImpedance == true ? "ON" : "OFF")}");
                    }
                    break;
                case DmmFunction.Voltage_DC_Ratio:
                    func = "VOLTAGE:DC:RATIO";
                    break;
                case DmmFunction.Resistance_4_Wire:
                    func = "FRESISTANCE";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    if (resolution != null)
                    {
                        commands.Add($"{func}:RESOLUTION {resolution}");
                    }

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    //if (analogFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:STATE {(analogFilter == true ? "ON" : "OFF")}");
                    //}

                    //if (digitalFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:DIGITAL:STATE {(digitalFilter == true ? "ON" : "OFF")}");
                    //}
                    break;
                case DmmFunction.Frequency:
                    func = "FREQUENCY";

                    if (range != null)
                    {
                        commands.Add($"{func}:VOLTAGE:RANGE {range}");
                    }

                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }
                    break;
                case DmmFunction.Period:
                    func = "PERIOD";

                    if (range != null)
                    {
                        commands.Add($"{func}:VOLTAGE:RANGE {range}");
                    }

                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }
                    break;
                case DmmFunction.Temperature_2_Wire:
                    func = "TEMPERATURE";
                    commands.Add($"{func}:TRANSDUCER:TYPE RTD");

                    if (rtdType != null)
                    {
                        commands.Add($"{func}:TYPE {rtdType}");
                    }

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    if (autoZero != null)
                    {
                        commands.Add($"{func}:ZERO:AUTO {(autoZero == true ? "ON" : "OFF")}");
                    }
                    break;
                case DmmFunction.Temperature_4_Wire:
                    func = "TEMPERATURE";
                    commands.Add($"{func}:TRANSDUCER:TYPE FRTD");

                    //if (rtdType != null)
                    //{
                    //    commands.Add($"{func}:TYPE {rtdType}");
                    //}

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    if (autoZero != null)
                    {
                        commands.Add($"{func}:ZERO:AUTO {(autoZero == true ? "ON" : "OFF")}");
                    }
                    break;
                case DmmFunction.Diode:
                    func = "DIODE";
                    //commands.Add($"CONFIGURE:{func} {(lowCurrent ? "ON" : "OFF")}, {(highVoltage ? "ON" : "OFF")}");
                    break;
                case DmmFunction.Continuity:
                    func = "CONTINUITY";
                    break;
                case DmmFunction.Resistance_2_Wire:
                default:
                    func = "RESISTANCE";

                    if (range != null)
                    {
                        commands.Add($"{func}:RANGE {range}");
                    }

                    if (resolution != null)
                    {
                        commands.Add($"{func}:RESOLUTION {resolution}");
                    }

                    // 34465A and 34470A only.
                    if (aperture != null)
                    {
                        commands.Add($"{func}:APERTURE {aperture}");
                    }

                    if (nplCycles != null)
                    {
                        commands.Add($"{func}:NPLC {nplCycles}");
                    }

                    //if (analogFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:STATE {(analogFilter == true ? "ON" : "OFF")}");
                    //}

                    //if (digitalFilter != null)
                    //{
                    //    commands.Add($"FILTER:DC:DIGITAL:STATE {(digitalFilter == true ? "ON" : "OFF")}");
                    //}

                    if (autoZero != null)
                    {
                        commands.Add($"{func}:ZERO:AUTO {(autoZero == true ? "ON" : "OFF")}");
                    }
                    break;
            }

            SendCommand("*CLS", false, token);
            if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            SendCommand("DISPLAY OFF", false, token);
            if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            //SendCommand("SYSTEM:REMOTE", false, token);
            //if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            SendCommand($"FUNCTION \"{func}\"", false, token);
            //SendCommand($"FUNCTION2 \"NON\"", false, token);
            //SendCommand("*OPC?", true, token);

            // 오류가 있는지 체크.
            var response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);

            foreach (var cmd in commands)
            {
                SendCommand(cmd, false, token);
                if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            }

            // 오류가 있는지 체크.
            response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);

            //if (autoZero != null)
            //{
            //    SendCommand($"ZERO:AUTO {(autoZero == true ? "ON" : "OFF")}", false, token);
            //}

            SendCommand("TRIGGER:SOURCE IMMEDIATE", false, token);
            if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            SendCommand("TRIGGER:DELAY 0", false, token);
            if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            SendCommand("TRIGGER:COUNT 1", false, token);
            if (commandDelay > 0) Task.Delay(commandDelay).Wait();
            //SendCommand("*OPC?", true, token);

            // 오류가 있는지 체크.
            response = SendCommand("SYSTEM:ERROR?", true, token);
            VisaDevice.ScpiSystemError(response);
        }

        public override double[] Read(DmmFunction function, int? sampleCount, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            SendCommand($"FUNCTION \"{GetFunctionName(function)}\"", false, token);
            if (sampleCount != null)
            {
                SendCommand($"SAMPLE:COUNT {sampleCount}", false, token);
            }

            // 오류가 있는지 체크.
            //var response = SendCommand("SYSTEM:ERROR?", true, token);
            //VisaDevice.ScpiSystemError(response);

            return SendAndReadLineListOfDouble("READ?", token);
        }

        public override byte[] Download(DownloadImageFormat? imageFormat, CancellationToken token)
        {
            SendCommand("*CLS", false, token);
            if (imageFormat != null)
            {
                SendCommand($"HCOPY:SDUMP:DATA:FORMAT {imageFormat}", false, token);
            }

            // 이미지 데이터 다운로드.
            visaDevice.WriteLine("HCOPY:SDUMP:DATA?", token);
            var imageData = visaDevice.ReadBinaryBlockOfByte(token);

            // 다운로드한 이미지 리턴.
            return imageData;
        }

        private static string GetFunctionName(DmmFunction function)
        {
            string func;
            switch (function)
            {
                case DmmFunction.Capacitance:
                    func = "CAPACITANCE";
                    break;
                case DmmFunction.Current_AC:
                    func = "CURRENT:AC";
                    break;
                case DmmFunction.Current_DC:
                    func = "CURRENT:DC";
                    break;
                case DmmFunction.Voltage_AC:
                    func = "VOLTAGE:AC";
                    break;
                case DmmFunction.Voltage_DC:
                    func = "VOLTAGE:DC";
                    break;
                case DmmFunction.Voltage_DC_Ratio:
                    func = "VOLTAGE:DC:RATIO";
                    break;
                case DmmFunction.Resistance_4_Wire:
                    func = "FRESISTANCE";
                    break;
                case DmmFunction.Frequency:
                    func = "FREQUENCY";
                    break;
                case DmmFunction.Period:
                    func = "PERIOD";
                    break;
                case DmmFunction.Temperature_2_Wire:
                    func = "TEMPERATURE";
                    break;
                case DmmFunction.Temperature_4_Wire:
                    func = "TEMPERATURE";
                    break;
                case DmmFunction.Diode:
                    func = "DIODE";
                    break;
                case DmmFunction.Continuity:
                    func = "CONTINUITY";
                    break;
                case DmmFunction.Resistance_2_Wire:
                default:
                    func = "RESISTANCE";
                    break;
            }
            return func;
        }

        private object GetPredefinedValue(object step, string paramName, string predefined, CancellationToken token)
        {
            object value = null;
            if (step is EolDmmStep dmmStep)
            {
                string func;
                switch (dmmStep.Function)
                {
                    case DmmFunction.Capacitance:
                        func = "CAPACITANCE";
                        break;
                    case DmmFunction.Current_AC:
                        func = "CURRENT:AC";
                        break;
                    case DmmFunction.Current_DC:
                        func = "CURRENT:DC";
                        break;
                    case DmmFunction.Voltage_AC:
                        func = "VOLTAGE:AC";
                        break;
                    case DmmFunction.Voltage_DC:
                        func = "VOLTAGE:DC";
                        break;
                    case DmmFunction.Voltage_DC_Ratio:
                        func = "VOLTAGE:DC:RATIO";
                        break;
                    case DmmFunction.Resistance_4_Wire:
                        func = "FRESISTANCE";
                        break;
                    case DmmFunction.Frequency:
                        func = "FREQUENCY";
                        break;
                    case DmmFunction.Period:
                        func = "PERIOD";
                        break;
                    case DmmFunction.Temperature_2_Wire:
                        func = "TEMPERATURE";
                        break;
                    case DmmFunction.Temperature_4_Wire:
                        func = "TEMPERATURE";
                        break;
                    case DmmFunction.Diode:
                        func = "DIODE";
                        break;
                    case DmmFunction.Continuity:
                        func = "CONTINUITY";
                        break;
                    case DmmFunction.Resistance_2_Wire:
                    default:
                        func = "RESISTANCE";
                        break;
                }

                switch (paramName)
                {
                    case nameof(EolDmmStep.Range):
                        value = SendAndReadDouble($"{func}:RANGE? {predefined}", token);
                        break;
                    case nameof(EolDmmStep.Resolution):
                        value = SendAndReadDouble($"{func}:RESOLUTION? {predefined}", token);
                        break;
                    case nameof(EolDmmStep.NPLCycles):
                        value = SendAndReadDouble($"{func}:NPLC? {predefined}", token);
                        break;
                    case nameof(EolDmmStep.Aperture):
                        value = SendAndReadDouble($"{func}:APERTURE? {predefined}", token);
                        break;
                    case nameof(EolDmmStep.BandWidth):
                        value = SendAndReadDouble($"{func}:BANDWIDTH? {predefined}", token);
                        break;
                    case nameof(EolDmmStep.SampleCount):
                        value = SendAndReadDouble($"SAMPLE:COUNT? {predefined}", token);
                        break;
                }
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
                    //serialDevice.Dispose();
                    visaDevice.Dispose();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
