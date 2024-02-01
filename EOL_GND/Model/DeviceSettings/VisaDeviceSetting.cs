using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// VISA 디바이스 연결을 위한 설정.
    /// </summary>
    [XmlInclude(typeof(WaveformGeneratorDeviceSetting))]
    [XmlInclude(typeof(DmmDeviceSetting))]
    [XmlInclude(typeof(OscopeDeviceSetting))]
    [XmlInclude(typeof(PowerDeviceSetting))]
    [XmlInclude(typeof(AmplifierDeviceSetting))]
    public abstract class VisaDeviceSetting : DeviceSetting
    {
        /// <summary>
        /// 연결하려는 리소스 주소.
        /// </summary>
        public string Address { get; set; }

        /// <summary>
        /// I/O 타임아웃(ms).
        /// </summary>
        public int IOTimeout { get; set; } = 2000;
    }
}
