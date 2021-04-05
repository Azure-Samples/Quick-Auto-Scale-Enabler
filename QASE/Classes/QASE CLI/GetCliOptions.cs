// <copyright file="GetCliOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes.CLI
{
    using System.Collections.Generic;
    using CommandLine;

    /// <summary>
    /// Command line parser options for getting autoscale settings.
    /// </summary>
    [Verb("Get", HelpText = "Retrieves autoscale settings and max throughputs of Azure Containers specified")]
    public class GetCliOptions
    {
        /// <summary>
        /// Gets or sets Azure Subscription Id
        /// </summary>
        [Option("Azure-Subscription-Id", Required = true)]
        public string AzureSubscriptionId { get; set; }

        /// <summary>
        /// Gets or sets Azure Resource group name
        /// </summary>
        [Option("Azure-Resource-group-name", Required = true)]
        public string AzureResourceGroupName { get; set; }

        /// <summary>
        /// Gets or sets Azure CosmosDb account name
        /// </summary>
        [Option("Azure-cosmosdb-account-name", Required = true)]
        public string AzureCosmosDbAccountName { get; set; }

        /// <summary>
        /// Gets or sets Azure CosmosDb database name
        /// </summary>
        [Option("Azure-cosmosdb-database-name", Required = true)]
        public string AzureCosmosDbDatabaseName { get; set; }

        /// <summary>
        /// Gets or sets Azure containers whose autoscale settings you want to view.
        /// </summary>
        [Option("Azure-container-names")]
        public IList<string> OrderedAzureContainerNames { get; set; }
    }
}
