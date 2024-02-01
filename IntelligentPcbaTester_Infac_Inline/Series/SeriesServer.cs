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
    /// 직렬연결을 위한 서버.
    /// </summary>
    internal class SeriesServer
    {
        /// <summary>
        /// 싱글턴 인스턴스.
        /// </summary>
        internal static readonly SeriesServer SharedServer = new SeriesServer();

        /// <summary>
        /// 테스트 상태에 따라 이 값을 바꿔주면 클라이언트 요청 시 이 값을 전송한다.
        /// </summary>
        internal bool Capable { get; set; }

        /// <summary>
        /// 서버가 실행중인가를 나타낸다.
        /// </summary>
        internal bool Running { get; private set; } = false;

        // 서버 리스닝 소켓.
        private Socket listeningSocket = null;

        // 연결 대기열 길이.
        private const int Backlog = 1;

        // 연결된 클라이언트 소켓.
        private Socket connectedSocket = null;

        // Socket 송/수신 버퍼 크기.
        private const int BufferSize = 65535;

        /// <summary>
        /// 서버를 Thread Pool에서 시작한다.
        /// </summary>
        /// <param name="port"></param>
        internal void Start(int port)
        {
            Stop();

            Task.Run(() => DoStart(port));
        }

        // 서버 시작 스레드 함수.
        private void DoStart(int port)
        {
            try
            {
                listeningSocket = new Socket(SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                listeningSocket.Listen(Backlog);
                Running = true;

                Logger.LogTimedMessage($"Series 서버가 포트 {port} 에서 시작되었습니다.");

                while (true)
                {
                    var client = listeningSocket.Accept();

                    Logger.LogTimedMessage($"Series client {client.RemoteEndPoint} 이 연결되었습니다.");

                    // 하나의 클라이언트만 허용한다.
                    ProcessClient(client);
                }
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"Series 서버 오류: {ex.Message}");
            }
            finally
            {
                Running = false;
                listeningSocket?.Close();
                listeningSocket = null;

                Logger.LogTimedMessage($"Series 서버가 종료되었습니다.");
            }
        }

        // 클라이언트와의 통신 처리.
        private void ProcessClient(Socket client)
        {
            connectedSocket = client;

            try
            {
                // 타임아웃 설정.
                client.SendTimeout = AppSettings.SeriesCommTimeout;
                client.ReceiveTimeout = -1;

                // 송/수신 버퍼 설정.
                client.SendBufferSize = BufferSize;
                client.ReceiveBufferSize = BufferSize;
                var buffer = new byte[BufferSize];

                // 수신명령 처리.
                while (true)
                {
                    // 먼저 헤더를 읽어 비교한다.
                    int receivedBytes = client.Receive(buffer, 1, SocketFlags.None);
                    if (receivedBytes == 0)
                    {
                        // 클라이언트 셧다운.
                        Logger.LogTimedMessage($"Series client {client.RemoteEndPoint} 이 셧다운 되었습니다.");
                        client.Shutdown(SocketShutdown.Both);
                        break;
                    }
                    if (buffer[0] != SeriesMessage.Header)
                    {
                        // 헤더 오류.
                        throw new Exception($"약속되지 않은 헤더(0x{buffer[0]:X2})를 수신했습니다.");
                    }

                    // 길이를 읽는다.
                    receivedBytes = client.Receive(buffer, 4, SocketFlags.None);
                    if (receivedBytes == 0)
                    {
                        // 클라이언트 셧다운.
                        Logger.LogTimedMessage($"Series client {client.RemoteEndPoint} 이 셧다운 되었습니다.");
                        client.Shutdown(SocketShutdown.Both);
                        break;
                    }
                    if (receivedBytes != 4)
                    {
                        // 통신 프로토콜 에러.
                        throw new Exception($"약속되지 않은 길이({receivedBytes})의 크기 정보를 수신했습니다.");
                    }
                    var unorderedLength = BitConverter.ToInt32(buffer, 0);
                    var length = IPAddress.NetworkToHostOrder(unorderedLength);

                    // 길이만큼 읽는다.
                    receivedBytes = 0;
                    int receivedLen = 0;
                    while (receivedBytes < length)
                    {
                        receivedLen = client.Receive(buffer, receivedBytes, length - receivedBytes, SocketFlags.None);
                        receivedBytes += receivedLen;
                        if (receivedLen == 0)
                        {
                            break;
                        }
                    }
                    if (receivedLen == 0)
                    {
                        // 클라이언트 셧다운.
                        Logger.LogTimedMessage($"Series client {client.RemoteEndPoint} 이 셧다운 되었습니다.");
                        client.Shutdown(SocketShutdown.Both);
                        break;
                    }
                    if (receivedBytes != length)
                    {
                        // 통신 프로토콜 에러.
                        throw new Exception($"약속되지 않은 길이({receivedBytes})의 메시지를 수신했습니다.");
                    }

                    // 메시지를 해석한다.
                    var request = SeriesRequestMessage.Decode(buffer, 0, receivedBytes);
                    switch (request.Command)
                    {
                        case SeriesMessage.RequestCommand.GetCapacity:
                            var response = new SeriesResponseMessage(request.Command, Capable ? 1 : 0);
                            var responseBytes = response.Encode();
                            int sentBytes = 0;
                            while (sentBytes < responseBytes.Length)
                            {
                                sentBytes += client.Send(responseBytes, sentBytes, responseBytes.Length - sentBytes, SocketFlags.None);
                            }
                            break;
                        default:
                            // TOD: 알 수 없는 요청임을 나타내는 에러 메시지 형식이 필요한지 검토 필요.
                            break;
                    }
                }
            }
            catch (Exception ex)
            {
                Logger.LogTimedMessage($"Series client 통신 오류: {ex.Message}");
            }
            finally
            {
                Logger.LogTimedMessage("Series client 연결이 종료되었습니다.");

                client.Close();
                connectedSocket = null;
            }
        }

        /// <summary>
        /// 서버를 종료한다.
        /// </summary>
        internal void Stop()
        {
            connectedSocket?.Close();
            connectedSocket = null;

            listeningSocket?.Close();
            listeningSocket = null;
            Running = false;
        }
    }
}
