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

    internal class InnerStorageAccountCloudBlobClientStrategy : CloudClientBase, ICloudBlobClient
    {
        private readonly Uri blobsBaseUri;

        public InnerStorageAccountCloudBlobClientStrategy(Uri blobsBaseUri, IStorageCredentials credentials, Dispatcher dispatcher)
            : base(dispatcher)
        {
            this.blobsBaseUri = blobsBaseUri;
            this.Credentials = credentials;
        }

        public IStorageCredentials Credentials { get; internal set; }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var containerUri = new Uri(string.Concat(this.blobsBaseUri.AbsoluteUri.TrimEnd('/'), "/", containerName.TrimStart('/')));
            return new StorageAccountCloudBlobContainer(containerUri, containerName, this.Credentials, this.Dispatcher);
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.ListContainers(string.Empty, callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            var queryString = "comp=list&timeout=10000";

            if (!string.IsNullOrWhiteSpace(prefix))
                queryString = string.Concat(queryString, "&prefix=", HttpUtility.UrlEncode(prefix.Replace(@"\", "/")));

            // Adding an incremental seed to avoid a cached response.
            queryString = string.Concat(queryString, "&", GenerateIncrementalSeedQueryString());

            var requestUri = BuildUri(this.blobsBaseUri, string.Empty, queryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, this.CloudBlobContainersMapper, callback);
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

        private IEnumerable<ICloudBlobContainer> CloudBlobContainersMapper(HttpWebResponse response)
        {
            var serializer = new DataContractSerializer(typeof(InnerCloudBlobContainerListResponse));

            return ((InnerCloudBlobContainerListResponse)serializer.ReadObject(response.GetResponseStream()))
                .Containers
                .Select(c => new StorageAccountCloudBlobContainer(c.Uri, c.Name, this.Credentials, this.Dispatcher) { Properties = c.Properties.ToBlobContainerProperties() })
                .Cast<ICloudBlobContainer>();
        }
    }
}
