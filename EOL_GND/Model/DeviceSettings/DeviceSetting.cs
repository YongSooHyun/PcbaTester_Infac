using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 장치연결을 위한 설정.
    /// </summary>
    [XmlInclude(typeof(SerialDeviceSetting))]
    [XmlInclude(typeof(VisaDeviceSetting))]
    [XmlInclude(typeof(CanDeviceSetting))]
    [XmlInclude(typeof(LinDeviceSetting))]
    public abstract class DeviceSetting
    {
        /// <summary>
        /// 장치의 구체적인 타입.
        /// </summary>
        public DeviceType DeviceType { get; set; } = DeviceType.ODA_EX_Series;

        /// <summary>
        /// 장치 이름.
        /// </summary>
        public string DeviceName { get; set; }

        /// <summary>
        /// 마지막 이름에 있는 수를 하나 증가시켜 새로운 이름을 만든다.
        /// </summary>
        /// <param name="lastName"></param>
        /// <param name="defaultName"></param>
        /// <returns></returns>
        public static string CreateNewName(string lastName, string defaultName)
        {
            if (!string.IsNullOrEmpty(lastName))
            {
                // 이름속에 포함된 마지막 수를 찾는다.
                var pattern = @"(\d+)(?!.*\d)";
                var match = Regex.Match(lastName, pattern);
                if (int.TryParse(match.Value, out int parsed))
                {
                    return Regex.Replace(lastName, pattern, (parsed + 1).ToString());
                }
            }

            return defaultName;
        }
    }
}
