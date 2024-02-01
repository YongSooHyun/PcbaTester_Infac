using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO.Ports;

namespace IntelligentPcbaTester
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
    }

    /// <summary>
    /// COM 포트 설정을 보관한다.
    /// </summary>
    public class ComportSettings
    {
        [DisplayName("Device 명")]
        public string DeviceName { get; set; } = "UNKNOWN";

        [DisplayName("COM Port")]
        public ComPort Port { get; set; } = ComPort.COM1;

        [DisplayName("Baudrate")]
        public int BaudRate { get; set; } = 9600;

        [DisplayName("Data Bits")]
        public int DataBits { get; set; } = 8;

        [DisplayName("Parity")]
        public Parity Parity { get; set; } = Parity.None;

        [DisplayName("CR")]
        public bool CR { get; set; } = false;

        [DisplayName("LF")]
        public bool LF { get; set; } = true;

        [DisplayName("Enabled")]
        public bool IsEnabled { get; set; } = true;

        [Browsable(false)]
        public bool Mandatory { get; set; } = true;

        internal string NewLine => (CR ? "\r" : "") + (LF ? "\n" : "");
    }
}
