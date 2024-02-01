using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using Peak.Can.Basic;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class EolLinStep : EolStep
    {
        [TypeConverter(typeof(DescEnumConverter))]
        public enum LinTestMode
        {
            Open,
            Close,
            Read,
        }

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public LinTestMode TestMethod
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
        private LinTestMode _testMethod = LinTestMode.Close;

        [Category(MethodCategory), Browsable(false), DefaultValue(19200), TypeConverter(typeof(LinBaudrateConverter)),
            Description("통신속도를 설정합니다.")]
        public int BaudRate
        {
            get => _baudRate;
            set
            {
                if (_baudRate != value)
                {
                    _baudRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _baudRate = 19200;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(ByteHexConverter)),
            Description("프레임 ID(0 ~ 3F). 16진수로 표시됩니다.")]
        public byte FrameID
        {
            get => _frameID;
            set
            {
                if (_frameID != value)
                {
                    _frameID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte _frameID;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(MaxReadTime) + " [ms]"),
            Description("지정한 Frame ID를 가진 LIN 메시지를 읽기 위한 최대 시간(밀리초). 이 시간동안 지정한 Frame ID를 가진 LIN 메시지를 읽지 못하면 스텝의 결과는 FAIL로 됩니다.")]
        public int MaxReadTime
        {
            get => _maxReadTime;
            set
            {
                if (_maxReadTime != value)
                {
                    _maxReadTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _maxReadTime = 200;


        internal override string CategoryName => StepCategory.LIN.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case LinTestMode.Open:
                        return $"BaudRate={(int)BaudRate}";
                    case LinTestMode.Read:
                        return $"{FrameID}";
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels => null;

        public override string MinValueText
        {
            get
            {
                var minValueText = base.MinValueText;
                if (minValueText == null)
                {
                    minValueText = GetReadInfo();
                }
                return minValueText;
            }
        }

        public override string MaxValueText
        {
            get
            {
                var maxValueText = base.MaxValueText;
                if (maxValueText == null)
                {
                    maxValueText = GetReadInfo();
                }
                return maxValueText;
            }
        }

        private string GetReadInfo()
        {
            string info = null;
            switch (TestMethod)
            {
                case LinTestMode.Read:
                    info = $"0x{FrameID:X}";
                    break;
            }

            return info;
        }

        public override string MeasuredValueDesc
        {
            get
            {
                var measuredValueText = base.MeasuredValueDesc;
                if (measuredValueText == null)
                {
                    measuredValueText = RunResult?.ResultData?.ToString();
                }
                return measuredValueText;
            }
        }

        private EolLinStep()
        {
            Name = StepCategory.LIN.GetText();
        }

        public EolLinStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return LinDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetLinSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.LIN, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.PeakLIN:
                        return new object[]
                        {
                            LinTestMode.Open,
                            LinTestMode.Close,
                            LinTestMode.Read,
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
                if (Enum.TryParse(stringValue, out LinTestMode parsed))
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
            // Do nothing.
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            nominalValue = null;
            upperLimit = null;
            lowerLimit = null;
        }

        private static string CreateMessageSummary(string deviceName, bool received, DateTime time, byte protectedId, byte length, IEnumerable<byte> data)
        {
            var direction = received ? "Rx" : "Tx";
            var timeText = time.ToString("HH:mm:ss.fff");
            var dataText = data == null ? "" : string.Join(" ", data.Take(length).Select(x => $"{x:X2}"));
            var id = protectedId & 0x3F;
            return $"{deviceName} {direction} [{timeText}] {id:X}({protectedId:X}) [{length}] {dataText}";
        }

        protected override TestResult RunTest(object device, CancellationToken token)
        {
            var result = new TestResult(this)
            {
                ResultState = ResultState.NoState,
                ResultInfo = "",
                ResultValue = null,
                ResultValueState = ResultValueState.Invalid,
                Unit = GetPhysicalUnit(),
            };

            var linDevice = device as LinDevice;
            if (linDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case LinTestMode.Open:
                    linDevice.Open(false, (ushort)BaudRate);
                    result.ResultState = ResultState.Pass;
                    break;
                case LinTestMode.Close:
                    linDevice.Close();
                    result.ResultState = ResultState.Pass;
                    break;
                case LinTestMode.Read:
                    // FrameID 필터 설정.
                    //linDevice.SetReceiveFilter(0xFFFF_FFFF_FFFF_FFFFUL/*1UL << FrameID*/);

                    // 최대 읽기 시간 MaxReadTime 동안 검사에 필요한 메시지가 발견될 때까지 읽기.
                    var stopwatch = Stopwatch.StartNew();
                    LinMessage message;
                    var bootTime = DateTime.Now.AddMilliseconds(-Environment.TickCount);
                    var linLogMessages = new StringBuilder();
                    while (stopwatch.ElapsedMilliseconds < MaxReadTime)
                    {
                        bool read = linDevice.Read(out message);
                        if (read)
                        {
                            // 받은 메시지 표시.
                            var messageTime = bootTime.AddMilliseconds(message.TimeStamp / 1000);
                            var messageSummary = CreateMessageSummary(DeviceName, true, messageTime, message.FrameId, message.Length, message.Data);
                            result.ResultLogBody = messageSummary;
                            result.ResultInfo = messageSummary;
                            linLogMessages.AppendLine(result.ResultInfo);

                            // 메시지를 읽었으면, Frame ID 비교.
                            if ((message.FrameId & 0x3F) == FrameID)
                            {
                                result.ResultState = ResultState.Pass;
                                //result.ResultValue = FrameID;
                                result.ResultData = $"0x{FrameID:X}";
                                break;
                            }
                        }
                        else
                        {
                            // 메시지가 없다면 10ms 딜레이.
                            Task.Delay(10).Wait(token);
                        }
                    }
                    stopwatch.Stop();

                    // 메시지 로그 출력.
                    if (linLogMessages.Length > 0)
                    {
                        var msg = linLogMessages.ToString();
                        Logger.LogInfo(msg);
                    }

                    if (result.ResultState != ResultState.Pass)
                    {
                        result.ResultState = ResultState.Fail;
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

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            Utils.SetBrowsableAttribute(this, nameof(BaudRate), TestMethod == LinTestMode.Open);
            Utils.SetBrowsableAttribute(this, nameof(FrameID), TestMethod == LinTestMode.Read);
            Utils.SetBrowsableAttribute(this, nameof(MaxReadTime), TestMethod == LinTestMode.Read);
        }

        protected override void UpdateToleranceAttributes()
        {
        }

        public override object Clone()
        {
            var newStep = new EolLinStep();
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolLinStep linStep)
            {
                linStep.TestMethod = TestMethod;
                linStep.BaudRate = BaudRate;
                linStep.FrameID = FrameID;
                linStep.MaxReadTime = MaxReadTime;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
