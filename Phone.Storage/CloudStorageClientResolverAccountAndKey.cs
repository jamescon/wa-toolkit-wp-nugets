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
    using System.Windows;
    using System.Windows.Threading;

    public class CloudStorageClientResolverAccountAndKey : ICloudStorageClientResolver
    {
        private readonly Uri blobEndpoint;
        private readonly Uri queuesBaseUri;
        private readonly Uri tablesBaseUri;
        private readonly Dispatcher dispatcher;

        private static ICloudStorageClientResolver devStoreAccountResolver;

        public CloudStorageClientResolverAccountAndKey(IStorageCredentials credentials, Uri blobEndpoint, Uri queueEndpoint, Uri tableEndpoint)
            : this(credentials, tableEndpoint, queueEndpoint, blobEndpoint, null)
        {
        }

        public CloudStorageClientResolverAccountAndKey(IStorageCredentials credentials, Uri blobEndpoint, Uri queueEndpoint, Uri tableEndpoint, Dispatcher dispatcher)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials", "The storage credentials cannot be null.");

            if (blobEndpoint == null)
                throw new ArgumentNullException("blobEndpoint", "The Blob endpoint cannot be null.");

            if (queueEndpoint == null)
                throw new ArgumentNullException("queueEndpoint", "The Queues endpoint cannot be null.");

            if (tableEndpoint == null)
                throw new ArgumentNullException("tableEndpoint", "The Tables endpoint cannot be null.");

            this.Credentials = credentials;
            this.blobEndpoint = blobEndpoint;
            this.queuesBaseUri = queueEndpoint;
            this.tablesBaseUri = tableEndpoint;
            this.dispatcher = dispatcher;
        }

        [CLSCompliant(false)]
        public static ICloudStorageClientResolver DevelopmentStorageAccountResolver
        {
            get
            {
                return devStoreAccountResolver
                       ??
                       (devStoreAccountResolver =
                        new CloudStorageClientResolverAccountAndKey(
                            new StorageCredentialsAccountAndKey(
                            "devstoreaccount1",
                            "Eby8vdM02xNOcqFlqUwJPLlmEtlCDXJ1OUzFT50uSRZ6IFsuFq2UVErCz4I6tq/K1SZFPTOtr/KBHBeksoGMGw=="),
                            new Uri("http://127.0.0.1:10000/devstoreaccount1"),
                            new Uri("http://127.0.0.1:10001/devstoreaccount1"),
                            new Uri("http://127.0.0.1:10002/devstoreaccount1"),
                            Deployment.Current.Dispatcher));
            }
        }

        public IStorageCredentials Credentials { get; set; }

        [CLSCompliant(false)]
        public TableServiceContext CreateTableServiceContext()
        {
            return new TableServiceContext(this.tablesBaseUri, this.Credentials);
        }

        [CLSCompliant(false)]
        public ICloudTableClient CreateCloudTableClient()
        {
            return new CloudTableClient(this.tablesBaseUri, this.Credentials, this.dispatcher);
        }

        public ICloudQueueClient CreateCloudQueueClient()
        {
            return new CloudQueueClient(this.queuesBaseUri, this.Credentials, this.dispatcher);
        }

        public ICloudBlobClient CreateCloudBlobClient()
        {
            return new CloudBlobClient(this.blobEndpoint, this.Credentials, this.dispatcher);
        }
    }
}
