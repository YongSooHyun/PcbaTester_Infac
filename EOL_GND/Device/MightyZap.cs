using EOL_GND.Common;
using Ivi.Visa;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO.Ports;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;

namespace EOL_GND.Device
{
    public class MightyZap : IDisposable
    {
        /// <summary>
        /// 디바이스 이름.
        /// </summary>
        public const string DeviceName = "MightyZap";

        // 시리얼 통신을 위한 SerialPort 클래스 인스턴스.
        private readonly SerialPort serialPort = new SerialPort();

        // Thread-Safe 를 위한 시리얼 포트 송/수신 Lock Object.
        private readonly object commLockObj = new SerialPort();

        // Dispose 패턴에 사용하는 변수.
        private bool disposedValue = false;

        /// <summary>
        /// 명령 코드.
        /// </summary>
        public enum Command : byte
        {
            /// <summary>
            /// Feedback Packet 수신
            /// </summary>
            Echo = 0xF1,

            /// <summary>
            /// Address를 보내고 Data를 Feedback 받음
            /// </summary>
            LoadData = 0xF2,

            /// <summary>
            /// Address와 Data를 보내고 저장
            /// </summary>
            StoreData = 0xF3,

            /// <summary>
            /// Address와 Data를 발송하여 임시 보관시킴
            /// </summary>
            SendData = 0xF4,

            /// <summary>
            /// Send Data를 통한 임시보관 정보를 실행시킴
            /// </summary>
            Execution = 0xF5,

            /// <summary>
            /// 공장 출하 상태인 기본 파라미터로 리셋
            /// </summary>
            FactoryReset = 0xF6,

            /// <summary>
            /// 서보 시스템 재시작
            /// </summary>
            Restart = 0xF8,

            /// <summary>
            /// 다수 서보의 동일한 Address에 Data를 저장
            /// </summary>
            SymmetricStore = 0x73,
        }

        /// <summary>
        /// bit별로 동작 중에 발생한 오류 상태
        /// </summary>
        [Flags]
        public enum Errors : byte
        {
            None = 0x00,

            /// <summary>
            /// 인가된 전압이 Control Table에 설정된 동작 전압 범위를 벗어났을 경우 1로 설정됨
            /// </summary>
            InputVoltageError = 0x01,

            /// <summary>
            /// 지정된 최대 Force로 현재의 하중을 제어할 수 없을 때 1 로 설정됨
            /// </summary>
            OverloadError = 0x20,
        }

        public enum DataAddress : byte
        {
            /// <summary>
            /// 기동력 켜기
            /// </summary>
            ForceOnOff = 0x80,

            /// <summary>
            /// 목표 위치 값의 하위 바이트
            /// </summary>
            GoalPositionLow = 0x86,

            /// <summary>
            /// 목표 위치 값의 상위 바이트
            /// </summary>
            GoalPositionHigh = 0x87,

            /// <summary>
            /// 움직임 유무. 0(default) 또는 1.
            /// </summary>
            Moving = 0x96,
        }

        /// <summary>
        /// 디바이스와 통신하기 위한 Open 을 진행한다.
        /// </summary>
        internal void Open(string portName, int baudRate)
        {
            if (serialPort.IsOpen)
            {
                return;
            }

            serialPort.PortName = portName;
            serialPort.BaudRate = baudRate;

            // 지정한 시간동안 retry.
            int waitTime = 3000;
            var stopwatch = Stopwatch.StartNew();
            while (true)
            {
                try
                {
                    serialPort.Open();
                    break;
                }
                catch (UnauthorizedAccessException uae)
                {
                    Logger.LogError($"Cannot open {portName}: {uae.Message}");
                    if (stopwatch.ElapsedMilliseconds > waitTime)
                    {
                        throw;
                    }
                    else
                    {
                        MultimediaTimer.Delay(30).Wait();
                        continue;
                    }
                }
            }
        }

        /// <summary>
        /// 디바이스와의 통신을 닫는다.
        /// </summary>
        internal void Close()
        {
            serialPort.Close();
        }

