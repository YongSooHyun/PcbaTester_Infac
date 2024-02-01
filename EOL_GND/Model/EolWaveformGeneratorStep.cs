using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Drawing.Design;
using System.IO;
using System.Linq;
using System.Reflection.Emit;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    public class EolWaveformGeneratorStep : EolStep
    {
        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)), 
            Description("테스트 방법을 설정합니다.")]
        public WaveformGeneratorTestMode TestMethod
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
        private WaveformGeneratorTestMode _testMethod = WaveformGeneratorTestMode.ReadIDN;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.WgStateLocation.Location1), 
            Description("계측기 상태를 저장하거나 불러올 저장 위치를 나타냅니다.")]
        public WaveformGeneratorDevice.WgStateLocation StateLocation
        {
            get => _stateLocation;
            set
            {
                if (_stateLocation != value)
                {
                    _stateLocation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.WgStateLocation _stateLocation = WaveformGeneratorDevice.WgStateLocation.Location1;
    
        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.Channel.CH1), 
            Description("신호를 출력할 채널을 나타냅니다.")]
        public WaveformGeneratorDevice.Channel OutputChannel
        {
            get => _outputChannel;
            set
            {
                if (_outputChannel != value)
                {
                    _outputChannel = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.Channel _outputChannel = WaveformGeneratorDevice.Channel.CH1;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.FunctionWaveform.Sinusoid), 
            Description("출력할 신호 파형을 나타냅니다.")]
        public WaveformGeneratorDevice.FunctionWaveform OutputFunction
        {
            get => _outputFunction;
            set
            {
                if (_outputFunction != value)
                {
                    _outputFunction = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.FunctionWaveform _outputFunction = WaveformGeneratorDevice.FunctionWaveform.Sinusoid;

        #region Pulse Parameters

        [Category(MethodCategory), DisplayName(nameof(PulseDutyCycle) + " [%] [Default = 10]"), Browsable(false), DefaultValue(10.0),
            Description("출력 신호 파형이 Pulse인 경우, 그 Duty Cycle을 퍼센트로 설정합니다. 가능한 값은 0 ~ 100.")]
        public double? PulseDutyCycle
        {
            get => _pulseDutyCycle;
            set
            {
                if (_pulseDutyCycle != value)
                {
                    _pulseDutyCycle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pulseDutyCycle = null;

        [Category(MethodCategory), DisplayName(nameof(PulseLeadingTime) + " [s] [Default = 10n]"), Browsable(false), 
            DefaultValue(10.0E-9), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("출력 신호 파형이 Pulse인 경우, 상승 엣지 타임을 설정합니다. 가능한 값은 8.4ns ~ 1μs.")]
        public double? PulseLeadingTime
        {
            get => _pulseLeadingTime;
            set
            {
                if (_pulseLeadingTime != value)
                {
                    _pulseLeadingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pulseLeadingTime = null;

        [Category(MethodCategory), DisplayName(nameof(PulseTrailingTime) + " [s] [Default = 10n]"), Browsable(false), 
            DefaultValue(10.0E-9), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("출력 신호 파형이 Pulse인 경우, 하강 엣지 타임을 설정합니다. 가능한 값은 8.4ns ~ 1μs.")]
        public double? PulseTrailingTime
        {
            get => _pulseTrailingTime;
            set
            {
                if (_pulseTrailingTime != value)
                {
                    _pulseTrailingTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pulseTrailingTime = null;

        #endregion

        [Category(MethodCategory), DisplayName(nameof(OutputFrequency) + " [Hz] [Default = 1K]"), Browsable(false), 
            DefaultValue(1000.0), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description("출력 주파수를 나타냅니다. PWM의 경우에는 반송파 주파수를 나타냅니다.")]
        public double? OutputFrequency
        {
            get => _outputFrequency;
            set
            {
                if (_outputFrequency != value)
                {
                    _outputFrequency = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _outputFrequency = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.VoltageUnit.Vpp), 
            DisplayName(nameof(OutputVoltageUnit) + " [Default = " + nameof(WaveformGeneratorDevice.VoltageUnit.Vpp) + "]"),
            Description("출력 전압 단위를 나타냅니다.")]
        public WaveformGeneratorDevice.VoltageUnit? OutputVoltageUnit
        {
            get => _outputVoltageUnit;
            set
            {
                if (_outputVoltageUnit != value)
                {
                    _outputVoltageUnit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.VoltageUnit? _outputVoltageUnit = null;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(0.1),
            Description("출력 전압을 나타냅니다. PWM의 경우에는 반송파 진폭을 나타냅니다.")]
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
        private double _outputVoltage = 0.1;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(0.0), 
            DisplayName(nameof(OutputDCVoltageOffset) + " [Default = 0]"),
            Description("출력 전압의 DC offset 전압을 나타냅니다.")]
        public double? OutputDCVoltageOffset
        {
            get => _outputDCVoltageOffset;
            set
            {
                if (_outputDCVoltageOffset != value)
                {
                    _outputDCVoltageOffset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        public double? _outputDCVoltageOffset = null;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(50.0),
            DisplayName(nameof(OutputImpedance) + " [Ω] [Default = 50]"),
            Description("예상 출력 터미네이션을 설정합니다. 출력에 연결된 로드 임피던스와 같아야 합니다. 1Ω ~ 10kΩ.")]
        public double? OutputImpedance
        {
            get => _outputImpedance;
            set
            {
                if (_outputImpedance != value)
                {
                    // 10kΩ 보다 큰 임피던스(INFINITY)는 1M 로 표시.
                    if (value > 10_000)
                    {
                        value = 1_000_000;
                    }
                    _outputImpedance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _outputImpedance;

        [Category(MethodCategory), DisplayName(nameof(PWMPulseWidthDeviation) + " [s] [Default = 10μ]"), 
            Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(1.0E-5), 
            Description("PWM 펄스 폭 편차로, 반송 펄스파의 펄스폭에 대한 ± 편차입니다.")]
        public double? PWMPulseWidthDeviation
        {
            get => _pwmPulseWidthDeviation;
            set
            {
                if (_pwmPulseWidthDeviation != value)
                {
                    _pwmPulseWidthDeviation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pwmPulseWidthDeviation = null;

        [Category(MethodCategory), DisplayName(nameof(PWMDutyCycleDeviation) + " [%] [Default = 1]"), Browsable(false), DefaultValue(1.0), 
            Description("Sets duty cycle deviation in percent of period. This is the peak variation in duty cycle from the underlying " +
            "pulse waveform. For example, if duty cycle is 10 % and duty cycle deviation is 5 %, the duty cycle of the modulated " +
            "waveform will vary from 5 % to 15 %.")]
        public double? PWMDutyCycleDeviation
        {
            get => _pwmDutyCycleDeviation;
            set
            {
                if (_pwmDutyCycleDeviation != value)
                {
                    _pwmDutyCycleDeviation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pwmDutyCycleDeviation = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.SignalSource.Internal),
            DisplayName(nameof(PWMModulationSource) + " [Default = " + nameof(WaveformGeneratorDevice.SignalSource.Internal) + "]"),
            Description("PWM 변조 신호 소스를 나타냅니다.")]
        public WaveformGeneratorDevice.SignalSource? PWMModulationSource
        {
            get => _pwmModulationSource;
            set
            {
                if (_pwmModulationSource != value)
                {
                    _pwmModulationSource = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        public WaveformGeneratorDevice.SignalSource? _pwmModulationSource = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.InternalFuncWaveform.Sinusoid),
            DisplayName(nameof(PWMModulationWaveform) + " [Default = " + nameof(WaveformGeneratorDevice.InternalFuncWaveform.Sinusoid) + "]"),
            Description("PWM 내부 변조 신호를 이용하는 경우, 그 파형을 나타냅니다.")]
        public WaveformGeneratorDevice.InternalFuncWaveform? PWMModulationWaveform
        {
            get => _pwmModulationWaveform;
            set
            {
                if (_pwmModulationWaveform != value)
                {
                    _pwmModulationWaveform = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.InternalFuncWaveform? _pwmModulationWaveform = null;

        [Category(MethodCategory), DisplayName(nameof(PWMModulationFrequency) + " [Hz] [Default = 10]"), 
            Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(10.0), 
            Description("PWM 내부 변조 신호를 이용하는 경우, 그 주파수를 나타냅니다.")]
        public double? PWMModulationFrequency
        {
            get => _pwmModulationFrequency;
            set
            {
                if (_pwmModulationFrequency != value)
                {
                    _pwmModulationFrequency = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _pwmModulationFrequency = null;

        [Category(MethodCategory), DisplayName(nameof(PhaseAngle) + " [°] [Default = 0]"),
            Browsable(false), DefaultValue(0.0),
            Description("출력 파형의 위상을 -360 ~ +360 도로 설정합니다.")]
        public double? PhaseAngle
        {
            get => _phaseAngle;
            set
            {
                if (_phaseAngle != value)
                {
                    _phaseAngle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _phaseAngle;

        [Category(MethodCategory), DisplayName(nameof(DSSC) + " [Default = False]"), 
            Browsable(false), DefaultValue(false), 
            Description("Double Sideband Suppressed Carrier (DSSC or DSB-SC, 양 측파대 억압 반송파) (True) or AM modulated carrier with sidebands (False).")]
        public bool? DSSC
        {
            get => _dssc;
            set
            {
                if (_dssc != value)
                {
                    _dssc = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _dssc;

        [Category(MethodCategory), DisplayName(nameof(ModulationDepth) + " [%] [Default = 100]"), 
            Browsable(false), DefaultValue(100.0), 
            Description("0 ~ 120%. Even at greater than 100% depth, the instrument will not exceed ±5 V peak on the output (into a 50 Ω load). " +
            "To achieve modulation depth greater than 100%, output carrier amplitude may be reduced.")]
        public double? ModulationDepth
        {
            get => _modulationDepth;
            set
            {
                if (_modulationDepth != value)
                {
                    _modulationDepth = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _modulationDepth;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.SignalSource.Internal),
            DisplayName(nameof(AMModulationSource) + " [Default = " + nameof(WaveformGeneratorDevice.SignalSource.Internal) + "]"),
            Description("변조 신호 소스를 나타냅니다.")]
        public WaveformGeneratorDevice.SignalSource? AMModulationSource
        {
            get => _amModulationSource;
            set
            {
                if (_amModulationSource != value)
                {
                    _amModulationSource = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        public WaveformGeneratorDevice.SignalSource? _amModulationSource = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(WaveformGeneratorDevice.InternalFuncWaveform.Sinusoid),
            DisplayName(nameof(AMModulationWaveform) + " [Default = " + nameof(WaveformGeneratorDevice.InternalFuncWaveform.Sinusoid) + "]"),
            Description("내부 변조 신호를 이용하는 경우, 그 파형을 나타냅니다.")]
        public WaveformGeneratorDevice.InternalFuncWaveform? AMModulationWaveform
        {
            get => _amModulationWaveform;
            set
            {
                if (_amModulationWaveform != value)
                {
                    _amModulationWaveform = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private WaveformGeneratorDevice.InternalFuncWaveform? _amModulationWaveform = null;

        [Category(MethodCategory), DisplayName(nameof(AMModulationFrequency) + " [Hz] [Default = 100]"),
            Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(100.0),
            Description("내부 변조 신호를 이용하는 경우, 그 주파수를 나타냅니다.")]
        public double? AMModulationFrequency
        {
            get => _amModulationFrequency;
            set
            {
                if (_amModulationFrequency != value)
                {
                    _amModulationFrequency = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _amModulationFrequency = null;

        public enum ImageType
        {
            BMP,
        }

        [Category(MethodCategory), Browsable(false), DefaultValue(ImageType.BMP), 
            DisplayName(nameof(DownloadImageFormat) + " [Default = " + nameof(ImageType.BMP) + "]"),
            Description("다운로드할 이미지 형식을 설정합니다.")]
        public ImageType? DownloadImageFormat
        {
            get => _downloadImageFormat;
            set
            {
                if (_downloadImageFormat != value)
                {
                    _downloadImageFormat = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ImageType? _downloadImageFormat;

        [Category(MethodCategory), Browsable(false), Editor(typeof(ImageFileNameEditor), typeof(UITypeEditor)),
            Description("다운로드한 이미지를 저장할 파일 위치를 지정합니다. 이 값을 비워두면 미리 설정된 위치와 이름으로 저장됩니다.")]
        public string DownloadFile
        {
            get => _downloadFile;
            set
            {
                if (_downloadFile != value)
                {
                    _downloadFile = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _downloadFile = null;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)), 
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)), 
            Description("디바이스의 1번 채널 High 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel1High
        {
            get => _testChannel1High;
            set
            {
                if (_testChannel1High != value)
                {
                    if (_testChannel1High != null)
                    {
                        _testChannel1High.ListChanged -= _testChannel1High_ListChanged;
                    }
                    _testChannel1High = value;
                    if (_testChannel1High != null)
                    {
                        _testChannel1High = new BindingList<int>(_testChannel1High.Distinct().ToList());
                        _testChannel1High.ListChanged += _testChannel1High_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel1High;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)), 
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)), 
            Description("디바이스의 1번 채널 Low 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel1Low
        {
            get => _testChannel1Low;
            set
            {
                if (_testChannel1Low != value)
                {
                    if (_testChannel1Low != null)
                    {
                        _testChannel1Low.ListChanged -= _testChannel1Low_ListChanged;
                    }
                    _testChannel1Low = value;
                    if (_testChannel1Low != null)
                    {
                        _testChannel1Low = new BindingList<int>(_testChannel1Low.Distinct().ToList());
                        _testChannel1Low.ListChanged += _testChannel1Low_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel1Low;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)), 
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)), 
            Description("디바이스의 2번 채널 High 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel2High
        {
            get => _testChannel2High;
            set
            {
                if (_testChannel2High != value)
                {
                    if (_testChannel2High != null)
                    {
                        _testChannel2High.ListChanged -= _testChannel2High_ListChanged;
                    }
                    _testChannel2High = value;
                    if (_testChannel2High != null)
                    {
                        _testChannel2High = new BindingList<int>(_testChannel2High.Distinct().ToList());
                        _testChannel2High.ListChanged += _testChannel2High_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel2High;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)), 
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)), 
            Description("디바이스의 2번 채널 Low 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel2Low
        {
            get => _testChannel2Low;
            set
            {
                if (_testChannel2Low != value)
                {
                    if (_testChannel2Low != null)
                    {
                        _testChannel2Low.ListChanged -= _testChannel2Low_ListChanged;
                    }
                    _testChannel2Low = value;
                    if (_testChannel2Low != null)
                    {
                        _testChannel2Low = new BindingList<int>(_testChannel2Low.Distinct().ToList());
                        _testChannel2Low.ListChanged += _testChannel2Low_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel2Low;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(ParameterNameConverter)),
            Description("최대/최소 값을 출력할 파라미터 이름을 나타냅니다.")]
        public string ParameterName
        {
            get => _parameterName;
            set
            {
                if (_parameterName != value)
                {
                    _parameterName = value;
                    bool browsable = _parameterName == nameof(OutputFrequency);
                    Utils.SetBrowsableAttribute(this, nameof(OutputFunction), browsable);
                    NotifyPropertyChanged();
                }
            }
        }
        private string _parameterName;

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

        internal override string CategoryName => StepCategory.WaveformGenerator.GetText();
        public override string TestModeDesc => TestMethod.ToString();
        public override string ParameterDesc
        {
            get
            {
                const string propertyDelimiter = ":";
                switch (TestMethod)
                {
                    case WaveformGeneratorTestMode.Save:
                    case WaveformGeneratorTestMode.Recall:
                        return $"Location={(int)StateLocation}";
                    case WaveformGeneratorTestMode.PWM_ON:
                        var valueText = GetPrefixExpression(OutputFrequency, PhysicalUnit.Hertz, out MetricPrefix prefix);
                        string carrierFrequency = valueText + prefix.GetText() + PhysicalUnit.Hertz.GetText();
                        valueText = GetPrefixExpression(OutputVoltage, PhysicalUnit.Volt, out prefix);
                        string carrierVoltage = valueText + prefix.GetText() + PhysicalUnit.Volt.GetText();
                        string modulationDesc = PWMModulationSource.ToString();
                        if (PWMModulationSource == WaveformGeneratorDevice.SignalSource.Internal)
                        {
                            valueText = GetPrefixExpression(PWMModulationFrequency, PhysicalUnit.Hertz, out prefix);
                            modulationDesc += $"{propertyDelimiter}{PWMModulationWaveform}{propertyDelimiter}" +
                                $"{valueText}{prefix.GetText()}{PhysicalUnit.Hertz.GetText()}";
                        }
                        return $"CH{(int)OutputChannel}, Carrier={carrierFrequency}{propertyDelimiter}{carrierVoltage}, Modulation={modulationDesc}";
                    case WaveformGeneratorTestMode.Waveform_ON:
                        valueText = GetPrefixExpression(OutputFrequency, PhysicalUnit.Hertz, out prefix);
                        string frequency = valueText + prefix.GetText() + PhysicalUnit.Hertz.GetText();
                        valueText = GetPrefixExpression(OutputVoltage, PhysicalUnit.Volt, out prefix);
                        string voltage = valueText + prefix.GetText() + OutputVoltageUnit;
                        return $"CH{(int)OutputChannel}, {frequency}, {voltage}";
                    case WaveformGeneratorTestMode.PWM_OFF:
                    case WaveformGeneratorTestMode.Output_OFF:
                        return $"CH{(int)OutputChannel}";
                    case WaveformGeneratorTestMode.ReadIDN:
                    case WaveformGeneratorTestMode.Reset:
                    default:
                        return "";
                }
            }
        }
        public override string ExpectedValueDesc => "";
        public override string TolerancePlusDesc => "";
        public override string ToleranceMinusDesc => "";

        public override List<int> AllTestChannels
        {
            get
            {
                switch (TestMethod)
                {
                    case WaveformGeneratorTestMode.PWM_ON:
                    case WaveformGeneratorTestMode.Waveform_ON:
                        var channels = new List<int>();
                        if (TestChannel1High != null)
                        {
                            channels.AddRange(TestChannel1High);
                        }
                        if (TestChannel1Low != null)
                        {
                            channels.AddRange(TestChannel1Low);
                        }
                        if (TestChannel2High != null)
                        {
                            channels.AddRange(TestChannel2High);
                        }
                        if (TestChannel2Low != null)
                        {
                            channels.AddRange(TestChannel2Low);
                        }
                        return channels;
                    default:
                        return null;
                }
            }
        }

        /// <summary>
        /// XML Serialize를 위해서는 파라미터 없는 생성자 필요.
        /// </summary>
        private EolWaveformGeneratorStep()
        {
            Name = StepCategory.WaveformGenerator.GetText();
        }

        public EolWaveformGeneratorStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return WaveformGeneratorDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetWaveformGeneratorSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.WaveformGenerator, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.Keysight_EDU33210_Series:
                        return new object[]
                        {
                            WaveformGeneratorTestMode.ReadIDN,
                            WaveformGeneratorTestMode.Reset,
                            WaveformGeneratorTestMode.Save,
                            WaveformGeneratorTestMode.Recall,
                            WaveformGeneratorTestMode.PWM_ON,
                            WaveformGeneratorTestMode.PWM_OFF,
                            WaveformGeneratorTestMode.Waveform_ON,
                            WaveformGeneratorTestMode.AM_ON,
                            WaveformGeneratorTestMode.Output_OFF,
                            WaveformGeneratorTestMode.Synchronize,
                            WaveformGeneratorTestMode.Download,
                            WaveformGeneratorTestMode.ReadMinValue,
                            WaveformGeneratorTestMode.ReadMaxValue,
                            WaveformGeneratorTestMode.RunCommandLine,
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
                if (Enum.TryParse(stringValue, out WaveformGeneratorTestMode parsed))
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
            switch (TestMethod)
            {
                case WaveformGeneratorTestMode.PWM_ON:
                case WaveformGeneratorTestMode.Waveform_ON:
                case WaveformGeneratorTestMode.AM_ON:
                    if (setting is WaveformGeneratorDeviceSetting wgdSetting)
                    {
                        var args = new List<IEnumerable<int>>();

                        // CH1.
                        if (OutputChannel == WaveformGeneratorDevice.Channel.CH1)
                        {
                            // CH1 High channels.
                            if (wgdSetting.Channel1High > 0 && TestChannel1High?.Count > 0)
                            {
                                args.Add(TestChannel1High.Concat(new int[] { wgdSetting.Channel1High }));
                            }

                            // CH1 Low channels.
                            if (wgdSetting.Channel1Low > 0 && TestChannel1Low?.Count > 0)
                            {
                                args.Add(TestChannel1Low.Concat(new int[] { wgdSetting.Channel1Low }));
                            }
                        }

                        // CH2.
                        if (OutputChannel == WaveformGeneratorDevice.Channel.CH2)
                        {
                            // CH2 High channels.
                            if (wgdSetting.Channel2High > 0 && TestChannel2High?.Count > 0)
                            {
                                args.Add(TestChannel2High.Concat(new int[] { wgdSetting.Channel2High }));
                            }

                            // CH2 Low channels.
                            if (wgdSetting.Channel2Low > 0 && TestChannel2Low?.Count > 0)
                            {
                                args.Add(TestChannel2Low.Concat(new int[] { wgdSetting.Channel2Low }));
                            }
                        }

                        // Relay ON.
                        ElozDevice.SharedInstance.ConnectChannels(elozTestSet, args.ToArray());
                    }
                    break;
                default:
                    break;
            }
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

            var generator = device as WaveformGeneratorDevice;
            if (generator == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case WaveformGeneratorTestMode.ReadIDN:
                    result.ResultInfo = generator.ReadIDN(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Reset:
                    generator.Reset(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Save:
                    generator.Save(StateLocation, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Recall:
                    generator.Recall(StateLocation, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.PWM_ON:
                    generator.PwmOn(OutputChannel, OutputFrequency, OutputVoltageUnit, OutputVoltage, OutputDCVoltageOffset, PhaseAngle, 
                        PulseDutyCycle, PulseLeadingTime, PulseTrailingTime, PWMPulseWidthDeviation, PWMDutyCycleDeviation, PWMModulationSource, 
                        PWMModulationWaveform, PWMModulationFrequency, OutputImpedance, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.PWM_OFF:
                    generator.PwmOff(OutputChannel, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Waveform_ON:
                    generator.WaveformOn(OutputChannel, OutputFunction, PulseDutyCycle, PulseLeadingTime, PulseTrailingTime, 
                        OutputFrequency, OutputVoltageUnit, OutputVoltage, OutputDCVoltageOffset, PhaseAngle, OutputImpedance, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Output_OFF:
                    generator.OutputOff(OutputChannel, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.AM_ON:
                    generator.AmplitudeModulation(OutputChannel, OutputFunction, OutputFrequency, OutputVoltageUnit, OutputVoltage, 
                        OutputDCVoltageOffset, PhaseAngle, DSSC, AMModulationSource, AMModulationWaveform, AMModulationFrequency, ModulationDepth, 
                        OutputImpedance, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Synchronize:
                    generator.Synchronize(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.Download:
                    var imageData = generator.Download(DownloadImageFormat, token);

                    // 다운로드한 이미지 보관.
                    string downloadFile;
                    string filePath = DownloadFile?.Trim();
                    if (!string.IsNullOrWhiteSpace(filePath))
                    {
                        downloadFile = filePath;
                        Directory.CreateDirectory(Path.GetDirectoryName(filePath));
                    }
                    else
                    {
                        downloadFile = "D:\\ElozPlugin\\WaveformGeneratorScreen\\" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".bmp"; ;
                        Directory.CreateDirectory(Path.GetDirectoryName(downloadFile));
                    }
                    File.WriteAllBytes(downloadFile, imageData);

                    result.ResultInfo = downloadFile;
                    result.ResultState = ResultState.Pass;
                    result.ResultData = Image.FromStream(new MemoryStream(imageData));
                    break;
                case WaveformGeneratorTestMode.ReadMinValue:
                    result.ResultValue = generator.GetMinValue(this, ParameterName, token) as double?;
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.ReadMaxValue:
                    result.ResultValue = generator.GetMaxValue(this, ParameterName, token) as double?;
                    result.ResultState = ResultState.Pass;
                    break;
                case WaveformGeneratorTestMode.RunCommandLine:
                    var response = generator.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, 2000, token);
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
                    throw new NotSupportedException($"디바이스 {DeviceName} 에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        /// <summary>
        /// MIN, MAX 값을 얻을 수 있는 파라미터 목록을 리턴한다.
        /// </summary>
        /// <returns></returns>
        public List<string> GetParameterNames()
        {
            var names = new List<string>();
            names.Add(nameof(PulseDutyCycle));
            names.Add(nameof(PulseLeadingTime));
            names.Add(nameof(PulseTrailingTime));
            names.Add(nameof(OutputFrequency));
            names.Add(nameof(OutputVoltage));
            names.Add(nameof(OutputDCVoltageOffset));
            names.Add(nameof(OutputImpedance));
            names.Add(nameof(PWMPulseWidthDeviation));
            names.Add(nameof(PWMDutyCycleDeviation));
            names.Add(nameof(PWMModulationFrequency));
            names.Add(nameof(AMModulationFrequency));
            names.Add(nameof(ModulationDepth));
            return names;
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            Utils.SetBrowsableAttribute(this, nameof(StateLocation), TestMethod == WaveformGeneratorTestMode.Save || TestMethod == WaveformGeneratorTestMode.Recall);
            bool showOutputChannel = TestMethod == WaveformGeneratorTestMode.Waveform_ON || TestMethod == WaveformGeneratorTestMode.PWM_ON
                || TestMethod == WaveformGeneratorTestMode.Output_OFF || TestMethod == WaveformGeneratorTestMode.PWM_OFF
                || TestMethod == WaveformGeneratorTestMode.AM_ON;
            Utils.SetBrowsableAttribute(this, nameof(OutputChannel), showOutputChannel);
            bool showOutputFunction;
            bool showParameterName;
            if (TestMethod == WaveformGeneratorTestMode.ReadMaxValue || TestMethod == WaveformGeneratorTestMode.ReadMinValue)
            {
                showOutputFunction = ParameterName == nameof(OutputFrequency);
                showParameterName = true;
            }
            else
            {
                showOutputFunction = TestMethod == WaveformGeneratorTestMode.Waveform_ON || TestMethod == WaveformGeneratorTestMode.AM_ON;
                showParameterName = false;
            }
            Utils.SetBrowsableAttribute(this, nameof(OutputFunction), showOutputFunction);
            bool showPulseParams = TestMethod == WaveformGeneratorTestMode.Waveform_ON && OutputFunction == WaveformGeneratorDevice.FunctionWaveform.Pulse ||
                TestMethod == WaveformGeneratorTestMode.PWM_ON;
            Utils.SetBrowsableAttribute(this, nameof(PulseDutyCycle), showPulseParams);
            Utils.SetBrowsableAttribute(this, nameof(PulseLeadingTime), showPulseParams);
            Utils.SetBrowsableAttribute(this, nameof(PulseTrailingTime), showPulseParams);
            bool showOutputParams = TestMethod == WaveformGeneratorTestMode.Waveform_ON || TestMethod == WaveformGeneratorTestMode.PWM_ON || 
                TestMethod == WaveformGeneratorTestMode.AM_ON;
            Utils.SetBrowsableAttribute(this, nameof(OutputFrequency), showOutputParams);
            Utils.SetBrowsableAttribute(this, nameof(OutputVoltageUnit), showOutputParams);
            Utils.SetBrowsableAttribute(this, nameof(OutputVoltage), showOutputParams);
            Utils.SetBrowsableAttribute(this, nameof(OutputDCVoltageOffset), showOutputParams);
            Utils.SetBrowsableAttribute(this, nameof(OutputImpedance), showOutputParams);
            Utils.SetBrowsableAttribute(this, nameof(PWMPulseWidthDeviation), TestMethod == WaveformGeneratorTestMode.PWM_ON);
            Utils.SetBrowsableAttribute(this, nameof(PWMDutyCycleDeviation), TestMethod == WaveformGeneratorTestMode.PWM_ON);
            Utils.SetBrowsableAttribute(this, nameof(PWMModulationSource), TestMethod == WaveformGeneratorTestMode.PWM_ON);
            bool isInternalSource = TestMethod == WaveformGeneratorTestMode.PWM_ON && PWMModulationSource == WaveformGeneratorDevice.SignalSource.Internal;
            Utils.SetBrowsableAttribute(this, nameof(PWMModulationWaveform), isInternalSource);
            Utils.SetBrowsableAttribute(this, nameof(PWMModulationFrequency), isInternalSource);
            Utils.SetBrowsableAttribute(this, nameof(PhaseAngle), showOutputParams);

            bool isAmMode = TestMethod == WaveformGeneratorTestMode.AM_ON;
            Utils.SetBrowsableAttribute(this, nameof(DSSC), isAmMode);
            Utils.SetBrowsableAttribute(this, nameof(ModulationDepth), isAmMode);
            Utils.SetBrowsableAttribute(this, nameof(AMModulationSource), isAmMode);
            bool isAmInternal = TestMethod == WaveformGeneratorTestMode.AM_ON && AMModulationSource == WaveformGeneratorDevice.SignalSource.Internal;
            Utils.SetBrowsableAttribute(this, nameof(AMModulationWaveform), isAmInternal);
            Utils.SetBrowsableAttribute(this, nameof(AMModulationFrequency), isAmInternal);
            Utils.SetBrowsableAttribute(this, nameof(DownloadImageFormat), TestMethod == WaveformGeneratorTestMode.Download);
            Utils.SetBrowsableAttribute(this, nameof(DownloadFile), TestMethod == WaveformGeneratorTestMode.Download);

            bool showChannel1 = showOutputParams && OutputChannel == WaveformGeneratorDevice.Channel.CH1;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel1High), showChannel1);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel1Low), showChannel1);

            bool showChannel2 = showOutputParams && OutputChannel == WaveformGeneratorDevice.Channel.CH2;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel2High), showChannel2);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel2Low), showChannel2);

            Utils.SetBrowsableAttribute(this, nameof(ParameterName), showParameterName);

            bool runCmdLine = TestMethod == WaveformGeneratorTestMode.RunCommandLine;
            Utils.SetBrowsableAttribute(this, nameof(CommandLine), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), runCmdLine);
            bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
            Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), runCmdLine && compareResponse);
            Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), runCmdLine && compareResponse);
        }

        protected override void UpdateToleranceAttributes()
        {
        }

        public override object Clone()
        {
            var newStep = new EolWaveformGeneratorStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolWaveformGeneratorStep destStep)
            {
                destStep.TestMethod = TestMethod;
                destStep.OutputChannel = OutputChannel;
                destStep.OutputFunction = OutputFunction;
                destStep.PulseDutyCycle = PulseDutyCycle;
                destStep.PulseLeadingTime = PulseLeadingTime;
                destStep.PulseTrailingTime = PulseTrailingTime;
                destStep.OutputFrequency = OutputFrequency;
                destStep.OutputVoltageUnit = OutputVoltageUnit;
                destStep.OutputVoltage = OutputVoltage;
                destStep.OutputDCVoltageOffset = OutputDCVoltageOffset;
                destStep.OutputImpedance = OutputImpedance;
                destStep.PWMPulseWidthDeviation = PWMPulseWidthDeviation;
                destStep.PWMDutyCycleDeviation = PWMDutyCycleDeviation;
                destStep.PWMModulationSource = PWMModulationSource;
                destStep.PWMModulationWaveform = PWMModulationWaveform;
                destStep.PWMModulationFrequency = PWMModulationFrequency;
                destStep.PhaseAngle = PhaseAngle;
                destStep.DSSC = DSSC;
                destStep.ModulationDepth = ModulationDepth;
                destStep.AMModulationSource = AMModulationSource;
                destStep.AMModulationWaveform = AMModulationWaveform;
                destStep.AMModulationFrequency = AMModulationFrequency;
                destStep.DownloadImageFormat = DownloadImageFormat;
                destStep.DownloadFile = DownloadFile;

                if (TestChannel1High != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel1High)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel1High = channels;
                }
                if (TestChannel1Low != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel1Low)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel1Low = channels;
                }
                if (TestChannel2High != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel2High)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel2High = channels;
                }
                if (TestChannel2Low != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel2Low)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel2Low = channels;
                }

                destStep.ParameterName = ParameterName;
                destStep.CommandLine = CommandLine;
                destStep.ResponseCheckMethod = ResponseCheckMethod;
                destStep.ResponsePattern = ResponsePattern;
                destStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;
            }

            dest.UpdateBrowsableAttributes();
        }

        private void _testChannel1High_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel1High));
        }
        private void _testChannel1Low_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel1Low));
        }
        private void _testChannel2High_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel2High));
        }
        private void _testChannel2Low_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel2Low));
        }

        /// <summary>
        /// 파형 발생기 기능 리스트.
        /// </summary>
        public enum WaveformGeneratorTestMode
        {
            ReadIDN,
            Recall,
            Save,
            Reset,
            PWM_ON,
            PWM_OFF,
            Waveform_ON,
            Output_OFF,
            AM_ON,
            Synchronize,
            Download,
            ReadMinValue,
            ReadMaxValue,
            RunCommandLine,
        }
    }
}
