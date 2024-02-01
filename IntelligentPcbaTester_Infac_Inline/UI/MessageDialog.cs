using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class MessageDialog : Form
    {
        public MessageDialog()
        {
            InitializeComponent();

            contentLabel.Text = "";
        }

        private void Config(string content, string title = "", bool showOkButton = true, bool showCancelButton = false)
        {
            contentLabel.Text = content;
            Text = title;
            if (!showOkButton)
            {
                okButton.Visible = false;
                int okColumn = buttonTLPanel.GetColumn(okButton);
                buttonTLPanel.ColumnStyles[okColumn].Width = 0;
            }
            if (!showCancelButton)
            {
                cancelButton.Visible = false;
                int cancelColumn = buttonTLPanel.GetColumn(cancelButton);
                buttonTLPanel.ColumnStyles[cancelColumn].Width = 0;
            }
            if (!showOkButton && !showCancelButton)
            {
                buttonTLPanel.Visible = false;
            }
        }

        /// <summary>
        /// Modal 대화상자를 표시한다.
        /// </summary>
        /// <param name="content">보여줄 내용.</param>
        /// <param name="title">대화상자 제목.</param>
        /// <param name="showOkButton">OK 버튼 표시 여부.</param>
        /// <param name="showCancelButton">Cancel 버튼 표시 여부.</param>
        /// <returns></returns>
        internal static DialogResult ShowModal(string content, string title = "", bool showOkButton = true, bool showCancelButton = false)
        {
            MessageDialog dlg = new MessageDialog();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.Config(content, title, showOkButton, showCancelButton);
            return dlg.ShowDialog();
        }

        /// <summary>
        /// Modeless 대화상자를 표시한다.
        /// </summary>
        /// <param name="content">보여줄 내용.</param>
        /// <param name="title">대화상자 제목.</param>
        /// <param name="showOkButton">OK 버튼 표시 여부.</param>
        /// <param name="showCancelButton">Cancel 버튼 표시 여부.</param>
        /// <returns></returns>
        internal static MessageDialog ShowModeless(string content, string title = "", bool showOkButton = true, bool showCancelButton = false)
        {
            MessageDialog dlg = new MessageDialog();
            dlg.StartPosition = FormStartPosition.CenterScreen;
            dlg.Config(content, title, showOkButton, showCancelButton);
            dlg.Show();
            return dlg;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
            Close();
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
            Close();
        }
    }
}
