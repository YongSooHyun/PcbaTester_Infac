using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    static class Logger
    {
        /// <summary>
        /// 로그를 기록하는 타깃.
        /// </summary>
        internal enum LogTarget
        {
            None,
            Window,
            File,
            Both
        }

        /// <summary>
        /// 로그를 표시할 타깃.
        /// </summary>
        internal static LogTarget Target { get; set; } = LogTarget.Window;

        // 로그를 저장할 파일 경로.
        private const string LogFileName = "log.txt";

        // DEBUG에 사용되는 컴파일 기호.
        private const string DebugSymbol = "DEBUG";

        /// <summary>
        /// 로그 레벨을 결정하는 스위치.
        /// </summary>
        internal static TraceSwitch Switch { get; } = new TraceSwitch("General", "Whole application");

        static Logger()
        {
            Switch.Level = TraceLevel.Verbose;
        }

        /// <summary>
        /// 메시지를 파일 및 윈도우에 기록한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        internal static void LogMessage(string message, bool isCommMessage)
        {
            switch (Target)
            {
                case LogTarget.Window:
                    OutputMessage(message, isCommMessage);
                    break;
                case LogTarget.File:
                    if (isCommMessage)
                    {
                        FileOutputMessage(message);
                    }
                    break;
                case LogTarget.Both:
                    OutputMessage(message, isCommMessage);
                    if (isCommMessage)
                    {
                        FileOutputMessage(message);
                    }
                    break;
            }
        }

        /// <summary>
        /// 파일에 지정한 메시지를 출력한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        internal static void FileOutputMessage(string message)
        {
            //string logFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", LogFileName);
            string logFilePath = AppSettings.AppLogFilePath;
            StreamWriter writer = null;
            try
            {
                writer = File.AppendText(logFilePath);
                writer.WriteLine(message);
            }
            catch (Exception e)
            {
                OutputMessage(string.Format($"File logging error: {e.Message}"), false);
            }
            finally
            {
                writer?.Close();
            }
        }

        /// <summary>
        /// 출력 윈도우에 지정한 메시지를 출력한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        internal static void OutputMessage(string message, bool isCommMessage)
        {
            var openForms = Application.OpenForms;
            for (int i = 0; i < openForms.Count; i++)
            {
                if (openForms[i] is MainForm mainForm)
                {
                    Utils.InvokeIfRequired(mainForm, () =>
                    {
                        if (isCommMessage)
                        {
                            mainForm.CommLogAppendLine(message);
                        }
                        else
                        {
                            mainForm.TestLogAppendLine(message);
                        }
                    });
                    break;
                }
            }
        }

        /// <summary>
        /// 현재 시간과 지정한 메시지를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        internal static void LogTimedMessage(string message, bool isCommMessage = true)
        {
            var logMessage = Utils.CreateTimeString(DateTime.Now) + " " + message;
            LogMessage(logMessage, isCommMessage);
        }

        /// <summary>
        /// 오류를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        internal static void LogError(string message)
        {
            if (Switch.TraceError)
            {
                LogTimedMessage(message);
            }
        }

        /// <summary>
        /// 경고를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        internal static void LogWarning(string message)
        {
            if (Switch.TraceWarning)
            {
                LogTimedMessage(message);
            }
        }

        /// <summary>
        /// 정보를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        internal static void LogInfo(string message)
        {
            if (Switch.TraceInfo)
            {
                LogTimedMessage(message);
            }
        }

        /// <summary>
        /// 디버그 정보를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        [Conditional(DebugSymbol)]
        internal static void LogDebugInfo(string message)
        {
            LogTimedMessage(message);
        }

        /// <summary>
        /// 통신 메시지를 기록한다.
        /// </summary>
        /// <param name="device">장치 이름.</param>
        /// <param name="message">기록하려는 메시지.</param>
        /// <param name="toDevice">통신 방향.</param>
        internal static void LogCommMessage(string device, string message, bool toDevice)
        {
            LogTimedMessage($"{device} {(toDevice ? "<-" : "->")} {message}", true);
        }
    }
}
