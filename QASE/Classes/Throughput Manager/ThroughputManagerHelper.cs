// <copyright file="ThroughputManagerHelper.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE.Classes
{
    using System;
    using System.Net.Http;
    using Microsoft.Azure.Cosmos;

    /// <summary>
    /// ThroughputManager helper for getting/setting Autoscale settings and max autoscale throughput
    /// </summary>
    public static class ThroughputManagerHelper
    {
        private static HttpClient httpclient;
        private static CosmosClient cosmosClient;

        /// <summary>
        /// Finds whether autoscale for a given Azure Container has been switched on.
        /// </summary>
        /// <param name="documentEndpoint">documentEndpoint</param>
        /// <param name="cosmosDbDatabaseName">cosmosDbDatabaseName</param>
        /// <param name="azureCosmosDbContainer">azureCosmosDbContainer</param>
        /// <param name="primaryKey">primaryKey</param>
        /// <returns>True is autoscale settings for a container has been turned on.</returns>
        public static Tuple<bool, int?> GetAutoScaleSetting(string documentEndpoint, string cosmosDbDatabaseName, string azureCosmosDbContainer, string primaryKey)
        {
            using (ThroughputManagerHelper.httpclient = new HttpClient())
            {
                using (ThroughputManagerHelper.cosmosClient = new CosmosClient(documentEndpoint, primaryKey))
                {
                    var database = ThroughputManagerHelper.cosmosClient.GetDatabase(cosmosDbDatabaseName);
                    var container = database.GetContainer(azureCosmosDbContainer);

                    var throughputManager = new ThroughputManager(ThroughputManagerHelper.httpclient);
                    return throughputManager.IsAutoScaleEnabled(container);
                }
            }
        }

        /// <summary>
        /// Changes the autoscale settings for an Azure Container with the specified max throughput
        /// </summary>
        /// <param name="documentEndpoint">documentEndpoint</param>
        /// <param name="cosmosDbDatabaseName">cosmosDbDatabaseName</param>
        /// <param name="azureCosmosDbContainer">azureCosmosDbContainer</param>
        /// <param name="primaryKey">primaryKey</param>
        /// <param name="newSetting">newSetting</param>
        /// <param name="maxThoughput">maxThoughput</param>
        public static void SetAutoScaleSetting(string documentEndpoint, string cosmosDbDatabaseName, string azureCosmosDbContainer, string primaryKey, bool newSetting, int? maxThoughput)
        {
            using (ThroughputManagerHelper.httpclient = new HttpClient())
            {
                using (ThroughputManagerHelper.cosmosClient = new CosmosClient(documentEndpoint, primaryKey))
                {
                    var database = ThroughputManagerHelper.cosmosClient.GetDatabase(cosmosDbDatabaseName);
                    var container = database.GetContainer(azureCosmosDbContainer);

                    var throughputManager = new ThroughputManager(ThroughputManagerHelper.httpclient);
                    throughputManager.SetThroughput(container, newSetting, maxThoughput);
                }
            }
        }
    }
}
