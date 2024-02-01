using System.ComponentModel;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 제품을 표현하며, 어떤 프로젝트 파일을 실행할 것인지의 정보가 들어 있다.
    /// 바코드를 파싱하여 등록된 특정 제품을 찾는다.
    /// </summary>
    public class Product
    {
        /// <summary>
        /// 바코드를 파싱하여 얻어지는 코드.
        /// </summary>
        [DisplayName("FGCODE")]
        public string FGCode { get; set; }

        /// <summary>
        /// 차종.
        /// </summary>
        [DisplayName("차종")]
        public string CarType { get; set; }

        /// <summary>
        /// 바코드에 있는 차종코드, 6번째부터 3글자.
        /// </summary>
        [DisplayName("차종코드")]
        public string CarTypeCode { get; set; } = "";

        /// <summary>
        /// 해당하는 프로젝트 경로.
        /// </summary>
        [DisplayName("Project")]
        public string ProjectPath { get; set; }

        /// <summary>
        /// ProjectPath로부터 로딩한 프로젝트.
        /// </summary>
        internal TestProject Project { get; set; }
    }
}
