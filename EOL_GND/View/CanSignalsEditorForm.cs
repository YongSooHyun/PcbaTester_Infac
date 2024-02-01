using BrightIdeasSoftware;
using DbcParserLib;
using EOL_GND.Common;
using EOL_GND.Model;
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
    public partial class CanSignalsEditorForm : Form
    {
        /// <summary>
        /// 편집한 시그널 리스트.
        /// </summary>
        public List<CanSignal> Signals => viewModel?.CanSignals;

        private readonly CanSignalsEditorViewModel viewModel;

        public CanSignalsEditorForm(IEnumerable<object> signals)
        {
            InitializeComponent();

            signalListView.CellEditUseWholeCell = true;
            viewModel = new CanSignalsEditorViewModel(signals);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateListViewRowHeight(signalListView);
            UpdateSignalDeleteButton();
            UpdateSignalMoveUpButton();
            UpdateSignalMoveDownButton();

            // 리스트 뷰 항목 보여주기.
            signalListView.SetObjects(viewModel.CanSignals);
        }

        private void UpdateListViewRowHeight(ObjectListView listView)
        {
            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = listView.Font;
            Controls.Add(dummyComboBox);
            listView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);
        }

        private void UpdateSignalDeleteButton()
        {
            signalDeleteButton.Enabled = signalListView.SelectedObjects.Count > 0;
        }

        private void UpdateSignalMoveUpButton()
        {
            signalMoveUpButton.Enabled = signalListView.SelectedIndex > 0;
        }

        private void UpdateSignalMoveDownButton()
        {
            signalMoveDownButton.Enabled = signalListView.SelectedIndex >= 0 && signalListView.SelectedIndex < signalListView.Items.Count - 1;
        }

        #region Ok, Cancel Buttons

        private void okButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.OK;
        }

        private void cancelButton_Click(object sender, EventArgs e)
        {
            DialogResult = DialogResult.Cancel;
        }

        #endregion // Ok, Cancel Buttons

        #region ToolStrip Event Handlers

        private void signalAddButton_Click(object sender, EventArgs e)
        {
            AddSignal();
        }

        private void AddSignal()
        {
            signalListView.CancelCellEdit();
            var newSignal = viewModel.CreateSignal();
            if (newSignal != null)
            {
                signalListView.AddObject(newSignal);
                signalListView.SelectObject(newSignal);
            }
        }

        private void signalDeleteButton_Click(object sender, EventArgs e)
        {
            DeleteSignals();
        }

        private void DeleteSignals()
        {
            try
            {
                var modelObjects = signalListView.SelectedObjects;
                if (modelObjects.Count == 0)
                {
                    return;
                }

                // 한번 더 물어본다.
                var answer = InformationBox.Show($"선택된 {modelObjects.Count}개의 시그널을 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                if (answer != InformationBoxResult.OK)
                {
                    return;
                }

                signalListView.CancelCellEdit();
                viewModel.DeleteSignals(modelObjects);
                signalListView.RemoveObjects(modelObjects);
            }
            catch (Exception ex)
            {
                Logger.LogError($"CAN signals deleting error: {ex.Message}");
            }
        }

        private void signalMoveUpButton_Click(object sender, EventArgs e)
        {
            MoveUpSignal();
        }

        private void MoveUpSignal()
        {
            var selectedIndex = signalListView.SelectedIndex;
            if (selectedIndex > 0)
            {
                var selectedObject = signalListView.SelectedObject;
                if (viewModel.MoveUpSignal(selectedObject, selectedIndex))
                {
                    signalListView.RemoveObject(selectedObject);
                    signalListView.InsertObjects(selectedIndex - 1, new[] { selectedObject });
                    signalListView.SelectObject(selectedObject);
                }
            }
        }

        private void signalMoveDownButton_Click(object sender, EventArgs e)
        {
            MoveDownSignal();
        }

        private void MoveDownSignal()
        {
            var selectedIndex = signalListView.SelectedIndex;
            if (selectedIndex >= 0 && selectedIndex < signalListView.Items.Count - 1)
            {
                var selectedObject = signalListView.SelectedObject;
                if (viewModel.MoveDownSignal(selectedObject, selectedIndex))
                {
                    signalListView.RemoveObject(selectedObject);
                    signalListView.InsertObjects(selectedIndex + 1, new[] { selectedObject });
                    signalListView.SelectObject(selectedObject);
                }
            }
        }

        #endregion // ToolStrip Event Handlers

        #region Context Menu

        private void signalContextMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteSignalsItem.Enabled = signalListView.SelectedObjects.Count > 0;
            moveUpItem.Enabled = signalListView.SelectedIndex > 0;
            moveDownItem.Enabled = signalListView.SelectedIndex >= 0 && signalListView.SelectedIndex < signalListView.Items.Count - 1;
        }

        private void addSignalItem_Click(object sender, EventArgs e)
        {
            AddSignal();
        }

        private void deleteSignalsItem_Click(object sender, EventArgs e)
        {
            DeleteSignals();
        }

        private void moveUpItem_Click(object sender, EventArgs e)
        {
            MoveUpSignal();
        }

        private void moveDownItem_Click(object sender, EventArgs e)
        {
            MoveDownSignal();
        }

        #endregion // Context Menu

        #region ListView Event Handlers

        private void signalListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void signalListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateSignalDeleteButton();
            UpdateSignalMoveUpButton();
            UpdateSignalMoveDownButton();
        }

        private void signalListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            // Custom editing.
            if (CanSignalsEditorViewModel.EditProperty(e.RowObject, e.Column.AspectName, e.Value))
            {
                e.Cancel = true;
                signalListView.RefreshObject(e.RowObject);
            }
        }

        private void signalListView_CellToolTipShowing(object sender, ToolTipShowingEventArgs e)
        {
            var toolTipText = CanSignalsEditorViewModel.GetToolTipText(e.Model, e.Column.AspectName);
            if (!string.IsNullOrEmpty(toolTipText))
            {
                e.Text = toolTipText;
                e.Font = new Font("Consolas", 9);
            }
        }

        #endregion // ListView Event Handlers
    }
}
