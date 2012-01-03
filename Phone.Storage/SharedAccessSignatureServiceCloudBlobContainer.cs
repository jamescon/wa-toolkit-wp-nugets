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

    public class SharedAccessSignatureServiceCloudBlobContainer : CloudBlobContainerBase
    {
        private Uri containerSharedAccessSignatureUri;

        public SharedAccessSignatureServiceCloudBlobContainer(string name, ISharedAccessSignatureServiceClient sasServiceClient)
            : base(name)
        {
            if (sasServiceClient == null)
                throw new ArgumentNullException("sasServiceClient", "The Shared Access Signature service client cannot be null.");

            this.SharedAccessSignatureServiceClient = sasServiceClient;
        }

        public override Dispatcher Dispatcher
        {
            get
            {
                if (this.SharedAccessSignatureServiceClient == null)
                    return null;

                return this.SharedAccessSignatureServiceClient.Dispatcher;
            }
        }

        public ISharedAccessSignatureServiceClient SharedAccessSignatureServiceClient { get; internal set; }

        public override void Create(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback)
        {
            this.SharedAccessSignatureServiceClient.CreateContainer(
                this.Name,
                publicAccessType,
                this.Metadata,
                r =>
                {
                    if (r.Success)
                        this.Uri = r.Response;

                    callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, r.Success, r.ErrorMessage));
                });
        }

        public override void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            this.SharedAccessSignatureServiceClient.DeleteContainer(this.Name, callback);
        }

        public override void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            this.SharedAccessSignatureServiceClient.GetContainerAttributes(
                this.Name,
                r =>
                {
                    if (r.Success)
                    {
                        this.Metadata = r.Response.Metadata;
                        this.Properties = r.Response.Properties;
                    }

                    callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, r.Success, r.ErrorMessage));
                });
        }

        public override void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            this.SharedAccessSignatureServiceClient.SetContainerMetadata(
                this.Name,
                this.Metadata,
                r =>
                {
                    if (r.Success)
                        this.Uri = r.Response;

                    callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, r.Success, r.ErrorMessage));
                });
        }

        public override void SetPermissions(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback)
        {
            this.SharedAccessSignatureServiceClient.SetContainerPublicAccessLevel(
                this.Name,
                publicAccessType,
                r =>
                {
                    if (r.Success)
                        this.Uri = r.Response;

                    callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, r.Success, r.ErrorMessage));
                });
        }

        public override void ListBlobs(string blobPrefix, bool flatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback)
        {
            this.ExecuteWithSas<IEnumerable<ICloudBlob>>(
                () => this.InnerListBlobs(this.containerSharedAccessSignatureUri, blobPrefix, flatBlobListing, callback),
                r => this.DispatchCallback(callback, r));
        }

        public override ICloudBlob GetBlobReference(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("The blob name cannot be null, empty or white space.", "blobName");

            var containerUri = (this.Uri ?? this.containerSharedAccessSignatureUri) ?? new Uri("http://tempuri");

            // Remove the SAS query string from the Uri.
            containerUri = new UriBuilder(containerUri) { Query = string.Empty }.Uri;

            var blobUri = new Uri(string.Concat(containerUri.AbsoluteUri.TrimEnd('/'), "/", blobName.TrimStart('/')));
            return new SharedAccessSignatureServiceCloudBlob(blobUri, blobName, this, this.SharedAccessSignatureServiceClient);
        }

        public override void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback)
        {
            this.SharedAccessSignatureServiceClient.GetContainerSharedAccessSignature(this.Name, callback);
        }

        protected override IEnumerable<ICloudBlob> CloudBlobsMapper(HttpWebResponse response)
        {
            if (response == null)
                return new List<ICloudBlob>();

            var serializer = new DataContractSerializer(typeof(InnerCloudBlobListResponse));
            var results = (InnerCloudBlobListResponse)serializer.ReadObject(response.GetResponseStream());
            
            return results.Blobs
                .Select(b => new SharedAccessSignatureServiceCloudBlob(b.Uri, b.Name, this, this.SharedAccessSignatureServiceClient) { Properties = b.Properties.ToBlobProperties() })
                .Cast<ICloudBlob>()
                .ToList();
        }

        protected override void OnSendingRequest(HttpWebRequest request, int contentLength)
        {
            if (request != null)
            {
                // Add x-ms-version header.
                request.Headers["x-ms-version"] = "2011-08-18";
            }
        }

        private void ExecuteWithSas<T>(Action validSasCallback, Action<CloudOperationResponse<T>> invalidSasCallback)
        {
            if (!this.containerSharedAccessSignatureUri.IsSasExpired())
                validSasCallback();
            else
            {
                this.GetSharedAccessSignature(
                    r =>
                    {
                        if (r.Success)
                        {
                            this.containerSharedAccessSignatureUri = r.Response;
                            validSasCallback();
                        }
                        else
                            invalidSasCallback(new CloudOperationResponse<T>(r.Success, r.StatusCode, default(T), r.ErrorMessage));
                    });
            }
        }
    }
}
