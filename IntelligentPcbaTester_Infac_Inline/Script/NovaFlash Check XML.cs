// 채널검사 스크립트
using TestFramework.PluginTestMethod;
using TestFramework.Common.StatusLogging;
using System;
using System.Text;
using System.Text.RegularExpressions;
using System.Runtime.InteropServices;
using System.IO;
using System.Collections.Generic;
using System.Xml.Serialization;

public class TestMethod
{
    // Novaflash 연결정보, 실행할 GRP 정보들이 들어있는 임시파일 경로(XML).
    private const string NovaTempFile = "D:\\ElozPlugin\\ipts_nova_grp_files.temp";

    // 테스트 결과를 체크하는 NovaFlash 채널 리스트.
    // 여기에 지정된 모든 채널들이 PASS로 되어야 최종 결과가 PASS로 된다.
    private readonly int[] checkingChannels = new int[] { 1 };

    // eloZ1 스크립트 내에서 추가로 헤더에 정보를 보여주려고 할 때 사용.
    private const string HeaderStart = "HEADER:";
    private const string FailMsgStart = "FAIL_MSG:";
    private const string HeaderSeparator = "|#|";

    public void Execute(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // Dispaly Messsage
        Log.SetStatus("Checking NovaFlash Status...");
        DebugClear();

        Novaflash device = new Novaflash();
        bool succeeded = true;
        string infoText = "";
        try
        {
            // NovaFlash 설정 정보 읽기.
            NovaflashData novaData = NovaflashData.Load(NovaTempFile);

            // Establish connection to the NovaFlash device.
            if (novaData.LanConnection)
            {
                DebugWriteLine("Connecting to LAN IP " + novaData.LanAddress);
                device.OpenLan(novaData.LanAddress);
            }
            else
            {
                DebugWriteLine("Connecting to Serial Port= " + novaData.SerialPortName + ", BaudRate=" + novaData.SerialBaudRate);
                device.OpenSerial(novaData.SerialPortName, novaData.SerialBaudRate);
            }

            // Command timeout in milliseconds. Large enough for infinite timeout.
            int timeout = 10000000;

            // Read channel states.
            device.ExecCommand("GETCHSTATE", timeout);

            // Parse the response.
            bool[] channelPassed = new bool[] { true, true, true, true };
            string response = device.GetAllResponse();
            DebugWriteLine("Channel State: " + response);
            string stripped = Regex.Replace(response, "\\s+", "");
            if (stripped.Length > 4 * 4 + 1)
            {
                for (int i = 0; i < 4; i++)
                {
                    string chState = stripped.Substring(i * 4 + 1, 4);

                    DebugWriteLine("Channel " + (i + 1) + " State = " + chState);

                    if (chState.StartsWith("P", StringComparison.OrdinalIgnoreCase))
                    {
                        // Passed.
                        channelPassed[i] = true;
                    }
                    else
                    {
                        // Failed.
                        channelPassed[i] = false;
                    }
                }
            }

            // Check if all channels are passed.
            foreach (int channel in checkingChannels)
            {
                if (channelPassed[channel - 1] == false)
                {
                    succeeded = false;
                }

                // Info text.
                if (infoText != "")
                {
                    infoText += ", ";
                }
                infoText += string.Format("CH{0} {1}", channel, channelPassed[channel - 1] ? "PASS" : "FAIL");
            }
        }
        catch (Exception ex)
        {
            DebugWriteLine("NovaFlash Error: " + ex.ToString());
            infoText = "Error: " + ex.Message;
            succeeded = false;
        }
        finally
        {
            device.Close();
        }

        string resultValueInfo = HeaderStart + "FLASHING_RESULT: " + infoText;

        // Set the test result.
        if (succeeded)
        {
            // Success.
            ThisTestStep.SetResult(
                TestStep.ResultState.Pass,
                infoText,
                0,
                TestStep.PhysicalUnit.NoPhysicalUnit,
                TestStep.ResultValueState.Invalid,
                resultValueInfo);

        }
        else
        {
            // Failed.
            ThisTestStep.SetResult(
                TestStep.ResultState.Fail,
                infoText,
                0,
                TestStep.PhysicalUnit.NoPhysicalUnit,
                TestStep.ResultValueState.Invalid,
                resultValueInfo + HeaderSeparator + FailMsgStart + "Flashing_FAIL");
        }
    }