        // Serial Port로 명령을 보내고 그 응답을 받아서 리턴한다.
        private byte[] SendCommand(byte servoId, Command command, byte[] parameters, bool readResponse = false, int readTimeout = 500, bool showLog = true)
        {
            lock (commLockObj)
            {
                serialPort.DiscardInBuffer();
                serialPort.DiscardOutBuffer();

                int parameterLength = parameters?.Length ?? 0;
                byte[] writeBuffer = new byte[6 + parameterLength + 1];

                // 헤더 설정.
                writeBuffer[0] = 0xFF;
                writeBuffer[1] = 0xFF;
                writeBuffer[2] = 0xFF;

                // Servo ID.
                writeBuffer[3] = servoId;

                // Packet size.
                writeBuffer[4] = (byte)(1 + parameterLength + 1);

                // Command.
                writeBuffer[5] = (byte)command;

                // Parameters.
                if (parameters != null)
                {
                    Array.Copy(parameters, 0, writeBuffer, 6, parameterLength);
                }

                // Checksum.
                int checksum = writeBuffer.Skip(3).Take(writeBuffer.Length - 4).Sum(x => x);
                writeBuffer[writeBuffer.Length - 1] = (byte)(0xFF - (checksum & 0xFF));

                // 데이터 전송.
                if (showLog)
                {
                    Logger.LogCommMessage(DeviceName, string.Join(" ", writeBuffer.Select(b => $"{b:X2}")), true);
                }
                serialPort.Write(writeBuffer, 0, writeBuffer.Length);

                if (!readResponse)
                {
                    return null;
                }

                // 응답 읽기.
                byte[] readBuffer = new byte[4096];

                // 응답 헤더.
                serialPort.ReadTimeout = readTimeout;
                int readBytes = 0;
                int sizePosition = 4;
                while (readBytes < sizePosition + 1)
                {
                    readBytes += serialPort.Read(readBuffer, readBytes, sizePosition + 1 - readBytes);
                }

                // 나머지 응답 전체.
                int size = readBuffer[sizePosition];
                while (readBytes < sizePosition + 1 + size)
                {
                    readBytes += serialPort.Read(readBuffer, readBytes, sizePosition + 1 + size - readBytes);
                }

                if (showLog)
                {
                    Logger.LogCommMessage(DeviceName, string.Join(" ", readBuffer.Take(readBytes).Select(b => $"{b:X2}")), false);
                }

                return readBuffer.Take(readBytes).ToArray();
            }
        }

        // 응답을 파싱해 파라미터를 리턴한다.
        private byte[] ParseResponse(byte[] readBuffer, out byte servoId, out Errors error)
        {
            servoId = readBuffer[3];
            error = (Errors)readBuffer[5];
            if (error != 0)
            {
                throw new Exception(error.ToString());
            }

            int parameterLength = readBuffer[4] - 2;
            return readBuffer.Skip(6).Take(parameterLength).ToArray();
        }

        /// <summary>
        /// 지정한 위치로 이동.
        /// </summary>
        /// <param name="servoId"></param>
        /// <param name="goalPosition"></param>
        public void Move(byte servoId, short goalPosition)
        {
            byte[] parameters = new byte[3];
            parameters[0] = (byte)DataAddress.GoalPositionLow;
            parameters[1] = (byte)(goalPosition & 0xFF);
            parameters[2] = (byte)((goalPosition & 0xFF00) >> 8);
            SendCommand(servoId, Command.SendData, parameters);

            // 5ms delay.
            MultimediaTimer.Delay(5).Wait();

            SendCommand(servoId, Command.Execution, null);
        }

        /// <summary>
        /// 모터의 움직임 유무 리턴.
        /// </summary>
        /// <param name="servoId"></param>
        /// <returns>움직이고 있으면 true, 정지하고 있으면 false.</returns>
        /// <exception cref="Exception"></exception>
        public bool GetMovingState(byte servoId)
        {
            var readData = SendCommand(servoId, Command.LoadData, new byte[] { (byte)DataAddress.Moving, 1 }, true);

            // 에러 검사.
            if (readData == null || readData.Length < 6)
            {
                throw new Exception($"{DeviceName} response error");
            }

            // 에러 코드.
            var errors = (Errors)readData[5];
            if (errors != 0)
            {
                throw new Exception($"{DeviceName} response error code = {errors}");
            }

            return readData[6] != 0;
        }

        protected virtual void Dispose(bool disposing)
        {
            if (!disposedValue)
            {
                if (disposing)
                {
                    // TODO: dispose managed state (managed objects)
                    Close();
                }

                // TODO: free unmanaged resources (unmanaged objects) and override finalizer
                // TODO: set large fields to null
                disposedValue = true;
            }
        }

        // // TODO: override finalizer only if 'Dispose(bool disposing)' has code to free unmanaged resources
        // ~DioDevice()
        // {
        //     // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
        //     Dispose(disposing: false);
        // }

        public void Dispose()
        {
            // Do not change this code. Put cleanup code in 'Dispose(bool disposing)' method
            Dispose(disposing: true);
            GC.SuppressFinalize(this);
        }
    }
}
