using Force.Crc32;
using InfoBox;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.IO.Ports;
using System.Net;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Windows.Forms;

namespace IntelligentPcbaTester
{
    internal static class Utils
    {
        private const int SW_MAXIMIZE = 3;
        private const int SW_MINIMIZE = 6;
        
        [DllImport("user32.dll")]
        [return: MarshalAs(UnmanagedType.Bool)]
        static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        public static extern IntPtr FindWindowEx(IntPtr hwndParent, IntPtr hwndChildAfter, string lpszClass, string lpszWindow);

        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, uint wParam, string lParam);
        [DllImport("user32.dll")]
        public static extern int SendMessage(IntPtr hWnd, uint uMsg, uint wParam, int lParam);

        private const uint WM_PASTE = 0x0302;
        private const uint WM_GETTEXTLENGTH = 0x000E;
        private const uint EM_SETSEL = 0x00B1;

        [DllImport("user32.dll")]
        public static extern bool SetForegroundWindow(IntPtr hWnd);

        /// <summary>
        /// 지정한 시간을 형식에 맞춰 문자열로 변환해 리턴한다.
        /// </summary>
        /// <param name="time">문자열로 변환하려는 시간.</param>
        /// <param name="includeMillisecond">밀리초를 표시할 것인지 여부.</param>
        /// <returns>"hh:mm:ss.mmm"형식의 시간 문자열.</returns>
        internal static string CreateTimeString(DateTime time, bool includeMillisecond = true)
        {
            return $"[{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}" + (includeMillisecond ? $".{time.Millisecond:D3}" : "") + "]";
        }

        /// <summary>
        /// 메인 스레드가 아닌 다른 스레드에서 UI를 업데이트 할 때 메인 스레드에서 코드가 실행되도록 한다.
        /// </summary>
        /// <typeparam name="T"><see cref="Control"/>을 상속하는 임의의 클래스.</typeparam>
        /// <param name="obj">UI 인스턴스. 이 인스턴스를 소유한 스레드에서 <paramref name="action"/>이 실행된다.</param>
        /// <param name="action">실행할 코드를 담은 <see cref="MethodInvoker"/> 델리게이트.</param>
        internal static void InvokeIfRequired<T>(T obj, MethodInvoker action, bool async = false) where T : Control
        {
            if (obj.InvokeRequired)
            {
                if (async)
                {
                    obj.BeginInvoke(action);
                }
                else
                {
                    obj.Invoke(action);
                }
            }
            else
            {
                action();
            }
        }

        /// <summary>
        /// Local IP주소를 리턴한다.
        /// </summary>
        /// <returns>IPv4 주소.</returns>
        internal static IPAddress GetLocalIPAddress()
        {
            var host = Dns.GetHostEntry(Dns.GetHostName());
            var ipList = new List<IPAddress>();
            foreach (var ipAddr in host.AddressList)
            {
                if (ipAddr.AddressFamily == System.Net.Sockets.AddressFamily.InterNetwork)
                {
                    ipList.Add(ipAddr);
                }
            }

            if (ipList.Count == 0)
            {
                return null;
            }
            else
            {
                foreach (var ipAddr in ipList)
                {
                    string addrText = ipAddr.ToString();
                    if (addrText.StartsWith("192.168.131") || addrText.StartsWith("192.168.130"))
                    {
                        return ipAddr;
                    }
                }

                return ipList[0];
            }
        }