    public void Evaluate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // Insert code evaluating the results.
        // Nothing to do if evaluation is already done in Execute.
    }


    public string Info()
    {
        return "";  // Insert a description of the script
    }

    public void Validate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // Insert code to validate the parameters if parameters are defined.
    }

    public void RequestResources(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // Insert code to request resources if eloZ1 hardware is involved.
        TestFramework.TestDevices.TesterController.ResourceRequest(ProcessID, TestFramework.TestDevices.TesterController.EvaluationModes.EarlyEvaluation);
    }

    public static void DebugClear()
    {
        TestFramework.Script.Diagnostics.Debug.ClearOutputScreen();
    }

    internal void DebugWriteLine(string message)
    {
        DateTime now = DateTime.Now;
        string timeString = string.Format("[{0:D2}:{1:D2}:{2:D2}.{3:D3}]", now.Hour, now.Minute, now.Second, now.Millisecond);
        TestFramework.Script.Diagnostics.Debug.WriteLine(timeString + " " + message);
    }
}

public class NovaflashData
{
    /// <summary>
    /// Novaflash Controller에 LAN을 이용하여 연결할지 여부.
    /// </summary>
    public bool LanConnection { get; set; }

    /// <summary>
    /// Novaflash Controller에 LAN을 이용해 연결할 때 연결할 주소.
    /// </summary>
    public string LanAddress { get; set; }

    /// <summary>
    /// Novaflash Controller에 연결하기 위한 시리얼 포트 이름.
    /// </summary>
    public string SerialPortName { get; set; }

    /// <summary>
    /// Novaflash Controller에 연결하기 위한 시리얼 통신 속도.
    /// </summary>
    public int SerialBaudRate { get; set; }

    /// <summary>
    /// 실행하려는 GRP 정보 리스트.
    /// </summary>
    public List<GrpInfo> GrpFiles { get; set; }

    /// <summary>
    /// 지정된 파일 경로에 XML 형식으로 데이터를 보관한다.
    /// </summary>
    /// <param name="filePath"></param>
    public void Save(string filePath)
    {
        using (var writer = new StreamWriter(filePath))
        {
            var xmlSerializer = new XmlSerializer(typeof(NovaflashData), "IntelligentPcbaTester");
            xmlSerializer.Serialize(writer, this);
        }
    }

    /// <summary>
    /// 지정된 XML 형식 파일 데이터로부터 로딩하여 클래스 인트턴스를 만든다.
    /// </summary>
    /// <param name="filePath"></param>
    /// <returns></returns>
    public static NovaflashData Load(string filePath)
    {
        using (var stream = new FileStream(filePath, FileMode.Open))
        {
            var xmlSerializer = new XmlSerializer(typeof(NovaflashData), "IntelligentPcbaTester");
            var obj = xmlSerializer.Deserialize(stream);
            return obj as NovaflashData;
        }
    }
}

public class GrpInfo
{
    /// <summary>
    /// 실행할 채널 번호. 현재 가능한 번호는 1 ~ 4 이다.
    /// </summary>
    public int Channel { get; set; } = 1;

    /// <summary>
    /// GRP 파일 경로. 반드시 존재해야 하며, Import파일이 있으면 빈 GRP, 없으면 비지 않은 GRP 이어야 한다.
    /// </summary>
    public string GrpFilePath { get; set; }

    /// <summary>
    /// Import 파일 경로.
    /// </summary>
    public string ImportFilePath { get; set; }

    /// <summary>
    /// 실행 순서. 낮은 실행순서가 먼저 실행되며 같은 실행순서는 동시에 실행된다.
    /// </summary>
    public int RunOrder { get; set; } = 1;

    /// <summary>
    /// CRC 값.
    /// </summary>
    public uint Crc { get; set; }

    /// <summary>
    /// MES CRC 값과 비교할지 여부.
    /// </summary>
    public bool CheckForMes { get; set; }

    /// <summary>
    /// 다운로드할 ROM 파일 이름. Import 파일이 없으면 GRP 파일이름을, Import 파일이 있으면 그 파일이름을 리턴한다.
    /// </summary>
    public string RomFileName
    {
        get
        {
            if (string.IsNullOrWhiteSpace(ImportFilePath))
            {
                return Path.GetFileName(GrpFilePath);
            }
            else
            {
                return Path.GetFileName(ImportFilePath);
            }
        }
    }

    /// <summary>
    /// 실행하기 위해 Hydra 디바이스로 업로드할 파일 이름 생성.
    /// </summary>
    public static string GetUploadFileName(string grpFilePath, string importFilePath)
    {
        string fileName;
        if (string.IsNullOrWhiteSpace(importFilePath))
        {
            fileName = Path.GetFileName(grpFilePath);
        }
        else
        {
            fileName = Path.GetFileName(importFilePath);
        }

        if (string.IsNullOrWhiteSpace(fileName))
        {
            return null;
        }

        fileName = Path.ChangeExtension(fileName, ".grp");

        // NovaFlash 에러 방지를 위해 파일이름에서 공백, 괄호를 '_'로 교체한다.
        return Novaflash.GetEscapedFileName(fileName);
    }
}

