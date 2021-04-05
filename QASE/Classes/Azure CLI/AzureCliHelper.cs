// <copyright file="AzureCliHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes.Azure_CLI
{
    using System;
    using System.Configuration;

    /// <summary>
    /// Helper class to leverage Azure CLI cmdlets
    /// </summary>
    public class AzureCliHelper
    {
        /// <summary>
        /// Cfg name for Azure CLI to retrieve Subscriptions
        /// </summary>
        private const string CfgRetrieveSubscriptions = "CfgRetrieveSubscriptions";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve ResourceGroups
        /// </summary>
        private const string CfgRetrieveResourceGroups = "CfgRetrieveResourceGroups";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve CosmosDbAccounts
        /// </summary>
        private const string CfgRetrieveCosmosDbAccounts = "CfgRetrieveCosmosDbAccounts";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve CosmosDbInfo
        /// </summary>
        private const string CfgRetrieveCosmosDbInfo = "CfgRetrieveCosmosDbInfo";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve CosmosDbDataBaseInfo
        /// </summary>
        private const string CfgRetrieveCosmosDbDataBaseInfo = "CfgRetrieveCosmosDbDataBaseInfo";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve CosmosDbDataBaseContainerInfo
        /// </summary>
        private const string CfgRetrieveCosmosDbDataBaseContainerInfo = "CfgRetrieveCosmosDbDataBaseContainerInfo";

        /// <summary>
        /// Cfg name for Azure CLI to retrieve CosmosDbKeys
        /// </summary>
        private const string CfgRetrieveCosmosDbKeys = "CfgRetrieveCosmosDbKeys";

        /// <summary>
        /// Cfg name for Azure CLI to Azure CliLogin
        /// </summary>
        private const string CfgAzureCliLogin = "CfgAzureCliLogin";

        /// <summary>
        /// Cfg name for Azure CLI to Check LoggedIn
        /// </summary>
        private const string CfgCliCheckLoggedIn = "CfgCliCheckLoggedIn";

        /// <summary>
        /// Cfg name for Azure CLI login failure message
        /// </summary>
        private const string CfgAzureCliLoginFailure = "CfgAzureCliLoginFailure";

        /// <summary>
        /// Cfg name for Azure CLI login credentials expired message
        /// </summary>
        private const string CfgAzureCliLoginCredentialsExpired = "CfgAzureCliLoginCredentialsExpired";

        private readonly string azureCliLoginFailure;
        private readonly string azureCliLoginCredentialsExpired;

        private readonly PowerShellExecutor powerShellExecutor;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCliHelper"/> class.
        /// </summary>
        /// <param name="powerShellExecutor">powerShellExecutor</param>
        public AzureCliHelper(PowerShellExecutor powerShellExecutor)
        {
            this.azureCliLoginFailure = ConfigurationManager.AppSettings[AzureCliHelper.CfgAzureCliLoginFailure];
            this.azureCliLoginCredentialsExpired = ConfigurationManager.AppSettings[AzureCliHelper.CfgAzureCliLoginCredentialsExpired];

            this.powerShellExecutor = powerShellExecutor ?? throw new ArgumentNullException(nameof(powerShellExecutor));
        }

        /// <summary>
        /// Logs in to AAD via Azure CLI.
        /// </summary>
        /// <param name="checkAlreadyLoggedIn">checkAlreadyLoggedIn</param>
        /// <returns>True if login suceeds, false otherwise</returns>
        public bool LoginToAzViaCLI(bool checkAlreadyLoggedIn = true)
        {
            if (checkAlreadyLoggedIn)
            {
                if (this.CliCheckLoggedIn())
                {
                    return true;
                }
            }

            var azloginOutput = this.AzureCliLogin();
            if (azloginOutput.Contains(this.azureCliLoginFailure))
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        /// <summary>
        /// Retreives Azure CLI output of subscriptions available to user.
        /// </summary>
        /// <returns>Azure CLI output of subscriptions available to user</returns>
        public string RetrieveSubscriptions()
        {
            return this.powerShellExecutor.ExecuteSynchronously(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveSubscriptions]);
        }

        /// <summary>
        /// Retreives Azure CLI output of Azure resource groups under a specified Azure subscription.
        /// </summary>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of Azure resource groups under a specified Azure subscription</returns>
        public string RetrieveResourceGroups(string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveResourceGroups], subscriptionId));
        }

        /// <summary>
        /// Retreives Azure CLI output of Azure CosmosDb accounts under a specified Azure subscription and Resource Group.
        /// </summary>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of Azure CosmosDb accounts under a specified Azure subscription and Resource Group</returns>
        public string RetrieveCosmosDbAccounts(string resourceGroup, string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveCosmosDbAccounts], resourceGroup, subscriptionId));
        }

        /// <summary>
        /// Retreives Azure CLI output of information of an Azure CosmosDb under a specified Azure subscription and Resource Group.
        /// </summary>
        /// <param name="cosmosDbAccountName">cosmosDbAccountName</param>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of Azure CosmosDb databases under a specified Azure subscription, Resource Group and CosmosDb account</returns>
        public string RetrieveCosmosDbInfo(string cosmosDbAccountName, string resourceGroup, string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveCosmosDbInfo], cosmosDbAccountName, resourceGroup, subscriptionId));
        }

        /// <summary>
        /// Retrieves Azure CLI output of Azure CosmosDb databases under a specified Azure subscription, Resource Group and CosmosDb account
        /// </summary>
        /// <param name="cosmosDbAccountName">cosmosDbAccountName</param>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of Azure CosmosDb databases under a specified Azure subscription, Resource Group and CosmosDb account</returns>
        public string RetrieveCosmosDbDataBaseInfo(string cosmosDbAccountName, string resourceGroup, string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveCosmosDbDataBaseInfo], cosmosDbAccountName, resourceGroup, subscriptionId));
        }

        /// <summary>
        /// Retreives Azure CLI output of information of an Azure Database under a specified Azure subscription, Resource Group and CosmosDb account.
        /// </summary>
        /// <param name="databaseName">databaseName</param>
        /// <param name="accountName">accountName</param>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of information of an Azure Database under a specified Azure subscription, Resource Group and CosmosDb account</returns>
        public string RetrieveCosmosDbDataBaseContainerInfo(string databaseName, string accountName, string resourceGroup, string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveCosmosDbDataBaseContainerInfo], databaseName, accountName, resourceGroup, subscriptionId));
        }

        /// <summary>
        /// Retrieves Azure CLI output of Azure CosmosDb keys of a specified Azure subscription, Resource Group and CosmosDb account.
        /// </summary>
        /// <param name="cosmosDbAccountName">cosmosDbAccountName</param>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>Azure CLI output of Azure CosmosDb keys of a specified Azure subscription, Resource Group and CosmosDb account</returns>
        public string RetrieveCosmosDbKeys(string cosmosDbAccountName, string resourceGroup, string subscriptionId)
        {
            return this.powerShellExecutor.ExecuteSynchronously(string.Format(ConfigurationManager.AppSettings[AzureCliHelper.CfgRetrieveCosmosDbKeys], cosmosDbAccountName, resourceGroup, subscriptionId));
        }

        private bool CliCheckLoggedIn()
        {
            var output = this.powerShellExecutor.ExecuteSynchronously(ConfigurationManager.AppSettings[AzureCliHelper.CfgCliCheckLoggedIn]);
            if (output.Contains(this.azureCliLoginCredentialsExpired) || output.Length == 0)
            {
                return false;
            }
            else
            {
                return true;
            }
        }

        private string AzureCliLogin()
        {
            return this.powerShellExecutor.ExecuteSynchronously(ConfigurationManager.AppSettings[AzureCliHelper.CfgAzureCliLogin]);
        }
    }
}
