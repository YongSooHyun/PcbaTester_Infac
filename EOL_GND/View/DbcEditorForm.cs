using BrightIdeasSoftware;
using EOL_GND.Common;
using EOL_GND.Device;
using EOL_GND.Model.DBC;
using EOL_GND.ViewModel;
using InfoBox;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using static System.Windows.Forms.VisualStyles.VisualStyleElement.Window;

namespace EOL_GND.View
{
    public partial class DbcEditorForm : Form
    {
        /// <summary>
        /// 메시지/시그널 추가, 삭제, 편집이 가능한지 여부.
        /// </summary>
        internal bool EditingEnabled { get; set; } = true;

        /// <summary>
        /// 메시지 또는 시그널 선택 여부.
        /// </summary>
        internal bool SelectingMessages { get; set; } = true;

        /// <summary>
        /// 처음 표시할 때 선택할 메시지 이름.
        /// </summary>
        internal string MessageName { get; set; }

        /// <summary>
        /// 처음 표시할 때 선택할 시그널 이름.
        /// </summary>
        internal string SignalName { get; set; }

        /// <summary>
        /// 사용자가 선택한 CAN 메시지.
        /// </summary>
        internal DbcMessage SelectedMessage { get; private set; } = null;

        /// <summary>
        /// 사용자가 선택한 CAN 시그널.
        /// </summary>
        internal DbcSignal SelectedSignal { get; private set; } = null;

        private readonly DbcEditorViewModel viewModel = new DbcEditorViewModel();

        public DbcEditorForm()
        {
            InitializeComponent();

            messageListView.CellEditUseWholeCell = true;
            dlcColumn.AspectToStringConverter = delegate (object o)
            {
                return ((CanDLC)o).GetText();
            };
            signalListView.CellEditUseWholeCell = true;
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            UpdateListViewRowHeight(messageListView);
            UpdateListViewRowHeight(signalListView);
            SetEditingEnabled(EditingEnabled);
            UpdateMessageDeleteButton();
            UpdateMessageCopyButton();
            UpdateMessagePasteButton();
            UpdateSignalAddButton();
            UpdateSignalDeleteButton();
            UpdateSignalCopyButton();
            UpdateSignalPasteButton();
            UpdateOkButton();

            // 데이터 표시.
            messageListView.SetObjects(viewModel.GetDbcMessages());

            // 선택된 메시지.
            if (!string.IsNullOrEmpty(MessageName))
            {
                var message = DbcManager.SharedInstance.FindMessageByName(MessageName, null);
                if (message != null)
                {
                    messageListView.SelectObject(message);
                    messageListView.Focus();
                }
            }
        }

        protected override async void OnShown(EventArgs e)
        {
            base.OnShown(e);

            // 선택된 시그널.
            if (!string.IsNullOrEmpty(SignalName))
            {
                var signal = DbcManager.SharedInstance.FindSignalByName(SignalName, null, out DbcMessage parent);
                if (signal != null)
                {
                    messageListView.SelectObject(parent);
                    await Task.Delay(200);
                    signalListView.SelectObject(signal);
                    signalListView.Focus();
                }
            }

            // 사용자 권한 설정.
            var permission = GeneralSettingsViewModel.GetUserPermission();
            var editAllowed = permission?.CanEditCanMessageSettings ?? false;
            messageToolStrip.Enabled = editAllowed;
            messageListView.CellEditActivation = editAllowed ? ObjectListView.CellEditActivateMode.DoubleClick : ObjectListView.CellEditActivateMode.None;
            messageContextMenu.Enabled = editAllowed;
            signalToolStrip.Enabled = editAllowed;
            signalListView.CellEditActivation = editAllowed ? ObjectListView.CellEditActivateMode.DoubleClick : ObjectListView.CellEditActivateMode.None;
            signalContextMenu.Enabled = editAllowed;
            byteOrderColumn.IsEditable = editAllowed;
            signedColumn.IsEditable = editAllowed;
        }

        private void UpdateListViewRowHeight(ObjectListView listView)
        {
            // ComboBox 높이에 맞게 Row Height를 조정한다.
            var dummyComboBox = new ComboBox();
            dummyComboBox.Font = listView.Font;
            Controls.Add(dummyComboBox);
            listView.RowHeight = dummyComboBox.Height;
            Controls.Remove(dummyComboBox);
        }

