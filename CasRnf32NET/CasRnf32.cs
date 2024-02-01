using System;
using System.Collections;
using System.Text;
using System.Runtime.InteropServices;

namespace CasRnf
{
  // *** interface class ***
  class CCasRnf
  {
#if CASAPICLIENT || CASAPICLIENT64
    public const string dllName = "CasAPIClient.dll";
#elif CASAPILANCLIENT
    public const string dllName = "CasApiLanClient.dll";
#elif CASMULTICLIENT || CASMULTICLIENT64
    public const string dllName = "CasMultiClient.dll";
#else
    public const string dllName = "CasRnf32.dll";
#endif

    // *** global constants ***

    // function returns
    public const int casOk                   = 0;        // all functions
    public const int casError                = -1;
    public const int casInvalidCall          = -2;
    public const int casInternalError        = -3;       // no Link to CASCON16
    public const int casBusy                 = -4;       // DDE only: CASCON is busy
    public const int casCancelled            = -10;      // CasUnpackArchive
    public const int casFunctionStarted      = 0x1000;   // return value in DDE async mode

    public const int casWildChars            = 1;        // GetFName           
    public const int casUutError             = 1;        // Ir/DrShift,Tapreset
    public const int casYes                  = 1;        // CasGetNextMsgBox   

    public const int casVirtualPass          = 0x100;

    // DLL state flags
    public const int  dsInit                 = 0x0001;   // InitCasRunF was called
    public const int  dsCalled               = 0x0002;   // during Call Backs
    public const int  dsStarted              = 0x0010;   // CasTeststart was called
    public const int  dsCaslanRunning        = 0x0020;   // CasInitCaslanTest was called 
    public const int  dsDebugOn              = 0x0100;   // CASCON Debugger is active

    // CasSelectUUT, CasGetUutTest, aSut
    public const int sutUutName              = 0x02;     // ahUUT is valid
    public const int sutBatchName            = 0x04;     // ahBatch is valid
    public const int sutTestName             = 0x40;     // ahTest is valid

    // CasGetFileName fType
    public const int   fnCaslanCOC           = 334;
    public const int   fnResult1             = 420;
    public const int   fnResult2             = 421;
    public const int   fnResult3             = 422;
    public const int   fnResult4             = 423;

    // Images for groups and registers
    public const int   imgDrive              = 0x08;
    public const int   imgMask               = 0x40;
    public const int   imgExpect             = 0x80;
    public const int   imgMeasure            = 0x02;
    public const int   imgFail               = 0x01;
    public const int   imgMeasure2           = 0x04;
    public const int   imgMeasure3           = 0x10;
    public const int   imgMeasure4           = 0x20;

    // CasAGroupPin, aIoca
    public const int   iocInput              = 0;
    public const int   iocOuput              = 1;
    public const int   iocControl            = 2;
    public const int   iocAll                = 3;

    // Caslan variable
    public const int isCaslanVar             = 1;
    public const int isNotCaslanVar          = 0;

    // Call back constants
    // . CancelCheck, CheckBreak
    public const int ccAbortNo               = 0;
    public const int ccAbortYes              = 1;
    // . MessageBoxes
    public const int udWarning               = 0x0000;   // Display a Warning box
    public const int udError                 = 0x0001;   // Dispaly a Error box
    public const int udInformation           = 0x0002;   // Display an Information Box
    public const int udConfirmation          = 0x0003;   // Display a Confirmation Box
    public const int udHeaderMask            = 0x0003;
    // . Message box button flags
    public const int udYesButton             = 0x0100;   // Put a Yes button into the dialog
    public const int udNoButton              = 0x0200;   // Put a No button into the dialog
    public const int udOKButton              = 0x0400;   // Put an OK button into the dialog
    public const int udCancelButton          = 0x0800;   // Put a Cancel button into the dialog
    public const int udButtonMask            = 0x0F00;

