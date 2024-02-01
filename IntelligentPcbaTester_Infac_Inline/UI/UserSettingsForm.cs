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
    public partial class UserSettingsForm : Form
    {
        // 삭제할 때 다시 한번 물어볼 것인지 설정하는 깃발.
        private bool confirmDeletion = true;

        public UserSettingsForm()
        {
            InitializeComponent();

            userDataGridView.DefaultCellStyle.DataSourceNullValue = null;

            roleColumn.DataSource = Enum.GetValues(typeof(UserRole));
            roleColumn.ValueType = typeof(UserRole);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init();
        }

        private void Init()
        {
            // 유저 정보를 불러와 보여준다.
            try
            {
                UserManager manager = AppSettings.GetUserManager();
                userBindingSource.DataSource = manager.Users;
            }
            catch (Exception e)
            {
                InformationBox.Show(e.Message, "사용자 정보 로딩 오류", icon: InformationBoxIcon.Error);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // 제일 마지막에 추가.
            var addingUser = new User();
            userBindingSource.Add(addingUser);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (confirmDeletion)
            {
                var result = InformationBox.Show($"{userDataGridView.SelectedRows.Count}개의 항목을 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
                if (result == InformationBoxResult.Cancel)
                {
                    return;
                }
            }

            foreach (DataGridViewRow selectedRow in userDataGridView.SelectedRows)
            {
                userBindingSource.RemoveAt(selectedRow.Index);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                UserManager manager = AppSettings.GetUserManager();
                manager.Save();

                InformationBox.Show("저장되었습니다.", "사용자 정보 저장", icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                InformationBox.Show(ex.Message, "사용자 정보 저장 오류", icon: InformationBoxIcon.Error);
            }
        }

        private void userDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // 선택된 행이 있을 때만 삭제 버튼을 enable 시킨다.
            deleteButton.Enabled = userDataGridView.SelectedRows.Count > 0;
        }

        private void userDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var users = userBindingSource.DataSource as List<User>;
            if (users != null)
            {
                User currentUser = users[e.RowIndex];
                string propertyName = userDataGridView.Columns[e.ColumnIndex].DataPropertyName;
                if (!UserManager.ValidateValue(users, currentUser, propertyName, e.FormattedValue.ToString(), out string errorMessage))
                {
                    userDataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
                    e.Cancel = true;
                }
            }
        }

        private void userDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            userDataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
        }
    }
}
