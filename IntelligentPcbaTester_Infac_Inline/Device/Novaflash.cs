using System;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Text;
using System.Text.RegularExpressions;

namespace IntelligentPcbaTester
{
    /// <summary>
    /// Novaflash 디바이스와 통신을 진행한다.
    /// </summary>
    partial class Novaflash
    {
        /// <summary>
        /// 장치 이름.
        /// </summary>
        public const string DeviceName = "ISP";

        /// <summary>
        /// Novaflash 파일 타입.
        /// </summary>
        public enum FileType
        {
            Grp = 0,
            Drv = 1,
            Dump = 2,
            Batch = 3,
            Log = 4,
        }

        /// <summary>
        /// 연결 상태.
        /// </summary>
        public bool Connected => handle != IntPtr.Zero;

        // 연결을 나타낸다.
        private IntPtr handle = IntPtr.Zero;

        /// <summary>
        /// Novaflash 디바이스에 LAN 을 통해 연결한다.
        /// </summary>
        /// <param name="ip">IP 주소.</param>
        public void OpenLan(string ip)
        {
            if (handle != IntPtr.Zero)
            {
                return;
            }

            handle = OpenLanConnection(ip);
            if (handle == IntPtr.Zero)
            {
                throw new Exception($"{ip} 에 연결할 수 없습니다.");
            }
        }

        /// <summary>
        /// Novaflash 디바이스에 Serial Port 를 통해 연결한다.
        /// </summary>
        /// <param name="portName">COM 포트 이름('COM1', 'COM2' 등).</param>
        /// <param name="baudRate">통신 속도(bps).</param>
        public void OpenSerial(string portName, int baudRate)
        {
            if (handle != IntPtr.Zero)
            {
                return;
            }

            handle = OpenSerialConnection(portName, baudRate);
            if (handle == IntPtr.Zero)
            {
                throw new Exception($"{portName} 에 연결할 수 없습니다.");
            }
        }

        /// <summary>
        /// Novaflash 디바이스와의 연결을 닫는다.
        /// </summary>
        public void Close()
        {
            if (handle != IntPtr.Zero)
            {
                CloseConnection(handle);
                handle = IntPtr.Zero;
            }
        }

        /// <summary>
        /// Novaflash 디바이스에 명령을 전송한다. 명령이 끝날 때까지 기다리지 않는다.
        /// </summary>
        /// <param name="command">실행하려는 명령.</param>
        public void SendCommand(string command)
        {
            int ret = SendCommand(handle, $"{command}\n");
            if (ret == -1)
            {
                throw new Exception($"{command} 명령 전송 오류가 발생했습니다.");
            }
        }

        /// <summary>
        /// Novaflash 디바이스에서 명령 실행이 끝나기를 기다린다.
        /// </summary>
        /// <param name="timeout">타임아웃 오류를 리턴할 때까지 기다리는 시간(ms).</param>
        /// <returns>실행한 명령의 리턴 값.</returns>
        public ulong WaitEndCommand(int timeout)
        {
            ulong retCode;
            int ret = WaitEndCommand(handle, timeout, out retCode);
            if (ret == -1)
            {
                throw new Exception($"명령을 기다리는 중 오류가 발생했습니다.");
            }

            return retCode;
        }

        /// <summary>
        /// Novaflash 디바이스에서 한줄의 응답을 읽는다.
        /// </summary>
        /// <param name="line">명령의 응답.</param>
        /// <returns>명령이 끝났으면 true를, 아니면 false를 리턴한다.</returns>
        public bool GetRespLine(out string line)
        {
            int ret = GetRespLine(handle, out IntPtr ptrLine);
            line = Marshal.PtrToStringAnsi(ptrLine);
            return ret != 0;
        }

        /// <summary>
        /// Novaflash 디바이스에서 명령의 응답들을 지운다. 실행된 명령의 응답을 무시하려고 할 때 사용된다.
        /// </summary>
        public void ClearResp()
        {
            ClearResp(handle);
        }

