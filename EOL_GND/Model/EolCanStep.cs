using DbcParserLib;
using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using EOL_GND.Model.DBC;
using EOL_GND.ViewModel;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing.Design;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// CAN 메시지를 전송할 때 포함될 시그널 정보.
    /// </summary>
    public class CanSignal : ICloneable
    {
        /// <summary>
        /// 시그널 이름. <see cref="DbcSignal.Name"/>
        /// </summary>
        public string SignalName { get; set; }

        /// <summary>
        /// <see cref="SignalName"/>으로 로딩한 시그널 정보. 저장되지 않고 내부적으로 이용.
        /// </summary>
        internal DbcSignal SignalInfo { get; set; }

        /// <summary>
        /// 신호값이 어떤 규칙에 따라 자동적으로 결정될 때 그 규칙의 형태.
        /// </summary>
        public enum AutoValueType
        {
            /// <summary>
            /// The data area for CRC calculation is based on the data length (DLC) of the message DB.<br/>
            /// The CRC shall be calculated over the entire data block (excluding the CRC bytes) including the user data, alive counter and Data ID.<br/>
            /// - CRC Polynomial (0x1021) , Initial Value : 0xFFFF, XOR value : 0x0000, Data ID : Message ID + 0xF800<br/>
            /// - The initial value is for CRC calculation and the message should be sent with the calculated result value.<br/>
            /// - The mathematical expression to 16 bit CRC polynomial is given here: G(x) = x16 + x12 + x5 + 1
            /// </summary>
            CRC,

            /// <summary>
            /// For the first transmission request for a data element the counter shall be initialized with 0 
            /// and shall be incremented by 1 for every subsequent send request. <br/>
            /// When the counter reaches the maximum value(0xFF), then it shall restart with 0 for the next send request.
            /// </summary>
            AliveCounter,

            /// <summary>
            /// 사용자가 설정한 값.
            /// </summary>
            Manual,
        }

        /// <summary>
        /// 값이 자동으로 결정되는 신호값(CRC 등) 형태.
        /// </summary>
        public AutoValueType ValueType { get; set; } = AutoValueType.Manual;

        /// <summary>
        /// <see cref="ValueType"/>이 <see cref="AutoValueType.Manual"/>일 때 사용자에 의한 설정 값.
        /// </summary>
        public double Value { get; set; }

        public object Clone()
        {
            var newObj = new CanSignal();
            newObj.SignalName = SignalName;
            newObj.ValueType = ValueType;
            newObj.Value = Value;
            return newObj;
        }
    }

    public class EolCanStep : EolStep
    {
        [DllImport("HKMC_AdvancedSeedKey_Win32_ICCU.dll", EntryPoint = "ASK_KeyGenerate")]
        private static extern int ASK_KeyGenerate_ICCU(byte[] seed, byte[] key);

        [DllImport("HKMC_AdvancedSeedKey_Win32_SX2.dll", EntryPoint = "ASK_KeyGenerate")]
        private static extern int ASK_KeyGenerate_SX2(byte[] seed, byte[] key);

        private static int ASK_KeyGenerate(ASKType askType, byte[] seed, byte[] key)
        {
            if (askType == ASKType.SX2_Inverter_Control)
            {
                return ASK_KeyGenerate_SX2(seed, key);
            }
            else
            {
                return ASK_KeyGenerate_ICCU(seed, key);
            }
        }

        private const int SEEDKEY_SUCCESS = 0;
        private const int SEEDKEY_FAIL = 1;

        [TypeConverter(typeof(DescEnumConverter))]
        public enum CanTestMode
        {
            /// <summary>
            /// CAN 로그 생성 시작/중지.
            /// </summary>
            SetLogMode,

            /// <summary>
            /// CAN 채널 열기.
            /// </summary>
            Open,

            /// <summary>
            /// CAN 채널 닫기.
            /// </summary>
            Close,

            /// <summary>
            /// CAN 채널 송/수신 큐 클리어.
            /// </summary>
            ResetQueues,

            /// <summary>
            /// 읽은 CAN 메시지 검사.
            /// </summary>
            Read,

            /// <summary>
            /// CAN 메시지 전송.
            /// </summary>
            Write,

            /// <summary>
            /// CAN 메시지 주기적 전송.
            /// </summary>
            WritePeriodic,

            /// <summary>
            /// CAN 메시지 주기적 전송 취소.
            /// </summary>
            StopPeriodic,

            UDS_Open,
            UDS_Close,

            [Description(nameof(UDS_ReadDataByIdentifier) + " (0x22)")]
            UDS_ReadDataByIdentifier,

            [Description(nameof(UDS_WriteDataByIdentifier) + " (0x2E)")]
            UDS_WriteDataByIdentifier,

            [Description(nameof(UDS_DiagnosticSessionControl) + " (0x10)")]
            UDS_DiagnosticSessionControl,

            [Description(nameof(UDS_SendKey) + " SecurityAccess(RequestSeed+SendKey) (0x27)")]
            UDS_SendKey,

            [Description(nameof(UDS_RoutineControl) + " (0x31)")]
            UDS_RoutineControl,

            [Description(nameof(UDS_InputOutputControlByIdentifier) + " (0x2F)")]
            UDS_InputOutputControlByIdentifier,

            [Description("Set UDS parameters")]
            UDS_SetParameters,

            [Description("Advanced SeedKey")]
            ASK_KeyGenerate,

            [Description("CRC-16-CCITT, Initial Value: 0xFFFF")]
            CRC_16_CCITT_0xFFFF_Calculate,
        }

        [Category(MethodCategory), TypeConverter(typeof(TestModeConverter)),
            Description("테스트 방법을 설정합니다.")]
        public CanTestMode TestMethod
        {
            get => _testMethod;
            set
            {
                if (_testMethod != value)
                {
                    _testMethod = value;
                    if (_testMethod == CanTestMode.Read)
                    {
                        CAN_FD = true;
                    }
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private CanTestMode _testMethod = CanTestMode.Close;

        [Category(MethodCategory), Browsable(false),
            Description("CAN 통신 로그를 생성할지 여부를 지정합니다.")]
        public bool LogEnabled
        {
            get => _logEnabled;
            set
            {
                if (_logEnabled != value)
                {
                    _logEnabled = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _logEnabled = false;

        [Category(MethodCategory), Browsable(false),
            Description("측정값의 단위를 설정합니다.")]
        public PhysicalUnit Unit
        {
            get => _unit;
            set
            {
                if (_unit != value)
                {
                    _unit = value;
                    UpdateDisplayNames();
                    NotifyPropertyChanged();
                }
            }
        }
        private PhysicalUnit _unit = PhysicalUnit.None;

        [Category(MethodCategory), DisplayName(nameof(ExpectedValue)), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("테스트 기대치를 설정합니다.")]
        public double ExpectedValue
        {
            get => _expectedValue;
            set
            {
                if (_expectedValue != value)
                {
                    _expectedValue = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _expectedValue;

        [Category(MethodCategory), Browsable(false),
            Description("허용오차 방식을 설정합니다.")]
        public ToleranceMode Tolerance
        {
            get => _toleranceMode;
            set
            {
                if (_toleranceMode != value)
                {
                    _toleranceMode = value;
                    UpdateToleranceAttributes();
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private ToleranceMode _toleranceMode = ToleranceMode.Relative;

        [Category(MethodCategory), DisplayName(DispNameTolPlus + " [%]"), Browsable(false),
            Description("플러스 허용오차.")]
        public double TolerancePlusPercent
        {
            get => _tolerancePlusPercent;
            set
            {
                if (_tolerancePlusPercent != value)
                {
                    _tolerancePlusPercent = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolMinus + " [%]"), Browsable(false),
            Description("마이너스 허용오차.")]
        public double ToleranceMinusPercent
        {
            get => _toleranceMinusPercent;
            set
            {
                if (_toleranceMinusPercent != value)
                {
                    _toleranceMinusPercent = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _toleranceMinusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolPlusMinus + " [%]"), Browsable(false),
            Description("플러스/마이너스 허용오차.")]
        public double TolerancePlusMinusPercent
        {
            get => _tolerancePlusMinusPercent;
            set
            {
                if (_tolerancePlusMinusPercent != value)
                {
                    _tolerancePlusMinusPercent = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusMinusPercent;

        [Category(MethodCategory), DisplayName(DispNameTolPlus + " []"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("플러스 허용오차.")]
        public double TolerancePlusAbsolute
        {
            get => _tolerancePlusAbsolute;
            set
            {
                if (_tolerancePlusAbsolute != value)
                {
                    _tolerancePlusAbsolute = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusAbsolute;

        [Category(MethodCategory), DisplayName(DispNameTolMinus + " []"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("마이너스 허용오차.")]
        public double ToleranceMinusAbsolute
        {
            get => _toleranceMinusAbsolute;
            set
            {
                if (_toleranceMinusAbsolute != value)
                {
                    _toleranceMinusAbsolute = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _toleranceMinusAbsolute;

        [Category(MethodCategory), DisplayName(DispNameTolPlusMinus + " []"), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            Description("플러스/마이너스 허용오차.")]
        public double TolerancePlusMinusAbsolute
        {
            get => _tolerancePlusMinusAbsolute;
            set
            {
                if (_tolerancePlusMinusAbsolute != value)
                {
                    _tolerancePlusMinusAbsolute = value;
                    UpdateSignalValuesToFinish();
                    NotifyPropertyChanged();
                }
            }
        }
        private double _tolerancePlusMinusAbsolute;

        [Category(MethodCategory), DisplayName(nameof(MeasureOffset)), Browsable(false), TypeConverter(typeof(PhysicalValueDynamicConverter)),
            DefaultValue(0.0), Description("측정값에서 뺄 옵셋을 설정합니다.")]
        public double MeasureOffset
        {
            get => _measureOffset;
            set
            {
                if (_measureOffset != value)
                {
                    _measureOffset = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double _measureOffset = 0;

        //
        // Open 에서 사용할 변수들.
        //

        [Category(MethodCategory), Browsable(true), DefaultValue(CanNominalBaudRate.BAUD_500K),
            Description("중재 통신속도 (BTR0BTR1 code).")]
        public CanNominalBaudRate NominalBaudRate
        {
            get => _nominalBaudRate;
            set
            {
                if (_nominalBaudRate != value)
                {
                    _nominalBaudRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CanNominalBaudRate _nominalBaudRate = CanNominalBaudRate.BAUD_500K;

        [Category(MethodCategory), Browsable(true), DefaultValue(CanDataBaudRate.BAUD_2M),
            Description("CAN FD 데이터 통신속도.")]
        public CanDataBaudRate DataBaudRate
        {
            get => _dataBaudRate;
            set
            {
                if (_dataBaudRate != value)
                {
                    _dataBaudRate = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CanDataBaudRate _dataBaudRate = CanDataBaudRate.BAUD_2M;

        [Category(MethodCategory), Browsable(false), DefaultValue(true),
            Description("CAN 메시지 이름을 이용해 데이터베이스에서 메시지 데이터를 로딩할 것인지 설정합니다.")]
        public bool UseMessageName
        {
            get => _useMessageName;
            set
            {
                if (_useMessageName != value)
                {
                    _useMessageName = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _useMessageName = true;

        [Category(MethodCategory), Browsable(false), Editor(typeof(CanMessageNameEditor), typeof(UITypeEditor)), TypeConverter(typeof(CanMessageNameConverter)),
            Description("CAN 메시지 이름. 비지 않았으면 데이터베이스에서 이 이름을 가진 메시지 데이터를 사용합니다.")]
        public string MessageName
        {
            get => _messageName;
            set
            {
                if (_messageName != value)
                {
                    _messageName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _messageName;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(CanIdConverter)),
            Description("11비트 또는 29비트 CAN ID를 지정합니다. 16진수로 표시됩니다.")]
        public uint MessageID
        {
            get => _messageID;
            set
            {
                if (_messageID != value)
                {
                    _messageID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _messageID;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("CAN 확장 프레임(29비트 ID) 여부(Hex).\r\nTrue : 확장 프레임(29비트 ID).\r\nFalse : 표준 프레임(11비트 ID).")]
        public bool ExtendedFrame
        {
            get => _extendedFrame;
            set
            {
                if (_extendedFrame != value)
                {
                    _extendedFrame = value;

                    if (_extendedFrame && MessageID > 0x1F_FF_FF_FF)
                    {
                        MessageID = 0x1F_FF_FF_FF;
                    }
                    else if (!_extendedFrame && MessageID > 0x7FF)
                    {
                        MessageID = 0x7FF;
                    }

                    NotifyPropertyChanged();
                }
            }
        }
        private bool _extendedFrame = false;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("CAN RTR(Remote-Transfer-Request) 프레임 여부.\r\nTrue : RTR 프레임.\r\nFalse : 데이터 프레임.")]
        public bool RemoteRequest
        {
            get => _remoteRequest;
            set
            {
                if (_remoteRequest != value)
                {
                    _remoteRequest = value;
                    if (_remoteRequest)
                    {
                        CAN_FD = false;
                    }
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _remoteRequest = false;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("CAN FD(Flexible Data) 프레임 여부.\r\nTrue : FD 프레임.\r\nFalse : Non-FD 프레임.")]
        public bool CAN_FD
        {
            get => _can_FD;
            set
            {
                if (_can_FD != value)
                {
                    _can_FD = value;
                    if (_can_FD)
                    {
                        RemoteRequest = false;
                    }
                    else
                    {
                        BitRateSwitch = false;

                        if ((int)DLC > 8)
                        {
                            DLC = (CanDLC)8;
                        }
                    }
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _can_FD = false;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("CAN FD BRS(Bit Rate Switch) 여부 (CAN 데이터 고속 전송 여부).\r\nTrue : BRS 사용.\r\nFalse : BRS 사용 안 함.")]
        public bool BitRateSwitch
        {
            get => _bitRateSwitch;
            set
            {
                if (_bitRateSwitch != value)
                {
                    _bitRateSwitch = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _bitRateSwitch = false;

        [Category(MethodCategory), Browsable(false),
            Description("CAN 메시지의 DLC(Data Length Code)를 지정합니다.")]
        public CanDLC DLC
        {
            get => _dlc;
            set
            {
                if (_dlc != value)
                {
                    _dlc = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private CanDLC _dlc = CanDLC.DLC_0;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(Cycle) + " [ms]"), DefaultValue(0u),
            Description("CAN 메시지 전송 주기(밀리초). CAN 메시지를 한번만 전송하려면 이 값을 0으로 설정하세요.")]
        public uint Cycle
        {
            get => _cycle;
            set
            {
                if (_cycle != value)
                {
                    _cycle = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private uint _cycle = 0;

        [Category(MethodCategory), Browsable(false), Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)), TypeConverter(typeof(HexByteArrayConverter)),
            Description("CAN 메시지 데이터를 설정합니다. CAN 읽기인 경우 이 값이 비어있지 않으면 응답 데이터의 앞부분이 이 데이터와 일치하는지 비교합니다.")]
        public byte[] MessageData
        {
            get => _messageData;
            set
            {
                if (_messageData != value)
                {
                    _messageData = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _messageData;

        [Category(MethodCategory), Browsable(false), Editor(typeof(CanSignalsEditor), typeof(UITypeEditor)), TypeConverter(typeof(CanSignalsConverter)),
            Description("CAN 메시지를 전송할 때 포함할 신호값들을 지정합니다. " + nameof(MessageData) + "로 지정한 데이터에 신호값들을 설정하여 전송합니다.")]
        public List<CanSignal> Signals
        {
            get => _signals;
            set
            {
                if (_signals != value)
                {
                    _signals = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private List<CanSignal> _signals;

        [Category(MethodCategory), Browsable(false), DefaultValue(false),
            Description("CAN 메시지를 CAN TP 채널을 이용하여 전송할지 여부를 설정합니다.")]
        public bool CAN_TP
        {
            get => _can_TP;
            set
            {
                if (_can_TP != value)
                {
                    _can_TP = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _can_TP;

        [Category(MethodCategory), Browsable(false), DisplayName(nameof(MaxReadTime) + " [ms]"),
            Description("지정한 CAN ID를 가진 CAN 메시지를 읽기 위한 최대 시간(밀리초). 이 시간동안 지정한 CAN ID를 가진 CAN 메시지를 읽지 못하면 스텝의 결과는 FAIL로 됩니다.")]
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
        private int _maxReadTime = 210;

        [Category(MethodCategory), Browsable(false), Editor(typeof(CanSignalNameEditor), typeof(UITypeEditor)), TypeConverter(typeof(CanSignalNameConverter)),
            Description("CAN 메시지 데이터에 포함된 CAN 신호 이름. 비어있으면 신호값 검사를 진행하지 않습니다.")]
        public string SignalName
        {
            get => _signalName;
            set
            {
                if (_signalName != value)
                {
                    _signalName = value;
                    UpdateBrowsableAttributes();
                    NotifyPropertyChanged();
                }
            }
        }
        private string _signalName;

        [Category(MethodCategory), Browsable(false), Editor(typeof(CanSignalNameEditor), typeof(UITypeEditor)), TypeConverter(typeof(CanSignalNameConverter)),
            Description("CAN 메시지 데이터에 포함된 CAN 신호 이름. " + nameof(SignalName) + "에 의해 지정된 Signal에 여기서 지정된 Signal값을 더해서 최종 결과값을 만듭니다.")]
        public string AdditionalSignalName
        {
            get => _additionalSignalName;
            set
            {
                if (_additionalSignalName != value)
                {
                    _additionalSignalName = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _additionalSignalName;

        [Category(MethodCategory), Browsable(false), DefaultValue(0),
            Description("Read 종료 조건. CAN 시그널을 읽을 때 같은 시그널 값이 연속으로 여기에 지정한 횟수만큼 나오면 읽기를 종료합니다. 이 값이 1 이하이면 적용되지 않습니다.")]
        public int SameSignalValueCount
        {
            get => _sameSignalValueCount;
            set
            {
                if (_sameSignalValueCount != value)
                {
                    _sameSignalValueCount = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _sameSignalValueCount = 0;

        [Category(MethodCategory), Browsable(false),
            Description("Read 종료 조건. CAN 시그널을 읽을 때 읽은 시그널 값이 이 값(10진수)과 같거나 클 때까지 읽습니다. 비워두면 적용되지 않습니다.")]
        public double? SignalValueMinToFinish
        {
            get => _signalValueMinToFinish;
            set
            {
                if (_signalValueMinToFinish != value)
                {
                    _signalValueMinToFinish = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _signalValueMinToFinish;

        [Category(MethodCategory), Browsable(false),
            Description("Read 종료 조건. CAN 시그널을 읽을 때 읽은 시그널 값이 이 값(10진수)과 같거나 작을 때까지 읽습니다. 비워두면 적용되지 않습니다.")]
        public double? SignalValueMaxToFinish
        {
            get => _signalValueMaxToFinish;
            set
            {
                if (_signalValueMaxToFinish != value)
                {
                    _signalValueMaxToFinish = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private double? _signalValueMaxToFinish;

        [Category(MethodCategory), Browsable(false), DefaultValue(UDS_ECU_Address.ECU_6),
            Description("ISO 15765-4에 따른 UDS ECU 주소를 지정합니다.")]
        public UDS_ECU_Address UDS_ECU_Address
        {
            get => _uds_ECU_Address;
            set
            {
                if (_uds_ECU_Address != value)
                {
                    _uds_ECU_Address = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private UDS_ECU_Address _uds_ECU_Address = UDS_ECU_Address.ECU_6;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(UShortHexConverter)),
            Description("UDS DID(Data Identifier)를 지정합니다.")]
        public ushort UDS_DID
        {
            get => _uds_DID;
            set
            {
                if (_uds_DID != value)
                {
                    _uds_DID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ushort _uds_DID;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(HexByteArrayConverter)), Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)),
            Description("UDS로 Write할 데이터를 설정합니다.")]
        public byte[] UDS_WriteData
        {
            get => _uds_writeData;
            set
            {
                if (_uds_writeData != value)
                {
                    _uds_writeData = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _uds_writeData;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(HexByteArrayConverter)), Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)),
            Description("체크할 UDS 응답 데이터를 지정합니다. 이 값과 " + nameof(UDS_ResponseASCII) + "가 비어있으면 응답 체크를 하지 않습니다.")]
        public byte[] UDS_Response
        {
            get => _uds_Response;
            set
            {
                if (_uds_Response != value)
                {
                    _uds_Response = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _uds_Response;

        [Category(MethodCategory), Browsable(false),
            Description("체크할 UDS 응답 데이터를 ASCII 문자열로 지정합니다. " + nameof(UDS_Response) + "가 비어있을 때만 이 값을 이용합니다.")]
        public string UDS_ResponseASCII
        {
            get => _uds_responseASCII;
            set
            {
                if (_uds_responseASCII != value)
                {
                    _uds_responseASCII = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private string _uds_responseASCII;

        [Category(MethodCategory), Browsable(false), DefaultValue(true),
            Description("UDS ReadDataByIdentifier ROM ID(DID=0xF1B1)의 경우, MES에서 받은 ROM ID와 진단통신 ROM ID를 비교할 것인지 설정합니다.")]
        public bool UDS_RDBI_Use_MES_ROM_ID
        {
            get => _useMES_ROM_ID;
            set
            {
                if (_useMES_ROM_ID != value)
                {
                    _useMES_ROM_ID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private bool _useMES_ROM_ID = true;

        [Category(MethodCategory), Browsable(false), 
            Description("UDS Diagnostic Session Control로 설정할 Session Type을 설정합니다.")]
        public UDS_SessionType UDS_SessionControlType
        {
            get => _uds_SessionControlType;
            set
            {
                if (_uds_SessionControlType != value)
                {
                    _uds_SessionControlType = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private UDS_SessionType _uds_SessionControlType = UDS_SessionType.ExtendedDiagnosticSession;

        [Category(MethodCategory), Browsable(false),
            Description("UDS Routine Control Type을 설정합니다.")]
        public UDS_RoutineControlType UDS_RoutineControl
        {
            get => _uds_RoutineControl;
            set
            {
                if (_uds_RoutineControl != value)
                {
                    _uds_RoutineControl = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private UDS_RoutineControlType _uds_RoutineControl = UDS_RoutineControlType.StartRoutine;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(UShortHexConverter)), 
            Description("UDS Server Local Routine Identifier.")]
        public ushort UDS_RoutineID
        {
            get => _uds_RoutineID;
            set
            {
                if (_uds_RoutineID != value)
                {
                    _uds_RoutineID = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ushort _uds_RoutineID;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(HexByteArrayConverter)), Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)),
            Description("UDS Routine Control Options (only with start and stop routine sub-functions).")]
        public byte[] UDS_RoutineControlOptions
        {
            get => _uds_RoutineControlOptions;
            set
            {
                if (_uds_RoutineControlOptions != value)
                {
                    _uds_RoutineControlOptions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _uds_RoutineControlOptions;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(HexByteArrayConverter)), Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)),
            Description("UDS InputOutputControlByIdentifier Control Options.")]
        public byte[] UDS_InputOutputControlOptions
        {
            get => _uds_InputOutputControlOptions;
            set
            {
                if (_uds_InputOutputControlOptions != value)
                {
                    _uds_InputOutputControlOptions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _uds_InputOutputControlOptions;

        [Category(MethodCategory), Browsable(false), DefaultValue((byte)10), TypeConverter(typeof(ByteHexConverter)),
            DisplayName(nameof(UDS_ParameterBlockSize) + " [Default = 0A(10)]"),
            Description("Possible Values: 0x00 (unlimited) to 0xFF. This value is used to set the BlockSize (BS) parameter " +
            "defined in the ISO-TP standard: it indicates to the sender the maximum number of consecutive frames that can be " +
            "received without an intermediate FlowControl frame from the receiving network entity. A value of 0 indicates " +
            "that no limit is set, and the sending network layer entity shall send all remaining consecutive frames.")]
        public byte? UDS_ParameterBlockSize
        {
            get => _uds_ParameterBlockSize;
            set
            {
                if (_uds_ParameterBlockSize != value)
                {
                    _uds_ParameterBlockSize = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte? _uds_ParameterBlockSize;

        [Category(MethodCategory), Browsable(false), DefaultValue((byte)10), TypeConverter(typeof(ByteHexConverter)),
            DisplayName(nameof(UDS_ParameterSeparationTime) + " [Default = 0A(10)ms]"),
            Description("Possible values: 0x00 to 0x7F (range from 0 to 127 milliseconds) and 0xF1 to 0xF9 (range from 100 to 900μs). " +
            "This value is used to set the Separation Time (STmin) parameter defined in the ISO-TP standard: it indicates " +
            "the minimum time the sender is to wait between the transmissions of two Consecutive Frames. Values between 0xF1 to 0xF3 " +
            "should define a minimum time of 100 to 300 μs, but in practice the time to transmit effectively a frame takes about " +
            "300 μs (which is to send the message to the CAN controller and to assert that the message is physically emitted on " +
            "the CAN bus). Other values than the ones stated above are ISO reserved.")]
        public byte? UDS_ParameterSeparationTime
        {
            get => _uds_ParameterSeparationTime;
            set
            {
                if (_uds_ParameterSeparationTime != value)
                {
                    _uds_ParameterSeparationTime = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte? _uds_ParameterSeparationTime;

        [Category(MethodCategory), Browsable(false), TypeConverter(typeof(HexByteArrayConverter)),
            Editor(typeof(ByteArrayEditor), typeof(UITypeEditor)), 
            Description("Advanced SeedKey, CRC-16-CCITT 기능을 테스트하기 위한 데이터를 지정합니다.")]
        public byte[] TestData
        {
            get => _testData;
            set
            {
                if (_testData != value)
                {
                    _testData = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private byte[] _testData;

        /// <summary>
        /// Advanced SeedKey 라이브러리 종류.
        /// </summary>
        public enum ASKType
        {
            ICCU_Control,
            SX2_Inverter_Control,
        }

        [Category(MethodCategory), Browsable(false), DefaultValue(ASKType.ICCU_Control),
            Description("Advanced SeedKey 라이브러리 유형을 지정합니다.")]
        public ASKType ASK_Type
        {
            get => _ask_Type;
            set
            {
                if (_ask_Type != value)
                {
                    _ask_Type = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private ASKType _ask_Type = ASKType.ICCU_Control;

        [Category(MethodCategory), DefaultValue(false), Browsable(false),
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

        //
        // Summary 표시를 위한 변수들.
        //

        internal override string CategoryName => StepCategory.CAN.GetText();
        public override string TestModeDesc => TestMethod.ToString();

        public override string ParameterDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case CanTestMode.Open:
                        return $"Nominal={NominalBaudRate.GetText()}, Data={DataBaudRate.GetText()}";
                    case CanTestMode.Read:
                        return $"ID={MessageName}(0x{MessageID:X})";
                    case CanTestMode.Write:
                        var text = $"ID=0x{MessageID:X}  [{CanMessage.GetLengthFromDLC((byte)DLC, CAN_FD)}]";
                        if (MessageData != null)
                        {
                            var dataText = string.Join(" ", MessageData.Select(x => $"{x:X2}"));
                            text += "  " + dataText;
                        }
                        return text;
                    default:
                        return "";
                }
            }
        }

        public override string ExpectedValueDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case CanTestMode.Read:
                    case CanTestMode.UDS_ReadDataByIdentifier:
                        var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                        if (signal != null)
                        {
                            var valueText = GetPrefixExpression(ExpectedValue, Unit, out MetricPrefix prefix);
                            return $"{valueText}{prefix.GetText()}{Unit.GetText()}";
                        }
                        break;
                    default:
                        break;
                }

                return GetReadInfo();
            }
        }

        public override string TolerancePlusDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case CanTestMode.Read:
                    case CanTestMode.UDS_ReadDataByIdentifier:
                        var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                        if (signal != null)
                        {
                            return GetTolerancePlusDesc(Tolerance, TolerancePlusPercent, TolerancePlusMinusPercent,
                                TolerancePlusAbsolute, TolerancePlusMinusAbsolute, Unit);
                        }
                        else
                        {
                            return "";
                        }
                    default:
                        return "";
                }
            }
        }

        public override string ToleranceMinusDesc
        {
            get
            {
                switch (TestMethod)
                {
                    case CanTestMode.Read:
                    case CanTestMode.UDS_ReadDataByIdentifier:
                        var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                        if (signal != null)
                        {
                            return GetToleranceMinusDesc(Tolerance, ToleranceMinusPercent, TolerancePlusMinusPercent,
                                ToleranceMinusAbsolute, TolerancePlusMinusAbsolute, Unit);
                        }
                        else
                        {
                            return "";
                        }
                    default:
                        return "";
                }
            }
        }

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

        private string GetReadInfo()
        {
            string info = null;
            switch (TestMethod)
            {
                case CanTestMode.Read:
                    // CAN 정보 추출.
                    uint canMessageId;
                    CanDLC canMessageDlc;
                    if (UseMessageName)
                    {
                        var dbcMessage = DbcManager.SharedInstance.FindMessageByName(MessageName, null);
                        if (dbcMessage != null)
                        {
                            canMessageId = dbcMessage.ID;
                            canMessageDlc = dbcMessage.DLC;
                        }
                        else
                        {
                            // 메시지 이름을 사용하는데, 이름이 없다면 에러.
                            return null;
                        }
                    }
                    else
                    {
                        canMessageId = MessageID;
                        canMessageDlc = DLC;
                    }

                    // CAN ID.
                    info = $"0x{canMessageId:X}";

                    if (MessageData?.Length > 0)
                    {
                        // DLC.
                        var dataLength = CanMessage.GetLengthFromDLC((byte)canMessageDlc, false);
                        info += $" [{dataLength}]";

                        // Data.
                        var dataText = string.Join(" ", MessageData.Select(b => $"{b:X2}"));
                        info += $" {dataText}";
                    }
                    break;
                case CanTestMode.UDS_ReadDataByIdentifier:
                    // UDS_Response 가 있으면 표시.
                    if (UDS_Response?.Length > 0)
                    {
                        info = string.Join(" ", UDS_Response.Select(b => $"{b:X2}"));
                    }
                    else if (!string.IsNullOrEmpty(UDS_ResponseASCII))
                    {
                        info = UDS_ResponseASCII;
                    }
                    break;
            }

            return info;
        }

        private EolCanStep()
        {
            Name = StepCategory.CAN.GetText();
            _messageData = new byte[CanMessage.GetLengthFromDLC((byte)DLC, CAN_FD)];
        }

        public EolCanStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            return CanDevice.CreateInstance(DeviceName);
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSettings = settingsManager.GetCanSettings();
            return deviceSettings.Select(setting => setting.DeviceName);
        }

        public override ICollection GetTestModes()
        {
            try
            {
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var deviceSetting = settingsManager.FindSetting(DeviceCategory.CAN, DeviceName);
                switch (deviceSetting.DeviceType)
                {
                    case DeviceType.PeakCAN:
                        return new object[]
                        {
                            CanTestMode.SetLogMode,
                            CanTestMode.Open,
                            CanTestMode.Close,
                            CanTestMode.ResetQueues,
                            CanTestMode.Read,
                            CanTestMode.Write,
                            CanTestMode.WritePeriodic,
                            CanTestMode.StopPeriodic,
                            CanTestMode.UDS_Open,
                            CanTestMode.UDS_Close,
                            CanTestMode.UDS_ReadDataByIdentifier,
                            CanTestMode.UDS_WriteDataByIdentifier,
                            CanTestMode.UDS_DiagnosticSessionControl,
                            CanTestMode.UDS_SendKey,
                            CanTestMode.UDS_RoutineControl,
                            CanTestMode.UDS_InputOutputControlByIdentifier,
                            CanTestMode.UDS_SetParameters,
                            CanTestMode.ASK_KeyGenerate,
                            CanTestMode.CRC_16_CCITT_0xFFFF_Calculate,
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
                if (Enum.TryParse(stringValue, out CanTestMode parsed))
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
            switch (TestMethod)
            {
                case CanTestMode.Read:
                case CanTestMode.UDS_ReadDataByIdentifier:
                    var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                    if (signal != null)
                    {
                        nominalValue = ExpectedValue;
                        GetLimitValues(ExpectedValue, Tolerance, ToleranceMinusPercent, TolerancePlusPercent, TolerancePlusMinusPercent,
                            ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, out upperLimit, out lowerLimit);
                    }
                    else
                    {
                        nominalValue = null;
                        upperLimit = null;
                        lowerLimit = null;
                    }
                    break;
                default:
                    nominalValue = null;
                    upperLimit = null;
                    lowerLimit = null;
                    break;
            }
        }

        private void UpdateSignalValuesToFinish()
        {
            GetNominalValues(out _, out double? upperLimit, out double? lowerLimit);

            if (SignalValueMinToFinish != null)
            {
                SignalValueMinToFinish = lowerLimit;
            }

            if (SignalValueMaxToFinish != null)
            {
                SignalValueMaxToFinish = upperLimit;
            }
        }

        private static string CreateMessageSummary(string deviceName, bool received, DateTime time, uint canId, byte dlc, IEnumerable<byte> data)
        {
            var direction = received ? "Rx" : "Tx";
            var timeText = time.ToString("HH:mm:ss.fff");
            var dataLength = CanMessage.GetLengthFromDLC(dlc, false);
            var dataText = string.Join(" ", data.Take(dataLength).Select(x => $"{x:X2}"));
            return $"{deviceName} {direction} [{timeText}] {canId:X} [{dataLength}] {dataText}";
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

            var canDevice = device as CanDevice;
            if (canDevice == null)
            {
                return result;
            }

            switch (TestMethod)
            {
                case CanTestMode.SetLogMode:
                    if (LogEnabled)
                    {
                        canDevice.SetLogMode(CanLogModes.TxMessage | CanLogModes.RxMessage, AppSettings.SharedInstance.LogFolderPath);
                    }
                    canDevice.SetLogEnabled(LogEnabled);
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.Open:
                    canDevice.Open(NominalBaudRate, DataBaudRate);
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.Close:
                    canDevice.Close();
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.ResetQueues:
                    canDevice.ResetQueues();
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.Read:
                    // 메시지 이름을 지정했다면 해당 데이터 이용.
                    uint canMessageId;
                    CanDLC canMessageDlc;
                    if (UseMessageName)
                    {
                        var dbcMessage = DbcManager.SharedInstance.FindMessageByName(MessageName, null);
                        if (dbcMessage != null)
                        {
                            canMessageId = dbcMessage.ID;
                            canMessageDlc = dbcMessage.DLC;
                        }
                        else
                        {
                            // 메시지 이름을 사용하는데, 이름이 없다면 에러.
                            var errorMessage = $"지정한 이름({MessageName})을 가진 CAN 메시지를 데이터베이스에서 찾을 수 없습니다.";
                            Logger.LogError(errorMessage);
                            result.ResultInfo = errorMessage;
                            result.ResultState = ResultState.Fail;
                            break;
                        }
                    }
                    else
                    {
                        canMessageId = MessageID;
                        canMessageDlc = DLC;
                    }

                    // 최대 읽기 시간 MaxReadTime 동안 검사에 필요한 메시지가 발견될 때까지 읽기.
                    var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                    var addingSignal = DbcManager.SharedInstance.FindSignalByName(AdditionalSignalName, null, out _);
                    var bootTime = DateTime.Now.AddMilliseconds(-Environment.TickCount);
                    bool messageRead = false;
                    double? lastSignalValue = null;
                    int sameSignalValueCount = 0;
                    var canLogMessages = new StringBuilder();
                    var stopwatch = Stopwatch.StartNew();
                    while (stopwatch.ElapsedMilliseconds < MaxReadTime)
                    {
                        var readMessages = canDevice.ReadAll();
                        if (readMessages.Count == 0)
                        {
                            Task.Delay(10).Wait(token);
                            continue;
                        }

                        bool finalSignal = false;
                        foreach (var recvMessage in readMessages)
                        {
                            finalSignal = false;

                            // CAN ID, DLC 비교.
                            if (recvMessage.ID != canMessageId || recvMessage.DLC != (byte)canMessageDlc)
                            {
                                continue;
                            }

                            // 메시지 읽음.
                            messageRead = true;

                            // 로그.
                            var dataLength = CanMessage.GetLengthFromDLC(recvMessage.DLC, false);
                            var messageTime = bootTime.AddMilliseconds(recvMessage.Timestamp / 1000);
                            var messageSummary = CreateMessageSummary(DeviceName, true, messageTime, recvMessage.ID, recvMessage.DLC, recvMessage.Data);
                            result.ResultInfo = messageSummary;
                            canLogMessages.AppendLine(messageSummary);

                            // Data 비교.
                            if (MessageData?.Length > 0)
                            {
                                result.ResultData = $"0x{recvMessage.ID:X} [{dataLength}] {string.Join(" ", recvMessage.Data.Take(dataLength).Select(x => $"{x:X2}"))}";

                                var compareLength = Math.Min(MessageData.Length, dataLength);
                                bool equals = ByteArrayEquals(MessageData, 0, recvMessage.Data, 0, compareLength);
                                if (!equals)
                                {
                                    // 수신한 메시지의 데이터 앞부분이 설정한 데이터와 일치하지 않는 경우.
                                    // 다음 메시지를 체크한다.
                                    continue;
                                }
                            }
                            else
                            {
                                result.ResultData = $"0x{recvMessage.ID:X}";
                            }

                            // Signal 검사.
                            if (signal == null)
                            {
                                result.ResultState = ResultState.Pass;
                                break;
                            }

                            var signalValue = GetSignalValue(signal, recvMessage.Data, 0, dataLength, out _, out string errorMessage);
                            if (signalValue == null)
                            {
                                // 값을 추출 못했으면 시작비트, 비트길이가 잘못된 것임.
                                result.ResultInfo += ", " + errorMessage;
                                result.ResultState = ResultState.Fail;
                                break;
                            }
                            else
                            {
                                // 메인 신호에 더해지는 추가 신호.
                                if (addingSignal != null)
                                {
                                    var addingSignalValue = GetSignalValue(addingSignal, recvMessage.Data, 0, dataLength, out _, out errorMessage);
                                    if (addingSignalValue == null)
                                    {
                                        // 값을 추출 못했으면 시작비트, 비트길이가 잘못된 것임.
                                        result.ResultInfo += ", " + errorMessage;
                                        result.ResultState = ResultState.Fail;
                                        break;
                                    }

                                    signalValue += addingSignalValue;
                                }

                                // 신호 읽기 종료 조건 체크.
                                finalSignal = CheckSignalValueToFinish(signalValue ?? 0, ref lastSignalValue, ref sameSignalValueCount);
                                if (finalSignal)
                                {
                                    break;
                                }
                            }
                        }

                        if (finalSignal)
                        {
                            break;
                        }

                        if (result.ResultState != ResultState.NoState)
                        {
                            break;
                        }
                    }
                    stopwatch.Stop();

                    // 로그 출력.
                    if (canLogMessages.Length > 0)
                    {
                        var msg = canLogMessages.ToString();
                        result.ResultLogBody = msg;
                        Logger.LogInfo("CAN messages:" + Environment.NewLine + msg);
                    }

                    // 결과가 결정된 경우.
                    if (result.ResultState != ResultState.NoState)
                    {
                        break;
                    }

                    if (!messageRead)
                    {
                        // 지정한 CAN ID와 DLC를 가진 메시지를 읽지 못한 경우.
                        result.ResultInfo = $"CAN ID 0x{canMessageId:X} 를 가진 메시지를 읽지 못했습니다.";
                        result.ResultState = ResultState.Fail;
                    }
                    else if (lastSignalValue != null)
                    {
                        result.Unit = Unit;
                        result.ResultValue = lastSignalValue - MeasureOffset;
                        result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue.GetValueOrDefault(), Tolerance, ToleranceMinusPercent,
                            TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, 
                            out double? cpAdjusted);

                        if (result.ResultValueState != ResultValueState.Good && Temp)
                        {
                            // HACK: Method == Temp 인 경우 가성 데이터 생성.
                            var randomValue = GetTempValue(Tolerance, ExpectedValue, TolerancePlusMinusAbsolute, TolerancePlusAbsolute,
                                ToleranceMinusAbsolute, TolerancePlusMinusPercent, TolerancePlusPercent, ToleranceMinusPercent);
                            result.ResultValue = Math.Round(randomValue, 3);
                            result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue.GetValueOrDefault(), Tolerance, ToleranceMinusPercent,
                                TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute, 
                                out cpAdjusted);
                        }

                        if (cpAdjusted != null)
                        {
                            result.ResultValue = cpAdjusted;
                        }

                        switch (result.ResultValueState)
                        {
                            case ResultValueState.Good:
                                result.ResultState = ResultState.Pass;
                                break;
                            case ResultValueState.Invalid:
                            case ResultValueState.Bad:
                            case ResultValueState.Low:
                            case ResultValueState.High:
                            default:
                                result.ResultState = ResultState.Fail;
                                break;
                        }
                    }
                    else
                    {
                        // 메시지를  읽었지만, 데이터가 일치하지 않는 경우.
                        string logMessage = $"수신한 CAN 메시지(ID=0x{canMessageId:X})의 데이터가 설정한 데이터와 일치하지 않습니다.";
                        Logger.LogError(logMessage);
                        result.ResultInfo += ", Data Error";
                        result.ResultState = ResultState.Fail;
                    }
                    break;
                case CanTestMode.Write:
                case CanTestMode.WritePeriodic:
                    // 메시지 이름을 지정했다면 해당 데이터 이용.
                    uint sendCycle;
                    if (UseMessageName)
                    {
                        var dbcMessage = DbcManager.SharedInstance.FindMessageByName(MessageName, null);
                        if (dbcMessage != null)
                        {
                            canMessageId = dbcMessage.ID;
                            canMessageDlc = dbcMessage.DLC;
                            sendCycle = (uint)dbcMessage.CycleTime;
                        }
                        else
                        {
                            // 메시지 이름을 사용하는데, 이름이 없다면 에러.
                            var errorMessage = $"지정한 이름({MessageName})을 가진 CAN 메시지를 데이터베이스에서 찾을 수 없습니다.";
                            Logger.LogError(errorMessage);
                            result.ResultInfo = errorMessage;
                            result.ResultState = ResultState.Fail;
                            break;
                        }
                    }
                    else
                    {
                        canMessageId = MessageID;
                        canMessageDlc = DLC;
                        sendCycle = Cycle;
                    }

                    var message = new CanMessage();
                    message.ID = canMessageId;
                    message.DLC = (byte)canMessageDlc;
                    message.MessageType = ExtendedFrame ? CanMessageTypes.Extended : CanMessageTypes.Standard;
                    if (CAN_FD)
                    {
                        message.MessageType |= CanMessageTypes.FD;
                        if (BitRateSwitch)
                        {
                            message.MessageType |= CanMessageTypes.BRS;
                        }
                    }
                    else if (RemoteRequest)
                    {
                        message.MessageType |= CanMessageTypes.RTR;
                    }

                    if (!RemoteRequest)
                    {
                        message.Data = MessageData;
                    }

                    message.Cycle = sendCycle;

                    // 시그널 리스트.
                    if (Signals != null)
                    {
                        foreach (var sendingSignal in Signals)
                        {
                            // 시그널 정보 로딩.
                            var foundSignalInfo = DbcManager.SharedInstance.FindSignalByName(sendingSignal.SignalName, null, out _);
                            if (foundSignalInfo == null)
                            {
                                throw new Exception($"시그널 '{sendingSignal.SignalName}'을 찾을 수 없습니다.");
                            }
                            sendingSignal.SignalInfo = foundSignalInfo;
                        }
                    }
                    message.SendingSignals = Signals;

                    // 메시지 전송.
                    if (TestMethod == CanTestMode.Write)
                    {
                        canDevice.Send(message, CAN_TP);
                    }
                    else
                    {
                        canDevice.SendPeriodic(message, CAN_TP);
                    }

                    // 전송한 메시지 로그 표시.
                    var msgSummary = CreateMessageSummary(DeviceName, false, DateTime.Now, message.ID, message.DLC, message.Data);
                    Logger.LogInfo(msgSummary);

                    result.ResultInfo = msgSummary;
                    result.ResultState = ResultState.Pass;
                    result.ResultLogBody = msgSummary;
                    break;
                case CanTestMode.StopPeriodic:
                    // 메시지 이름을 지정했다면 해당 데이터 이용.
                    uint? canId;
                    if (UseMessageName)
                    {
                        if (string.IsNullOrEmpty(MessageName))
                        {
                            canId = null;
                        }
                        else
                        {
                            var dbcMessage = DbcManager.SharedInstance.FindMessageByName(MessageName, null);
                            if (dbcMessage != null)
                            {
                                canId = dbcMessage.ID;
                            }
                            else
                            {
                                // 메시지 이름을 사용하는데, 이름이 없다면 에러.
                                var errorMessage = $"지정한 이름({MessageName})을 가진 CAN 메시지를 데이터베이스에서 찾을 수 없습니다.";
                                Logger.LogError(errorMessage);
                                result.ResultInfo = errorMessage;
                                result.ResultState = ResultState.Fail;
                                break;
                            }
                        }
                    }
                    else
                    {
                        canId = MessageID;
                    }

                    // 메시지 주기적 전송 중지.
                    canDevice.StopPeriodic(canId, true);

                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.UDS_Open:
                    canDevice.UDS_Open(NominalBaudRate, DataBaudRate);
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.UDS_Close:
                    canDevice.UDS_Close();
                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.UDS_ReadDataByIdentifier:
                    var commLog = new StringBuilder();
                    canDevice.UDS_ReadDataByIdentifier(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_DID, out byte[] responseData, commLog, token);
                    result.ResultLogBody = commLog.ToString();
                    result.ResultInfo = $"UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                    if (UDS_DID == 0xF1C1 && responseData?.Length > 3)
                    {
                        var dataStr = string.Join(" ", responseData.Skip(3).Select(x => $"{x:X2}"));
                        result.ResultLogInfo = $"ECU ID : {UDS_ECU_Address}(0x{(ushort)UDS_ECU_Address:X})";
                        result.ResultLogInfo += $", IVD : {dataStr}";
                    }
                    if (responseData.Length >= 1)
                    {
                        // 응답 결과 저장.
                        if (responseData.Length > 3)
                        {
                            if ((UDS_Response == null || UDS_Response.Length == 0) && !string.IsNullOrEmpty(UDS_ResponseASCII))
                            {
                                // ASCII 응답 체크가 되어있으면 응답 데이터를 ASCII 문자열로 변환해 저장.
                                var dataText = Encoding.ASCII.GetString(responseData, 3, responseData.Length - 3);
                                result.ResultData = dataText;
                            }
                            else
                            {
                                // Hex string.
                                var dataText = string.Join(" ", responseData.Skip(3).Select(b => $"{b:X2}"));
                                result.ResultData = dataText;
                            }
                        }

                        bool responseOk = responseData[0] == 0x62;
                        if (responseOk)
                        {
                            // 응답 데이터 체크.
                            if (UDS_Response?.Length > 0 || !string.IsNullOrEmpty(UDS_ResponseASCII))
                            {
                                byte[] bytesToCompare;

                                // DID가 0xF1B1 이면, MES로부터 불러와 저장한 ROM ID를 체크한다.
                                var mesRomId = SequenceViewModel.GetRomId();
                                if (UDS_RDBI_Use_MES_ROM_ID && UDS_DID == 0xF1B1 && !string.IsNullOrEmpty(mesRomId))
                                {
                                    // MES ROM ID 와 비교.
                                    Logger.LogInfo($"MES ROM ID = {mesRomId}");
                                    bytesToCompare = Encoding.ASCII.GetBytes(mesRomId);
                                }
                                else
                                {
                                    if (UDS_Response?.Length > 0)
                                    {
                                        bytesToCompare = UDS_Response;
                                    }
                                    else
                                    {
                                        bytesToCompare = Encoding.ASCII.GetBytes(UDS_ResponseASCII);
                                    }
                                }

                                bool equals = ByteArrayEquals(responseData, 3, bytesToCompare, 0, bytesToCompare.Length);
                                result.ResultState = equals ? ResultState.Pass : ResultState.Fail;

                                // 결과
                            }
                            else
                            {
                                result.ResultState = ResultState.Pass;
                            }

                            // 신호 값 검사 진행.
                            if (result.ResultState == ResultState.Pass)
                            {
                                signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
                                if (signal != null)
                                {
                                    var value = GetSignalValue(signal, responseData, 3, responseData.Length - 3, out _, out string errorMessage);
                                    if (value == null)
                                    {
                                        result.ResultInfo += ", " + errorMessage;
                                        result.ResultState = ResultState.Fail;
                                    }
                                    else
                                    {
                                        result.Unit = Unit;
                                        result.ResultValue = value - MeasureOffset;
                                        result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue.GetValueOrDefault(), Tolerance, ToleranceMinusPercent,
                                            TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute,
                                            out double? cpAdjusted);

                                        if (result.ResultValueState != ResultValueState.Good && Temp)
                                        {
                                            // HACK: Method == Temp 인 경우 가성 데이터 생성.
                                            var randomValue = GetTempValue(Tolerance, ExpectedValue, TolerancePlusMinusAbsolute, TolerancePlusAbsolute,
                                                ToleranceMinusAbsolute, TolerancePlusMinusPercent, TolerancePlusPercent, ToleranceMinusPercent);
                                            result.ResultValue = Math.Round(randomValue, 3);
                                            result.ResultValueState = CalcValueState(ExpectedValue, result.ResultValue.GetValueOrDefault(), Tolerance, ToleranceMinusPercent,
                                                TolerancePlusPercent, TolerancePlusMinusPercent, ToleranceMinusAbsolute, TolerancePlusAbsolute, TolerancePlusMinusAbsolute,
                                                out cpAdjusted);
                                        }

                                        if (cpAdjusted != null)
                                        {
                                            result.ResultValue = cpAdjusted;
                                        }

                                        switch (result.ResultValueState)
                                        {
                                            case ResultValueState.Good:
                                                result.ResultState = ResultState.Pass;
                                                break;
                                            case ResultValueState.Invalid:
                                            case ResultValueState.Bad:
                                            case ResultValueState.Low:
                                            case ResultValueState.High:
                                            default:
                                                result.ResultState = ResultState.Fail;
                                                break;
                                        }
                                    }
                                }
                            }
                        }
                        else
                        {
                            result.ResultState = ResultState.Fail;
                        }
                    }
                    else
                    {
                        result.ResultState = ResultState.Fail;
                    }
                    break;
                case CanTestMode.UDS_WriteDataByIdentifier:
                    commLog = new StringBuilder();
                    canDevice.UDS_WriteDataByIdentifier(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_DID, UDS_WriteData, out responseData, commLog, token);
                    result.ResultLogBody = commLog.ToString();
                    result.ResultInfo = $"UDS Rx [{responseData?.Length}] {string.Join(" ", responseData?.Select(x => $"{x:X2}"))}";
                    if (responseData.Length >= 1)
                    {
                        bool responseOk = responseData[0] == 0x6E;
                        if (responseOk)
                        {
                            result.ResultState = ResultState.Pass;
                            break;
                        }
                    }
                    result.ResultState = ResultState.Fail;
                    break;
                case CanTestMode.UDS_DiagnosticSessionControl:
                    commLog = new StringBuilder();
                    canDevice.UDS_DiagnosticSessionControl(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_SessionControlType, out responseData, commLog, token);
                    result.ResultLogBody = commLog.ToString();
                    result.ResultInfo = $"UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                    if (responseData.Length >= 1)
                    {
                        bool responseOk = responseData[0] == 0x50;
                        if (responseOk)
                        {
                            result.ResultState = ResultState.Pass;
                            break;
                        }
                    }
                    result.ResultState = ResultState.Fail;
                    break;
                case CanTestMode.UDS_SendKey:
                    // Seed 요청.
                    commLog = new StringBuilder();
                    canDevice.UDS_SecurityAccess(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_SecurityAccessType.RequestSeed_0x11, new byte[0], out responseData, commLog, token);
                    result.ResultInfo = $"UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                    Logger.LogInfo(result.ResultInfo);

                    if (responseData.Length >= 10 && responseData[0] == 0x67)
                    {
                        // Request Seed Success.
                        var seed = new byte[8];
                        Array.Copy(responseData, 2, seed, 0, 8);
                        var key = new byte[8];
                        int success = ASK_KeyGenerate(ASK_Type, seed, key);
                        Logger.LogInfo($"Seed = {string.Join(" ", seed.Select(b => $"{b:X2}"))}, Key = {string.Join(" ", key.Select(b => $"{b:X2}"))}");
                        if (success == SEEDKEY_SUCCESS)
                        {
                            canDevice.UDS_SecurityAccess(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_SecurityAccessType.SendKey_0x12, key, out responseData, commLog, token);
                            result.ResultLogBody = commLog.ToString();
                            result.ResultInfo += $", UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                            Logger.LogInfo(result.ResultInfo);
                            if (responseData.Length >= 1 && responseData[0] == 0x67)
                            {
                                result.ResultState = ResultState.Pass;
                                break;
                            }
                        }
                    }
                    result.ResultLogBody = commLog.ToString();
                    result.ResultState = ResultState.Fail;
                    break;
                case CanTestMode.UDS_RoutineControl:
                    byte[] controlOption = UDS_RoutineControlOptions ?? new byte[0];
                    commLog = new StringBuilder();
                    canDevice.UDS_RoutineControl(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_RoutineControl, UDS_RoutineID, controlOption, out responseData, commLog, token);
                    result.ResultLogBody = commLog.ToString();
                    result.ResultInfo = $"UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                    if (responseData.Length >= 1 && responseData[0] == 0x71)
                    {
                        result.ResultState = ResultState.Pass;
                        break;
                    }
                    result.ResultState = ResultState.Fail;
                    break;
                case CanTestMode.UDS_InputOutputControlByIdentifier:
                    controlOption = UDS_InputOutputControlOptions ?? new byte[0];
                    commLog = new StringBuilder();
                    canDevice.UDS_InputOutputControlByIdentifier(UDS_ECU_Address, CAN_FD, BitRateSwitch, UDS_DID, controlOption, out responseData, commLog, token);
                    result.ResultLogBody = commLog.ToString();
                    result.ResultInfo = $"UDS Rx [{responseData.Length}] {string.Join(" ", responseData.Select(x => $"{x:X2}"))}";
                    if (responseData.Length >= 1)
                    {
                        bool responseOk = responseData[0] == 0x6F;
                        if (responseOk)
                        {
                            result.ResultState = ResultState.Pass;
                        }
                        else
                        {
                            result.ResultState = ResultState.Fail;
                        }
                    }
                    else
                    {
                        result.ResultState = ResultState.Fail;
                    }
                    break;
                case CanTestMode.UDS_SetParameters:
                    if (UDS_ParameterBlockSize != null)
                    {
                        canDevice.UDS_SetValue((uint)Peak.Can.Uds.uds_parameter.PUDS_PARAMETER_BLOCK_SIZE, (uint)UDS_ParameterBlockSize);
                    }

                    if (UDS_ParameterSeparationTime != null)
                    {
                        canDevice.UDS_SetValue((uint)Peak.Can.Uds.uds_parameter.PUDS_PARAMETER_SEPARATION_TIME, (uint)UDS_ParameterSeparationTime);
                    }

                    result.ResultState = ResultState.Pass;
                    break;
                case CanTestMode.ASK_KeyGenerate:
                    if (TestData == null || TestData.Length == 0)
                    {
                        result.ResultInfo = nameof(TestData) + "가 비어 있습니다.";
                        result.ResultState = ResultState.Fail;
                    }
                    else
                    {
                        var seed = new byte[8];
                        Array.Copy(TestData, 0, seed, 0, Math.Min(TestData.Length, seed.Length));
                        var key = new byte[8];
                        int success = ASK_KeyGenerate(ASK_Type, seed, key);
                        string resultMessage = $"Seed = {string.Join(" ", seed.Select(b => $"{b:X2}"))}, Key = {string.Join(" ", key.Select(b => $"{b:X2}"))}" + 
                            $", Result = {success}";
                        Logger.LogInfo(resultMessage);
                        result.ResultInfo = resultMessage;
                        result.ResultState = success == SEEDKEY_SUCCESS ? ResultState.Pass : ResultState.Fail;
                    }
                    break;
                case CanTestMode.CRC_16_CCITT_0xFFFF_Calculate:
                    if (TestData == null || TestData.Length == 0)
                    {
                        result.ResultInfo = nameof(TestData) + "가 비어 있습니다.";
                        result.ResultState = ResultState.Fail;
                    }
                    else
                    {
                        var crc = NullFX.CRC.Crc16.ComputeChecksum(NullFX.CRC.Crc16Algorithm.CcittInitialValue0xFFFF, TestData);
                        string byteSuffix = TestData.Length > 1 ? "s" : "";
                        string resultMessage = $"TestData = {TestData.Length}Byte{byteSuffix}, CRC-16 = 0x{crc:X4}";
                        Logger.LogInfo(resultMessage);
                        result.ResultInfo = resultMessage;
                        result.ResultState = ResultState.Pass;
                    }
                    break;
                default:
                    throw new NotSupportedException($"디바이스 {DeviceName} 에 대해 {TestMethod} 기능을 사용할 수 없습니다.");
            }

            return result;
        }

        // 시그널 값이 종료 조건에 맞는가 검사.
        private bool CheckSignalValueToFinish(double signalValue, ref double? lastSignalValue, ref int sameSignalValueCount)
        {
            // 최종 같은 시그널 값 횟수 판단.
            if (lastSignalValue == signalValue)
            {
                sameSignalValueCount++;
            }
            else
            {
                sameSignalValueCount = 1;
            }
            lastSignalValue = signalValue;

            bool finalValueCount;
            if (SameSignalValueCount > 1)
            {
                finalValueCount = sameSignalValueCount >= SameSignalValueCount;
            }
            else
            {
                finalValueCount = true;
            }

            // 최종 최소 시그널 값 판단.
            bool finalMinValue;
            if (SignalValueMinToFinish != null)
            {
                finalMinValue = signalValue >= SignalValueMinToFinish;
            }
            else
            {
                finalMinValue = true;
            }

            // 최종 최대 시그널 값 판단.
            bool finalMaxValue;
            if (SignalValueMaxToFinish != null)
            {
                finalMaxValue = signalValue <= SignalValueMaxToFinish;
            }
            else
            {
                finalMaxValue = true;
            }

            return finalValueCount && finalMinValue && finalMaxValue;
        }

        private double? GetSignalValue(DbcSignal signal, byte[] data, int dataStartIndex, int dataLength, out ulong bitValue, out string errorMessage)
        {
            // 신호의 시작위치와 길이가 데이터 범위 내에 있는지 검사.
            if (signal.StartBit < 0 || signal.StartBit >= dataLength * 8 || signal.Length <= 0 || (signal.StartBit + signal.Length) > dataLength * 8)
            {
                errorMessage = "신호의 StartBit와 Length가 데이터 범위를 벗어났습니다.";
                bitValue = 0;
                return null;
            }

            bitValue = CanSignalUnpack(data, dataStartIndex, dataLength, signal.StartBit, signal.Length, signal.IntelByteOrder);
            double value;
            if (signal.Length <= 8)
            {
                if (!signal.Signed)
                {
                    value = (byte)bitValue;
                }
                else
                {
                    value = (sbyte)bitValue;
                }
            }
            else if (signal.Length <= 16)
            {
                if (!signal.Signed)
                {
                    value = (ushort)bitValue;
                }
                else
                {
                    value = (short)bitValue;
                }
            }
            else if (signal.Length <= 32)
            {
                if (!signal.Signed)
                {
                    value = (uint)bitValue;
                }
                else
                {
                    value = (int)bitValue;
                }
            }
            else
            {
                if (!signal.Signed)
                {
                    value = bitValue;
                }
                else
                {
                    value = (long)bitValue;
                }
            }
            value = value * signal.Factor + signal.Offset;

            errorMessage = null;
            return value;
        }

        // 바이트 데이터의 일치 여부를 판단한다.
        private bool ByteArrayEquals(byte[] first, int firstStartIndex, byte[] second, int secondStartIndex, int length)
        {
            if (length <= 0)
            {
                return true;
            }

            if (first == null || second == null)
            {
                return false;
            }

            if (firstStartIndex < 0 || first.Length < firstStartIndex + length)
            {
                return false;
            }

            if (secondStartIndex < 0 || second.Length < secondStartIndex + length)
            {
                return false;
            }

            for (int i = 0; i < length; i++)
            {
                if (first[firstStartIndex + i] != second[secondStartIndex + i])
                {
                    return false;
                }
            }

            return true;
        }

        /// <summary>
        /// Unpack a signal from a CAN message.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <param name="startBit"></param>
        /// <param name="bitLength"></param>
        /// <returns></returns>
        public static ulong CanSignalUnpack(byte[] data, int offset, int length, int startBit, int bitLength, bool isLittleEndian)
        {
            // 신호가 시작되는 바이트 번호.
            int startByte = startBit / 8;

            // 신호가 시작되는 바이트 내에서 시작비트 위치.
            int startBitInByte = startBit % 8;

            // 신호의 바이트 길이.
            int byteLength = (startBitInByte + bitLength) / 8 + ((startBitInByte + bitLength) % 8 > 0 ? 1 : 0);

            ulong bytesValue = 0;
            for (int i = 0; i < byteLength; i++)
            {
                if (isLittleEndian)
                {
                    bytesValue += (ulong)data[offset + startByte + i] << (8 * i);
                }
                else
                {
                    bytesValue += (ulong)data[offset + length - 1 - startByte - i] << (8 * i);
                }
            }

            ulong bitMask = (1UL << bitLength) - 1;
            var value = (bytesValue >> startBitInByte) & bitMask;

            return value;
        }

        /// <summary>
        /// 신호값들을 데이터에 쓴다.
        /// </summary>
        /// <param name="data"></param>
        /// <param name="signals"></param>
        public static void CanSignalPack(byte[] data, int offset, int length, IEnumerable<CanSignal> signals)
        {
            foreach (var signal in signals)
            {
                if (signal.SignalInfo.Factor == 0)
                {
                    throw new Exception($"CAN신호 {signal.SignalInfo.Name}의 Factor가 0입니다.");
                }

                // 신호의 시작비트와 길이가 데이터 범위 내에 있는지 검사한다.
                if (signal.SignalInfo.StartBit < 0)
                {
                    throw new Exception($"CAN신호 {signal.SignalInfo.Name}의 시작위치({signal.SignalInfo.StartBit})가 0보다 작습니다.");
                }

                if (signal.SignalInfo.Length <= 0)
                {
                    throw new Exception($"CAN신호 {signal.SignalInfo.Name}의 길이({signal.SignalInfo.Length})가 0과 같거나 작습니다.");
                }

                if (signal.SignalInfo.StartBit + signal.SignalInfo.Length > length * 8)
                {
                    throw new Exception($"CAN신호 {signal.SignalInfo.Name}(Start = {signal.SignalInfo.StartBit}, Length = {signal.SignalInfo.Length})가 데이터 범위(Length = {length*8})를 벗어납니다.");
                }

                // 신호값을 데이터에 설정.
                int startByte = signal.SignalInfo.StartBit / 8;
                int startBitInByte = signal.SignalInfo.StartBit % 8;
                int byteLength = (startBitInByte + signal.SignalInfo.Length) / 8 + ((startBitInByte + signal.SignalInfo.Length) % 8 > 0 ? 1 : 0);

                ulong bitMask = (1UL << signal.SignalInfo.Length) - 1;
                ulong bitValue = (ulong)((signal.Value - signal.SignalInfo.Offset) / signal.SignalInfo.Factor) & bitMask;

                // Debugging.
                //Logger.LogInfo($"Signal = {signal.SignalInfo.Name}, Value = {signal.Value}, Bit Value = {bitValue}(0x{bitValue:X})");

                for (int i = 0; i < byteLength; i++)
                {
                    int byteIndex;
                    if (signal.SignalInfo.IntelByteOrder)
                    {
                        byteIndex = offset + startByte + i;
                    }
                    else
                    {
                        byteIndex = offset + length - 1 - startByte - i;
                    }

                    byte dataBitMask = 0;
                    if (i == 0)
                    {
                        dataBitMask = (byte)((1 << startBitInByte) - 1);
                    }

                    if (i == byteLength - 1)
                    {
                        int lastByteRemainderBits = byteLength * 8 - signal.SignalInfo.Length - startBitInByte;
                        if (lastByteRemainderBits > 0)
                        {
                            dataBitMask += (byte)(0xFF << (8 - lastByteRemainderBits));
                        }
                    }

                    byte signalBitMask = (byte)(0xFF - dataBitMask);
                    byte signalByte = (byte)(((bitValue << startBitInByte) & (0xFFUL << (i * 8))) >> (i * 8));

                    data[byteIndex] = (byte)((data[byteIndex] & dataBitMask) + (signalByte & signalBitMask));
                }
            }
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return Unit;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();

            var browsable = Utils.GetBrowsableAttribute(this, nameof(CP));
            bool visible = browsable ?? false;
            Utils.SetBrowsableAttribute(this, nameof(Temp), visible);
            Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), visible);
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            // 검사하려는 Signal이 있는 경우에만 Expected, Tolerance 를 보여준다.
            var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
            bool showSignalName = TestMethod == CanTestMode.Read || TestMethod == CanTestMode.UDS_ReadDataByIdentifier;
            bool showTolerance = showSignalName && signal != null;

            Utils.SetBrowsableAttribute(this, nameof(LogEnabled), TestMethod == CanTestMode.SetLogMode);

            Utils.SetBrowsableAttribute(this, nameof(Unit), showTolerance);
            Utils.SetBrowsableAttribute(this, nameof(ExpectedValue), showTolerance);
            Utils.SetBrowsableAttribute(this, nameof(Tolerance), showTolerance);
            UpdateToleranceAttributes();
            UpdateDisplayNames();
            //Utils.SetBrowsableAttribute(this, nameof(MeasureOffset), showTolerance);

            Utils.SetBrowsableAttribute(this, nameof(NominalBaudRate), TestMethod == CanTestMode.Open || TestMethod == CanTestMode.UDS_Open);
            Utils.SetBrowsableAttribute(this, nameof(DataBaudRate), TestMethod == CanTestMode.Open || TestMethod == CanTestMode.UDS_Open);

            bool isWrite = TestMethod == CanTestMode.Write || TestMethod == CanTestMode.WritePeriodic;
            bool isReadWrite = TestMethod == CanTestMode.Read || isWrite;
            Utils.SetBrowsableAttribute(this, nameof(UseMessageName), isReadWrite || TestMethod == CanTestMode.StopPeriodic);
            Utils.SetBrowsableAttribute(this, nameof(MessageName), (isReadWrite || TestMethod == CanTestMode.StopPeriodic) && UseMessageName);
            Utils.SetBrowsableAttribute(this, nameof(MessageID), (isReadWrite || TestMethod == CanTestMode.StopPeriodic) && !UseMessageName);
            Utils.SetBrowsableAttribute(this, nameof(DLC), isReadWrite && !UseMessageName);
            Utils.SetBrowsableAttribute(this, nameof(Cycle), isWrite && !UseMessageName);
            Utils.SetBrowsableAttribute(this, nameof(MaxReadTime), TestMethod == CanTestMode.Read);
            Utils.SetBrowsableAttribute(this, nameof(SignalName), showSignalName);
            Utils.SetBrowsableAttribute(this, nameof(AdditionalSignalName), showSignalName && signal != null);
            Utils.SetBrowsableAttribute(this, nameof(SameSignalValueCount), TestMethod == CanTestMode.Read && signal != null);
            Utils.SetBrowsableAttribute(this, nameof(SignalValueMinToFinish), TestMethod == CanTestMode.Read && signal != null);
            Utils.SetBrowsableAttribute(this, nameof(SignalValueMaxToFinish), TestMethod == CanTestMode.Read && signal != null);

            bool isUds = TestMethod == CanTestMode.UDS_ReadDataByIdentifier || TestMethod == CanTestMode.UDS_WriteDataByIdentifier
                || TestMethod == CanTestMode.UDS_DiagnosticSessionControl || TestMethod == CanTestMode.UDS_SendKey
                || TestMethod == CanTestMode.UDS_RoutineControl || TestMethod == CanTestMode.UDS_InputOutputControlByIdentifier;

            Utils.SetBrowsableAttribute(this, nameof(ExtendedFrame), isWrite);
            Utils.SetBrowsableAttribute(this, nameof(RemoteRequest), isWrite && !CAN_FD);
            Utils.SetBrowsableAttribute(this, nameof(CAN_FD), isWrite && !RemoteRequest || isUds);
            Utils.SetBrowsableAttribute(this, nameof(BitRateSwitch), (isWrite || isUds) && CAN_FD);
            Utils.SetBrowsableAttribute(this, nameof(MessageData), isWrite || TestMethod == CanTestMode.Read);
            Utils.SetBrowsableAttribute(this, nameof(Signals), isWrite);
            Utils.SetBrowsableAttribute(this, nameof(CAN_TP), isWrite);

            Utils.SetBrowsableAttribute(this, nameof(UDS_ECU_Address), isUds);
            Utils.SetBrowsableAttribute(this, nameof(UDS_DID), TestMethod == CanTestMode.UDS_ReadDataByIdentifier
                || TestMethod == CanTestMode.UDS_WriteDataByIdentifier || TestMethod == CanTestMode.UDS_InputOutputControlByIdentifier);
            Utils.SetBrowsableAttribute(this, nameof(UDS_WriteData), TestMethod == CanTestMode.UDS_WriteDataByIdentifier);
            Utils.SetBrowsableAttribute(this, nameof(UDS_Response), TestMethod == CanTestMode.UDS_ReadDataByIdentifier);
            Utils.SetBrowsableAttribute(this, nameof(UDS_ResponseASCII), TestMethod == CanTestMode.UDS_ReadDataByIdentifier);
            Utils.SetBrowsableAttribute(this, nameof(UDS_RDBI_Use_MES_ROM_ID), TestMethod == CanTestMode.UDS_ReadDataByIdentifier && UDS_DID == 0xF1B1);
            Utils.SetBrowsableAttribute(this, nameof(UDS_SessionControlType), TestMethod == CanTestMode.UDS_DiagnosticSessionControl);
            Utils.SetBrowsableAttribute(this, nameof(UDS_RoutineControl), TestMethod == CanTestMode.UDS_RoutineControl);
            Utils.SetBrowsableAttribute(this, nameof(UDS_RoutineID), TestMethod == CanTestMode.UDS_RoutineControl);
            Utils.SetBrowsableAttribute(this, nameof(UDS_RoutineControlOptions), TestMethod == CanTestMode.UDS_RoutineControl);
            Utils.SetBrowsableAttribute(this, nameof(UDS_InputOutputControlOptions), TestMethod == CanTestMode.UDS_InputOutputControlByIdentifier);
            Utils.SetBrowsableAttribute(this, nameof(UDS_ParameterBlockSize), TestMethod == CanTestMode.UDS_SetParameters);
            Utils.SetBrowsableAttribute(this, nameof(UDS_ParameterSeparationTime), TestMethod == CanTestMode.UDS_SetParameters);

            Utils.SetBrowsableAttribute(this, nameof(ASK_Type), TestMethod == CanTestMode.UDS_SendKey || TestMethod == CanTestMode.ASK_KeyGenerate);
            Utils.SetBrowsableAttribute(this, nameof(TestData), TestMethod == CanTestMode.ASK_KeyGenerate || TestMethod == CanTestMode.CRC_16_CCITT_0xFFFF_Calculate);

            //Utils.SetBrowsableAttribute(this, nameof(Temp), TestMethod == CanTestMode.Read || TestMethod == CanTestMode.UDS_ReadDataByIdentifier);
        }

        protected override void UpdateToleranceAttributes()
        {
            // 검사하려는 Signal이 있는 경우에만 Expected, Tolerance 를 보여준다.
            var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out _);
            bool isInspect = (TestMethod == CanTestMode.Read || TestMethod == CanTestMode.UDS_ReadDataByIdentifier) && (signal != null);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusPercent), isInspect && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusPercent), isInspect && Tolerance == ToleranceMode.RelativePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusPercent), isInspect && Tolerance == ToleranceMode.Relative);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusAbsolute), isInspect && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(ToleranceMinusAbsolute), isInspect && Tolerance == ToleranceMode.AbsolutePlusMinus);
            Utils.SetBrowsableAttribute(this, nameof(TolerancePlusMinusAbsolute), isInspect && Tolerance == ToleranceMode.Absolute);
        }

        private void UpdateDisplayNames()
        {
            string unitText = Unit == PhysicalUnit.None ? "" : $" [{Unit.GetText()}]";
            string tolerancePlusDisplayName = DispNameTolPrefix + PlusSign + unitText;
            string toleranceMinusDisplayName = DispNameTolPrefix + MinusSign + unitText;
            string tolerancePlusMinusDisplayName = DispNameTolPrefix + PlusMinusSign + unitText;
            Utils.SetDisplayNameAttribute(this, nameof(TolerancePlusAbsolute), tolerancePlusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(ToleranceMinusAbsolute), toleranceMinusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(TolerancePlusMinusAbsolute), tolerancePlusMinusDisplayName);
            Utils.SetDisplayNameAttribute(this, nameof(ExpectedValue), nameof(ExpectedValue) + unitText);
        }

        public override object Clone()
        {
            var newStep = new EolCanStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolCanStep destStep)
            {
                destStep.TestMethod = TestMethod;
                destStep.LogEnabled = LogEnabled;
                destStep.Unit = Unit;
                destStep.ExpectedValue = ExpectedValue;
                destStep.Tolerance = Tolerance;
                destStep.TolerancePlusPercent = TolerancePlusPercent;
                destStep.ToleranceMinusPercent = ToleranceMinusPercent;
                destStep.TolerancePlusMinusPercent = TolerancePlusMinusPercent;
                destStep.TolerancePlusAbsolute = TolerancePlusAbsolute;
                destStep.ToleranceMinusAbsolute = ToleranceMinusAbsolute;
                destStep.TolerancePlusMinusAbsolute = TolerancePlusMinusAbsolute;
                destStep.MeasureOffset = MeasureOffset;
                destStep.NominalBaudRate = NominalBaudRate;
                destStep.DataBaudRate = DataBaudRate;
                destStep.UseMessageName = UseMessageName;
                destStep.MessageName = MessageName;
                destStep.MessageID = MessageID;
                destStep.DLC = DLC;
                destStep.Cycle = Cycle;
                destStep.MaxReadTime = MaxReadTime;
                destStep.SignalName = SignalName;
                destStep.AdditionalSignalName = AdditionalSignalName;
                destStep.SameSignalValueCount = SameSignalValueCount;
                destStep.SignalValueMinToFinish = SignalValueMinToFinish;
                destStep.SignalValueMaxToFinish = SignalValueMaxToFinish;
                destStep.ExtendedFrame = ExtendedFrame;
                destStep.RemoteRequest = RemoteRequest;
                destStep.CAN_FD = CAN_FD;
                destStep.BitRateSwitch = BitRateSwitch;
                if (MessageData != null)
                {
                    destStep.MessageData = new byte[MessageData.Length];
                    MessageData.CopyTo(destStep.MessageData, 0);
                }
                if (destStep.Signals != Signals)
                {
                    destStep.Signals = new List<CanSignal>();
                    destStep.Signals.AddRange(Signals);
                }
                destStep.CAN_TP = CAN_TP;
                destStep.UDS_ECU_Address = UDS_ECU_Address;
                destStep.UDS_DID = UDS_DID;
                if (UDS_WriteData != null)
                {
                    destStep.UDS_WriteData = new byte[UDS_WriteData.Length];
                    UDS_WriteData.CopyTo(destStep.UDS_WriteData, 0);
                }
                if (UDS_Response != null)
                {
                    destStep.UDS_Response = new byte[UDS_Response.Length];
                    UDS_Response.CopyTo(destStep.UDS_Response, 0);
                }
                destStep.UDS_ResponseASCII = UDS_ResponseASCII;
                destStep.UDS_RDBI_Use_MES_ROM_ID = UDS_RDBI_Use_MES_ROM_ID;
                destStep.UDS_SessionControlType = UDS_SessionControlType;
                destStep.UDS_RoutineControl = UDS_RoutineControl;
                destStep.UDS_RoutineID = UDS_RoutineID;
                if (UDS_RoutineControlOptions != null)
                {
                    destStep.UDS_RoutineControlOptions = new byte[UDS_RoutineControlOptions.Length];
                    UDS_RoutineControlOptions.CopyTo(destStep.UDS_RoutineControlOptions, 0);
                }
                if (UDS_InputOutputControlOptions != null)
                {
                    destStep.UDS_InputOutputControlOptions = new byte[UDS_InputOutputControlOptions.Length];
                    UDS_InputOutputControlOptions.CopyTo(destStep.UDS_InputOutputControlOptions, 0);
                }
                destStep.UDS_ParameterBlockSize = UDS_ParameterBlockSize;
                destStep.UDS_ParameterSeparationTime = UDS_ParameterSeparationTime;
                if (TestData != null)
                {
                    destStep.TestData = new byte[TestData.Length];
                    TestData.CopyTo(destStep.TestData, 0);
                }
                destStep.ASK_Type = ASK_Type;
                destStep.Temp = Temp;
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
