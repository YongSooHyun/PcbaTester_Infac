using E100RC_Production;
using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using Ivi.Visa;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class EolMightyZapStep : EolStep
    {
        public enum MightyZapMethod
        {
            Move,
            Wait,
        }

        [Category(MethodCategory),
            Description("테스트 방법을 설정합니다.")]
        public MightyZapMethod TestMethod
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
        private MightyZapMethod _testMethod = MightyZapMethod.Move;

        [Category(MethodCategory),
            Description("연결할 COM포트를 설정합니다.")]
        public ComPort PortName
        {
            get => _portName;
            set
            {
                if (_portName != value)
                {
                    _portName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ComPort _portName = ComPort.COM1;

        [Category(MethodCategory), DefaultValue(57600),
            Description("COM포트 속도를 지정합니다.")]
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
        private int _baudRate = 57600;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntArrayConverter)),
            Description("대상 서보 액츄에이터의 ID 리스트.")]
        public int[] ServoIDs
        {
            get => _servoIDs;
            set
            {
                if (_servoIDs != value)
                {
                    _servoIDs = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _servoIDs;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntArrayConverter)),
            Description("이동시킬 위치 리스트를 설정합니다. 서보 ID 리스트 수와 같아야 합니다. 0 ~ 4,095")]
        public int[] Positions
        {
            get => _positions;
            set
            {
                if (_positions != value)
                {
                    _positions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _positions;

        //
        // Summary 표시를 위한 변수들.
        //

        internal override string CategoryName => StepCategory.MightyZap.GetText();
        public override string TestModeDesc => TestMethod.ToString();
        public override string ParameterDesc => null;
        public override string ExpectedValueDesc => null;
        public override string TolerancePlusDesc => null;
        public override string ToleranceMinusDesc => null;
        public override List<int> AllTestChannels => null;

        private EolMightyZapStep()
        {
            Name = StepCategory.MightyZap.GetText();
        }

        public EolMightyZapStep(string deviceName) : this()
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
            throw new NotSupportedException();
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            throw new NotSupportedException();
        }

        protected override void RelayOn(object elozTestSet, DeviceSetting setting)
        {
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
                case MightyZapMethod.Move:
                    // 움직여야 서보 ID 리스트.
                    if (ServoIDs?.Length > 0)
                    {
                        var positions = new int[ServoIDs.Length];
                        if (Positions != null)
                        {
                            int copyLength = Math.Min(ServoIDs.Length, Positions.Length);
                            Array.Copy(Positions, positions, copyLength);
                        }

                        // 시리얼 포트 열기.
                        var mightyZap = new MightyZap();
                        try
                        {
                            mightyZap.Open(PortName.ToString(), BaudRate);
                            for (int i = 0; i < ServoIDs.Length; i++)
                            {
                                // 서보 이동.
                                mightyZap.Move((byte)ServoIDs[i], (short)positions[i]);
                            }
                        }
                        finally
                        {
                            mightyZap.Close();
                        }
                    }

                    if (result.ResultState == ResultState.NoState)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                    break;
                case MightyZapMethod.Wait:
                    // 기다려야 할 서보 리스트.
                    if (ServoIDs?.Length > 0)
                    {
                        // 시리얼 포트 열기.
                        var mightyZap = new MightyZap();
                        try
                        {
                            mightyZap.Open(PortName.ToString(), BaudRate);
                            for (int i = 0; i < ServoIDs.Length; i++)
                            {
                                // 서보 이동 상태 체크.
                                while (true)
                                {
                                    bool moving = mightyZap.GetMovingState((byte)ServoIDs[i]);
                                    if (moving)
                                    {
                                        MultimediaTimer.Delay(10).Wait(token);
                                    }
                                    else
                                    {
                                        break;
                                    }
                                }
                            }
                        }
                        finally
                        {
                            mightyZap.Close();
                        }
                    }

                    if (result.ResultState == ResultState.NoState)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                    break;
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            Utils.SetBrowsableAttribute(this, nameof(Positions), TestMethod == MightyZapMethod.Move);
        }

        protected override void UpdateToleranceAttributes()
        {
        }

        public override object Clone()
        {
            var newStep = new EolMightyZapStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolMightyZapStep mightyStep)
            {
                mightyStep.TestMethod = TestMethod;
                mightyStep.PortName = PortName;
                mightyStep.BaudRate = BaudRate;
                if (ServoIDs != null)
                {
                    mightyStep.ServoIDs = new int[ServoIDs.Length];
                    ServoIDs.CopyTo(mightyStep.ServoIDs, 0);
                }
                if (Positions != null)
                {
                    mightyStep.Positions = new int[Positions.Length];
                    Positions.CopyTo(mightyStep.Positions, 0);
                }
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
