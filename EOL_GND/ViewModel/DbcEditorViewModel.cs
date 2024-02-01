using EOL_GND.Device;
using EOL_GND.Model;
using EOL_GND.Model.DBC;
using InfoBox;
using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class DbcEditorViewModel
    {
        internal bool MessagePasteEnabled => copiedMessages.Count > 0;
        internal bool SignalPasteEnabled => copiedSignals.Count > 0;

        private readonly List<DbcMessage> copiedMessages = new List<DbcMessage>();
        private readonly List<DbcSignal> copiedSignals = new List<DbcSignal>();

        internal List<DbcMessage> GetDbcMessages()
        {
            return DbcManager.SharedInstance.Messages;
        }

        internal void Save()
        {
            DbcManager.SharedInstance.Save();
        }

        internal DbcMessage CreateMessage()
        {
            var newName = DbcManager.SharedInstance.GetNewMessageName("NewMessage");
            var newMessage = new DbcMessage { Name = newName };
            DbcManager.SharedInstance.Messages.Add(newMessage);
            return newMessage;
        }

        internal void DeleteMessages(IList messageObjects)
        {
            var messages = DbcManager.SharedInstance.Messages;
            foreach (var messageObject in messageObjects)
            {
                messages.Remove(messageObject as DbcMessage);
            }
        }

        internal void CopyMessages(IList messageObjects)
        {
            copiedMessages.Clear();
            foreach (var messageObject in messageObjects)
            {
                if (messageObject is DbcMessage message)
                {
                    copiedMessages.Add(message);
                }
            }
        }

        internal List<DbcMessage> PasteMessages()
        {
            var pastedMessages = new List<DbcMessage>();
            foreach (var message in copiedMessages)
            {
                var clonedMessage = message.Clone() as DbcMessage;
                pastedMessages.Add(clonedMessage);
            }
            DbcManager.SharedInstance.Messages.AddRange(pastedMessages);
            return pastedMessages;
        }

        internal void ConfigureMessageEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(DbcMessage.ID):
                    if (editControl is NumericUpDown idNUDown)
                    {
                        idNUDown.Hexadecimal = true;
                    }
                    break;
                case nameof(DbcMessage.DLC):
                    if (editControl is ComboBox dlcComboBox)
                    {
                        dlcComboBox.FormattingEnabled = true;
                        dlcComboBox.Format += delegate (object sender, ListControlConvertEventArgs e)
                        {
                            e.Value = ((CanDLC)e.Value).GetText();
                        };
                    }
                    break;
            }

            if (editControl is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (editControl is NumericUpDown nuDown)
            {
                nuDown.Select(0, nuDown.Text.Length);
            }
        }

        internal bool GetNewMessageValue(object rowObject, string propertyName, object value, out object newValue)
        {
            newValue = value;
            switch (propertyName)
            {
                case nameof(DbcMessage.Name):
                    // 이름이 중복되는가 검사한다.
                    var newName = value as string;
                    var message = DbcManager.SharedInstance.FindMessageByName(newName, rowObject);
                    if (message != null)
                    {
                        InformationBox.Show($"지정한 이름({newName})을 가진 CAN 메시지가 이미 존재합니다.",
                            "Validation Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                        return false;
                    }
                    break;
            }
            return true;
        }

        internal List<DbcSignal> GetDbcSignals(object dbcMessage)
        {
            if (dbcMessage is DbcMessage message)
            {
                return message.Signals;
            }

            return null;
        }

        internal DbcSignal CreateSignal(object dbcMessage)
        {
            DbcSignal signal = null;
            if (dbcMessage is DbcMessage message)
            {
                var newName = DbcManager.SharedInstance.GetNewSignalName($"{message.Name}_NewSignal");
                signal = new DbcSignal { Name= newName };
                message.Signals.Add(signal);
            }

            return signal;
        }

        internal void DeleteSignals(object messageObject, IList signalObjects)
        {
            if (messageObject is DbcMessage dbcMessage)
            {
                foreach (var signalObject in signalObjects)
                {
                    dbcMessage.Signals.Remove(signalObject as DbcSignal);
                }
            }
        }

        internal void CopySignals(IList signalObjects)
        {
            copiedSignals.Clear();
            foreach (var signalObject in signalObjects)
            {
                if (signalObject is DbcSignal signal)
                {
                    copiedSignals.Add(signal);
                }
            }
        }

        internal List<DbcSignal> PasteSignals(object messageObject)
        {
            if (messageObject is DbcMessage message)
            {
                var pastedSignals = new List<DbcSignal>();
                foreach (var signal in copiedSignals)
                {
                    var clonedSignal = signal.Clone() as DbcSignal;
                    pastedSignals.Add(clonedSignal);
                }
                message.Signals.AddRange(pastedSignals);
                return pastedSignals;
            }

            return null;
        }

        internal void ConfigureSignalEditingControl(string propertyName, Control editControl)
        {
            switch (propertyName)
            {
                case nameof(DbcMessage.ID):
                    if (editControl is NumericUpDown idNUDown)
                    {
                        idNUDown.Hexadecimal = true;
                    }
                    break;
                case nameof(DbcMessage.DLC):
                    if (editControl is ComboBox dlcComboBox)
                    {
                        dlcComboBox.FormattingEnabled = true;
                        dlcComboBox.Format += delegate (object sender, ListControlConvertEventArgs e)
                        {
                            e.Value = ((CanDLC)e.Value).GetText();
                        };
                    }
                    break;
            }

            if (editControl is TextBox textBox)
            {
                textBox.SelectAll();
            }
            else if (editControl is NumericUpDown nuDown)
            {
                nuDown.Select(0, nuDown.Text.Length);
            }
        }

        internal Control GetSignalEditingControl(string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(DbcSignal.ErrorID):
                    var textBox = new TextBox();
                    if (value is long?)
                    {
                        var longValue = (long?)value;
                        if (longValue != null)
                        {
                            textBox.Text = string.Format("{0:X}", longValue);
                        }
                    }
                    textBox.SelectAll();
                    return textBox;
            }

            return null;
        }

        internal bool GetNewSignalValue(object rowObject, string propertyName, object value, out object newValue)
        {
            newValue = value;
            switch (propertyName)
            {
                case nameof(DbcSignal.ErrorID):
                    if (value is string textValue)
                    {
                        if (string.IsNullOrWhiteSpace(textValue))
                        {
                            newValue = null;
                        }
                        else
                        {
                            if (long.TryParse(textValue, NumberStyles.HexNumber, CultureInfo.InvariantCulture, out long parsed))
                            {
                                newValue = parsed;
                            }
                            else
                            {
                                newValue = null;
                            }
                        }
                    }
                    break;
                case nameof(DbcSignal.Name):
                    // 이름이 중복되는가 검사한다.
                    var newName = value as string;
                    var signal = DbcManager.SharedInstance.FindSignalByName(newName, rowObject, out _);
                    if (signal != null)
                    {
                        InformationBox.Show($"지정한 이름({newName})을 가진 CAN 시그널이 이미 존재합니다.",
                            "Validation Error", buttons: InformationBoxButtons.OK, icon: InformationBoxIcon.Error);
                        return false;
                    }
                    break;
            }
            return true;
        }
    }
}
