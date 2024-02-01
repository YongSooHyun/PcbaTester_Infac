using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    public class ParameterNameConverter : TypeConverter
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
            List<string> values = null;
            switch (context.Instance)
            {
                case EolWaveformGeneratorStep wgStep:
                    values = wgStep.GetParameterNames();
                    break;
                case EolDmmStep dmmStep:
                    values = dmmStep.GetParameterNames();
                    break;
            }

            return new StandardValuesCollection(values);
        }
    }
}
