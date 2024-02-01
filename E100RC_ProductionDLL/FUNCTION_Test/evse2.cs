using System;
using System.Text;
using System.Runtime.InteropServices;
//using System.Windows.Forms;
//using System.Drawing;

namespace E100RC_Production
{


    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class Msg_Header2
    {
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] chSign = new byte[2] { (byte)'R', (byte)'N' };        // ----------------------> Company Signiture
        public byte Msg_ID = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] reserved = new byte[3] { 0, 0, 0 };
        public ushort Msg_LEN = 0;

        public Msg_Header2()
        {
        }

        public Msg_Header2(byte[] data)
        {
            if (data.Length == 2)
            {
                chSign[0] = data[0];
                chSign[1] = data[1];
            }
            else if (data.Length > Marshal.SizeOf(typeof(Msg_Header)))
            {
                chSign[0] = data[0];
                chSign[1] = data[1];
                Msg_ID = data[2];
                Msg_LEN = data[3];
            }

        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stSessionSetup2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVCCIDLength = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
        public byte[] EVCCID = new byte[8] { 0, 0, 0, 0, 0, 0, 0, 0 };
        public uint Checksum = 0;
        /*
            public stSessionSetupReq(byte[] a_chSign)
            {
                MsgHeader = new Msg_Header2(a_chSign);
            }
        */
        public stSessionSetup2Req(byte[] data)
        {
            int pos = Marshal.SizeOf(typeof(Msg_Header2));
            MsgHeader = new Msg_Header2(data);
            EVCCIDLength = data[pos];
            pos += (1 + Padding.Length);
            for (int cnt = 0; cnt < EVCCID.Length; cnt++)
            {
                EVCCID[cnt] = data[cnt + pos];
            }
            pos += (EVCCID.Length);
            Checksum = (uint)(data[pos] | (data[pos + 1] << 8) | (data[pos + 2] << 16) | (data[pos + 3] << 24));

        }


    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stSessionSetup2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public uint DateTimeNow;
        public byte EVSEIDLength;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
        public byte[] EVSEID = new byte[32];
        public uint Checksum;


        public stSessionSetup2Res(byte nResCode = 0)
        {
            ResCode = nResCode;
            init();
        }

        public stSessionSetup2Res(byte[] a_chSign, byte nResCode = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            init();

        }

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.SessionSetupRes;//0x82;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionSetup2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));

            //ResCode = nResCode;
            EVSEIDLength = 1;

            System.Array.Clear(Padding, 0, Padding.Length);
            System.Array.Clear(EVSEID, 0, EVSEID.Length);

            Checksum = 0;

            DateTime datetime = DateTime.UtcNow;
            DateTimeNow = ToDosDateTime(datetime);

        }



        public static UInt32 ToDosDateTime(DateTime dateTime)
        {
            DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
            TimeSpan currTime = dateTime - startTime;
            UInt32 time_t = Convert.ToUInt32(Math.Abs(currTime.TotalSeconds));
            return time_t;
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stServiceDiscovery2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ServiceCategory = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte Checksum = 0;

        public stServiceDiscovery2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

        public stServiceDiscovery2Req()
        {
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stServiceDiscovery2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public ushort ServiceID;
        public byte ServiceCategory;
        public byte EnergyTransferType;
        public byte NumOfOptions;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Options = new byte[2];
        public byte Padding2;
        public uint Checksum;

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ServiceDiscoveryRes;//0x83;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stServiceDiscovery2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            //ResCode = nResCode;
            System.Array.Clear(Padding, 0, Padding.Length);

            ServiceID = 1;
            ServiceCategory = 0;
            EnergyTransferType = 3;
            NumOfOptions = 1;
            Options[0] = 1;
            Options[1] = 0;
            Padding2 = 0;
            Checksum = 0;
        }

        public stServiceDiscovery2Res(byte nResCode = 0)
        {
            ResCode = nResCode;
            init();
        }

        public stServiceDiscovery2Res(byte[] a_chSign, byte nResCode = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            init();
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stServicePaymentSelection2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte SelectedPaymentOption = 0;
        public byte NumOfServices = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] ServiceID = new byte[4];
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding = new byte[2];
        public uint Checksum = 0;

        public stServicePaymentSelection2Req()
        {
        }

        public stServicePaymentSelection2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stServicePaymentSelection2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public uint Checksum;

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ServicePaymentSelectionRes;//0x85;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stServicePaymentSelection2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            System.Array.Clear(Padding, 0, Padding.Length);
            Checksum = 0;
        }

        public stServicePaymentSelection2Res(byte nResCode = 0)
        {
            ResCode = nResCode;
            init();
        }

        public stServicePaymentSelection2Res(byte[] a_chSign, byte nResCode = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            init();
        }

    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stContractAuthentication2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public uint Checksum = 0;

        public stContractAuthentication2Req()
        {
        }
        public stContractAuthentication2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }
    }

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stContractAuthentication2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        public byte EVSEProcessing;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding = new byte[2];
        public uint Checksum;

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ContractAuthenticationRes;//0x87;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stContractAuthentication2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            System.Array.Clear(Padding, 0, Padding.Length);
            Checksum = 0;

        }

        public stContractAuthentication2Res(byte nResCode = 0, byte nEVSEProcessing = 0)
        {
            ResCode = nResCode;
            EVSEProcessing = nEVSEProcessing;
            init();
        }

        public stContractAuthentication2Res(byte[] a_chSign, byte nResCode = 0, byte nEVSEProcessing = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            EVSEProcessing = nEVSEProcessing;
            init();
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stChargeParameterDiscovery2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVRequestedEngergyTransferType = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte EVStatus_EVReady = 0;
        public byte EVStatus_EVCabinConditioning = 0;
        public byte EVStatus_EVRESSConditioning = 0;
        public byte EVStatus_EVErrorCode = 0;
        public byte EVStatus_EVRESSSOC = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding2 = new byte[3];
        public PhysicalValue EVMaximumCurrentLimit = new PhysicalValue();
        public PhysicalValue EVMaximumPowerLimit = new PhysicalValue();
        public PhysicalValue EVMaximumVoltageLimit = new PhysicalValue();
        public PhysicalValue EVEnergyCapacity = new PhysicalValue();
        public PhysicalValue EVEnergyRequest = new PhysicalValue();
        public byte FullSOC = 0;
        public byte BulkSOC = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding3 = new byte[2];
        public uint Checksum = 0;

        public stChargeParameterDiscovery2Req()
        {
        }
        public stChargeParameterDiscovery2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stChargeParameterDiscovery2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte EVSEProcessing = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding2 = new byte[3];
        public byte EVSEStatus_EVSEIsolationStatus = 0;
        public byte EVSEStatus_EVSEStatusCode = 0;
        public byte EVSEStatus_EVSENotification = 0;
        public byte Padding3 = 0;
        public uint EVSEStatus_NotificationMaxDelay = 0;
        public PhysicalValue EVSEMaximumCurrentLimit = new PhysicalValue();
        public PhysicalValue EVSEMaximumPowerLimit = new PhysicalValue();
        public PhysicalValue EVSEMaximumVoltageLimit = new PhysicalValue();
        public PhysicalValue EVSEMinimumCurrentLimit = new PhysicalValue();
        public PhysicalValue EVSEMinimumVoltageLimit = new PhysicalValue();
        public PhysicalValue EVSECurrentRegulationTolerance = new PhysicalValue();
        public PhysicalValue EVSEPeakCurrentRipple = new PhysicalValue();
        public PhysicalValue EVSEEnergyToBeDelivered = new PhysicalValue();
        public byte NumOfSAScheduleTuples = 1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding4 = new byte[3];
        public ushort SAScheduleTuple0_SAScheduleTupleID = 10;
        public ushort SAScheduleTuple0_PMaxScheduleID = 20;
        public byte SAScheduleTuple0_NumOfEntries = 1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding5 = new byte[3];
        public ushort SAScheduleTuple0_PMaxSchedule0_Pmax = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding6 = new byte[2];
        public uint SAScheduleTuple0_PMaxSchedule0_Start = 0;
        public ushort SAScheduleTuple0_PMaxSchedule1_Pmax = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding7 = new byte[2];
        public uint SAScheduleTuple0_PMaxSchedule1_Start = 0;
        public byte SAScheduleTuple0_durationvalid = 1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding8 = new byte[3];
        public uint SAScheduleTuple0_duration = 86400;
        public ushort SAScheduleTuple1_SAScheduleTupleID = 11;
        public ushort SAScheduleTuple1_PMaxScheduleID = 21;
        public byte SAScheduleTuple1_NumOfEntries = 2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding9 = new byte[3];
        public ushort SAScheduleTuple1_PMaxSchedule0_Pmax = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding10 = new byte[2];
        public uint SAScheduleTuple1_PMaxSchedule0_Start = 0;
        public ushort SAScheduleTuple1_PMaxSchedule1_Pmax = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding11 = new byte[2];
        public uint SAScheduleTuple1_PMaxSchedule1_Start = 0;
        public byte SAScheduleTuple1_durationvalid = 1;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding12 = new byte[3];
        public uint SAScheduleTuple1_duration = 0;
        public uint Checksum = 0;

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ChargeParameterDiscoveryRes;//0x85;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stChargeParameterDiscovery2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));

            EVSEProcessing = (byte)EVSEProcessingType.Finished;
            EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
            EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;

        }

        public stChargeParameterDiscovery2Res(byte nResCode = 0, byte uEvseStatus = 0)
        {
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            init();
        }

        public stChargeParameterDiscovery2Res(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            init();
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stPowerDelivery2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ReadyToChargeState;     //1 이면 충전 시작, 0 충전정지
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte EVStatus_EVReady;     //1 이면 EV ready
        public byte EVStatus_EVCabinConditioning;
        public byte EVStatus_EVRESSConditioning;
        public byte EVStatus_EVErrorCode;
        public byte EVStatus_EVRESSSOC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding2 = new byte[3];
        public byte BulkChargingComplete;
        public byte ChargingComplete;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding3 = new byte[2];
        public ushort SAScheduleTupleID; //앞서 충전기가 제공한 ID들 중 EV가 선택한 ID
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding4 = new byte[2];
        public byte NumOfProfileEntries;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding5 = new byte[3];

        public uint ProfileEntry0_ChargingProfileEntryStart;
        public ushort ProfileEntry0_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding6 = new byte[2];
        public uint ProfileEntry1_ChargingProfileEntryStart;
        public ushort ProfileEntry1_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding7 = new byte[2];
        public uint ProfileEntry2_ChargingProfileEntryStart;
        public ushort ProfileEntry2_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding8 = new byte[2];
        public uint ProfileEntry3_ChargingProfileEntryStart;
        public ushort ProfileEntry3_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding9 = new byte[2];
        public uint ProfileEntry4_ChargingProfileEntryStart;
        public ushort ProfileEntry4_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding10 = new byte[2];
        public uint ProfileEntry5_ChargingProfileEntryStart;
        public ushort ProfileEntry5_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding11 = new byte[2];

        public uint ProfileEntry6_ChargingProfileEntryStart;
        public ushort ProfileEntry6_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding12 = new byte[2];
        public uint ProfileEntry7_ChargingProfileEntryStart;
        public ushort ProfileEntry7_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding13 = new byte[2];
        public uint ProfileEntry8_ChargingProfileEntryStart;
        public ushort ProfileEntry8_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding14 = new byte[2];
        public uint ProfileEntry9_ChargingProfileEntryStart;
        public ushort ProfileEntry9_ChargingProfileEntryMaxPower;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding15 = new byte[2];


        public stPowerDelivery2Req()
        { }

        public stPowerDelivery2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stPowerDelivery2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte EVSEStatus_EVSEIsolationStatus;
        public byte EVSEStatus_EVSEStatusCode;
        public byte EVSEStatus_EVSENotification;
        public byte Padding2 = 0;
        public uint EVSEStatus_NotificationMaxDelay;
        public uint Checksum = 0;

        public void init()
        {
            MsgHeader.Msg_ID = (byte)MsgID.PowerDeliveryRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPowerDelivery2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
            EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
            EVSEStatus_NotificationMaxDelay = 1;

            //memset(Padding,0, sizeof(Padding));
            System.Array.Clear(Padding, 0, Padding.Length);
        }

        public stPowerDelivery2Res(byte nResCode = 0, byte uEvseStatus = 0)
        {
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            init();
        }

        public stPowerDelivery2Res(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            init();
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stCableCheck2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVSEStatus_EVReady;     //1 이면 EV ready
        public byte EVStatus_EVCabinConditioning;
        public byte EVStatus_EVRESSConditioning;
        public byte EVStatus_EVErrorCode;
        public byte EVStatus_EVRESSSOC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public uint Checksum;

        public stCableCheck2Req(byte[] data)
        {
            if (data.Length == 2)
            {
                MsgHeader = new Msg_Header2(data);
            }
            else if (data.Length >= Marshal.SizeOf(typeof(stCableCheckReq)))
            {
                int pos = Marshal.SizeOf(typeof(Msg_Header2));
                MsgHeader = new Msg_Header2(data);
                EVSEStatus_EVReady = data[pos];
                pos += 1;
                EVStatus_EVCabinConditioning = data[pos];
                pos += 1;
                EVStatus_EVRESSConditioning = data[pos];
                pos += 1;
                EVStatus_EVErrorCode = data[pos];
                pos += 1;
                EVStatus_EVRESSSOC = data[pos];

            }
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stCableCheck2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public byte EVSEProcessing;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Padding2 = new byte[4] { 0, 0, 0, 0 };
        public byte EVSEStatus_EVSEIsolationStatus;
        public byte EVSEStatus_EVSEStatusCode;
        public byte EVSEStatus_EVSENotification;
        public byte Padding3 = 0;
        public uint EVSEStatus_NotificationMaxDelay;
        public uint Checksum = 0;

        public stCableCheck2Res(byte nResCode, byte uEvseStatus)
        {
            MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheck2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEProcessing = (byte)EVSEProcessingType.Finished;
            EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
            EVSEStatus_NotificationMaxDelay = 0;
        }
        public stCableCheck2Res(byte[] a_chSign, byte nResCode, byte uEvseStatus)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheck2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEProcessing = (byte)EVSEProcessingType.Finished;
            EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
            EVSEStatus_NotificationMaxDelay = 0;
        }
        public stCableCheck2Res(byte[] a_chSign, byte nResCode, byte uEvseStatus, byte EVSEProcessing)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheck2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            this.EVSEProcessing = EVSEProcessing;
            EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
            EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
            EVSEStatus_NotificationMaxDelay = 0;
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stPreCharge2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVSEStatus_EVReady = 0;     //1 이면 EV ready
        public byte EVStatus_EVCabinConditioning = 0;
        public byte EVStatus_EVRESSConditioning = 0;
        public byte EVStatus_EVErrorCode = 0;
        public byte EVStatus_EVRESSSOC = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public PhysicalValue EVTargetVoltage = new PhysicalValue();
        public PhysicalValue EVTargetCurrent = new PhysicalValue();
        public uint Checksum = 0;
        public stPreCharge2Req()
        { }
        public stPreCharge2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stPreCharge2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        public byte EVSEStatus_EVSEStatusCode;
        public byte EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        public byte Padding2 = 0;
        public uint EVSEStatus_NotificationMaxDelay = 0;
        public PhysicalValue EVSEPresentVoltage = new PhysicalValue();
        public uint Checksum = 0;

        public stPreCharge2Res(byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader.Msg_ID = (byte)MsgID.PreChargeRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPreCharge2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }
        public stPreCharge2Res(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.PreChargeRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPreCharge2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stCurrentDemand2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVSEStatus_EVReady = 0;     //1 이면 EV ready
        public byte EVStatus_EVCabinConditioning = 0;
        public byte EVStatus_EVRESSConditioning = 0;
        public byte EVStatus_EVErrorCode = 0;
        public byte EVStatus_EVRESSSOC = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public PhysicalValue EVTargetCurrent = new PhysicalValue();
        public PhysicalValue EVMaximumVoltageLimit = new PhysicalValue();
        public PhysicalValue EVMaximumCurrentLimit = new PhysicalValue();
        public PhysicalValue EVMaximumPowerLimit = new PhysicalValue();
        public byte BulkChargingComplete = 0;
        public byte ChargingComplete = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
        public byte[] Padding2 = new byte[2] { 0, 0 };
        public PhysicalValue RemainingTimeToFullSoC = new PhysicalValue();
        public PhysicalValue RemainingTimeToBulkSoC = new PhysicalValue();
        public PhysicalValue EVTargetVoltage = new PhysicalValue();
        public uint Checksum = 0;
        public stCurrentDemand2Req()
        { }
        public stCurrentDemand2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stCurrentDemand2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public byte EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        public byte EVSEStatus_EVSEStatusCode = 0;
        public byte EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        public byte Padding2 = 0;
        public uint EVSEStatus_NotificationMaxDelay = 0;
        public PhysicalValue EVSEPresentVoltage = new PhysicalValue();
        public PhysicalValue EVSEPresentCurrent = new PhysicalValue();
        public byte EVSECurrentLimitAchieved = 0;
        public byte EVSEVoltageAchieved = 0;
        public byte EVSEPowerLimitAchieved = 0;
        public byte Padding3 = 0;
        public PhysicalValue EVSEMaximumVoltageLimit = new PhysicalValue();
        public PhysicalValue EVSEMaximumCurrentLimit = new PhysicalValue();
        public PhysicalValue EVSEMaximumPowerLimit = new PhysicalValue();
        public uint Checksum = 0;

        public stCurrentDemand2Res(byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader.Msg_ID = (byte)MsgID.CurrentDemandRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCurrentDemand2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }
        public stCurrentDemand2Res(byte[] a_chSigh, byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader = new Msg_Header2(a_chSigh);
            MsgHeader.Msg_ID = (byte)MsgID.CurrentDemandRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCurrentDemand2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stWeldingDetection2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte EVSEStatus_EVReady;     //1 이면 EV ready
        public byte EVStatus_EVCabinConditioning;
        public byte EVStatus_EVRESSConditioning;
        public byte EVStatus_EVErrorCode;
        public byte EVStatus_EVRESSSOC;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public uint Checksum;
        public stWeldingDetection2Req()
        { }
        public stWeldingDetection2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stWeldingDetection2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public byte EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        public byte EVSEStatus_EVSEStatusCode = 0;
        public byte EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        public byte Padding2 = 0;
        public uint EVSEStatus_NotificationMaxDelay = 0;
        public PhysicalValue EVSEPresentVoltage = new PhysicalValue();
        public uint Checksum = 0;

        public stWeldingDetection2Res(byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader.Msg_ID = (byte)MsgID.WeldingDetectionRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stWeldingDetection2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }
        public stWeldingDetection2Res(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.WeldingDetectionRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stWeldingDetection2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
            EVSEStatus_EVSEStatusCode = uEvseStatus;
        }

    };
        
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stSessionStop2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public uint Checksum;
        public stSessionStop2Req()
        { }
        public stSessionStop2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stSessionStop2Res
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ResCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public uint Checksum = 0;

        public stSessionStop2Res(byte nResCode = 0)
        {
            MsgHeader.Msg_ID = (byte)MsgID.SessionStopRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionStop2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
        }
        public stSessionStop2Res(byte[] a_chSign, byte nResCode = 0)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.SessionStopRes;
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionStop2Res)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
            ResCode = nResCode;
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportINIT2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] chSign = new byte[4] { (byte)'I', (byte)'N', (byte)'I', (byte)'T' };
        public uint Checksum;

        public stReportINIT2Req()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
            MsgHeader.Msg_LEN = 4;
        }
        public stReportINIT2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
            MsgHeader.Msg_LEN = 4;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportINIT2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ErrorCode = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Dummy = new byte[3];
        public byte MajorVersion = 0;
        public byte MinorVersion = 0;
        public byte ReleaseVersion = 0;
        public byte InfoLen = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
        public byte[] INFO = new byte[24]; //"UM-REL-20150302-01"
        public uint Checksum = 0;

        public stReportINIT2()
        {
            MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
        }
        public stReportINIT2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stStartRequest2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Info = new byte[4] { 0, 0, 0, 0 };
        public uint Checksum;

        public stStartRequest2()
        {
            MsgHeader.Msg_ID = (byte)MsgID.StartRequest; //0xFC;
            MsgHeader.Msg_LEN = 4;
            Checksum = 0;
        }
        public stStartRequest2(byte[] a_chSign, byte[] info)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = (byte)MsgID.StartRequest; //0xFC;
            MsgHeader.Msg_LEN = 4;
            this.Info[0] = info[0];
            this.Info[1] = info[1];
            this.Info[2] = info[2];
            this.Info[3] = info[3];
            Checksum = 0;
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stStartResponse2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] Info = new byte[4];
        public uint Checksum;

        public stStartResponse2()
        {
        }
        public stStartResponse2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stStartResponse2)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportSLAC2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ErrorCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3];
        public byte AverageAttenuation;  //AverageAttenuation 40 보다 크면 중지 노이즈 값
        public byte Padding2;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
        public byte[] PEV_MAC = new byte[6];
        public uint Checksum = 0;

        public stReportSLAC2()
        {

        }
        public stReportSLAC2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stReportSLAC2)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
        }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportSDP2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ErrorCode = 0;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Padding = new byte[3] { 0, 0, 0 };
        public ushort Tcp_port = 0;
        public byte Sec = 0;
        public byte Tcp = 0;
        public uint Checksum = 0;

        public stReportSDP2()
        { }
        public stReportSDP2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stReportSDP2)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportV2G2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte ErrorCode;
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] Dummy = new byte[3];
        public uint Checksum;
        public stReportV2G2()
        { }
        public stReportV2G2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stReportV2G2)) - Marshal.SizeOf(typeof(Msg_Header2)) - Marshal.SizeOf(typeof(uint)));
        }
    };

    /*
    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class stValidate2Req
    {
    public Msg_Header2 MsgHeader = new Msg_Header2();      //ID : 0x75
    public byte ErrorCode = (byte)EVSENotificationType.None;
    public byte Timer;           //0 – 100ms, 1- 200ms
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public uint Checksum;

    public stValidateReq()
    { }
    public stValidateReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header2(a_chSign);
    }
    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    class stValidate2Res
    {
    public Msg_Header2 MsgHeader = new Msg_Header2();      //ID : 0x75
    public byte ErrorCode;
    public uint ToggleNum;
    public uint Checksum = 0;
    public stValidateRes(uint toggleNum)
    {
        MsgHeader.Msg_ID = 0xF5;
        MsgHeader.Msg_LEN = 4;
        ErrorCode = (byte)EVSENotificationType.None;
        ToggleNum = toggleNum;
    }
    public stValidate2Res(byte[] a_chSigh, uint toggleNum)
    {
        MsgHeader = new Msg_Header2(a_chSign);
        MsgHeader.Msg_ID = 0xF5;
        MsgHeader.Msg_LEN = 4;
        ErrorCode = (byte)EVSENotificationType.None;
        ToggleNum = toggleNum;
    }

    };
    */

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportSTATE2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte type; //1 = CP State, 2 = PD State,
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] info = new byte[3];
        public uint Checksum;
        public stReportSTATE2Req(byte nType)
        {
            MsgHeader.Msg_ID = (byte)MsgID.ReportSTATEReq;
            MsgHeader.Msg_LEN = 4;
            type = nType;
        }
        public stReportSTATE2Req(byte[] a_chSigh, byte nType)
        {
            MsgHeader = new Msg_Header2(a_chSigh);
            MsgHeader.Msg_ID = (byte)MsgID.ReportSTATEReq;
            MsgHeader.Msg_LEN = 4;
            type = nType;
        }


    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stReportSTATE2
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        public byte type;       //type = 1
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
        public byte[] info = new byte[3];    //info[0] = cp voltage
        public uint Checksum;   //type = 2
        //info[0] = PD type (1,2)
        //PDtype = 1, Info[1] = (0=UNPLUG, 1=S3CLOSE, 2=S3OPEN)
        //PDtype = 2, Info[1] = (0=UNPLUG, 1=PLUG)

        public stReportSTATE2()
        { }
        public stReportSTATE2(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
        }

    };

    [StructLayout(LayoutKind.Sequential, Pack = 1)]
    public class stAllStop2Req
    {
        public Msg_Header2 MsgHeader = new Msg_Header2();
        [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
        public byte[] info = new byte[4] { (byte)'S', (byte)'T', (byte)'O', (byte)'P' };
        public uint Checksum;
        public stAllStop2Req()
        {
            MsgHeader.Msg_ID = 0xFE;
            MsgHeader.Msg_LEN = 4;
            Checksum = 0;
        }

        public stAllStop2Req(byte[] a_chSign)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = 0xFE;
            MsgHeader.Msg_LEN = 4;
            Checksum = 0;
        }
        public stAllStop2Req(byte[] a_chSign, byte[] info)
        {
            MsgHeader = new Msg_Header2(a_chSign);
            MsgHeader.Msg_ID = 0xFE;
            MsgHeader.Msg_LEN = 4;
            this.info[0] = info[0];
            this.info[1] = info[1];
            this.info[2] = info[2];
            this.info[3] = info[3];
            Checksum = 0;
        }

    };



    /// <summary>
    /// //////////////////////////////////////////////////////////////////////////////////////////////////
    /// </summary>



    partial class  dcComboPLC
    {

        // for EOL Test

        public unsafe void recieveData2(byte[] aData)
        {
            //int nRemain = 0;
            byte[] DataBuffer = new byte[1];
            string curtime = DateTime.Now.ToString("HH:mm:ss.fff ");
            string str_temp = "[" + curtime + "]" + ":recieveData Cmd 0x" + Convert.ToUInt32(aData[2]).ToString("x02");
            DebugString(str_temp + "\n");

            if (dbglevel > 5)
            {
                str_temp = "";
                for (int cnt = 0; cnt < aData.Length; cnt++)
                {
                    if ((cnt % 8 == 0) && (cnt != 0))
                    {
                        str_temp += "\n";
                    }
                    str_temp += (aData[cnt].ToString("x02") + " ");
                }
                DebugString(str_temp + "\n");
            }
            MsgID msgid = (MsgID)aData[2];
            switch (msgid)
            {
                case MsgID.ReportINITRes: // 0x71
                    {
                        DebugString("receive ReportINITRes" + "\n");
                        stReportINIT2 pReportINIT = new stReportINIT2(chSign);// = NULL;
                        fixed (byte* pData = aData)
                        {
                            pReportINIT = (stReportINIT2)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportINIT2));
                        }

                        string strInfo = Encoding.Default.GetString(pReportINIT.INFO);

                        DebugString(strInfo + "\n");

                        if (pReportINIT.ErrorCode == 0)
                        {
                            bPLC_Connect = true;
                            bCurrentReq = false;


                            stStartRequest2 pStartReq = new stStartRequest2(chSign, startReq_info);
                            DataBuffer = new byte[Marshal.SizeOf(typeof(stStartRequest2))];
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
                            }
                            pStartReq.Checksum = getChecksum(DataBuffer);
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
                            }
                            DebugString("receive stStartRequest2" + "\n");
                            string str_dbg = "start_req.info (hex) = " + startReq_info[0].ToString("x02") + " " + startReq_info[1].ToString("x02") + " " + startReq_info[3].ToString("x02") + " " + startReq_info[3].ToString("x02");
                            DebugString(str_dbg + "\n");


                        }
                        else
                        {
                            str_temp = ("ReportINITRes ErrorCode : " + pReportINIT.ErrorCode.ToString("x02"));
                            DebugString(str_temp + "\n");
                        }
                        break;
                    }

                #region CCS Charging Message


                case MsgID.SessionSetupReq:     // 0x02
                    {
                        DebugString("receive SessionSetupReq" + "\n");

                        curChagerStep = MsgID.SessionSetupReq;
                        bPLC_SessionRequest = true;

                        stSessionSetup2Req pSessionSetupReq = new stSessionSetup2Req(aData);

                        string HexID = "EVCCID =";
                        for (int i = 0; i < 8; i++)
                        {
                            HexID += pSessionSetupReq.EVCCID[i].ToString("x02") + " ";
                        }
                        Console.WriteLine(HexID);
                        status_text += (HexID + "\n");

                        responseCodeType ResponseCode = responseCodeType.OK_NewSessionEstablished;
                        if (evse_resp_stop_condition.checkStop_sessionSetup())
                            ResponseCode = evse_resp_stop_condition.sessionSetupResCode;

                        stSessionSetup2Res pSessionSetupRes = new stSessionSetup2Res(chSign, (byte)ResponseCode);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stSessionSetup2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pSessionSetupRes, (IntPtr)pData, false);
                        }
                        pSessionSetupRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pSessionSetupRes, (IntPtr)pData, false);
                        }

                        str_temp = ("send SessionSetupRes ");
                        DebugString(str_temp + "\n");

                        break;
                    }

                case MsgID.ServiceDiscoveryReq:     // 0x03
                    {
                        DebugString("receive ServiceDiscoveryReq" + "\n");

                        curChagerStep = MsgID.ServiceDiscoveryReq;
                        nPLC_CommCheck++;
                        bPLC_SessionRequest = false;

                        stServiceDiscovery2Req pServiceDiscoveryReq = new stServiceDiscovery2Req(chSign);// = NULL;
                        fixed (byte* pData = aData)
                        {
                            pServiceDiscoveryReq = (stServiceDiscovery2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stServiceDiscovery2Req));
                        }


                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_serviceDiscovery())
                            ResponseCode = evse_resp_stop_condition.serviceDiscoveryResCode;

                        if ((pServiceDiscoveryReq.ServiceCategory == 0)
                            || (ResponseCode >= responseCodeType.FAILED))
                        {
                            str_temp = ("receive ServiceDiscoveryReq - ServiceCategory 0");
                            DebugString(str_temp + "\n");

                            stServiceDiscovery2Res pServiceDiscoveryRes = new stServiceDiscovery2Res(chSign, (byte)ResponseCode);
                            DataBuffer = new byte[Marshal.SizeOf(typeof(stServiceDiscovery2Res))];
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pServiceDiscoveryRes, (IntPtr)pData, false);
                            }

                            pServiceDiscoveryRes.Checksum = getChecksum(DataBuffer);
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pServiceDiscoveryRes, (IntPtr)pData, false);
                            }
                            str_temp = ("send ServiceDiscoveryRes ");
                            DebugString(str_temp + "\n");
                        }
                        break;
                    }

                case MsgID.ServicePaymentSelectionReq:
                    {
                        DebugString("receive ServicePaymentSelectionReq" + "\n");

                        curChagerStep = MsgID.ServiceDiscoveryReq;
                        nPLC_CommCheck++;
                        stServicePaymentSelection2Req pServicePaymentSelectionReq = new stServicePaymentSelection2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pServicePaymentSelectionReq = (stServicePaymentSelection2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stServicePaymentSelection2Req));
                        }

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_servicePaymentSelection())
                            ResponseCode = evse_resp_stop_condition.servicePaymentSelectionResCode;


                        if (pServicePaymentSelectionReq.SelectedPaymentOption == 1 &&
                           pServicePaymentSelectionReq.NumOfServices == 1 &&
                           pServicePaymentSelectionReq.ServiceID[0] == 1)
                        {

                            stServicePaymentSelection2Res pServicePaymentSelectionRes = new stServicePaymentSelection2Res(chSign, (byte)ResponseCode);
                            DataBuffer = new byte[Marshal.SizeOf(typeof(stServicePaymentSelection2Res))];
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pServicePaymentSelectionRes, (IntPtr)pData, false);
                            }
                            pServicePaymentSelectionRes.Checksum = getChecksum(DataBuffer);
                            fixed (byte* pData = DataBuffer)
                            {
                                Marshal.StructureToPtr(pServicePaymentSelectionRes, (IntPtr)pData, false);
                            }

                            str_temp = ("send ServicePaymentSelectionRes ");
                            DebugString(str_temp + "\n");
                        }

                        break;
                    }

                case MsgID.ContractAuthenticationReq:
                    {
                        DebugString("receive ContractAuthenticationReq" + "\n");
                        curChagerStep = MsgID.ContractAuthenticationReq;
                        nPLC_CommCheck++;

                        //must update
                        eNumEVSEProcess = EVSEProcessingType.Finished;

                        if (evse_resp_stop_condition.checkStop_contractAuthentication())
                            eNumresponseCode = evse_resp_stop_condition.contractAuthenticationResCode;

                        stContractAuthentication2Res pContractAuthenticationRes = new stContractAuthentication2Res(chSign, (byte)eNumresponseCode, (byte)eNumEVSEProcess);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stContractAuthentication2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pContractAuthenticationRes, (IntPtr)pData, false);
                        }
                        pContractAuthenticationRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pContractAuthenticationRes, (IntPtr)pData, false);
                        }

                        str_temp = ("send ContractAuthenticationRes ");
                        DebugString(str_temp + "\n");

                        break;
                    }

                case MsgID.ChargeParameterDiscoveryReq:
                    {
                        DebugString("receive ChargeParameterDiscoveryReq" + "\n");

                        curChagerStep = MsgID.ChargeParameterDiscoveryReq;
                        nPLC_CommCheck++;

                        stChargeParameterDiscovery2Req pChargeParameterDiscoveryReq = new stChargeParameterDiscovery2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pChargeParameterDiscoveryReq = (stChargeParameterDiscovery2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stChargeParameterDiscovery2Req));
                        }

                        str_temp = ("    EVStatus_EVErrorCode = " + pChargeParameterDiscoveryReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");

                        UnitValue uvMaxCurrentLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumCurrentLimit);
                        UnitValue uvPowerLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumPowerLimit);
                        UnitValue uvMaxEVMaxVoltageLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumVoltageLimit);
                        UnitValue uvMaxEnergyCapacity = new UnitValue(pChargeParameterDiscoveryReq.EVEnergyCapacity);
                        UnitValue uvMaxEnergyRequest = new UnitValue(pChargeParameterDiscoveryReq.EVEnergyRequest);


                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_chargeParameterDiscovery())
                            ResponseCode = evse_resp_stop_condition.chargeParameterDiscoveryResCode;


                        stChargeParameterDiscovery2Res pChargeParameterDiscoveryRes = new stChargeParameterDiscovery2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);


                        if (evse_status_condition.checkStop_chargeParameterDiscovery())
                        {
                            pChargeParameterDiscoveryRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.chargeParameterDiscovery_evse_status.EVSEIsolationStatus;
                            pChargeParameterDiscoveryRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.chargeParameterDiscovery_evse_status.EVSEStatusCode;
                            pChargeParameterDiscoveryRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.chargeParameterDiscovery_evse_status.EVSENotification;
                            pChargeParameterDiscoveryRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.chargeParameterDiscovery_evse_status.NotificationMaxDelay;
                        }

                        pChargeParameterDiscoveryRes.EVSEMaximumCurrentLimit = MaxEVSECurrentLimit;
                        pChargeParameterDiscoveryRes.EVSEMaximumVoltageLimit = MaxEVSEVoltageLimit;
                        pChargeParameterDiscoveryRes.EVSEMaximumPowerLimit = MaxEVSEPowerLimit;
                        pChargeParameterDiscoveryRes.EVSEMinimumCurrentLimit = MinEVSECurrentLimit;
                        pChargeParameterDiscoveryRes.EVSEMinimumVoltageLimit = MinEVSEVoltageLimit;


                        DebugString("    EVSEMaximumCurrentLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumCurrentLimit.Value + " A" + "\n");
                        DebugString("    EVSEMaximumVoltageLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumVoltageLimit.Value + " V" + "\n");
                        DebugString("    EVSEMaximumPowerLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumPowerLimit.Value / 10 + "."
                                                                + pChargeParameterDiscoveryRes.EVSEMaximumPowerLimit.Value % 10 + " kW" + "\n");
                        DebugString("    EVSEMinimumCurrentLimit = " + pChargeParameterDiscoveryRes.EVSEMinimumCurrentLimit.Value + " A" + "\n");
                        DebugString("    EVSEMinimumVoltageLimit = " + pChargeParameterDiscoveryRes.EVSEMinimumVoltageLimit.Value + " V" + "\n");


                        PhysicalValue EVSECurrentRegulationTolerance = new PhysicalValue();
                        PhysicalValue EVSEPeakCurrentRipple = new PhysicalValue();
                        PhysicalValue EVSEEnergyToBeDelivered = new PhysicalValue();

                        SetPhysicalValue(ref EVSECurrentRegulationTolerance, 0, 3, 1);
                        SetPhysicalValue(ref EVSEPeakCurrentRipple, 0, 3, 1);
                        SetPhysicalValue(ref EVSEEnergyToBeDelivered, 0, 7, 25000);
                        pChargeParameterDiscoveryRes.EVSECurrentRegulationTolerance = EVSECurrentRegulationTolerance;
                        pChargeParameterDiscoveryRes.EVSEPeakCurrentRipple = EVSEPeakCurrentRipple;
                        pChargeParameterDiscoveryRes.EVSEEnergyToBeDelivered = EVSEEnergyToBeDelivered;
                        pChargeParameterDiscoveryRes.SAScheduleTuple0_PMaxSchedule0_Pmax = 20000;

                        DataBuffer = new byte[Marshal.SizeOf(typeof(stChargeParameterDiscovery2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pChargeParameterDiscoveryRes, (IntPtr)pData, false);
                        }
                        pChargeParameterDiscoveryRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pChargeParameterDiscoveryRes, (IntPtr)pData, false);
                        }

                        str_temp = ("send ChargeParameterDiscoveryRes ");
                        DebugString(str_temp + "\n");

                        break;
                    }
                case MsgID.PowerDeliveryReq:
                    {
                        DebugString("receive PowerDeliveryReq" + "\n");

                        curChagerStep = MsgID.PowerDeliveryReq;
                        nPLC_CommCheck++;

                        stPowerDelivery2Req pPowerDeliveryReq = new stPowerDelivery2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pPowerDeliveryReq = (stPowerDelivery2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stPowerDelivery2Req));
                        }

                        EV_SOC = pPowerDeliveryReq.EVStatus_EVRESSSOC;
                        str_temp = ("    EV_SOC = " + EV_SOC);
                        DebugString(str_temp + "\n");

                        str_temp = ("    EVStatus_EVErrorCode = " + pPowerDeliveryReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");

                        if (pPowerDeliveryReq.ReadyToChargeState == 0)
                        {
                            bCurrentReq = false;
                        }

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_powerDelivery())
                            ResponseCode = evse_resp_stop_condition.powerDeliveryResCode;


                        stPowerDelivery2Res pPowerDeliveryRes = new stPowerDelivery2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);

                        if (evse_status_condition.checkStop_powerDelivery())
                        {
                            pPowerDeliveryRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.powerDelivery_evse_status.EVSEIsolationStatus;
                            pPowerDeliveryRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.powerDelivery_evse_status.EVSEStatusCode;
                            pPowerDeliveryRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.powerDelivery_evse_status.EVSENotification;
                            pPowerDeliveryRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.powerDelivery_evse_status.NotificationMaxDelay;
                        }

                        DataBuffer = new byte[Marshal.SizeOf(typeof(stPowerDelivery2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pPowerDeliveryRes, (IntPtr)pData, false);
                        }
                        pPowerDeliveryRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pPowerDeliveryRes, (IntPtr)pData, false);
                        }


                        break;
                    }

                case MsgID.CableCheckReq:  // 0x08
                    {
                        DebugString("receive CableCheckReq" + "\n");

                        curChagerStep = MsgID.CableCheckReq;
                        nPLC_CommCheck++;

                        stCableCheck2Req pCableCheckReq = new stCableCheck2Req(aData);

                        str_temp = ("    EVStatus_EVErrorCode = " + pCableCheckReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_cableCheck())
                            ResponseCode = evse_resp_stop_condition.cableCheckResCode;

                        stCableCheck2Res pCableCheckRes;
                        if (cableCheck_cnt >= cableCheck_cnt_max)
                        {
                            pCableCheckRes = new stCableCheck2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);
                        }
                        else
                        {
                            pCableCheckRes = new stCableCheck2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode, (byte)EVSEProcessingType.Ongoing);
                        }



                        pCableCheckRes.EVSEStatus_EVSEIsolationStatus = (byte)v2gisolationLevelType.v2gisolationLevelType_Valid;

                        if (evse_status_condition.checkStop_cableCheck())
                        {
                            pCableCheckRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.cableCheck_evse_status.EVSEIsolationStatus;
                            pCableCheckRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.cableCheck_evse_status.EVSEStatusCode;
                            pCableCheckRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.cableCheck_evse_status.EVSENotification;
                            pCableCheckRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.cableCheck_evse_status.NotificationMaxDelay;
                        }


                        DataBuffer = new byte[Marshal.SizeOf(typeof(stCableCheck2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pCableCheckRes, (IntPtr)pData, false);
                        }
                        pCableCheckRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pCableCheckRes, (IntPtr)pData, false);
                        }

                        break;
                    }
                case MsgID.PreChargeReq:
                    {
                        DebugString("receive PreChargeReq" + "\n");

                        curChagerStep = MsgID.PreChargeReq;
                        nPLC_CommCheck++;
                        bCurrentReq = true;

                        stPreCharge2Req pPreChargeReq = new stPreCharge2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pPreChargeReq = (stPreCharge2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stPreCharge2Req));
                        }

                        UnitValue unitTargetVoltage = new UnitValue(pPreChargeReq.EVTargetVoltage);
                        UnitValue unitTargetCurrent = new UnitValue(pPreChargeReq.EVTargetCurrent);

                        evTargetVoltage = (short)unitTargetVoltage.fValue;
                        evTargetCurrent = (short)unitTargetCurrent.fValue;

                        str_temp = ("    preCharge Target Cur " + unitTargetCurrent.fValue * 1000 + " A");
                        DebugString(str_temp + "\n");
                        if (evseStatusCode == DC_EVSEStatusCodeType.EVSE_EmergencyShutdown)
                        {
                            evTargetVoltage = 0;
                        }

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_precharge())
                            ResponseCode = evse_resp_stop_condition.prechargeResCode;

                        stPreCharge2Res pPreChargeRes = new stPreCharge2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);

                        str_temp = ("    StartSoc =" + pPreChargeReq.EVStatus_EVRESSSOC);
                        DebugString(str_temp + "\n");

                        str_temp = ("    EVStatus_EVErrorCode =" + pPreChargeReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");

                        pPreChargeRes.EVSEPresentVoltage = EVSEVoltage;
                        str_temp = ("    EVSEVoltage =" + EVSEVoltage.Value + "V");
                        DebugString(str_temp + "\n");

                        if (evse_status_condition.checkStop_precharge())
                        {
                            pPreChargeRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.precharge_evse_status.EVSEIsolationStatus;
                            pPreChargeRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.precharge_evse_status.EVSEStatusCode;
                            pPreChargeRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.precharge_evse_status.EVSENotification;
                            pPreChargeRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.precharge_evse_status.NotificationMaxDelay;
                        }

                        DataBuffer = new byte[Marshal.SizeOf(typeof(stPreCharge2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pPreChargeRes, (IntPtr)pData, false);
                        }
                        pPreChargeRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pPreChargeRes, (IntPtr)pData, false);
                        }

                        break;
                    }
                case MsgID.CurrentDemandReq:
                    {
                        DebugString("receive CurrentDemandReq" + "\n");

                        curChagerStep = MsgID.CurrentDemandReq;
                        nPLC_CommCheck++;
                        bCurrentReq = true;

                        stCurrentDemand2Req pCurrentDemandReq = new stCurrentDemand2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pCurrentDemandReq = (stCurrentDemand2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stCurrentDemand2Req));
                        }

                        EV_SOC = pCurrentDemandReq.EVStatus_EVRESSSOC;
                        str_temp = ("    EV_SOC = " + pCurrentDemandReq.EVStatus_EVRESSSOC);
                        DebugString(str_temp + "\n");
                        str_temp = ("    EVStatus_EVErrorCode = " + pCurrentDemandReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");
                        str_temp = ("    EVSEStatus_EVRESSSOC = " + EV_SOC);
                        DebugString(str_temp + "\n");


                        UnitValue uvEVTargetCurrent = new UnitValue(pCurrentDemandReq.EVTargetCurrent);
                        UnitValue uvEVTargetVoltage = new UnitValue(pCurrentDemandReq.EVTargetVoltage);
                        UnitValue uvEVMaximumCurrentLimit = new UnitValue(pCurrentDemandReq.EVMaximumCurrentLimit);
                        UnitValue uvEVMaximumVoltageLimit = new UnitValue(pCurrentDemandReq.EVMaximumVoltageLimit);
                        UnitValue uvEVMaximumPowerLimit = new UnitValue(pCurrentDemandReq.EVMaximumPowerLimit);
                        UnitValue uvRemainingTimeToFullSoC = new UnitValue(pCurrentDemandReq.RemainingTimeToFullSoC);

                        ui_Min = (byte)((uint)uvRemainingTimeToFullSoC.fValue / 60);
                        ui_Sec = (byte)((uint)uvRemainingTimeToFullSoC.fValue % 60);
                        str_temp = ("    Remaining Time: " + ui_Min + ":" + ui_Sec);
                        DebugString(str_temp + "\n");


                        str_temp = ("    EVTargetCurrent = " + uvEVTargetCurrent.sValue );
                        DebugString(str_temp + "\n");
                        str_temp = ("    uvEVTargetVoltage = " + uvEVTargetVoltage.sValue);
                        DebugString(str_temp + "\n");

                        str_temp = ("    uvEVMaximumCurrentLimit = " + uvEVMaximumCurrentLimit.sValue );
                        DebugString(str_temp + "\n");
                        str_temp = ("    uvEVMaximumVoltageLimit = " + uvEVMaximumVoltageLimit.sValue );
                        DebugString(str_temp + "\n");
                        str_temp = ("    uvEVMaximumPowerLimit = " + uvEVMaximumPowerLimit.sValue);
                        DebugString(str_temp + "\n");

                        evTargetVoltage = (short)(uvEVTargetVoltage.fValue);
                        evTargetCurrent = (short)(uvEVTargetCurrent.fValue);


                        if (evseStatusCode == DC_EVSEStatusCodeType.EVSE_EmergencyShutdown)
                        {
                            evTargetVoltage = 0;
                        }

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_currentDemand())
                            ResponseCode = evse_resp_stop_condition.currentDemandResCode;

                        stCurrentDemand2Res pCurrentDemandRes = new stCurrentDemand2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);

                        pCurrentDemandRes.EVSECurrentLimitAchieved = 0;
                        pCurrentDemandRes.EVSEVoltageAchieved = 0;
                        pCurrentDemandRes.EVSEPowerLimitAchieved = 0;

                        pCurrentDemandRes.EVSEMaximumCurrentLimit = MaxEVSECurrentLimit;
                        pCurrentDemandRes.EVSEMaximumPowerLimit = MaxEVSEPowerLimit;
                        pCurrentDemandRes.EVSEMaximumVoltageLimit = MaxEVSEVoltageLimit;

                        pCurrentDemandRes.EVSEPresentCurrent = EVSECurrent;
                        pCurrentDemandRes.EVSEPresentVoltage = EVSEVoltage;

                        str_temp = ("    MaxEVSECurrentLimit = " + MaxEVSECurrentLimit.Value + " A");
                        DebugString(str_temp + "\n");

                        str_temp = ("    MaxEVSEPowerLimit = " + MaxEVSEPowerLimit.Value + " W");
                        DebugString(str_temp + "\n");

                        str_temp = ("    MaxEVSEVoltageLimit = " + MaxEVSEVoltageLimit.Value + " V");
                        DebugString(str_temp + "\n");

                        str_temp = ("    EVSECurrent = " + EVSECurrent.Value);
                        DebugString(str_temp + "\n");

                        str_temp = ("    EVSEVoltage = " + EVSEVoltage.Value);
                        DebugString(str_temp + "\n");

                        if (evse_status_condition.checkStop_currentDemand())
                        {
                            pCurrentDemandRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.currentDemand_evse_status.EVSEIsolationStatus;
                            pCurrentDemandRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.currentDemand_evse_status.EVSEStatusCode;
                            pCurrentDemandRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.currentDemand_evse_status.EVSENotification;
                            pCurrentDemandRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.currentDemand_evse_status.NotificationMaxDelay;
                        }


                        DataBuffer = new byte[Marshal.SizeOf(typeof(stCurrentDemand2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pCurrentDemandRes, (IntPtr)pData, false);
                        }
                        pCurrentDemandRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pCurrentDemandRes, (IntPtr)pData, false);
                        }

                        break;
                    }

                case MsgID.WeldingDetectionReq:
                    {
                        DebugString("receive WeldingDetectionReq" + "\n");

                        curChagerStep = MsgID.WeldingDetectionReq;
                        nPLC_CommCheck++;

                        bCurrentReq = false;


                        stWeldingDetection2Req pWeldingDetectionReq = new stWeldingDetection2Req(chSign);
                        fixed (byte* pData = aData)
                        {
                            pWeldingDetectionReq = (stWeldingDetection2Req)Marshal.PtrToStructure((IntPtr)pData, typeof(stWeldingDetection2Req));
                        }
                        str_temp = ("    EVSEStatus_EVReady " + pWeldingDetectionReq.EVSEStatus_EVReady);
                        DebugString(str_temp + "\n");
                        str_temp = ("    EVStatus_EVRESSSOC " + pWeldingDetectionReq.EVStatus_EVRESSSOC);
                        DebugString(str_temp + "\n");
                        str_temp = ("    EVStatus_EVErrorCode = " + pWeldingDetectionReq.EVStatus_EVErrorCode);
                        DebugString(str_temp + "\n");

                        responseCodeType ResponseCode = responseCodeType.OK;
                        if (evse_resp_stop_condition.checkStop_weldingDetection())
                            ResponseCode = evse_resp_stop_condition.weldingDetectionResCode;


                        stWeldingDetection2Res pWeldingDetectionRes = new stWeldingDetection2Res(chSign, (byte)ResponseCode, (byte)evseStatusCode);
                        if (imiuMinusResistor <= 100.0f || imiuPlusResistor <= 100.0f)
                        {
                            pWeldingDetectionRes.EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Fault;
                        }

                        PhysicalValue EVSEVoltage_Welding = new PhysicalValue();

                        pWeldingDetectionRes.EVSEPresentVoltage = EVSEVoltage_Welding;
                        str_temp = ("send stWeldingDetectionRes : " + EVSEVoltage_Welding);
                        DebugString(str_temp + "\n");

                        if (evse_status_condition.checkStop_weldingDetection())
                        {
                            pWeldingDetectionRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.weldingDetection_evse_status.EVSEIsolationStatus;
                            pWeldingDetectionRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.weldingDetection_evse_status.EVSEStatusCode;
                            pWeldingDetectionRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.weldingDetection_evse_status.EVSENotification;
                            pWeldingDetectionRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.weldingDetection_evse_status.NotificationMaxDelay;
                        }


                        DataBuffer = new byte[Marshal.SizeOf(typeof(stWeldingDetection2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pWeldingDetectionRes, (IntPtr)pData, false);
                        }
                        pWeldingDetectionRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pWeldingDetectionRes, (IntPtr)pData, false);
                        }
                        break;
                    }


                case MsgID.SessionStopReq:
                    {
                        DebugString("receive SessionStopReq" + "\n");
                        curChagerStep = MsgID.SessionStopReq;


                        stSessionStop2Res pSessionStopRes = new stSessionStop2Res(chSign, 0);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stSessionStop2Res))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pSessionStopRes, (IntPtr)pData, false);
                        }
                        pSessionStopRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pSessionStopRes, (IntPtr)pData, false);
                        }
                        str_temp = ("send SessionStopRes");
                        DebugString(str_temp + "\n");

                        bPLC_SessionOK = false;
                        bCurrentReq = false;
                        break;
                    }
                #endregion CCS Charging Message


                #region Report Message
                case MsgID.ReportSLAC:
                    {
                        DebugString("receive ReportSLAC\n");
                        stReportSLAC2 pReportSLAC = new stReportSLAC2(chSign);
                        fixed (byte* pData = aData)
                        {
                            pReportSLAC = (stReportSLAC2)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSLAC2));
                        }
                        string sDebug = "    [SLAC]AverageAttenuation - " + pReportSLAC.AverageAttenuation;
                        DebugString(sDebug + "\n");
                        string str_HexID = "";
                        for (int i = 0; i < 6; i++)
                        {
                            str_HexID += pReportSLAC.PEV_MAC[i].ToString("x02") + " ";
                        }
                        str_HexID = "    PEV_MAC: " + str_HexID;
                        DebugString(str_HexID + "\n");
                        break;
                    }

                case MsgID.ReportSDP:
                    {
                        DebugString("receive ReportSDP\n");
                        stReportSDP2 pReportSDP = new stReportSDP2(chSign);
                        fixed (byte* pData = aData)
                        {
                            pReportSDP = (stReportSDP2)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSDP2));
                        }
                        string sDebug = "[SDP]Tcp_port : - " + pReportSDP.Tcp_port +
                                         ", Sec : " + pReportSDP.Tcp_port +
                                         ", Tcp : " + pReportSDP.Tcp;
                        DebugString(sDebug + "\n");
                        break;
                    }

                case MsgID.ReportV2G:
                    {
                        DebugString("receive ReportV2G" + "\n");

                        stReportV2G2 pReportV2G = new stReportV2G2(chSign);
                        fixed (byte* pData = aData)
                        {
                            pReportV2G = (stReportV2G2)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportV2G2));
                        }
                        str_temp = ("    ReportV2G Error_Code " + pReportV2G.ErrorCode);
                        DebugString(str_temp + "\n");
                        if (pReportV2G.ErrorCode > 0)
                        {
                            str_temp = ("    Charger_ErrorCode = " + pReportV2G.ErrorCode);
                            DebugString(str_temp + "\n");
                            if (curChagerStep == MsgID.PreChargeReq || curChagerStep == MsgID.CurrentDemandReq)
                            {
                                //sgnErrorStop(g_Group);
                            }
                        }
                        break;
                    }

                case MsgID.ReportSTATE:
                    {
                        DebugString("receive ReportSTATE" + "\n");

                        stReportSTATE2 pReportSTATE = new stReportSTATE2(chSign);
                        fixed (byte* pData = aData)
                        {
                            pReportSTATE = (stReportSTATE2)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSTATE2));
                        }

                        if (pReportSTATE.type == 1)
                        {
                            if (pReportSTATE.info[0] == 9)
                            {
                                bPLG = true;
                            }

                            if ((Plc_Voltage == 9 || Plc_Voltage == 6) && pReportSTATE.info[0] == 12)
                            {
                                bPLG = false;
                            }

                            Plc_Voltage = pReportSTATE.info[0];
                            str_temp = ("    Report cp Voltage = " + pReportSTATE.info[0] + " V");
                            DebugString(str_temp + "\n");

                            if (bCurrentReq && (pReportSTATE.info[0] == 9 || pReportSTATE.info[0] == 12))
                            {
                                evTargetCurrent = 0;
                                //evseStatusCode = EVSE_EmergencyShutdown;
                                bCurrentReq = false;
                            }

                        }

                    }
                    break;
                #endregion Report Message

                /////////////////////////////////////////////////////////////////////////////////////////////////////////
//      for EOL Test
                /////////////////////////////////////////////////////////////////////////////////////////////////////////
              








            }

            if (DataBuffer.Length > 7)
            {
                SendPLC(evse_serial, DataBuffer);
            }

        }


    }



}
