using GitHub.secile.Video;
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
    public partial class CameraViewer : Form
    {
        public CameraViewer()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Camera sources.
            var devices = UsbCamera.FindDevices();
            sourceItem.Enabled = devices.Length > 0;
            for (int i = 0; i < devices.Length; i++)
            {
                var cameraItem = sourceItem.DropDownItems.Add($"{i+1}. {devices[i]}");
                cameraItem.Click += (sender, ev) => {
                    if (((ToolStripMenuItem)cameraItem).Checked && Tag is UsbCamera checkedCamera)
                    {
                        checkedCamera.Stop();
                        ((ToolStripMenuItem)cameraItem).Checked = false;
                        return;
                    }

                    var camera = cameraItem.Tag as UsbCamera;
                    if (camera == null)
                    {
                        var formats = UsbCamera.GetVideoFormat(i);
                        camera = new UsbCamera(i, formats[0]);
                        FormClosing += (s, evt) => camera.Release();

                        camera.SetPreviewControl(Handle, ClientSize);
                        Resize += (s, evt) => camera.SetPreviewSize(ClientSize);
                    }

                    // 이미 보여주던 카메라 정지.
                    if (Tag is UsbCamera oldCamera)
                    {
                        oldCamera.Stop();
                    }
                    Tag = camera;
                    camera.Start();

                    ((ToolStripMenuItem)cameraItem).Checked = true;

                    // 다른 모든 
                };
            }
        }
    }
}
