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
    using System.Runtime.Serialization.Json;
    using System.Windows.Threading;

    public class SharedAccessSignatureServiceClient : CloudClientBase, ISharedAccessSignatureServiceClient
    {
        private const string ContainersOperation = "containers";
        private const string BlobsOperation = "blobs";
        private const string ContainerPublicAccessLevelQueryString = "comp=acl";
        private const string MetadataQueryString = "comp=metadata";
        private const string SasQueryString = "comp=sas";
        private const string PrefixQueryString = "prefix=";

        private readonly Uri sasServiceEndpoint;
        private readonly Action<WebRequest> signRequestDelegate;

        public SharedAccessSignatureServiceClient(Uri sasServiceBaseUri)
            : this(sasServiceBaseUri, null, null)
        {
        }

        public SharedAccessSignatureServiceClient(Uri sasServiceEndpoint, Action<WebRequest> signRequestDelegate, Dispatcher dispatcher)
            : base(dispatcher)
        {
            if (sasServiceEndpoint == null)
                throw new ArgumentNullException("sasServiceEndpoint", "The Shared Access Signature service endpoint cannot be null.");

            this.sasServiceEndpoint = sasServiceEndpoint;
            this.signRequestDelegate = signRequestDelegate;
        }

        public void CreateContainer(string containerName, Action<CloudOperationResponse<Uri>> callback)
        {
            this.CreateContainer(containerName, BlobContainerPublicAccessType.Off, null, callback);
        }

        public void CreateContainer(string containerName, BlobContainerPublicAccessType publicAccessType, IDictionary<string, string> metadata, Action<CloudOperationResponse<Uri>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                string.Empty);
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Set the permissions for the specified container.
            request.Headers.AddContainerPublicAccessTypeHeader(publicAccessType);

            // Add metadata for the specified container.
            request.Headers.AddMetadataHeaders(metadata);

            this.SendRequest(request, HttpStatusCode.Created, UriResponseMapper, callback);
        }

        public void DeleteContainer(string containerName, Action<CloudOperationResponse<bool>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                string.Empty);
            var request = this.ResolveRequest(requestUri);

            request.Method = "DELETE";

            this.SendRequest(request, HttpStatusCode.Accepted, r => true, callback);
        }

        public void SetContainerPublicAccessLevel(string containerName, BlobContainerPublicAccessType publicAccessType, Action<CloudOperationResponse<Uri>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                ContainerPublicAccessLevelQueryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Set the permissions for the specified container.
            request.Headers.AddContainerPublicAccessTypeHeader(publicAccessType);

            this.SendRequest(request, HttpStatusCode.OK, UriResponseMapper, callback);
        }

        public void SetContainerMetadata(string containerName, IDictionary<string, string> metadata, Action<CloudOperationResponse<Uri>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                MetadataQueryString);
            var request = this.ResolveRequest(requestUri);

            request.Method = "PUT";

            // Add metadata for the specified container.
            request.Headers.AddMetadataHeaders(metadata);

            this.SendRequest(request, HttpStatusCode.OK, UriResponseMapper, callback);
        }

        public void GetContainerAttributes(string containerName, Action<CloudOperationResponse<BlobContainerAttributes>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                GenerateIncrementalSeedQueryString());
            var request = this.ResolveRequest(requestUri);

            request.Method = "HEAD";

            this.SendRequest(request, HttpStatusCode.OK, CloudBlobContainerAttributesResponseMapper, callback);
        }

        public void ListContainers(string containerPrefix, Action<CloudOperationResponse<IEnumerable<ICloudBlobContainer>>> callback)
        {
            var queryString = !string.IsNullOrWhiteSpace(containerPrefix)
                ? string.Concat(PrefixQueryString, HttpUtility.UrlEncode(containerPrefix.Replace(@"\", "/")))
                : string.Empty;

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation),
                !string.IsNullOrWhiteSpace(queryString) ? string.Concat(queryString, "&", GenerateIncrementalSeedQueryString()) : GenerateIncrementalSeedQueryString());
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";
            request.Accept = "application/json";

            this.SendRequest(request, HttpStatusCode.OK, this.CloudBlobContainersMapper, callback);
        }

        public void GetContainerSharedAccessSignature(string containerName, Action<CloudOperationResponse<Uri>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(ContainersOperation, containerName),
                string.Concat(SasQueryString, "&", GenerateIncrementalSeedQueryString()));
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, UriResponseMapper, callback);
        }

        public void GetBlobSharedAccessSignature(string containerName, string blobName, Action<CloudOperationResponse<Uri>> callback)
        {
            if (string.IsNullOrWhiteSpace(containerName))
                throw new ArgumentException("The container name cannot be null, empty or white space.", "containerName");

            if (string.IsNullOrWhiteSpace(blobName))
                throw new ArgumentException("The blob name cannot be null, empty or white space.", "blobName");

            var requestUri = BuildUri(
                this.sasServiceEndpoint,
                BuilUriPath(BlobsOperation, containerName, blobName),
                string.Concat(SasQueryString, "&", GenerateIncrementalSeedQueryString()));
            var request = this.ResolveRequest(requestUri);

            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, UriResponseMapper, callback);
        }

        protected override void OnSendingRequest(HttpWebRequest request, int contentLength)
        {
            // Sign the request to add the authorization header (if necessary).
            if (this.signRequestDelegate != null)
                this.signRequestDelegate(request);
        }

        private static BlobContainerAttributes CloudBlobContainerAttributesResponseMapper(HttpWebResponse response)
        {
            return new BlobContainerAttributes
                {
                    Properties = new BlobContainerProperties { ETag = response.Headers.GetValue("ETag"), LastModifiedUtc = Helpers.ParseDateTimeUtc(response.Headers.GetValue("Last-Modified")) },
                    Metadata = response.Headers.GetMetadataHeaders()
                };
        }

        private static Uri UriResponseMapper(HttpWebResponse response)
        {
            var uri = response.GetResponseString();

            return new Uri(uri, UriKind.Absolute);
        }

        private IEnumerable<ICloudBlobContainer> CloudBlobContainersMapper(HttpWebResponse response)
        {
            var serializer = new DataContractJsonSerializer(typeof(InnerCloudBlobContainerListResponse));

            return ((InnerCloudBlobContainerListResponse)serializer.ReadObject(response.GetResponseStream()))
                .Containers
                .Select(c => new SharedAccessSignatureServiceCloudBlobContainer(c.Name, this) { Uri = c.Uri, Properties = c.Properties.ToBlobContainerProperties() })
                .Cast<ICloudBlobContainer>();
        }
    }
}
