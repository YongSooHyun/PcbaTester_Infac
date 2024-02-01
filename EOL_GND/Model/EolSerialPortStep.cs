using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EOL_GND.Model.EolDioStep;

namespace EOL_GND.Model
{
    public class EolSerialPortStep : EolStep
    {
        /// <summary>
        /// MAC Address 정보를 저장하는 파일의 폴더 경로.
        /// </summary>
        public const string MacAddrFileFolder = "D:\\ElozPlugin\\EOL";

        /// <summary>
        /// MAC Address 정보를 저장하는 파일의 이름.
        /// </summary>
        public const string MacAddrFileName = "mac_address_log.csv";

        /// <summary>
        /// 최종 MAC Address 정보를 저장하는 파일의 이름.
        /// </summary>
        public const string MacAddrReportFileName = "mac_address_report.csv";

        /// <summary>
        /// 쓴 MAC Address 를 나타내는 헤더.
        /// </summary>
        public const string MacAddrHeader = "ORIG_MAC = ";

        /// <summary>
        /// SerialPort 테스트 방법 리스트.
        /// </summary>
        public enum SerialPortTestMode
        {
            LogStart,
            LogStop,
        }

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public SerialPortTestMode TestMethod
        {
            get => _testMethod;
            set
            {
                if (_testMethod != value)
                {
                    _testMethod = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private SerialPortTestMode _testMethod = SerialPortTestMode.LogStart;

        internal override string CategoryName => StepCategory.SerialPort.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc => null;

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels => null;

        private EolSerialPortStep()
        {
            Name = StepCategory.SerialPort.GetText();
        }

        public EolSerialPortStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return SerialPortDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetSerialPortSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            return new object[]
            {
                SerialPortTestMode.LogStart,
                SerialPortTestMode.LogStop,
            };
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out SerialPortTestMode parsed))
                {
                    testMode = parsed;
                    return true;
                }
            }

            testMode = null;
            return false;
        }

        protected override void RelayOn(object elozTestSet, DeviceSetting setting)
        {
            // Do nothing.
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            nominalValue = null;
            upperLimit = null;
            lowerLimit = null;
        }

        protected override TestResult RunTest(object device, CancellationToken token)
        {
            var result = new TestResult(this)
            {
                ResultState = ResultState.NoState,
                ResultInfo = null,
                ResultValue = null,
                ResultValueState = ResultValueState.Invalid,
                Unit = GetPhysicalUnit(),
            };

            var serialDevice = device as SerialPortDevice;
            if (serialDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case SerialPortTestMode.LogStart:
                    serialDevice.LogStart();
                    result.ResultState = ResultState.Pass;
                    break;
                case SerialPortTestMode.LogStop:
                    serialDevice.LogStop();
                    var portName = (serialDevice.Setting as SerialDeviceSetting).Port;

                    // MAC Address 찾아서 넣기.
                    var logText = serialDevice.LogBuilder.ToString();
                    var macPattern = "PEV_MAC = ";
                    var macAddrIndex = logText.IndexOf(macPattern, StringComparison.OrdinalIgnoreCase);
                    if (macAddrIndex >= 0)
                    {
                        string macAddrText;
                        var newLineIndex = logText.IndexOf("\n", macAddrIndex);
                        if (newLineIndex >= 0)
                        {
                            macAddrText = logText.Substring(macAddrIndex, newLineIndex - macAddrIndex);
                        }
                        else
                        {
                            macAddrText = logText.Substring(macAddrIndex);
                        }
                        result.ResultLogInfo = macAddrText.Trim();

                        // MAC Address 계산.
                        var macAddrValText = macAddrText.Substring(macPattern.Length).Trim();
                        var words = macAddrValText.Split(':');
                        string origAddr = "";
                        if (words.Length >= 4 && byte.TryParse(words[3], System.Globalization.NumberStyles.HexNumber, null, out byte parsed))
                        {
                            parsed = (byte)(parsed + 2);
                            words[3] = $"{parsed:X2}";
                            origAddr = string.Join(":", words);
                            result.ResultLogInfo += "; " + MacAddrHeader + origAddr;
                        }

                        // MAC Address 파일에 내용 저장.
                        try
                        {
                            Directory.CreateDirectory(MacAddrFileFolder);
                            string logFilePath = Path.Combine(MacAddrFileFolder, MacAddrFileName);
                            File.AppendAllText(logFilePath, $"{DateTime.Now:yyyy-MM-dd HH:mm:ss},{origAddr}," + Environment.NewLine);
                        }
                        catch (Exception ex)
                        {
                            string message = $"MAC Address save error: {ex.Message}";
                            Logger.LogError(message);
                            result.ResultInfo = message;
                            result.ResultState = ResultState.Fail;
                        }
                    }

                    result.ResultLogBody = $"{portName} Text Log:" + Environment.NewLine + logText;
                    Logger.LogInfo(result.ResultLogBody);
                    if (result.ResultState == ResultState.NoState)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                    break;
                default:
                    throw new NotSupportedException($"디바이스 {DeviceName} 에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolSerialPortStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolSerialPortStep serialStep)
            {
                serialStep.TestMethod = TestMethod;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
