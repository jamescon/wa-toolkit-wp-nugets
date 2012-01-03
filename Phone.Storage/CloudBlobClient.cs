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
    using System.Windows.Threading;

    public class CloudBlobClient : ICloudBlobClient
    {
        private readonly ICloudBlobClient strategy;

        /// <summary>
        /// Initializes a new instance of the CloudBlobClient class using Shared Access Signatures to perform operations.
        /// </summary>
        /// <param name="sasServiceClient">The Shared Access Signature service client.</param>
        public CloudBlobClient(ISharedAccessSignatureServiceClient sasServiceClient)
        {
            if (sasServiceClient == null)
                throw new ArgumentNullException("sasServiceClient", "The Shared Access Signature service client cannot be null.");

            this.strategy = new InnerSharedAccessSignatureServiceCloudBlobClientStrategy(sasServiceClient);
            this.Dispatcher = sasServiceClient.Dispatcher;
        }

        /// <summary>
        /// Initializes a new instance of the CloudBlobClient class using the Windows Azure Storage Account credentials to perform operations.
        /// </summary>
        /// <param name="blobsBaseUri">The blobs endpoint.</param>
        /// <param name="credentials">The storage credentials.</param>
        public CloudBlobClient(Uri blobsBaseUri, IStorageCredentials credentials)
            : this(blobsBaseUri, credentials, null)
        {
        }

        /// <summary>
        /// Initializes a new instance of the CloudBlobClient class using the Windows Azure Storage Account credentials to perform operations.
        /// </summary>
        /// <param name="blobsBaseUri">The blobs endpoint.</param>
        /// <param name="credentials">The storage credentials.</param>
        /// <param name="dispatcher">The dispatcher used to invoke the callbacks.</param>
        public CloudBlobClient(Uri blobsBaseUri, IStorageCredentials credentials, Dispatcher dispatcher)
        {
            if (blobsBaseUri == null)
                throw new ArgumentNullException("blobsBaseUri", "The Blobs base uri cannot be null.");

            if (credentials == null)
                throw new ArgumentNullException("credentials", "The storage credentials cannot be null.");

            this.Credentials = credentials;
            this.Dispatcher = dispatcher;
            this.strategy = new InnerStorageAccountCloudBlobClientStrategy(blobsBaseUri, this.Credentials, dispatcher);
        }

        public IStorageCredentials Credentials { get; internal set; }

        public Dispatcher Dispatcher { get; internal set; }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            return this.strategy.GetContainerReference(containerName);
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.strategy.ListContainers(callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.strategy.ListContainers(prefix, callback);
        }
    }
}