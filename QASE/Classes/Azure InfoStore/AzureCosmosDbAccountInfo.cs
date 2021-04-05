// <copyright file="AzureCosmosDbAccountInfo.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Collections.Generic;
    using System.Security;

    /// <summary>
    /// Class to hold CosmosDb account information
    /// </summary>
    public class AzureCosmosDbAccountInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureCosmosDbAccountInfo"/> class.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">name</param>
        /// <param name="provisioningState">provisioningState</param>
        /// <param name="resourceGroup">resourceGroup</param>
        /// <param name="documentEndpoint">documentEndpoint</param>
        /// <param name="primaryKey">primaryKey</param>
        public AzureCosmosDbAccountInfo(string id, string name, string provisioningState, string resourceGroup, string documentEndpoint, SecureString primaryKey)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.ProvisioningState = provisioningState ?? throw new ArgumentNullException(nameof(provisioningState));
            this.ResourceGroup = resourceGroup ?? throw new ArgumentNullException(nameof(resourceGroup));
            this.DocumentEndpoint = documentEndpoint ?? throw new ArgumentNullException(nameof(documentEndpoint));
            this.PrimaryKey = primaryKey ?? throw new ArgumentNullException(nameof(primaryKey));
        }

        /// <summary>
        /// Gets or Sets Id
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        ///  Gets or Sets Name
        /// </summary>
        public string Name { get; private set; }

        /// <summary>
        ///  Gets or Sets ProvisioningState
        /// </summary>
        public string ProvisioningState { get; private set; }

        /// <summary>
        /// Gets or Sets ResourceGroup
        /// </summary>
        public string ResourceGroup { get; private set; }

        /// <summary>
        ///  Gets or Sets DocumentEndpoint
        /// </summary>
        public string DocumentEndpoint { get; private set; }

        /// <summary>
        ///  Gets or Sets PrimaryKey
        /// </summary>
        public SecureString PrimaryKey { get; private set; }

        /// <summary>
        ///  Gets or Sets the CosmosDb account's container database info.
        /// </summary>
        public Dictionary<string, List<string>> ContainersDatabaseInfo { get; private set; }

        /// <summary>
        /// Adds or updates container information.
        /// </summary>
        /// <param name="databaseName">databaseName</param>
        /// <param name="containerName">containerName</param>
        public void AddOrUpdateContainerInfo(string databaseName, string containerName)
        {
            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(containerName)));
            }

            if (string.IsNullOrEmpty(containerName))
            {
                throw new ArgumentException(string.Format("'{0}' cannot be null or empty", nameof(containerName)));
            }

            if (!this.ContainersDatabaseInfo.ContainsKey(databaseName))
            {
                this.ContainersDatabaseInfo.Add(databaseName, new List<string> { containerName });
            }
            else
            {
                this.ContainersDatabaseInfo[databaseName].Add(containerName);
            }
        }

        /// <summary>
        /// Removes a database from CosmosDb account information.
        /// </summary>
        /// <param name="databaseName">databaseName</param>
        /// <returns>True if removal is successful, false otherwise</returns>
        public bool RemoveAzureCosmosDbsInfo(string databaseName)
        {
            if (databaseName is null)
            {
                throw new ArgumentNullException(nameof(databaseName));
            }

            return this.ContainersDatabaseInfo.Remove(databaseName);
        }
    }
}
