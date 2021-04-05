// <copyright file="CliHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes.CLI
{
    using System;
    using System.Collections.Generic;
    using System.Drawing;
    using QASE.Classes.Azure_CLI;

    /// <summary>
    /// Cli helper
    /// </summary>
    public static class CliHelper
    {
        /// <summary>
        /// Sets autoscale settings of specified Azure containers (and their respective max autoscale throughputs, if autoscale is enabled)
        /// </summary>
        /// <param name="azureSubscriptionId">AzureSubscriptionId</param>
        /// <param name="azureResourceGroupName">AzureResourceGroupName</param>
        /// <param name="azureCosmosDbAccountName">AzureCosmosDbAccountName</param>
        /// <param name="azureCosmosDbDatabseName">AzureCosmosDbDatabseName</param>
        /// <param name="azureContainerNamesAndMaxThroughputSettings">azureContainerNamesAndMaxThroughputSettings</param>
        /// <returns>Tuple with autoscale setting and max autoscale throughput (if autoscale has been enabled)</returns>
        public static List<Tuple<bool, int?>> SetAutoScaleSettings(
            string azureSubscriptionId,
            string azureResourceGroupName,
            string azureCosmosDbAccountName,
            string azureCosmosDbDatabseName,
            IList<Tuple<string, bool, int?>> azureContainerNamesAndMaxThroughputSettings)
        {
            if (string.IsNullOrEmpty(azureSubscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureSubscriptionId)));
            }

            if (string.IsNullOrEmpty(azureResourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureResourceGroupName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbAccountName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbAccountName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbDatabseName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbDatabseName)));
            }

            if (azureContainerNamesAndMaxThroughputSettings is null)
            {
                throw new ArgumentNullException(nameof(azureContainerNamesAndMaxThroughputSettings));
            }

            var azureCliHelper = new AzureCliHelper(new PowerShellExecutor());
            azureCliHelper.LoginToAzViaCLI();

            List<Tuple<bool, int?>> returnValue = new List<Tuple<bool, int?>>();

            var azureResourceInfoStore = new AzureResourceInfoStore();
            var azureCosmosDbAccountInfo = azureResourceInfoStore.RetrieveAzureCosmosDbAccountInfoUnderAzureResourceGroup(
                azureResourceGroupName,
                azureSubscriptionId);

            var index = azureCosmosDbAccountInfo.FindIndex(account => account.Name.Equals(azureCosmosDbAccountName));
            if (index >= 0)
            {
                var azureCosmosDbAccountOfContainer = azureCosmosDbAccountInfo[index];
                foreach (var azureContainerNameAndMaxThroughputSetting in azureContainerNamesAndMaxThroughputSettings)
                {
                    string decryptedPrimaryKey = new System.Net.NetworkCredential(string.Empty, azureCosmosDbAccountOfContainer.PrimaryKey).Password;

                    Console.WriteLine(
                        string.Format(
                            "Processing container: {0}. Setting autoscale to {1}. Max Autoscale/provisioned throughput: {2}",
                            azureContainerNameAndMaxThroughputSetting.Item1,
                            azureContainerNameAndMaxThroughputSetting.Item2,
                            azureContainerNameAndMaxThroughputSetting.Item3.HasValue ? azureContainerNameAndMaxThroughputSetting.Item3.ToString() : "nil"));

                    try
                    {
                        ThroughputManagerHelper.SetAutoScaleSetting(
                            azureCosmosDbAccountOfContainer.DocumentEndpoint,
                            azureCosmosDbDatabseName,
                            azureContainerNameAndMaxThroughputSetting.Item1,
                            decryptedPrimaryKey,
                            azureContainerNameAndMaxThroughputSetting.Item2,
                            azureContainerNameAndMaxThroughputSetting.Item3);

                        // Add succesful record with new throughput
                        returnValue.Add(new Tuple<bool, int?>(
                            azureContainerNameAndMaxThroughputSetting.Item2,
                            azureContainerNameAndMaxThroughputSetting.Item2 ? azureContainerNameAndMaxThroughputSetting.Item3 : null));
                    }
                    catch (Exception ex)
                    {
                        Console.WriteLine(string.Format("Error while updating autoscale settings for: {0}. Exception: {1}", azureContainerNameAndMaxThroughputSetting.Item1, ex));
                    }
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Retrieves autoscale settings of specified Azure containers (and their respective max autoscale throughputs, if autoscale is enabled)
        /// </summary>
        /// <param name="azureSubscriptionId">AzureSubscriptionId</param>
        /// <param name="azureResourceGroupName">AzureResourceGroupName</param>
        /// <param name="azureCosmosDbAccountName">AzureCosmosDbAccountName</param>
        /// <param name="azureCosmosDbDatabseName">AzureCosmosDbDatabseName</param>
        /// <param name="azureContainerNames">azureContainerNames</param>
        /// <returns>Tuple with autoscale setting and max autoscale throughput (if autoscale has been enabled)</returns>
        public static IList<Tuple<bool, int?>> GetAutoScaleSettings(
            string azureSubscriptionId,
            string azureResourceGroupName,
            string azureCosmosDbAccountName,
            string azureCosmosDbDatabseName,
            IList<string> azureContainerNames)
        {
            if (string.IsNullOrEmpty(azureSubscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureSubscriptionId)));
            }

            if (string.IsNullOrEmpty(azureResourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureResourceGroupName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbAccountName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbAccountName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbDatabseName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbDatabseName)));
            }

            if (azureContainerNames is null)
            {
                throw new ArgumentNullException(nameof(azureContainerNames));
            }

            var azureCliHelper = new AzureCliHelper(new PowerShellExecutor());
            azureCliHelper.LoginToAzViaCLI();

            var azureResourceInfoStore = new AzureResourceInfoStore();
            var azureCosmosDbAccountInfo = azureResourceInfoStore.RetrieveAzureCosmosDbAccountInfoUnderAzureResourceGroup(
                azureResourceGroupName,
                azureSubscriptionId);

            List<Tuple<bool, int?>> returnValue = new List<Tuple<bool, int?>>();
            int index = azureCosmosDbAccountInfo.FindIndex(account => account.Name.Equals(azureCosmosDbAccountName));
            if (index >= 0)
            {
                var azureCosmosDbAccountOfContainer = azureCosmosDbAccountInfo[index];
                foreach (var azureContainerName in azureContainerNames)
                {
                    string decryptedPrimaryKey = new System.Net.NetworkCredential(string.Empty, azureCosmosDbAccountOfContainer.PrimaryKey).Password;

                    Console.WriteLine(
                        string.Format(
                            "Processing container: {0}",
                            azureContainerName));

                    var autoScaleSettingOfContainer = ThroughputManagerHelper.GetAutoScaleSetting(
                        azureCosmosDbAccountOfContainer.DocumentEndpoint,
                        azureCosmosDbDatabseName,
                        azureContainerName,
                        decryptedPrimaryKey);

                    returnValue.Add(new Tuple<bool, int?>(autoScaleSettingOfContainer.Item1, autoScaleSettingOfContainer.Item2));
                }
            }

            return returnValue;
        }

        /// <summary>
        /// Retrieves container names under specified subscriptionID, resource group, and CosmosDbAccount.
        /// </summary>
        /// <param name="azureSubscriptionId">AzureSubscriptionId</param>
        /// <param name="azureResourceGroupName">AzureResourceGroupName</param>
        /// <param name="azureCosmosDbAccountName">AzureCosmosDbAccountName</param>
        /// <param name="azureCosmosDbDatabaseName">AzureCosmosDbDatabseName</param>
        /// <returns>List of container names</returns>
        public static IList<string> GetContainersUnderCosmosDbDatabaseAndAccount(
            string azureSubscriptionId,
            string azureResourceGroupName,
            string azureCosmosDbAccountName,
            string azureCosmosDbDatabaseName)
        {
            if (string.IsNullOrEmpty(azureSubscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureSubscriptionId)));
            }

            if (string.IsNullOrEmpty(azureResourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureResourceGroupName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbAccountName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbAccountName)));
            }

            if (string.IsNullOrEmpty(azureCosmosDbDatabaseName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(azureCosmosDbDatabaseName)));
            }

            var azureCliHelper = new AzureCliHelper(new PowerShellExecutor());
            azureCliHelper.LoginToAzViaCLI();

            var azureResourceInfoStore = new AzureResourceInfoStore();
            var azureCosmosDbContainerNames = azureResourceInfoStore.RetrieveCosmosDbContainersUnderCosmosDbDatabase(
                azureCosmosDbDatabaseName,
                azureCosmosDbAccountName,
                azureResourceGroupName,
                azureSubscriptionId);

            return azureCosmosDbContainerNames;
        }
    }
}
