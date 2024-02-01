using BrightIdeasSoftware;
using EOL_GND.Common;
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
    public partial class ChangeHistoryForm : Form
    {
        /// <summary>
        /// 히스토리를 보여줄 텍스트 파일.
        /// </summary>
        internal string FilePath { get; set; }

        private readonly ChangeHistoryViewModel viewModel = new ChangeHistoryViewModel();

        public ChangeHistoryForm()
        {
            InitializeComponent();
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            try
            {
                // Font 크기에 맞게 list view row height를 조정한다.
                UpdateListViewRowHeight(historyListView);

                // 데이터 표시.
                viewModel.Load(FilePath);
                historyListView.SetObjects(viewModel.GetChanges());
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
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

        private void historyListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void historyListView_ItemActivate(object sender, EventArgs e)
        {
            try
            {
                if (historyListView.SelectedObject == null)
                {
                    return;
                }

                ShowNewText(historyListView.SelectedObject);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void historyContextMenu_Opening(object sender, CancelEventArgs e)
        {
            var selectedCount = historyListView.SelectedObjects.Count;
            differenceItem.Enabled = selectedCount == 1 || selectedCount == 2;
            mergeItem.Enabled = selectedCount > 1 && ChangeHistoryViewModel.GetUserPermission()?.CanEditSequence == true;
        }

        private void differenceItem_Click(object sender, EventArgs e)
        {
            try
            {
                Enabled = false;

                var selectedCount = historyListView.SelectedObjects.Count;
                object firstObj, secondObj;
                if (selectedCount == 1)
                {
                    firstObj = null;
                    secondObj = historyListView.SelectedObjects[0];
                }
                else if (selectedCount == 2)
                {
                    firstObj = historyListView.SelectedObjects[1];
                    secondObj = historyListView.SelectedObjects[0];
                }
                else
                {
                    return;
                }

                // 현재 선택한 레코드와 이전 레코드의 텍스트를 비교하는 뷰를 보여준다.
                var leftText = viewModel.GetText(firstObj, out DateTime leftTime);
                var rightText = viewModel.GetText(secondObj, out DateTime rightTime);
                var dialog = new DiffForm();
                dialog.StartPosition = FormStartPosition.CenterParent;
                dialog.WindowState = FormWindowState.Maximized;
                dialog.ShowInTaskbar = false;
                dialog.OldText = leftText;
                dialog.OldTime = leftTime;
                dialog.NewText = rightText;
                dialog.NewTime = rightTime;
                dialog.ShowDialog(this);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
            finally
            {
                Enabled = true;
            }
        }

        // 지정한 레코드에 해당하는 텍스트를 보여준다.
        private void ShowNewText(object recordObject)
        {
            var newText = viewModel.GetText(recordObject, out DateTime newTime);
            var dialog = new TextViewer();
            dialog.Content = newText;
            dialog.Text = newTime.ToString();
            dialog.StartPosition = FormStartPosition.CenterParent;
            dialog.ShowDialog(this);
        }

        private void initialVersionItem_Click(object sender, EventArgs e)
        {
            try
            {
                ShowNewText(null);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void newestItem_Click(object sender, EventArgs e)
        {
            try
            {
                MergeRecords(true);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void oldestItem_Click(object sender, EventArgs e)
        {
            try
            {
                MergeRecords(false);
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void MergeRecords(bool intoLast)
        {
            var selectedObjects = historyListView.SelectedObjects;
            var mergedObject = viewModel.MergeRecords(selectedObjects, intoLast);
            if (mergedObject != null)
            {
                var firstIndex = historyListView.IndexOf(selectedObjects[0]);
                historyListView.RemoveObjects(selectedObjects);
                historyListView.InsertObjects(firstIndex, new[] { mergedObject });
                historyListView.SelectObject(mergedObject);

                // 병합 내용을 저장한다.
                //viewModel.Save(FilePath);
            }
        }
    }
}