        /// <summary>
        /// Assembly Description.
        /// </summary>
        internal static string AssemblyDescription
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyDescriptionAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyDescriptionAttribute)attributes[0]).Description;
            }
        }

        /// <summary>
        /// 파일 이름이 유효한 파일 이름인가를 검사한다.
        /// </summary>
        /// <param name="fileName">검사하려는 파일 이름.</param>
        /// <returns></returns>
        internal static bool IsValidFileName(string fileName)
        {
            char[] invalidFileNameChars = Path.GetInvalidFileNameChars();
            return fileName?.IndexOfAny(invalidFileNameChars) < 0;
        }

        /// <summary>
        /// 경로가 유효한가를 검사한다.
        /// </summary>
        /// <param name="path"></param>
        /// <returns></returns>
        internal static bool IsValidPath(string path)
        {
            char[] invalidPathChars = Path.GetInvalidPathChars();
            return path.IndexOfAny(invalidPathChars) < 0;
        }

        /// <summary>
        /// Serial Port로부터 한 줄을 읽는다. 다 읽을 때까지 블록된다.
        /// </summary>
        /// <param name="serialPort">Open된 Serial Port 디바이스.</param>
        /// <param name="useNewLine">개행 문자 사용 여부.</param>
        /// <returns></returns>
        internal static string ReadLine(SerialPort serialPort, bool useNewLine)
        {
            if (useNewLine)
            {
                return serialPort.ReadLine();
            }
            else
            {
                // New Line 으로 끝나지 않기 때문에 특별한 방법으로 읽는다.
                string received = "";
                int receivedChar = serialPort.ReadChar();
                received += (char)receivedChar;

                // 첫 글자를 읽은 다음부터는 Timeout 을 작게 설정해 계속 읽는다.
                serialPort.ReadTimeout = 10;

                try
                {
                    while (true)
                    {
                        receivedChar = serialPort.ReadChar();
                        received += (char)receivedChar;
                    }
                }
                catch (TimeoutException)
                {
                    // Do nothing.
                }

                return received;
            }
        }

        /// <summary>
        /// 파일의 CRC32 를 계산한다.
        /// </summary>
        /// <param name="filePath">파일 경로.</param>
        /// <returns>파일로부터 계산된 CRC32.</returns>
        internal static uint CalcCrc32(string filePath)
        {
            using (var stream = new FileStream(filePath, FileMode.Open, FileAccess.Read))
            {
                var buffer = new byte[4096];
                int readBytes;
                uint crc = 0;
                while ((readBytes = stream.Read(buffer, 0, buffer.Length)) > 0)
                {
                    crc = Crc32Algorithm.Append(crc, buffer, 0, readBytes);
                }
                return crc;
            }
        }

        /// <summary>
        /// 외부 실행파일 또는 문서를 실행하고 그 출력을 리턴한다.
        /// </summary>
        /// <param name="fileName">실행하려는 파일 경로.</param>
        /// <param name="arguments">명령행 인수.</param>
        /// <returns>명령의 표준 출력 문자열.</returns>
        internal static string ExecuteExternalCommand(string fileName, string arguments)
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.CreateNoWindow = true;
            info.UseShellExecute = false;
            info.FileName = fileName;
            info.Arguments = arguments;
            info.RedirectStandardOutput = true;

            using (Process exeProcess = Process.Start(info))
            {
                exeProcess.WaitForExit();
                return exeProcess.StandardOutput.ReadToEnd();
            }
        }

        /// <summary>
        /// 지정한 파일이 실행되고 있는가를 검사한다.
        /// </summary>
        /// <param name="filePath"></param>
        /// <returns></returns>
        internal static bool IsProgramRunning(string filePath)
        {
            string fullPath = Path.GetFullPath(filePath);
            string fileName = Path.GetFileNameWithoutExtension(fullPath);
            Process[] procList = Process.GetProcessesByName(fileName);
            foreach (Process proc in procList)
            {
                if (proc.MainModule.FileName.Equals(fullPath, StringComparison.OrdinalIgnoreCase))
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// 지정한 파일을 실행한다.
        /// </summary>
        /// <param name="fileName"></param>
        /// <param name="windowStyle"></param>
        internal static void StartProcess(string fileName, ProcessWindowStyle windowStyle = ProcessWindowStyle.Normal, string arguments = "")
        {
            ProcessStartInfo info = new ProcessStartInfo();
            info.CreateNoWindow = false;
            info.FileName = fileName;
            info.WindowStyle = windowStyle;
            info.UseShellExecute = true;
            info.Arguments = arguments;
            Process process = Process.Start(info);
            //if (process != null)
            //{
            //    process.WaitForInputIdle();
            //    ShowWindow(process.MainWindowHandle, SW_MINIMIZE);
            //}
        }

        /// <summary>
        /// 입력 문자열에서 모든 공백문자(개행문자 포함)를 제거한 문자열을 리턴한다.
        /// </summary>
        /// <param name="input">입력 문자열.</param>
        /// <returns>모든 공백문자가 제거된 문자열.</returns>
        internal static string RemoveAllWhiteSpaces(string input)
        {
            return Regex.Replace(input, "\\s+", "");
        }

        /// <summary>
        /// Assembly 제품 이름.
        /// </summary>
        internal static string AssemblyProduct
        {
            get
            {
                object[] attributes = Assembly.GetExecutingAssembly().GetCustomAttributes(typeof(AssemblyProductAttribute), false);
                if (attributes.Length == 0)
                {
                    return "";
                }
                return ((AssemblyProductAttribute)attributes[0]).Product;
            }
        }

        /// <summary>
        /// 문자열을 지정한 최대 길이로 자른 문자열을 리턴한다.
        /// </summary>
        /// <param name="value">자르려는 문자열.</param>
        /// <param name="maxLength">최대 길이.</param>
        /// <returns>문자열이 지정한 최대길이보다 짧으면 그대로, 길면 잘라서 리턴한다.</returns>
        internal static string Truncate(string value, int maxLength)
        {
            if (string.IsNullOrEmpty(value) || value.Length <= maxLength)
            {
                return value;
            }
            else
            {
                return value.Substring(0, maxLength);
            }
        }

        /// <summary>
        /// 리스트를 문자열로 변환한다.
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="source"></param>
        /// <param name="separator"></param>
        /// <returns></returns>
        internal static string ListToString<T>(List<T> source, string separator)
        {
            if (source == null || source.Count == 0)
            {
                return "";
            }

            if (separator == null)
            {
                separator = " ";
            }

            StringBuilder joinedText = new StringBuilder();
            for (int i = 0; i < source.Count; i++)
            {
                if (i > 0)
                {
                    joinedText.Append(separator);
                }
                joinedText.Append(source[i]);
            }
            return joinedText.ToString();
        }

        /// <summary>
        /// Sound Player에서 지원하는 오디오 파일을 오픈한다.
        /// </summary>
        /// <returns></returns>
        internal static string SelectWavFile()
        {
            var dialog = new OpenFileDialog();
            //dialog.Filter = "Audio Files(*.wav;*.snd;*.au;*.aif;*.aifc;*.aiff;*.wma;*.mp2;*.mp3;*.adt;*.adts;*.aac;) | *.wav;*.snd;*.au;*.aif;*.aifc;*.aiff;*.wma;*.mp2;*.mp3;*.adt;*.adts;*.aac; | All Files(*.*) | *.*";
            dialog.Filter = "WAV Files(*.wav)|*.wav;|All Files(*.*)|*.*;";
            dialog.RestoreDirectory = true;
            dialog.CheckFileExists = true;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.FileName;
            }
            else
            {
                return null;
            }
        }

        private static MethodInvoker invokeAction = null;

        /// <summary>
        /// 주어진 시간 후에 주어진 액션을 실행한다.
        /// </summary>
        /// <param name="delay"></param>
        /// <param name="action"></param>
        internal static void InvokeAfterDelay(int delay, MethodInvoker action)
        {
            var timer = new System.Timers.Timer();
            timer.Interval = delay;
            timer.AutoReset = false;
            timer.Elapsed += InvokeTimer_Elapsed;
            invokeAction = action;
            timer.Start();
        }

        private static void InvokeTimer_Elapsed(object sender, System.Timers.ElapsedEventArgs e)
        {
            invokeAction();
        }

        /// <summary>
        /// Windows의 Notepad 프로그램에 지정한 텍스트를 추가한다.
        /// </summary>
        /// <param name="message"></param>
        /// <param name="newLine"></param>
        internal static void AppendTextToNotepad(string message, bool newLine, Form activeForm)
        {
            Process notepad;
            var notepadProcs = Process.GetProcessesByName("notepad");
            bool newlyStarted = false;
            if (notepadProcs == null || notepadProcs.Length == 0)
            {
                notepad = Process.Start("notepad.exe");
                notepad?.WaitForInputIdle();
                newlyStarted = true;
            }
            else
            {
                notepad = notepadProcs[0];
            }

            if (notepad != null)
            {
                string appendingMessage = message + (newLine ? Environment.NewLine : "");
                Clipboard.SetText(appendingMessage);
                IntPtr editWindowHandle = FindWindowEx(notepad.MainWindowHandle, IntPtr.Zero, "Edit", null);
                int textLength = SendMessage(editWindowHandle, WM_GETTEXTLENGTH, 0, null);
                SendMessage(editWindowHandle, EM_SETSEL, (uint)textLength, textLength);
                SendMessage(editWindowHandle, WM_PASTE, 0, appendingMessage);
            }

            if (newlyStarted && activeForm != null)
            {
                SetForegroundWindow(activeForm.Handle);
            }
        }

        // 오류 메시지 대화상자 표시.
        internal static void ShowErrorDialog(Exception ex, string title = "Error")
        {
            StringBuilder errorMessage = new StringBuilder();
            string debugMessage = ex.ToString();
            while (ex != null)
            {
                errorMessage.AppendLine(ex.Message);
                ex = ex.InnerException;
            }

#if DEBUG
            errorMessage.AppendLine("====== Debug ======");
            errorMessage.AppendLine(debugMessage);
#endif

            InformationBox.Show(errorMessage.ToString(), title: title, buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
        }
    }
}
