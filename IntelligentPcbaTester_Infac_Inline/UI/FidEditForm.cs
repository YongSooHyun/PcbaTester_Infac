using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class FidEditForm : Form
    {
        /// <summary>
        /// 편집하려는 FID 리스트.
        /// </summary>
        public BindingList<int> FidList { get; set; }

        public FidEditForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitList();
        }

        // FID 리스트를 표시한다.
        private void InitList()
        {
            if (FidList != null)
            {
                fidListBox.DataSource = FidList;
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            int addingValue = (int)fidNUDown.Value;
            if (FidList != null && !FidList.Contains(addingValue))
            {
                FidList.Add(addingValue);
            }
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (fidListBox.SelectedIndex >= 0)
            {
                FidList.RemoveAt(fidListBox.SelectedIndex);
            }
        }

        private void removeAllButton_Click(object sender, EventArgs e)
        {
            FidList?.Clear();
        }

        private void fidListBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            removeButton.Enabled = fidListBox.SelectedIndex >= 0;
        }
    }
}
