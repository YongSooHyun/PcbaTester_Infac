using InfoBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Diagnostics.Eventing.Reader;
using System.Drawing;
using System.IO;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using TestFramework.Common.StatusLogging;
using TestFramework.PluginTestCell;
using TestFramework.PluginTestCell.TestResults;

namespace IntelligentPcbaTester
{
    public partial class MainForm : Form
    {
        // ViewModel 인스턴스.
        internal MainViewModel ViewModel;

        // Stage 상태에 따른 색깔 (PCB IN -> SCAN -> TESTING -> PCB OUT).
        private readonly Color stageCompletedColor = Color.DarkCyan;
        private readonly Color stageRunningColor = Color.SandyBrown;
        private readonly Color stageColor = Color.Sienna;
        private readonly Color stageErrorColor = Color.Red;

        // Button 색깔.
        private readonly Color startButtonColor = Color.SkyBlue;
        private readonly Color stopButtonColor = Color.OrangeRed;

        // FID, ProbeCount 정상, 경고, 에러 색깔.
        private readonly static Color probeOkColor = Color.LimeGreen;
        private readonly static Color probeErrColor = Color.Red;
        private readonly static Color probeWarnColor = Color.Orange;
        private const string ProbeOkText = "OK";
        private const string ProbeErrText = "ERR";
        private const string ProbeWarnText = "경고";
        private const float ProbeWarnPercent = 0.9F;

        private const string UserAbortedMessage = "사용자에 의해 중지되었습니다.";

        // 테스트 결과 색깔.
        private readonly static Color resultPassColor = Color.LimeGreen;
        private readonly static Color resultFailColor = Color.Red;
        private readonly static Color resultAbortedColor = Color.Yellow;
        private readonly static Color resultNoColor = Color.FromArgb(212, 208, 200);

        // 자동/수동 모드.
        private void SetAutoMode(bool autoMode)
        {
            autoModeLabel.Text = autoMode ? "자동" : "수동";
        }

        // 스테이지 Blinking Start/Stop 을 위한 변수들.
        private const int blinkingInterval = 40;        // ms.
        private readonly ManualResetEvent stageResetEvent = new ManualResetEvent(true);

        // ICT, ISP 등 페이즈 Blinking 을 위한 변수들.
        private readonly ManualResetEvent phaseResetEvent = new ManualResetEvent(true);

        // Prompt Blinking 을 위한 변수들.
        private readonly ManualResetEvent promptResetEvent = new ManualResetEvent(true);
        private readonly static Color promptStartColor = Color.Black;
        private readonly static Color promptEndColor = Color.Red;

        // Board Blinking을 위한 변수들.
        private readonly ManualResetEvent boardResetEvent = new ManualResetEvent(true);

        // 테스트 상태 설정.
        private enum TestStatus
        {
            //FinishedNoState = 0,
            FinishedPass = 1,
            FinishedFail = 2,
            FinishedAborted = 3,
            Running,
            NotRunning,
            AbortedError
        }

        // 현재 테스트 상태.
        private TestStatus currentTestStatus;

        // 테스트 상태 관리를 위한 Lock obj.
        private readonly object testStatusLockObj = new object();

        // 현재 로딩된 프로젝트.
        private Product currentProduct;

        // eloZ1 테스트 결과.
        private ResultState Eloz1FinishState
        {
            get => _eloz1FinishState;
            set
            {
                _eloz1FinishState = value;
            }
        }
        private ResultState _eloz1FinishState = ResultState.NoState;

        // 사용자가 Stop 버튼을 클릭했는지 여부.
        private bool userClickedStop = false;

        // 사용자에게 어떤 액션을 취할것을 지시하는 대화상자. 자동으로 없애기 위해 이용.
        //private MessageDialog promptDlg = null;

        // PLC 디바이스 상태 폴링을 위한 타이머.
        private System.Timers.Timer statusReadTimer = null;

        // 테스트 중 핸들러에서 이상상태 감지 시 처리를 위한 변수들.
        private volatile bool handlerErrorOccurred = false;
        private string handlerErrorMessage = "";

        // 프로젝트 섹션 타이틀 배경색.
        private Color sectionTitleBackColor;

        // MultiPanel Board 타이틀 배경색.
        private Color boardBackColor;
        private Font boardTitleFont;
        private readonly Color boardDisabledColor = Color.Gray;

        // Ctrl + D 기능 사용 여부.
        private bool probeCountEditEnabled = true;

        // Fixture 파워 상태.
        private bool fixturePowered = false;
        private bool shouldMonitorFixturePower = true;

        // 바코드 읽기 대기 상태.
        private bool barcodeReading = false;
        private readonly FidScanner fidScanner = new FidScanner();

        // PCB IN 대기 상태.
        private bool waitingPcbIn = false;

        // PCB OUT 대기 상태.
        private bool waitingPcbOut = false;

        // Novaflash connection status.
        private bool novaConnected = false;

        // 상태 업데이트를 위한 타이머 스레드 개수를 제한한다.
        private int novaThreadCount = 0;
        private const int NovaThreadMaxCount = 1;
        private int plcThreadCount = 0;
        private const int PlcThreadMaxCount = 2;

        // DIO 오프라인 장비를 이용한 수동 테스트 모드.
        private readonly bool dioManualMode = AppSettings.DioManualMode;

        // PLC 디바이스 연결 상태.
        private void SetPlcConnected(bool connected)
        {
            ShowConnectionImage(plcPictureBox, connected);
        }

        // DIO 디바이스 연결 상태.
        private void SetDioConnected(bool connected)
        {
            ShowConnectionImage(dioPictureBox, connected);
        }

        // DMM 연결 상태.
        private void SetDmmConnected(bool connected)
        {
            //ShowConnectionImage(dmmPictureBox, connected);
        }

        // eloZ1 연결 상태.
        private void SetIsEloz1Connected(bool connected)
        {
            ShowConnectionImage(elozPictureBox, connected);
        }

        // ISP 연결 상태.
        private void SetIspConnected(bool connected)
        {
            novaConnected = connected;
            ShowConnectionImage(ispPictureBox, connected);
        }

        // Power 연결 상태.
        private void UpdatePowerStatus()
        {
            bool power1Connected = ViewModel.Power1CheckConnection();

            var manager = ComportSettingsManager.Load();
            var portSettings = manager.FindSettings(PowerSupply.Power2Name);
            bool power2Connected = portSettings.IsEnabled ? ViewModel.Power2CheckConnection() : true;
            ShowConnectionImage(powerPictureBox, power1Connected && power2Connected);
        }

        // JTAG 연결 상태.
        private void UpdateJtagStatus(object _)
        {
            if (!AppSettings.UseJtag)
            {
                return;
            }

            bool connected = ViewModel.JtagCheckConnection();
            Utils.InvokeIfRequired(this, () =>
            {
                ShowConnectionImage(jtagPictureBox, connected);
            });
        }

        // Barcode.
        private string CurrentBarcode
        {
            get => _currentBarcode;
            set
            {
                _currentBarcode = value;
                Eloz1.SetGlobalStorageValue(MainViewModel.BarcodeVarName, value);
            }
        }
        private string _currentBarcode = "";

        // 현재 표시하고 있는 Probe Count.
        private FixtureProbeCount CurrentProbeCount
        {
            get => _currentProbeCount;
            set
            {
                _currentProbeCount = value;
                Utils.InvokeIfRequired(this, () =>
                {
                    countInitButton.Enabled = _currentProbeCount != null;
                });
            }
        }
        private volatile FixtureProbeCount _currentProbeCount;

        public MainForm()
        {
            InitializeComponent();

            sectionTitleBackColor = ictTitleLabel.BackColor;
            boardBackColor = board1TLPanel.BackColor;
            boardTitleFont = board1TitleLabel.Font;
        }

        private void LogRecorder_StatusChanged(object sender, LogRecorder.StatusChangedEventArgs e)
        {
            Utils.InvokeIfRequired(this, () =>
            {
                promptLabel.Text = e.Status;
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Text = Utils.AssemblyProduct;
            companyStatusLabel.Text = Assembly.GetExecutingAssembly().GetCustomAttribute<AssemblyCompanyAttribute>()?.Company;

            ViewModel = new MainViewModel();

            InitUI();
            InitProductUI();
            InitStageUI();

            // 테스트 상태 초기화.
            SetCurrentTestStatus(TestStatus.NotRunning);

            Eloz1FinishState = ResultState.NoState;

            // Eloz 상태 메시지 출력.
            LogRecorder.StatusChanged += LogRecorder_StatusChanged;

            try
            {
                CheckDeviceConnections();
                InitEloz1();

                // 상태 업데이트.
                UpdatePlcStatus();
            }
            catch (Exception ex)
            {
                //Logger.LogDebugInfo(ex.ToString());
                Logger.LogError($"초기화 오류: {ex.Message}");
            }

            if (ViewModel != null)
            {
                plcThreadCount = 0;
                novaThreadCount = 0;
                statusReadTimer = new System.Timers.Timer(500);
                statusReadTimer.Elapsed += StatusReadTimer_Elapsed;
                statusReadTimer.AutoReset = true;
                statusReadTimer.Start();
            }
        }

        private void MainForm_Activated(object sender, EventArgs e)
        {
            // 작업자는 Config를 할 수 없다.
            if (AppSettings.LoggedUser?.Role == UserRole.작업자)
            {
                configButton.Enabled = false;
            }
            else
            {
                configButton.Enabled = true;
            }
        }

        private void StatusReadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdatePlcStatus();
            if (novaConnected)
            {
                UpdatePodStatus();
            }
        }

        private void UpdatePlcStatus()
        {
            if (plcThreadCount >= PlcThreadMaxCount)
            {
                return;
            }

            try
            {
                Interlocked.Increment(ref plcThreadCount);

                if (!dioManualMode)
                {
                    var plcStatus = ViewModel.PlcReadStatus(false);

                    // 안전센서.
                    bool isSafetyBad = (plcStatus & PlcReadFlags.SafetySensor) == 0;

                    bool isAutoMode = (plcStatus & PlcReadFlags.Mode) == 0;
                    bool doorOpened = (plcStatus & PlcReadFlags.FrontDoor) == 0 || (plcStatus & PlcReadFlags.RearDoor) == 0;
                    Utils.InvokeIfRequired(this, () =>
                    {
                        // 자동/수동 모드.
                        SetAutoMode(isAutoMode);

                        // 도어 오픈 상태.
                        if (doorOpened)
                        {
                            doorLabel.Text = "Open";
                            doorLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            doorLabel.Text = "정상";
                            doorLabel.BackColor = Color.Green;
                        }

                        // 공압.
                        bool isPneumaticLow = (plcStatus & PlcReadFlags.AirPressure) == 0;
                        if (isPneumaticLow)
                        {
                            pneumaticLabel.Text = "Low";
                            pneumaticLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            pneumaticLabel.Text = "정상";
                            pneumaticLabel.BackColor = Color.Green;
                        }

                        // 안전센서.
                        if (isSafetyBad)
                        {
                            safetyLabel.Text = "비정상";
                            safetyLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            safetyLabel.Text = "정상";
                            safetyLabel.BackColor = Color.Green;
                        }
                    });

                    // 테스트 진행 중 기구 이상을 감지하여 처리.
                    if (!handlerErrorOccurred)
                    {
                        bool isEmergency = (plcStatus & PlcReadFlags.Emergency) == 0;

                        if (doorOpened && AppSettings.CheckDoorOpenStatus)    // 도어 오픈 상태 체크.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "도어가 열렸습니다.";
                        }
                        else if (isEmergency)   // Emergency 체크.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "비상 상황이 감지되었습니다.";
                        }
                        else if (isSafetyBad)   // 안전센서 에러.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "안전센서가 비정상입니다.";
                        }
                        else if (!isAutoMode && AppSettings.CheckAutoMode)        // 자동 모드 체크.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "자동 모드가 아닙니다.";
                        }

                        if (handlerErrorOccurred)
                        {
                            ViewModel.Eloz1Stop();
                        }
                    }

                    bool isBottomFixtureMounted = (plcStatus & PlcReadFlags.BottomFixtureSensor) != 0;

                    // Turn off novaflash pods powers.
                    if (!isBottomFixtureMounted)
                    {
                        try
                        {
                            ViewModel.NovaOpen();
                            ViewModel.NovaPodPower(false, true, true, true, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Novaflash: {ex.Message}");
                        }
                    }

                    // DIO fixture power on/off.
                    if (shouldMonitorFixturePower)
                    {
                        if (isBottomFixtureMounted && !fixturePowered)
                        {
                            // DIO fixture power on.
                            ViewModel.DioOpen();
                            ViewModel.DioFixturePower(true, true);
                            fixturePowered = true;
                        }
                        else if (!isBottomFixtureMounted && fixturePowered)
                        {
                            // DIO fixture power off.
                            ViewModel.DioOpen();
                            ViewModel.DioFixturePower(false, true);
                            fixturePowered = false;
                        }
                    }
                }
                else
                {
                    var dioStatus = ViewModel.DioRWReadStatus(false);

                    // 안전센서.
                    bool isSafetyBad = (dioStatus & DioReadFlags.SafetySensor) == 0;

                    bool isAutoMode = (dioStatus & DioReadFlags.Mode) == 0;
                    bool doorOpened = (dioStatus & DioReadFlags.DoorSensor) == 0;
                    bool isPneumaticLow = (dioStatus & DioReadFlags.AirSensorOut) == 0;
                    Utils.InvokeIfRequired(this, () =>
                    {
                        // 자동/수동 모드.
                        SetAutoMode(isAutoMode);

                        // 도어 오픈 상태.
                        if (doorOpened)
                        {
                            doorLabel.Text = "Open";
                            doorLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            doorLabel.Text = "정상";
                            doorLabel.BackColor = Color.Green;
                        }

                        // 공압.
                        if (isPneumaticLow)
                        {
                            pneumaticLabel.Text = "Low";
                            pneumaticLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            pneumaticLabel.Text = "정상";
                            pneumaticLabel.BackColor = Color.Green;
                        }

                        // 안전센서.
                        if (isSafetyBad)
                        {
                            safetyLabel.Text = "비정상";
                            safetyLabel.BackColor = Color.Red;
                        }
                        else
                        {
                            safetyLabel.Text = "정상";
                            safetyLabel.BackColor = Color.Green;
                        }
                    });

                    // 테스트 진행 중 기구 이상을 감지하여 처리.
                    if (!handlerErrorOccurred)
                    {
                        if (!isAutoMode && AppSettings.CheckAutoMode)        // 자동 모드 체크.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "자동 모드가 아닙니다.";
                        }
                        else if (doorOpened && AppSettings.CheckDoorOpenStatus)    // 도어 오픈 상태 체크.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "도어가 열렸습니다.";
                        }
                        else if (isAutoMode && isSafetyBad)   // 안전센서 에러.
                        {
                            handlerErrorOccurred = true;
                            handlerErrorMessage = "안전센서가 비정상입니다.";
                        }

                        if (handlerErrorOccurred && ViewModel.IsEloz1Running)
                        {
                            ViewModel.Eloz1Stop();
                        }
                    }

                    bool isBottomFixtureMounted = (dioStatus & DioReadFlags.BotFixtureSensor) != 0;

