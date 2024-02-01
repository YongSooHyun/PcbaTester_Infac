using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model;
using EOL_GND.ViewModel;
using InfoBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Linq;

namespace EOL_GND.View
{
    /// <summary>
    /// EOL 시퀀스를 편집하고 실행하는 메인 UI.
    /// </summary>
    public partial class SequenceForm : Form
    {
        // ViewModel instance.
        internal SequenceViewModel ViewModel { get; private set; } = null;

        // 테스트 실행 상태.
        private bool TestRunning
        {
            get => _testRunningState.WaitOne(0);
            set
            {
                bool running = _testRunningState.WaitOne(0);
                if (running != value)
                {
                    if (value)
                    {
                        _testRunningState.Set();
                    }
                    else
                    {
                        _testRunningState.Reset();
                    }

                    Utils.InvokeIfRequired(this, () =>
                    {
                        if (value)
                        {
                            //runningProgressBar.Style = ProgressBarStyle.Marquee;
                            runningProgressBar.Visible = true;
                        }
                        else
                        {
                            //runningProgressBar.Style = ProgressBarStyle.Blocks;
                            runningProgressBar.Visible = false;
                            runningProgressBar.Value = runningProgressBar.Minimum;
                        }
                    });
                }
            }
        }
        private readonly ManualResetEvent _testRunningState = new ManualResetEvent(false);

        // 실행 중지에 쓰이는 token.
        private CancellationTokenSource tokenSource = null;

        // 시작할 때 열려는 파일.
        private readonly string openFilePath = null;

        // Relay 실행에 이용되는 필드.
        private readonly object elozTestSet = null;

        // 시퀀스를 자동 실행할지 여부.
        private readonly bool autoStart = false;

        // 시퀀스를 자동 실행할 때 시작 번호, 끝 번호, 스텝 개수.
        private readonly int startNumber, endNumber, stepCount;

        // 마지막 검색에서 스텝을 발견했는지 여부.
        private bool patternFound = false;

        // 현재 검색 문자열, 대소문자 구분 여부.
        private string searchPattern = null;
        private bool ignoreCase = true;
        private readonly BindingList<string> searchedPattens = new BindingList<string>();

        /// <summary>
        /// 시퀀스 자동실행 결과.
        /// </summary>
        public EolStep.ResultState AutoStartResult { get; private set; } = Model.EolStep.ResultState.NoState;

        /// <summary>
        /// 윈도우 자동 꺼짐 시간(ms). 0 또는 그보다 작으면 자동으로 꺼지지 않음.
        /// </summary>
        public int AutoCloseDelay { get; set; } = 0;

        /// <summary>
        /// 실행하려는 variant.
        /// </summary>
        public string SelectedVariant { get; set; } = null;

        public SequenceForm(string filePath, object testSet, bool autoStart, int startNumber, int endNumber, int count)
        {
            InitializeComponent();

            // ViewModel 설정.
            ViewModel = new SequenceViewModel();

            // 폼 타이틀 설정.
            UpdateTitle();
            companyStatusLabel.Text = ViewModel.GetCompany();

            // ObjectListView 설정.
            FilterSteps();
            //enabledColumn.AspectToStringConverter = rowObject => string.Empty;
            enabledColumn.ImageGetter = SequenceViewModel.EnabledImageGetter;
            nameColumn.ImageGetter = SequenceViewModel.StepImageGetter;
            groupColumn.ImageGetter = SequenceViewModel.GroupImageGetter;
            specLogColumn.ImageGetter = SequenceViewModel.SpecLogImageGetter;
            UpdateFont();

            // 테스트 상태.
            SequenceViewModel.GetStateInfo(EolStep.ResultState.NoState, out string status, out Color backColor, out _);
            statusLabel.Text = status;
            statusLabel.BackColor = backColor;

            // Toolbar 업데이트.
            UpdateToolStripButtons();

            // 시작과 동시에 열려는 파일.
            if (File.Exists(filePath))
            {
                openFilePath = filePath;
            }

            // eloz 디바이스.
            elozTestSet = testSet;

            // 자동실행 여부.
            this.autoStart = autoStart;
            this.startNumber = startNumber;
            this.endNumber = endNumber;
            stepCount = count;

            // 현재 사용자 보여주기.
            ShowCurrentUser();
        }

        // 현재 로그인한 사용자를 보여준다.
        private void ShowCurrentUser()
        {
            logoutTSButton.Text = ViewModel.CurrentUser?.UserName;
        }

        private void FilterSteps()
        {
            bool showEnabledStepsOnly = AppSettings.SharedInstance.ShowEnabledStepsOnly;
            bool showSpecStepsOnly = AppSettings.SharedInstance.ShowSpecStepsOnly;

            showEnabledTSButton.Checked = showEnabledStepsOnly;
            showSpecTSButton.Checked = showSpecStepsOnly;

            if (showEnabledStepsOnly || showSpecStepsOnly)
            {
                sequenceListView.AdditionalFilter = new ModelFilter(model =>
                {
                    var step = model as EolStep;
                    if (step != null)
                    {
                        bool show = true;
                        if (showEnabledStepsOnly)
                        {
                            show = step.Enabled;
                        }

                        if (show && showSpecStepsOnly)
                        {
                            show = step.ResultSpecLog;
                        }

                        return show;
                    }
                    else
                    {
                        return true;
                    }
                });
            }
            else
            {
                sequenceListView.AdditionalFilter = null;
            }
        }

