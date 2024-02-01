using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class OscopeDeviceSetting : VisaDeviceSetting
    {
        public int Channel1High { get; set; }
        public int Channel1Low { get; set; }
        public int Channel2High { get; set; }
        public int Channel2Low { get; set; }
        public int Channel3High { get; set; }
        public int Channel3Low { get; set; }
        public int Channel4High { get; set; }
        public int Channel4Low { get; set; }

        public OscopeDeviceSetting()
        {
            DeviceType = DeviceType.Keysight_InfiniiVision_3000T_X;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
