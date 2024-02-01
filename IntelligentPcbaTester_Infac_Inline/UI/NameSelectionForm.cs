using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class NameSelectionForm : Form
    {
        /// <summary>
        /// 리스트에 보여줄 항목들.
        /// </summary>
        private List<string> SourceList = null;

        /// <summary>
        /// 사용자가 선택한 항목.
        /// </summary>
        public string Selected = null;

        public NameSelectionForm(List<string> items)
        {
            SourceList = items;

            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            listBox.DataSource = SourceList;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            Selected = listBox.SelectedItem?.ToString();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            Selected = null;
        }
    }
}
