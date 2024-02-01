using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class StepEditForm : Form
    {
        public object CurrentStep { get; private set; }

        private StepEditViewModel viewModel;
        private readonly System.Collections.IList stepList;
        private bool testRunning = false;
        private CancellationTokenSource tokenSource;
        private object elozTestSet = null;

        public StepEditForm(System.Collections.IList steps, object step, object testSet)
        {
            InitializeComponent();

            UpdateFont();
            runCountComboBox.SelectedIndex = 0;

            elozTestSet = testSet;
            stepList = steps;
            CurrentStep = step;
            ShowStep(step);
        }

        internal void UpdateFont()
        {
            stepPropertyGrid.Font = GeneralSettingsViewModel.EditFont;
        }

        // 지정한 스텝을 편집할 수 있도록 한다.
        private void ShowStep(object step)
        {
            try
            {
                if (viewModel != null)
                {
                    viewModel.CurrentStep.PropertyChanged -= CurrentStep_PropertyChanged;
                }

                // Property Grid 설정.
                viewModel = new StepEditViewModel(stepList, step);

                // 사용자 권한 반영.
                var permission = GeneralSettingsViewModel.GetUserPermission();
                var editAllowed = permission?.CanEditSequence ?? false;
                saveTSButton.Enabled = editAllowed;
                if (!editAllowed && viewModel.CurrentStep != null)
                {
                    TypeDescriptor.AddAttributes(viewModel.CurrentStep, new ReadOnlyAttribute(true));
                }

                stepPropertyGrid.SelectedObject = viewModel.CurrentStep;
                viewModel.CurrentStep.PropertyChanged += CurrentStep_PropertyChanged;

                // 텍스트, 이미지 보여주기.
                ShowStepInfo();

                // 타이틀.
                Text = viewModel.GetTitle();

                // 앞, 뒤 이동 버튼.
                prevStepTSButton.Enabled = viewModel.GetPreviousStep() != null;
                nextStepTSButton.Enabled = viewModel.GetNextStep() != null;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 스텝을 표현하는 텍스트, 이미지 보여주기.
        private void ShowStepInfo()
        {
            stepPictureBox.Image = viewModel.CurrentStep.GetStepImage();

            string deviceTypeName = "";
            var category = viewModel.CurrentStep.GetDeviceCategory();
            if (category != null)
            {
                try
                {
                    var settingsManager = DeviceSettingsManager.SharedInstance;
                    var deviceSetting = settingsManager.FindSetting(category ?? DeviceCategory.Power, viewModel.CurrentStep.DeviceName);
                    if (deviceSetting is SerialDeviceSetting serialSetting)
                    {
                        deviceTypeName = " - " + serialSetting.Port;
                    }
                    else
                    {
                        deviceTypeName = " - " + deviceSetting.DeviceType.GetText();
                    }
                }
                catch
                {
                }
            }
            stepCategoryLabel.Text = viewModel.CurrentStep.CategoryName + deviceTypeName;
        }

        private void CurrentStep_PropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            Utils.InvokeIfRequired(this, () =>
            {
                stepPropertyGrid.Refresh();
                Text = viewModel.GetTitle();

                // 디바이스 타입에 맞게 스텝 이미지 업데이트.
                if (e.PropertyName == nameof(Model.EolStep.DeviceName))
                {
                    ShowStepInfo();
                }
            });
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 마지막 윈도우 상태 복원.
            if (GeneralSettingsViewModel.StepEditorRestoreState)
            {
                Utils.SetWindowState(this, GeneralSettingsViewModel.StepEditorState, GeneralSettingsViewModel.StepEditorLocation,
                    GeneralSettingsViewModel.StepEditorSize);

                // 마지막 결과 리스트 뷰 상태 복원.
                var lastListViewState = GeneralSettingsViewModel.TestResultListViewState;
                if (lastListViewState != null)
                {
                    testResultListView.RestoreState(lastListViewState);
                }
            }
        }

        // Save 버튼 누를 때의 처리.
        private void saveTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                viewModel.Save();
                Text = viewModel.GetTitle();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 현재 열려있는 스텝이 변경되었으면 저장여부를 사용자에게 물어 저장한다.
        // 사용자가 Cancel 버튼을 눌렀는지 여부를 리턴한다.
        private bool SaveIfNeeded()
        {
            if (viewModel.Modified)
            {
                var result = InformationBox.Show($"변경된 내용이 있습니다.{Environment.NewLine}변경된 내용을 저장하시겠습니까?",
                    "테스트 스텝 저장", buttons: InformationBoxButtons.YesNoCancel, icon: InformationBoxIcon.Question);
                if (result == InformationBoxResult.Yes)
                {
                    viewModel.Save();
                }
                else if (result == InformationBoxResult.Cancel)
                {
                    return true;
                }
            }

            return false;
        }

        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (SaveIfNeeded())
                {
                    e.Cancel = true;
                    return;
                }

                // 윈도우 상태, 위치, 크기 저장.
                GeneralSettingsViewModel.StepEditorState = WindowState;
                GeneralSettingsViewModel.StepEditorLocation = Location;
                GeneralSettingsViewModel.StepEditorSize = Size;

                // 리스트 뷰 상태 저장.
                GeneralSettingsViewModel.TestResultListViewState = testResultListView.SaveState();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }

            base.OnFormClosing(e);
        }

        private void prevStepTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveIfNeeded())
                {
                    return;
                }

                var prevStep = viewModel.GetPreviousStep();
                if (prevStep != null)
                {
                    ShowStep(prevStep);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void nextStepTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveIfNeeded())
                {
                    return;
                }

                var nextStep = viewModel.GetNextStep();
                if (nextStep != null)
                {
                    ShowStep(nextStep);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    if (saveTSButton.Enabled)
                    {
                        saveTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Alt | Keys.Left:
                    if (prevStepTSButton.Enabled)
                    {
                        prevStepTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Alt | Keys.Right:
                    if (nextStepTSButton.Enabled)
                    {
                        nextStepTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.F5:
                    if (runTSButton.Enabled)
                    {
                        runTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.F6:
                    if (stopTSButton.Enabled)
                    {
                        stopTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Escape:
                    mainToolStrip.Focus();
                    Close();
                    return true;
                case Keys.Control | Keys.K:
                    viewModel.ToggleHiddenProperties();
                    stepPropertyGrid.Refresh();
                    return true;

            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        // ToolStrip 버튼 업데이트.
        private void UpdateToolStripButtons()
        {
            if (testRunning)
            {
                prevStepTSButton.Enabled = false;
                nextStepTSButton.Enabled = false;
                runTSButton.Enabled = false;
                stopTSButton.Enabled = true;
            }
            else
            {
                prevStepTSButton.Enabled = viewModel.GetPreviousStep() != null;
                nextStepTSButton.Enabled = viewModel.GetNextStep() != null;
                runTSButton.Enabled = true;
                stopTSButton.Enabled = false;
            }
        }

        private async void runTSButton_Click(object sender, EventArgs e)
        {
            if (viewModel == null || viewModel.CurrentStep == null)
            {
                return;
            }

            var totalResultState = EolStep.ResultState.NoState;
            var stopwatch = new Stopwatch();
            var elapsedTimer = new System.Timers.Timer();
            try
            {
                elapsedTimer.Elapsed += (tsender, te) =>
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        elapsedLabel.Text = SequenceViewModel.GetElapsedTimeText(stopwatch.ElapsedMilliseconds);
                    });
                };
                elapsedTimer.Interval = 500;
                elapsedTimer.AutoReset = true;
                elapsedTimer.Start();
                stopwatch.Start();

                testRunning = true;
                UpdateToolStripButtons();
                stepPropertyGrid.Enabled = false;
                stepPropertyGrid.UseWaitCursor = true;
                SequenceViewModel.GetRunningTextColor(out string runningMessage, out Color runningBackColor);
                resultStatusLabel.Text = runningMessage;
                resultStatusLabel.BackColor = runningBackColor;
                elapsedLabel.Text = "";
                infoLabel.Text = "";
                testResultListView.ClearObjects();

                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();

                var results = new List<EolStep.TestResult>();
                var runCount = int.Parse(runCountComboBox.Text);
                for (int i = 0; i < runCount; i++)
                {
                    var runResult = await Task.Run(() => viewModel.RunCurrentStep(elozTestSet, tokenSource.Token));
                    results.Add(runResult);

                    // Async 다운로드를 위한 처리.
                    runResult.PropertyChanged += (s, args) =>
                    {
                        if (args.PropertyName == nameof(EolStep.TestResult.ResultData))
                        {
                            // 이미지 보여주기.
                            if (runResult.ResultData is Image image)
                            {
                                Utils.InvokeIfRequired(this, () =>
                                {
                                    stepPictureBox.Image = image;
                                });
                            }
                        }
                    };

                    if (tokenSource.IsCancellationRequested)
                    {
                        break;
                    }
                }

                // 다운로드한 이미지가 있으면 표시.
                var lastResult = results.Last();
                if (lastResult.ResultData is Image img)
                {
                    stepPictureBox.Image = img;
                }

                testResultListView.BeginUpdate();
                testResultListView.AddObjects(results);
                //testResultListView.SelectObject(runResult);
                testResultListView.EndUpdate();
                testResultListView.EnsureModelVisible(lastResult);

                // 통계 표시.
                var values = results.Select(result => result.ResultValue.GetValueOrDefault());
                double min = values.Min();
                double max = values.Max();
                double mean = values.Average();
                double stdev = Math.Sqrt(values.Average(value => Math.Pow(value - mean, 2)));
                viewModel.CurrentStep.GetNominalValues(out double? expected, out double? upperLimit, out double? lowerLimit);
                double? cp = double.NaN;
                if (expected != null)
                {
                    double? delta = viewModel.CurrentStep.GetUpperLowerDelta(upperLimit, lowerLimit);
                    cp = delta / (6 * stdev);
                    var k = ((upperLimit + lowerLimit) / 2 - mean) / ((upperLimit - lowerLimit) / 2);
                    if (k != null)
                    {
                        k = Math.Abs(k.GetValueOrDefault());
                    }
                    var cpk = (1 - k) * cp;
                    infoLabel.Text = $"Min={min:F4}, Max={max:F4}, Mean={mean:F4}, StDev={stdev:F4}, CP={cp:F4}, CPK={cpk:F4}";
                }

                totalResultState = SequenceViewModel.GetTotalResultState(results, false);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                totalResultState = EolStep.ResultState.Aborted;

                if (tokenSource == null || !tokenSource.IsCancellationRequested)
                {
                    infoLabel.Text = "Error: " + ex.Message;
                }
            }
            finally
            {
                stopwatch.Stop();
                elapsedTimer.Stop();

                SequenceViewModel.GetStateInfo(totalResultState, out string stateText, out Color backColor, out Color foreColor);
                resultStatusLabel.Text = stateText;
                resultStatusLabel.ForeColor = foreColor;
                resultStatusLabel.BackColor = backColor;
                elapsedLabel.Text = SequenceViewModel.GetElapsedTimeText(stopwatch.ElapsedMilliseconds);

                elapsedTimer.Dispose();
                testRunning = false;
                stepPropertyGrid.UseWaitCursor = false;
                stepPropertyGrid.Enabled = true;
                UpdateToolStripButtons();
            }
        }

        private void stopTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                tokenSource?.Cancel();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void testResultListView_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            // 행 번호를 보여준다.
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void testResultListView_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (viewModel.GetCellBackColor(e.Column.AspectName, e.Model, out Color backColor))
            {
                e.SubItem.BackColor = backColor;
            }
        }

        private void contextMenu_Opening(object sender, CancelEventArgs e)
        {
            // Reset 메뉴 Enable/Disable
            var currentItem = stepPropertyGrid.SelectedGridItem;
            var component = currentItem?.Parent?.Value;
            resetToolStripMenuItem.Enabled = currentItem?.PropertyDescriptor?.CanResetValue(component) ?? false;
        }

        private void resetToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // Reset 액션
            stepPropertyGrid.ResetSelectedProperty();
        }

        private void stepPictureBox_DoubleClick(object sender, EventArgs e)
        {
            try
            {
                StepEditViewModel.RunShellImageViewer(stepPictureBox.Image);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Showing oscilloscope image: {ex.Message}");
            }
        }

        private void resultContextMenu_Opening(object sender, CancelEventArgs e)
        {
            clearToolStripMenuItem.Enabled = testResultListView.Objects?.Cast<object>()?.Count() > 0;
        }

        private void clearToolStripMenuItem_Click(object sender, EventArgs e)
        {
            testResultListView.ClearObjects();
        }
    }
}
