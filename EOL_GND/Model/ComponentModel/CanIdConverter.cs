using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class CanIdConverter : TypeConverter
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
            if (destinationType == typeof(string) && value is uint canId)
            {
                if (context.Instance is EolCanStep canStep)
                {
                    if (canStep.ExtendedFrame)
                    {
                        return $"{canId:X8}";
                    }
                    else
                    {
                        return $"{canId:X3}";
                    }
                }

                return $"{canId:X}";
            }

            return base.ConvertTo(context, culture, value, destinationType);
        }

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
            if (context.Instance is EolCanStep canStep && value is string textValue)
            {
                var parsedValue = Convert.ToUInt32(textValue, 16);
                if (canStep.ExtendedFrame)
                {
                    if (parsedValue > 0x1F_FF_FF_FF)
                    {
                        parsedValue = 0x1F_FF_FF_FF;
                    }
                }
                else
                {
                    if (parsedValue > 0x7FF)
                    {
                        parsedValue = 0x7FF;
                    }
                }
                return parsedValue;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
