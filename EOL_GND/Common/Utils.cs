using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO.Ports;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.Common
{
    /// <summary>
    /// 유용한 함수들 제공.
    /// </summary>
    public static class Utils
    {
        /// <summary>
        /// 정규분포에 따른 난수 발생을 위한 변수.
        /// </summary>
        private static readonly Dictionary<int, Random> randoms = new Dictionary<int, Random>();

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

        /// <summary>
        /// 지정한 시간을 형식에 맞춰 문자열로 변환해 리턴한다.
        /// </summary>
        /// <param name="time">문자열로 변환하려는 시간.</param>
        /// <param name="includeMillisecond">밀리초를 표시할 것인지 여부.</param>
        /// <returns>"hh:mm:ss.mmm"형식의 시간 문자열.</returns>
        internal static string CreateTimeString(DateTime time, bool includeMillisecond = true)
        {
            return $"{time.Hour:D2}:{time.Minute:D2}:{time.Second:D2}" + (includeMillisecond ? $".{time.Millisecond:D3}" : "");
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
        /// 지정한 프로퍼티의 <see cref="BrowsableAttribute"/>를 설정한다.
        /// </summary>
        /// <param name="obj">속성을 변경할 프로퍼티가 정의된 오브젝트.</param>
        /// <param name="propertyName">속성을 변경할 프로퍼티 이름.</param>
        /// <param name="browsable">Browsable 속성 값.</param>
        internal static void SetBrowsableAttribute(object obj, string propertyName, bool browsable)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(obj)[propertyName];
            BrowsableAttribute browsableAttribute = propertyDescriptor?.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;

            // TODO: BrowsableAttribute의 browsable필드 이름이 바뀌면 작동하지 않으므로, .NET 버전 업데이트 시 확인이 필요하다.
            FieldInfo fieldInfo = browsableAttribute?.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(browsableAttribute, browsable);
        }

        /// <summary>
        /// 지정한 프로퍼티의 <see cref="BrowsableAttribute"/>를 얻는다.
        /// </summary>
        /// <param name="obj"></param>
        /// <param name="propertyName"></param>
        /// <returns></returns>
        internal static bool? GetBrowsableAttribute(object obj, string propertyName)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(obj)[propertyName];
            BrowsableAttribute browsableAttribute = propertyDescriptor?.Attributes[typeof(BrowsableAttribute)] as BrowsableAttribute;

            // TODO: BrowsableAttribute의 browsable필드 이름이 바뀌면 작동하지 않으므로, .NET 버전 업데이트 시 확인이 필요하다.
            FieldInfo fieldInfo = browsableAttribute?.GetType().GetField("browsable", BindingFlags.NonPublic | BindingFlags.Instance);
            return fieldInfo?.GetValue(browsableAttribute) as bool?;
        }

        /// <summary>
        /// 지정한 프로퍼티의 <see cref="DisplayNameAttribute"/>를 설정한다.
        /// </summary>
        /// <param name="obj">속성을 변경할 프로퍼티가 정의된 오브젝트.</param>
        /// <param name="propertyName">속성을 변경할 프로퍼티 이름.</param>
        /// <param name="displayName">DisplayName 속성 값.</param>
        internal static void SetDisplayNameAttribute(object obj, string propertyName, string displayName)
        {
            PropertyDescriptor propertyDescriptor = TypeDescriptor.GetProperties(obj)[propertyName];
            DisplayNameAttribute displayNameAttribute = propertyDescriptor?.Attributes[typeof(DisplayNameAttribute)] as DisplayNameAttribute;

            // TODO: DisplayNameAttribute의 _displayName필드 이름이 바뀌면 작동하지 않으므로, .NET 버전 업데이트 시 확인이 필요하다.
            FieldInfo fieldInfo = displayNameAttribute?.GetType().GetField("_displayName", BindingFlags.NonPublic | BindingFlags.Instance);
            fieldInfo?.SetValue(displayNameAttribute, displayName);
        }

        /// <summary>
        /// 메인 스레드가 아닌 다른 스레드에서 UI를 업데이트 할 때 메인 스레드에서 코드가 실행되도록 한다.
        /// </summary>
        /// <typeparam name="T"><see cref="Control"/>을 상속하는 임의의 클래스.</typeparam>
        /// <param name="obj">UI 인스턴스. 이 인스턴스를 소유한 스레드에서 <paramref name="action"/>이 실행된다.</param>
        /// <param name="action">실행할 코드를 담은 <see cref="MethodInvoker"/> 델리게이트.</param>
        internal static void InvokeIfRequired<T>(T obj, MethodInvoker action) where T : Control
        {
            if (obj.InvokeRequired)
            {
                try
                {
                    if (AppSettings.SharedInstance.ControlInvokeAsync)
                    {
                        obj.BeginInvoke(action);
                    }
                    else
                    {
                        obj.Invoke(action);
                    }
                }
                catch (InvalidOperationException)
                {}
            }
            else
            {
                action();
            }
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
        /// Form의 상태, 위치, 크기를 지정한대로 설정.
        /// </summary>
        /// <param name="window"></param>
        /// <param name="state"></param>
        /// <param name="location"></param>
        /// <param name="size"></param>
        internal static void SetWindowState(Form window, FormWindowState state, Point location, Size size)
        {
            if (size.Width > 0 && size.Height > 0)
            {
                switch (state)
                {
                    case FormWindowState.Normal:
                        window.Location = location;
                        window.Size = size;
                        break;
                    case FormWindowState.Maximized:
                        window.WindowState = FormWindowState.Maximized;
                        break;
                }
            }
        }

        /// <summary>
        /// 정규분포에 따르는 난수 발생.
        /// </summary>
        /// <param name="mean"></param>
        /// <param name="stdev"></param>
        /// <returns></returns>
        internal static double NextGaussian(int stepId, double mean, double stdev)
        {
            Random rand;
            if (randoms.ContainsKey(stepId))
            {
                rand = randoms[stepId];
            }
            else
            {
                rand = new Random();
                randoms.Add(stepId, rand);
            }

            double u1 = 1 - rand.NextDouble();
            double u2 = 1 - rand.NextDouble();
            double randStdNormal = Math.Sqrt(-2 * Math.Log(u1)) * Math.Sin(2 * Math.PI * u2);
            double randNormal = mean + stdev * randStdNormal;
            return randNormal;
        }

        /// <summary>
        /// 지정한 개수의 유효숫자만 가진 수 리턴.
        /// </summary>
        /// <param name="value"></param>
        /// <param name="digits"></param>
        /// <returns></returns>
        internal static double RoundToSignificantDigits(double value, int digits)
        {
            if (value == 0)
            {
                return value;
            }

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(value))) + 1);
            return scale * Math.Round(value / scale, digits);
        }

        /// <summary>
        /// 지정한 해상도에 따른 소수부 자르기.
        /// </summary>
        /// <param name="resolution"></param>
        /// <returns></returns>
        public static double ResolutionTruncate(double value, double resolution)
        {
            if (resolution == 0)
            {
                return value;
            }

            double scale = Math.Pow(10, Math.Floor(Math.Log10(Math.Abs(resolution))));
            return scale * Math.Round(value / scale, 0);
        }
    }
}
