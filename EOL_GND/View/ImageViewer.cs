using EOL_GND.Common;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Drawing.Imaging;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class ImageViewer : Form
    {
        /// <summary>
        /// Gets or sets the image that is displayed.
        /// </summary>
        public Image Image
        {
            get => pictureBox.Image;
            set => pictureBox.Image = value;
        }

        /// <summary>
        /// Indicates how the image is displayed.
        /// </summary>
        public PictureBoxSizeMode SizeMode
        {
            get => pictureBox.SizeMode;
            set => pictureBox.SizeMode = value;
        }

        /// <summary>
        /// 윈도우 크기를 조정하는 방식.
        /// </summary>
        public static OscopeDisplaySettings.DisplayWindowSize DisplaySizeMode { get; set; } = OscopeDisplaySettings.DisplayWindowSize.RestoreLastState;

        /// <summary>
        /// 사용자 설정 윈도우 위치.
        /// </summary>
        public static Point DisplayLocation { get; set; }

        /// <summary>
        /// 사용자 설정 윈도우 크기.
        /// </summary>
        public static Size DisplaySize { get; set; }

        /// <summary>
        /// 윈도우가 자동으로 꺼질 때까지 시간(ms).
        /// </summary>
        public static int AutoCloseDelay { get; set; }

        // 이미지 뷰어 하나만 보여주기 위한 변수.
        public static ImageViewer SharedViewer { get; private set; }

        public ImageViewer()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 이미지 또는 설정한 크기에 맞게 폼 크기 조정.
            ResizeToMatch();
        }

        // 이미지 또는 설정한 크기에 맞게 폼 크기 조정.
        private void ResizeToMatch()
        {
            int width, height;
            switch (DisplaySizeMode)
            {
                case OscopeDisplaySettings.DisplayWindowSize.RestoreLastState:
                    // 마지막 윈도우 상태 복원.
                    if (SharedViewer != null)
                    {
                        return;
                    }

                    if (GeneralSettingsViewModel.ImageViewerSize == Size.Empty)
                    {
                        GeneralSettingsViewModel.ImageViewerSize = new Size(Image?.Width ?? 0, Image?.Height ?? 0);
                    }

                    Utils.SetWindowState(this, GeneralSettingsViewModel.ImageViewerState, GeneralSettingsViewModel.ImageViewerLocation,
                        GeneralSettingsViewModel.ImageViewerSize);
                    return;
                case OscopeDisplaySettings.DisplayWindowSize.SpecifiedLocationAndSize:
                    width = DisplaySize.Width;
                    height = DisplaySize.Height;
                    break;
                case OscopeDisplaySettings.DisplayWindowSize.SameAsImageSize:
                default:
                    width = Image?.Width ?? 0;
                    height = Image?.Height ?? 0;
                    break;
            }

            int widthDelta = width - pictureBox.Width;
            int heightDelta = height - pictureBox.Height;

            WindowState = FormWindowState.Normal;
            if (DisplaySizeMode == OscopeDisplaySettings.DisplayWindowSize.SpecifiedLocationAndSize)
            {
                Location = DisplayLocation;
            }
            else if (SharedViewer == null)
            {
                Location = GeneralSettingsViewModel.ImageViewerLocation;
            }
            Size = new Size(Size.Width + widthDelta, Size.Height + heightDelta);
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            // 윈도우 상태, 위치, 크기 저장.
            GeneralSettingsViewModel.ImageViewerState = WindowState;
            GeneralSettingsViewModel.ImageViewerLocation = Location;
            GeneralSettingsViewModel.ImageViewerSize = Size;

            // 표시된 뷰어 제거.
            SharedViewer = null;

            base.OnFormClosing(e);
        }

        /// <summary>
        /// 지정한 이미지를 보여주는 폼을 보여줍니다.
        /// </summary>
        /// <param name="parent"></param>
        /// <param name="image"></param>
        /// <param name="title"></param>
        /// <param name="resizeToFit"></param>
        /// <param name="sizeMode"></param>
        /// <returns></returns>
        public static void Show(Form parent, Image image, string title, PictureBoxSizeMode sizeMode = PictureBoxSizeMode.Zoom)
        {
            if (SharedViewer == null)
            {
                var viewer = new ImageViewer
                {
                    Text = title,
                    Image = image,
                    SizeMode = sizeMode,
                };
                viewer.Show(parent);
                SharedViewer = viewer;
            }
            else
            {
                SharedViewer.Text = title;
                SharedViewer.Image = image;
                SharedViewer.SizeMode = sizeMode;
                SharedViewer.ResizeToMatch();
            }

            if (AutoCloseDelay > 0)
            {
                Task.Delay(AutoCloseDelay).ContinueWith(t => SharedViewer?.Close());
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            copyItem.Enabled = pictureBox.Image != null;
            saveAsItem.Enabled = pictureBox.Image != null;
        }

        private void copyToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null)
            {
                return;
            }

            try
            {
                Clipboard.SetDataObject(pictureBox.Image, true);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot copy an image to clipboard: {ex.Message}");
            }
        }

        private void saveAsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (pictureBox.Image == null)
            {
                return;
            }

            try
            {
                var dialog = new SaveFileDialog();
                dialog.RestoreDirectory = true;

                // Filter.
                string firstFilter;
                var imageFormat = pictureBox.Image.RawFormat;
                if (imageFormat.Equals(ImageFormat.Png))
                {
                    firstFilter = "PNG Files (*.png)|*.png;|";
                }
                else if (imageFormat.Equals(ImageFormat.Bmp))
                {
                    firstFilter = "Bitmap Files (*.bmp)|*.bmp;|";
                }
                else if (imageFormat.Equals(ImageFormat.Jpeg))
                {
                    firstFilter = "JPEG Files (*.jpeg;*.jpg)|*.jpeg;*.jpg;|";
                }
                else if (imageFormat.Equals(ImageFormat.Gif))
                {
                    firstFilter = "GIF Files (*.gif)|*.gif;|";
                }
                else
                {
                    firstFilter = "";
                }

                dialog.Filter = $"{firstFilter}All Files (*.*)|*.*;";
                dialog.OverwritePrompt = true;

                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    pictureBox.Image.Save(dialog.FileName);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }
    }
}
