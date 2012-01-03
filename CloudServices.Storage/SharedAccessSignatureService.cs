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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage
{
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Text.RegularExpressions;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Helpers;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Security;
    using Microsoft.WindowsAzure.StorageClient;

    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class SharedAccessSignatureService
    {
        private const SharedAccessPermissions ContainerSharedAccessPermissions = SharedAccessPermissions.Write | SharedAccessPermissions.Delete | SharedAccessPermissions.List | SharedAccessPermissions.Read;

        private const string MetadataHeaderPrefix = "x-ms-meta-";

        private const string BlobPublicLevelHeaderName = "x-ms-blob-public-access";

        private readonly CloudBlobClient cloudBlobClient;

        public SharedAccessSignatureService()
            : this(StorageServicesContext.Current.Configuration.CloudStorageAccount)
        {
        }

        public SharedAccessSignatureService(CloudStorageAccount storageAccount)
        {
            if (storageAccount == null)
                throw new ArgumentNullException("storageAccount", Resource.CloudStorageAccountNullArgumentErrorMessage);

            this.cloudBlobClient = storageAccount.CreateCloudBlobClient();
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebInvoke(Method = "PUT", UriTemplate = "/containers/{containerName}?comp={operation}")]
        public HttpResponseMessage CreateContainer(HttpRequestMessage request, string containerName, string operation)
        {
            if (request == null)
                throw new ArgumentNullException("request", Resource.RequestNullErrorMessage);

            // Determine if the public-access header was sent
            string publicAccessMode = GetPublicAccessMode(request);

            // operation is ACL but headers have not been sent
            if (string.IsNullOrEmpty(publicAccessMode) && !string.IsNullOrEmpty(operation) && operation.Equals("acl", StringComparison.OrdinalIgnoreCase))
                publicAccessMode = "OFF";

            // operation is null and container name is null 
            if (string.IsNullOrWhiteSpace(containerName) && string.IsNullOrWhiteSpace(operation))
                throw WebException(Resource.ContainerNameNullArgumentErrorMessage, HttpStatusCode.BadRequest);

            try
            {
                var responseStatusCode = HttpStatusCode.InternalServerError;
                var container = this.cloudBlobClient.GetContainerReference(containerName);

                UpdateContainerWithMetadataFromRequest(request, container);

                // operation is null --> the create container mode is requested
                if (string.IsNullOrEmpty(operation))
                {
                    responseStatusCode = HttpStatusCode.Created;
                    container.Create();
                }

                // publicAccessMode =! null
                if (!string.IsNullOrEmpty(publicAccessMode))
                {
                    BlobContainerPermissions containerPermissions = GetBlobContainerPermissions(publicAccessMode);
                    container.SetPermissions(containerPermissions);

                    if (responseStatusCode == HttpStatusCode.InternalServerError)
                        responseStatusCode = HttpStatusCode.OK;
                }

                // operation == metadata
                if (!string.IsNullOrEmpty(operation) && operation.Equals("metadata", StringComparison.OrdinalIgnoreCase))
                {
                    responseStatusCode = HttpStatusCode.OK;
                    container.SetMetadata();
                }

                return new HttpResponseMessage { Content = new StringContent(container.Uri.AbsoluteUri), StatusCode = responseStatusCode };
            }
            catch (StorageClientException exception)
            {
                throw WebException(exception.ExtendedErrorInformation.ErrorMessage, exception.StatusCode);
            }
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebInvoke(Method = "DELETE", UriTemplate = "/containers/{containerName}")]
        public HttpResponseMessage DeleteContainer(string containerName)
        {
            try
            {
                var container = this.cloudBlobClient.GetContainerReference(containerName);
                container.Delete();

                return new HttpResponseMessage(HttpStatusCode.Accepted);
            }
            catch (StorageClientException exception)
            {
                throw WebException(exception.Message, exception.StatusCode);
            }
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebInvoke(Method = "HEAD", UriTemplate = "/containers/{containerName}")]
        public HttpResponseMessage GetContainerProperties(string containerName)
        {
            try
            {
                var container = this.cloudBlobClient.GetContainerReference(containerName);
                container.FetchAttributes();

                var result = new HttpResponseMessage(HttpStatusCode.OK);
                result.Headers.ETag = new EntityTagHeaderValue(container.Properties.ETag);
                result.Content = new StringContent(string.Empty);
                result.Content.Headers.LastModified = container.Properties.LastModifiedUtc;

                foreach (var metadataKey in container.Metadata.AllKeys)
                {
                    result.Headers.Add(string.Concat(MetadataHeaderPrefix, metadataKey), container.Metadata[metadataKey]);
                }

                return result;
            }
            catch (StorageClientException exception)
            {
                throw WebException(exception.Message, exception.StatusCode);
            }
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebGet(UriTemplate = "/containers?prefix={containerPrefix}")]
        public HttpResponseMessage<SasCloudBlobContainerListResponse> ListContainers(string containerPrefix)
        {
            IEnumerable<CloudBlobContainer> containers;
            if (!string.IsNullOrEmpty(containerPrefix))
            {
                containerPrefix = containerPrefix.TrimStart('/', '\\').Replace('\\', '/');
                containers = this.cloudBlobClient.ListContainers(containerPrefix);
            }
            else
            {
                containers = this.cloudBlobClient.ListContainers();
            }

            var result = new SasCloudBlobContainerListResponse
            {
                Containers = containers
                                .Where(c => true)
                                .Select(c => c.ToModel())
                                .ToArray()
            };

            return new HttpResponseMessage<SasCloudBlobContainerListResponse>(result, HttpStatusCode.OK);
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebGet(UriTemplate = "/containers/{containerName}?comp={operation}")]
        public HttpResponseMessage GetContainerSharedAccessSignature(string containerName, string operation)
        {
            if (string.IsNullOrEmpty(operation) || !operation.Equals("sas", StringComparison.OrdinalIgnoreCase))
                throw WebException(Resource.CompMustBeSasArgumentErrorMessage, HttpStatusCode.BadRequest);

            var container = this.cloudBlobClient.GetContainerReference(containerName);
            var sas = container.GetSharedAccessSignature(new SharedAccessPolicy
            {
                Permissions = ContainerSharedAccessPermissions,
                SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromMinutes(StorageServicesContext.Current.Configuration.ContainerSasExpirationTime)
            });

            var uriBuilder = new UriBuilder(container.Uri) { Query = sas.TrimStart('?') };
            return new HttpResponseMessage { Content = new StringContent(uriBuilder.Uri.AbsoluteUri), StatusCode = HttpStatusCode.OK };
        }

        [OperationContract]
        [AuthorizeBlobsAccess]
        [CLSCompliant(false)]
        [WebGet(UriTemplate = "/blobs/{containerName}/{*blobName}?comp={operation}")]
        public HttpResponseMessage GetBlobSharedAccessSignature(string containerName, string blobName, string operation)
        {
            if (string.IsNullOrEmpty(operation) || !operation.Equals("sas", StringComparison.OrdinalIgnoreCase))
                throw WebException(Resource.CompMustBeSasArgumentErrorMessage, HttpStatusCode.BadRequest);

            try
            {
                var container = this.cloudBlobClient.GetContainerReference(containerName);

                var containerPermissions = container.GetPermissions();

                if (!containerPermissions.SharedAccessPolicies.ContainsKey("readonly"))
                    SetReadOnlySharedAccessPolicy(container);

                var blob = container.GetBlobReference(blobName);
                var sas = blob.GetSharedAccessSignature(new SharedAccessPolicy(), "readonly");
                var uriBuilder = new UriBuilder(blob.Uri) { Query = sas.TrimStart('?') };

                return new HttpResponseMessage { Content = new StringContent(uriBuilder.Uri.AbsoluteUri), StatusCode = HttpStatusCode.OK };
            }
            catch (StorageClientException exception)
            {
                throw WebException(exception.Message, exception.StatusCode);
            }
        }

        private static void SetReadOnlySharedAccessPolicy(CloudBlobContainer container)
        {
            var permissions = container.GetPermissions();
            var options = new BlobRequestOptions
            {
                // Fail if someone else has already changed the container before we do.
                AccessCondition = AccessCondition.IfMatch(container.Properties.ETag)
            };
            var sharedAccessPolicy = new SharedAccessPolicy
            {
                Permissions = SharedAccessPermissions.Read,
                SharedAccessExpiryTime = DateTime.UtcNow + TimeSpan.FromDays(StorageServicesContext.Current.Configuration.BlobsSasExpirationTime)
            };

            permissions.SharedAccessPolicies.Remove("readonly");
            permissions.SharedAccessPolicies.Add("readonly", sharedAccessPolicy);

            container.SetPermissions(permissions, options);
        }

        private static HttpResponseException WebException(string message, HttpStatusCode code)
        {
            return new HttpResponseException(new HttpResponseMessage(code) { Content = new StringContent(message) });
        }

        private static void UpdateContainerWithMetadataFromRequest(HttpRequestMessage request, CloudBlobContainer container)
        {
            // Extract metadata headers
            var metadataHeaders = request.Headers.Where(h => h.Key.StartsWith(MetadataHeaderPrefix, StringComparison.OrdinalIgnoreCase));
            foreach (var metadataHeader in metadataHeaders)
            {
                var key = Regex.Replace(metadataHeader.Key, MetadataHeaderPrefix, string.Empty, RegexOptions.IgnoreCase);
                var value = metadataHeader.Value.FirstOrDefault();
                if (value != null)
                    container.Metadata.Add(key, value);
            }
        }

        private static BlobContainerPermissions GetBlobContainerPermissions(string publicAccessMode)
        {
            BlobContainerPermissions containerPermissions;
            switch (publicAccessMode.ToUpperInvariant())
            {
                case "CONTAINER":
                    containerPermissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Container };
                    break;
                case "BLOB":
                    containerPermissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Blob };
                    break;
                case "OFF":
                    containerPermissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off };
                    break;
                default:
                    containerPermissions = new BlobContainerPermissions { PublicAccess = BlobContainerPublicAccessType.Off };
                    break;
            }

            return containerPermissions;
        }

        private static string GetPublicAccessMode(HttpRequestMessage request)
        {
            string publicAccessMode = null;
            var publicLevelHeader = request.Headers.FirstOrDefault(h => h.Key.Equals(BlobPublicLevelHeaderName, StringComparison.OrdinalIgnoreCase));

            if (publicLevelHeader.Value != null && publicLevelHeader.Value.Count() > 0)
                publicAccessMode = publicLevelHeader.Value.First();

            return publicAccessMode;
        }
    }
}
