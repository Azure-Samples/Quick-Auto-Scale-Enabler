// <copyright file="AzureSubscriptionInfo.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Collections.Generic;

    /// <summary>
    /// Class to store Azure subscription info.
    /// </summary>
    public class AzureSubscriptionInfo
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="AzureSubscriptionInfo"/> class.
        /// </summary>
        /// <param name="id">id</param>
        /// <param name="name">name</param>
        public AzureSubscriptionInfo(string id, string name)
        {
            this.Id = id ?? throw new ArgumentNullException(nameof(id));
            this.Name = name ?? throw new ArgumentNullException(nameof(name));
        }

        /// <summary>
        /// Gets or sets the Id.
        /// </summary>
        public string Id { get; set; }

        /// <summary>
        /// Gets or sets the name.
        /// </summary>
        public string Name { get; set; }
    }
}
