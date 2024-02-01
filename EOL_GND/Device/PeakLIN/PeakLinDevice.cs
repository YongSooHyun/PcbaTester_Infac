using DbcParserLib.Model;
using EOL_GND.Common;
using EOL_GND.Model;
using Peak.Can.IsoTp;
using Peak.Lin;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Xml.Linq;
using static BrightIdeasSoftware.TreeListView;

namespace EOL_GND.Device
{
    /// <summary>
    /// LIN hardware types.
    /// </summary>
    public enum PeakLinHardwareType : byte
    {
        /// <summary>
        /// PCAN-USB Pro LIN type
        /// </summary>
        PCAN_USB_Pro = PLinApi.LIN_HW_TYPE_USB_PRO,

        /// <summary>
        /// PCAN-USB Pro FD LIN
        /// </summary>
        PCAN_USB_Pro_FD = PLinApi.LIN_HW_TYPE_USB_PRO_FD,

        /// <summary>
        /// PLIN-USB
        /// </summary>		
        PLIN_USB = PLinApi.LIN_HW_TYPE_PLIN_USB,
    }

    /// <summary>
    /// Hardware operation modes.
    /// </summary>
    public enum PeakLinHardwareMode : byte
    {
        /// <summary>
        /// Hardware is not initialized
        /// </summary>
        None = TLINHardwareMode.modNone,

        /// <summary>
        /// Hardware working as Slave
        /// </summary>
        Slave = TLINHardwareMode.modSlave,

        /// <summary>
        /// Hardware working as Master
        /// </summary>
        Master = TLINHardwareMode.modMaster,
    }

    /// <summary>
    /// Received Message Types
    /// </summary>
    public enum LinMessageType : byte
    {
        /// <summary>
        /// Standard LIN Message
        /// </summary>
        Standard = TLINMsgType.mstStandard,

        /// <summary>
        /// Bus Sleep status message
        /// </summary>
        BusSleep = TLINMsgType.mstBusSleep,

        /// <summary>
        /// Bus WakeUp status message
        /// </summary>
        BusWakeUp = TLINMsgType.mstBusWakeUp,

        /// <summary>
        /// Auto-baudrate Timeout status message
        /// </summary>
        AutobaudrateTimeOut = TLINMsgType.mstAutobaudrateTimeOut,

        /// <summary>
        /// Auto-baudrate Reply status message
        /// </summary>
        AutobaudrateReply = TLINMsgType.mstAutobaudrateReply,

        /// <summary>
        /// Bus Overrun status message
        /// </summary>
        Overrun = TLINMsgType.mstOverrun,

        /// <summary>
        /// Queue Overrun status message
        /// </summary>
        QueueOverrun = TLINMsgType.mstQueueOverrun,

        /// <summary>
        /// Client's receive queue overrun status message
        /// </summary>
        ClientQueueOverrun = TLINMsgType.mstClientQueueOverrun,
    }

    /// <summary>
    /// Message Direction Types
    /// </summary>
    public enum LinDirection : byte
    {
        /// <summary>
        /// Direction disabled
        /// </summary>
        Disabled = TLINDirection.dirDisabled,

        /// <summary>
        /// Direction is Publisher
        /// </summary>
        Publisher = TLINDirection.dirPublisher,

        /// <summary>
        /// Direction is Subscriber
        /// </summary>
        Subscriber = TLINDirection.dirSubscriber,

        /// <summary>
        /// Direction is Subscriber (detect Length)
        /// </summary>
        SubscriberAutoLength = TLINDirection.dirSubscriberAutoLength,
    }

    /// <summary>
    /// LIN Message checksum Types
    /// </summary>
    public enum LinChecksumType : byte
    {
        /// <summary>
        /// Custom checksum
        /// </summary>
        Custom = TLINChecksumType.cstCustom,

        /// <summary>
        /// Classic checksum (ver 1.x)
        /// </summary>
        Classic = TLINChecksumType.cstClassic,

