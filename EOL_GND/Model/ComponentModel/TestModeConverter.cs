using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class TestModeConverter : TypeConverter
    {
        public override bool GetStandardValuesSupported(ITypeDescriptorContext context)
        {
            if (context.Instance is EolStep)
            {
                return true;
            }
            else
            {
                return base.GetStandardValuesSupported(context);
            }
        }

        public override bool GetStandardValuesExclusive(ITypeDescriptorContext context)
        {
            return true;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            var step = context.Instance as EolStep;
            var testModes = step.GetTestModes();
            return new StandardValuesCollection(testModes);
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type sourceType)
        {
            if (context.Instance is EolStep && sourceType == typeof(string))
            {
                return true;
            }

            return base.CanConvertFrom(context, sourceType);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            if (context.Instance is EolStep step && step.TryParseTestMode(value, out object parsed))
            {
                return parsed;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
