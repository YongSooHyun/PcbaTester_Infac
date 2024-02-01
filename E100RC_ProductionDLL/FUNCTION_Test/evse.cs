using System;
using System.Text;
using System.Runtime.InteropServices;
using System.IO.Ports;
//using System.Windows.Forms;
//using System.Drawing;

namespace E100RC_Production
{


    //#include <QObject>
    //#include <QtMath>
    //#include <QDateTime>
    //#include <QElapsedTimer>
    //#include "powermdl.h"
    //#include "powercontrol.h"

    //Enum class For Message ID
    public enum MsgID : byte
{
    NONE_ID         = 0,
    SessionSetupReq = 0x02,
    SessionSetupRes = 0x82,
    ServiceDiscoveryReq = 0x03,
    ServiceDiscoveryRes = 0x83,
    ServicePaymentSelectionReq = 0x05,
    ServicePaymentSelectionRes = 0x85,
    ContractAuthenticationReq = 0x07,
    ContractAuthenticationRes = 0x87,
    ChargeParameterDiscoveryReq = 0x08,
    ChargeParameterDiscoveryRes = 0x88,
    ChargeParameterDiscovery2Req = 0x48,
    PowerDeliveryReq = 0x09,
    PowerDeliveryRes = 0x89,
    CableCheckReq = 0x0F,
    CableCheckRes = 0x8F,
    PreChargeReq = 0x10,
    PreChargeRes = 0x90,
    CurrentDemandReq = 0x11,
    CurrentDemandRes = 0x91,
    WeldingDetectionReq = 0x12,
    WeldingDetectionRes = 0x92,
    SessionStopReq = 0x0C,
    SessionStopRes = 0x8C,

    ReportINITReq = 0xF1,
    ReportINITRes = 0x71,
    ReportSLAC = 0x72,
    ReportSDP = 0x73,
    ReportV2G = 0x74,
    ReportSTATE = 0x75,
    ReportSTATEReq = 0xF5,

    // COMMAND
    StartRequest = 0xFC,

};

public enum responseCodeType : byte
{
    OK=0,
    OK_NewSessionEstablished=1,
    OK_OldSessionJoined=2,
    OK_CertificateExpiresSoon=3,
    FAILED=4,
    FAILED_SequenceError=5,
    FAILED_ServiceIDInvalid=6,
    FAILED_UnknownSession=7,
    FAILED_ServiceSelectionInvalid=8,
    FAILED_PaymentSelectionInvalid=9,
    FAILED_CertificateExpired=10,
    FAILED_SignatureError=11,
    FAILED_NoCertificateAvailable=12,
    FAILED_CertChainError=13,
    FAILED_ChallengeInvalid=14,
    FAILED_ContractCanceled=15,
    FAILED_WrongChargeParameter=16,
    FAILED_PowerDeliveryNotApplied=17,
    FAILED_TariffSelectionInvalid=18,
    FAILED_ChargingProfileInvalid=19,
    FAILED_EVSEPresentVoltageToLow=20,
    FAILED_MeteringSignatureNotValid=21,
    FAILED_WrongEnergyTransferType=22
}




public enum Error_Code : byte
{
    GQ_NO_ERROR = 0,
    GQ_INIT_ERROR_GENERAL = 0x10,
    GQ_INIT_ERROR_IFADDR = 0x11,
    GQ_INIT_ERROR_THREAD = 0x12,
    GQ_INIT_ERROR_OPENCHANNEL = 0x13,
    GQ_INIT_ERROR_KEY = 0x14,
    GQ_SLAC_ERROR_GENERAL = 0x20,
    GQ_SLAC_ERROR_TIMER_INIT = 0x21,
    GQ_SLAC_ERROR_TIMER_TIMEOUT = 0x22,
    GQ_SLAC_ERROR_TIMER_MISC = 0x23,
    GQ_SLAC_ERROR_PARAM_TIMEOUT = 0x24,
    GQ_SLAC_ERROR_PARAM_SOCKET = 0x25,
    GQ_SLAC_ERROR_START_ATTEN_CHAR_TIMEOUT = 0x26,
    GQ_SLAC_ERROR_MNBC_SOUND_TIMEOUT = 0x27,
    GQ_SLAC_ERROR_ATTEN_CHAR_TIMEOUT = 0x28,
    GQ_SLAC_ERROR_ATTEN_CHAR_SOCKET = 0x29,
    GQ_SLAC_ERROR_VALIDATE_1_TIMEOUT = 0x2a,
    GQ_SLAC_ERROR_VALIDATE_1_SOCKET = 0x2b,
    GQ_SLAC_ERROR_VALIDATE_2_TIMEOUT = 0x2c,
    GQ_SLAC_ERROR_VALIDATE_2_SOCKET = 0x2d,
    GQ_SLAC_ERROR_BCB_TOGGLE_TIMEOUT = 0x2e,
    GQ_SLAC_ERROR_MATCH_TIMEOUT = 0x2f,
    GQ_SLAC_ERROR_MATCH_SOCKET = 0x30,
    GQ_SLAC_ERROR_READ_SOCKET = 0x31
    //GQ_SLAC_ERROR_SET_KEY = 0x32,
}

public enum serviceCategoryType : byte
{
    EVCharging=0,
    Internet=1,
    ContractCertificate=2,
    OtherCustom_=3
}

public enum paymentOptionType : byte
{
    Contract=0,
    ExternalPayment=1
}

public enum EVSESupportedEnergyTransferType : byte
{
    AC_single_phase_core=0,
    AC_three_phase_core=1,
    DC_core=2,
    DC_extended=3,
    DC_combo_core=4,
    DC_dual=5,
    AC_core1p_DC_extended=6,
    AC_single_DC_core=7,
    AC_single_phase_three_phase_core_DC_extended=8,
    AC_core3p_DC_extended=9
}

public enum EVSEProcessingType : byte
{
    Finished=0,
    Ongoing=1
}


/*
enum EVRequestedEnergyTransferType
{
    AC_single_phase_core=0,
    AC_three_phase_core=1,
    DC_core=2,
    DC_extended=3,
    DC_combo=4,
    DC_unique=5
};
*/
public enum EVSENotificationType : byte
{
    None=0,
    StopCharging=1,
    ReNegotiation=2
}

public enum EVSEIsolationStatus : byte 
{
    Invalid=0,  //An isolation test has not been carried out.
    Valid=1,    //The isolation test has been carried out successfully
                //and did not result in an isolation warning or fault.
    Warning=2,  //The measured isolation resistance
                //is below the warning level defined in IEC 61851-23.
    Fault=2     //The measured isolation resistance is
                //below the fault level defined in IEC 61851-23.
}

public enum DC_EVSEStatusCodeType : byte
{
    EVSE_NotReady=0,
    EVSE_Ready=1,
    EVSE_Shutdown=2,                  // charger stop request
    EVSE_UtilityInterruptEvent=3,
    EVSE_IsolationMonitoringActive=4,
    EVSE_EmergencyShutdown=5,
    EVSE_Malfunction=6,
    Reserved_8=7,
    Reserved_9=8,
    Reserved_A=9,
    Reserved_B=10,
    Reserved_C=11
}
public enum unitSymbolType : byte
{
    h=0,
    m=1,
    s=2,
    A=3,
    Ah=4,
    V=5,
    VA=6,
    W=7,
    W_s=8,
    Wh=9
}

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class PhysicalValue
{
   public sbyte Multiplier = 0;
   public byte Unit = 0;
   public short Value = 0;
}


[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class UnitValue
{
    public string sValue;
    public string sUnit;
    public float fValue;
    public UnitValue(PhysicalValue pValue)
    {
        fValue =  pValue.Value * (float)Math.Pow(10, pValue.Multiplier);
        sUnit = getUnit(pValue.Unit);
        sValue = fValue.ToString() + sUnit;
    }


    public static string getUnit(int nUnit)
    {
        string sUnit_temp = "";
        switch (nUnit) {
            case 0 : sUnit_temp = "h"; break;
            case 1 : sUnit_temp = "m"; break;
            case 2 : sUnit_temp = "s"; break;
            case 3 : sUnit_temp = "A"; break;
            case 4 : sUnit_temp = "Ah"; break;
            case 5 : sUnit_temp = "V"; break;
            case 6 : sUnit_temp = "VA"; break;
            case 7 : sUnit_temp = "W"; break;
            case 8 : sUnit_temp = "W_s"; break;
            case 9 : sUnit_temp = "Wh"; break;

        }
        return sUnit_temp;
    }

}



[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class Msg_Header
{
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] chSign = new byte[2] { (byte)'R', (byte)'N' };        // ----------------------> Company Signiture
    public byte Msg_ID = 0;
    public byte Msg_LEN = 0;

    public Msg_Header()
    { 
    }

    public Msg_Header(byte[] data)
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

#region Message Format For ISO15118
//********************************************//
//*****    Message Format For ISO15118   *****//
//********************************************//

//SessionSetupReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stSessionSetupReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte EVCCIDLength = 0;
    public byte ProtocolSelection = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Padding = new byte[2] { 0, 0 };
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 8)]
    public byte[] EVCCID = new byte[8] {0,0,0,0,0,0,0,0};
    public uint Checksum = 0;

    /*
    public stSessionSetupReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }     
     */

    public stSessionSetupReq(byte[] data)
    {
        int pos = Marshal.SizeOf(typeof(Msg_Header));
        MsgHeader = new Msg_Header(data);
        EVCCIDLength = data[pos];
        pos += (1+Padding.Length);
        for(int cnt = 0;cnt<EVCCID.Length;cnt++)
        {
            EVCCID[cnt] = data[cnt + pos];
        }
        pos += (EVCCID.Length);
        Checksum = (uint)( data[pos] | (data[pos + 1] << 8) | (data[pos + 2] << 16) | (data[pos + 3] << 24));

    }


}

//SessionSetupRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stSessionSetupRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public uint DateTimeNow;
    public byte EVSEIDLength;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 32)]
    public byte[] EVSEID = new byte[32];
    public uint Checksum;


    public stSessionSetupRes( byte nResCode = 0)
    {
        ResCode = nResCode;
        init();
    }

    public stSessionSetupRes(byte[] a_chSign, byte nResCode = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        init();

    }

    public void init()
    {
        MsgHeader.Msg_ID = (byte)MsgID.SessionSetupRes;//0x82;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionSetupRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));

        //ResCode = nResCode;
        EVSEIDLength = 1;

        System.Array.Clear(Padding, 0, Padding.Length);
        System.Array.Clear(EVSEID, 0, EVSEID.Length);

        Checksum = 0;

        DateTime datetime = DateTime.UtcNow;
        DateTimeNow = ToDosDateTime(datetime);
    
    }



    public static UInt32 ToDosDateTime( DateTime dateTime)
    {
        DateTime startTime = new DateTime(1970, 1, 1, 0, 0, 0, 0);
        TimeSpan currTime = dateTime - startTime;
        UInt32 time_t = Convert.ToUInt32(Math.Abs(currTime.TotalSeconds));
        return time_t;
    }

}

//ServiceDiscoveryReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stServiceDiscoveryReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ServiceCategory = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public byte Checksum = 0;

    public stServiceDiscoveryReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

    public stServiceDiscoveryReq()
    { 
    }
}

//ServiceDiscoveryRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stServiceDiscoveryRes
{
    public Msg_Header MsgHeader = new Msg_Header();
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
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stServiceDiscoveryRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
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

    public stServiceDiscoveryRes(byte nResCode = 0)
    {
        ResCode = nResCode;
        init();
    }

    public stServiceDiscoveryRes(byte[] a_chSign, byte nResCode = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        init();
    }


};

//ServicePaymentSelectionReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stServicePaymentSelectionReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte SelectedPaymentOption = 0;
    public byte NumOfServices = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] ServiceID = new byte[4];
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Padding = new byte[2];
    public uint Checksum = 0;

    public stServicePaymentSelectionReq()
    { 
    }

    public stServicePaymentSelectionReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

//ServicePaymentSelectionRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stServicePaymentSelectionRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public uint Checksum;

    public void init()
    {
        MsgHeader.Msg_ID = (byte)MsgID.ServicePaymentSelectionRes;//0x85;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stServicePaymentSelectionRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        System.Array.Clear(Padding, 0, Padding.Length);
        Checksum = 0;
    }

    public stServicePaymentSelectionRes(byte nResCode = 0)
    {
        ResCode = nResCode;
        init();
    }

    public stServicePaymentSelectionRes(byte[] a_chSign, byte nResCode = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        init();
    }

}

//ContractAuthenticationReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stContractAuthenticationReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public uint Checksum = 0;

    public stContractAuthenticationReq()
    { 
    }
    public stContractAuthenticationReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
}

//ContractAuthenticationRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stContractAuthenticationRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    public byte EVSEProcessing;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Padding = new byte[2];
    public uint Checksum;

    public void init()
    {
        MsgHeader.Msg_ID = (byte)MsgID.ContractAuthenticationRes;//0x87;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stContractAuthenticationRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        System.Array.Clear(Padding, 0, Padding.Length);
        Checksum = 0;
    
    }

    public stContractAuthenticationRes(byte nResCode = 0, byte nEVSEProcessing = 0)
    {
        ResCode = nResCode;
        EVSEProcessing = nEVSEProcessing;
        init();
    }

    public stContractAuthenticationRes(byte[] a_chSign, byte nResCode = 0, byte nEVSEProcessing = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        EVSEProcessing = nEVSEProcessing;
        init();
    }


};

//ChargeParameterDiscoveryReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stChargeParameterDiscoveryReq
{
    public Msg_Header MsgHeader = new Msg_Header();
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

    public stChargeParameterDiscoveryReq()
    { 
    }
    public stChargeParameterDiscoveryReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

//ChargeParameterDiscoveryRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stChargeParameterDiscoveryRes
{
    public Msg_Header MsgHeader = new Msg_Header();
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
    public uint Checksum =0;

    public void init()
    {
        MsgHeader.Msg_ID = (byte)MsgID.ChargeParameterDiscoveryRes;//0x85;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stChargeParameterDiscoveryRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));

        EVSEProcessing = (byte)EVSEProcessingType.Finished;
        EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
    
    }

    public stChargeParameterDiscoveryRes(byte nResCode = 0, byte uEvseStatus = 0)
    {
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        init();
    }

    public stChargeParameterDiscoveryRes(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        init();
    }


};

//PowerDeliveryReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stPowerDeliveryReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ReadyToChargeState;                         //1 : Charging Start | 0 : Charging Stop
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public byte EVStatus_EVReady;                           //if value : 1 , EVready.
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
    public ushort SAScheduleTupleID;                        //ID chosen by EV
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


    public stPowerDeliveryReq()
    { }

    public stPowerDeliveryReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

//PowerDeliveryRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stPowerDeliveryRes
{
    public Msg_Header MsgHeader = new Msg_Header();
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
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPowerDeliveryRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        EVSEStatus_NotificationMaxDelay = 1;

        //memset(Padding,0, sizeof(Padding));
        System.Array.Clear(Padding, 0, Padding.Length);
    }

    public stPowerDeliveryRes(byte nResCode = 0, byte uEvseStatus = 0)
    {
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        init();
    }

    public stPowerDeliveryRes(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        init();
    }


};

//CableCheckReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stCableCheckReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte EVSEStatus_EVReady;                 //if value : 1 , EVready.
    public byte EVStatus_EVCabinConditioning;
    public byte EVStatus_EVRESSConditioning;
    public byte EVStatus_EVErrorCode;
    public byte EVStatus_EVRESSSOC;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public uint Checksum;

    public stCableCheckReq(byte[] data)
    {
        if (data.Length == 2)
        {
            MsgHeader = new Msg_Header(data);
        }
        else if (data.Length >= Marshal.SizeOf(typeof(stCableCheckReq)))
        {
            int pos = Marshal.SizeOf(typeof(Msg_Header));
            MsgHeader = new Msg_Header(data);
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

//CableCheckRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stCableCheckRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public byte EVSEProcessing;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Padding2 = new byte[4] {0,0,0,0};
    public byte EVSEStatus_EVSEIsolationStatus;
    public byte EVSEStatus_EVSEStatusCode;
    public byte EVSEStatus_EVSENotification;
    public byte Padding3 = 0;
    public uint EVSEStatus_NotificationMaxDelay;
    public uint Checksum = 0;

    public stCableCheckRes(byte nResCode, byte uEvseStatus)
    {
        MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheckRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEProcessing = (byte)EVSEProcessingType.Finished;
        EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        EVSEStatus_NotificationMaxDelay = 0;
    }
    public stCableCheckRes(byte[] a_chSign,byte nResCode, byte uEvseStatus)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheckRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEProcessing = (byte)EVSEProcessingType.Finished;
        EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        EVSEStatus_NotificationMaxDelay = 0;
    }

    public stCableCheckRes(byte[] a_chSign, byte nResCode, byte uEvseStatus, byte EVSEProcessing)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.CableCheckRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCableCheckRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        this.EVSEProcessing = EVSEProcessing;
        EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Valid;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
        EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
        EVSEStatus_NotificationMaxDelay = 0;
    }



};

//PreChargeReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stPreChargeReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte EVSEStatus_EVReady = 0;     //if value : 1 , EVready.
    public byte EVStatus_EVCabinConditioning = 0;
    public byte EVStatus_EVRESSConditioning = 0;
    public byte EVStatus_EVErrorCode = 0;
    public byte EVStatus_EVRESSSOC = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public PhysicalValue EVTargetVoltage = new PhysicalValue();
    public PhysicalValue EVTargetCurrent = new PhysicalValue();
    public uint Checksum = 0;
    public stPreChargeReq()
    { }
    public stPreChargeReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
};
//PreChargeRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stPreChargeRes
{
    public Msg_Header MsgHeader = new Msg_Header();
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

    public stPreChargeRes(byte nResCode = 0, byte uEvseStatus=0)
    {
        MsgHeader.Msg_ID = (byte)MsgID.PreChargeRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPreChargeRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }
    public stPreChargeRes(byte[] a_chSign, byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.PreChargeRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stPreChargeRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }


};

//CurrentDemandReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stCurrentDemandReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte EVSEStatus_EVReady = 0;     //if value : 1 , EVready.
    public byte EVStatus_EVCabinConditioning = 0;
    public byte EVStatus_EVRESSConditioning = 0;
    public byte EVStatus_EVErrorCode = 0;
    public byte EVStatus_EVRESSSOC = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public PhysicalValue EVTargetCurrent = new PhysicalValue();
    public PhysicalValue EVMaximumVoltageLimit = new PhysicalValue();
    public PhysicalValue EVMaximumCurrentLimit = new PhysicalValue();
    public PhysicalValue EVMaximumPowerLimit = new PhysicalValue();
    public byte BulkChargingComplete = 0;
    public byte ChargingComplete = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 2)]
    public byte[] Padding2 = new byte[2] {0,0};
    public PhysicalValue RemainingTimeToFullSoC = new PhysicalValue();
    public PhysicalValue RemainingTimeToBulkSoC = new PhysicalValue();
    public PhysicalValue EVTargetVoltage = new PhysicalValue();
    public uint Checksum = 0;
    public stCurrentDemandReq()
    { }
    public stCurrentDemandReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

//CurrentDemandRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stCurrentDemandRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
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

    public stCurrentDemandRes(byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader.Msg_ID = (byte)MsgID.CurrentDemandRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCurrentDemandRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }
    public stCurrentDemandRes(byte[] a_chSigh, byte nResCode, byte uEvseStatus)
    {
        MsgHeader = new Msg_Header(a_chSigh);
        MsgHeader.Msg_ID = (byte)MsgID.CurrentDemandRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stCurrentDemandRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }


};

//WeldingDetectionReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stWeldingDetectionReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte EVSEStatus_EVReady;     //if value : 1 , EVready.
    public byte EVStatus_EVCabinConditioning;
    public byte EVStatus_EVRESSConditioning;
    public byte EVStatus_EVErrorCode;
    public byte EVStatus_EVRESSSOC;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public uint Checksum;
    public stWeldingDetectionReq()
    { }
    public stWeldingDetectionReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
};

//WeldingDetectionRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stWeldingDetectionRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public byte EVSEStatus_EVSEIsolationStatus =(byte)EVSEIsolationStatus.Valid;
    public byte EVSEStatus_EVSEStatusCode = 0;
    public byte EVSEStatus_EVSENotification = (byte)EVSENotificationType.None;
    public byte Padding2 = 0;
    public uint EVSEStatus_NotificationMaxDelay = 0;
    public PhysicalValue EVSEPresentVoltage = new PhysicalValue();
    public uint Checksum = 0;

    public stWeldingDetectionRes(byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader.Msg_ID = (byte)MsgID.WeldingDetectionRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stWeldingDetectionRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }
    public stWeldingDetectionRes(byte[] a_chSign,byte nResCode = 0, byte uEvseStatus = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.WeldingDetectionRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stWeldingDetectionRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
        EVSEStatus_EVSEStatusCode = uEvseStatus;
    }

};


//SessionStopReq
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stSessionStopReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public uint Checksum;
    public stSessionStopReq()
    {}
    public stSessionStopReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
};

//SessionStopRes
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stSessionStopRes
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ResCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public uint Checksum =0; 

    public stSessionStopRes(byte nResCode = 0)
    {
        MsgHeader.Msg_ID = (byte)MsgID.SessionStopRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionStopRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
    }
    public stSessionStopRes(byte[] a_chSign,byte nResCode = 0)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.SessionStopRes;
        MsgHeader.Msg_LEN = (byte)(Marshal.SizeOf(typeof(stSessionStopRes)) - Marshal.SizeOf(typeof(Msg_Header)) - Marshal.SizeOf(typeof(uint)));
        ResCode = nResCode;
    }

};
#endregion Message Format For ISO15118

#region Message For Charging Start
//********************************************//
//*****    Message For Charging Start    *****//
//********************************************//
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportINITReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    //public byte[] info = new byte[4] { (byte)'I', (byte)'N', (byte)'I', (byte)'T' };
    public byte[] info = new byte[4] { 0, 0, 0, 0 };
    public uint Checksum;

    public stReportINITReq()
    {
        MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
        MsgHeader.Msg_LEN = 4;
    }
    public stReportINITReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
        MsgHeader.Msg_LEN = 4;
    }
    public stReportINITReq(byte[] a_chSign, byte[]info)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
        MsgHeader.Msg_LEN = 4;
        if (info.Length >= MsgHeader.Msg_LEN)
        {
            this.info[0] = info[0];
            this.info[1] = info[1];
            this.info[2] = info[2];
            this.info[3] = info[3];
        }
    }



};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportINIT
{
    public Msg_Header  MsgHeader = new Msg_Header();
    public byte ErrorCode = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Dummy = new byte[3];
    public byte MajorVersion = 0;
    public byte MinorVersion = 0;
    public byte ReleaseVersion = 0;
    public byte InfoLen= 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 24)]
    public byte[] INFO = new byte[24]; //"UM-REL-20150302-01"
    public uint Checksum = 0;

    public stReportINIT()
    {
        MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
    }
    public stReportINIT(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = (byte)MsgID.ReportINITReq;
    }
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stStartRequest
{
    public Msg_Header MsgHeader = new Msg_Header();
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Info = new byte[4] {0,0,0,0};
    public uint Checksum;

    public stStartRequest()
    {
        MsgHeader.Msg_ID = (byte)MsgID.StartRequest; //0xFC;
        MsgHeader.Msg_LEN = 4;
        Checksum = 0;
    }
    public stStartRequest(byte[] a_chSign,byte[] info)
    {
        MsgHeader = new Msg_Header(a_chSign);
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
public class stStartResponse
{
    public Msg_Header MsgHeader = new Msg_Header();
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] Info = new byte[4];
    public uint Checksum;

    public stStartResponse()
    { 
    }
    public stStartResponse(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportSLAC
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ErrorCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3];
    public byte AverageAttenuation;  //Stop when AverageAttenuation is greater than 40. It's noise value
    public byte Padding2;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 6)]
    public byte[] PEV_MAC = new byte[6];
    public uint Checksum = 0;

    public stReportSLAC()
    { 
    
    }
    public stReportSLAC(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportSDP
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ErrorCode = 0;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Padding = new byte[3] {0,0,0};
    public ushort Tcp_port = 0;
    public byte Sec = 0;
    public byte Tcp = 0;
    public uint Checksum = 0;

    public stReportSDP()
    { }
    public stReportSDP(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportV2G
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte ErrorCode;
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] Dummy = new byte[3];
    public uint Checksum;
    public stReportV2G()
    { }
    public stReportV2G(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }
};
/*
[StructLayout(LayoutKind.Sequential, Pack = 1)]
class stValidateReq
{
public Msg_Header MsgHeader = new Msg_Header();      //ID : 0x75
public byte ErrorCode = (byte)EVSENotificationType.None;
public byte Timer;           //0 – 100ms, 1- 200ms
[MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
public byte[] Padding = new byte[3];
public uint Checksum;

public stValidateReq()
{ }
public stValidateReq(byte[] a_chSign)
{
    MsgHeader = new Msg_Header(a_chSign);
}
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
class stValidateRes
{
public Msg_Header MsgHeader = new Msg_Header();      //ID : 0x75
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
public stValidateRes(byte[] a_chSigh, uint toggleNum)
{
    MsgHeader = new Msg_Header(a_chSign);
    MsgHeader.Msg_ID = 0xF5;
    MsgHeader.Msg_LEN = 4;
    ErrorCode = (byte)EVSENotificationType.None;
    ToggleNum = toggleNum;
}

};
*/

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportSTATEReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte type; //1 = CP State, 2 = PD State,
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] info = new byte[3];
    public uint Checksum;
    public stReportSTATEReq(byte nType)
    {
        MsgHeader.Msg_ID = (byte)MsgID.ReportSTATEReq;
        MsgHeader.Msg_LEN = 4;
        type = nType;
    }
    public stReportSTATEReq(byte[]a_chSigh, byte nType)
    {
        MsgHeader = new Msg_Header(a_chSigh);
        MsgHeader.Msg_ID = (byte)MsgID.ReportSTATEReq;
        MsgHeader.Msg_LEN = 4;
        type = nType;
    }
};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stReportSTATE
{
    public Msg_Header MsgHeader = new Msg_Header();
    public byte type;                                    //type = 1
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 3)]
    public byte[] info = new byte[3];                    //info[0] = cp voltage
    public uint Checksum;                                //type = 2
                                                         //info[0] = PD type (1,2)
                                                         //PDtype = 1, Info[1] = (0=UNPLUG, 1=S3CLOSE, 2=S3OPEN)
                                                         //PDtype = 2, Info[1] = (0=UNPLUG, 1=PLUG)

    public stReportSTATE()
    { }
    public stReportSTATE(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
    }

};

[StructLayout(LayoutKind.Sequential, Pack = 1)]
public class stAllStopReq
{
    public Msg_Header MsgHeader = new Msg_Header();
    [MarshalAs(UnmanagedType.ByValArray, SizeConst = 4)]
    public byte[] info = new byte[4] { (byte)'S', (byte)'T', (byte)'O', (byte)'P' };
    public uint Checksum;
    public stAllStopReq()
    {
        MsgHeader.Msg_ID = 0xFE;
        MsgHeader.Msg_LEN = 4;
        Checksum = 0;
    }

    public stAllStopReq(byte[] a_chSign)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = 0xFE;
        MsgHeader.Msg_LEN = 4;
        Checksum = 0;
    }

    public stAllStopReq(byte[] a_chSign,byte[] info)
    {
        MsgHeader = new Msg_Header(a_chSign);
        MsgHeader.Msg_ID = 0xFE;
        MsgHeader.Msg_LEN = 4;
        this.info[0] = info[0];
        this.info[1] = info[1];
        this.info[2] = info[2];
        this.info[3] = info[3];
        Checksum = 0;
    }


};
#endregion Message For Charging Start

/*
struct stChargerStart
{
    byte  start;          //start : 1 stop 2
    byte  ChargerType;    //0: not selected, 1: DC_combo type 2, 2: GB type
    byte[] Dummy = new byte[6];

    stChargerStart(byte nStart, byte nCType)
    {
        start = nStart;
        ChargerType = nCType;
    }
};
*/


