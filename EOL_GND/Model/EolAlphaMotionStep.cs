using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.ComponentModel;
using Ivi.Visa;
using Peak.Lin;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class EolAlphaMotionStep : EolStep
    {
        public enum AlphaMotionMethod
        {
            SingleAxisMove,
            MultipleAxisMove,
            Wait,
            Initialize,
            Deinitialize,
        }

        [Category(MethodCategory), 
            Description("테스트 방법을 설정합니다.")]
        public AlphaMotionMethod TestMethod
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
        private AlphaMotionMethod _testMethod = AlphaMotionMethod.SingleAxisMove;

        [Category(MethodCategory), Browsable(true), DefaultValue(0),
            Description("Controller 번호. Controller 번호는 0 ~ {Controller 개수 - 1} 사이의 값을 가질 수 있습니다.")]
        public int ControllerIndex
        {
            get => _controllerIndex;
            set
            {
                if (_controllerIndex != value)
                {
                    _controllerIndex = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int _controllerIndex = 0;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntArrayConverter)),
            Description("축 번호 리스트. 축 번호는 0 ~ {축 개수 - 1} 사이의 값을 가질 수 있습니다. 예: 0-3, 5, 7, 12-17")]
        public int[] Axes
        {
            get => _axes;
            set
            {
                if (_axes != value)
                {
                    _axes = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _axes;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntArrayConverter)),
            DisplayName(nameof(Velocities) + " [pps]"),
            Description("속도 리스트. 0 ~ 6,553,500")]
        public int[] Velocities
        {
            get => _velocities;
            set
            {
                if (_velocities != value)
                {
                    _velocities = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _velocities;

        public enum PositioningMethod
        {
            Absolute,
            Relative,
        }

        [Category(MethodCategory), Browsable(true), DefaultValue(PositioningMethod.Absolute),
            Description("위치 지정 방식을 설정합니다.")]
        public PositioningMethod PositionMode
        {
            get => _positionMode;
            set
            {
                if (_positionMode != value)
                {
                    _positionMode = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private PositioningMethod _positionMode = PositioningMethod.Absolute;

        [Category(MethodCategory), Browsable(true), TypeConverter(typeof(IntArrayConverter)),
            DisplayName(nameof(Positions) + " [pulse]"),
            Description("이동시킬 위치 리스트를 설정합니다. 축 번호 리스트 수와 같아야 합니다. -134,217,728 ~ 134,217,727")]
        public int[] Positions
        {
            get => _positions;
            set
            {
                if (_positions != value)
                {
                    _positions = value;
                    NotifyPropertyChanged();
                }
            }
        }
        private int[] _positions;

        //
        // Summary 표시를 위한 변수들.
        //

        internal override string CategoryName => StepCategory.AlphaMotion.GetText();
        public override string TestModeDesc => TestMethod.ToString();
        public override string ParameterDesc => $"{ControllerIndex} : {(Axes == null ? "" : string.Join(", ", Axes))}";
        public override string ExpectedValueDesc => null;
        public override string TolerancePlusDesc => null;
        public override string ToleranceMinusDesc => null;
        public override List<int> AllTestChannels => null;

        private EolAlphaMotionStep()
        {
            Name = StepCategory.AlphaMotion.GetText();
        }

        public EolAlphaMotionStep(string deviceName) : this()
        {
            DeviceName = deviceName;
        }

        public override TestDevice CreateDevice()
        {
            throw new NotSupportedException();
        }

        public override IEnumerable<string> GetDeviceNames()
        {
            throw new NotSupportedException();
        }

        public override ICollection GetTestModes()
        {
            throw new NotSupportedException();
        }

        public override bool TryParseTestMode(object value, out object testMode)
        {
            throw new NotSupportedException();
        }

        protected override void RelayOn(object elozTestSet, DeviceSetting setting)
        {
        }

        public override void GetNominalValues(out double? nominalValue, out double? upperLimit, out double? lowerLimit)
        {
            nominalValue = null;
            upperLimit = null;
            lowerLimit = null;
        }

        // 모션 라이브러리 에러 코드를 파싱하여 성공이면 true, 실패이면 false를 리턴한다.
        private bool PmiCheckResult(int errorCode, out string errorMessage)
        {
            switch (errorCode)
            {
                case pmiMApiDefs.TMC_RV_OK: // Success
                    errorMessage = "Success";
                    break;
                case pmiMApiDefs.TMC_RV_MOT_INIT_ERR:
                    errorMessage = "라이브러리 초기화 실패";
                    break;
                case pmiMApiDefs.TMC_RV_MOT_FILE_SAVE_ERR:
                    errorMessage = "모션 설정값을 저장하는 파일 저장에 실패함";
                    break;
                case pmiMApiDefs.TMC_RV_MOT_FILE_LOAD_ERR:
                    errorMessage = "모션 설정값이 저장된 파일이 로드가 안됨";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_P_S_END_ERR:
                    errorMessage = "(+) 방향 소프트웨어 리미트에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_N_S_END_ERR:
                    errorMessage = "(-) 방향 소프트웨어 리미트에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_CMP3_ERR:
                    errorMessage = "CMP3 에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_CMP4_ERR:
                    errorMessage = "CMP4 에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_CMP5_ERR:
                    errorMessage = "CMP5 에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_P_H_END_ERR:
                    errorMessage = "(+) 방향 하드웨어 리미트에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_N_H_END_ERR:
                    errorMessage = "(-) 방향 하드웨어 리미트에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ALM_ERR:
                    errorMessage = "서보 알람에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_CSTP_ERR:
                    errorMessage = "CSTP에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_EMG_ERR:
                    errorMessage = "긴급 정지 신호에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESSD_ERR:
                    errorMessage = "SD 입력 ON 에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESDT_ERR:
                    errorMessage = "보간 동작 DATA 이상에 의해 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESIP_ERR:
                    errorMessage = "보간 동작 중에 타축의 이상에 의해 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESPO_ERR:
                    errorMessage = "PA/PB 입력용 버퍼 의 오버플로워 발생에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESAO_ERR:
                    errorMessage = "보간 동작 때 위치결정용 카운터의 카운터 범위 OVER 발생에 의한 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESEE_ERR:
                    errorMessage = "EA/EB 입력 ERROR 발생 때(정지 하지 않음)";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_ESPE_ERR:
                    errorMessage = "PA/PB 입력 ERROR 발생 때(정지 하지 않음)";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_SYNC_ERR:
                    errorMessage = "SYNC 동작 중에 마스터 축과 편차 에 의해 정지";
                    break;
                case pmiMApiDefs.TMC_RV_STOP_GANT_ERR:
                    errorMessage = "GANT 동작 중에 마스터 축과 편차 에 의해 정지";
                    break;
                case pmiMApiDefs.TMC_RV_DRV_VER_ERR:
                    errorMessage = "드라이버 버전 에러";
                    break;
                case pmiMApiDefs.TMC_RV_LOC_MEM_ERR:
                    errorMessage = "메모리 생성 실패";
                    break;
                case pmiMApiDefs.TMC_RV_GLB_MEM_ERR:
                    errorMessage = "공유 메모리 생성 실패";
                    break;
                case pmiMApiDefs.TMC_RV_HANDLE_ERR:
                    errorMessage = "드바이스 핸들값이 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CREATE_KERNEL_ERR:
                    errorMessage = "커널 드라이브 생성 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CREATE_THREAD_ERR:
                    errorMessage = "스레드 생성 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CREATE_EVENT_ERR:
                    errorMessage = "이벤트 생성 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CREATE_FILE_ERR:
                    errorMessage = "파일 생성 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CON_NOT_FOUND_ERR:
                    errorMessage = "CONTROLLER NOT FOUND 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CON_NOT_LOAD_ERR:
                    errorMessage = "CONTROLLER LOAD 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CON_DIP_SW_ERR:
                    errorMessage = "보드 ID 세팅 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CON_MAX_ERR:
                    errorMessage = "CONTROLLER 최대 갯수 에러";
                    break;
                case pmiMApiDefs.TMC_RV_PCI_BUS_LINE_ERR:
                    errorMessage = "PCI 버스 데이타가 이상";
                    break;
                case pmiMApiDefs.TMC_RV_MOD_POS_ERR:
                    errorMessage = "모듈 순서가 잘못되었습니다";
                    break;
                case pmiMApiDefs.TMC_RV_SUPPORT_PROCESS:
                    errorMessage = "지원하지 않은 프로세스";
                    break;
                case pmiMApiDefs.TMC_RV_SUPPORT_FUCTION_ERR:
                    errorMessage = "지원하지 않은 함수";
                    break;
                case pmiMApiDefs.TMC_RV_CON_OPEN_MODE_ERR:
                    errorMessage = "수동/자동 모드가 틀림";
                    break;
                case pmiMApiDefs.TMC_RV_PRM_LOAD_ERR:
                    errorMessage = "파라미터 로드 에러";
                    break;
                case pmiMApiDefs.TMC_RV_PRM_VAL_ERR:
                    errorMessage = "파라미터 값 에러";
                    break;
                case pmiMApiDefs.TMC_RV_PRM_FILENAME_ERR:
                    errorMessage = "파라미터 파일이 존재하지 않음";
                    break;
                case pmiMApiDefs.TMC_RV_NOT_SPT_ERR:
                    errorMessage = "모델에서 지원하지 않는 기능";
                    break;
                case pmiMApiDefs.TMC_RV_DIV_BY_ZERO_ERR:
                    errorMessage = "DIVIDE BY ZERO 에러";
                    break;
                case pmiMApiDefs.TMC_RV_TIME_OUT_ERR:
                    errorMessage = "TIME OUT 에러";
                    break;
                case pmiMApiDefs.TMC_RV_WM_QUIT_ERR:
                    errorMessage = "WM_QUIT 발생 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CON_NO_ERR:
                    errorMessage = "해당 카드번호가 존재하지 않음";
                    break;
                case pmiMApiDefs.TMC_RV_AXIS_NO_ERR:
                    errorMessage = "축 번호 에러";
                    break;
                case pmiMApiDefs.TMC_RV_MASTER_AXIS_NO_ERR:
                    errorMessage = "마스터축 번호 에러";
                    break;
                case pmiMApiDefs.TMC_RV_SLAVE_AXIS_NO_ERR:
                    errorMessage = "슬레이브축 번호 에러";
                    break;
                case pmiMApiDefs.TMC_RV_COORD_NO_ERR:
                    errorMessage = "COORDINATE 번호 에러";
                    break;
                case pmiMApiDefs.TMC_RV_ARG_RNG_ERR:
                    errorMessage = "함수 인자 범위 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CS_AXIS_ERR:
                    errorMessage = "COORDINATE AXIS 에러";
                    break;
                case pmiMApiDefs.TMC_RV_INT_CFG_ERR:
                    errorMessage = "인터럽트 설정 에러";
                    break;
                case pmiMApiDefs.TMC_RV_GROUP_RNG_ERR:
                    errorMessage = "Group 범위 에러";
                    break;
                case pmiMApiDefs.TMC_RV_AXES_MIN_ERR:
                    errorMessage = "AXIS 최소 갯수 에러";
                    break;
                case pmiMApiDefs.TMC_RV_AXES_MAX_ERR:
                    errorMessage = "AXIS 최대 갯수 에러";
                    break;
                case pmiMApiDefs.TMC_RV_MTN_IN_STOP_ERR:
                    errorMessage = "모션 구동중이어야 되는데 모션 구동중이 아닐 때 에러";
                    break;
                case pmiMApiDefs.TMC_RV_MTN_DRV_ERR:
                    errorMessage = "모션 정지중이어야 되는데 모션 구동중 일 때 에러";
                    break;
                case pmiMApiDefs.TMC_RV_HOME_BUSY_ERR:
                    errorMessage = "ORG SEARCH 중인 상태";
                    break;
                case pmiMApiDefs.TMC_RV_DRV_STEADY_ERR:
                    errorMessage = "정지 상태";
                    break;
                case pmiMApiDefs.TMC_RV_DRV_PAB_ERR:
                    errorMessage = "수동펄스 시작중일때 모션 명령 실행 때 에러";
                    break;
                case pmiMApiDefs.TMC_RV_DRV_MODIFY_POS_ERR:
                    errorMessage = "모션 에러가 발생 또는 이미 이송이 완료되어 위치 오버라이드가 적용되지 않음";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_VEL_ERR:
                    errorMessage = "MIN VEL VALUE IS UNDER VALID";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_VEL_ERR:
                    errorMessage = "MAX VEL VALUE IS OVER VALID";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_STARTV_ERR:
                    errorMessage = "START VEL VALUE IS UNDER VALID SV";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_STARTV_ERR:
                    errorMessage = "START VEL VALUE IS OVER VALID SV";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_WORKV_ERR:
                    errorMessage = "WORK VEL VALUE IS UNDER VALID WV";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_WORKV_ERR:
                    errorMessage = "WORK VEL IS OVER VALID WV";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_ACC_ERR:
                    errorMessage = "ACC TIME IS UNDER VALID AC";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_ACC_ERR:
                    errorMessage = "ACC TIME IS OVER VALID AC";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_DEC_ERR:
                    errorMessage = "DEC TIME IS UNDER VALID AC";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_DEC_ERR:
                    errorMessage = "DEC TIME IS OVER VALID AC";
                    break;
                case pmiMApiDefs.TMC_RV_MIN_DISTANCE_ERR:
                    errorMessage = "이동 거리가 최소값 이상";
                    break;
                case pmiMApiDefs.TMC_RV_MAX_DISTANCE_ERR:
                    errorMessage = "이동 거리가 최대값 이상";
                    break;
                case pmiMApiDefs.TMC_RV_DLOG_ERR:
                    errorMessage = "FUNCTION 로그 에러";
                    break;
                case pmiMApiDefs.TMC_RV_DLOG_LEVEL_ERR:
                    errorMessage = "FUNCTION 로그 레벨 에러";
                    break;
                case pmiMApiDefs.TMC_RV_STATION_COMMUNICATION_ERR:
                    errorMessage = "슬레이브 연결 에러";
                    break;
                case pmiMApiDefs.TMC_RV_STATION_INFO_ERR:
                    errorMessage = "슬레이브 정보 획득 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CONNECT_CYCLIC_ERR:
                    errorMessage = "사이클릭 통신 시작 에러 또는 Off 상태";
                    break;
                case pmiMApiDefs.TMC_RV_STATION_NO_ERR:
                    errorMessage = "슬레이브 번호 에러";
                    break;
                case pmiMApiDefs.TMC_RV_CONNECT_SYS_ERR:
                    errorMessage = "시스템 통신 시작 에러";
                    break;
                case pmiMApiDefs.TMC_RV_COMM_RESET_ERR:
                    errorMessage = "모든 통신 리셋 에러";
                    break;
                case pmiMApiDefs.TMC_RV_SLAVE_ZERO_ERR:
                    errorMessage = "사용하는 슬레이브가 0으로 사이클릭 통신 사작";
                    break;
                case pmiMApiDefs.TMC_RV_UNKNOWN_ERR:
                    errorMessage = "알수 없는 에러";
                    break;
                default:
                    errorMessage = "Unknown Error";
                    break;
            }

            errorMessage = $"ErrorCode = {errorCode}, {errorMessage}";
            return errorCode == pmiMApiDefs.TMC_RV_OK;
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

            switch (TestMethod)
            {
                case AlphaMotionMethod.Initialize:
                    // 라이브러리 초기화.
                    int controllerCount = 0;
                    int errorCode = pmiMApi.pmiSysLoad(pmiMApiDefs.TMC_FALSE, ref controllerCount);
                    if (!PmiCheckResult(errorCode, out string errorMessage))
                    {
                        result.ResultState = ResultState.Fail;
                        result.ResultInfo = errorMessage;
                        break;
                    }
                    string infoMessage = $"Controller Count = {controllerCount}";
                    Logger.LogInfo(infoMessage);
                    result.ResultInfo = infoMessage;

                    if (controllerCount == 0)
                    {
                        result.ResultState = ResultState.Pass;
                        break;
                    }

                    // 축 개수.
                    int controllerIndex = 0;
                    int axisCount = 0;
                    errorCode = pmiMApi.pmiGnGetAxesNum(controllerIndex, ref axisCount);
                    if (!PmiCheckResult(errorCode, out errorMessage))
                    {
                        result.ResultState = ResultState.Fail;
                        result.ResultInfo = errorMessage;
                        break;
                    }
                    infoMessage = $"Controller = {controllerIndex}, Axis Count = {axisCount}";
                    Logger.LogInfo(infoMessage);
                    result.ResultInfo += ", " + infoMessage;

                    // Digital IO 개수.
                    int digitalInputCount = 0, digitalOutputCount = 0;
                    errorCode = pmiMApi.pmiGnGetDioNum(controllerIndex, ref digitalInputCount, ref digitalOutputCount);
                    if (!PmiCheckResult(errorCode, out errorMessage))
                    {
                        result.ResultState = ResultState.Fail;
                        result.ResultInfo = errorMessage;
                        break;
                    }
                    infoMessage = $"Digital IO = {digitalInputCount}/{digitalOutputCount}";
                    Logger.LogInfo(infoMessage);
                    result.ResultInfo += ", " + infoMessage;

                    // 모델명 읽어오기.
                    int modelNumber = 0;
                    errorCode = pmiMApi.pmiConGetModel(controllerIndex, ref modelNumber);
                    if (!PmiCheckResult(errorCode, out errorMessage))
                    {
                        result.ResultState = ResultState.Fail;
                        result.ResultInfo = errorMessage;
                        break;
                    }
                    switch (modelNumber)
                    {
                        case pmiMApiDefs.TMC_BA800P:
                        case pmiMApiDefs.TMC_BA600P:
                        case pmiMApiDefs.TMC_BA400P:
                        case pmiMApiDefs.TMC_BA200P:
                            infoMessage = $"TMC-BA{axisCount}{digitalInputCount:D2}P";
                            break;
                        case pmiMApiDefs.TMC_BB160P:
                        case pmiMApiDefs.TMC_BB120P:
                        case pmiMApiDefs.TMC_BB800P:
                        case pmiMApiDefs.TMC_BB400P:
                            infoMessage = $"TMC-BB{axisCount}{digitalInputCount:D2}P";
                            break;
                        default:
                            infoMessage = $"ModelNumber = {modelNumber}";
                            break;
                    }
                    Logger.LogInfo("Model = " + infoMessage);
                    result.ResultInfo += ", " + infoMessage;
                    result.ResultState = ResultState.Pass;
                    break;
                case AlphaMotionMethod.Deinitialize:
                    // 라이브러리 자원 해제.
                    pmiMApi.pmiSysUnload();
                    result.ResultState = ResultState.Pass;
                    break;
                case AlphaMotionMethod.SingleAxisMove:
                case AlphaMotionMethod.MultipleAxisMove:
                    // 움직여야 할 축.
                    if (Axes?.Length > 0)
                    {
                        // 이동 속도.
                        var velocities = new double[Axes.Length];
                        if (Velocities != null)
                        {
                            int copyLength = Math.Min(Axes.Length, Velocities.Length);
                            Array.Copy(Velocities, velocities, copyLength);
                        }

                        // 서보를 켜고 이동 속도 설정.
                        for (int i = 0; i < Axes.Length; i++)
                        {
                            int axis = Axes[i];
                            errorCode = pmiMApi.pmiAxSetServoOn(ControllerIndex, axis, pmiMApiDefs.emON);
                            if (!PmiCheckResult(errorCode, out errorMessage))
                            {
                                result.ResultState = ResultState.Fail;
                                result.ResultInfo = errorMessage;
                                break;
                            }

                            double velocity = velocities[i] > 0 ? velocities[i] : 10000;
                            errorCode = pmiMApi.pmiAxSetVelProf(ControllerIndex, axis, pmiMApiDefs.emPROF_S, velocity, 100, 100);
                            if (!PmiCheckResult(errorCode, out errorMessage))
                            {
                                result.ResultState = ResultState.Fail;
                                result.ResultInfo = errorMessage;
                                break;
                            }
                        }

                        if (result.ResultState != ResultState.NoState)
                        {
                            break;
                        }

                        // 모터 위치 이동.
                        int absMode = PositionMode == PositioningMethod.Absolute ? pmiMApiDefs.emABS : pmiMApiDefs.emINC;
                        var positions = new double[Axes.Length];
                        if (Positions != null)
                        {
                            int copyLength = Math.Min(Axes.Length, Positions.Length);
                            Array.Copy(Positions, positions, copyLength);
                        }

                        if (TestMethod == AlphaMotionMethod.SingleAxisMove)
                        {
                            // 단축 이동.
                            for (int i = 0; i < Axes.Length; i++)
                            {
                                errorCode = pmiMApi.pmiAxPosMove(ControllerIndex, Axes[i], absMode, positions[i]);
                                if (!PmiCheckResult(errorCode, out errorMessage))
                                {
                                    result.ResultState = ResultState.Fail;
                                    result.ResultInfo = errorMessage;
                                    break;
                                }
                            }
                        }
                        else
                        {
                            // 다축 이동.
                            errorCode = pmiMApi.pmiMxPosMove(ControllerIndex, Axes.Length, absMode, Axes, positions);
                            if (!PmiCheckResult(errorCode, out errorMessage))
                            {
                                result.ResultState = ResultState.Fail;
                                result.ResultInfo = errorMessage;
                                break;
                            }
                        }
                    }

                    if (result.ResultState == ResultState.NoState)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                    break;
                case AlphaMotionMethod.Wait:
                    // 기다려야 할 축.
                    if (Axes?.Length > 0)
                    {
                        while (true)
                        {
                            int status = 0;
                            errorCode = pmiMApi.pmiMxCheckDone(ControllerIndex, Axes.Length, Axes, ref status);
                            if (!PmiCheckResult(errorCode, out errorMessage))
                            {
                                result.ResultState = ResultState.Fail;
                                result.ResultInfo = errorMessage;
                                break;
                            }

                            // 이동중이면 10ms 이후 다시 체크.
                            if (status != pmiMApiDefs.emRUNNING)
                            {
                                break;
                            }
                            MultimediaTimer.Delay(10).Wait(token);
                        }
                    }

                    if (result.ResultState == ResultState.NoState)
                    {
                        result.ResultState = ResultState.Pass;
                    }
                    break;
            }

            return result;
        }

        public override PhysicalUnit GetPhysicalUnit()
        {
            return PhysicalUnit.None;
        }

        public override void ToggleHiddenProperties()
        {
            base.ToggleHiddenProperties();
        }

        public override void UpdateBrowsableAttributes()
        {
            base.UpdateBrowsableAttributes();

            Utils.SetBrowsableAttribute(this, nameof(DeviceName), true);
            Utils.SetBrowsableAttribute(this, nameof(RetestMode), true);
            Utils.SetBrowsableAttribute(this, nameof(DelayAfter), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultLogInfo), true);
            Utils.SetBrowsableAttribute(this, nameof(ResultFailLogMessage), true);

            bool moveMethod = TestMethod == AlphaMotionMethod.SingleAxisMove || TestMethod == AlphaMotionMethod.MultipleAxisMove; 
            bool waitMethod = TestMethod == AlphaMotionMethod.Wait;
            Utils.SetBrowsableAttribute(this, nameof(ControllerIndex), moveMethod || waitMethod);
            Utils.SetBrowsableAttribute(this, nameof(Axes), moveMethod || waitMethod);
            Utils.SetBrowsableAttribute(this, nameof(Velocities), moveMethod);
            Utils.SetBrowsableAttribute(this, nameof(PositionMode), moveMethod);
            Utils.SetBrowsableAttribute(this, nameof(Positions), moveMethod);
        }

        protected override void UpdateToleranceAttributes()
        {
        }

        public override object Clone()
        {
            var newStep = new EolAlphaMotionStep(DeviceName);
            CopyTo(newStep);
            return newStep;
        }

        public override void CopyTo(EolStep dest)
        {
            base.CopyTo(dest);

            if (dest is EolAlphaMotionStep alphaMotionStep)
            {
                alphaMotionStep.TestMethod = TestMethod;
                alphaMotionStep.ControllerIndex = ControllerIndex;
                if (Axes != null)
                {
                    alphaMotionStep.Axes = new int[Axes.Length];
                    Axes.CopyTo(alphaMotionStep.Axes, 0);
                }
                if (Velocities != null)
                {
                    alphaMotionStep.Velocities = new int[Velocities.Length];
                    Velocities.CopyTo(alphaMotionStep.Velocities, 0);
                }
                alphaMotionStep.PositionMode = PositionMode;
                if (Positions != null)
                {
                    alphaMotionStep.Positions = new int[Positions.Length];
                    Positions.CopyTo(alphaMotionStep.Positions, 0);
                }
            }

            dest.UpdateBrowsableAttributes();
        }
    }
}
