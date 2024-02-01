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
    public class OscopeAcquireSettings : INotifyPropertyChanged, ICloneable
    {
        [DefaultValue(false), DisplayName(nameof(DigitizerMode) + " [Default = False]"), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\nDigitizer 모드를 켜거나 끕니다.\r\n" +
            "보통 Digitizer 모드가 꺼지면(자동모드), 오실로스코프의 분할당 시간 설정은 오실로스코프가 실행 중인 동안" +
            "(연속적으로 수집하는 동안) 파형 디스플레이를 데이터로 채울 수 있도록 샘플링 속도와 메모리 깊이를 결정합니다.\r\n" +
            "Ditigizer 모드에서는 수집 샘플링 속도와 메모리 깊이를 직접 설정하며, 이러한 설정은 오실로스코프의 time/div " +
            "설정에 따라 캡처된 데이터가 파형 디스플레이의 가장자리를 훨씬 초과하거나 파형 디스플레이의 작은 부분을 차지하더라도 사용됩니다.\r\n" +
            "Digitizer 모드는 주로 여러 계측기의 데이터를 제어하고 결합하는 외부 소프트웨어를 지원합니다.")]
        public bool? DigitizerMode
        {
            get => _digitizerMode;
            set
            {
                if (_digitizerMode != value)
                {
                    _digitizerMode = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool? _digitizerMode = null;

        [Browsable(false), DefaultValue(10000), DisplayName(nameof(Points) + " [Default = 10000]"), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n원하는 수집 메모리 깊이(포인트 개수)를 설정합니다.")]
        public int? Points
        {
            get => _points;
            set
            {
                if (_points != value)
                {
                    _points = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int? _points = null;

        [DisplayName(nameof(SampleRate) + " [Sa/s] [Default = 5G]"), 
            Browsable(false), DefaultValue(5E9), TypeConverter(typeof(PhysicalValueStaticConverter)),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n원하는 수집 샘플링 속도를 설정합니다.")]
        public double? SampleRate
        {
            get => _sampleRate;
            set
            {
                if (_sampleRate != value)
                {
                    _sampleRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _sampleRate = null;

        public override string ToString()
        {
            return "";
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public void UpdateBrowsableAttributes()
        {
            Utils.SetBrowsableAttribute(this, nameof(Points), DigitizerMode == true);
            Utils.SetBrowsableAttribute(this, nameof(SampleRate), DigitizerMode == true);
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
            var obj = new OscopeAcquireSettings();
            obj.DigitizerMode = DigitizerMode;
            obj.Points = Points;
            obj.SampleRate = SampleRate;
            obj.UpdateBrowsableAttributes();
            return obj;
        }
    }
}