        /// <summary>
        /// Novaflash 디바이스에 명령을 전송하고 실행이 끝날 때까지 기다린다.
        /// </summary>
        /// <param name="cmd">실행하려는 명령.</param>
        /// <param name="timeout">타임아웃 오류를 리턴할 때까지 기다리는 시간(ms).</param>
        /// <returns>실행한 명령의 리턴 코드.</returns>
        public ulong ExecCommand(string cmd, int timeout)
        {
            int ret = ExecCommand(handle, $"{cmd}\n", timeout, out ulong retCode);
            if (ret == -1)
            {
                throw new Exception($"{cmd} 명령 실행 오류가 발생했습니다.");
            }
            return retCode;
        }

        /// <summary>
        /// Novaflash 디바이스에 파일을 전송한다.
        /// </summary>
        /// <param name="fileType">전송하려는 파일의 타입.</param>
        /// <param name="srcFilePath">전송하려는 파일 경로.</param>
        /// <param name="destFileName">전송된 다음 파일에 붙일 이름.</param>
        public void SendFile(FileType fileType, string srcFilePath, string destFileName)
        {
            int ret = SendFile(handle, fileType, srcFilePath, destFileName);
            if (ret == -1)
            {
                throw new Exception($"파일을 전송할 수 없습니다.");
            }
        }

        /// <summary>
        /// Novaflash 디바이스에서 파일을 받는다.
        /// </summary>
        /// <param name="fileType">받으려는 파일 타입.</param>
        /// <param name="dstFilePath">받는 파일을 저장할 로컬 경로.</param>
        /// <param name="srcFileName">Novaflash 디바이스에 있는 파일 이름.</param>
        public void GetFile(FileType fileType, string dstFilePath, string srcFileName)
        {
            int ret = GetFile(handle, fileType, dstFilePath, srcFileName);
            if (ret == -1)
            {
                throw new Exception($"파일을 받을 수 없습니다.");
            }
        }

        /// <summary>
        /// Novaflash 디바이스의 모든 응답을 읽는다.
        /// </summary>
        /// <returns>응답 문자열.</returns>
        public string GetAllResponse()
        {
            StringBuilder response = new StringBuilder();
            int bracketCount = 0;
            string line;
            while (true)
            {
                GetRespLine(out line);
                if (line != null)
                {
                    response.AppendLine(line);
                    string trimmed = line.Trim();
                    if (trimmed.StartsWith("<"))
                    {
                        bracketCount++;
                    }
                    if (trimmed.EndsWith(">") || trimmed.EndsWith("$"))
                    {
                        bracketCount--;
                    }
                }

                if (line == null && bracketCount == 0)
                {
                    break;
                }
            }

            return response.ToString();
        }

