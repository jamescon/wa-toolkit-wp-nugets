// ----------------------------------------------------------------------------------
// Microsoft Developer & Platform Evangelism
// 
// Copyright (c) Microsoft Corporation. All rights reserved.
// 
// THIS CODE AND INFORMATION ARE PROVIDED "AS IS" WITHOUT WARRANTY OF ANY KIND, 
// EITHER EXPRESSED OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE IMPLIED WARRANTIES 
// OF MERCHANTABILITY AND/OR FITNESS FOR A PARTICULAR PURPOSE.
// ----------------------------------------------------------------------------------
// The example companies, organizations, products, domain names,
// e-mail addresses, logos, people, places, and events depicted
// herein are fictitious.  No association with any real company,
// organization, product, domain name, email address, logo, person,
// places, or events is intended or should be inferred.
// ----------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Samples.Phone.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Windows.Threading;

    public class CloudQueueClient : CloudClientBase, ICloudQueueClient
    {
        private readonly Uri queuesBaseUri;

        /// <summary>
        /// Initializes a new instance of the CloudQueueClient class.
        /// </summary>
        /// <param name="queuesBaseUri">The Queues endpoint.</param>
        /// <param name="credentials">The storage credentials.</param>
        public CloudQueueClient(Uri queuesBaseUri, IStorageCredentials credentials)
            : this(queuesBaseUri, credentials, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CloudQueueClient class.
        /// </summary>
        /// <param name="queuesBaseUri">The Queues endpoint.</param>
        /// <param name="credentials">The storage credentials.</param>
        /// <param name="dispatcher">The dispatcher used to invoke the callbacks.</param>
        public CloudQueueClient(Uri queuesBaseUri, IStorageCredentials credentials, Dispatcher dispatcher)
            : base(dispatcher)
        {
            if (queuesBaseUri == null)
                throw new ArgumentNullException("queuesBaseUri", "The Queues base uri cannot be null.");

            if (credentials == null)
                throw new ArgumentNullException("credentials", "The storage credentials cannot be null.");

            this.queuesBaseUri = queuesBaseUri;
            this.Credentials = credentials;
        }

        public IStorageCredentials Credentials { get; internal set; }

        public ICloudQueue GetQueueReference(string queueName)
        {
            if (string.IsNullOrWhiteSpace(queueName))
                throw new ArgumentException("The queue name cannot be null, empty or white space.", "queueName");

            var queueUri = new Uri(string.Concat(this.queuesBaseUri.AbsoluteUri.TrimEnd('/'), "/", queueName.TrimStart('/')));
            return new CloudQueue(queueUri, queueName, this.Credentials, this.Dispatcher);
        }

        public void ListQueues(Action<CloudOperationResponse<IEnumerable<ICloudQueue>>> callback)
        {
            this.ListQueues(string.Empty, callback);
        }

        public void ListQueues(string queuePrefix, Action<CloudOperationResponse<IEnumerable<ICloudQueue>>> callback)
        {
            var queryString = "comp=list&timeout=10000";

            if (!string.IsNullOrWhiteSpace(queuePrefix))
                queryString = string.Concat(queryString, "&prefix=", HttpUtility.UrlEncode(queuePrefix));

            // Adding an incremental seed to avoid a cached response.
            queryString = string.Concat(queryString, "&", GenerateIncrementalSeedQueryString());

            var requestUri = BuildUri(this.queuesBaseUri, string.Empty, queryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, this.CloudQueuesMapper, callback);
        }

        protected override void OnSendingRequest(HttpWebRequest request, int contentLength)
        {
            if (request != null)
            {
                // Add x-ms-version header.
                request.Headers["x-ms-version"] = "2011-08-18";

                // Sign the request to add the authorization header.
                this.Credentials.SignRequest(request, contentLength);
            }
        }

        private IEnumerable<ICloudQueue> CloudQueuesMapper(WebResponse response)
        {
            var serializer = new DataContractSerializer(typeof(InnerCloudQueueListResponse));

            return ((InnerCloudQueueListResponse)serializer.ReadObject(response.GetResponseStream()))
                .Queues
                .Select(q => new CloudQueue(q.Uri, q.Name, this.Credentials, this.Dispatcher))
                .Cast<ICloudQueue>();
        }
    }
}
