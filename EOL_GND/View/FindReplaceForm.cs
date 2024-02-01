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
    public partial class FindReplaceForm : Form
    {
        internal string Pattern { get; set; }
        internal BindingList<string> SearchedPatterns { get; set; }
        internal bool IgnoreCase { get; set; } = true;

        public FindReplaceForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            patternComboBox.DataSource = SearchedPatterns?.ToList();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // Center parent if this form is shown as modeless.
            if (!Modal && Owner != null && StartPosition == FormStartPosition.CenterParent)
            {
                var x = (Owner.Width - Width) / 2;
                var y = (Owner.Height - Height) / 2;
                Location = new Point(Owner.Location.X + x, Owner.Location.Y + y);
            }

            patternComboBox.Text = Pattern;
            matchCaseCheckBox.Checked = !IgnoreCase;
            UpdateButtons();
        }

        protected override void OnActivated(EventArgs e)
        {
            base.OnActivated(e);

            patternComboBox.Focus();
        }

        private void findPrevButton_Click(object sender, EventArgs e)
        {
            if (Owner is SequenceForm parent)
            {
                parent.Find(false);
                patternComboBox.DataSource = SearchedPatterns?.ToList();
            }
        }

        private void findNextButton_Click(object sender, EventArgs e)
        {
            if (Owner is SequenceForm parent)
            {
                parent.Find(true);
                patternComboBox.DataSource = SearchedPatterns?.ToList();
            }
        }

        private void closeButton_Click(object sender, EventArgs e)
        {
            Close();
        }

        private void patternComboBox_TextChanged(object sender, EventArgs e)
        {
            UpdateButtons();
            if (Owner is SequenceForm parent)
            {
                parent.SetSearchPattern(patternComboBox.Text);
            }
        }

        private void UpdateButtons()
        {
            var pattern = patternComboBox.Text;
            if (string.IsNullOrEmpty(pattern))
            {
                findPrevButton.Enabled = false;
                findNextButton.Enabled = false;
            }
            else
            {
                findPrevButton.Enabled = true;
                findNextButton.Enabled = true;
            }
        }

        private void matchCaseCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            if (Owner is SequenceForm parent)
            {
                parent.SetSearchIgnoreCase(!matchCaseCheckBox.Checked);
            }
        }
    }
}
