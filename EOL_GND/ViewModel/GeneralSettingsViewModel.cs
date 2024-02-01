using EOL_GND.Common;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Runtime.Serialization.Formatters;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class GeneralSettingsViewModel
    {
        internal static Font SequenceFont
        {
            get => AppSettings.SharedInstance.SequenceFont;
            set => AppSettings.SharedInstance.SequenceFont = value;
        }

        internal static Font EditFont
        {
            get => AppSettings.SharedInstance.EditFont;
            set => AppSettings.SharedInstance.EditFont = value;
        }

        internal static bool SequenceEditorRestoreState
        {
            get => AppSettings.SharedInstance.SequenceEditorRestoreState;
            set => AppSettings.SharedInstance.SequenceEditorRestoreState = value;
        }

        internal static FormWindowState SequenceEditorState
        {
            get => AppSettings.SharedInstance.SequenceEditorState;
            set => AppSettings.SharedInstance.SequenceEditorState = value;
        }

        internal static Point SequenceEditorLocation
        {
            get => AppSettings.SharedInstance.SequenceEditorLocation;
            set => AppSettings.SharedInstance.SequenceEditorLocation = value;
        }

        internal static Size SequenceEditorSize
        {
            get => AppSettings.SharedInstance.SequenceEditorSize;
            set => AppSettings.SharedInstance.SequenceEditorSize = value;
        }

        internal static bool StepEditorRestoreState
        {
            get => AppSettings.SharedInstance.StepEditorRestoreState;
            set => AppSettings.SharedInstance.StepEditorRestoreState = value;
        }

        internal static FormWindowState StepEditorState
        {
            get => AppSettings.SharedInstance.StepEditorState;
            set => AppSettings.SharedInstance.StepEditorState = value;
        }

        internal static Point StepEditorLocation
        {
            get => AppSettings.SharedInstance.StepEditorLocation;
            set => AppSettings.SharedInstance.StepEditorLocation = value;
        }

        internal static Size StepEditorSize
        {
            get => AppSettings.SharedInstance.StepEditorSize;
            set => AppSettings.SharedInstance.StepEditorSize = value;
        }

        internal static byte[] SequenceListViewState
        {
            get => AppSettings.SharedInstance.SequenceListViewState;
            set => AppSettings.SharedInstance.SequenceListViewState = value;
        }

        internal static byte[] TestResultListViewState
        {
            get => AppSettings.SharedInstance.TestResultListViewState;
            set => AppSettings.SharedInstance.TestResultListViewState = value;
        }

        internal static string LogFolderPath
        {
            get => AppSettings.SharedInstance.LogFolderPath;
            set => AppSettings.SharedInstance.LogFolderPath = value;
        }

        internal static string LogFileNameFormat
        {
            get => AppSettings.SharedInstance.LogFileNameFormat;
            set => AppSettings.SharedInstance.LogFileNameFormat = value;
        }

        internal static bool LogEnabled
        {
            get => AppSettings.SharedInstance.LogEnabled;
            set => AppSettings.SharedInstance.LogEnabled = value;
        }

        internal static string ConvertToString(Font font)
        {
            //return $"{font.Name}, {font.Size}pt";
            var converter = TypeDescriptor.GetConverter(typeof(Font));
            return converter?.ConvertToString(font);
        }

        internal static Font ConvertFrom(string text)
        {
            var converter = TypeDescriptor.GetConverter(typeof(Font));
            return converter?.ConvertFrom(text) as Font;
        }

        internal static Logger.LogTarget AppLogTarget
        {
            get => AppSettings.SharedInstance.AppLogTarget;
            set
            {
                AppSettings.SharedInstance.AppLogTarget = value;
                Logger.Target = value;
            }
        }

        internal static TraceLevel AppLogLevel
        {
            get => AppSettings.SharedInstance.AppLogLevel;
            set
            {
                AppSettings.SharedInstance.AppLogLevel = value;
                Logger.Switch.Level = value;
            }
        }

        internal static string AppLogFolderPath
        {
            get => AppSettings.SharedInstance.AppLogFolderPath;
            set
            {
                AppSettings.SharedInstance.AppLogFolderPath = value;
                Logger.LogFolder = value;
            }
        }

        internal static string AppLogFileNameFormat
        {
            get => AppSettings.SharedInstance.AppLogFileNameFormat;
            set
            {
                AppSettings.SharedInstance.AppLogFileNameFormat = value;
                Logger.LogFileNameFormat = value;
            }
        }

        internal static bool AskToEnterHistoryRemarks
        {
            get => AppSettings.SharedInstance.AskToEnterHistoryRemarks;
            set => AppSettings.SharedInstance.AskToEnterHistoryRemarks = value;
        }

        internal static bool ControlInvokeAsync
        {
            get => AppSettings.SharedInstance.ControlInvokeAsync;
            set => AppSettings.SharedInstance.ControlInvokeAsync = value;
        }

        internal static UserPermission GetUserPermission()
        {
            return AppSettings.GetCurrentUser()?.GetPermission();
        }

        internal static bool ImageViewerRestoreState
        {
            get => AppSettings.SharedInstance.ImageViewerRestoreState;
            set => AppSettings.SharedInstance.ImageViewerRestoreState = value;
        }

        internal static FormWindowState ImageViewerState
        {
            get => AppSettings.SharedInstance.ImageViewerState;
            set => AppSettings.SharedInstance.ImageViewerState = value;
        }

        internal static Point ImageViewerLocation
        {
            get => AppSettings.SharedInstance.ImageViewerLocation;
            set => AppSettings.SharedInstance.ImageViewerLocation = value;
        }

        internal static Size ImageViewerSize
        {
            get => AppSettings.SharedInstance.ImageViewerSize;
            set => AppSettings.SharedInstance.ImageViewerSize = value;
        }
    }
}
