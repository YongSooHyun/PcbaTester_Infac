using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Runtime.Remoting.Contexts;
using System.Text;
using System.Threading.Tasks;
using static EOL_GND.Model.EolDioStep;

namespace EOL_GND.Model.ComponentModel
{
    public class DioCommandListConverter : CollectionConverter
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
            if (destinationType == typeof(string) && value is IEnumerable<EolDioStep.DioCommand> dioCommands)
            {
                return string.Join(", ", dioCommands.Select(dioCmd => dioCmd.Command));
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
            var stringValue = value as string;
            if (stringValue != null)
            {
                // 변환한 명령 리스트.
                var dioCommands = new BindingList<DioCommand>();

                // DIO 디바이스 설정 찾기.
                var dioStep = context.Instance as EolDioStep;
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var dioSetting = settingsManager.FindSetting(Device.DeviceCategory.DIO, dioStep?.DeviceName) as DioDeviceSetting;
                if (dioSetting.Commands.Count == 0)
                {
                    return dioCommands;
                }

                // 문자열을 ','로 분리하고 매 명령이 설정에 있는지 체크.
                var commands = stringValue.Split(',');
                foreach (var cmd in commands)
                {
                    var command = cmd.Trim();

                    // 명령을 명령 리스트에서 찾기.
                    var found = dioSetting.Commands.Where(cmdInfo => command.Equals(cmdInfo.Command, StringComparison.OrdinalIgnoreCase));
                    if (found != null && found.Any())
                    {
                        dioCommands.Add(new DioCommand { Command = found.First().Command });
                    }
                }

                return dioCommands;
            }

            return base.ConvertFrom(context, culture, value);
        }
    }
}
