using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class HexByteArrayConverter : TypeConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destinationType)
        {
            if (destinationType == typeof(string))
            {
                return true;
            }

            return base.CanConvertTo(context, destinationType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                var words = stringValue.Split();
                var parsedList = new List<byte>();
                foreach (var word in words)
                {
                    if (string.IsNullOrWhiteSpace(word))
                    {
                        continue;
                    }

                    parsedList.Add(Convert.ToByte(word, 16));
                }
                return parsedList.ToArray();
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is IEnumerable<byte> byteList && destinationType == typeof(string))
            {
                return string.Join(" ", byteList.Select(b => $"{b:X2}"));
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
