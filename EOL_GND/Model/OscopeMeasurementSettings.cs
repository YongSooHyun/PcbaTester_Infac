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
    public class OscopeMeasurementSettings : INotifyPropertyChanged, ICloneable
    {
        public enum MeasureMethod
        {
            BitRate,
            BurstWidth,
            Clear,
            Counter,
            Delay,
            FallingEdgeCount,
            FallingPulseCount,
            FallTime,
            Frequency,
            NegativeDutyCycle,
            NegativePulseWidth,
            Overshoot,
            Period,
            Phase,
            PositiveDutyCycle,
            PositivePulseWidth,
            Preshoot,
            RiseTime,
            RisingEdgeCount,
            RisingPulseCount,
            StanardDeviation,
            VAmplitude,
            VAverage,
            VBase,
            VMax,
            VMin,
            VPP,
            VRatio,
            VRMS,
            VTop,
            XMax,
            XMin,
            YatX,
        }

        [Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            " • Clear: 선택한 모든 측정 및 마커를 화면에서 지웁니다.\r\n" +
            " • BitRate: 비트 속도 측정을 진행합니다.\r\n" +
            " • BurstWidth: burst 폭 측정을 진행합니다.\r\n" +
            " • Counter: 카운터 측정을 진행합니다. 카운터 측정은 특정 시간(게이트 시간) 내에 트리거 레벨 교차를 카운트하고 결과를 Hz 단위로 표시합니다. " +
            "게이트 시간은 오실로스코프의 수평 범위이지만 >= 0.1초 및 <= 10초로 제한됩니다. 다른 측정과 달리 줌 수평 타임베이스 창은 카운터 측정을 게이트하지 않습니다. " +
            "카운터 측정은 오실로스코프의 대역폭까지 주파수를 측정할 수 있습니다. 지원되는 최소 주파수는 2.0 / gateTime입니다. 한번에 하나의 카운터 측정만 표시할 수 있습니다.\r\n" +
            " • Delay: 지연 측정을 진행합니다.\r\n")]
        public MeasureMethod Method
        {
            get => _method;
            set
            {
                if (_method != value)
                {
                    _method = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private MeasureMethod _method = MeasureMethod.Clear;

        [DefaultValue(OscopeDevice.OscopeChannel.Channel1), Browsable(false), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "측정에 대한 첫번째 소스를 설정합니다.")]
        public OscopeDevice.OscopeChannel Source1
        {
            get => _source1;
            set
            {
                if (_source1 != value)
                {
                    _source1 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeDevice.OscopeChannel _source1 = OscopeDevice.OscopeChannel.Channel1;

        [DefaultValue(OscopeDevice.OscopeChannel.Channel2), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "측정에 대한 두번째 소스를 설정합니다.")]
        public OscopeDevice.OscopeChannel? Source2
        {
            get => _source2;
            set
            {
                if (_source2 != value)
                {
                    _source2 = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private OscopeDevice.OscopeChannel? _source2;

        public enum EdgeSelectMode
        {
            Manual,
            Auto,
        }

        [DefaultValue(EdgeSelectMode.Auto), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            " • Auto — 에지가 자동으로 선택됩니다. 타임베이스 기준점에 가장 가까운 소스1 에지가 사용되고 소스1 에지에 가장 가까운 소스2 에지가 사용됩니다.\r\n" +
            " • Manual — 에지 번호는 설정(Slope, Number, Threshold)에 따라 결정됩니다. 두 소스 모두에 대해 0보다 큰 에지 번호들이 디스플레이 왼쪽에서부터 카운트 됩니다.")]
        public EdgeSelectMode DelayEdgeSelectMode
        {
            get => _delayEdgeSelectMode;
            set
            {
                if (_delayEdgeSelectMode != value)
                {
                    _delayEdgeSelectMode = value;
                    UpdateEdgeSelectionBrowsables();
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeSelectMode _delayEdgeSelectMode = EdgeSelectMode.Auto;

        public enum EdgeSlope
        {
            Rising,
            Falling,
        }

        public enum EdgeThreshold
        {
            Lower,
            Middle,
            Upper,
        }

        [DefaultValue(EdgeSlope.Rising), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지연 측정에 대한 소스1 기울기 방향을 정의합니다.")]
        public EdgeSlope Source1EdgeSlope
        {
            get => _source1EdgeSlope;
            set
            {
                if (_source1EdgeSlope != value)
                {
                    _source1EdgeSlope = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeSlope _source1EdgeSlope = EdgeSlope.Rising;

        [DefaultValue(0), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "0 ~ 1000. 지연 측정에 대해 소스1 에지 번호를 정의합니다. 0으로 설정하면 타임 베이스 기준점에 가장 가까운 에지가 자동으로 선택됩니다. " +
            "이 경우 Source2EdgeNumber 설정도 0이어야 하며 선택된 소스1 에지에 가장 가까운 소스2 에지가 사용됩니다. " +
            "두 소스 모두에 대해 에지 번호들이 0보다 크면 디스플레이의 왼쪽에서부터 계수됩니다.")]
        public int Source1EdgeNumber
        {
            get => _source1EdgeNumber;
            set
            {
                if (_source1EdgeNumber != value)
                {
                    _source1EdgeNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _source1EdgeNumber = 0;

        [DefaultValue(EdgeThreshold.Middle), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지연 측정에 대한 소스1 에지 임계값을 정의합니다.")]
        public EdgeThreshold Source1EdgeThreshold
        {
            get => _source1EdgeThreshold;
            set
            {
                if (_source1EdgeThreshold != value)
                {
                    _source1EdgeThreshold = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeThreshold _source1EdgeThreshold = EdgeThreshold.Middle;

        [DefaultValue(EdgeSlope.Rising), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지연 측정에 대한 소스2 기울기 방향을 정의합니다.")]
        public EdgeSlope Source2EdgeSlope
        {
            get => _source2EdgeSlope;
            set
            {
                if (_source2EdgeSlope != value)
                {
                    _source2EdgeSlope = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeSlope _source2EdgeSlope = EdgeSlope.Rising;

        [DefaultValue(0), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "0 ~ 1000. 지연 측정에 대한 소스2 에지 번호를 정의합니다.")]
        public int Source2EdgeNumber
        {
            get => _source2EdgeNumber;
            set
            {
                if (_source2EdgeNumber != value)
                {
                    _source2EdgeNumber = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _source2EdgeNumber = 0;

        [DefaultValue(EdgeThreshold.Middle), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "지연 측정에 대한 소스2 에지 임계값을 정의합니다.")]
        public EdgeThreshold Source2EdgeThreshold
        {
            get => _source2EdgeThreshold;
            set
            {
                if (_source2EdgeThreshold != value)
                {
                    _source2EdgeThreshold = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private EdgeThreshold _source2EdgeThreshold = EdgeThreshold.Middle;

        public enum VrmsTypeMode
        {
            AC,
            DC
        }

        [DefaultValue(VrmsTypeMode.DC), Browsable(false),
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "이 옵션을 사용하면 DC RMS 측정과 AC RMS 측정 중 하나를 선택할 수 있습니다.")]
        public VrmsTypeMode VrmsType
        {
            get => _vrmsType;
            set
            {
                if (_vrmsType != value)
                {
                    _vrmsType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private VrmsTypeMode _vrmsType = VrmsTypeMode.DC;

        [DisplayName(nameof(HorizontalLocation) + " [s]"), DefaultValue(0.0), Browsable(false), TypeConverter(typeof(PhysicalValueStaticConverter)), 
            Description(OscopeDevice.KeysightInfiniiVision3000T_X_Series + "\r\n" +
            "트리거로부터의 시간으로, 지정된 값이 화면에 있어야 합니다.")]
        public double HorizontalLocation
        {
            get => _horizontalLocation;
            set
            {
                if (_horizontalLocation != value)
                {
                    _horizontalLocation = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _horizontalLocation = 0;

        [DefaultValue(false), Browsable(false), 
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

        public OscopeMeasurementSettings()
        {
            UpdateBrowsableAttributes();
        }

        public void UpdateBrowsableAttributes()
        {
            Utils.SetBrowsableAttribute(this, nameof(Source1), false);
            Utils.SetBrowsableAttribute(this, nameof(Source2), false);
            Utils.SetBrowsableAttribute(this, nameof(DelayEdgeSelectMode), false);
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeSlope), false);
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeNumber), false);
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeThreshold), false);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeSlope), false);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeNumber), false);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeThreshold), false);
            Utils.SetBrowsableAttribute(this, nameof(VrmsType), false);
            Utils.SetBrowsableAttribute(this, nameof(HorizontalLocation), false);
            //Utils.SetBrowsableAttribute(this, nameof(Temp), Method != MeasureMethod.Clear);

            switch (Method)
            {
                case MeasureMethod.Clear:
                    break;
                case MeasureMethod.BitRate:
                case MeasureMethod.BurstWidth:
                case MeasureMethod.Counter:
                case MeasureMethod.PositiveDutyCycle:
                case MeasureMethod.NegativeDutyCycle:
                case MeasureMethod.FallTime:
                case MeasureMethod.RiseTime:
                case MeasureMethod.FallingEdgeCount:
                case MeasureMethod.RisingEdgeCount:
                case MeasureMethod.FallingPulseCount:
                case MeasureMethod.RisingPulseCount:
                case MeasureMethod.PositivePulseWidth:
                case MeasureMethod.NegativePulseWidth:
                case MeasureMethod.Overshoot:
                case MeasureMethod.Preshoot:
                case MeasureMethod.Frequency:
                case MeasureMethod.Period:
                case MeasureMethod.StanardDeviation:
                case MeasureMethod.VAmplitude:
                case MeasureMethod.VAverage:
                case MeasureMethod.VBase:
                case MeasureMethod.VMax:
                case MeasureMethod.VMin:
                case MeasureMethod.VPP:
                case MeasureMethod.VTop:
                case MeasureMethod.XMax:
                case MeasureMethod.XMin:
                    Utils.SetBrowsableAttribute(this, nameof(Source1), true);
                    Source2 = null;
                    break;
                case MeasureMethod.Delay:
                case MeasureMethod.Phase:
                    Utils.SetBrowsableAttribute(this, nameof(Source1), true);
                    Utils.SetBrowsableAttribute(this, nameof(Source2), true);
                    Utils.SetBrowsableAttribute(this, nameof(DelayEdgeSelectMode), true);
                    // Update edge select mode
                    UpdateEdgeSelectionBrowsables();
                    break;
                case MeasureMethod.VRatio:
                    Utils.SetBrowsableAttribute(this, nameof(Source1), true);
                    Utils.SetBrowsableAttribute(this, nameof(Source2), true);
                    break;
                case MeasureMethod.VRMS:
                    Utils.SetBrowsableAttribute(this, nameof(Source1), true);
                    Source2 = null;
                    Utils.SetBrowsableAttribute(this, nameof(VrmsType), true);
                    break;
                case MeasureMethod.YatX:
                    Utils.SetBrowsableAttribute(this, nameof(Source1), true);
                    Source2 = null;
                    Utils.SetBrowsableAttribute(this, nameof(HorizontalLocation), true);
                    break;
            }
        }

        private void UpdateEdgeSelectionBrowsables()
        {
            bool browsable = DelayEdgeSelectMode == EdgeSelectMode.Manual;
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeSlope), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeNumber), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Source1EdgeThreshold), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeSlope), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeNumber), browsable);
            Utils.SetBrowsableAttribute(this, nameof(Source2EdgeThreshold), browsable);
        }

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
            var obj = new OscopeMeasurementSettings();
            obj.DelayEdgeSelectMode = DelayEdgeSelectMode;
            obj.HorizontalLocation = HorizontalLocation;
            obj.Method = Method;
            obj.Source1 = Source1;
            obj.Source1EdgeNumber = Source1EdgeNumber;
            obj.Source1EdgeSlope = Source1EdgeSlope;
            obj.Source1EdgeThreshold = Source1EdgeThreshold;
            obj.Source2 = Source2;
            obj.Source2EdgeNumber = Source2EdgeNumber;
            obj.Source2EdgeSlope = Source2EdgeSlope;
            obj.Source2EdgeThreshold = Source2EdgeThreshold;
            obj.VrmsType = VrmsType;
            obj.Temp = Temp;
            return obj;
        }
    }
}
