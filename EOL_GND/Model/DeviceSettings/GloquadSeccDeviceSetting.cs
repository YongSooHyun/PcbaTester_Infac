using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class GloquadSeccDeviceSetting : SerialDeviceSetting
    {
        public GloquadSeccDeviceSetting()
        {
            DeviceType = DeviceType.GloquadSECC;
            DeviceName = DeviceType.GetCategory().ToString();
            BaudRate = 38400;
        }
    }
}
