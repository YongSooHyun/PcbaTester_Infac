using EOL_GND.Common;
using EOL_GND.Model;
using Peak.Can.Basic;
using Peak.Can.IsoTp;
using Peak.Can.Uds;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public enum PeakCanDeviceType
    {
        PCI,        // Up to 16 channels
        USB,        // Up to 16 channels
        LAN,        // Up to 16 channels
    }

    /// <summary>
    /// PEAK-System Technik GmbH사의 CAN장치 기능을 제공한다.
    /// </summary>
    public class PeakCanDevice : CanDevice
    {
        private class PeriodicMessageTask
        {
            internal PeakCanDevice Device { get; private set; }
            internal CanMessage Message { get; private set; }
            internal bool CanTp { get; private set; }
            internal Task Task { get; private set; }
            internal CancellationTokenSource CTSource { get; private set; }

            internal PeriodicMessageTask(PeakCanDevice device, CanMessage message, bool canTp)
            {
                if (device == null)
                {
                    throw new ArgumentNullException(nameof(device));
                }

                if (message == null)
                {
                    throw new ArgumentNullException(nameof(message));
                }

                Device = device;
                Message = message;
                CanTp = canTp;

                // 메시지 전송 태스크 만들기.
                CTSource = new CancellationTokenSource();
                Task = Task.Run(SendMessagePeriodic);
            }

            private void SendMessagePeriodic()
            {
                try
                {
                    // Alive Counter 초기화.
                    if (Message.SendingSignals != null)
                    {
                        foreach (var signal in Message.SendingSignals)
                        {
                            if (signal.ValueType == CanSignal.AutoValueType.AliveCounter)
                            {
                                signal.Value = 0;
                            }
                        }
                    }

                    while (true)
                    {
                        if (CTSource.IsCancellationRequested)
                        {
                            break;
                        }

                        Device.Send(Message, CanTp);

                        MultimediaTimer.Delay((int)Message.Cycle).Wait(CTSource.Token);
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError($"{Device.Setting.DeviceName} 0x{Message.ID:X} Periodic send error: {ex.Message}");
                    //throw;
                }
            }

            internal void Start()
            {
                if (CTSource.IsCancellationRequested)
                {
                    // 메시지 전송 태스크 만들기.
                    CTSource = new CancellationTokenSource();
                    Task = Task.Run(SendMessagePeriodic);
                }
            }
        }

        /// <summary>
        /// CAN 로그 파일 이름.
        /// </summary>
        public const string LogFileName = "PCANBasic.log";

        // CAN 메시지를 주기적으로 보내는 태스크 리스트.
        private List<PeriodicMessageTask> periodicMessageTasks = new List<PeriodicMessageTask>();

        // CAN 디바이스 핸들.
        private ushort handle = 0;

        // UDS 핸들.
        private cantp_handle udsHandle = 0;

        // Dispose 패턴을 위한 필드.
        private bool disposedValue = false;

        /// <summary>
        /// 디바이스 타입과 채널 번호로부터 핸들값을 얻는다.
        /// </summary>
        /// <param name="deviceType"></param>
        /// <param name="channel"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ushort GetHandle(PeakCanDeviceType deviceType, int channel)
        {
            switch (deviceType)
            {
                case PeakCanDeviceType.PCI:
                    if (channel <= 8)
                    {
                        return (ushort)(0x40 + channel);
                    }
                    else
                    {
                        return (ushort)(0x400 + channel);
                    }
                case PeakCanDeviceType.USB:
                    if (channel <= 8)
                    {
                        return (ushort)(0x50 + channel);
                    }
                    else
                    {
                        return (ushort)(0x500 + channel);
                    }
                case PeakCanDeviceType.LAN:
                    return (ushort)(0x800 + channel);
                default:
                    throw new Exception($"Unknown device type: {deviceType}");
            }
        }

        /// <summary>
        /// CAN FD 통신을 위해 clock = 80MHz일 때 중재비트속도 파라미터 계산.
        /// </summary>
        /// <param name="baudrate">Represents a PCAN bit rate register value.</param>
        /// <param name="brp">Clock prescaler for nominal time quantum (1..1024).</param>
        /// <param name="tseg1">TSEG1 segment for nominal bit rate in time quanta (1..256).</param>
        /// <param name="tseg2">TSEG2 segment for nominal bit rate in time quanta (1..128).</param>
        /// <param name="sjw">Synchronization Jump Width for nominal bit rate in time quanta (1..128).</param>
        /// <exception cref="Exception"></exception>
        private static void CalcNominalBitRateParameters(CanNominalBaudRate baudrate, out int brp, out int tseg1, out int tseg2, out int sjw)
        {
            switch (baudrate)
            {
                case CanNominalBaudRate.BAUD_5K:
                    brp = 640; tseg1 = 16; tseg2 = 8; sjw = 2;
                    break;
                case CanNominalBaudRate.BAUD_10K:
                    brp = 400; tseg1 = 16; tseg2 = 3; sjw = 2;
                    break;
                case CanNominalBaudRate.BAUD_20K:
                    brp = 200; tseg1 = 16; tseg2 = 3; sjw = 2;
                    break;
                case CanNominalBaudRate.BAUD_33K:
                    brp = 120; tseg1 = 16; tseg2 = 3; sjw = 3;
                    break;
                case CanNominalBaudRate.BAUD_47K:
                    brp = 210; tseg1 = 5; tseg2 = 2; sjw = 1;
                    break;
                case CanNominalBaudRate.BAUD_50K:
                    brp = 80; tseg1 = 16; tseg2 = 3; sjw = 2;
                    break;
                case CanNominalBaudRate.BAUD_83K:
                    brp = 60; tseg1 = 12; tseg2 = 3; sjw = 3;
                    break;
                case CanNominalBaudRate.BAUD_95K:
                    brp = 40; tseg1 = 15; tseg2 = 5; sjw = 4;
                    break;
                case CanNominalBaudRate.BAUD_100K:
                    brp = 40; tseg1 = 16; tseg2 = 3; sjw = 2;
                    break;
                case CanNominalBaudRate.BAUD_125K:
                    brp = 40; tseg1 = 13; tseg2 = 2; sjw = 1;
                    break;
                case CanNominalBaudRate.BAUD_250K:
                    brp = 20; tseg1 = 12; tseg2 = 3; sjw = 3;
                    break;
                case CanNominalBaudRate.BAUD_500K:
                    brp = 2; tseg1 = 63; tseg2 = 16; sjw = 16;
                    break;
                case CanNominalBaudRate.BAUD_800K:
                    brp = 10; tseg1 = 7; tseg2 = 2; sjw = 1;
                    break;
                case CanNominalBaudRate.BAUD_1M:
                    brp = 10; tseg1 = 5; tseg2 = 2; sjw = 2;
                    break;
                default:
                    throw new Exception($"Unknown nominal bit rate: {baudrate}");
            }
        }

        /// <summary>
        /// CAN FD 통신을 위해 clock = 80MHz일 때 데이터전송속도 파라미터 계산.
        /// </summary>
        /// <param name="baudrate">Represents a data bit rate register value.</param>
        /// <param name="brp">Clock prescaler for fast data time quantum (1..1024).</param>
        /// <param name="tseg1">TSEG1 segment for fast data bit rate in time quanta (1..32).</param>
        /// <param name="tseg2">TSEG2 segment for fast data bit rate in time quanta (1..16).</param>
        /// <param name="sjw">Synchronization Jump Width for fast data bit rate in time quanta (1..16).</param>
        /// <exception cref="Exception"></exception>
        private static void CalcDataBitRateParameters(CanDataBaudRate baudrate, out int brp, out int tseg1, out int tseg2, out int sjw)
        {
            switch (baudrate)
            {
                case CanDataBaudRate.BAUD_500K:
                    brp = 4; tseg1 = 29; tseg2 = 10; sjw = 10;
                    break;
                case CanDataBaudRate.BAUD_1M:
                    brp = 2; tseg1 = 29; tseg2 = 10; sjw = 10;
                    break;
                case CanDataBaudRate.BAUD_2M:
                    brp = 2; tseg1 = 15; tseg2 = 4; sjw = 4;
                    break;
                case CanDataBaudRate.BAUD_4M:
                    brp = 2; tseg1 = 7; tseg2 = 2; sjw = 1;
                    break;
                case CanDataBaudRate.BAUD_5M:
                    brp = 1; tseg1 = 11; tseg2 = 4; sjw = 4;
                    break;
                case CanDataBaudRate.BAUD_8M:
                    brp = 1; tseg1 = 7; tseg2 = 2; sjw = 1;
                    break;
                case CanDataBaudRate.BAUD_10M:
                    brp = 1; tseg1 = 5; tseg2 = 2; sjw = 1;
                    break;
                default:
                    throw new Exception($"Unknown data bit rate: {baudrate}");
            }
        }

        public static string CreateBitRateText(CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate)
        {
            // Bit rate 파라미터 문자열 생성.
            CalcNominalBitRateParameters(nominalBaudRate, out int nominalBrp, out int nominalTseg1, out int nominalTseg2, out int nominalSjw);
            CalcDataBitRateParameters(dataBaudRate, out int dataBrp, out int dataTseg1, out int dataTseg2, out int dataSjw);
            return $"f_clock_mhz=80, nom_brp={nominalBrp}, nom_tseg1={nominalTseg1}, nom_tseg2={nominalTseg2}, nom_sjw={nominalSjw}" +
                $", data_brp={dataBrp}, data_tseg1={dataTseg1}, data_tseg2={dataTseg2}, data_sjw={dataSjw}";
        }

        /// <summary>
        /// 시스템에 연결된 모든 채널들에 대한 정보를 얻는다.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static TPCANChannelInformation[] GetAttachedChannels()
        {
            // 시스템에 연결된 채널 개수 얻기.
            var result = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS_COUNT, out uint channelCount, sizeof(uint));
            ThrowIfError(result);

            // 채널 정보 얻기.
            var channelInfos = new TPCANChannelInformation[channelCount];
            result = PCANBasic.GetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_ATTACHED_CHANNELS, channelInfos);
            ThrowIfError(result);

            return channelInfos;
        }

        /// <summary>
        /// 해당 채널에 연결할 수 있는가를 리턴한다.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static bool GetChannelAvailable(TPCANChannelInformation channelInfo)
        {
            return (channelInfo.channel_condition & PCANBasic.PCAN_CHANNEL_AVAILABLE) == PCANBasic.PCAN_CHANNEL_AVAILABLE;
        }

        /// <summary>
        /// 해당 채널이 FD를 지원하는지 여부를 리턴한다.
        /// </summary>
        /// <param name="channelInfo"></param>
        /// <returns></returns>
        public static bool GetChannelFDCapable(TPCANChannelInformation channelInfo)
        {
            return (channelInfo.device_features & PCANBasic.FEATURE_FD_CAPABLE) == PCANBasic.FEATURE_FD_CAPABLE;
        }

        /// <summary>
        /// 1부터 시작하는 채널번호를 얻는다.
        /// </summary>
        /// <param name="handle"></param>
        /// <returns></returns>
        public static byte GetChannelIndex(ushort handle)
        {
            if (handle < 0x100)
            {
                return (byte)(handle & 0xF);
            }
            else
            {
                return (byte)(handle & 0xFF);
            }
        }

        // 에러 상태이면 관련 에러 메시지를 얻어 예외를 던진다.
        private static void ThrowIfError(TPCANStatus status)
        {
            if (status != TPCANStatus.PCAN_ERROR_OK)
            {
                var textBuffer = new StringBuilder(256);
                PCANBasic.GetErrorText(status, 0, textBuffer);
                throw new Exception($"CAN Error: {textBuffer}");
            }
        }

        // 에러 상태이면 관련 에러 메시지를 얻어 예외를 던진다.
        private static void TP_ThrowIfError(cantp_status status)
        {
            if (!CanTpApi.StatusIsOk_2016(status))
            {
                // Get error text.
                var textBuffer = new StringBuilder(256);
                CanTpApi.GetErrorText_2016(status, 0, textBuffer, (uint)textBuffer.Capacity);
                throw new Exception($"CAN TP Error: {textBuffer}");
            }
        }

        /// <summary>
        /// 일반 CAN 채널 초기화.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="baudrate"></param>
        /// <exception cref="Exception"></exception>
        public static void Initialize(ushort handle, CanNominalBaudRate baudrate)
        {
            var result = PCANBasic.Initialize(handle, (TPCANBaudrate)baudrate);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                if (result != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    ThrowIfError(result);
                }
                else
                {
                    // Indicates that the channel has been initialized but at a different bit rate as the given one.
                    Logger.LogWarning($"The channel({handle}) has been initialized but at a different bit rate as the given one.");
                }
            }
        }

        /// <summary>
        /// FD 가능한 채널 초기화.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="nominalBaudRate"></param>
        /// <param name="dataBaudRate"></param>
        /// <exception cref="Exception"></exception>
        public static void InitializeFD(ushort handle, CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate)
        {
            var result = PCANBasic.InitializeFD(handle, CreateBitRateText(nominalBaudRate, dataBaudRate));
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                if (result != TPCANStatus.PCAN_ERROR_CAUTION)
                {
                    ThrowIfError(result);
                }
                else
                {
                    // Indicates that the channel has been initialized but at a different bit rate as the given one.
                    Logger.LogWarning($"The channel({handle}) has been initialized but at a different bit rate as the given one.");
                }
            }
        }

        /// <summary>
        /// 채널 해제.
        /// </summary>
        /// <param name="handle"></param>
        public static void Uninitialize(ushort handle)
        {
            PCANBasic.Uninitialize(handle);
        }

        /// <summary>
        /// 송/수신 큐를 리셋한다.
        /// </summary>
        /// <param name="handle"></param>
        /// <exception cref="Exception"></exception>
        public static void ResetQueues(ushort handle)
        {
            var result = PCANBasic.Reset(handle);
            ThrowIfError(result);
        }

        private static void PackSignals(byte[] data, CanMessage message)
        {
            // 시그널 처리.
            var dataLength = CanMessage.GetLengthFromDLC(message.DLC, false);
            if (message.SendingSignals != null)
            {
                // 자동계산되는 시그널 값 계산.
                foreach (var signal in message.SendingSignals)
                {
                    switch (signal.ValueType)
                    {
                        case CanSignal.AutoValueType.CRC:
                            // The data area for CRC calculation is based on the data length (DLC) of the message DB.<br/>
                            // The CRC shall be calculated over the entire data block (excluding the CRC bytes) including the user data, alive counter and Data ID.<br/>
                            // - CRC Polynomial (0x1021) , Initial Value : 0xFFFF, XOR value : 0x0000, Data ID : Message ID + 0xF800<br/>
                            // - The initial value is for CRC calculation and the message should be sent with the calculated result value.<br/>
                            // - The mathematical expression to 16 bit CRC polynomial is given here: G(x) = x16 + x12 + x5 + 1
                            ushort dataId = (ushort)(message.ID + 0xF800UL);
                            var dataIdBytes = BitConverter.GetBytes(dataId);
                            if (!BitConverter.IsLittleEndian)
                            {
                                Array.Reverse(dataIdBytes);
                            }

                            // 이 신호의 바이트들은 제외.
                            int startByte = signal.SignalInfo.StartBit / 8;
                            int startBitInByte = signal.SignalInfo.StartBit % 8;
                            int byteLength = (startBitInByte + signal.SignalInfo.Length) / 8 + ((startBitInByte + signal.SignalInfo.Length) % 8 > 0 ? 1 : 0);
                            var dataBytes = data.Take(startByte).Concat(data.Skip(startByte + byteLength).Take(dataLength - startByte - byteLength));

                            var crcBytes = dataBytes.Concat(dataIdBytes);
                            var crcValue = NullFX.CRC.Crc16.ComputeChecksum(NullFX.CRC.Crc16Algorithm.CcittInitialValue0xFFFF, crcBytes.ToArray());
                            signal.Value = crcValue;

                            EolCanStep.CanSignalPack(data, 0, dataLength, new[] { signal });
                            break;
                        case CanSignal.AutoValueType.AliveCounter:
                        case CanSignal.AutoValueType.Manual:
                        default:
                            // 시그널 값 그대로 반영.
                            EolCanStep.CanSignalPack(data, 0, dataLength, new[] { signal });
                            if (signal.ValueType == CanSignal.AutoValueType.AliveCounter)
                            {
                                // 최대값에 도달하면 0으로 설정.
                                var counterMaxValue = (1UL << signal.SignalInfo.Length) - 1;
                                if (signal.Value >= counterMaxValue)
                                {
                                    signal.Value = 0;
                                }
                                else
                                {
                                    // Alive Counter인 경우 다음 전송을 위한 값을 1 증가.
                                    signal.Value += 1;
                                }
                            }
                            break;
                    }
                }
            }
        }

        /// <summary>
        /// CAN 메시지를 전송한다.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void Write(ushort handle, CanMessage message)
        {
            var msg = new TPCANMsg();
            msg.ID = message.ID;
            msg.MSGTYPE = (TPCANMessageType)message.MessageType;
            msg.LEN = message.DLC;
            msg.DATA = new byte[8];
            if (message.Data != null)
            {
                message.Data.CopyTo(msg.DATA, 0);
            }

            // 시그널 Packing.
            PackSignals(msg.DATA, message);

            // 만들어진 데이터 반영.
            message.Data = msg.DATA;

            var result = PCANBasic.Write(handle, ref msg);
            ThrowIfError(result);
        }

        /// <summary>
        /// FD 가능한 채널로 CAN 메시지를 전송한다.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        /// <returns></returns>
        public static void WriteFD(ushort handle, CanMessage message)
        {
            var msg = new TPCANMsgFD();
            msg.ID = message.ID;
            msg.MSGTYPE = (TPCANMessageType)message.MessageType;
            msg.DLC = message.DLC;
            msg.DATA = new byte[64];
            message.Data?.CopyTo(msg.DATA, 0);

            // 시그널 Packing.
            PackSignals(msg.DATA, message);

            // 만들어진 데이터 반영.
            message.Data = msg.DATA;

            var result = PCANBasic.WriteFD(handle, ref msg);
            ThrowIfError(result);
        }

        /// <summary>
        /// ISO TP 채널로 CAN Frame을 전송한다.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        public static void WriteTP(cantp_handle handle, CanMessage message)
        {
            cantp_msg cantpMessage = new cantp_msg();

            try
            {
                // 메시지 생성.
                var status = CanTpApi.MsgDataAlloc_2016(out cantpMessage, cantp_msgtype.PCANTP_MSGTYPE_CANFD);
                TP_ThrowIfError(status);

                var data = new byte[64];
                message.Data?.CopyTo(data, 0);

                // 시그널 Packing.
                PackSignals(data, message);

                var msgType = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (message.MessageType.HasFlag(CanMessageTypes.RTR))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_RTR;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.Extended))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_EXTENDED;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.FD))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.BRS))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.ESI))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_ESI;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.ErrorFrame))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_ERRFRAME;
                }
                if (message.MessageType.HasFlag(CanMessageTypes.PcanStatus))
                {
                    msgType |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STATUS;
                }

                // 생성된 메시지 초기화.
                status = CanTpApi.MsgDataInit_2016(out cantpMessage, message.ID, msgType,
                    (uint)CanMessage.GetLengthFromDLC(message.DLC, false), data);

                TP_ThrowIfError(status);

                // ISO TP 메시지 전송.
                status = CanTpApi.Write_2016(handle, ref cantpMessage);

                TP_ThrowIfError(status);
            }
            finally
            {
                CanTpApi.MsgDataFree_2016(ref cantpMessage);
            }
        }

        /// <summary>
        /// 수신 큐에서 CAN 메시지와 타임 스탬프를 읽는다.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        /// <param name="timestamp">마이크로초.</param>
        /// <returns>메시지를 읽었으면 true, 수신 큐가 비었으면 false.</returns>
        /// <exception cref="Exception"></exception>
        public static bool Read(ushort handle, out CanMessage message, out ulong timestamp)
        {
            message = new CanMessage();
            timestamp = 0;

            var result = PCANBasic.Read(handle, out var msg, out var ts);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                if (result == TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                {
                    return false;
                }

                ThrowIfError(result);
            }

            message.ID = msg.ID;
            message.MessageType = (CanMessageTypes)msg.MSGTYPE;
            message.DLC = msg.LEN;
            if (msg.DATA != null)
            {
                message.Data = new byte[msg.DATA.Length];
                msg.DATA.CopyTo(message.Data, 0);
            }

            timestamp = ts.micros + 1000UL * ts.millis + 0x100000000UL * 1000 * ts.millis_overflow;

            return true;
        }

        /// <summary>
        /// FD 가능한 채널의 수신 큐에서 CAN 메시지와 타임 스탬프를 읽는다.
        /// </summary>
        /// <param name="handle"></param>
        /// <param name="message"></param>
        /// <param name="timestamp">Windows 시작부터의 마이크로초.</param>
        /// <returns>메시지를 읽었으면 true, 수신 큐가 비었으면 false.</returns>
        /// <exception cref="Exception"></exception>
        public static bool ReadFD(ushort handle, out CanMessage message, out ulong timestamp)
        {
            message = new CanMessage();

            var result = PCANBasic.ReadFD(handle, out var msg, out timestamp);
            if (result != TPCANStatus.PCAN_ERROR_OK)
            {
                if (result == TPCANStatus.PCAN_ERROR_QRCVEMPTY)
                {
                    return false;
                }

                ThrowIfError(result);
            }

            message.ID = msg.ID;
            message.MessageType = (CanMessageTypes)msg.MSGTYPE;
            message.DLC = msg.DLC;
            if (msg.DATA != null)
            {
                message.Data = new byte[msg.DATA.Length];
                msg.DATA.CopyTo(message.Data, 0);
            }

            return true;
        }

        public PeakCanDevice(string name) : base(DeviceType.PeakCAN, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
        }

        public override void Disconnect()
        {
        }

        public ushort GetHandle()
        {
            var canSetting = Setting as CanDeviceSetting;
            return GetHandle(canSetting.ConnectionType, canSetting.Channel);
        }

        public override void Open(CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate)
        {
            // 디바이스 핸들을 얻는다.
            if (handle == 0)
            {
                var deviceHandle = GetHandle();
                InitializeFD(deviceHandle, nominalBaudRate, dataBaudRate);
                handle = deviceHandle;

                //StartPeriodic();
            }
        }

        public override void Close()
        {
            StopPeriodic(null, true);
            Uninitialize(handle);
            handle = 0;
        }

        public override void ResetQueues()
        {
            ResetQueues(handle);
        }

        public override void Send(CanMessage message, bool canTp)
        {
            //// Debugging.
            //var channelHandle = handle != 0 ? handle : (ushort)udsHandle;

            if (!canTp)
            {
                WriteFD(handle, message);
            }
            else
            {
                WriteTP(udsHandle, message);
            }
        }

        public override void SendPeriodic(CanMessage message, bool canTp)
        {
            // Cycle이 0이면 한번만 보낸다.
            if (message.Cycle == 0)
            {
                Send(message, canTp);
                return;
            }

            // 해당 메시지를 보내는 태스크가 있는지 체크.
            // 같은 ID를 가진 메시지를 보내는 태스크들은 Cancel한다.
            StopPeriodic(message.ID, true);

            // 메시지를 주기적으로 보내는 새로운 태스크를 만든다.
            periodicMessageTasks.Add(new PeriodicMessageTask(this, message, canTp));
        }

        public override void StopPeriodic(uint? canId, bool remove)
        {
            if (canId != null)
            {
                for (int i = periodicMessageTasks.Count - 1; i >= 0; i--)
                {
                    var periodicTask = periodicMessageTasks[i];
                    if (periodicTask.Message.ID == canId)
                    {
                        periodicTask.CTSource.Cancel();

                        if (remove)
                        {
                            periodicMessageTasks.RemoveAt(i);
                        }
                    }
                }
            }
            else
            {
                foreach (var periodicTask in periodicMessageTasks)
                {
                    periodicTask.CTSource.Cancel();
                }

                if (remove)
                {
                    periodicMessageTasks.Clear();
                }
            }
        }

        private void StartPeriodic()
        {
            foreach (var periodicTask in periodicMessageTasks)
            {
                periodicTask.Start();
            }
        }

        public override List<CanMessage> ReadAll()
        {
            var messages = new List<CanMessage>();

            // PCAN 메시지 큐가 빌 때까지 CAN 메시지들을 읽는다.
            while (ReadFD(handle, out var message, out var timestamp))
            {
                message.Timestamp = timestamp;
                messages.Add(message);
            }

            return messages;
        }

        public override void SetLogMode(CanLogModes? logMode, string directory)
        {
            if (logMode != null)
            {
                uint logFunction = PCANBasic.LOG_FUNCTION_DEFAULT;

                if (logMode?.HasFlag(CanLogModes.RxMessage) ?? false)
                {
                    logFunction |= PCANBasic.LOG_FUNCTION_READ;
                }

                if (logMode?.HasFlag(CanLogModes.TxMessage) ?? false)
                {
                    logFunction |= PCANBasic.LOG_FUNCTION_WRITE;
                }

                // 로그 모드 설정.
                var result = PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_CONFIGURE, ref logFunction, sizeof(uint));
                ThrowIfError(result);
            }

            if (directory != null)
            {
                // 로그 폴더 설정.
                Directory.CreateDirectory(directory);
                var result = PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_LOCATION, directory, (uint)directory.Length);
                ThrowIfError(result);
            }
        }

        public override void SetLogEnabled(bool enabled)
        {
            uint status = (uint)(enabled ? PCANBasic.PCAN_PARAMETER_ON : PCANBasic.PCAN_PARAMETER_OFF);
            var result = PCANBasic.SetValue(PCANBasic.PCAN_NONEBUS, TPCANParameter.PCAN_LOG_STATUS, ref status, sizeof(uint));
            ThrowIfError(result);
        }

        #region UDS Methods

        // 에러 상태이면 관련 에러 메시지를 얻어 예외를 던진다.
        private static void UDS_ThrowIfError(uds_status status)
        {
            if (!UDSApi.StatusIsOk_2013(status))
            {
                // Get error text.
                var textBuffer = new StringBuilder(256);
                UDSApi.GetErrorText_2013(status, 0, textBuffer, (uint)textBuffer.Capacity);
                throw new Exception($"CAN UDS Error: {textBuffer}");
            }
        }

        /// <summary>A function that displays UDS Request and Response messages (and count error if no response)</summary>
        /// <param name="request">Request message</param>
        /// <param name="response">Received response message</param>
        /// <param name="no_response_expected">if no response is expected, do not increment error counter</param>
        private static void display_uds_msg(ref uds_msg request, ref uds_msg response, bool no_response_expected)
        {
            display_uds_msg_request(ref request);

            display_uds_msg_response(ref response, no_response_expected);
        }

        private static void display_uds_msg_request(ref uds_msg request)
        {
            cantp_msgdata_isotp req_isotp = new cantp_msgdata_isotp();
            if (request.msg.Msgdata != IntPtr.Zero)
                req_isotp = request.msg.Msgdata_isotp_Copy;

            Logger.LogDebug(string.Format("UDS request from 0x{0:X4} (to 0x{1:X4}, with extension address 0x{2:X2}) - result: {3} - {4}",
                req_isotp.netaddrinfo.source_addr, req_isotp.netaddrinfo.target_addr,
                req_isotp.netaddrinfo.extension_addr, (int)req_isotp.netstatus,
                (req_isotp.netstatus != cantp_netstatus.PCANTP_NETSTATUS_OK) ? "ERROR !!!" : "OK !"));

            // display data
            var data = new List<byte>();
            for (int i = 0; i < req_isotp.length; i++)
            {
                CanTpApi.getData_2016(ref request.msg, i, out byte val);
                data.Add(val);
            }
            Logger.LogDebug(string.Format("-> Length: {0}, Data = {1}", req_isotp.length, string.Join(" ", data.Select(b => $"{b:X2}"))));

            // Display CAN info.
            Logger.LogInfo($"UDS Tx {request.msg.can_info.can_id:X} [{data.Count}] {string.Join(" ", data.Select(b => $"{b:X2}"))}");
        }

        private static void display_uds_msg_response(ref uds_msg response, bool no_response_expected)
        {
            if (response.msg.Msgdata != IntPtr.Zero)
            {
                cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;

                Logger.LogDebug(string.Format("UDS RESPONSE from 0x{0:X4} (to 0x{1:X4}, with extension address 0x{2:X2}) - result: {3} - {4}",
                    resp_isotp.netaddrinfo.source_addr, resp_isotp.netaddrinfo.target_addr,
                    resp_isotp.netaddrinfo.extension_addr, (int)resp_isotp.netstatus,
                    (resp_isotp.netstatus != cantp_netstatus.PCANTP_NETSTATUS_OK) ? "ERROR !!!" : "OK !"));

                // display data
                var data = new List<byte>();
                for (int i = 0; i < resp_isotp.length; i++)
                {
                    CanTpApi.getData_2016(ref response.msg, i, out byte val);
                    data.Add(val);
                }
                Logger.LogDebug(string.Format("-> Length: {0}, Data = {1}", resp_isotp.length, string.Join(" ", data.Select(b => $"{b:X2}"))));

                // Display CAN info.
                Logger.LogInfo($"UDS Rx {response.msg.can_info.can_id:X} [{data.Count}] {string.Join(" ", data.Select(b => $"{b:X2}"))}");
            }
            else if (!no_response_expected)
            {
                Logger.LogDebug(" /!\\ ERROR: NO UDS RESPONSE !!");
            }
        }

        private static string CreateSummary(string deviceName, bool received, uds_msg message)
        {
            var direction = received ? "Rx" : "Tx";
            var timeText = DateTime.Now.ToString("HH:mm:ss.fff");
            var data = new List<byte>();
            if (message.msg.Msgdata != IntPtr.Zero)
            {
                for (int i = 0; i < message.msg.Msgdata_isotp_Copy.length; i++)
                {
                    CanTpApi.getData_2016(ref message.msg, i, out byte val);
                    data.Add(val);
                }
            }
            var dataText = string.Join(" ", data.Select(x => $"{x:X2}"));
            return $"{deviceName} {direction} [{timeText}] {message.msg.can_info.can_id:X} [{data.Count}] {dataText}";
        }

        public override void UDS_Open(CanNominalBaudRate nominalBaudRate, CanDataBaudRate dataBaudRate)
        {
            if (udsHandle == 0)
            {
                // Initializing of the UDS Communication session
                cantp_handle clientHandle = (cantp_handle)GetHandle();
                uds_status status = UDSApi.InitializeFD_2013(clientHandle, CreateBitRateText(nominalBaudRate, dataBaudRate));
                Logger.LogError($"UDS InitializeFD_2013: {status}");
                UDS_ThrowIfError(status);

                // Define Timeouts
                uint buffer = CanTpApi.PCANTP_ISO_TIMEOUTS_15765_2;
                status = UDSApi.SetValue_2013(clientHandle, uds_parameter.PUDS_PARAMETER_ISO_TIMEOUTS, ref buffer, sizeof(uint));
                Logger.LogDebug($"UDS Set ISO 15765-2 timeouts values: {status}");
                UDS_ThrowIfError(status);

                udsHandle = clientHandle;
            }
        }

        public override void UDS_Close()
        {
            StopPeriodic(null, true);
            var status = UDSApi.Uninitialize_2013(udsHandle);
            udsHandle = 0;
            Logger.LogDebug($"UDS Uninitialize_2013: {status}");
        }

        public override void UDS_ReadDataByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort did, out byte[] responseData, 
            StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Send request.
                UDSApi.uds_svc_param_di[] buffer = { (UDSApi.uds_svc_param_di)did };
                uds_status status = UDSApi.SvcReadDataByIdentifier_2013(udsHandle, config, out request, buffer, 1);
                Logger.LogDebug($"UDS SvcReadDataByIdentifier_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // Receive response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                Logger.LogDebug($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        public override void UDS_WriteDataByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort did, byte[] writingData, out byte[] responseData,
            StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Send request.
                uds_status status = UDSApi.SvcWriteDataByIdentifier_2013(udsHandle, config, out request, (UDSApi.uds_svc_param_di)did,
                    writingData, (uint)writingData?.Length);
                Logger.LogDebug($"UDS SvcWriteDataByIdentifier_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // Receive response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                Logger.LogDebug($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        public override void UDS_DiagnosticSessionControl(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_SessionType sessionType, 
            out byte[] responseData, StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Read default session information Server is not yet known (status will be PUDS_ERROR_NOT_INITIALIZED)
                // yet the API will still set session info to default values
                uds_sessioninfo session_info = new uds_sessioninfo();
                session_info.nai = config.nai;
                int session_size = Marshal.SizeOf(session_info);
                IntPtr session_ptr = Marshal.AllocHGlobal(session_size);
                Marshal.StructureToPtr(session_info, session_ptr, true);
                var status = UDSApi.GetValue_2013(udsHandle, uds_parameter.PUDS_PARAMETER_SESSION_INFO, session_ptr, (uint)session_size);
                session_info = (uds_sessioninfo)Marshal.PtrToStructure(session_ptr, typeof(uds_sessioninfo));
                Marshal.FreeHGlobal(session_ptr);
                Logger.LogDebug(string.Format("Diagnostic Session Information: {0}, 0x{1:X4} => {2} = [{3:X4}; {4:X4}]",
                    status, session_info.nai.target_addr, session_info.session_type,
                    session_info.timeout_p2can_server_max, session_info.timeout_enhanced_p2can_server_max));

                // Set Diagnostic session
                status = UDSApi.SvcDiagnosticSessionControl_2013(udsHandle, config, out request, (UDSApi.uds_svc_param_dsc)sessionType);
                Logger.LogDebug($"UDS SvcDiagnosticSessionControl_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // Read response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                Logger.LogDebug($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        public override void UDS_SecurityAccess(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_SecurityAccessType accessType, byte[] securityData, 
            out byte[] responseData, StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Send request.
                var status = UDSApi.SvcSecurityAccess_2013(udsHandle, config, out request, (byte)accessType, securityData, (uint)securityData.Length);
                Logger.LogDebug($"UDS SvcSecurityAccess_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // Receive response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                Logger.LogDebug($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        /// <summary>Helper: Inverts the bytes of a 32 bits numeric value</summary>
        /// <param name="v">Value to revert</param>
        /// <returns>Reverted value</returns>
        private static uint Reverse32(uint v)
        {
            return (v & 0x000000FF) << 24 | (v & 0x0000FF00) << 8 | (v & 0x00FF0000) >> 8 | ((v & 0xFF000000) >> 24) & 0x000000FF;
        }

        static void UInt32ToBytes(uint dw_buffer, byte[] byte_buffer)
        {
            byte_buffer[0] = (byte)(dw_buffer & 0x000000FF);
            byte_buffer[1] = (byte)((dw_buffer & 0x0000FF00) >> 8);
            byte_buffer[2] = (byte)((dw_buffer & 0x00FF0000) >> 16);
            byte_buffer[3] = (byte)((dw_buffer & 0xFF000000) >> 24);
        }

        public override void UDS_RoutineControl(UDS_ECU_Address ecuAddr, bool fd, bool brs, UDS_RoutineControlType controlType, ushort routineId, 
            byte[] controlOption, out byte[] responseData, StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Sends a physical RoutineControl message
                var status = UDSApi.SvcRoutineControl_2013(udsHandle, config, out request, (UDSApi.uds_svc_param_rc)controlType,
                    (UDSApi.uds_svc_param_rc_rid)routineId, controlOption, (uint)controlOption.Length);
                Logger.LogVerbose($"UDS SvcRoutineControl_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // HACK: Pending Response가 계속 오는 것과 관련, 3초 후 강제 종료.
                bool waitingForService = true;
                Task.Delay(3000).ContinueWith(t =>
                {
                    if (waitingForService)
                    {
                        UDS_Close();
                    }
                });

                // Receive response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                waitingForService = false;
                Logger.LogVerbose($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            catch (Exception ex)
            {
                responseData = new byte[0];
                Logger.LogError($"UDS_RoutineControl Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        public override void UDS_InputOutputControlByIdentifier(UDS_ECU_Address ecuAddr, bool fd, bool brs, ushort dataIdentifier,
            byte[] controlOption, out byte[] responseData, StringBuilder commLog, CancellationToken token)
        {
            if (udsHandle == 0)
            {
                UDS_ThrowIfError(uds_status.PUDS_STATUS_NOT_INITIALIZED);
            }

            token.ThrowIfCancellationRequested();
            token.Register(() => UDS_Close());

            var request = new uds_msg();
            var response = new uds_msg();
            var confirmation = new uds_msg();
            try
            {
                // Address, options.
                var config = new uds_msgconfig();
                config.can_id = (uint)(ecuAddr - UDS_ECU_Address.ECU_1 + uds_can_id.PUDS_CAN_ID_ISO_15765_4_PHYSICAL_REQUEST_1);
                config.can_msgtype = cantp_can_msgtype.PCANTP_CAN_MSGTYPE_STANDARD;
                if (fd)
                {
                    config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_FD;
                    if (brs)
                    {
                        config.can_msgtype |= cantp_can_msgtype.PCANTP_CAN_MSGTYPE_BRS;
                    }
                }
                config.nai.protocol = uds_msgprotocol.PUDS_MSGPROTOCOL_ISO_15765_2_11B_NORMAL;
                config.nai.target_type = cantp_isotp_addressing.PCANTP_ISOTP_ADDRESSING_PHYSICAL;
                config.type = uds_msgtype.PUDS_MSGTYPE_USDT;
                config.nai.source_addr = (ushort)uds_address.PUDS_ADDRESS_ISO_15765_4_ADDR_TEST_EQUIPMENT;
                config.nai.target_addr = (ushort)ecuAddr;

                // Sends a physical InputOutputControlByIdentifier message
                var status = UDSApi.SvcInputOutputControlByIdentifier_2013(udsHandle, config, out request, (UDSApi.uds_svc_param_di)dataIdentifier,
                    controlOption, (uint)controlOption.Length);
                Logger.LogVerbose($"UDS SvcInputOutputControlByIdentifier_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, false, request));
                }
                display_uds_msg_request(ref request);

                // Receive response.
                status = UDSApi.WaitForService_2013(udsHandle, ref request, out response, out confirmation);
                Logger.LogVerbose($"UDS WaitForService_2013: {status}");
                UDS_ThrowIfError(status);

                // Log.
                if (commLog != null)
                {
                    commLog.AppendLine(CreateSummary(Setting.DeviceName, true, response));
                }
                display_uds_msg_response(ref response, false);

                // Response data.
                var responseBytes = new List<byte>();
                if (response.msg.Msgdata != IntPtr.Zero)
                {
                    cantp_msgdata_isotp resp_isotp = response.msg.Msgdata_isotp_Copy;
                    for (int i = 0; i < resp_isotp.length; i++)
                    {
                        CanTpApi.getData_2016(ref response.msg, i, out byte val);
                        responseBytes.Add(val);
                    }
                }
                responseData = responseBytes.ToArray();
            }
            catch (Exception ex)
            {
                responseData = new byte[0];
                Logger.LogError($"UDS_InputOutputControlByIdentifier Error: {ex.GetType()} {ex.Message}");
            }
            finally
            {
                // Free messages
                UDSApi.MsgFree_2013(ref request);
                UDSApi.MsgFree_2013(ref response);
                UDSApi.MsgFree_2013(ref confirmation);
            }
        }

        public override void UDS_SetValue(uint parameter, uint value)
        {
            var status = UDSApi.SetValue_2013(udsHandle, (uds_parameter)parameter, ref parameter, sizeof(uint));
            Logger.LogVerbose($"UDS SetValue_2013: {status}");
            UDS_ThrowIfError(status);
        }

        #endregion // UDS Methods

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopPeriodic(null, true);
                    Close();
                    UDS_Close();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
