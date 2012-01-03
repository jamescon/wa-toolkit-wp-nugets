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

    public abstract class CloudBlobContainerBase : CloudClientBase, ICloudBlobContainer
    {
        protected CloudBlobContainerBase(string name)
            : this(name, null)
        {
        }

        protected CloudBlobContainerBase(string name, Dispatcher dispatcher)
            : base(dispatcher)
        {
            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "name");

            this.Name = name;
            this.Metadata = new Dictionary<string, string>();
            this.Properties = new BlobContainerProperties();
        }

        public virtual Uri Uri { get; internal set; }

        public string Name { get; internal set; }

        public IDictionary<string, string> Metadata { get; internal set; }

        public BlobContainerProperties Properties { get; internal set; }

        public IStorageCredentials Credentials { get; internal set; }

        public virtual void Create(Action<CloudOperationResponse<bool>> callback)
        {
            this.Create(BlobContainerPublicAccessType.Off, callback);
        }

        public abstract void Create(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback);

        public virtual void CreateIfNotExist(Action<CloudOperationResponse<bool>> callback)
        {
            this.FetchAttributes(
                r =>
                {
                    if (r.Success)
                        callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, false, r.ErrorMessage));
                    else if (r.StatusCode == HttpStatusCode.NotFound)
                        this.Create(callback);
                    else
                        callback(r);
                });
        }

        public virtual void CreateIfNotExist(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback)
        {
            this.FetchAttributes(
                r =>
                {
                    if (r.Success)
                        this.SetPermissions(publicAccessType, callback);
                    else if (r.StatusCode == HttpStatusCode.NotFound)
                        this.Create(publicAccessType, callback);
                    else
                        callback(r);
                });
        }

        public abstract void Delete(Action<CloudOperationResponse<bool>> callback);

        public abstract void FetchAttributes(Action<CloudOperationResponse<bool>> callback);

        public abstract void SetMetadata(Action<CloudOperationResponse<bool>> callback);

        public abstract void SetPermissions(BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<bool>> callback);

        public virtual void ListBlobs(Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback)
        {
            this.ListBlobs(string.Empty, false, callback);
        }

        public abstract void ListBlobs(string blobPrefix, bool flatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback);

        public abstract ICloudBlob GetBlobReference(string blobName);

        public abstract void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback);

        protected void InnerListBlobs(Uri containerUri, string blobPrefix, bool flatBlobListing, Action<CloudOperationResponse<IEnumerable<ICloudBlob>>> callback)
        {
            var queryString = "restype=container&comp=list&timeout=10000";

            if (!flatBlobListing)
                queryString = string.Concat(queryString, "&delimiter=", HttpUtility.UrlEncode("/"));

            if (!string.IsNullOrWhiteSpace(blobPrefix))
                queryString = string.Concat(queryString, "&prefix=", HttpUtility.UrlEncode(blobPrefix.Replace(@"\", "/")));

            // Adding an incremental seed to avoid a cached response.
            queryString = string.Concat(queryString, "&", GenerateIncrementalSeedQueryString());

            var requestUri = BuildUri(containerUri, string.Empty, queryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, this.CloudBlobsMapper, callback);
        }

        protected abstract IEnumerable<ICloudBlob> CloudBlobsMapper(HttpWebResponse response);
    }
}