    public const int udYesNoCancel           = udYesButton + udNoButton + udCancelButton;
										                                     // Standard Yes, No, Cancel dialog
    public const int udOKCancel              = udOKButton + udCancelButton;
								                                         // Standard OK, Cancel dialog
    // . Results of a MessageBox
    public const int udrOk                   = 1;        //idOk
    public const int udrCancel               = 2;        //idCancel
    public const int udrAbort                = 3;        //idAbort
    public const int udrRetry                = 4;        //idRetry
    public const int udrIgnore               = 5;        //idIgnore
    public const int udrYes                  = 6;        //idYes
    public const int udrNo                   = 7;        //idNo

    // public const  {ISP -Attribute}
    public const int ispNoIsp                = 0;
    public const int ispInit		             = 1;
    public const int ispIdcodes	             = 2;
    public const int ispErase	               = 3;
    public const int ispBlankCheck           = 4;
    public const int ispProgram	             = 5;
    public const int ispVerify	             = 6;
    public const int ispCrc		               = 7;
    public const int ispRead		             = 8;

    // added CASCON 4.0.1, 20.06.2002
    // . CasSetDebugMode . aDebugOn
    public const int cCasDebugOff            = 0;
    public const int cCasDebugOn             = 1;

    // . PIP access
    public const int casPIP1                 = 1;
    public const int casPIP2                 = 2;
    public const int casPIP3                 = 3;
    public const int casPIP4                 = 4;
    public const int casPIP12                = 5;
    public const int casPIP34                = 6;
    public const int casPIP1234              = 7;

    // . MPP access
    public const int casMPP_1                = 0x100;
    public const int casMPP_2                = 0x101;
    public const int casMPP_3                = 0x102;
    public const int casMPP_4                = 0x103;
    public const int casMPP_5                = 0x104;
    public const int casMPP_6                = 0x105;
    public const int casMPP_7                = 0x106;
    public const int casMPP_8                = 0x107;
    public const int casMPP_12               = 0x110;
    public const int casMPP_34               = 0x111;
    public const int casMPP_56               = 0x112;
    public const int casMPP_78               = 0x113;
    public const int casMPP_1234             = 0x120;
    public const int casMPP_5678             = 0x121;

    // . PIP direction
    public const int casDisablePIP           = 0;
    public const int casEnablePIP            = 1;

    // . BScan controller type
    public const int casCtrlNoType           = -1;
    public const int casCtrlVirtual          = 0;
    public const int casCtrlMfcAsc           = 1;
    public const int casCtrlScanBooster      = 2;
    public const int casCtrlPCI_A            = 3;
    public const int casCtrlPCI_B            = 4;
    public const int casCtrlPcmcia_A         = 5;
    public const int casCtrlPcmcia_B         = 6;
    public const int casCtrlPXI_A            = 7;
    public const int casCtrlPXI_B            = 8;
    public const int casCtrlUSB_A            = 9;
    public const int casCtrlUSB_B            = 10;
    public const int casCtrlLAN_B            = 11;
    public const int casCtrlLAN_BV2          = 12;
    public const int casCtrlUniPort          = 13;
    public const int casCtrlVsc_A            = 14;
    public const int casCtrlVsc_B            = 15;
    public const int casCtrlScanFlex         = 100;
    public const int casCtrlTesterConfig     = 101;

    public const int casCtrlUserDefined      = 1000;

    public const int cas3Project             = 3;
    public const int cas4Project             = 4;

    // added CASCON 4.0.4, 13.05.2003
    // . CasShowExecWin Enable
    public const int sewInitExecWin          = 0x0001;   // Level 1      
    public const int sewUseExecWin           = 0x0002;   // Level 1	  
    public const int sewIgnoreExecWin        = 0x0004;   // Level 1	  
    public const int sewReleaseResults       = 0x0008;   // Level 1	  
    public const int sewShowExecWin          = 0x0010;   // Level 1,2,3  
    public const int sewHideExecWin          = 0x0020;   // Level 1,2,3  

    public const int sewUserMsgBox           = 0x0100;   // Level 1,2,3  
    public const int sewBuiltInMsgBox        = 0x0300;   // Level 1,2,3  

    public const int sewUserReadChar         = 0x0400;   // Level 1,2,3  
    public const int sewBuiltInReadChar      = 0x0C00;   // Level 1,2,3  