        internal void UpdateFont()
        {
            sequenceListView.Font = GeneralSettingsViewModel.SequenceFont;
            UpdateListViewRowHeight();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // ListView Row Height를 조정한다.
            UpdateListViewRowHeight();

            // 마지막 윈도우 상태 복원.
            if (GeneralSettingsViewModel.SequenceEditorRestoreState)
            {
                Utils.SetWindowState(this, GeneralSettingsViewModel.SequenceEditorState, GeneralSettingsViewModel.SequenceEditorLocation, 
                    GeneralSettingsViewModel.SequenceEditorSize);

                // 마지막 리스트 뷰 상태 복원.
                var lastListViewState = GeneralSettingsViewModel.SequenceListViewState;
                if (lastListViewState != null)
                {
                    sequenceListView.RestoreState(lastListViewState);
                }
            }

            // Sort상태 제거.
            sequenceListView.Unsort();
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            //// Check the license.
            //if (!SequenceViewModel.GetLicenseEnabled(AppSettings.SharedInstance.LicenseKey))
            //{
            //    // Show the license dialog.
            //    var dialog = new LicenseForm();
            //    dialog.StartPosition = FormStartPosition.CenterParent;
            //    if (dialog.ShowDialog() != DialogResult.OK)
            //    {
            //        Close();
            //        return;
            //    }
            //}

            // 지정한 파일 열기.
            if (openFilePath != null)
            {
                try
                {
                    LoadFile(openFilePath);

                    // 자동실행.
                    if (elozTestSet != null && autoStart)
                    {
                        // 파라미터 검증.
                        // 시작번호, 끝번호가 0이면 검증 없이 모든 스텝 실행.
                        IList testSteps;
                        if (startNumber == 0 && endNumber == 0)
                        {
                            // Do nothing.
                            testSteps = ViewModel.OriginalSteps;
                        }
                        else
                        {
                            if (startNumber < 1 || endNumber < 1)
                            {
                                throw new IndexOutOfRangeException("시퀀스 번호는 1 이상이어야 합니다.");
                            }

                            if (startNumber > endNumber)
                            {
                                throw new IndexOutOfRangeException("시퀀스 끝번호는 시퀀스 시작번호와 같거나 커야 합니다.");
                            }

                            if (endNumber > ViewModel.OriginalSteps.Count)
                            {
                                throw new IndexOutOfRangeException("시퀀스 끝번호는 시퀀스 길이와 같거나 작아야 합니다.");
                            }

                            // Enabled 된 스텝 개수 체크.
                            int enabledSteps = 0;
                            for (int i = startNumber - 1; i < endNumber; i++)
                            {
                                if (ViewModel.OriginalSteps[i].Enabled)
                                {
                                    enabledSteps++;
                                }
                            }
                            if (enabledSteps < stepCount)
                            {
                                throw new Exception($"실행할 최소 스텝 개수 조건을 충족하지 않습니다({enabledSteps} < {stepCount}).");
                            }

                            testSteps = ViewModel.OriginalSteps.ToList().GetRange(startNumber - 1, endNumber - startNumber + 1);
                        }

                        await Task.Delay(100);

                        tokenSource?.Dispose();
                        tokenSource = new CancellationTokenSource();
                        AutoStartResult = await RunSteps(testSteps, SelectedVariant, true, true, tokenSource.Token);

                        // 시간 지연 후 자동 close.
                        if (AutoCloseDelay > 0)
                        {
                            _ = Task.Delay(AutoCloseDelay).ContinueWith((t) =>
                            {
                                Close();
                            });
                        }
                    }
                }
                catch (Exception ex)
                {
                    AutoStartResult = EolStep.ResultState.Aborted;

                    Logger.LogError($"Error: {ex.Message}");
                    Utils.ShowErrorDialog(ex);
                }
            }
        }

        private void UpdateListViewRowHeight()
        {
            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = sequenceListView.Font;
            Controls.Add(dummyComboBox);
            sequenceListView.RowHeight = dummyComboBox.Height + 4;
            Controls.Remove(dummyComboBox);
        }

        // 시퀀스 새로 만들기.
        private void NewTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveIfNeeded())
                {
                    return;
                }

