using Be.Windows.Forms;
using EOL_GND.Common;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class ByteArrayEditorForm : Form
    {
        public byte[] Data;

        public ByteArrayEditorForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            if (Data != null)
            {
                hexBox.ByteProvider = new DynamicByteProvider(Data);
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            var byteProvider = hexBox.ByteProvider as DynamicByteProvider;
            if (byteProvider != null)
            {
                Data = byteProvider.Bytes?.ToArray();
            }

            base.OnFormClosing(e);
        }

        private void importFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                var openDialog = new OpenFileDialog();
                openDialog.Title = "Import File";
                openDialog.Multiselect = false;
                openDialog.RestoreDirectory = true;
                if (openDialog.ShowDialog() == DialogResult.OK)
                {
                    var bytes = File.ReadAllBytes(openDialog.FileName);
                    hexBox.ByteProvider = new DynamicByteProvider(bytes);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Byte array editor: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void exportFileButton_Click(object sender, EventArgs e)
        {
            try
            {
                var saveDialog = new SaveFileDialog();
                saveDialog.Title = "Export to File";
                saveDialog.OverwritePrompt = true;
                saveDialog.RestoreDirectory = true;
                if (saveDialog.ShowDialog() == DialogResult.OK)
                {
                    var byteProvider = hexBox.ByteProvider as DynamicByteProvider;
                    byte[] bytes = byteProvider?.Bytes?.Count > 0 ? byteProvider.Bytes.ToArray() : new byte[0];
                    File.WriteAllBytes(saveDialog.FileName, bytes);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Byte array editor: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }
    }
}
