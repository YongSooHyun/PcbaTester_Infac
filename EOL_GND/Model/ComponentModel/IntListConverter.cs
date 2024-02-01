using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class IntListConverter : CollectionConverter
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
                var words = stringValue.Split(new string[] { "," }, StringSplitOptions.RemoveEmptyEntries);
                var parsedList = new BindingList<int>();
                foreach (var word in words)
                {
                    if (int.TryParse(word, out int parsedInt))
                    {
                        parsedList.Add(parsedInt);
                    }
                    else
                    {
                        throw new Exception("Invalid format.");
                    }
                }
                return parsedList;
            }

            return base.ConvertFrom(context, culture, value);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destinationType)
        {
            if (value is IEnumerable<int> intList && destinationType == typeof(string))
            {
                return string.Join(", ", intList);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
