using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class PhysicalValueDynamicConverter : DoubleConverter
    {
        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (value is string stringValue)
            {
                var result = EolStep.ConvertToDouble(context.PropertyDescriptor, stringValue, out double? converted);
                if (result)
                {
                    return converted;
                }
                else
                {
                    throw new NotSupportedException($"{value} is not valid for {context.PropertyDescriptor.PropertyType.Name}.");
                }
            }

            return base.ConvertFrom(context, culture, value);
        }

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
            if (destinationType == typeof(string) && context.Instance is EolStep eolStep)
            {
                return eolStep.ConvertToString(context.PropertyDescriptor, value as double?);
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }
    }
}
