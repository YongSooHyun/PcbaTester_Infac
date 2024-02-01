using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms.Design;

namespace EOL_GND.Model.ComponentModel
{
    public class CanMessageNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null)
            {
                var dialog = new View.DbcEditorForm();
                dialog.MessageName = value as string;
                dialog.EditingEnabled = false;
                dialog.SelectingMessages = true;
                dialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                if (dialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.SelectedMessage?.Name;
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
