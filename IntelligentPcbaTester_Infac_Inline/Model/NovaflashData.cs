using System.Collections.Generic;
using System.IO;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    public class NovaflashData
    {
        /// <summary>
        /// Novaflash Controller에 LAN을 이용하여 연결할지 여부.
        /// </summary>
        public bool LanConnection { get; set; }

        /// <summary>
        /// Novaflash Controller에 LAN을 이용해 연결할 때 연결할 주소.
        /// </summary>
        public string LanAddress { get; set; }

        /// <summary>
        /// Novaflash Controller에 연결하기 위한 시리얼 포트 이름.
        /// </summary>
        public string SerialPortName { get; set; }

        /// <summary>
        /// Novaflash Controller에 연결하기 위한 시리얼 통신 속도.
        /// </summary>
        public int SerialBaudRate { get; set; }

        /// <summary>
        /// 실행하려는 GRP 정보 리스트.
        /// </summary>
        public List<GrpInfo> GrpFiles { get; set; }

        /// <summary>
        /// 지정된 파일 경로에 XML 형식으로 데이터를 보관한다.
        /// </summary>
        /// <param name="filePath"></param>
        public void Save(string filePath)
        {
            using (var writer = new StreamWriter(filePath))
            {
                var xmlSerializer = new XmlSerializer(typeof(NovaflashData), typeof(NovaflashData).Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 지정된 XML 형식 파일 데이터로부터 로딩하여 클래스 인트턴스를 만든다.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        public static NovaflashData Load(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open))
            {
                var xmlSerializer = new XmlSerializer(typeof(NovaflashData), typeof(NovaflashData).Namespace);
                var obj = xmlSerializer.Deserialize(stream);
                return obj as NovaflashData;
            }
        }
    }

    /// <summary>
    /// GRP파일의 CRC를 체크하는 방법들을 정의한다.
    /// </summary>
    public enum CrcCheckMethod
    {
        /// <summary>
        /// 로컬에 저장한 CRC와 파일에서 추출한 CRC를 비교.
        /// </summary>
        Local,

        /// <summary>
        /// 로컬에 저장한 CRC와 MES(ICT)에서 받은 CRC를 추출한 CRC와 비교.
        /// </summary>
        MES_ICT,

        /// <summary>
        /// 로컬에 저장한 CRC와 MES(EOL)에서 받은 CRC를 추출한 CRC와 비교.
        /// </summary>
        MES_EOL,
    }

    public class GrpInfo
    {
        /// <summary>
        /// 실행할 채널 번호. 현재 가능한 번호는 1 ~ 4 이다.
        /// </summary>
        public int Channel { get; set; } = 1;

        /// <summary>
        /// GRP 파일 경로. 반드시 존재해야 하며, Import파일이 있으면 빈 GRP, 없으면 비지 않은 GRP 이어야 한다.
        /// </summary>
        public string GrpFilePath { get; set; }

        /// <summary>
        /// Import 파일 경로.
        /// </summary>
        public string ImportFilePath { get; set; }

        /// <summary>
        /// 실행 순서. 낮은 실행순서가 먼저 실행되며 같은 실행순서는 동시에 실행된다.
        /// </summary>
        public int RunOrder { get; set; } = 1;

        /// <summary>
        /// CRC 값.
        /// </summary>
        public uint Crc { get; set; }

        /// <summary>
        /// CRC 비교 방법.
        /// </summary>
        public CrcCheckMethod CrcCheck { get; set; }

        /// <summary>
        /// 다운로드할 ROM 파일 이름. Import 파일이 없으면 GRP 파일이름을, Import 파일이 있으면 그 파일이름을 리턴한다.
        /// </summary>
        public string RomFileName
        {
            get
            {
                if (string.IsNullOrWhiteSpace(ImportFilePath))
                {
                    return Path.GetFileName(GrpFilePath);
                }
                else
                {
                    return Path.GetFileName(ImportFilePath);
                }
            }
        }

        /// <summary>
        /// 실행하기 위해 Hydra 디바이스로 업로드할 파일 이름 생성.
        /// </summary>
        public static string GetUploadFileName(string grpFilePath, string importFilePath)
        {
            string fileName;
            if (string.IsNullOrWhiteSpace(importFilePath))
            {
                fileName = Path.GetFileName(grpFilePath);
            }
            else
            {
                fileName = Path.GetFileName(importFilePath);
            }

            if (string.IsNullOrWhiteSpace(fileName))
            {
                return null;
            }

            fileName = Path.ChangeExtension(fileName, ".grp");

            // NovaFlash 에러 방지를 위해 파일이름에서 공백, 괄호를 '_'로 교체한다.
            return Novaflash.GetEscapedFileName(fileName);
        }
    }
}
