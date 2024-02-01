using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class DmmDeviceSetting : VisaDeviceSetting
    {
        public int ChannelHighInput { get; set; }
        public int ChannelLowInput { get; set; }
        public int ChannelHighSense { get; set; }
        public int ChannelLowSense { get; set; }

        public DmmDeviceSetting()
        {
            DeviceType = DeviceType.Fluke_8845A_8846A;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
