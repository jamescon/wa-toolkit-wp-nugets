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
    using System.Net;
    using System.Windows.Threading;

    public class CloudStorageClientResolverProxies : ICloudStorageClientResolver
    {
        private readonly Uri sasServiceBaseUri;
        private readonly Uri queuesProxyBaseUri;
        private readonly Uri tablesProxyBaseUri;
        private readonly Dispatcher dispatcher;

        public CloudStorageClientResolverProxies(IStorageCredentials credentials, Uri sasServiceEndpoint, Uri queueProxyEndpoint, Uri tableProxyEndpoint)
            : this(credentials, sasServiceEndpoint, queueProxyEndpoint, tableProxyEndpoint, null)
        {
        }

        public CloudStorageClientResolverProxies(IStorageCredentials credentials, Uri sasServiceEndpoint, Uri queueProxyEndpoint, Uri tableProxyEndpoint, Dispatcher dispatcher)
        {
            if (credentials == null)
                throw new ArgumentNullException("credentials", "The storage credentials cannot be null.");

            if (sasServiceEndpoint == null)
                throw new ArgumentNullException("sasServiceEndpoint", "The Shared Access Signature service endpoint cannot be null.");

            if (queueProxyEndpoint == null)
                throw new ArgumentNullException("queueProxyEndpoint", "The Queues proxy endpoint cannot be null.");

            if (tableProxyEndpoint == null)
                throw new ArgumentNullException("tableProxyEndpoint", "The Table proxy endpoint cannot be null.");

            this.Credentials = credentials;
            this.sasServiceBaseUri = sasServiceEndpoint;
            this.queuesProxyBaseUri = queueProxyEndpoint;
            this.tablesProxyBaseUri = tableProxyEndpoint;
            this.dispatcher = dispatcher;
        }

        public IStorageCredentials Credentials { get; set; }
        
        [CLSCompliant(false)]
        public TableServiceContext CreateTableServiceContext()
        {
            return new TableServiceContext(this.tablesProxyBaseUri, this.Credentials);
        }

        [CLSCompliant(false)]
        public ICloudTableClient CreateCloudTableClient()
        {
            return new CloudTableClient(this.tablesProxyBaseUri, this.Credentials, this.dispatcher);
        }

        public ICloudQueueClient CreateCloudQueueClient()
        {
            return new CloudQueueClient(this.queuesProxyBaseUri, this.Credentials, this.dispatcher);
        }

        public ICloudBlobClient CreateCloudBlobClient()
        {
            return new CloudBlobClient(this.ResolveSharedAccessSignatureServiceClient());
        }

        protected virtual ISharedAccessSignatureServiceClient ResolveSharedAccessSignatureServiceClient()
        {
            Action<WebRequest> signRequestDelegate = r => this.Credentials.SignRequestLite(r);
            return new SharedAccessSignatureServiceClient(
                this.sasServiceBaseUri,
                this.Credentials != null ? signRequestDelegate : null,
                this.dispatcher);
        }
    }
}
