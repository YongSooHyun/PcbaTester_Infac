using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// DIO 명령들을 실행한다.
    /// </summary>
    public class EolDioStep : EolStep
    {
        /// <summary>
        /// DIO 명령 리스트를 편집하기 쉽게 하기 위한 Wrapper Class.
        /// </summary>
        public class DioCommand : INotifyPropertyChanged
        {
            /// <summary>
            /// 명령 정보.
            /// </summary>
            public string Command
            {
                get => _command;
                set
                {
                    if (_command != value)
                    {
                        _command = value;
                        NotifyPropertyChanged();
                    }
                }
            }
            private string _command = "";

            ///// <summary>
            ///// ComponentModel을 이용한 명령 편집에 이용.
            ///// </summary>
            //internal string DeviceName { get; set; }

            // INotifyPropertyChanged
            public event PropertyChangedEventHandler PropertyChanged;

            public override string ToString()
            {
                return Command;
            }

            // This method is called by the Set accessor of each property.  
            // The CallerMemberName attribute that is applied to the optional propertyName  
            //   parameter causes the property name of the caller to be substituted as an argument.
            protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
            {
                PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        /// <summary>
        /// DIO 테스트 방법 리스트.
        /// </summary>
        public enum DioTestMode
        {
            RunCommand,
            RunCommandLine,
        }

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public DioTestMode TestMethod
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
        private DioTestMode _testMethod = DioTestMode.RunCommand;

        [Category(MethodCategory), TypeConverter(typeof(DioCommandListConverter)), 
            Editor(typeof(DioCommandListEditor), typeof(UITypeEditor)), Browsable(true), 
            Description("실행할 명령들을 설정합니다. 명령은 리스트에 보여지는 순서대로 실행됩니다.")]
        public BindingList<DioCommand> Commands
        {
            get => _commands;
            set
            {
                if (_commands != value)
                {
                    if (_commands != null)
                    {
                        _commands.ListChanged -= _commands_ListChanged;
                    }
                    _commands = value;
                    if (_commands != null)
                    {
                        _commands.ListChanged += _commands_ListChanged;
                    }
                    NotifyPropertyChanged();
                }
            }
        }
        private BindingList<DioCommand> _commands = new BindingList<DioCommand>();

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

        [Category(MethodCategory), Browsable(false), DefaultValue(500),
            Description("명령 라인을 직접 입력해 실행하는 경우, 그 응답을 읽기 위한 Timeout을 ms단위로 지정합니다.")]
        public int ReadTimeout
        {
            get => _readTimeout;
            set
            {
                if (_readTimeout != value)
                {
                    _readTimeout = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _readTimeout = 500;

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

        internal override string CategoryName => StepCategory.DIO.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case DioTestMode.RunCommand:
                        return string.Join(", ", Commands.Select(command => command.Command));
                    case DioTestMode.RunCommandLine:
                        return CommandLine;
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc => null;

        public override string TolerancePlusDesc => null;

        public override string ToleranceMinusDesc => null;

        public override List<int> AllTestChannels => null;

        private EolDioStep()
        {
            Name = StepCategory.DIO.GetText();
        }

        public EolDioStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return DioDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetDioSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            return new object[]
            {
                DioTestMode.RunCommand,
                DioTestMode.RunCommandLine,
            };
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            if (value is string stringValue)
            {
                if (Enum.TryParse(stringValue, out DioTestMode parsed))
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

            var dioDevice = device as DioDevice;
            if (dioDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case DioTestMode.RunCommand:
                    var responses = new StringBuilder();
                    foreach (var dioCmd in Commands)
                    {
                        if (string.IsNullOrEmpty(dioCmd.Command))
                        {
                            continue;
                        }

                        // 해당 명령의 Timeout 설정 찾기.
                        var settingsManager = DeviceSettingsManager.SharedInstance;
                        var dioSetting = settingsManager.FindSetting(DeviceCategory.DIO, DeviceName) as DioDeviceSetting;
                        DioDevice.CommandInfo cmdInfo = null;
                        foreach (var info in dioSetting.Commands)
                        {
                            if (dioCmd.Command.Equals(info.Command, StringComparison.OrdinalIgnoreCase))
                            {
                                cmdInfo = info;
                                break;
                            }
                        }

                        if (cmdInfo == null)
                        {
                            continue;
                        }

                        if (responses.Length > 0)
                        {
                            responses.Append("; ");
                        }
                        responses.Append(dioDevice.RunCommand(cmdInfo, token));
                    }
                    result.ResultInfo = responses.ToString();
                    result.ResultState = ResultState.Pass;
                    break;
                case DioTestMode.RunCommandLine:
                    var response = dioDevice.RunCommand(CommandLine, ResponseCheckMethod != CmdResponseCheckMode.None, ReadTimeout, token);
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

            switch (TestMethod)
            {
                case DioTestMode.RunCommand:
                    Utils.SetBrowsableAttribute(this, nameof(Commands), true);
                    Utils.SetBrowsableAttribute(this, nameof(CommandLine), false);
                    Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), false);
                    Utils.SetBrowsableAttribute(this, nameof(ReadTimeout), false);
                    Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), false);
                    Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), false);
                    break;
                case DioTestMode.RunCommandLine:
                    Utils.SetBrowsableAttribute(this, nameof(Commands), false);
                    Utils.SetBrowsableAttribute(this, nameof(CommandLine), true);
                    Utils.SetBrowsableAttribute(this, nameof(ResponseCheckMethod), true);
                    Utils.SetBrowsableAttribute(this, nameof(ReadTimeout), ResponseCheckMethod != CmdResponseCheckMode.None);
                    bool compareResponse = ResponseCheckMethod != CmdResponseCheckMode.None && ResponseCheckMethod != CmdResponseCheckMode.ReadLine;
                    Utils.SetBrowsableAttribute(this, nameof(ResponsePattern), compareResponse);
                    Utils.SetBrowsableAttribute(this, nameof(ResponsePatternCaseSensitive), compareResponse);
                    break;
            }
        }

        protected override void UpdateToleranceAttributes()
        {
            // Do nothing.
        }

        public override object Clone()
        {
            var newStep = new EolDioStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolDioStep dioStep)
            {
                dioStep.TestMethod = TestMethod;
                if (Commands != null)
                {
                    dioStep.Commands = new BindingList<DioCommand>();
                    foreach (var command in Commands)
                    {
                        var dioCommand = new DioCommand
                        {
                            Command = command.Command,
                        };
                        dioStep.Commands.Add(dioCommand);
                    }
                }

                dioStep.CommandLine = CommandLine;
                dioStep.ResponseCheckMethod = ResponseCheckMethod;
                dioStep.ReadTimeout = ReadTimeout;
                dioStep.ResponsePattern = ResponsePattern;
                dioStep.ResponsePatternCaseSensitive = ResponsePatternCaseSensitive;
            }

            dest.UpdateBrowsableAttributes();
        }

        private void _commands_ListChanged(object sender, ListChangedEventArgs e)
        {
            NotifyPropertyChanged(nameof(Commands));
        }
    }
}
