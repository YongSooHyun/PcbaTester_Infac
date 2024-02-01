using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// EOL power step.
    /// </summary>
    public class EolPowerStep : EolStep
    {
        [Category(MethodCategory), TypeConverter(typeof(StringArrayConverter)),
            Description("이 테스트 스텝에서 사용할 추가적인 장치 이름들을 설정합니다. 추가적인 장치들은 " + nameof(DeviceName) + 
            "에 설정된 장치와 같은 디바이스 타입이어야 합니다.")]
        public string[] AdditionalDeviceNames
        {
            get => GetAdditionalDeviceNames(_additionalDeviceNames);
            set
            {
                if (_additionalDeviceNames != value)
                {
                    _additionalDeviceNames = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string[] _additionalDeviceNames;

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)), 
            Description("테스트 방법을 설정합니다.")]
        public PowerTestMode TestMethod
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
        private PowerTestMode _testMethod = PowerTestMode.ReadIDN;

        [Category(MethodCategory), DisplayName(nameof(ExpectedValue)), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)), 
            Description("테스트 기대치를 설정합니다.")]
        public double ExpectedValue
        {
            get => _expectedValue;
            set
            {
                if (_expectedValue != value)
                {
                    _expectedValue = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _expectedValue;

        [Category(MethodCategory), Browsable(false), 
            Description("허용오차 방식을 설정합니다.")]
        public ToleranceMode Tolerance
        {
            get => _toleranceMode;
            set
            {
                if (_toleranceMode != value)
                {
                    _toleranceMode = value;
                    UpdateToleranceAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private ToleranceMode _toleranceMode = ToleranceMode.Relative;

        [Category(MethodCategory), DisplayName(DispNameTolPlus + " [%]"), Browsable(false), 
            Description("플러스 허용오차.")]
        public double TolerancePlusPercent
        {
            get => _tolerancePlusPercent;
            set
            {
                if (_tolerancePlusPercent != value)
                {
                    _tolerancePlusPercent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolMinus + " [%]"), Browsable(false), 
            Description("마이너스 허용오차.")]
        public double ToleranceMinusPercent
        {
            get => _toleranceMinusPercent;
            set
            {
                if (_toleranceMinusPercent != value)
                {
                    _toleranceMinusPercent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _toleranceMinusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolPlusMinus + " [%]"), Browsable(false), 
            Description("플러스/마이너스 허용오차.")]
        public double TolerancePlusMinusPercent
        {
            get => _tolerancePlusMinusPercent;
            set
            {
                if (_tolerancePlusMinusPercent != value)
                {
                    _tolerancePlusMinusPercent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusMinusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolPlus + " [V]"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)), 
            Description("플러스 허용오차.")]
        public double TolerancePlusAbsolute
        {
            get => _tolerancePlusAbsolute;
            set
            {
                if (_tolerancePlusAbsolute != value)
                {
                    _tolerancePlusAbsolute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusAbsolute;

        [Category(MethodCategory), DisplayName(DispNameTolMinus + " [V]"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)), 
            Description("마이너스 허용오차.")]
        public double ToleranceMinusAbsolute
        {
            get => _toleranceMinusAbsolute;
            set
            {
                if (_toleranceMinusAbsolute != value)
                {
                    _toleranceMinusAbsolute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _toleranceMinusAbsolute;

        [Category(MethodCategory), DisplayName(DispNameTolPlusMinus + " [V]"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)), 
            Description("플러스/마이너스 허용오차.")]
        public double TolerancePlusMinusAbsolute
        {
            get => _tolerancePlusMinusAbsolute;
            set
            {
                if (_tolerancePlusMinusAbsolute != value)
                {
                    _tolerancePlusMinusAbsolute = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusMinusAbsolute;

        [Category(MethodCategory), DisplayName(nameof(MeasureOffset)), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("측정값에서 뺄 옵셋을 설정합니다.")]
        public double MeasureOffset
        {
            get => _measureOffset;
            set
            {
                if (_measureOffset != value)
                {
                    _measureOffset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _measureOffset = 0;

        [Category(MethodCategory), Browsable(false), DefaultValue(1),
            Description("1부터 시작하는 파워 채널을 설정합니다. 채널을 지원하지 않는 디바이스에 대해서는 무시됩니다.")]
        public int Channel
        {
            get => _channel;
            set
            {
                if (_channel != value)
                {
                    _channel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _channel = 1;

        [Category(MethodCategory), DisplayName("OutputVoltage [V]"), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("출력 전압을 설정합니다.")]
        public double OutputVoltage
        {
            get => _outputVoltage;
            set
            {
                if (_outputVoltage != value)
                {
                    _outputVoltage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _outputVoltage;

        [Category(MethodCategory), DisplayName("OutputCurrent [A]"), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("출력 전류를 설정합니다. 출력 전류를 설정하지 않으려면 이 값을 비워두세요.")]
        public double? OutputCurrent
        {
            get => _outputCurrent;
            set
            {
                if (_outputCurrent != value)
                {
                    _outputCurrent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _outputCurrent;

        [Category(MethodCategory), DisplayName("QueryDelay [ms]"), Browsable(false), 
            Description("출력 전압/전류를 설정한 다음 설정된 값을 불러오기 전 Delay 타임을 ms 단위로 설정합니다.")]
        public int QueryDelay
        {
            get => _queryDelay;
            set
            {
                if (_queryDelay != value)
                {
                    _queryDelay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _queryDelay;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행합니다.")]
        public string CommandLine
        {
            get => _commandLine;
            set
            {
                if (_commandLine != value)
                {
                    _commandLine = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _commandLine;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 분석해 Pass/Fail로 판단하는 방법을 설정합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.None) + ": 응답을 읽지 않습니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLine) + ": 응답을 읽지만, 그 Pass/Fail 여부는 판단하지 않습니다. 응답이 없으면 Fail로 간주합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineStartsWith) + ": 응답을 읽어 그 첫 시작이 ResponsePattern으로 시작하는지 검사합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineContains) + ": 응답을 읽어 ResponsePattern을 포함하는지 검사합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineEndsWith) + ": 응답을 읽어 그 마지막이 ResponsePattern으로 끝나는지 검사합니다.")]
        public CmdResponseCheckMode ResponseCheckMethod
        {
            get => _responseCheckMethod;
            set
            {
                if (_responseCheckMethod != value)
                {
                    _responseCheckMethod = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private CmdResponseCheckMode _responseCheckMethod = CmdResponseCheckMode.None;

        [Category(MethodCategory), Browsable(false), DefaultValue(500),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 읽기 위한 Timeout을 ms단위로 지정합니다.")]
        public int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                if (_readTimeout != value)
                {
                    _readTimeout = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _readTimeout = 500;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 체크하기 위한 Pattern문자열을 지정합니다.")]
        public string ResponsePattern
        {
            get => _responsePattern;
            set
            {
                if (_responsePattern != value)
                {
                    _responsePattern = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _responsePattern;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 대소문자를 구분하여 체크할 것인지 설정합니다.")]
        public bool ResponsePatternCaseSensitive
        {
            get => _responsePatternCaseSensitive;
            set
            {
                if (_responsePatternCaseSensitive != value)
                {
                    _responsePatternCaseSensitive = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _responsePatternCaseSensitive = false;

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class CorrectionSettings : INotifyPropertyChanged
        {
            public event PropertyChangedEventHandler PropertyChanged;

            [DefaultValue(false)]
            public bool Enabled
            {
                get => _enabled;
                set
                {
                    if (_enabled != value)
                    {
                        _enabled = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private bool _enabled;

            [TypeConverter(typeof(PhysicalValueStaticConverter))]
            public double? UpperLimit
            {
                get => _upperLimit;
                set
                {
                    if (_upperLimit != value)
                    {
                        _upperLimit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private double? _upperLimit;

            [TypeConverter(typeof(PhysicalValueStaticConverter))]
            public double? LowerLimit
            {
                get => _lowerLimit;
                set
                {
                    if (_lowerLimit != value)
                    {
                        _lowerLimit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private double? _lowerLimit;

            [TypeConverter(typeof(PhysicalValueStaticConverter))]
            public double ToValue
            {
                get => _toValue;
                set
                {
                    if (_toValue != value)
                    {
                        _toValue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private double _toValue;

            public override string ToString()
            {
                return "";
            }

            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        [Category(MethodCategory), Browsable(false),
            Description("")]
        public CorrectionSettings Correction
        {
            get => _correction;
            set
            {
                if (_correction != value)
                {
                    if (_correction != null)
                    {
                        _correction.PropertyChanged -= _correction_PropertyChanged;
                    }
                    _correction = value;
                    if (_correction != null)
                    {
                        _correction.PropertyChanged += _correction_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private CorrectionSettings _correction = new CorrectionSettings();

        private void _correction_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Correction));
        }


        internal override string CategoryName => StepCategory.Power.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case PowerTestMode.Set:
                        string voltage = GetPrefixExpression(OutputVoltage, PhysicalUnit.Volt, out MetricPrefix prefix);
                        string desc = voltage + prefix.GetText() + PhysicalUnit.Volt.GetText();
                        if (OutputCurrent != null)
                        {
                            string current = GetPrefixExpression(OutputCurrent, PhysicalUnit.Ampere, out prefix);
                            desc += ", " + current + prefix.GetText() + PhysicalUnit.Ampere.GetText();
                        }
                        return desc;
                    case PowerTestMode.ON:
                    case PowerTestMode.OFF:
                    case PowerTestMode.MeasureVoltage:
                    case PowerTestMode.MeasureCurrent:
                    case PowerTestMode.ReadPowerState:
                    case PowerTestMode.ReadVoltageCurrent:
                    case PowerTestMode.ReadIDN:
                    case PowerTestMode.ReadSN:
                    case PowerTestMode.Reset:
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case PowerTestMode.MeasureVoltage:
                        return $"{GetPrefixExpression(ExpectedValue, PhysicalUnit.Volt, out MetricPrefix prefix)}{prefix.GetText()}{PhysicalUnit.Volt.GetText()}";
                    case PowerTestMode.MeasureCurrent:
                        return $"{GetPrefixExpression(ExpectedValue, PhysicalUnit.Ampere, out prefix)}{prefix.GetText()}{PhysicalUnit.Ampere.GetText()}";
                    case PowerTestMode.Set:
                    case PowerTestMode.ON:
                    case PowerTestMode.OFF:
                    case PowerTestMode.ReadPowerState:
                    case PowerTestMode.ReadVoltageCurrent:
                    case PowerTestMode.ReadIDN:
                    case PowerTestMode.ReadSN:
                    case PowerTestMode.Reset:
                    default:
                        return "";
                }
            }
        }

        public override string TolerancePlusDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case PowerTestMode.MeasureVoltage:
                        return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                            TolerancePlusAbsolute, TolerancePlusMinusAbsolute, PhysicalUnit.Volt);
                    case PowerTestMode.MeasureCurrent:
                        return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                            TolerancePlusAbsolute, TolerancePlusMinusAbsolute, PhysicalUnit.Ampere);
                    case PowerTestMode.Set:
                    case PowerTestMode.ON:
                    case PowerTestMode.OFF:
                    case PowerTestMode.ReadPowerState:
                    case PowerTestMode.ReadVoltageCurrent:
                    case PowerTestMode.ReadIDN:
                    case PowerTestMode.ReadSN:
                    case PowerTestMode.Reset:
                    default:
                        return "";
                }
            }
        }

        public override string ToleranceMinusDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case PowerTestMode.MeasureVoltage:
                        return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent, 
                            ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, PhysicalUnit.Volt);
                    case PowerTestMode.MeasureCurrent:
                        return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent,
                            ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, PhysicalUnit.Ampere);
                    case PowerTestMode.Set:
                    case PowerTestMode.ON:
                    case PowerTestMode.OFF:
                    case PowerTestMode.ReadPowerState:
                    case PowerTestMode.ReadVoltageCurrent:
                    case PowerTestMode.ReadIDN:
                    case PowerTestMode.ReadSN:
                    case PowerTestMode.Reset:
                    default:
                        return "";
                }
            }
        }

        public override List<int> AllTestChannels => null;

        internal static string[] GetDeviceNamesOfSameType(string deviceName)
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.Power, deviceName);
                return settingsManager.GetPowerSettings().Where(s => s.DeviceType == deviceSetting.DeviceType).Select(s => s.DeviceName).ToArray();
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// XML Serialize를 위해서는 파라미터 없는 생성자 필요.
        /// </summary>
        private EolPowerStep()
        {
            Name = StepCategory.Power.GetText();

            _correction.PropertyChanged += _correction_PropertyChanged;
        }

        public EolPowerStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        internal string[] GetAdditionalDeviceNames(IEnumerable<string> names)
        {
            if (names == null)
            {
                return null;
            }

            var deviceNames = GetDeviceNamesOfSameType(DeviceName);
            if (deviceNames == null || deviceNames.Length == 0)
            {
                return null;
            }

            var addDeviceNames = new List<string>();
            foreach (string name in names)
            {
                var found = deviceNames.Where(deviceName => string.Equals(deviceName, name, StringComparison.OrdinalIgnoreCase));
                if (found.Any())
                {
                    addDeviceNames.Add(found.First());
                }
            }
            return addDeviceNames.ToArray();
        }

        public override TestDevice CreateDevice()
        {
            return PowerDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetPowerSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.Power, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.ODA_EX_Series:
                        return new object[]
                        {
                            PowerTestMode.ReadIDN,
                            PowerTestMode.ReadSN,
                            PowerTestMode.Reset,
                            PowerTestMode.ON,
                            PowerTestMode.OFF,
                            PowerTestMode.Set,
                            PowerTestMode.MeasureVoltage,
                            PowerTestMode.MeasureCurrent,
                            PowerTestMode.ReadPowerState,
                            PowerTestMode.ReadVoltageCurrent,
                            PowerTestMode.RunCommandLine,
                        };
                    case DeviceType.MK_P_Series:
                        return new object[]
                        {
                            PowerTestMode.ReadIDN,
                            PowerTestMode.ON,
                            PowerTestMode.OFF,
                            PowerTestMode.Set,
                            PowerTestMode.MeasureVoltage,
                            PowerTestMode.MeasureCurrent,
                            PowerTestMode.ReadVoltageCurrent,
                            PowerTestMode.RunCommandLine,
                        };
                }
            }
            catch
            {
            }

            return null;
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out PowerTestMode parsed))
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
            if (TestMethod == PowerTestMode.MeasureVoltage || TestMethod == PowerTestMode.MeasureCurrent)
            {
                nominalValue = ExpectedValue;
                GetLimitValues(ExpectedValue, Tolerance, ToleranceMinusPercent, TolerancePlusPercent, TolerancePlusMinusPercent,
                    ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, out upperLimit, out lowerLimit);
            }
            else
            {
                nominalValue = null;
                upperLimit = null;
                lowerLimit = null;
            }
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

            var mainDevice = device as PowerDevice;
            if (mainDevice == null)
            {
                return result;
            }

            // 메인 장치와 추가 장치들을 포함한 전체 디바이스 리스트.
            var deviceList = new List<PowerDevice>() { mainDevice };

            // 추가적인 장치들.
            var additionalDevNames = AdditionalDeviceNames;
            if (additionalDevNames != null)
            {
                foreach (var devName in additionalDevNames)
                {
                    var powerDevice = PowerDevice.CreateInstance(devName);
                    powerDevice.Connect(token);
                    deviceList.Add(powerDevice);
                }
            }

            // 모든 장치들에 대해 기능 실행.
            foreach (var powerDevice in deviceList)
            {
                double measuredValue;
                switch (TestMethod)
                {
                    case PowerTestMode.ON:
                        powerDevice.PowerOn(token);
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.OFF:
                        powerDevice.PowerOff(token);
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.Set:
                        double? outCurrent = OutputCurrent == 0 ? null : OutputCurrent;
                        powerDevice.SetPower(Channel, OutputVoltage, outCurrent, QueryDelay, token);
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.ReadPowerState:
                        bool powered = powerDevice.GetPowerState(token);
                        result.ResultInfo = powered ? "ON" : "OFF";
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.ReadVoltageCurrent:
                        powerDevice.ReadPower(Channel, out double voltage, out double current, token);
                        string voltageExpr = GetPrefixExpression(voltage, PhysicalUnit.Volt, out MetricPrefix voltagePrefix);
                        string currentExpr = GetPrefixExpression(current, PhysicalUnit.Ampere, out MetricPrefix currentPrefix);
                        result.ResultInfo = $"{voltageExpr}{voltagePrefix.GetText()}{PhysicalUnit.Volt.GetText()}, " +
                            $"{currentExpr}{currentPrefix.GetText()}{PhysicalUnit.Ampere.GetText()}";
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.ReadIDN:
                        var idn = powerDevice.ReadIDN(token);
                        result.ResultInfo = idn;
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.ReadSN:
                        var sn = powerDevice.ReadSN(token);
                        result.ResultInfo = sn;
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.Reset:
                        powerDevice.Reset(token);
                        result.ResultState = ResultState.Pass;
                        break;
                    case PowerTestMode.MeasureVoltage:
                        measuredValue = powerDevice.MeasureVoltage(Channel, token) - MeasureOffset;
                        result.ResultValue = measuredValue;
                        result.Unit = GetPhysicalUnit();
                        result.ResultValueState = CalcValueState(ExpectedValue, measuredValue, Tolerance, ToleranceMinusPercent,
                            TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, 
                            out double? cpAdjusted);

                        if (cpAdjusted != null)
                        {
                            result.ResultValue = cpAdjusted;
                        }

                        switch (result.ResultValueState)
                        {
                            case ResultValueState.Good:
                                result.ResultState = ResultState.Pass;
                                break;
                            case ResultValueState.Invalid:
                            case ResultValueState.Bad:
                            case ResultValueState.Low:
                            case ResultValueState.High:
                            default:
                                result.ResultState = ResultState.Fail;
                                break;
                        }
                        break;
                    case PowerTestMode.MeasureCurrent:
                        measuredValue = powerDevice.MeasureCurrent(Channel, token) - MeasureOffset;

                        // 측정값이 Correction 범위 내에 있으면 Correction에 정한 값으로 설정.
                        if (Correction.Enabled)
                        {
                            bool upperLimitOk = Correction.UpperLimit == null || measuredValue <= Correction.UpperLimit;
                            bool lowerLimitOk = Correction.LowerLimit == null || measuredValue >= Correction.LowerLimit;
                            if (upperLimitOk && lowerLimitOk)
                            {
                                measuredValue = Correction.ToValue;
                            }
                        }

                        result.ResultValue = measuredValue;
                        result.Unit = GetPhysicalUnit();
                        result.ResultValueState = CalcValueState(ExpectedValue, measuredValue, Tolerance, ToleranceMinusPercent,
                            TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute,
                            out cpAdjusted);

                        if (cpAdjusted != null)
                        {
                            result.ResultValue = cpAdjusted;
                        }

                        switch (result.ResultValueState)
                        {
                            case ResultValueState.Good:
                                result.ResultState = ResultState.Pass;
                                break;
                            case ResultValueState.Invalid:
                            case ResultValueState.Bad:
                            case ResultValueState.Low:
                            case ResultValueState.High:
                            default:
                                result.ResultState = ResultState.Fail;
                                break;
                        }
                        break;
                    case PowerTestMode.RunCommandLine:
                        var response = powerDevice.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, ReadTimeout, token);
                        result.ResultInfo = response;
                        var textComparison = ResponsePatternCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                        switch (ResponseCheckMethod)
                        {
                            case CmdResponseCheckMode.ReadLineStartsWith:
                                bool passed;
                                if (string.IsNullOrEmpty(ResponsePattern))
                                {
                                    passed = true;
                                }
                                else if (string.IsNullOrEmpty(response))
                                {
                                    passed = false;
                                }
                                else
                                {
                                    passed = response.StartsWith(ResponsePattern, textComparison);
                                }
                                result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                                break;
                            case CmdResponseCheckMode.ReadLineContains:
                                if (string.IsNullOrEmpty(ResponsePattern))
                                {
                                    passed = true;
                                }
                                else if (string.IsNullOrEmpty(response))
                                {
                                    passed = false;
                                }
                                else
                                {
                                    passed = response.IndexOf(ResponsePattern, textComparison) >= 0;
                                }
                                result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                                break;
                            case CmdResponseCheckMode.ReadLineEndsWith:
                                if (string.IsNullOrEmpty(ResponsePattern))
                                {
                                    passed = true;
                                }
                                else if (string.IsNullOrEmpty(response))
                                {
                                    passed = false;
                                }
                                else
                                {
                                    passed = response.EndsWith(ResponsePattern, textComparison);
                                }
                                result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                                break;
                            default:
                                result.ResultState = ResultState.Pass;
                                break;
                        }
                        break;
                    default:
                        throw new NotSupportedException($"디바이스 {powerDevice.Setting.DeviceName} 에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
                }

                if (result.ResultState != ResultState.Pass)
                {
                    break;
                }
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            var unit = PhysicalUnit.None;
            switch (TestMethod)
            {
                case PowerTestMode.MeasureVoltage:
                    unit = PhysicalUnit.Volt;
                    break;
                case PowerTestMode.MeasureCurrent:
                    unit = PhysicalUnit.Ampere;
                    break;
            }
            return unit;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();

            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            bool visible = browsable ?? false;
            Utils.SetBrowsableAttribute(this, nameof(Correction), visible);
            Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), visible);
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            bool showMeasureOptions = TestMethod == PowerTestMode.MeasureVoltage || TestMethod == PowerTestMode.MeasureCurrent;
            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), showMeasureOptions);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), showMeasureOptions);
            UpdateToleranceAttributes();
            //Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), showMeasureOptions);

            // 채널 표시 여부 판단.
            bool channelAcceptable;
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.Power, DeviceName);
                channelAcceptable = deviceSetting.DeviceType == DeviceType.MK_P_Series;
            }
            catch
            {
                channelAcceptable = false;
            }
            Utils.SetBrowsableAttribute(this, nameof(Channel), channelAcceptable && (TestMethod == PowerTestMode.MeasureCurrent 
                || TestMethod == PowerTestMode.MeasureVoltage || TestMethod == PowerTestMode.Set || TestMethod == PowerTestMode.ReadVoltageCurrent));

            Utils.SetBrowsableAttribute(this, nameof(OutputVoltage), TestMethod == PowerTestMode.Set);
            Utils.SetBrowsableAttribute(this, nameof(OutputCurrent), TestMethod == PowerTestMode.Set);
            Utils.SetBrowsableAttribute(this, nameof(QueryDelay), TestMethod == PowerTestMode.Set);

            bool runCmdLine = TestMethod == PowerTestMode.RunCommandLine;
            Utils.SetBrowsableAttribute(this, nameof(CommandLine), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ReadTimeout), runCmdLine && ResponseCheckMethod != CmdResponseCheckMode.None);
            bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
            Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), runCmdLine && compareResponse);
            Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), runCmdLine && compareResponse);
        }

        protected override void UpdateToleranceAttributes()
        {
            bool showMeasureOptions = TestMethod == PowerTestMode.MeasureVoltage || TestMethod == PowerTestMode.MeasureCurrent;
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.Absolute);

            // Update display names.
            string unitText;
            if (TestMethod == PowerTestMode.MeasureCurrent)
            {
                unitText = " [" + PhysicalUnit.Ampere.GetText() + "]";
            }
            else
            {
                unitText = " [" + PhysicalUnit.Volt.GetText() + "]";
            }
            string tolerancePlusDisplayName = DispNameTolPrefix + PlusSign + unitText;
            string toleranceMinusDisplayName = DispNameTolPrefix + MinusSign + unitText;
            string tolerancePlusMinusDisplayName = DispNameTolPrefix + PlusMinusSign + unitText;
            Utils.SetDisplayNameAttribute(this, nameof(TolerancePlusAbsolute), tolerancePlusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(ToleranceMinusAbsolute), toleranceMinusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(TolerancePlusMinusAbsolute), tolerancePlusMinusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(MeasureOffset), nameof(MeasureOffset) + unitText);
            Utils.SetDisplayNameAttribute(this, nameof(ExpectedValue), nameof(ExpectedValue) + unitText);
        }

        public override object Clone()
        {
            var newStep = new EolPowerStep(DeviceName);

            // Power 고유 프로퍼티.
            CopyTo(newStep);

            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolPowerStep powerStep)
            {
                if (_additionalDeviceNames != null)
                {
                    powerStep.AdditionalDeviceNames = new string[_additionalDeviceNames.Length];
                    _additionalDeviceNames.CopyTo(powerStep._additionalDeviceNames, 0);
                }
                powerStep.TestMethod = TestMethod;
                powerStep.ExpectedValue = ExpectedValue;
                powerStep.Tolerance = Tolerance;
                powerStep.TolerancePlusPercent = TolerancePlusPercent;
                powerStep.ToleranceMinusPercent = ToleranceMinusPercent;
                powerStep.TolerancePlusMinusPercent = TolerancePlusMinusPercent;
                powerStep.TolerancePlusAbsolute = TolerancePlusAbsolute;
                powerStep.ToleranceMinusAbsolute = ToleranceMinusAbsolute;
                powerStep.TolerancePlusMinusAbsolute = TolerancePlusMinusAbsolute;
                powerStep.MeasureOffset = MeasureOffset;
                powerStep.Channel = Channel;
                powerStep.OutputVoltage = OutputVoltage;
                powerStep.OutputCurrent = OutputCurrent;
                powerStep.QueryDelay = QueryDelay;

                powerStep.CommandLine = CommandLine;
                powerStep.ResponseCheckMethod = ResponseCheckMethod;
                powerStep.ReadTimeout = ReadTimeout;
                powerStep.ResponsePattern = ResponsePattern;
                powerStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;

                if (Correction != null)
                {
                    powerStep.Correction = new CorrectionSettings
                    {
                        Enabled = Correction.Enabled,
                        UpperLimit = Correction.UpperLimit,
                        LowerLimit = Correction.LowerLimit,
                        ToValue = Correction.ToValue,
                    };
                }
            }

            dest.UpdateBrowsableAttributes();
        }

        /// <summary>
        /// 파워 기능 리스트.
        /// </summary>
        public enum PowerTestMode
        {
            ON,
            OFF,
            Set,
            MeasureVoltage,
            MeasureCurrent,
            ReadPowerState,
            ReadVoltageCurrent,
            ReadIDN,
            ReadSN,
            Reset,
            RunCommandLine,
        }
    }
}
