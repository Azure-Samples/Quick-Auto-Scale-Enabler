// <copyright file="SetCliOptions.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes.CLI
{
    using System.Collections.Generic;
    using CommandLine;

    /// <summary>
    /// Command line parser options for setting autoscale settings.
    /// </summary>
    [Verb("Set", HelpText = "Updates autoscale settings (and/or max throughput settings) of Azure Containers specified")]
    public class SetCliOptions
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
        /// Gets or sets Azure containers whose autoscale settings you want to alter.
        /// </summary>
        [Option("Azure-ordered-container-names", Required = true)]
        public IList<string> OrderedAzureContainerNames { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of new autoscale settings.
        /// </summary>
        [Option("Azure-ordered-autoscaleSettings", Required = true)]
        public IList<bool> OrderedNewAutoScaleSettings { get; set; }

        /// <summary>
        /// Gets or sets the ordered list of max auto scale throughput.
        /// </summary>
        [Option("Azure-ordered-NewMaxAutoscaleThroughputsOrProvisionedThroughputValues", Required = true)]
        public IList<int?> OrderedNewMaxAutoScaleThroughput { get; set; }
    }
}
