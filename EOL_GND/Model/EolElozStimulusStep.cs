using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EOL_GND.Model.EolPowerStep;

namespace EOL_GND.Model
{
    /// <summary>
    /// eloZ1 Stimulus 기능을 제공한다.
    /// </summary>
    public class EolElozStimulusStep : EolStep
    {
        public enum StimulusTestMode
        {
            Stimulate,
            Unstimulate,
            UnstimulateAll,
            GetMaxStimulationVoltage,
            GetMaxStimulationCurrent,
            GetConnectionInfo,
            GetSettingsInfo,
            GetStimulationVoltage,
            GetStimulationCurrent,
            MeasureVoltage,
            MeasureCurrent,
            GetMeasurementVoltageRange,
            GetMeasurementCurrentRange,
            SetMeasurementVoltageRange,
            SetMeasurementCurrentRange,
        }

        [Category(MethodCategory),
            Description("테스트 방법을 설정합니다.")]
        public StimulusTestMode TestMethod
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
        private StimulusTestMode _testMethod = StimulusTestMode.Stimulate;

        [Category(MethodCategory), DefaultValue(1), Browsable(true), 
            Description("1부터 시작하는 디바이스 번호를 설정합니다.")]
        public int DeviceNumber
        {
            get => _deviceNumber;
            set
            {
                if (_deviceNumber != value)
                {
                    _deviceNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _deviceNumber = 1;

        [Category(MethodCategory), DefaultValue(true), Browsable(true), 
            Description("True이면 Stimulus10V 유닛을, False이면 Stimulus60V 유닛을 사용합니다.")]
        public bool IsStimulus10V
        {
            get => _isStimulus10V;
            set
            {
                if (_isStimulus10V != value)
                {
                    _isStimulus10V = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _isStimulus10V = true;

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

        [Category(MethodCategory), DisplayName(nameof(Voltage) + " [V]"), Browsable(true), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("인가할 전압을 설정합니다.")]
        public double Voltage
        {
            get => _voltage;
            set
            {
                if (_voltage != value)
                {
                    _voltage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _voltage;

        [Category(MethodCategory), DisplayName(nameof(Current) + " [A]"), Browsable(true), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("인가할 전류를 설정합니다.")]
        public double Current
        {
            get => _current;
            set
            {
                if (_current != value)
                {
                    _current = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _current;

        [Category(MethodCategory), DisplayName(nameof(DischargeDelay) + " [s]"), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("전원을 해제할 때 지정한 시간동안 Discharge를 진행합니다.")]
        public double DischargeDelay
        {
            get => _dischargeDelay;
            set
            {
                if (_dischargeDelay != value)
                {
                    _dischargeDelay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _dischargeDelay;

        [Category(MethodCategory), DisplayName(nameof(MeasurementVoltageRange) + " [V]"), Browsable(false), 
            TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("측정하려는 최대 전압을 설정합니다.")]
        public double MeasurementVoltageRange
        {
            get => _measurementVoltageRange;
            set
            {
                if (_measurementVoltageRange != value)
                {
                    _measurementVoltageRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _measurementVoltageRange = 10;

        [Category(MethodCategory), DisplayName(nameof(MeasurementCurrentRange) + " [A]"), Browsable(false),
            TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("측정하려는 최대 전류를 설정합니다.")]
        public double MeasurementCurrentRange
        {
            get => _measurementCurrentRange;
            set
            {
                if (_measurementCurrentRange != value)
                {
                    _measurementCurrentRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _measurementCurrentRange = 1E-5;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("연결할 High Force 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelsHighForce
        {
            get => _testChannelsHighForce;
            set
            {
                if (_testChannelsHighForce != value)
                {
                    if (_testChannelsHighForce != null)
                    {
                        _testChannelsHighForce.ListChanged -= _testChannelsHighForce_ListChanged;
                    }
                    _testChannelsHighForce = value;
                    if (_testChannelsHighForce != null)
                    {
                        _testChannelsHighForce = new BindingList<int>(_testChannelsHighForce.Distinct().ToList());
                        _testChannelsHighForce.ListChanged += _testChannelsHighForce_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelsHighForce;

        private void _testChannelsHighForce_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelsHighForce));
        }

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("연결할 Low Force 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelsLowForce
        {
            get => _testChannelsLowForce;
            set
            {
                if (_testChannelsLowForce != value)
                {
                    if (_testChannelsLowForce != null)
                    {
                        _testChannelsLowForce.ListChanged -= _testChannelsLowForce_ListChanged;
                    }
                    _testChannelsLowForce = value;
                    if (_testChannelsLowForce != null)
                    {
                        _testChannelsLowForce = new BindingList<int>(_testChannelsLowForce.Distinct().ToList());
                        _testChannelsLowForce.ListChanged += _testChannelsLowForce_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelsLowForce;

        private void _testChannelsLowForce_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelsLowForce));
        }

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("연결할 High Sense 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelsHighSense
        {
            get => _testChannelsHighSense;
            set
            {
                if (_testChannelsHighSense != value)
                {
                    if (_testChannelsHighSense != null)
                    {
                        _testChannelsHighSense.ListChanged -= _testChannelsHighSense_ListChanged;
                    }
                    _testChannelsHighSense = value;
                    if (_testChannelsHighSense != null)
                    {
                        _testChannelsHighSense = new BindingList<int>(_testChannelsHighSense.Distinct().ToList());
                        _testChannelsHighSense.ListChanged += _testChannelsHighSense_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelsHighSense;

        private void _testChannelsHighSense_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelsHighSense));
        }

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("연결할 Low Sense 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelsLowSense
        {
            get => _testChannelsLowSense;
            set
            {
                if (_testChannelsLowSense != value)
                {
                    if (_testChannelsLowSense != null)
                    {
                        _testChannelsLowSense.ListChanged -= _testChannelsLowSense_ListChanged;
                    }
                    _testChannelsLowSense = value;
                    if (_testChannelsLowSense != null)
                    {
                        _testChannelsLowSense = new BindingList<int>(_testChannelsLowSense.Distinct().ToList());
                        _testChannelsLowSense.ListChanged += _testChannelsLowSense_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelsLowSense;

        private void _testChannelsLowSense_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelsLowSense));
        }

        internal override string CategoryName => StepCategory.ElozStimulus.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case StimulusTestMode.Stimulate:
                        var voltageValueText = GetPrefixExpression(Voltage, PhysicalUnit.Volt, out MetricPrefix voltagePrefix);
                        var currentValueText = GetPrefixExpression(Current, PhysicalUnit.Ampere, out MetricPrefix currentPrefix);
                        return $"Stimulus{(IsStimulus10V ? "10V" : "60V")}: {voltageValueText}{voltagePrefix.GetText()}{PhysicalUnit.Volt.GetText()}, " +
                            $"{currentValueText}{currentPrefix.GetText()}{PhysicalUnit.Ampere.GetText()}";
                    case StimulusTestMode.Unstimulate:
                        var valueText = GetPrefixExpression(DischargeDelay, PhysicalUnit.Second, out MetricPrefix prefix);
                        return $"Stimulus{(IsStimulus10V ? "10V" : "60V")}: Delay={valueText}{prefix.GetText()}{PhysicalUnit.Second.GetText()}";
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels
        {
            get
            {
                switch (TestMethod)
                {
                    case StimulusTestMode.Stimulate:
                        var channels = new List<int>();
                        if (TestChannelsHighForce != null)
                        {
                            channels.AddRange(TestChannelsHighForce);
                        }
                        if (TestChannelsLowForce != null)
                        {
                            channels.AddRange(TestChannelsLowForce);
                        }
                        return channels;
                    case StimulusTestMode.MeasureVoltage:
                    case StimulusTestMode.MeasureCurrent:
                        channels = new List<int>();
                        if (TestChannelsHighForce != null)
                        {
                            channels.AddRange(TestChannelsHighForce);
                        }
                        if (TestChannelsLowForce != null)
                        {
                            channels.AddRange(TestChannelsLowForce);
                        }
                        if (TestChannelsHighSense != null)
                        {
                            channels.AddRange(TestChannelsHighSense);
                        }
                        if (TestChannelsLowSense != null)
                        {
                            channels.AddRange(TestChannelsLowSense);
                        }
                        return channels;
                    default:
                        return null;
                }
            }
        }

        private EolElozStimulusStep()
        {
            Name = StepCategory.ElozStimulus.GetText();
        }

        public EolElozStimulusStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            throw new NotSupportedException();
        }

        public override ICollection GetTestModes()
        {
            return Enum.GetValues(typeof(StimulusTestMode));
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out StimulusTestMode parsed))
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
            throw new NotSupportedException();
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            if (TestMethod == StimulusTestMode.MeasureVoltage || TestMethod == StimulusTestMode.MeasureCurrent)
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

            switch (TestMethod)
            {
                case StimulusTestMode.Stimulate:
                    ElozDevice.SharedInstance.Stimulate(DeviceNumber, IsStimulus10V, device, Voltage, Current, 
                        TestChannelsHighForce.ToArray(), TestChannelsLowForce.ToArray());
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.Unstimulate:
                    ElozDevice.SharedInstance.Unstimulate(DeviceNumber, IsStimulus10V, device, DischargeDelay);
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.UnstimulateAll:
                    ElozDevice.SharedInstance.UnstimulateAll(device, DischargeDelay);
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetMaxStimulationVoltage:
                    var maxVoltage = ElozDevice.SharedInstance.GetMaxStimulation(DeviceNumber, IsStimulus10V, device, true);
                    var unit = PhysicalUnit.Volt;
                    var valueText = GetPrefixExpression(maxVoltage, unit, out MetricPrefix prefix);
                    result.ResultInfo = $"MaxStimulationVoltage = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetMaxStimulationCurrent:
                    var maxCurrent = ElozDevice.SharedInstance.GetMaxStimulation(DeviceNumber, IsStimulus10V, device, false);
                    unit = PhysicalUnit.Ampere;
                    valueText = GetPrefixExpression(maxCurrent, unit, out prefix);
                    result.ResultInfo = $"MaxStimulationCurrent = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetConnectionInfo:
                    result.ResultInfo = ElozDevice.SharedInstance.GetStimulusUnitConnectionInfo(DeviceNumber, IsStimulus10V, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetSettingsInfo:
                    result.ResultInfo = ElozDevice.SharedInstance.GetStimulusUnitSettingsInfo(DeviceNumber, IsStimulus10V, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetStimulationCurrent:
                    var current = ElozDevice.SharedInstance.GetStimulation(DeviceNumber, IsStimulus10V, device, false);
                    unit = PhysicalUnit.Ampere;
                    valueText = GetPrefixExpression(current, unit, out prefix);
                    result.ResultInfo = $"StimulationCurrent = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetStimulationVoltage:
                    var voltage = ElozDevice.SharedInstance.GetStimulation(DeviceNumber, IsStimulus10V, device, true);
                    unit = PhysicalUnit.Volt;
                    valueText = GetPrefixExpression(voltage, unit, out prefix);
                    result.ResultInfo = $"StimulationVoltage = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.MeasureVoltage:
                    ElozDevice.SharedInstance.StimulusUnitMeasure(DeviceNumber, IsStimulus10V, device, out double measuredValue, out _,
                        TestChannelsHighForce.ToArray(), TestChannelsLowForce.ToArray(), TestChannelsHighSense.ToArray(), TestChannelsLowSense.ToArray());
                    result.ResultValue = measuredValue - MeasureOffset;
                    result.Unit = GetPhysicalUnit();
                    result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue ?? 0, Tolerance, ToleranceMinusPercent,
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
                case StimulusTestMode.MeasureCurrent:
                    ElozDevice.SharedInstance.StimulusUnitMeasure(DeviceNumber, IsStimulus10V, device, out _, out measuredValue,
                        TestChannelsHighForce.ToArray(), TestChannelsLowForce.ToArray(), TestChannelsHighSense.ToArray(), TestChannelsLowSense.ToArray());
                    result.ResultValue = measuredValue - MeasureOffset;
                    result.Unit = GetPhysicalUnit();
                    result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue ?? 0, Tolerance, ToleranceMinusPercent,
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
                case StimulusTestMode.GetMeasurementVoltageRange:
                    var voltageRange = ElozDevice.SharedInstance.GetStimulusUnitMeasurementRange(DeviceNumber, IsStimulus10V, device, true);
                    unit = PhysicalUnit.Volt;
                    valueText = GetPrefixExpression(voltageRange, unit, out prefix);
                    result.ResultInfo = $"MeasurementVoltageRange = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.GetMeasurementCurrentRange:
                    var currentRange = ElozDevice.SharedInstance.GetStimulusUnitMeasurementRange(DeviceNumber, IsStimulus10V, device, false);
                    unit = PhysicalUnit.Ampere;
                    valueText = GetPrefixExpression(currentRange, unit, out prefix);
                    result.ResultInfo = $"MeasurementCurrentRange = {valueText}{prefix.GetText()}{unit.GetText()}";
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.SetMeasurementVoltageRange:
                    ElozDevice.SharedInstance.SetStimulusUnitMeasurementRange(DeviceNumber, IsStimulus10V, device, true, MeasurementVoltageRange);
                    result.ResultState = ResultState.Pass;
                    break;
                case StimulusTestMode.SetMeasurementCurrentRange:
                    ElozDevice.SharedInstance.SetStimulusUnitMeasurementRange(DeviceNumber, IsStimulus10V, device, false, MeasurementCurrentRange);
                    result.ResultState = ResultState.Pass;
                    break;
                default:
                    throw new NotSupportedException($"eloZ1 디바이스에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            var unit = PhysicalUnit.None;
            switch (TestMethod)
            {
                case StimulusTestMode.MeasureVoltage:
                    unit = PhysicalUnit.Volt;
                    break;
                case StimulusTestMode.MeasureCurrent:
                    unit = PhysicalUnit.Ampere;
                    break;
            }
            return unit;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();

            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), browsable ?? false);
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), false);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            Utils.SetBrowsableAttribute(this, nameof(DeviceNumber), TestMethod != StimulusTestMode.UnstimulateAll);
            Utils.SetBrowsableAttribute(this, nameof(IsStimulus10V), TestMethod != StimulusTestMode.UnstimulateAll);
            bool showMeasureOptions = TestMethod == StimulusTestMode.MeasureVoltage || TestMethod == StimulusTestMode.MeasureCurrent;
            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), showMeasureOptions);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), showMeasureOptions);
            UpdateToleranceAttributes();
            Utils.SetBrowsableAttribute(this, nameof(Current), TestMethod == StimulusTestMode.Stimulate);
            Utils.SetBrowsableAttribute(this, nameof(Voltage), TestMethod == StimulusTestMode.Stimulate);
            Utils.SetBrowsableAttribute(this, nameof(DischargeDelay), TestMethod == StimulusTestMode.Unstimulate || TestMethod == StimulusTestMode.UnstimulateAll);
            Utils.SetBrowsableAttribute(this, nameof(MeasurementVoltageRange), TestMethod == StimulusTestMode.SetMeasurementVoltageRange);
            Utils.SetBrowsableAttribute(this, nameof(MeasurementCurrentRange), TestMethod == StimulusTestMode.SetMeasurementCurrentRange);
            bool showForceChannels = TestMethod == StimulusTestMode.Stimulate || TestMethod == StimulusTestMode.MeasureVoltage;
            Utils.SetBrowsableAttribute(this, nameof(TestChannelsHighForce), showForceChannels);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelsLowForce), showForceChannels);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelsHighSense), TestMethod == StimulusTestMode.MeasureVoltage);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelsLowSense), TestMethod == StimulusTestMode.MeasureVoltage);
        }

        protected override void UpdateToleranceAttributes()
        {
            bool showMeasureOptions = TestMethod == StimulusTestMode.MeasureVoltage || TestMethod == StimulusTestMode.MeasureCurrent;
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.Absolute);

            // Update display names.
            string unitText;
            if (TestMethod == StimulusTestMode.MeasureCurrent)
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
            var newStep = new EolElozStimulusStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolElozStimulusStep stimulusStep)
            {
                stimulusStep.TestMethod = TestMethod;
                stimulusStep.DeviceNumber = DeviceNumber;
                stimulusStep.IsStimulus10V = IsStimulus10V;
                stimulusStep.ExpectedValue = ExpectedValue;
                stimulusStep.Tolerance = Tolerance;
                stimulusStep.TolerancePlusPercent = TolerancePlusPercent;
                stimulusStep.ToleranceMinusPercent = ToleranceMinusPercent;
                stimulusStep.TolerancePlusMinusPercent = TolerancePlusMinusPercent;
                stimulusStep.TolerancePlusAbsolute = TolerancePlusAbsolute;
                stimulusStep.ToleranceMinusAbsolute = ToleranceMinusAbsolute;
                stimulusStep.TolerancePlusMinusAbsolute = TolerancePlusMinusAbsolute;
                stimulusStep.MeasureOffset = MeasureOffset;
                stimulusStep.Current = Current;
                stimulusStep.Voltage = Voltage;
                stimulusStep.DischargeDelay = DischargeDelay;
                stimulusStep.MeasurementVoltageRange = MeasurementVoltageRange;
                stimulusStep.MeasurementCurrentRange = MeasurementCurrentRange;
                if (TestChannelsHighForce != null)
                {
                    stimulusStep.TestChannelsHighForce = new BindingList<int>();
                    foreach (var channel in TestChannelsHighForce)
                    {
                        stimulusStep.TestChannelsHighForce.Add(channel);
                    }
                }
                if (TestChannelsHighSense != null)
                {
                    stimulusStep.TestChannelsHighSense = new BindingList<int>();
                    foreach (var channel in TestChannelsHighSense)
                    {
                        stimulusStep.TestChannelsHighSense.Add(channel);
                    }
                }
                if (TestChannelsLowForce != null)
                {
                    stimulusStep.TestChannelsLowForce = new BindingList<int>();
                    foreach (var channel in TestChannelsLowForce)
                    {
                        stimulusStep.TestChannelsLowForce.Add(channel);
                    }
                }
                if (TestChannelsLowSense != null)
                {
                    stimulusStep.TestChannelsLowSense = new BindingList<int>();
                    foreach (var channel in TestChannelsLowSense)
                    {
                        stimulusStep.TestChannelsLowSense.Add(channel);
                    }
                }
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
