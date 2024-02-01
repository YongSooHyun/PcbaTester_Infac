using System.Linq;

namespace E100RC_Production
{
    public class evse_stop_conditions
    {
        
    }



    public class evse_resp_stop_condition
    {

        static string str_OK = "OK = 0";
        static string str_OK_NewSessionEstablished = "OK_NewSessionEstablished = 1";
        static string str_OK_OldSessionJoined = "OK_OldSessionJoined = 2";
        static string str_OK_CertificateExpiresSoon = "OK_CertificateExpiresSoon = 3";
        static string str_FAILED = "FAILED = 4";
        static string str_FAILED_SequenceError = "FAILED_SequenceError = 5";
        static string str_FAILED_ServiceIDInvalid = "FAILED_ServiceIDInvalid = 6";
        static string str_FAILED_UnknownSession = "FAILED_UnknownSession = 7";
        static string str_FAILED_ServiceSelectionInvalid = "FAILED_ServiceSelectionInvalid = 8";
        static string str_FAILED_PaymentSelectionInvalid = "FAILED_PaymentSelectionInvalid = 9";
        static string str_FAILED_CertificateExpired = "FAILED_CertificateExpired = 10";
        static string str_FAILED_SignatureError = "FAILED_SignatureError = 11";
        static string str_FAILED_NoCertificateAvailable = "FAILED_NoCertificateAvailable = 12";
        static string str_FAILED_CertChainError = "FAILED_CertChainError = 13";
        static string str_FAILED_ChallengeInvalid = "FAILED_ChallengeInvalid = 14";
        static string str_FAILED_ContractCanceled = "FAILED_ContractCanceled = 15";
        static string str_FAILED_WrongChargeParameter = "FAILED_WrongChargeParameter = 16";
        static string str_FAILED_PowerDeliveryNotApplied = "FAILED_PowerDeliveryNotApplied = 17";
        static string str_FAILED_TariffSelectionInvalid = "FAILED_TariffSelectionInvalid = 18";
        static string str_FAILED_ChargingProfileInvalid = "FAILED_ChargingProfileInvalid = 19";
        static string str_FAILED_EVSEPresentVoltageToLow = "FAILED_EVSEPresentVoltageToLow = 20";
        static string str_FAILED_MeteringSignatureNotValid = "FAILED_MeteringSignatureNotValid = 21";
        static string str_FAILED_WrongEnergyTransferType = "FAILED_WrongEnergyTransferType = 22";

        static public string[] str_conditions = { 
                                                str_OK ,
                                                str_OK_NewSessionEstablished ,
                                                str_OK_OldSessionJoined ,
                                                str_OK_CertificateExpiresSoon ,
                                                str_FAILED ,
                                                str_FAILED_SequenceError ,
                                                str_FAILED_ServiceIDInvalid ,
                                                str_FAILED_UnknownSession ,
                                                str_FAILED_ServiceSelectionInvalid ,
                                                str_FAILED_PaymentSelectionInvalid ,
                                                str_FAILED_CertificateExpired ,
                                                str_FAILED_SignatureError ,
                                                str_FAILED_NoCertificateAvailable ,
                                                str_FAILED_CertChainError ,
                                                str_FAILED_ChallengeInvalid ,
                                                str_FAILED_ContractCanceled ,
                                                str_FAILED_WrongChargeParameter ,
                                                str_FAILED_PowerDeliveryNotApplied ,
                                                str_FAILED_TariffSelectionInvalid ,
                                                str_FAILED_ChargingProfileInvalid ,
                                                str_FAILED_EVSEPresentVoltageToLow ,
                                                str_FAILED_MeteringSignatureNotValid ,
                                                str_FAILED_WrongEnergyTransferType ,
                                            };



