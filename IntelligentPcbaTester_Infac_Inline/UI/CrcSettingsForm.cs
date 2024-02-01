using BrightIdeasSoftware;
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
    public partial class CrcSettingsForm : Form
    {
        // 삭제할 때 다시 한번 물어볼 것인지 설정하는 깃발.
        private bool confirmDeletion = true;

        public CrcSettingsForm()
        {
            InitializeComponent();

            crcListView.CellEditUseWholeCell = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init();
            UpdateDeleteButton();
        }

        private void Init()
        {
            // CRC Table 정보를 불러와 보여준다.
            try
            {
                // ComboBox 높이에 맞게 Row Height를 조정한다.
                var dummyComboBox = new ComboBox();
                dummyComboBox.Font = crcListView.Font;
                Controls.Add(dummyComboBox);
                crcListView.RowHeight = dummyComboBox.Height;
                Controls.Remove(dummyComboBox);

                crcListView.SetObjects(CrcTable.SharedInstance.Records);
            }
            catch (Exception ex)
            {
                Logger.LogError($"CRC table error: {ex.Message}");
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // 제일 마지막에 추가.
            var newRecord = new CrcRecord();
            CrcTable.SharedInstance.Records.Add(newRecord);
            crcListView.AddObject(newRecord);
            crcListView.SelectObject(newRecord);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            try
            {
                var selectedObjects = crcListView.SelectedObjects;
                if (selectedObjects.Count > 0)
                {
                    if (confirmDeletion)
                    {
                        var answer = InformationBox.Show($"선택된 {selectedObjects.Count}개의 레코드를 삭제하시겠습니까?",
                            "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                        if (answer != InformationBoxResult.OK)
                        {
                            return;
                        }
                    }

                    crcListView.CancelCellEdit();

                    foreach (var obj in selectedObjects)
                    {
                        CrcTable.SharedInstance.Records.Remove(obj as CrcRecord);
                    }

                    crcListView.RemoveObjects(selectedObjects);
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"CRC Table: {ex.Message}");
                InformationBox.Show(ex.Message, "CRC 정보 삭제 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                CrcTable.SharedInstance.Save();
                InformationBox.Show("저장되었습니다.", "CRC 테이블 저장", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                Logger.LogError($"CRC Table: {ex.Message}");
                InformationBox.Show(ex.Message, "CRC 정보 저장 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
        }

        private void crcListView_FormatRow(object sender, BrightIdeasSoftware.FormatRowEventArgs e)
        {
            // 행 번호를 보여준다.
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void crcListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            try
            {
                if (e.Control is NumericUpDown nuDown)
                {
                    var hexNUDown = new HexNumericUpDown
                    {
                        Maximum = uint.MaxValue,
                        Minimum = uint.MinValue,
                        Value = nuDown.Value,
                        Bounds = nuDown.Bounds,
                        TextAlign = nuDown.TextAlign,
                    };
                    hexNUDown.Select(0, hexNUDown.Text.Length);
                    e.Control = hexNUDown;
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"CRC Table: {ex.Message}");
            }
        }

        private void crcListView_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            try
            {
                if (!e.Cancel && e.RowObject is CrcRecord record && e.NewValue is decimal decimalValue)
                {
                    switch (e.Column.AspectName)
                    {
                        case nameof(CrcRecord.MesCrc):
                            record.MesCrc = (uint)decimalValue;
                            break;
                        case nameof(CrcRecord.NovaCrc):
                            record.NovaCrc = (uint)decimalValue;
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogError($"CRC Table: {ex.Message}");
            }
        }

        private void crcListView_SelectionChanged(object sender, EventArgs e)
        {
            UpdateDeleteButton();
        }

        private void UpdateDeleteButton()
        {
            deleteButton.Enabled = crcListView.SelectedObjects.Count > 0;
        }
    }
}
