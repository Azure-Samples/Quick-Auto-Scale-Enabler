// <copyright file="ThroughputManager.cs" company="Microsoft Corporation">
// Copyright (c) Microsoft Corporation. All rights reserved.
// </copyright>

namespace QASE
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.Reflection;
    using System.Threading.Tasks;
    using Microsoft.Azure.Cosmos;
    using Newtonsoft.Json;

    /// <summary>
    /// Class to manager thoughput and autoscale settings
    /// </summary>
    public class ThroughputManager
    {
        private readonly HttpClient httpClient;

        /// <summary>
        /// Initializes a new instance of the <see cref="ThroughputManager"/> class.
        /// </summary>
        /// <param name="httpClient">httpClient</param>
        public ThroughputManager(HttpClient httpClient)
        {
            this.httpClient = httpClient;
        }

        /// <summary>
        /// Returns the state of autoscale settings for a specified container.
        /// </summary>
        /// <param name="container">container</param>
        /// <returns>True if autoscale is turned on, false otherwise</returns>
        public Tuple<bool, int?> IsAutoScaleEnabled(Container container)
        {
            ThroughputResponse currrentThroughput = container.ReadThroughputAsync(new RequestOptions { }).GetAwaiter().GetResult();
            var currentThroughput = currrentThroughput.Resource;
            return new Tuple<bool, int?>(
                currentThroughput.AutoscaleMaxThroughput.HasValue,
                currentThroughput.AutoscaleMaxThroughput.HasValue ? currrentThroughput.Resource.AutoscaleMaxThroughput : null);
        }

        /// <summary>
        /// Updates autoscale settings of the container with the given max throughput
        /// </summary>
        /// <param name="container">container</param>
        /// <param name="autoScale">autoScale</param>
        /// <param name="maxThroughput">maxThroughput</param>
        public void SetThroughput(Container container, bool autoScale, int? maxThroughput)
        {
            ThroughputResponse oldThroughputResponse = container.ReadThroughputAsync(new RequestOptions { }).GetAwaiter().GetResult();
            var currentThroughput = oldThroughputResponse.Resource;

            ThroughputProperties newThroughput = GenerateThroughputProperties(autoScale, maxThroughput);

            if (currentThroughput.AutoscaleMaxThroughput.HasValue != autoScale)
            {
                this.ChangeScalingMethodology(container.Database.Client, currentThroughput, this.GenerateContainerLink(container));
            }

            container.ReplaceThroughputAsync(newThroughput).GetAwaiter().GetResult();
        }

        /// <summary>
        /// Genreates throughput properties given autoscale settings and maxThroughput
        /// Relevant documentation on RU limits wrt Azure Databases/Containers: https://docs.microsoft.com/en-us/azure/cosmos-db/set-throughput
        /// </summary>
        /// <param name="autoScale">autoScale setting</param>
        /// <param name="maxThroughput">maxThroughput RUs</param>
        /// <returns>Generated ThroughputProperties</returns>
        private static ThroughputProperties GenerateThroughputProperties(bool autoScale, int? maxThroughput = null)
        {
            if (!autoScale)
            {
                if (!maxThroughput.HasValue || maxThroughput < 400)
                {
                    maxThroughput = 400;
                }

                return ThroughputProperties.CreateManualThroughput(maxThroughput.Value);
            }
            else
            {
                if (!maxThroughput.HasValue || maxThroughput < 4000)
                {
                    maxThroughput = 4000;
                }

                return ThroughputProperties.CreateAutoscaleThroughput(maxThroughput.Value);
            }
        }

        /// <summary>
        /// Toggle between Autoscale and Manual scaling methodologies for a database or container.
        /// </summary>
        /// <param name="client">client</param>
        /// <param name="currentThroughput">currentThroughput</param>
        /// <param name="scalableItemLink">scalableItemLink</param>
        private void ChangeScalingMethodology(CosmosClient client, ThroughputProperties currentThroughput, string scalableItemLink)
        {
            bool changeToAutoScale = !currentThroughput.AutoscaleMaxThroughput.HasValue;

            string offerId = currentThroughput.SelfLink.Split('/')[1];
            string offerResource = string.Format("offers/{0}", offerId);
            var url = string.Format("{0}://{1}/{2}", client.Endpoint.Scheme, client.Endpoint.Host, offerResource);
            var restEndpointUri = new Uri(url);
            var method = HttpMethod.Put;
            var httpDate = DateTime.UtcNow.ToString("R");
            string auth = this.GenerateAuthToken(method, "offers", offerId, httpDate, extractAuthKey());
            var request = new HttpRequestMessage
            {
                RequestUri = restEndpointUri,
                Method = method,
                Headers =
                {
                    { HttpRequestHeader.Authorization.ToString(), auth },
                    { "x-ms-version", "2018-12-31" },
                    { "x-ms-date", httpDate },
                },
                Content = new StringContent(JsonConvert.SerializeObject(createOffer()))
            };

            if (changeToAutoScale)
            {
                request.Headers.Add("x-ms-cosmos-migrate-offer-to-autopilot", "true");
            }
            else
            {
                request.Headers.Add("x-ms-cosmos-migrate-offer-to-manual-throughput", "true");
            }

            HttpResponseMessage putResponse = this.httpClient.SendAsync(request).GetAwaiter().GetResult();
            if (!putResponse.IsSuccessStatusCode)
            {
                var content = putResponse.Content.ReadAsStringAsync().GetAwaiter().GetResult();
                throw new Exception(string.Format("Error changing throughput scheme: '{0}'.\nContent: {1} \n StatusCode: '{2}'", putResponse.ReasonPhrase, content, putResponse.StatusCode));
            }

            object createOffer()
            {
                // Read the ResourceRID using reflection because the property is protected.
                string resourceRID = currentThroughput.GetType()
                    .GetProperty("ResourceRID", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(currentThroughput).ToString();
                string resourceLink = scalableItemLink;
                object content;
                if (changeToAutoScale)
                {
                    content = new
                    {
                        offerThroughput = -1
                    };
                }
                else
                {
                    content = new
                    {
                        offerAutopilotSettings = new { maxThroughput = -1 }
                    };
                }

                return new
                {
                    offerVersion = "V2",
                    offerType = "Invalid",
                    content = content,
                    resource = resourceLink,
                    offerResourceId = resourceRID,
                    id = offerId,
                    _rid = offerId,
                };
            }

            string extractAuthKey()
            {
                // Read the AccountKey using reflection because the property is protected.
                return client.GetType()
                    .GetProperty("AccountKey", BindingFlags.NonPublic | BindingFlags.Instance)
                    .GetValue(client).ToString();
            }
        }

        private string GenerateContainerLink(Container container) => string.Format("dbs/{0}/colls/{1}/", container.Database.Id, container.Id);

        /// <summary>
        /// Generate the HTTP authorization header value needed to connect with Cosmos DB
        /// </summary>
        /// <param name="method">The Verb portion of the string is the HTTP verb, such as GET, POST, or PUT.</param>
        /// <param name="resourceType">The ResourceType portion of the string identifies the type of resource that the request is for, Eg. "dbs", "colls", "docs".</param>
        /// <param name="resourceLink">The ResourceLink portion of the string is the identity property of the resource that the request is directed at. ResourceLink must maintain its case for the ID of the resource. Example, for a container it looks like: "dbs/MyDatabase/colls/MyContainer".</param>
        /// <param name="date">The Date portion of the string is the UTC date and time the message was sent (in "HTTP-date" format as defined by RFC 7231 Date/Time Formats), for example, Tue, 01 Nov 1994 08:12:31 GMT. In C#, it can be obtained by using the "R" format specifier on the DateTime.UtcNow value. This same date(in same format) also needs to be passed as x-ms-date header in the request.</param>
        /// <param name="key">Cosmos DB key token (found in the Azure Portal)</param>
        /// <param name="keyType">denotes the type of token: master or resource.</param>
        /// <param name="tokenVersion">denotes the version of the token, currently 1.0.</param>
        /// <returns>auth token string</returns>
        // Source: https://docs.microsoft.com/en-us/rest/api/cosmos-db/access-control-on-cosmosdb-resources
        private string GenerateAuthToken(HttpMethod method, string resourceType, string resourceLink, string date, string key, string keyType = "master", string tokenVersion = "1.0")
        {
            var hmacSha256 = new System.Security.Cryptography.HMACSHA256 { Key = Convert.FromBase64String(key) };

            var verb = method?.Method ?? string.Empty;
            resourceType = resourceType ?? string.Empty;
            resourceLink = resourceLink?.ToLower() ?? string.Empty; // Without ToLower(), we get an 'unauthorized' error.

            string payLoad = string.Format(
                System.Globalization.CultureInfo.InvariantCulture,
                "{0}\n{1}\n{2}\n{3}\n{4}\n",
                verb.ToLowerInvariant(),
                resourceType.ToLowerInvariant(),
                resourceLink,
                date.ToLowerInvariant(),
                string.Empty);

            byte[] hashPayLoad = hmacSha256.ComputeHash(System.Text.Encoding.UTF8.GetBytes(payLoad));
            string signature = Convert.ToBase64String(hashPayLoad);

            return System.Web.HttpUtility.UrlEncode(
                string.Format(
                    System.Globalization.CultureInfo.InvariantCulture,
                    "type={0}&ver={1}&sig={2}",
                    keyType,
                    tokenVersion,
                    signature));
        }
    }
}