        /// <summary>
        /// Enhanced checksum
        /// </summary>
        Enhanced = TLINChecksumType.cstEnhanced,

        /// <summary>
        /// Detect checksum
        /// </summary>
        Auto = TLINChecksumType.cstAuto,
    }
    /// <summary>
    /// Error flags for LIN Rcv Msgs
    /// </summary>
    [Flags]
    public enum LinMessageErrors : int
    {
        /// <summary>
        /// Error on Synchronization field
        /// </summary>
        InconsistentSynch = TLINMsgErrors.InconsistentSynch,

        /// <summary>
        /// Wrong parity Bit 0
        /// </summary>
        IdParityBit0 = TLINMsgErrors.IdParityBit0,

        /// <summary>
        /// Wrong parity Bit 1
        /// </summary>
        IdParityBit1 = TLINMsgErrors.IdParityBit1,

        /// <summary>
        /// Slave not responding error
        /// </summary>
        SlaveNotResponding = TLINMsgErrors.SlaveNotResponding,

        /// <summary>
        /// A timeout was reached
        /// </summary>
        Timeout = TLINMsgErrors.Timeout,

        /// <summary>
        /// Wrong checksum
        /// </summary>
        Checksum = TLINMsgErrors.Checksum,

        /// <summary>
        /// Bus shorted to ground
        /// </summary>
        GroundShort = TLINMsgErrors.GroundShort,

        /// <summary>
        /// Bus shorted to Vbat
        /// </summary>
        VBatShort = TLINMsgErrors.VBatShort,

        /// <summary>
        /// A slot time (delay) was too small
        /// </summary>
        SlotDelay = TLINMsgErrors.SlotDelay,

        /// <summary>
        /// Response was received from other station
        /// </summary>
        OtherResponse = TLINMsgErrors.OtherResponse,
    }

    /// <summary>
    ///  LIN Message
    /// </summary>
    public struct LinMessage
    {
        /// <summary>
        /// Frame type (see Received Message Types)
        /// </summary>
        public LinMessageType Type;

        /// <summary>
        /// Frame ID (6 bit) + Parity (2 bit).
        /// </summary>
        public byte FrameId;

        /// <summary>
        /// Frame Length (1..8)
        /// </summary>
        public byte Length;

        /// <summary>
        /// Frame Direction (see Message Direction Types)
        /// </summary>
        public LinDirection Direction;

        /// <summary>
        /// Frame Checksum type (see Message Checksum Types)
        /// </summary>
        public LinChecksumType ChecksumType;

        /// <summary>
        /// Data bytes (0..7)
        /// </summary>
        public byte[] Data;

        /// <summary>
        /// Frame Checksum
        /// </summary>
        public byte Checksum;

        /// <summary>
        /// Frame error flags (see Error flags for LIN Rcv Msgs)
        /// </summary>
        public LinMessageErrors ErrorFlags;

        /// <summary>
        /// Timestamp in microseconds
        /// </summary>
        public ulong TimeStamp;

        /// <summary>
        /// Handle of the Hardware which received the message
        /// </summary>
        public ushort HardwareHandle;
    }

    /// <summary>
    /// LIN Frame Entry Flags.
    /// </summary>
    [Flags]
    public enum LinFrameFlags : ushort
    {
        None = 0,

        /// <summary>
        /// Enables/disables in a frame with direction dirPublisher, the sending of the Response-Field.
        /// </summary>
        ResponseEnable = 1,

        /// <summary>
        /// Slave only. Configures a frame with direction dirPublisher to send the Response-Field only when the data was actualized. 
        /// A single shot response will be retransmitted until transmission was successful. 
        /// The internal update flag will be cleared when the transmission was error-free.Otherwise pending responses could be lost.
        /// </summary>
        SingleShot = 2,

        /// <summary>
        /// Suppress the Initial Data associated with the Frame Entry.
        /// </summary>
        IgnoreInitialData = 4,
    }

