using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model.DBC
{
    public class DbcSignal : ICloneable
    {
        /// <summary>
        /// 신호 이름.
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// 비트 시작위치.
        /// </summary>
        public int StartBit { get; set; }

        /// <summary>
        /// 비트 길이.
        /// </summary>
        public int Length { get; set; }

        /// <summary>
        /// LittleEndian 여부.
        /// </summary>
        public bool IntelByteOrder { get; set; }

        public bool Signed { get; set; }
        public double Factor { get; set; }
        public double Offset { get; set; }
        public double Minimum { get; set; }
        public double Maximum { get; set; }
        public PhysicalUnit Unit { get; set; }
        public long? ErrorID { get; set; }

        public object Clone()
        {
            var newObj = new DbcSignal();
            newObj.Name = Name;
            newObj.StartBit = StartBit;
            newObj.Length = Length;
            newObj.IntelByteOrder = IntelByteOrder;
            newObj.Signed = Signed;
            newObj.Factor = Factor;
            newObj.Offset = Offset;
            newObj.Minimum = Minimum;
            newObj.Maximum = Maximum;
            newObj.Unit = Unit;
            newObj.ErrorID = ErrorID;
            return newObj;
        }
    }
}
