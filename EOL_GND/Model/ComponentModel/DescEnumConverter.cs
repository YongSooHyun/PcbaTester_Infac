using EOL_GND.Device;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    /// <summary>
    /// Enum의 Description 속성값을 표시하도록 해준다.
    /// </summary>
    public class DescEnumConverter : EnumConverter
    {
        private Type enumType;

        public DescEnumConverter(Type type) : base(type)
        {
            enumType = type;
        }

        public override StandardValuesCollection GetStandardValues(ITypeDescriptorContext context)
        {
            if (enumType == typeof(CanNominalBaudRate))
            {
                var baudRates = new CanNominalBaudRate[]
                {
                    CanNominalBaudRate.BAUD_5K,
                    CanNominalBaudRate.BAUD_10K,
                    CanNominalBaudRate.BAUD_20K,
                    CanNominalBaudRate.BAUD_33K,
                    CanNominalBaudRate.BAUD_47K,
                    CanNominalBaudRate.BAUD_50K,
                    CanNominalBaudRate.BAUD_83K,
                    CanNominalBaudRate.BAUD_95K,
                    CanNominalBaudRate.BAUD_100K,
                    CanNominalBaudRate.BAUD_125K,
                    CanNominalBaudRate.BAUD_250K,
                    CanNominalBaudRate.BAUD_500K,
                    CanNominalBaudRate.BAUD_800K,
                    CanNominalBaudRate.BAUD_1M,
                };
                return new StandardValuesCollection(baudRates);
            }
            else if (enumType == typeof(CanDLC))
            {
                if (context.Instance is EolCanStep canStep && !canStep.CAN_FD)
                {
                    var dlcs = new CanDLC[]
                    {
                        CanDLC.DLC_0, CanDLC.DLC_1, CanDLC.DLC_2, CanDLC.DLC_3, CanDLC.DLC_4, CanDLC.DLC_5, CanDLC.DLC_6, CanDLC.DLC_7, CanDLC.DLC_8,
                    };
                    return new StandardValuesCollection(dlcs);
                }
            }

            return base.GetStandardValues(context);
        }

        public override bool CanConvertTo(ITypeDescriptorContext context, Type destType)
        {
            return destType == typeof(string);
        }

        public override object ConvertTo(ITypeDescriptorContext context, CultureInfo culture, object value, Type destType)
        {
            FieldInfo fi = enumType.GetField(Enum.GetName(enumType, value));
            DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
            if (da != null)
                return da.Description;
            else
                return value.ToString();
        }

        public override bool CanConvertFrom(ITypeDescriptorContext context, Type srcType)
        {
            return srcType == typeof(string);
        }

        public override object ConvertFrom(ITypeDescriptorContext context, CultureInfo culture, object value)
        {
            foreach (FieldInfo fi in enumType.GetFields())
            {
                DescriptionAttribute da = (DescriptionAttribute)Attribute.GetCustomAttribute(fi, typeof(DescriptionAttribute));
                if ((da != null) && ((string)value == da.Description))
                    return Enum.Parse(enumType, fi.Name);
            }
            return Enum.Parse(enumType, (string)value);
        }
    }
}
