using CasRnf32NET;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class TestProjectEditForm : Form
    {
        /// <summary>
        /// 편집하려는 프로젝트가 설정된 Product. 폼이 로딩되기 전에 설정해야 한다.
        /// </summary>
        internal Product CurrentProduct { get; set; }

        // 편집하는 프로젝트 인스턴스.
        private TestProject project;

        public TestProjectEditForm()
        {
            InitializeComponent();

            multiPanelBarcodeComboBox.DataSource = Enum.GetValues(typeof(MultiPanelBarcodeType));
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // 관리자만 Master Good/Fail 편집을 할 수 있다.
            var userPermission = AppSettings.LoggedUser?.GetPermission();
            if (!(userPermission?.CanEditMasterProject ?? true))
            {
                mainTLPanel.RowStyles[3].Height = 0;
            }

            // 리스트 뷰 설정.
            grpListView.CellEditUseWholeCell = true;
            grpListView.AdjustRowHeightByFont();

            LoadProject();
        }

        private void LoadProject()
        {
            try
            {
                if (!string.IsNullOrEmpty(CurrentProduct?.ProjectPath))
                {
                    project = TestProjectManager.Load(CurrentProduct.ProjectPath);
                    if (project != null)
                    {
                        nameTextBox.Text = Path.GetFileNameWithoutExtension(project.Path);

                        fidTextBox.Text = Utils.ListToString(project.FIDs, ",");

                        panelNumericUpDown.Value = project.Panel;
                        UpdateBoardCheckboxes();
                        board1CheckBox.Checked = project.Board1Checked;
                        board2CheckBox.Checked = project.Board2Checked;
                        board3CheckBox.Checked = project.Board3Checked;
                        board4CheckBox.Checked = project.Board4Checked;
                        modelTextBox.Text = project.Model;
                        pcbInfoTextBox.Text = project.PcbInfoFile;

                        variantTextBox.Text = project.IctVariantName;

                        ictTextBox.Text = project.IctProjectName;
                        masterGoodTextBox.Text = project.IctMasterGoodProjectName;
                        masterGoodSnTextBox.Text = project.IctMasterGoodSerialNumber;
                        masterFailTextBox.Text = project.IctMasterFailProjectName;
                        masterFailSnTextBox.Text = project.IctMasterFailSerialNumber;

                        ictUnpowerCheckBox.Checked = project.IctUnpowerEnabled;
                        ictUnpowerTitleTextBox.Text = project.IctUnpowerSectionTitle;
                        ictUnpowerTextBox.Text = project.IctUnpowerSectionName;
                        ictUnpowerRunOrderNUDown.Value = project.IctUnpowerRunOrder;
                        ictUnpowerRetryCheckBox.Checked = project.IctUnpowerRetryEnabled;
                        if (project.IctUnpowerPressDown)
                        {
                            ictUnpowerPressDownCheckBox.CheckState = project.IctUnpowerPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            ictUnpowerPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        ictUnpowerEolCheckBox.Checked = project.IctUnpowerIsEOL;

                        ictPowerCheckBox.Checked = project.IctPowerEnabled;
                        ictPowerTitleTextBox.Text = project.IctPowerSectionTitle;
                        ictPowerTextBox.Text = project.IctPowerSectionName;
                        ictPowerRunOrderNUDown.Value = project.IctPowerRunOrder;
                        ictPowerRetryCheckBox.Checked = project.IctPowerRetryEnabled;
                        if (project.IctPowerPressDown)
                        {
                            ictPowerPressDownCheckBox.CheckState = project.IctPowerPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            ictPowerPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        ictPowerEolCheckBox.Checked = project.IctPowerIsEOL;

                        novaflashCheckBox.Checked = project.NovaEnabled;
                        novaflashTitleTextBox.Text = project.NovaSectionTitle;
                        novaflashTextBox.Text = project.NovaSectionName;
                        ispRunOrderNUDown.Value = project.NovaRunOrder;
                        novaflashRetryCheckBox.Checked = project.NovaRetryEnabled;
                        if (project.NovaPressDown)
                        {
                            ispPressDownCheckBox.CheckState = project.NovaPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            ispPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        ispEolCheckBox.Checked = project.NovaIsEOL;

                        jtagCheckBox.Checked = project.JtagEnabled;
                        jtagTitleTextBox.Text = project.JtagSectionTitle;
                        jtagTextBox.Text = project.JtagSectionName;
                        jtagRunOrderNUDown.Value = project.JtagRunOrder;
                        jtagRetryCheckBox.Checked = project.JtagRetryEnabled;
                        if (project.JtagPressDown)
                        {
                            jtagPressDownCheckBox.CheckState = project.JtagPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            jtagPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        jtagEolCheckBox.Checked = project.JtagIsEOL;
                        uutTextBox.Text = project.JtagProjectName;

                        functionCheckBox.Checked = project.FunctionEnabled;
                        functionTitleTextBox.Text = project.FunctionSectionTitle;
                        functionTextBox.Text = project.FunctionSectionName;
                        functionRunOrderNUDown.Value = project.FunctionRunOrder;
                        functionRetryCheckBox.Checked = project.FunctionRetryEnabled;
                        if (project.FunctionPressDown)
                        {
                            funcPressDownCheckBox.CheckState = project.FunctionPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            funcPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        funcEolCheckBox.Checked = project.FunctionIsEOL;

                        eolCheckBox.Checked = project.EolEnabled;
                        eolTitleTextBox.Text = project.EolSectionTitle;
                        eolTextBox.Text = project.EolSectionName;
                        eolRunOrderNUDown.Value = project.EolRunOrder;
                        eolRetryCheckBox.Checked = project.EolRetryEnabled;
                        if (project.EolPressDown)
                        {
                            eolPressDownCheckBox.CheckState = project.EolPressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            eolPressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        eolEolCheckBox.Checked = project.EolIsEOL;

                        ext1CheckBox.Checked = project.Ext1Enabled;
                        ext1TitleTextBox.Text = project.Ext1SectionTitle;
                        ext1TextBox.Text = project.Ext1SectionName;
                        ext1RunOrderNUDown.Value = project.Ext1RunOrder;
                        ext1RetryCheckBox.Checked = project.Ext1RetryEnabled;
                        if (project.Ext1PressDown)
                        {
                            ext1PressDownCheckBox.CheckState = project.Ext1PressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            ext1PressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        ext1EolCheckBox.Checked = project.Ext1IsEOL;

                        ext2CheckBox.Checked = project.Ext2Enabled;
                        ext2TitleTextBox.Text = project.Ext2SectionTitle;
                        ext2TextBox.Text = project.Ext2SectionName;
                        ext2RunOrderNUDown.Value = project.Ext2RunOrder;
                        ext2RetryCheckBox.Checked = project.Ext2RetryEnabled;
                        if (project.Ext2PressDown)
                        {
                            ext2PressDownCheckBox.CheckState = project.Ext2PressUp ? CheckState.Indeterminate : CheckState.Checked;
                        }
                        else
                        {
                            ext2PressDownCheckBox.CheckState = CheckState.Unchecked;
                        }
                        ext2EolCheckBox.Checked = project.Ext2IsEOL;

                        power1VoltageNumericUpDown.Value = (decimal)project.Power1Voltage;
                        power1CurrentNumericUpDown.Value = (decimal)project.Power1Current;
                        power1UseCheckBox.Checked = project.Power1Enabled;
                        power2VoltageNumericUpDown.Value = (decimal)project.Power2Voltage;
                        power2CurrentNumericUpDown.Value = (decimal)project.Power2Current;
                        power2UseCheckBox.Checked = project.Power2Enabled;

                        // GRP 정보 로딩.
                        var grpFiles = project.GetOrderedGrpInfos();
                        grpListView.SetObjects(grpFiles);

                        multiPanelBarcodeComboBox.SelectedItem = project.MultiPanelBarcodeType;
                        twoBoardsDirectionCheckBox.Checked = project.TwoBoardsLeftRight;
                        bottomBoardFirstCheckBox.Checked = project.BottomBoardFirst;
                    }
                }
            }
            catch (Exception e)
            {
                InformationBox.Show(e.Message, "프로젝트 로딩 오류", icon: InformationBoxIcon.Error);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            if (project == null)
            {
                InformationBox.Show("로딩된 프로젝트가 없습니다.", "오류", icon: InformationBoxIcon.Error);
                return;
            }

            try
            {
                // 입력값들을 검증한다.

                // 이름.
                StringBuilder errorMessage = new StringBuilder();
                string newProjectPath = Path.Combine(Path.GetDirectoryName(project.Path), nameTextBox.Text) + "." + TestProjectManager.FileExtension;
                if (!TestProjectManager.ValidateValue(project.Path, nameof(TestProject.Path), newProjectPath, errorMessage))
                {
                    throw new Exception(errorMessage.ToString());
                }

                project.Path = newProjectPath;
                project.Panel = (int)panelNumericUpDown.Value;
                project.Board1Checked = board1CheckBox.Checked;
                project.Board2Checked = board2CheckBox.Checked;
                project.Board3Checked = board3CheckBox.Checked;
                project.Board4Checked = board4CheckBox.Checked;
                project.Model = modelTextBox.Text;
                project.PcbInfoFile = pcbInfoTextBox.Text;

                project.IctVariantName = variantTextBox.Text;

                project.IctMasterGoodProjectName = masterGoodTextBox.Text;
                project.IctMasterGoodSerialNumber = masterGoodSnTextBox.Text;
                project.IctMasterFailProjectName = masterFailTextBox.Text;
                project.IctMasterFailSerialNumber = masterFailSnTextBox.Text;

                project.IctProjectName = ictTextBox.Text;

                project.IctUnpowerEnabled = ictUnpowerCheckBox.Checked;
                project.IctUnpowerSectionTitle = ictUnpowerTitleTextBox.Text;
                project.IctUnpowerSectionName = ictUnpowerTextBox.Text;
                project.IctUnpowerRunOrder = (int)ictUnpowerRunOrderNUDown.Value;
                project.IctUnpowerRetryEnabled = ictUnpowerRetryCheckBox.Checked;
                project.IctUnpowerPressDown = ictUnpowerPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.IctUnpowerPressUp = ictUnpowerPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.IctUnpowerIsEOL = ictUnpowerEolCheckBox.Checked;

                project.IctPowerEnabled = ictPowerCheckBox.Checked;
                project.IctPowerSectionTitle = ictPowerTitleTextBox.Text;
                project.IctPowerSectionName = ictPowerTextBox.Text;
                project.IctPowerRunOrder = (int)ictPowerRunOrderNUDown.Value;
                project.IctPowerRetryEnabled = ictPowerRetryCheckBox.Checked;
                project.IctPowerPressDown = ictPowerPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.IctPowerPressUp = ictPowerPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.IctPowerIsEOL = ictPowerEolCheckBox.Checked;

                project.NovaEnabled = novaflashCheckBox.Checked;
                project.NovaSectionTitle = novaflashTitleTextBox.Text;
                project.NovaSectionName = novaflashTextBox.Text;
                project.NovaRunOrder = (int)ispRunOrderNUDown.Value;
                project.NovaRetryEnabled = novaflashRetryCheckBox.Checked;
                project.NovaPressDown = ispPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.NovaPressUp = ispPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.NovaIsEOL = ispEolCheckBox.Checked;

                project.JtagEnabled = jtagCheckBox.Checked;
                project.JtagSectionTitle = jtagTitleTextBox.Text;
                project.JtagSectionName = jtagTextBox.Text;
                project.JtagRunOrder = (int)jtagRunOrderNUDown.Value;
                project.JtagRetryEnabled = jtagRetryCheckBox.Checked;
                project.JtagPressDown = jtagPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.JtagPressUp = jtagPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.JtagIsEOL = jtagEolCheckBox.Checked;
                project.JtagProjectName = uutTextBox.Text;

                project.FunctionEnabled = functionCheckBox.Checked;
                project.FunctionSectionTitle = functionTitleTextBox.Text;
                project.FunctionSectionName = functionTextBox.Text;
                project.FunctionRunOrder = (int)functionRunOrderNUDown.Value;
                project.FunctionRetryEnabled = functionRetryCheckBox.Checked;
                project.FunctionPressDown = funcPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.FunctionPressUp = funcPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.FunctionIsEOL = funcEolCheckBox.Checked;

                project.EolEnabled = eolCheckBox.Checked;
                project.EolSectionTitle = eolTitleTextBox.Text;
                project.EolSectionName = eolTextBox.Text;
                project.EolRunOrder = (int)eolRunOrderNUDown.Value;
                project.EolRetryEnabled = eolRetryCheckBox.Checked;
                project.EolPressDown = eolPressDownCheckBox.CheckState != CheckState.Unchecked;
                project.EolPressUp = eolPressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.EolIsEOL = eolEolCheckBox.Checked;

                project.Ext1Enabled = ext1CheckBox.Checked;
                project.Ext1SectionTitle = ext1TitleTextBox.Text;
                project.Ext1SectionName = ext1TextBox.Text;
                project.Ext1RunOrder = (int)ext1RunOrderNUDown.Value;
                project.Ext1RetryEnabled = ext1RetryCheckBox.Checked;
                project.Ext1PressDown = ext1PressDownCheckBox.CheckState != CheckState.Unchecked;
                project.Ext1PressUp = ext1PressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.Ext1IsEOL = ext1EolCheckBox.Checked;

                project.Ext2Enabled = ext2CheckBox.Checked;
                project.Ext2SectionTitle = ext2TitleTextBox.Text;
                project.Ext2SectionName = ext2TextBox.Text;
                project.Ext2RunOrder = (int)ext2RunOrderNUDown.Value;
                project.Ext2RetryEnabled = ext2RetryCheckBox.Checked;
                project.Ext2PressDown = ext2PressDownCheckBox.CheckState != CheckState.Unchecked;
                project.Ext2PressUp = ext2PressDownCheckBox.CheckState == CheckState.Indeterminate;
                project.Ext2IsEOL = ext2EolCheckBox.Checked;

                project.Power1Voltage = (float)power1VoltageNumericUpDown.Value;
                project.Power1Current = (float)power1CurrentNumericUpDown.Value;
                project.Power1Enabled = power1UseCheckBox.Checked;
                project.Power2Voltage = (float)power2VoltageNumericUpDown.Value;
                project.Power2Current = (float)power2CurrentNumericUpDown.Value;
                project.Power2Enabled = power2UseCheckBox.Checked;

                project.MultiPanelBarcodeType = (MultiPanelBarcodeType)multiPanelBarcodeComboBox.SelectedItem;
                project.TwoBoardsLeftRight = twoBoardsDirectionCheckBox.Checked;
                project.BottomBoardFirst = bottomBoardFirstCheckBox.Checked;

                // GRP 정보 저장.
                var grpFiles = new List<GrpInfo>();
                var listViewFiles = grpListView.Objects?.Cast<GrpInfo>();
                if (listViewFiles != null)
                {
                    grpFiles.AddRange(listViewFiles);
                }
                project.GrpFiles = grpFiles;

                TestProjectManager.Save(project, CurrentProduct.ProjectPath);
                CurrentProduct.ProjectPath = project.Path;
                ProductSettingsViewModel.Save();

                InformationBox.Show("저장되었습니다.", "프로젝트 저장", icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                InformationBox.Show(ex.Message, "프로젝트 저장 오류", icon: InformationBoxIcon.Error);
            }
        }

        private string SelectElozProject(string prompt, string title)
        {
            try
            {
                return Eloz1.SelectProject(prompt, title);
            }
            catch (Exception e)
            {
                InformationBox.Show($"프로젝트 선택 오류: {e.Message}", "오류", icon: InformationBoxIcon.Error);
                return null;
            }
        }

        private void ictBrowseButton_Click(object sender, EventArgs e)
        {
            var projectName = SelectElozProject("테스트를 진행할 ICT 프로젝트를 선택하세요.", "ICT 테스트 프로젝트 선택");
            if (projectName != null)
            {
                ictTextBox.Text = projectName;
            }
        }

        private void masterGoodBrowseButton_Click(object sender, EventArgs e)
        {
            var projectName = SelectElozProject("테스트를 진행할 Master Good 프로젝트를 선택하세요.", "Master Good 테스트 프로젝트 선택");
            if (projectName != null)
            {
                masterGoodTextBox.Text = projectName;
            }
        }

        private void masterFailBrowseButton_Click(object sender, EventArgs e)
        {
            var projectName = SelectElozProject("테스트를 진행할 Master Fail 프로젝트를 선택하세요.", "Master Fail 테스트 프로젝트 선택");
            if (projectName != null)
            {
                masterFailTextBox.Text = projectName;
            }
        }

        private void novaflashBrowseButton_Click(object sender, EventArgs e)
        {
            string projectName = SelectElozProject("테스트를 진행할 Novaflash 프로젝트를 선택하세요.", "Novaflash 테스트 프로젝트 선택");
            if (projectName != null)
            {
                novaflashTextBox.Text = projectName;
            }
        }

        private string NovaflashSelectFile()
        {
            string filter = $"NovaFlash GRP Files(*.grp)|*.grp;|All Files(*.*)|*.*;";
            return SelectFile(null, filter);
        }

        private string SelectFile(string initialFolder, string filter)
        {
            var dialog = new OpenFileDialog();
            dialog.Filter = filter;
            if (initialFolder != null)
            {
                dialog.InitialDirectory = initialFolder;
            }
            dialog.RestoreDirectory = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private void power1ResetButton_Click(object sender, EventArgs e)
        {
            ResetPower(PowerSupply.Power1Name);
        }

        private void power2ResetButton_Click(object sender, EventArgs e)
        {
            ResetPower(PowerSupply.Power2Name);
        }

        private void ResetPower(string name)
        {
            PowerSupply power = null;
            try
            {
                power = new PowerSupply(name);
                power.Open();
                power.Reset();
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"{name} Reset Error: {ex.Message}");
            }
            finally
            {
                power?.Close();
            }
        }

        private void fidBrowseButton_Click(object sender, EventArgs e)
        {
            var dialog = new FidEditForm();
            if (project.FIDs != null)
            {
                dialog.FidList = new BindingList<int>(project.FIDs);
            }
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                project.FIDs = new List<int>(dialog.FidList);
                fidTextBox.Text = Utils.ListToString(project.FIDs, ",");
            }
        }

        private void uutBrowseButton_Click(object sender, EventArgs e)
        {
            try
            {
                // Debugging.
                string uutBaseDir = Cascon.GetUutBaseDir();
                Logger.LogInfo($"UUT Base Dir: {uutBaseDir}");

                var uutNames = Cascon.GetUutNames();
                var dlg = new NameSelectionForm(uutNames);
                dlg.Text = "Select a UUT";
                dlg.StartPosition = FormStartPosition.CenterParent;
                if (dlg.ShowDialog() == DialogResult.OK)
                {
                    uutTextBox.Text = dlg.Selected;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"JTAG: {ex.Message}");
            }
        }

        private void panelNumericUpDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateBoardCheckboxes();
        }

        private void UpdateBoardCheckboxes()
        {
            int panelCount = (int)panelNumericUpDown.Value;
            switch (panelCount)
            {
                case 2:
                    board1CheckBox.Enabled = true;
                    board2CheckBox.Enabled = true;
                    board3CheckBox.Enabled = false;
                    board3CheckBox.Checked = false;
                    board4CheckBox.Enabled = false;
                    board4CheckBox.Checked = false;
                    break;
                case 3:
                    board1CheckBox.Enabled = true;
                    board2CheckBox.Enabled = true;
                    board3CheckBox.Enabled = true;
                    board4CheckBox.Enabled = false;
                    board4CheckBox.Checked = false;
                    break;
                case 4:
                    board1CheckBox.Enabled = true;
                    board2CheckBox.Enabled = true;
                    board3CheckBox.Enabled = true;
                    board4CheckBox.Enabled = true;
                    break;
                case 1:
                default:
                    board1CheckBox.Enabled = true;
                    board1CheckBox.Checked = true;
                    board2CheckBox.Enabled = false;
                    board2CheckBox.Checked = false;
                    board3CheckBox.Enabled = false;
                    board3CheckBox.Checked = false;
                    board4CheckBox.Enabled = false;
                    board4CheckBox.Checked = false;
                    break;
            }
        }

        private void pcbInfoBrowseButton_Click(object sender, EventArgs e)
        {
            string fileName = SelectFile(null, "PCB Info Files(*.pcb_info)|*.pcb_info;|All Files(*.*)|*.*;");
            if (fileName != null)
            {
                pcbInfoTextBox.Text = fileName;
            }
        }

        private void sectionRunOrderNUDown_ValueChanged(object sender, EventArgs e)
        {
            UpdateEolCheckedStates();
        }

        private void eolCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            UpdateEolCheckedStates();
        }

        private void UpdateEolCheckedStates()
        {
            var controls = new (NumericUpDown RunOrderNUDown, CheckBox EolCheckBox)[]
            {
                (ictUnpowerRunOrderNUDown, ictUnpowerEolCheckBox),
                (ictPowerRunOrderNUDown, ictPowerEolCheckBox),
                (ispRunOrderNUDown, ispEolCheckBox),
                (jtagRunOrderNUDown, jtagEolCheckBox),
                (functionRunOrderNUDown, funcEolCheckBox),
                (eolRunOrderNUDown, eolEolCheckBox),
                (ext1RunOrderNUDown, ext1EolCheckBox),
                (ext2RunOrderNUDown, ext2EolCheckBox),
            };

            var sortedControls = controls.OrderBy(t => t.RunOrderNUDown.Value);
            var eolChecked = false;
            foreach (var control in sortedControls)
            {
                if (eolChecked)
                {
                    control.EolCheckBox.Checked = true;
                }
                else if (control.EolCheckBox.Checked)
                {
                    eolChecked = true;
                }
            }
        }

        private void twoBoardsDirectionCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            bottomBoardFirstCheckBox.Enabled = !twoBoardsDirectionCheckBox.Checked;
        }

        private string SelectRomFile()
        {
            string filter = "Intel Hex, Motorola S19, Binary (*.hex, *.s19, *.bin)|*.hex;*.s19;*.bin;|All Files (*.*)|*.*;";
            return SelectFile(null, filter);
        }

        private void addContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 프로젝트 추가.
                var grpInfo = new GrpInfo();
                grpListView.AddObject(grpInfo);
                grpListView.SelectObject(grpInfo);
            }
            catch (Exception ex)
            {
                Logger.LogError("GRP info creation error: " + ex.Message);
                Utils.ShowErrorDialog(ex);
            }
        }

        private void deleteContextMenuItem_Click(object sender, EventArgs e)
        {
            try
            {
                // 선택된 파일 실행 정보 삭제.
                var selectedObjects = grpListView.SelectedObjects;
                if (selectedObjects.Count == 0)
                {
                    return;
                }

                // 한번 더 물어본다.
                var result = MessageDialog.ShowModal($"선택된 {selectedObjects.Count}개의 GRP 정보를 삭제하시겠습니까?",
                    "GRP 정보 삭제", true, true);
                if (result != DialogResult.OK)
                {
                    return;
                }

                grpListView.RemoveObjects(selectedObjects);
            }
            catch (Exception ex)
            {
                Logger.LogError("GRP info deleting error: " + ex.Message);
                Utils.ShowErrorDialog(ex);
            }
        }

        private void grpContextMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteContextMenuItem.Enabled = grpListView.SelectedObjects.Count > 0;
        }

        private void grpListView_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void grpListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            // 편집하는 컨트롤의 경계가 Cell 경계에 딱 맞도록 함.
            e.Control.Bounds = e.CellBounds;

            // 편집 컨트롤 추가 설정.
            switch (e.Column.AspectName)
            {
                case nameof(GrpInfo.Channel):
                    if (e.Control is NumericUpDown nuDown)
                    {
                        nuDown.Minimum = 1;
                        nuDown.Maximum = 4;
                    }
                    break;
                case nameof(GrpInfo.RunOrder):
                    if (e.Control is NumericUpDown runOrderNUDown)
                    {
                        runOrderNUDown.Minimum = 1;
                    }
                    break;
                case nameof(GrpInfo.Crc):
                    var hexControl = new HexNumericUpDown();
                    hexControl.Minimum = uint.MinValue;
                    hexControl.Maximum = uint.MaxValue;
                    hexControl.Bounds = e.CellBounds;
                    hexControl.TextAlign = e.Column.TextAlign;
                    if (e.RowObject is GrpInfo info)
                    {
                        hexControl.Value = info.Crc;
                    }
                    e.Control = hexControl;
                    break;
                case nameof(GrpInfo.GrpFilePath):
                case nameof(GrpInfo.ImportFilePath):
                    var textBox = e.Control as TextBox;
                    textBox.Bounds = new Rectangle(0, 0, e.CellBounds.Width - e.CellBounds.Height - 1, e.CellBounds.Height);

                    var browseButton = new Button();
                    browseButton.UseVisualStyleBackColor = true;
                    browseButton.Bounds = new Rectangle(e.CellBounds.Width - e.CellBounds.Height - 1, -1, e.CellBounds.Height + 1, e.CellBounds.Height + 1);
                    browseButton.Text = "...";
                    browseButton.Click += (s, args) =>
                    {
                        var dialog = new OpenFileDialog();
                        dialog.RestoreDirectory = true;

                        var path = e.Value?.ToString();
                        if (path != null)
                        {
                            dialog.FileName = path;
                        }

                        if (e.Column.AspectName == nameof(GrpInfo.GrpFilePath))
                        {
                            dialog.Title = "GRP 파일 선택";
                            dialog.Filter = "NovaFlash GRP (*.grp)|*.grp;|All Files (*.*)|*.*;";
                        }
                        else
                        {
                            dialog.Title = "Import 파일 선택";
                            dialog.Filter = "Intel Hex (*.hex)|*.hex;|Motorola S19 (*.s19)|*.s19;|Binary (*.bin)|*.bin;|All Files (*.*)|*.*;";
                        }

                        if (dialog.ShowDialog() == DialogResult.OK)
                        {
                            textBox.Text = dialog.FileName;
                        }
                    };

                    var panel = new Panel();
                    panel.Bounds = e.CellBounds;
                    panel.Controls.Add(textBox);
                    panel.Controls.Add(browseButton);
                    e.Control = panel;
                    break;
            }
        }

        private void grpListView_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            if (e.Cancel)
            {
                return;
            }

            switch (e.Column.AspectName)
            {
                case nameof(GrpInfo.Crc):
                    e.NewValue = (uint)(e.Control as HexNumericUpDown).Value;
                    break;
                case nameof(GrpInfo.GrpFilePath):
                case nameof(GrpInfo.ImportFilePath):
                    var panel = e.Control as Panel;
                    var textBox = panel.Controls[0] as TextBox;
                    if (textBox == null)
                    {
                        textBox = panel.Controls[1] as TextBox;
                    }
                    e.NewValue = textBox.Text;
                    break;
            }
        }
    }
}
