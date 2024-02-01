using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OscopeTriggerSettings : INotifyPropertyChanged, ICloneable
    {
        [DefaultValue(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "트리거 조건이 충족되지 않았더라도 캡처되도록 합니다. 이 명령은 전면 패널 [Force Trigger] 키에 해당합니다.")]
        public bool Force
        {
            get => _force;
            set
            {
                if (_force != value)
                {
                    _force = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _force = false;

        [DefaultValue(false), DisplayName(nameof(HighFrequencyRejectFilter) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "고주파 제거 필터를 켜거나 끕니다. 고주파 제거 필터는 트리거 경로에 50kHz 저주파 통과 필터를 추가하여 트리거 파형에서 " +
            "고주파 성분을 제거합니다. 이 필터를 사용하여 트리거 경로에서 AM 또는 FM 방송국과 같은 고주파 잡음을 제거합니다.")]
        public bool? HighFrequencyRejectFilter
        {
            get => _highFrequencyRejectFilter;
            set
            {
                if (_highFrequencyRejectFilter != value)
                {
                    _highFrequencyRejectFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _highFrequencyRejectFilter = null;

        [DisplayName(nameof(HoldoffTime) + " [s] [Default = 40n]"), DefaultValue(4.0E-8), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "홀드오프 시간 값을 초 단위로 정의합니다. 홀드오프는 마지막 트리거 이후 일정 시간이 경과할 때까지 트리거가 발생하지 않도록 합니다. " +
            "이 기능은 파형이 파형의 한 주기 동안 트리거 레벨을 여러 번 교차하는 경우에 유용합니다. 홀드오프가 없으면 오실로스코프가 각 교차로에서 " +
            "트리거하여 혼란스러운 파형을 생성할 수 있습니다. 홀드오프가 올바르게 설정된 경우 오실로스코프는 항상 동일한 교차로에서 트리거합니다. " +
            "올바른 홀드오프 설정은 일반적으로 1주기보다 약간 작습니다.")]
        public double? HoldoffTime
        {
            get => _holdoffTime;
            set
            {
                if (_holdoffTime != value)
                {
                    _holdoffTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _holdoffTime = null;

        [DefaultValue(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "표시되는 모든 아날로그 채널의 트리거 레벨을 파형의 50% 값으로 자동 설정합니다. AC 커플링을 사용하는 경우 트리거 레벨이 0V로 설정됩니다. " +
            "높음 및 낮음(듀얼) 트리거 레벨을 사용하는 경우(예를 들어 상승/하강 시간 및 런트 트리거와 마찬가지로) 이 명령은 아무런 영향을 미치지 않습니다.")]
        public bool LevelAutoSetup
        {
            get => _levelAutoSetup;
            set
            {
                if (_levelAutoSetup != value)
                {
                    _levelAutoSetup = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _levelAutoSetup = false;

        public enum TriggerMode
        {
            Edge,
            //Glitch,
            //Pattern,
            //TV,
            //Delay,
            //NthEdgeBurst,
            //ORedEdge,
            //Runt,
            //Shold,
            //Transition,
            //SBus1,
            //SBus2,
            //NFC,
            //USB,
        }

        [DefaultValue(TriggerMode.Edge),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            " • Edge triggering — 파형에서 지정된 기울기와 전압 레벨을 찾아 트리거를 식별합니다.\r\n" +
            " • Nth Edge Burst triggering — 유휴 시간 후 발생하는 버스트의 N번째 에지에서 트리거할 수 있습니다.\r\n" +
            " • Pulse width triggering — 오실로스코프가 지정된 폭의 양의 펄스 또는 음의 펄스에 트리거하도록 설정합니다.\r\n" +
            " • Pattern triggering — 지정된 패턴을 찾아 트리거 조건을 식별합니다. 이 패턴은 채널의 논리적 AND 조합입니다. 또한 패턴의 지정된 시간에 트리거할 수 있습니다.\r\n" +
            " • TV triggering — 텔레비전 장비의 복잡한 파형을 캡처하는 데 사용됩니다. 트리거 회로는 파형의 수직 및 수평 간격을 감지하고 선택한 " +
            "TV 트리거 설정을 기반으로 트리거를 생성합니다. TV 트리거링을 사용하려면 아날로그 채널을 트리거 소스로 사용하여 동기 진폭을 1/4 이상 분할해야 합니다.\r\n" +
            " • USB(Universal Serial Bus) triggering — 차동 USB 데이터 라인의 패킷 시작(SOP), 패킷 종료(EOP), 리셋 완료, 일시 중단 시작 또는 일시 중단 종료 " +
            "신호에서 트리거됩니다. 이 트리거는 USB 저속 및 최대 속도를 지원합니다.")]
        public TriggerMode Mode
        {
            get => _mode;
            set
            {
                if (_mode != value)
                {
                    _mode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private TriggerMode _mode = TriggerMode.Edge;

        [Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "에지 모드 트리거를 설정합니다.")]
        public EdgeSettings EdgeModeSettings
        {
            get => _edgeModeSettings;
            set
            {
                if (_edgeModeSettings != value)
                {
                    if (_edgeModeSettings != null)
                    {
                        _edgeModeSettings.PropertyChanged -= EdgeModeSettings_PropertyChanged;
                    }
                    _edgeModeSettings = value;
                    if (_edgeModeSettings != null)
                    {
                        _edgeModeSettings.PropertyChanged += EdgeModeSettings_PropertyChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeSettings _edgeModeSettings = new EdgeSettings();

        private void EdgeModeSettings_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(EdgeModeSettings));
        }


        [DefaultValue(false), DisplayName(nameof(NoiseRejectFilter) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "노이즈 제거 필터를 끄거나 켭니다. 노이즈 제거 필터가 켜져 있으면 트리거 회로가 노이즈에 덜 민감하지만 오실로스코프를 " +
            "트리거하려면 더 큰 진폭의 파형이 필요할 수 있습니다. 이 명령은 TV 트리거 모드에서 사용할 수 없습니다.")]
        public bool? NoiseRejectFilter
        {
            get => _noiseRejectFilter;
            set
            {
                if (_noiseRejectFilter != value)
                {
                    _noiseRejectFilter = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _noiseRejectFilter = null;

        public enum SweepMode
        {
            Auto,
            Normal
        }

        [DefaultValue(SweepMode.Auto), DisplayName(nameof(Sweep) + " [Default = " + nameof(SweepMode.Auto) + "]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            " • Normal mode — 트리거 신호가 있고 트리거 조건이 충족되는 경우에만 파형을 표시합니다. 그렇지 않으면 오실로스코프가 " +
            "트리거하지 않고 디스플레이가 업데이트되지 않습니다. 이 모드는 낮은 반복 속도 신호에 유용합니다.\r\n" +
            " • Auto trigger mode — 트리거 사양이 사전 설정된 시간 내에 충족되지 않을 경우 인공 트리거 이벤트를 생성하고, 동기화되지 않은 데이터를 " +
            "수집하여 표시합니다. 자동 모드는 저반복 속도 신호 이외의 신호에 유용합니다. 트리거할 에지가 없으므로 DC 신호를 표시하려면 이 모드를 사용해야 합니다.")]
        public SweepMode? Sweep
        {
            get => _sweep;
            set
            {
                if (_sweep != value)
                {
                    _sweep = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private SweepMode? _sweep = null;

        public event PropertyChangedEventHandler PropertyChanged;

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        //   parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public OscopeTriggerSettings()
        {
            _edgeModeSettings.PropertyChanged += EdgeModeSettings_PropertyChanged;
        }

        public override string ToString()
        {
            return "";
        }

        public object Clone()
        {
            var obj = new OscopeTriggerSettings();
            obj.EdgeModeSettings = EdgeModeSettings.Clone() as EdgeSettings;
            obj.Force = Force;
            obj.HighFrequencyRejectFilter = HighFrequencyRejectFilter;
            obj.HoldoffTime = HoldoffTime;
            obj.LevelAutoSetup = LevelAutoSetup;
            obj.Mode = Mode;
            obj.NoiseRejectFilter = NoiseRejectFilter;
            obj.Sweep = Sweep;
            return obj;
        }

        [TypeConverter(typeof(ExpandableObjectConverter))]
        public class EdgeSettings : INotifyPropertyChanged, ICloneable
        {
            public enum InputCouplingMode
            {
                AC,
                DC,
                LowFrequencyReject,
            }

            [DefaultValue(InputCouplingMode.DC), DisplayName(nameof(Coupling) + " [Default = " + nameof(InputCouplingMode.DC) + "]"),
                Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
                "선택한 트리거 소스에 대한 입력 커플링을 설정합니다. 커플링은 AC, DC 또는 Low Frequency Reject로 설정할 수 있습니다.\r\n" +
                " • AC 커플링은 트리거 경로에 하이패스 필터(아날로그 채널의 경우 10Hz, 모든 외부 트리거 입력의 경우 3.5Hz)를 배치하여 트리거 파형에서 " +
                "DC 오프셋 전압을 제거합니다. AC 커플링을 사용하여 파형의 DC 오프셋이 클 때 안정적인 에지 트리거를 얻을 수 있습니다.\r\n" +
                " • DC 커플링을 사용하면 DC 및 AC 신호가 트리거 경로로 들어갈 수 있습니다.\r\n" +
                " • LowFrequencyReject 커플링은 트리거 경로에 50KHz 하이패스 필터를 배치합니다.\r\n" +
                "Coupling과 Reject 선택이 결합됩니다. Reject의 설정을 변경하면 Coupling 설정이 변경될 수 있습니다.")]
            public InputCouplingMode? Coupling
            {
                get => _coupling;
                set
                {
                    if (_coupling != value)
                    {
                        _coupling = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private InputCouplingMode? _coupling = null;

            [DisplayName(nameof(Level) + " [V] [Default = 0]"), DefaultValue(0.0), TypeConverter(typeof(PhysicalValueStaticConverter)), 
                Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
                "활성 트리거 소스의 트리거 레벨 전압을 설정합니다.\r\n" +
                " • 내부 트리거의 경우 화면 중심에서 0.75 x full-scale 전압\r\n" +
                " • 외부 트리거의 경우 ±(external range setting)\r\n" +
                " • 디지털 채널(MSO 모델) ±8 V")]
            public double? Level
            {
                get => _level;
                set
                {
                    if (_level != value)
                    {
                        _level = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private double? _level = null;

            public enum RejectMode
            {
                OFF,
                LowFrequencyReject,
                HighFrequencyReject,
            }

            [DefaultValue(RejectMode.OFF), DisplayName(nameof(Reject) + " [Default = " + nameof(RejectMode.OFF) + "]"),
                Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
                "저주파 또는 고주파 제거 필터를 켜거나 끕니다. 이 필터들 중 하나만 켤 수 있습니다.\r\n" +
                " • 고주파 제거 필터는 트리거 경로에 50kHz 저역 통과 필터를 추가하여 트리거 파형에서 고주파 성분을 제거합니다. " +
                "고주파 제거 필터를 사용하여 트리거 경로에서 AM 또는 FM 방송국과 같은 고주파 노이즈를 제거합니다.\r\n" +
                " • 저주파 제거 필터는 트리거 파형과 직렬로 50kHz 하이패스 필터를 추가하여 적절한 트리거링을 방해할 수 있는 전원 라인 " +
                "주파수와 같은 원하지 않는 저주파 성분을 트리거 파형에서 제거합니다.\r\n" +
                "Reject 및 Coupling 선택 항목이 결합됩니다. Coupling 설정을 변경하면 Reject 설정이 변경될 수 있습니다.")]
            public RejectMode? Reject
            {
                get => _reject;
                set
                {
                    if (_reject != value)
                    {
                        _reject = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private RejectMode? _reject = null;

            public enum SlopeMode
            {
                Negative,
                Positive,
                Either,
                Alternate,
            }

            [DefaultValue(SlopeMode.Positive), DisplayName(nameof(Slope) + " [Default = " + nameof(SlopeMode.Positive) + "]"),
                Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
                "트리거에 대한 에지의 기울기를 지정합니다. TV 트리거 모드에서는 이 값이 유효하지 않습니다.")]
            public SlopeMode? Slope
            {
                get => _slope;
                set
                {
                    if (_slope != value)
                    {
                        _slope = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private SlopeMode? _slope = null;

            [DefaultValue(OscopeDevice.OscopeChannel.Channel1), 
                DisplayName(nameof(Source) + " [Default = " + nameof(OscopeDevice.OscopeChannel.Channel1) + "]"),
                Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
                "트리거를 발생하는 입력 신호를 선택합니다.")]
            public OscopeDevice.OscopeChannel? Source
            {
                get => _source;
                set
                {
                    if (_source != value)
                    {
                        _source = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private OscopeDevice.OscopeChannel? _source = null;

            public override string ToString()
            {
                return "";
            }

            public event PropertyChangedEventHandler PropertyChanged;

            // This method is called by the Set accessor of each property.  
            // The CallerMemberName attribute that is applied to the optional propertyName  
            //   parameter causes the property name of the caller to be substituted as an argument.
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }

            public object Clone()
            {
                var obj = new EdgeSettings();
                obj.Coupling = Coupling;
                obj.Level = Level;
                obj.Reject = Reject;
                obj.Slope = Slope;
                obj.Source = Source;
                return obj;
            }
        }
    }
}
