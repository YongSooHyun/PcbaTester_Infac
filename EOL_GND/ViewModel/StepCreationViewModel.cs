using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.ViewModel
{
    internal class StepCreationViewModel
    {
        internal static StepCategoryInfo[] GetCategoryInfos()
        {
            return StepCategoryInfo.GetCategoryInfos();
        }

        internal static string CategoryImageGetter(object rowObject)
        {
            if (rowObject is StepCategoryInfo info)
            {
                return info.ImageName;
            }
            else
            {
                return null;
            }
        }
    }
}
