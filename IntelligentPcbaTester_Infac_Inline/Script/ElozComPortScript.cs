using System;
using System.IO.Ports;
using TestFramework.PluginTestMethod;

public class TestMethod
{
    // 오픈하려는 시리얼 포트 이름.
    private const string comPort = "COM9";

    // 전송하려는 명령.
    // DIO 명령 리스트(PLC) : VER, FIXPWRON, FIXPWROFF, ALLOFF, DISCON, DISCOFF
    // DIO 명령 리스트(Offline) : DOWN, UP, CYLINIT, INIT, FCTDOWN, FCTUP, ...
    // PLC 명령 리스트 : VER, DOWN, UP(실린더+컨베이어), FCTUP, MIDUP(실린더), ...
    private const string command = "DOWN";
    private const string commandPrefix = "@";
    private const string commandRespPrefix = ">";

    // 시리얼 포트 속도.
    private const int baudRate = 115200;

    // 읽기 타임아웃. 실린더 다운 같은 긴 시간이 필요한 명령은 길게 줘야 한다.
    private const int readTimeout = 7000;   // ms

    // 쓰기 타임아웃.
    private const int writeTimeout = 500;  // ms

    // New Line 문자. DIO의 경우 "\r" 이다.
    private const string newLine = "\n";

    public void Execute(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code executing the method
        DebugClear();
        string resultInfo = "";
        TestStep.ResultState resultState;
        SerialPort serialPort = new SerialPort();
        try
        {
            // Configure serial port.
            serialPort.PortName = comPort;
            serialPort.BaudRate = baudRate;
            serialPort.ReadTimeout = readTimeout;
            serialPort.WriteTimeout = writeTimeout;
            serialPort.NewLine = newLine;
            serialPort.Open();

            // Send command.
            string commandLine = commandPrefix + command;
            DebugWriteLine(comPort + " <- " + commandLine);
            serialPort.WriteLine(commandLine);

            // Read response.
            string response = serialPort.ReadLine().TrimEnd();
            response = response.Replace("\0", "\\0");
            DebugWriteLine(comPort + " -> " + response);
            resultInfo = response;
            if (response.StartsWith(commandRespPrefix + command, StringComparison.OrdinalIgnoreCase))
            {
                resultState = TestStep.ResultState.Pass;
            }
            else
            {
                resultState = TestStep.ResultState.Fail;
            }
        }
        catch (Exception ex)
        {
            resultState = TestStep.ResultState.Aborted;
            resultInfo = "Error: " + ex.Message;
            DebugWriteLine(resultInfo);
        }
        finally
        {
            serialPort.Close();
        }

        ThisTestStep.SetResult(
            resultState,
            resultInfo,
            0,
            TestStep.PhysicalUnit.NoPhysicalUnit,
            TestStep.ResultValueState.Invalid);
    }

    public string Info()
    {
        return "";
    }

    public void Validate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code to validate the parameters
    }

    public void RequestResources(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code to request resources
    }

    public void Evaluate(int ProcessID, TestFramework.PluginTestMethod.TestStep ThisTestStep)
    {
        // insert code evaluating the results
    }

    public static void DebugClear()
    {
        TestFramework.Script.Diagnostics.Debug.ClearOutputScreen();
    }

    public static void DebugWriteLine(string message)
    {
        DateTime now = DateTime.Now;
        string timeString = string.Format("[{0:D2}:{1:D2}:{2:D2}.{3:D3}]", now.Hour, now.Minute, now.Second, now.Millisecond);
        TestFramework.Script.Diagnostics.Debug.WriteLine(timeString + " " + message);
    }
}
