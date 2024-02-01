using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 직렬연결을 위한 클라이언트의 요청 메시지 관련 기능들을 정의한다.
    /// </summary>
    internal class SeriesRequestMessage : SeriesMessage
    {
        internal SeriesRequestMessage(RequestCommand command)
        {
            Command = command;
        }

        /// <summary>
        /// 요청 메시지로부터 프로토콜에 따라 바이트 배열을 만들어 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal byte[] Encode()
        {
            // 바디 - 명령.
            byte[] body = TextEncoding.GetBytes(Command.ToString());

            // 전체 버퍼 생성.
            var buffer = new byte[5 + body.Length];

            // 헤더.
            buffer[0] = Header;

            // Body 길이.
            var reordered = IPAddress.HostToNetworkOrder(body.Length);
            var reorderedBytes = BitConverter.GetBytes(reordered);
            reorderedBytes.CopyTo(buffer, 1);

            // Body.
            body.CopyTo(buffer, 5);

            Logger.LogTimedMessage($"Series Request={Command}");

            return buffer;
        }

        /// <summary>
        /// 바이트 배열로부터 프로토콜에 따라 요청 메시지를 만들어 리턴한다.
        /// </summary>
        /// <param name="buffer"></param>
        /// <param name="offset"></param>
        /// <param name="length"></param>
            /// <returns></returns>
        internal static SeriesRequestMessage Decode(byte[] buffer, int offset, int length)
        {
            // 문자열 Decoding.
            var requestMessage = TextEncoding.GetString(buffer, offset, length);
            Logger.LogTimedMessage($"Series Request={requestMessage}");

            // 명령.
            var command = (RequestCommand)Enum.Parse(typeof(RequestCommand), requestMessage);

            return new SeriesRequestMessage(command);
        }
    }
}
