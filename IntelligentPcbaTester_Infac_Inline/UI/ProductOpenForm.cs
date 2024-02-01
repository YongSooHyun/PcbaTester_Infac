using InfoBox;
using System;
using System.Collections.Generic;
using System.Text;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    public partial class ProductOpenForm : Form
    {
        /// <summary>
        /// 오픈하려고 선택한 프로덕트.
        /// </summary>
        public Product Product { get; private set; }

        // 제품 정보 리스트.
        private List<Product> products;

        public ProductOpenForm()
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

        private void productDataGridView_CellDoubleClick(object sender, DataGridViewCellEventArgs e)
        {
            openButton.PerformClick();
        }

        private void productDataGridView_CurrentCellChanged(object sender, EventArgs e)
        {
            UpdateOpenButton();
        }

        private void UpdateOpenButton()
        {
            openButton.Enabled = productDataGridView.CurrentCell != null && productDataGridView.CurrentCell.RowIndex >= 0;
        }

        private void openButton_Click(object sender, EventArgs e)
        {
            if (productDataGridView.CurrentCell != null && productDataGridView.CurrentCell.RowIndex >= 0)
            {
                Product = productBindingSource[productDataGridView.CurrentCell.RowIndex] as Product;
            }
        }
    }
}
