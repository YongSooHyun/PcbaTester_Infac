using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using EOL_GND.View;
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
    public class EolOscopeStep : EolStep
    {
        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)), 
            Description("테스트 방법을 설정합니다.")]
        public OscopeTestMode TestMethod
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
        private OscopeTestMode _testMethod = OscopeTestMode.ReadIDN;

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

        [Category(MethodCategory), Browsable(false), DefaultValue(OscopeDevice.OscopeStateLocation.Location1), 
            Description("계측기 상태를 저장하거나 불러올 저장 위치를 나타냅니다.")]
        public OscopeDevice.OscopeStateLocation StateLocation
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
        private OscopeDevice.OscopeStateLocation _stateLocation = OscopeDevice.OscopeStateLocation.Location1;

        [Category(MethodCategory), Browsable(false), 
            Description("1번 채널에 대한 설정을 합니다.")]
        public OscopeChannelSettings Channel1Settings
        {
            get => _channel1Settings;
            set
            {
                if (_channel1Settings != value)
                {
                    if (_channel1Settings != null)
                    {
                        _channel1Settings.PropertyChanged -= _channel1Settings_PropertyChanged;
                    }
                    _channel1Settings = value;
                    if (_channel1Settings != null)
                    {
                        _channel1Settings.PropertyChanged += _channel1Settings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeChannelSettings _channel1Settings = new OscopeChannelSettings();

        private void _channel1Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Channel1Settings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("2번 채널에 대한 설정을 합니다.")]
        public OscopeChannelSettings Channel2Settings
        {
            get => _channel2Settings;
            set
            {
                if (_channel2Settings != value)
                {
                    if (_channel2Settings != null)
                    {
                        _channel2Settings.PropertyChanged -= _channel2Settings_PropertyChanged;
                    }
                    _channel2Settings = value;
                    if (_channel2Settings != null)
                    {
                        _channel2Settings.PropertyChanged += _channel2Settings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeChannelSettings _channel2Settings = new OscopeChannelSettings();

        private void _channel2Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Channel2Settings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("3번 채널에 대한 설정을 합니다.")]
        public OscopeChannelSettings Channel3Settings
        {
            get => _channel3Settings;
            set
            {
                if (_channel3Settings != value)
                {
                    if (_channel3Settings != null)
                    {
                        _channel3Settings.PropertyChanged -= _channel3Settings_PropertyChanged;
                    }
                    _channel3Settings = value;
                    if (_channel3Settings != null)
                    {
                        _channel3Settings.PropertyChanged += _channel3Settings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeChannelSettings _channel3Settings = new OscopeChannelSettings();

        private void _channel3Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Channel3Settings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("4번 채널에 대한 설정을 합니다.")]
        public OscopeChannelSettings Channel4Settings
        {
            get => _channel4Settings;
            set
            {
                if (_channel4Settings != value)
                {
                    if (_channel4Settings != null)
                    {
                        _channel4Settings.PropertyChanged -= _channel4Settings_PropertyChanged;
                    }
                    _channel4Settings = value;
                    if (_channel4Settings != null)
                    {
                        _channel4Settings.PropertyChanged += _channel4Settings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeChannelSettings _channel4Settings = new OscopeChannelSettings();

        private void _channel4Settings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Channel4Settings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("트리거를 설정합니다.")]
        public OscopeTriggerSettings TriggerSettings
        {
            get => _triggerSettings;
            set
            {
                if (_triggerSettings != value)
                {
                    if (_triggerSettings != null)
                    {
                        _triggerSettings.PropertyChanged -= _triggerSettings_PropertyChanged;
                    }
                    _triggerSettings = value;
                    if (_triggerSettings != null)
                    {
                        _triggerSettings.PropertyChanged += _triggerSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeTriggerSettings _triggerSettings = new OscopeTriggerSettings();

        private void _triggerSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TriggerSettings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("데이터 수집 파라미터들을 설정합니다.")]
        public OscopeAcquireSettings AcquireSettings
        {
            get => _acquireSettings;
            set
            {
                if (_acquireSettings != value)
                {
                    if (_acquireSettings != null)
                    {
                        _acquireSettings.PropertyChanged -= _acquireSettings_PropertyChanged;
                    }
                    _acquireSettings = value;
                    if (_acquireSettings != null)
                    {
                        _acquireSettings.PropertyChanged += _acquireSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private void _acquireSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(AcquireSettings));
        }

        private OscopeAcquireSettings _acquireSettings = new OscopeAcquireSettings();

        [Category(MethodCategory), Browsable(false), 
            Description("수평(X축) 기능을 제어하고 오실로스코프를 X-Y 모드로 설정합니다(여기서 채널 1은 X 입력, 채널 2는 Y 입력). " +
            "메인 및 윈도우(확대된) 시간축에 대해 눈금당 시간, 지연, 버니어, 기준점을 제어할 수 있습니다.")]
        public OscopeTimebaseSettings TimebaseSettings
        {
            get => _timebaseSettings;
            set
            {
                if (_timebaseSettings != value)
                {
                    if (_timebaseSettings != null)
                    {
                        _timebaseSettings.PropertyChanged -= _timebaseSettings_PropertyChanged;
                    }
                    _timebaseSettings = value;
                    if (_timebaseSettings != null)
                    {
                        _timebaseSettings.PropertyChanged += _timebaseSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }

        private void _timebaseSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TimebaseSettings));
        }

        private OscopeTimebaseSettings _timebaseSettings = new OscopeTimebaseSettings();

        [Category(MethodCategory), Browsable(false), 
            Description("측정 관련 파라미터들을 설정합니다.")]
        public OscopeMeasurementSettings MeasurementSettings
        {
            get => _measurementSettings;
            set
            {
                if (_measurementSettings != value)
                {
                    if (_measurementSettings != null)
                    {
                        _measurementSettings.PropertyChanged -= _measurementSettings_PropertyChanged;
                    }
                    _measurementSettings = value;
                    if (_measurementSettings != null)
                    {
                        _measurementSettings.PropertyChanged += _measurementSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeMeasurementSettings _measurementSettings = new OscopeMeasurementSettings();

        private void _measurementSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OscopeMeasurementSettings.Method))
            {
                UpdateToleranceAttributes();
            }
            NotifyPropertyChanged(nameof(MeasurementSettings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("DVM(Digital VoltMeter) 관련 파라미터들을 설정합니다.")]
        public OscopeDvmSettings DigitalVoltmeterSettings
        {
            get => _digitalVoltmeterSettings;
            set
            {
                if (_digitalVoltmeterSettings != value)
                {
                    if (_digitalVoltmeterSettings != null)
                    {
                        _digitalVoltmeterSettings.PropertyChanged -= _digitalVoltmeterSettings_PropertyChanged;
                    }
                    _digitalVoltmeterSettings = value;
                    if (_digitalVoltmeterSettings != null)
                    {
                        _digitalVoltmeterSettings.PropertyChanged += _digitalVoltmeterSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeDvmSettings _digitalVoltmeterSettings = new OscopeDvmSettings();

        private void _digitalVoltmeterSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == nameof(OscopeDvmSettings.Source))
            {
                UpdateBrowsableAttributes();
            }
            NotifyPropertyChanged(nameof(DigitalVoltmeterSettings));
        }

        [Category(MethodCategory), Browsable(false),
            Description("화면 표시 및 화면 이미지 다운로드 관련 파라미터들을 설정합니다.")]
        public OscopeDisplaySettings DisplaySettings
        {
            get => _displaySettings;
            set
            {
                if (_displaySettings != value)
                {
                    if (_displaySettings != null)
                    {
                        _displaySettings.PropertyChanged -= _displaySettings_PropertyChanged;
                    }
                    _displaySettings = value;
                    if (_displaySettings != null)
                    {
                        _displaySettings.PropertyChanged += _displaySettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeDisplaySettings _displaySettings = new OscopeDisplaySettings();

        private void _displaySettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(DisplaySettings));
        }

        [Category(MethodCategory), Browsable(false), DefaultValue(OscopeDevice.OscopeChannels.CH1), 
            Description("파형을 캡쳐하려는 채널을 지정합니다.")]
        public OscopeDevice.OscopeChannels CaptureSources
        {
            get => _captureSources;
            set
            {
                if (_captureSources != value)
                {
                    _captureSources = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeDevice.OscopeChannels _captureSources = OscopeDevice.OscopeChannels.CH1;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("모든 입력 신호를 평가하고 신호를 표시하기 위한 올바른 조건을 설정합니다. 이것은 전면 패널에 있는 [Auto Scale]키를 누르는 것과 같습니다.")]
        public bool AutoScale
        {
            get => _autoScale;
            set
            {
                if (_autoScale != value)
                {
                    _autoScale = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _autoScale = false;

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

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 3번 채널 High 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel3High
        {
            get => _testChannel3High;
            set
            {
                if (_testChannel3High != value)
                {
                    if (_testChannel3High != null)
                    {
                        _testChannel3High.ListChanged -= _testChannel3High_ListChanged;
                    }
                    _testChannel3High = value;
                    if (_testChannel3High != null)
                    {
                        _testChannel3High = new BindingList<int>(_testChannel3High.Distinct().ToList());
                        _testChannel3High.ListChanged += _testChannel3High_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel3High;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 3번 채널 Low 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel3Low
        {
            get => _testChannel3Low;
            set
            {
                if (_testChannel3Low != value)
                {
                    if (_testChannel3Low != null)
                    {
                        _testChannel3Low.ListChanged -= _testChannel3Low_ListChanged;
                    }
                    _testChannel3Low = value;
                    if (_testChannel3Low != null)
                    {
                        _testChannel3Low = new BindingList<int>(_testChannel3Low.Distinct().ToList());
                        _testChannel3Low.ListChanged += _testChannel3Low_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel3Low;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 4번 채널 High 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel4High
        {
            get => _testChannel4High;
            set
            {
                if (_testChannel4High != value)
                {
                    if (_testChannel4High != null)
                    {
                        _testChannel4High.ListChanged -= _testChannel4High_ListChanged;
                    }
                    _testChannel4High = value;
                    if (_testChannel4High != null)
                    {
                        _testChannel4High = new BindingList<int>(_testChannel4High.Distinct().ToList());
                        _testChannel4High.ListChanged += _testChannel4High_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel4High;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("디바이스의 4번 채널 Low 에 연결될 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannel4Low
        {
            get => _testChannel4Low;
            set
            {
                if (_testChannel4Low != value)
                {
                    if (_testChannel4Low != null)
                    {
                        _testChannel4Low.ListChanged -= _testChannel4Low_ListChanged;
                    }
                    _testChannel4Low = value;
                    if (_testChannel4Low != null)
                    {
                        _testChannel4Low = new BindingList<int>(_testChannel4Low.Distinct().ToList());
                        _testChannel4Low.ListChanged += _testChannel4Low_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannel4Low;

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

        private void _testChannel3High_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel3High));
        }
        private void _testChannel3Low_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel3Low));
        }
        private void _testChannel4High_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel4High));
        }
        private void _testChannel4Low_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannel4Low));
        }


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

        internal override string CategoryName => StepCategory.Oscilloscope.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case OscopeTestMode.Save:
                    case OscopeTestMode.Recall:
                        return $"Location={(int)StateLocation}";
                    case OscopeTestMode.Measure:
                        return MeasurementSettings.Method.ToString();
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc
        {
            get
            {
                PhysicalUnit unit;
                switch (TestMethod)
                {
                    case OscopeTestMode.DigitalVoltmeter:
                        unit = GetPhysicalUnit();
                        var valueText = GetPrefixExpression(ExpectedValue, unit, out MetricPrefix prefix);
                        return $"{valueText}{prefix.GetText()}{unit.GetText()}";
                    case OscopeTestMode.Measure:
                        if (MeasurementSettings.Method != OscopeMeasurementSettings.MeasureMethod.Clear)
                        {
                            unit = GetPhysicalUnit();
                            valueText = GetPrefixExpression(ExpectedValue, unit, out prefix);
                            return $"{valueText}{prefix.GetText()}{unit.GetText()}";
                        }
                        break;
                }

                return "";
            }
        }

        public override string TolerancePlusDesc
        {
            get
            {
                PhysicalUnit unit;
                switch (TestMethod)
                {
                    case OscopeTestMode.Measure:
                        if (MeasurementSettings.Method != OscopeMeasurementSettings.MeasureMethod.Clear)
                        {
                            unit = GetPhysicalUnit();
                            return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                                TolerancePlusAbsolute, TolerancePlusMinusAbsolute, unit);
                        }
                        break;
                    case OscopeTestMode.DigitalVoltmeter:
                        unit = GetPhysicalUnit();
                        return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                            TolerancePlusAbsolute, TolerancePlusMinusAbsolute, unit);
                }

                return "";
            }
        }

        public override string ToleranceMinusDesc
        {
            get
            {
                PhysicalUnit unit;
                switch (TestMethod)
                {
                    case OscopeTestMode.Measure:
                        if (MeasurementSettings.Method != OscopeMeasurementSettings.MeasureMethod.Clear)
                        {
                            unit = GetPhysicalUnit();
                            return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent,
                                ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, unit);
                        }
                        break;
                    case OscopeTestMode.DigitalVoltmeter:
                        unit = GetPhysicalUnit();
                        return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent,
                            ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, unit);
                }

                return "";
            }
        }

        public override List<int> AllTestChannels
        {
            get
            {
                switch (TestMethod)
                {
                    case OscopeTestMode.Capture:
                    case OscopeTestMode.BeginCapture:
                    case OscopeTestMode.DigitalVoltmeter:
                        var channels = new List<int>();
                        var testChannels = new BindingList<int>[] { 
                            TestChannel1High, TestChannel1Low, TestChannel2High, TestChannel2Low, 
                            TestChannel3High, TestChannel3Low, TestChannel4High, TestChannel4Low,
                        };
                        foreach (var chList in testChannels)
                        {
                            if (chList != null)
                            {
                                channels.AddRange(chList);
                            }
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
        private EolOscopeStep()
        {
            Name = StepCategory.Oscilloscope.GetText();

            _channel1Settings.PropertyChanged += _channel1Settings_PropertyChanged;
            _channel2Settings.PropertyChanged += _channel2Settings_PropertyChanged;
            _channel3Settings.PropertyChanged += _channel3Settings_PropertyChanged;
            _channel4Settings.PropertyChanged += _channel4Settings_PropertyChanged;
            _triggerSettings.PropertyChanged += _triggerSettings_PropertyChanged;
            _acquireSettings.PropertyChanged += _acquireSettings_PropertyChanged;
            _measurementSettings.PropertyChanged += _measurementSettings_PropertyChanged;
            _digitalVoltmeterSettings.PropertyChanged += _digitalVoltmeterSettings_PropertyChanged;
            _displaySettings.PropertyChanged += _displaySettings_PropertyChanged;
        }

        public EolOscopeStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return OscopeDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetOscopeSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.Oscilloscope, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.Keysight_InfiniiVision_3000T_X:
                        return new object[]
                        {
                            OscopeTestMode.ReadIDN,
                            OscopeTestMode.Reset,
                            OscopeTestMode.Save,
                            OscopeTestMode.Recall,
                            OscopeTestMode.Configure,
                            OscopeTestMode.Capture,
                            OscopeTestMode.BeginCapture,
                            OscopeTestMode.EndCapture,
                            OscopeTestMode.Measure,
                            OscopeTestMode.DigitalVoltmeter,
                            OscopeTestMode.Download,
                            OscopeTestMode.RunCommandLine,
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
                if (Enum.TryParse(stringValue, out OscopeTestMode parsed))
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
                case OscopeTestMode.Capture:
                case OscopeTestMode.BeginCapture:
                case OscopeTestMode.DigitalVoltmeter:
                    if (setting is OscopeDeviceSetting oscopeSetting)
                    {
                        var args = new List<IEnumerable<int>>();
                        bool isCapture = TestMethod == OscopeTestMode.Capture || TestMethod == OscopeTestMode.BeginCapture;

                        // CH1.
                        if (isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH1) || 
                            TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel1)
                        {
                            // CH1 High channels.
                            if (oscopeSetting.Channel1High > 0 && TestChannel1High?.Count > 0)
                            {
                                args.Add(TestChannel1High.Concat(new int[] { oscopeSetting.Channel1High }));
                            }

                            // CH1 Low channels.
                            if (oscopeSetting.Channel1Low > 0 && TestChannel1Low?.Count > 0)
                            {
                                args.Add(TestChannel1Low.Concat(new int[] { oscopeSetting.Channel1Low }));
                            }
                        }

                        // CH2.
                        if (isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH2) || 
                            TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel2)
                        {
                            // CH2 High channels.
                            if (oscopeSetting.Channel2High > 0 && TestChannel2High?.Count > 0)
                            {
                                args.Add(TestChannel2High.Concat(new int[] { oscopeSetting.Channel2High }));
                            }

                            // CH2 Low channels.
                            if (oscopeSetting.Channel2Low > 0 && TestChannel2Low?.Count > 0)
                            {
                                args.Add(TestChannel2Low.Concat(new int[] { oscopeSetting.Channel2Low }));
                            }
                        }

                        // CH3.
                        if (isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH3) ||
                            TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel3)
                        {
                            // CH3 High channels.
                            if (oscopeSetting.Channel3High > 0 && TestChannel3High?.Count > 0)
                            {
                                args.Add(TestChannel3High.Concat(new int[] { oscopeSetting.Channel3High }));
                            }

                            // CH3 Low channels.
                            if (oscopeSetting.Channel3Low > 0 && TestChannel3Low?.Count > 0)
                            {
                                args.Add(TestChannel3Low.Concat(new int[] { oscopeSetting.Channel3Low }));
                            }
                        }

                        // CH4.
                        if (isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH4) ||
                            TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel4)
                        {
                            // CH4 High channels.
                            if (oscopeSetting.Channel4High > 0 && TestChannel4High?.Count > 0)
                            {
                                args.Add(TestChannel4High.Concat(new int[] { oscopeSetting.Channel4High }));
                            }

                            // CH4 Low channels.
                            if (oscopeSetting.Channel4Low > 0 && TestChannel4Low?.Count > 0)
                            {
                                args.Add(TestChannel4Low.Concat(new int[] { oscopeSetting.Channel4Low }));
                            }
                        }

                        // Relay ON.
                        ElozDevice.SharedInstance.ConnectChannels(elozTestSet, args.ToArray());
                    }
                    break;
            }
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            if (TestMethod == OscopeTestMode.Measure && MeasurementSettings.Method != OscopeMeasurementSettings.MeasureMethod.Clear 
                || TestMethod == OscopeTestMode.DigitalVoltmeter)
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

            var oscope = device as OscopeDevice;
            if (oscope == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case OscopeTestMode.ReadIDN:
                    result.ResultInfo = oscope.ReadIDN(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Reset:
                    oscope.Reset(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Save:
                    oscope.Save(StateLocation, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Recall:
                    oscope.Recall(StateLocation, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Configure:
                    var chSettings = new List<OscopeChannelSettings>
                    {
                        Channel1Settings,
                        Channel2Settings,
                        Channel3Settings,
                        Channel4Settings
                    };
                    oscope.Configure(chSettings, TriggerSettings, AcquireSettings, TimebaseSettings, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Capture:
                case OscopeTestMode.BeginCapture:
                    // Channels.
                    var channels = new List<OscopeDevice.OscopeChannel>();
                    if (CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH1))
                    {
                        channels.Add(OscopeDevice.OscopeChannel.Channel1);
                    }
                    if (CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH2))
                    {
                        channels.Add(OscopeDevice.OscopeChannel.Channel2);
                    }
                    if (CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH3))
                    {
                        channels.Add(OscopeDevice.OscopeChannel.Channel3);
                    }
                    if (CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH4))
                    {
                        channels.Add(OscopeDevice.OscopeChannel.Channel4);
                    }

                    if (TestMethod == OscopeTestMode.Capture)
                    {
                        oscope.Capture(AutoScale, channels, token);
                    }
                    else
                    {
                        oscope.BeginCapture(AutoScale, channels, token);
                    }

                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.EndCapture:
                    oscope.EndCapture(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case OscopeTestMode.Measure:
                    double? resultValue = oscope.Measure(MeasurementSettings, token);
                    if (resultValue != null)
                    {
                        UpdateResult(result, resultValue ?? 0);
                        if (MeasurementSettings.Temp && result.ResultValueState != ResultValueState.Good)
                        {
                            // HACK: Method == Temp 인 경우 가성 데이터 생성.
                            var randomValue = GetTempValue(Tolerance, ExpectedValue, TolerancePlusMinusAbsolute, TolerancePlusAbsolute,
                                ToleranceMinusAbsolute, TolerancePlusMinusPercent, TolerancePlusPercent, ToleranceMinusPercent);
                            resultValue = Math.Round(randomValue, 3) + MeasureOffset;
                            UpdateResult(result, resultValue.GetValueOrDefault());
                        }
                    }
                    else
                    {
                        if (MeasurementSettings.Method == OscopeMeasurementSettings.MeasureMethod.Clear)
                        {
                            result.ResultState = ResultState.Pass;
                        }
                    }
                    break;
                case OscopeTestMode.DigitalVoltmeter:
                    var measuredVolt = oscope.DigitalVoltmeter(DigitalVoltmeterSettings, token);
                    UpdateResult(result, measuredVolt);
                    break;
                case OscopeTestMode.Download:
                    // 윈도우에 다운로드한 이미지를 보여주는 경우, 윈도우 크기 조정 방식 지정.
                    if (DisplaySettings.DownloadCompleteActions.HasFlag(OscopeDisplaySettings.ImageOperations.DisplayAsWindow))
                    {
                        ImageViewer.DisplaySizeMode = DisplaySettings.ImageDisplayMode;
                        ImageViewer.DisplayLocation = DisplaySettings.ImageDisplayLocation;
                        ImageViewer.DisplaySize = DisplaySettings.ImageDisplaySize;
                        ImageViewer.AutoCloseDelay = DisplaySettings.ImageDisplayCloseDelay;
                    }

                    if (DisplaySettings.DownloadAsync)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                Download(oscope, result, token);
                            }
                            catch (Exception ex)
                            {
                                Logger.LogError("Scope download error: " + ex.Message);
                                result.ResultInfo = ex.Message;
                                result.ResultState = ResultState.Fail;
                            }
                        });
                    }
                    else
                    {
                        Download(oscope, result, token);
                    }
                    break;
                case OscopeTestMode.RunCommandLine:
                    var response = oscope.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, 2000, token);
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

        // 기준값과 측정오차, 측정값으로부터 결과 Pass/Fail, Value 상태를 결정한다.
        private void UpdateResult(TestResult result, double value)
        {
            double measuredValue = value - MeasureOffset;
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

        // 스코프 스크린 이미지를 다운로드한다.
        private void Download(OscopeDevice oscope, TestResult result, CancellationToken token)
        {
            var imageData = oscope.Download(DisplaySettings, token);
            if (result != null && imageData != null)
            {
                result.ResultData = Image.FromStream(new MemoryStream(imageData));
            }

            if (DisplaySettings.DownloadCompleteActions.HasFlag(OscopeDisplaySettings.ImageOperations.SaveToFile))
            {
                // 다운로드한 이미지 보관.
                var dateTimeText = DateTime.Now.ToString("yyyy-MM-dd_HHmmss");
                string downloadFile;
                string filePath = DisplaySettings.DownloadFile?.Trim();
                if (!string.IsNullOrWhiteSpace(filePath))
                {
                    if (DisplaySettings.DownloadFileAppendTime)
                    {
                        var fileName = Path.GetFileNameWithoutExtension(filePath) + "_" + dateTimeText;
                        var fileExt = Path.GetExtension(filePath);
                        var fullName = fileName + (string.IsNullOrEmpty(fileExt) ? "" : fileExt);
                        downloadFile = Path.Combine(Path.GetDirectoryName(filePath), fullName);
                    }
                    else
                    {
                        downloadFile = filePath;
                    }

                    Directory.CreateDirectory(Path.GetDirectoryName(downloadFile));
                }
                else
                {
                    downloadFile = "D:\\ElozPlugin\\OscilloscopeScreen\\" + dateTimeText + ".png"; ;
                    Directory.CreateDirectory(Path.GetDirectoryName(downloadFile));
                }
                File.WriteAllBytes(downloadFile, imageData ?? new byte[0]);

                if (result != null)
                {
                    result.ResultInfo = downloadFile;
                }
            }

            if (result != null)
            {
                result.ResultState = ResultState.Pass;
            }
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            if (TestMethod != OscopeTestMode.Measure && TestMethod != OscopeTestMode.DigitalVoltmeter)
            {
                return PhysicalUnit.None;
            }

            if (TestMethod == OscopeTestMode.DigitalVoltmeter)
            {
                return PhysicalUnit.Volt;
            }

            // Measure.
            switch (MeasurementSettings.Method)
            {
                case OscopeMeasurementSettings.MeasureMethod.Clear:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.BitRate:
                    return PhysicalUnit.Hertz;
                case OscopeMeasurementSettings.MeasureMethod.BurstWidth:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.Counter:
                    return PhysicalUnit.Hertz;
                case OscopeMeasurementSettings.MeasureMethod.Delay:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.PositiveDutyCycle:
                    return PhysicalUnit.Percent;
                case OscopeMeasurementSettings.MeasureMethod.FallTime:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.Frequency:
                    return PhysicalUnit.Hertz;
                case OscopeMeasurementSettings.MeasureMethod.NegativeDutyCycle:
                    return PhysicalUnit.Percent;
                case OscopeMeasurementSettings.MeasureMethod.FallingEdgeCount:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.FallingPulseCount:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.NegativePulseWidth:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.Overshoot:
                    return PhysicalUnit.Percent;
                case OscopeMeasurementSettings.MeasureMethod.RisingEdgeCount:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.Period:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.Phase:
                    return PhysicalUnit.Degree;
                case OscopeMeasurementSettings.MeasureMethod.RisingPulseCount:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.Preshoot:
                    return PhysicalUnit.Percent;
                case OscopeMeasurementSettings.MeasureMethod.PositivePulseWidth:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.RiseTime:
                    return PhysicalUnit.Second;
                case OscopeMeasurementSettings.MeasureMethod.StanardDeviation:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.VAmplitude:
                case OscopeMeasurementSettings.MeasureMethod.VAverage:
                case OscopeMeasurementSettings.MeasureMethod.VBase:
                case OscopeMeasurementSettings.MeasureMethod.VMax:
                case OscopeMeasurementSettings.MeasureMethod.VMin:
                case OscopeMeasurementSettings.MeasureMethod.VPP:
                case OscopeMeasurementSettings.MeasureMethod.VRMS:
                case OscopeMeasurementSettings.MeasureMethod.VTop:
                    return PhysicalUnit.Volt;
                case OscopeMeasurementSettings.MeasureMethod.XMax:
                case OscopeMeasurementSettings.MeasureMethod.XMin:
                case OscopeMeasurementSettings.MeasureMethod.YatX:
                    return PhysicalUnit.None;
                case OscopeMeasurementSettings.MeasureMethod.VRatio:
                    return PhysicalUnit.Decibel;
                default:
                    return PhysicalUnit.None;
            }
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();

            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            bool visible = browsable ?? false;
            Utils.SetBrowsableAttribute(MeasurementSettings, nameof(OscopeMeasurementSettings.Temp), visible);
            Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), visible);
        }

        public override bool ShouldShowResultImage()
        {
            return TestMethod == OscopeTestMode.Download
                && DisplaySettings.DownloadCompleteActions.HasFlag(OscopeDisplaySettings.ImageOperations.DisplayAsWindow);
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            UpdateToleranceAttributes();
            Utils.SetBrowsableAttribute(this, nameof(StateLocation), TestMethod == OscopeTestMode.Save || TestMethod == OscopeTestMode.Recall);
            Utils.SetBrowsableAttribute(this, nameof(Channel1Settings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(Channel2Settings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(Channel3Settings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(Channel4Settings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(TriggerSettings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(AcquireSettings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(TimebaseSettings), TestMethod == OscopeTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(MeasurementSettings), TestMethod == OscopeTestMode.Measure);
            Utils.SetBrowsableAttribute(this, nameof(DigitalVoltmeterSettings), TestMethod == OscopeTestMode.DigitalVoltmeter);
            Utils.SetBrowsableAttribute(this, nameof(DisplaySettings), TestMethod == OscopeTestMode.Download);
            bool isCapture = TestMethod == OscopeTestMode.Capture || TestMethod == OscopeTestMode.BeginCapture;
            Utils.SetBrowsableAttribute(this, nameof(CaptureSources), isCapture);
            Utils.SetBrowsableAttribute(this, nameof(AutoScale), isCapture);

            bool showChannel1 = isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH1) ||
                TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel1;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel1High), showChannel1);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel1Low), showChannel1);

            bool showChannel2 = isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH2) ||
                TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel2;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel2High), showChannel2);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel2Low), showChannel2);

            bool showChannel3 = isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH3) ||
                TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel3;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel3High), showChannel3);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel3Low), showChannel3);

            bool showChannel4 = isCapture && CaptureSources.HasFlag(OscopeDevice.OscopeChannels.CH4) ||
                TestMethod == OscopeTestMode.DigitalVoltmeter && DigitalVoltmeterSettings.Source == OscopeDevice.OscopeChannel.Channel4;
            Utils.SetBrowsableAttribute(this, nameof(TestChannel4High), showChannel4);
            Utils.SetBrowsableAttribute(this, nameof(TestChannel4Low), showChannel4);

            bool runCmdLine = TestMethod == OscopeTestMode.RunCommandLine;
            Utils.SetBrowsableAttribute(this, nameof(CommandLine), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), runCmdLine);
            bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
            Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), runCmdLine && compareResponse);
            Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), runCmdLine && compareResponse);
        }

        protected override void UpdateToleranceAttributes()
        {
            bool browsable = TestMethod == OscopeTestMode.Measure 
                && MeasurementSettings.Method != OscopeMeasurementSettings.MeasureMethod.Clear
                || TestMethod == OscopeTestMode.DigitalVoltmeter;
            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), browsable);
            //Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), browsable);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), browsable && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), browsable && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), browsable && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), browsable && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), browsable && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), browsable && Tolerance == ToleranceMode.Absolute);

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
            var newStep = new EolOscopeStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolOscopeStep destStep)
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
                destStep.StateLocation = StateLocation;

                destStep.Channel1Settings = Channel1Settings.Clone() as OscopeChannelSettings;
                destStep.Channel2Settings = Channel2Settings.Clone() as OscopeChannelSettings;
                destStep.Channel3Settings = Channel3Settings.Clone() as OscopeChannelSettings;
                destStep.Channel4Settings = Channel4Settings.Clone() as OscopeChannelSettings;
                destStep.TriggerSettings = TriggerSettings.Clone() as OscopeTriggerSettings;
                destStep.AcquireSettings = AcquireSettings.Clone() as OscopeAcquireSettings;
                destStep.TimebaseSettings = TimebaseSettings.Clone() as OscopeTimebaseSettings;
                destStep.MeasurementSettings = MeasurementSettings.Clone() as OscopeMeasurementSettings;
                destStep.DigitalVoltmeterSettings = DigitalVoltmeterSettings.Clone() as OscopeDvmSettings;
                destStep.DisplaySettings = DisplaySettings.Clone() as OscopeDisplaySettings;
                destStep.CaptureSources = CaptureSources;
                destStep.AutoScale = AutoScale;

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
                if (TestChannel3High != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel3High)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel3High = channels;
                }
                if (TestChannel3Low != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel3Low)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel3Low = channels;
                }
                if (TestChannel4High != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel4High)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel4High = channels;
                }
                if (TestChannel4Low != null)
                {
                    var channels = new BindingList<int>();
                    foreach (var channel in TestChannel4Low)
                    {
                        channels.Add(channel);
                    }
                    destStep.TestChannel4Low = channels;
                }

                destStep.CommandLine = CommandLine;
                destStep.ResponseCheckMethod = ResponseCheckMethod;
                destStep.ResponsePattern = ResponsePattern;
                destStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;
            }

            dest.UpdateBrowsableAttributes();
        }

        /// <summary>
        /// Oscilloscope 기능 리스트.
        /// </summary>
        public enum OscopeTestMode
        {
            ReadIDN,
            Reset,
            Save,
            Recall,
            Configure,
            Capture,
            BeginCapture,
            EndCapture,
            Measure,
            DigitalVoltmeter,
            Download,
            RunCommandLine,
        }
    }
}
