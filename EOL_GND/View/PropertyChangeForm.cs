using BrightIdeasSoftware;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class PropertyChangeForm : Form
    {
        /// <summary>
        /// 표시할 칼럼 리스트.
        /// </summary>
        public List<string> PropertyNames { get; set; }

        /// <summary>
        /// 입력한 프로퍼티 이름.
        /// </summary>
        public string SelectedName { get; private set; }

        /// <summary>
        /// 입력한 내용.
        /// </summary>
        public string Value { get; private set; }

        public PropertyChangeForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            propertyComboBox.DataSource = PropertyNames;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            propertyComboBox.Text = "";
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            SelectedName = propertyComboBox.Text;
            Value = valueTextBox.Text;

            base.OnFormClosing(e);
        }
    }
}
