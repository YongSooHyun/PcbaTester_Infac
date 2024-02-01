using BrightIdeasSoftware;
using DbcParserLib;
using EOL_GND.Common;
using EOL_GND.Device;
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
using System.Windows.Media.Animation;

namespace EOL_GND.View
{
    public partial class VariantEditorForm : Form
    {
        internal SequenceViewModel ViewModel { get; set; }
        internal List<string> CheckedVariants { get; set; }

        private bool isLoading = true;

        public VariantEditorForm(bool selectionMode)
        {
            InitializeComponent();

            variantListView.CellEditUseWholeCell = true;
            variantListView.CellEditActivation = selectionMode ? ObjectListView.CellEditActivateMode.None : ObjectListView.CellEditActivateMode.DoubleClick;
            variantListView.CheckBoxes = selectionMode;
            variantToolStrip.Visible = !selectionMode;
            variantsTextBox.Visible = selectionMode;
            okButton.Visible = selectionMode;
            closeButton.Text = selectionMode ? "&Cancel" : "&Close";
            variantListView.ContextMenuStrip = selectionMode ? null : variantContextMenu;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateListViewRowHeight(variantListView);
            UpdateVariantDeleteButton();

            if (ViewModel == null)
            {
                foreach (var form in Application.OpenForms)
                {
                    if (form is SequenceForm seqForm)
                    {
                        ViewModel = seqForm.ViewModel;
                    }
                }
            }

            if (CheckedVariants == null)
            {
                CheckedVariants = new List<string>();
            }

            // 리스트 뷰 항목 보여주기.
            if (ViewModel != null)
            {
                variantListView.SetObjects(ViewModel.Variants);
            }

            ShowCheckedVariants();

            // 오브젝트 체크.
            variantListView.CheckObjects(GetCheckedObjects());

            isLoading = false;
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

        private void UpdateVariantDeleteButton()
        {
            variantDeleteButton.Enabled = variantListView.SelectedObject != null;
        }

        #region Variant Selection

        private void ShowCheckedVariants()
        {
            if (CheckedVariants != null)
            {
                variantsTextBox.Text = string.Join(",", CheckedVariants);
            }
        }

        internal List<object> GetCheckedObjects()
        {
            var checkedObjects = new List<object>();

            if (ViewModel?.Variants != null && CheckedVariants != null)
            {
                foreach (var checkedVariantName in CheckedVariants)
                {
                    foreach (var variant in ViewModel.Variants)
                    {
                        if (string.Equals(checkedVariantName, variant.Name, StringComparison.OrdinalIgnoreCase))
                        {
                            checkedObjects.Add(variant);
                        }
                    }
                }
            }

            return checkedObjects;
        }

        #endregion // Variant Selection

        #region ToolStrip Event Handlers

        private void variantAddButton_Click(object sender, EventArgs e)
        {
            AddVariant();
        }

        private void AddVariant()
        {
            if (ViewModel == null)
            {
                return;
            }

            variantListView.CancelCellEdit();
            var newVariant = ViewModel.CreateVariant();
            if (newVariant != null)
            {
                ViewModel.AddVariant(newVariant);
                variantListView.AddObject(newVariant);
                variantListView.SelectObject(newVariant);
                variantListView.EnsureModelVisible(newVariant);
            }
        }

        private void variantDeleteButton_Click(object sender, EventArgs e)
        {
            DeleteVariant();
        }

        private void DeleteVariant()
        {
            try
            {
                var selectedObj = variantListView.SelectedObject;
                if (selectedObj == null)
                {
                    return;
                }

                // 삭제하려는 variant가 포함된 스텝이 있는지 체크.
                if (ViewModel.ContainsVariantStep(ViewModel.OriginalSteps, selectedObj, false))
                {
                    // 삭제하려는 variant가 모든 스텝들에서 삭제된다는 것을 경고, 대답에 따라 삭제.
                    var answer = InformationBox.Show($"선택된 variant를 포함한 스텝들이 있습니다.\r\n" +
                        $"variant를 삭제하면 이 스텝들에서도 삭제됩니다.\r\n삭제하시겠습니까?",
                        "variant 삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                    if (answer != InformationBoxResult.OK)
                    {
                        return;
                    }
                }

                variantListView.CancelCellEdit();
                ViewModel.RemoveVariant(selectedObj);
                variantListView.RemoveObjects(new[] {selectedObj});
            }
            catch (Exception ex)
            {
                Logger.LogError($"Variant deleting error: {ex.Message}");
            }
        }

        #endregion // ToolStrip Event Handlers

        #region Context Menu

        private void variantContextMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteVariantItem.Enabled = variantListView.SelectedObjects.Count > 0;
        }

        private void addVariantItem_Click(object sender, EventArgs e)
        {
            AddVariant();
        }

        private void deleteVariantItem_Click(object sender, EventArgs e)
        {
            DeleteVariant();
        }

        #endregion // Context Menu

        #region ListView Event Handlers

        private void variantListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void variantListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateVariantDeleteButton();
        }

        private void variantListView_ItemChecked(object sender, ItemCheckedEventArgs e)
        {
            if (isLoading || CheckedVariants == null)
            {
                return;
            }

            var olvItem = e.Item as OLVListItem;
            var variant = olvItem.RowObject as BoardVariant;
            if (olvItem.Checked)
            {
                CheckedVariants.Add(variant.Name);
            }
            else
            {
                for (int i = CheckedVariants.Count - 1; i >= 0; i--)
                {
                    if (string.Equals(CheckedVariants[i], variant.Name, StringComparison.OrdinalIgnoreCase))
                    {
                        CheckedVariants.RemoveAt(i);
                    }
                }
            }

            ShowCheckedVariants();
        }

        #endregion // ListView Event Handlers
    }
}
