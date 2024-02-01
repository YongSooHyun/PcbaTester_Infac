using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 제품 정보 저장, 읽기를 진행한다.
    /// </summary>
    public class ProductManager
    {
        /// <summary>
        /// 제품 정보 보관 파일 이름.
        /// </summary>
        private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "products.cfg");

        /// <summary>
        /// 제품 정보 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<Product> Products { get; set; } = new List<Product>();

        private ProductManager()
        {
        }

        /// <summary>
        /// 이 클래스의 인스턴스를 파일에 보관한다.
        /// </summary>
        internal void Save()
        {
            using (var writer = new StreamWriter(FileName))
            {
                var xmlSerializer = new XmlSerializer(GetType(), GetType().Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 이 클래스를 XML파일로부터 로딩한다.
        /// </summary>
        /// <returns>로딩한 오브젝트.</returns>
        internal static ProductManager Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(FileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(ProductManager), typeof(ProductManager).Namespace);
                var obj = xmlSerializer.Deserialize(stream) as ProductManager;
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(ProductManager)}.{nameof(Load)}(): {e.Message}");
                return new ProductManager();
            }
            finally
            {
                stream?.Close();
            }
        }
    }
}
