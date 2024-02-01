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
    public partial class StepIdForm : Form
    {
        internal int Id
        {
            get => (int)idNUDown.Value;
            set => idNUDown.Value = value;
        }

        public StepIdForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            idNUDown.Select(0, idNUDown.Text.Length);
        }
    }
}
