using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class LinDeviceSetting : DeviceSetting
    {
        /// <summary>
        /// Peak 디바이스 타입.
        /// </summary>
        public PeakLinHardwareType HardwareType { get; set; } = PeakLinHardwareType.PLIN_USB;

        /// <summary>
        /// 1부터 시작하는 디바이스 번호.
        /// </summary>
        public int DeviceNumber { get; set; } = 1;

        /// <summary>
        /// 1부터 시작하는 채널 번호.
        /// </summary>
        public int Channel { get; set; } = 1;

        public LinDeviceSetting()
        {
            DeviceType = DeviceType.PeakLIN;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