        uint sessionSetupCnt = 0;
        uint serviceDiscoveryCnt = 0;
        uint servicePaymentSelectionCnt = 0;
        uint contractAuthenticationCnt = 0;
        uint chargeParameterDiscoveryCnt = 0;
        uint cableCheckCnt = 0;
        uint prechargeCnt = 0;
        uint powerDeliveryCnt = 0;
        uint currentDemandCnt = 0;
        uint powerDelivery2Cnt = 0;
        uint weldingDetectionCnt = 0;

        public uint sessionSetupEvseRespStopNum = 0;
        public uint serviceDiscoveryEvseRespStopNum = 0;
        public uint servicePaymentSelectionEvseRespStopNum = 0;
        public uint contractAuthenticationEvseRespStopNum = 0;
        public uint chargeParameterDiscoveryEvseRespStopNum = 0;
        public uint cableCheckEvseRespStopNum = 0;
        public uint prechargeEvseRespStopNum = 0;
        public uint powerDeliveryEvseRespStopNum = 0;
        public uint currentDemandEvseRespStopNum = 0;
        public uint powerDelivery2EvseRespStopNum = 0;
        public uint weldingDetectionEvseRespStopNum = 0;

        public responseCodeType sessionSetupResCode = responseCodeType.OK;
        public responseCodeType serviceDiscoveryResCode = responseCodeType.OK;
        public responseCodeType servicePaymentSelectionResCode = responseCodeType.OK;
        public responseCodeType contractAuthenticationResCode = responseCodeType.OK;
        public responseCodeType chargeParameterDiscoveryResCode = responseCodeType.OK;
        public responseCodeType cableCheckResCode = responseCodeType.OK;
        public responseCodeType prechargeResCode = responseCodeType.OK;
        public responseCodeType powerDeliveryResCode = responseCodeType.OK;
        public responseCodeType currentDemandResCode = responseCodeType.OK;
        public responseCodeType powerDelivery2ResCode = responseCodeType.OK;
        public responseCodeType weldingDetectionResCode = responseCodeType.OK;


        public void init()
        {
            sessionSetupCnt = 0;
            serviceDiscoveryCnt = 0;
            servicePaymentSelectionCnt = 0;
            contractAuthenticationCnt = 0;
            chargeParameterDiscoveryCnt = 0;
            cableCheckCnt = 0;
            prechargeCnt = 0;
            powerDeliveryCnt = 0;
            currentDemandCnt = 0;
            powerDelivery2Cnt = 0;
            weldingDetectionCnt = 0;

            sessionSetupEvseRespStopNum = 0;
            serviceDiscoveryEvseRespStopNum = 0;
            servicePaymentSelectionEvseRespStopNum = 0;
            contractAuthenticationEvseRespStopNum = 0;
            chargeParameterDiscoveryEvseRespStopNum = 0;
            cableCheckEvseRespStopNum = 0;
            prechargeEvseRespStopNum = 0;
            powerDeliveryEvseRespStopNum = 0;
            currentDemandEvseRespStopNum = 0;
            powerDelivery2EvseRespStopNum = 0;
            weldingDetectionEvseRespStopNum = 0;


            sessionSetupResCode = responseCodeType.OK;
            serviceDiscoveryResCode = responseCodeType.OK;
            servicePaymentSelectionResCode = responseCodeType.OK;
            contractAuthenticationResCode = responseCodeType.OK;
            chargeParameterDiscoveryResCode = responseCodeType.OK;
            cableCheckResCode = responseCodeType.OK;
            prechargeResCode = responseCodeType.OK;
            powerDeliveryResCode = responseCodeType.OK;
            currentDemandResCode = responseCodeType.OK;
            powerDelivery2ResCode = responseCodeType.OK;
            weldingDetectionResCode = responseCodeType.OK;


        }

