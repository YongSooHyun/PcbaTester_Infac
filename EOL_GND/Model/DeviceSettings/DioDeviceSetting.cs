using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class DioDeviceSetting : SerialDeviceSetting
    {
        /// <summary>
        /// 사용 가능한 명령 리스트.
        /// </summary>
        public List<DioDevice.CommandInfo> Commands { get; set; } = new List<DioDevice.CommandInfo>();

        public DioDeviceSetting()
        {
            DeviceType = DeviceType.DIO;
            DeviceName = DeviceType.GetCategory().ToString();

            BaudRate = 115200;
            CR = true;
            LF = false;
        }
    }
}