    // added CASCON 4.1.1   14.11.2003
    // . SetCasCallBack aCallBackType
    public const int cbtFLASHProgress        = 0;

    // . FFlashCallBack aUpdateType
    public const int futPosUpdate            = 0;        // several times
    public const int futFileRead             = 1;        // one time at beginning
    public const int futFileWrite            = 2;        // one time at beginning

    // added CASCON 4.4.0,  19.02.2007
    // . lloXXXX: error returns of CasExecClockAtTapState, CasExecLowLevelTAPMulti, CasExecLowLevelTAP
    public const int lloWrongAction          = -3;
    public const int lloWrongActionBeforeGetMeasure
                                      = -4;
    public const int lloWrongTapState        = -5;
    public const int lloSizeError            = -6;
    public const int lloTapMismatch          = -7;
    public const int lloExecError            = -8;
    public const int lloWrongUutNr           = -9;

    //. ecatsXXXX: CasExecClockAtTapState aState
    public const int ecatsClockReset         = 0;        // goto TAPReset, then clock with TMS = HIGH		 
    public const int ecatsClockRunIdle       = 1;        // goto Run Test/IDLE, then clock with TMS = LOW	 
    public const int ecatsClockTmsLow        = 2;        // clock with TMS = LOW								 

    // . llaXXXX: CasExecLowLevelTAP aAction
    public const int llaTRST                 = 1;
    public const int llaSetState             = 2;
    public const int llaSIR                  = 3;
    public const int llaSDR                  = 4;
    public const int llaTmsAndTdo            = 5;
    public const int llaGetMeasure           = 6;
    public const int llaSetUsedUUTs          = 7;

    // . llmXXXX: CasExecLowLevelTAP  LowLevelModifier
    public const int llmGetMeasure           = 0x01;     // together with llaSDR, llaSIR, llaTmsAndTdo
    public const int llmWithoutMeasure       = 0x02;     // ignored, if llmGetMeasure is set
    public const int llmAsciiSVF             = 0x04;     // databuf  is ASCII SVF format
    public const int llmBurstBreakInShift    = 0x08;     // don't go to PauseXR, when the Burst must be splitted
    public const int llmWithoutUpdate        = 0x10;     // when started in PauseXR go directly to ShiftXR  without UpdateXR

    // . tapXXXXX tLowLevelTAPData mEndTapState, CasExecLowLevelTAP aEndTapState
    public const int tapReset                = 0;
    public const int tapRunIdle              = 1;
    public const int tapSelectDR             = 2;
    public const int tapSelectIR             = 3;
    public const int tapCaptureDR            = 4;
    public const int tapShiftDR              = 5;
    public const int tapExit1DR              = 6;
    public const int tapPauseDR              = 7;
    public const int tapExit2DR              = 8;
    public const int tapUpdateDR             = 9;
    public const int tapCaptureIR            = 10;
    public const int tapShiftIR              = 11;
    public const int tapExit1IR              = 12;
    public const int tapPauseIR              = 13;
    public const int tapExit2IR              = 14;
    public const int tapUpdateIR             = 15;

    // . tatXXXX bit in DataBuf for llaTmsAndTdo
    public const int tatTMS                  = 1;        //                    - set by user application
    public const int tatTDO                  = 2;        // controller => UUT  - set by user application
    public const int tatTDI                  = 4;        // UUT => controller  - set by GetMeasure      

