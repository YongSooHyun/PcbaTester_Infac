using EOL_GND.Model;
using EOL_GND.View;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace EOL_GND.Common
{
    public class AppSettings
    {
        /// <summary>
        /// Config 파일이 저장되는 경로.
        /// </summary>
        public const string ConfigFileName = "D:\\ElozPlugin\\eol_settings.config";

        /// <summary>
        /// 로그파일 이름 생성에 이용되는 디폴트 포맷.
        /// </summary>
        internal const string DefaultLogFileNameFormat = "{EOLFileName}_{Date:yyyy-MM-dd_HH_mm_ss}";

        /// <summary>
        /// Shared instance.
        /// </summary>
        public static AppSettings SharedInstance
        {
            get
            {
                if (_sharedInstance == null)
                {
                    _sharedInstance = Load();
                }
                return _sharedInstance;
            }
        }
        private static AppSettings _sharedInstance = null;

        #region Settings

        /// <summary>
        /// 시퀀스 리스트 뷰 상태 저장을 위한 프로퍼티.
        /// </summary>
        public byte[] SequenceListViewState { get; set; }

        /// <summary>
        /// Step Editor 윈도우의 테스트 결과 리스트 뷰 상태 저장을 위한 프로퍼티.
        /// </summary>
        public byte[] TestResultListViewState { get; set; }

        /// <summary>
        /// SequenceForm 마지막 윈도우 상태, 위치, 크기를 복원할지 여부.
        /// </summary>
        public bool SequenceEditorRestoreState { get; set; } = true;

        /// <summary>
        /// SequenceForm 마지막 윈도우 상태.
        /// </summary>
        public FormWindowState SequenceEditorState { get; set; } = FormWindowState.Normal;

        /// <summary>
        /// SequenceForm 마지막 윈도우 위치.
        /// </summary>
        public Point SequenceEditorLocation { get; set; }

        /// <summary>
        /// SequenceForm 마지막 윈도우 크기.
        /// </summary>
        public Size SequenceEditorSize { get; set; } = Size.Empty;

        /// <summary>
        /// SequenceForm 마지막 윈도우 상태, 위치, 크기를 복원할지 여부.
        /// </summary>
        public bool StepEditorRestoreState { get; set; } = true;

        /// <summary>
        /// SequenceForm 마지막 윈도우 상태.
        /// </summary>
        public FormWindowState StepEditorState { get; set; } = FormWindowState.Normal;

        /// <summary>
        /// SequenceForm 마지막 윈도우 위치.
        /// </summary>
        public Point StepEditorLocation { get; set; }

        /// <summary>
        /// SequenceForm 마지막 윈도우 크기.
        /// </summary>
        public Size StepEditorSize { get; set; } = Size.Empty;

        /// <summary>
        /// 시퀀스를 보여주는 리스트의 폰트.
        /// </summary>
        [XmlIgnore]
        public Font SequenceFont
        {
            get
            {
                var converter = TypeDescriptor.GetConverter(typeof(Font));
                var font = converter.ConvertFromString(SequenceFontString ?? "");
                return font as Font ?? new Font("Segoe UI", 11);
            }
            set
            {
                var converter = TypeDescriptor.GetConverter(typeof(Font));
                SequenceFontString = converter.ConvertTo(value, typeof(string)) as string;

                // 이미 열려있는 화면이 있으면 업데이트.
                foreach (var form in Application.OpenForms)
                {
                    if (form is SequenceForm seqForm)
                    {
                        seqForm.UpdateFont();
                    }
                }
            }
        }
        public string SequenceFontString { get; set; }

        /// <summary>
        /// 스텝을 편집하는 Property Grid의 폰트.
        /// </summary>
        [XmlIgnore]
        public Font EditFont
        {
            get
            {
                var converter = TypeDescriptor.GetConverter(typeof(Font));
                var font = converter.ConvertFromString(EditFontString ?? "");
                return font as Font ?? new Font("Courier New", 9);
            }
            set
            {
                var converter = TypeDescriptor.GetConverter(typeof(Font));
                EditFontString = converter.ConvertTo(value, typeof(string)) as string;

                // 이미 열려있는 화면이 있으면 업데이트.
                foreach (var form in Application.OpenForms)
                {
                    if (form is StepEditForm editForm)
                    {
                        editForm.UpdateFont();
                    }
                }
            }
        }
        public string EditFontString { get; set; }

        /// <summary>
        /// 로그파일이 저장될 폴더 경로.
        /// </summary>
        public string LogFolderPath { get; set; } = "D:\\ElozPlugin\\EOL\\Log";

        /// <summary>
        /// 로그파일 이름 포맷.
        /// </summary>
        public string LogFileNameFormat { get; set; } = DefaultLogFileNameFormat;

        /// <summary>
        /// 테스트가 끝난 다음 로그파일에 실행결과를 저장할지 여부.
        /// </summary>
        public bool LogEnabled { get; set; } = false;

        /// <summary>
        /// License Key.
        /// </summary>
        public string LicenseKey { get; set; }

        /// <summary>
        /// Enabled된 스텝들만 보여줄 것인지 여부.
        /// </summary>
        public bool ShowEnabledStepsOnly { get; set; } = true;

        /// <summary>
        /// 사양서 로그에 보여지는 스텝들만 보여줄 것인지 여부.
        /// </summary>
        public bool ShowSpecStepsOnly { get; set; } = true;

        /// <summary>
        /// App 로그를 기록할 위치.
        /// </summary>
        public Logger.LogTarget AppLogTarget { get; set; } = Logger.LogTarget.Window;

        /// <summary>
        /// App 로그 기록 레벨.
        /// </summary>
        public TraceLevel AppLogLevel { get; set; } = TraceLevel.Verbose;

        /// <summary>
        /// 로그파일이 저장될 폴더 경로.
        /// </summary>
        public string AppLogFolderPath { get; set; } = "D:\\ElozPlugin\\AppLogs";

        /// <summary>
        /// 로그파일 이름 포맷.
        /// </summary>
        public string AppLogFileNameFormat { get; set; } = "eol_gnd_log_{Date:yyyy-MM-dd}.txt";

        /// <summary>
        /// 시퀀스를 저장할 때 사용자가 추가 설명을 입력하도록 하는 대화상자를 보여줄 것인지 여부.
        /// </summary>
        public bool AskToEnterHistoryRemarks { get; set; } = true;

        /// <summary>
        /// UI Control Invoke 호출 Async 여부.
        /// </summary>
        public bool ControlInvokeAsync { get; set; } = true;

        /// <summary>
        /// ImageViewer 마지막 윈도우 상태, 위치, 크기를 복원할지 여부.
        /// </summary>
        public bool ImageViewerRestoreState { get; set; } = true;

        /// <summary>
        /// ImageViewer 마지막 윈도우 상태.
        /// </summary>
        public FormWindowState ImageViewerState { get; set; } = FormWindowState.Normal;

        /// <summary>
        /// ImageViewer 마지막 윈도우 위치.
        /// </summary>
        public Point ImageViewerLocation { get; set; }

        /// <summary>
        /// ImageViewer 마지막 윈도우 크기.
        /// </summary>
        public Size ImageViewerSize { get; set; } = Size.Empty;

        #endregion // Settings

        #region Constructors

        public AppSettings()
        {
        }

        #endregion

        #region Load & Save

        /// <summary>
        /// Config 파일을 로딩하여 클래스 인스턴스를 만든다.
        /// </summary>
        /// <returns></returns>
        private static AppSettings Load()
        {
            FileStream stream = null;
            try
            {
                stream = new FileStream(ConfigFileName, FileMode.Open);
                var xmlSerializer = new XmlSerializer(typeof(AppSettings));
                var obj = xmlSerializer.Deserialize(stream) as AppSettings;
                return obj;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Cannot load app settings: {ex.Message}");

                var obj = new AppSettings();
                return obj;
            }
            finally
            {
                stream?.Close();
            }
        }

        /// <summary>
        /// 설정을 저장한다.
        /// </summary>
        public void Save()
        {
            // 파일을 저장할 폴더가 없으면 만든다.
            Directory.CreateDirectory(Path.GetDirectoryName(ConfigFileName));
            using (var writer = new StreamWriter(ConfigFileName))
            {
                var xmlSerializer = new XmlSerializer(GetType());
                xmlSerializer.Serialize(writer, this);
            }
        }

        #endregion

        /// <summary>
        /// DLL 버전이 바뀌면 elowerk TestBuilder 스크립트 에러가 발생하므로, 내부적으로 버전을 정의해 이용한다.
        /// </summary>
        /// <returns></returns>
        public static Version GetVersion()
        {
            return new Version(2, 3, 0);
        }

        /// <summary>
        /// 현재 로그인한 사용자를 리턴한다.
        /// </summary>
        /// <returns></returns>
        internal static User GetCurrentUser()
        {
            var openForms = Application.OpenForms;
            for (int i = 0; i < openForms.Count; i++)
            {
                if (openForms[i] is SequenceForm sequenceForm)
                {
                    return sequenceForm.ViewModel.CurrentUser;
                }
            }

            return null;
        }
    }
}
