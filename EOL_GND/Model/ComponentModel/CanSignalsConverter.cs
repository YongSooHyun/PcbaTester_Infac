using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class CanSignalsConverter : TypeConverter
    {
        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                var signals = value as List<CanSignal>;
                if (signals == null || signals.Count == 0)
                {
                    return string.Empty;
                }

                return string.Join(", ", signals.Select(s => s.SignalName));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