    // String options
    public const int soEnvVar                = 20;
    public const int soEnvVarOfIdx           = 21;
    public const int soEnvKeyOfIdx           = 22;
    public const int soSVFDir                = 30;
    public const int soCasFileDir            = 31;
    public const int soResultFileName        = 40;
    public const int soSVFFile               = 120;
    public const int soJamSTAPLFile          = 121;
    public const int soSVFCommands           = 130;
    public const int soJamSTAPLCommands      = 131;
    public const int soUutComment            = 140;
    public const int soTestComment           = 141;
    public const int soBatchComment          = 142;
    public const int soCasconIdCode          = 150;
    public const int soHelpAbout             = 160;
    public const int soTesterConfig          = 161;
    public const int soNextPart              = 1000;
    public const int soSerialNoMask          = 2000;
    public const int soSerialNoSample        = 2001;
    public const int soSerialNoHint          = 2002;
    // Modifiers
    public const int somCompile              = 1;
    public const int somUpdateJAMFormat      = 1;  /* Find format of file, (alwayse, when filename is changed) */
    // integer options
    public const int ioEnableAllProjects     = 11;
    public const int ioUnpackQuiet           = 12;
    public const int ioUutIdentBy            = 13;
    public const int ioEnvVars               = 14;
    public const int ioLicExpiryDate         = 15;
    public const int ioCtrlInitFlags         = 16;
    public const int ioProjectVersion        = 17;
    public const int ioUseRegExpModeForSerialNoMask = 3000;
    // values for ioUutIdentBy
    public const int uibTestStartNumber      = 0;
    public const int uibSerialNumUserDlg     = 1;
    public const int uibSerialNumTempFile    = 2;
    public const int uibSerialNumDDE         = 3;
    // values for ioCtrlInitFlags
    public const int cifTapState             = 1;
    public const int cifPipMpp               = 2;

    // Version 4.5.4b 04.05.2011
    // CasGetExecutedTestsEntry.aTestType
    public const int  ttUnknown              = -1;
    public const int  ttInfrastructure       = 0;
    public const int  ttInterconnection      = 1;
    public const int  ttRAM                  = 2;
    public const int  ttCluster              = 3;
    public const int  ttModel                = 4;
    public const int  ttIEEE1445             = 5;
    public const int  ttFlyingprobe          = 6;
    public const int  ttFLASH                = 7;
    public const int  ttVarioTAPTest         = 8;
    public const int  ttVarioTAPFlash        = 9;
    public const int  ttManual               = 10;
    public const int  ttJAMSTAPL             = 11;
    public const int  ttSVF                  = 12;
    public const int  ttIEEE1532             = 13;
    public const int  ttWinExe               = 14;
    public const int  ttCEVimported          = 15;
    public const int  ttSVFimported          = 16;
    public const int  ttBatchconverted       = 17;
    public const int  ttInteractiveATE       = 19;
    public const int  ttChipAssisted         = 20;

    // *** CASRUNF functions ***

    public delegate int FCheckBreakProc(uint aMainData);
    public delegate int FReadKeyProc(uint aMainData);
    public delegate int FMsgBoxProc(uint aMainData, string aHC, uint aFlags);

