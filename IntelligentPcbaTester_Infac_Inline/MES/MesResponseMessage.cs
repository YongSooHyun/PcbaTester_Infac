using System;
using System.Collections.Generic;
using System.Globalization;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// MES Client에서 설비PC로 보내는 응답을 정의한다.
    /// </summary>
    class MesResponseMessage : MesMessage
    {
        /// <summary>
        /// MES 메시지 코드, 길이 100, 응답메시지 중 MES가 NG인 경우에만 메시지 발생.
        /// </summary>
        internal string MesMessageCode { get; set; }

        /// <summary>
        /// MES 메시지, 길이 100, 응답메시지 중 MES가 NG인 경우에만 메시지 발생.
        /// </summary>
        internal string MesMessageContent { get; set; }

        /// <summary>
        /// 바이트 배열을 프로토콜에 따라 MES 메시지로 변환한다.
        /// </summary>
        /// <param name="byteData">MES Client로부터 받은 바이트 배열.</param>
        /// <param name="offset">바이트 시작위치.</param>
        /// <param name="length">바이트 길이.</param>
        /// <returns>바이트 배열을 파싱하여 생성한 MES 응답 메시지.</returns>
        internal static MesResponseMessage Decode(byte[] byteData, int offset, int length)
        {
            // null 체크.
            if (byteData == null)
            {
                throw new ArgumentNullException(nameof(byteData), "응답 데이터가 null 입니다.");
            }

            if (length <= 0)
            {
                throw new ArgumentOutOfRangeException(nameof(length));
            }

            // 문자열 Decoding.
            string mesMessageStr = Encoding.ASCII.GetString(byteData, offset, length);

            Logger.LogMessage($"MES Response={mesMessageStr}", true);

            // 시작, 종료 문자 체크.
            //if (!mesMessageStr.StartsWith($"{FrameStartChar}"))
            //{
            //    throw new Exception("응답 문자열이 STX로 시작하지 않습니다.");
            //}
            if (!mesMessageStr.EndsWith($"{FrameEndChar}"))
            {
                throw new Exception("응답 문자열이 ETX로 끝나지 않습니다.");
            }

            // 메시지 필드 얻기.
            mesMessageStr = mesMessageStr.TrimStart(FrameStartChar);
            mesMessageStr = mesMessageStr.TrimEnd(FrameEndChar);
            string[] fields = mesMessageStr.Split(new string[] { FieldSeparator }, StringSplitOptions.None);

            // 필드 개수 체크.
            const int fieldCount = 24;
            if (fields.Length != fieldCount)
            {
                throw new Exception($"응답 필드 개수가 {fieldCount}개가 아닙니다.");
            }

            // 필드 파싱.
            MesResponseMessage responseMessage = new MesResponseMessage();
            responseMessage.MessageId = fields[0];
            responseMessage.PcId = fields[1];

            // Process Flag.
            try
            {
                responseMessage.ProcessFlag = (MessageType)Enum.Parse(typeof(MessageType), fields[2], false);
            }
            catch
            {
                throw new Exception("응답 메시지의 Process Flag가 지원되지 않는 값입니다.");
            }

            responseMessage.FactoryId = fields[3];
            responseMessage.LineId = fields[4];
            responseMessage.OperId = fields[5];

            // Transaction Time.
            try
            {
                responseMessage.TransactionTime = DateTime.ParseExact(fields[6], DateTimeFormat, CultureInfo.InvariantCulture);
            }
            catch
            {
                throw new Exception("응답 메시지의 Transaction Time이 지원되지 않는 값입니다.");
            }

            responseMessage.Step = fields[7];
            responseMessage.EquipmentId = fields[8];

            // Barcode Type.
            try
            {
                responseMessage.BarcodeType = (Barcode)Enum.Parse(typeof(Barcode), fields[9], false);
            }
            catch
            {
                throw new Exception("응답 메시지의 Barcode Type이 지원되지 않는 값입니다.");
            }

            responseMessage.BarcodeNo = fields[10];
            responseMessage.TagId = fields[11];

            // Status.
            try
            {
                responseMessage.Status = (StatusCode)Enum.Parse(typeof(StatusCode), fields[12], false);
            }
            catch
            {
                throw new Exception("응답 메시지시의 Status가 지원되지 않는 값입니다.");
            }

            responseMessage.InspSeq = fields[13];
            responseMessage.InspData = fields[14];
            responseMessage.Prod = fields[15];

            // MES Result.
            try
            {
                responseMessage.MesResult = int.Parse(fields[16]);
            }
            catch
            {
                throw new Exception("응답 메시지의 MES Result가 지원되지 않는 값입니다.");
            }

            responseMessage.UserDefined1 = fields[17];
            responseMessage.UserDefined2 = fields[18];
            responseMessage.UserDefined3 = fields[19];
            responseMessage.UserDefined4 = fields[20];
            responseMessage.UserDefined5 = fields[21];
            responseMessage.MesMessageCode = fields[22];
            responseMessage.MesMessageContent = fields[23];

            return responseMessage;
        }

        /// <summary>
        /// 사이버 보안을 위한 CRC32 얻기. <see cref="MesMessage.UserDefined4"/> 필드에 16진수 형태로 저장.
        /// </summary>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal uint GetCrc32()
        {
            if (string.IsNullOrEmpty(UserDefined4))
            {
                throw new Exception("CRC32 필드가 비어 있습니다.");
            }

            return uint.Parse(UserDefined4, NumberStyles.HexNumber);
        }
    }
}
