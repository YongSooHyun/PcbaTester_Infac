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
    /// <summary>
    /// 스텝을 새로 만들기 위한 정보를 수집한다.
    /// </summary>
    public partial class StepCreationForm : Form
    {
        /// <summary>
        /// 사용자가 선택한 스텝 종류.
        /// </summary>
        public object SelectedCategoryInfo { get; private set; }

        /// <summary>
        /// 만들려는 스텝 개수.
        /// </summary>
        public int Count { get; private set; } = 1;

        public StepCreationForm()
        {
            InitializeComponent();

            // ListView 설정.
            nameColumn.ImageGetter = StepCreationViewModel.CategoryImageGetter;

            // 템플릿 불러오기.
            var templates = StepCreationViewModel.GetCategoryInfos();
            templatesListView.SetObjects(templates);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);
        }

        private void okButton_Click(object sender, EventArgs e)
        {
            if (templatesListView.SelectedObject == null)
            {
                InformationBox.Show("테스트 스텝 유형을 선택하세요.", "Warning", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Warning);
                DialogResult = DialogResult.None;
            }

            SelectedCategoryInfo = templatesListView.SelectedObject;
            Count = (int)countNUDown.Value;
        }

        private void templatesListView_ItemActivate(object sender, EventArgs e)
        {
            okButton.PerformClick();
        }
    }
}
