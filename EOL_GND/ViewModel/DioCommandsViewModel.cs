using EOL_GND.Device;
using EOL_GND.Model;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND.ViewModel
{
    internal class DioCommandsViewModel
    {
        internal List<DioDevice.CommandInfo> Commands { get; private set; }

        /// <summary>
        /// 사용자가 선택한 명령 리스트.
        /// </summary>
        internal List<string> CheckedCommands { get; set; } = new List<string>();

        internal DioCommandsViewModel(object value)
        {
            Commands = value as List<DioDevice.CommandInfo>;
        }

        internal DioDevice.CommandInfo CreateNewCommand()
        {
            var command = new DioDevice.CommandInfo();

            // 새로운 이름 만들기.
            var currentCommands = Commands;
            string lastCommandName = null;
            if (currentCommands?.Count() > 0)
            {
                lastCommandName = currentCommands.Last().Command;
            }
            command.Command = DeviceSetting.CreateNewName(lastCommandName, "NewCommand");

            if (Commands == null)
            {
                Commands = new List<DioDevice.CommandInfo>();
            }
            Commands.Add(command);

            return command;
        }

        internal bool RemoveObjects(System.Collections.IEnumerable objects)
        {
            if (Commands == null)
            {
                return false;
            }

            bool removed = false;
            foreach (var obj in objects)
            {
                if (Commands.Remove(obj as DioDevice.CommandInfo))
                {
                    removed = true;
                }
            }

            return removed;
        }

        internal object AfterEdit(string propertyName, object value)
        {
            object editedValue = value;
            switch (propertyName)
            {
                case nameof(DioDevice.CommandInfo.Command):
                    if (value is string textValue)
                    {
                        editedValue = textValue.Trim();
                    }
                    break;
            }

            return editedValue;
        }

        internal void CheckObject(object obj, bool checkState)
        {
            if (obj is DioDevice.CommandInfo commandInfo)
            {
                if (checkState)
                {
                    CheckedCommands.Add(commandInfo.Command);
                }
                else
                {
                    for (int i = CheckedCommands.Count - 1; i >= 0; i--)
                    {
                        if (string.Equals(CheckedCommands[i], commandInfo.Command, StringComparison.OrdinalIgnoreCase))
                        {
                            CheckedCommands.RemoveAt(i);
                            break;
                        }
                    }
                }
            }
        }

        internal List<DioDevice.CommandInfo> GetCheckedObjects()
        {
            var checkedObjects = new List<DioDevice.CommandInfo>();

            foreach (var commandInfo in Commands)
            {
                foreach (var command in CheckedCommands)
                {
                    if (string.Equals(command, commandInfo.Command, StringComparison.OrdinalIgnoreCase))
                    {
                        checkedObjects.Add(commandInfo);
                    }
                }
            }

            return checkedObjects;
        }
    }
}
