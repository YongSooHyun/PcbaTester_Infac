using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Reflection;
using System.Text;
using System.Xml.Serialization;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// MES Result 코드별로 오류 메시지를 설정하는데 이용된다.
    /// </summary>
    public class MesResultInfo
    {
        /// <summary>
        /// MES Result.
        /// </summary>
        public int ResultCode { get; set; } = 0;

        /// <summary>
        /// Result에 대한 오류 메시지.
        /// </summary>
        [DisplayName("오류 메시지 (줄바꿈 /)")]
        public string ErrorMessage { get; set; } = null;
    }

    public class MesResultInfoManager
    {
        /// <summary>
        /// 정보 보관 파일 이름.
        /// </summary>
        private static string FileName => Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "mes_result.cfg");

        /// <summary>
        /// 정보 리스트. 파일로부터 Serialize/Deserialize 된다.
        /// </summary>
        public List<MesResultInfo> Results { get; set; } = new List<MesResultInfo>();

        // 한번만 로딩하기 위한 필드.
        private static MesResultInfoManager sharedInstance;

        private MesResultInfoManager()
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
        internal static MesResultInfoManager Load()
        {
            if (sharedInstance != null)
            {
                return sharedInstance;
            }

            FileStream stream = null;
            try
            {
                stream = new FileStream(FileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(MesResultInfoManager), typeof(MesResultInfoManager).Namespace);
                sharedInstance = xmlSerializer.Deserialize(stream) as MesResultInfoManager;
                return sharedInstance;
            }
            catch (Exception e)
            {
                Logger.LogError($"{nameof(MesResultInfoManager)}.{nameof(Load)}(): {e.Message}");
                sharedInstance = new MesResultInfoManager();
                return sharedInstance;
            }
            finally
            {
                stream?.Close();
            }
        }

        internal bool ValidateValue(int resultIndex, string propertyName, string propertyValue, out string errorMessage)
        {
            switch (propertyName)
            {
                case nameof(MesResultInfo.ResultCode):
                    if (string.IsNullOrEmpty(propertyValue))
                    {
                        errorMessage = "응답 코드를 입력하세요.";
                        return false;
                    }

                    // 응답 코드는 정수이어야 한다.
                    if (!int.TryParse(propertyValue, out int newResultCode))
                    {
                        errorMessage = "응답 코드는 정수이어야 합니다.";
                        return false;
                    }

                    // 응답 코드는 유일해야 한다.
                    for (int index = 0; index < Results.Count; index++)
                    {
                        if (index == resultIndex)
                        {
                            continue;
                        }

                        if (newResultCode == Results[index].ResultCode)
                        {
                            errorMessage = "해당 응답 코드가 이미 등록되어 있습니다.";
                            return false;
                        }
                    }
                    break;
            }

            errorMessage = "";
            return true;
        }
    }

    /// <summary>
    /// MES Client와 주고받는 메시지 형식을 정의한다.
    /// </summary>
    abstract class MesMessage
    {
        /// <summary>
        /// MesResult가 성공임을 나타내는 상수.
        /// </summary>
        internal const int ResultSuccess = 0;

        /// <summary>
        /// 메시지 필드들을 구분하는 문자열.
        /// </summary>
        internal const string FieldSeparator = "/";

        /// <summary>
        /// 필드 구분 문자열이 필드에 포함되는 경우 대체 문자열.
        /// </summary>
        internal const string SeparatorEscapeStr = "#0x2F";

        /// <summary>
        /// 메시지 프레임 시작 문자.
        /// </summary>
        internal const char FrameStartChar = '\x02';

        /// <summary>
        /// 메시지 프레임 종료 문자.
        /// </summary>
        internal const char FrameEndChar = '\x03';

        /// <summary>
        /// 메시지 프레임의 최대 길이.
        /// </summary>
        internal const int MaxFrameLength = 20_000;

        /// <summary>
        /// DateTime 포맷 문자열.
        /// </summary>
        internal const string DateTimeFormat = "yyyyMMddHHmmss";

        /// <summary>
        /// 메시지 ID, User 설정가능한 필드, 길이 10.
        /// </summary>
        internal string MessageId { get; set; }

        /// <summary>
        /// PD ID, Default Value : empty string →  응답메시지에 MES 설비 코드 전송 (설비 무시), 길이 30.
        /// </summary>
        internal string PcId { get; set; }

        /// <summary>
        /// 메시지 타입 정의.
        /// </summary>
        internal enum MessageType
        {
            /// <summary>
            /// 이전공정 체크요청.
            /// </summary>
            M1,

            /// <summary>
            /// 이전공정 체크결과.
            /// </summary>
            M2,

            /// <summary>
            /// 공정결과 처리요청.
            /// </summary>
            M3,

            /// <summary>
            /// 공정결과 처리응답.
            /// </summary>
            M4,

            /// <summary>
            /// 알람 통보.
            /// </summary>
            MA
        }

        /// <summary>
        /// 메시지 구분, 길이 10.
        /// </summary>
        internal MessageType ProcessFlag { get; set; } = MessageType.M1;

        /// <summary>
        /// 공장 ID, User 설정가능한 필드, 길이 20.
        /// </summary>
        internal string FactoryId { get; set; }

        /// <summary>
        /// 라인 ID, User 설정가능한 필드, 길이 10.
        /// </summary>
        internal string LineId { get; set; }

        /// <summary>
        /// 공정 ID, User 설정가능한 필드, 길이 10.
        /// </summary>
        internal string OperId { get; set; }

        /// <summary>
        /// 전송시간, yyyymmddHHmmss 포맷, 길이 14.
        /// </summary>
        internal DateTime TransactionTime { get; set; } = DateTime.Now;

        /// <summary>
        /// 공정 구분, Default Value = 1, 길이 10.
        /// </summary>
        internal string Step { get; set; } = "1";

        internal enum AlarmFlagType
        {
            /// <summary>
            /// 알람 설정.
            /// </summary>
            S,

            /// <summary>
            /// 알람 해제.
            /// </summary>
            E
        }

        /// <summary>
        /// <see cref="ProcessFlag"/>가 <see cref="MessageType.MA"/>일 때(알람) 알람 설정/해제 구분.
        /// </summary>
        internal AlarmFlagType AlarmFlag = AlarmFlagType.S;

        /// <summary>
        /// 설비 위치 ID, User 설정가능, 길이 1.
        /// </summary>
        internal string EquipmentId { get; set; }

        /// <summary>
        /// 바코드 형태.
        /// </summary>
        internal enum Barcode
        {
            PCB = 1,
            TagID = 2,
        }

        /// <summary>
        /// 바코드 유형, 길이 1.
        /// </summary>
        internal Barcode BarcodeType { get; set; } = Barcode.PCB;

        /// <summary>
        /// 바코드, 길이 256.
        /// </summary>
        internal string BarcodeNo { get; set; }

        /// <summary>
        /// Tag ID, 길이 20.
        /// </summary>
        internal string TagId { get; set; }

        /// <summary>
        /// 설비 결과.
        /// </summary>
        internal enum StatusCode
        {
            OK = 0,
            NG = 1,
        }

        /// <summary>
        /// 설비 결과, 길이 30. M1은 무조건 OK, M3은 OK 또는 NG.
        /// </summary>
        internal StatusCode Status { get; set; } = StatusCode.OK;

        /// <summary>
        /// 검사 순번, 길이 3.
        /// </summary>
        internal string InspSeq { get; set; } = "1";

        /// <summary>
        /// 검사 데이터, 길이 100. 무시.
        /// </summary>
        internal string InspData { get; set; }

        internal enum AlarmCodeType
        {
            Alarm1,
        }

        /// <summary>
        /// <see cref="ProcessFlag"/>가 <see cref="MessageType.MA"/>일 때(알람) 알람 코드.
        /// </summary>
        internal AlarmCodeType AlarmCode { get; set; } = AlarmCodeType.Alarm1;

        /// <summary>
        /// 제품 코드(품번), 응답(M2, M4) 메시지 : 품번, 길이 30.
        /// </summary>
        internal string Prod { get; set; }

        /// <summary>
        /// MES 결과, 요청(M1, M3) 메시지 Empty / 응답(M2, M4) 메시지 OK 또는 NG, 길이 30.
        /// </summary>
        internal int MesResult { get; set; } = 0;

        /// <summary>
        /// 사용자 정의 필드 1, 길이 1000.
        /// </summary>
        internal string UserDefined1 { get; set; }

        /// <summary>
        /// 사용자 정의 필드 2, 길이 1000.
        /// </summary>
        internal string UserDefined2 { get; set; }

        /// <summary>
        /// 사용자 정의 필드 3, 길이 1000.
        /// </summary>
        internal string UserDefined3 { get; set; }

        /// <summary>
        /// 사용자 정의 필드 4, 길이 1000.
        /// </summary>
        internal string UserDefined4 { get; set; }

        /// <summary>
        /// 사용자 정의 필드 5, 길이 1000.
        /// </summary>
        internal string UserDefined5 { get; set; }
    }
}
