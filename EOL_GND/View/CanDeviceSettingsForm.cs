using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.ViewModel;
using InfoBox;
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
    public partial class CanDeviceSettingsForm : Form
    {
        private readonly CanDeviceSettingsViewModel viewModel = new CanDeviceSettingsViewModel();

        public CanDeviceSettingsForm()
        {
            InitializeComponent();

            listView.CellEditUseWholeCell = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = listView.Font;
            Controls.Add(dummyComboBox);
            listView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);

            listView.SetObjects(viewModel.GetDeviceSettings());

            // 사용자 권한 설정.
            var permission = GeneralSettingsViewModel.GetUserPermission();
            var editAllowed = permission?.CanEditDeviceSettings ?? false;
            mainToolStrip.Enabled = editAllowed;
            listView.CellEditActivation = editAllowed ? ObjectListView.CellEditActivateMode.DoubleClick : ObjectListView.CellEditActivateMode.None;
        }

        private void ListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            // 행 번호를 보여준다.
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void listView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            viewModel.ConfigureEditingControl(e.Column.AspectName, e.Control);
            e.Control.Bounds = e.CellBounds;
            if (e.Control is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (e.Control is NumericUpDown nuDown)
            {
                nuDown.Select(0, nuDown.Text.Length);
            }
        }

        private void listView_CellEditValidating(object sender, CellEditEventArgs e)
        {
            if (!viewModel.ValidateValue(e.Column.AspectName, e.NewValue, out string errorMessage))
            {
                InformationBox.Show(errorMessage, "Validation Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                e.Cancel = true;
            }
        }

        private void saveTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                listView.CancelCellEdit();
                viewModel.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void addTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                listView.CancelCellEdit();
                var newSetting = viewModel.CreateSetting();
                listView.AddObject(newSetting);
                listView.SelectObject(newSetting);
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void deleteTSButton_Click(object sender, EventArgs e)
        {
            try
            {
                var rowObjects = listView.SelectedObjects;
                if (rowObjects.Count > 0)
                {
                    // 한번 더 물어본다.
                    var answer = InformationBox.Show($"선택된 {rowObjects.Count}개의 디바이스를 삭제하시겠습니까?",
                        "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                    if (answer != InformationBoxResult.OK)
                    {
                        return;
                    }

                    listView.CancelCellEdit();

                    viewModel.DeleteSettings(rowObjects);
                    listView.RemoveObjects(rowObjects);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"Error: {ex.Message}");
                Utils.ShowErrorDialog(ex);
            }
        }

        private void listView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateRemoveButton();
        }

        // 스텝 삭제 버튼을 Enable/Disable 한다.
        private void UpdateRemoveButton()
        {
            deleteTSButton.Enabled = listView.SelectedObjects?.Count > 0;
        }
    }
}
