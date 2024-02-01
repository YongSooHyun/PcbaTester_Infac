using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Windows.Forms.Design;

namespace EOL_GND.Model.ComponentModel
{
    public class ImageFileNameEditor : UITypeEditor
    {
        public override UITypeEditorEditStyle GetEditStyle(ITypeDescriptorContext context)
        {
            return UITypeEditorEditStyle.Modal;
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            using (var dialog = new SaveFileDialog())
            {
                if (value is string fileName)
                {
                    dialog.FileName = fileName;
                }
                dialog.RestoreDirectory = true;
                dialog.Filter = "PNG & BMP Files (*.png;*bmp)|*.png;*bmp;|All Files (*.*)|*.*;";
                dialog.Title = "Save Download File";

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    return dialog.FileName;
                }
                else
                {
                    return null;
                }
            }
        }
    }
}
