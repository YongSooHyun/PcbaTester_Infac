using BrightIdeasSoftware;
using EOL_GND.ViewModel;
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
    public partial class DioCommandsForm : Form
    {
        /// <summary>
        /// 편집한 명령 리스트.
        /// </summary>
        internal List<Device.DioDevice.CommandInfo> Commands => viewModel?.Commands;

        /// <summary>
        /// 사용자가 선택한 명령 리스트.
        /// </summary>
        internal List<string> CheckedCommands => viewModel?.CheckedCommands;

        private readonly DioCommandsViewModel viewModel;
        private readonly bool editMode = true;
        private bool isLoading = true;

        internal DioCommandsForm(DioCommandsViewModel viewModel, bool edit)
        {
            InitializeComponent();

            commandsListView.CellEditUseWholeCell = true;
            this.viewModel = viewModel;
            editMode = edit;
            if (editMode)
            {
                okButton.Visible = false;
                commandsListView.CheckBoxes = false;
                noColumn.TextAlign = HorizontalAlignment.Center;
                commandsTextBox.Visible = false;
            }
            else
            {
                commandsListView.MultiSelect = false;
                commandsListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
                addButton.Visible = false;
                removeButton.Visible = false;
                ShowCheckedCommands();
            }
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = commandsListView.Font;
            Controls.Add(dummyComboBox);
            commandsListView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);

            Init();
        }

        private void Init()
        {
            if (viewModel == null)
            {
                return;
            }

            // Commands 보여주기.
            commandsListView.SetObjects(viewModel.Commands);

            // Remove 버튼 Disable.
            removeButton.Enabled = false;

            // 오브젝트 체크.
            commandsListView.CheckObjects(viewModel.GetCheckedObjects());

            isLoading = false;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 명령 이름에 따라 정렬.
            commandsListView.Sort(commandColumn);
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            var command = viewModel.CreateNewCommand();
            commandsListView.AddObject(command);
        }

        private void removeButton_Click(object sender, EventArgs e)
        {
            if (viewModel.RemoveObjects(commandsListView.SelectedObjects))
            {
                commandsListView.RemoveObjects(commandsListView.SelectedObjects);
            }
        }

        private void commandsListView_SelectionChanged(object sender, EventArgs e)
        {
            removeButton.Enabled = commandsListView.SelectedObjects.Count > 0;
        }

        private void commandsListView_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            // 행 번호.
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void commandsListView_CellEditFinishing(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            e.NewValue = viewModel.AfterEdit(e.Column.AspectName, e.NewValue);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void commandsListView_ItemActivate(object sender, EventArgs e)
        {
            okButton.PerformClick();
        }

        private void commandsListView_CellEditStarting(object sender, BrightIdeasSoftware.CellEditEventArgs e)
        {
            e.Control.Bounds = e.CellBounds;
        }

        private void commandsListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (isLoading)
            {
                return;
            }

            var olvItem = e.Item as OLVListItem;
            viewModel.CheckObject(olvItem.RowObject, olvItem.Checked);
            ShowCheckedCommands();
        }

        private void ShowCheckedCommands()
        {
            commandsTextBox.Text = string.Join(", ", CheckedCommands);
        }
    }
}
