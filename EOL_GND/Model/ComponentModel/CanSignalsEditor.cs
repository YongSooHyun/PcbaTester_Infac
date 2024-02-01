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
    public class CanSignalsEditor : UITypeEditor
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
                var dialog = new View.CanSignalsEditorForm(value as IEnumerable<object>);
                dialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                var result = dialog.ShowDialog();
                if (result == System.Windows.Forms.DialogResult.OK)
                {
                    return dialog.Signals;
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