        private void SetEditingEnabled(bool editingEnabled)
        {
            if (editingEnabled)
            {
                messageSaveButton.Visible = true;
                messageSeparator1.Visible = true;
                messageAddButton.Visible = true;
                messageDeleteButton.Visible = true;
                messageSeparator2.Visible = true;
                messageCopyButton.Visible = true;
                messagePasteButton.Visible = true;
                signalAddButton.Visible = true;
                signalDeleteButton.Visible = true;
                signalSeparator1.Visible = true;
                signalCopyButton.Visible = true;
                signalPasteButton.Visible = true;
                buttonPanel.Visible = false;

                messageListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
                signalListView.CellEditActivation = ObjectListView.CellEditActivateMode.DoubleClick;
                byteOrderColumn.IsEditable = true;
                signedColumn.IsEditable = true;

                mainSplitContainer.Panel2Collapsed = false;
            }
            else
            {
                messageSaveButton.Visible = false;
                messageSeparator1.Visible = false;
                messageAddButton.Visible = false;
                messageDeleteButton.Visible = false;
                messageSeparator2.Visible = false;
                messageCopyButton.Visible = false;
                messagePasteButton.Visible = false;
                signalAddButton.Visible = false;
                signalDeleteButton.Visible = false;
                signalSeparator1.Visible = false;
                signalCopyButton.Visible = false;
                signalPasteButton.Visible = false;
                buttonPanel.Visible = true;

                messageListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
                signalListView.CellEditActivation = ObjectListView.CellEditActivateMode.None;
                byteOrderColumn.IsEditable = false;
                signedColumn.IsEditable = false;

                mainSplitContainer.Panel2Collapsed = SelectingMessages;
            }
        }

        private void messageSaveButton_Click(object sender, EventArgs e)
        {
            Save();
        }

        private void Save()
        {
            try
            {
                messageListView.CancelCellEdit();
                signalListView.CancelCellEdit();
                viewModel.Save();
            }
            catch (Exception ex)
            {
                Logger.LogError($"CAN messages saving error: {ex.Message}");
            }
        }

        private void messageAddButton_Click(object sender, EventArgs e)
        {
            AddMessage();
        }

        private void AddMessage()
        {
            try
            {
                messageListView.CancelCellEdit();
                var newMessage = viewModel.CreateMessage();
                messageListView.AddObject(newMessage);
                messageListView.SelectObject(newMessage);
            }
            catch (Exception ex)
            {
                Logger.LogError($"CAN message creating error: {ex.Message}");
            }
        }

        private void messageDeleteButton_Click(object sender, EventArgs e)
        {
            DeleteMessages();
        }

        private void DeleteMessages()
        {
            try
            {
                var modelObjects = messageListView.SelectedObjects;
                if (modelObjects.Count == 0)
                {
                    return;
                }

                // 한번 더 물어본다.
                var answer = InformationBox.Show($"선택된 {modelObjects.Count}개의 메시지를 삭제하시겠습니까?",
                    "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
                if (answer != InformationBoxResult.OK)
                {
                    return;
                }

                messageListView.CancelCellEdit();
                viewModel.DeleteMessages(modelObjects);
                messageListView.RemoveObjects(modelObjects);
            }
            catch (Exception ex)
            {
                Logger.LogError($"CAN messages deleting error: {ex.Message}");
            }
        }

        private void messageCopyButton_Click(object sender, EventArgs e)
        {
            CopyMessages();
        }

        private void CopyMessages()
        {
            var modelObjects = messageListView.SelectedObjects;
            if (modelObjects.Count > 0)
            {
                viewModel.CopyMessages(modelObjects);
                UpdateMessagePasteButton();
            }
        }

        private void messagePasteButton_Click(object sender, EventArgs e)
        {
            PasteMessages();
        }

        private void PasteMessages()
        {
            var pastedObjects = viewModel.PasteMessages();
            if (pastedObjects?.Count > 0)
            {
                messageListView.AddObjects(pastedObjects);
                messageListView.SelectObjects(pastedObjects);
            }
        }

        private void messageListView_SelectionChanged(object sender, EventArgs e)
        {
            // 선택된 메시지 업데이트.
            SelectedMessage = messageListView.SelectedObject as DbcMessage;

            UpdateMessageDeleteButton();
            UpdateMessageCopyButton();
            UpdateMessagePasteButton();
            UpdateSignals();
            UpdateSignalAddButton();
            UpdateSignalDeleteButton();
            UpdateSignalCopyButton();
            UpdateSignalPasteButton();
            UpdateOkButton();
        }

        private void UpdateMessageDeleteButton()
        {
            messageDeleteButton.Enabled = messageListView.SelectedObjects.Count > 0;
        }

        private void UpdateMessageCopyButton()
        {
            messageCopyButton.Enabled = messageListView.SelectedObjects.Count > 0;
        }

        private void UpdateMessagePasteButton()
        {
            messagePasteButton.Enabled = viewModel.MessagePasteEnabled;
        }

        private void messageListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void messageListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            messageContextMenu.Enabled = false;
            viewModel.ConfigureMessageEditingControl(e.Column.AspectName, e.Control);
        }

