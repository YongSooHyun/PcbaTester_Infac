using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.DBC
{
    public class DbcMessage : ICloneable
    {
        public uint ID { get; set; }
        public string Name { get; set; }
        public CanDLC DLC { get; set; }
        public int CycleTime { get; set; }
        public string Comment { get; set; }
        public List<DbcSignal> Signals { get; set; } = new List<DbcSignal>();

        public object Clone()
        {
            var obj = new DbcMessage();
            obj.ID = ID;
            obj.Name = Name;
            obj.DLC = DLC;
            obj.CycleTime = CycleTime;
            obj.Comment = Comment;
            foreach (var signal in Signals)
            {
                obj.Signals.Add(signal.Clone() as DbcSignal);
            }
            return obj;
        }
    }
}
