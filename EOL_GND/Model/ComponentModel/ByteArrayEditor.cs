using EOL_GND.ViewModel;
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
    public class ByteArrayEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editorService = (IWindowsFormsEditorService)provider.GetService(typeof(IWindowsFormsEditorService));
            if (editorService != null && (value is byte[] || value == null))
            {
                var editorDialog = new View.ByteArrayEditorForm();
                var byteArray = value as byte[];
                if (byteArray == null)
                {
                    byteArray = new byte[0];
                }
                editorDialog.Data = byteArray;
                editorDialog.StartPosition = System.Windows.Forms.FormStartPosition.CenterParent;
                if (editorDialog.ShowDialog() == System.Windows.Forms.DialogResult.OK)
                {
                    return editorDialog.Data;
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
