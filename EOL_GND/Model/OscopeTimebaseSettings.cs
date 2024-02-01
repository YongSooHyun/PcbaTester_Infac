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
    public class OscopeTimebaseSettings : INotifyPropertyChanged, ICloneable
    {
        public enum TimebaseMode
        {
            Main,
            Window,
            XY,
            Roll
        }

        [DefaultValue(TimebaseMode.Main), DisplayName(nameof(Mode) + " [Default = " + nameof(TimebaseMode.Main) + "]"),
            Description("• MAIN — 표준 타임베이스 모드가 MAIN 타임베이스입니다. *RST(Reset) 명령 이후의 디폴트 타임베이스 모드입니다.\r\n" +
            "• Window — Window(확대 또는 지연) 타임베이스 모드에서는 가능한 경우 확대 타임베이스에서 측정이 이루어지며, 그렇지 않으면 주 타임베이스에서 측정이 이루어집니다.\r\n" +
            "• XY — XY 모드에서는 Range, Position, Reference 설정을 할 수 없습니다. 이 모드에서는 측정을 사용할 수 없습니다.\r\n" +
            "• Roll — Roll 모드에서는 데이터가 디스플레이 전체에서 왼쪽에서 오른쪽으로 계속 이동합니다. 오실로스코프가 계속 실행되며 트리거되지 않습니다. " +
            "Reference 선택 항목이 Right 로 변경됩니다.")]
        public TimebaseMode? Mode
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
        private TimebaseMode? _mode = null;

        [DisplayName(nameof(Position) + " [s] [Default = 0]"), DefaultValue(0.0), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("트리거 이벤트와 화면의 디스플레이 기준점 사이의 시간 간격을 설정합니다. " +
            "디스플레이 기준점은 왼쪽, 오른쪽 또는 가운데이며 Reference 로 설정합니다. 최대 위치 값은 time/division 설정에 따라 달라집니다.")]
        public double? Position
        {
            get => _position;
            set
            {
                if (_position != value)
                {
                    _position = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _position = null;

        [DisplayName(nameof(Range) + " [s] [Default = 1m]"), DefaultValue(1.0E-3), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("메인 윈도우의 전체 수평 시간(초)을 설정합니다. Range 는 현재 time/division 의 10배입니다.")]
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

        public enum TimebaseReference
        {
            Left,
            Center,
            Right,
        }

        [DefaultValue(TimebaseReference.Center), DisplayName(nameof(Reference) + " [Default = " + nameof(TimebaseReference.Center) + "]"),
            Description("디스플레이 기준점을 설정합니다.\r\n" +
            "• Left — 화면 왼쪽으로부터 한 눈금." +
            "• Center - 화면 중심.\r\n" +
            "• Right - 화면 오른쪽으로부터 한 눈금.\r\n" +
            "Reference는 트리거 포인트의 기준이 되는 화면상의 포인트입니다.")]
        public TimebaseReference? Reference
        {
            get => _reference;
            set
            {
                if (_reference != value)
                {
                    _reference = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private TimebaseReference? _reference = null;

        [DisplayName(nameof(Scale) + " [s] [Default = 100μ]"), DefaultValue(1.0E-4), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description("메인 윈도우의 수평 스케일 또는 눈금당 단위를 설정합니다.")]
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

        [DefaultValue(false), DisplayName(nameof(Vernier) + " [Default = False]"),
            Description("타임베이스 컨트롤의 버니어(미세 수평 조정) 설정을 켜거나(True) 끕니다(False).")]
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

        public event PropertyChangedEventHandler PropertyChanged;

        public override string ToString()
        {
            return "";
        }

        // This method is called by the Set accessor of each property.  
        // The CallerMemberName attribute that is applied to the optional propertyName  
        //   parameter causes the property name of the caller to be substituted as an argument.
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }

        public object Clone()
        {
            var obj = new OscopeTimebaseSettings();
            obj.Mode = Mode;
            obj.Position = Position;
            obj.Range = Range;
            obj.Reference = Reference;
            obj.Scale = Scale;
            obj.Vernier = Vernier;
            return obj;
        }
    }
}
