using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Threading;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class FidScanForm : Form
    {
        /// <summary>
        /// 바코드를 스캔하여 읽은 Fixture ID.
        /// </summary>
        internal int Fid { get; private set; } = 0;

        // FID 스캔에 이용되는 바코드 스캐너.
        private readonly FidScanner scanner = new FidScanner();

        // 사용자가 Cancel 버튼을 눌렀는지 여부.
        private bool userCancelled = false;

        public FidScanForm()
        {
            InitializeComponent();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 바코드 스캔 대기.
            ThreadPool.QueueUserWorkItem(DoScan);
        }

        private void DoScan(object state)
        {
            try
            {
                scanner.Open();
            bottomFidScan:
                // 하단 Fixture ID 스캔.
                Utils.InvokeIfRequired(this, () =>
                {
                    promptLabel.Text = "하단 픽스처 바코드를 스캔하세요.";
                });
                string bottomBarcode = scanner.ReadBarcode();
                Utils.InvokeIfRequired(this, () =>
                {
                    barcodeTextBox.Text = bottomBarcode;
                });
                int bottomFid = MainViewModel.ExtractFid(bottomBarcode, out char bottomClassifier);
                if (bottomClassifier != MainViewModel.BottomFidClassifier)
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show("하단 픽스처 바코드가 아닙니다.", "바코드 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    });
                    goto bottomFidScan;
                }

            topFidScan:
                // 상단 Fixture ID 스캔.
                Utils.InvokeIfRequired(this, () =>
                {
                    promptLabel.Text = "상단 픽스처 바코드를 스캔하세요.";
                });
                string topBarcode = scanner.ReadBarcode();
                Utils.InvokeIfRequired(this, () =>
                {
                    barcodeTextBox.Text = topBarcode;
                });
                int topFid = MainViewModel.ExtractFid(topBarcode, out char topClassifier);
                if (topClassifier != MainViewModel.TopFidClassifier)
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show("상단 픽스처 바코드가 아닙니다.", "바코드 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    });
                    goto topFidScan;
                }

            maskFidScan:
                // 마스크 Fixture ID 스캔.
                Utils.InvokeIfRequired(this, () =>
                {
                    promptLabel.Text = "마스크 바코드를 스캔하세요.";
                });
                string maskBarcode = scanner.ReadBarcode();
                Utils.InvokeIfRequired(this, () =>
                {
                    barcodeTextBox.Text = topBarcode;
                });
                int maskFid = MainViewModel.ExtractFid(maskBarcode, out char maskClassifier);
                if (maskClassifier != MainViewModel.MaskFidClassifier)
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show("마스트 바코드가 아닙니다.", "바코드 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    });
                    goto maskFidScan;
                }

                // 하단, 상단 FID 일치 여부 검사.
                if (bottomFid != topFid || bottomFid != maskFid)
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show(
                            $"FID가 일치하지 않습니다.{Environment.NewLine}상단 = {topFid}, 하단 = {bottomFid}, 마스크 = {maskFid}",
                            "FID 불일치", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                    });
                    goto bottomFidScan;
                }
                else
                {
                    Fid = bottomFid;
                    DialogResult = DialogResult.OK;
                }
            }
            catch(Exception e)
            {
                if (!userCancelled)
                {
                    Logger.LogError($"FID Read: {e.Message}");
                    Utils.InvokeIfRequired(this, () =>
                    {
                        InformationBox.Show(e.Message, "FID 읽기 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                        DialogResult = DialogResult.Cancel;
                    });
                }
            }
            finally
            {
                scanner.Close();
            }
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            if (e.CloseReason == CloseReason.UserClosing)
            {
                userCancelled = true;
            }
            scanner.Close();

            base.OnFormClosing(e);
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            userCancelled = true;
        }
    }
}
