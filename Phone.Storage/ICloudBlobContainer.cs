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

    /// <summary>
    /// Represents the a cloud blob container.
    /// Modelled after the interface from the Microsoft.WindowsAzure.StorageClient
    /// http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudblobcontainer_members.aspx
    /// </summary>
    public interface ICloudBlobContainer
    {
        Uri Uri { get; }

        string Name { get; }

        IDictionary<string, string> Metadata { get; }

        BlobContainerProperties Properties { get; }

        IStorageCredentials Credentials { get; }

        Dispatcher Dispatcher { get; }

        void Create(Action<CloudOperationResponse<bool>> callback);

        void Create(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback);

        void CreateIfNotExist(Action<CloudOperationResponse<bool>> callback);

        void CreateIfNotExist(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback);

        void Delete(Action<CloudOperationResponse<bool>> callback);

        void FetchAttributes(Action<CloudOperationResponse<bool>> callback);

        void SetMetadata(Action<CloudOperationResponse<bool>> callback);

        void SetPermissions(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback);

        void ListBlobs(Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback);

        void ListBlobs(string blobPrefix, bool flatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback);

        ICloudBlob GetBlobReference(string blobName);

        void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback);
    }
}
