// <copyright file="PowerShellExecutor.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System.Management.Automation;
    using System.Text;

    /// <summary>
    /// Provides PowerShell script execution.
    /// </summary>
    public class PowerShellExecutor
    {
        private readonly PowerShell powerShellInstance;

        /// <summary>
        /// Initializes a new instance of the <see cref="PowerShellExecutor"/> class.
        /// </summary>
        public PowerShellExecutor()
        {
            this.powerShellInstance = PowerShell.Create();
        }

        /// <summary>
        /// Executes powershell commands synchronously
        /// </summary>
        /// <param name="command">command</param>
        /// <returns>Output of command being executed.</returns>
        public string ExecuteSynchronously(string command)
        {
            this.powerShellInstance.AddScript(command);
            var psOutput = this.powerShellInstance.Invoke();
            this.powerShellInstance.Commands.Clear();

            StringBuilder stringBuilder = new StringBuilder();
            foreach (PSObject outputItem in psOutput)
            {
                stringBuilder.Append(outputItem.BaseObject.ToString());
            }

            return stringBuilder.ToString();
        }
    }
}
