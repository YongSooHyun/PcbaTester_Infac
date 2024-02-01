using EOL_GND.Common;
using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 각 디바이스의 COM Port 설정을 관리(저장, 로딩) 한다.
    /// </summary>
    public class DeviceSettingsManager
    {
        /// <summary>
        /// 설정 보관 파일 이름.
        /// </summary>
        //private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "devices.cfg");
        private static string FileName => "D:\\ElozPlugin\\eol_devices.config";

        /// <summary>
        /// Shared instance.
        /// </summary>
        public static DeviceSettingsManager SharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = Load();
                }
                return _sharedInstance;
            }
        }
        private static DeviceSettingsManager _sharedInstance = null;

        /// <summary>
        /// 디바이스 설정 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<DeviceSetting> DeviceSettings { get; set; } = new List<DeviceSetting>();

        /// <summary>
        /// 계측기 1번 채널 High의 릴레이 번호를 입력할 때 나머지 릴레이 번호들도 자동으로 업데이트할지 여부.
        /// </summary>
        public bool AutoFillChannels { get; set; } = true;

        /// <summary>
        /// 릴레이 채널 자동 할당을 위한 릴레이 카드 번호.
        /// </summary>
        public int RelayCardNumber { get; set; } = 18;

        /// <summary>
        /// 릴레이 카드 당 채널 수.
        /// </summary>
        public int RelayChannelsPerCard { get; set; } = 64;

        /// <summary>
        /// 릴레이 카드의 시작 채널.
        /// </summary>
        public int RelayChannelOffset { get; set; } = 33;

        /// <summary>
        /// 릴레이 채널 장치 할당 리스트.
        /// </summary>
        public List<StepCategoryInfo> RelayDevices { get; set; }

        private DeviceSettingsManager()
        {
        }

        /// <summary>
        /// 이 클래스의 인스턴스를 파일에 보관한다.
        /// </summary>
        internal void Save()
        {
            // 파일을 저장할 폴더가 없으면 만든다.
            Directory.CreateDirectory(Path.GetDirectoryName(FileName));
            using (var writer = new StreamWriter(FileName))
            {
                var xmlSerializer = new XmlSerializer(GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 이 클래스를 XML파일로부터 로딩한다.
        /// </summary>
        /// <returns>로딩한 오브젝트.</returns>
        private static DeviceSettingsManager Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(FileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(DeviceSettingsManager));
                var obj = xmlSerializer.Deserialize(stream) as DeviceSettingsManager;
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot load device settings: {ex.Message}");
                var obj = new DeviceSettingsManager();
                CreateDefaultSettings(obj);
                return obj;
            }
            finally
            {
                stream?.Close();
            }
        }

        private static void CreateDefaultSettings(DeviceSettingsManager manager)
        {
        }

        /// <summary>
        /// 두 설정이 같은 디바이스를 가리키는지 체크한다.
        /// </summary>
        /// <param name="setting1"></param>
        /// <param name="setting2"></param>
        /// <returns></returns>
        internal static bool IsSameDevice(DeviceSetting setting1, DeviceSetting setting2)
        {
            return setting1.DeviceType == setting2.DeviceType 
                && string.Equals(setting1.DeviceName, setting2.DeviceName, StringComparison.OrdinalIgnoreCase);
        }

        /// <summary>
        /// 디바이스의 설정을 찾아서 리턴한다.
        /// </summary>
        /// <param name="deviceName">찾으려는 디바이스 이름.</param>
        /// <returns>찾은 설정 오브젝트. 없으면 예외를 발생시킨다.</returns>
        internal DeviceSetting FindSetting(DeviceCategory devCategory, string deviceName)
        {
            foreach (var setting in DeviceSettings)
            {
                if (setting.DeviceType.GetCategory() == devCategory && 
                    string.Equals(setting.DeviceName, deviceName, StringComparison.OrdinalIgnoreCase))
                {
                    return setting;
                }
            }

            throw new Exception($"디바이스 '{deviceName}'의 설정정보를 찾을 수 없습니다.");
        }

        internal List<PowerDeviceSetting> GetPowerSettings()
        {
            return DeviceSettings.Where(setting => setting is PowerDeviceSetting).Cast<PowerDeviceSetting>().ToList();
        }

        internal List<WaveformGeneratorDeviceSetting> GetWaveformGeneratorSettings()
        {
            return DeviceSettings.Where(setting => setting is WaveformGeneratorDeviceSetting).Cast<WaveformGeneratorDeviceSetting>().ToList();
        }

        internal List<DmmDeviceSetting> GetDmmSettings()
        {
            return DeviceSettings.Where(setting => setting is DmmDeviceSetting).Cast<DmmDeviceSetting>().ToList();
        }

        internal List<OscopeDeviceSetting> GetOscopeSettings()
        {
            return DeviceSettings.Where(setting => setting is OscopeDeviceSetting).Cast<OscopeDeviceSetting>().ToList();
        }

        internal List<DioDeviceSetting> GetDioSettings()
        {
            return DeviceSettings.Where(setting => setting is DioDeviceSetting).Cast<DioDeviceSetting>().ToList();
        }

        internal List<GloquadSeccDeviceSetting> GetGloquadSeccSettings()
        {
            return DeviceSettings.Where(setting => setting is GloquadSeccDeviceSetting).Cast<GloquadSeccDeviceSetting>().ToList();
        }

        internal List<CanDeviceSetting> GetCanSettings()
        {
            return DeviceSettings.Where(setting => setting is CanDeviceSetting).Cast<CanDeviceSetting>().ToList();
        }

        internal List<LinDeviceSetting> GetLinSettings()
        {
            return DeviceSettings.Where(setting => setting is LinDeviceSetting).Cast<LinDeviceSetting>().ToList();
        }

        internal List<AmplifierDeviceSetting> GetAmplifierSettings()
        {
            return DeviceSettings.Where(setting => setting is AmplifierDeviceSetting).Cast<AmplifierDeviceSetting>().ToList();
        }

        internal List<SerialPortSetting> GetSerialPortSettings()
        {
            return DeviceSettings.Where(setting => setting is SerialPortSetting).Cast<SerialPortSetting>().ToList();
        }
    }
}