    // initialization, level 0->1, 1->0
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int InitCasRunF(
			uint* aCas, string aCmdLine, uint aMainData, FCheckBreakProc aCheckBreakProc, 
      FReadKeyProc aReadKeyProc, FMsgBoxProc aMsgBoxProc, StringBuilder IniErrorMsg);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int DoneCasRunF(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int EnableDoneCasRunF(int aEnabled);
    
    // state request, operates at each level
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int GetCasRunFState(uint aCas);
    
    // version request, operates at each level
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int GetCasVersion(StringBuilder aVersion);
    
    // utility functions, level 1, 2, 3
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetNextMsgBox(uint aCas, StringBuilder aMessage);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetFileName(uint aCas, int fType, StringBuilder aFileName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetUUTTest(
      uint aCas, StringBuilder ahUUT, StringBuilder ahBatch, StringBuilder ahTest);
    
    // level 1 functions
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecute(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasCommand(uint aCas, string aCommand);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSelectUUT(uint aCas, string ahUUT, string ahBatch, string ahTest, uint aSut);
    
    // level 1, 2 functions
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSelectBatch(uint aCas, string ahBatch);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSelectTest(uint aCas, string ahTest);
    
    // switch level 1->2, 2->1
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTestStart(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTestEnde(uint aCas);
    
    // level 2 functions
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasRunTest(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasRunBatch(uint aCas);
    
    // switch level 2->3, 3->2
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasInitCaslanTest(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasDoneCaslanTest(uint aCas);
    
    // level 3 functions for CASLAN execution
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecCaslanTest(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasRestartCaslanTest(uint aCas);

    // level 3 functions for direct access of CASLAN elements
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetVariable(uint aCas, uint aVarId, byte[] buf, int bCnt);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetVariable(uint aCas, uint aVarId, byte[] buf, int bCnt);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetGroup(uint aCas, uint aGroupId, int aImage, byte[] buf, int bCnt);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetGroup(uint aCas, uint aGroupId, int aImage, byte[] buf, int bCnt);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasLDI(uint aCas, byte[] aLdiRec);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetRegister(uint aCas, uint aRegId, int aImage, byte[] buf, int bCnt);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetRegister(uint aCas, uint aRegId, int aImage, byte[] buf, int bCnt);
    
    // level 3 functions for direct UUT access
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasDrShift(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasIrShift(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTapReset(uint aCas);

    // level 3 functions for diagnosis support
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasAGetSymbolValue(uint aCas, string acVarName, StringBuilder acVarValue);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailPinCount(
      uint aCas, string acIcName, int aStringVarIcName, int* failcount);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailPin(
      uint aCas, string acIcName, int aStringVarIcName, string acFailNr, int aVarFailNr, int aDel,
      StringBuilder fcIcName, StringBuilder fcPinName, int* fFail);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailGroupCount(
      uint aCas, string acGroupName, int aStringVarGroupName, int* fGroupFailCount, int* fPinFailCount);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailGroup(
      uint aCas, string acGroupNr, int aCharGroupNr, int aDel, StringBuilder fcGroupName,
      int* fGroupFailPins, StringBuilder acLowFail, StringBuilder acHighFail);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailGroupIndex(
      uint aCas, string acGroupName, int aStringVarGroupName, string acFailNr, int aVarFailNr, int aDel,
      StringBuilder fcGroupName, int* fGroupIndex, int* fFail);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAFailGroupPin(
      uint aCas, string acGroupName, int aStringVarGroupName, string acFailNr, int aVarFailNr, int aDel,
      StringBuilder fcIcName, StringBuilder fcPinName, int* fFail);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasAGroupSize(
      uint aCas, string acGroupName, int aStringVarGroupName, int* aGroupSize);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasAGroupPin(
      uint aCas, string acGroupName, int aStringVarGroupName, string acGroupIndex, int aVarGroupIndex,
      int aIoca, StringBuilder fcIcName, StringBuilder fnPinName);

    // added 25.09.2000
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasUutList(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTestList(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasUutListEntry(uint aCas, int aIndex, StringBuilder aUutName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTestListEntry(uint aCas, int aIndex, StringBuilder aTestName);

    // added CASCON 3.43, 22.02.2001
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasTestListEntryComment(uint aCas, int aIndex, StringBuilder aComment);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetUutBaseDir(uint aCas, StringBuilder aBaseDir);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetUutBaseDir(uint aCas, string aBaseDir);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetUutDir(uint aCas, StringBuilder aUutDir);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasUnpackArchive(uint aCas, string aArchive);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasProcList(uint aCas, string aTest);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasProcListEntry(
      uint aCas, int aIndex, StringBuilder aResult, int* aIntParams, int* aStringParams,
      int* aIspAttribute);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasExecCaslanProc(
      uint aCas, int aIndex, int aIntParams, int aStringParams, int* aParams);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasExecCaslanProcByName(
      uint aCas, string aName, int aIntParams, int aStringParams, int* aParams);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetDataBlockSize(uint aCas, int aSize);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasGetDataBlockSize(uint aCas, int* aSize);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetDataBlock(
      uint aCas, byte[] aAppData_Block, int aAppSize, int aBlockIndex);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetDataBlock(
      uint aCas, byte[] aAppData_Block, int aAppSize, int aBlockIndex);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasBatchList(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasBatchListEntry(uint aCas, int aIndex, StringBuilder aBatchName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasUutListEntryComment(int aCas, int aIndex, StringBuilder aComment);

    // added CASCON 4.0.1, 20.06.2002
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int GetCasLicense(uint aCas, StringBuilder aLicense);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int GetCasProjectType(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetExitCode(uint aCas, int aUutNr);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetSerialNumber(uint aCas, int aUutNr, StringBuilder aSerialNr);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetSerialNumber(uint aCas, int aUutNr, string aSerialNr);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasGetUsedUUTs(uint aCas, uint* aUsedUuts);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetUsedUUTs(uint aCas, int aUutCount, uint aUsedUuts);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasPIPOut(uint aCas, uint aPipNr, uint aPipVal);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasPIPDir(uint aCas, uint aPipNr, uint aPipDir);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasPIPIn(uint aCas, uint aPipNr, uint* aInValue);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasGetTCK(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetTCK(uint aCas, int aTck);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int CasGetController(
      uint aCas, int* aControllerID, StringBuilder aDriverDllName, StringBuilder aControllerName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetController(uint aCas, int aControllerID, string aDriverDllName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasSetDebugMode(uint aCas, int aDebugOn);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecuteBatch(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecuteTest(uint aCas);

    // added CASCON 4.0.4, 13.05.2003
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasShowExecWin(uint aCas, uint aEnable);

    // added CASCON 4.1.1, 14.11.2003
    public delegate int FFlashCallBack(uint aData, int aUpdateType, int aFlashPos, int aFlashSize);
        // aFlashPos is valid, if aFlashSize > 0
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int SetCasCallBack(
      uint aCas, int aCallBackType, FFlashCallBack aCallBackFunc, uint aCallBackData);    

    // added CASCON 4.1.2, 26.03.2004
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    unsafe public static extern int SetScanFlexPointer(uint aCas, string aScanFlexModul, uint* aFrame);

    // added CASCON 4.4.0, 19.02.2007
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecClockAtTapState(
      uint aCas, int aState, int aClockCount, double aMinTime, double aMaxTime);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
    public static extern int CasExecLowLevelTAP(
      uint aCas, int aAction, uint aActionModifier, int aDataLen, byte[] aDataBuf, int aDataSize,
      int aEndTapState, uint aUutNr);

    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasSetSOption(uint aCas, int aWhat, string aValue, int aModifier);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetSOption(uint aCas, int aWhat, StringBuilder aValue, int aValueSize, int aModifier);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasSetSOptionEntry(uint aCas, int aWhat, string aEntry, string aValue, int aModifier);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetSOptionEntry(uint aCas, int aWhat, string aEntry, StringBuilder aValue, int aValueSize, int aModifier);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasSetIOption(uint aCas, int aWhat, int aValue, int aModifier);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int CasGetIOption(uint aCas, int aWhat, int* aValue, int aModifier);

    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetExecutedTests(uint aCas, int aUutNr);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int CasGetExecutedTestsEntry(uint aCas, int aUutNr, int aIndex,
      StringBuilder aTestName, int* aExitCode, int* aTestType, StringBuilder aTestDetails, int* aErrorCode);

#if CASAPICLIENT || CASAPICLIENT64
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern void UnloadCasAPIClient();

    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int GetCasHandle(uint* aCas);
#elif CASAPILANCLIENT
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int CasConnectToLanServer(uint* aCas, string aHostName, uint aPort, StringBuilder aErrorMsg);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasDisconnectLanServer(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetVersion(uint aCas, StringBuilder aVersion);

    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasFetchFileContent(uint aCas, int fType);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasSaveFileContent(uint aCas, string aFileName);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetFileLine(uint aCas, StringBuilder aLineContent, int aLineNumber);
#elif CASMULTICLIENT || CASMULTICLIENT64
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int CasConnectToServer(uint* aCas, uint aServerId, StringBuilder aErrorMsg);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasCloseServer(uint aCas);
    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      public static extern int CasGetVersion(uint aCas, StringBuilder aVersion);

    [DllImport(dllName, CharSet=CharSet.Ansi, CallingConvention=CallingConvention.StdCall)]
      unsafe public static extern int GetCasMultiHandle(uint aServerId, uint* aCas);
#endif
  }
}
