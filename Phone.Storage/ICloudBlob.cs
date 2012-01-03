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
    using System.IO;
    using System.Windows.Threading;

    /// <summary>
    /// Represents the a cloud blob.
    /// Modelled after the interface from the Microsoft.WindowsAzure.StorageClient
    /// http://msdn.microsoft.com/en-us/library/microsoft.windowsazure.storageclient.cloudblob_members.aspx
    /// </summary>
    public interface ICloudBlob
    {
        string Name { get; }

        Uri Uri { get; }

        ICloudBlobContainer Container { get; }

        IDictionary<string, string> Metadata { get; }

        BlobProperties Properties { get; }

        Dispatcher Dispatcher { get; }

        void UploadFromStream(Stream source, Action<CloudOperationResponse<bool>> callback);

        void Delete(Action<CloudOperationResponse<bool>> callback);

        void FetchAttributes(Action<CloudOperationResponse<bool>> callback);

        void SetMetadata(Action<CloudOperationResponse<bool>> callback);

        void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback);
    }
}
