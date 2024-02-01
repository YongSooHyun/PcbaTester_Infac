using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Drawing.Design;
using System.Linq;
using System.Net;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    public enum StepCategory
    {
        Power,
        DMM,
        Oscilloscope,
        WaveformGenerator,
        Amplifier,
        DIO,
        CAN,
        LIN,
        ElozRelay,
        ElozStimulus,
        ElozVoltmeter,
        AbortOnFail,
        Dummy,
        GloquadSECC,
        SerialPort,
        AlphaMotion,
        MightyZap,
    }
    public static class StepCategoryExtensions
    {
        public static string GetText(this StepCategory category)
        {
            switch (category)
            {
                case StepCategory.DMM:
                    return "DMM";
                case StepCategory.WaveformGenerator:
                    return "WaveformGenerator";
                case StepCategory.Oscilloscope:
                    return "Oscilloscope";
                case StepCategory.Amplifier:
                    return "Amplifier";
                case StepCategory.DIO:
                    return "DIO";
                case StepCategory.ElozRelay:
                    return "ElozRelay";
                case StepCategory.ElozStimulus:
                    return "ElozStimulus";
                case StepCategory.ElozVoltmeter:
                    return "ElozVoltmeter";
                case StepCategory.Power:
                    return "Power";
                case StepCategory.CAN:
                    return "CAN";
                case StepCategory.LIN:
                    return "LIN";
                case StepCategory.AbortOnFail:
                    return "AbortOnFail";
                case StepCategory.Dummy:
                    return "Dummy";
                case StepCategory.GloquadSECC:
                    return "GloquadSECC";
                case StepCategory.SerialPort:
                    return "SerialPort";
                case StepCategory.AlphaMotion:
                    return "AlphaMotion";
                case StepCategory.MightyZap:
                    return "MightyZap";
                default:
                    return "Unknown";
            }
        }

        public static string GetImageName(this StepCategory category)
        {
            switch (category)
            {
                case StepCategory.DMM:
                    return "dmm-32.png";
                case StepCategory.WaveformGenerator:
                    return "waveform-32.png";
                case StepCategory.Oscilloscope:
                    return "oscilloscope-32.png";
                case StepCategory.Amplifier:
                    return "amplifier-32.png";
                case StepCategory.DIO:
                    return "microchip-32.png";
                case StepCategory.ElozRelay:
                    return "relay-32.png";
                case StepCategory.ElozStimulus:
                    return "stimulus-32.png";
                case StepCategory.ElozVoltmeter:
                    return "voltmeter-32.png";
                case StepCategory.Power:
                    return "power-32.png";
                case StepCategory.CAN:
                    return "can-32.png";
                case StepCategory.LIN:
                    return "lin-32.png";
                case StepCategory.AbortOnFail:
                    return "abort-32.png";
                case StepCategory.Dummy:
                    return "null-32.png";
                case StepCategory.GloquadSECC:
                    return "secc-32.png";
                case StepCategory.SerialPort:
                    return "serial_port-32.png";
                case StepCategory.AlphaMotion:
                    return "servo-32.png";
                case StepCategory.MightyZap:
                    return "mighty_zap-32.png";
                default:
                    return null;
            }
        }
    }

    /// <summary>
    /// EOL step base class.
    /// </summary>
    [XmlInclude(typeof(EolPowerStep))]
    [XmlInclude(typeof(EolWaveformGeneratorStep))]
    [XmlInclude(typeof(EolDmmStep))]
    [XmlInclude(typeof(EolOscopeStep))]
    [XmlInclude(typeof(EolDioStep))]
    [XmlInclude(typeof(EolCanStep))]
    [XmlInclude(typeof(EolElozRelayStep))]
    [XmlInclude(typeof(EolElozStimulusStep))]
    [XmlInclude(typeof(EolElozVoltmeterStep))]
    [XmlInclude(typeof(EolAbortStep))]
    [XmlInclude(typeof(EolDummyStep))]
    [XmlInclude(typeof(EolLinStep))]
    [XmlInclude(typeof(EolAmplifierStep))]
    [XmlInclude(typeof(EolGloquadEvseStep))]
    [XmlInclude(typeof(EolSerialPortStep))]
    [XmlInclude(typeof(EolAlphaMotionStep))]
    [XmlInclude(typeof(EolMightyZapStep))]
    public abstract class EolStep : INotifyPropertyChanged, ICloneable
    {
        protected const string DispNameTolPrefix = "   ";
        protected const string PlusSign = "+";
        protected const string MinusSign = "-";
        protected const string PlusMinusSign = "±";
        protected const string DispNameTolPlus = DispNameTolPrefix + PlusSign;
        protected const string DispNameTolMinus = DispNameTolPrefix + MinusSign;
        protected const string DispNameTolPlusMinus = DispNameTolPrefix + PlusMinusSign;

        private const string CallCategory = "Call Settings";
        private const string GeneralCategory = "General Settings";
        protected const string MethodCategory = "Method Settings";

        public event PropertyChangedEventHandler PropertyChanged;

        /// <summary>
        /// Unique ID.
        /// </summary>
        [Category(CallCategory), Browsable(true), ReadOnly(true),
            Description("이 테스트 스텝을 유일하게 식별하는 ID로, 자동 할당됩니다.")]
        public int Id
        {
            get => _id;
            set
            {
                if (_id != value)
                {
                    _id = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _id;

        [Category(CallCategory), Browsable(false), XmlIgnore]
        public int No
        {
            get => _no;
            set
            {
                if (_no != value)
                {
                    _no = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _no;

        /// <summary>
        /// 스텝을 실행할 것인지 여부.
        /// </summary>
        [Category(CallCategory), DefaultValue(true), Description("이 테스트 스텝을 실행할 것인지 아닌지를 나타냅니다.")]
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
        private bool _enabled = true;

        /// <summary>
        /// 스텝 ID.
        /// </summary>
        [Category(GeneralCategory), Description("이 테스트 스텝의 ID를 설정합니다.")]
        public int StepId
        {
            get => _stepId;
            set
            {
                if (_stepId != value)
                {
                    _stepId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _stepId;

        /// <summary>
        /// 섹션.
        /// </summary>
        [Category(GeneralCategory), Description("이 테스트 스텝의 섹션을 지정합니다.")]
        public string Section
        {
            get => _section;
            set
            {
                if (_section != value)
                {
                    _section = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _section;

        /// <summary>
        /// 스텝 이름.
        /// </summary>
        [Category(GeneralCategory), Description("이 테스트 스텝의 이름을 설정합니다.")]
        public string Name
        {
            get => _name;
            set
            {
                if (_name != value)
                {
                    _name = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _name;

        /// <summary>
        /// 디바이스 이름.
        /// </summary>
        [Category(GeneralCategory), TypeConverter(typeof(DeviceNameConverter)), Browsable(true),
            Description("이 테스트 스텝에서 사용할 장치 이름을 설정합니다.")]
        public string DeviceName
        {
            get => _deviceName;
            set
            {
                if (_deviceName != value)
                {
                    _deviceName = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private string _deviceName;

        /// <summary>
        /// Remarks.
        /// </summary>
        [Category(GeneralCategory), Description("이 테스트 스텝에 대한 설명을 설정합니다.")]
        public string Remarks
        {
            get => _remarks;
            set
            {
                if (_remarks != value)
                {
                    _remarks = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _remarks;

        /// <summary>
        /// 다시 테스트하는 방식.
        /// </summary>
        [Category(GeneralCategory), DefaultValue(TestRetryMode.Never), Browsable(true),
            Description("테스트를 다시 실행하는 방식을 설정합니다.")]
        public TestRetryMode RetestMode
        {
            get => _retestMode;
            set
            {
                if (_retestMode != value)
                {
                    _retestMode = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private TestRetryMode _retestMode = TestRetryMode.Never;

        /// <summary>
        /// 다시 테스트하는 횟수.
        /// </summary>
        [Category(GeneralCategory), Browsable(false), DefaultValue(1),
            Description("테스트를 다시 실행하는 횟수를 설정합니다.")]
        public int RetestCount
        {
            get => _retestCount;
            set
            {
                if (_retestCount != value)
                {
                    _retestCount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _retestCount = 1;

        /// <summary>
        /// 다시 테스트할 때까지 지연시간, millisecond.
        /// </summary>
        [Category(GeneralCategory), DisplayName("RetestDelay [ms]"), Browsable(true), DefaultValue(0), 
            Description("테스트를 다시 실행하는 경우, 다시 테스트할 때까지의 지연 시간을 ms로 설정합니다.")]
        public int RetestDelay
        {
            get => _retestDelay;
            set
            {
                if (_retestDelay != value)
                {
                    _retestDelay = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _retestDelay = 0;

        [Category(GeneralCategory), DisplayName("DelayBefore [ms]"), DefaultValue(0), 
            Description("테스트를 실행할 때까지 지연 시간을 ms로 설정합니다.")]
        public int DelayBefore
        {
            get => _delayBefore;
            set
            {
                if (_delayBefore != value)
                {
                    _delayBefore = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _delayBefore = 0;

        [Category(GeneralCategory), DisplayName("DelayAfter [ms]"), DefaultValue(0), Browsable(true),
            Description("테스트를 실행한 다음 종료할 때까지 지연 시간을 ms로 설정합니다.")]
        public int DelayAfter
        {
            get => _delayAfter;
            set
            {
                if (_delayAfter != value)
                {
                    _delayAfter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _delayAfter = 0;

        public enum ElozRelayMode
        {
            ON,
            OFF,
        }

        [Category(GeneralCategory), DefaultValue(ElozRelayMode.OFF),
            Description("테스트가 끝난 다음 eloZ1 Relay를 끌 것인지를 나타냅니다.")]
        public ElozRelayMode ElozRelayAfter
        {
            get => _elozRelayAfter;
            set
            {
                if (_elozRelayAfter != value)
                {
                    _elozRelayAfter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ElozRelayMode _elozRelayAfter = ElozRelayMode.OFF;

        [Category(GeneralCategory), DefaultValue(false), Browsable(true),
            Description("테스트 결과 로그에 추가적인 ResultLogInfo를 반영할지 여부를 나타냅니다.")]
        public bool ResultLogInfo
        {
            get => _resultLogInfo;
            set
            {
                if (_resultLogInfo != value)
                {
                    _resultLogInfo = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _resultLogInfo = false;

        [Category(GeneralCategory), Browsable(true),
            Description("테스트 결과가 FAIL인 경우 로그에 반영할 결과 메시지를 지정합니다.")]
        public string ResultFailLogMessage
        {
            get => _resultFailLogMessage;
            set
            {
                if (_resultFailLogMessage != value)
                {
                    _resultFailLogMessage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _resultFailLogMessage;

        [Category(GeneralCategory), Browsable(true), DefaultValue(false)]
        [Description("테스트 결과를 사양서 비교를 위한 로그에 저장할 것인지 여부를 나타냅니다.")]
        public bool ResultSpecLog
        {
            get => _resultSpecLog;
            set
            {
                if (_resultSpecLog != value)
                {
                    _resultSpecLog = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _resultSpecLog = false;

        [Category(GeneralCategory), Browsable(true), DefaultValue(false)]
        [Description("전체 테스트 결과를 집계할 때 이 스텝의 테스트 결과를 반영하지 않을 것인지 설정합니다.")]
        public bool IgnoreResult
        {
            get => _ignoreResult;
            set
            {
                if (_ignoreResult != value)
                {
                    _ignoreResult = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _ignoreResult = false;

        [Category(GeneralCategory), Browsable(true), DefaultValue(false)]
        [Description("이 스텝의 테스트 결과를 반전(Pass->Fail or Fail->Pass)시킬 것인지 설정합니다.")]
        public bool InvertResult
        {
            get => _invertResult;
            set
            {
                if (_invertResult != value)
                {
                    _invertResult = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _invertResult = false;

        [Category(GeneralCategory), Browsable(true), DefaultValue(true)]
        [Description("이 스텝의 실행 결과가 FAIL이면 전체 테스트를 종료할 것인지 여부를 설정합니다.")]
        public bool AbortOnFail
        {
            get => _abortOnFail;
            set
            {
                if (_abortOnFail != value)
                {
                    _abortOnFail = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _abortOnFail = true;

        /// <summary>
        /// 스텝을 어떤 조건에서 실행할지 나타낸다.
        /// </summary>
        public enum RunMode
        {
            /// <summary>
            /// 스텝 항상 실행.
            /// </summary>
            Always,

            /// <summary>
            /// 마스터보드가 아닐 때 실행.
            /// </summary>
            PCB_Not_Master,

            /// <summary>
            /// 마스터보드일 때 실행.
            /// </summary>
            PCB_Master,

            /// <summary>
            /// 양품 마스터일 때 실행.
            /// </summary>
            PCB_Master_OK,

            /// <summary>
            /// 불량 마스터일 때 실행.
            /// </summary>
            PCB_Master_NG,

            /// <summary>
            /// 양품 마스터가 아닐 때 실행.
            /// </summary>
            PCB_Not_Master_OK,

            /// <summary>
            /// 불량 마스터가 아닐 때 실행.
            /// </summary>
            PCB_Not_Master_NG,
        }

        [Category(GeneralCategory), Browsable(true), DefaultValue(RunMode.Always)]
        [Description("이 스텝이 어떤 조건에서 실행되는지 설정합니다.\r\n" +
            " • " + nameof(RunMode.Always) + " : 이 스텝이 항상 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Not_Master) + " : PCB가 마스터 보드가 아닐 때 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Master) + " : PCB가 양품/불량 마스터 보드일 때 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Master_OK) + " : PCB가 양품 마스터 보드일 때 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Master_NG) + " : PCB가 불량 마스터 보드일 때 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Not_Master_OK) + " : PCB가 양품 마스터 보드가 아닐 때 실행됩니다.\r\n" +
            " • " + nameof(RunMode.PCB_Not_Master_NG) + " : PCB가 불량 마스터 보드가 아닐 때 실행됩니다.")]
        public RunMode ExecuteMode
        {
            get => _executeMode;
            set
            {
                if (_executeMode != value)
                {
                    _executeMode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private RunMode _executeMode = RunMode.Always;

        [Category(GeneralCategory), Browsable(true)]
        [TypeConverter(typeof(StringArrayConverter))]
        [Editor(typeof(VariantArrayEditor), typeof(UITypeEditor))]
        [Description("이 스텝이 실행되는 variant들을 나열합니다. 비어있으면 variant에 관계없이 항상 실행됩니다.")]
        public string[] Variants
        {
            get => _variants;
            set
            {
                if (_variants != value)
                {
                    _variants = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string[] _variants;

        [Category(MethodCategory), DefaultValue(0.0), Browsable(false),
            Description("테스트 결과가 Pass일 때 CP(Capability of Process)를 설정한 값에 근접하도록 측정값을 조정합니다.")]
        public double CP
        {
            get => _cp;
            set
            {
                if (_cp != value)
                {
                    _cp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _cp;

        [Category(MethodCategory), DefaultValue(0), Browsable(false),
            Description("")]
        public int TempSignificantDigits
        {
            get => _tempSignificantDigits;
            set
            {
                if (_tempSignificantDigits != value)
                {
                    _tempSignificantDigits = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _tempSignificantDigits = 0;

        [Category(MethodCategory), DefaultValue(1E-3), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("")]
        public double TempResolution
        {
            get => _tempResolution;
            set
            {
                if (_tempResolution != value)
                {
                    _tempResolution = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tempResolution = 1E-3;

        internal abstract string CategoryName { get; }

        /// <summary>
        /// Test Mode 문자열.
        /// </summary>
        [Browsable(false)]
        public abstract string TestModeDesc { get; }

        /// <summary>
        /// Parameter 문자열.
        /// </summary>
        [Browsable(false)]
        public abstract string ParameterDesc { get; }

        /// <summary>
        /// 예상 값.
        /// </summary>
        [Browsable(false)]
        public abstract string ExpectedValueDesc { get; }

        [Browsable(false)]
        public abstract string TolerancePlusDesc { get; }

        [Browsable(false)]
        public abstract string ToleranceMinusDesc { get; }

        [Browsable(false)]
        public virtual string MinValueText
        {
            get
            {
                GetNominalValues(out double? nominal, out _, out double? lowerLimit);
                if (lowerLimit == null)
                {
                    if (nominal == null)
                    {
                        return null;
                    }
                    else
                    {
                        return "-∞";
                    }
                }
                else
                {
                    var unit = GetPhysicalUnit();
                    var valueText = GetPrefixExpression(lowerLimit, unit, out MetricPrefix prefix);
                    return valueText + prefix.GetText() + unit.GetText();
                }
            }
        }

        [Browsable(false)]
        public virtual string MaxValueText
        {
            get
            {
                GetNominalValues(out double? nominal, out double? upperLimit, out _);
                if (upperLimit == null)
                {
                    if (nominal == null)
                    {
                        return null;
                    }
                    else
                    {
                        return "+∞";
                    }
                }
                else
                {
                    var unit = GetPhysicalUnit();
                    var valueText = GetPrefixExpression(upperLimit, unit, out MetricPrefix prefix);
                    return valueText + prefix.GetText() + unit.GetText();
                }
            }
        }

        [Browsable(false)]
        public abstract List<int> AllTestChannels { get; }

        [Browsable(false)]
        public virtual string MeasuredValueDesc => RunResult?.ResultValueDesc;

        [Browsable(false)]
        public string ResultStateDesc => IgnoreResult ? null : RunResult?.ResultStateDesc;

        [Browsable(false)]
        public string ResultInfo => RunResult?.ResultInfo;

        [Browsable(false)]
        public long? ResultTotalMilliseconds => RunResult?.TotalMilliseconds;

        [Browsable(false)]
        public string VariantsDesc => Variants == null ? null : string.Join(",", Variants);

        /// <summary>
        /// 테스트 결과.
        /// </summary>
        [XmlIgnore, Browsable(false)]
        public TestResult RunResult
        {
            get => _runResult;
            set
            {
                if (_runResult != value)
                {
                    if (_runResult != null)
                    {
                        _runResult.PropertyChanged -= RunResult_PropertyChanged;
                    }

                    _runResult = value;

                    if (_runResult != null)
                    {
                        _runResult.PropertyChanged += RunResult_PropertyChanged;
                    }

                    NotifyPropertyChanged();
                }
            }
        }
        private TestResult _runResult;

        private void RunResult_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(RunResult));
        }


        /// <summary>
        /// 프로퍼티들의 <see cref="BrowsableAttribute"/>들을 업데이트한다.
        /// </summary>
        public virtual void UpdateBrowsableAttributes()
        {
            switch (RetestMode)
            {
                case TestRetryMode.Never:
                    Utils.SetBrowsableAttribute(this, nameof(RetestCount), false);
                    Utils.SetBrowsableAttribute(this, nameof(RetestDelay), false);
                    break;
                case TestRetryMode.OnFail:
                    Utils.SetBrowsableAttribute(this, nameof(RetestCount), true);
                    Utils.SetBrowsableAttribute(this, nameof(RetestDelay), true);
                    break;
            }
        }

        /// <summary>
        /// 양품/불량 마스터, 일반 보드 여부에 따라 이 스텝을 실행해야 하는지 결정.
        /// </summary>
        /// <param name="isMasterGood">양품 마스터이면 true, 불량 마스터이면 false, 일반 보드이면 null.</param>
        /// <returns>이 스텝을 실행해야 하면 true, 아니면 false.</returns>
        public bool CheckExecuteMode(bool? isMasterGood)
        {
            switch (ExecuteMode)
            {
                case RunMode.Always:
                    return true;
                case RunMode.PCB_Not_Master:
                    return isMasterGood == null;
                case RunMode.PCB_Master:
                    return isMasterGood != null;
                case RunMode.PCB_Master_OK:
                    return isMasterGood == true;
                case RunMode.PCB_Master_NG:
                    return isMasterGood == false;
                case RunMode.PCB_Not_Master_OK:
                    return isMasterGood != true;
                case RunMode.PCB_Not_Master_NG:
                    return isMasterGood != false;
                default:
                    return true;
            }
        }

        /// <summary>
        /// 주어진 variant에 따라 이 스텝을 실행해야 하는지 결정.
        /// </summary>
        /// <param name="variant"></param>
        /// <returns></returns>
        public bool CheckVariant(string variant)
        {
            if (Variants == null || Variants.Length == 0)
            {
                // 이 스텝의 Variants가 비어있으면 스텝 무조건 실행.
                return true;
            }
            else if (string.IsNullOrWhiteSpace(variant))
            {
                // 이 스텝의 Variants가 비어있지 않은데 실행 variant가 비어있으면 에러.
                throw new Exception("시퀀스 실행 variant가 설정되지 않았습니다.");
            }

            return Variants.Contains(variant, StringComparer.OrdinalIgnoreCase);
        }

        protected abstract void UpdateToleranceAttributes();

        /// <summary>
        /// 스텝이 이용할 디바이스를 만든다.
        /// </summary>
        /// <returns></returns>
        public abstract TestDevice CreateDevice();

        /// <summary>
        /// eloZ1 Relay ON.
        /// </summary>
        /// <param name="elozTestSet"></param>
        /// <param name="setting"></param>
        protected abstract void RelayOn(object elozTestSet, DeviceSetting setting);

        /// <summary>
        /// eloZ1 Relay OFF.
        /// </summary>
        /// <param name="elozTestSet"></param>
        internal void RelayOff(object elozTestSet)
        {
            ElozDevice.SharedInstance.RelayOff(elozTestSet);
        }

        // 스텝을 실행하는 실제 코드.
        protected abstract TestResult RunTest(object device, CancellationToken token);

        // 물리량 표시를 단위로 하기 위한 메서드.
        public abstract PhysicalUnit GetPhysicalUnit();

        /// <summary>
        /// 보조단위가 붙은 double?값 문자열을 double?로 변환한다.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="value"></param>
        /// <param name="converted"></param>
        /// <returns></returns>
        public static bool ConvertToDouble(PropertyDescriptor descriptor, string value, out double? converted)
        {
            if (string.IsNullOrEmpty(value))
            {
                converted = null;

                if (descriptor.PropertyType == typeof(double?))
                {
                    return true;
                }

                return false;
            }

            // 문자열이 보조단위로 끝나는가 검사.
            var prefixes = MetricPrefixExtensions.GetPrefixes();
            foreach (var prefix in prefixes)
            {
                if (value.EndsWith(prefix))
                {
                    var numberString = value.Substring(0, value.LastIndexOf(prefix));
                    if (double.TryParse(numberString, out double doubleValue) && MetricPrefixExtensions.TryParse(prefix, out MetricPrefix unitPrefix))
                    {
                        converted = doubleValue * unitPrefix.GetMultiplier();
                        return true;
                    }

                    converted = null;
                    return false;
                }
            }

            // 디폴트 변환.
            var converter = TypeDescriptor.GetConverter(descriptor.PropertyType);
            if (converter.CanConvertFrom(value.GetType()))
            {
                converted = converter.ConvertFrom(value) as double?;
                return true;
            }
            else
            {
                converted = null;
                return false;
            }
        }

        /// <summary>
        /// double?값을 보조단위가 붙은 문자열로 변환한다.
        /// </summary>
        /// <param name="descriptor"></param>
        /// <param name="value"></param>
        /// <returns></returns>
        public string ConvertToString(PropertyDescriptor descriptor, double? value)
        {
            if (value == null)
            {
                return "";
            }

            var valueText = GetPrefixExpression(value, GetPhysicalUnit(), out MetricPrefix prefix);
            return valueText + prefix.GetText();
        }

        public void ClearRunResult()
        {
            RunResult = null;
        }

        /// <summary>
        /// 테스트 스텝을 실행한다.
        /// </summary>
        public TestResult Execute(object elozTestSet, bool forceRelayOff, CancellationToken token)
        {
            // 테스트 실행.
            Logger.LogVerbose($"---Running step(Id={Id}, Name={Name})---");

            RunResult = null;

            // 실행 시간 측정.
            var startTime = DateTime.Now;
            var stopwatch = Stopwatch.StartNew();

            // 실행 결과 초기화.
            var result = new TestResult(this)
            {
                ResultState = ResultState.NoState,
                ResultInfo = null,
                ResultValue = null,
                ResultValueState = ResultValueState.Invalid,
                Unit = GetPhysicalUnit(),
            };

            // 테스트에 이용하는 디바이스.
            TestDevice testDevice = null;

            // eloZ1 relay 스텝인지 체크.
            bool isRelay = this is EolElozRelayStep;
            bool isStimulus = this is EolElozStimulusStep;
            bool isVoltmeter = this is EolElozVoltmeterStep;
            bool isDummy = this is EolDummyStep;
            bool isAlphaMotion = this is EolAlphaMotionStep;
            bool isMightyZap = this is EolMightyZapStep;

            try
            {
                // 실행 전 Delay.
                if (DelayBefore > 0)
                {
                    MultimediaTimer.Delay(DelayBefore, token).Wait(token);
                }

                try
                {
                    if (!isRelay && !isStimulus && !isVoltmeter && !isDummy && !isAlphaMotion && !isMightyZap)
                    {
                        // DMM PureTemp 여부.
                        bool isDmmPureTemp = this is EolDmmStep dmmStep && dmmStep.IsPureTemp;

                        // 디바이스 설정에 따라 이용할 디바이스를 만든다.
                        if (!isDmmPureTemp)
                        {
                            testDevice = CreateDevice();
                        }

                        // eloZ1 Relay ON.
                        if (elozTestSet != null)
                        {
                            RelayOn(elozTestSet, testDevice.Setting);
                        }

                        if (elozTestSet != null)
                        {
                            Logger.LogDebug("----- Connection Info -----\r\n" + ElozDevice.SharedInstance.GetConnectionInfo(elozTestSet));
                        }

                        // 장치 연결.
                        if (!isDmmPureTemp)
                        {
                            testDevice.Connect(token);
                        }

                        // 테스트 실행.
                        result = RunTest(testDevice, token);
                    }
                    else
                    {
                        if (elozTestSet != null)
                        {
                            Logger.LogDebug("----- Connection Info -----\r\n" + ElozDevice.SharedInstance.GetConnectionInfo(elozTestSet));
                        }

                        // 테스트 실행.
                        result = RunTest(elozTestSet, token);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"Step({Id}, {Name}) Error: {ex.Message}");
                    result.ResultValue = null;
                    result.ResultInfo = "Error: " + ex.Message;
                    result.ResultState = (ex is OperationCanceledException) ? ResultState.Aborted : ResultState.Fail;
                    result.ResultValueState = ResultValueState.Invalid;
                }

                // 테스트가 실패하면 설정에 따라 다시 시도.
                // 오실로스코프 Measure 인 경우 바로 위의 Capture부터 다시 시작하므로, 여기서는 다시 테스트 안함.
                bool isOscopeMeasure = this is EolOscopeStep oscopeStep && oscopeStep.TestMethod == EolOscopeStep.OscopeTestMode.Measure;
                if (!isOscopeMeasure && RetestMode == TestRetryMode.OnFail && result.ResultState == ResultState.Fail)
                {
                    for (int i = 0; i < RetestCount; i++)
                    {
                        // Cancel여부 체크.
                        token.ThrowIfCancellationRequested();

                        if (RetestDelay > 0)
                        {
                            // TODO: Delay타임이 1ms인 경우 최대 3ms까지 오차 발생하므로, eloz 디바이스 타이머 이용 고려해야.
                            MultimediaTimer.Delay(RetestDelay, token).Wait(token);
                        }

                        // 테스트 실행.
                        try
                        {
                            if (!isRelay && !isStimulus && !isVoltmeter && !isDummy)
                            {
                                result = RunTest(testDevice, token);
                            }
                            else
                            {
                                result = RunTest(elozTestSet, token);
                            }
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Step({Id}, {Name}) Error: {ex.Message}");
                            result.ResultValue = null;
                            result.ResultInfo = "Error: " + ex.Message;
                            result.ResultState = (ex is OperationCanceledException) ? ResultState.Aborted : ResultState.Fail;
                            result.ResultValueState = ResultValueState.Invalid;
                        }

                        // 테스트가 실패하지 않았으면 다시 시도 중지.
                        if (result.ResultState != ResultState.Fail)
                        {
                            break;
                        }
                    }
                }

                // 테스트 실행 결과 반전.
                if (InvertResult)
                {
                    if (result.ResultState == ResultState.Pass)
                    {
                        result.ResultState = ResultState.Fail;
                    }
                    else if (result.ResultState == ResultState.Fail)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Step({Id}, {Name}) Error: {ex.Message}");
                result.ResultValue = null;
                result.ResultInfo = "Error: " + ex.Message;
                result.ResultState = (ex is OperationCanceledException) ? ResultState.Aborted : ResultState.Fail;
                result.ResultValueState = ResultValueState.Invalid;
            }
            finally
            {
                //testDevice?.Disconnect();

                // eloZ1 Relay OFF.
                if (elozTestSet != null)
                {
                    if (forceRelayOff || ElozRelayAfter == ElozRelayMode.OFF)
                    {
                        try
                        {
                            RelayOff(elozTestSet);
                        }
                        catch (Exception ex)
                        {
                            result.ResultState = ResultState.Aborted;
                            result.ResultInfo = "Error: " + ex.Message;
                        }
                    }
                }

                // 실행 후 Delay.
                if (DelayAfter > 0)
                {
                    try
                    {
                        MultimediaTimer.Delay(DelayAfter, token).Wait(token);
                    }
                    catch (Exception ex)
                    {
                        result.ResultState = ResultState.Aborted;
                        result.ResultInfo = "Error: " + ex.Message;
                    }
                }

                stopwatch.Stop();
                result.TotalMilliseconds = stopwatch.ElapsedMilliseconds;
                result.FinishTime = DateTime.Now;
                result.StartTime = startTime;
            }

            // 테스트 실행.
            Logger.LogVerbose($"---Finished step(Id={StepId}, Name={Name})---");

            RunResult = result;
            return result;
        }

        /// <summary>
        /// 사람이 읽기 쉽게 보조단위로 변환한 문자열 리턴.
        /// </summary>
        /// <param name="value">변환하려는 값.</param>
        /// <param name="unit">단위.</param>
        /// <returns>접두어에 따라 변환된 값 문자열(접두어, 단위 붙지 않음).</returns>
        public static string GetPrefixExpression(double? value, PhysicalUnit unit, out MetricPrefix prefix)
        {
            if (value == null)
            {
                prefix = MetricPrefix.None;
                return null;
            }

            string valueText;
            switch (unit)
            {
                case PhysicalUnit.Ampere:
                case PhysicalUnit.Farad:
                case PhysicalUnit.Henry:
                case PhysicalUnit.Hertz:
                case PhysicalUnit.Newton:
                case PhysicalUnit.Ohm:
                case PhysicalUnit.Second:
                case PhysicalUnit.Volt:
                case PhysicalUnit.Watt:
                    prefix = GetMetricPrefix(value.GetValueOrDefault());
                    var multiplier = prefix.GetMultiplier();

                    // 초는 Kilo, Mega, Giga 등 사용 안 함.
                    if (unit == PhysicalUnit.Second && multiplier > 1)
                    {
                        prefix = MetricPrefix.None;
                        multiplier = prefix.GetMultiplier();
                    }

                    var convertedValue = value / multiplier;
                    if (convertedValue >= 1000)
                    {
                        valueText = $"{convertedValue}";
                    }
                    else
                    {
                        valueText = $"{convertedValue:0.####}";
                    }
                    break;
                case PhysicalUnit.Celsius:
                case PhysicalUnit.Decibel:
                case PhysicalUnit.Degree:
                case PhysicalUnit.Percent:
                case PhysicalUnit.None:
                default:
                    prefix = MetricPrefix.None;
                    valueText = $"{value}";
                    break;
            }

            return valueText;
        }

        /// <summary>
        /// 사람이 읽기 쉬운 단위 접두어를 리턴.
        /// </summary>
        /// <param name="value"></param>
        /// <returns></returns>
        public static MetricPrefix GetMetricPrefix(double value)
        {
            MetricPrefix prefix;
            double absValue = Math.Abs(value);
            if (absValue == 0)
            {
                prefix = MetricPrefix.None;
            }
            else if (absValue >= 1E9)
            {
                prefix = MetricPrefix.Giga;
            }
            else if (absValue >= 1E6)
            {
                prefix = MetricPrefix.Mega;
            }
            else if (absValue >= 1E3)
            {
                prefix = MetricPrefix.Kilo;
            }
            else if (absValue >= 1)
            {
                prefix = MetricPrefix.None;
            }
            else if (absValue >= 1E-3)
            {
                prefix = MetricPrefix.Milli;
            }
            else if (absValue >= 1E-6)
            {
                prefix = MetricPrefix.Micro;
            }
            else if (absValue >= 1E-9)
            {
                prefix = MetricPrefix.Nano;
            }
            else
            {
                prefix = MetricPrefix.Pico;
            }
            return prefix;
        }

        /// <summary>
        /// 주어진 유형의 테스트 스텝을 만든다.
        /// </summary>
        /// <param name="category"></param>
        /// <returns></returns>
        public static EolStep CreateStep(StepCategory category)
        {
            switch (category)
            {
                case StepCategory.WaveformGenerator:
                    return new EolWaveformGeneratorStep(null);
                case StepCategory.DMM:
                    return new EolDmmStep(null);
                case StepCategory.Oscilloscope:
                    return new EolOscopeStep(null);
                case StepCategory.Amplifier:
                    return new EolAmplifierStep(null);
                case StepCategory.DIO:
                    return new EolDioStep(null);
                case StepCategory.ElozRelay:
                    return new EolElozRelayStep(null);
                case StepCategory.ElozStimulus:
                    return new EolElozStimulusStep(null);
                case StepCategory.ElozVoltmeter:
                    return new EolElozVoltmeterStep(null);
                case StepCategory.CAN:
                    return new EolCanStep(null);
                case StepCategory.LIN:
                    return new EolLinStep(null);
                case StepCategory.AbortOnFail:
                    return new EolAbortStep(null);
                case StepCategory.Dummy:
                    return new EolDummyStep(null);
                case StepCategory.GloquadSECC:
                    return new EolGloquadEvseStep(null);
                case StepCategory.SerialPort:
                    return new EolSerialPortStep(null);
                case StepCategory.AlphaMotion:
                    return new EolAlphaMotionStep(null);
                case StepCategory.MightyZap:
                    return new EolMightyZapStep(null);
                case StepCategory.Power:
                default:
                    return new EolPowerStep(null);
            }
        }

        /// <summary>
        /// 측정값이 허용오차 범위내에 있는가를 판단한다.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="measured"></param>
        /// <param name="tolMode"></param>
        /// <param name="tolMinusPercent"></param>
        /// <param name="tolPlusPercent"></param>
        /// <param name="tolPlusMinusPercent"></param>
        /// <param name="tolMinusAbs"></param>
        /// <param name="tolPlusAbs"></param>
        /// <param name="tolPlusMinusAbs"></param>
        /// <returns></returns>
        public ResultValueState CalcValueState(double expected, double measured, ToleranceMode tolMode, double tolMinusPercent, 
            double tolPlusPercent, double tolPlusMinusPercent, double tolMinusAbs, double tolPlusAbs, double tolPlusMinusAbs, out double? adjusted)
        {
            adjusted = null;
            ResultValueState valueState;

            GetLimitValues(expected, tolMode, tolMinusPercent, tolPlusPercent, tolPlusMinusPercent, tolMinusAbs, tolPlusAbs, 
                tolPlusMinusAbs, out double? upperLimit, out double? lowerLimit);
            if (upperLimit != null && measured > upperLimit)
            {
                valueState = ResultValueState.High;
            }
            else if (lowerLimit != null && measured < lowerLimit)
            {
                valueState = ResultValueState.Low;
            }
            else
            {
                valueState = ResultValueState.Good;
            }

            // CP 설정에 맞는 랜덤 수 발생.
            if (valueState == ResultValueState.Good && CP > 0)
            {
                // 표준편차 계산.
                var delta = GetUpperLowerDelta(upperLimit, lowerLimit);
                if (delta > 0)
                {
                    double stdev = delta.GetValueOrDefault() / (6 * CP);
                    double nextGaussian = Utils.NextGaussian(Id, expected, stdev);
                    if (upperLimit != null && nextGaussian > upperLimit)
                    {
                        nextGaussian = upperLimit.GetValueOrDefault();
                    }
                    else if (lowerLimit != null && nextGaussian < lowerLimit)
                    {
                        nextGaussian = lowerLimit.GetValueOrDefault();
                    }

                    // Resolution 설정에 따라 밑의 숫자 반올림.
                    adjusted = Utils.ResolutionTruncate(nextGaussian, TempResolution);

                    // 유효숫자만 남기고 반올림.
                    if (TempSignificantDigits > 0)
                    {
                        adjusted = Utils.RoundToSignificantDigits(nextGaussian, TempSignificantDigits);
                    }

                    if (upperLimit != null && adjusted > upperLimit)
                    {
                        adjusted = upperLimit;
                    }
                    else if (lowerLimit != null && adjusted < lowerLimit)
                    {
                        adjusted = lowerLimit;
                    }
                }
            }

            return valueState;
        }

        /// <summary>
        /// CP 계산을 위한 UpperLimit, LowerLimit 차를 리턴한다.
        /// </summary>
        /// <param name="expected"></param>
        /// <param name="upperLimit"></param>
        /// <param name="lowerLimit"></param>
        /// <returns></returns>
        internal double? GetUpperLowerDelta(double? upperLimit, double? lowerLimit)
        {
            return upperLimit - lowerLimit;
        }

        public double GetTempValue(ToleranceMode tolMode, double expected, double tolPlusMinusAbsolute, double tolPlusAbsolute, 
            double tolMinusAbsolute, double tolPlusMinusPercent, double tolPlusPercent, double tolMinusPercent)
        {
            double lowerLimit, upperLimit;
            switch (tolMode)
            {
                case ToleranceMode.Absolute:
                    lowerLimit = expected - tolPlusMinusAbsolute;
                    upperLimit = expected + tolPlusMinusAbsolute;
                    break;
                case ToleranceMode.AbsolutePlusMinus:
                    lowerLimit = expected - tolMinusAbsolute;
                    upperLimit = expected + tolPlusAbsolute;
                    break;
                case ToleranceMode.Less:
                    if (expected > 0)
                    {
                        lowerLimit = 0;
                    }
                    else
                    {
                        lowerLimit = double.MinValue;
                    }
                    upperLimit = expected;
                    break;
                case ToleranceMode.Greater:
                    lowerLimit = expected;
                    if (expected >= 0)
                    {
                        upperLimit = double.MaxValue;
                    }
                    else
                    {
                        upperLimit = 0;
                    }
                    break;
                case ToleranceMode.RelativePlusMinus:
                    lowerLimit = expected - Math.Abs(expected) * tolMinusPercent / 100;
                    upperLimit = expected + Math.Abs(expected) * tolPlusPercent / 100;
                    break;
                case ToleranceMode.Relative:
                default:
                    var delta = Math.Abs(expected) * tolPlusMinusPercent / 100;
                    lowerLimit = expected - delta;
                    upperLimit = expected + delta;
                    break;
            }

            // 난수 발생.
            var randomValue = new Random().NextDouble() * (upperLimit - lowerLimit) + lowerLimit;

            // Resolution 설정에 따라 밑의 숫자 반올림.
            randomValue = Utils.ResolutionTruncate(randomValue, TempResolution);

            // 유효숫자만 남기고 반올림.
            if (TempSignificantDigits > 0)
            {
                randomValue = Utils.RoundToSignificantDigits(randomValue, TempSignificantDigits);
            }

            if (randomValue < lowerLimit)
            {
                randomValue = lowerLimit;
            }
            else if (randomValue > upperLimit)
            {
                randomValue = upperLimit;
            }

            return randomValue;
        }

        public static void GetLimitValues(double expected, ToleranceMode tolMode, double tolMinusPercent, double tolPlusPercent, 
            double tolPlusMinusPercent, double tolMinusAbs, double tolPlusAbs, double tolPlusMinusAbs, out double? upperLimit, out double? lowerLimit)
        {
            switch (tolMode)
            {
                case ToleranceMode.Absolute:
                    lowerLimit = expected - tolPlusMinusAbs;
                    upperLimit = expected + tolPlusMinusAbs;
                    break;
                case ToleranceMode.AbsolutePlusMinus:
                    lowerLimit = expected - tolMinusAbs;
                    upperLimit = expected + tolPlusAbs;
                    break;
                case ToleranceMode.Greater:
                    lowerLimit = expected;
                    upperLimit = null;
                    break;
                case ToleranceMode.Less:
                    lowerLimit = null;
                    upperLimit = expected;
                    break;
                case ToleranceMode.RelativePlusMinus:
                    lowerLimit = expected - Math.Abs(expected) * tolMinusPercent / 100;
                    upperLimit = expected + Math.Abs(expected) * tolPlusPercent / 100;
                    break;
                case ToleranceMode.Relative:
                default:
                    double delta = Math.Abs(expected) * tolPlusMinusPercent / 100;
                    lowerLimit = expected - delta;
                    upperLimit = expected + delta;
                    break;
            }
        }

        public static string GetTolerancePlusDesc(ToleranceMode tolMode, double tolPlusPercent, double tolPlusMinusPercent, 
            double tolPlusAbs, double tolPlusMinusAbs, PhysicalUnit unit)
        {
            switch (tolMode)
            {
                case ToleranceMode.Absolute:
                    var valueText = GetPrefixExpression(tolPlusMinusAbs, unit, out MetricPrefix prefix);
                    return $"{valueText}{prefix.GetText()}{unit.GetText()}";
                case ToleranceMode.AbsolutePlusMinus:
                    valueText = GetPrefixExpression(tolPlusAbs, unit, out prefix);
                    return $"{valueText}{prefix.GetText()}{unit.GetText()}";
                case ToleranceMode.Greater:
                    return "+∞";
                case ToleranceMode.Less:
                    return "1";
                case ToleranceMode.RelativePlusMinus:
                    return $"{tolPlusPercent}%";
                case ToleranceMode.Relative:
                default:
                    return $"{tolPlusMinusPercent}%";
            }
        }

        public static string GetToleranceMinusDesc(ToleranceMode tolMode, double tolMinusPercent, double tolPlusMinusPercent, 
            double tolMinusAbs, double tolPlusMinusAbs, PhysicalUnit unit)
        {
            switch (tolMode)
            {
                case ToleranceMode.Absolute:
                    return $"{GetPrefixExpression(tolPlusMinusAbs, unit, out MetricPrefix prefix)}{prefix.GetText()}{unit.GetText()}";
                case ToleranceMode.AbsolutePlusMinus:
                    return $"{GetPrefixExpression(tolMinusAbs, unit, out prefix)}{prefix.GetText()}{unit.GetText()}";
                case ToleranceMode.Greater:
                    return "1";
                case ToleranceMode.Less:
                    return "-∞";
                case ToleranceMode.RelativePlusMinus:
                    return $"{tolMinusPercent}%";
                case ToleranceMode.Relative:
                default:
                    return $"{tolPlusMinusPercent}%";
            }
        }

        /// <summary>
        /// 스텝을 표현하는 256x256 이미지를 리턴.
        /// </summary>
        /// <returns></returns>
        public Image GetStepImage()
        {
            try
            {
                DeviceCategory? devCategory = null;

                if (this is EolAbortStep)
                {
                    return Properties.Resources.abort_256;
                }
                else if (this is EolDummyStep)
                {
                    return Properties.Resources.null_256;
                }
                else if (this is EolElozRelayStep)
                {
                    return Properties.Resources.relay_256;
                }
                else if (this is EolElozStimulusStep)
                {
                    return Properties.Resources.stimulus_256;
                }
                else if (this is EolElozVoltmeterStep)
                {
                    return Properties.Resources.voltmeter_256;
                }
                else if (this is EolPowerStep)
                {
                    devCategory = DeviceCategory.Power;
                }
                else if (this is EolDmmStep)
                {
                    devCategory = DeviceCategory.DMM;
                }
                else if (this is EolOscopeStep)
                {
                    devCategory = DeviceCategory.Oscilloscope;
                }
                else if (this is EolWaveformGeneratorStep)
                {
                    devCategory = DeviceCategory.WaveformGenerator;
                }
                else if (this is EolDioStep)
                {
                    devCategory = DeviceCategory.DIO;
                }
                else if (this is EolCanStep)
                {
                    devCategory = DeviceCategory.CAN;
                }
                else if (this is EolLinStep)
                {
                    devCategory = DeviceCategory.LIN;
                }
                else if (this is EolAmplifierStep)
                {
                    devCategory = DeviceCategory.Amplifier;
                }
                else if (this is EolGloquadEvseStep)
                {
                    devCategory = DeviceCategory.GloquadSECC;
                }
                else if (this is EolSerialPortStep)
                {
                    devCategory = DeviceCategory.SerialPort;
                }
                else if (this is EolAlphaMotionStep)
                {
                    return Properties.Resources.servo_256;
                }
                else if (this is EolMightyZapStep)
                {
                    return Properties.Resources.mighty_zap_256;
                }

                if (devCategory == null)
                {
                    return null;
                }

                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(devCategory ?? DeviceCategory.Power, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.ODA_EX_Series:
                        return Properties.Resources.power_oda_ex_256;
                    case DeviceType.MK_P_Series:
                        return Properties.Resources.power_mk_p_256;
                    case DeviceType.Fluke_8845A_8846A:
                        return Properties.Resources.dmm_fluke_8845a_256;
                    case DeviceType.Keysight_Truevolt_Series:
                        return Properties.Resources.dmm_keysight_34465a_256;
                    case DeviceType.Keysight_InfiniiVision_3000T_X:
                        return Properties.Resources.oscope_keysight_infiniivision3024t_256;
                    case DeviceType.Keysight_EDU33210_Series:
                        return Properties.Resources.wgen_keysight_edu33212a_256;
                    case DeviceType.DIO:
                        return Properties.Resources.microchip_256;
                    case DeviceType.PeakCAN:
                        var canSetting = deviceSetting as CanDeviceSetting;
                        if (canSetting?.ConnectionType == PeakCanDeviceType.PCI)
                        {
                            return Properties.Resources.can_peak_pcie_fd_256;
                        }
                        return Properties.Resources.can_256;
                    case DeviceType.PeakLIN:
                        var linSetting = deviceSetting as LinDeviceSetting;
                        if (linSetting?.HardwareType == PeakLinHardwareType.PLIN_USB)
                        {
                            return Properties.Resources.lin_peak_usb_256;
                        }
                        return Properties.Resources.lin_256;
                    case DeviceType.Keysight_33502A:
                        return Properties.Resources.amp_keysight_33502A_256;
                    case DeviceType.GloquadSECC:
                        return Properties.Resources.gloquad_secc;
                    case DeviceType.SerialPort:
                        return Properties.Resources.serial_port_256;
                }
            }
            catch
            {
            }

            return null;
        }

        /// <summary>
        /// 스텝에서 디바이스를 이용하는 경우 해당 디바이스의 타입을 리턴.
        /// </summary>
        /// <returns></returns>
        public DeviceType? GetDeviceType()
        {
            var category = GetDeviceCategory();
            if (category == null)
            {
                return null;
            }

            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(category ?? DeviceCategory.Power, DeviceName);
            return deviceSetting.DeviceType;
        }

        /// <summary>
        /// 스텝에서 디바이스를 이용하는 경우 해당 디바이스의 카테고리를 리턴.
        /// </summary>
        /// <returns></returns>
        public DeviceCategory? GetDeviceCategory()
        {
            switch (this)
            {
                case EolPowerStep _:
                    return DeviceCategory.Power;
                case EolDmmStep _:
                    return DeviceCategory.DMM;
                case EolOscopeStep _:
                    return DeviceCategory.Oscilloscope;
                case EolWaveformGeneratorStep _:
                    return DeviceCategory.WaveformGenerator;
                case EolDioStep _:
                    return DeviceCategory.DIO;
                case EolCanStep _:
                    return DeviceCategory.CAN;
                case EolLinStep _:
                    return DeviceCategory.LIN;
                case EolAmplifierStep _:
                    return DeviceCategory.Amplifier;
                case EolGloquadEvseStep _:
                    return DeviceCategory.GloquadSECC;
                case EolSerialPortStep _:
                    return DeviceCategory.SerialPort;
                default:
                    return null;
            }
        }

        /// <summary>
        /// Hidden 프로퍼티를 보여주거나 숨긴다.
        /// </summary>
        public virtual void ToggleHiddenProperties()
        {
            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            bool visible = !(browsable ?? false);
            Utils.SetBrowsableAttribute(this, nameof(CP), visible);
            Utils.SetBrowsableAttribute(this, nameof(TempSignificantDigits), visible);
            Utils.SetBrowsableAttribute(this, nameof(TempResolution), visible);
        }

        /// <summary>
        /// <see cref="TestResult.ResultData"/>가 이미지인 경우, 그것을 다른 윈도우로 보여줄지 여부.
        /// </summary>
        /// <returns></returns>
        public virtual bool ShouldShowResultImage()
        {
            return false;
        }

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        //   parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public abstract object Clone();

        /// <summary>
        /// 스텝의 내용을 지정한 스텝에 복사한다.
        /// </summary>
        /// <param name="dest"></param>
        public virtual void CopyTo(EolStep dest)
        {
            dest.Id = Id;
            dest.Enabled = Enabled;
            dest.StepId = StepId;
            dest.Section = Section;
            dest.Name = Name;
            dest.DeviceName = DeviceName;
            dest.Remarks = Remarks;
            dest.RetestMode = RetestMode;
            dest.RetestCount = RetestCount;
            dest.RetestDelay = RetestDelay;
            dest.DelayBefore = DelayBefore;
            dest.DelayAfter = DelayAfter;
            dest.ElozRelayAfter = ElozRelayAfter;
            dest.ResultLogInfo = ResultLogInfo;
            dest.ResultFailLogMessage = ResultFailLogMessage;
            dest.ResultSpecLog = ResultSpecLog;
            dest.IgnoreResult = IgnoreResult;
            dest.InvertResult = InvertResult;
            dest.AbortOnFail = AbortOnFail;
            dest.ExecuteMode = ExecuteMode;
            dest.Variants = Variants;
            dest.CP = CP;
            dest.TempSignificantDigits = TempSignificantDigits;
            dest.TempResolution = TempResolution;
        }

        /// <summary>
        /// 스텝에서 이용할 수 있는 장치들을 리턴한다.
        /// </summary>
        /// <returns></returns>
        public abstract IEnumerable<string> GetDeviceNames();

        /// <summary>
        /// 스텝의 디바이스 설정에 따라, 가능한 테스트 방법들을 리턴한다.
        /// </summary>
        /// <returns></returns>
        public abstract ICollection GetTestModes();

        /// <summary>
        /// 문자열을 파싱하여 Test Mode로 변환한다.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="testMode"></param>
        /// <returns></returns>
        public abstract bool TryParseTestMode(object value, out object testMode);

        /// <summary>
        /// 표준값, 상한 하한 얻기.
        /// </summary>
        /// <param name="nominalValue"></param>
        /// <param name="upperLimit"></param>
        /// <param name="lowerLimit"></param>
        public abstract void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit);

        /// <summary>
        /// 스텝 로그 방식.
        /// </summary>
        public enum LogMode
        {
            /// <summary>
            /// 항상 로깅.
            /// </summary>
            Always,

            /// <summary>
            /// 스텝이 Fail인 경우에만 로깅.
            /// </summary>
            FailOnly,

            /// <summary>
            /// 로깅하지 않음.
            /// </summary>
            Never
        }

        /// <summary>
        /// 다시 테스트하는 방식.
        /// </summary>
        public enum TestRetryMode
        {
            /// <summary>
            /// 실패한 경우 다시 테스트.
            /// </summary>
            OnFail,

            /// <summary>
            /// 다시 테스트하지 않음.
            /// </summary>
            Never
        }

        /// <summary>
        /// 허용오차 방식.
        /// </summary>
        public enum ToleranceMode
        {
            /// <summary>
            /// 값으로 표시, 상한과 하한이 같음.
            /// </summary>
            Absolute,

            /// <summary>
            /// 값으로 표시, 상한과 하한이 다름.
            /// </summary>
            AbsolutePlusMinus,

            /// <summary>
            /// 퍼센트로 표시, 상한과 하한이 같음.
            /// </summary>
            Relative,

            /// <summary>
            /// 퍼센트로 표시, 상한과 하한이 다름.
            /// </summary>
            RelativePlusMinus,

            /// <summary>
            /// 값으로 표시, 상한만 있음.
            /// </summary>
            Less,

            /// <summary>
            /// 값으로 표시, 하한만 있음.
            /// </summary>
            Greater,
        }

        public enum ResultState
        {
            NoState,
            Pass,
            Fail,
            Aborted,
        }

        public enum ResultValueState
        {
            Invalid,
            Good,
            Bad,
            Low,
            High,
        }

        public class TestResult : INotifyPropertyChanged
        {
            /// <summary>
            /// 테스트를 진행한 스텝.
            /// </summary>
            public EolStep Step
            {
                get => _step;
                internal set
                {
                    if (_step != value)
                    {
                        _step = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private EolStep _step;

            /// <summary>
            /// 테스트 시작 시간.
            /// </summary>
            public DateTime StartTime
            {
                get => _startTime;
                internal set
                {
                    if (_startTime != value)
                    {
                        _startTime = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private DateTime _startTime;

            /// <summary>
            /// 테스트 종료 시간.
            /// </summary>
            public DateTime FinishTime
            {
                get => _finishTime;
                internal set
                {
                    if (_finishTime != value)
                    {
                        _finishTime = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private DateTime _finishTime;

            /// <summary>
            /// 테스트하는데 걸린 시간(ms).
            /// </summary>
            public long TotalMilliseconds
            {
                get => _totalMilliseconds;
                internal set
                {
                    if (_totalMilliseconds != value)
                    {
                        _totalMilliseconds = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private long _totalMilliseconds;

            /// <summary>
            /// 테스트 결과.
            /// </summary>
            public ResultState ResultState
            {
                get => _resultState;
                internal set
                {
                    if (_resultState != value)
                    {
                        _resultState = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private ResultState _resultState = ResultState.NoState;

            /// <summary>
            /// 테스트 결과 정보.
            /// </summary>
            public string ResultInfo
            {
                get => _resultInfo;
                internal set
                {
                    if (_resultInfo != value)
                    {
                        _resultInfo = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _resultInfo;

            /// <summary>
            /// 로그에 저장하기 위한 추가 헤더 정보.
            /// </summary>
            public string ResultLogInfo
            {
                get => _resultLogInfo;
                internal set
                {
                    if (_resultLogInfo != value)
                    {
                        _resultLogInfo = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _resultLogInfo;

            /// <summary>
            /// EOL 로그 Item 바로 다음 라인에 추가되는 텍스트.
            /// </summary>
            public string ResultLogBody
            {
                get => _resultLogBody;
                internal set
                {
                    if (_resultLogBody != value)
                    {
                        _resultLogBody = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _resultLogBody;

            /// <summary>
            /// 측정값.
            /// </summary>
            public double? ResultValue
            {
                get => _resultValue;
                internal set
                {
                    if (_resultValue != value)
                    {
                        _resultValue = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private double? _resultValue;

            /// <summary>
            /// 다운로드한 화면 이미지 등의 데이터.
            /// </summary>
            public object ResultData
            {
                get => _resultData;
                internal set
                {
                    if (_resultData != value)
                    {
                        _resultData = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private object _resultData;

            /// <summary>
            /// 측정값 상태.
            /// </summary>
            public ResultValueState ResultValueState
            {
                get => _resultValueState;
                internal set
                {
                    if (_resultValueState != value)
                    {
                        _resultValueState = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private ResultValueState _resultValueState = ResultValueState.Invalid;

            /// <summary>
            /// 측정값 단위.
            /// </summary>
            public PhysicalUnit Unit
            {
                get => _unit;
                internal set
                {
                    if (_unit != value)
                    {
                        _unit = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private PhysicalUnit _unit = PhysicalUnit.None;

            /// <summary>
            /// 로그를 위해 사용됨.
            /// </summary>
            public string IctSectionName
            {
                get => _ictSectionName;
                set
                {
                    if (_ictSectionName != value)
                    {
                        _ictSectionName = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _ictSectionName;

            public string ResultStateDesc => ResultState.GetText();

            /// <summary>
            /// 측정값 + 단위.
            /// </summary>
            public string ResultValueDesc
            {
                get
                {
                    if (ResultValue != null)
                    {
                        return GetPrefixExpression(ResultValue, Unit, out MetricPrefix prefix) + prefix.GetText() + Unit.GetText();
                    }
                    else
                    {
                        return null;
                    }
                }
            }

            public string ResultValueStateDesc => ResultValue == null ? null : ResultValueState.ToString();

            public TestResult(EolStep step)
            {
                Step = step;
            }

            public event PropertyChangedEventHandler PropertyChanged;

            // This method is called by the Set accessor of each property.  
            // The CallerMemberName attribute that is applied to the optional propertyName  
            //   parameter causes the property name of the caller to be substituted as an argument.
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }
    }

    public enum MetricPrefix
    {
        None,
        Pico,
        Nano,
        Micro,
        Milli,
        Kilo,
        Mega,
        Giga,
    }

    public static class MetricPrefixExtensions
    {
        public static string GetText(this MetricPrefix prefix)
        {
            switch (prefix)
            {
                case MetricPrefix.Giga:
                    return "G";
                case MetricPrefix.Mega:
                    return "M";
                case MetricPrefix.Kilo:
                    return "K";
                case MetricPrefix.Milli:
                    return "m";
                case MetricPrefix.Micro:
                    return "μ";
                case MetricPrefix.Nano:
                    return "n";
                case MetricPrefix.Pico:
                    return "p";
                case MetricPrefix.None:
                default:
                    return "";
            }
        }

        public static double GetMultiplier(this MetricPrefix prefix)
        {
            switch (prefix)
            {
                case MetricPrefix.Giga:
                    return 1E9;
                case MetricPrefix.Mega:
                    return 1E6;
                case MetricPrefix.Kilo:
                    return 1E3;
                case MetricPrefix.Milli:
                    return 1E-3;
                case MetricPrefix.Micro:
                    return 1E-6;
                case MetricPrefix.Nano:
                    return 1E-9;
                case MetricPrefix.Pico:
                    return 1E-12;
                case MetricPrefix.None:
                default:
                    return 1;
            }
        }

        public static string[] GetPrefixes() => new string[] { "G", "M", "k", "K", "m", "u", "μ", "n", "p" };

        public static bool TryParse(string unitString, out MetricPrefix prefix)
        {
            switch (unitString)
            {
                case "G":
                    prefix = MetricPrefix.Giga;
                    return true;
                case "M":
                    prefix = MetricPrefix.Mega;
                    return true;
                case "k":
                case "K":
                    prefix = MetricPrefix.Kilo;
                    return true;
                case "m":
                    prefix = MetricPrefix.Milli;
                    return true;
                case "u":
                case "μ":
                    prefix = MetricPrefix.Micro;
                    return true;
                case "n":
                    prefix = MetricPrefix.Nano;
                    return true;
                case "p":
                    prefix = MetricPrefix.Pico;
                    return true;
                default:
                    prefix = MetricPrefix.None;
                    return false;
            }
        }
    }

    [TypeConverter(typeof(DescEnumConverter))]
    public enum PhysicalUnit
    {
        None,
        Watt,
        Volt,
        Ampere,
        Ohm,
        Farad,
        Henry,
        Second,
        Hertz,
        Celsius,
        Newton,
        Percent,
        Degree,
        Decibel,
        RPM,
    }

    public static class PhysicalUnitExtensions
    {
        public static string GetText(this PhysicalUnit unit)
        {
            switch (unit)
            {
                case PhysicalUnit.Ampere:
                    return "A";
                case PhysicalUnit.Celsius:
                    return "°C";
                case PhysicalUnit.Farad:
                    return "F";
                case PhysicalUnit.Henry:
                    return "H";
                case PhysicalUnit.Hertz:
                    return "Hz";
                case PhysicalUnit.Newton:
                    return "N";
                case PhysicalUnit.Ohm:
                    return "Ω";
                case PhysicalUnit.Second:
                    return "s";
                case PhysicalUnit.Volt:
                    return "V";
                case PhysicalUnit.Watt:
                    return "W";
                case PhysicalUnit.Percent:
                    return "%";
                case PhysicalUnit.Degree:
                    return "°";
                case PhysicalUnit.Decibel:
                    return "dB";
                case PhysicalUnit.RPM:
                    return "rpm";
                case PhysicalUnit.None:
                default:
                    return "";
            }
        }

        public static PhysicalUnit ParseText(string text)
        {
            switch (text)
            {
                case "A":
                    return PhysicalUnit.Ampere;
                case "°C":
                    return PhysicalUnit.Celsius;
                case "F":
                    return PhysicalUnit.Farad;
                case "H":
                    return PhysicalUnit.Henry;
                case "Hz":
                    return PhysicalUnit.Hertz;
                case "N":
                    return PhysicalUnit.Newton;
                case "Ω":
                    return PhysicalUnit.Ohm;
                case "s":
                    return PhysicalUnit.Second;
                case "V":
                    return PhysicalUnit.Volt;
                case "W":
                    return PhysicalUnit.Watt;
                case "%":
                    return PhysicalUnit.Percent;
                case "°":
                    return PhysicalUnit.Degree;
                case "dB":
                    return PhysicalUnit.Decibel;
                case "rpm":
                    return PhysicalUnit.RPM;
                default:
                    return PhysicalUnit.None;
            }
        }

        public static PhysicalUnit From(TestFramework.PluginTestCell.TestResults.PhysicalUnit elozPhysicalUnit)
        {
            switch (elozPhysicalUnit)
            {
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Ampere:
                    return PhysicalUnit.Ampere;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Celsius:
                    return PhysicalUnit.Celsius;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Farad:
                    return PhysicalUnit.Farad;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Henry:
                    return PhysicalUnit.Henry;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Hertz:
                    return PhysicalUnit.Hertz;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Newton:
                    return PhysicalUnit.Newton;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Ohm:
                    return PhysicalUnit.Ohm;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Second:
                    return PhysicalUnit.Second;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Volt:
                    return PhysicalUnit.Volt;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.Watt:
                    return PhysicalUnit.Watt;
                case TestFramework.PluginTestCell.TestResults.PhysicalUnit.None:
                default:
                    return PhysicalUnit.None;
            }
        }
    }

    public static class ResultStateExtensions
    {
        public static string GetText(this EolStep.ResultState resultState)
        {
            switch (resultState)
            {
                case EolStep.ResultState.Pass:
                    return "PASS";
                case EolStep.ResultState.Fail:
                    return "FAIL";
                case EolStep.ResultState.Aborted:
                    return "ABORTED";
                case EolStep.ResultState.NoState:
                default:
                    return "NOSTATE";
            }
        }

        public static void GetColors(this EolStep.ResultState resultState, out Color backColor, out Color foreColor)
        {
            switch (resultState)
            {
                case EolStep.ResultState.Pass:
                    backColor = Color.LimeGreen;
                    foreColor = Color.Black;
                    break;
                case EolStep.ResultState.Fail:
                    backColor = Color.Red;
                    foreColor = Color.Black;
                    break;
                case EolStep.ResultState.Aborted:
                    backColor = Color.Orange;
                    foreColor = Color.Black;
                    break;
                case EolStep.ResultState.NoState:
                default:
                    backColor = Color.Silver;
                    foreColor = Color.Black;
                    break;
            }
        }

        public static EolStep.ResultState From(TestFramework.PluginTestCell.TestResults.ResultState elozResultState)
        {
            switch (elozResultState)
            {
                case TestFramework.PluginTestCell.TestResults.ResultState.Aborted:
                    return EolStep.ResultState.Aborted;
                case TestFramework.PluginTestCell.TestResults.ResultState.Fail:
                    return EolStep.ResultState.Fail;
                case TestFramework.PluginTestCell.TestResults.ResultState.Pass:
                    return EolStep.ResultState.Pass;
                case TestFramework.PluginTestCell.TestResults.ResultState.NoState:
                default:
                    return EolStep.ResultState.NoState;
            }
        }
    }

    public static class ResultValueStateExtensions
    {
        public static EolStep.ResultValueState From(TestFramework.PluginTestCell.TestResults.ResultValueState elozResultValueState)
        {
            switch (elozResultValueState)
            {
                case TestFramework.PluginTestCell.TestResults.ResultValueState.Bad:
                    return EolStep.ResultValueState.Bad;
                case TestFramework.PluginTestCell.TestResults.ResultValueState.BadHigh:
                    return EolStep.ResultValueState.High;
                case TestFramework.PluginTestCell.TestResults.ResultValueState.BadLow:
                    return EolStep.ResultValueState.Low;
                case TestFramework.PluginTestCell.TestResults.ResultValueState.Good:
                    return EolStep.ResultValueState.Good;
                case TestFramework.PluginTestCell.TestResults.ResultValueState.Invalid:
                default:
                    return EolStep.ResultValueState.Invalid;
            }
        }
    }

    public enum CmdResponseCheckMode
    {
        None,
        ReadLine,
        ReadLineStartsWith,
        ReadLineContains,
        ReadLineEndsWith,
    }
}
