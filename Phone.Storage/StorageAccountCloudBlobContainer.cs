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

    public class StorageAccountCloudBlobContainer : CloudBlobContainerBase
    {
        private Uri uri;

        public StorageAccountCloudBlobContainer(Uri uri, string name, IStorageCredentials credentials)
            : this(uri, name, credentials, null)
        {
        }

        public StorageAccountCloudBlobContainer(Uri uri, string name, IStorageCredentials credentials, Dispatcher dispatcher)
            : base(name, dispatcher)
        {
            if (uri == null)
                throw new ArgumentNullException("uri", "The container uri cannot be null.");

            if (credentials == null)
                throw new ArgumentNullException("credentials", "The credentials cannot be null.");

            this.uri = uri;
            this.Credentials = credentials;
        }

        public override Uri Uri
        {
            get
            {
                return this.uri;
            }

            internal set
            {
                this.uri = value;
            }
        }

        public override void Create(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.Uri, string.Empty, "restype=container&timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Set the permissions for the specified container.
            request.Headers.AddContainerPublicAccessTypeHeader(publicAccessType);

            // Add metadata for the specified container.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(request, HttpStatusCode.Created, r => true, callback);
        }

        public override void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.Uri, string.Empty, "restype=container&timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "DELETE";

            this.SendRequest(request, HttpStatusCode.Accepted, r => true, callback);
        }

        public override void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            var queryString = string.Concat("restype=container&timeout=10000&", GenerateIncrementalSeedQueryString());
            var requestUri = BuildUri(this.Uri, string.Empty, queryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(
                request,
                HttpStatusCode.OK,
                CloudBlobContainerAttributesResponseMapper,
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
            var requestUri = BuildUri(this.Uri, string.Empty, "restype=container&comp=metadata&timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Add metadata for the specified container.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(request, HttpStatusCode.OK, r => true, callback);
        }

        public override void SetPermissions(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.Uri, string.Empty, "restype=container&comp=acl&timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Set the permissions for the specified container.
            request.Headers.AddContainerPublicAccessTypeHeader(publicAccessType);

            this.SendRequest(request, HttpStatusCode.OK, r => true, callback);
        }

        public override void ListBlobs(string blobPrefix, bool flatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback)
        {
            this.InnerListBlobs(this.Uri, blobPrefix, flatBlobListing, callback);
        }

        public override ICloudBlob GetBlobReference(string blobName)
        {
            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("The blob name cannot be null, empty or white space.", "blobName");

            var blobUri = new Uri(string.Concat(this.Uri.AbsoluteUri.TrimEnd('/'), "/", blobName.TrimStart('/')));
            return new StorageAccountCloudBlob(blobUri, blobName, this);
        }

        public override void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback)
        {
            throw new NotSupportedException("This operation is not supported in this implementation.");
        }

        protected override IEnumerable<ICloudBlob> CloudBlobsMapper(HttpWebResponse response)
        {
            if (response == null)
                return new List<ICloudBlob>();

            var serializer = new DataContractSerializer(typeof(InnerCloudBlobListResponse));
            var results = (InnerCloudBlobListResponse)serializer.ReadObject(response.GetResponseStream());

            return results.Blobs
                .Select(b => new StorageAccountCloudBlob(b.Uri, b.Name, this) { Properties = b.Properties.ToBlobProperties() })
                .Cast<ICloudBlob>()
                .ToList();
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

        private static BlobContainerAttributes CloudBlobContainerAttributesResponseMapper(HttpWebResponse response)
        {
            return new BlobContainerAttributes
            {
                Properties = new BlobContainerProperties { ETag = response.Headers.GetValue("ETag"), LastModifiedUtc = Helpers.ParseDateTimeUtc(response.Headers.GetValue("Last-Modified")) },
                Metadata = response.Headers.GetMetadataHeaders()
            };
        }
    }
}
