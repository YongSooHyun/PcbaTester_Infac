using E100RC_Production;
using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.Properties;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.IO.Ports;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public class GloquadSeccDevice : TestDevice
    {
        /// <summary>
        /// 충전 Progress가 변할 때 호출된다. 상태 메시지를 파라미터로 전달한다.
        /// </summary>
        public event EventHandler<string> ProgressChanged;

        public bool Opened { get; private set; } = false;

        // EVSE controller.
        private readonly dcComboPLC evseController = new dcComboPLC();
        private BackgroundWorker evseBgWorker;
        private ManualResetEvent evseBgWorkerResetEvent = new ManualResetEvent(false);

        // Dispose패턴 필드.
        private bool disposedValue = false;

        /// <summary>
        /// 지정한 이름을 가진 Gloquad SECC 디바이스를 리턴한다.
        /// 해당 이름을 가진 Gloquad SECC 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static GloquadSeccDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var deviceSetting = settingsManager.FindSetting(DeviceCategory.GloquadSECC, deviceName);

            var oldDevice = FindDevice(deviceSetting);
            if (oldDevice is GloquadSeccDevice seccDevice)
            {
                Logger.LogVerbose($"Using old device: {deviceSetting.DeviceType}, {deviceSetting.DeviceName}");
                return seccDevice;
            }

            GloquadSeccDevice device;
            switch (deviceSetting.DeviceType)
            {
                case DeviceType.GloquadSECC:
                default:
                    device = new GloquadSeccDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        private GloquadSeccDevice(string name) : base(DeviceType.GloquadSECC, name)
        {
        }

        public override void Connect(CancellationToken token)
        {
            token.ThrowIfCancellationRequested();

            // value init
            evseController.evseStatusCode = DC_EVSEStatusCodeType.EVSE_Ready;
            evseController.eNumEVSEProcess = EVSEProcessingType.Finished;
            evseController.eNumresponseCode = responseCodeType.OK;

            evseController.ui_Min = evseController.ui_Sec = 0;
            evseController.initValues();

            // Serial init
            evseController.evse_serial.DataReceived += Evse_serial_DataReceived;

            // Open.
            var serialSetting = Setting as SerialDeviceSetting;
            if (!evseController.OpenSerialPort(serialSetting.Port.ToString(), serialSetting.BaudRate))
            {
                throw new Exception($"Gloquad SECC cannot open device(Port={serialSetting.Port}, Baudrate={serialSetting.BaudRate})");
            }

            Opened = true;
        }

        private void Evse_serial_DataReceived(object sender, SerialDataReceivedEventArgs e)
        {
            try
            {
                evseController.readPLC_COM();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Gloquad SECC error: {ex.Message}");
            }
        }

        public void StartCharging(CancellationToken token)
        {
            evseController.startReq_info[0] = 0;
            evseController.startReq_info[1] = 0;
            evseController.startReq_info[2] = 0;
            evseController.startReq_info[3] = 0;

            evse_resp_stop_condition evse_resp_stop_condition = evseController.evse_resp_stop_condition;
            evse_status_condition evse_status_condition = evseController.evse_status_condition;

            evse_resp_stop_condition.init();
            evse_status_condition.init();
            evseController.SetPhysicalValue(ref evseController.MinEVSEVoltageLimit, 0, 5, 100);      // 100 V
            evseController.SetPhysicalValue(ref evseController.MinEVSECurrentLimit, 0, 3, 0);  // 0A

            evseController.SetPhysicalValue(ref evseController.MaxEVSEVoltageLimit, 0, 5, 750);      // 750V
            evseController.SetPhysicalValue(ref evseController.MaxEVSECurrentLimit, 0, 3, 10);   // 10A
            evseController.SetPhysicalValue(ref evseController.MaxEVSEPowerLimit, 2, 7, (short)(50000 / 100));      // 50,000W
            evseController.SetPhysicalValue(ref evseController.EVSEVoltage, 0, 5, 0);

            if (!Opened)
            {
                Connect(token);
            }

            token.Register(() =>
            {
                evseController.Charger_Stop();
                evseBgWorker?.CancelAsync();
            });

            evseController.Charger_Start();
            InitBgWorker();
        }

        private void InitBgWorker()
        {
            evseBgWorker?.Dispose();
            evseBgWorker = new BackgroundWorker();
            evseBgWorker.WorkerReportsProgress = true;
            evseBgWorker.WorkerSupportsCancellation = true;

            // BackgroundWorker event handler
            evseBgWorker.DoWork += EvseBgWorker_DoWork;
            evseBgWorker.RunWorkerCompleted += EvseBgWorker_RunWorkerCompleted;
            evseBgWorker.ProgressChanged += EvseBgWorker_ProgressChanged;

            // BgWorker를 끝내는데 사용.
            evseBgWorkerResetEvent.Reset();

            // Running BackgroundWorker
            // It is possible to run it by putting parameters.
            // If there are multiple parameters, use an array.
            evseBgWorker.RunWorkerAsync();
        }

        private void EvseBgWorker_DoWork(object sender, DoWorkEventArgs e)
        {
            uint timer_cnt = 0;
            short prechargeCnt = 10;
            short currentDemandCnt = 10;

            var bgWorker = sender as BackgroundWorker;
            while (true)
            {
                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                var signaled = evseBgWorkerResetEvent.WaitOne(100);
                if (signaled)
                {
                    break;
                }

                if (bgWorker.CancellationPending)
                {
                    e.Cancel = true;
                    break;
                }

                timer_cnt++;
                if (timer_cnt % 3 == 0 && evseController.status_text.Length > 0)
                {
                    evseBgWorker.ReportProgress(0xff);
                }

                if (timer_cnt % 5 == 0)
                {
                    if (evseController.startReq_info[0] == 0)
                    {
                        if (evseController.curChagerStep == MsgID.PreChargeReq)
                        {
                            if (evseController.EVSEVoltage.Value < evseController.evTargetVoltage)
                            {
                                evseController.EVSEVoltage.Value = (short)(evseController.EVSEVoltage.Value + (evseController.evTargetVoltage / prechargeCnt));
                                if (evseController.EVSEVoltage.Value < evseController.evTargetVoltage)
                                {
                                    evseController.EVSEVoltage.Value = evseController.evTargetVoltage;
                                }
                                //Logger.LogVerbose("Gloquad SECC BgWork EVSE Voltage Set");
                                Logger.LogVerbose("Gloquad SECC EVSEVoltage.Value = " + evseController.EVSEVoltage.Value + "V" + ", CurChargerStep = " + evseController.curChagerStep);
                            }
                        }
                        else if (evseController.curChagerStep == MsgID.CurrentDemandReq)
                        {
                            //evse_ctrl.EVSEVoltage.Value = (short)(evse_ctrl.EVSEVoltage.Value + (evse_ctrl.evTargetVoltage / currentDemandCnt));
                            if (evseController.EVSEVoltage.Value < evseController.evTargetVoltage)
                            {
                                evseController.EVSEVoltage.Value = (short)(evseController.EVSEVoltage.Value + (evseController.evTargetVoltage / currentDemandCnt));
                                if (evseController.EVSEVoltage.Value < evseController.evTargetVoltage)
                                {
                                    evseController.EVSEVoltage.Value = evseController.evTargetVoltage;
                                }
                                //Logger.LogVerbose("Gloquad SECC BgWork EVSE Voltage Set");
                                Logger.LogVerbose("Gloquad SECC EVSEVoltage.Value = " + evseController.EVSEVoltage.Value + "V" + ", CurChargerStep = " + evseController.curChagerStep);
                            }

                            if (evseController.EVSECurrent.Value < evseController.evTargetCurrent)
                            {
                                evseController.EVSECurrent.Value = (short)(evseController.EVSECurrent.Value + (evseController.evTargetCurrent / currentDemandCnt));
                                if (evseController.EVSECurrent.Value < evseController.evTargetCurrent)
                                {
                                    evseController.EVSECurrent.Value = evseController.evTargetCurrent;
                                }
                                //Logger.LogVerbose("Gloquad SECC BgWork EVSE Current Set");
                                Logger.LogVerbose("Gloquad SECC EVSECurrent.Value = " + evseController.EVSECurrent.Value + "V" + ", CurChargerStep = " + evseController.curChagerStep);
                            }
                        }
                    }
                }
            }
        }

        private void EvseBgWorker_ProgressChanged(object sender, ProgressChangedEventArgs e)
        {
            // ProgressChanged
            // An event is raised when there is a change in progress.
            // writing the code here to show how far it progressed.
            Logger.LogVerbose($"Gloquad SECC Progress = {evseController.status_text}");
            ProgressChanged?.Invoke(this, evseController.status_text);
            evseController.status_text = "";
        }

        private void EvseBgWorker_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            string statusText = "";
            if (e.Error != null)
            {
                statusText = $"Exception: {e.Error.Message}";
            }
            else if (e.Cancelled)
            {
                statusText = "Cancelled";
            }
            else
            {
                statusText = "Completed";
            }

            Logger.LogVerbose($"Gloquad SECC BgWork {statusText}");
        }

        public void StopCharging()
        {
            if (Opened)
            {
                evseController.Charger_Stop();
                evseBgWorkerResetEvent.Set();
            }

            Disconnect();
        }

        public override void Disconnect()
        {
            evseController.CloseSerialPort();
            evseController.evse_serial.DataReceived -= Evse_serial_DataReceived;
            Opened = false;
        }

        public override string RunCommand(string command, bool read, int readTimeout, CancellationToken token)
        {
            throw new NotSupportedException();
        }

        public override object GetMinValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        public override object GetMaxValue(object step, string paramName, CancellationToken token)
        {
            return null;
        }

        protected override void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    StopCharging();
                    evseBgWorkerResetEvent.Dispose();
                }

                disposedValue = true;
            }

            base.Dispose(disposing);
        }
    }
}
