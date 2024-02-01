using DiffMatchPatch;
using EOL_GND.Common;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Serialization;

namespace EOL_GND.Model
{
    /// <summary>
    /// 텍스트 파일의 전체 변경 정보를 표현한다.
    /// </summary>
    public class ChangeHistory
    {
        /// <summary>
        /// 이 히스토리를 저장하는 파일의 확장자. '.'을 포함하지 않는다.
        /// 원본 파일이름 뒤에 '.' + <see cref="FileExtension"/>을 붙여 <see cref="ChangeHistory"/> 저장 파일을 만든다.
        /// </summary>
        public const string FileExtension = "chhist";

        /// <summary>
        /// 히스토리 생성 시의 파일 변경 시간.
        /// </summary>
        [XmlIgnore]
        public DateTime ModificationTime { get; set; }

        /// <summary>
        /// <see cref="ModificationTime"/>의 XML serialization을 위한 프로퍼티.
        /// </summary>
        [XmlElement(nameof(ModificationTime))]
        public long ModificationTimeLong
        {
            get => ModificationTime.Ticks;
            set => ModificationTime = new DateTime(value);
        }

        /// <summary>
        /// 파일을 생성한 프로그램 사용자 이름.
        /// </summary>
        public string UserName { get; set; }

        /// <summary>
        /// 파일을 생성한 프로그램 사용자 권한.
        /// </summary>
        public string UserRole { get; set; }

        /// <summary>
        /// 파일을 생성한 프로그램 버전.
        /// </summary>
        public string EditorVersion { get; set; }

        /// <summary>
        /// 추가 설명.
        /// </summary>
        public string Remarks { get; set; }

        /// <summary>
        /// 초기 파일 내용.
        /// </summary>
        public string InitialContent { get; set; }

        /// <summary>
        /// 초기 파일 내용으로부터 변경된 이력.
        /// </summary>
        public List<ChangeRecord> HistoryRecords { get; set; }

        /// <summary>
        /// 텍스트 파일의 히스토리 초기 정보를 만든다.
        /// </summary>
        /// <param name="fileName">텍스트 파일 경로.</param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        public static ChangeHistory Create(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new Exception($"The specified file does not exist: {fileName}");
            }

