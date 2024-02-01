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
using static EOL_GND.Model.EolPowerStep;

namespace EOL_GND.Model
{
    public class EolAmplifierStep : EolStep
    {
        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public AmplifierTestMode TestMethod
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
        private AmplifierTestMode _testMethod = AmplifierTestMode.ReadIDN;

        [Category(MethodCategory), Browsable(false), DefaultValue(AmplifierDevice.AmplifierChannels.CH1),
            Description("설정하려는 채널을 나타냅니다.")]
        public AmplifierDevice.AmplifierChannels Channel
        {
            get => _channel;
            set
            {
                if (_channel != value)
                {
                    _channel = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private AmplifierDevice.AmplifierChannels _channel = AmplifierDevice.AmplifierChannels.CH1;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(InputCoupling) + " [Default = " + nameof(AmplifierDevice.AmplifierCoupling.DC) + "]"),
            Description("채널의 입력 커플링을 설정합니다.")]
        public AmplifierDevice.AmplifierCoupling? InputCoupling
        {
            get => _inputCoupling;
            set
            {
                if (_inputCoupling != value)
                {
                    _inputCoupling = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private AmplifierDevice.AmplifierCoupling? _inputCoupling;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(InputImpedance) + " [Default = 1 MΩ]"),
            Description("채널의 입력 임피던스를 설정합니다.")]
        public AmplifierDevice.AmplifierImpedance? InputImpedance
        {
            get => _inputImpedance;
            set
            {
                if (_inputImpedance != value)
                {
                    _inputImpedance = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private AmplifierDevice.AmplifierImpedance? _inputImpedance;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(Route) + " [Default = " + nameof(AmplifierDevice.AmplifierGain.Direct) + "]"),
            Description("채널의 증폭 여부를 설정합니다.\r\n" +
            " • " + nameof(AmplifierDevice.AmplifierGain.Direct) + ": 증폭하지 않는 직접경로로 설정.\r\n" +
            " • " + nameof(AmplifierDevice.AmplifierGain.Amplifier) + ": 5배 증폭하는 경로로 설정.\r\n")]
        public AmplifierDevice.AmplifierGain? Route
        {
            get => _route;
            set
            {
                if (_route != value)
                {
                    _route = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private AmplifierDevice.AmplifierGain? _route;

        public enum OutputState
        {
            ON,
            OFF,
        }

        [Category(MethodCategory), Browsable(false),
            Description("채널의 출력을 끄거나 켭니다.")]
        public OutputState Output
        {
            get => _output;
            set
            {
                if (_output != value)
                {
                    _output = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private OutputState _output = OutputState.OFF;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행합니다.")]
        public string CommandLine
        {
            get => _commandLine;
            set
            {
                if (_commandLine != value)
                {
                    _commandLine = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _commandLine;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 분석해 Pass/Fail로 판단하는 방법을 설정합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.None) + ": 응답을 읽지 않습니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLine) + ": 응답을 읽지만, 그 Pass/Fail 여부는 판단하지 않습니다. 응답이 없으면 Fail로 간주합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineStartsWith) + ": 응답을 읽어 그 첫 시작이 ResponsePattern으로 시작하는지 검사합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineContains) + ": 응답을 읽어 ResponsePattern을 포함하는지 검사합니다.\r\n" +
            " • " + nameof(CmdResponseCheckMode.ReadLineEndsWith) + ": 응답을 읽어 그 마지막이 ResponsePattern으로 끝나는지 검사합니다.")]
        public CmdResponseCheckMode ResponseCheckMethod
        {
            get => _responseCheckMethod;
            set
            {
                if (_responseCheckMethod != value)
                {
                    _responseCheckMethod = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private CmdResponseCheckMode _responseCheckMethod = CmdResponseCheckMode.None;

        [Category(MethodCategory), Browsable(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 체크하기 위한 Pattern문자열을 지정합니다.")]
        public string ResponsePattern
        {
            get => _responsePattern;
            set
            {
                if (_responsePattern != value)
                {
                    _responsePattern = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _responsePattern;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 대소문자를 구분하여 체크할 것인지 설정합니다.")]
        public bool ResponsePatternCaseSensitive
        {
            get => _responsePatternCaseSensitive;
            set
            {
                if (_responsePatternCaseSensitive != value)
                {
                    _responsePatternCaseSensitive = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _responsePatternCaseSensitive = false;


        internal override string CategoryName => StepCategory.Amplifier.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case AmplifierTestMode.SetOutput:
                        return Output.ToString();
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc => null;
        public override string TolerancePlusDesc => null;
        public override string ToleranceMinusDesc => null;
        public override List<int> AllTestChannels => null;

        /// <summary>
        /// XML Serialize를 위해서는 파라미터 없는 생성자 필요.
        /// </summary>
        private EolAmplifierStep()
        {
            Name = StepCategory.Amplifier.GetText();
        }

        public EolAmplifierStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return AmplifierDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetAmplifierSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.Amplifier, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.Keysight_33502A:
                        return new object[]
                        {
                            AmplifierTestMode.ReadIDN,
                            AmplifierTestMode.Reset,
                            AmplifierTestMode.Configure,
                            AmplifierTestMode.SetOutput,
                            AmplifierTestMode.RunCommandLine,
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
                if (Enum.TryParse(stringValue, out AmplifierTestMode parsed))
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

            var ampDevice = device as AmplifierDevice;
            if (ampDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case AmplifierTestMode.ReadIDN:
                    result.ResultInfo = ampDevice.ReadIDN(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case AmplifierTestMode.Reset:
                    ampDevice.Reset(token);
                    result.ResultState = ResultState.Pass;
                    break;
                case AmplifierTestMode.Configure:
                    ampDevice.Configure(Channel, InputCoupling, InputImpedance, Route, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case AmplifierTestMode.SetOutput:
                    ampDevice.SetOutput(Channel, Output == OutputState.ON, token);
                    result.ResultState = ResultState.Pass;
                    break;
                case AmplifierTestMode.RunCommandLine:
                    var response = ampDevice.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, 2000, token);
                    result.ResultInfo = response;
                    var textComparison = ResponsePatternCaseSensitive ? StringComparison.Ordinal : StringComparison.OrdinalIgnoreCase;
                    switch (ResponseCheckMethod)
                    {
                        case CmdResponseCheckMode.ReadLineStartsWith:
                            bool passed;
                            if (string.IsNullOrEmpty(ResponsePattern))
                            {
                                passed = true;
                            }
                            else if (string.IsNullOrEmpty(response))
                            {
                                passed = false;
                            }
                            else
                            {
                                passed = response.StartsWith(ResponsePattern, textComparison);
                            }
                            result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                            break;
                        case CmdResponseCheckMode.ReadLineContains:
                            if (string.IsNullOrEmpty(ResponsePattern))
                            {
                                passed = true;
                            }
                            else if (string.IsNullOrEmpty(response))
                            {
                                passed = false;
                            }
                            else
                            {
                                passed = response.IndexOf(ResponsePattern, textComparison) >= 0;
                            }
                            result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                            break;
                        case CmdResponseCheckMode.ReadLineEndsWith:
                            if (string.IsNullOrEmpty(ResponsePattern))
                            {
                                passed = true;
                            }
                            else if (string.IsNullOrEmpty(response))
                            {
                                passed = false;
                            }
                            else
                            {
                                passed = response.EndsWith(ResponsePattern, textComparison);
                            }
                            result.ResultState = passed ? ResultState.Pass : ResultState.Fail;
                            break;
                        default:
                            result.ResultState = ResultState.Pass;
                            break;
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

            bool showChannel = TestMethod == AmplifierTestMode.Configure || TestMethod == AmplifierTestMode.SetOutput;
            Utils.SetBrowsableAttribute(this, nameof(Channel), showChannel);
            Utils.SetBrowsableAttribute(this, nameof(InputCoupling), TestMethod == AmplifierTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(InputImpedance), TestMethod == AmplifierTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(Route), TestMethod == AmplifierTestMode.Configure);
            Utils.SetBrowsableAttribute(this, nameof(Output), TestMethod == AmplifierTestMode.SetOutput);

            bool runCmdLine = TestMethod == AmplifierTestMode.RunCommandLine;
            Utils.SetBrowsableAttribute(this, nameof(CommandLine), runCmdLine);
            Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), runCmdLine);
            bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
            Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), runCmdLine && compareResponse);
            Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), runCmdLine && compareResponse);
        }

        protected override void UpdateToleranceAttributes()
        {
        }

        public override object Clone()
        {
            var newStep = new EolAmplifierStep(DeviceName);

            // Amplifier 고유 프로퍼티.
            CopyTo(newStep);

            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolAmplifierStep ampStep)
            {
                ampStep.TestMethod = TestMethod;
                ampStep.Channel = Channel;
                ampStep.InputCoupling = InputCoupling;
                ampStep.InputImpedance = InputImpedance;
                ampStep.Route = Route;
                ampStep.Output = Output;

                ampStep.CommandLine = CommandLine;
                ampStep.ResponseCheckMethod = ResponseCheckMethod;
                ampStep.ResponsePattern = ResponsePattern;
                ampStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;
            }

            dest.UpdateBrowsableAttributes();
        }

        public enum AmplifierTestMode
        {
            ReadIDN,
            Reset,
            Configure,
            SetOutput,
            RunCommandLine,
        }
    }
}
