using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public static class ObjectListViewExtensions
    {
        /// <summary>
        /// <see cref="ObjectListView"/>의 행 높이를 폰트 크기에 맞게 조절한다.
        /// </summary>
        /// <param name="listView"></param>
        public static void AdjustRowHeightByFont(this ObjectListView listView)
        {
            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = listView.Font;
            listView.Controls.Add(dummyComboBox);
            listView.RowHeight = dummyComboBox.Height;
            listView.Controls.Remove(dummyComboBox);
        }
    }
}
