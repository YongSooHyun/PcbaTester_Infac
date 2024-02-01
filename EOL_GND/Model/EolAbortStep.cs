using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    public class EolAbortStep : EolStep
    {
        public enum AbortMode
        {
            /// <summary>
            /// 앞서 실행된 스텝들 중 하나 이상이 FAIL인 경우 중지.
            /// </summary>
            AbortOnStepFailure,

            /// <summary>
            /// 바로 앞 스텝의 결과가 FAIL인 경우 중지.
            /// </summary>
            AbortOnPreviousStepFailure,

            /// <summary>
            /// 지정한 스텝부터의 결과가 FAIL인 경우 중지.
            /// </summary>
            AbortOnSpecifiedStepFailure,

            /// <summary>
            /// 항상 정지.
            /// </summary>
            AbortAlways,
        }

        [Category(MethodCategory), DefaultValue(AbortMode.AbortOnStepFailure),
            Description("Abort 조건을 설정합니다.\r\n" +
            " • " + nameof(AbortMode.AbortOnStepFailure) + ": 처음 스텝부터의 실행결과들 중 하나 이상이 FAIL인 경우\r\n" +
            " • " + nameof(AbortMode.AbortOnPreviousStepFailure) + ": 바로 앞의 실행결과가 FAIL인 경우\r\n" +
            " • " + nameof(AbortMode.AbortOnSpecifiedStepFailure) + ": 지정한 스텝부터의 실행결과들 중 하나 이상이 FAIL인 경우\r\n" +
            " • " + nameof(AbortMode.AbortAlways) + ": 항상")]
        public AbortMode AbortCondition
        {
            get => _abortCondition;
            set
            {
                if (_abortCondition != value)
                {
                    _abortCondition = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private AbortMode _abortCondition = AbortMode.AbortOnStepFailure;

        [Category(MethodCategory), Browsable(false),
            Description(nameof(AbortCondition) + "이 " + nameof(AbortMode.AbortOnSpecifiedStepFailure) + "일 때 " +
            "실행결과가 FAIL인지 체크하기 시작하는 스텝의 Id 입니다.")]
        public int AbortOnSpecifiedStepId
        {
            get => _abortOnSpecifiedStepId;
            set
            {
                if (_abortOnSpecifiedStepId != value)
                {
                    _abortOnSpecifiedStepId = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _abortOnSpecifiedStepId;

        public enum AbortAction
        {
            /// <summary>
            /// 바로 중지.
            /// </summary>
            Abort,

            /// <summary>
            /// 지정한 스텝들 실행 후 중지.
            /// </summary>
            RunAndAbort,

            /// <summary>
            /// 지정한 스텝 위치부터 실행.
            /// </summary>
            JumpTo,
        }

        [Category(MethodCategory), DefaultValue(AbortAction.Abort),
            Description("Abort 조건이 만족될 때의 액션을 설정합니다.\r\n" +
            " • " + nameof(AbortAction.Abort) + ": 테스트 중지\r\n" +
            " • " + nameof(AbortAction.RunAndAbort) + ": 지정한 스텝들을 실행한 후 테스트 중지\r\n" +
            " • " + nameof(AbortAction.JumpTo) + ": 지정한 스텝으로 실행 위치를 이동")]
        public AbortAction Action
        {
            get => _action;
            set
            {
                if (_action != value)
                {
                    _action = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private AbortAction _action = AbortAction.Abort;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(IntArrayConverter)),
            Description("실행할 스텝 유일 Id들을 지정합니다. 예: 1~3, 5, 7, 12~17")]
        public int[] StepsToRun
        {
            get => _stepsToRun;
            set
            {
                if (_stepsToRun != value)
                {
                    _stepsToRun = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _stepsToRun;

        [Category(MethodCategory), Browsable(false), 
            Description("Jump할 위치의 스텝 유일 Id를 지정합니다.")]
        public int JumpPosition
        {
            get => _jumpPosition;
            set
            {
                if (_jumpPosition != value)
                {
                    _jumpPosition = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _jumpPosition;

        [Category(MethodCategory), Browsable(false),
            Description("Jump할 최대 횟수를 지정합니다. " + nameof(AbortCondition) + "이 성립되어도 이 횟수를 초과하면 Jump하지 않습니다.")]
        public int JumpMaxCount
        {
            get => _jumpMaxCount;
            set
            {
                if (_jumpMaxCount != value)
                {
                    _jumpMaxCount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _jumpMaxCount;

        /// <summary>
        /// Jump한 횟수.
        /// </summary>
        [Browsable(false), XmlIgnore]
        internal int JumpedCount { get; set; } = 0;

        //
        // Summary 표시를 위한 변수들.
        //

        internal override string CategoryName => StepCategory.AbortOnFail.GetText();
        public override string TestModeDesc => AbortCondition.ToString();
        public override string ParameterDesc => null;
        public override string ExpectedValueDesc => null;
        public override string TolerancePlusDesc => null;
        public override string ToleranceMinusDesc => null;
        public override List<int> AllTestChannels => null;

        private EolAbortStep()
        {
            Name = StepCategory.AbortOnFail.GetText();
        }

        public EolAbortStep(string deviceName) : this()
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
            return Enum.GetValues(typeof(AbortMode));
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out AbortMode parsed))
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
            throw new NotSupportedException();
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), false);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), false);
            Utils.SetBrowsableAttribute(this, nameof(RetestCount), false);
            Utils.SetBrowsableAttribute(this, nameof(RetestDelay), false);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), false);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), false);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), false);

            Utils.SetBrowsableAttribute(this, nameof(AbortOnSpecifiedStepId), AbortCondition == AbortMode.AbortOnSpecifiedStepFailure);
            Utils.SetBrowsableAttribute(this, nameof(StepsToRun), Action == AbortAction.RunAndAbort);
            Utils.SetBrowsableAttribute(this, nameof(JumpPosition), Action == AbortAction.JumpTo);
            Utils.SetBrowsableAttribute(this, nameof(JumpMaxCount), Action == AbortAction.JumpTo);
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolAbortStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolAbortStep abortStep)
            {
                abortStep.AbortCondition = AbortCondition;
                abortStep.AbortOnSpecifiedStepId = AbortOnSpecifiedStepId;
                abortStep.Action = Action;
                if (StepsToRun != null)
                {
                    abortStep.StepsToRun = new int[StepsToRun.Length];
                    StepsToRun.CopyTo(abortStep.StepsToRun, 0);
                }
                abortStep.JumpPosition = JumpPosition;
                abortStep.JumpMaxCount = JumpMaxCount;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
