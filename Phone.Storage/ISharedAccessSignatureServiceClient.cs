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

    public interface ISharedAccessSignatureServiceClient
    {
        Dispatcher Dispatcher { get; }

        void CreateContainer(string containerName, Action<CloudOperationResponse<Uri>> callback);

        void CreateContainer(string containerName, BlobContainerPublicAccessType publicAccessType, IDictionary<string, string> metadata, Action<CloudOperationResponse<Uri>> callback);

        void DeleteContainer(string containerName, Action<CloudOperationResponse<bool>> callback);

        void SetContainerPublicAccessLevel(string containerName, BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<Uri>> callback);

        void SetContainerMetadata(string containerName, IDictionary<string, string> metadata, Action<CloudOperationResponse<Uri>> callback);

        void GetContainerAttributes(string containerName, Action<CloudOperationResponse<BlobContainerAttributes>> callback);

        void ListContainers(string containerPrefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback);

        void GetContainerSharedAccessSignature(string containerName, Action<CloudOperationResponse<Uri>> callback);

        void GetBlobSharedAccessSignature(string containerName, string blobName, Action<CloudOperationResponse<Uri>> callback);
    }
}
