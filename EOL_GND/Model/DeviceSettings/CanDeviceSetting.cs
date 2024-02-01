using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class CanDeviceSetting : DeviceSetting
    {
        /// <summary>
        /// Peak 디바이스 타입.
        /// </summary>
        public PeakCanDeviceType ConnectionType { get; set; } = PeakCanDeviceType.PCI;

        /// <summary>
        /// 1부터 시작하는 채널 번호.
        /// </summary>
        public int Channel { get; set; } = 1;

        public CanDeviceSetting()
        {
            DeviceType = DeviceType.PeakCAN;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
