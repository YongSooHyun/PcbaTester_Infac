using EOL_GND.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND_Test
{
    public partial class MainForm : Form
    {
        public MainForm()
        {
            InitializeComponent();
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var form = new SequenceForm(null, null, false, 0, 0, 0);
            form.StartPosition = FormStartPosition.CenterScreen;
            form.ShowDialog();
        }

        private void runButton_Click(object sender, EventArgs e)
        {
            var form = new ElozRunLogForm(null, null, null);
            form.ShowDialog();
        }
    }
}
