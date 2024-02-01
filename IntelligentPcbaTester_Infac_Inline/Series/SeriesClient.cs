using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading.Tasks;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// 직렬연결 통신 클라이언트.
    /// </summary>
    internal class SeriesClient
    {
        /// <summary>
        /// 싱글턴 인스턴스.
        /// </summary>
        internal static readonly SeriesClient SharedClient = new SeriesClient();

        /// <summary>
        /// 송/수신 버퍼 크기.
        /// </summary>
        internal const int BufferSize = 65535;

        /// <summary>
        /// Series 서버 연결 여부.
        /// </summary>
        internal bool Connected
        {
            get
            {
                if (connectedSocket == null)
                {
                    return false;
                }

                try
                {
                    bool selectRead = connectedSocket.Poll(1000, SelectMode.SelectRead);
                    bool dataAvailable = connectedSocket.Available > 0;
                    return !(selectRead && !dataAvailable);
                }
                catch
                {
                    return false;
                }
            }
        }

        // 통신에 이용하는 소켓.
        private Socket connectedSocket = null;

        // 연결되지 않은 경우의 에러 메시지.
        private const string NotConnectedMessage = "Series 서버에 연결되지 않았습니다.";

        /// <summary>
        /// 서버에 연결한다.
        /// </summary>
        internal void Connect(string serverName, int port)
        {
            try
            {
                connectedSocket?.Close();
                connectedSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);

                // 버퍼 크기 설정.
                connectedSocket.SendBufferSize = BufferSize;
                connectedSocket.ReceiveBufferSize = BufferSize;

                // 타임아웃 설정.
                connectedSocket.SendTimeout = AppSettings.SeriesCommTimeout;
                connectedSocket.ReceiveTimeout = AppSettings.SeriesCommTimeout;

                // 서버에 연결.
                connectedSocket.Connect(serverName, port);
            }
            catch
            {
                connectedSocket?.Close();
                connectedSocket = null;
                throw;
            }
        }

        /// <summary>
        /// 서버에 요청을 전송하고 응답을 받아 리턴한다.
        /// </summary>
        /// <param name="request"></param>
        /// <returns></returns>
        /// <exception cref="Exception"></exception>
        internal SeriesResponseMessage SendRequest(SeriesRequestMessage request)
        {
            try
            {
                if (connectedSocket == null)
                {
                    throw new Exception(NotConnectedMessage);
                }

                // 메시지 Encoding.
                var sendBuffer = request.Encode();
                int sentBytes = 0;
                while (sentBytes < sendBuffer.Length)
                {
                    sentBytes += connectedSocket.Send(sendBuffer, sentBytes, sendBuffer.Length - sentBytes, SocketFlags.None);
                }

                // 응답 수신.
                var receiveBuffer = new byte[connectedSocket.ReceiveBufferSize];

                // 헤더 수신.
                int receivedBytes = connectedSocket.Receive(receiveBuffer, 1, SocketFlags.None);
                if (receivedBytes == 0)
                {
                    // 서버 셧다운.
                    connectedSocket.Shutdown(SocketShutdown.Both);
                    throw new Exception("Series 서버가 셧다운 되었습니다.");
                }
                if (receiveBuffer[0] != SeriesMessage.Header)
                {
                    // 헤더 오류.
                    throw new Exception($"Series 서버가 약속되지 않은 헤더(0x{receiveBuffer[0]:X2})를 전송했습니다.");
                }

                // 길이를 읽는다.
                receivedBytes = connectedSocket.Receive(receiveBuffer, 4, SocketFlags.None);
                if (receivedBytes == 0)
                {
                    // 서버 셧다운.
                    connectedSocket.Shutdown(SocketShutdown.Both);
                    throw new Exception("Series 서버가 셧다운 되었습니다.");
                }
                if (receivedBytes != 4)
                {
                    // 통신 프로토콜 에러.
                    throw new Exception($"Series 서버가 약속되지 않은 길이({receivedBytes})의 크기 정보를 전송했습니다.");
                }
                var unorderedLength = BitConverter.ToInt32(receiveBuffer, 0);
                var length = IPAddress.NetworkToHostOrder(unorderedLength);

                // 길이만큼 읽는다.
                receivedBytes = 0;
                int receivedLen = 0;
                while (receivedBytes < length)
                {
                    receivedLen = connectedSocket.Receive(receiveBuffer, receivedBytes, length - receivedBytes, SocketFlags.None);
                    receivedBytes += receivedLen;
                    if (receivedLen == 0)
                    {
                        break;
                    }
                }
                if (receivedLen == 0)
                {
                    // 서버 셧다운.
                    connectedSocket.Shutdown(SocketShutdown.Both);
                    throw new Exception("Series 서버가 셧다운 되었습니다.");
                }
                if (receivedBytes != length)
                {
                    // 통신 프로토콜 에러.
                    throw new Exception($"Series 서버가 약속되지 않은 길이({receivedBytes})의 메시지를 전송했습니다.");
                }

                // 응답 Decoding.
                var response = SeriesResponseMessage.Decode(receiveBuffer, 0, receivedBytes);
                return response;
            }
            catch
            {
                connectedSocket?.Close();
                connectedSocket = null;
                throw;
            }
        }

        /// <summary>
        /// 서버 연결을 해제한다.
        /// </summary>
        internal void Disconnect()
        {
            connectedSocket?.Close();
            connectedSocket = null;
        }
    }
}
