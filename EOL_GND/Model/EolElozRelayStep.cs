using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static EOL_GND.Model.EolDioStep;

namespace EOL_GND.Model
{
    /// <summary>
    /// eloZ1 Relay 기능을 제공한다.
    /// </summary>
    public class EolElozRelayStep : EolStep
    {
        public enum RelayTestMode
        {
            ConnectChannels,
            DisconnectChannels,
            ClearConnections,
            GetConnectionInfo,
            GetChannelConnectionInfo,
        }

        [Category(MethodCategory),
            Description("테스트 방법을 설정합니다.")]
        public RelayTestMode TestMethod
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
        private RelayTestMode _testMethod = RelayTestMode.GetConnectionInfo;

        [Category(MethodCategory), Browsable(false), 
            Description("연결 정보를 얻을 테스트 채널을 지정합니다.")]
        public int TestChannel
        {
            get => _testChannel;
            set
            {
                if (_testChannel != value)
                {
                    _testChannel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _testChannel;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntListConverter)),
            Editor(typeof(TestChannelEditor), typeof(UITypeEditor)),
            Description("연결하거나 연결을 해제할 테스트 채널들을 설정합니다.")]
        public BindingList<int> TestChannels
        {
            get => _testChannels;
            set
            {
                if (_testChannels != value)
                {
                    if (_testChannels != null)
                    {
                        _testChannels.ListChanged -= _testChannels_ListChanged;
                    }
                    _testChannels = value;
                    if (_testChannels != null)
                    {
                        _testChannels = new BindingList<int>(_testChannels.Distinct().ToList());
                        _testChannels.ListChanged += _testChannels_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<int> _testChannels;

        private void _testChannels_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(TestChannels));
        }

        internal override string CategoryName => StepCategory.ElozRelay.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case RelayTestMode.GetChannelConnectionInfo:
                        return TestChannel.ToString();
                    case RelayTestMode.ConnectChannels:
                    case RelayTestMode.DisconnectChannels:
                        return string.Join(",", TestChannels);
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels
        {
            get
            {
                switch (TestMethod)
                {
                    case RelayTestMode.ConnectChannels:
                    case RelayTestMode.DisconnectChannels:
                        return TestChannels?.ToList();
                    default:
                        return null;
                }
            }
        }

        private EolElozRelayStep()
        {
            Name = StepCategory.ElozRelay.GetText();
        }

        public EolElozRelayStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            throw new NotSupportedException();
        }

        public override ICollection GetTestModes()
        {
            return Enum.GetValues(typeof(RelayTestMode));
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out RelayTestMode parsed))
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
            throw new NotSupportedException();
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

            switch (TestMethod)
            {
                case RelayTestMode.GetConnectionInfo:
                    var infoText = ElozDevice.SharedInstance.GetConnectionInfo(device);
                    Logger.LogInfo("eloZ1 connection info:" + Environment.NewLine + infoText);
                    result.ResultInfo = infoText;
                    result.ResultState = ResultState.Pass;
                    break;
                case RelayTestMode.GetChannelConnectionInfo:
                    infoText = ElozDevice.SharedInstance.GetChannelConnectionInfo(device, TestChannel);
                    Logger.LogInfo($"eloZ1 channel({TestChannel}) connection info:" + Environment.NewLine + result.ResultInfo);
                    result.ResultInfo = infoText;
                    result.ResultState = ResultState.Pass;
                    break;
                case RelayTestMode.ConnectChannels:
                    ElozDevice.SharedInstance.ConnectChannels(device, TestChannels);
                    result.ResultState = ResultState.Pass;
                    break;
                case RelayTestMode.DisconnectChannels:
                    ElozDevice.SharedInstance.DisconnectChannels(device, TestChannels);
                    result.ResultState = ResultState.Pass;
                    break;
                case RelayTestMode.ClearConnections:
                    ElozDevice.SharedInstance.RelayOff(device);
                    result.ResultState = ResultState.Pass;
                    break;
                default:
                    throw new NotSupportedException($"eloZ1 디바이스에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), false);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            Utils.SetBrowsableAttribute(this, nameof(TestChannel), TestMethod == RelayTestMode.GetChannelConnectionInfo);
            Utils.SetBrowsableAttribute(this, nameof(TestChannels), TestMethod == RelayTestMode.ConnectChannels || TestMethod == RelayTestMode.DisconnectChannels);
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolElozRelayStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolElozRelayStep relayStep)
            {
                relayStep.TestMethod = TestMethod;
                relayStep.TestChannel = TestChannel;
                if (TestChannels != null)
                {
                    relayStep.TestChannels = new BindingList<int>();
                    foreach (var channel in TestChannels)
                    {
                        relayStep.TestChannels.Add(channel);
                    }
                }
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