enum start_req_info : byte
{
    CHARGER_MODE_ISO15118 = 0,
    CHARGER_MODE_CHADEMO = 1,
	CHARGER_MODE_J1772 = 2,
	CHARGER_MODE_TestEOL = 0x20,
	CHARGER_MODE_TestSWQA = 0x21,
}

enum chademo_start_req_info : byte
{
    CHARGER_MODE_CHADEMO = 0,
    CHARGER_MODE_CHADEMO_ADVANCED = 1,
    CHARGER_MODE_CHADEMO_CUSTOM = 2,
    CHARGER_MODE_CHADEMO_2 = 3,
    CHARGER_MODE_CHADEMO_V2H = 0x10,
}




public partial class dcComboPLC
{
    public int dbglevel = 3;

    public SerialPort evse_serial = new SerialPort();
    byte[] rcvData = new byte[1024];
    int rcvData_size = 0;

    public string status_text = "";

    public byte[] chSign = new byte[2] { (byte)'R', (byte)'N' }; //Company Check Sign
    public byte[] startReq_info = new byte[4] { 0, 0, 0, 0 };
    public byte[] reportInit_info = new byte[4] { 0, 0, 0, 0 };

    public bool g_bFast;

    public EVSEProcessingType eNumEVSEProcess = EVSEProcessingType.Finished;
    public responseCodeType eNumresponseCode = responseCodeType.OK;

    public short evTargetVoltage = 0;
    public short evTargetCurrent = 0;

    public bool bCurrent2Powerflag = false;
    public bool bCurrentReq = false;
    public bool bPLC_Connect = false;
    public bool bPLC_SessionOK = false;
    public bool bPLC_SessionRequest = false;
    public bool bChargingstopcontrol = false;

    public byte Plc_Voltage = 0;
    public bool bPLG = false;

    public bool bEmergencyStop = false;

    public DC_EVSEStatusCodeType evseStatusCode = 0;

    public byte EV_SOC = 0;

    public float imiuPlusResistor = 0;
    public float imiuMinusResistor = 0;

    public PhysicalValue MaxEVCurrentLimit = new PhysicalValue();
    public PhysicalValue MaxEVVoltageLimit = new PhysicalValue();
    public PhysicalValue MaxEVPowerLimit = new PhysicalValue();

    public PhysicalValue MaxEVSECurrentLimit = new PhysicalValue();
    public PhysicalValue MaxEVSEVoltageLimit = new PhysicalValue();
    public PhysicalValue MaxEVSEPowerLimit = new PhysicalValue();

    public PhysicalValue MinEVSECurrentLimit = new PhysicalValue();
    public PhysicalValue MinEVSEVoltageLimit = new PhysicalValue();

    public PhysicalValue EVSECurrent = new PhysicalValue();
    public PhysicalValue EVSEVoltage = new PhysicalValue();

    public PhysicalValue EVSEAvailableInputCurrentLimit_discharge = new PhysicalValue();
    public PhysicalValue EVSEAvailableInputVoltageLimit_discharge = new PhysicalValue();
    public PhysicalValue EVSEThresholdVoltage_discharge = new PhysicalValue();

    public byte ui_Min = 0;
    public byte ui_Sec = 0;

    public MsgID curChagerStep;

    public uint nPLC_CommCheck = 0;
    public uint nOldPLC_CommCheck = 0;
    public uint stop_charging_in_cd;
    public evse_resp_stop_condition evse_resp_stop_condition = new evse_resp_stop_condition();
    public evse_status_condition evse_status_condition = new evse_status_condition();

    public uint cableCheck_cnt = 0;
    public uint cableCheck_cnt_max = 3;

    ///////////////////////////////////////
    // for ChargeParameterDiscoveryRes
    ///////////////////////////////////////
    public PhysicalValue EVSECurrentRegulationTolerance = new PhysicalValue();
    public PhysicalValue EVSEEnergyToBeDelivered = new PhysicalValue();





    public dcComboPLC()
    {
        SetPhysicalValue(ref MinEVSECurrentLimit, 0, 3, 0);
        SetPhysicalValue(ref MinEVSEVoltageLimit, 0, 5, 100);

        SetPhysicalValue(ref MaxEVSEVoltageLimit, 0, 5, 750);
        SetPhysicalValue(ref MaxEVSECurrentLimit, 0, 3, 120);
        SetPhysicalValue(ref MaxEVSEPowerLimit, 2, 7, 70);

        initValues();
    }


    public void initValues()
    {

        bEmergencyStop = false;

        bPLC_Connect = false;
        bPLC_SessionOK = false;
        bCurrentReq = false;
        bChargingstopcontrol = false;
        bPLG = false;
        EV_SOC = 0;
        Plc_Voltage = 12;

        evTargetCurrent = evTargetVoltage = 0;
        evseStatusCode = DC_EVSEStatusCodeType.EVSE_Ready;
        g_bFast = false;
        stop_charging_in_cd = 0;
        curChagerStep = MsgID.NONE_ID;

    }

    public void DebugString(string str_Message)
    {
        string curtime = DateTime.Now.ToString("HH:mm:ss.fff ");
        str_Message = "[" + curtime + "]" + " - " + str_Message;
        Console.WriteLine(str_Message);
        status_text += str_Message;
    }

    public bool OpenSerialPort(string portname, int baudrate)
    {
        bool res = false;

        // Check Port is Open. Close the port if it is open
        if (evse_serial.IsOpen)
        {
            evse_serial.Close();
            return res;
        }
        // checked portname 
        bool isChecked = false;
        string[] ports = SerialPort.GetPortNames();
        foreach (string port in ports)
        {
            if (port.Contains(portname) == true)
                isChecked = true;
        }

        if (isChecked == false)
            return res;

        evse_serial.PortName = portname;
        evse_serial.BaudRate = baudrate;

        string str_temp = "Open EVSE Control Serial " + evse_serial.PortName + " : " + evse_serial.BaudRate + ", 8N1";
        Console.WriteLine(str_temp);
        status_text += str_temp;

        try
        {
            evse_serial.Open();
        }
        catch (Exception ex)
        {
            str_temp = "We cannot open Serial Port " + evse_serial.PortName + ".";
            Console.WriteLine(str_temp);
            System.Console.WriteLine(ex + "\r\n");
        }

        if (evse_serial.IsOpen == true)
        {
            res = true;
        }


        return res;
    }

    public bool CloseSerialPort()
    {
        bool res = false;

        // Check Port is Open. Close the port if it is open
        if (evse_serial.IsOpen)
        {
            evse_serial.Close();
            res = true;
        }
        return res;
    }


    public void readPLC_COM()
    {
        byte[] rxData = new byte[1024];
        int length = 0;
        int len_head_tail = 0;
        int size = 0;
        //int recvSize = 0;
        //QString curtime3 = QDateTime::currentDateTime().toString("hh:mm:ss.zzz ");
        size = evse_serial.Read(rxData, 0, rxData.Length);
        Array.Copy(rxData, 0, rcvData, rcvData_size, size);
        rcvData_size += size;

        Console.WriteLine("rcvData size =" + size);
        for (int cnt = 0; cnt < size; cnt++)
        {
            Console.Write(rcvData[cnt].ToString("x02") + " ");
        }
        Console.WriteLine();

        while (rcvData_size > 4)
        {
            if (rcvData[0] == 'G' && rcvData[1] == 'Q')
            {
                if (chSign[0] == 'Q' && chSign[1] == 'A')
                {
                    length = rcvData[6] + (rcvData[7] << 8);
                    len_head_tail = 12;
                    if (rcvData_size >= length + len_head_tail)
                    {
                        byte[] data = new byte[length + len_head_tail];
                        Buffer.BlockCopy(rcvData, 0, data, 0, length + len_head_tail);
                        recieveData2(data);
                        for (int cnt = 0; cnt < (rcvData.Length - (length + len_head_tail)); cnt++)
                        {
                            rcvData[cnt] = rcvData[length + len_head_tail + cnt];
                        }
                        rcvData_size = rcvData_size - (length + len_head_tail);
                    }
                    else
                        break;
                }
                else if (startReq_info[0] == (short)start_req_info.CHARGER_MODE_ISO15118)
                {
                    Console.Write("V2G Charging Sequence Start\n");
                    length = rcvData[3];
                    len_head_tail = 8;

                    if (rcvData_size >= length + len_head_tail)
                    {
                        byte[] data = new byte[length + len_head_tail];
                        Buffer.BlockCopy(rcvData, 0, data, 0, length + len_head_tail);

                        recieveData(data);

                        for (int cnt = 0; cnt < (rcvData.Length - (length + len_head_tail)); cnt++)
                        {
                            rcvData[cnt] = rcvData[length + len_head_tail + cnt];
                        }
                        rcvData_size = rcvData_size - (length + len_head_tail);
                    }
                    else
                        break;
                }
            }
            else
            {
                for (int cnt = 0; cnt < rcvData.Length - 1; cnt++)
                {
                    rcvData[cnt] = rcvData[cnt + 1];
                }
                rcvData[rcvData.Length - 1] = 0;
                rcvData_size = rcvData_size - 1;
            }
        }
    }

