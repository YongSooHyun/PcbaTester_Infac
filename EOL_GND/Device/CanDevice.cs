using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.Model.ComponentModel;
using Peak.Can.Basic;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// Arbitration 비트 속도.
    /// </summary>
    [TypeConverter(typeof(DescEnumConverter))]
    public enum CanNominalBaudRate : ushort
    {
        [Description("5 Kbit/s")]
        BAUD_5K = TPCANBaudrate.PCAN_BAUD_5K,

        [Description("10 Kbit/s")]
        BAUD_10K = TPCANBaudrate.PCAN_BAUD_10K,

        [Description("20 Kbit/s")]
        BAUD_20K = TPCANBaudrate.PCAN_BAUD_20K,

        [Description("33.333 Kbit/s")]
        BAUD_33K = TPCANBaudrate.PCAN_BAUD_33K,

        [Description("47.619 Kbit/s")]
        BAUD_47K = TPCANBaudrate.PCAN_BAUD_47K,

        [Description("50 Kbit/s")]
        BAUD_50K = TPCANBaudrate.PCAN_BAUD_50K,

        [Description("83.333 Kbit/s")]
        BAUD_83K = TPCANBaudrate.PCAN_BAUD_83K,

        [Description("95.238 Kbit/s")]
        BAUD_95K = TPCANBaudrate.PCAN_BAUD_95K,

        [Description("100 Kbit/s")]
        BAUD_100K = TPCANBaudrate.PCAN_BAUD_100K,

        [Description("125 Kbit/s")]
        BAUD_125K = TPCANBaudrate.PCAN_BAUD_125K,

        [Description("250 Kbit/s")]
        BAUD_250K = TPCANBaudrate.PCAN_BAUD_250K,

        [Description("500 Kbit/s")]
        BAUD_500K = TPCANBaudrate.PCAN_BAUD_500K,

        [Description("800 Kbit/s")]
        BAUD_800K = TPCANBaudrate.PCAN_BAUD_800K,

        [Description("1 Mbit/s")]
        BAUD_1M = TPCANBaudrate.PCAN_BAUD_1M,
    }

    public static class CanNominalBaudRateExtensions
    {
        public static string GetText(this CanNominalBaudRate baudRate)
        {
            var enumType = typeof(CanNominalBaudRate);
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, baudRate));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (da != null)
                return da.Description;
            else
                return baudRate.ToString();
        }
    }

    /// <summary>
    /// CAN FD에서 사용될 수 있는 데이터 전송 속도 리스트.
    /// </summary>
    [TypeConverter(typeof(DescEnumConverter))]
    public enum CanDataBaudRate
    {
        [Description("500 Kbit/s")]
        BAUD_500K,

        [Description("1 Mbit/s")]
        BAUD_1M,

        [Description("2 Mbit/s")]
        BAUD_2M,

        [Description("4 Mbit/s")]
        BAUD_4M,

        [Description("5 Mbit/s")]
        BAUD_5M,

        [Description("8 Mbit/s")]
        BAUD_8M,

        [Description("10 Mbit/s")]
        BAUD_10M,
    }

    public static class CanDataBaudRateExtensions
    {
        public static string GetText(this CanDataBaudRate baudRate)
        {
            var enumType = typeof(CanNominalBaudRate);
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, baudRate));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (da != null)
                return da.Description;
            else
                return baudRate.ToString();
        }
    }

    /// <summary>
    /// CAN 메시지 타입.
    /// </summary>
    [Flags]
    public enum CanMessageTypes : byte
    {
        /// <summary>
        /// CAN Standard Frame (11-bit identifier)
        /// </summary>
        Standard = 0x00,

        /// <summary>
        /// CAN RTR(Remote-Transfer-Request) Frame
        /// </summary>
        RTR = 0x01,

        /// <summary>
        /// CAN Extended Frame (29-bit identifier)
        /// </summary>
        Extended = 0x02,

        /// <summary>
        /// FD frame in terms of CiA Specs
        /// </summary>
        FD = 0x04,

        /// <summary>
        /// FD BRS(Bit Rate Switch) (CAN data at a higher bit rate)
        /// </summary>
        BRS = 0x08,

        /// <summary>
        /// FD ESI(Error State Indicator) (CAN FD transmitter was error active)
        /// </summary>
        ESI = 0x10,

        /// <summary>
        /// Echo CAN Frame
        /// </summary>
        Echo = 0x20,

        /// <summary>
        /// Error frame
        /// </summary>
        ErrorFrame = 0x40,

        /// <summary>
        /// PCAN status message
        /// </summary>
        PcanStatus = 0x80,
    }

    [TypeConverter(typeof(DescEnumConverter))]
    public enum CanDLC : byte
    {
        [Description("0")]
        DLC_0 = 0,

        [Description("1")]
        DLC_1 = 1,

        [Description("2")]
        DLC_2 = 2,

        [Description("3")]
        DLC_3 = 3,

        [Description("4")]
        DLC_4 = 4,

        [Description("5")]
        DLC_5 = 5,

        [Description("6")]
        DLC_6 = 6,

        [Description("7")]
        DLC_7 = 7,

        [Description("8")]
        DLC_8 = 8,

        [Description("9(12)")]
        DLC_9 = 9,

        [Description("10(16)")]
        DLC_10 = 10,

        [Description("11(20)")]
        DLC_11 = 11,

        [Description("12(24)")]
        DLC_12 = 12,

        [Description("13(32)")]
        DLC_13 = 13,

        [Description("14(48)")]
        DLC_14 = 14,

        [Description("15(64)")]
        DLC_15 = 15,
    }

    public static class CanDLCExtensions
    {
        public static string GetText(this CanDLC dlc)
        {
            var enumType = typeof(CanDLC);
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, dlc));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (da != null)
                return da.Description;
            else
                return dlc.ToString();
        }
    }

    /// <summary>
    /// CAN 메시지.
    /// </summary>
    public class CanMessage
    {
        /// <summary>
        /// 11/29-bit message identifier
        /// </summary>
        public uint ID { get; set; }

        /// <summary>
        /// Type of the message
        /// </summary>
        public CanMessageTypes MessageType { get; set; }

        /// <summary>
        /// Data Length Code of the message (0..15)
        /// </summary>
        public byte DLC { get; set; }

        /// <summary>
        /// Data of the message (DATA[0]..DATA[63])
        /// </summary>
        public byte[] Data { get; set; }

        /// <summary>
        /// 수신한 메시지의 timestamp, 마이크로초.
        /// </summary>
        public ulong Timestamp { get; set; }

        /// <summary>
        /// 메시지를 전송할 때의 주기, ms.
        /// </summary>
        public uint Cycle { get; set; }

        /// <summary>
        /// 메시지 전송 시 포함할 시그널 리스트.
        /// </summary>
        public List<CanSignal> SendingSignals { get; set; }

        /// <summary>
        /// Convert a CAN DLC value into the actual data length of the CAN/CAN-FD frame.
        /// </summary>
        /// <param name="dlc">A value between 0 and 15 (CAN and FD DLC range)</param>
        /// <param name="isStandard">A value indicating if the msg is a standard CAN (FD Flag not checked)</param>
        /// <returns>The length represented by the DLC</returns>
        public static int GetLengthFromDLC(byte dlc, bool isStandard)
        {
            if (dlc <= 8)
                return dlc;

            if (isStandard)
                return 8;

            switch (dlc)
            {
                case 9: return 12;
                case 10: return 16;
                case 11: return 20;
                case 12: return 24;
                case 13: return 32;
                case 14: return 48;
                case 15: return 64;
                default: return dlc;
            }
        }
    }

    [TypeConverter(typeof(DescEnumConverter))]
    public enum UDS_SessionType : byte
    {
        /// <summary>
        /// Default Session
        /// </summary>
        [Description(nameof(DefaultSession) + " (0x01)")]
        DefaultSession = 0x01,

        /// <summary>
        /// ECU Programming Session
        /// </summary>
        [Description(nameof(ProgrammingSession) + " (0x02)")]
        ProgrammingSession = 0x02,

        /// <summary>
        /// ECU Extended Diagnostic Session
        /// </summary>
        [Description(nameof(ExtendedDiagnosticSession) + " (0x03)")]
        ExtendedDiagnosticSession = 0x03,

        /// <summary>
        /// Safety System Diagnostic Session
        /// </summary>
        [Description(nameof(SafetySystemDiagnosticSession) + " (0x04)")]
        SafetySystemDiagnosticSession = 0x04,
    }

    public enum UDS_SecurityAccessType : byte
    {
        RequestSeed_0x11 = 0x11,
        SendKey_0x12 = 0x12,
    }

    [TypeConverter(typeof(DescEnumConverter))]
    public enum UDS_RoutineControlType : byte
    {
        /// <summary>
        /// Start Routine. With the start-message, a service can be initiated. 
        /// It can be defined to confirm the beginning of the execution or to notify when the service is completed.
        /// </summary>
        [Description(nameof(StartRoutine) + " (0x01)")]
        StartRoutine = 0x01,

        /// <summary>
        /// Stop Routine. With the Stop message, a running service can be interrupted at any time.
        /// </summary>
        [Description(nameof(StopRoutine) + " (0x02)")]
        StopRoutine = 0x02,

        /// <summary>
        /// Request Routine Results. The third option is a message to query the results of the service.
        /// </summary>
        [Description(nameof(RequestRoutineResults) + " (0x03)")]
        RequestRoutineResults = 0x03
    }

    /// <summary>
    /// ISO-15765-4 address definitions
    /// </summary>
    [TypeConverter(typeof(DescEnumConverter))]
    public enum UDS_ECU_Address : ushort
    {
        /// <summary>
        /// ECU 1, Request: CAN ID 0x7E8, Response: CAN ID 0x7E0
        /// </summary>
        [Description(nameof(ECU_1) + " (0x7E0 <-> 0x7E8)")]
        ECU_1 = 0x01,

        /// <summary>
        /// ECU 2, Request: CAN ID 0x7E9, Response: CAN ID 0x7E1
        /// </summary>
        [Description(nameof(ECU_2) + " (0x7E1 <-> 0x7E9)")]
        ECU_2 = 0x02,

        /// <summary>
        /// ECU 3, Request: CAN ID 0x7EA, Response: CAN ID 0x7E2
        /// </summary>
        [Description(nameof(ECU_3) + " (0x7E2 <-> 0x7EA)")]
        ECU_3 = 0x03,

        /// <summary>
        /// ECU 4, Request: CAN ID 0x7EB, Response: CAN ID 0x7E3
        /// </summary>
        [Description(nameof(ECU_4) + " (0x7E3 <-> 0x7EB)")]
        ECU_4 = 0x04,

        /// <summary>
        /// ECU 5, Request: CAN ID 0x7EC, Response: CAN ID 0x7E4
        /// </summary>
        [Description(nameof(ECU_5) + " (0x7E4 <-> 0x7EC)")]
        ECU_5 = 0x05,

        /// <summary>
        /// ECU 6, Request: CAN ID 0x7ED, Response: CAN ID 0x7E5
        /// </summary>
        [Description(nameof(ECU_6) + " (0x7E5 <-> 0x7ED)")]
        ECU_6 = 0x06,

        /// <summary>
        /// ECU 7, Request: CAN ID 0x7EE, Response: CAN ID 0x7E6
        /// </summary>
        [Description(nameof(ECU_7) + " (0x7E6 <-> 0x7EE)")]
        ECU_7 = 0x07,

        /// <summary>
        /// ECU 8, Request: CAN ID 0x7EF, Response: CAN ID 0x7E7
        /// </summary>
        [Description(nameof(ECU_8) + " (0x7E7 <-> 0x7EF)")]
        ECU_8 = 0x08
    }

    [Flags]
    public enum CanLogModes
    {
        RxMessage = 1,
        TxMessage = 2,
    }

    /// <summary>
    /// CAN 디바이스.
    /// </summary>
    public abstract class CanDevice : TestDevice
    {
        /// <summary>
        /// 지정한 이름을 가진 CAN 디바이스를 리턴한다.
        /// 해당 이름을 가진 CAN 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static CanDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.CAN, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is CanDevice canDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return canDevice;
            }

            CanDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.PeakCAN:
                default:
                    device = new PeakCanDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected CanDevice(DeviceType deviceType, string name) : base(deviceType, name)
        {
        }

        /// <summary>
        /// CAN 로그 관련 설정을 진행한다.
        /// </summary>
        /// <param name="logMode"></param>
        /// <param name="directory"></param>
        public abstract void SetLogMode(CanLogModes? logMode, string directory);

        /// <summary>
        /// CAN 로깅을 시작하거나 중지한다.
        /// </summary>
        /// <param name="enabled"></param>
        public abstract void SetLogEnabled(bool enabled);

        /// <summary>
        /// CAN 채널 오픈.
        /// </summary>
        /// <param name="nominalBaudRate"></param>
        /// <param name="dataBaudRate"></param>
        public abstract void Open(CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate);

        /// <summary>
        /// CAN 채널 close.
        /// </summary>
        public abstract void Close();

        /// <summary>
        /// CAN 채널 송/수신 큐 클리어.
        /// </summary>
        public abstract void ResetQueues();

        /// <summary>
        /// CAN 메시지를 한번만 전송한다.
        /// </summary>
        /// <param name="message"></param>
        public abstract void Send(CanMessage message, bool canTp);

        /// <summary>
        /// CAN 메시지를 주기적으로 전송한다. Cycle == 0 이면 <see cref="Send(CanMessage, bool)"/>와 같다.
        /// </summary>
        /// <param name="message"></param>
        public abstract void SendPeriodic(CanMessage message, bool canTp);

        /// <summary>
        /// CAN 메시지의 주기적 전송을 중지한다.
        /// </summary>
        /// <param name="canId">중지하려는 CAN ID. null이면 모든 주기적 전송을 중지한다.</param>
        public abstract void StopPeriodic(uint? canId, bool remove);

        /// <summary>
        /// CAN 메시지를 읽는다.
        /// </summary>
        /// <returns></returns>
        public abstract List<CanMessage> ReadAll();

        /// <summary>
        /// CAN UDS 통신 개시.
        /// </summary>
        /// <param name="nominalBaudRate"></param>
        /// <param name="dataBaudRate"></param>
        public abstract void UDS_Open(CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate);

        /// <summary>
        /// CAN UDS 통신 종료.
        /// </summary>
        public abstract void UDS_Close();

        public abstract void UDS_ReadDataByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort did, out byte[] responseData, 
            StringBuilder commLog, CancellationToken token);
        public abstract void UDS_WriteDataByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort did, byte[] writingData, out byte[] responseData,
            StringBuilder commLog, CancellationToken token);
        public abstract void UDS_DiagnosticSessionControl(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_SessionType sessionType, 
            out byte[] responseData, StringBuilder commLog, CancellationToken token);
        public abstract void UDS_SecurityAccess(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_SecurityAccessType accessType, byte[] securityData, 
            out byte[] responseData, StringBuilder commLog, CancellationToken token);
        public abstract void UDS_RoutineControl(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_RoutineControlType controlType, ushort routineId, 
            byte[] controlOption, out byte[] responseData, StringBuilder commLog, CancellationToken token);
        public abstract void UDS_InputOutputControlByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort dataIdentifier,
            byte[] controlOption, out byte[] responseData, StringBuilder commLog, CancellationToken token);
        public abstract void UDS_SetValue(uint parameter, uint value);

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            // Do nothing.
            return null;
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            // Do nothing.
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            // Do nothing.
            return null;
        }
    }
}
