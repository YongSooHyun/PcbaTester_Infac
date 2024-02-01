using EOL_GND.View;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.Common
{
    /// <summary>
    /// Logging 기능을 제공한다.
    /// </summary>
    public class Logger
    {
        /// <summary>
        /// 로그를 기록하는 타깃.
        /// </summary>
        public enum LogTarget
        {
            None,
            Window,
            File,
            Both
        }

        /// <summary>
        /// 로그를 표시할 타깃.
        /// </summary>
        internal static LogTarget Target { get; set; } = AppSettings.SharedInstance.AppLogTarget;

        // 로그를 저장할 파일 경로.
        internal static string LogFolder = AppSettings.SharedInstance.AppLogFolderPath;
        internal static string LogFileNameFormat = AppSettings.SharedInstance.AppLogFileNameFormat;

        // DEBUG에 사용되는 컴파일 기호.
        private const string DebugSymbol = "DEBUG";

        /// <summary>
        /// 로그 레벨을 결정하는 스위치.
        /// </summary>
        internal static TraceSwitch Switch { get; } = new TraceSwitch("General", "Whole application");

        static Logger()
        {
            Switch.Level = AppSettings.SharedInstance.AppLogLevel;
        }

        /// <summary>
        /// 메시지를 파일 및 윈도우에 기록한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        private static void LogMessage(string message)
        {
            switch (Target)
            {
                case LogTarget.Window:
                    FormOutputMessage(message);
                    break;
                case LogTarget.File:
                    FileOutputMessage(message);
                    break;
                case LogTarget.Both:
                    FormOutputMessage(message);
                    FileOutputMessage(message);
                    break;
            }
        }

        /// <summary>
        /// 파일에 지정한 메시지를 출력한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        private static void FileOutputMessage(string message)
        {
            //string logFilePath = Path.Combine(Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location) ?? "", LogFileName);

            // 파일이름 생성.
            var fileNameFormat = LogFileNameFormat.Replace("{Date", "{0");
            var fileName = string.Format(fileNameFormat, DateTime.Now);

            string logFilePath = Path.Combine(LogFolder, fileName);
            try
            {
                Directory.CreateDirectory(LogFolder);
                File.AppendAllText(logFilePath, message + Environment.NewLine);
            }
            catch (Exception e)
            {
                FormOutputMessage($"File logging error: {e.Message}");
            }
        }

        /// <summary>
        /// 출력 윈도우에 지정한 메시지를 출력한다.
        /// </summary>
        /// <param name="message">출력하려는 메시지.</param>
        private static void FormOutputMessage(string message)
        {
            var openForms = Application.OpenForms;
            for (int i = 0; i < openForms.Count; i++)
            {
                if (openForms[i] is SequenceForm sequenceForm)
                {
                    Utils.InvokeIfRequired(sequenceForm, () =>
                    {
                        sequenceForm.LogAppendLine(message);
                    });
                }
            }
        }

        /// <summary>
        /// 현재 시간과 지정한 메시지를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        private static void LogTimedMessage(string message)
        {
            var logMessage = "[" + Utils.CreateTimeString(DateTime.Now) + "] " + message;
            LogMessage(logMessage);
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

        internal static void LogError(string format, params object[] args)
        {
            LogError(string.Format(format, args));
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

        internal static void LogWarning(string format, params object[] args)
        {
            LogWarning(string.Format(format, args));
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

        internal static void LogInfo(string format, params object[] args)
        {
            LogInfo(string.Format(format, args));
        }

        /// <summary>
        /// 부가 정보를 기록한다.
        /// </summary>
        /// <param name="message">기록하려는 메시지.</param>
        internal static void LogVerbose(string message)
        {
            if (Switch.TraceVerbose)
            {
                LogTimedMessage(message);
            }
        }

        internal static void LogVerbose(string format, params object[] args)
        {
            LogVerbose(string.Format(format, args));
        }

        /// <summary>
        /// 디버그 정보를 기록한다.
        /// </summary>
        /// <param name="message"></param>
        [Conditional(DebugSymbol)]
        internal static void LogDebug(string message)
        {
            LogTimedMessage(message);
        }

        [Conditional(DebugSymbol)]
        internal static void LogDebug(string format, params object[] args)
        {
            LogDebug(string.Format(format, args));
        }

        /// <summary>
        /// 통신 메시지를 기록한다.
        /// </summary>
        /// <param name="device">장치 이름.</param>
        /// <param name="message">기록하려는 메시지.</param>
        /// <param name="toDevice">통신 방향.</param>
        internal static void LogCommMessage(string device, string message, bool toDevice)
        {
            string logMessage = $"{device} {(toDevice ? "<-" : "->")} {message}";
            LogInfo(logMessage);
        }
    }
}
