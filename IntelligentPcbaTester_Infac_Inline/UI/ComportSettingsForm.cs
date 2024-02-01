using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.IO.Ports;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class ComportSettingsForm : Form
    {
        // COM 포트 설정 리스트.
        private ComportSettingsManager manager;

        public ComportSettingsForm()
        {
            InitializeComponent();

            portSettingsDataGridView.DefaultCellStyle.DataSourceNullValue = null;

            portColumn.DataSource = Enum.GetValues(typeof(ComPort));
            portColumn.ValueType = typeof(ComPort);

            parityColumn.DataSource = Enum.GetValues(typeof(Parity));
            parityColumn.ValueType = typeof(Parity);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init();
        }

        private void Init()
        {
            // 설정 정보를 불러와 보여준다.
            try
            {
                manager = ComportSettingsManager.Load();
                comportSettingsBindingSource.DataSource = manager.ComSettings;
            }
            catch (Exception e)
            {
                InformationBox.Show(e.Message, "COM포트 설정 로딩 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
        }

        private void saveButton_Click(object sender, EventArgs e)
        {
            try
            {
                manager.Save();
                InformationBox.Show("저장되었습니다.", "COM포트 설정 저장", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                InformationBox.Show(ex.Message, "COM포트 설정 저장 오류", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
            }
        }

        private void portSettingsDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            var errorMessage = new StringBuilder();
            if (!ComportSettingsManager.ValidateValue(portSettingsDataGridView.Columns[e.ColumnIndex].DataPropertyName,
                e.FormattedValue.ToString(), errorMessage))
            {
                portSettingsDataGridView[0, e.RowIndex].ErrorText = errorMessage.ToString();
                e.Cancel = true;
            }
        }

        private void portSettingsDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            portSettingsDataGridView[0, e.RowIndex].ErrorText = string.Empty;
        }

        private void ComportSettingsForm_Shown(object sender, EventArgs e)
        {
            for (int row = 0; row < manager.ComSettings.Count; row++)
            {
                var cell = portSettingsDataGridView[1, row];
                var isReadOnly = manager.ComSettings[row].Mandatory;
                cell.ReadOnly = isReadOnly;
                if (isReadOnly)
                {
                    cell.Style.ForeColor = Color.Gray;
                }
            }
        }
    }
}
