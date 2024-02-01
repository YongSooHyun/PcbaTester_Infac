using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class SettingsForm : Form
    {
        public SettingsForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            InitPages();
        }

        private void InitPages()
        {
            var pages = new List<Form>();
            pages.Add(new GeneralSettingsForm());
            pages.Add(new PathSettingsForm());

            var userPermission = AppSettings.LoggedUser?.GetPermission();
            if (userPermission?.CanEditProdcut ?? true)
            {
                pages.Add(new ProductSettingsForm());
            }
            if (userPermission?.CanEditComPorts ?? true)
            {
                pages.Add(new ComportSettingsForm());
            }
            pages.Add(new NovaflashSettingsForm());
            pages.Add(new JtagSettingsForm());
            pages.Add(new MesSettingsForm());
            if (userPermission?.CanManageUsers ?? true)
            {
                pages.Add(new UserSettingsForm());
            }
            pages.Add(new SeriesSettingsForm());
            //pages.Add(new CrcSettingsForm());
            pages.Add(new PasswordSettingsForm());

            pagesTreeView.BeginUpdate();
            foreach (var page in pages)
            {
                var node = pagesTreeView.Nodes.Add(page.Text);
                node.Tag = page;
            }
            pagesTreeView.EndUpdate();
            pagesTreeView.SelectedNode = pagesTreeView.Nodes[0];
        }

        private void pagesTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (pagesTreeView.SelectedNode?.Tag is Form selectedPage)
            {
                selectedPage.TopLevel = false;
                mainSplitContainer.Panel2.Controls.Clear();
                mainSplitContainer.Panel2.Controls.Add(selectedPage);
                selectedPage.FormBorderStyle = FormBorderStyle.None;
                selectedPage.Dock = DockStyle.Fill;
                selectedPage.Show();
            }
        }
    }
}