        private void messageListView_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (viewModel.GetNewMessageValue(e.RowObject, e.Column.AspectName, e.NewValue, out object calcValue))
            {
                e.NewValue = calcValue;
            }
            else
            {
                e.NewValue = e.Value;
            }
            messageContextMenu.Enabled = true;
        }

        #region Message Context Menu

        private void messageContextMenu_Opening(object sender, CancelEventArgs e)
        {
            deleteMessagesToolStripMenuItem.Enabled = messageListView.SelectedObjects.Count > 0;
            copyMessagesToolStripMenuItem.Enabled = messageListView.SelectedObjects.Count > 0;
            pasteMessagesToolStripMenuItem.Enabled = viewModel.MessagePasteEnabled;
        }

        private void addMessageToolStripMenuItem_Click(object sender, EventArgs e)
        {
            AddMessage();
        }

        private void deleteMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            DeleteMessages();
        }

        private void copyMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            CopyMessages();
        }

        private void pasteMessagesToolStripMenuItem_Click(object sender, EventArgs e)
        {
            PasteMessages();
        }

        #endregion // Message Context Menu

        #region Signal UI

        private void UpdateSignals()
        {
            var messageObject = messageListView.SelectedObject;
            if (messageObject == null)
            {
                signalListView.ClearObjects();
                UpdateSignalDeleteButton();
                return;
            }

            var signals = viewModel.GetDbcSignals(messageObject);
            signalListView.SetObjects(signals);
        }

        private void UpdateSignalAddButton()
        {
            signalAddButton.Enabled = messageListView.SelectedObject != null;
        }

        private void UpdateSignalDeleteButton()
        {
            signalDeleteButton.Enabled = signalListView.SelectedObjects.Count > 0;
        }

        private void UpdateSignalCopyButton()
        {
            signalCopyButton.Enabled = signalListView.SelectedObjects.Count > 0;
        }

        private void UpdateSignalPasteButton()
        {
            signalPasteButton.Enabled = messageListView.SelectedObject != null && viewModel.SignalPasteEnabled;
        }

        private void signalListView_CellEditStarting(object sender, CellEditEventArgs e)
        {
            signalContextMenu.Enabled = false;
            viewModel.ConfigureSignalEditingControl(e.Column.AspectName, e.Control);

            // ErrorID 는 Nullable 타입이므로, TextBox로 편집한다.
            var control = viewModel.GetSignalEditingControl(e.Column.AspectName, e.Value);
            if (control != null)
            {
                e.Control = control;
                control.Bounds = e.CellBounds;
            }
        }

        private void signalListView_CellEditFinishing(object sender, CellEditEventArgs e)
        {
            if (viewModel.GetNewSignalValue(e.RowObject, e.Column.AspectName, e.NewValue, out object calcValue))
            {
                e.NewValue = calcValue;
            }
            else
            {
                e.NewValue = e.Value;
            }
            signalContextMenu.Enabled = true;
        }

        private void signalListView_SelectionChanged(object sender, EventArgs e)
        {
            // 선택된 시그널 설정.
            SelectedSignal = signalListView.SelectedObject as DbcSignal;

            UpdateSignalDeleteButton();
            UpdateSignalCopyButton();
            UpdateSignalPasteButton();
            UpdateOkButton();
        }

        private void signalListView_FormatRow(object sender, FormatRowEventArgs e)
        {
            e.Item.Text = (e.RowIndex + 1).ToString();
        }

        private void signalAddButton_Click(object sender, EventArgs e)
        {
            AddSignal();
        }

        private void AddSignal()
        {
            if (messageListView.SelectedObject != null)
            {
                signalListView.CancelCellEdit();
                var signal = viewModel.CreateSignal(messageListView.SelectedObject);
                signalListView.AddObject(signal);
                signalListView.SelectObject(signal);
            }
        }

        private void signalDeleteButton_Click(object sender, EventArgs e)
        {
            DeleteSignals();
        }

        private void DeleteSignals()
        {
            var messageObject = messageListView.SelectedObject;
            if (messageObject == null)
            {
                return;
            }

            var modelObjects = signalListView.SelectedObjects;
            if (modelObjects.Count == 0)
            {
                return;
            }

            // 한번 더 물어본다.
            var answer = InformationBox.Show($"선택된 {modelObjects.Count}개의 시그널을 삭제하시겠습니까?",
                "삭제 확인", buttons: InformationBoxButtons.OKCancel, icon: InformationBoxIcon.Warning);
            if (answer != InformationBoxResult.OK)
            {
                return;
            }

            signalListView.CancelCellEdit();
            viewModel.DeleteSignals(messageObject, modelObjects);
            signalListView.RemoveObjects(modelObjects);
        }

        private void signalCopyButton_Click(object sender, EventArgs e)
        {
            CopySignals();
        }

        private void CopySignals()
        {
            var modelObjects = signalListView.SelectedObjects;
            if (modelObjects.Count > 0)
            {
                viewModel.CopySignals(modelObjects);
                UpdateSignalPasteButton();
            }
        }

        private void signalPasteButton_Click(object sender, EventArgs e)
        {
            PasteSignals();
        }

        private void PasteSignals()
        {
            var messageObject = messageListView.SelectedObject;
            if (messageObject == null)
            {
                return;
            }

            var pastedSignals = viewModel.PasteSignals(messageObject);
            if (pastedSignals?.Count > 0)
            {
                signalListView.AddObjects(pastedSignals);
                signalListView.SelectObjects(pastedSignals);
            }
        }

        #endregion // Signal UI

        #region Signal Selection

        private void UpdateOkButton()
        {
            if (SelectingMessages)
            {
                okButton.Enabled = messageListView.SelectedObject != null;
            }
            else
            {
                okButton.Enabled = signalListView.SelectedObject != null;
            }
        }

        #endregion // Signal Selection

        #region Signal Context Menu

        private void signalContextMenu_Opening(object sender, CancelEventArgs e)
        {
            addSignalItem.Enabled = messageListView.SelectedObject != null;
            deleteSignalsItem.Enabled = signalListView.SelectedObjects.Count > 0;
            copySignalsItem.Enabled = signalListView.SelectedObjects.Count > 0;
            pasteSignalsItem.Enabled = viewModel.SignalPasteEnabled;
        }

        private void addSignalItem_Click(object sender, EventArgs e)
        {
            AddSignal();
        }

        private void deleteSignalsItem_Click(object sender, EventArgs e)
        {
            DeleteSignals();
        }

        private void copySignalsItem_Click(object sender, EventArgs e)
        {
            CopySignals();
        }

        private void pasteSignalsItem_Click(object sender, EventArgs e)
        {
            PasteSignals();
        }

        #endregion // Signal Context Menu

        // Shortcut Key들을 정의한다.
        protected override bool ProcessCmdKey(ref Message msg, Keys keyData)
        {
            switch (keyData)
            {
                case Keys.Control | Keys.S:
                    messageSaveButton.PerformClick();
                    return true;
            }

            return base.ProcessCmdKey(ref msg, keyData);
        }

        private void messageListView_ItemActivate(object sender, EventArgs e)
        {
            if (!EditingEnabled && SelectingMessages)
            {
                DialogResult = DialogResult.OK;
            }
        }

        private void signalListView_ItemActivate(object sender, EventArgs e)
        {
            if (!EditingEnabled && !SelectingMessages)
            {
                DialogResult = DialogResult.OK;
            }
        }
    }
}
