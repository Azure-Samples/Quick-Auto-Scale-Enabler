// <copyright file="AzureResourceInfoStore.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Collections.Generic;
    using System.Security;
    using System.Windows.Forms;
    using Newtonsoft.Json;
    using QASE.Classes.Azure_CLI;

    /// <summary>
    /// Class for handling Azure Resource information throughput Azure CLI.
    /// </summary>
    public class AzureResourceInfoStore
    {
        private readonly AzureCliHelper azureCliHelper;

        /// <summary>
        /// Initializes a new instance of the <see cref="AzureResourceInfoStore"/> class.
        /// </summary>
        public AzureResourceInfoStore()
        {
            this.azureCliHelper = new AzureCliHelper(new PowerShellExecutor());

            if (!Common.IsApplicationInstalled(Common.AzureCliSoftwareName))
            {
                throw new Exception("Azure is required for QASE to run");
            }
        }

        /// <summary>
        /// Retrives all Azure subcriptions avaiable to a user. Prompts Azure CLI login as needed.
        /// </summary>
        /// <returns>List of <see cref="AzureSubscriptionInfo"/> available to a user</returns>
        public List<AzureSubscriptionInfo> RetrieveAvailableAzureSubscriptions()
        {
            // Log-in if credentials have expired / not logged-in yet.
            if (!this.azureCliHelper.LoginToAzViaCLI())
            {
                return null;
            }

            var azureSubscriptions = new List<AzureSubscriptionInfo>();
            var azGetSubscriptionsOutput = this.azureCliHelper.RetrieveSubscriptions();

            // Get Azure subscriptions
            dynamic subscriptionsInfo = JsonConvert.DeserializeObject(azGetSubscriptionsOutput);

            // No subscriptions retreieved || JIT not obtained
            if (subscriptionsInfo == null || !subscriptionsInfo.HasValues)
            {
                return new List<AzureSubscriptionInfo>();
            }

            // Go over each available subscription
            foreach (var subscription in subscriptionsInfo)
            {
                string subscriptionId = subscription.id;
                string subscriptionName = subscription.name;

                var azureSubscription = new AzureSubscriptionInfo(
                    id: subscriptionId,
                    name: subscriptionName);

                azureSubscriptions.Add(azureSubscription);
            }

            return azureSubscriptions;
        }

        /// <summary>
        /// Retrives all Azure resource group details under a given subcription name
        /// </summary>
        /// <param name="subscriptionName">subscriptionName</param>
        /// <returns>List of <see cref="AzureResourceInfoStore"/> under a specified Azure subscription</returns>
        public List<AzureResourceGroupInfo> RetrieveAzureResourceGroupInfoUnderAzureSubscription(string subscriptionName)
        {
            if (string.IsNullOrWhiteSpace(subscriptionName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(subscriptionName)));
            }

            if (!this.azureCliHelper.LoginToAzViaCLI())
            {
                return null;
            }

            // Get available Azure resource groups under subscription
            dynamic resourceGroupsUnderSubscriptionInfo = JsonConvert.DeserializeObject(this.azureCliHelper.RetrieveResourceGroups(subscriptionName));

            // No Azure Resource groups retreieved || JIT not obtained
            if (resourceGroupsUnderSubscriptionInfo == null || !resourceGroupsUnderSubscriptionInfo.HasValues)
            {
                return new List<AzureResourceGroupInfo>();
            }

            List<AzureResourceGroupInfo> azureResourceGroupsunderSubscription = new List<AzureResourceGroupInfo>();
            foreach (var resourceGroupUnderSubscription in resourceGroupsUnderSubscriptionInfo)
            {
                string resourceGroupName = resourceGroupUnderSubscription.name;

                var azureResourceGroupInfo = new AzureResourceGroupInfo(
                    id: (string)resourceGroupUnderSubscription.id,
                    name: (string)resourceGroupUnderSubscription.name,
                    location: (string)resourceGroupUnderSubscription.location,
                    provisioningState: (string)resourceGroupUnderSubscription.properties.provisioningState);

                azureResourceGroupsunderSubscription.Add(azureResourceGroupInfo);
            }

            return azureResourceGroupsunderSubscription;
        }

        /// <summary>
        /// Retreives all Azure CosmosDb account information under a specified Azure resource group and Azure subscription
        /// </summary>
        /// <param name="resourceGroupName">resourceGroupName</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>List of <see cref="AzureCosmosDbAccountInfo"/> under a specified Azure resource group and Azure subscription</returns>
        public List<AzureCosmosDbAccountInfo> RetrieveAzureCosmosDbAccountInfoUnderAzureResourceGroup(string resourceGroupName, string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(resourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(resourceGroupName)));
            }

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(subscriptionId)));
            }

            // Log-in if credentials have expired / not logged-in yet.
            if (!this.azureCliHelper.LoginToAzViaCLI())
            {
                return null;
            }

            dynamic cosmosDbsUnderResourceGroupInfo = JsonConvert.DeserializeObject(this.azureCliHelper.RetrieveCosmosDbAccounts(resourceGroupName, subscriptionId));

            // No Azure CosmosDb accounts retreieved
            if (cosmosDbsUnderResourceGroupInfo == null || !cosmosDbsUnderResourceGroupInfo.HasValues)
            {
                return new List<AzureCosmosDbAccountInfo>();
            }

            List<AzureCosmosDbAccountInfo> cosmosDbsUnderResourceGroup = new List<AzureCosmosDbAccountInfo>();
            foreach (var cosmosDbUnderResourceGroup in cosmosDbsUnderResourceGroupInfo)
            {
                string cosmosDbAccountName = cosmosDbUnderResourceGroup.name;

                // Retrieve and store Readonly primary key of each Azure CosmosDb account
                SecureString primaryReadonlyMasterKey = new SecureString();
                dynamic connectionStringInfo = JsonConvert.DeserializeObject(this.azureCliHelper.RetrieveCosmosDbKeys(cosmosDbAccountName, resourceGroupName, subscriptionId));

                if (connectionStringInfo == null)
                {
                    continue;
                }

                string connectionString = connectionStringInfo.primaryMasterKey;
                foreach (var character in connectionString)
                {
                    primaryReadonlyMasterKey.AppendChar(character);
                }

                var azureCosmosDbAccountInfo = new AzureCosmosDbAccountInfo(
                    id: (string)cosmosDbUnderResourceGroup.id,
                    name: (string)cosmosDbUnderResourceGroup.name,
                    provisioningState: (string)cosmosDbUnderResourceGroup.provisioningState,
                    resourceGroup: (string)cosmosDbUnderResourceGroup.resourceGroup,
                    documentEndpoint: (string)cosmosDbUnderResourceGroup.documentEndpoint,
                    primaryKey: primaryReadonlyMasterKey);

                cosmosDbsUnderResourceGroup.Add(azureCosmosDbAccountInfo);
            }

            return cosmosDbsUnderResourceGroup;
        }

        /// <summary>
        /// Retrieves CosmosDb databases under a specified Azure CosmosDb account name, Azure resource group and Azure subscription
        /// </summary>
        /// <param name="cosmosDbAccountName">cosmosDbAccountName</param>
        /// <param name="resourceGroupName">resourceGroupName</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>String-List of CosmosDb Databses under a specified Azure CosmosDb account name, Azure resource group and Azure subscription</returns>
        public List<string> RetrieveCosmosDbDatabasesUnderCosmosDbAccount(string cosmosDbAccountName, string resourceGroupName, string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(cosmosDbAccountName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(cosmosDbAccountName)));
            }

            if (string.IsNullOrWhiteSpace(resourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(resourceGroupName)));
            }

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(subscriptionId)));
            }

            // Log-in if credentials have expired / not logged-in yet.
            if (!this.azureCliHelper.LoginToAzViaCLI())
            {
                return null;
            }

            // Get Databases under each Azure CosmosDb account
            dynamic cosmosDbDatabaseInfo = JsonConvert.DeserializeObject(this.azureCliHelper.RetrieveCosmosDbDataBaseInfo(cosmosDbAccountName, resourceGroupName, subscriptionId));

            // No Databases retreieved
            if (cosmosDbDatabaseInfo == null || !cosmosDbDatabaseInfo.HasValues)
            {
                return new List<string>();
            }

            List<string> databasesUnderCosmosDbAccount = new List<string>();

            // Go over each Database under Azure CosmosDb account
            foreach (var cosmosDbDatabase in cosmosDbDatabaseInfo)
            {
                databasesUnderCosmosDbAccount.Add(cosmosDbDatabase.name.ToString());
            }

            return databasesUnderCosmosDbAccount;
        }

        /// <summary>
        /// Retrieves CosmosDb Container under a specified Azure CosmosDb Database, Azure CosmosDb account name, Azure resource group and Azure subscription
        /// </summary>
        /// <param name="cosmosDbDatabaseName">cosmosDbDatabaseName</param>
        /// <param name="cosmosDbAccountName">cosmosDbAccountName</param>
        /// <param name="resourceGroupName">resourceGroupName</param>
        /// <param name="subscriptionId">subscriptionId</param>
        /// <returns>String-List of CosmosDb containers under a specified Azure CosmosDb Database, Azure CosmosDb account name, Azure resource group and Azure subscription</returns>
        public List<string> RetrieveCosmosDbContainersUnderCosmosDbDatabase(string cosmosDbDatabaseName, string cosmosDbAccountName, string resourceGroupName, string subscriptionId)
        {
            if (string.IsNullOrWhiteSpace(cosmosDbDatabaseName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(cosmosDbDatabaseName)));
            }

            if (string.IsNullOrWhiteSpace(cosmosDbAccountName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(cosmosDbAccountName)));
            }

            if (string.IsNullOrWhiteSpace(resourceGroupName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(resourceGroupName)));
            }

            if (string.IsNullOrWhiteSpace(subscriptionId))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or whitespace", nameof(subscriptionId)));
            }

            // Log-in if credentials have expired / not logged-in yet.
            if (!this.azureCliHelper.LoginToAzViaCLI())
            {
                return null;
            }

            // Get Containers under each Azure CosmosDb account
            dynamic containersUnderCosmosDbDatabaseInfo = JsonConvert.DeserializeObject(this.azureCliHelper.RetrieveCosmosDbDataBaseContainerInfo(cosmosDbDatabaseName, cosmosDbAccountName, resourceGroupName, subscriptionId));

            // No containers retreieved
            if (containersUnderCosmosDbDatabaseInfo == null || !containersUnderCosmosDbDatabaseInfo.HasValues)
            {
                return new List<string>();
            }

            List<string> containersUnderCosmosDbDatabase = new List<string>();
            foreach (var containerUnderCosmosDbDatabase in containersUnderCosmosDbDatabaseInfo)
            {
                containersUnderCosmosDbDatabase.Add((string)containerUnderCosmosDbDatabase.name);
            }

            return containersUnderCosmosDbDatabase;
        }
    }
}
