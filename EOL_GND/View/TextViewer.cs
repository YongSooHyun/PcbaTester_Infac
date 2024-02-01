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
    public partial class TextViewer : Form
    {
        public string Content
        {
            get => textBox.Text;
            set => textBox.Text = value?.Replace("\n", "\r\n");
        }

        public TextViewer()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            textBox.SelectionLength = 0;
        }
    }
}
