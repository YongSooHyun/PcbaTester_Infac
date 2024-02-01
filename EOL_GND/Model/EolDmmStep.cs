using BrightIdeasSoftware;
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
    public class EolDmmStep : EolStep
    {
        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)), 
            Description("테스트 방법을 설정합니다.")]
        public DmmTestMode TestMethod
        {
            get => _testMethod;
            set
            {
                if (_testMethod != value)
                {
                    _testMethod = value;
                    if (_testMethod == DmmTestMode.OpenTest || TestMethod == DmmTestMode.ShortTest)
                    {
                        Function = DmmDevice.DmmFunction.Resistance_2_Wire;
                        if (_testMethod == DmmTestMode.OpenTest)
                        {
                            Tolerance = ToleranceMode.Greater;
                        }
                        else
                        {
                            Tolerance = ToleranceMode.Less;
                        }
                    }
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private DmmTestMode _testMethod = DmmTestMode.ReadIDN;

        [Category(MethodCategory), Browsable(false), DefaultValue(DmmDevice.DmmFunction.Voltage_DC), 
            Description("측정 기능을 설정합니다.")]
        public DmmDevice.DmmFunction Function
        {
            get => _function;
            set
            {
                if (_function != value)
                {
                    _function = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private DmmDevice.DmmFunction _function = DmmDevice.DmmFunction.Voltage_DC;

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
            DefaultValue(0.0), Description("측정값에서 뺄 옵셋을 설정합니다.")]
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

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("측정 기능의 상한과 하한 사이의 범위입니다. 주파수와 주기 측정 기능에 대해서 이 값은 주파수와 주기의 범위가 아니라 입력신호의 전압 범위를 가리킵니다.\r\n" +
            " - 주파수 측정을 위해 계측기는 3Hz ~ 300KHz 사이의 모든 입력신호에 대해 하나의 범위를 사용합니다. 입력신호가 없으면 주파수 측정값은 0 입니다.\r\n" +
            " - 주기 측정을 위해 계측기는 0.33s ~ 3.3μs 사이의 모든 입력신호에 대해 하나의 범위를 사용합니다. 입력신호가 없으면 주기 측정값은 0 입니다.\r\n" +
            " - 온도, 다이오드, Continuity 측정에 대해 범위는 고정됩니다.")]
        public double? Range
        {
            get => _range;
            set
            {
                if (_range != value)
                {
                    _range = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _range = null;

        [Category(MethodCategory), DisplayName(nameof(RTDType) + " [Default = " + nameof(DmmDevice.RtdType.PT100_385) + "]"), 
            Browsable(false), DefaultValue(DmmDevice.RtdType.PT100_385), 
            Description("온도 측정 기능에 대해 RTD(Resistance Temperature Detector) 타입을 설정합니다.\r\n" +
            " • PT100_385 = PT100 385 (R0 to 100Ω and Alpha to 0.00385055)\r\n" +
            " • PT100_392 = PT100 392 (R0 to 100Ω and Alpha to 0.00391600)")]
        public DmmDevice.RtdType? RTDType
        {
            get => _rtdType;
            set
            {
                if (_rtdType != value)
                {
                    _rtdType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private DmmDevice.RtdType? _rtdType = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(false), 
            Description("True이면 다이오드 전류를 0.1mA로, False이면 다이오드 전류를 1mA로 설정합니다.")]
        public bool DiodeLowCurrent
        {
            get => _diodeLowCurrent;
            set
            {
                if (_diodeLowCurrent != value)
                {
                    _diodeLowCurrent = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _diodeLowCurrent = false;

        [Category(MethodCategory), Browsable(false), DefaultValue(false), 
            Description("true이면 다이오드 전압을 10 V로, false이면 다이오드 전압을 5 V로 설정합니다.")]
        public bool DiodeHighVoltage
        {
            get => _diodeHighVoltage;
            set
            {
                if (_diodeHighVoltage != value)
                {
                    _diodeHighVoltage = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _diodeHighVoltage = false;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("측정 분해능으로, 측정 기능과 동일한 단위로 나타냅니다. 예를 들어, AC/DC 전압 측정 기능에 대해서는 V(Volt)로 나타냅니다.\r\n" +
            " - AC 전압 측정에 대한 분해능은 6½ digits 로 고정됩니다.\r\n" +
            " - 온도, 다이오드, Continuity 측정에 대해 분해능은 5½ digits 로 고정됩니다.")]
        public double? Resolution
        {
            get => _resolution;
            set
            {
                if (_resolution != value)
                {
                    _resolution = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _resolution = null;

        [Category(MethodCategory), Browsable(false),
            DisplayName(nameof(Aperture) + " [s] [Default = 0.1]"),
            TypeConverter(typeof(PhysicalValueStaticConverter)), DefaultValue(0.1),
            Description("Integration 시간을 초 단위로 설정합니다. Keysight Truevolt Series인 경우 34465A/70A 만 지원합니다.\r\n" +
            " • 0.01s, 0.1s, 1s - Fluke 8845A/8846A\r\n" +
            " • 200μs ~ 1s (2μs resolution) - Keysight 34465A/70A without the DIG option\r\n" +
            " • 20μs ~ 1s (2μs resolution) - Keysight 34465A/70A with the DIG option")]
        public double? Aperture
        {
            get => _aperture;
            set
            {
                if (_aperture != value)
                {
                    _aperture = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _aperture = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(10.0),
            DisplayName(nameof(NPLCycles) + " [Default = 10]"),
            Description("Number of Power Line Cycles. 0.001 ~ 100 사이의 값으로, Integration 시간을 미리 정한 다음 값(Power Line Cycle)들 중 하나로 설정합니다:\r\n" +
            " • 0.02, 0.2, 1, 10, 100 - Fluke 8845A/8846A, Keysight 34460A/61A\r\n" +
            " • 0.02, 0.06, 0.2, 1, 10, 100 - Keysight 34465A/70A without DIG option\r\n" +
            " • 0.001, 0.002, 0.006, 0.02, 0.06, 0.2, 1, 10, 100 - Keysight 34465A/70A with DIG option\r\n" +
            "이 항목은 DC 전압/전류, 온도, 저항 측정 기능들에 대해서만 유효합니다.")]
        public double? NPLCycles
        {
            get => _nplCycles;
            set
            {
                if (_nplCycles != value)
                {
                    _nplCycles = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _nplCycles = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(20.0),
            DisplayName(nameof(BandWidth) + " [Default = 20]"),
            Description("적절한 주파수 필터를 설정합니다.\r\n" +
            " • 3              느린 필터 설정\r\n" +
            " • 20(default)    중간 필터 설정\r\n" +
            " • 200            빠른 필터 설정")]
        public double? BandWidth
        {
            get => _bandWidth;
            set
            {
                if (_bandWidth != value)
                {
                    _bandWidth = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _bandWidth = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            DisplayName(nameof(AnalogDCFilter) + " [Default = False]"),
            Description("DC 측정 기능들의 잡음 내성을 높이기 위해  3-pole 아날로그 필터를 활성화/비활성화 합니다.")]
        public bool? AnalogDCFilter
        {
            get => _analogDCFilter;
            set
            {
                if (_analogDCFilter != value)
                {
                    _analogDCFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _analogDCFilter = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(true),
            DisplayName(nameof(DigitalAveragingFilter) + " [Default = True]"),
            Description("DC 측정 기능들의 잡음 내성을 높이기 위해 디지털 평균 필터(Digital Averaging Filter)를 활성화/비활성화 합니다.")]
        public bool? DigitalAveragingFilter
        {
            get => _digitalAveragingFilter;
            set
            {
                if (_digitalAveragingFilter != value)
                {
                    _digitalAveragingFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _digitalAveragingFilter = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(true),
            DisplayName(nameof(AutoZero) + " [Default = True]"),
            Description("autozero 모드를 활성화(default)/비활성화 합니다.")]
        public bool? AutoZero
        {
            get => _autoZero;
            set
            {
                if (_autoZero != value)
                {
                    _autoZero = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _autoZero = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            DisplayName(nameof(AutoInputImpedance) + " [Default = False]"),
            Description("DC 전압 측정에 대해 자동 입력 임피던스 모드를 활성화/비활성화(default) 합니다.\r\n" +
            "OFF 이면 입력 임피던스는 모든 범위에 대해 10 MΩ 으로 고정됩니다.\r\n" +
            "ON 이면 입력 임피던스는 100mV, 1V, 10V 범위들에 대해 10 GΩ 보다 큽니다.")]
        public bool? AutoInputImpedance
        {
            get => _autoInputImpedance;
            set
            {
                if (_autoInputImpedance != value)
                {
                    _autoInputImpedance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _autoInputImpedance = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(1),
            DisplayName(nameof(SampleCount) + " [Default = 1]"),
            Description("트리거당 측정 횟수를 설정합니다.")]
        public int? SampleCount
        {
            get => _sampleCount;
            set
            {
                if (_sampleCount != value)
                {
                    _sampleCount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int? _sampleCount = null;

        [Category(MethodCategory), Browsable(false), DefaultValue(DmmDevice.DownloadImageFormat.PNG),
            DisplayName(nameof(DownloadImageFormat) + " [Default = " + nameof(DmmDevice.DownloadImageFormat.PNG) + "]"),
            Description("다운로드할 이미지 형식을 설정합니다.")]
        public DmmDevice.DownloadImageFormat? DownloadImageFormat
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
        private DmmDevice.DownloadImageFormat? _downloadImageFormat;

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
                    NotifyPropertyChanged();
                }
            }
        }
        private string _parameterName = null;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 High Input 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelHighInput
        {
            get => _testChannelHighInput;
            set
            {
                if (_testChannelHighInput != value)
                {
                    if (_testChannelHighInput != null)
                    {
                        _testChannelHighInput.ListChanged -= TestChannelHighInput_ListChanged;
                    }
                    _testChannelHighInput = value;
                    if (_testChannelHighInput != null)
                    {
                        _testChannelHighInput = new BindingList<int>(_testChannelHighInput.Distinct().ToList());
                        _testChannelHighInput.ListChanged += TestChannelHighInput_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelHighInput;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 Low Input 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelLowInput
        {
            get => _testChannelLowInput;
            set
            {
                if (_testChannelLowInput != value)
                {
                    if (_testChannelLowInput != null)
                    {
                        _testChannelLowInput.ListChanged -= TestChannelLowInput_ListChanged;
                    }
                    _testChannelLowInput = value;
                    if (_testChannelLowInput != null)
                    {
                        _testChannelLowInput = new BindingList<int>(_testChannelLowInput.Distinct().ToList());
                        _testChannelLowInput.ListChanged += TestChannelLowInput_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelLowInput;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)), 
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)), 
            Description("디바이스의 High Sense 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelHighSense
        {
            get => _testChannelHighSense;
            set
            {
                if (_testChannelHighSense != value)
                {
                    if (_testChannelHighSense != null)
                    {
                        _testChannelHighSense.ListChanged -= TestChannelHighSense_ListChanged;
                    }
                    _testChannelHighSense = value;
                    if (_testChannelHighSense != null)
                    {
                        _testChannelHighSense = new BindingList<int>(_testChannelHighSense.Distinct().ToList());
                        _testChannelHighSense.ListChanged += TestChannelHighSense_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelHighSense;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 Low Sense 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannelLowSense
        {
            get => _testChannelLowSense;
            set
            {
                if (_testChannelLowSense != value)
                {
                    if (_testChannelLowSense != null)
                    {
                        _testChannelLowSense.ListChanged -= TestChannelLowSense_ListChanged;
                    }
                    _testChannelLowSense = value;
                    if (_testChannelLowSense != null)
                    {
                        _testChannelLowSense = new BindingList<int>(_testChannelLowSense.Distinct().ToList());
                        _testChannelLowSense.ListChanged += TestChannelLowSense_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannelLowSense;


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

        [Category(MethodCategory), DefaultValue(false), Browsable(false),
            Description("")]
        public bool Temp
        {
            get => _temp;
            set
            {
                if (_temp != value)
                {
                    _temp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _temp = false;

        [Category(MethodCategory), DefaultValue(false), Browsable(false),
            Description("")]
        public bool PureTemp
        {
            get => _pureTemp;
            set
            {
                if (_pureTemp != value)
                {
                    _pureTemp = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _pureTemp = false;

        [Category(MethodCategory), DefaultValue(ResultState.Pass), Browsable(false),
            Description("")]
        public ResultState PureTempResultState
        {
            get => _pureTempResultState;
            set
            {
                if (_pureTempResultState != value)
                {
                    _pureTempResultState = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ResultState _pureTempResultState = ResultState.Pass;

        internal override string CategoryName => StepCategory.DMM.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case DmmTestMode.Configure:
                    case DmmTestMode.Read:
                        return Function.ToString();
                    case DmmTestMode.ReadIDN:
                    case DmmTestMode.Reset:
                    case DmmTestMode.ReadMinValue:
                    case DmmTestMode.ReadMaxValue:
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
                    case DmmTestMode.Read:
                        PhysicalUnit unit = GetPhysicalUnit();
                        var valueText = GetPrefixExpression(ExpectedValue, unit, out MetricPrefix prefix);
                        return $"{valueText}{prefix.GetText()}{unit.GetText()}";
                    case DmmTestMode.ReadIDN:
                    case DmmTestMode.Reset:
                    case DmmTestMode.ReadMinValue:
                    case DmmTestMode.ReadMaxValue:
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
                    case DmmTestMode.Read:
                        PhysicalUnit unit = GetPhysicalUnit();
                        return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                            TolerancePlusAbsolute, TolerancePlusMinusAbsolute, unit);
                    case DmmTestMode.ReadIDN:
                    case DmmTestMode.Reset:
                    case DmmTestMode.ReadMinValue:
                    case DmmTestMode.ReadMaxValue:
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
                    case DmmTestMode.Read:
                        PhysicalUnit unit = GetPhysicalUnit();
                        return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent,
                            ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, unit);
                    case DmmTestMode.ReadIDN:
                    case DmmTestMode.Reset:
                    case DmmTestMode.ReadMinValue:
                    case DmmTestMode.ReadMaxValue:
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
                    case DmmTestMode.Read:
                        var channels = new List<int>();
                        if (TestChannelHighInput != null)
                        {
                            channels.AddRange(TestChannelHighInput);
                        }
                        if (TestChannelLowInput != null)
                        {
                            channels.AddRange(TestChannelLowInput);
                        }
                        if (TestChannelHighSense != null)
                        {
                            channels.AddRange(TestChannelHighSense);
                        }
                        if (TestChannelLowSense != null)
                        {
                            channels.AddRange(TestChannelLowSense);
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
        private EolDmmStep()
        {
            Name = StepCategory.DMM.GetText();
        }

        public EolDmmStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return DmmDevice.CreateInstance(DeviceName, !IsPureTemp);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetDmmSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.DMM, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.Fluke_8845A_8846A:
                        return new object[] {
                            DmmTestMode.ReadIDN,
                            DmmTestMode.Reset,
                            DmmTestMode.Configure,
                            DmmTestMode.Read,
                            DmmTestMode.ReadMinValue,
                            DmmTestMode.ReadMaxValue,
                            DmmTestMode.RunCommandLine,
                            DmmTestMode.OpenTest,
                            DmmTestMode.ShortTest,
                        };
                    case DeviceType.Keysight_Truevolt_Series:
                        return new object[] {
                            DmmTestMode.ReadIDN,
                            DmmTestMode.Reset,
                            DmmTestMode.Configure,
                            DmmTestMode.Read,
                            DmmTestMode.Download,
                            DmmTestMode.ReadMinValue,
                            DmmTestMode.ReadMaxValue,
                            DmmTestMode.RunCommandLine,
                            DmmTestMode.OpenTest,
                            DmmTestMode.ShortTest,
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
                if (Enum.TryParse(stringValue, out DmmTestMode parsed))
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
                case DmmTestMode.Read:
                    if (setting is DmmDeviceSetting dmmSetting)
                    {
                        var args = new List<IEnumerable<int>>();

                        // High Input channels.
                        if (dmmSetting.ChannelHighInput > 0 && TestChannelHighInput?.Count > 0)
                        {
                            args.Add(TestChannelHighInput.Concat(new int[] { dmmSetting.ChannelHighInput }));
                        }

                        // Low Input channels.
                        if (dmmSetting.ChannelLowInput > 0 && TestChannelLowInput?.Count > 0)
                        {
                            args.Add(TestChannelLowInput.Concat(new int[] { dmmSetting.ChannelLowInput }));
                        }

                        // High Sense channels.
                        if (dmmSetting.ChannelHighSense > 0 && TestChannelHighSense?.Count > 0)
                        {
                            args.Add(TestChannelHighSense.Concat(new int[] { dmmSetting.ChannelHighSense }));
                        }

                        // Low Sense channels.
                        if (dmmSetting.ChannelLowSense > 0 && TestChannelLowSense?.Count > 0)
                        {
                            args.Add(TestChannelLowSense.Concat(new int[] { dmmSetting.ChannelLowSense }));
                        }

                        // eloZ1 relay on.
                        ElozDevice.SharedInstance.ConnectChannels(elozTestSet, args.ToArray());
                    }
                    break;
            }
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            switch (TestMethod)
            {
                case DmmTestMode.Read:
                case DmmTestMode.OpenTest:
                case DmmTestMode.ShortTest:
                    nominalValue = ExpectedValue;
                    GetLimitValues(ExpectedValue, Tolerance, ToleranceMinusPercent, TolerancePlusPercent, TolerancePlusMinusPercent,
                        ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, out upperLimit, out lowerLimit);
                    break;
                default:
                    nominalValue = null;
                    upperLimit = null;
                    lowerLimit = null;
                    break;
            }
        }

        /// <summary>
        /// 읽기 시도도 하지 않는 완전한 Temp 스텝인지 여부를 리턴한다.
        /// </summary>
        internal bool IsPureTemp => (TestMethod == DmmTestMode.Read || TestMethod == DmmTestMode.OpenTest || TestMethod == DmmTestMode.ShortTest) && PureTemp;

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

            var dmm = device as DmmDevice;
            if (!IsPureTemp && dmm == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case DmmTestMode.ReadIDN:
                    result.ResultInfo = dmm.ReadIDN(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case DmmTestMode.Reset:
                    dmm.Reset(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case DmmTestMode.Configure:
                    dmm.Configure(Function, Range, RTDType, DiodeLowCurrent, Resolution, DiodeHighVoltage, NPLCycles, Aperture,
                        BandWidth, AnalogDCFilter, DigitalAveragingFilter, AutoZero, AutoInputImpedance, 0, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case DmmTestMode.Read:
                case DmmTestMode.OpenTest:
                case DmmTestMode.ShortTest:
                    double[] values;

                    // 완전한 Temp 기능이면 읽기 시도 안 함.
                    if (!PureTemp)
                    {
                        values = dmm.Read(Function, SampleCount, token);
                        UpdateResult(result, values);
                    }

                    if (result.ResultValueState != ResultValueState.Good && (Temp || PureTemp))
                    {
                        // HACK: Method == Temp 인 경우 가성 데이터 생성.
                        var randomValue = GetTempValue(Tolerance, ExpectedValue, TolerancePlusMinusAbsolute, TolerancePlusAbsolute,
                            ToleranceMinusAbsolute, TolerancePlusMinusPercent, TolerancePlusPercent, ToleranceMinusPercent);

                        var resultValue = randomValue + MeasureOffset;
                        if (TestMethod == DmmTestMode.Read)
                        {
                            resultValue += MeasureOffset;
                        }
                        values = new double[] { resultValue };
                        UpdateResult(result, values);

                        // 너무 빨리 끝나는것 방지 위해 딜레이.
                        if (PureTemp)
                        {
                            MultimediaTimer.Delay(100).Wait(token);
                        }
                    }

                    // Open, Short 테스트의 경우.
                    if (TestMethod == DmmTestMode.OpenTest || TestMethod == DmmTestMode.ShortTest)
                    {
                        result.ResultValue = null;
                        result.ResultState = PureTempResultState;
                    }

                    break;
                case DmmTestMode.Download:
                    var imageData = dmm.Download(DownloadImageFormat, token);

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
                        downloadFile = "D:\\ElozPlugin\\DMMScreen\\" + DateTime.Now.ToString("yyyy-MM-dd_HHmmss") + ".png"; ;
                        Directory.CreateDirectory(Path.GetDirectoryName(downloadFile));
                    }
                    File.WriteAllBytes(downloadFile, imageData);

                    result.ResultInfo = downloadFile;
                    result.ResultState = ResultState.Pass;
                    result.ResultData = Image.FromStream(new MemoryStream(imageData));
                    break;
                case DmmTestMode.ReadMinValue:
                    result.ResultValue = dmm.GetMinValue(this, ParameterName, token) as double?;
                    result.ResultState = ResultState.Pass;
                    break;
                case DmmTestMode.ReadMaxValue:
                    result.ResultValue = dmm.GetMaxValue(this, ParameterName, token) as double?;
                    result.ResultState = ResultState.Pass;
                    break;
                case DmmTestMode.RunCommandLine:
                    var response = dmm.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, 2000, token);
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

        // 기준값과 측정오차, 측정값들로부터 결과 Pass/Fail, Value 상태를 결정한다.
        private void UpdateResult(TestResult result, double[] values)
        {
            if (values?.Length > 0)
            {
                double measuredValue = values.Average() - MeasureOffset;
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
            }
        }

        /// <summary>
        /// MIN, MAX 값을 얻을 수 있는 파라미터 목록을 리턴한다.
        /// </summary>
        /// <returns></returns>
        public List<string> GetParameterNames()
        {
            var names = new List<string>();

            bool showRange = Function != DmmDevice.DmmFunction.Temperature_2_Wire
                && Function != DmmDevice.DmmFunction.Temperature_4_Wire
                && Function != DmmDevice.DmmFunction.Diode
                && Function != DmmDevice.DmmFunction.Continuity;
            if (showRange)
            {
                names.Add(nameof(Range));
            }

            bool showResolution = showRange && Function != DmmDevice.DmmFunction.Frequency && Function != DmmDevice.DmmFunction.Period;
            if (showResolution)
            {
                names.Add(nameof(Resolution));
            }

            bool showNPLCycles = Function == DmmDevice.DmmFunction.Voltage_DC
                || Function == DmmDevice.DmmFunction.Current_DC
                || Function == DmmDevice.DmmFunction.Temperature_2_Wire
                || Function == DmmDevice.DmmFunction.Temperature_4_Wire
                || Function == DmmDevice.DmmFunction.Resistance_2_Wire
                || Function == DmmDevice.DmmFunction.Resistance_4_Wire;
            if (showNPLCycles)
            {
                names.Add(nameof(NPLCycles));
            }

            bool showAperture = Function == DmmDevice.DmmFunction.Frequency || Function == DmmDevice.DmmFunction.Period;
            if (showAperture)
            {
                names.Add(nameof(Aperture));
            }

            bool showBandWidth = Function == DmmDevice.DmmFunction.Current_AC || Function == DmmDevice.DmmFunction.Voltage_AC;
            if (showBandWidth)
            {
                names.Add(nameof(BandWidth));
            }

            names.Add(nameof(SampleCount));

            return names;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            PhysicalUnit unit;
            switch (Function)
            {
                case DmmDevice.DmmFunction.Capacitance:
                    unit = PhysicalUnit.Farad;
                    break;
                case DmmDevice.DmmFunction.Current_AC:
                case DmmDevice.DmmFunction.Current_DC:
                    unit = PhysicalUnit.Ampere;
                    break;
                case DmmDevice.DmmFunction.Voltage_AC:
                case DmmDevice.DmmFunction.Voltage_DC:
                case DmmDevice.DmmFunction.Diode:
                    unit = PhysicalUnit.Volt;
                    break;
                case DmmDevice.DmmFunction.Voltage_DC_Ratio:
                    unit = PhysicalUnit.None;
                    break;
                case DmmDevice.DmmFunction.Resistance_2_Wire:
                case DmmDevice.DmmFunction.Resistance_4_Wire:
                case DmmDevice.DmmFunction.Continuity:
                    unit = PhysicalUnit.Ohm;
                    break;
                case DmmDevice.DmmFunction.Frequency:
                    unit = PhysicalUnit.Hertz;
                    break;
                case DmmDevice.DmmFunction.Period:
                    unit = PhysicalUnit.Second;
                    break;
                case DmmDevice.DmmFunction.Temperature_2_Wire:
                case DmmDevice.DmmFunction.Temperature_4_Wire:
                    unit = PhysicalUnit.Celsius;
                    break;
                default:
                    unit = PhysicalUnit.None;
                    break;
            }
            return unit;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();

            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            bool visible = browsable ?? false;
            Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), visible);
            Utils.SetBrowsableAttribute(this, nameof(Temp), visible);
            Utils.SetBrowsableAttribute(this, nameof(PureTemp), visible);
            Utils.SetBrowsableAttribute(this, nameof(PureTempResultState), visible);
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            bool showMeasProperties = TestMethod == DmmTestMode.Read || TestMethod == DmmTestMode.OpenTest || TestMethod == DmmTestMode.ShortTest;
            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), showMeasProperties);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), TestMethod == DmmTestMode.Read);
            UpdateToleranceAttributes();
            //Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), showMeasProperties);

            bool showFunction = TestMethod == DmmTestMode.Configure
                || TestMethod == DmmTestMode.Read
                || TestMethod == DmmTestMode.ReadMinValue 
                || TestMethod == DmmTestMode.ReadMaxValue;
            Utils.SetBrowsableAttribute(this, nameof(Function), showFunction);

            bool showRange = TestMethod == DmmTestMode.Configure 
                && Function != DmmDevice.DmmFunction.Temperature_2_Wire 
                && Function != DmmDevice.DmmFunction.Temperature_4_Wire 
                && Function != DmmDevice.DmmFunction.Diode 
                && Function != DmmDevice.DmmFunction.Continuity 
                && Function != DmmDevice.DmmFunction.Voltage_DC_Ratio;
            Utils.SetBrowsableAttribute(this, nameof(Range), showRange);

            // 장치 타입별로 파라미터들을 보여주거나 숨긴다.
            DeviceType? deviceType = null;
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var dmmSetting = settingsManager.FindSetting(DeviceCategory.DMM, DeviceName) as DmmDeviceSetting;
                if (dmmSetting != null)
                {
                    deviceType = dmmSetting.DeviceType;
                }
            }
            catch {}

            bool isFlukeDevice = deviceType == null || deviceType == DeviceType.Fluke_8845A_8846A;
            bool showRtdType =  TestMethod == DmmTestMode.Configure && (
                Function == DmmDevice.DmmFunction.Temperature_2_Wire || 
                Function == DmmDevice.DmmFunction.Temperature_4_Wire);
            Utils.SetBrowsableAttribute(this, nameof(RTDType), isFlukeDevice && showRtdType);

            bool showDiode = TestMethod == DmmTestMode.Configure && Function == DmmDevice.DmmFunction.Diode;
            Utils.SetBrowsableAttribute(this, nameof(DiodeLowCurrent), isFlukeDevice && showDiode);
            Utils.SetBrowsableAttribute(this, nameof(DiodeHighVoltage), isFlukeDevice && showDiode);

            bool showResolution = showRange 
                && Function != DmmDevice.DmmFunction.Frequency 
                && Function != DmmDevice.DmmFunction.Period
                && Function != DmmDevice.DmmFunction.Voltage_AC
                && Function != DmmDevice.DmmFunction.Current_AC;
            Utils.SetBrowsableAttribute(this, nameof(Resolution), showResolution);

            bool showNPLCycles = TestMethod == DmmTestMode.Configure && (
                Function == DmmDevice.DmmFunction.Voltage_DC ||
                Function == DmmDevice.DmmFunction.Current_DC ||
                Function == DmmDevice.DmmFunction.Temperature_2_Wire ||
                Function == DmmDevice.DmmFunction.Temperature_4_Wire ||
                Function == DmmDevice.DmmFunction.Resistance_2_Wire ||
                Function == DmmDevice.DmmFunction.Resistance_4_Wire);
            Utils.SetBrowsableAttribute(this, nameof(NPLCycles), showNPLCycles);

            bool showAperture;
            if (isFlukeDevice)
            {
                showAperture = TestMethod == DmmTestMode.Configure && (
                    Function == DmmDevice.DmmFunction.Frequency ||
                    Function == DmmDevice.DmmFunction.Period);
            }
            else
            {
                showAperture = TestMethod == DmmTestMode.Configure && (
                    Function == DmmDevice.DmmFunction.Frequency ||
                    Function == DmmDevice.DmmFunction.Period ||
                    Function == DmmDevice.DmmFunction.Current_DC ||
                    Function == DmmDevice.DmmFunction.Resistance_2_Wire ||
                    Function == DmmDevice.DmmFunction.Resistance_4_Wire ||
                    Function == DmmDevice.DmmFunction.Temperature_2_Wire ||
                    Function == DmmDevice.DmmFunction.Temperature_4_Wire ||
                    Function == DmmDevice.DmmFunction.Voltage_DC);
            }
            Utils.SetBrowsableAttribute(this, nameof(Aperture), showAperture);

            bool showBandWidth = TestMethod == DmmTestMode.Configure && (
                Function == DmmDevice.DmmFunction.Voltage_AC ||
                Function == DmmDevice.DmmFunction.Current_AC);
            Utils.SetBrowsableAttribute(this, nameof(BandWidth), showBandWidth);

            bool showDCFilters = TestMethod == DmmTestMode.Configure && (
                Function == DmmDevice.DmmFunction.Current_DC ||
                Function == DmmDevice.DmmFunction.Voltage_DC || 
                Function == DmmDevice.DmmFunction.Resistance_2_Wire || 
                Function == DmmDevice.DmmFunction.Resistance_4_Wire);
            Utils.SetBrowsableAttribute(this, nameof(AnalogDCFilter), isFlukeDevice && showDCFilters);
            Utils.SetBrowsableAttribute(this, nameof(DigitalAveragingFilter), isFlukeDevice && showDCFilters);

            bool showAutoZero;
            if (isFlukeDevice)
            {
                showAutoZero = TestMethod == DmmTestMode.Configure;
            }
            else
            {
                showAutoZero = TestMethod == DmmTestMode.Configure && (
                    Function == DmmDevice.DmmFunction.Current_DC ||
                    Function == DmmDevice.DmmFunction.Resistance_2_Wire || 
                    Function == DmmDevice.DmmFunction.Temperature_2_Wire || 
                    Function == DmmDevice.DmmFunction.Temperature_4_Wire || 
                    Function == DmmDevice.DmmFunction.Voltage_DC);
            }
            Utils.SetBrowsableAttribute(this, nameof(AutoZero), showAutoZero);

            Utils.SetBrowsableAttribute(this, nameof(AutoInputImpedance), TestMethod == DmmTestMode.Configure && Function == DmmDevice.DmmFunction.Voltage_DC);
            Utils.SetBrowsableAttribute(this, nameof(SampleCount), TestMethod == DmmTestMode.Read);
            Utils.SetBrowsableAttribute(this, nameof(DownloadImageFormat), TestMethod == DmmTestMode.Download);
            Utils.SetBrowsableAttribute(this, nameof(DownloadFile), TestMethod == DmmTestMode.Download);
            Utils.SetBrowsableAttribute(this, nameof(ParameterName), TestMethod == DmmTestMode.ReadMinValue || TestMethod == DmmTestMode.ReadMaxValue);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelHighSense), showMeasProperties);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelLowSense), showMeasProperties);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelHighInput), showMeasProperties);
            Utils.SetBrowsableAttribute(this, nameof(TestChannelLowInput), showMeasProperties);

            bool runCmdLine = TestMethod == DmmTestMode.RunCommandLine;
            Utils.SetBrowsableAttribute(this, nameof(CommandLine), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), runCmdLine);
            bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
            Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), runCmdLine && compareResponse);
            Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), runCmdLine && compareResponse);
            //Utils.SetBrowsableAttribute(this, nameof(Temp), TestMethod == DmmTestMode.Read);
        }

        protected override void UpdateToleranceAttributes()
        {
            bool showMeasProperties = TestMethod == DmmTestMode.Read;
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), showMeasProperties && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), showMeasProperties && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), showMeasProperties && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), showMeasProperties && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), showMeasProperties && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), showMeasProperties && Tolerance == ToleranceMode.Absolute);

            // Update display names.
            PhysicalUnit unit = GetPhysicalUnit();
            string unitText = unit == PhysicalUnit.None ? "" : $" [{unit.GetText()}]";
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
            var newStep = new EolDmmStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolDmmStep destStep)
            {
                destStep.TestMethod = TestMethod;
                destStep.ExpectedValue = ExpectedValue;
                destStep.Tolerance = Tolerance;
                destStep.TolerancePlusPercent = TolerancePlusPercent;
                destStep.ToleranceMinusPercent = ToleranceMinusPercent;
                destStep.TolerancePlusMinusPercent = TolerancePlusMinusPercent;
                destStep.TolerancePlusAbsolute = TolerancePlusAbsolute;
                destStep.ToleranceMinusAbsolute = ToleranceMinusAbsolute;
                destStep.TolerancePlusMinusAbsolute = TolerancePlusMinusAbsolute;
                destStep.MeasureOffset = MeasureOffset;
                destStep.Function = Function;
                destStep.Range = Range;
                destStep.RTDType = RTDType;
                destStep.DiodeLowCurrent = DiodeLowCurrent;
                destStep.DiodeHighVoltage = DiodeHighVoltage;
                destStep.Resolution = Resolution;
                destStep.NPLCycles = NPLCycles;
                destStep.Aperture = Aperture;
                destStep.BandWidth = BandWidth;
                destStep.AnalogDCFilter = AnalogDCFilter;
                destStep.DigitalAveragingFilter = DigitalAveragingFilter;
                destStep.AutoZero = AutoZero;
                destStep.AutoInputImpedance = AutoInputImpedance;
                destStep.SampleCount = SampleCount;
                destStep.DownloadImageFormat = DownloadImageFormat;
                destStep.DownloadFile = DownloadFile;

                if (TestChannelHighSense != null)
                {
                    var channelList = new BindingList<int>();
                    foreach (var channel in TestChannelHighSense)
                    {
                        channelList.Add(channel);
                    }
                    destStep.TestChannelHighSense = channelList;
                }
                if (TestChannelLowSense != null)
                {
                    var channelList = new BindingList<int>();
                    foreach (var channel in TestChannelLowSense)
                    {
                        channelList.Add(channel);
                    }
                    destStep.TestChannelLowSense = channelList;
                }
                if (TestChannelHighInput != null)
                {
                    var channelList = new BindingList<int>();
                    foreach (var channel in TestChannelHighInput)
                    {
                        channelList.Add(channel);
                    }
                    destStep.TestChannelHighInput = channelList;
                }
                if (TestChannelLowInput != null)
                {
                    var channelList = new BindingList<int>();
                    foreach (var channel in TestChannelLowInput)
                    {
                        channelList.Add(channel);
                    }
                    destStep.TestChannelLowInput = channelList;
                }

                destStep.ParameterName = ParameterName;
                destStep.CommandLine = CommandLine;
                destStep.ResponseCheckMethod = ResponseCheckMethod;
                destStep.ResponsePattern = ResponsePattern;
                destStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;
                destStep.Temp = Temp;
                destStep.PureTemp = PureTemp;
                destStep.PureTempResultState = PureTempResultState;
            }

            dest.UpdateBrowsableAttributes();
        }

        private void TestChannelHighSense_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelHighSense));
        }
        private void TestChannelLowSense_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelLowSense));
        }
        private void TestChannelHighInput_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelHighInput));
        }
        private void TestChannelLowInput_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannelLowInput));
        }

        /// <summary>
        /// DMM 기능 리스트.
        /// </summary>
        public enum DmmTestMode
        {
            ReadIDN,
            Reset,
            Configure,
            Read,
            Download,
            ReadMinValue,
            ReadMaxValue,
            RunCommandLine,
            OpenTest,
            ShortTest,
        }
    }
}