                    // Turn off novaflash pods powers.
                    if (!isBottomFixtureMounted)
                    {
                        try
                        {
                            ViewModel.NovaOpen();
                            ViewModel.NovaPodPower(false, true, true, true, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Novaflash: {ex.Message}");
                        }
                    }
                }
            }
            catch (Exception)
            {
                // Do nothing.
            }
            finally
            {
                Interlocked.Decrement(ref plcThreadCount);
            }
        }

        private void UpdateProbeCount()
        {
            int fid = AppSettings.CurrentFid;
            if (fid > 0)        // fid == 0 이면 픽스쳐가 없는 것임.
            {
                if (CurrentProbeCount == null || CurrentProbeCount.FixtureId != fid)
                {
                    CurrentProbeCount = MainViewModel.GetProbeCount(fid);
                    DisplayProbeCount(CurrentProbeCount);
                }
            }
            else
            {
                CurrentProbeCount = null;
                DisplayProbeCount(CurrentProbeCount);
            }
        }

        private void UpdatePodStatus(object state = null)
        {
            if (novaThreadCount >= NovaThreadMaxCount)
            {
                return;
            }
            
            try
            {
                Interlocked.Increment(ref novaThreadCount);

                if (state is int delay && delay > 0)
                {
                    Thread.Sleep(delay);
                }

                // POD 전원 상태 업데이트.
                UpdateNovaChannelStates(new int[] { 1, 2, 3, 4 });
            }
            catch (Exception e)
            {
                Logger.LogDebugInfo("Novaflash Error: " + e.Message);
            }
            finally
            {
                Interlocked.Decrement(ref novaThreadCount);
            }
        }

        private void UpdateNovaChannelStates(IEnumerable<int> channels)
        {
            if (channels == null || !channels.Any())
            {
                return;
            }

            ViewModel.NovaOpen();
            foreach (int channel in channels)
            {
                var channelState = ViewModel.NovaGetChState(channel);
                Utils.InvokeIfRequired(this, () =>
                {
                    var pictureBoxes = new PictureBox[] { isp1PictureBox, isp2PictureBox, isp3PictureBox, isp4PictureBox };
                    Bitmap statusImage;
                    switch (channelState)
                    {
                        case Novaflash.ChannelState.Pass:
                            statusImage = Properties.Resources.icons8_green_circle_48;
                            break;
                        case Novaflash.ChannelState.Fail:
                            statusImage = Properties.Resources.icons8_red_circle_48;
                            break;
                        case Novaflash.ChannelState.Running:
                            statusImage = Properties.Resources.icons8_yellow_circle_48;
                            break;
                        case Novaflash.ChannelState.StatusCleared:
                            statusImage = Properties.Resources.icons8_gray_circle_48;
                            break;
                        case Novaflash.ChannelState.NotConnected:
                            statusImage = Properties.Resources.icons8_black_circle_48;
                            break;
                        default:
                            statusImage = null;
                            break;
                    }
                    pictureBoxes[channel - 1].Image = statusImage;
                });
            }
        }

        protected override void OnVisibleChanged(EventArgs e)
        {
            base.OnVisibleChanged(e);

            // Login Mode.
            loginModeLabel.Text = AppSettings.LoggedUser?.Role.ToString();

            // Login Name.
            loginIdLabel.Text = AppSettings.LoggedUser?.UserName;
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            // MES Mode.
            UpdateMesStatus();

            // JTAG 사용 여부에 따라 아이콘 보여주거나 숨기기.
            jtagDevicePanel.Visible = AppSettings.UseJtag;
        }

        private void UpdateMesStatus()
        {
            if (AppSettings.MesEnabled)
            {
                mesModeLabel.Text = "MES 사용";
                mesModeLabel.BackColor = Color.Green;
            }
            else
            {
                mesModeLabel.Text = "MES 미사용";
                mesModeLabel.BackColor = Color.Red;
            }
        }

        private void InitUI()
        {
            // 날짜 표시.
            dateLabel.Text = DateTime.Now.ToString("yyyy년 MM월 dd일");

            // 버전 표시.
            Version ver = Assembly.GetExecutingAssembly().GetName().Version;
            versionLabel.Text = $"V{ver.Major}.{ver.Minor}.{ver.Build}";

            // IP 주소.
            ipLabel.Text = Utils.GetLocalIPAddress()?.ToString();

            // 상태표시줄에 프로젝트 폴더 표시.
            pathStatusLabel.Text = AppSettings.ProjectFolder;

            promptLabel.Text = "";

            // Stop 버튼 Disable.
            SetButtonEnabled(stopButton, stopButtonColor, false);
        }

        private void InitProductUI()
        {
            // Barcode 초기화.
            barcodeLabel.Text = "";
            fgcodeLabel.Text = "";

            // 모델 초기화.
            modelLabel.Text = "";

            // Project 이름 초기화.
            projectLabel.Text = "";
            projectNameLabel.Text = "";

            // Project 정보 초기화.
            ictLabel.Text = "";
            ictTitleLabel.BackColor = sectionTitleBackColor;
            ictPowerTitleLabel.BackColor = sectionTitleBackColor;
            ictPowerLabel.Text = "";
            ispLabel.Text = "";
            ispTitleLabel.BackColor = sectionTitleBackColor;
            isp1Label.Text = "";
            isp1CrcLabel.Text = "";
            isp1CrcLabel.BackColor = Color.LightGray;
            isp2Label.Text = "";
            isp2CrcLabel.Text = "";
            isp2CrcLabel.BackColor = Color.LightGray;
            isp3Label.Text = "";
            isp3CrcLabel.Text = "";
            isp3CrcLabel.BackColor = Color.LightGray;
            isp4Label.Text = "";
            isp4CrcLabel.Text = "";
            isp4CrcLabel.BackColor = Color.LightGray;
            jtagLabel.Text = "";
            jtagTitleLabel.BackColor = sectionTitleBackColor;
            funcLabel.Text = "";
            funcTitleLabel.BackColor = sectionTitleBackColor;
            eolLabel.Text = "";
            eolTitleLabel.BackColor = sectionTitleBackColor;
            ext1Label.Text = "";
            ext1TitleLabel.BackColor = sectionTitleBackColor;
            ext2Label.Text = "";
            ext2TitleLabel.BackColor = sectionTitleBackColor;

            // Fixture 정보 초기화.
            fixtureIdLabel.Text = "";
            fixtureStatusLabel.Text = ProbeOkText;
            fixtureStatusLabel.BackColor = probeOkColor;

            // Probe count 초기화.
            probeCountLabel.Text = "";
            probeStatusLabel.Text = ProbeOkText;
            probeStatusLabel.BackColor = probeOkColor;

            // Statistics 초기화.
            todayTestedLabel.Text = "";
            todayPassLabel.Text = "";
            todayFailLabel.Text = "";
            todayYieldLabel.Text = "";

            // Pie Chart 초기화.
            pieChart.Percent = 0;

            // 테스트 시간 초기화.
            testDurationLabel.Text = "";

            promptLabel.Text = "";

            // Board 초기화.
            board1TLPanel.BackColor = boardBackColor;
            board1BarcodeLabel.Text = "";
            board2TLPanel.BackColor = boardBackColor;
            board2BarcodeLabel.Text = "";
            board3TLPanel.BackColor = boardBackColor;
            board3BarcodeLabel.Text = "";
            board4TLPanel.BackColor = boardBackColor;
            board4BarcodeLabel.Text = "";

            // Nova POD Power Status.
            isp1PictureBox.Visible = true;
            isp2PictureBox.Visible = true;
            isp3PictureBox.Visible = true;
            isp4PictureBox.Visible = true;
        }

        private void InitStageUI()
        {
            // Stage 초기화.
            stagePcbInLabel.BackColor = stageColor;
            stageScanLabel.BackColor = stageColor;
            stageTestingLabel.BackColor = stageColor;
            stagePcbOutLabel.BackColor = stageColor;
        }

        private void SetCurrentTestStatus(TestStatus status)
        {
            lock (testStatusLockObj)
            {
                currentTestStatus = status;

                // 현재 상태 표시.
                string statusMessage;
                Color statusColor;
                switch (currentTestStatus)
                {
                    //case TestStatus.FinishedNoState:
                    //    statusMessage = "NO RESULT";
                    //    statusColor = Color.FromArgb(212, 208, 200);
                    //    break;
                    case TestStatus.FinishedPass:
                        statusMessage = "PASS";
                        statusColor = resultPassColor;
                        break;
                    case TestStatus.FinishedFail:
                        statusMessage = "FAIL";
                        statusColor = resultFailColor;
                        break;
                    case TestStatus.FinishedAborted:
                        statusMessage = "중단";
                        statusColor = resultAbortedColor;
                        break;
                    case TestStatus.AbortedError:
                        statusMessage = "ERROR";
                        statusColor = Color.Orange;
                        break;
                    case TestStatus.Running:
                        statusMessage = "시험중";
                        statusColor = Color.Bisque;
                        break;
                    case TestStatus.NotRunning:
                    default:
                        statusMessage = "시험 대기";
                        statusColor = resultNoColor;
                        break;
                }
                Utils.InvokeIfRequired(this, () =>
                {
                    testStatusLabel.Text = statusMessage;
                    testStatusLabel.BackColor = statusColor;
                });
            }
        }

        private void UpdateButtonStates(bool running)
        {
            if (running)
            {
                SetButtonEnabled(startButton, startButtonColor, false);
                SetButtonEnabled(stopButton, stopButtonColor, true);
                exitButton.Enabled = false;
                resetButton.Enabled = false;
                logoutButton.Enabled = false;
                //fixtureChangeButton.Enabled = false;
                projectOpenButton.Enabled = false;
                elozProjOpenButton.Enabled = false;
            }
            else
            {
                SetButtonEnabled(startButton, startButtonColor, true);
                SetButtonEnabled(stopButton, stopButtonColor, false);
                exitButton.Enabled = true;
                resetButton.Enabled = true;
                logoutButton.Enabled = true;
                //fixtureChangeButton.Enabled = true;
                projectOpenButton.Enabled = true;
                elozProjOpenButton.Enabled = true;
            }
        }

        private void SetButtonEnabled(Button button, Color enabledBackColor, bool enabled)
        {
            if (enabled)
            {
                button.Enabled = true;
                button.BackColor = enabledBackColor;
            }
            else
            {
                button.BackColor = SystemColors.Control;
                button.Enabled = false;
            }
        }

        private void CheckDeviceConnections()
        {
            // PLC 연결 여부 체크.
            SetPlcConnected(ViewModel.PlcCheckConnection());

            // DIO 연결 여부 체크.
            if (dioManualMode)
            {
                SetDioConnected(ViewModel.DioRWCheckConnection());
            }
            else
            {
                SetDioConnected(ViewModel.DioCheckConnection());
            }

            // Power Supply 연결 여부 체크.
            UpdatePowerStatus();

            // Always true.
            SetIsEloz1Connected(true);

            // JTAG 연결 상태.
            ThreadPool.QueueUserWorkItem(UpdateJtagStatus);

            // ISP 와 DMM 은 LAN 연결이고, 시간이 오래 걸릴 수 있으므로 다른 스레드에서 실행한다.
            ThreadPool.QueueUserWorkItem(CheckIspDmmConnection);
        }

        private void CheckIspDmmConnection(object state)
        {
            // ISP 연결 체크.
            bool novaConnected = false;
            try
            {
                ViewModel.NovaOpen();

                novaConnected = true;
            }
            catch (Exception e)
            {
                Logger.LogError($"{Novaflash.DeviceName}: {e.Message}");
            }

            // ISP 상태 변경.
            Utils.InvokeIfRequired(this, () =>
            {
                SetIspConnected(novaConnected);
            });

            // DMM 연결 체크.
            //bool dmmConnected = false;
            //try
            //{
            //    DmmDevice.GetIdn(AppSettings.DmmResourceName);
            //    dmmConnected = true;
            //}
            //catch (Exception e)
            //{
            //    Logger.LogError($"{DmmDevice.DeviceName}: {e}");
            //}

            //// DMM 상태 변경.
            //Utils.InvokeIfRequired(this, () =>
            //{
            //    SetDmmConnected(dmmConnected);
            //});
        }

        private void InitEloz1()
        {
            if (ViewModel != null)
            {
                ViewModel.Eloz1EnvironmentErrorOccurred += ViewModel_Eloz1EnvironmentErrorOccurred;
                ViewModel.Eloz1TestLog += ViewModel_Eloz1TestLog;
                ViewModel.Eloz1TestFinished += ViewModel_Eloz1TestFinished;
            }
        }

        private void ViewModel_Eloz1TestFinished(object sender, TestRunFinishedEventArgs e)
        {
            //PhaseStopBlink(ictTitleLabel, true);
            Eloz1FinishState = e.FinishedState;
        }

        private void ViewModel_Eloz1TestLog(object sender, Eloz1TestLogEventArgs e)
        {
            Utils.InvokeIfRequired(this, () =>
            {
                TestLogAppend(e.Message);
            });
        }

        private void ViewModel_Eloz1EnvironmentErrorOccurred(object sender, ErrorOccurredEventArgs e)
        {
            InformationBox.Show($"{e.ErrorId}{Environment.NewLine}{e.ErrorMessage}{Environment.NewLine}{e.ErrorDetails}",
                "eloZ1 Environment Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
        }

        private void ShowConnectionImage(PictureBox box, bool connected)
        {
            if (connected)
            {
                box.Image = Properties.Resources.icons8_green_circle_48;
            }
            else
            {
                box.Image = Properties.Resources.icons8_red_circle_48;
            }
        }

        internal void CommLogAppendLine(string message)
        {
            commLogTextBox.AppendText($"{message}{Environment.NewLine}");
        }

        internal void CommLogClear()
        {
            commLogTextBox.Clear();
        }

        internal void TestLogAppend(string message)
        {
            testLogTextBox.AppendText(message);
        }

        internal void TestLogAppendLine(string message)
        {
            testLogTextBox.AppendText($"{message}{Environment.NewLine}");
        }

        internal void TestLogClear()
        {
            testLogTextBox.Clear();
        }

        private void configButton_Click(object sender, EventArgs e)
        {
            var dlg = new SettingsForm();
            dlg.ShowDialog();
        }

        private void Blink(WaitHandle waitHandle, Control control, Color color1, Color color2, int cycleMilliseconds, bool useBackColor)
        {
            var watch = Stopwatch.StartNew();
            int halfCycle = (int)Math.Round(cycleMilliseconds * 0.5);
            while (!waitHandle.WaitOne(blinkingInterval))
            {
                Application.DoEvents();
                long time = watch.ElapsedMilliseconds % cycleMilliseconds;
                double multiplier = (double)Math.Abs(time - halfCycle) / halfCycle;
                double red = (color2.R - color1.R) * multiplier + color1.R;
                double green = (color2.G - color1.G) * multiplier + color1.G;
                double blue = (color2.B - color1.B) * multiplier + color1.B;
                var color = Color.FromArgb((byte)red, (byte)green, (byte)blue);
                Utils.InvokeIfRequired(this, () =>
                {
                    if (useBackColor)
                    {
                        control.BackColor = color;
                    }
                    else
                    {
                        control.ForeColor = color;
                    }
                });
            }
        }

        private Task PhaseBlinkStart(Control control)
        {
            return Task.Run(() =>
            {
                phaseResetEvent.WaitOne();
                phaseResetEvent.Reset();
                Blink(phaseResetEvent, control, stageColor, stageRunningColor, 1000, true);
            });
        }

        private void PhaseBlinkStop(Control control, bool completed)
        {
            Thread.Sleep(10);
            phaseResetEvent.Set();
            if (completed)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                Utils.InvokeIfRequired(control, () =>
                {
                    control.BackColor = stageCompletedColor;
                });
            }
        }

        private Task BoardBlinkStart(Control control)
        {
            return Task.Run(() =>
            {
                boardResetEvent.WaitOne();
                boardResetEvent.Reset();
                Blink(boardResetEvent, control, stageColor, stageRunningColor, 1000, true);
            });
        }

        private void BoardBlinkStop(Control control, bool completed)
        {
            Thread.Sleep(10);
            boardResetEvent.Set();
            if (completed)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                Utils.InvokeIfRequired(control, () =>
                {
                    control.BackColor = stageCompletedColor;
                });
            }
        }

        private Task StageBlinkStart(Control control)
        {
            return Task.Run(() =>
            {
                stageResetEvent.WaitOne();
                stageResetEvent.Reset();
                Blink(stageResetEvent, control, stageColor, stageRunningColor, 1000, true);
            });
        }

        private void StageBlinkStop(Control control, bool completed)
        {
            Thread.Sleep(10);
            stageResetEvent.Set();
            if (completed)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                Utils.InvokeIfRequired(control, () =>
                {
                    control.BackColor = stageCompletedColor;
                });
            }
        }

        private Task PromptBlinkStart(Control control)
        {
            return Task.Run(() =>
            {
                promptResetEvent.WaitOne();
                promptResetEvent.Reset();
                Blink(promptResetEvent, control, promptStartColor, promptEndColor, 1000, false);
            });
        }

        private void PromptBlinkStop(Control control, bool completed)
        {
            Thread.Sleep(10);
            promptResetEvent.Set();
            if (completed)
            {
                Thread.Sleep(10);
                Application.DoEvents();
                Utils.InvokeIfRequired(control, () =>
                {
                    control.ForeColor = promptStartColor;
                });
            }
        }

        private void startButton_Click(object sender, EventArgs e)
        {
            promptLabel.Text = "";
            TestLogClear();
            CommLogClear();

            ictTitleLabel.BackColor = sectionTitleBackColor;
            ictPowerTitleLabel.BackColor = sectionTitleBackColor;
            ispTitleLabel.BackColor = sectionTitleBackColor;
            jtagTitleLabel.BackColor = sectionTitleBackColor;
            funcTitleLabel.BackColor = sectionTitleBackColor;
            eolTitleLabel.BackColor = sectionTitleBackColor;
            ext1TitleLabel.BackColor = sectionTitleBackColor;
            ext2TitleLabel.BackColor = sectionTitleBackColor;

            if (currentProduct?.Project != null)
            {
                board2TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                if (currentProduct.Project.Panel == 2 && !currentProduct.Project.TwoBoardsLeftRight)
                {
                    // 2연배열 보드 상/하 배치.
                    if (currentProduct.Project.BottomBoardFirst)
                    {
                        // 밑의 보드가 첫번째.
                        board1TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                        board3TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                    }
                    else
                    {
                        // 밑의 보드가 두번째.
                        board1TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                        board3TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                    }
                }
                else
                {
                    board1TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                    board3TLPanel.BackColor = currentProduct.Project.Board3Checked ? boardBackColor : boardDisabledColor;
                }
                board4TLPanel.BackColor = currentProduct.Project.Board4Checked ? boardBackColor : boardDisabledColor;
            }

            ThreadPool.QueueUserWorkItem(DoStartTest);
        }

        // 중단 루틴.
        void StopRoutine(bool rethrow)
        {
            // 전원 차단.
            try
            {
                // Power 1, 2 Off.
                Power12Off();
                waitingPcbOut = true;

            }
            catch (Exception e)
            {
                if (rethrow)
                {
                    throw;
                }
                else
                {
                    Logger.LogError($"StopRoutine: {e.Message}");
                }
            }
            finally
            {
                waitingPcbOut = false;
            }
        }

        private void Power12Off()
        {
            ThreadPool.QueueUserWorkItem(DoPower12Off);
        }

        private void DoPower12Off(object o)
        {
            try
            {
                ViewModel.Power12Off(currentProduct?.Project?.Power1Enabled ?? true, currentProduct?.Project?.Power2Enabled ?? true);
                if (!dioManualMode)
                {
                    ViewModel.DioOpen();
                    ViewModel.DioPowerOff();
                    ViewModel.DioDischarge(true);

                    Thread.Sleep(1000);

                    ViewModel.DioOpen();
                    ViewModel.DioDischarge(false);
                }
            }
            catch (Exception e)
            {
                Logger.LogTimedMessage($"Power 1,2 Off: {e.Message}");
            }
        }

        private void SetMesStatus(bool isICT, bool isM2, bool? ok)
        {
            var ictEolText = isICT ? "ICT" : "EOL";
            Label mesResultLabel = isM2 ? mesM1ResultLabel : mesM3ResultLabel;
            if (ok == null)
            {
                mesResultLabel.BackColor = SystemColors.Control;
                mesResultLabel.Text = "";
                mesStatusLabel.Text = "";
            }
            else if (ok == true)
            {
                mesResultLabel.BackColor = Color.LimeGreen;
                mesResultLabel.Text = ictEolText + " OK";
                mesStatusLabel.Text = "";
            }
            else
            {
                mesResultLabel.BackColor = Color.Red;
                mesResultLabel.Text = ictEolText + " NG";
                if (isM2)
                {
                    mesStatusLabel.Text = "MES 이전공정 체크 NG";
                }
                else
                {
                    mesStatusLabel.Text = "MES 처리결과 전송 NG";
                }
            }
        }

        private void DoStartTest(object state)
        {
            Stopwatch elozDurationWatch = Stopwatch.StartNew();
            var elapsedTimer = new System.Timers.Timer();
            elapsedTimer.Interval = 200;
            elapsedTimer.AutoReset = true;
            elapsedTimer.Elapsed += (object sender, System.Timers.ElapsedEventArgs e) =>
            {
                // 테스트 시간 표시
                double duration = elozDurationWatch.ElapsedMilliseconds / 1000.0;
                Utils.InvokeIfRequired(this, () => {
                    testDurationLabel.Text = $"{duration:0.0}초";
                });
            };

            // 테스트 종료 후 Hydra 파일들을 제거하기 위해 필요.
            bool ispSectionEnabled = false;

            try
            {
                // MES 서버를 시작한다.
                if (AppSettings.MesEnabled)
                {
                    // ICT 서버 시작.
                    if (!MesServer.SharedIctServer.IsRunning)
                    {
                        MesServer.SharedIctServer.StartServer(AppSettings.MesIctServerPort);

                        // 서버가 시작할 때까지 기다렸다가 시작 상태를 체크한다.
                        Thread.Sleep(100);
                        if (!MesServer.SharedIctServer.IsRunning)
                        {
                            throw new Exception("MES ICT 서버를 시작할 수 없습니다.");
                        }
                    }

                    // EOL 서버 시작.
                    if (!MesServer.SharedEolServer.IsRunning)
                    {
                        MesServer.SharedEolServer.StartServer(AppSettings.MesEolServerPort);

                        // 서버가 시작할 때까지 기다렸다가 시작 상태를 체크한다.
                        Thread.Sleep(100);
                        if (!MesServer.SharedEolServer.IsRunning)
                        {
                            throw new Exception("MES EOL 서버를 시작할 수 없습니다.");
                        }
                    }
                }

                Utils.InvokeIfRequired(this, () =>
                {
                    UpdateButtonStates(true);

                    // MES M1, M3 상태 초기화.
                    SetMesStatus(true, true, null);
                    SetMesStatus(true, false, null);

                    // Ctrl+D, 검사수량 초기화 버튼 Disable.
                    probeCountEditEnabled = false;
                    countInitButton.Enabled = false;
                });
                userClickedStop = false;            // 사용자가 Stop버튼을 눌렀는지 여부.
                bool isFirstRun = true;             // 첫번째 실행인가를 표시.
                PlcReadFlags plcStatus = 0;
                DioReadFlags dioRWStatus = 0;

                handlerErrorOccurred = false;       // 기구부에서 에러가 생겼는지 여부.
                handlerErrorMessage = "";           // 기구부에서 에러가 생긴 경우 그 에러 메시지.
                waitingPcbIn = false;               // PCB 투입 대기중인지 여부.
                waitingPcbOut = false;              // PCB 배출 대기중인지 여부.
                shouldMonitorFixturePower = false;  // Fixture Power 를 끄거나 켜는 동작을 할지 여부.

                // 초기의 PCB 상태.
                var pcbScanZone = PcbTestResult.NoPcb;      // 스캔존에 PCB 투입여부 및 SMEMA 이전공정 결과.
                var pcbMesResult = PcbMesResult.NoPcb;      // MES 이전공정 검사 결과.
                var pcbMesBarcode = "";                     // 바코드.
                var pcbMesErrorMessage = "";                // MES 통신 에러 메시지.
    
                // 직렬연결을 위한 변수들.
                var prevResult = PcbTestResult.NoPcb;   // 앞에서 테스트한 결과.

                while (true)
                {
                    Task.Run(() => MainViewModel.RemoveOldHistory());

                    // 테스트 기록.
                    var history = new TestHistory();
                    history.StartTime = DateTime.Now;

                    // 로딩된 프로젝트가 있는지 체크.
                    if (currentProduct?.Project == null)
                    {
                        throw new Exception("로딩된 프로젝트가 없습니다.");
                    }

                    if (!isFirstRun && !AppSettings.AutoRestartTest)
                    {
                        break;
                    }

                    // Max Probe Count 초과 여부 검사.
                    if (CurrentProbeCount != null && CurrentProbeCount.TotalProbeCount >= CurrentProbeCount.MaxProbeCount)
                    {
                        throw new Exception("최대 검사 횟수를 초과하였습니다.");
                    }

                    // FID 일치 여부 검사.
                    int fid = AppSettings.CurrentFid;
                    if (!dioManualMode)
                    {
                        bool containsFid = currentProduct.Project.FIDs.Contains(fid);
                        Utils.InvokeIfRequired(this, () =>
                        {
                            UpdateFidStatus(containsFid);
                        });
                        if (!containsFid)
                        {
                            throw new Exception($"FID={fid} 이(가) 프로젝트에 포함되지 않았습니다.");
                        }
                    }

                    Utils.InvokeIfRequired(this, () =>
                    {
                        InitStageUI();

                        // 테스트 Phase 초기화.
                        ictTitleLabel.BackColor = sectionTitleBackColor;
                        ictPowerTitleLabel.BackColor = sectionTitleBackColor;
                        ispTitleLabel.BackColor = sectionTitleBackColor;
                        jtagTitleLabel.BackColor = sectionTitleBackColor;
                        funcTitleLabel.BackColor = sectionTitleBackColor;
                        eolTitleLabel.BackColor = sectionTitleBackColor;
                        ext1TitleLabel.BackColor = sectionTitleBackColor;
                        ext2TitleLabel.BackColor = sectionTitleBackColor;

                        // Board 초기화.
                        board1BarcodeLabel.Text = "";
                        board2BarcodeLabel.Text = "";
                        board3BarcodeLabel.Text = "";
                        board4BarcodeLabel.Text = "";
                        board2TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                        if (currentProduct.Project.Panel == 2 && !currentProduct.Project.TwoBoardsLeftRight)
                        {
                            // 2연배열 상/하 배치.
                            if (currentProduct.Project.BottomBoardFirst)
                            {
                                // 밑의 보드가 첫번째.
                                board1TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                                board3TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                            }
                            else
                            {
                                // 밑의 보드가 두번째.
                                board1TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                                board3TLPanel.BackColor = currentProduct.Project.Board2Checked ? boardBackColor : boardDisabledColor;
                            }
                        }
                        else
                        {
                            board1TLPanel.BackColor = currentProduct.Project.Board1Checked ? boardBackColor : boardDisabledColor;
                            board3TLPanel.BackColor = currentProduct.Project.Board3Checked ? boardBackColor : boardDisabledColor;
                        }
                        board4TLPanel.BackColor = currentProduct.Project.Board4Checked ? boardBackColor : boardDisabledColor;
                    });

                    // 각종 상태 체크를 위한 상태 읽기.
                    if (!dioManualMode)
                    {
                        plcStatus = ViewModel.PlcReadStatus();
                    }
                    else
                    {
                        dioRWStatus = ViewModel.DioRWReadStatus();
                    }

                    // Auto 모드 확인.
                    bool autoMode;
                    if (!dioManualMode)
                    {
                        autoMode = (plcStatus & PlcReadFlags.Mode) == 0;
                        Utils.InvokeIfRequired(this, () =>
                        {
                            SetAutoMode(autoMode);
                        });
                    }
                    else
                    {
                        autoMode = false;
                    }

                    if (AppSettings.CheckAutoMode)
                    {
                        if (!autoMode)
                        {
                            throw new Exception("자동 모드가 아닙니다.");
                        }
                    }

                    // 공압 체크.
                    if (AppSettings.CheckPneumatic)
                    {
                        bool isNormal;
                        if (!dioManualMode)
                        {
                            isNormal = (plcStatus & PlcReadFlags.AirPressure) != 0;
                        }
                        else
                        {
                            isNormal = (dioRWStatus & DioReadFlags.AirSensorOut) != 0;
                        }
                        if (!isNormal)
                        {
                            throw new Exception("공압이 비정상입니다.");
                        }
                    }

                    // 도어 오픈 상태 체크.
                    if (AppSettings.CheckDoorOpenStatus)
                    {
                        bool isOpened;
                        if (!dioManualMode)
                        {
                            isOpened = (plcStatus & PlcReadFlags.FrontDoor) == 0 || (plcStatus & PlcReadFlags.RearDoor) == 0;
                        }
                        else
                        {
                            isOpened = (dioRWStatus & DioReadFlags.DoorSensor) == 0;
                        }
                        if (isOpened)
                        {
                            throw new Exception("도어가 열려있습니다.");
                        }
                    }

                    // 안전센서 체크.
                    if (!dioManualMode)
                    {
                        if ((plcStatus & PlcReadFlags.SafetySensor) == 0)
                        {
                            throw new Exception("안전센서가 비정상입니다.");
                        }
                    }
                    else
                    {
                        if ((dioRWStatus & DioReadFlags.SafetySensor) == 0)
                        {
                            throw new Exception("안전센서가 비정상입니다.");
                        }
                    }

                    // FIXTURE LOCK.
                    if (!dioManualMode)
                    {
                        if ((plcStatus & PlcReadFlags.TopFixtureSensor) == 0 || (plcStatus & PlcReadFlags.BottomFixtureSensor) == 0)
                        {
                            throw new Exception("FIXTURE가 Lock 되지 않았습니다.");
                        }
                    }

                    // 중단 검사.
                    if (userClickedStop)
                    {
                        throw new Exception(UserAbortedMessage);
                    }

                    // 픽스처 파워 켜기.
                    if (!dioManualMode)
                    {
                        if (isFirstRun)
                        {
                            ViewModel.DioOpen();
                            ViewModel.DioFixturePower(true, true);
                        }
                    }

                    bool novaEnabled = false;
                    var grpList = currentProduct.Project.GetOrderedGrpInfos();
                    if (grpList != null)
                    {
                        for (int i = 0; i < grpList.Count; i++)
                        {
                            if (!string.IsNullOrWhiteSpace(grpList[i].GrpFilePath))
                            {
                                novaEnabled = true;
                                break;
                            }
                        }
                    }

                    // 실행할 섹션 리스트.
                    var sectionInfos = currentProduct.Project.GetOrderedSectionInfos(out int ispSectionIndex, out int eolFirstSectionIndex, out int ictFirstSectionIndex);

                    //if (!sectionInfos[ispSectionIndex].Enabled || string.IsNullOrEmpty(sectionInfos[ispSectionIndex].SectionName))
                    //{
                    //    ispSectionEnabled = false;
                    //}

                    // 실행할 섹션 중에 isp 문자열을 포함하는 섹션 이름이 있으면 isp 실행으로 간주.
                    ispSectionEnabled = sectionInfos.Where(info => info.Enabled && info.IsIspSection).Any();

                    if (isFirstRun && novaEnabled && ispSectionEnabled)
                    {
                        //
                        // Novaflash 체크.
                        //

                        foreach (var grp in grpList)
                        {
                            if (File.Exists(grp.GrpFilePath))
                            {
                                // 중단 검사.
                                if (userClickedStop)
                                {
                                    throw new Exception(UserAbortedMessage);
                                }

                                // 핸들러 이상상태 체크.
                                if (handlerErrorOccurred)
                                {
                                    throw new Exception(handlerErrorMessage);
                                }

                                string createdGrpPath = null;
                                try
                                {
                                    // CRC 체크.
                                    createdGrpPath = ViewModel.CreateGRP(grp.GrpFilePath, grp.ImportFilePath);
                                    var uploadGrpPath = createdGrpPath ?? grp.GrpFilePath;
                                    var actualCrc = Novaflash.GetDataCrc32(uploadGrpPath);
                                    if (grp.Crc != actualCrc)
                                    {
                                        throw new Exception($"GRP({grp.RomFileName}) 파일의 DATA CRC 오류입니다.");
                                    }

                                    // GRP 파일 전송.

                                    // 먼저, 이 GRP 파일이 Hydra 디바이스에 있는지 검사한다.
                                    // 로컬과 Hydra 디바이스에 있는 GRP 파일의 이름, GRP CRC 둘 다 같으면 같은 파일로 간주.
                                    uint localGrpCrc = Novaflash.GetGrpCrc32(uploadGrpPath);
                                    string destFileName = GrpInfo.GetUploadFileName(grp.GrpFilePath, grp.ImportFilePath);
                                    bool grpExists = ViewModel.NovaGrpExists(destFileName, localGrpCrc);
                                    if (grpExists)
                                    {
                                        continue;
                                    }

                                    string message = $"NovaFlash: Uploading {grp.RomFileName}...";
                                    Logger.LogTimedMessage(message);

                                    // 상태 메시지 표시.
                                    Utils.InvokeIfRequired(this, () =>
                                    {
                                        promptLabel.Text = message;
                                    });
                                    PromptBlinkStart(promptLabel);

                                    // GRP 파일 업로드.
                                    ViewModel.NovaOpen();

                                    ViewModel.NovaSendGrpFile(uploadGrpPath, destFileName);

                                    message = "NovaFlash: Uploading completed";
                                    Logger.LogTimedMessage(message);

                                    // 상태 메시지 표시.
                                    PromptBlinkStop(promptLabel, true);
                                    Utils.InvokeIfRequired(this, () =>
                                    {
                                        promptLabel.Text = message;
                                    });
                                }
                                finally
                                {
                                    // 업로드가 끝나면 생성된 임시파일 제거.
                                    if (!string.IsNullOrWhiteSpace(createdGrpPath))
                                    {
                                        File.Delete(createdGrpPath);
                                    }
                                }
                            }
                            else if (!string.IsNullOrWhiteSpace(grp.GrpFilePath))
                            {
                                throw new Exception("GRP file does not exist: " + grp.GrpFilePath);
                            }
                        }

                        // 중단 검사.
                        if (userClickedStop)
                        {
                            throw new Exception(UserAbortedMessage);
                        }

                        // 핸들러 이상상태 체크.
                        if (handlerErrorOccurred)
                        {
                            throw new Exception(handlerErrorMessage);
                        }

                        // POD 전원 켜기.
                        bool[] chEnabled = new bool[] { false, false, false, false };
                        foreach (var grp in grpList)
                        {
                            if (grp.Channel >= 1 && grp.Channel <= 4 && !string.IsNullOrWhiteSpace(grp.GrpFilePath))
                            {
                                chEnabled[grp.Channel - 1] = true;
                            }
                        }
                        ViewModel.NovaPodPower(true, chEnabled[0], chEnabled[1], chEnabled[2], chEnabled[3]);

                        // POD 상태 업데이트.
                        ThreadPool.QueueUserWorkItem(UpdatePodStatus, 1000);
                    }

                    if (isFirstRun)
                    {
                        // GRP 파일 이름 임시파일에 저장.
                        MainViewModel.SaveNovaflashData(currentProduct.Project);

                        // JTAG 관련 설정 저장.
                        MainViewModel.SaveJtagProjectInfo(currentProduct.Project.JtagProjectName);
                    }

                    // Series 서버 시작.
                    if (AppSettings.SeriesConnOption == SeriesOption.SeriesNext && !MainViewModel.SeriesServerRunning())
                    {
                        MainViewModel.SeriesServerStart();
                        Thread.Sleep(50);
                        if (!MainViewModel.SeriesServerRunning())
                        {
                            throw new Exception("직렬연결 서버를 시작할 수 없습니다.");
                        }
                    }

                    // Series 서버에 연결.
                    if (AppSettings.SeriesConnOption == SeriesOption.SeriesPrev && !MainViewModel.SeriesConnected())
                    {
                        // 직렬연결 앞설비가 서버에 연결할 수 없으면 에러 발생.
                        try
                        {
                            MainViewModel.SeriesConnect();
                        }
                        catch (Exception ex)
                        {
                            throw new Exception($"직렬연결 서버 접속 오류: {ex.Message}");
                        }
                    }

                    // PCB IN 스테이지 시작.
                    StageBlinkStart(stagePcbInLabel);

                    // PCB 스캔존에 투입.

                    if (autoMode)
                    {
                        // 앞 사이클에서 MES NG가 나온 것이 있으면 에러 출력.
                        if (pcbMesResult == PcbMesResult.NG)
                        {
                            if (AppSettings.PrevFailAction == PrevProcFailAction.Stop)
                            {
                                // 테스트 정지.
                                throw new Exception(pcbMesErrorMessage);
                            }
                            else
                            {
                                // 테스트 NG로 처리.
                            }
                        }
                    }

                    waitingPcbIn = true;

                    if (autoMode)
                    {
                        ViewModel.PlcOpen();
                        // 스캔존, 테스트존에 PCB가 없으면 무한 대기.
                        if (pcbScanZone == PcbTestResult.NoPcb && pcbMesResult == PcbMesResult.NoPcb)
                        {
                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = true;

                            pcbScanZone = ViewModel.PlcMovePcb(PcbScanZone.Necessary, PcbMesResult.NoPcb, PcbTestResult.NoPcb);

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested;

                            if (pcbScanZone == PcbTestResult.NoPcb)
                            {
                                // 무한 대기 단계를 거쳤는데 PCB가 없으면 기구 오류.
                                throw new Exception("알 수 없는 기구 오류가 발생했습니다.");
                            }
                        }
                    }

                    waitingPcbIn = false;

                    // PCB IN 스테이지 완료.
                    StageBlinkStop(stagePcbInLabel, true);

                    // 중단 검사.
                    if (userClickedStop)
                    {
                        throw new Exception(UserAbortedMessage);
                    }

                    // 핸들러 이상상태 체크.
                    if (handlerErrorOccurred)
                    {
                        throw new Exception(handlerErrorMessage);
                    }

                    // 실행할 섹션 리스트.
                    bool ictEnabled = ictFirstSectionIndex >= 0;
                    bool eolEnabled = eolFirstSectionIndex >= 0;

                    // SCAN 스테이지 시작.
                    StageBlinkStart(stageScanLabel);

                    var prevMesResult = pcbMesResult;
                    var prevMesBarcode = pcbMesBarcode;
                    if (autoMode)
                    {
                        // PCB가 스캔존에 있으면 바코드 읽기 및 MES통신.
                        if (pcbScanZone != PcbTestResult.NoPcb)
                        {
                            ScanAndCheckMes(ictEnabled, out pcbMesResult, out pcbMesBarcode, out pcbMesErrorMessage);
                        }
                        else
                        {
                            pcbMesResult = PcbMesResult.NoPcb;
                        }
                    }
                    else
                    {
                        // 수동모드이면 바코드 읽기.
                        if (AppSettings.ShouldScanBarcode || AppSettings.MesEnabled)
                        {
                            Utils.InvokeIfRequired(this, () =>
                            {
                                promptLabel.Text = "수동모드: 테스트하려는 PCB의 대표 바코드를 스캔하세요.";
                            });

                            PromptBlinkStart(promptLabel);

                            string barcode = "";
                            int retryCount = AppSettings.BarcodeRetryCount;
                            int retryInterval = AppSettings.BarcodeRetryInterval;
                            int readCount = 0;
                            do
                            {
                                try
                                {
                                    // 핸들러 이상상태 체크.
                                    if (handlerErrorOccurred)
                                    {
                                        throw new Exception(handlerErrorMessage);
                                    }

                                    barcodeReading = true;
                                    fidScanner.Open();
                                    barcode = fidScanner.ReadBarcode()?.Trim();
                                    if (!string.IsNullOrEmpty(barcode))
                                    {
                                        break;
                                    }
                                }
                                catch (TimeoutException te)
                                {
                                    Logger.LogTimedMessage("바코드 읽기 타임아웃: " + te.Message);

                                    readCount++;
                                    if (retryInterval > 0 && readCount <= retryCount)
                                    {
                                        Thread.Sleep(retryInterval);
                                    }

                                    // 중단 검사.
                                    if (userClickedStop)
                                    {
                                        throw new Exception(UserAbortedMessage);
                                    }

                                    // 핸들러 이상상태 체크.
                                    if (handlerErrorOccurred)
                                    {
                                        throw new Exception(handlerErrorMessage);
                                    }
                                }
                                finally
                                {
                                    fidScanner.Close();
                                    barcodeReading = false;
                                }
                            } while (readCount <= retryCount);

                            PromptBlinkStop(promptLabel, true);

                            Utils.InvokeIfRequired(this, () =>
                            {
                                promptLabel.Text = "";
                            });

                            // 바코드를 못읽었으면 오류 발생.
                            if (string.IsNullOrEmpty(barcode))
                            {
                                throw new Exception("바코드 읽기에 실패하였습니다.");
                            }
                            else
                            {
                                // 바코드 검사 및 MES 통신.
                                bool success = CheckMes(ictEnabled, barcode, out string errorMessage);
                                if (!success)
                                {
                                    throw new Exception(errorMessage);
                                }
                            }

                            CurrentBarcode = barcode;
                            DisplayBarcode(barcode);

                            // 바코드 차종코드 검사.
                            if (AppSettings.CarTypeCodeLength > 0)
                            {
                                int startIndex = Math.Max(0, AppSettings.CarTypeCodeStartPosition - 1);
                                var carTypeCode = barcode.Substring(startIndex, AppSettings.CarTypeCodeLength);
                                if (!MainViewModel.ContainsCarTypeCode(currentProduct.CarTypeCode, carTypeCode))
                                {
                                    throw new Exception("프로젝트와 PCB의 차종코드가 일치하지 않습니다.");
                                }
                            }
                        }
                        else
                        {
                            // 바코드 없이 수동 테스트.
                            CurrentBarcode = "";
                        }
                    }

                    // SCAN 스테이지 완료.
                    StageBlinkStop(stageScanLabel, true);

                    // 중단 검사.
                    if (userClickedStop)
                    {
                        throw new Exception(UserAbortedMessage);
                    }

                    // 핸들러 이상상태 체크.
                    if (handlerErrorOccurred)
                    {
                        throw new Exception(handlerErrorMessage);
                    }

                    // PCB 테스트존으로 투입.
                    bool mesNgAsTestNg = false;
                    if (autoMode)
                    {
                        // 앞 사이클의 컨베이어 이동에서 투입한 PCB가 없으면 투입.
                        if (prevMesResult == PcbMesResult.NoPcb)
                        {
                            // 테스트존에 PCB가 없으므로, 스캔존의 PCB 바코드 표시.
                            CurrentBarcode = pcbMesBarcode;
                            DisplayBarcode(pcbMesBarcode);

                            // 테스트존에 PCB가 없는데 스캔존에 PCB가 있고 그 결과가 MES NG라면 에러 출력.
                            if (pcbMesResult == PcbMesResult.NG)
                            {
                                if (AppSettings.PrevFailAction == PrevProcFailAction.Stop)
                                {
                                    throw new Exception(pcbMesErrorMessage);
                                }
                                else
                                {
                                    // MES NG를 TEST NG로 처리할지 여부.
                                    mesNgAsTestNg = true;
                                }
                            }

                            // 앞에서 테스트한 결과.
                            prevResult = pcbScanZone;

                            // 테스트존으로 PCB 투입.

                            waitingPcbIn = true;
                            pcbScanZone = ViewModel.PlcMovePcb(PcbScanZone.Optional, mesNgAsTestNg ? PcbMesResult.OK : pcbMesResult, PcbTestResult.NoPcb);
                            waitingPcbIn = false;

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested && prevResult != PcbTestResult.NotTested;

                            // 스캔존에 새로 투입된 PCB가 있으면, 바코드를 스캔하고 MES와 통신한다.
                            if (pcbScanZone != PcbTestResult.NoPcb)
                            {
                                StageBlinkStart(stageScanLabel);
                                ScanAndCheckMes(ictEnabled, out pcbMesResult, out pcbMesBarcode, out pcbMesErrorMessage);
                                StageBlinkStop(stageScanLabel, true);
                            }
                            else
                            {
                                pcbMesResult = PcbMesResult.NoPcb;
                            }
                        }
                        else
                        {
                            // 테스트존에 앞 사이클에서 투입한 PCB가 있으므로 그 바코드 표시.
                            CurrentBarcode = prevMesBarcode;
                            DisplayBarcode(prevMesBarcode);

                            // MES NG를 TEST NG로 처리할지 여부.
                            if (prevMesResult == PcbMesResult.NG)
                            {
                                mesNgAsTestNg = true;
                            }
                        }

                        // 테스트존으로 PCB가 투입되면, 테스트를 할 것인지 결정.

                        // MES NG가 나올 때 TEST NG로 처리하는 경우.
                        if (mesNgAsTestNg)
                        {
                            StageBlinkStart(stagePcbOutLabel);
                            waitingPcbOut = true;

                            mesNgAsTestNg = pcbMesResult == PcbMesResult.NG && AppSettings.PrevFailAction != PrevProcFailAction.Stop;

                            // 앞에서 테스트한 결과.
                            prevResult = pcbScanZone;

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested;

                            pcbScanZone = ViewModel.PlcMovePcb(PcbScanZone.Optional, mesNgAsTestNg ? PcbMesResult.OK : pcbMesResult, PcbTestResult.Fail);

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested && prevResult != PcbTestResult.NotTested;

                            waitingPcbOut = false;
                            StageBlinkStop(stagePcbOutLabel, true);

                            continue;
                        }

                        // 직렬연결 앞설비의 경우, 뒷설비의 투입요구가 있으면 테스트하지 않고 그냥 보낸다.
                        if (AppSettings.SeriesConnOption == SeriesOption.SeriesPrev)
                        {
                            bool passToNext = false;
                            try
                            {
                                // Capacity 문의.
                                int capacity = MainViewModel.SeriesGetCapacity();
                                passToNext = capacity > 0;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception($"직렬연결 통신 오류: {ex.Message}");
                            }

                            if (passToNext)
                            {
                                StageBlinkStart(stagePcbOutLabel);
                                waitingPcbOut = true;

                                mesNgAsTestNg = pcbMesResult == PcbMesResult.NG && AppSettings.PrevFailAction != PrevProcFailAction.Stop;

                                // 앞에서 테스트한 결과.
                                prevResult = pcbScanZone;

                                pcbScanZone = ViewModel.PlcMovePcb(PcbScanZone.Optional, mesNgAsTestNg ? PcbMesResult.OK : pcbMesResult, PcbTestResult.NotTested);

                                waitingPcbOut = false;
                                StageBlinkStop(stagePcbOutLabel, true);

                                continue;
                            }
                        }

                        // 직렬연결 뒷설비의 경우, 테스트하지 않은 보드만 테스트한다.
                        if (AppSettings.SeriesConnOption == SeriesOption.SeriesNext && prevResult != PcbTestResult.NotTested && prevResult != PcbTestResult.NoPcb)
                        {
                            StageBlinkStart(stagePcbOutLabel);
                            waitingPcbOut = true;

                            mesNgAsTestNg = pcbMesResult == PcbMesResult.NG && AppSettings.PrevFailAction != PrevProcFailAction.Stop;

                            // 앞에서 테스트한 결과.
                            var testResult = prevResult;
                            prevResult = pcbScanZone;

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested;

                            pcbScanZone = ViewModel.PlcMovePcb(PcbScanZone.Optional, mesNgAsTestNg ? PcbMesResult.OK : pcbMesResult, testResult);

                            // 직렬연결을 위해 Capacity를 설정.
                            SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested && prevResult != PcbTestResult.NotTested;

                            waitingPcbOut = false;
                            StageBlinkStop(stagePcbOutLabel, true);

                            continue;
                        }

                        // 직렬연결을 위해 Capacity를 설정.
                        SeriesServer.SharedServer.Capable = false;
                    }
                    else
                    {
                        // 수동모드이면 PCB를 장착하라는 메시지 표시.
                        while (true)
                        {
                            string message;
                            if (!dioManualMode)
                            {
                                message = "PCB를 컨베이어 위에서 스토퍼에 밀착시켜 장착하고 OK 버튼을 누르세요.";
                            }
                            else
                            {
                                message = "PCB를 하부 픽스처 위에 장착하세요.";
                            }

                            var result = MessageDialog.ShowModal(message, "수동모드 PCB 장착", true, true);
                            if (result != DialogResult.OK)
                            {
                                userClickedStop = true;
                                throw new Exception("사용자에 의해 수동모드 생산이 중지되었습니다.");
                            }

                            // PCB 안착 여부 체크.
                            bool pcbExists;
                            if (!dioManualMode)
                            {
                                plcStatus = ViewModel.PlcReadStatus();
                                pcbExists = (plcStatus & PlcReadFlags.PcbSensor) != 0;
                            }
                            else
                            {
                                pcbExists = true;
                            }

                            Utils.InvokeIfRequired(this, () =>
                            {
                                promptLabel.Text = pcbExists ? "" : "PCB가 감지되지 않았습니다.";
                            });

                            if (pcbExists)
                            {
                                // 안전센서 검사.
                                if (!dioManualMode)
                                {
                                    bool isSafe = (plcStatus & PlcReadFlags.SafetySensor) != 0;
                                    if (!isSafe)
                                    {
                                        Utils.InvokeIfRequired(this, () =>
                                        {
                                            promptLabel.Text = "안전센서 이상이 감지되었습니다.";
                                        });
                                        continue;
                                    }
                                }
                                break;
                            }
                        }
                    }

                    // Testing 스테이지 시작.
                    StageBlinkStart(stageTestingLabel);

                    // 시험이 연속적으로 반복되는 경우, 마지막 테스트 상태가 빨리 없어지는 것을 막기 위해 여기에서 상태 설정.
                    SetCurrentTestStatus(TestStatus.Running);

                    // Power Off.
                    if (isFirstRun)
                    {
                        DoPower12Off(null);
                    }

                    // 테스트 시간 측정 시작.
                    Utils.InvokeIfRequired(this, () =>
                    {
                        testDurationLabel.Text = "";
                    });
                    Stopwatch testDurationWatch = Stopwatch.StartNew();

                    // 중단 검사.
                    if (userClickedStop)
                    {
                        throw new Exception(UserAbortedMessage);
                    }

                    // 핸들러 이상상태 체크.
                    if (handlerErrorOccurred)
                    {
                        throw new Exception(handlerErrorMessage);
                    }

                    // 실행할 프로젝트 결정.
                    currentProduct.Project.GetIctProjectName(CurrentBarcode, out string ictProjectName, out string projectSuffix, out bool? masterGood);

                    // 마스터보드 여부 저장.
                    EOL_GND.ViewModel.SequenceViewModel.SetIsMasterBoard(masterGood);

                    bool[] boardChecked = new bool[]
                    {
                        currentProduct.Project.Board1Checked,
                        currentProduct.Project.Board2Checked,
                        currentProduct.Project.Board3Checked,
                        currentProduct.Project.Board4Checked,
                    };
                    Control[] boardControls;
                    if (currentProduct.Project.Panel == 2 && !currentProduct.Project.TwoBoardsLeftRight)
                    {
                        // 2연배 상/하 배치.
                        if (currentProduct.Project.BottomBoardFirst)
                        {
                            // 밑의 보드가 첫번째.
                            boardControls = new Control[]
                            {
                                board3TLPanel,
                                board1TLPanel,
                                board2TLPanel,
                                board4TLPanel,
                            };
                        }
                        else
                        {
                            // 밑의 보드가 두번째.
                            boardControls = new Control[]
                            {
                                board1TLPanel,
                                board3TLPanel,
                                board2TLPanel,
                                board4TLPanel,
                            };
                        }
                    }
                    else
                    {
                        boardControls = new Control[]
                        {
                            board1TLPanel,
                            board2TLPanel,
                            board3TLPanel,
                            board4TLPanel,
                        };
                    }
                    var sectionTitleLabels = new Label[] { 
                        ictTitleLabel, 
                        ictPowerTitleLabel, 
                        ispTitleLabel, 
                        jtagTitleLabel,
                        funcTitleLabel,
                        eolTitleLabel,
                        ext1TitleLabel,
                        ext2TitleLabel,
                    };

                    int testRetryCount = AppSettings.TestRetryCount;
                    if (masterGood == false)
                    {
                        // 불량 마스터 보드는 다른 retry count를 이용.
                        testRetryCount = AppSettings.NgMasterRetryCount;
                    }
                    int testRetryInterval = AppSettings.TestRetryInterval;

                    // eloZ1 실행시간 측정 시작.
                    Utils.InvokeIfRequired(this, () =>
                    {
                        promptLabel.Text = "";

                        TestLogClear();
                        TestLogAppendLine($"Model: {currentProduct.Project.Model}");
                        TestLogAppendLine($"S/N: {barcodeLabel.Text}");
                        TestLogAppendLine($"Date: {DateTime.Now}");
                        TestLogAppendLine($"Proj: {currentProduct.Project.IctProjectName} {projectSuffix}");

                        testProgressBar.Style = ProgressBarStyle.Marquee;
                    });
                    elozDurationWatch.Reset();
                    elapsedTimer.Start();

                    long elozTotalDurationPrev = 0;
                    ViewModel.LoggedSteps = 0;
                    ViewModel.ClearLog();
                    ViewModel.ClearNotepadLog();

                    // Fixture Probe Count 가져오기.
                    CurrentProbeCount = MainViewModel.GetProbeCount(fid);
                    DisplayProbeCount(CurrentProbeCount);

                    // 테스트를 계속 진행하는 경우, Pass/Fail 저장.
                    var boardPassed = new bool?[sectionInfos.Count, currentProduct.Project.Panel];
                    for (int i = 0; i < boardPassed.GetLength(0); i++)
                    {
                        for (int j = 0; j < boardPassed.GetLength(1); j++)
                        {
                            boardPassed[i, j] = null;
                        }
                    }

                    // 프레스 위치 조정을 위한 변수.
                    bool isCylinderFct = false;
                    bool isCylinderDown = false;

                    // 섹션별로 Board1, Board2, ... Boardn 실행.
                    int sectionIndex;
                    long ictElapsedMilliseconds = -1;

                    // 보드별 테스트 시간 저장.
                    var boardTestTimes = new long[sectionInfos.Count, currentProduct.Project.Panel];
                    for (int i = 0; i < boardTestTimes.GetLength(0); i++)
                    {
                        for (int j = 0; j < boardTestTimes.GetLength(1); j++)
                        {
                            boardTestTimes[i, j] = 0;
                        }
                    }

                    bool ictPassed = true;
                    for (sectionIndex = 0; sectionIndex < sectionInfos.Count; sectionIndex++)
                    {
                        // 핸들러 이상상태 체크.
                        if (handlerErrorOccurred)
                        {
                            throw new Exception(handlerErrorMessage);
                        }

                        if (sectionIndex == eolFirstSectionIndex)
                        {
                            ictElapsedMilliseconds = elozDurationWatch.ElapsedMilliseconds;
                        }

                        var sectionInfo = sectionInfos[sectionIndex];
                        if (!sectionInfo.Enabled || string.IsNullOrEmpty(sectionInfo.SectionName))
                        {
                            continue;
                        }

                        // MES 통신.
                        if (ictEnabled && eolEnabled && sectionIndex == eolFirstSectionIndex)
                        {
                            // ICT 검사결과 전송.
                            ictPassed = MainViewModel.CheckIctPassed(boardPassed, eolFirstSectionIndex);
                            SendMesResult(true, CurrentBarcode, ictPassed);

                            // ICT 검사가 FAIL이고 마스터보드가 아니면 EOL 검사 하지 않음.
                            if (!ictPassed && string.IsNullOrEmpty(projectSuffix))
                            {
                                // FAIL로 처리하고 다음 PCB 테스트.
                                break;
                            }

                            // EOL 이전공정 체크.
                            bool eolCheckResult = CheckMes(false, CurrentBarcode, out string mesErrorMessage);
                            if (!eolCheckResult)
                            {
                                throw new Exception(mesErrorMessage);
                            }
                        }

                        PhaseBlinkStart(sectionTitleLabels[sectionIndex]);

                        bool sectionPassed = false;
                        bool shouldContinue = false;

                        for (int boardIndex = 0; boardIndex < currentProduct.Project.Panel; boardIndex++)
                        {
                            int sectionRetryCount = testRetryCount;
                            if (!boardChecked[boardIndex])
                            {
                                continue;
                            }

                            BoardBlinkStart(boardControls[boardIndex]);
                            string boardName = $"{TestProject.BoardPrefix}{boardIndex + 1}";
                            string sectionName;
                            if (currentProduct.Project.Panel > 1)
                            {
                                sectionName = $"{sectionInfo.SectionName}({boardName})";
                            }
                            else
                            {
                                sectionName = sectionInfo.SectionName;
                            }

                            // JTAG를 위해 바코드 저장.
                            //if (sectionIndex == jtagSectionIndex)
                            //{
                            //    MainViewModel.SaveJtagBarcodeInfo(1, MainViewModel.IncreaseBarcode(currentBarcode, boardIndex));
                            //}

                            // NovaFlash 다운로드 스크립트에서 다시 테스트할지 여부 체크하는 변수 초기화.
                            if (sectionInfo.IsIspSection)
                            {
                                Eloz1.SetGlobalStorageValue(MainViewModel.IspRetestVarName, null);
                            }

                            do
                            {
                                // 중단 검사.
                                if (userClickedStop)
                                {
                                    throw new Exception("User stopped the test.");
                                }

                                // 핸들러 이상상태 체크.
                                if (handlerErrorOccurred)
                                {
                                    throw new Exception(handlerErrorMessage);
                                }

                                // 프레스 다운 설정에 따라 프레스 컨트롤.
                                if (!dioManualMode)
                                {
                                    ViewModel.PlcOpen();
                                }
                                else
                                {
                                    ViewModel.DioRWOpen();
                                }

                                if (sectionInfo.PressDown && !isCylinderDown)
                                {
                                    if (!dioManualMode)
                                    {
                                        if (sectionInfo.PressUp)
                                        {
                                            ViewModel.PlcCylinderMidUp();
                                        }
                                        ViewModel.PlcCylinderDown();
                                    }
                                    else
                                    {
                                        if (sectionInfo.PressUp)
                                        {
                                            ViewModel.DioRWCylinderUp();
                                        }
                                        ViewModel.DioRWCylinderDown();
                                    }

                                    isCylinderDown = true;
                                    isCylinderFct = false;

                                    // Fixture Probe Count 가져오기.
                                    CurrentProbeCount = MainViewModel.GetProbeCount(fid);
                                    
                                    // Async 모드이면 Probe Count 수동 증가.
                                    if (!AppSettings.ProbeCountSyncMode && CurrentProbeCount != null)
                                    {
                                        // Fixture Probe Count 가져오기.
                                        CurrentProbeCount = MainViewModel.GetProbeCount(fid);
                                        CurrentProbeCount.TotalProbeCount++;
                                    }

                                    DisplayProbeCount(CurrentProbeCount);
                                }
                                else if (!sectionInfo.PressDown && !isCylinderFct)
                                {
                                    if (!dioManualMode)
                                    {
                                        if (!isCylinderDown)
                                        {
                                            if (sectionInfo.PressUp)
                                            {
                                                ViewModel.PlcCylinderMidUp();
                                            }
                                            ViewModel.PlcCylinderDown();
                                        }
                                        Thread.Sleep(200);
                                        ViewModel.PlcCylinderFctUp();
                                    }
                                    else
                                    {
                                        if (sectionInfo.PressUp)
                                        {
                                            ViewModel.DioRWCylinderUp();
                                        }
                                        ViewModel.DioRWCylinderDown();
                                        ViewModel.DioRWCylinderFctUp();
                                    }

                                    isCylinderDown = false;
                                    isCylinderFct = true;
                                }

                                // 핸들러 이상상태 체크.
                                if (handlerErrorOccurred)
                                {
                                    throw new Exception(handlerErrorMessage);
                                }

                                // DIO를 닫는다.
                                if (!dioManualMode)
                                {
                                    ViewModel.DioClose();
                                }
                                Logger.LogTimedMessage("Closed DIO port, starting eloz test...");

                                // EOL 로그 제거.
                                ViewModel.RemoveEolLog(boardIndex);
                                ViewModel.EolBoardIndex = boardIndex;

                                elozDurationWatch.Start();
                                ViewModel.Eloz1Open(ictProjectName);
                                ViewModel.Eloz1Run(sectionName, currentProduct.Project.IctVariantName);
                                elozDurationWatch.Stop();

                                Logger.LogTimedMessage("Finished eloz test");

                                // 실행시간 출력.
                                long elapsed = elozDurationWatch.ElapsedMilliseconds - elozTotalDurationPrev;
                                elozTotalDurationPrev = elozDurationWatch.ElapsedMilliseconds;
                                string message = $"{sectionName} Test Time: {elapsed} ms";
                                Utils.InvokeIfRequired(this, () =>
                                {
                                    TestLogAppendLine(message + Environment.NewLine);
                                });

                                // 테스트 시간 저장.
                                boardTestTimes[sectionIndex, boardIndex] = elapsed;

                                bool final = Eloz1FinishState == ResultState.Pass || !sectionInfo.RetryEnabled || sectionRetryCount < 1;
                                if (final)
                                {
                                    // 최종결과만 로그파일에 저장한다.
                                    ViewModel.CreateLog(boardIndex, eolEnabled && sectionIndex >= eolFirstSectionIndex);

                                    if (Eloz1FinishState != ResultState.Pass)
                                    {
                                        ViewModel.CreateNotepadLog();
                                        ViewModel.CreateEolNotepadLog(sectionName);
                                    }

                                    break;
                                }

                                // 전원 Off.
                                DoPower12Off(null);

                                Logger.LogTimedMessage($"시험 재시도 남은 횟수: {sectionRetryCount}");

                                // 핸들러 이상상태 체크.
                                if (handlerErrorOccurred)
                                {
                                    throw new Exception(handlerErrorMessage);
                                }

                                // 실린더 올림.
                                if (!dioManualMode)
                                {
                                    ViewModel.PlcOpen();
                                    ViewModel.PlcCylinderMidUp();
                                }
                                else
                                {
                                    ViewModel.DioRWOpen();
                                    ViewModel.DioRWCylinderUp();
                                }
                                isCylinderFct = false;
                                isCylinderDown = false;

                                sectionRetryCount--;
                                if (testRetryInterval > 0)
                                {
                                    Thread.Sleep(testRetryInterval);
                                }

                                // NovaFlash 다운로드 스크립트에서 다시 테스트할지 여부 체크하도록 저장.
                                if (sectionInfo.IsIspSection)
                                {
                                    Eloz1.SetGlobalStorageValue(MainViewModel.IspRetestVarName, true);
                                }
                            }
                            while (sectionRetryCount >= 0);

                            // Board 실행결과 표시.
                            sectionPassed = Eloz1FinishState == ResultState.Pass;
                            BoardBlinkStop(boardControls[boardIndex], true);
                            Utils.InvokeIfRequired(this, () =>
                            {
                                boardControls[boardIndex].BackColor = sectionPassed ? resultPassColor : resultFailColor;
                            });

                            // 실행결과 저장.
                            boardPassed[sectionIndex, boardIndex] = sectionPassed;

                            // 테스트를 계속할 것인지 판단.
                            if (AppSettings.SectionFailAction == SectionFailAction.Stop)
                            {
                                shouldContinue = false;
                            }
                            else
                            {
                                bool shouldCheckShort = AppSettings.SectionFailAction == SectionFailAction.ContinueNoShort
                                    || (AppSettings.SectionFailAction == SectionFailAction.ContinueMasterNoShort && projectSuffix != "");
                                if (!sectionPassed && shouldCheckShort)
                                {
                                    // 테스트 결과를 체크해 Open, Short 테스트 Fail 인지 체크.
                                    shouldContinue = ViewModel.IsOpenShortFail();
                                }
                                else
                                {
                                    if ((AppSettings.SectionFailAction == SectionFailAction.ContinueMaster
                                        || AppSettings.SectionFailAction == SectionFailAction.ContinueMasterNoShort) && projectSuffix == "")
                                    {
                                        // Master 보드만 계속하라고 한 경우, Master 보드가 아닐 때.
                                        shouldContinue = false;
                                    }
                                    else
                                    {
                                        shouldContinue = true;
                                    }
                                }
                            }

                            if (!sectionPassed && !shouldContinue)
                            {
                                break;
                            }
                        }

                        // ICT, power, ISP ... 등 phase 결과 표시.
                        PhaseBlinkStop(sectionTitleLabels[sectionIndex], true);
                        bool phasePassed = MainViewModel.CheckPhasePassed(boardPassed, sectionIndex);
                        Utils.InvokeIfRequired(this, () =>
                        {
                            sectionTitleLabels[sectionIndex].BackColor = phasePassed ? resultPassColor : resultFailColor;
                        });

                        if (!phasePassed && !shouldContinue)
                        {
                            break;
                        }
                    }

                    elapsedTimer.Stop();
                    elozDurationWatch.Stop();
                    Utils.InvokeIfRequired(this, () =>
                    {
                        testProgressBar.Style = ProgressBarStyle.Blocks;
                    });


                    // eloZ1 실행시간 표시.
                    double elozDuration = elozDurationWatch.ElapsedMilliseconds / 1000.0;
                    Utils.InvokeIfRequired(this, () =>
                    {
                        testDurationLabel.Text = $"{elozDuration:0.0}초";
                    });

                    // Testing 스테이지 종료.
                    StageBlinkStop(stageTestingLabel, true);

                    // PCB OUT 스테이지 시작.
                    StageBlinkStart(stagePcbOutLabel);

                    //
                    // 종료 처리 - 양품, 불량품, 중단 루틴 수행.
                    //
                    if (Eloz1FinishState == ResultState.Aborted && userClickedStop)
                    {
                        testDurationWatch.Stop();
                        SetCurrentTestStatus(TestStatus.FinishedAborted);
                        StopRoutine(true);

                        // PCB OUT 스테이지 완료.
                        StageBlinkStop(stagePcbOutLabel, true);

                        break;
                    }
                    else if (handlerErrorOccurred)
                    {
                        throw new Exception(handlerErrorMessage);
                    }
                    else
                    {
                        // 양품/불량품 루틴.
                        //bool testPassed = Eloz1FinishState == ResultState.Pass;
                        bool testPassed = MainViewModel.CheckTestPassed(boardPassed);

                        // 테스트 상태 설정.
                        SetCurrentTestStatus(testPassed ? TestStatus.FinishedPass : TestStatus.FinishedFail);

                        // 양품/불량품 수량 추가.
                        if (CurrentProbeCount != null)
                        {
                            CurrentProbeCount.IncreaseTodayTestCount(CurrentBarcode, testPassed, currentProduct.Project.Panel);
                            MainViewModel.SaveProbeCount(CurrentProbeCount);
                            Utils.InvokeIfRequired(this, () =>
                            {
                                DisplayProbeCount(CurrentProbeCount);
                            });
                        }

                        // 이력 저장.
                        List<string> logFiles = null;
                        try
                        {
                            long eolElapsedMilliseconds = 0;
                            if (ictElapsedMilliseconds < 0)
                            {
                                ictElapsedMilliseconds = elozDurationWatch.ElapsedMilliseconds;
                            }
                            else
                            {
                                eolElapsedMilliseconds = elozDurationWatch.ElapsedMilliseconds - ictElapsedMilliseconds;
                            }

                            logFiles = ViewModel.SaveTestLog(testPassed, boardPassed, CurrentBarcode, currentProduct,
                                testLogTextBox.Text, boardTestTimes, eolFirstSectionIndex);
                        }
                        catch (Exception ex)
                        {
                            // 오류가 발생하면 테스트를 종료한다.
                            throw new Exception($"테스트 이력 저장 실패: {ex.Message}");
                        }

                        // 결과에 따른 사운드 플레이.
                        ViewModel.PlayResultSound(testPassed);

                        // Power Off.
                        Power12Off();

                        // 테스트결과 프린트.
                        var printMode = AppSettings.PrintingMode;
                        if (printMode == PrintingOptions.All || printMode == PrintingOptions.FailOnly && !testPassed)
                        {
                            var printThread = new Thread(() => ViewModel.Print(CurrentBarcode, currentProduct.Project.Model, ictProjectName));
                            printThread.SetApartmentState(ApartmentState.STA);
                            printThread.Start();
                        }

                        // 실패한 경우 Notepad에 추가.
                        if (!testPassed && AppSettings.ShowFailInfoNotepad)
                        {
                            var notepadThread = new Thread(() => ViewModel.ShowFailInfoNotepad(this, CurrentBarcode, currentProduct.Project.Model, ictProjectName));
                            notepadThread.SetApartmentState(ApartmentState.STA);
                            notepadThread.Start();
                        }

                        // 테스트 결과 MES 전송.
                        if (!eolEnabled || sectionIndex < eolFirstSectionIndex)
                        {
                            // 테스트를 EOL 섹션 전까지 진행했으면 ICT 결과 전송.
                            ictPassed = MainViewModel.CheckIctPassed(boardPassed, eolFirstSectionIndex);
                            SendMesResult(true, CurrentBarcode, ictPassed);
                        }
                        else
                        {
                            bool eolPassed = MainViewModel.CheckEolPassed(boardPassed, eolFirstSectionIndex);
                            SendMesResult(false, CurrentBarcode, eolPassed);
                        }

                        // MES 모드 변경 라벨 Enable.
                        Utils.InvokeIfRequired(this, () => {
                            mesModeLabel.Enabled = true;
                        });

                        // PCB Viewer 보여주기.
                        if (!testPassed && AppSettings.ShowPcbViewer)
                        {
                            ViewModel.ShowPcbViewer(currentProduct.Project);
                        }

                        // PCB 배출.
                        waitingPcbOut = true;

                        var testFailAction = AppSettings.TestFailAction;
                        if (autoMode)
                        {
                            // 자동모드이면 PCB 배출 명령 PLC로 전송.
                            if (testPassed || testFailAction == TestFailAction.Continue)
                            {
                                // 직렬연결을 위해 Capacity를 설정.
                                SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested;

                                // 앞에서 테스트한 결과.
                                prevResult = pcbScanZone;

                                // MES NG의 경우 Test NG로 처리할 것인지 여부.
                                mesNgAsTestNg = pcbMesResult == PcbMesResult.NG && AppSettings.PrevFailAction != PrevProcFailAction.Stop;

                                // 스캔존 투입 여부.
                                var scanZoneIn = (pcbMesResult == PcbMesResult.NG && !mesNgAsTestNg) ? PcbScanZone.NoPcb : PcbScanZone.Optional;

                                // 테스트존 투입 여부.
                                var testZoneIn = mesNgAsTestNg ? PcbMesResult.OK : pcbMesResult;

                                pcbScanZone = ViewModel.PlcMovePcb(scanZoneIn, testZoneIn, testPassed ? PcbTestResult.Pass : PcbTestResult.Fail);

                                // 직렬연결을 위해 Capacity를 설정.
                                SeriesServer.SharedServer.Capable = pcbScanZone != PcbTestResult.NotTested && prevResult != PcbTestResult.NotTested;
                            }
                        }
                        else
                        {
                            // 수동모드이면 프레스, 컨베이어 UP.
                            if (!dioManualMode)
                            {
                                ViewModel.PlcCylinderUp();
                            }
                            else
                            {
                                ViewModel.DioRWCylinderUp();
                            }
                        }

                        waitingPcbOut = false;

                        // 테스트 시간 표시
                        testDurationWatch.Stop();
                        double testDuration = testDurationWatch.ElapsedMilliseconds / 1000.0;
                        Utils.InvokeIfRequired(this, () =>
                        {
                            testDurationLabel.Text += $" ({testDuration:0.0}초)";
                        });

                        // PCB OUT 스테이지 완료.
                        StageBlinkStop(stagePcbOutLabel, true);

                        // 테스트 이력 보관.
                        history.FinishTime = DateTime.Now;
                        history.ElozDuration = (int)elozDuration;
                        history.TotalDuration = (int)testDuration;
                        history.Result = testPassed;
                        history.ProjectName = Path.GetFileNameWithoutExtension(currentProduct.Project.Path);
                        history.IctProject = currentProduct.Project.IctProjectName + " (" + currentProduct.Project.IctVariantName + ")";
                        history.Model = currentProduct.Project.Model;
                        history.SerialNumber = CurrentBarcode;
                        history.PrintLogFile = logFiles[0];
                        history.Board1LogFile = logFiles.Count > 1 ? logFiles[1] : "";
                        history.Board2LogFile = logFiles.Count > 2 ? logFiles[2] : "";
                        history.Board3LogFile = logFiles.Count > 3 ? logFiles[3] : "";
                        history.Board4LogFile = logFiles.Count > 4 ? logFiles[4] : "";
                        history.FixtureId = fid;
                        MainViewModel.AddTestHistory(history, CurrentProbeCount);

                        // 테스트 불량 시.
                        if (autoMode && !testPassed)
                        {
                            if (testFailAction == TestFailAction.StopAndAlarm)
                            {
                                // 경광등 켜고 정지.
                                ViewModel.PlcSendError();
                                break;
                            }
                            else if (testFailAction == TestFailAction.Stop)
                            {
                                // 그냥 정지.
                                break;
                            }
                        }
                    }

                    isFirstRun = false;
                }
            }
            catch (Exception ex)
            {
                // 직렬연결을 위해 Capacity를 설정.
                SeriesServer.SharedServer.Capable = false;

                Logger.LogError(ex.Message);

                if (userClickedStop)    // 사용자에 의해 중지됨.
                {
                    StopRoutine(false);
                    SetCurrentTestStatus(TestStatus.FinishedAborted);
                }
                else
                {
                    SetCurrentTestStatus(TestStatus.AbortedError);

                    if (!handlerErrorOccurred && !(ex is PlcException) && !dioManualMode)
                    {
                        Task.Run(() =>
                        {
                            try
                            {
                                ViewModel.PlcOpen();
                                ViewModel.PlcSendError();
                            }
                            catch (Exception plcEx)
                            {
                                Logger.LogError(plcEx.Message);
                            }
                        });
                    }

                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show(ex.Message, "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    });
                }
            }
            finally
            {
                try
                {
                    // 저장한 바코드 제거.
                    Eloz1.SetGlobalStorageValue(MainViewModel.BarcodeVarName, null);

                    // 직렬연결을 위해 Capacity를 설정.
                    SeriesServer.SharedServer.Capable = false;

                    Utils.InvokeIfRequired(this, () =>
                    {
                        promptLabel.Text = "";
                        mesModeLabel.Enabled = true;

                        // Ctrl+D, 검사수량 초기화 버튼 Disable.
                        probeCountEditEnabled = true;
                        countInitButton.Enabled = true;
                    });

                    if (!dioManualMode)
                    {
                        ViewModel.PlcClose();
                    }
                    else
                    {
                        ViewModel.DioRWClose();
                    }

                    StageBlinkStop(null, false);
                    PhaseBlinkStop(null, false);
                    BoardBlinkStop(null, false);
                    PromptBlinkStop(null, false);

                    Utils.InvokeIfRequired(this, () =>
                    {
                        UpdateButtonStates(false);
                    });

                    elapsedTimer.Stop();
                    elapsedTimer.Dispose();
                    Utils.InvokeIfRequired(this, () =>
                    {
                        testProgressBar.Style = ProgressBarStyle.Blocks;
                    });

                    waitingPcbIn = false;
                    waitingPcbOut = false;
                    shouldMonitorFixturePower = true;

                    // POD 전원 끄기.
                    if (novaConnected)
                    {
                        try
                        {
                            ViewModel.NovaOpen();
                            ViewModel.NovaPodPower(false, true, true, true, true);
                        }
                        catch (Exception ex)
                        {
                            Logger.LogError($"Novaflash: {ex.Message}");
                        }
                    }

                    MainViewModel.SeriesDisconnect();
                    MainViewModel.SeriesServerStop();

                    MesServer.SharedIctServer.Stop();
                    MesServer.SharedEolServer.Stop();

                    // 마스터보드 여부 제거.
                    EOL_GND.ViewModel.SequenceViewModel.SetIsMasterBoard(null);

                    // NovaFlash 다시 테스트 여부 제거.
                    Eloz1.SetGlobalStorageValue(MainViewModel.IspRetestVarName, null);

                    // DIO 닫기.
                    ViewModel.DioClose();

                    // Hydra 디바이스에서 파일 제거.
                    //if (currentProduct != null && ispSectionEnabled)
                    //{
                    //    try
                    //    {
                    //        ViewModel.DeleteHydraFiles(currentProduct.Project.GetOrderedGrpInfos());
                    //    }
                    //    catch (Exception ex)
                    //    {
                    //        Logger.LogError("NovaFlash Error: " + ex.Message);
                    //    }
                    //}
                }
                catch (Exception ex)
                {
                    Logger.LogError("Error: " + ex.Message);
                }
            }
        }

        private void ScanAndCheckMes(bool isICT, out PcbMesResult pcbMesResult, out string pcbMesBarcode, out string errorMessage)
        {
            if (AppSettings.ShouldScanBarcode || AppSettings.MesEnabled)
            {
                Utils.InvokeIfRequired(this, () =>
                {
                    promptLabel.Text = "바코드 스캔 중...";
                });

                PromptBlinkStart(promptLabel);
                string barcode = ScanBarcode();
                PromptBlinkStop(promptLabel, true);

                Utils.InvokeIfRequired(this, () =>
                {
                    promptLabel.Text = "";
                });

                // 바코드를 못읽었으면 오류 발생.
                if (string.IsNullOrEmpty(barcode))
                {
                    pcbMesBarcode = barcode;
                    pcbMesResult = PcbMesResult.NG;
                    errorMessage = "바코드 읽기에 실패하였습니다.";
                }
                else
                {
                    pcbMesBarcode = barcode;

                    // 바코드 차종코드 검사.
                    if (AppSettings.CarTypeCodeLength > 0)
                    {
                        int startIndex = Math.Max(0, AppSettings.CarTypeCodeStartPosition - 1);
                        var carTypeCode = barcode.Substring(startIndex, AppSettings.CarTypeCodeLength);
                        if (!MainViewModel.ContainsCarTypeCode(currentProduct.CarTypeCode, carTypeCode))
                        {
                            errorMessage = $"프로젝트({currentProduct.CarTypeCode})와 PCB({carTypeCode})의 차종코드가 일치하지 않습니다.";
                            pcbMesResult = PcbMesResult.NG;
                            return;
                        }
                    }

                    // 바코드 검사 및 MES 통신.
                    bool success = CheckMes(isICT, barcode, out errorMessage);
                    pcbMesResult = success ? PcbMesResult.OK : PcbMesResult.NG;
                }
            }
            else
            {
                pcbMesBarcode = "";

                // 바코드를 읽지 않거나 MES를 사용하지 않는 경우 @MES OK를 핸들러에 보낸다.
                pcbMesResult = PcbMesResult.OK;
                errorMessage = "";
            }
        }

        private bool CheckMes(bool isICT, string barcode, out string errorMessage)
        {
            var ictEolText = isICT ? "ICT" : "EOL";

            // 중단 검사.
            if (userClickedStop)
            {
                throw new Exception(UserAbortedMessage);
            }

            // 핸들러 이상상태 체크.
            if (handlerErrorOccurred)
            {
                throw new Exception(handlerErrorMessage);
            }

            // MES M1 메시지 전송.
            if (AppSettings.MesEnabled)
            {
                // MES M2 상태 초기화.
                Utils.InvokeIfRequired(this, () =>
                {
                    SetMesStatus(isICT, true, null);
                });

                int mesRetryCount = AppSettings.MesM1RetryCount;
                int mesRetryInterval = AppSettings.MesM1RetryInterval;
                MesResponseMessage response = null;
                do
                {
                    try
                    {
                        if (isICT)
                        {
                            MainViewModel.MesIctSendM1Message(barcode, out response);
                        }
                        else
                        {
                            MainViewModel.MesEolSendM1Message(barcode, out response);
                        }

                        if (response.MesResult != MesMessage.ResultSuccess)
                        {
                            Logger.LogTimedMessage($"MES {ictEolText} M2 Result={response.MesResult}, Message: {response.MesMessageCode} => {response.MesMessageContent}");
                        }

                        if (response.MesResult == MesMessage.ResultSuccess || mesRetryCount <= 0)
                        {
                            break;
                        }

                        Logger.LogTimedMessage($"MES {ictEolText} M1 재시도 남은 횟수: {mesRetryCount}");
                        mesRetryCount--;
                        if (mesRetryInterval > 0)
                        {
                            Thread.Sleep(mesRetryInterval);
                        }

                        // 중단 검사.
                        if (userClickedStop)
                        {
                            throw new Exception(UserAbortedMessage);
                        }

                        // 핸들러 이상상태 체크.
                        if (handlerErrorOccurred)
                        {
                            throw new Exception(handlerErrorMessage);
                        }
                    }
                    catch (Exception e)
                    {
                        errorMessage = $"MES {ictEolText} 이전공정 조회 오류: {e.Message}";
                        Logger.LogTimedMessage(errorMessage);

                        Logger.LogTimedMessage($"MES {ictEolText} M1 재시도 남은 횟수: {mesRetryCount}");
                        mesRetryCount--;

                        if (mesRetryCount < 0)
                        {
                            return false;
                        }

                        // 서버를 재부팅한다.
                        if (isICT)
                        {
                            MesServer.SharedIctServer.Stop();
                            MesServer.SharedIctServer.StartServer(AppSettings.MesIctServerPort);
                        }
                        else
                        {
                            MesServer.SharedEolServer.Stop();
                            MesServer.SharedEolServer.StartServer(AppSettings.MesEolServerPort);
                        }

                        // 클라이언트가 연결될 때까지 기다림.
                        if (mesRetryInterval > 0)
                        {
                            Logger.LogTimedMessage($"MES {ictEolText} 클라이언트 연결 대기...");
                            Thread.Sleep(mesRetryInterval);
                        }

                        // 중단 검사.
                        if (userClickedStop)
                        {
                            throw new Exception(UserAbortedMessage);
                        }

                        // 핸들러 이상상태 체크.
                        if (handlerErrorOccurred)
                        {
                            throw new Exception(handlerErrorMessage);
                        }
                    }
                } while (mesRetryCount >= 0);

                // M1 결과 표시.
                bool resultOk = response.MesResult == MesMessage.ResultSuccess;

                // MES 응답의 FGCODE와 제품 설정의 FGCODE 비교.
                bool fgcodeNg = AppSettings.FgcodeCheckMethod != FgcodeCheckMode.None && !response.Prod.Equals(currentProduct.FGCode, StringComparison.OrdinalIgnoreCase);

                bool m2Ok = resultOk && !fgcodeNg;
                Utils.InvokeIfRequired(this, () =>
                {
                    SetMesStatus(isICT, true, m2Ok);
                });

                // 사운드 플레이.
                ViewModel.PlayMesSound(true, resultOk);

                if (fgcodeNg)
                {
                    errorMessage = $"MES {ictEolText} M2 오류: M2 품번({response.Prod})이 프로젝트 FGCODE({currentProduct.FGCode})와 일치하지 않습니다.";
                    Logger.LogError(errorMessage);
                    return false;
                }

                // 최종적으로 실패한 경우.
                if (!resultOk)
                {
                    errorMessage = $"MES {ictEolText} 이전공정 조회 오류: {response.MesMessageCode} => {response.MesMessageContent}";
                    Logger.LogError(errorMessage);

                    return false;
                }
                else
                {
                    // MES 결과 저장.
                    errorMessage = "";

                    // MES CRC 체크.

                    //// 실행할 섹션 리스트.
                    //var sectionInfos = currentProduct.Project.GetOrderedSectionInfos(out int ispSectionIndex, out int eolFirstSectionIndex, out int ictFirstSectionIndex);

                    //// 실행할 섹션 중에 isp 문자열을 포함하는 섹션 이름이 있으면 isp 실행으로 간주.
                    //var ispSectionEnabled = sectionInfos.Where(info => info.Enabled && info.IsIspSection).Any();

                    // 항상 CRC 체크.
                    bool ispSectionEnabled = true;

                    if (ispSectionEnabled)
                    {
                        // GRP 리스트에 대해 MES CRC 검사.
                        var grpList = currentProduct.Project.GetOrderedGrpInfos();
                        foreach (var grpInfo  in grpList)
                        {
                            if (!string.IsNullOrWhiteSpace(grpInfo.GrpFilePath))
                            {
                                if (isICT && grpInfo.CrcCheck == CrcCheckMethod.MES_ICT || !isICT && grpInfo.CrcCheck == CrcCheckMethod.MES_EOL)
                                {
                                    Logger.LogInfo($"MES CRC = {response.UserDefined4}, Local CRC = {grpInfo.Crc:X}");

                                    // MES CRC와 Local CRC 비교.
                                    var mesCrc = response.GetCrc32();
                                    bool crcOk = mesCrc == grpInfo.Crc;

                                    Logger.LogInfo($"CRC {(crcOk ? "OK" : "NG")}");
                                    
                                    if (!crcOk)
                                    {
                                        errorMessage = $"CRC 매칭 오류: MES CRC = {response.UserDefined4}, Local CRC = {grpInfo.Crc:X}";
                                        return false;
                                    }
                                }
                            }
                        }
                    }

                    return true;
                }
            }
            else
            {
                Utils.InvokeIfRequired(this, () =>
                {
                    mesModeLabel.Enabled = false;
                });

                // MES를 사용하지 않는 경우 @MES OK를 핸들러에 보낸다.
                errorMessage = "";
                return true;
            }
        }

        private void SendMesResult(bool isICT, string barcode, bool testPassed)
        {
            if (!AppSettings.MesEnabled)
            {
                return;
            }

            string ictEolText = isICT ? "ICT" : "EOL";

            bool sendResult;
            if (AppSettings.MesReportAction == MesResultAction.MES)
            {
                sendResult = true;
            }
            else if (AppSettings.MesReportAction == MesResultAction.Stop)
            {
                sendResult = false;
            }
            else
            {
                // 사용자에게 물어보기.
                sendResult = InformationBox.Show("시험 결과를 MES로 전송 하시겠습니까?", "MES 전송",
                    buttons: InformationBoxButtons.YesNo, icon: InformationBoxIcon.Question, order: InformationBoxOrder.TopMost)
                    == InformationBoxResult.Yes;
            }

            if (!sendResult)
            {
                return;
            }

            // MES M3 요청 전송.
            int mesRetryCount = AppSettings.MesM3RetryCount;
            int mesRetryInterval = AppSettings.MesM3RetryInterval;
            MesResponseMessage response = null;
            MesMessage.StatusCode status = testPassed ? MesMessage.StatusCode.OK : MesMessage.StatusCode.NG;
            do
            {
                try
                {
                    if (isICT)
                    {
                        MainViewModel.MesIctSendM3Message(barcode, testPassed, out response);
                    }
                    else
                    {
                        MainViewModel.MesEolSendM3Message(barcode, testPassed, out response);
                    }

                    if (response.MesResult != (int)status)
                    {
                        // MES error.
                        Logger.LogTimedMessage($"MES {ictEolText} M4 Result={response.MesResult}, Message: {response.MesMessageCode} => {response.MesMessageContent}");
                    }

                    break;
                }
                catch (Exception e)
                {
                    var errorMessage = $"MES {ictEolText} 검사결과 전송 오류: {e.Message}";
                    Logger.LogTimedMessage(errorMessage);

                    Logger.LogTimedMessage($"MES {ictEolText} M3 재시도 남은 횟수: {mesRetryCount}");
                    mesRetryCount--;

                    if (mesRetryCount < 0)
                    {
                        throw new Exception(errorMessage);
                    }

                    // 서버를 재부팅한다.
                    if (isICT)
                    {
                        MesServer.SharedIctServer.Stop();
                        MesServer.SharedIctServer.StartServer(AppSettings.MesIctServerPort);
                    }
                    else
                    {
                        MesServer.SharedEolServer.Stop();
                        MesServer.SharedEolServer.StartServer(AppSettings.MesEolServerPort);
                    }

                    // 클라이언트가 연결될 때까지 기다림.
                    if (mesRetryInterval > 0)
                    {
                        Logger.LogTimedMessage($"MES {ictEolText} 클라이언트 연결 대기...");
                        Thread.Sleep(mesRetryInterval);
                    }
                }
            } while (mesRetryCount >= 0);

            // 최종적으로 실패한 경우.
            bool m4Ok = response.MesResult == (int)status;

            // 결과 표시.
            Utils.InvokeIfRequired(this, () =>
            {
                SetMesStatus(isICT, false, m4Ok);
            });

            // 사운드 플레이.
            ViewModel.PlayMesSound(false, m4Ok);

            if (!m4Ok)
            {
                string message = $"MES {ictEolText} 검사결과 전송 오류: {response.MesMessageCode} => {response.MesMessageContent}";
                //Logger.LogError(message);
                throw new Exception(message);
            }
        }

        private void DisplayBarcode(string barcode)
        {
            Utils.InvokeIfRequired(barcodeLabel, () =>
            {
                if (string.IsNullOrEmpty(barcode))
                {
                    barcodeLabel.Text = $"{currentProduct.Project.Model}_{DateTime.Now:yyyyMMddHHmmss}";
                    mesModeLabel.Enabled = false;
                    board1BarcodeLabel.Text = "";
                    board2BarcodeLabel.Text = "";
                    board3BarcodeLabel.Text = "";
                    board4BarcodeLabel.Text = "";
                }
                else
                {
                    barcodeLabel.Text = barcode;
                    board2BarcodeLabel.Text = MainViewModel.IncreaseBarcode(barcode, 1);
                    if (currentProduct.Project.Panel == 2 && !currentProduct.Project.TwoBoardsLeftRight)
                    {
                        // 2연배열 상/하 배치.
                        if (currentProduct.Project.BottomBoardFirst)
                        {
                            // 밑의 보드가 첫번째.
                            board3BarcodeLabel.Text = barcode;
                            board1BarcodeLabel.Text = MainViewModel.IncreaseBarcode(barcode, 1);
                        }
                        else
                        {
                            // 밑의 보드가 두번째.
                            board1BarcodeLabel.Text = barcode;
                            board3BarcodeLabel.Text = MainViewModel.IncreaseBarcode(barcode, 1);
                        }
                    }
                    else
                    {
                        board1BarcodeLabel.Text = barcode;
                        board3BarcodeLabel.Text = MainViewModel.IncreaseBarcode(barcode, 2);
                    }
                    board4BarcodeLabel.Text = MainViewModel.IncreaseBarcode(barcode, 3);
                }
            });
        }

        private string ScanBarcode()
        {
            string barcode = "";
            int retryCount = AppSettings.BarcodeRetryCount;
            int retryInterval = AppSettings.BarcodeRetryInterval;
            int readCount = 0;
            do
            {
                try
                {
                    barcode = ViewModel.ScannerReadBarcode();
                    if (!string.IsNullOrEmpty(barcode))
                    {
                        break;
                    }

                    // 홀수/짝수 여부 체크.
                    if (currentProduct.Project.Panel > 1)
                    {
                        if (currentProduct.Project.MultiPanelBarcodeType == MultiPanelBarcodeType.Odd)
                        {
                            int serialNr = MainViewModel.GetBarcodeSerialNumber(barcode);
                            if (serialNr % 2 != 1)
                            {
                                throw new Exception($"읽은 대표 바코드({barcode})는 홀수이어야 합니다.");
                            }
                        }
                        else if (currentProduct.Project.MultiPanelBarcodeType == MultiPanelBarcodeType.Even)
                        {
                            int serialNr = MainViewModel.GetBarcodeSerialNumber(barcode);
                            if (serialNr % 2 != 0)
                            {
                                throw new Exception($"읽은 대표 바코드({barcode})는 짝수이어야 합니다.");
                            }
                        }
                    }
                }
                catch (TimeoutException te)
                {
                    Logger.LogTimedMessage("바코드 읽기 타임아웃: " + te.Message);

                    readCount++;
                    if (retryInterval > 0 && readCount <= retryCount)
                    {
                        Thread.Sleep(retryInterval);
                    }

                    // 중단 검사.
                    if (userClickedStop)
                    {
                        throw new Exception(UserAbortedMessage);
                    }

                    // 핸들러 이상상태 체크.
                    if (handlerErrorOccurred)
                    {
                        throw new Exception(handlerErrorMessage);
                    }
                }
            } while (readCount <= retryCount);

            return barcode;
        }

        private void SetCurrentProduct(Product product)
        {
            currentProduct = product;

            Utils.InvokeIfRequired(this, () =>
            {
                if (product?.Project == null)
                {
                    InitProductUI();
                }
                else
                {
                    DisplayProduct(product);
                }
            });

            // Power 설정을 진행한다.
            if (product?.Project?.Power1Enabled ?? false)
            {
                ViewModel.Power1SetVA(product.Project.Power1Voltage, product.Project.Power1Current);
            }
            if (product?.Project?.Power2Enabled ?? false)
            {
                ViewModel.Power2SetVA(product.Project.Power2Voltage, product.Project.Power2Current);
            }
        }

        private void DisplayProduct(Product product)
        {
            // FGcode.
            fgcodeLabel.Text = product.FGCode;

            // 프로젝트를 수동으로 로딩하면, 바코드를 가상 바코드로 표시.
            barcodeLabel.Text = $"{product.Project.Model}_{DateTime.Now:yyyyMMddHHmmss}";
            CurrentBarcode = "";

            // 모델.
            modelLabel.Text = product.Project.Model;

            // Project 이름.
            var ictProjName = product.Project.IctProjectName;
            if (!string.IsNullOrEmpty(product.Project.IctVariantName))
            {
                ictProjName += $" ({product.Project.IctVariantName})";
            }
            projectLabel.Text = ictProjName;
            projectNameLabel.Text = Path.GetFileNameWithoutExtension(product.Project.Path);

            // Section 타이틀.
            var sectionTitleLabels = new Label[]
            {
                ictTitleLabel, ictPowerTitleLabel, ispTitleLabel, jtagTitleLabel, funcTitleLabel, eolTitleLabel, ext1TitleLabel, ext2TitleLabel
            };

            // Section 이름.
            var sectionNameLabels = new Label[]
            {
                ictLabel, ictPowerLabel, ispLabel, jtagLabel, funcLabel, eolLabel, ext1Label, ext2Label
            };

            // GRP 파일 리스트.
            var grpPictureBoxes = new PictureBox[]
            {
                isp1PictureBox, isp2PictureBox, isp3PictureBox, isp4PictureBox,
            };
            var grpNameLabels = new Label[]
            {
                isp1Label, isp2Label, isp3Label, isp4Label,
            };
            var grpCrcLabels = new Label[]
            {
                isp1CrcLabel, isp2CrcLabel, isp3CrcLabel, isp4CrcLabel,
            };

            // ISP 섹션 밑으로 GRP 관련 UI들을 이동한다.
            sectionInfoTLPanel.SuspendLayout();

            for (int i = 0; i < sectionTitleLabels.Length; i++)
            {
                sectionInfoTLPanel.Controls.Remove(sectionTitleLabels[i]);
                sectionInfoTLPanel.Controls.Remove(sectionNameLabels[i]);
            }

            for (int i = 0; i < grpPictureBoxes.Length; i++)
            {
                sectionInfoTLPanel.Controls.Remove(grpPictureBoxes[i]);
                sectionInfoTLPanel.Controls.Remove(grpNameLabels[i]);
                sectionInfoTLPanel.Controls.Remove(grpCrcLabels[i]);
            }

            // 차례로 추가.
            var sectionInfos = product.Project.GetOrderedSectionInfos(out int ispSectionIndex, out _, out _);
            int row = 0;
            for (int i = 0; i < sectionTitleLabels.Length; i++)
            {
                sectionInfoTLPanel.Controls.Add(sectionTitleLabels[i], 0, row);
                sectionInfoTLPanel.Controls.Add(sectionNameLabels[i], 1, row);
                sectionInfoTLPanel.SetColumnSpan(sectionNameLabels[i], 2);
                if (i == ispSectionIndex)
                {
                    // GRP 관련 Controls 추가.
                    for (int grpIndex = 0; grpIndex < grpPictureBoxes.Length; grpIndex++)
                    {
                        row++;
                        sectionInfoTLPanel.Controls.Add(grpPictureBoxes[grpIndex], 0, row);
                        sectionInfoTLPanel.Controls.Add(grpNameLabels[grpIndex], 1, row);
                        sectionInfoTLPanel.Controls.Add(grpCrcLabels[grpIndex], 2, row);
                    }
                }
                row++;
            }

            sectionInfoTLPanel.ResumeLayout();

            // 섹션 정보 표시.
            for (int i = 0; i < sectionInfos.Count; i++)
            {
                var info = sectionInfos[i];
                sectionTitleLabels[i].BackColor = sectionTitleBackColor;
                sectionTitleLabels[i].Text = info.SectionTitle;
                sectionNameLabels[i].Text = info.SectionName;

                if (info.Enabled)
                {
                    sectionNameLabels[i].Font = new Font(sectionNameLabels[i].Font, FontStyle.Regular);
                }
                else
                {
                    sectionNameLabels[i].Font = new Font(sectionNameLabels[i].Font, FontStyle.Strikeout);
                }
            }

            // GRP 정보 표시.
            var grpList = product.Project.GetOrderedGrpInfos();
            for (int channelIndex = 0; channelIndex < grpNameLabels.Length; channelIndex++)
            {
                // 현 채널에서 실행할 GRP 리스트 찾기.
                var grpInfos = grpList?.Where(g => g.Channel == channelIndex + 1)?.ToList();
                if (grpInfos == null || grpInfos.Count == 0)
                {
                    continue;
                }

                // 현 채널 GRP 파일 정보 표시.
                string grpNames = "", dataCrcs = "";
                bool dataCrcOk = true;
                for (int grpIndex = 0; grpIndex < grpInfos.Count; grpIndex++)
                {
                    var grpInfo = grpInfos[grpIndex];
                    if (!string.IsNullOrWhiteSpace(grpInfo.GrpFilePath))
                    {
                        var fileName = grpInfo.RomFileName;
                        if (grpNames == "")
                        {
                            grpNames = fileName;
                        }
                        else
                        {
                            grpNames += " | " + fileName;
                        }

                        var dataCrc = $"{grpInfo.Crc:X}";
                        if (dataCrcs == "")
                        {
                            dataCrcs = dataCrc;
                        }
                        else
                        {
                            dataCrcs += " | " + dataCrc;
                        }

                        // CRC 체크.
                        string createdGrpPath = null;
                        try
                        {
                            createdGrpPath = ViewModel.CreateGRP(grpInfo.GrpFilePath, grpInfo.ImportFilePath);
                            var uploadGrpPath = createdGrpPath ?? grpInfo.GrpFilePath;
                            var actualCrc = Novaflash.GetDataCrc32(uploadGrpPath);
                            if (grpInfo.Crc != actualCrc)
                            {
                                dataCrcOk = false;
                                Logger.LogTimedMessage($"DATA CRC Error: File={fileName}, CRC={grpInfo.Crc:X}, Calculated={actualCrc:X}");
                            }
                        }
                        finally
                        {
                            // 임시로 생성한 GRP 파일 제거.
                            if (!string.IsNullOrWhiteSpace(createdGrpPath))
                            {
                                File.Delete(createdGrpPath);
                            }
                        }
                    }
                }

                grpNameLabels[channelIndex].Text = grpNames;
                grpCrcLabels[channelIndex].Text = dataCrcs;
                if (grpNames == "")
                {
                    grpCrcLabels[channelIndex].BackColor = Color.LightGray;
                    grpPictureBoxes[channelIndex].Visible = false;
                }
                else
                {
                    grpCrcLabels[channelIndex].BackColor = dataCrcOk ? Color.LightGreen : resultFailColor;
                    grpPictureBoxes[channelIndex].Visible = true;
                }
            }

            testDurationLabel.Text = "";
            testProgressBar.Style = ProgressBarStyle.Blocks;

            SetMesStatus(true, true, null);
            SetMesStatus(true, false, null);

            // Multi Panel 관련 설정.
            if (product.Project.Board1Checked)
            {
                board1TLPanel.BackColor = boardBackColor;
                board1TitleLabel.Font = boardTitleFont;
            }
            else
            {
                board1TLPanel.BackColor = boardDisabledColor;
                board1TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
            }
            board1BarcodeLabel.Text = "";

            if (product.Project.Board2Checked)
            {
                board2TLPanel.BackColor = boardBackColor;
                board2TitleLabel.Font = boardTitleFont;
            }
            else
            {
                board2TLPanel.BackColor = boardDisabledColor;
                board2TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
            }
            board2BarcodeLabel.Text = "";

            if (product.Project.Board3Checked)
            {
                board3TLPanel.BackColor = boardBackColor;
                board3TitleLabel.Font = boardTitleFont;
            }
            else
            {
                board3TLPanel.BackColor = boardDisabledColor;
                board3TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
            }
            board3BarcodeLabel.Text = "";

            if (product.Project.Board1Checked)
            {
                board4TLPanel.BackColor = boardBackColor;
                board4TitleLabel.Font = boardTitleFont;
            }
            else
            {
                board4TLPanel.BackColor = boardDisabledColor;
                board4TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
            }
            board4BarcodeLabel.Text = "";

            if (product.Project.Panel <= 1)
            {
                boardTLPanel.RowStyles[0].Height = 100;
                boardTLPanel.RowStyles[1].Height = 0;
                boardTLPanel.ColumnStyles[0].Width = 100;
                boardTLPanel.ColumnStyles[1].Width = 0;
            }
            else if (product.Project.Panel == 2)
            {
                if (product.Project.TwoBoardsLeftRight)
                {
                    boardTLPanel.RowStyles[0].Height = 100;
                    boardTLPanel.RowStyles[1].Height = 0;
                    boardTLPanel.ColumnStyles[0].Width = 50;
                    boardTLPanel.ColumnStyles[1].Width = 50;

                    board1TitleLabel.Text = "Board1";
                    board3TitleLabel.Text = "Board3";
                }
                else
                {
                    // 2연배열 상/하 배치.
                    boardTLPanel.RowStyles[0].Height = 50;
                    boardTLPanel.RowStyles[1].Height = 50;
                    boardTLPanel.ColumnStyles[0].Width = 100;
                    boardTLPanel.ColumnStyles[1].Width = 0;

                    if (currentProduct.Project.BottomBoardFirst)
                    {
                        // 밑의 보드가 첫번째.
                        board1TitleLabel.Text = "Board2";
                        board3TitleLabel.Text = "Board1";

                        if (product.Project.Board2Checked)
                        {
                            board1TLPanel.BackColor = boardBackColor;
                            board1TitleLabel.Font = boardTitleFont;
                        }
                        else
                        {
                            board1TLPanel.BackColor = boardDisabledColor;
                            board1TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
                        }

                        if (product.Project.Board1Checked)
                        {
                            board3TLPanel.BackColor = boardBackColor;
                            board3TitleLabel.Font = boardTitleFont;
                        }
                        else
                        {
                            board3TLPanel.BackColor = boardDisabledColor;
                            board3TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
                        }
                    }
                    else
                    {
                        // 밑의 보드가 두번째.
                        board1TitleLabel.Text = "Board1";
                        board3TitleLabel.Text = "Board2";

                        if (product.Project.Board1Checked)
                        {
                            board1TLPanel.BackColor = boardBackColor;
                            board1TitleLabel.Font = boardTitleFont;
                        }
                        else
                        {
                            board1TLPanel.BackColor = boardDisabledColor;
                            board1TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
                        }

                        if (product.Project.Board2Checked)
                        {
                            board3TLPanel.BackColor = boardBackColor;
                            board3TitleLabel.Font = boardTitleFont;
                        }
                        else
                        {
                            board3TLPanel.BackColor = boardDisabledColor;
                            board3TitleLabel.Font = new Font(boardTitleFont, FontStyle.Strikeout);
                        }
                    }
                }
            }
            else if (product.Project.Panel <= 4)
            {
                boardTLPanel.RowStyles[0].Height = 50;
                boardTLPanel.RowStyles[1].Height = 50;
                boardTLPanel.ColumnStyles[0].Width = 50;
                boardTLPanel.ColumnStyles[1].Width = 50;
            }
        }

        private void DisplayProbeCount(FixtureProbeCount probeCount)
        {
            Utils.InvokeIfRequired(this, () =>
            {
                if (probeCount == null)
                {
                    fixtureIdLabel.Text = "";

                    probeCountLabel.Text = "";
                    probeStatusLabel.Text = "";
                    probeStatusLabel.BackColor = probeOkColor;

                    todayTestedLabel.Text = "";
                    todayPassLabel.Text = "";
                    todayFailLabel.Text = "";

                    todayYieldLabel.Text = "";
                    pieChart.Percent = 0;

                    return;
                }

                // FID 표시.
                fixtureIdLabel.Text = $"{probeCount.FixtureId}";

                probeCountLabel.Text = $"{probeCount.TotalProbeCount} / {probeCount.MaxProbeCount}";
                if (probeCount.TotalProbeCount >= probeCount.MaxProbeCount)
                {
                    probeStatusLabel.Text = ProbeErrText;
                    probeStatusLabel.BackColor = probeErrColor;
                }
                else if (probeCount.TotalProbeCount >= probeCount.MaxProbeCount * ProbeWarnPercent)
                {
                    probeStatusLabel.Text = ProbeWarnText;
                    probeStatusLabel.BackColor = probeWarnColor;
                }
                else
                {
                    probeStatusLabel.Text = ProbeOkText;
                    probeStatusLabel.BackColor = probeOkColor;
                }

                // Statistics.
                todayTestedLabel.Text = $"{probeCount.TodayTestCount}";
                todayPassLabel.Text = $"{probeCount.TodayPassCount}";
                todayFailLabel.Text = $"{probeCount.TodayFailCount}";
                float todayPassPercent = FixtureProbeCount.CalcYieldPercent(probeCount.TodayTestCount, probeCount.TodayPassCount);
                todayYieldLabel.Text = $"{todayPassPercent:0.0} %";
                pieChart.Percent = todayPassPercent;
            });
        }

        private void UpdateFidStatus(bool ok)
        {
            if (ok)
            {
                fixtureStatusLabel.Text = ProbeOkText;
                fixtureStatusLabel.BackColor = probeOkColor;
            }
            else
            {
                fixtureStatusLabel.Text = ProbeErrText;
                fixtureStatusLabel.BackColor = probeErrColor;
            }
        }

        private void stopButton_Click(object sender, EventArgs e)
        {
            StopTest();
        }

        private void StopTest()
        {
            userClickedStop = true;
            if (barcodeReading)
            {
                fidScanner.Close();
            }
            if (ViewModel.IsEloz1Running)
            {
                ViewModel.Eloz1Stop();
            }
            if (waitingPcbIn || waitingPcbOut)
            {
                ViewModel.PlcClose();
            }
        }

        private void exitButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (!exitButton.Enabled)
            {
                e.Cancel = true;
                return;
            }

            if (currentTestStatus == TestStatus.Running)
            {
                var dlgResult = InformationBox.Show("테스트가 실행 중입니다." + Environment.NewLine + "실행을 중지하고 끝내겠습니까?",
                    "종료 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
                if (dlgResult == InformationBoxResult.OK)
                {
                    StopTest();
                }
                else
                {
                    e.Cancel = true;
                    return;
                }
            }
            else
            {
                if (!Visible)
                {
                    // Logout 한 상태, 물어보지 않고 닫는다.
                }
                else
                {
                    // 한번 더 물어본다.
                    var dlgResult = InformationBox.Show("프로그램을 종료하겠습니까?", "종료 확인", buttons: InformationBoxButtons.YesNo,
                        icon: InformationBoxIcon.Question);
                    if (dlgResult == InformationBoxResult.No)
                    {
                        e.Cancel = true;
                        return;
                    }
                }
            }

            statusReadTimer?.Stop();
            statusReadTimer?.Dispose();
            statusReadTimer = null;

            try
            {
                ViewModel?.Close();
            }
            catch { }
            ViewModel = null;

            Thread.Sleep(100);

            base.OnFormClosing(e);
        }

        private async void projectOpenButton_Click(object sender, EventArgs e)
        {
            // FID를 PLC에 보내야 한다면 자동모드가 아니어야 한다.
            bool checkFid = AppSettings.ShouldCheckFid;

            var dialog = new ProductOpenForm();
            if (dialog.ShowDialog() == DialogResult.OK && !string.IsNullOrEmpty(dialog?.Product?.ProjectPath))
            {
                try
                {
                    var product = dialog.Product;
                    TestProject project = TestProjectManager.LoadFile(dialog?.Product?.ProjectPath);
                    if (project == null)
                    {
                        return;
                    }

                    // GRP CRC32 매칭 상태 업데이트.
                    //project.CheckGrpCrc();

                    // Data CRC32 추출.
                    //project.CheckDataCrc();

                    // 하단, 상단 FID를 읽는다.
                    if (checkFid)
                    {
                        using (var dlg = new FidScanForm())
                        {
                            if (dlg.ShowDialog() == DialogResult.OK)
                            {
                                // 현 프로젝트가 해당 FID를 포함하는지 검사.
                                if (!dioManualMode && !project.FIDs.Contains(dlg.Fid))
                                {
                                    InformationBox.Show($"프로젝트가 FID({dlg.Fid})를 포함하지 않습니다.", "FID 오류", buttons: InformationBoxButtons.OK,
                                        icon: InformationBoxIcon.Error);
                                    return;
                                }

                                SetButtonEnabled(startButton, startButtonColor, false);
                                resetButton.Enabled = false;
                                logoutButton.Enabled = false;
                                projectOpenButton.Enabled = false;

                                // 컨베이어 조절에 시간이 오래 걸리므로 다른 스레드로 실행.
                                promptLabel.Text = "컨베이어를 조정하는 중...";
                                await Task.Run(() =>
                                {
                                    try
                                    {
                                        if (!dioManualMode)
                                        {
                                            ViewModel.PlcOpen();
                                            ViewModel.PlcSendFid(dlg.Fid);
                                        }
                                        AppSettings.CurrentFid = dlg.Fid;
                                        product.Project = project;
                                        SetCurrentProduct(product);
                                        UpdateProbeCount();
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogError(ex.Message);
                                    }
                                });

                                SetButtonEnabled(startButton, startButtonColor, true);
                                resetButton.Enabled = true;
                                logoutButton.Enabled = true;
                                projectOpenButton.Enabled = true;
                                promptLabel.Text = "";
                            }
                        }
                    }
                    else
                    {
                        product.Project = project;
                        SetCurrentProduct(product);
                        UpdateProbeCount();
                    }
                }
                catch (Exception ex)
                {
                    Logger.LogError(ex.Message);
                    InformationBox.Show(ex.Message, "프로젝트 로딩 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                }
            }
        }

        private void fixtureChangeButton_Click(object sender, EventArgs e)
        {
        }

        private void resetButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 장치 연결상태 체크.
                CheckDeviceConnections();

                // PLC 디바이스 초기화.
                if (!dioManualMode)
                {
                    ViewModel.PlcOpen();
                    ViewModel.PlcCylinderInit();
                }
                else
                {
                    ViewModel.DioRWOpen();
                    ViewModel.DioRWCylinderInit();
                }

                // 전원 차단.
                Power12Off();
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                InformationBox.Show(ex.Message, "초기화 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
            finally
            {
                if (!dioManualMode)
                {
                    ViewModel.PlcClose();
                }
                else
                {
                    ViewModel.DioRWClose();
                }
            }
        }

        private void logoutButton_Click(object sender, EventArgs e)
        {
            // 한번 더 물어본다.
            var result = InformationBox.Show("로그아웃 하겠습니까?", "로그아웃 확인", buttons: InformationBoxButtons.YesNo, icon: InformationBoxIcon.Question);
            if (result == InformationBoxResult.No)
            {
                return;
            }

            // 현재 유저를 지우고 로그인 대화상자를 보여준다.
            AppSettings.Logout();

            Hide();
            var loginDlg = new LoginForm();
            var dlgResult = loginDlg.ShowDialog();
            if (dlgResult == DialogResult.OK)
            {
                Show();
            }
            else
            {
                Close();
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            if (keyData == (Keys.Control | Keys.D) && probeCountEditEnabled)
            {
                // Probe Count 편집.
                EditProbeCount();

                return true;
            }
            else if (keyData == (Keys.Control | Keys.H))
            {
                var started = statusReadTimer?.Enabled;
                try
                {
                    statusReadTimer?.Stop();

                    // Test History 보기.
                    var dialog = new TestHistoryForm();
                    dialog.ShowDialog();
                }
                finally
                {
                    if (started == true)
                    {
                        statusReadTimer?.Start();
                    }
                }

                return true;
            }
            else if (keyData == (Keys.Control | Keys.M))
            {
                var started = statusReadTimer?.Enabled;
                try
                {
                    statusReadTimer?.Stop();
                    var dlg = new MechanicsControlForm();
                    dlg.ViewModel = ViewModel;
                    dlg.ShowDialog();
                }
                finally
                {
                    if (started == true)
                    {
                        statusReadTimer?.Start();
                    }
                }

                return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void EditProbeCount()
        {
            try
            {
                statusReadTimer?.Stop();

                var editForm = new ProbeCountEditForm();
                editForm.Fid = CurrentProbeCount?.FixtureId ?? 1;
                if (editForm.ShowDialog() == DialogResult.OK && CurrentProbeCount != null)
                {
                    CurrentProbeCount = MainViewModel.GetProbeCount(CurrentProbeCount.FixtureId);
                    DisplayProbeCount(CurrentProbeCount);
                }
            }
            catch (Exception e)
            {
                Logger.LogTimedMessage($"Probe Count 편집 오류: {e.Message}");
            }
            finally
            {
                statusReadTimer?.Start();
            }
        }

        // 현재 날짜를 표시하는 타이머.
        private void dateTimer_Tick(object sender, EventArgs e)
        {
            dateLabel.Text = DateTime.Now.ToString("yyyy년 MM월 dd일");
        }

        private void mesModeLabel_Click(object sender, EventArgs e)
        {
            AppSettings.MesEnabled = !AppSettings.MesEnabled;
            UpdateMesStatus();
        }

        private void countInitButton_Click(object sender, EventArgs e)
        {
            if (CurrentProbeCount == null)
            {
                return;
            }

            // 한번 더 물어본다.
            string message = $"{CurrentProbeCount.FixtureId}번 픽스쳐의 검사수량을 초기화하겠습니까?";
            var result = InformationBox.Show(message, "검사수량 초기화", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
            if (result != InformationBoxResult.OK)
            {
                return;
            }

            CurrentProbeCount.ResetTodayTestCount();
            MainViewModel.SaveProbeCount(CurrentProbeCount);
            DisplayProbeCount(CurrentProbeCount);

            MainViewModel.AddHistoryGroup();
        }

        private void pcbViewButton_Click(object sender, EventArgs e)
        {
            ViewModel?.ShowPcbViewer(currentProduct?.Project);
        }

        private void UpdateNovaChannelStates(bool updateChannel1, bool updateChannel2, bool updateChannel3, bool updateChannel4)
        {
            try
            {
                var channels = new List<int>();
                if (updateChannel1) channels.Add(1);
                if (updateChannel2) channels.Add(2);
                if (updateChannel3) channels.Add(3);
                if (updateChannel4) channels.Add(4);

                UpdateNovaChannelStates(channels);
            }
            catch (Exception e)
            {
                Logger.LogError($"Novaflash 상태 오류: {e.Message}");
            }
        }

        private string MakeMesErrorMessage(int resultCode)
        {
            try
            {
                var infoManager = MesResultInfoManager.Load();
                foreach (var resultInfo in infoManager.Results)
                {
                    if (resultInfo.ResultCode == resultCode)
                    {
                        string errorMessage = resultInfo.ErrorMessage.Replace("/", Environment.NewLine);
                        return $"MES 응답 코드: {resultCode}{Environment.NewLine}{errorMessage}";
                    }
                }

                return $"MES 응답 코드: {resultCode}{Environment.NewLine}정의되지 않은 응답 코드입니다.";
            }
            catch (Exception e)
            {
                Logger.LogTimedMessage($"MES 결과 정보 로딩 오류: {e.Message}");
                return $"MES 응답 코드: {resultCode}{Environment.NewLine}응답 코드 정보 로딩에 실패했습니다.";
            }
        }

        private void versionLabel_Click(object sender, EventArgs e)
        {
            var form = new VersionHistoryForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowDialog(this);
        }

        private void elozProjOpenButton_Click(object sender, EventArgs e)
        {
            try
            {
                UseWaitCursor = true;
                Enabled = false;

                // 상태 업데이트 종료.
                statusReadTimer.Stop();

                ViewModel.PlcClose();
                ViewModel.DioClose();
                ViewModel.DioRWClose();

                string ictProject = currentProduct?.Project?.IctProjectName;
                string selectedProject = Eloz1.SelectProject("열려는 프로젝트를 선택하세요.", "eloZ1 프로젝트 열기", ictProject);
                if (!string.IsNullOrEmpty(selectedProject))
                {
                    ViewModel?.Eloz1ShowTestEditor(selectedProject);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
            }
            finally
            {
                UseWaitCursor = false;
                Enabled = true;

                // 상태 업데이트 다시 시작.
                statusReadTimer.Start();
            }
        }
    }
}
