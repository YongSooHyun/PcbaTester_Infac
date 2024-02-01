using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.ComponentModel.Design;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.Model.ComponentModel
{
    /// <summary>
    /// Test Channel들을 편집한다. 중복된 Channel들을 제거한다.
    /// </summary>
    public class TestChannelEditor : CollectionEditor
    {
        public TestChannelEditor(Type type) : base(type)
        {
        }

        public override object EditValue(ITypeDescriptorContext context, IServiceProvider provider, object value)
        {
            var editedValue = base.EditValue(context, provider, value);
            if (editedValue is BindingList<int> bindingList)
            {
                var removed = new BindingList<int>(bindingList.Distinct().ToList());
                return removed;
            }
            return editedValue;
        }
    }
}
