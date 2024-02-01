using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// NovaFlash GRP CRC 관련 데이터.
    /// </summary>
    public class CrcTable
    {
        // 파일 보관 경로.
        private const string FilePath = "D:\\ElozPlugin\\crc_table.cfg";

        /// <summary>
        /// Shared instance.
        /// </summary>
        public static CrcTable SharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = Load();
                }
                return _sharedInstance;
            }
        }
        private static CrcTable _sharedInstance = null;

        /// <summary>
        /// CRC 대응 테이블.
        /// </summary>
        public List<CrcRecord> Records { get; set; } = new List<CrcRecord>();

        /// <summary>
        /// XML 파일을 로딩하여 클래스 인스턴스를 만든다.
        /// </summary>
        /// <returns></returns>
        private static CrcTable Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(FilePath, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(CrcTable), typeof(CrcTable).Namespace);
                var obj = (CrcTable)xmlSerializer.Deserialize(stream);
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot load CRC table: {ex.Message}");

                return new CrcTable();
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 설정을 저장한다.
        /// </summary>
        public void Save()
        {
            // 파일을 저장할 폴더가 없으면 만든다.
            Directory.CreateDirectory(Path.GetDirectoryName(FilePath));
            using (var writer = new StreamWriter(FilePath))
            {
                var xmlSerializer = new XmlSerializer(GetType(), GetType().Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }
    }

    /// <summary>
    /// GRP CRC 레코드 정보.
    /// </summary>
    public class CrcRecord
    {
        /// <summary>
        /// NovaFlash CRC.
        /// </summary>
        public uint NovaCrc { get; set; }

        /// <summary>
        /// MES에서 보내주는 CRC.
        /// </summary>
        public uint MesCrc { get; set; }
    }
}
