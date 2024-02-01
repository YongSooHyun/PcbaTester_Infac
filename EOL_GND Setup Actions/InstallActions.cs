using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace EOL_GND_Setup_Actions
{
    public static class InstallActions
    {
        private const string EnvPathVarName = "PATH";

        /// <summary>
        /// Add value to the environment PATH variable.
        /// </summary>
        /// <param name="value"></param>
        public static void AddEnvironmentPathValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            string pathValue = Environment.GetEnvironmentVariable(EnvPathVarName, EnvironmentVariableTarget.Machine);
            string newPathValue;
            if (string.IsNullOrEmpty(pathValue))
            {
                newPathValue = value;
            }
            else
            {
                newPathValue = $"{pathValue}{Path.PathSeparator}{value}";
            }
            Environment.SetEnvironmentVariable(EnvPathVarName, newPathValue, EnvironmentVariableTarget.Machine);
        }

        /// <summary>
        /// Remove value from the environment PATH variable.
        /// </summary>
        /// <param name="value"></param>
        public static void RemoveEnvironmentPathValue(string value)
        {
            if (string.IsNullOrEmpty(value))
            {
                return;
            }

            string pathValue = Environment.GetEnvironmentVariable(EnvPathVarName, EnvironmentVariableTarget.Machine);
            if (!string.IsNullOrEmpty(pathValue))
            {
                Environment.SetEnvironmentVariable(EnvPathVarName, pathValue.Replace($"{Path.PathSeparator}{value}", null), EnvironmentVariableTarget.Machine);
            }
        }
    }
}
