using System;
using System.Collections.Generic;
using System.Net;
using System.Net.Sockets;
using System.Text;
using System.Threading;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// MES 서버 시작, 종료 및 통신을 관리한다.
    /// </summary>
    class MesServer
    {
        /// <summary>
        /// 싱글턴 인스턴스.
        /// </summary>
        internal static readonly MesServer SharedIctServer = new MesServer("ICT");
        internal static readonly MesServer SharedEolServer = new MesServer("EOL");

        /// <summary>
        /// ICT, EOL 서버 2개가 실행되므로 구분하기 위해 이름 적용.
        /// </summary>
        internal string Name { get; private set; }

        /// <summary>
        /// 서버가 실행중인가를 나타낸다.
        /// </summary>
        internal bool IsRunning { get; private set; } = false;

        /// <summary>
        /// MES client가 연결되었는지를 나타낸다.
        /// </summary>
        internal bool ClientConnected => connectedClient != null;

        /// <summary>
        /// 서버가 리스닝하고있는 포트.
        /// </summary>
        internal int ListeningPort { get; private set; }

        // 서버 리스닝 소켓.
        private Socket listeningSocket = null;

        // 연결 대기열 길이.
        private const int BackLog = 1;

        // 연결된 클라이언트 소켓.
        private Socket connectedClient = null;

        // Socket 송/수신 버퍼.
        private const int ReceiveBufferSize = 65535;
        private const int SendBufferSize = 65535;

        // Keep-Alive 체크 메서드에 메시지 송수신중임을 알리는 깃발.
        private readonly ManualResetEvent messagingEvent = new ManualResetEvent(false);

        internal MesServer(string name)
        {
            Name = name;
        }

        /// <summary>
        /// 서버를 스레드 풀에서 시작한다.
        /// </summary>
        /// <param name="port"></param>
        internal void StartServer(int port)
        {
            ListeningPort = port;
            ThreadPool.QueueUserWorkItem(DoStart);
        }

        private void DoStart(object state)
        {
            try
            {
                Start(ListeningPort);
            }
            catch (Exception e)
            {
                Logger.LogTimedMessage($"MES({Name}) 서버 오류: {e.Message}");
                Logger.LogDebugInfo(e.StackTrace);
            }
        }

        /// <summary>
        /// 서버를 시작한다.
        /// </summary>
        /// <param name="port">리스닝하려는 포트.</param>
        private void Start(int port)
        {
            try
            {
                listeningSocket = new Socket(AddressFamily.InterNetwork, SocketType.Stream, ProtocolType.Tcp);
                listeningSocket.Bind(new IPEndPoint(IPAddress.Any, port));
                listeningSocket.Listen(BackLog);
                ListeningPort = port;
                IsRunning = true;

                Logger.LogTimedMessage($"MES({Name}) 서버가 포트 {port} 에서 시작되었습니다.");

                while (true)
                {
                    Socket client = listeningSocket.Accept();

                    Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 이 연결되었습니다.");

                    // 한번에 클라이언트 하나만 처리할 수 있다.
                    ProcessClient(client);
                }
            }
            finally
            {
                IsRunning = false;
                listeningSocket?.Close();
                listeningSocket = null;

                Logger.LogTimedMessage($"MES({Name}) 서버가 종료되었습니다.");
            }
        }

        /// <summary>
        /// 서버를 종료한다.
        /// </summary>
        internal void Stop()
        {
            connectedClient?.Shutdown(SocketShutdown.Both);
            connectedClient?.Close();
            connectedClient = null;

            listeningSocket?.Close();
            listeningSocket = null;
            IsRunning = false;
        }

        // 클라이언트 접속을 처리한다.
        // 클라이언트로부터 Keep-Alive 메시지를 주기적으로 읽는다.
        // 만일 일정 시간동안 Keep-Alive 메시지를 받지 못하면 서버를 Restart한다.
        private void ProcessClient(Socket client)
        {
            connectedClient = client;
            //connectedClient.NoDelay = true;

            try
            {
                client.SendBufferSize = SendBufferSize;
                client.ReceiveBufferSize = ReceiveBufferSize;

                int waitTimeout = AppSettings.MesKeepAliveTimeout;
                int elapsedTime = 0;
                const int sleepTime = 100;
                while (true)
                {
                    // 메시지 송수신 중인가 체크한다.
                    bool messaging = messagingEvent.WaitOne(sleepTime);
                    if (messaging)
                    {
                        elapsedTime = 0;
                        Thread.Sleep(sleepTime);
                        continue;
                    }

                    elapsedTime += sleepTime;

                    // 메시지 송수신 중이 아니라면, 버퍼에 있는 데이터를 체크한다.
                    int availableBytes = client.Available;

                    // 소켓 연결상태 체크.
                    bool readable = client.Poll(1000, SelectMode.SelectRead);
                    if (readable && availableBytes == 0)
                    {
                        // Disconnected.
                        Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 연결 종료");
                        client.Close();
                        connectedClient = null;
                        break;
                    }

                    if (availableBytes > 0)
                    {
                        // 읽기 가능한 데이터가 있어도, elapsed Time 전에는 체크하지 않는다.
                        if (elapsedTime <= waitTimeout)
                        {
                            continue;
                        }

                        // STX 읽기.
                        // 메시지 송수신을 할 동안 대기.
                        //client.ReceiveTimeout = 100;
                        byte[] receiveBuffer = new byte[availableBytes];
                        int receivedBytes = client.Receive(receiveBuffer);

                        if (receivedBytes == 0)
                        {
                            // 클라이언트 소켓 셧다운.
                            Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 이 셧다운되었습니다.");
                            client.Close();
                            connectedClient = null;
                            return;
                        }
                        else
                        {
                            //Logger.LogTimedMessage($"Debug: Keep-Alive: Received 0x{receiveBuffer[0]:X2}.");
                            if (receiveBuffer[0] != MesMessage.FrameStartChar)
                            {
                                // Keep-Alive 오류.
                                Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 가 약속되지 않은 Keep-Alive 문자 {receiveBuffer[0]:X2} 를 전송했습니다.");

                                // 클라이언트 통신 종료, 서버 재부팅.
                                Stop();
                                Start(ListeningPort);
                                return;
                            }
                            else
                            {
                                // Keep-Alive 수신.
                                elapsedTime = 0;
                            }
                        }
                    }
                    else
                    {
                        if (elapsedTime > waitTimeout)
                        {
                            // 시간 초과.
                            Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 가 Keep-Alive 문자를 보내지 않아 서버를 재부팅합니다.");
                            Stop();
                            Start(ListeningPort);
                            return;
                        }
                    }
                }
            }
            catch (Exception e)
            {
                //if (e is SocketException se && se.SocketErrorCode == SocketError.TimedOut)
                //{
                //    // 타임아웃이 발생한 경우, 서버를 재부팅한다.
                //    Logger.LogTimedMessage($"MES client {client.RemoteEndPoint} 가 Keep-Alive 문자를 보내지 않아 서버를 재부팅합니다.");
                //    Stop();
                //    Start(ListeningPort);
                //}
                //else
                //{
                    client?.Close();
                    connectedClient = null;

                    // 일반 오류의 경우, 에러 메시지를 출력하고 클라이언트와의 통신을 종료한다.
                    Logger.LogTimedMessage($"MES({Name}) client {client.RemoteEndPoint} 연결 종료: {e.Message}");
                //}
            }
        }

        /// <summary>
        /// MES client에 요청을 보내고 필요한 경우 응답을 받는다.
        /// </summary>
        /// <param name="request">MES client에 보내려는 요청.</param>
        /// <param name="response">응답이 있는 경우(Alarm이 아닌 경우) 서버 응답, 응답이 없는 경우 null.</param>
        internal void SendRequest(MesRequestMessage request, out MesResponseMessage response)
        {
            if (request == null)
            {
                throw new ArgumentNullException(nameof(request));
            }

            if (connectedClient == null)
            {
                throw new Exception($"MES({Name}) client가 연결되지 않았습니다.");
            }

            try
            {
                Logger.LogInfo($"MES({Name}) send request");
                byte[] sendBuffer = request.Encode();

                // MES client로부터 Keep-Alive신호를 받지 않도록 한다.
                messagingEvent.Set();
                Thread.Sleep(150);

                // 데이터를 전송하고 응답을 기다린다.
                connectedClient.SendTimeout = AppSettings.MesT3Timeout;
                connectedClient.ReceiveTimeout = AppSettings.MesT3Timeout;

                // Debugging.
                //Logger.LogTimedMessage($"Debug: Sending {sendBuffer.Length} bytes");

                int sentBytes = connectedClient.Send(sendBuffer);

                // Debugging.
                Logger.LogTimedMessage($"MES({Name}) Debug: Sent {sentBytes} bytes");

                if (sentBytes != sendBuffer.Length)
                {
                    string errorMessage = $"MES({Name}) client {connectedClient.RemoteEndPoint} 요청 전송 오류: {sendBuffer.Length} 바이트 중 {sentBytes} 바이트 전송";
                    throw new Exception(errorMessage);
                }

                if (request.ProcessFlag != MesMessage.MessageType.MA)
                {
                    // 응답을 받아 파싱한다.
                    byte[] receiveBuffer = new byte[ReceiveBufferSize];

                    int totalReceivedBytes = 0;
                    while (true)
                    {
                        int receivedBytes = connectedClient.Receive(receiveBuffer, totalReceivedBytes, receiveBuffer.Length - totalReceivedBytes, SocketFlags.None);

                        // Debugging.
                        Logger.LogTimedMessage($"MES({Name}) Debug: Received {receivedBytes} bytes");

                        //if (receivedBytes > 0)
                        //{
                        //    var receivedMessage = Encoding.ASCII.GetString(receiveBuffer, totalReceivedBytes, receivedBytes);
                        //    Logger.LogTimedMessage($"Debug: Received Message: {receivedMessage}.");
                        //}

                        if (receivedBytes == 0)
                        {
                            // Client shutdown.
                            string errorMessage = $"MES({Name}) request client {connectedClient.RemoteEndPoint} 이 셧다운 되었습니다.";
                            connectedClient.Close();
                            connectedClient = null;
                            throw new Exception(errorMessage);
                        }
                        else
                        {
                            totalReceivedBytes += receivedBytes;
                            if (receiveBuffer[totalReceivedBytes - 1] == MesMessage.FrameEndChar)
                            {
                                break;
                            }
                        }
                    }

                    response = MesResponseMessage.Decode(receiveBuffer, 0, totalReceivedBytes);
                }
                else
                {
                    response = null;
                }
            }
            finally
            {
                messagingEvent.Reset();
            }
        }
    }
}
