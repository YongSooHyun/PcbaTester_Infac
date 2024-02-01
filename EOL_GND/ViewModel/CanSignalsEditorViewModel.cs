using EOL_GND.Model;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace EOL_GND.ViewModel
{
    internal class CanSignalsEditorViewModel
    {
        internal List<CanSignal> CanSignals { get; private set; }

        internal CanSignalsEditorViewModel(IEnumerable<object> signals)
        {
            var convertedSignals = signals?.Cast<CanSignal>()?.ToList();
            CanSignals = new List<CanSignal>();
            if (convertedSignals != null)
            {
                foreach (var signal in convertedSignals)
                {
                    CanSignals.Add(signal.Clone() as CanSignal);
                }
            }
        }

        internal CanSignal CreateSignal()
        {
            var dialog = new View.DbcEditorForm();
            dialog.EditingEnabled = false;
            dialog.SelectingMessages = false;
            dialog.StartPosition = FormStartPosition.CenterParent;
            if (dialog.ShowDialog() == DialogResult.OK)
            {
                var signal = new CanSignal();
                signal.SignalName = dialog.SelectedSignal.Name;
                //signal.SignalInfo = dialog.SelectedSignal;
                if (CanSignals == null)
                {
                    CanSignals = new List<CanSignal>();
                }
                CanSignals.Add(signal);
                return signal;
            }

            return null;
        }

        internal void DeleteSignals(IList signalObjects)
        {
            foreach (var signalObject in signalObjects)
            {
                CanSignals.Remove(signalObject as CanSignal);
            }
        }

        internal bool MoveUpSignal(object selectedObject, int selectedIndex)
        {
            bool removed = false;
            if (selectedIndex > 0)
            {
                var selectedSignal = selectedObject as CanSignal;
                removed = CanSignals.Remove(selectedSignal);
                if (removed)
                {
                    CanSignals.Insert(selectedIndex - 1, selectedSignal);
                }
            }
            return removed;
        }

        internal bool MoveDownSignal(object selectedObject, int selectedIndex)
        {
            bool removed = false;
            if (selectedIndex >= 0 && selectedIndex < CanSignals.Count - 1)
            {
                var selectedSignal = selectedObject as CanSignal;
                removed = CanSignals.Remove(selectedSignal);
                if (removed)
                {
                    CanSignals.Insert(selectedIndex + 1, selectedSignal);
                }
            }
            return removed;
        }

        internal static bool EditProperty(object rowObject, string propertyName, object value)
        {
            switch (propertyName)
            {
                case nameof(CanSignal.SignalName):
                    var editingSignal = rowObject as CanSignal;
                    var dialog = new View.DbcEditorForm();
                    dialog.SignalName = editingSignal?.SignalName;
                    dialog.EditingEnabled = false;
                    dialog.SelectingMessages = false;
                    dialog.StartPosition = FormStartPosition.CenterParent;
                    if (dialog.ShowDialog() == DialogResult.OK)
                    {
                        if (editingSignal != null)
                        {
                            editingSignal.SignalName = dialog.SelectedSignal.Name;
                            //editingSignal.SignalInfo = dialog.SelectedSignal;
                        }
                    }
                    return true;
            }

            return false;
        }

        internal static string GetToolTipText(object model, string propertyName)
        {
            switch (propertyName)
            {
                case nameof(CanSignal.ValueType):
                    return 
                        $"• {nameof(CanSignal.AutoValueType.CRC)}: 이 시그널을 제외하고 사용자 데이터, Alive Counter, Data ID를 포함한 전체 데이터 블록에 대하여 계산된 CRC 값.\r\n" +
                        $"  - CRC 다항식: 0x1021, 초기값: 0xFFFF, XOR 값: 0x0000, Data ID: Message ID + 0xF800\r\n" +
                        $"  - 초기값은 CRC 계산을 위한 것이고, 메시지는 계산된 결과값과 함께 전송되어야 합니다.\r\n" +
                        $"  - 16 비트 CRC 다항식의 수학적 표현: G(x) = x16 + x12 + x5 + 1\r\n" +
                        $"• {nameof(CanSignal.AutoValueType.AliveCounter)}: 첫 전송에서는 counter가 0이고 그 다음은 1씩 증가합니다. counter가 최대값 0xFF에 도달하면 0부터 다시 시작합니다.\r\n" +
                        $"• {nameof(CanSignal.AutoValueType.Manual)}: {nameof(CanSignal.Value)}로 지정한 값이 사용됩니다.";
            }

            return null;
        }
    }
}
