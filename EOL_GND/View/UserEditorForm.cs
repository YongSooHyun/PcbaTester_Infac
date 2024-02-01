using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.Model.DBC;
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
    public partial class UserEditorForm : Form
    {
        private readonly UserEditorViewModel viewModel = new UserEditorViewModel();

        public UserEditorForm()
        {
            InitializeComponent();

            userListView.CellEditUseWholeCell = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Password 칼럼 설정.
            passwordColumn.AspectToStringConverter = delegate (object obj)
            {
                string password = obj as string;
                if (string.IsNullOrEmpty(password))
                {
                    return string.Empty;
                }
                else
                {
                    return new string('●', password.Length);
                }
            };

            // Font 크기에 맞게 list view row height를 조정한다.
            UpdateListViewRowHeight(userListView);

            // 데이터 표시.
            userListView.SetObjects(viewModel.GetUsers());

            UpdateDeleteButton();
        }

        // Font 크기에 맞게 list view row height를 조정한다.
        private void UpdateListViewRowHeight(ObjectListView listView)
        {
            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = listView.Font;
            Controls.Add(dummyComboBox);
            listView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);
        }

        private void UpdateDeleteButton()
        {
            deleteButton.Enabled = userListView.SelectedObjects.Count > 0;
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                userListView.CancelCellEdit();
                viewModel.Save();
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            try
            {
                userListView.CancelCellEdit();
                var newUser = viewModel.CreateUser();
                userListView.AddObject(newUser);
                userListView.SelectObject(newUser);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var modelObjects = userListView.SelectedObjects;
                if (modelObjects.Count == 0)
                {
                    return;
                }

                // 한번 더 물어본다.
                var answer = InformationBox.Show($"선택된 {modelObjects.Count}개의 사용자를 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                if (answer != InformationBoxResult.OK)
                {
                    return;
                }

                userListView.CancelCellEdit();
                viewModel.DeleteUsers(modelObjects);
                userListView.RemoveObjects(modelObjects);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void userListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDeleteButton();
        }

        private void userListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void userListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            viewModel.ConfigureEditingControl(e.Column.AspectName, e.Control);
            e.Control.Bounds = e.CellBounds;
        }
    }
}
