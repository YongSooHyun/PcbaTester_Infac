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
using static EOL_GND.Model.EolDioStep;

namespace EOL_GND.Model
{
    /// <summary>
    /// GloquadTech EVSE Controller 스텝.
    /// </summary>
    public class EolGloquadEvseStep : EolStep
    {
        /// <summary>
        /// EVSE 테스트 방법 리스트.
        /// </summary>
        public enum GloquadEvseTestMode
        {
            ChargerStart,
            ChargerStop,
        }

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public GloquadEvseTestMode TestMethod
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
        private GloquadEvseTestMode _testMethod = GloquadEvseTestMode.ChargerStart;

        internal override string CategoryName => StepCategory.GloquadSECC.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc => null;

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels => null;

        private EolGloquadEvseStep()
        {
            Name = StepCategory.GloquadSECC.GetText();
        }

        public EolGloquadEvseStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return GloquadSeccDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetGloquadSeccSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            return new object[]
            {
                GloquadEvseTestMode.ChargerStart,
                GloquadEvseTestMode.ChargerStop,
            };
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out GloquadEvseTestMode parsed))
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

            var seccDevice = device as GloquadSeccDevice;
            if (seccDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case GloquadEvseTestMode.ChargerStart:
                    seccDevice.StartCharging(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case GloquadEvseTestMode.ChargerStop:
                    seccDevice.StopCharging();
                    result.ResultState = ResultState.Pass;
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
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolGloquadEvseStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolGloquadEvseStep evseStep)
            {
                evseStep.TestMethod = TestMethod;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
