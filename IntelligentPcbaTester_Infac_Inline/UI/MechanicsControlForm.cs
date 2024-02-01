using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class MechanicsControlForm : Form
    {
        internal MainViewModel ViewModel { get; set; }

        // 읽기를 위한 타이머.
        private readonly System.Timers.Timer readTimer = new System.Timers.Timer(500);

        // 읽기 개수를 제한한다.
        private int plcReadCount = 0;
        private const int PlcReadMaxCount = 2;

        public MechanicsControlForm()
        {
            InitializeComponent();

            readTimer.Elapsed += ReadTimer_Elapsed;
            readTimer.Start();
        }

        private void ReadTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            UpdateState();
        }

        private void UpdateState()
        {
            if (plcReadCount >= PlcReadMaxCount || ViewModel == null)
            {
                return;
            }

            try
            {
                Interlocked.Increment(ref plcReadCount);

                var state = ViewModel.PlcReadStatus(false);

                // 자동/수동 모드.
                bool isAutoMode = (state & PlcReadFlags.Mode) == 0;

                // 전면 도어 오픈 상태.
                bool frontDoorOpened = (state & PlcReadFlags.FrontDoor) == 0;

                // 후면 도어 오픈 상태.
                bool rearDoorOpened = (state & PlcReadFlags.RearDoor) == 0;

                // 하부 픽스처 장착 상태.
                bool bottomFixtureEquipped = (state & PlcReadFlags.BottomFixtureSensor) != 0;

                // 상부 픽스처 장착 상태.
                bool topFixtureEquipped = (state & PlcReadFlags.TopFixtureSensor) != 0;

                // PCB 존재 여부.
                bool pcbExists = (state & PlcReadFlags.PcbSensor) != 0;

                // 실린더가 하부에 있는지 여부.
                bool isCylinderAtBottom = (state & PlcReadFlags.CylinderBottomSensor) != 0;

                // 실린더가 FCT 위치에 있는지 여부.
                bool isCylinderAtFct = (state & PlcReadFlags.CylinderFctSensor) != 0;

                // 실린더가 상부에 있는지 여부.
                bool isCylinderAtTop = (state & PlcReadFlags.CylinderTopSensor) != 0;

                // 공압 정상 여부.
                bool isPneumaticNormal = (state & PlcReadFlags.AirPressure) != 0;

                // 안전센서 여부.
                bool isSafetyNormal = (state & PlcReadFlags.SafetySensor) != 0;

                // 컨베이어 작동 여부.
                bool isConveyorWorking = (state & PlcReadFlags.ConveyorSensor) != 0;

                // 비상 상황 여부.
                bool isEmergency = (state & PlcReadFlags.Emergency) == 0;

                // 하부 클램프.
                bool bottomClampOn = (state & PlcReadFlags.BottomClamp) != 0;

                // 상부 클램프.
                bool topClampOn = (state & PlcReadFlags.TopClamp) != 0;

                Utils.InvokeIfRequired(this, () =>
                {
                    modeControl.ON = isAutoMode;
                    frontDoorControl.ON = !frontDoorOpened;
                    rearDoorControl.ON = !rearDoorOpened;
                    bottomFixtureControl.ON = bottomFixtureEquipped;
                    topFixtureControl.ON = topFixtureEquipped;
                    pcbControl.ON = pcbExists;
                    cylinderBottomControl.ON = isCylinderAtBottom;
                    cylinderFctControl.ON = isCylinderAtFct;
                    cylinderTopControl.ON = isCylinderAtTop;
                    pneumaticControl.ON = isPneumaticNormal;
                    safetyControl.ON = isSafetyNormal;
                    conveyorControl.ON = !isConveyorWorking;
                    emergencyControl.ON = isEmergency;
                    bottomClampControl.ON = bottomClampOn;
                    topClampControl.ON = topClampOn;
                });
            }
            catch
            {
            }
            finally
            {
                Interlocked.Decrement(ref plcReadCount);
            }
        }

        private async void modeButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcSetMode(!modeControl.ON);
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async void cylDownButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcCylinderDown();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async void cylUpButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcCylinderUp();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async void cylFctUpButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcCylinderFctUp();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async void cylMidUpButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcCylinderMidUp();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        private async void cylInitButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel == null)
                {
                    return;
                }

                Enabled = false;
                UseWaitCursor = true;

                await Task.Run(() =>
                {
                    ViewModel?.PlcOpen();
                    ViewModel?.PlcCylinderInit();
                });
            }
            catch (Exception ex)
            {
                Logger.LogError(ex.Message);
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
                UseWaitCursor = false;
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            readTimer.Dispose();

            base.OnFormClosing(e);
        }
    }
}