        public bool checkStop_sessionSetup()
        {
            if (sessionSetupEvseRespStopNum == 0)
                return false;

            sessionSetupCnt++;
            if (sessionSetupCnt >= sessionSetupEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_serviceDiscovery()
        {
            if (serviceDiscoveryEvseRespStopNum == 0)
                return false;

            serviceDiscoveryCnt++;
            if (serviceDiscoveryCnt >= serviceDiscoveryEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_servicePaymentSelection()
        {
            if (servicePaymentSelectionEvseRespStopNum == 0)
                return false;

            servicePaymentSelectionCnt++;
            if (servicePaymentSelectionCnt >= servicePaymentSelectionEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_contractAuthentication()
        {
            if (contractAuthenticationEvseRespStopNum == 0)
                return false;

            contractAuthenticationCnt++;
            if (contractAuthenticationCnt >= contractAuthenticationEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_chargeParameterDiscovery()
        {
            if (chargeParameterDiscoveryEvseRespStopNum == 0)
                return false;

            chargeParameterDiscoveryCnt++;
            if (chargeParameterDiscoveryCnt >= chargeParameterDiscoveryEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_cableCheck()
        {
            if (cableCheckEvseRespStopNum == 0)
                return false;

            cableCheckCnt++;
            if (cableCheckCnt >= cableCheckEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_precharge()
        {
            if (prechargeEvseRespStopNum == 0)
                return false;

            prechargeCnt++;
            if (prechargeCnt >= prechargeEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_powerDelivery()
        {
            if (powerDeliveryEvseRespStopNum == 0)
                return false;

            powerDeliveryCnt++;
            if (powerDeliveryCnt >= powerDeliveryEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_currentDemand()
        {
            if (currentDemandEvseRespStopNum == 0)
                return false;

            currentDemandCnt++;
            if (currentDemandCnt >= currentDemandEvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_powerDelivery2()
        {
            if (powerDelivery2EvseRespStopNum == 0)
                return false;

            powerDelivery2Cnt++;
            if (currentDemandCnt <= powerDelivery2EvseRespStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_weldingDetection()
        {
            if (weldingDetectionEvseRespStopNum == 0)
                return false;

            weldingDetectionCnt++;
            if (weldingDetectionCnt >= weldingDetectionEvseRespStopNum)
                return true;
            else
                return false;
        }


        public responseCodeType get_stop_condition(string str_ResCode)
        {
            responseCodeType res = 0;
            for (uint cnt = 0; cnt < str_conditions.Count(); cnt++)
            {
                if (str_ResCode.CompareTo(str_conditions[cnt]) == 0)
                {
                    res = (responseCodeType)cnt;
                    break;
                }
            }
            return res;
        }

    }


    public enum v2gisolationLevelType{
	    v2gisolationLevelType_Invalid = 0,
	    v2gisolationLevelType_Valid = 1,
	    v2gisolationLevelType_Warning = 2,
	    v2gisolationLevelType_Fault = 3,
	    v2gisolationLevelType_No_IMD = 4,
    };


    public enum dinDC_EVSEStatusCodeType{
	    dinDC_EVSEStatusCodeType_EVSE_NotReady = 0,
	    dinDC_EVSEStatusCodeType_EVSE_Ready = 1,
	    dinDC_EVSEStatusCodeType_EVSE_Shutdown = 2,
	    dinDC_EVSEStatusCodeType_EVSE_UtilityInterruptEvent = 3,
	    dinDC_EVSEStatusCodeType_EVSE_IsolationMonitoringActive = 4,
	    dinDC_EVSEStatusCodeType_EVSE_EmergencyShutdown = 5,
	    dinDC_EVSEStatusCodeType_EVSE_Malfunction = 6,
	    dinDC_EVSEStatusCodeType_Reserved_8 = 7,
	    dinDC_EVSEStatusCodeType_Reserved_9 = 8,
	    dinDC_EVSEStatusCodeType_Reserved_A = 9,
	    dinDC_EVSEStatusCodeType_Reserved_B = 10,
	    dinDC_EVSEStatusCodeType_Reserved_C = 11,
	    dinDC_EVSEStatusCodeType_EVSE_BatteryIncompatibility = 12,
	    dinDC_EVSEStatusCodeType_EVSE_ChargingSystemMalfunction = 13,
    };
    public enum dinEVSENotificationType{
	    dinEVSENotificationType_None = 0,
	    dinEVSENotificationType_StopCharging = 1,
	    dinEVSENotificationType_ReNegotiation = 2
    };



    public class evse_status
    {

        public static readonly string[] str_v2gisolationLevelTypes = {         v2gisolationLevelType.v2gisolationLevelType_Invalid.ToString(),
                                                                        v2gisolationLevelType.v2gisolationLevelType_Valid.ToString(),
                                                                        v2gisolationLevelType.v2gisolationLevelType_Warning.ToString(),
                                                                        v2gisolationLevelType.v2gisolationLevelType_Fault.ToString(),
                                                                        v2gisolationLevelType.v2gisolationLevelType_No_IMD.ToString()
                                                                    };
        public static readonly string[] str_dinDC_EVSEStatusCodeTypes = {
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_NotReady.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Ready.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Shutdown.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_UtilityInterruptEvent.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_IsolationMonitoringActive.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_EmergencyShutdown.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Malfunction.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_8.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_9.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_A.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_B.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_C.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_BatteryIncompatibility.ToString(),
                                                                    dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_ChargingSystemMalfunction.ToString()
                                                        };
        public static readonly string[] str_dinEVSENotificationTypes = {
                                                                    dinEVSENotificationType.dinEVSENotificationType_None.ToString(),
                                                                    dinEVSENotificationType.dinEVSENotificationType_StopCharging.ToString(),
                                                                    dinEVSENotificationType.dinEVSENotificationType_ReNegotiation.ToString()
                                                                };
        //Chademo 2.0
        /*   Reserved  */
        //public byte vehicledischargecompatiblity = 0;
        public byte dynamiccontrol = 0;
        public byte highcurrentcontrol = 0;
        public byte highvoltagecontrol = 0;
        public byte operatingcondtion = 0;
        public byte coolingfunction_for_cable = 0;
        public byte currentlimitingfunction_for_cable = 0;
        public byte coolingfunction_for_connect = 0;
        public byte currentlimitingfunction_for_connect = 0;
        public byte overtemperatureprotection = 0;
        public byte reliabilitydesign = 0;

        public v2gisolationLevelType EVSEIsolationStatus = v2gisolationLevelType.v2gisolationLevelType_Invalid;
        public dinDC_EVSEStatusCodeType EVSEStatusCode = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_NotReady;
        public dinEVSENotificationType EVSENotification = dinEVSENotificationType.dinEVSENotificationType_None;
        //byte padding;
        public uint NotificationMaxDelay = 0;


    }


    public class evse_status_condition
    {

        evse_status evse_status = new evse_status();

        //uint sessionSetupCnt = 0;
        //uint serviceDiscoveryCnt = 0;
        //uint servicePaymentSelectionCnt = 0;
        //uint contractAuthenticationCnt = 0;
        uint chargeParameterDiscoveryCnt = 0;
        uint cableCheckCnt = 0;
        uint prechargeCnt = 0;
        uint powerDeliveryCnt = 0;
        uint currentDemandCnt = 0;
        uint powerDelivery2Cnt = 0;
        uint weldingDetectionCnt = 0;

        //public uint sessionSetupEvseStatusStopNum = 0;
        //public uint serviceDiscoveryEvseStatusStopNum = 0;
        //public uint servicePaymentSelectionEvseStatusStopNum = 0;
        //public uint contractAuthenticationEvseStatusStopNum = 0;
        public uint chargeParameterDiscoveryEvseStatusStopNum = 0;
        public uint cableCheckEvseStatusStopNum = 0;
        public uint prechargeEvseStatusStopNum = 0;
        public uint powerDeliveryEvseStatusStopNum = 0;
        public uint currentDemandEvseStatusStopNum = 0;
        public uint powerDelivery2EvseStatusStopNum = 0;
        public uint weldingDetectionEvseStatusStopNum = 0;


        //public evse_status sessionSetup_evse_status = new evse_status();
        //public evse_status serviceDiscovery_evse_status = new evse_status();
        //public evse_status servicePaymentSelection_evse_status = new evse_status();
        //public evse_status contractAuthentication_evse_status = new evse_status();
        public evse_status chargeParameterDiscovery_evse_status = new evse_status();
        public evse_status cableCheck_evse_status = new evse_status();
        public evse_status precharge_evse_status = new evse_status();
        public evse_status powerDelivery_evse_status = new evse_status();
        public evse_status currentDemand_evse_status = new evse_status();
        public evse_status powerDelivery2_evse_status = new evse_status();
        public evse_status weldingDetection_evse_status = new evse_status();


        public void init()
        {
            //sessionSetupCnt = 0;
            //serviceDiscoveryCnt = 0;
            //servicePaymentSelectionCnt = 0;
            //contractAuthenticationCnt = 0;
            chargeParameterDiscoveryCnt = 0;
            cableCheckCnt = 0;
            prechargeCnt = 0;
            powerDeliveryCnt = 0;
            currentDemandCnt = 0;
            powerDelivery2Cnt = 0;
            weldingDetectionCnt = 0;

            //sessionSetupEvseStatusStopNum = 0;
            //serviceDiscoveryEvseStatusStopNum = 0;
            //servicePaymentSelectionEvseStatusStopNum = 0;
            //contractAuthenticationEvseStatusStopNum = 0;
            chargeParameterDiscoveryEvseStatusStopNum = 0;
            cableCheckEvseStatusStopNum = 0;
            prechargeEvseStatusStopNum = 0;
            powerDeliveryEvseStatusStopNum = 0;
            currentDemandEvseStatusStopNum = 0;
            powerDelivery2EvseStatusStopNum = 0;
            weldingDetectionEvseStatusStopNum = 0;

            evse_status sessionSetup_evse_status = new evse_status();
            evse_status serviceDiscovery_evse_status = new evse_status();
            evse_status servicePaymentSelection_evse_status = new evse_status();
            evse_status contractAuthentication_evse_status = new evse_status();
            evse_status chargeParameterDiscovery_evse_status = new evse_status();
            evse_status cableCheck_evse_status = new evse_status();
            evse_status prechargeRes_evse_status = new evse_status();
            evse_status powerDelivery_evse_status = new evse_status();
            evse_status currentDemand_evse_status = new evse_status();
            evse_status powerDelivery2_evse_status = new evse_status();
            evse_status weldingDetection_evse_status = new evse_status();


        }
#if false
        public bool checkStop_sessionSetup()
        {
            if (sessionSetupEvseStatusStopNum == 0)
                return false;

            sessionSetupCnt++;
            if (sessionSetupCnt >= sessionSetupEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_serviceDiscovery()
        {
            if (serviceDiscoveryEvseStatusStopNum == 0)
                return false;

            serviceDiscoveryCnt++;
            if (serviceDiscoveryCnt >= serviceDiscoveryEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_servicePaymentSelection()
        {
            if (servicePaymentSelectionEvseStatusStopNum == 0)
                return false;

            servicePaymentSelectionCnt++;
            if (servicePaymentSelectionCnt >= servicePaymentSelectionEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_contractAuthentication()
        {
            if (contractAuthenticationEvseStatusStopNum == 0)
                return false;

            contractAuthenticationCnt++;
            if (contractAuthenticationCnt >= contractAuthenticationEvseStatusStopNum)
                return true;
            else
                return false;
        }
#endif
        public bool checkStop_chargeParameterDiscovery()
        {
            if (chargeParameterDiscoveryEvseStatusStopNum == 0)
                return false;

            chargeParameterDiscoveryCnt++;
            if (chargeParameterDiscoveryCnt >= chargeParameterDiscoveryEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_cableCheck()
        {
            if (cableCheckEvseStatusStopNum == 0)
                return false;

            cableCheckCnt++;
            if (cableCheckCnt >= cableCheckEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_precharge()
        {
            if (prechargeEvseStatusStopNum == 0)
                return false;

            prechargeCnt++;
            if (prechargeCnt >= prechargeEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_powerDelivery()
        {
            if (powerDeliveryEvseStatusStopNum == 0)
                return false;

            powerDeliveryCnt++;
            if (powerDeliveryCnt >= powerDeliveryEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_currentDemand()
        {
            if (currentDemandEvseStatusStopNum == 0)
                return false;

            currentDemandCnt++;
            if (currentDemandCnt >= currentDemandEvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_powerDelivery2()
        {
            if (powerDelivery2EvseStatusStopNum == 0)
                return false;

            powerDelivery2Cnt++;
            if (currentDemandCnt <= powerDelivery2EvseStatusStopNum)
                return true;
            else
                return false;
        }
        public bool checkStop_weldingDetection()
        {
            if (weldingDetectionEvseStatusStopNum == 0)
                return false;

            weldingDetectionCnt++;
            if (weldingDetectionCnt >= weldingDetectionEvseStatusStopNum)
                return true;
            else
                return false;
        }


        public v2gisolationLevelType get_condition_isolation(string str_ResCode)
        {
            v2gisolationLevelType res = v2gisolationLevelType.v2gisolationLevelType_Invalid;

            if (str_ResCode.Contains(v2gisolationLevelType.v2gisolationLevelType_Invalid.ToString()))
                res = v2gisolationLevelType.v2gisolationLevelType_Invalid;
            else if (str_ResCode.Contains(v2gisolationLevelType.v2gisolationLevelType_Valid.ToString()))
                res = v2gisolationLevelType.v2gisolationLevelType_Valid;
            else if (str_ResCode.Contains(v2gisolationLevelType.v2gisolationLevelType_Warning.ToString()))
                res = v2gisolationLevelType.v2gisolationLevelType_Warning;
            else if (str_ResCode.Contains(v2gisolationLevelType.v2gisolationLevelType_Fault.ToString()))
                res = v2gisolationLevelType.v2gisolationLevelType_Fault;
            else if (str_ResCode.Contains(v2gisolationLevelType.v2gisolationLevelType_No_IMD.ToString()))
                res = v2gisolationLevelType.v2gisolationLevelType_No_IMD;
            
            return res;
        }

        public dinDC_EVSEStatusCodeType get_condition_evse_status_code(string str_ResCode)
        {
            dinDC_EVSEStatusCodeType res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_NotReady;

            if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_NotReady.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_NotReady;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Ready.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Ready;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Shutdown.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Shutdown;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_UtilityInterruptEvent.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_UtilityInterruptEvent;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_IsolationMonitoringActive.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_IsolationMonitoringActive;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_EmergencyShutdown.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_EmergencyShutdown;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Malfunction.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_Malfunction;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_8.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_8;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_9.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_9;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_A.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_A;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_B.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_B;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_C.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_Reserved_C;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_BatteryIncompatibility.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_BatteryIncompatibility;
            else if (str_ResCode.Contains(dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_ChargingSystemMalfunction.ToString()))
                res = dinDC_EVSEStatusCodeType.dinDC_EVSEStatusCodeType_EVSE_ChargingSystemMalfunction;

            return res;
        }


        public dinEVSENotificationType get_condition_evse_notification(string str_ResCode)
        {
            dinEVSENotificationType res = dinEVSENotificationType.dinEVSENotificationType_None;

            if (str_ResCode.Contains(dinEVSENotificationType.dinEVSENotificationType_None.ToString()))
                res = dinEVSENotificationType.dinEVSENotificationType_None;
            else if (str_ResCode.Contains(dinEVSENotificationType.dinEVSENotificationType_StopCharging.ToString()))
                res = dinEVSENotificationType.dinEVSENotificationType_StopCharging;
            else if (str_ResCode.Contains(dinEVSENotificationType.dinEVSENotificationType_ReNegotiation.ToString()))
                res = dinEVSENotificationType.dinEVSENotificationType_ReNegotiation;

            return res;
        }



    }






}
