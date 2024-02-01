using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    [TypeConverter(typeof(ExpandableObjectConverter))]
    public class OscopeDvmSettings : INotifyPropertyChanged, ICloneable
    {
        [DefaultValue(false), DisplayName(nameof(Enable) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "디지털 전압계(DVM) 분석 기능을 켜거나 끕니다.")]
        public bool? Enable
        {
            get => _enable;
            set
            {
                if (_enable != value)
                {
                    _enable = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _enable = null;

        [DefaultValue(OscopeDevice.OscopeChannel.Channel1),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "디지털 전압계(DVM) 측정이 수행되는 아날로그 채널을 설정합니다. DVM 측정을 수행하기 위해 선택한 채널이 켜져 있을(파형 표시) 필요는 없습니다.")]
        public OscopeDevice.OscopeChannel Source
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
        private OscopeDevice.OscopeChannel _source = OscopeDevice.OscopeChannel.Channel1;

        public enum DvmMode
        {
            DC,
            DCRMS,
            ACRMS,
        }

        [DefaultValue(DvmMode.DC), DisplayName(nameof(Mode) + " [Default = " + nameof(DvmMode.DC) + "]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            " • DC (default) — 수집된 데이터의 DC 값을 표시합니다.\r\n" +
            " • DCRMS — 수집된 데이터의 제곱근 값을 표시합니다.\r\n" +
            " • ACRMS — DC 성분이 제거된 수집된 데이터의 평균 제곱근 값을 표시합니다.")]
        public DvmMode? Mode
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
        private DvmMode? _mode = null;

        [DefaultValue(false), DisplayName(nameof(AutoRange) + " [Default = False]"),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "선택한 디지털 전압계(DVM) 소스 채널이 오실로스코프 트리거링에 사용되지 않는 경우 디지털 전압계의 자동 범위 기능을 켜거나 끕니다.\r\n" +
            " • 켜면 DVM 채널의 수직 스케일, 수직(접지 레벨) 위치 및 트리거(임계 전압) 레벨(카운터 주파수 측정에 사용)이 자동으로 조정됩니다. " +
            "자동 범위 기능은 채널의 수직 스케일 및 위치를 조정하려고 시도한 것보다 우선합니다.\r\n" +
            " • 꺼진 경우 채널의 수직 스케일 및 위치를 정상적으로 조정할 수 있습니다.")]
        public bool? AutoRange
        {
            get => _autoRange;
            set
            {
                if (_autoRange != value)
                {
                    _autoRange = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _autoRange = null;

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
            var obj = new OscopeDvmSettings();
            obj.AutoRange = AutoRange;
            obj.Enable = Enable;
            obj.Mode = Mode;
            obj.Source = Source;
            return obj;
        }
    }
}
