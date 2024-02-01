using EOL_GND.Common;
using EOL_GND.ViewModel;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Diagnostics;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.View
{
    public partial class GeneralSettingsForm : Form
    {
        private bool isLoading = true;

        public GeneralSettingsForm()
        {
            InitializeComponent();

            SetSequenceFont(GeneralSettingsViewModel.SequenceFont);
            SetEditFont(GeneralSettingsViewModel.EditFont);
            sequenceEditorCheckBox.Checked = GeneralSettingsViewModel.SequenceEditorRestoreState;
            stepEditorCheckBox.Checked = GeneralSettingsViewModel.StepEditorRestoreState;

            logFilesCheckBox.Checked = GeneralSettingsViewModel.LogEnabled;
            logFolderTextBox.Text = GeneralSettingsViewModel.LogFolderPath;
            logFileNameTextBox.Text = GeneralSettingsViewModel.LogFileNameFormat;

            logTargetComboBox.DataSource = Enum.GetValues(typeof(Logger.LogTarget));
            logLevelComboBox.DataSource = Enum.GetValues(typeof(TraceLevel));
            appLogFolderTextBox.Text = GeneralSettingsViewModel.AppLogFolderPath;
            appLogFileNameTextBox.Text = GeneralSettingsViewModel.AppLogFileNameFormat;

            historyRemarksCheckBox.Checked = GeneralSettingsViewModel.AskToEnterHistoryRemarks;

            invokeAsyncCheckBox.Checked = GeneralSettingsViewModel.ControlInvokeAsync;
        }

        protected override void OnShown(EventArgs e)
        {
            base.OnShown(e);

            logTargetComboBox.SelectedItem = GeneralSettingsViewModel.AppLogTarget;
            logLevelComboBox.SelectedItem = GeneralSettingsViewModel.AppLogLevel;
            isLoading = false;

            // 사용자 권한 설정.
            var permission = GeneralSettingsViewModel.GetUserPermission();
            changeHistoryGroupBox.Enabled = permission?.CanEditSequence ?? false;
            expertGroupBox.Enabled = permission?.CanEditExpertSettings ?? false;
        }

        private void SetSequenceFont(Font font)
        {
            GeneralSettingsViewModel.SequenceFont = font;
            sequenceFontTextBox.Text = GeneralSettingsViewModel.ConvertToString(font);
            sequenceFontTextBox.Font = font;
        }

        private void SetEditFont(Font font)
        {
            GeneralSettingsViewModel.EditFont = font;
            editFontTextBox.Text = GeneralSettingsViewModel.ConvertToString(font);
            editFontTextBox.Font = font;
        }

        private void sequenceFontBrowseButton_Click(object sender, EventArgs e)
        {
            var font = SelectFont(GeneralSettingsViewModel.SequenceFont);
            if (font != null)
            {
                SetSequenceFont(font);
            }
        }

        private void editFontBrowseButton_Click(object sender, EventArgs e)
        {
            var font = SelectFont(GeneralSettingsViewModel.EditFont);
            if (font != null)
            {
                SetEditFont(font);
            }
        }

        private Font SelectFont(Font oldFont)
        {
            var dialog = new FontDialog();
            dialog.Font = oldFont;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                return dialog.Font;
            }
            else
            {
                return null;
            }
        }

        private void SequenceEditorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.SequenceEditorRestoreState = sequenceEditorCheckBox.Checked;
        }

        private void StepEditorCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.StepEditorRestoreState = stepEditorCheckBox.Checked;
        }

        private void logFolderTextBox_TextChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.LogFolderPath = logFolderTextBox.Text;
        }

        private void logFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = logFolderTextBox.Text;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    logFolderTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void logFileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.LogFileNameFormat = logFileNameTextBox.Text;
        }

        private void logFilesCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.LogEnabled = logFilesCheckBox.Checked;
        }

        private void logTargetComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                GeneralSettingsViewModel.AppLogTarget = (Logger.LogTarget)logTargetComboBox.SelectedItem;
            }
        }

        private void logLevelComboBox_SelectedIndexChanged(object sender, EventArgs e)
        {
            if (!isLoading)
            {
                GeneralSettingsViewModel.AppLogLevel = (TraceLevel)logLevelComboBox.SelectedItem;
            }
        }

        private void appLogFolderBrowseButton_Click(object sender, EventArgs e)
        {
            using (var dialog = new FolderBrowserDialog())
            {
                dialog.SelectedPath = appLogFolderTextBox.Text;
                dialog.ShowNewFolderButton = true;
                if (dialog.ShowDialog(this) == DialogResult.OK)
                {
                    appLogFolderTextBox.Text = dialog.SelectedPath;
                }
            }
        }

        private void appLogFolderTextBox_TextChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.AppLogFolderPath = appLogFolderTextBox.Text;
        }

        private void appLogFileNameTextBox_TextChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.AppLogFileNameFormat = appLogFileNameTextBox.Text;
        }

        private void historyRemarksCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.AskToEnterHistoryRemarks = historyRemarksCheckBox.Checked;
        }

        private void invokeAsyncCheckBox_CheckedChanged(object sender, EventArgs e)
        {
            GeneralSettingsViewModel.ControlInvokeAsync = invokeAsyncCheckBox.Checked;
        }

        private void SetSequenceFontFrom(string fontText)
        {
            try
            {
                var font = GeneralSettingsViewModel.ConvertFrom(fontText);
                if (font != null)
                {
                    SetSequenceFont(font);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void SetEditFontFrom(string fontText)
        {
            try
            {
                var font = GeneralSettingsViewModel.ConvertFrom(fontText);
                if (font != null)
                {
                    SetEditFont(font);
                }
            }
            catch (Exception ex)
            {
                Utils.ShowErrorDialog(ex);
            }
        }

        private void sequenceFontTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SetSequenceFontFrom(sequenceFontTextBox.Text);
            }
        }

        private void editFontTextBox_KeyUp(object sender, KeyEventArgs e)
        {
            if (e.KeyCode == Keys.Enter)
            {
                e.Handled = true;
                SetEditFontFrom(editFontTextBox.Text);
            }
        }
    }
}