                using (var saveDialog = CreateSaveFileDialog())
                {
                    if (saveDialog.ShowDialog() == DialogResult.OK)
                    {
                        ViewModel.CreateSequence(saveDialog.FileName);
                        UpdateSequenceView();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 현재 열려있는 시퀀스가 변경되었으면 저장여부를 사용자에게 물어 저장한다.
        // 사용자가 Cancel 버튼을 눌렀는지 여부를 리턴한다.
        private bool SaveIfNeeded()
        {
            bool canceled = false;
            if (ViewModel.SequenceOpened && ViewModel.SequenceModified)
            {
                var result = InformationBox.Show($"변경된 내용이 있습니다.{Environment.NewLine}변경된 내용을 저장하시겠습니까?",
                    "시퀀스 저장", buttons: InformationBoxButtons.YesNoCancel, icon: InformationBoxIcon.Question);

                if (result == InformationBoxResult.Yes)
                {
                    SaveSequence();
                }

                canceled = result == InformationBoxResult.Cancel;
            }
            return canceled;
        }

        // 시퀀스 저장.
        private void SaveSequence()
        {
            bool modified = ViewModel.SequenceOpened && ViewModel.SequenceModified;

            ViewModel.SaveSequence();
            UpdateTitle();

            // 시퀀스를 저장할 때마다 변경 내용 기록.
            string remarks = null;
            if (AppSettings.SharedInstance.AskToEnterHistoryRemarks && modified)
            {
                var dialog = new ChangeRemarksForm();
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    remarks = dialog.EnteredText?.Replace("\r", " ").Replace("\n", " ");
                }
            }
            ViewModel.UpdateChangeHistory(false, remarks);
        }

        // 시퀀스를 새로 만들 때 이용되는 파일 대화상자 만들기.
        internal static SaveFileDialog CreateSaveFileDialog()
        {
            var dialog = new SaveFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Filter = SequenceViewModel.GetFileFilter();
            return dialog;
        }

        // 시퀀스를 열 때 이용되는 파일 대화상자 만들기.
        private OpenFileDialog CreateOpenFileDialog()
        {
            var dialog = new OpenFileDialog();
            dialog.RestoreDirectory = true;
            dialog.Filter = SequenceViewModel.GetFileFilter();
            return dialog;
        }

        // 시퀀스 리스트 뷰 업데이트.
        private void UpdateListView()
        {
            if (ViewModel.OriginalSteps != null)
            {
                ViewModel.OriginalSteps.ListChanged -= Steps_ListChanged;
            }
            sequenceListView.SetObjects(ViewModel.OriginalSteps);
            if (ViewModel.OriginalSteps != null)
            {
                ViewModel.OriginalSteps.ListChanged += Steps_ListChanged;
            }
        }

        // 시퀀스 변경사항을 반영해 ListView, Title을 업데이트한다.
        private void Steps_ListChanged(object sender, ListChangedEventArgs e)
        {
            try
            {
                Utils.InvokeIfRequired(this, () =>
                {
                    UpdateTitle();

                    if (AppSettings.SharedInstance.ShowEnabledStepsOnly && e.ListChangedType == ListChangedType.ItemChanged &&
                        e.PropertyDescriptor?.Name == nameof(EolStep.Enabled) || 
                        AppSettings.SharedInstance.ShowSpecStepsOnly && e.ListChangedType == ListChangedType.ItemChanged && 
                        e.PropertyDescriptor?.Name == nameof(EolStep.ResultSpecLog))
                    {
                        FilterSteps();
                    }
                });
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // ToolStrip 버튼 업데이트.
        private void UpdateToolStripButtons()
        {
            if (TestRunning)
            {
                newTSButton.Enabled = false;
                openTSSButton.Enabled = false;
                saveTSButton.Enabled = false;
                saveAsTSButton.Enabled = false;
                closeTSButton.Enabled = false;
                stepAddTSButton.Enabled = false;
                stepRemoveTSButton.Enabled = false;
                findTSButton.Enabled = false;
                runAllTSButton.Enabled = false;
                stopTSButton.Enabled = true;
                clearResultsTSButton.Enabled = false;
                sortResetTSButton.Enabled = false;
                settingsTSButton.Enabled = false;
                seqOptionsTSButton.Enabled = false;
                variantsTSButton.Enabled = false;
                multiBoardTSButton.Enabled = false;
                changeHistoryTSButton.Enabled = false;
                loginTSButton.Enabled = false;
                logoutTSButton.Enabled = false;
            }
            else
            {
                var permission = ViewModel.CurrentUser?.GetPermission();
                newTSButton.Enabled = permission?.CanEditSequence ?? false;
                openTSSButton.Enabled = true;
                if (ViewModel.SequenceOpened)
                {
                    saveTSButton.Enabled = permission?.CanEditSequence ?? false;
                    saveAsTSButton.Enabled = permission?.CanEditSequence ?? false;
                    closeTSButton.Enabled = true;
                    stepAddTSButton.Enabled = permission?.CanEditSequence ?? false;
                    findTSButton.Enabled = true;
                    runAllTSButton.Enabled = true;
                    clearResultsTSButton.Enabled = true;
                    sortResetTSButton.Enabled = sequenceListView.PrimarySortColumn != null;
                    seqOptionsTSButton.Enabled = permission?.CanEditSequence ?? false;
                    variantsTSButton.Enabled = permission?.CanEditSequence ?? false;
                    multiBoardTSButton.Enabled = permission?.CanEditSequence ?? false;
                    changeHistoryTSButton.Enabled = true;
                }
                else
                {
                    saveTSButton.Enabled = false;
                    saveAsTSButton.Enabled = false;
                    closeTSButton.Enabled = false;
                    stepAddTSButton.Enabled = false;
                    findTSButton.Enabled = false;
                    runAllTSButton.Enabled = false;
                    clearResultsTSButton.Enabled = false;
                    sortResetTSButton.Enabled = false;
                    seqOptionsTSButton.Enabled = false;
                    variantsTSButton.Enabled = false;
                    multiBoardTSButton.Enabled = false;
                    changeHistoryTSButton.Enabled = false;
                }
                UpdateRemoveButton();
                stopTSButton.Enabled = false;
                settingsTSButton.Enabled = true;
                loginTSButton.Enabled = true;
                logoutTSButton.Enabled = ViewModel.CurrentUser != null;
            }
        }

        // 윈도우 타이틀 업데이트.
        private void UpdateTitle()
        {
            Text = ViewModel.GetTitle();
        }

        // 시퀀스 오픈.
        private void OpenTSSButton_ButtonClick(object sender, EventArgs e)
        {
            try
            {
                LoadFile();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 지정한 시퀀스 파일을 로딩.
        // 현재 오픈된 시퀀스가 있고 변경된 내용이 있으면 저장할지 여부를 물어본다.
        private void LoadFile(string filePath = null)
        {
            if (SaveIfNeeded())
            {
                return;
            }

            if (filePath == null)
            {
                using (var dialog = CreateOpenFileDialog())
                {
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        filePath = dialog.FileName;
                    }
                }
            }

            if (filePath != null)
            {
                ViewModel.LoadSequence(filePath);
                UpdateSequenceView();

                // 파일을 로딩할 때마다 외부에 의해 변경된 내용 저장.
                ViewModel.UpdateChangeHistory(true, null);
            }
        }

        // 시퀀스를 새로 열었을 때 이와 관련된 뷰들을 업데이트.
        private void UpdateSequenceView()
        {
            UpdateListView();
            UpdateToolStripButtons();
            UpdateTitle();
        }

        // 시퀀스 저장.
        private void SaveTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                SaveSequence();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Save sequence error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 시퀀스 다른 이름으로 저장.
        private void saveAsTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                using (var dialog = CreateSaveFileDialog())
                {
                    dialog.Title = "Save As";
                    dialog.OverwritePrompt = true;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        ViewModel.SaveAsSequence(dialog.FileName, true);
                        UpdateTitle();

                        // 시퀀스를 저장할 때마다 변경 내용 기록.
                        string remarks = null;
                        if (AppSettings.SharedInstance.AskToEnterHistoryRemarks)
                        {
                            var remarksDialog = new ChangeRemarksForm();
                            if (remarksDialog.ShowDialog() == DialogResult.OK)
                            {
                                remarks = remarksDialog.EnteredText?.Replace("\r", " ").Replace("\n", " ");
                            }
                        }
                        ViewModel.UpdateChangeHistory(false, remarks);
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Save as sequence error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void closeTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (SaveIfNeeded())
                {
                    return;
                }

                ViewModel.CloseSequence();
                UpdateSequenceView();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Close sequence error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 최근 열어본 파일 리스트 보여주기.
        private void OpenTSSButton_DropDownOpening(object sender, EventArgs e)
        {
            // MRU list.
            openTSSButton.DropDownItems.Clear();
            for (int i = 0; i < ViewModel.MruFiles.Count; i++)
            {
                var item = openTSSButton.DropDownItems.Add($"{i + 1} {ViewModel.MruFiles[i]}");
                item.Tag = i;
            }
        }

        // 최근 열어본 파일 리스트 중 하나를 선택했을 때 호출되는 함수.
        private void OpenTSSButton_DropDownItemClicked(object sender, ToolStripItemClickedEventArgs e)
        {
            try
            {
                int fileIndex = (int)e.ClickedItem.Tag;
                var filePath = ViewModel.MruFiles[fileIndex];
                if (File.Exists(filePath))
                {
                    LoadFile(filePath);
                }
                else
                {
                    ViewModel.MruFiles.RemoveAt(fileIndex);
                    InformationBox.Show("지정한 파일이 없습니다.", "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // Form을 닫을 때 호출되는 메서드 오버라이드.
        protected override void OnFormClosing(FormClosingEventArgs e)
        {
            try
            {
                if (TestRunning)
                {
                    e.Cancel = true;
                    return;
                }

                if (SaveIfNeeded())
                {
                    e.Cancel = true;
                    return;
                }

                TestDevice.CloseAllDevices();
                tokenSource?.Dispose();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
            }

            try
            {
                // 윈도우 상태, 위치, 크기 저장.
                GeneralSettingsViewModel.SequenceEditorState = WindowState;
                GeneralSettingsViewModel.SequenceEditorLocation = Location;
                GeneralSettingsViewModel.SequenceEditorSize = Size;

                // 리스트 뷰 상태 저장.
                GeneralSettingsViewModel.SequenceListViewState = sequenceListView.SaveState();

                // App 설정 저장.
                ViewModel.SaveAppSettings();
            }
            catch
            { }

            // 열려있는 다른 윈도우 모두 닫기.
            for (int i = Application.OpenForms.Count - 1; i >= 0; i--)
            {
                if (Application.OpenForms[i] is StepEditForm editForm)
                {
                    editForm.Close();
                }
            }

            // ImageViewer 닫기.
            ImageViewer.SharedViewer?.Close();

            base.OnFormClosing(e);
        }

        // 스텝 추가 버튼을 누를 때의 처리.
        private void StepAddTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                if (ViewModel.SequenceOpened == false)
                {
                    return;
                }

                // 스텝을 새로 만들기 위한 대화상자를 보여준다.
                var dialog = new StepCreationForm();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ShowInTaskbar = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    // 스텝을 새로 만든다.
                    object lastSelectedObj = null;
                    int selectedObjCount = sequenceListView.SelectedObjects.Count;
                    if (selectedObjCount > 0)
                    {
                        lastSelectedObj = sequenceListView.SelectedObjects[selectedObjCount - 1];
                    }
                    var modelObjects = sequenceListView.Objects.Cast<object>().ToList();
                    int insertPosition = modelObjects.IndexOf(lastSelectedObj);
                    if (insertPosition < 0)
                    {
                        insertPosition = modelObjects.Count;
                    }
                    else
                    {
                        insertPosition++;
                    }

                    var newSteps = ViewModel.InsertNewSteps(insertPosition, dialog.SelectedCategoryInfo, dialog.Count);

                    // 리스트 뷰 업데이트.
                    //sequenceListView.BuildList();
                    if (newSteps != null)
                    {
                        sequenceListView.InsertObjects(insertPosition, newSteps);
                        sequenceListView.SelectObjects(newSteps);

                        // 변경여부를 보여주기 위해 타이틀 업데이트.
                        UpdateTitle();
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 스텝 삭제 버튼을 누를 때의 처리.
        private void StepRemoveTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedObjects = sequenceListView.SelectedObjects;
                if (selectedObjects.Count == 0)
                {
                    return;
                }

                // 다시 한번 물어본다.
                var answer = InformationBox.Show($"선택된 {selectedObjects.Count}개의 스텝을 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                if (answer != InformationBoxResult.OK)
                {
                    return;
                }

                bool removed = ViewModel.RemoveSteps(selectedObjects);
                if (removed)
                {
                    sequenceListView.RemoveObjects(selectedObjects);
                    UpdateTitle();
                    UpdateRemoveButton();
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 문자열 검색 윈도우를 보여준다.
        private void findTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 이미 열려있는 dialog가 있는지 체크.
                foreach (Form form in OwnedForms)
                {
                    if (form is FindReplaceForm)
                    {
                        form.Activate();
                        return;
                    }
                }

                var dialog = new FindReplaceForm();
                dialog.Owner = this;
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.Pattern = searchPattern;
                dialog.SearchedPatterns = searchedPattens;
                dialog.IgnoreCase = ignoreCase;
                dialog.Show();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        // 찾을 문자열을 설정한다.
        internal void SetSearchPattern(string pattern)
        {
            patternFound = false;
            searchPattern = pattern;
        }

        // 검색 대소문자 구분 설정.
        internal void SetSearchIgnoreCase(bool ignoreCase)
        {
            patternFound = false;
            this.ignoreCase = ignoreCase;
        }

        private void AddSearchPattern(string newPattern)
        {
            for (int i = 0; i < searchedPattens.Count; i++)
            {
                if (string.Equals(newPattern, searchedPattens[i]))
                {
                    searchedPattens.RemoveAt(i);
                    break;
                }
            }

            searchedPattens.Insert(0, newPattern);
        }

        // 문자열이 포함된 스텝을 찾는다.
        internal void Find(bool forward)
        {
            if (string.IsNullOrEmpty(searchPattern))
            {
                return;
            }

            // 검색이력 추가.
            AddSearchPattern(searchPattern);

            object foundStep = ViewModel.Find(sequenceListView.SelectedObject, searchPattern, ignoreCase, forward, 
                AppSettings.SharedInstance.ShowEnabledStepsOnly, AppSettings.SharedInstance.ShowSpecStepsOnly);
            if (foundStep != null)
            {
                patternFound = true;
                sequenceListView.SelectObject(foundStep);
                sequenceListView.EnsureModelVisible(foundStep);
            }
            else
            {
                string message;
                if (patternFound)
                {
                    message = "No more occurrences found the following text: " + Environment.NewLine + searchPattern;
                }
                else
                {
                    message = "The following specified text was not found: " + Environment.NewLine + searchPattern;
                }

                InformationBox.Show(message, "Find", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Warning);
            }
        }

        // 스텝 실행 순서대로 보여주기 버튼 누를 때.
        private void SortResetTSButton_Click(object sender, EventArgs e)
        {
            // 정렬을 해제한다.
            sequenceListView.BeginUpdate();
            sequenceListView.Unsort();
            sequenceListView.ClearObjects();
            UpdateListView();
            sequenceListView.EndUpdate();
            sortResetTSButton.Enabled = false;
        }

        // 정렬이 바뀐 다음 호출된다.
        private void SequenceListView_AfterSorting(object sender, BrightIdeasSoftware.AfterSortingEventArgs e)
        {
            // 실행순서로 정렬 버튼 Enable/Disable
            sortResetTSButton.Enabled = !TestRunning && sequenceListView.PrimarySortColumn != null;
        }

        // 로깅 TextBox에 메시지와 NewLine을 출력.
        internal void LogAppendLine(string message)
        {
            logTextBox.AppendText(message + Environment.NewLine);
        }

        // 로그 메시지를 지운다.
        internal void LogClear()
        {
            logTextBox.Clear();
        }

        // 로깅 보여주기/숨기기 버튼 누를 때.
        private void LogTSButton_Click(object sender, EventArgs e)
        {
            // 로깅 TextBox 보여주기/숨기기.
            mainSplitContainer.Panel2Collapsed = !(logTSButton.Checked = !logTSButton.Checked);
        }

        // Clear Log 버튼 누를 때의 처리.
        private void LogClearTSButton_Click(object sender, EventArgs e)
        {
            LogClear();
        }

        // List View의 row 선택이 변경될 때의 처리.
        private void SequenceListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRemoveButton();
        }

        // 스텝 삭제 버튼을 Enable/Disable 한다.
        private void UpdateRemoveButton()
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!TestRunning && (permission?.CanEditSequence ?? false))
            {
                stepRemoveTSButton.Enabled = sequenceListView.SelectedObjects?.Count > 0;
            }
            else
            {
                stepRemoveTSButton.Enabled = false;
            }
        }

        // ListView Item을 더블 클릭할 때 호출된다.
        private void SequenceListView_ItemActivate(object sender, EventArgs e)
        {
            // 같은 스텝이 열려있는가 검사.
            var openForms = Application.OpenForms;
            for (int i = 0; i < openForms.Count; i++)
            {
                if (openForms[i] is StepEditForm editForm && editForm.CurrentStep == sequenceListView.SelectedObject)
                {
                    editForm.Activate();
                    return;
                }
            }

            if (sequenceListView.SelectedObject != null && !TestRunning)
            {
                var form = new StepEditForm(ViewModel.OriginalSteps, sequenceListView.SelectedObject, elozTestSet);
                form.ShowInTaskbar = true;
                form.StartPosition = FormStartPosition.CenterScreen;
                form.Show();
            }
        }

        // 설정 대화상자 표시.
        private void SettingsTSButton_Click(object sender, EventArgs e)
        {
            var form = new SettingsForm();
            form.StartPosition = FormStartPosition.CenterParent;
            form.ShowInTaskbar = false;
            form.ShowDialog();
        }

        // 라이선스 대화상자 표시.
        private void licenseTSButton_Click(object sender, EventArgs e)
        {
            // Show the license dialog.
            var dialog = new LicenseForm();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog();
        }

        // 시퀀스 실행.
        private async void RunAllTSButton_Click(object sender, EventArgs e)
        {
            tokenSource?.Dispose();
            tokenSource = new CancellationTokenSource();
            await RunSteps(ViewModel.OriginalSteps, null, true, true, tokenSource.Token);
        }

        // 지정한 스텝들을 실행한다.
        private async Task<EolStep.ResultState> RunSteps(IList modelObjects, string variant, bool? skipDisabled, bool runCleanup, CancellationToken token)
        {
            LogClear();
            var stopwatch = new Stopwatch();
            var resultState = EolStep.ResultState.NoState;

            // 경과시간을 보여주기 위한 타이머.
            var elapsedTimer = new System.Timers.Timer();

            string stateText;
            Color stateBackColor;
            try
            {
                // 실행 옵션을 사용자가 입력하도록 한다.
                bool shouldAskOptions = false;
                if (skipDisabled == null)
                {
                    shouldAskOptions = ViewModel.ContainsDisabledStep(modelObjects);
                }

                if (!shouldAskOptions && string.IsNullOrWhiteSpace(variant))
                {
                    // 스텝들 중 variant가 설정된 것이 있는지 체크.
                    shouldAskOptions = ViewModel.ContainsVariantStep(modelObjects, null, skipDisabled);
                }

                if (shouldAskOptions)
                {
                    var runOptionsDialog = new RunOptionsForm();
                    runOptionsDialog.StartPosition = FormStartPosition.CenterParent;
                    runOptionsDialog.ViewModel = ViewModel;
                    if (runOptionsDialog.ShowDialog() != DialogResult.OK)
                    {
                        return EolStep.ResultState.NoState;
                    }

                    skipDisabled = runOptionsDialog.SkipDisabled;
                    variant = runOptionsDialog.Variant;
                }

                elapsedTimer.Elapsed += (sender, e) =>
                {
                    Utils.InvokeIfRequired(this, () =>
                    {
                        elapsedLabel.Text = SequenceViewModel.GetElapsedTimeText(stopwatch.ElapsedMilliseconds);
                    });
                };
                elapsedTimer.Interval = 500;
                elapsedTimer.AutoReset = true;
                elapsedTimer.Start();

                TestRunning = true;
                UpdateToolStripButtons();
                sequenceListView.ShowCommandMenuOnRightClick = false;
                sequenceListView.ContextMenuStrip = null;

                SequenceViewModel.GetRunningTextColor(out stateText, out stateBackColor);
                statusLabel.Text = stateText;
                statusLabel.BackColor = stateBackColor;
                elapsedLabel.Text = "";
                infoLabel.Text = "";

                ViewModel.ClearTestResults();

                List<EolStep.TestResult> results = null;
                stopwatch.Start();
                int stepCount = modelObjects.Count;
                if (ViewModel.SequenceOpened && ViewModel.OriginalSteps?.Count > 0 && stepCount > 0)
                {
                    int currentStepIndex = 0;
                    results = await Task.Run(() => {
                        var runResults = ViewModel.RunTestSteps(modelObjects, variant, skipDisabled ?? true, elozTestSet, runCleanup, token, step =>
                        {
                            Utils.InvokeIfRequired(this, () =>
                            {
                                // Visible 상태를 체크해 현재 실행되고 있는 스텝 이후의 첫 Visible 스텝을 선택.
                                EolStep firstVisibleStep = null;
                                bool showEnabledOnly = AppSettings.SharedInstance.ShowEnabledStepsOnly;
                                bool showSpecOnly = AppSettings.SharedInstance.ShowSpecStepsOnly;
                                int currentIndex = modelObjects.IndexOf(step);
                                if (currentIndex >= 0)
                                {
                                    for (int i = currentIndex; i < modelObjects.Count; i++)
                                    {
                                        var modelStep = modelObjects[i] as EolStep;
                                        if (modelStep != null)
                                        {
                                            var visible = true;
                                            if (showEnabledOnly)
                                            {
                                                visible = modelStep.Enabled;
                                            }

                                            if (visible && showSpecOnly)
                                            {
                                                visible = modelStep.ResultSpecLog;
                                            }

                                            if (visible)
                                            {
                                                firstVisibleStep = modelStep;
                                                break;
                                            }
                                        }
                                    }
                                }

                                if (firstVisibleStep != null)
                                {
                                    sequenceListView.SelectObject(firstVisibleStep);
                                    sequenceListView.EnsureModelVisible(firstVisibleStep);
                                }
                            });
                        }, (step, percent) =>
                        {
                            Utils.InvokeIfRequired(this, () =>
                            {
                                // ProgressBar 업데이트.
                                currentStepIndex++;
                                runningProgressBar.Value = (int)(runningProgressBar.Minimum + (runningProgressBar.Maximum - runningProgressBar.Minimum) * percent);
                            });

                            // Result image 보여주기 위한 처리.
                            if (step.ShouldShowResultImage())
                            {
                                if (step.RunResult?.ResultData is Image resultImage)
                                {
                                    Utils.InvokeIfRequired(this, () =>
                                    {
                                        ImageViewer.Show(this, resultImage, step.DeviceName + " Image");
                                    });
                                }

                                // Async 방식으로 이미지를 다운로드할 때 필요한 처리.
                                if (step.RunResult != null)
                                {
                                    try
                                    {
                                        step.RunResult.PropertyChanged += (sender, eventArgs) =>
                                        {
                                            if (eventArgs.PropertyName == nameof(EolStep.TestResult.ResultData)
                                                && step?.RunResult?.ResultData is Image image)
                                            {
                                                Utils.InvokeIfRequired(this, () =>
                                                {
                                                    ImageViewer.Show(this, image, step.DeviceName + " Image");
                                                });
                                            }
                                        };
                                    }
                                    catch (Exception ex)
                                    {
                                        Logger.LogError($"Image display error: {ex.Message}");
                                    }
                                }
                            }
                        });

                        // Debugging.
                        Logger.LogVerbose("Task: all steps are executed");

                        return runResults;
                    });

                    // Debugging.
                    Logger.LogVerbose("All steps are executed");
                }

                resultState = SequenceViewModel.GetTotalResultState(results);
            }
            catch (Exception ex)
            {
                resultState = EolStep.ResultState.Aborted;
                Logger.LogError($"Error: {ex.Message}");

                if (tokenSource == null || !tokenSource.IsCancellationRequested)
                {
                    infoLabel.Text = "Error: " + ex.Message;
                }

                InformationBox.Show(ex.Message, title: "Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
            finally
            {
                stopwatch.Stop();
                elapsedTimer.Stop();

                TestRunning = false;

                SequenceViewModel.GetStateInfo(resultState, out stateText, out stateBackColor, out _);
                statusLabel.Text = stateText;
                statusLabel.BackColor = stateBackColor;
                elapsedLabel.Text = SequenceViewModel.GetElapsedTimeText(stopwatch.ElapsedMilliseconds);

                elapsedTimer.Dispose();
                sequenceListView.ShowCommandMenuOnRightClick = true;
                sequenceListView.ContextMenuStrip = stepsContextMenu;
                UpdateToolStripButtons();
            }

            return resultState;
        }

        // 실행중인 테스트를 중지한다.
        private void stopTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                tokenSource.Cancel();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void sequenceListView_KeyDown(object sender, KeyEventArgs e)
        {
            // 테스트가 실행중이면 KeyDown 이벤트를 처리하지 않도록 한다.
            if (TestRunning)
            {
                e.Handled = true;
            }
        }

        private void sequenceListView_MouseDown(object sender, MouseEventArgs e)
        {
        }

        // Shortcut Key들을 정의한다.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            switch (keyData)
            {
                case Keys.Control | Keys.O:
                    if (openTSSButton.Enabled)
                    {
                        openTSSButton.PerformButtonClick();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.S:
                    if (saveTSButton.Enabled)
                    {
                        saveTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.W:
                    if (closeTSButton.Enabled)
                    {
                        closeTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.N:
                    if (stepAddTSButton.Enabled)
                    {
                        stepAddTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Delete:
                    if (stepRemoveTSButton.Enabled)
                    {
                        stepRemoveTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.F:
                    if (findTSButton.Enabled)
                    {
                        findTSButton.PerformClick();
                        return true;
                    }
                    break;
                case Keys.F3:
                    if (findTSButton.Enabled)
                    {
                        Find(true);
                        return true;
                    }
                    break;
                case Keys.Shift | Keys.F3:
                    if (findTSButton.Enabled)
                    {
                        Find(false);
                        return true;
                    }
                    break;
                case Keys.F5:
                    if (runAllTSButton.Enabled)
                    {
                        runAllTSButton.PerformClick();
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
                case Keys.Control | Keys.C:
                    if (permission?.CanEditSequence ?? false)
                    {
                        Copy();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.X:
                    if (permission?.CanEditSequence ?? false)
                    {
                        Cut();
                        return true;
                    }
                    break;
                case Keys.Control | Keys.V:
                    if (permission?.CanEditSequence ?? false)
                    {
                        Paste();
                        return true;
                    }
                    break;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void clearResultsTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                // 테스트 상태.
                SequenceViewModel.GetStateInfo(EolStep.ResultState.NoState, out string status, out Color backColor, out _);
                statusLabel.Text = status;
                statusLabel.BackColor = backColor;
                elapsedLabel.Text = "";
                infoLabel.Text = "";

                ViewModel.ClearTestResults();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void userTSButton_Click(object sender, EventArgs e)
        {
            var dialog = new LoginForm();
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                ViewModel.CurrentUser = dialog.LoggedUser;
                ShowCurrentUser();
                UpdateToolStripButtons();
            }
        }

        private void logoutTSButton_Click(object sender, EventArgs e)
        {
            // 한번 더 물어본다.
            var answer = InformationBox.Show("로그아웃 하시겠습니까?", "로그아웃 확인", buttons: InformationBoxButtons.YesNo,
                icon: InformationBoxIcon.Question);
            if (answer == InformationBoxResult.Yes)
            {
                ViewModel.CurrentUser = null;
                ShowCurrentUser();
                UpdateToolStripButtons();
            }
        }

        #region Context menu

        // Context menu 항목들을 Enable/Disable 시킨다.
        private void stepsContextMenu_Opening(object sender, CancelEventArgs e)
        {
            if (!ViewModel.SequenceOpened)
            {
                stepsContextMenu.Enabled = false;
                return;
            }

            var permission = ViewModel.CurrentUser?.GetPermission();
            stepsContextMenu.Enabled = true;

            var selectedObjs = sequenceListView.SelectedObjects;
            enableTestStepsToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && selectedObjs.Count > 0;
            disableTestStepsToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && selectedObjs.Count > 0;
            runTestStepsToolStripMenuItem.Enabled = selectedObjs.Count > 0;

            // Copy, Cut 메뉴 항목.
            copyTestStepsToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && selectedObjs.Count > 0;
            cutTestStepsToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && selectedObjs.Count > 0;

            // Paste 메뉴 항목.
            pasteTestStepsToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && SequenceViewModel.CopiedSteps.Count > 0;
            insertBeneathCurrentRowToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && sequenceListView.SelectedObject != null;

            // Delete steps menu.
            if (!TestRunning && (permission?.CanEditSequence ?? false))
            {
                deleteTestStepsToolStripMenuItem.Enabled = selectedObjs.Count > 0;
            }
            else
            {
                deleteTestStepsToolStripMenuItem.Enabled = false;
            }

            // Copy to clipboard.
            copyToClipboardToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && selectedObjs.Count > 0;

            // Paste from clipboard.
            pasteFromClipboardToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && SequenceViewModel.GetClipboard()?.Count > 0;
            insertBeneathCurrentRowToolStripMenuItem1.Enabled = (permission?.CanEditSequence ?? false) && sequenceListView.SelectedObject != null;

            // Restart ID 메뉴 항목.
            restartIDToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && sequenceListView.SelectedObjects.Count > 0;

            // Change column contents 메뉴 항목.
            changePropertiesToolStripMenuItem.Enabled = (permission?.CanEditSequence ?? false) && sequenceListView.SelectedObjects.Count > 0;
        }

        private void enableTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStepsEnabled(sequenceListView.SelectedObjects, true);
        }

        private void disableTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            SetStepsEnabled(sequenceListView.SelectedObjects, false);
        }

        private void SetStepsEnabled(IList objects, bool enabled)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (permission?.CanEditSequence ?? false)
            {
                ViewModel.SetStepsEnabled(objects, enabled);
            }
        }

        private async void runTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            // 선택된 test step들을 실행한다.
            var selectedObjs = sequenceListView.SelectedObjects;
            if (selectedObjs.Count > 0)
            {
                tokenSource?.Dispose();
                tokenSource = new CancellationTokenSource();
                await RunSteps(selectedObjs, null, null, false, tokenSource.Token);
            }
        }

        private void copyTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Copy();
        }

        private void Copy()
        {
            if (sequenceListView.SelectedObjects.Count == 0)
            {
                return;
            }

            var permission = ViewModel.CurrentUser?.GetPermission();
            if (permission?.CanEditSequence ?? false)
            {
                ViewModel.CopySteps(sequenceListView.SelectedObjects);
                sequenceListView.EnableObjects(sequenceListView.DisabledObjects);
            }
        }

        private void cutTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Cut();
        }

        private void Cut()
        {
            var selectedObjs = sequenceListView.SelectedObjects;
            if (selectedObjs.Count == 0)
            {
                return;
            }

            var permission = ViewModel.CurrentUser?.GetPermission();
            if (permission?.CanEditSequence ?? false)
            {
                ViewModel.CutSteps(selectedObjs);

                sequenceListView.BeginUpdate();
                sequenceListView.EnableObjects(sequenceListView.DisabledObjects);
                sequenceListView.DisableObjects(selectedObjs);
                sequenceListView.EndUpdate();
            }
        }

        private void insertOnTopToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSteps(SequenceViewModel.InsertPosition.Top, null, false);
        }

        private void insertBeneathCurrentRowToolStripMenuItem_Click(object sender, EventArgs e)
        {
            Paste();
        }

        private void Paste()
        {
            PasteSteps(SequenceViewModel.InsertPosition.Current, sequenceListView.SelectedObject, false);
        }

        private void insertAtBottomToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteSteps(SequenceViewModel.InsertPosition.Bottom, null, false);
        }

        private void PasteSteps(SequenceViewModel.InsertPosition pos, object currentObj, bool fromClipboard)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            bool move = ViewModel.Move;
            IList pastedSteps;
            int insertedPos;
            if (!fromClipboard)
            {
                pastedSteps = ViewModel.PasteSteps(pos, currentObj, out insertedPos);
            }
            else
            {
                pastedSteps = ViewModel.PasteClipboard(pos, currentObj, out insertedPos);
            }

            if (pastedSteps.Count > 0)
            {
                sequenceListView.BeginUpdate();
                sequenceListView.EnableObjects(sequenceListView.DisabledObjects);
                if (fromClipboard || !move)
                {
                    // Copy & Paste.
                    sequenceListView.InsertObjects(insertedPos, pastedSteps);
                    sequenceListView.SelectObjects(pastedSteps);
                }
                else
                {
                    // Cut & Paste.
                    sequenceListView.RemoveObjects(pastedSteps);
                    sequenceListView.InsertObjects(insertedPos, pastedSteps);
                    sequenceListView.SelectObjects(pastedSteps);
                }
                sequenceListView.EndUpdate();
            }
        }

        private void deleteTestStepsToolStripMenuItem_Click(object sender, EventArgs e)
        {
            if (stepRemoveTSButton.Enabled)
            {
                stepRemoveTSButton.PerformClick();
            }
        }

        private void copyToClipboardToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            SequenceViewModel.SetClipboard(sequenceListView.SelectedObjects);
        }

        private void insertOnTopToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            PasteSteps(SequenceViewModel.InsertPosition.Top, null, true);
        }

        private void insertBeneathCurrentRowToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            PasteSteps(SequenceViewModel.InsertPosition.Current, sequenceListView.SelectedObject, true);
        }

        private void insertAtBottomToolStripMenuItem1_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            PasteSteps(SequenceViewModel.InsertPosition.Bottom, null, true);
        }

        private void restartIDToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            // Step ID를 지정한 스텝부터 다시 시작하여 증가시킨다.
            ViewModel.RestartStepId(sequenceListView.SelectedObjects);
        }

        private void SequenceForm_DragOver(object sender, DragEventArgs e)
        {
            if (e.Data.GetDataPresent(DataFormats.FileDrop))
            {
                var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                if (files?.Length == 1)
                {
                    e.Effect = DragDropEffects.Copy;
                }
            }
        }

        private void SequenceForm_DragDrop(object sender, DragEventArgs e)
        {
            try
            {
                if (e.Data.GetDataPresent(DataFormats.FileDrop))
                {
                    var files = e.Data.GetData(DataFormats.FileDrop) as string[];
                    if (files?.Length == 1)
                    {
                        LoadFile(files.First());
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void versionHistoryTSButton_Click(object sender, EventArgs e)
        {
            var dialog = new VersionHistoryForm();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void showEnabledTSButton_Click(object sender, EventArgs e)
        {
            AppSettings.SharedInstance.ShowEnabledStepsOnly = !AppSettings.SharedInstance.ShowEnabledStepsOnly;
            FilterSteps();
        }

        private void showSpecTSButton_Click(object sender, EventArgs e)
        {
            AppSettings.SharedInstance.ShowSpecStepsOnly = !AppSettings.SharedInstance.ShowSpecStepsOnly;
            FilterSteps();
        }

        private void seqOptionsTSButton_Click(object sender, EventArgs e)
        {
            if (ViewModel.SequenceOpened)
            {
                var dialog = new SequenceOptionsForm();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ViewModel = ViewModel;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    UpdateTitle();
                }
            }
        }

        private void variantsTSButton_Click(object sender, EventArgs e)
        {
            if (ViewModel.SequenceOpened)
            {
                var dialog = new VariantEditorForm(false);
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ViewModel = ViewModel;
                dialog.ShowDialog();
                UpdateTitle();
            }
        }

        private void multiBoardTSButton_Click(object sender, EventArgs e)
        {
            if (ViewModel.SequenceOpened)
            {
                var dialog = new MultiBoardForm();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.ViewModel = ViewModel;
                dialog.ShowDialog(this);
            }
        }

        private void changeHistoryTSButton_Click(object sender, EventArgs e)
        {
            if (!ViewModel.SequenceOpened)
            {
                return;
            }

            var dialog = new ChangeHistoryForm();
            dialog.FilePath = ViewModel.FilePath;
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowInTaskbar = false;
            dialog.ShowDialog(this);
        }

        private void changePropertiesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            var permission = ViewModel.CurrentUser?.GetPermission();
            if (!(permission?.CanEditSequence ?? false))
            {
                return;
            }

            var modelObjects = sequenceListView.SelectedObjects;
            if (modelObjects.Count == 0)
            {
                return;
            }

            // 지정한 프로퍼티의 값들을 변경한다.
            var dialog = new PropertyChangeForm();
            dialog.PropertyNames = new List<string>()
            {
                nameof(EolStep.ElozRelayAfter),
                nameof(EolStep.ResultSpecLog),
                nameof(EolStep.Section),
                nameof(EolStep.Name),
                nameof(EolStep.Remarks),
                nameof(EolStep.DeviceName),
            };
            if (dialog.ShowDialog() == DialogResult.OK && dialog.SelectedName != null)
            {
                ViewModel.ChangeProperties(modelObjects, dialog.SelectedName, dialog.Value);
            }
        }

        #endregion // Context menu

        private void sequenceListView_FormatCell(object sender, BrightIdeasSoftware.FormatCellEventArgs e)
        {
            if (SequenceViewModel.GetCellBackColor(e.Column.AspectName, e.Model, out Color backColor))
            {
                e.SubItem.BackColor = backColor;
            }
        }
    }
}