        /// <summary>
        /// Novaflash 디바이스에 파일이 있는지 체크한다.
        /// </summary>
        /// <param name="type">파일 타입.</param>
        /// <param name="fileName">파일 이름(확장자 포함).</param>
        /// <param name="timeout">명령 실행 타임아웃.</param>
        /// <returns>있으면 true, 없으면 false.</returns>
        public bool FileExists(FileType type, string fileName, int timeout)
        {
            // 파일이 있는지 체크하는 명령을 실행한다.
            ExecCommand($"FS_EXIST {type} {fileName}", timeout);

            // 명령의 출력을 읽는다.
            string output = GetAllResponse();

            // 출력 문자열을 파싱한다.
            string stripped = Utils.RemoveAllWhiteSpaces(output);
            if (stripped == "<>")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Novaflash 디바이스에서 파일을 삭제한다.
        /// </summary>
        /// <param name="type">파일 타입.</param>
        /// <param name="fileName">파일 이름(확장자 포함).</param>
        /// <param name="timeout">명령 실행 타임아웃.</param>
        /// <returns>있으면 true, 없으면 false.</returns>
        public bool DeleteFile(FileType type, string fileName, int timeout)
        {
            // 파일이 있는지 체크하는 명령을 실행한다.
            ExecCommand($"FS_RM {type} {fileName}", timeout);

            // 명령의 출력을 읽는다.
            string output = GetAllResponse();

            // 출력 문자열을 파싱한다.
            string stripped = Utils.RemoveAllWhiteSpaces(output);
            if (stripped == "<>")
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Novaflash 디바이스에 GRP 파일이 있는지 체크한다.
        /// </summary>
        /// <param name="filePath">파일 이름(확장자 포함).</param>
        /// <param name="grpCrc">비교할 GRP CRC.</param>
        /// <param name="timeout">명령 실행 타임아웃.</param>
        /// <returns>있으면 true, 없으면 false.</returns>
        public bool GrpExists(string filePath, uint grpCrc, int timeout)
        {
            // GRP를 얻는 명령을 실행한다.
            var fileName = Path.GetFileName(filePath);
            ExecCommand($"GETGRPCRC {fileName}", timeout);

            // 명령 출력 읽기.
            string output = GetAllResponse();

            // 출력 파싱.
            string stripped = Utils.RemoveAllWhiteSpaces(output);
            //Logger.LogTimedMessage($"NovaFlash Response = {stripped}");
            var comparison = StringComparison.OrdinalIgnoreCase;
            string startWord = "<CRC:";
            string endWord = ">";
            if (stripped.StartsWith(startWord, comparison) && stripped.EndsWith(endWord, comparison))
            {
                var numberText = stripped.Substring(startWord.Length, stripped.Length - startWord.Length - endWord.Length).Trim();
                if (uint.TryParse(numberText, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out uint parsed))
                {
                    //Logger.LogTimedMessage($"NovaFlash {fileName} GRP=0x{grpCrc:X} HYDRA_GRP=0x{parsed:X}");
                    return parsed == grpCrc;
                }
            }

            return false;
        }

        /// <summary>
        /// 지정한 채널 POD 전원을 켜거나 끈다.
        /// </summary>
        /// <param name="channel"></param>
        /// <param name="on"></param>
        /// <param name="timeout"></param>
        public void PodPower(int channel, bool on, int timeout)
        {
            ExecCommand($"PODPOWER {channel} {(on ? "ON" : "OFF")}", timeout);
        }

        /// <summary>
        /// Novaflash 디바이스의 채널 상태를 얻는다.
        /// </summary>
        /// <param name="channel"></param>
        /// <returns></returns>
        public ChannelState GetChannelState(int channel)
        {
            var status = ChannelState.NotConnected;

            ClearResp();
            ExecCommand("GETCHSTATE " + channel, 3000);
            string response = GetAllResponse();
            string stripped = Regex.Replace(response, "\\s+", "");
            if (stripped.Length > 1)
            {
                switch (stripped[1])
                {
                    case 'P':
                        status = ChannelState.Pass;
                        break;
                    case 'F':
                        status = ChannelState.Fail;
                        break;
                    case 'R':
                        status = ChannelState.Running;
                        break;
                    case '_':
                        status = ChannelState.StatusCleared;
                        break;
                    case '!':
                        status = ChannelState.NotConnected;
                        break;
                }
            }

            return status;
        }

        /// <summary>
        /// 파일의 GRP CRC32 를 얻는다.
        /// </summary>
        /// <param name="filePath">GRP 파일 경로 이름.</param>
        /// <returns></returns>
        internal static uint GetGrpCrc32(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return 0;
            }

            // grpMan.exe 를 실행한다.
            string grpManFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "grpMan.exe");
            string output = Utils.ExecuteExternalCommand(grpManFilePath, $"-s \"{filePath}\" -c");

            // 출력을 파싱한다.
            string trimmed = output.Trim();
            if (uint.TryParse(trimmed, System.Globalization.NumberStyles.HexNumber, null, out uint result))
            {
                return result;
            }
            else
            {
                return 0;
            }
        }

        /// <summary>
        /// 파일의 Source CRC32 를 얻는다.
        /// </summary>
        /// <param name="filePath">GRP 파일 경로 이름.</param>
        /// <returns></returns>
        internal static uint GetSourceCrc32(string filePath)
        {
            if (!File.Exists(filePath))
            {
                return 0;
            }

            // grpMan.exe 를 실행한다.
            string grpManFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "grpMan.exe");
            string output = Utils.ExecuteExternalCommand(grpManFilePath, $"-s \"{filePath}\" -p");

