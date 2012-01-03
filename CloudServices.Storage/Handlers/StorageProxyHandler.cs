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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage.Handlers
{
    using System;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text.RegularExpressions;
    using System.Web;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Helpers;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties;

    public abstract class StorageProxyHandler
    {
        public const string RequestedPathPropertyName = "requestedPath";
        private const char UriSeparator = '/';

        protected static CloudStorageAccount CloudStorageAccount
        {
            get
            {
                return StorageServicesContext.Current.Configuration.CloudStorageAccount;
            }
        }

        protected abstract string AzureStorageEndpoint { get; }

        [CLSCompliant(false)]
        public HttpResponseMessage ProcessRequest(HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException("request", Resource.RequestCannotBeNullErrorMessage);

            var originalUri = request.RequestUri;

            var mappedUri =
                new Uri(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        "{0}://{1}/",
                        request.RequestUri.GetComponents(UriComponents.Scheme, UriFormat.SafeUnescaped),
                                            request.Headers.Host));

            var azureResponse = this.GetAzureStorageResponse(this.GetAzureStorageRequestUri(request), request);

            var readTask = azureResponse.Content.ReadAsStringAsync();
            readTask.Wait();

            var azureStorageResponseBody = readTask.Result;

            var headerName = azureResponse.Headers.Where(h => h.Key.Equals("Transfer-Encoding", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(headerName))
                azureResponse.Headers.Remove(headerName);

            if (!string.IsNullOrEmpty(azureStorageResponseBody))
                azureResponse.Content = azureResponse.Content.CloneContent(this.GetProxyResponseBody(azureStorageResponseBody, originalUri, mappedUri, request));
            return azureResponse;
        }

        [CLSCompliant(false)]
        protected string GetAzureStorageRequestBody(string proxyRequestBody, HttpRequestMessage request)
        {
            if (request == null)
                throw new ArgumentNullException("request", Resource.RequestCannotBeNullErrorMessage);

            if (string.IsNullOrWhiteSpace(proxyRequestBody))
            {
                return string.Empty;
            }

            var oldValue = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                request.RequestUri.GetComponents(UriComponents.SchemeAndServer & ~UriComponents.Port, UriFormat.SafeUnescaped),
                request.RequestUri.GetComponents(UriComponents.Path, UriFormat.SafeUnescaped));

            var newValue = this.AzureStorageEndpoint;

            return proxyRequestBody.Replace(oldValue, newValue);
        }

        [CLSCompliant(false)]
        protected string GetProxyResponseBody(string azureStorageResponseBody, Uri originalUri, Uri mappedUri, HttpRequestMessage request)
        {
            if (string.IsNullOrWhiteSpace(azureStorageResponseBody))
            {
                return string.Empty;
            }

            var oldValue = this.AzureStorageEndpoint;
            var newValue = GetLocalPath(originalUri, mappedUri, request);

            return azureStorageResponseBody.Replace(oldValue, newValue);
        }

        [CLSCompliant(false)]
        protected abstract void SignRequest(HttpRequestMessage request);

        private static string GetLocalPath(Uri originalUri, Uri mappedUri, HttpRequestMessage request)
        {
            var fullPath = Regex.Replace(originalUri.AbsolutePath, Regex.Escape(request.Properties[RequestedPathPropertyName].ToString()) + "$", string.Empty);
            return string.Format(CultureInfo.InvariantCulture, "{0}{1}", mappedUri.AbsoluteUri.TrimEnd(UriSeparator), fullPath).TrimEnd(UriSeparator);
        }

        private Uri GetAzureStorageRequestUri(HttpRequestMessage request)
        {
            var resourceName = string.Concat("/", request.Properties[RequestedPathPropertyName]);
            
            var relativeUrl = (request.RequestUri.Query.Length > 0)
                    ? string.Format(CultureInfo.InvariantCulture, "{0}{1}", resourceName, request.RequestUri.Query)
                    : resourceName;

            return new Uri(string.Format(CultureInfo.InvariantCulture, "{0}{1}", this.AzureStorageEndpoint.TrimEnd('/'), relativeUrl), UriKind.Absolute);
        }

        private HttpResponseMessage GetAzureStorageResponse(Uri azureStorageRequestUri, HttpRequestMessage request)
        {
            request.RequestUri = azureStorageRequestUri;

            var azureClient = new HttpClient { MaxResponseContentBufferSize = StorageServicesContext.Current.Configuration.WindowsAzureStorageMaximumResponseSize };
            request.Headers.Host = string.Format(CultureInfo.InvariantCulture, "{0}", azureStorageRequestUri.GetComponents(UriComponents.HostAndPort, UriFormat.SafeUnescaped));

            var readTask = request.Content.ReadAsStringAsync();
            readTask.Wait();

            var azureStorageRequestBody = this.GetAzureStorageRequestBody(readTask.Result, request);

            if (!request.Method.Equals(HttpMethod.Get) &&
                !request.Method.Equals(HttpMethod.Head) &&
                !request.Method.Equals(HttpMethod.Delete) &&
                request.Content.Headers.ContentLength > 0)
            {
                request.Content = new StringContent(azureStorageRequestBody);
                request.Content.Headers.ContentDisposition = request.Content.Headers.ContentDisposition;
                request.Content.Headers.ContentLocation = request.Content.Headers.ContentLocation;
                request.Content.Headers.ContentRange = request.Content.Headers.ContentRange;
                request.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
                request.Content.Headers.Expires = request.Content.Headers.Expires;
                request.Content.Headers.LastModified = request.Content.Headers.LastModified;
            }
            else
            {
                request.Content = null;
            }

            this.SignRequest(request);

            try
            {
                var task = azureClient.SendAsync(request);

                // Synchronize Azure Invokation
                task.Wait();

                return task.Result;
            }
            catch (HttpException webException)
            {
                throw Extensions.StorageException(HttpStatusCode.InternalServerError, Resource.WindowsAzureStorageExceptionStringMessage, webException.Message);
            }
            catch (ObjectDisposedException exception)
            {
                throw Extensions.StorageException(HttpStatusCode.InternalServerError, Resource.HttpClientDisposedErrorString, exception.Message);
            }
        }
    }
}
