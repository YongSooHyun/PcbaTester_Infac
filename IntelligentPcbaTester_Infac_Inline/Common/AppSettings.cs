using System;
using System.Configuration;
using System.Drawing;
using System.IO;
using System.Reflection;
using System.Runtime.Serialization.Formatters;
using System.Windows.Forms;
using TestFramework.PluginTestCell;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 로그파일 저장 형식.
    /// </summary>
    public enum LogFileMode
    {
        TXT,
        CSV,
        ZIP,
    }

    /// <summary>
    /// 테스트 실행 결과에 따르는 프린팅 설정.
    /// </summary>
    public enum PrintingOptions
    {
        All,
        FailOnly,
        None
    }

    /// <summary>
    /// MES 를 사용할 때, 시험 결과를 MES로 전송할 것인지 결정.
    /// </summary>
    public enum MesResultAction
    {
        /// <summary>
        /// 시험 결과 MES 전송.
        /// </summary>
        MES = 0,

        /// <summary>
        /// MES 전송하지 않음.
        /// </summary>
        Stop,

        /// <summary>
        /// MES 전송 여부를 작업자에게 문의.
        /// </summary>
        Operator
    }

    /// <summary>
    /// 검사이력을 보관하는 기간을 표현하는데 사용된다.
    /// </summary>
    public enum TimespanUnit
    {
        Days,
        Weeks,
        Months,
        Years
    }

    /// <summary>
    /// 테스트 결과 NG일 때 동작 정의.
    /// </summary>
    public enum TestFailAction
    {
        Continue,       // 테스트 계속 진행
        Stop,           // 테스트 정지
        StopAndAlarm,   // 테스트 정지, 경광등 켜기.
    }

    /// <summary>
    /// 앞공정 NG 시의 처리 동작.
    /// </summary>
    public enum PrevProcFailAction
    {
        Stop,       // 테스트 정지.
        TestNg,     // 테스트 NG 처리.
    }

    /// <summary>
    /// MES에서 받은 FGCODE와 제품정보 테이블의 FGCODE를 비교하는 방식.
    /// </summary>
    public enum FgcodeCheckMode
    {
        /// <summary>
        /// FGCODE 검사 안함.
        /// </summary>
        None = 0,

        /// <summary>
        /// FGCODE 일치 여부 검사.
        /// </summary>
        Check,
    }

    /// <summary>
    /// 직렬연결 옵션.
    /// </summary>
    public enum SeriesOption
    {
        None,           // 직렬연결 안함.
        SeriesPrev,     // 직렬연결 1번 설비.
        SeriesNext,     // 직렬연결 2번 설비.
    }

    /// <summary>
    /// 테스트 섹션이 NG인 경우 테스트를 계속 진행할 것인지 여부.
    /// </summary>
    public enum SectionFailAction
    {
        Stop,                   // 테스트 정지.
        Continue,               // 테스트 계속 진행.
        ContinueNoShort,        // 테스트 계속 진행(Short, Open NG 제외).
        ContinueMaster,         // Master Good/Fail 보드의 경우에만 계속 진행.
        ContinueMasterNoShort,  // Master Good/Fail 보드의 경우에만 계속 진행(Short, Open NG 제외).
    }

    static class AppSettings
    {
        // App 설정을 Configuration 인스턴스.
        private static Configuration appConfig = null;

        // 공유하여 사용하는 User Manager 인스턴스.
        private static UserManager sharedUserManager;

        /// <summary>
        /// 로그인한 유저.
        /// </summary>
        internal static User LoggedUser { get; set; }

        /// <summary>
        /// <see cref="UserManager"/> 인스턴스를 생성한다.
        /// </summary>
        internal static UserManager GetUserManager()
        {
            if (sharedUserManager == null)
            {
                sharedUserManager = UserManager.Load();
            }

            return sharedUserManager;
        }

        /// <summary>
        /// Logout 처리를 진행한다.
        /// </summary>
        internal static void Logout()
        {
            LoggedUser = null;
            sharedUserManager = null;
        }

        /// <summary>
        /// JTAG 사용 여부.
        /// </summary>
        internal static bool UseJtag
        {
            get => GetBoolValue(nameof(UseJtag)) ?? false;
            set => SaveValue(nameof(UseJtag), value);
        }

        /// <summary>
        /// DIO 수동모드 작동 여부.
        /// </summary>
        internal static bool DioManualMode
        {
            get => GetBoolValue(nameof(DioManualMode)) ?? false;
            set => SaveValue(nameof(DioManualMode), value);
        }

        /// <summary>
        /// Section Fail일 때 테스트를 계속 진행할지 여부.
        /// </summary>
        internal static SectionFailAction SectionFailAction
        {
            get
            {
                string savedValue = GetValue(nameof(SectionFailAction));
                if (Enum.TryParse(savedValue, out SectionFailAction option))
                {
                    return option;
                }
                else
                {
                    return SectionFailAction.Stop;
                }
            }
            set => SaveValue(nameof(SectionFailAction), value.ToString());
        }

        /// <summary>
        /// 직렬연결 서버 포트.
        /// </summary>
        internal static int SeriesServerPort
        {
            get => GetIntValue(nameof(SeriesServerPort)) ?? 57391;
            set => SaveValue(nameof(SeriesServerPort), value);
        }

        /// <summary>
        /// 직렬연결 서버(뒷설비).
        /// </summary>
        internal static string SeriesServer
        {
            get => GetValue(nameof(SeriesServer));
            set => SaveValue(nameof(SeriesServer), value);
        }

        /// <summary>
        /// 직렬연결을 위한 소켓 통신 타임아웃.
        /// </summary>
        internal static int SeriesCommTimeout
        {
            get => GetIntValue(nameof(SeriesCommTimeout)) ?? 3000;
            set => SaveValue(nameof(SeriesCommTimeout), value);
        }

        /// <summary>
        /// 직렬연결 설정.
        /// </summary>
        internal static SeriesOption SeriesConnOption
        {
            get
            {
                string savedValue = GetValue(nameof(SeriesConnOption));
                if (Enum.TryParse(savedValue, out SeriesOption option))
                {
                    return option;
                }
                else
                {
                    return SeriesOption.None;
                }
            }
            set => SaveValue(nameof(SeriesConnOption), value.ToString());
        }

        /// <summary>
        /// 앞공정 NG 시 어떻게 처리할 것인지 설정.
        /// </summary>
        internal static PrevProcFailAction PrevFailAction
        {
            get
            {
                string savedValue = GetValue(nameof(PrevFailAction));
                if (Enum.TryParse(savedValue, out PrevProcFailAction action))
                {
                    return action;
                }
                else
                {
                    return PrevProcFailAction.Stop;
                }
            }
            set => SaveValue(nameof(PrevFailAction), value.ToString());
        }

        /// <summary>
        /// 테스트결과가 NG인 경우 어떻게 처리할 것인지 설정.
        /// </summary>
        internal static TestFailAction TestFailAction
        {
            get
            {
                string savedValue = GetValue(nameof(TestFailAction));
                if (Enum.TryParse(savedValue, out TestFailAction action))
                {
                    return action;
                }
                else
                {
                    return TestFailAction.Continue;
                }
            }
            set => SaveValue(nameof(TestFailAction), value.ToString());
        }

        /// <summary>
        /// 테스트 이력을 보관하는 기간.
        /// </summary>
        internal static int HistoryKeepingPeriod
        {
            get => GetIntValue(nameof(HistoryKeepingPeriod)) ?? 1;
            set => SaveValue(nameof(HistoryKeepingPeriod), value);
        }

        /// <summary>
        /// 테스트 이력을 보관하는 기간의 단위.
        /// </summary>
        internal static TimespanUnit HistoryKeepingUnit
        {
            get
            {
                string savedValue = GetValue(nameof(HistoryKeepingUnit));
                if (Enum.TryParse(savedValue, out TimespanUnit unit))
                {
                    return unit;
                }
                else
                {
                    return TimespanUnit.Months;
                }
            }
            set
            {
                SaveValue(nameof(HistoryKeepingUnit), value.ToString());
            }
        }

        /// <summary>
        /// 테스트의 FAIL 스텝 정보들을 Notepad에 보여줄 것인지 여부.
        /// </summary>
        internal static bool ShowFailInfoNotepad
        {
            get => GetBoolValue(nameof(ShowFailInfoNotepad)) ?? true;
            set => SaveValue(nameof(ShowFailInfoNotepad), value);
        }

        /// <summary>
        /// 테스트가 실패할 경우 PCB Viewer를 보여줄 것인지 여부.
        /// </summary>
        internal static bool ShowPcbViewer
        {
            get => GetBoolValue(nameof(ShowPcbViewer)) ?? false;
            set => SaveValue(nameof(ShowPcbViewer), value);
        }

        /// <summary>
        /// PCB Viewer EXE file path.
        /// </summary>
        internal static string PcbViewerPath
        {
            get => GetValue(nameof(PcbViewerPath));
            set => SaveValue(nameof(PcbViewerPath), value);
        }

        /// <summary>
        /// 바코드에서 차종코드 시작 위치, 1부터 시작.
        /// </summary>
        internal static int CarTypeCodeStartPosition
        {
            get => GetIntValue(nameof(CarTypeCodeStartPosition)) ?? 6;
            set
            {
                SaveValue(nameof(CarTypeCodeStartPosition), value);
            }
        }

        /// <summary>
        /// 바코드에서 차종코드 길이.
        /// </summary>
        internal static int CarTypeCodeLength
        {
            get => GetIntValue(nameof(CarTypeCodeLength)) ?? 3;
            set
            {
                SaveValue(nameof(CarTypeCodeLength), value);
            }
        }

        /// <summary>
        /// 로그파일 저장 형식.
        /// </summary>
        internal static LogFileMode LogFileMode
        {
            get
            {
                string savedValue = GetValue(nameof(LogFileMode));
                if (Enum.TryParse(savedValue, out LogFileMode mode))
                {
                    return mode;
                }
                else
                {
                    return LogFileMode.TXT;
                }
            }
            set
            {
                SaveValue(nameof(LogFileMode), value.ToString());
            }
        }

        /// <summary>
        /// 프로젝트를 오픈할 때 FID를 스캔하여 검사할 것인지 여부.
        /// </summary>
        internal static bool ShouldCheckFid
        {
            get => GetBoolValue(nameof(ShouldCheckFid)) ?? true;
            set
            {
                SaveValue(nameof(ShouldCheckFid), value);
            }
        }

        /// <summary>
        /// 바코드 읽기 오류 시 재시도 횟수.
        /// </summary>
        internal static int BarcodeRetryCount
        {
            get => GetIntValue(nameof(BarcodeRetryCount)) ?? 2;
            set
            {
                SaveValue(nameof(BarcodeRetryCount), value);
            }
        }

        /// <summary>
        /// 바코드 읽기 재시도 간격, ms.
        /// </summary>
        internal static int BarcodeRetryInterval
        {
            get => GetIntValue(nameof(BarcodeRetryInterval)) ?? 0;
            set
            {
                SaveValue(nameof(BarcodeRetryInterval), value);
            }
        }

        /// <summary>
        /// 현재의 FID를 저장하기 위한 변수.
        /// </summary>
        internal static int CurrentFid
        {
            get => GetIntValue(nameof(CurrentFid)) ?? 0;
            set
            {
                SaveValue(nameof(CurrentFid), value);
            }
        }

        /// <summary>
        /// 테스트 시작 시 소리 모드.
        /// </summary>
        internal static int StartSoundMode
        {
            get => GetIntValue(nameof(StartSoundMode)) ?? 0;
            set
            {
                SaveValue(nameof(StartSoundMode), value);
            }
        }

        /// <summary>
        /// 테스트 시작 시 Start1, Start2 버튼이 눌렸을 때 소리 파일.
        /// </summary>
        internal static string StartSoundFile
        {
            get => GetValue(nameof(StartSoundFile));
            set
            {
                SaveValue(nameof(StartSoundFile), value);
            }
        }

        /// <summary>
        /// MES 처리결과를 알려주는 Sound 플레이 모드.
        /// </summary>
        internal static int MesSoundPlayMode
        {
            get => GetIntValue(nameof(MesSoundPlayMode)) ?? 0;
            set
            {
                SaveValue(nameof(MesSoundPlayMode), value);
            }
        }

        /// <summary>
        /// MES M2 응답이 OK일 때 플레이할 오디오 파일 경로.
        /// </summary>
        internal static string MesM2OkSoundFile
        {
            get => GetValue(nameof(MesM2OkSoundFile));
            set
            {
                SaveValue(nameof(MesM2OkSoundFile), value);
            }
        }

        /// <summary>
        /// MES M2 응답이 NG일 때 플레이할 오디오 파일 경로.
        /// </summary>
        internal static string MesM2NgSoundFile
        {
            get => GetValue(nameof(MesM2NgSoundFile));
            set
            {
                SaveValue(nameof(MesM2NgSoundFile), value);
            }
        }

        /// <summary>
        /// MES M4 응답이 OK일 때 플레이할 오디오 파일 경로.
        /// </summary>
        internal static string MesM4OkSoundFile
        {
            get => GetValue(nameof(MesM4OkSoundFile));
            set
            {
                SaveValue(nameof(MesM4OkSoundFile), value);
            }
        }

        /// <summary>
        /// MES M4 응답이 NG일 때 플레이할 오디오 파일 경로.
        /// </summary>
        internal static string MesM4NgSoundFile
        {
            get => GetValue(nameof(MesM4NgSoundFile));
            set
            {
                SaveValue(nameof(MesM4NgSoundFile), value);
            }
        }

        /// <summary>
        /// 테스트 결과를 알려주는 Sound 플레이 모드.
        /// </summary>
        internal static int ResultSoundPlayMode
        {
            get => GetIntValue(nameof(ResultSoundPlayMode)) ?? 0;
            set
            {
                SaveValue(nameof(ResultSoundPlayMode), value);
            }
        }

        /// <summary>
        /// 테스트 결과가 양품일 때 플레이할 사운드 파일 경로.
        /// </summary>
        internal static string ResultPassSoundFile
        {
            get => GetValue(nameof(ResultPassSoundFile));
            set
            {
                SaveValue(nameof(ResultPassSoundFile), value);
            }
        }

        /// <summary>
        /// 테스트 결과가 불량일 때 플레이할 사운드 파일 경로.
        /// </summary>
        internal static string ResultFailSoundFile
        {
            get => GetValue(nameof(ResultFailSoundFile));
            set
            {
                SaveValue(nameof(ResultFailSoundFile), value);
            }
        }

        /// <summary>
        /// TEST Log를 저장할 때, 테스트 상하한 값을 퍼센트로 저장할 것인지 설정한다.
        /// </summary>
        internal static bool ResultValueLimitAsPercent
        {
            get => GetBoolValue(nameof(ResultValueLimitAsPercent)) ?? true;
            set
            {
                SaveValue(nameof(ResultValueLimitAsPercent), value);
            }
        }

        /// <summary>
        /// Fixture Probe Count 관리 모드.
        /// true이면 Sync, false이면 Async 이다.
        /// </summary>
        internal static bool ProbeCountSyncMode
        {
            get => GetBoolValue(nameof(ProbeCountSyncMode)) ?? true;
            set
            {
                SaveValue(nameof(ProbeCountSyncMode), value);
            }
        }

        /// <summary>
        /// 시험이 실패할 경우, 재시도 횟수.
        /// </summary>
        internal static int TestRetryCount
        {
            get => GetIntValue(nameof(TestRetryCount)) ?? 2;
            set
            {
                SaveValue(nameof(TestRetryCount), value);
            }
        }

        /// <summary>
        /// 불량 마스터 시험이 실패할 경우 재시도 횟수.
        /// </summary>
        internal static int NgMasterRetryCount
        {
            get => GetIntValue(nameof(NgMasterRetryCount)) ?? 0;
            set => SaveValue(nameof(NgMasterRetryCount), value);
        }

        /// <summary>
        /// 시험이 실패할 경우, 얼마동안 간격을 두고 다시 시도할 것인지 설정, ms단위.
        /// </summary>
        internal static int TestRetryInterval
        {
            get => GetIntValue(nameof(TestRetryInterval)) ?? 1_000;
            set
            {
                SaveValue(nameof(TestRetryInterval), value);
            }
        }

        /// <summary>
        /// 테스트 종료 후 결과를 MES로 보낼 것인지 설정.
        /// </summary>
        internal static MesResultAction MesReportAction
        {
            get
            {
                string savedValue = GetValue(nameof(MesReportAction));
                try
                {
                    return (MesResultAction)Enum.Parse(typeof(MesResultAction), savedValue);
                }
                catch
                {
                    return MesResultAction.MES;
                }
            }
            set
            {
                SaveValue(nameof(MesReportAction), value.ToString());
            }
        }

        /// <summary>
        /// MES Message ID.
        /// </summary>
        internal static string MesMessageId
        {
            get => GetValue(nameof(MesMessageId));
            set
            {
                SaveValue(nameof(MesMessageId), value);
            }
        }

        /// <summary>
        /// MES PC ID.
        /// </summary>
        internal static string MesPcId
        {
            get => GetValue(nameof(MesPcId));
            set
            {
                SaveValue(nameof(MesPcId), value);
            }
        }

        /// <summary>
        /// MES Factory ID.
        /// </summary>
        internal static string MesFactoryId
        {
            get => GetValue(nameof(MesFactoryId));
            set
            {
                SaveValue(nameof(MesFactoryId), value);
            }
        }

        /// <summary>
        /// MES Line ID.
        /// </summary>
        internal static string MesLineId
        {
            get => GetValue(nameof(MesLineId));
            set
            {
                SaveValue(nameof(MesLineId), value);
            }
        }

        /// <summary>
        /// MES Oper ID.
        /// </summary>
        internal static string MesOperId
        {
            get => GetValue(nameof(MesOperId));
            set
            {
                SaveValue(nameof(MesOperId), value);
            }
        }

        /// <summary>
        /// MES Equipment ID.
        /// </summary>
        internal static string MesEquipmentId
        {
            get => GetValue(nameof(MesEquipmentId));
            set
            {
                SaveValue(nameof(MesEquipmentId), value);
            }
        }

        /// <summary>
        /// MES Barcode Type.
        /// </summary>
        internal static MesMessage.Barcode MesBarcodeType
        {
            get
            {
                string savedValue = GetValue(nameof(MesBarcodeType));
                try
                {
                    var type = (MesMessage.Barcode)Enum.Parse(typeof(MesMessage.Barcode), savedValue);
                    return type;
                }
                catch
                {
                    // Do nothing.
                }
                return MesMessage.Barcode.PCB;
            }
            set
            {
                SaveValue(nameof(MesBarcodeType), value.ToString());
            }
        }

        /// <summary>
        /// MES ICT Server Listening Port.
        /// </summary>
        internal static int MesIctServerPort
        {
            get => GetIntValue(nameof(MesIctServerPort)) ?? 9000;
            set
            {
                SaveValue(nameof(MesIctServerPort), value);
            }
        }

        /// <summary>
        /// MES EOL Server Listening Port.
        /// </summary>
        internal static int MesEolServerPort
        {
            get => GetIntValue(nameof(MesEolServerPort)) ?? 9001;
            set
            {
                SaveValue(nameof(MesEolServerPort), value);
            }
        }

        /// <summary>
        /// MES T3 timeout(Request -> Response) in milliseconds.
        /// </summary>
        internal static int MesT3Timeout
        {
            get => GetIntValue(nameof(MesT3Timeout)) ?? 10_000;
            set
            {
                SaveValue(nameof(MesT3Timeout), value);
            }
        }

        /// <summary>
        /// MES Keep-Alive timeout in milliseconds.
        /// </summary>
        internal static int MesKeepAliveTimeout
        {
            get => GetIntValue(nameof(MesKeepAliveTimeout)) ?? 5_000;
            set
            {
                SaveValue(nameof(MesKeepAliveTimeout), value);
            }
        }

        /// <summary>
        /// MES와 연동할 것인지를 나타낸다.
        /// </summary>
        internal static bool MesEnabled
        {
            get => GetBoolValue(nameof(MesEnabled)) ?? true;
            set
            {
                SaveValue(nameof(MesEnabled), value);
            }
        }

        /// <summary>
        /// MES M2 응답의 MES Result가 NG인 경우, 다시 시도할 때까지의 시간, ms.
        /// </summary>
        internal static int MesM1RetryInterval
        {
            get => GetIntValue(nameof(MesM1RetryInterval)) ?? 1_000;
            set
            {
                SaveValue(nameof(MesM1RetryInterval), value);
            }
        }

        /// <summary>
        /// MES M2 응답의 MES Result가 NG인 경우, 다시 시도할 횟수.
        /// </summary>
        internal static int MesM1RetryCount
        {
            get => GetIntValue(nameof(MesM1RetryCount)) ?? 2;
            set
            {
                SaveValue(nameof(MesM1RetryCount), value);
            }
        }

        /// <summary>
        /// MES M4 응답의 MES Result가 NG인 경우, 다시 시도할 때까지의 시간, ms.
        /// </summary>
        internal static int MesM3RetryInterval
        {
            get => GetIntValue(nameof(MesM3RetryInterval)) ?? 1_000;
            set
            {
                SaveValue(nameof(MesM3RetryInterval), value);
            }
        }

        /// <summary>
        /// MES M4 응답의 MES Result가 NG인 경우, 다시 시도할 횟수.
        /// </summary>
        internal static int MesM3RetryCount
        {
            get => GetIntValue(nameof(MesM3RetryCount)) ?? 2;
            set
            {
                SaveValue(nameof(MesM3RetryCount), value);
            }
        }

        /// <summary>
        /// MES에서 받은 FGCODE와 제품정보 테이블의 FGCODE를 비교하는 방식.
        /// </summary>
        internal static FgcodeCheckMode FgcodeCheckMethod
        {
            get
            {
                string savedValue = GetValue(nameof(FgcodeCheckMethod));
                try
                {
                    return (FgcodeCheckMode)Enum.Parse(typeof(FgcodeCheckMode), savedValue);
                }
                catch
                {
                    return FgcodeCheckMode.Check;
                }
            }
            set
            {
                SaveValue(nameof(FgcodeCheckMethod), value.ToString());
            }
        }

        /// <summary>
        /// 프린트 폰트 크기.
        /// </summary>
        internal static int PrintFontSize
        {
            get => GetIntValue(nameof(PrintFontSize)) ?? 8;
            set
            {
                SaveValue(nameof(PrintFontSize), value);
            }
        }

        /// <summary>
        /// Log text box에 출력하는 스텝 최대 개수.
        /// 0과 같거나 작으면 모든 스텝을 로깅한다.
        /// </summary>
        internal static int LoggingStepCount
        {
            get => GetIntValue(nameof(LoggingStepCount)) ?? 0;
            set
            {
                SaveValue(nameof(LoggingStepCount), value);
            }
        }

        /// <summary>
        /// 프린트를 어떻게 할 것인지 설정하는 옵션.
        /// </summary>
        internal static PrintingOptions PrintingMode
        {
            get
            {
                string savedValue = GetValue(nameof(PrintingMode));
                try
                {
                    var mode = (PrintingOptions)Enum.Parse(typeof(PrintingOptions), savedValue);
                    return mode;
                }
                catch
                {
                    // Do nothing.
                }
                return PrintingOptions.All;
            }
            set
            {
                SaveValue(nameof(PrintingMode), value.ToString());
            }
        }

        /// <summary>
        /// Eloz Log Mode.
        /// </summary>
        internal static Project.LogMode ElozLogMode
        {
            get
            {
                string savedValue = GetValue(nameof(ElozLogMode));
                try
                {
                    var mode = (Project.LogMode)Enum.Parse(typeof(Project.LogMode), savedValue);
                    return mode;
                }
                catch (Exception)
                {
                    // Do nothing.
                }
                return Project.LogMode.LogAlways;
            }
            set
            {
                SaveValue(nameof(ElozLogMode), value.ToString());
            }
        }

        /// <summary>
        /// DMM 디바이스 연결에 사용하는 주소.
        /// </summary>
        internal static string DmmResourceName
        {
            get => GetValue(nameof(DmmResourceName)) ?? "";
            set
            {
                SaveValue(nameof(DmmResourceName), value);
            }
        }

        /// <summary>
        /// 테스트를 자동으로 다시 시작할 것인지 설정하는 깃발.
        /// </summary>
        internal static bool AutoRestartTest
        {
            get => GetBoolValue(nameof(AutoRestartTest)) ?? true;
            set
            {
                SaveValue(nameof(AutoRestartTest), value);
            }
        }

        /// <summary>
        /// 테스트 시작 시 공압을 체크할 것인지 결정한다.
        /// </summary>
        internal static bool CheckPneumatic
        {
            get => GetBoolValue(nameof(CheckPneumatic)) ?? true;
            set
            {
                SaveValue(nameof(CheckPneumatic), value);
            }
        }

        /// <summary>
        /// 테스트 시작 시 도어 오픈 상태를 체크할 것인지 결정한다.
        /// </summary>
        internal static bool CheckDoorOpenStatus
        {
            get => GetBoolValue(nameof(CheckDoorOpenStatus)) ?? true;
            set
            {
                SaveValue(nameof(CheckDoorOpenStatus), value);
            }
        }

        /// <summary>
        /// 테스트 시작 시 자동 모드인가를 체크할 것인지 결정한다.
        /// </summary>
        internal static bool CheckAutoMode
        {
            get => GetBoolValue(nameof(CheckAutoMode)) ?? true;
            set
            {
                SaveValue(nameof(CheckAutoMode), value);
            }
        }

        /// <summary>
        /// Novaflash 연결에 LAN을 사용할 것인지 여부.
        /// </summary>
        internal static bool NovaflashUseLan
        {
            get => GetBoolValue(nameof(NovaflashUseLan)) ?? true;
            set
            {
                SaveValue(nameof(NovaflashUseLan), value);
            }
        }

        /// <summary>
        /// Novaflash 디바이스 LAN IP.
        /// </summary>
        internal static string NovaflashLanIp
        {
            get => GetValue(nameof(NovaflashLanIp)) ?? "192.168.1.100";
            set
            {
                SaveValue(nameof(NovaflashLanIp), value);
            }
        }

        /// <summary>
        /// Novaflash 시리얼 포트 이름.
        /// </summary>
        internal static string NovaflashSerialPortName
        {
            get => GetValue(nameof(NovaflashSerialPortName));
            set
            {
                SaveValue(nameof(NovaflashSerialPortName), value);
            }
        }

        /// <summary>
        /// Novaflash 시리얼 통신 속도.
        /// </summary>
        internal static int NovaflashSerialBaudRate
        {
            get => GetIntValue(nameof(NovaflashSerialBaudRate)) ?? 115200;
            set
            {
                SaveValue(nameof(NovaflashSerialBaudRate), value);
            }
        }

        /// <summary>
        /// 프로그램 실행 중 출력된 로그 파일 경로.
        /// </summary>
        internal static string AppLogFilePath
        {
            get
            {
                string savedValue = GetValue(nameof(AppLogFilePath));
                if (string.IsNullOrEmpty(savedValue))
                {
                    return Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location), $"{Assembly.GetExecutingAssembly().GetName().Name}.log");
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(AppLogFilePath), value);
            }
        }

        /// <summary>
        /// 텍스트박스에 출력한 시험 로그 저장 폴더.
        /// </summary>
        internal static string PrintLogFolder
        {
            get
            {
                string savedValue = GetValue(nameof(PrintLogFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return "D:\\ICT_Logs\\PrintLog";
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(PrintLogFolder), value);
            }
        }

        /// <summary>
        /// 시험 이력 저장 로컬 폴더.
        /// </summary>
        internal static string TestLogLocalFolder
        {
            get
            {
                string savedValue = GetValue(nameof(TestLogLocalFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return "D:\\ICT_Logs\\Local";
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(TestLogLocalFolder), value);
            }
        }

        /// <summary>
        /// 시험 이력 저장 MES 폴더.
        /// </summary>
        internal static string TestLogMesFolder
        {
            get
            {
                string savedValue = GetValue(nameof(TestLogMesFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return "D:\\ICT_Logs\\MES";
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(TestLogMesFolder), value);
            }
        }

        /// <summary>
        /// 시험 이력 저장 기타 폴더.
        /// </summary>
        internal static string TestLogOtherFolder
        {
            get
            {
                string savedValue = GetValue(nameof(TestLogOtherFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return "D:\\ICT_Logs\\Other";
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(TestLogOtherFolder), value);
            }
        }

        /// <summary>
        /// 시험 이력 저장 폴더 이름.
        /// </summary>
        internal static string TestLogFolderName
        {
            get => GetValue(nameof(TestLogFolderName)) ?? "insp_log";
            set
            {
                SaveValue(nameof(TestLogFolderName), value);
            }
        }

        /// <summary>
        /// 바코드를 읽을 것인지 설정한다.
        /// </summary>
        internal static bool ShouldScanBarcode
        {
            get => GetBoolValue(nameof(ShouldScanBarcode)) ?? true;
            set
            {
                SaveValue(nameof(ShouldScanBarcode), value);
            }
        }

        /// <summary>
        /// 프로젝트 파일들이 있는 경로.
        /// </summary>
        internal static string ProjectFolder
        {
            get
            {
                string savedValue = GetValue(nameof(ProjectFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return "D:\\ICT_Projects";
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(ProjectFolder), value);
            }
        }

        /// <summary>
        /// Novaflash 파일 경로.
        /// </summary>
        internal static string NovaflashFolder
        {
            get
            {
                string savedValue = GetValue(nameof(NovaflashFolder));
                if (string.IsNullOrEmpty(savedValue))
                {
                    //return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location);
                    return Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "NovaFlash\\Projects");
                }
                return savedValue;
            }
            set
            {
                SaveValue(nameof(NovaflashFolder), value);
            }
        }

        /// <summary>
        /// 경고 출력의 기준이 되는 최대 허용 Probe 수.
        /// </summary>
        internal static int MaxProbeCount
        {
            get
            {
                var savedValue = GetIntValue(nameof(MaxProbeCount));
                if (savedValue > 0)
                {
                    return savedValue.GetValueOrDefault();
                }
                else
                {
                    return 30_000;
                }
            }
            set
            {
                SaveValue(nameof(MaxProbeCount), value);
            }
        }

        /// <summary>
        /// App 설정 파일을 오픈한다.
        /// </summary>
        /// <returns></returns>
        internal static Configuration OpenConfig()
        {
            if (appConfig == null)
            {
                //appConfig = ConfigurationManager.OpenExeConfiguration(ConfigurationUserLevel.None);
                appConfig = ConfigurationManager.OpenExeConfiguration(Assembly.GetExecutingAssembly().Location);
            }

            return appConfig;
        }

        /// <summary>
        /// 지정한 키에 해당하는 int값을 애플리케이션 설정 파일에서 읽는다.
        /// </summary>
        /// <param name="key">읽으려는 값의 키.</param>
        /// <returns>키가 존재하면 그 값을, 오류가 있거나 키가 없으면 null을 리턴한다.</returns>
        internal static string GetValue(string key)
        {
            string value = null;
            try
            {
                Configuration appConfig = OpenConfig();
                KeyValueConfigurationElement element = appConfig.AppSettings.Settings[key];
                value = element?.Value;
            }
            catch (ConfigurationErrorsException)
            {
                // Do nothing.
            }
            return value;
        }

        /// <summary>
        /// 지정한 키에 해당하는 int값을 애플리케이션 설정 파일에서 읽는다.
        /// </summary>
        /// <param name="key">읽으려는 값의 키.</param>
        /// <returns>키가 존재하면 그 값을, 오류가 있거나 키가 없으면 null을 리턴한다.</returns>
        internal static int? GetIntValue(string key)
        {
            int? value = null;
            if (int.TryParse(GetValue(key), out int parsed))
            {
                value = parsed;
            }
            return value;
        }

        /// <summary>
        /// 지정한 키에 해당하는 int값을 애플리케이션 설정 파일에서 읽는다.
        /// </summary>
        /// <param name="key">읽으려는 값의 키.</param>
        /// <returns>키가 존재하면 그 값을, 오류가 있거나 키가 없으면 null을 리턴한다.</returns>
        internal static bool? GetBoolValue(string key)
        {
            bool? value = null;
            if (bool.TryParse(GetValue(key), out bool parsed))
            {
                value = parsed;
            }
            return value;
        }

        /// <summary>
        /// 지정한 키에 해당하는 Color값을 애플리케이션 설정 파일에서 읽는다.
        /// </summary>
        /// <param name="key">읽으려는 값의 키.</param>
        /// <returns>키가 존재하면 그 값을, 오류가 있거나 키가 없으면 null을 리턴한다.</returns>
        internal static Color? GetColorValue(string key)
        {
            Color? value = null;
            string valueString = GetValue(key);
            if (!string.IsNullOrEmpty(valueString))
            {
                try
                {
                    value = ColorTranslator.FromHtml(valueString);
                }
                catch (Exception)
                {
                    // Do nothing
                }
            }
            return value;
        }

        /// <summary>
        /// 지정한 키와 값의 쌍을 애플리케이션 설정 파일에 저장한다.
        /// </summary>
        /// <param name="key">저장하려는 키.</param>
        /// <param name="value">키에 해당하는 값.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal static bool SaveValue(string key, string value)
        {
            try
            {
                Configuration appConfig = OpenConfig();
                KeyValueConfigurationElement element = appConfig.AppSettings.Settings[key];
                if (element != null)
                {
                    element.Value = value;
                }
                else
                {
                    appConfig.AppSettings.Settings.Add(key, value);
                }
                appConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(appConfig.AppSettings.SectionInformation.Name);
                return true;
            }
            catch (ConfigurationErrorsException)
            {
                return false;
            }
        }

        /// <summary>
        /// 지정한 키와 값의 쌍을 애플리케이션 설정 파일에 저장한다.
        /// </summary>
        /// <param name="key">저장하려는 키.</param>
        /// <param name="value">키에 해당하는 값.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal static bool SaveValue(string key, int value)
        {
            return SaveValue(key, $"{value}");
        }

        /// <summary>
        /// 지정한 키와 값의 쌍을 애플리케이션 설정 파일에 저장한다.
        /// </summary>
        /// <param name="key">저장하려는 키.</param>
        /// <param name="value">키에 해당하는 값.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal static bool SaveValue(string key, bool value)
        {
            return SaveValue(key, $"{value}");
        }

        /// <summary>
        /// 지정한 키와 값의 쌍을 애플리케이션 설정 파일에 저장한다.
        /// </summary>
        /// <param name="key">저장하려는 키.</param>
        /// <param name="value">키에 해당하는 값.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal static bool SaveValue(string key, Color value)
        {
            return SaveValue(key, ColorTranslator.ToHtml(value));
        }

        /// <summary>
        /// 지정한 키를 가진 값을 애플리케이션 설정 파일에서 지운다.
        /// </summary>
        /// <param name="key">지우려는 값의 키.</param>
        /// <returns>성공하면 true, 실패하면 false.</returns>
        internal static bool RemoveValue(string key)
        {
            try
            {
                Configuration appConfig = OpenConfig();
                appConfig.AppSettings.Settings.Remove(key);
                appConfig.Save(ConfigurationSaveMode.Modified);
                ConfigurationManager.RefreshSection(appConfig.AppSettings.SectionInformation.Name);
                return true;
            }
            catch (ConfigurationErrorsException)
            {
                return false;
            }
        }

        /// <summary>
        /// 전체 설정을 초기화한다.
        /// </summary>
        internal static void ResetSettings()
        {
            //RemoveValue(nameof(HardwareIndex));
        }
    }
}
