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
    public class OscopeChannelSettings : INotifyPropertyChanged, ICloneable
    {
        [DefaultValue(false), DisplayName(nameof(BandwidthLimit) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n내부 저주파 통과 필터를 제어합니다.\r\n" +
            "필터가 켜지면(True) 지정된 채널의 대역폭은 대략 25 MHz 로 제한됩니다.")]
        public bool? BandwidthLimit
        {
            get => _bandwidthLimit;
            set
            {
                if (_bandwidthLimit != value)
                {
                    _bandwidthLimit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _bandwidthLimit = null;

        public enum CouplingMode
        {
            AC,
            DC,
        }

        [DefaultValue(CouplingMode.DC), DisplayName(nameof(Coupling) + " [Default = " + nameof(CouplingMode.DC) + "]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n지정된 채널의 입력 커플링을 선택합니다. " +
            "각 아날로그 채널의 커플링은 AC 또는 DC 로 설정할 수 있습니다.")]
        public CouplingMode? Coupling
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
        private CouplingMode? _coupling = null;

        [DefaultValue(true), DisplayName(nameof(Display) + " [Default = CH1:True, CHn:False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n지정된 채널의 디스플레이를 켜거나(True) 끌 수(False) 있습니다.")]
        public bool? Display
        {
            get => _display;
            set
            {
                if (_display != value)
                {
                    _display = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _display = null;

        [TypeConverter(typeof(DescEnumConverter))]
        public enum ImpedanceMode
        {
            [Description("1 MΩ")]
            _1_MΩ,

            [Description("50 Ω")]
            _50_Ω,
        }

        [DefaultValue(ImpedanceMode._1_MΩ), DisplayName(nameof(Impedance) + " [Default = 1 MΩ"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n지정된 아날로그 채널의 입력 임피던스를 선택합니다.")]
        public ImpedanceMode? Impedance
        {
            get => _impedance;
            set
            {
                if (_impedance != value)
                {
                    _impedance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ImpedanceMode? _impedance = null;

        [DefaultValue(false), DisplayName(nameof(Invert) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지정된 채널의 입력 신호를 반전시킬지 여부를 선택합니다. 반전시키려면 True, 반전시키지 않으려면 False로 설정하세요.")]
        public bool? Invert
        {
            get => _invert;
            set
            {
                if (_invert != value)
                {
                    _invert = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _invert = null;

        [Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "아날로그 채널의 라벨 문자열을 설정합니다. 라벨 문자열은 32개 이하의 공용 ASCII 문자들을 포함할 수 있습니다. " +
            "32개보다 많은 글자수의 라벨은 32개로 잘립니다. 소문자들은 대문자들로 변환됩니다.")]
        public string Label
        {
            get => _label;
            set
            {
                if (_label != value)
                {
                    _label = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _label = null;

        [DisplayName(nameof(Offset) + " [V] [Default = 0]"), DefaultValue(0.0), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "선택된 채널의 화면 중심에 표시될 값을 설정합니다. " +
            "유효한 값 범위는 Range와 Scale 파라미터 설정에 따라 달라집니다. " +
            "만일 이 값을 유효한 값 범위 밖으로 설정하면 자동적으로 가장 가까운 유효한 값으로 설정됩니다. " +
            "유효한 값은 프로브 감쇠 설정에 의해 영향을 받습니다.")]
        public double? Offset
        {
            get => _offset;
            set
            {
                if (_offset != value)
                {
                    _offset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _offset = null;

        [DisplayName(nameof(ProbeAttenuationFactor) + " [Default = 10]"), DefaultValue(10.0), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지정된 채널의 프로브 감쇠 계수를 설정합니다. " +
            "프로브 감쇠 계수는 0.001 ~ 10000 사이의 값이 될 수 있습니다. 이 값은 오실로스코프의 실제 입력 감도를 변화시키지 않습니다. " +
            "이 값은 표시 계수의 스케일링, 자동 측정 및 트리거 레벨 설정에 대한 기준 상수를 변경합니다. " +
            "자동 프로브가 오실로스코프에 연결된 경우 감쇠 값을 감지된 값에서 변경할 수 없습니다. " +
            "오실로스코프를 감지된 값이 아닌 감쇠 값으로 설정하려고 하면 오류가 발생합니다.")]
        public double? ProbeAttenuationFactor
        {
            get => _probeAttenuationFactor;
            set
            {
                if (_probeAttenuationFactor != value)
                {
                    _probeAttenuationFactor = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _probeAttenuationFactor = null;

        [DefaultValue(false), DisplayName(nameof(ProbeExternal) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "아날로그 입력 채널에서 외부 스케일링을 활성화하거나 비활성화합니다. " +
            "외부 스케일링은 프로브 시스템의 추가 감쇠기, 어댑터 등을 고려하여 입력 채널에 추가 gain을 적용할 수 있습니다.")]
        public bool? ProbeExternal
        {
            get => _probeExternal;
            set
            {
                if (_probeExternal != value)
                {
                    _probeExternal = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _probeExternal = null;

        [DefaultValue(1.0), DisplayName(nameof(ProbeExternalGain) + " [Default = 1]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "외부 스케일링과 관련된 gain을 설정합니다. gain은 0.0001 ~ 1000 사이 값이 될 수 있습니다.")]
        public double? ProbeExternalGain
        {
            get => _probeExternalGain;
            set
            {
                if (_probeExternalGain != value)
                {
                    _probeExternalGain = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _probeExternalGain = null;

        [DefaultValue(MeasurementUnit.Volt), DisplayName(nameof(ProbeExternalUnit) + " [Default = " + nameof(MeasurementUnit.Volt) + "]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "연결된 프로브의 측정 단위를 설정합니다. " +
            "전압 프로브에 대해서는 볼트를, 전류 프로브에 대해서는 암페어를 설정하세요. " +
            "측정 결과, 채널 감도, 트리거 레벨은 선택한 측정단위를 반영합니다.")]
        public MeasurementUnit? ProbeExternalUnit
        {
            get => _probeExternalUnit;
            set
            {
                if (_probeExternalUnit != value)
                {
                    _probeExternalUnit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private MeasurementUnit? _probeExternalUnit = null;

        [DisplayName(nameof(ProbeSkew) + " [s] [Default = 0]"), DefaultValue(0.0), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지정된 채널의 채널 간 왜곡 계수를 설정합니다. " +
            "각 아날로그 채널은 + 또는 - 100 ns, 총 200 ns 의 채널 간 차이에 대해 조정될 수 있습니다. " +
            "오실로스코프의 프로브 왜곡 제어를 통해 채널 간 케이블 지연 오류를 제거할 수 있습니다.")]
        public double? ProbeSkew
        {
            get => _probeSkew;
            set
            {
                if (_probeSkew != value)
                {
                    _probeSkew = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _probeSkew = null;

        [DisplayName(nameof(Range) + " [V] [Default = 40]"), DefaultValue(40.0), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "선택한 채널의 전체 수직 축을 정의합니다. 1:1 프로브 감쇠를 사용하는 경우, 유효한 범위는 8mV ~ 40V입니다. " +
            "프로브 감쇠가 변경되면 범위 값에 프로브 감쇠 계수가 곱됩니다.")]
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

        [DisplayName(nameof(Scale) + " [V] [Default = 5]"), DefaultValue(5.0), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "선택한 채널의 수직 스케일 또는 눈금당 단위를 설정합니다. 프로브 감쇠가 변경되면 스케일 값에 프로브의 감쇠 계수가 곱됩니다.")]
        public double? Scale
        {
            get => _scale;
            set
            {
                if (_scale != value)
                {
                    _scale = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _scale = null;

        [DefaultValue(MeasurementUnit.Volt), DisplayName(nameof(Unit) + " [Default = " + nameof(MeasurementUnit.Volt) + "]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "연결된 프로브의 측정 단위를 설정합니다. " +
            "전압 프로브에 대해서는 볼트를, 전류 프로브에 대해서는 암페어를 설정하세요. " +
            "측정 결과, 채널 감도, 트리거 레벨은 선택한 측정단위를 반영합니다.")]
        public MeasurementUnit? Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private MeasurementUnit? _unit = null;

        [DefaultValue(false), DisplayName(nameof(Vernier) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "채널의 버니어(미세 수직 조정) 설정이 ON(True)인지 OFF(False)인지를 지정합니다.")]
        public bool? Vernier
        {
            get => _vernier;
            set
            {
                if (_vernier != value)
                {
                    _vernier = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _vernier = null;

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
            var obj = new OscopeChannelSettings();
            obj.BandwidthLimit = BandwidthLimit;
            obj.Coupling = Coupling;
            obj.Display = Display;
            obj.Impedance = Impedance;
            obj.Invert = Invert;
            obj.Label = Label;
            obj.Offset = Offset;
            obj.ProbeAttenuationFactor = ProbeAttenuationFactor;
            obj.ProbeExternal = ProbeExternal;
            obj.ProbeExternalGain = ProbeExternalGain;
            obj.ProbeExternalUnit = ProbeExternalUnit;
            obj.ProbeSkew = ProbeSkew;
            obj.Range = Range;
            obj.Scale = Scale;
            obj.Unit = Unit;
            obj.Vernier = Vernier;
            return obj;
        }

        public enum MeasurementUnit
        {
            Volt,
            Ampere,
        }
    }
}
