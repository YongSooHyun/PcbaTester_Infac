using System.Collections.Generic;
using System.IO;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// <see cref="ProductSettingsForm"/>에서 사용하는 ViewModel 클래스.
    /// </summary>
    class ProductSettingsViewModel
    {
        // 제품 정보를 파일에 읽고 쓰기 위한 필드.
        private static ProductManager productManager;

        private static ProductManager GetProductManager()
        {
            if (productManager == null)
            {
                productManager = ProductManager.Load();
            }

            return productManager;
        }

        internal static List<Product> GetProducts()
        {
            return GetProductManager().Products;
        }

        internal static void Save()
        {
            productManager?.Save();
        }

        internal static bool ValidateValue(Product obj, string propertyName, string propertyValue, out string errorMessage)
        {
            //switch (propertyName)
            //{
            //    case nameof(Product.FGCode):
            //        if (string.IsNullOrEmpty(propertyValue))
            //        {
            //            errorMessage = "빈 문자열은 입력할 수 없습니다.";
            //            return false;
            //        }

            //        // FGCode는 유일해야 한다.
            //        var products = GetProducts();
            //        foreach (var product in products)
            //        {
            //            if (product == obj)
            //            {
            //                continue;
            //            }

            //            if (propertyValue.Equals(product.FGCode, System.StringComparison.OrdinalIgnoreCase))
            //            {
            //                errorMessage = "해당 FGCODE 값이 이미 등록되어 있습니다.";
            //                return false;
            //            }
            //        }
            //        break;
            //    case nameof(Product.ProjectPath):
            //        if (string.IsNullOrEmpty(propertyValue))
            //        {
            //            errorMessage = "빈 문자열은 입력할 수 없습니다.";
            //            return false;
            //        }

            //        // 프로젝트 경로는 파일 경로이므로, 유효한 파일 이름인가를 검사한다.
            //        if (!Utils.IsValidPath(propertyValue))
            //        {
            //            errorMessage = "프로젝트 경로가 허용되지 않는 문자를 포함하고 있습니다.";
            //            return false;
            //        }
            //        break;
            //}

            errorMessage = "";
            return true;
        }
    }
}
