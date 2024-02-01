using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class ProbeCountEditForm : Form
    {
        public int Fid { get; set; }

        public ProbeCountEditForm()
        {
            InitializeComponent();

            Init();
        }

        private void Init()
        {
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            fidNumericUpDown.Value = Fid;
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            try
            {
                int todayTotal = (int)todayNumericUpDown.Value;
                int todayPassed = (int)todayPassedNumericUpDown.Value;
                int total = (int)totalNumericUpDown.Value;
                int totalPassed = (int)totalPassedNumericUpDown.Value;
                if (todayPassed > todayTotal || totalPassed > total)
                {
                    InformationBox.Show("Tested 값은 Passed 값과 같거나 커야 합니다.", "입력 오류", icon: InformationBoxIcon.Error);
                    DialogResult = DialogResult.None;
                    return;
                }

                int fid = (int)fidNumericUpDown.Value;
                var fixtureProbeCount = MainViewModel.GetProbeCount(fid);
                fixtureProbeCount.TodayTestCount = todayTotal;
                fixtureProbeCount.TodayPassCount = todayPassed;
                fixtureProbeCount.TotalProbeCount = total;
                MainViewModel.SaveProbeCount(fixtureProbeCount);
            }
            catch (Exception ex)
            {
                InformationBox.Show(ex.Message, "오류", icon: InformationBoxIcon.Error);
                DialogResult = DialogResult.None;
            }
        }

        private void totalNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTodayRatio();
        }

        private void passedNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTodayRatio();
        }

        private void UpdateTodayRatio()
        {
            int total = (int)todayNumericUpDown.Value;
            int passed = (int)todayPassedNumericUpDown.Value;
            float ratio = 0;
            if (total != 0)
            {
                ratio = (float)passed / total * 100;
            }
            todayRatioLabel.Text = $"{ratio:0.0} %";
        }

        private void fidNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            // Fixture Probe Count 가져오기.
            int fid = (int)fidNumericUpDown.Value;
            var fixtureProbeCount = MainViewModel.GetProbeCount(fid);
            todayNumericUpDown.Value = fixtureProbeCount.TodayTestCount;
            todayPassedNumericUpDown.Value = fixtureProbeCount.TodayPassCount;
            totalNumericUpDown.Value = fixtureProbeCount.TotalProbeCount;
        }

        private void totalNumericUpDown_ValueChanged_1(object sender, EventArgs e)
        {
            UpdateTotalRatio();
        }

        private void totalPassedNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateTotalRatio();
        }

        private void UpdateTotalRatio()
        {
            int total = (int)totalNumericUpDown.Value;
            int passed = (int)totalPassedNumericUpDown.Value;
            float ratio = 0;
            if (total != 0)
            {
                ratio = (float)passed / total * 100;
            }
            totalRatioLabel.Text = $"{ratio:0.0} %";
        }
    }
}