            // 출력을 파싱한다.
            var lines = output.Split('\n');

            // SRC CRC 는 두번째 줄에 출력됨. 0x3E24_A69B
            if (lines.Length < 2)
            {
                return 0;
            }

            // _ 제거.
            var hexString = lines[1].Replace("_", "").Trim();

            try
            {
                var converted = Convert.ToUInt32(hexString, 16);
                return converted;
            }
            catch (Exception ex)
            {
                Logger.LogError($"grpMan Source CRC({hexString}): " + ex.Message);
            }

            return 0;
        }

        /// <summary>
        /// 파일의 DATA CRC32 를 얻는다.
        /// </summary>
        /// <param name="filePath">GRP 파일 경로 이름.</param>
        /// <returns></returns>
        internal static uint GetDataCrc32(string filePath)
        {
            if (!File.Exists(filePath))
            {
                throw new Exception("File does not exist: " + filePath);
            }

            // grpMan.exe 를 실행한다.
            string grpManFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "grpMan.exe");
            string output = Utils.ExecuteExternalCommand(grpManFilePath, $"-s \"{filePath}\" -r");

            // 출력을 파싱한다.
            var hexString = output.Trim();

            try
            {
                var converted = Convert.ToUInt32(hexString, 16);
                return converted;
            }
            catch (Exception ex)
            {
                throw new Exception($"grpMan DATA CRC({hexString}): " + ex.Message);
            }
        }

        /// <summary>
        /// 파일이름에 공백, (, ) 이 포함될 때 실행 오류가 생기는 NovaFlash 버그를 방지하기 위한 처리.
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        internal static string GetEscapedFileName(string fileName)
        {
            var replacement = '_';
            return fileName?.Replace(' ', replacement)?.Replace('(', replacement)?.Replace(')', replacement);
        }

        /// <summary>
        /// 지정한 GRP 파일에 ROM 파일을 추가하여 새로운 GRP 파일을 만든다.
        /// </summary>
        /// <param name="inputGrpFile"></param>
        /// <param name="outputGrpFile"></param>
        /// <param name="importFile"></param>
        /// <exception cref="Exception"></exception>
        internal static void CreateGRP(string inputGrpFile, string outputGrpFile, string importFile, string importMemType)
        {
            // 입력 파일 존재 여부 체크.
            if (!File.Exists(inputGrpFile))
            {
                throw new Exception($"파일 '{inputGrpFile}'이 존재하지 않습니다.");
            }

            // Import 파일 존재 여부 체크.
            if (!string.IsNullOrEmpty(importFile) && !File.Exists(importFile))
            {
                throw new Exception($"파일 '{importFile}'이 존재하지 않습니다.");
            }

            // Command arguments.
            var arguments = new StringBuilder();
            arguments.Append($"-s \"{inputGrpFile}\" -o \"{outputGrpFile}\"");
            if (!string.IsNullOrEmpty(importFile))
            {
                string extension = Path.GetExtension(importFile)?.ToLower();
                switch (extension)
                {
                    case ".s19":    // Motorola S19 file.
                        arguments.Append(" -m");
                        break;
                    case ".bin":    // Binary file.
                        arguments.Append(" -b");
                        break;
                    case ".hex":    // Intel hex file.
                    default:
                        arguments.Append(" -i");
                        break;
                }
                arguments.Append($" \"{importFile}\"");

                if (!string.IsNullOrEmpty(importMemType))
                {
                    arguments.Append($" -f {importMemType}");
                }
            }

            // grpMan.exe 를 실행한다.
            string grpManFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", "grpMan.exe");
            string output = Utils.ExecuteExternalCommand(grpManFilePath, arguments.ToString());

            // 성공하면 아무런 출력도 하지 않는다.
            var trimmed = output?.Trim();
            if (!string.IsNullOrEmpty(trimmed))
            {
                throw new Exception($"grpMan GRP creation error: {trimmed}");
            }
        }

        /// <summary>
        /// Novaflash 디바이스의 채널 상태.
        /// </summary>
        internal enum ChannelState
        {
            Pass,
            Fail,
            Running,
            StatusCleared,
            NotConnected
        }
    }
}
