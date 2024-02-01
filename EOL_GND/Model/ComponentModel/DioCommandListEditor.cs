using EOL_GND.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace EOL_GND.Model.ComponentModel
{
    public class DioCommandListEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null && context.Instance is EolDioStep dioStep)
            {
                // 디바이스 명령 리스트 가져오기.
                var settingsManager = DeviceSettingsManager.SharedInstance;
                var dioSetting = settingsManager.FindSetting(Device.DeviceCategory.DIO, dioStep.DeviceName) as DioDeviceSetting;
                var viewModel = new DioCommandsViewModel(dioSetting.Commands);
                viewModel.CheckedCommands = dioStep.Commands.Select(cmd => cmd.Command).ToList();
                var dialog = new View.DioCommandsForm(viewModel, false);
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    var dioCommands = new BindingList<EolDioStep.DioCommand>();
                    foreach (var checkedCommand in dialog.CheckedCommands)
                    {
                        dioCommands.Add(new EolDioStep.DioCommand { Command= checkedCommand });
                    }
                    return dioCommands;
                }
                else
                {
                    return value;
                }
            }

            return base.EditValue(context, provider, value);
        }
    }
}
