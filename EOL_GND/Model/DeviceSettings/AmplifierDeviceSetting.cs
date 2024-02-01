using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class AmplifierDeviceSetting : VisaDeviceSetting
    {
        public AmplifierDeviceSetting()
        {
            DeviceType = DeviceType.Keysight_33502A;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