    /// <summary>
    /// A LIN Frame Entry 
    /// </summary>
    public struct LinFrameEntry
    {
        /// <summary>
        /// Frame ID (without parity)
        /// </summary>
        public byte FrameId;

        /// <summary>
        /// Frame Length (1..8)
        /// </summary>
        public byte Length;

        /// <summary>
        /// Frame Direction (see Message Direction Types)
        /// </summary>
        public LinDirection Direction;

        /// <summary>
        /// Frame Checksum type (see Message Checksum Types)
        /// </summary>
        public LinChecksumType ChecksumType;

        /// <summary>
        /// Frame flags (see Frame flags for LIN Msgs)
        /// </summary>
        public LinFrameFlags Flags;

        /// <summary>
        /// Data bytes (0..7)
        /// </summary>
        public byte[] InitialData;
    }

    public class PeakLinDevice : LinDevice
    {
        /// <summary>
        /// Invalid handle number.
        /// </summary>
        public const byte InvalidHandle = PLinApi.INVALID_LIN_HANDLE;

        // 내부적으로 클라이언트 등록에 이용하는 이름.
        private const string ClientName = "SmartDevPeakLINClient";

        // LIN 클라이언트 핸들.
        private byte clientHandle = InvalidHandle;

        // LIN 하드웨어 핸들.
        private ushort hardwareHandle = InvalidHandle;

        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        // 에러이면 그를 설명하는 메시지를 얻어 예외를 던진다.
        private static void ThrowIfError(TLINError error)
        {
            if (error != TLINError.errOK)
            {
                var errorMessage = new StringBuilder(255);
                PLinApi.GetErrorText(error, 0, errorMessage, errorMessage.Capacity);
                throw new Exception($"Peak LIN Error: {errorMessage}");
            }
        }

        /// <summary>
        /// 지정한 하드웨어 타입과 디바이스 번호, 채널번호를 가진 하드웨어 핸들을 리턴한다.
        /// </summary>
        /// <param name="hardwareType"></param>
        /// <param name="deviceNumber"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ushort GetHardwareHandle(PeakLinHardwareType hardwareType, int deviceNumber, int channel)
        {
            // 연결된 모든 LIN 디바이스 얻기.
            var hwHandles = new ushort[16];
            var linError = PLinApi.GetAvailableHardware(hwHandles, (ushort)(hwHandles.Length * sizeof(ushort)), out ushort hwCount);
            ThrowIfError(linError);

            // LIN 디바이스들 중 조건에 맞는 디바이스가 있는가 검사.
            for (int i = 0; i < hwCount; i++)
            {
                var hwHandle = hwHandles[i];

                // 하드웨어 타입 얻기.
                PLinApi.GetHardwareParam(hwHandle, TLINHardwareParam.hwpType, out int hwType, 0);

                // 디바이스 번호 읽기.
                PLinApi.GetHardwareParam(hwHandle, TLINHardwareParam.hwpDeviceNumber, out int devNo, 0);

                // 채널 번호 읽기.
                PLinApi.GetHardwareParam(hwHandle, TLINHardwareParam.hwpChannelNumber, out int channelNo, 0);

                // 조건에 맞는가 비교.
                if (hwType == (int)hardwareType && devNo == deviceNumber && channelNo == channel)
                {
                    return hwHandle;
                }
            }

            // 조건에 맞는 하드웨어 없음.
            return InvalidHandle;
        }

        public PeakLinDevice(string name) : base(DeviceType.PeakLIN, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
        }

        public override void Disconnect()
        {
        }

