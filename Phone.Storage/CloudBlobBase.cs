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
    using System.Net;
    using System.Windows.Threading;

    public abstract class CloudBlobBase : CloudClientBase, ICloudBlob
    {
        private readonly Uri uri;

        protected CloudBlobBase(Uri uri, string name, ICloudBlobContainer container)
        {
            if (uri == null)
                throw new ArgumentNullException("uri", "The blob uri cannot be null.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The blob name cannot be null, empty or white space.", "name");

            if (container == null)
                throw new ArgumentNullException("container", "The container cannot be null.");

            this.uri = uri;
            this.Name = name;
            this.Container = container;
            this.Metadata = new Dictionary<string, string>();
            this.Properties = new BlobProperties();
        }

        public string Name { get; internal set; }

        public virtual Uri Uri
        {
            get
            {
                return this.uri;
            }
        }

        public ICloudBlobContainer Container { get; internal set; }

        public IDictionary<string, string> Metadata { get; internal set; }

        public BlobProperties Properties { get; internal set; }

        public override Dispatcher Dispatcher
        {
            get
            {
                if (this.Container == null)
                    return null;

                return this.Container.Dispatcher;
            }
        }

        protected abstract Uri BaseOperationUri { get; }

        protected IStorageCredentials Credentials
        {
            get
            {
                if (this.Container == null)
                    return null;

                return this.Container.Credentials;
            }
        }

        public virtual void UploadFromStream(Stream source, Action<CloudOperationResponse<bool>> callback)
        {
            var uploader = new CloudBlobUploader(source, this.BaseOperationUri, this.Metadata, this.Properties, this.Credentials, this.Dispatcher);

            uploader.Upload(callback);
        }

        public virtual void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.BaseOperationUri, string.Empty, "timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "DELETE";

            this.SendRequest(request, HttpStatusCode.Accepted, r => true, callback);
        }

        public virtual void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.BaseOperationUri, string.Empty, "timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "HEAD";

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

        public virtual void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            var requestUri = BuildUri(this.BaseOperationUri, string.Empty, "comp=metadata&timeout=10000");
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Add metadata for the specified container.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(request, HttpStatusCode.OK, r => true, callback);
        }

        public abstract void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback);

        private static BlobAttributes CloudBlobContainerAttributesResponseMapper(HttpWebResponse response)
        {
            return new BlobAttributes
            {
                Properties = new BlobProperties
                    {
                        BlobType = Helpers.PaseEnum<BlobType>(response.Headers.GetValue("x-ms-blob-type")),
                        CacheControl = response.Headers.GetValue("Cache-Control"),
                        ContentEncoding = response.Headers.GetValue("Content-Encoding"),
                        ContentLanguage = response.Headers.GetValue("Content-Language"),
                        ContentMD5 = response.Headers.GetValue("Content-MD5"),
                        ContentType = response.ContentType,
                        ETag = response.Headers.GetValue("Etag"),
                        LastModifiedUtc = Helpers.ParseDateTimeUtc(response.Headers.GetValue("Last-Modified")),
                        LeaseStatus = Helpers.PaseEnum<LeaseStatus>(response.Headers.GetValue("x-ms-lease-status")),
                        Length = response.ContentLength
                    },
                Metadata = response.Headers.GetMetadataHeaders()
            };
        }
    }
}
