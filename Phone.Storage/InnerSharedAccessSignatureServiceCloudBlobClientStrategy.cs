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

    internal class InnerSharedAccessSignatureServiceCloudBlobClientStrategy : ICloudBlobClient
    {
        private readonly ISharedAccessSignatureServiceClient sasServiceClient;

        public InnerSharedAccessSignatureServiceCloudBlobClientStrategy(ISharedAccessSignatureServiceClient sasServiceClient)
        {
            this.sasServiceClient = sasServiceClient;
        }

        public IStorageCredentials Credentials { get; internal set; }

        public Dispatcher Dispatcher
        {
            get
            {
                if (this.sasServiceClient == null)
                    return null;

                return this.sasServiceClient.Dispatcher;
            }
        }

        public ICloudBlobContainer GetContainerReference(string containerName)
        {
            return new SharedAccessSignatureServiceCloudBlobContainer(containerName, this.sasServiceClient);
        }

        public void ListContainers(Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.sasServiceClient.ListContainers(null, callback);
        }

        public void ListContainers(string prefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            this.sasServiceClient.ListContainers(prefix, callback);
        }
    }
}