        public override void Open(bool masterMode, ushort baudrate)
        {
            if (clientHandle != InvalidHandle || hardwareHandle != InvalidHandle)
            {
                return;
            }

            byte clntHandle = InvalidHandle;
            ushort hwHandle = InvalidHandle;
            try
            {
                // 설정에 따른 하드웨어 핸들 얻기.
                var linSetting = Setting as LinDeviceSetting;
                hwHandle = GetHardwareHandle(linSetting.HardwareType, linSetting.DeviceNumber, linSetting.Channel);
                if (hwHandle == InvalidHandle)
                {
                    throw new Exception($"LIN 디바이스({linSetting.HardwareType}, Device={linSetting.DeviceNumber}, Channel={linSetting.Channel})가 연결되지 않았습니다.");
                }

                // 클라이언트 등록.
                var linError = PLinApi.RegisterClient(ClientName, IntPtr.Zero, out clntHandle);
                ThrowIfError(linError);

                // 하드웨어 연결.
                Logger.LogDebug($"Connecting to {linSetting.DeviceName}(client={clntHandle}, hardware={hwHandle})");
                linError = PLinApi.ConnectClient(clntHandle, hwHandle);
                ThrowIfError(linError);

                // 마스터/슬레이브 모드, 속도 설정.
                linError = PLinApi.InitializeHardware(clntHandle, hwHandle, masterMode ? TLINHardwareMode.modMaster : TLINHardwareMode.modSlave, baudrate);
                ThrowIfError(linError);

                // 초기에는 모든 메시지 받도록 필터 적용.
                //linError = PLinApi.SetClientFilter(clntHandle, hwHandle, 0xFFFF_FFFF_FFFF_FFFFUL);
                linError = PLinApi.RegisterFrameId(clntHandle, hwHandle, 0, 63);
                ThrowIfError(linError);

                clientHandle = clntHandle;
                hardwareHandle = hwHandle;
            }
            catch
            {
                PLinApi.DisconnectClient(clntHandle, hwHandle);
                PLinApi.RemoveClient(clntHandle);
                throw;
            }
        }

        public override void Close()
        {
            PLinApi.ResetHardwareConfig(clientHandle, hardwareHandle);
            PLinApi.DisconnectClient(clientHandle, hardwareHandle);
            PLinApi.RemoveClient(clientHandle);
            clientHandle = InvalidHandle;
            hardwareHandle = InvalidHandle;
        }

        public override void SetReceiveFilter(ulong receiveMask)
        {
            var linError = PLinApi.SetClientFilter(clientHandle, hardwareHandle, receiveMask);
            ThrowIfError(linError);
        }

        public override void SetFrameEntry(LinFrameEntry frameEntry)
        {
            var entry = new TLINFrameEntry();
            entry.FrameId = frameEntry.FrameId;
            entry.Length = frameEntry.Length;
            entry.Direction = (TLINDirection)frameEntry.Direction;
            entry.ChecksumType = (TLINChecksumType)frameEntry.ChecksumType;
            entry.Flags = (ushort)frameEntry.Flags;
            if (frameEntry.InitialData != null)
            {
                entry.InitialData = new byte[frameEntry.InitialData.Length];
                frameEntry.InitialData.CopyTo(entry.InitialData, 0);
            }

            var linError = PLinApi.SetFrameEntry(clientHandle, hardwareHandle, ref entry);
            ThrowIfError(linError);
        }

        public override bool Read(out LinMessage message)
        {
            TLINRcvMsg msg;
            while (true)
            {
                var linError = PLinApi.Read(clientHandle, out msg);
                if (linError == TLINError.errRcvQueueEmpty)
                {
                    message = default;
                    return false;
                }

                ThrowIfError(linError);

                if (msg.Type == TLINMsgType.mstStandard)
                {
                    break;
                }
            }

            message = new LinMessage();
            message.Type = (LinMessageType)msg.Type;
            message.FrameId = msg.FrameId;
            message.Length = msg.Length;
            message.Direction = (LinDirection)msg.Direction;
            message.ChecksumType = (LinChecksumType)msg.ChecksumType;
            if (msg.Data != null)
            {
                message.Data = new byte[msg.Data.Length];
                msg.Data.CopyTo(message.Data, 0);
            }
            message.Checksum = msg.Checksum;
            message.ErrorFlags = (LinMessageErrors)msg.ErrorFlags;
            message.TimeStamp = msg.TimeStamp;
            message.HardwareHandle = msg.hHw;

            return true;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    Close();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
