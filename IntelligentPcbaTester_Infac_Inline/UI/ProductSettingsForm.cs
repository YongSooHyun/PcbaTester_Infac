using InfoBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class ProductSettingsForm : Form
    {
        // 삭제할 때 다시 한번 물어볼 것인지 설정하는 깃발.
        private bool confirmDeletion = true;

        // 제품 정보 리스트.
        private List<Product> products;

        public ProductSettingsForm()
        {
            InitializeComponent();

            productDataGridView.DefaultCellStyle.DataSourceNullValue = null;
            productDataGridView.DefaultCellStyle.Font = new System.Drawing.Font("Consolas", 9);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            Init();
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            projectPathColumn.AutoSizeMode = DataGridViewAutoSizeColumnMode.None;
        }

        private void Init()
        {
            // 제품 정보를 불러와 보여준다.
            try
            {
                products = ProductSettingsViewModel.GetProducts();
                productBindingSource.DataSource = products;
            }
            catch (Exception e)
            {
                InformationBox.Show(e.Message, "제품정보 로딩 오류", icon: InformationBoxIcon.Error);
            }
        }

        private void saveButton_Click(object sender, System.EventArgs e)
        {
            try
            {
                ProductSettingsViewModel.Save();
                InformationBox.Show("저장되었습니다.", "제품정보 저장", icon: InformationBoxIcon.Success);
            }
            catch (Exception ex)
            {
                InformationBox.Show(ex.Message, "제품정보 저장 오류", icon: InformationBoxIcon.Error);
            }
        }

        private void addButton_Click(object sender, EventArgs e)
        {
            // 제일 마지막에 추가.
            var addingProduct = new Product();
            productBindingSource.Add(addingProduct);
        }

        private void deleteButton_Click(object sender, EventArgs e)
        {
            if (confirmDeletion)
            {
                var result = InformationBox.Show($"{productDataGridView.SelectedRows.Count}개의 항목을 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
                if (result == InformationBoxResult.Cancel)
                {
                    return;
                }
            }

            foreach (DataGridViewRow selectedRow in productDataGridView.SelectedRows)
            {
                productBindingSource.RemoveAt(selectedRow.Index);
            }
        }

        private void productDataGridView_SelectionChanged(object sender, EventArgs e)
        {
            // 선택된 행이 있을 때만 삭제 버튼을 enable 시킨다.
            deleteButton.Enabled = productDataGridView.SelectedRows.Count > 0;

            // 프로젝트 편집 버튼 Enable/Disable.
            UpdateEditButtonState();
        }

        // 프로젝트 편집 버튼 Enable/Disable.
        private void UpdateEditButtonState()
        {
            var selectedCells = productDataGridView.SelectedCells;
            if (selectedCells?.Count == 1 &&
                productDataGridView.Columns[selectedCells[0].ColumnIndex].DataPropertyName == nameof(Product.ProjectPath))
            {
                var currentProduct = productBindingSource[selectedCells[0].RowIndex] as Product;
                editButton.Enabled = TestProjectManager.Exists(currentProduct.ProjectPath);
            }
            else
            {
                editButton.Enabled = false;
            }
        }

        private void editButton_Click(object sender, EventArgs e)
        {
            var selectedCells = productDataGridView.SelectedCells;
            if (selectedCells?.Count == 1 &&
                productDataGridView.Columns[selectedCells[0].ColumnIndex].DataPropertyName == nameof(Product.ProjectPath))
            {
                var currentProduct = productBindingSource[selectedCells[0].RowIndex] as Product;
                var dlg = new TestProjectEditForm();
                dlg.CurrentProduct = currentProduct;
                dlg.ShowDialog();

                productDataGridView.UpdateCellValue(selectedCells[0].ColumnIndex, selectedCells[0].RowIndex);
            }
        }

        private void productDataGridView_CellValidating(object sender, DataGridViewCellValidatingEventArgs e)
        {
            if (!ProductSettingsViewModel.ValidateValue(productBindingSource[e.RowIndex] as Product, 
                productDataGridView.Columns[e.ColumnIndex].DataPropertyName, 
                e.FormattedValue.ToString(), out string errorMessage))
            {
                productDataGridView.Rows[e.RowIndex].ErrorText = errorMessage;
                e.Cancel = true;
            }
        }

        private void productDataGridView_CellEndEdit(object sender, DataGridViewCellEventArgs e)
        {
            // Clear the row error in case the user presses ESC.
            productDataGridView.Rows[e.RowIndex].ErrorText = string.Empty;
        }

        private void productDataGridView_CellValueChanged(object sender, DataGridViewCellEventArgs e)
        {
            if (productDataGridView.Columns[e.ColumnIndex].DataPropertyName == nameof(Product.ProjectPath))
            {
                // 프로젝트 이름이 변경된 경우, 프로젝트 파일이 없으면 새로 만들 것인지 물어본다.
                if (e.RowIndex < 0 || e.RowIndex >= productBindingSource.Count)
                {
                    return;
                }
                Product product = productBindingSource[e.RowIndex] as Product;
                if (!TestProjectManager.Exists(product.ProjectPath))
                {
                    var result = InformationBox.Show($"프로젝트 {product.ProjectPath} 이 없습니다. 새로 만들겠습니까?",
                        "프로젝트 만들기", buttons: InformationBoxButtons.YesNo, icon: InformationBoxIcon.Question);
                    if (result == InformationBoxResult.Yes)
                    {
                        try
                        {
                            TestProjectManager.CreateProjectFile(product.ProjectPath);
                        }
                        catch (Exception ex)
                        {
                            InformationBox.Show(ex.Message, "프로젝트 만들기 오류", icon: InformationBoxIcon.Error);
                        }
                    }
                }

                UpdateEditButtonState();
            }
        }

        private void productDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            if (e.RowIndex < 0 || e.RowIndex >= productBindingSource.Count)
            {
                return;
            }

            if (productDataGridView.Columns[e.ColumnIndex].DataPropertyName == nameof(Product.ProjectPath))
            {
                // 프로젝트 경로 선택 대화상자 오픈.
                var dialog = new OpenFileDialog();
                string fileExtension = TestProjectManager.FileExtension;
                dialog.Filter = $"Project Files(*.{fileExtension})|*.{fileExtension};|All Files(*.*)|*.*;";
                dialog.RestoreDirectory = true;
                dialog.CheckFileExists = false;
                if (dialog.ShowDialog() == DialogResult.OK)
                {
                    //Product currentProduct = productBindingSource[e.RowIndex] as Product;
                    //currentProduct.ProjectPath = dialog.FileName;
                    productDataGridView[e.ColumnIndex, e.RowIndex].Value = dialog.FileName;
                }
            }
        }
    }
}
