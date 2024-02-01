using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class PowerDeviceSetting : VisaDeviceSetting
    {
        public PowerDeviceSetting()
        {
            DeviceType = DeviceType.ODA_EX_Series;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
