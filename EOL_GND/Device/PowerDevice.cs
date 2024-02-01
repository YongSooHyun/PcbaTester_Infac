using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace EOL_GND.Device
{
    /// <summary>
    /// POWER 장치.
    /// </summary>
    public abstract class PowerDevice : TestDevice
    {
        /// <summary>
        /// 전원 On.
        /// </summary>
        public abstract void PowerOn(CancellationToken token);

        /// <summary>
        /// 전원 Off.
        /// </summary>
        public abstract void PowerOff(CancellationToken token);

        /// <summary>
        /// 전원 출력상태 리턴.
        /// </summary>
        /// <returns></returns>
        public abstract bool GetPowerState(CancellationToken token);

        /// <summary>
        /// 전압, 전류 설정.
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        public abstract void SetPower(int channel, double voltage, double? current, int queryDelay, CancellationToken token);

        /// <summary>
        /// 현재 설정된 전압, 전류 값을 읽어들인다.
        /// </summary>
        /// <param name="voltage"></param>
        /// <param name="current"></param>
        public abstract void ReadPower(int channel, out double voltage, out double current, CancellationToken token);

        /// <summary>
        /// 출력 전압 측정.
        /// </summary>
        /// <returns></returns>
        public abstract double MeasureVoltage(int channel, CancellationToken token);

        /// <summary>
        /// 출력 전류 측정.
        /// </summary>
        /// <returns></returns>
        public abstract double MeasureCurrent(int channel, CancellationToken token);

        /// <summary>
        /// 식별문자열 읽기.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadIDN(CancellationToken token);

        /// <summary>
        /// 시리얼 넘버를 확인.
        /// </summary>
        /// <returns></returns>
        public abstract string ReadSN(CancellationToken token);

        /// <summary>
        /// 리셋.
        /// </summary>
        public abstract void Reset(CancellationToken token);

        /// <summary>
        /// 지정한 이름을 가진 파워 디바이스를 리턴한다.
        /// 해당 이름을 가진 파워 디바이스 설정이 없으면 예외를 발생시킨다.
        /// </summary>
        /// <param name="deviceName"></param>
        /// <returns></returns>
        public static PowerDevice CreateInstance(string deviceName)
        {
            var settingsManager = DeviceSettingsManager.SharedInstance;
            var powerSetting = settingsManager.FindSetting(DeviceCategory.Power, deviceName);

            var oldDevice = FindDevice(powerSetting);
            if (oldDevice is PowerDevice powerDevice)
            {
                Logger.LogVerbose($"Using old device: {powerSetting.DeviceType}, {powerSetting.DeviceName}");
                return powerDevice;
            }

            PowerDevice device;
            switch (powerSetting.DeviceType)
            {
                case DeviceType.MK_P_Series:
                    device = new MkSeriesDevice(deviceName);
                    break;
                case DeviceType.ODA_EX_Series:
                default:
                    device = new OdaExSeriesDevice(deviceName);
                    break;
            }

            AddDevice(device);
            return device;
        }

        protected PowerDevice(DeviceType devType, string name)
            : base(devType, name)
        {
        }
    }
}
