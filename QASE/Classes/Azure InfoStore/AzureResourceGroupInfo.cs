// <copyright file="AzureResourceGroupInfo.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to hold Azure resource group information.
    /// </summary>
    public class AzureResourceGroupInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureResourceGroupInfo"/> class.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="location">location</param>
        /// <param name="name">name</param>
        /// <param name="provisioningState">provisioningState</param>
        public AzureResourceGroupInfo(string id, string location, string name, string provisioningState)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Location = location ?? throw new ArgumentNullException(nameof(location));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
            this.ProvisioningState = provisioningState ?? throw new ArgumentNullException(nameof(provisioningState));
        }

        /// <summary>
        /// Gets or sets Id
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets Location
        /// </summary>
        public string Location { get; set; }

        /// <summary>
        /// Gets or sets Name
        /// </summary>
        public string Name { get; set; }

        /// <summary>
        /// Gets or sets ProvisioningState
        /// </summary>
        public string ProvisioningState { get; set; }
    }
}
