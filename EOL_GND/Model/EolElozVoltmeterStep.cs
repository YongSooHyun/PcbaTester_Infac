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
using static EOL_GND.Model.EolElozStimulusStep;

namespace EOL_GND.Model
{
    public class EolElozVoltmeterStep : EolStep
    {
        public enum VoltmeterTestMode
        {
            GetMeasurementUnitConnectionInfo,
            GetMeasurementUnitSettingsInfo,
            ResetMeasurementUnit,
            SetMeasuringRange,
            GetMeasuringRange,
            GetVoltmeterSettingsInfo,
            Measure,
        }

        [Category(MethodCategory),
            Description("테스트 방법을 설정합니다.")]
        public VoltmeterTestMode TestMethod
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
        private VoltmeterTestMode _testMethod = VoltmeterTestMode.ResetMeasurementUnit;

        [Category(MethodCategory), DefaultValue(1),
            Description("1부터 시작하는 측정 유닛 번호를 설정합니다.")]
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

        [Category(MethodCategory), DisplayName(nameof(MaxVoltage) + " [V]"), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("Voltmeter 측정범위(최대전압)를 설정합니다.")]
        public double MaxVoltage
        {
            get => _maxVoltage;
            set
            {
                if (_maxVoltage != value)
                {
                    _maxVoltage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _maxVoltage;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntArrayConverter)),
            Description("연결할 High Input 테스트 채널들을 설정합니다.")]
        public int[] TestChannelsHighInput
        {
            get => _testChannelsInputHigh;
            set
            {
                if (_testChannelsInputHigh != value)
                {
                    _testChannelsInputHigh = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _testChannelsInputHigh;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntArrayConverter)),
            Description("연결할 Low Input 테스트 채널들을 설정합니다.")]
        public int[] TestChannelsLowInput
        {
            get => _testChannelsInputLow;
            set
            {
                if (_testChannelsInputLow != value)
                {
                    _testChannelsInputLow = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _testChannelsInputLow;

        internal override string CategoryName => StepCategory.ElozVoltmeter.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case VoltmeterTestMode.SetMeasuringRange:
                        var valueText = GetPrefixExpression(MaxVoltage, PhysicalUnit.Volt, out MetricPrefix prefix);
                        return valueText + prefix.GetText() + PhysicalUnit.Volt.GetText();
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
                    case VoltmeterTestMode.Measure:
                        var unit = GetPhysicalUnit();
                        var valueText = GetPrefixExpression(ExpectedValue, unit, out MetricPrefix prefix);
                        return valueText + prefix.GetText() + unit.GetText();
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
                    case VoltmeterTestMode.Measure:
                        return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent, TolerancePlusAbsolute, 
                            TolerancePlusMinusAbsolute, GetPhysicalUnit());
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
                    case VoltmeterTestMode.Measure:
                        return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute,
                            TolerancePlusMinusAbsolute, GetPhysicalUnit());
                    default:
                        return "";
                }
            }
        }

        public override List<int> AllTestChannels
        {
            get
            {
                switch (TestMethod)
                {
                    case VoltmeterTestMode.Measure:
                        var channels = new List<int>();
                        if (TestChannelsHighInput != null)
                        {
                            channels.AddRange(TestChannelsHighInput);
                        }
                        if (TestChannelsLowInput != null)
                        {
                            channels.AddRange(TestChannelsLowInput);
                        }
                        return channels;
                    default:
                        return null;
                }
            }
        }

        private EolElozVoltmeterStep()
        {
            Name = StepCategory.ElozVoltmeter.GetText();
        }

        public EolElozVoltmeterStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override Device.TestDevice CreateDevice()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            throw new NotSupportedException();
        }

        public override ICollection GetTestModes()
        {
            return Enum.GetValues(typeof(VoltmeterTestMode));
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out VoltmeterTestMode parsed))
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
            if (TestMethod == VoltmeterTestMode.Measure)
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
                case VoltmeterTestMode.GetMeasurementUnitConnectionInfo:
                    result.ResultInfo = ElozDevice.SharedInstance.GetMeasurementUnitConnectionInfo(DeviceNumber, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.GetMeasurementUnitSettingsInfo:
                    result.ResultInfo = ElozDevice.SharedInstance.GetMeasurementUnitSettingsInfo(DeviceNumber, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.ResetMeasurementUnit:
                    ElozDevice.SharedInstance.ResetMeasurementUnit(DeviceNumber, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.SetMeasuringRange:
                    ElozDevice.SharedInstance.SetVoltmeterMeasuringRange(DeviceNumber, device, MaxVoltage);
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.GetMeasuringRange:
                    var range = ElozDevice.SharedInstance.GetVoltmeterMeasuringRange(DeviceNumber, device);
                    result.ResultInfo = range + PhysicalUnit.Volt.GetText();
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.GetVoltmeterSettingsInfo:
                    result.ResultInfo = ElozDevice.SharedInstance.GetVoltmeterSettingsInfo(DeviceNumber, device);
                    result.ResultState = ResultState.Pass;
                    break;
                case VoltmeterTestMode.Measure:
                    var measured = ElozDevice.SharedInstance.VoltmeterMeasure(DeviceNumber, device, TestChannelsHighInput, TestChannelsLowInput);
                    result.ResultValue = measured - MeasureOffset;
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
                case VoltmeterTestMode.Measure:
                    unit = PhysicalUnit.Volt;
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

            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), TestMethod == VoltmeterTestMode.Measure);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), TestMethod == VoltmeterTestMode.Measure);
            UpdateToleranceAttributes();
            //Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), TestMethod == VoltmeterTestMode.Measure);

            Utils.SetBrowsableAttribute(this, nameof(MaxVoltage), TestMethod == VoltmeterTestMode.SetMeasuringRange);

            Utils.SetBrowsableAttribute(this, nameof(TestChannelsHighInput), TestMethod == VoltmeterTestMode.Measure);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelsLowInput), TestMethod == VoltmeterTestMode.Measure);
        }

        protected override void UpdateToleranceAttributes()
        {
            bool showMeasureOptions = TestMethod == VoltmeterTestMode.Measure;
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), showMeasureOptions && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), showMeasureOptions && Tolerance == ToleranceMode.Absolute);

            // Update display names.
            string unitText = " [" + PhysicalUnit.Volt.GetText() + "]";
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
            var newStep = new EolElozVoltmeterStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolElozVoltmeterStep voltmeterStep)
            {
                voltmeterStep.TestMethod = TestMethod;
                voltmeterStep.DeviceNumber = DeviceNumber;
                voltmeterStep.ExpectedValue = ExpectedValue;
                voltmeterStep.Tolerance = Tolerance;
                voltmeterStep.TolerancePlusPercent = TolerancePlusPercent;
                voltmeterStep.ToleranceMinusPercent = ToleranceMinusPercent;
                voltmeterStep.TolerancePlusMinusPercent = TolerancePlusMinusPercent;
                voltmeterStep.TolerancePlusAbsolute = TolerancePlusAbsolute;
                voltmeterStep.ToleranceMinusAbsolute = ToleranceMinusAbsolute;
                voltmeterStep.TolerancePlusMinusAbsolute = TolerancePlusMinusAbsolute;
                voltmeterStep.MeasureOffset = MeasureOffset;
                voltmeterStep.MaxVoltage = MaxVoltage;
                if (TestChannelsHighInput != null)
                {
                    voltmeterStep.TestChannelsHighInput = new int[TestChannelsHighInput.Length];
                    TestChannelsHighInput.CopyTo(voltmeterStep.TestChannelsHighInput, 0);
                }
                if (TestChannelsLowInput != null)
                {
                    voltmeterStep.TestChannelsLowInput = new int[TestChannelsLowInput.Length];
                    TestChannelsLowInput.CopyTo(voltmeterStep.TestChannelsLowInput, 0);
                }
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
