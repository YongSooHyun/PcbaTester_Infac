using EOL_GND.ViewModel;
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
            pageTreeView.BeginUpdate();

            // Genenral settings.
            var generalNode = pageTreeView.Nodes.Add("General");
            generalNode.Tag = new GeneralSettingsForm();

            // CAN messages and signals.
            var canMessageNode = pageTreeView.Nodes.Add("CAN Messages");
            canMessageNode.Tag = new DbcEditorForm();

            // Device general settings.
            var deviceNode = pageTreeView.Nodes.Add("Device");
            deviceNode.Tag = new DeviceSettingsForm();

            // Power settings.
            var powerNode = deviceNode.Nodes.Add("Power");
            powerNode.Tag = new PowerDeviceSettingsForm();

            // Waveform generator settings.
            var waveformGeneratorNode = deviceNode.Nodes.Add("Waveform Generator");
            waveformGeneratorNode.Tag = new WaveformGenSettingsForm();

            // DMM settings.
            var dmmNode = deviceNode.Nodes.Add("DMM");
            dmmNode.Tag = new DmmDeviceSettingsForm();

            // Oscilloscope settings.
            var oscopeNode = deviceNode.Nodes.Add("Oscilloscope");
            oscopeNode.Tag = new OscopeDeviceSettingsForm();

            // Amplifier settings.
            var ampNode = deviceNode.Nodes.Add("Amplifier");
            ampNode.Tag = new AmplifierDeviceSettingsForm();

            // DIO settings.
            var dioNode = deviceNode.Nodes.Add("DIO");
            dioNode.Tag = new DioDeviceSettingsForm();

            // CAN device settings.
            var canNode = deviceNode.Nodes.Add("CAN");
            canNode.Tag = new CanDeviceSettingsForm();

            // LIN device settings.
            var linNode = deviceNode.Nodes.Add("LIN");
            linNode.Tag = new LinDeviceSettingsForm();

            // Gloquad SECC device settings.
            //var gloquadSeccNode = deviceNode.Nodes.Add("Gloquad SECC");
            //gloquadSeccNode.Tag = new GloquadSeccDeviceSettingsForm();

            // Serial port settings.
            var serialPortNode = deviceNode.Nodes.Add("Serial Port");
            serialPortNode.Tag = new SerialPortSettingsForm();

            // User management.
            var permission = GeneralSettingsViewModel.GetUserPermission();
            if (permission?.CanManageUsers ?? false)
            {
                var userManagementNode = pageTreeView.Nodes.Add("User Management");
                userManagementNode.Tag = new UserEditorForm();
            }

            // Change password.
            if (permission != null)
            {
                var passwordChangeNode = pageTreeView.Nodes.Add("Change Password");
                passwordChangeNode.Tag = new PasswordChangeSettingsForm();
            }

            deviceNode.Expand();
            pageTreeView.SelectedNode = pageTreeView.Nodes[0];

            pageTreeView.EndUpdate();
        }

        private void pageTreeView_AfterSelect(object sender, TreeViewEventArgs e)
        {
            if (pageTreeView.SelectedNode?.Tag is Form selectedPage)
            {
                selectedPage.TopLevel = false;
                splitContainer.Panel2.Controls.Clear();
                splitContainer.Panel2.Controls.Add(selectedPage);
                selectedPage.FormBorderStyle = FormBorderStyle.None;
                selectedPage.Dock = DockStyle.Fill;
                selectedPage.Show();

                if (selectedPage is DmmDeviceSettingsForm dmmForm)
                {
                    dmmForm.RefreshObjects();
                }
                else if (selectedPage is OscopeDeviceSettingsForm oscopeForm)
                {
                    oscopeForm.RefreshObjects();
                }
                else if (selectedPage is WaveformGenSettingsForm wgenForm)
                {
                    wgenForm.RefreshObjects();
                }
            }
            else
            {
                splitContainer.Panel2.Controls.Clear();
            }
        }
    }
}
