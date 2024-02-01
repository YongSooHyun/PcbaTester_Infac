using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    public enum ComPort
    {
        COM1 = 1,
        COM2,
        COM3,
        COM4,
        COM5,
        COM6,
        COM7,
        COM8,
        COM9,
        COM10,
        COM11,
        COM12,
        COM13,
        COM14,
        COM15,
        COM16,
        COM17,
        COM18,
        COM19,
        COM20,
        COM21,
        COM22,
        COM23,
        COM24,
        COM25,
        COM26,
        COM27,
        COM28,
        COM29,
        COM30,
    }

    /// <summary>
    /// COM 포트 설정을 보관한다.
    /// </summary>
    [XmlInclude(typeof(DioDeviceSetting))]
    [XmlInclude(typeof(GloquadSeccDeviceSetting))]
    [XmlInclude(typeof(SerialPortSetting))]
    public abstract class SerialDeviceSetting : DeviceSetting
    {
        /// <summary>
        /// COM 포트.
        /// </summary>
        public ComPort Port { get; set; } = ComPort.COM1;

        public int BaudRate { get; set; } = 9600;

        public int DataBits { get; set; } = 8;

        public Parity Parity { get; set; } = Parity.None;

        public bool CR { get; set; } = false;

        public bool LF { get; set; } = true;

        internal string NewLine => (CR ? "\r" : "") + (LF ? "\n" : "");

        /// <summary>
        /// Read/write timeout(ms).
        /// </summary>
        public int Timeout { get; set; } = 2000;
    }
}