/// <summary>
/// Novaflash 디바이스와 통신을 진행한다.
/// </summary>
class Novaflash
{
    // FNDLL_API fnConnHandle_p fn_openLanConnection(const WCHAR *ip);
    [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_openLanConnection")]
    private static extern IntPtr OpenLanConnection(string ip);

    // FNDLL_API fnConnHandle_p fn_openSerialConnection(const WCHAR *device, int baudRate);
    [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_openSerialConnection")]
    private static extern IntPtr OpenSerialConnection(string device, int baudRate);

    // FNDLL_API void fn_CloseConnection(fnConnHandle_p handle);
    [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_CloseConnection")]
    private static extern void CloseConnection(IntPtr handle);

    // FNDLL_API int fn_sendFile(fnConnHandle_p handle, int fileType, const WCHAR *srcFilePath, const WCHAR *destFileName);
    [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_sendFile")]
    private static extern int SendFile(IntPtr handle, FileType fileType, string srcFilePath, string destFileName);

    // FNDLL_API int fn_getFile(fnConnHandle_p handle, int fileType, const WCHAR *dstFilePath, const WCHAR *srcFileName);
    [DllImport("fn-dll.dll", CharSet = CharSet.Unicode, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_getFile")]
    private static extern int GetFile(IntPtr handle, FileType fileType, string dstFilePath, string srcFileName);

    // FNDLL_API int fn_sendCommand(fnConnHandle_p handle, const char *cmd);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_sendCommand")]
    private static extern int SendCommand(IntPtr handle, string cmd);

    // FNDLL_API int fn_waitEndCommand(fnConnHandle_p handle, int timeOut, unsigned long *retCode);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_waitEndCommand")]
    private static extern int WaitEndCommand(IntPtr handle, int timeout, out ulong retCode);

    // FNDLL_API int fn_getRespLine(fnConnHandle_p handle, const char **line);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_getRespLine")]
    private static extern int GetRespLine(IntPtr handle, out IntPtr ptrLine);

    // FNDLL_API int fn_clearResp(fnConnHandle_p handle);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_clearResp")]
    private static extern int ClearResp(IntPtr handle);

    // FNDLL_API int fn_execCommand(fnConnHandle_p handle, const char *cmd, int timeOut, unsigned long *retCode);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_execCommand")]
    private static extern int ExecCommand(IntPtr handle, string cmd, int timeout, out ulong retCode);

    // FNDLL_API void fn_setProgressCb(fnConnHandle_p handle, fnProgressEvent_p cb, void* clientp);
    public delegate int ProgressEvent(IntPtr clienttp, double dltotal, double dlnow, double ultotal, double ulnow);
    [DllImport("fn-dll.dll", CharSet = CharSet.Ansi, CallingConvention = CallingConvention.Cdecl, EntryPoint = "fn_setProgressCb")]
    private static extern void SetProgressCb(IntPtr handle, ProgressEvent cb, IntPtr clientp);

    /// <summary>
    /// Novaflash 파일 타입.
    /// </summary>
    internal enum FileType
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

    internal bool Connected
    {
        get
        {
            return handle != IntPtr.Zero;
        }
    }

    // 연결을 나타낸다.
    private IntPtr handle = IntPtr.Zero;

    /// <summary>
    /// Novaflash 디바이스에 LAN 을 통해 연결한다.
    /// </summary>
    /// <param name="ip">IP 주소.</param>
    internal void OpenLan(string ip)
    {
        handle = OpenLanConnection(ip);
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Can't connect to Novaflash device using LAN.");
        }
    }

    /// <summary>
    /// Novaflash 디바이스에 Serial Port 를 통해 연결한다.
    /// </summary>
    /// <param name="portName">COM 포트 이름('COM1', 'COM2' 등).</param>
    /// <param name="baudRate">통신 속도(bps).</param>
    internal void OpenSerial(string portName, int baudRate)
    {
        handle = OpenSerialConnection(portName, baudRate);
        if (handle == IntPtr.Zero)
        {
            throw new Exception("Can't connect to Novaflash device using Serial.");
        }
    }

    /// <summary>
    /// Novaflash 디바이스와의 연결을 닫는다.
    /// </summary>
    internal void Close()
    {
        CloseConnection(handle);
    }

    /// <summary>
    /// Novaflash 디바이스에 명령을 전송한다. 명령이 끝날 때까지 기다리지 않는다.
    /// </summary>
    /// <param name="command">실행하려는 명령.</param>
    internal void SendCommand(string command)
    {
        int ret = SendCommand(handle, command + "\n");
        if (ret == -1)
        {
            throw new Exception("Error occurred while sending a command.");
        }
    }

    /// <summary>
    /// Novaflash 디바이스에서 명령 실행이 끝나기를 기다린다.
    /// </summary>
    /// <param name="timeout">타임아웃 오류를 리턴할 때까지 기다리는 시간(ms).</param>
    /// <returns>실행한 명령의 리턴 값.</returns>
    internal ulong WaitEndCommand(int timeout)
    {
        ulong retCode;
        int ret = WaitEndCommand(handle, timeout, out retCode);
        if (ret == -1)
        {
            throw new Exception("Error occurred while waiting a command to finish.");
        }

        return retCode;
    }

    /// <summary>
    /// Novaflash 디바이스에서 한줄의 응답을 읽는다.
    /// </summary>
    /// <param name="line">명령의 응답.</param>
    /// <returns>명령이 끝났으면 true를, 아니면 false를 리턴한다.</returns>
    internal bool GetRespLine(out string line)
    {
        IntPtr ptrLine;
        int ret = GetRespLine(handle, out ptrLine);
        line = Marshal.PtrToStringAnsi(ptrLine);
        return ret != 0;
    }

    /// <summary>
    /// Novaflash 디바이스에서 명령의 응답들을 지운다. 실행된 명령의 응답을 무시하려고 할 때 사용된다.
    /// </summary>
    internal void ClearResp()
    {
        ClearResp(handle);
    }

    /// <summary>
    /// Novaflash 디바이스에 명령을 전송하고 실행이 끝날 때까지 기다린다.
    /// </summary>
    /// <param name="cmd">실행하려는 명령.</param>
    /// <param name="timeout">타임아웃 오류를 리턴할 때까지 기다리는 시간(ms).</param>
    /// <returns>실행한 명령의 리턴 코드.</returns>
    internal ulong ExecCommand(string cmd, int timeout)
    {
        ulong retCode;
        int ret = ExecCommand(handle, cmd + "\n", timeout, out retCode);
        if (ret == -1)
        {
            throw new Exception("Error occurred while executing a command.");
        }
        return retCode;
    }

    /// <summary>
    /// Novaflash 디바이스에 파일을 전송한다.
    /// </summary>
    /// <param name="fileType">전송하려는 파일의 타입.</param>
    /// <param name="srcFilePath">전송하려는 파일 경로.</param>
    /// <param name="destFileName">전송된 다음 파일에 붙일 이름.</param>
    internal void SendFile(FileType fileType, string srcFilePath, string destFileName)
    {
        int ret = SendFile(handle, fileType, srcFilePath, destFileName);
        if (ret == -1)
        {
            throw new Exception("Can't send a file.");
        }
    }

    /// <summary>
    /// Novaflash 디바이스에서 파일을 받는다.
    /// </summary>
    /// <param name="fileType">받으려는 파일 타입.</param>
    /// <param name="dstFilePath">받는 파일을 저장할 로컬 경로.</param>
    /// <param name="srcFileName">Novaflash 디바이스에 있는 파일 이름.</param>
    internal void GetFile(FileType fileType, string dstFilePath, string srcFileName)
    {
        int ret = GetFile(handle, fileType, dstFilePath, srcFileName);
        if (ret == -1)
        {
            throw new Exception("Can't receive a file.");
        }
    }

    /// <summary>
    /// Novaflash 디바이스의 모든 응답을 읽는다.
    /// </summary>
    /// <returns>응답 문자열.</returns>
    internal string GetAllResponse()
    {
        StringBuilder response = new StringBuilder();
        int bracketCount = 0;
        string line;
        while (true)
        {
            GetRespLine(out line);
            if (line != null)
            {
                response.Append(line);
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
    /// 파일이름에 공백, (, ) 이 포함될 때 실행 오류가 생기는 NovaFlash 버그를 방지하기 위한 처리.
    /// </summary>
    /// <param name="fileName"></param>
    /// <returns></returns>
    internal static string GetEscapedFileName(string fileName)
    {
        if (fileName == null)
        {
            return null;
        }

        var replacement = '_';
        return fileName.Replace(' ', replacement).Replace('(', replacement).Replace(')', replacement);
    }
}
