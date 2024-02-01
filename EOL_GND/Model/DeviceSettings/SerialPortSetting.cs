using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model
{
    public class SerialPortSetting : SerialDeviceSetting
    {
        public SerialPortSetting()
        {
            DeviceType = DeviceType.SerialPort;
            DeviceName = DeviceType.GetCategory().ToString();
        }
    }
}
