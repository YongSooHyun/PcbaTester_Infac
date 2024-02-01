using System;
using System.Collections.Generic;
using System.Text;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 설비PC에서 MES Client로 보내는 요청을 정의한다.
    /// </summary>
    class MesRequestMessage : MesMessage
    {
        /// <summary>
        /// MES 메시지를 프로토콜에 따라 바이트 배열로 변환한다.
        /// </summary>
        /// <returns></returns>
        internal byte[] Encode()
        {
            // MES 메시지 문자열을 만든다.
            StringBuilder mesMessageStr = new StringBuilder();

            // STX 추가.
            mesMessageStr.Append(FrameStartChar);

            // Message ID.
            string escaped = MessageId?.Replace(FieldSeparator, SeparatorEscapeStr);
            string truncated = Utils.Truncate(escaped, 10);
            mesMessageStr.Append(truncated);

            // PC ID.
            mesMessageStr.Append(FieldSeparator);
            escaped = PcId?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 30);
            mesMessageStr.Append(truncated);

            // Process Flag.
            mesMessageStr.Append(FieldSeparator);
            mesMessageStr.Append(ProcessFlag.ToString());

            // Factory ID.
            mesMessageStr.Append(FieldSeparator);
            escaped = FactoryId?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 20);
            mesMessageStr.Append(truncated);

            // Line ID.
            mesMessageStr.Append(FieldSeparator);
            escaped = LineId?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 10);
            mesMessageStr.Append(truncated);

            // Oper ID.
            mesMessageStr.Append(FieldSeparator);
            escaped = OperId?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 10);
            mesMessageStr.Append(truncated);

            // Transaction Time.
            mesMessageStr.Append(FieldSeparator);
            mesMessageStr.Append(TransactionTime.ToString(DateTimeFormat));

            // Step.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = Step?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 10);
                mesMessageStr.Append(truncated);
            }
            else
            {
                mesMessageStr.Append(AlarmFlag.ToString());
            }

            // Equipment ID.
            mesMessageStr.Append(FieldSeparator);
            escaped = EquipmentId?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 10);
            mesMessageStr.Append(truncated);

            // Barcode Type.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                mesMessageStr.Append((int)BarcodeType);
            }

            // Barcode No.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = BarcodeNo?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 256);
                mesMessageStr.Append(truncated);
            }

            // Tag ID.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = TagId?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 20);
                mesMessageStr.Append(truncated);
            }

            // Status.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                mesMessageStr.Append((int)Status);
            }

            // Insp Seq.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = InspSeq?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 3);
                mesMessageStr.Append(truncated);
            }

            // Insp Data.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = InspData?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 100);
                mesMessageStr.Append(truncated);
            }
            else
            {
                mesMessageStr.Append((int)AlarmCode);
            }

            // Prod.
            mesMessageStr.Append(FieldSeparator);
            if (ProcessFlag != MessageType.MA)
            {
                escaped = Prod?.Replace(FieldSeparator, SeparatorEscapeStr);
                truncated = Utils.Truncate(escaped, 30);
                mesMessageStr.Append(truncated);
            }

            // MesResult.
            mesMessageStr.Append(FieldSeparator);

            // User Defined #1
            mesMessageStr.Append(FieldSeparator);
            escaped = UserDefined1?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 1000);
            mesMessageStr.Append(truncated);

            // User Defined #2
            mesMessageStr.Append(FieldSeparator);
            escaped = UserDefined2?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 1000);
            mesMessageStr.Append(truncated);

            // User Defined #3
            mesMessageStr.Append(FieldSeparator);
            escaped = UserDefined3?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 1000);
            mesMessageStr.Append(truncated);

            // User Defined #4
            mesMessageStr.Append(FieldSeparator);
            escaped = UserDefined4?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 1000);
            mesMessageStr.Append(truncated);

            // User Defined #5
            mesMessageStr.Append(FieldSeparator);
            escaped = UserDefined5?.Replace(FieldSeparator, SeparatorEscapeStr);
            truncated = Utils.Truncate(escaped, 1000);
            mesMessageStr.Append(truncated);

            // ETX 추가.
            mesMessageStr.Append(FrameEndChar);

            // ASCII Encoding을 이용해서 바이트 배열로 변환.
            Logger.LogMessage($"MES Request={mesMessageStr}", true);
            return Encoding.ASCII.GetBytes(mesMessageStr.ToString());
        }
    }
}
