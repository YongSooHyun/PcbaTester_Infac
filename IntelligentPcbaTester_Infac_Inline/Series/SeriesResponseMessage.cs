using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 직렬연결을 위한 서버의 응답 메시지 관련 기능들을 정의한다.
    /// </summary>
    internal class SeriesResponseMessage : SeriesMessage
    {
        /// <summary>
        /// 처리 가능한 PCB 개수.
        /// </summary>
        internal int Capacity { get; set; }

        internal SeriesResponseMessage(RequestCommand command, int capacity)
        {
            Command = command;
            Capacity = capacity;
        }

        /// <summary>
        /// 응답 메시지로부터 프로토콜에 따라 바이트 배열을 만들어 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal byte[] Encode()
        {
            // 메시지 문자열을 만든다.
            var responseMessageBuilder = new StringBuilder();

            // 명령.
            responseMessageBuilder.Append(Command.ToString());

            // Capacity.
            responseMessageBuilder.Append(Separator);
            responseMessageBuilder.Append(Capacity);

            // 메지시 문자열 Encoding.
            var responseMessage = responseMessageBuilder.ToString();
            var responseMessageBytes = TextEncoding.GetBytes(responseMessage);

            // 전체 메시지 만들기.
            var buffer = new byte[5 + responseMessageBytes.Length];

            // Header.
            buffer[0] = Header;

            // Length.
            var length = IPAddress.HostToNetworkOrder(responseMessageBytes.Length);
            var lengthBytes = BitConverter.GetBytes(length);
            lengthBytes.CopyTo(buffer, 1);

            // Body.
            responseMessageBytes.CopyTo(buffer, 5);

            Logger.LogTimedMessage($"Series Response={responseMessage}");

            return buffer;
        }

        /// <summary>
        /// 바이트 배열로부터 프로토콜에 따라 응답 메시지를 만들어 리턴한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
        /// <returns></returns>
        internal static SeriesResponseMessage Decode(byte[] buffer, int offset, int length)
        {
            // 문자열 Decoding.
            var responseMessage = TextEncoding.GetString(buffer, offset, length);
            Logger.LogTimedMessage($"Series Response={responseMessage}");

            // 필드 개수 체크.
            var fields = responseMessage.Split(new string[] { Separator }, StringSplitOptions.None);
            if (fields.Length != 2)
            {
                throw new Exception($"Series 응답 필드 개수({fields.Length}) 오류입니다.");
            }

            // 명령.
            var command = (RequestCommand)Enum.Parse(typeof(RequestCommand), fields[0]);

            // Capacity.
            var capacity = int.Parse(fields[1]);

            return new SeriesResponseMessage(command, capacity);
        }
    }
}
