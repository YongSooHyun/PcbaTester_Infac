using Menees.Diffs;
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
    public partial class DiffForm : Form
    {
        public string OldText { get; set; }
        public DateTime OldTime { get; set; }
        public string NewText { get; set; }
        public DateTime NewTime { get; set; }

        public DiffForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Split a string into lines.
            var newLineSeparators = new[] { "\n", "\r\n" };
            var oldTextLines = (OldText ?? "").Split(newLineSeparators, StringSplitOptions.None);
            var newTextLines = (NewText ?? "").Split(newLineSeparators, StringSplitOptions.None);

            // Calc diff.
            var diff = new TextDiff(HashType.HashCode, false, false, 0, true);
            var script = diff.Execute(oldTextLines, newTextLines);

            // Show diff.
            diffControl1.SetData(oldTextLines, newTextLines, script, OldTime.ToString(), NewTime.ToString());
        }
    }
}