            return new ChangeHistory
            {
                ModificationTime = fileInfo.LastWriteTime,
                EditorVersion = AppSettings.GetVersion().ToString(),
                InitialContent = File.ReadAllText(fileName),
            };
        }

        /// <summary>
        /// 지정한 파일에 해당하는(<see cref="FileExtension"/>이 뒤에 붙은) 히스토리 파일을 로딩한다.
        /// </summary>
        /// <param name="fileName">히스토리를 로딩할 텍스트 파일 경로.</param>
        /// <returns>히스토리 파일이 없으면 null을 리턴한다.</returns>
        /// <exception cref="Exception">히스토리를 로딩할 수 없는 경우.</exception>
        public static ChangeHistory Load(string fileName)
        {
            FileStream stream = null;
            var historyFileName = GetHistoryFileName(fileName);
            if (!File.Exists(historyFileName))
            {
                return null;
            }

            try
            {
                stream = new FileStream(historyFileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(ChangeHistory));
                var obj = xmlSerializer.Deserialize(stream) as ChangeHistory;
                return obj;
            }
            catch (Exception ex)
            {
                var message = $"Cannot load change history {historyFileName}: {ex.Message}";
                Logger.LogError(message);
                throw new Exception(message);
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 텍스트 파일 <paramref name="fileName"/>의 히스토리 파일 경로를 리턴한다.
        /// </summary>
        /// <param name="fileName">텍스트 파일 경로.</param>
        /// <returns></returns>
        private static string GetHistoryFileName(string fileName)
        {
            return fileName + "." + FileExtension;
        }

        /// <summary>
        /// 지정한 파일에 해당하는(<see cref="FileExtension"/>이 뒤에 붙은) 히스토리 파일을 저장한다.
        /// </summary>
        /// <param name="fileName">히스토리에 대응되는 텍스트 파일 경로.</param>
        public void Save(string fileName)
        {
            // 파일을 저장할 폴더가 없으면 만든다.
            var historyFileName = GetHistoryFileName(fileName);
            using (var writer = new StreamWriter(historyFileName))
            {
                var xmlSerializer = new XmlSerializer(GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        /// <summary>
        /// 텍스트에 patch들을 적용하여 새로운 텍스트를 만든다.
        /// </summary>
        /// <param name="initial">원본 텍스트.</param>
        /// <param name="records">patch들을 포함한 변경 레코드.</param>
        /// <returns></returns>
        /// <exception cref="Exception">patch 적용에 실패한 경우.</exception>
        public static string ApplyRecords(string initial, List<ChangeRecord> records)
        {
            if (records == null || records.Count == 0)
            {
                return initial;
            }

            var diffMatchPatch = new diff_match_patch();
            var appliedText = initial;
            for (int recordIndex = 0; recordIndex < records.Count; recordIndex++)
            {
                // Patch 리스트 생성.
                var recordPatches = diffMatchPatch.patch_fromText(records[recordIndex].PatchText);

                // Patch 리스트 적용.
                var applyResult = diffMatchPatch.patch_apply(recordPatches, appliedText);
                appliedText = applyResult[0] as string;

                // 성공 여부 체크.
                var patchesApplied = applyResult[1] as bool[];
                for (int patchIndex = 0; patchIndex < patchesApplied.Length; patchIndex++)
                {
                    if (!patchesApplied[patchIndex])
                    {
                        throw new Exception($"A patch(#{patchIndex+1}) of record" +
                            $"({records[recordIndex].ModificationTime:yyyy-MM-dd HH:mm:ss}) is not applied.");
                    }
                }
            }
            return appliedText;
        }

        /// <summary>
        /// 파일의 마지막 변경 시간과 텍스트 내용을 비교하여 새로운 변경 정보 레코드를 만든다.
        /// </summary>
        /// <param name="fileName">텍스트 파일 경로.</param>
        /// <returns>변경 내용이 없는 경우 null을 리턴한다.</returns>
        /// <exception cref="Exception"></exception>
        public ChangeRecord CreateRecord(string fileName)
        {
            var fileInfo = new FileInfo(fileName);
            if (!fileInfo.Exists)
            {
                throw new Exception($"The specified file does not exist: {fileName}");
            }

            // 마지막 변경 시간 체크.
            var lastModificationTime = ModificationTime;
            if (HistoryRecords?.Count > 0)
            {
                lastModificationTime = HistoryRecords.Last().ModificationTime;
            }

            // 마지막 변경 시간이 같으면 변화가 없는 것으로 간주.
            if (lastModificationTime == fileInfo.LastWriteTime)
            {
                return null;
            }

            // 변경되기 전 마지막 텍스트 계산.
            string currentContent = File.ReadAllText(fileName);
            string lastContent = ApplyRecords(InitialContent, HistoryRecords);

            // 현재 텍스트와 마지막 텍스트의 차이 계산.
            var diffMatchPatch = new diff_match_patch();
            var patches = diffMatchPatch.patch_make(lastContent, currentContent);
            if (patches == null || patches.Count == 0)
            {
                return null;
            }

            // 레코드 생성.
            return new ChangeRecord
            {
                ModificationTime = fileInfo.LastWriteTime,
                PatchText = diffMatchPatch.patch_toText(patches),
                EditorVersion = AppSettings.GetVersion().ToString(),
            };
        }

        /// <summary>
        /// 레코드들을 하나로 병합한 레코드를 리턴한다.
        /// </summary>
        /// <param name="records">병합하려는 레코드들. 연속된 레코드들이어야 한다.</param>
        /// <param name="intoLast">마지막 레코드에 병합하려면 true, 첫 레코드에 병합하려면 false.</param>
        /// <returns>하나로 병합된 레코드. <paramref name="records"/>가 null이거나 비었으면 null을 리턴.</returns>
        public static ChangeRecord Merge(List<ChangeRecord> records, bool intoLast)
        {
            if (records == null || records.Count == 0)
            {
                return null;
            }

            var patches = new List<Patch>();
            var diffMatchPatch = new diff_match_patch();
            foreach (var record in records)
            {
                patches.AddRange(diffMatchPatch.patch_fromText(record.PatchText));
            }

            // 처음 레코드 또는 마지막 레코드에 병합한다.
            var mergedRecord = intoLast ? records.Last() : records.First();
            mergedRecord.PatchText = diffMatchPatch.patch_toText(patches);
            return mergedRecord;
        }
    }
}
