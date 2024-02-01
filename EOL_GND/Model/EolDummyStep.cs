using EOL_GND.Common;
using EOL_GND.Device;
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
    /// <summary>
    /// 테스트 결과를 임의로 설정하여 디버깅에 사용할 수 있는 스텝.
    /// </summary>
    public class EolDummyStep : EolStep
    {
        [Category(MethodCategory), DefaultValue(ResultState.NoState), 
            Description("이 스텝의 테스트 결과를 설정합니다.")]
        public ResultState TestResultState
        {
            get => _testResultState;
            set
            {
                if (_testResultState != value)
                {
                    _testResultState = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ResultState _testResultState = ResultState.NoState;

        //
        // Summary 표시를 위한 변수들.
        //

        internal override string CategoryName => StepCategory.Dummy.GetText();
        public override string TestModeDesc => TestResultState.GetText();
        public override string ParameterDesc => null;
        public override string ExpectedValueDesc => null;
        public override string TolerancePlusDesc => null;
        public override string ToleranceMinusDesc => null;
        public override List<int> AllTestChannels => null;

        private EolDummyStep()
        {
            Name = StepCategory.Dummy.GetText();
        }

        public EolDummyStep(string deviceName) : this()
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
            return new TestResult(this)
            {
                ResultState = TestResultState,
                ResultInfo = null,
                ResultValue = null,
                ResultValueState = ResultValueState.Invalid,
                Unit = GetPhysicalUnit(),
            };
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
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), false);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolDummyStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolDummyStep dummyStep)
            {
                dummyStep.TestResultState = TestResultState;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
