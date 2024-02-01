using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Fixture Probe Count 정보 저장, 읽기를 진행한다.
    /// </summary>
    public class FixtureProbeCountManager
    {
        /// <summary>
        /// 제품 정보 보관 파일 이름.
        /// </summary>
        private readonly static string fileName = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "fix_probe_count.cfg");

        /// <summary>
        /// Probe Count 정보 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<FixtureProbeCount> Probes { get; set; } = new List<FixtureProbeCount>();

        private FixtureProbeCountManager()
        {
        }

        /// <summary>
        /// 이 클래스의 인스턴스를 파일에 보관한다.
        /// </summary>
        internal void Save()
        {
            using (var writer = new StreamWriter(fileName))
            {
                var xmlSerializer = new XmlSerializer(GetType(), GetType().Namespace);
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 이 클래스를 XML파일로부터 로딩한다.
        /// </summary>
        /// <returns>로딩한 오브젝트.</returns>
        internal static FixtureProbeCountManager Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(fileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(FixtureProbeCountManager), typeof(FixtureProbeCountManager).Namespace);
                var obj = xmlSerializer.Deserialize(stream) as FixtureProbeCountManager;
                return obj;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(FixtureProbeCountManager)}.{nameof(Load)}(): {e.Message}");
                return new FixtureProbeCountManager();
            }
            finally
            {
                stream?.Close();
            }
        }
    }
}
