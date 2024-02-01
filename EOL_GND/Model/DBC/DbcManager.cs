using DbcParserLib;
using EOL_GND.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Xml.Serialization;

namespace EOL_GND.Model.DBC
{
    public class DbcManager
    {
        /// <summary>
        /// Config 파일이 저장되는 경로.
        /// </summary>
        public const string ConfigFileName = "D:\\ElozPlugin\\eol_can_messages.config";

        /// <summary>
        /// Shared instance.
        /// </summary>
        public static DbcManager SharedInstance
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
        private static DbcManager _sharedInstance = null;

        public List<DbcMessage> Messages { get; set; } = new List<DbcMessage>();

        #region Load & Save

        /// <summary>
        /// Config 파일을 로딩하여 클래스 인스턴스를 만든다.
        /// </summary>
        /// <returns></returns>
        private static DbcManager Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(ConfigFileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(DbcManager));
                var obj = xmlSerializer.Deserialize(stream) as DbcManager;
                return obj;
            }
            catch (Exception ex)
            {
                Logger.LogError($"Cannot load CAN messages: {ex.Message}");

                var obj = new DbcManager();
                return obj;
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
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFileName));
            using (var writer = new StreamWriter(ConfigFileName))
            {
                var xmlSerializer = new XmlSerializer(GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 지정한 이름을 가진 CAN 메시지를 찾아 리턴한다. 없으면 null을 리턴한다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbcMessage FindMessageByName(string name, object exceptOne)
        {
            foreach (var message in Messages)
            {
                if (message == exceptOne)
                {
                    continue;
                }

                if (string.Equals(message.Name, name, StringComparison.OrdinalIgnoreCase))
                {
                    return message;
                }
            }

            return null;
        }

        /// <summary>
        /// 지정한 이름을 가진 CAN 시그널을 찾아 리턴한다. 없으면 null을 리턴한다.
        /// </summary>
        /// <param name="name"></param>
        /// <returns></returns>
        public DbcSignal FindSignalByName(string name, object exceptOne, out DbcMessage parentMessage)
        {
            foreach (var message in Messages)
            {
                foreach (var signal in message.Signals)
                {
                    if (signal == exceptOne)
                    {
                        continue;
                    }

                    if (string.Equals(signal.Name, name, StringComparison.OrdinalIgnoreCase))
                    {
                        parentMessage = message;
                        return signal;
                    }
                }
            }

            parentMessage = null;
            return null;
        }

        /// <summary>
        /// 중복되지 않는 새 메시지 이름을 만든다.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        internal string GetNewMessageName(string baseName, int startIndex = 1)
        {
            while (startIndex < int.MaxValue)
            {
                var newName = $"{baseName}_{startIndex}";
                if (FindMessageByName(newName, null) == null)
                {
                    return newName;
                }
                startIndex++;
            }

            return baseName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
        }

        /// <summary>
        /// 중복되지 않는 새 시그널 이름을 만든다.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="startIndex"></param>
        /// <returns></returns>
        internal string GetNewSignalName(string baseName, int startIndex = 1)
        {
            while (startIndex < int.MaxValue)
            {
                var newName = $"{baseName}_{startIndex}";
                if (FindSignalByName(newName, null, out _) == null)
                {
                    return newName;
                }
                startIndex++;
            }

            return baseName + "_" + DateTime.Now.ToString("yyyy-MM-dd_HH_mm_ss");
        }

        /// <summary>
        /// 전체 시그널 리스트를 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal List<DbcSignal> GetAllSignals()
        {
            var signals = new List<DbcSignal>();
            foreach (var message in Messages)
            {
                if (message.Signals != null)
                {
                    signals.AddRange(message.Signals);
                }
            }
            return signals;
        }

        #endregion
    }
}
