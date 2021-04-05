// <copyright file="Common.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Configuration;
    using System.Linq;
    using Microsoft.PowerShell.Commands;
    using Microsoft.Win32;

    /// <summary>
    /// Class for common functions.
    /// </summary>
    public static class Common
    {
        /// <summary>
        /// Name of Azure dependency required.
        /// </summary>
        public static readonly string AzureCliSoftwareName = "Microsoft Azure CLI";

        /// <summary>
        /// Cfg name for DefaultMaxAutoscaleThroughput
        /// </summary>
        private const string CfgDefaultMaxThroughput = "DefaultMaxAutoscaleThroughput";

        /// <summary>
        /// Finds if an applicaiton is intalled.
        /// </summary>
        /// <param name="softwareTitle">p_name</param>
        /// <returns>True if application specified is installed, false otherwise</returns>
        public static bool IsApplicationInstalled(string softwareTitle)
        {
            string displayName;
            RegistryKey key;

            // search in: CurrentUser
            key = Registry.CurrentUser.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (softwareTitle.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }

            // search in: LocalMachine_32
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (softwareTitle.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }

            // search in: LocalMachine_64
            key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Wow6432Node\Microsoft\Windows\CurrentVersion\Uninstall");
            foreach (string keyName in key.GetSubKeyNames())
            {
                RegistryKey subkey = key.OpenSubKey(keyName);
                displayName = subkey.GetValue("DisplayName") as string;
                if (softwareTitle.Equals(displayName, StringComparison.OrdinalIgnoreCase) == true)
                {
                    return true;
                }
            }

            return false;
        }

        /// <summary>
        /// Returns the default max autoscale throughput settings from app.config
        /// </summary>
        /// <returns>default max autoscale settings as specified in app.config</returns>
        public static int DefaultMaxAutosscaleThroughput()
        {
            var maxAutoScaleThroughputFromAppConfig = ConfigurationManager.AppSettings[Common.CfgDefaultMaxThroughput];
            var isInteger = int.TryParse(maxAutoScaleThroughputFromAppConfig, out int resVal);
            if (isInteger)
            {
                return resVal;
            }
            else
            {
                return 8000;
            }
        }
    }
}
