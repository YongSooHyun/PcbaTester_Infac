using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class WaveformGeneratorDeviceSetting : VisaDeviceSetting
    {
        public int Channel1High { get; set; }
        public int Channel1Low { get; set; }
        public int Channel2High { get; set; }
        public int Channel2Low { get; set; }

        public WaveformGeneratorDeviceSetting()
        {
            DeviceType = DeviceType.Keysight_EDU33210_Series;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