    public unsafe void recieveData(byte[] aData)
    {
        string _msg_name;
        byte[] DataBuffer = new byte[1];
        string str_temp = "recieveData Cmd 0x" + Convert.ToUInt32(aData[2]).ToString("x02");
        DebugString(str_temp + "\n");

        if (dbglevel >= 5)
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
                    stReportINIT pReportINIT = new stReportINIT(chSign);// = NULL;
                    fixed (byte* pData = aData)
                    {
                        pReportINIT = (stReportINIT)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportINIT));
                    }

                    string strInfo = Encoding.Default.GetString(pReportINIT.INFO);

                    Console.WriteLine(strInfo);

                    if (pReportINIT.ErrorCode == 0)
                    {
                        bPLC_Connect = true;
                        bCurrentReq = false;

                        stStartRequest pStartReq = new stStartRequest(chSign, startReq_info);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stStartRequest))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
                        }
                        pStartReq.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
                        }
                        DebugString("receive stStartRequest" + "\n");
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

            case MsgID.SessionSetupReq:     // 0x02
                {
                    _msg_name = " <-- SessionSetupReq ( 0x02 )";
                    DebugString(_msg_name + "\n");
                    //DebugString("SessionSetupReq data:\n");

                    curChagerStep = MsgID.SessionSetupReq;
                    bPLC_SessionRequest = true;

                    stSessionSetupReq pSessionSetupReq = new stSessionSetupReq(aData);

                    string HexID = "EVCCID =";

                    for (int i = 0; i < 8; i++)
                    {
                        HexID += pSessionSetupReq.EVCCID[i].ToString("x02") + " ";
                    }
                    DebugString(HexID + "\n");


                    responseCodeType ResponseCode = responseCodeType.OK_NewSessionEstablished;
                    if (evse_resp_stop_condition.checkStop_sessionSetup())
                        ResponseCode = evse_resp_stop_condition.sessionSetupResCode;

                    stSessionSetupRes pSessionSetupRes = new stSessionSetupRes(chSign, (byte)ResponseCode);
                    DataBuffer = new byte[Marshal.SizeOf(typeof(stSessionSetupRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pSessionSetupRes, (IntPtr)pData, false);
                    }

                    pSessionSetupRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pSessionSetupRes, (IntPtr)pData, false);
                    }

                    //DebugString("SessionSetupRes data:\n");
                    _msg_name = " --> SessionSetupRes (0x82)";
                    DebugString(_msg_name + "\n");
                    str_temp = ("       ResponseCode=" + pSessionSetupRes.ResCode);
                    DebugString(str_temp + "\n");
                    
                    break;
                }

            case MsgID.ServiceDiscoveryReq:     // 0x03
                {
                    _msg_name = " <-- ServiceDiscoveryReq (0x03)";
                    DebugString(_msg_name + "\n");
                    //_msg_name = " ServiceDiscoveryReq data:";
                    //DebugString(_msg_name + "\n");

                    curChagerStep = MsgID.ServiceDiscoveryReq;
                    bPLC_SessionRequest = false;

                    stServiceDiscoveryReq pServiceDiscoveryReq = new stServiceDiscoveryReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pServiceDiscoveryReq = (stServiceDiscoveryReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stServiceDiscoveryReq));
                    }

                    //DebugString("ServiceDiscoveryRes data:" + "\n");
                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_serviceDiscovery())
                        ResponseCode = evse_resp_stop_condition.serviceDiscoveryResCode;

                    if ((pServiceDiscoveryReq.ServiceCategory == 0)
                        || (ResponseCode >= responseCodeType.FAILED))
                    {
                        if (pServiceDiscoveryReq.ServiceCategory == 0)
                        {
                            //str_temp = ("    --- ServiceCategory 0");
                            //DebugString(str_temp + "\n");
                        }

                        stServiceDiscoveryRes pServiceDiscoveryRes = new stServiceDiscoveryRes(chSign, (byte)ResponseCode);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stServiceDiscoveryRes))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pServiceDiscoveryRes, (IntPtr)pData, false);
                        }

                        pServiceDiscoveryRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pServiceDiscoveryRes, (IntPtr)pData, false);
                        }

                        _msg_name = " --> ServiceDiscoveryRes (0x83)";
                        DebugString(_msg_name + "\n");
                    }
                    break;
                }

            case MsgID.ServicePaymentSelectionReq:
                {
                    DebugString(" <-- ServicePaymentSelectionReq (0x05)" + "\n");
                    //_msg_name = " ServicePaymentSelectionReq data:";
                    //DebugString(_msg_name + "\n");
                    curChagerStep = MsgID.ServicePaymentSelectionReq;
                    stServicePaymentSelectionReq pServicePaymentSelectionReq = new stServicePaymentSelectionReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pServicePaymentSelectionReq = (stServicePaymentSelectionReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stServicePaymentSelectionReq));
                    }

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_servicePaymentSelection())
                        ResponseCode = evse_resp_stop_condition.servicePaymentSelectionResCode;

                    if (pServicePaymentSelectionReq.SelectedPaymentOption == 1 &&
                       pServicePaymentSelectionReq.NumOfServices == 1 &&
                       pServicePaymentSelectionReq.ServiceID[0] == 1)
                    {

                        stServicePaymentSelectionRes pServicePaymentSelectionRes = new stServicePaymentSelectionRes(chSign, (byte)ResponseCode);
                        DataBuffer = new byte[Marshal.SizeOf(typeof(stServicePaymentSelectionRes))];
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pServicePaymentSelectionRes, (IntPtr)pData, false);
                        }
                        pServicePaymentSelectionRes.Checksum = getChecksum(DataBuffer);
                        fixed (byte* pData = DataBuffer)
                        {
                            Marshal.StructureToPtr(pServicePaymentSelectionRes, (IntPtr)pData, false);
                        }

                        str_temp = (" --> ServicePaymentSelectionRes (0x85) ");
                        DebugString(str_temp + "\n");

                    }

                    break;
                }

            case MsgID.ContractAuthenticationReq:
                {
                    curChagerStep = MsgID.ContractAuthenticationReq;
                    str_temp = (" <-- ContractAuthenticationReq (0x07) ");
                    DebugString(str_temp + "\n");

                    eNumEVSEProcess = EVSEProcessingType.Finished;

                    if (evse_resp_stop_condition.checkStop_contractAuthentication())
                        eNumresponseCode = evse_resp_stop_condition.contractAuthenticationResCode;

                    stContractAuthenticationRes pContractAuthenticationRes = new stContractAuthenticationRes(chSign, (byte)eNumresponseCode, (byte)eNumEVSEProcess);
                    DataBuffer = new byte[Marshal.SizeOf(typeof(stContractAuthenticationRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pContractAuthenticationRes, (IntPtr)pData, false);
                    }
                    pContractAuthenticationRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pContractAuthenticationRes, (IntPtr)pData, false);
                    }

                    str_temp = (" --> ContractAuthenticationRes (0x87) ");
                    DebugString(str_temp + "\n");

                    break;
                }

            case MsgID.ChargeParameterDiscoveryReq:
                {
                    curChagerStep = MsgID.ChargeParameterDiscoveryReq;
                    nPLC_CommCheck++;
                    str_temp = (" <-- ChargeParameterDiscoveryReq (0x08) ");
                    DebugString(str_temp + "\n");

                    stChargeParameterDiscoveryReq pChargeParameterDiscoveryReq = new stChargeParameterDiscoveryReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pChargeParameterDiscoveryReq = (stChargeParameterDiscoveryReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stChargeParameterDiscoveryReq));
                    }

                    //DebugString("ChargeParameterDiscoveryReq data: \n");
                    //str_temp = ("       EVStatus_EVErrorCode = " + pChargeParameterDiscoveryReq.EVStatus_EVErrorCode);
                    //DebugString(str_temp + "\n");



                    UnitValue uvMaxCurrentLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumCurrentLimit);
                    UnitValue uvPowerLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumPowerLimit);
                    UnitValue uvMaxEVMaxVoltageLimit = new UnitValue(pChargeParameterDiscoveryReq.EVMaximumVoltageLimit);
                    UnitValue uvMaxEnergyCapacity = new UnitValue(pChargeParameterDiscoveryReq.EVEnergyCapacity);
                    UnitValue uvMaxEnergyRequest = new UnitValue(pChargeParameterDiscoveryReq.EVEnergyRequest);

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_chargeParameterDiscovery())
                        ResponseCode = evse_resp_stop_condition.chargeParameterDiscoveryResCode;


                    stChargeParameterDiscoveryRes pChargeParameterDiscoveryRes = new stChargeParameterDiscoveryRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);

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

                    DebugString("ChargeParameterDiscoveryRes data:" + "\n");
                    DebugString("       EVSEMaximumCurrentLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumCurrentLimit.Value + " A" + "\n");
                    DebugString("       EVSEMaximumVoltageLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumVoltageLimit.Value + " V" + "\n");
                    DebugString("       EVSEMaximumPowerLimit = " + pChargeParameterDiscoveryRes.EVSEMaximumPowerLimit.Value / 10 + "."
                                                            + pChargeParameterDiscoveryRes.EVSEMaximumPowerLimit.Value % 10 + " kW" + "\n");
                    DebugString("       EVSEMinimumCurrentLimit = " + pChargeParameterDiscoveryRes.EVSEMinimumCurrentLimit.Value + " A" + "\n");
                    DebugString("       EVSEMinimumVoltageLimit = " + pChargeParameterDiscoveryRes.EVSEMinimumVoltageLimit.Value + " V" + "\n");


                    //PhysicalValue EVSECurrentRegulationTolerance = new PhysicalValue();
                    PhysicalValue EVSEPeakCurrentRipple = new PhysicalValue();
                    //PhysicalValue EVSEEnergyToBeDelivered = new PhysicalValue();


                    //SetPhysicalValue(ref EVSECurrentRegulationTolerance, 0, 3, 1);  // btn_evse_start_Click()함수에서 setting
                    SetPhysicalValue(ref EVSEPeakCurrentRipple, 0, 3, 1);
                    //SetPhysicalValue(ref EVSEEnergyToBeDelivered, 0, 9, 25000);  // btn_evse_start_Click()함수에서 setting
                    pChargeParameterDiscoveryRes.EVSECurrentRegulationTolerance = this.EVSECurrentRegulationTolerance;
                    pChargeParameterDiscoveryRes.EVSEPeakCurrentRipple = EVSEPeakCurrentRipple;
                    pChargeParameterDiscoveryRes.EVSEEnergyToBeDelivered = this.EVSEEnergyToBeDelivered;
                    pChargeParameterDiscoveryRes.SAScheduleTuple0_PMaxSchedule0_Pmax = 20000;

                    DataBuffer = new byte[Marshal.SizeOf(typeof(stChargeParameterDiscoveryRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pChargeParameterDiscoveryRes, (IntPtr)pData, false);
                    }
                    pChargeParameterDiscoveryRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pChargeParameterDiscoveryRes, (IntPtr)pData, false);
                    }

                    str_temp = (" --> ChargeParameterDiscoveryRes (0x88) ");
                    DebugString(str_temp + "\n");

                    break;
                }
            case MsgID.PowerDeliveryReq:
                {
                    curChagerStep = MsgID.PowerDeliveryReq;
                    nPLC_CommCheck++;
                    str_temp = (" <-- PowerDeliveryReq (0x09) ");
                    DebugString(str_temp + "\n");

                    stPowerDeliveryReq pPowerDeliveryReq = new stPowerDeliveryReq(chSign);
                    //pPowerDeliveryReq = reinterpret_cast<stPowerDeliveryReq*>(aData.data());
                    fixed (byte* pData = aData)
                    {
                        pPowerDeliveryReq = (stPowerDeliveryReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stPowerDeliveryReq));
                    }

                    //DebugString("PowerDeliveryReq data: \n");
                    EV_SOC = pPowerDeliveryReq.EVStatus_EVRESSSOC;
                    //str_temp = ("       EV_SOC = " + EV_SOC);
                    //DebugString(str_temp + "\n");

                    //str_temp = ("       EVStatus_EVErrorCode = " + pPowerDeliveryReq.EVStatus_EVErrorCode);
                    //DebugString(str_temp + "\n");

                    if (pPowerDeliveryReq.ReadyToChargeState == 0)
                    {
                        bCurrentReq = false;
                    }

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_powerDelivery())
                        ResponseCode = evse_resp_stop_condition.powerDeliveryResCode;

                    stPowerDeliveryRes pPowerDeliveryRes = new stPowerDeliveryRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);
                    //DataBuffer.append((char*)pPowerDeliveryRes, sizeof(stPowerDeliveryRes));

                    if (evse_status_condition.checkStop_powerDelivery())
                    {
                        pPowerDeliveryRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.powerDelivery_evse_status.EVSEIsolationStatus;
                        pPowerDeliveryRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.powerDelivery_evse_status.EVSEStatusCode;
                        pPowerDeliveryRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.powerDelivery_evse_status.EVSENotification;
                        pPowerDeliveryRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.powerDelivery_evse_status.NotificationMaxDelay;
                    }


                    DataBuffer = new byte[Marshal.SizeOf(typeof(stPowerDeliveryRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pPowerDeliveryRes, (IntPtr)pData, false);
                    }
                    pPowerDeliveryRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pPowerDeliveryRes, (IntPtr)pData, false);
                    }

                    _msg_name = " --> PowerDeliveryRes (0x89) ";
                    DebugString(_msg_name + "\n");
                    break;
                }

            case MsgID.CableCheckReq:  // 0x0F
                {
                    curChagerStep = MsgID.CableCheckReq;
                    nPLC_CommCheck++;
                    cableCheck_cnt++;
                    str_temp = (" <-- CableCheckReq (0x0F) ");
                    DebugString(str_temp + "\n");

                    stCableCheckReq pCableCheckReq = new stCableCheckReq(aData);

                    //DebugString("CableCheckReq data: \n");
                    //str_temp = ("       EVStatus_EVErrorCode = " + pCableCheckReq.EVStatus_EVErrorCode);
                    //DebugString(str_temp + "\n");

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_cableCheck())
                        ResponseCode = evse_resp_stop_condition.cableCheckResCode;

                    stCableCheckRes pCableCheckRes;
                    if (cableCheck_cnt >= cableCheck_cnt_max)
                    {
                        pCableCheckRes = new stCableCheckRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);
                    }
                    else
                    {
                        pCableCheckRes = new stCableCheckRes(chSign, (byte)ResponseCode, (byte)evseStatusCode, (byte)EVSEProcessingType.Ongoing);
                    }

                    pCableCheckRes.EVSEStatus_EVSEIsolationStatus = (byte)v2gisolationLevelType.v2gisolationLevelType_Valid;
                    //DataBuffer.append((char*)pCableCheckRes, sizeof(stCableCheckRes));
                    if (evse_status_condition.checkStop_cableCheck())
                    {
                        pCableCheckRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.cableCheck_evse_status.EVSEIsolationStatus;
                        pCableCheckRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.cableCheck_evse_status.EVSEStatusCode;
                        pCableCheckRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.cableCheck_evse_status.EVSENotification;
                        pCableCheckRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.cableCheck_evse_status.NotificationMaxDelay;
                    }

                    DataBuffer = new byte[Marshal.SizeOf(typeof(stCableCheckRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pCableCheckRes, (IntPtr)pData, false);
                    }
                    pCableCheckRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pCableCheckRes, (IntPtr)pData, false);
                    }

                    //DebugString("CableCheckRes data:" + "\n");
                    _msg_name = " --> CableCheckRes (0x8F) ";
                    DebugString(_msg_name + "\n");
                    break;
                }
            case MsgID.PreChargeReq:
                {
                    curChagerStep = MsgID.PreChargeReq;
                    nPLC_CommCheck++;
                    bCurrentReq = true;

                    _msg_name = " <-- preChargeReq (0x10) ";
                    DebugString(_msg_name + "\n");
                    //DebugString("preChargeReq data:" + "\n");
                    stPreChargeReq pPreChargeReq = new stPreChargeReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pPreChargeReq = (stPreChargeReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stPreChargeReq));
                    }

                    UnitValue unitTargetVoltage = new UnitValue(pPreChargeReq.EVTargetVoltage);
                    UnitValue unitTargetCurrent = new UnitValue(pPreChargeReq.EVTargetCurrent);

                    evTargetVoltage = (short)unitTargetVoltage.fValue;

                    str_temp = ("       preCharge Target Cur " + unitTargetCurrent.fValue * 1000 + " A");
                    DebugString(str_temp + "\n");
                    if (evseStatusCode == DC_EVSEStatusCodeType.EVSE_EmergencyShutdown)
                    {
                        evTargetVoltage = 0;
                    }

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_precharge())
                        ResponseCode = evse_resp_stop_condition.prechargeResCode;

                    DebugString("preChargeRes data:" + "\n");
                    stPreChargeRes pPreChargeRes = new stPreChargeRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);

                    //str_temp = ("       EVStatus_EVRESSSOC =" + pPreChargeReq.EVStatus_EVRESSSOC);
                    //DebugString(str_temp + "\n");

                    //str_temp = ("       EVStatus_EVErrorCode =" + pPreChargeReq.EVStatus_EVErrorCode);
                    //DebugString(str_temp + "\n");

                    //EVSEVoltage = pPreChargeReq.EVTargetVoltage;
                    pPreChargeRes.EVSEPresentVoltage = EVSEVoltage;

                    str_temp = ("       EVSEVoltage =" + EVSEVoltage.Value + "V");
                    DebugString(str_temp + "\n");

                    if (evse_status_condition.checkStop_precharge())
                    {
                        pPreChargeRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.precharge_evse_status.EVSEIsolationStatus;
                        pPreChargeRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.precharge_evse_status.EVSEStatusCode;
                        pPreChargeRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.precharge_evse_status.EVSENotification;
                        pPreChargeRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.precharge_evse_status.NotificationMaxDelay;
                    }



                    //DataBuffer.append((char*)pPreChargeRes, sizeof(stPreChargeRes));
                    DataBuffer = new byte[Marshal.SizeOf(typeof(stPreChargeRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pPreChargeRes, (IntPtr)pData, false);
                    }
                    pPreChargeRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pPreChargeRes, (IntPtr)pData, false);
                    }

                    _msg_name = " --> preChargeRes (0x90) ";
                    DebugString(_msg_name + "\n");
                    break;
                }
            case MsgID.CurrentDemandReq:
                {
                    _msg_name = " <-- CurrentDemandReq (0x11) ";
                    DebugString(_msg_name + "\n");
                    DebugString("CurrentDemandReq data:" + "\n");

                    curChagerStep = MsgID.CurrentDemandReq;
                    nPLC_CommCheck++;
                    bCurrentReq = true;
                    stop_charging_in_cd++;

                    stCurrentDemandReq pCurrentDemandReq = new stCurrentDemandReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pCurrentDemandReq = (stCurrentDemandReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stCurrentDemandReq));
                    }

                    EV_SOC = pCurrentDemandReq.EVStatus_EVRESSSOC;
                    str_temp = ("        EVStatus_EVErrorCode = " + pCurrentDemandReq.EVStatus_EVErrorCode);
                    DebugString(str_temp + "\n");
                    str_temp = ("        EVSEStatus_EVRESSSOC = " + EV_SOC);
                    DebugString(str_temp + "\n");


                    UnitValue uvEVTargetCurrent = new UnitValue(pCurrentDemandReq.EVTargetCurrent);
                    UnitValue uvEVTargetVoltage = new UnitValue(pCurrentDemandReq.EVTargetVoltage);
                    UnitValue uvEVMaximumCurrentLimit = new UnitValue(pCurrentDemandReq.EVMaximumCurrentLimit);
                    UnitValue uvEVMaximumVoltageLimit = new UnitValue(pCurrentDemandReq.EVMaximumVoltageLimit);
                    UnitValue uvEVMaximumPowerLimit = new UnitValue(pCurrentDemandReq.EVMaximumPowerLimit);
                    UnitValue uvRemainingTimeToFullSoC = new UnitValue(pCurrentDemandReq.RemainingTimeToFullSoC);

                    ui_Min = (byte)((uint)uvRemainingTimeToFullSoC.fValue / 60);
                    ui_Sec = (byte)((uint)uvRemainingTimeToFullSoC.fValue % 60);
                    str_temp = ("       Remaining Time: " + ui_Min + ":" + ui_Sec);
                    DebugString(str_temp + "\n");


                    str_temp = ("       EVTargetCurrent = " + uvEVTargetCurrent.sValue);
                    DebugString(str_temp + "\n");
                    str_temp = ("       uvEVTargetVoltage = " + uvEVTargetVoltage.sValue);
                    DebugString(str_temp + "\n");

                    str_temp = ("       uvEVMaximumCurrentLimit = " + uvEVMaximumCurrentLimit.sValue);
                    DebugString(str_temp + "\n");
                    str_temp = ("       uvEVMaximumVoltageLimit = " + uvEVMaximumVoltageLimit.sValue);
                    DebugString(str_temp + "\n");
                    str_temp = ("       uvEVMaximumPowerLimit = " + uvEVMaximumPowerLimit.sValue);
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


                    stCurrentDemandRes pCurrentDemandRes = new stCurrentDemandRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);

                    if (stop_charging_in_cd >= 30)
                    {
                        pCurrentDemandRes.EVSEStatus_EVSEStatusCode = 2;
                    }

                    pCurrentDemandRes.EVSECurrentLimitAchieved = 0;
                    pCurrentDemandRes.EVSEVoltageAchieved = 0;
                    pCurrentDemandRes.EVSEPowerLimitAchieved = 0;

                    pCurrentDemandRes.EVSEMaximumCurrentLimit = MaxEVSECurrentLimit;
                    pCurrentDemandRes.EVSEMaximumPowerLimit = MaxEVSEPowerLimit;
                    pCurrentDemandRes.EVSEMaximumVoltageLimit = MaxEVSEVoltageLimit;

                    pCurrentDemandRes.EVSEPresentCurrent = EVSECurrent;
                    pCurrentDemandRes.EVSEPresentVoltage = EVSEVoltage;

                    DebugString("CurrentDemandRes data:" + "\n");
                    str_temp = ("       MaxEVSECurrentLimit = " + MaxEVSECurrentLimit.Value + " A");
                    DebugString(str_temp + "\n");

                    str_temp = ("       MaxEVSEPowerLimit = " + MaxEVSEPowerLimit.Value + " W");
                    DebugString(str_temp + "\n");

                    str_temp = ("       MaxEVSEVoltageLimit = " + MaxEVSEVoltageLimit.Value + " V");
                    DebugString(str_temp + "\n");

                    str_temp = ("       EVSECurrent = " + EVSECurrent.Value + " A");
                    DebugString(str_temp + "\n");

                    str_temp = ("       EVSEVoltage = " + EVSEVoltage.Value + "V");
                    DebugString(str_temp + "\n");

                    if (evse_status_condition.checkStop_currentDemand())
                    {
                        pCurrentDemandRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.currentDemand_evse_status.EVSEIsolationStatus;
                        pCurrentDemandRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.currentDemand_evse_status.EVSEStatusCode;
                        pCurrentDemandRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.currentDemand_evse_status.EVSENotification;
                        pCurrentDemandRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.currentDemand_evse_status.NotificationMaxDelay;
                    }


                    DataBuffer = new byte[Marshal.SizeOf(typeof(stCurrentDemandRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pCurrentDemandRes, (IntPtr)pData, false);
                    }
                    pCurrentDemandRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pCurrentDemandRes, (IntPtr)pData, false);
                    }

                    _msg_name = " --> CurrentDemandRes (0x91) ";
                    DebugString(_msg_name + "\n");
                    break;
                }

            case MsgID.WeldingDetectionReq:
                {
                    curChagerStep = MsgID.WeldingDetectionReq;
                    nPLC_CommCheck++;

                    bCurrentReq = false;

                    str_temp = (" <-- WeldingDetectionReq  ");
                    DebugString(str_temp + "\n");
                    DebugString("WeldingDetectionReq data:" + "\n");

                    stWeldingDetectionReq pWeldingDetectionReq = new stWeldingDetectionReq(chSign);
                    fixed (byte* pData = aData)
                    {
                        pWeldingDetectionReq = (stWeldingDetectionReq)Marshal.PtrToStructure((IntPtr)pData, typeof(stWeldingDetectionReq));
                    }
                    str_temp = ("       EVSEStatus_EVReady " + pWeldingDetectionReq.EVSEStatus_EVReady);
                    DebugString(str_temp + "\n");
                    str_temp = ("       EVRESSSOC " + pWeldingDetectionReq.EVStatus_EVRESSSOC);
                    DebugString(str_temp + "\n");
                    str_temp = ("       EVStatus_EVErrorCode = " + pWeldingDetectionReq.EVStatus_EVErrorCode);
                    DebugString(str_temp + "\n");

                    responseCodeType ResponseCode = responseCodeType.OK;
                    if (evse_resp_stop_condition.checkStop_weldingDetection())
                        ResponseCode = evse_resp_stop_condition.weldingDetectionResCode;

                    stWeldingDetectionRes pWeldingDetectionRes = new stWeldingDetectionRes(chSign, (byte)ResponseCode, (byte)evseStatusCode);
                    if (imiuMinusResistor <= 100.0f || imiuPlusResistor <= 100.0f)
                    {
                        pWeldingDetectionRes.EVSEStatus_EVSEIsolationStatus = (byte)EVSEIsolationStatus.Fault;
                    }


                    PhysicalValue EVSEVoltage_Welding = new PhysicalValue();

                    pWeldingDetectionRes.EVSEPresentVoltage = EVSEVoltage_Welding;

                    DebugString("WeldingDetectionRes data:" + "\n");

                    str_temp = ("       EVSEVoltage_Welding : " + EVSEVoltage_Welding);
                    DebugString(str_temp + "\n");

                    if (evse_status_condition.checkStop_weldingDetection())
                    {
                        pWeldingDetectionRes.EVSEStatus_EVSEIsolationStatus = (byte)evse_status_condition.weldingDetection_evse_status.EVSEIsolationStatus;
                        pWeldingDetectionRes.EVSEStatus_EVSEStatusCode = (byte)evse_status_condition.weldingDetection_evse_status.EVSEStatusCode;
                        pWeldingDetectionRes.EVSEStatus_EVSENotification = (byte)evse_status_condition.weldingDetection_evse_status.EVSENotification;
                        pWeldingDetectionRes.EVSEStatus_NotificationMaxDelay = evse_status_condition.weldingDetection_evse_status.NotificationMaxDelay;
                    }


                    DataBuffer = new byte[Marshal.SizeOf(typeof(stWeldingDetectionRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pWeldingDetectionRes, (IntPtr)pData, false);
                    }
                    pWeldingDetectionRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pWeldingDetectionRes, (IntPtr)pData, false);
                    }

                    _msg_name = " --> WeldingDetectionRes ";
                    DebugString(_msg_name + "\n");
                    break;
                }


            case MsgID.SessionStopReq:
                {
                    curChagerStep = MsgID.SessionStopReq;
                    stop_charging_in_cd = 0;
                    str_temp = (" <-- SessionStopReq (0x0C) ");
                    DebugString(str_temp + "\n");

                    stSessionStopRes pSessionStopRes = new stSessionStopRes(chSign, 0);
                    //DataBuffer.append((char*)pSessionStopRes, sizeof(stSessionStopRes));


                    DataBuffer = new byte[Marshal.SizeOf(typeof(stSessionStopRes))];
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pSessionStopRes, (IntPtr)pData, false);
                    }
                    pSessionStopRes.Checksum = getChecksum(DataBuffer);
                    fixed (byte* pData = DataBuffer)
                    {
                        Marshal.StructureToPtr(pSessionStopRes, (IntPtr)pData, false);
                    }
                    str_temp = (" --> SessionStopRes (0x8C) ");
                    DebugString(str_temp + "\n");

                    bPLC_SessionOK = false;
                    bCurrentReq = false;
                    //emit sgnSessionStop(g_Group);
                    break;
                }

            case MsgID.ReportSLAC:
                {
                    DebugString("*** ReportSLAC ***\n");
                    stReportSLAC pReportSLAC = new stReportSLAC(chSign);
                    fixed (byte* pData = aData)
                    {
                        pReportSLAC = (stReportSLAC)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSLAC));
                    }
                    string sDebug = "[SLAC]AverageAttenuation - " + pReportSLAC.AverageAttenuation;
                    DebugString(sDebug + "\n");
                    string str_HexID = "";
                    for (int i = 0; i < 6; i++)
                    {
                        str_HexID += pReportSLAC.PEV_MAC[i].ToString("x02") + " ";
                    }
                    str_HexID = "PEV_MAC: " + str_HexID;
                    DebugString(str_HexID + "\n");
                    break;
                }

            case MsgID.ReportSDP:
                {
                    DebugString("*** ReportSDP ***\n");
                    stReportSDP pReportSDP = new stReportSDP(chSign);
                    fixed (byte* pData = aData)
                    {
                        pReportSDP = (stReportSDP)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSDP));
                    }
                    string sDebug = "[SDP]Tcp_port : - " + pReportSDP.Tcp_port +
                                     ", Sec : " + pReportSDP.Tcp_port +
                                     ", Tcp : " + pReportSDP.Tcp;
                    DebugString(sDebug);

                    break;
                }

            case MsgID.ReportV2G:
                {
                    DebugString("*** ReportV2G ***\n");

                    stReportV2G pReportV2G = new stReportV2G(chSign);
                    //pReportV2G = reinterpret_cast<stReportV2G*>(aData.data());
                    fixed (byte* pData = aData)
                    {
                        pReportV2G = (stReportV2G)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportV2G));
                    }
                    str_temp = ("ReportV2G Error_Code " + pReportV2G.ErrorCode);
                    DebugString(str_temp + "\n");
                    if (pReportV2G.ErrorCode > 0)
                    {
                        str_temp = ("Charger_ErrorCode = " + pReportV2G.ErrorCode);
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
                    str_temp = ("*** ReportSTATE ***");
                    DebugString(str_temp + "\n");

                    stReportSTATE pReportSTATE = new stReportSTATE(chSign);
                    //pReportSTATE = reinterpret_cast<stReportSTATE*>(aData.data());
                    fixed (byte* pData = aData)
                    {
                        pReportSTATE = (stReportSTATE)Marshal.PtrToStructure((IntPtr)pData, typeof(stReportSTATE));
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
                        str_temp = ("Report cp Voltage = " + pReportSTATE.info[0] + " V");
                        DebugString(str_temp + "\n");

                        if (bCurrentReq && (pReportSTATE.info[0] == 9 || pReportSTATE.info[0] == 12))
                        {
                            evTargetCurrent = 0;
                            bCurrentReq = false;
                        }

                    }

                }
                break;
        }

        if (DataBuffer.Length > 7)
        {
            SendPLC(evse_serial, DataBuffer);
        }

    }

    public void SendPLC(SerialPort SerialObj, byte[] aData)
    {
        if (SerialObj.IsOpen)
        {
            string str_temp = "";
            SerialObj.Write(aData, 0, aData.Length);

            string curtime = DateTime.Now.ToString("HH:mm:ss.fff ");
            str_temp = ("send Data Size = " + aData.Length);
            DebugString(str_temp + "\n");

            if (dbglevel >= 5)
            {
                string str_data = "";
                for (int cnt = 0; cnt < aData.Length; cnt++)
                {
                    if ((cnt % 8 == 0) && (cnt != 0))
                    {
                        str_data += "\n";
                    }
                    str_data += (aData[cnt].ToString("x02") + " ");
                }
                str_data += "\n";
                Console.WriteLine(str_data);
                status_text += (str_data + "\n");
            }

        }

    }

    public uint getChecksum(byte[] aData)
    {
        uint nChecksum = 0;
        int nIndex = Marshal.SizeOf(typeof(Msg_Header));
        for (int cnt = nIndex; cnt < (aData.Length - 4); cnt++)
        {
            nChecksum += aData[cnt];
        }

        return nChecksum;
    }

    public void SetPhysicalValue(ref PhysicalValue pValue, sbyte nMutiplier, byte nUnit, short nValue)
    {
        pValue.Multiplier = nMutiplier;
        pValue.Unit = nUnit;
        pValue.Value = nValue;
    }

    public unsafe void Request_ReportSTATE()
    {
        if (chSign[0] == 'Q' && chSign[1] == 'A')
        {
            stReportSTATE2Req pReportSTATEReq = new stReportSTATE2Req(chSign, (byte)1);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stReportSTATE2Req))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pReportSTATEReq, (IntPtr)pData, false);
            }
            pReportSTATEReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pReportSTATEReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send stReportSTATE2Req\n");
        }
        else if (startReq_info[0] == 0)
        {
            stReportSTATEReq pReportSTATEReq = new stReportSTATEReq(chSign, (byte)1);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stReportSTATEReq))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pReportSTATEReq, (IntPtr)pData, false);
            }
            pReportSTATEReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pReportSTATEReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send stReportSTATEReq\n");
        }

    }

    public unsafe void Send_AllStopReq()
    {
        stStartRequest checkstreqinfo = new stStartRequest();

        if (chSign[0] == 'Q' && chSign[1] == 'A')
        {
            stAllStop2Req pAllStopReq = new stAllStop2Req(chSign);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stAllStop2Req))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pAllStopReq, (IntPtr)pData, false);
            }
            pAllStopReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pAllStopReq, (IntPtr)pData, false);
            }
            DebugString("send stAllStop2Req\n");
            SendPLC(evse_serial, DataBuffer);
        }
        else if (startReq_info[0] == 0)
        {
            stAllStopReq pAllStopReq = new stAllStopReq(chSign);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stAllStopReq))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pAllStopReq, (IntPtr)pData, false);
            }
            pAllStopReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pAllStopReq, (IntPtr)pData, false);
            }
            DebugString("send stAllStopReq\n");
            SendPLC(evse_serial, DataBuffer);
        }

        curChagerStep = MsgID.NONE_ID;

    }

    public unsafe void Charger_Stop()
    {
        // 2021.09.28 부사장님 요청사항 적용 - hyeonho
        Send_AllStopReq();

        /* 
        if( curChagerStep < MsgID.PowerDeliveryReq)
        {
            Send_AllStopReq();
        }
        else
        {
            evseStatusCode = DC_EVSEStatusCodeType.EVSE_Shutdown; // 2;//EVSE_Shutdown;
        }
        */
    }

    public void SetAuthentication(EVSEProcessingType eProcessing)
    {
        eNumEVSEProcess = eProcessing;
    }

    public void SetResponseCodeType(responseCodeType eResponseCode)
    {
        eNumresponseCode = eResponseCode;
    }

    public void EmergencyStop()
    {
        evseStatusCode = DC_EVSEStatusCodeType.EVSE_EmergencyShutdown;
        bEmergencyStop = true;
    }

    public unsafe void EVSE_RESET()
    {
        byte[] info = new byte[4] { (byte)'H', (byte)'R', (byte)'S', (byte)'T' };
        if (chSign[0] == 'Q' && chSign[1] == 'A')
        {
            stAllStop2Req pstAllStop2Req = new stAllStop2Req(chSign);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stAllStop2Req))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pstAllStop2Req, (IntPtr)pData, false);
            }
            pstAllStop2Req.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pstAllStop2Req, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send stReportINIT2Req\n");
        }
        else if (startReq_info[0] == 0)
        {
            stAllStopReq pstAllStopReq = new stAllStopReq(chSign, info);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stAllStopReq))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pstAllStopReq, (IntPtr)pData, false);
            }
            pstAllStopReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pstAllStopReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send stReportINIT1Req\n");
        }
    }


    public unsafe void Charger_Start()
    {
        evseStatusCode = DC_EVSEStatusCodeType.EVSE_Ready;
        bPLC_Connect = false;
        bPLC_SessionOK = false;
        bPLC_SessionRequest = false;
        bCurrentReq = false;
        ui_Min = 0;
        ui_Sec = 0;
        EV_SOC = 0;

        curChagerStep = MsgID.NONE_ID;
        bEmergencyStop = false;

        nPLC_CommCheck = 0;
        nOldPLC_CommCheck = 0;

        eNumEVSEProcess = EVSEProcessingType.Finished;
        eNumresponseCode = responseCodeType.OK;



        DebugString("Start Charger\n");

        send_ReportInitReq();

        DebugString("Check LOG\n");
    }

    public unsafe void send_ReportInitReq()
    {
        if (chSign[0] == 'Q' && chSign[1] == 'A')
        {
            // for EOL Test
            stReportINIT2Req pInitReq = new stReportINIT2Req(chSign);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stReportINIT2Req))];

            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pInitReq, (IntPtr)pData, false);
            }
            pInitReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pInitReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send stReportINIT2Req\n");
        }
        else if (startReq_info[0] == 0)
        {
            stReportINITReq pInitReq = new stReportINITReq(chSign, reportInit_info);

            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stReportINITReq))];

            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pInitReq, (IntPtr)pData, false);
            }
            pInitReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pInitReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send V2G DC Charging stReportINITReq\n");
        }



    }

    public unsafe void send_StartReq()
    {
        if (chSign[0] == 'Q' && chSign[1] == 'A')
        {

            // for EOL Test
            stStartRequest2 pStartReq = new stStartRequest2(chSign, startReq_info);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stStartRequest2))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
            }
            pStartReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
            }
            SendPLC(evse_serial, DataBuffer);
            DebugString("send pStartReq\n");
        }
        else if (startReq_info[0] == 0)
        {
            stStartRequest pStartReq = new stStartRequest(chSign, startReq_info);
            byte[] DataBuffer = new byte[Marshal.SizeOf(typeof(stStartRequest))];
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
            }
            pStartReq.Checksum = getChecksum(DataBuffer);
            fixed (byte* pData = DataBuffer)
            {
                Marshal.StructureToPtr(pStartReq, (IntPtr)pData, false);
            }


            SendPLC(evse_serial, DataBuffer);
            DebugString("send stStartRequest\n");
        }


        }
    }
} 
