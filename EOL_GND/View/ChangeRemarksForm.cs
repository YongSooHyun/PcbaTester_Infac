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
    public partial class ChangeRemarksForm : Form
    {
        /// <summary>
        /// 사용자가 입력한 텍스트.
        /// </summary>
        public string EnteredText => textBox.Text;

        public ChangeRemarksForm()
        {
            InitializeComponent();
        }
    }
}
