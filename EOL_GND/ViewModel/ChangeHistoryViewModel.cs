using EOL_GND.Common;
using EOL_GND.Model;
using InfoBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class ChangeHistoryViewModel
    {
        private ChangeHistory history;

        /// <summary>
        /// 텍스트 파일의 변경 히스토리를 로딩한다.
        /// </summary>
        /// <param name="filePath">텍스트 파일 경로.</param>
        internal void Load(string filePath)
        {
            history = ChangeHistory.Load(filePath);
        }

        /// <summary>
        /// 텍스트 파일의 변경 히스토리를 저장한다.
        /// </summary>
        /// <param name="filePath"></param>
        internal void Save(string filePath)
        {
            if (history == null)
            {
                return;
            }

            history.Save(filePath);
        }

        /// <summary>
        /// 변경 레코드들을 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal List<ChangeRecord> GetChanges()
        {
            return history?.HistoryRecords.Reverse<ChangeRecord>().ToList();
        }

        /// <summary>
        /// 지정한 히스토리 레코드까지의 텍스트를 리턴.
        /// </summary>
        /// <param name="recordObj">히스토리 레코드.</param>
        /// <returns></returns>
        internal string GetText(object recordObj, out DateTime modificationTime)
        {
            var record = recordObj as ChangeRecord;
            if (record == null || history?.HistoryRecords == null || history.HistoryRecords.Count == 0)
            {
                // 레코드가 없으면 초기 텍스트를 리턴.
                modificationTime = history.ModificationTime;
                return history.InitialContent;
            }

            var recordIndex = history.HistoryRecords.IndexOf(record);
            if (recordIndex < 0)
            {
                // 레코드가 없으면 초기 텍스트를 리턴.
                modificationTime = history.ModificationTime;
                return history.InitialContent;
            }

            modificationTime = record.ModificationTime;
            var subRecords = history.HistoryRecords.GetRange(0, recordIndex + 1);
            return ChangeHistory.ApplyRecords(history.InitialContent, subRecords);
        }

        /// <summary>
        /// 히스토리 레코드들을 가장 최근의 히스토리 레코드에 병합합니다.
        /// </summary>
        /// <param name="recordObjects">병합하려는 레코드들.</param>
        /// <returns>하나로 병합된 레코드. 히스토리가 로딩되지 않았거나, <paramref name="recordObjects"/>가 비었으면 null을 리턴합니다.</returns>
        /// <exception cref="Exception"></exception>
        internal object MergeRecords(IList recordObjects, bool intoLast)
        {
            if (history == null || history.HistoryRecords == null || recordObjects == null || recordObjects.Count < 2)
            {
                return null;
            }

            // 시간에 따른 정렬 방법에 따라 레코드들은 시간순서 또는 역순서로 정렬될 수 있다.
            int recordIndexIncrement = -1;
            string unknownErrorMessage = "알 수 없는 에러가 발생했습니다.";

            var firstRecord = recordObjects[0] as ChangeRecord;
            int firstRecordIndex = history.HistoryRecords.IndexOf(firstRecord);
            if (firstRecordIndex < 0)
            {
                throw new Exception(unknownErrorMessage);
            }

            var records = new List<ChangeRecord>() { firstRecord };
            for (int objIndex = 1; objIndex < recordObjects.Count; objIndex++)
            {
                var record = recordObjects[objIndex] as ChangeRecord;
                int recordIndex = history.HistoryRecords.IndexOf(record);
                if (recordIndex < 0)
                {
                    throw new Exception(unknownErrorMessage);
                }

                // 인덱스 증가 또는 감소 순서로 정렬되었는지 저장.
                if (objIndex == 1)
                {
                    recordIndexIncrement = firstRecordIndex < recordIndex ? 1 : -1;
                }

                // 인덱스가 연속되어 있는지 체크.
                if (recordIndex != firstRecordIndex + recordIndexIncrement * objIndex)
                {
                    throw new Exception("연속된 레코드들만 병합할 수 있습니다.");
                }

                if (recordIndexIncrement < 0)
                {
                    records.Insert(0, record);
                }
                else
                {
                    records.Add(record);
                }
            }

            // 레코드들을 병합한다.
            var mergedRecord = ChangeHistory.Merge(records, intoLast);
            if (mergedRecord != null)
            {
                // 최종 병합 전에 한번 더 물어본다.
                var text = $"{records.Count}개의 레코드들을 병합하여 다음과 같은 하나의 레코드를 만듭니다.\r\n\r\n" +
                    $"변경 시간: {mergedRecord.ModificationTime}\r\n" +
                    $"사 용 자: {mergedRecord.UserInfo}\r\n" +
                    $"설   명: {mergedRecord.Remarks}\r\n" +
                    $"프로그램 버전: {mergedRecord.EditorVersion}\r\n\r\n" +
                    $"병합을 진행하시겠습니까?";
                var result = InformationBox.Show(text: text, title: "병합 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Question);
                if (result != InformationBoxResult.OK)
                {
                    return null;
                }

                int insertPosition = history.HistoryRecords.IndexOf(records[0]);
                history.HistoryRecords.RemoveRange(insertPosition, records.Count);
                history.HistoryRecords.Insert(insertPosition, mergedRecord);
            }
            
            return mergedRecord;
        }

        /// <summary>
        /// 현재 로그인한 사용자의 권한 정보를 얻는다.
        /// </summary>
        /// <returns></returns>
        internal static UserPermission GetUserPermission()
        {
            return AppSettings.GetCurrentUser()?.GetPermission();
        }
    }
}
