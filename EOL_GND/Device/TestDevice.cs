using EOL_GND.Common;
using EOL_GND.Model;
using Ivi.Visa;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    public abstract class TestDevice : IDisposable
    {
        /// <summary>
        /// 디바이스 설정.
        /// </summary>
        public DeviceSetting Setting { get; protected set; }

        /// <summary>
        /// 디바이스에 연결/끊기 반복을 피하기 위한 디바이스 리스트.
        /// </summary>
        private readonly static List<TestDevice> devices = new List<TestDevice>();

        private bool disposedValue = false;

        /// <summary>
        /// 이미 만든 디바이스들 중에서 같은 설정을 가진 디바이스를 찾는다.
        /// </summary>
        /// <param name="setting"></param>
        /// <returns></returns>
        public static TestDevice FindDevice(DeviceSetting setting, bool reconnect = true)
        {
            foreach (var device in devices)
            {
                if (DeviceSettingsManager.IsSameDevice(device.Setting, setting))
                {
                    // 연결이 끊어진 경우에 대처하기 위한 처리.
                    //var dmmDevice = device as DmmDevice;
                    //var powerDevice = device as PowerDevice;
                    //if ((dmmDevice != null || powerDevice != null) && reconnect)
                    //{
                    //    try
                    //    {
                    //        dmmDevice?.ReadIDN(CancellationToken.None);
                    //        powerDevice?.ReadIDN(CancellationToken.None);
                    //    }
                    //    catch (NativeVisaException nativeVisaEx)
                    //    {
                    //        // 만일 연결이 끊어졌으면 연결 재설정.
                    //        if (nativeVisaEx.ErrorCode == NativeErrorCode.ConnectionLost)
                    //        {
                    //            dmmDevice?.Disconnect();
                    //            dmmDevice?.Connect(CancellationToken.None);

                    //            powerDevice?.Disconnect();
                    //            powerDevice?.Connect(CancellationToken.None);

                    //            dmmDevice?.ReadIDN(CancellationToken.None);
                    //            powerDevice?.ReadIDN(CancellationToken.None);
                    //        }
                    //        else
                    //        {
                    //            throw;
                    //        }
                    //    }
                    //}

                    return device;
                }
            }

            return null;
        }

        /// <summary>
        /// 새로 만든 디바이스를 추가.
        /// </summary>
        /// <param name="device"></param>
        public static void AddDevice(TestDevice device)
        {
            devices.Insert(0, device);
        }

        /// <summary>
        /// 연결된 모든 디바이스들을 연결 해제한다.
        /// </summary>
        public static void CloseAllDevices()
        {
            foreach (var device in devices)
            {
                // ASYNC 다운로드가 끝날 때까지 기다린다.
                if (device is KeysightInfiniiVision3000TXDevice scopeDevice)
                {
                    scopeDevice.DownloadEvent.WaitOne();
                }

                device.Dispose();
            }
            devices.Clear();
        }

        protected TestDevice(DeviceType deviceType, string name)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            Setting = settingsManager.FindSetting(deviceType.GetCategory(), name);
        }

        /// <summary>
        /// 디바이스에 연결한다.
        /// </summary>
        public abstract void Connect(CancellationToken token);

        /// <summary>
        /// 디바이스 연결을 해제한다.
        /// </summary>
        public abstract void Disconnect();

        /// <summary>
        /// 디바이스에 지정한 명령을 전송하고 그 응답을 선택적으로 읽는다.
        /// 응답을 해석하지 않고 그대로 보여준다.
        /// </summary>
        /// <param name="command"></param>
        /// <param name="read"></param>
        /// <param name="token"></param>
        /// <returns></returns>
        public abstract string RunCommand(string command, bool read, int readTimeout, CancellationToken token);

        /// <summary>
        /// 지정한 파라미터의 최솟값을 얻는다.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public abstract object GetMinValue(object step, string paramName, CancellationToken token);

        /// <summary>
        /// 지정한 파라미터의 최댓값을 얻는다.
        /// </summary>
        /// <param name="paramName"></param>
        /// <returns></returns>
        public abstract object GetMaxValue(object step, string paramName, CancellationToken token);

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // Dispose managed state (managed objects)
                }

                // Free unmanaged resources (unmanaged objects) and override finalizer
                // Set large fields to null
                disposedValue = true;
            }
        }

        // Override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~TestDevice()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }

    /// <summary>
    /// 디바이스 종류.
    /// </summary>
    public enum DeviceCategory
    {
        Power,
        DMM,
        Oscilloscope,
        WaveformGenerator,
        Amplifier,
        DIO,
        CAN,
        LIN,
        GloquadSECC,
        SerialPort,
    }

    /// <summary>
    /// 시스템에서 지원하는 디바이스 리스트.
    /// </summary>
    public enum DeviceType
    {
        ODA_EX_Series,
        MK_P_Series,
        Fluke_8845A_8846A,
        Keysight_Truevolt_Series,
        Keysight_InfiniiVision_3000T_X,
        Keysight_EDU33210_Series,
        Keysight_33502A,
        DIO,
        PeakCAN,
        PeakLIN,
        GloquadSECC,
        SerialPort,
    }

    public static class DeviceTypeExtensions
    {
        public static string GetText(this DeviceType type)
        {
            switch (type)
            {
                case DeviceType.ODA_EX_Series:
                    return "ODA EX Series";
                    case DeviceType.MK_P_Series:
                    return "MKPOWER P Series";
                case DeviceType.Fluke_8845A_8846A:
                    return "Fluke 8845A/8846A";
                case DeviceType.Keysight_Truevolt_Series:
                    return "Keysight Truevolt Series(34460A/34461A/34465A/34470A)";
                case DeviceType.Keysight_InfiniiVision_3000T_X:
                    return "Keysight InfiniiVision 3000T X-Series";
                case DeviceType.Keysight_EDU33210_Series:
                    return "Keysight EDU33210 Series";
                case DeviceType.Keysight_33502A:
                    return "Keysight 33502A";
                case DeviceType.DIO:
                    return "DIO";
                case DeviceType.PeakCAN:
                    return "PEAK-System PCAN";
                case DeviceType.PeakLIN:
                    return "PEAK-System PLIN";
                case DeviceType.GloquadSECC:
                    return "GloquadTech SECC";
                case DeviceType.SerialPort:
                    return "Serial Port";
                default:
                    return null;
            }
        }

        public static DeviceCategory GetCategory(this DeviceType deviceType)
        {
            DeviceCategory category;
            switch (deviceType)
            {
                case DeviceType.ODA_EX_Series:
                case DeviceType.MK_P_Series:
                    category = DeviceCategory.Power;
                    break;
                case DeviceType.Fluke_8845A_8846A:
                case DeviceType.Keysight_Truevolt_Series:
                    category = DeviceCategory.DMM;
                    break;
                case DeviceType.Keysight_InfiniiVision_3000T_X:
                    category = DeviceCategory.Oscilloscope;
                    break;
                case DeviceType.Keysight_EDU33210_Series:
                    category = DeviceCategory.WaveformGenerator;
                    break;
                case DeviceType.Keysight_33502A:
                    category = DeviceCategory.Amplifier;
                    break;
                case DeviceType.DIO:
                    category = DeviceCategory.DIO;
                    break;
                case DeviceType.PeakCAN:
                    category = DeviceCategory.CAN;
                    break;
                case DeviceType.PeakLIN:
                    category = DeviceCategory.LIN;
                    break;
                case DeviceType.GloquadSECC:
                    category = DeviceCategory.GloquadSECC;
                    break;
                case DeviceType.SerialPort:
                    category = DeviceCategory.SerialPort;
                    break;
                default:
                    category = DeviceCategory.Power;
                    break;
            }
            return category;
        }
    }
}
