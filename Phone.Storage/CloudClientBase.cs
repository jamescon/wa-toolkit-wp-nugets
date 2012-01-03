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
    using System.Globalization;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Threading;

    public abstract class CloudClientBase
    {
        private readonly Dispatcher dispatcher;

        protected CloudClientBase()
            : this(null)
        {
        }

        protected CloudClientBase(Dispatcher dispatcher)
        {
            this.dispatcher = dispatcher;
        }

        public virtual Dispatcher Dispatcher
        {
            get
            {
                return this.dispatcher;
            }
        }

        internal static Uri BuildUri(Uri baseUri, string path, string queryString)
        {
            var builder = new UriBuilder(baseUri);

            if (!string.IsNullOrWhiteSpace(path))
                builder.Path = string.Concat(builder.Path.TrimEnd('/'), "/", path.TrimStart('/', '\\'));

            if (!string.IsNullOrWhiteSpace(queryString))
                builder.Query = string.IsNullOrWhiteSpace(builder.Query) ? queryString : string.Concat(builder.Query.TrimStart('?'), "&", queryString);

            return builder.Uri;
        }

        internal static Uri BuildUriAndCleanQuery(Uri baseUri, string path)
        {
            var builder = new UriBuilder(baseUri);

            if (!string.IsNullOrWhiteSpace(path))
                builder.Path = string.Concat(builder.Path.TrimEnd('/'), "/", path.TrimStart('/', '\\'));

            builder.Query = string.Empty;

            return builder.Uri;
        }

        internal static string BuilUriPath(params string[] parameters)
        {
            if (parameters == null || parameters.Length == 0)
                return string.Empty;

            var builder = new StringBuilder();
            foreach (var param in parameters)
            {
                builder.Append(HttpUtility.UrlEncode(param.TrimStart('/', '\\').TrimEnd('/', '\\')));
                builder.Append("/");
            }

            // Remove the last slash character ("/").
            builder.Remove(builder.Length - 1, 1);

            return builder.ToString();
        }

        internal static string GenerateIncrementalSeedQueryString()
        {
            return string.Concat("incrementalSeed=", DateTime.UtcNow.Ticks);
        }

        protected virtual void SendRequest<T>(HttpWebRequest request, HttpStatusCode successStatusCode, Func<HttpWebResponse, T> responseMapper, Action<CloudOperationResponse<T>> callback)
        {
            if (request == null)
                throw new ArgumentNullException("request", "The request cannot be null.");

            var contentLength = request.Method.Equals("GET", StringComparison.OrdinalIgnoreCase) ? -1 : 0;
            this.OnSendingRequest(request, contentLength);

            request.BeginGetResponse(
                asyncResult => this.OnGetResponse(asyncResult, successStatusCode, responseMapper, callback),
                request);
        }

        protected virtual void SendRequest<T>(HttpWebRequest request, byte[] body, HttpStatusCode successStatusCode, Func<HttpWebResponse, T> responseMapper, Action<CloudOperationResponse<T>> callback)
        {
            if (request == null)
                throw new ArgumentNullException("request", "The request cannot be null.");

            this.OnSendingRequest(request, body.Length);

            request.BeginGetRequestStream(
                ar =>
                {
                    var postStream = request.EndGetRequestStream(ar);
                    postStream.Write(body, 0, body.Length);
                    postStream.Close();

                    request.BeginGetResponse(
                        asyncResult => this.OnGetResponse(asyncResult, successStatusCode, responseMapper, callback),
                        request);
                },
                request);
        }

        protected abstract void OnSendingRequest(HttpWebRequest request, int contentLength);

        protected virtual HttpWebRequest ResolveRequest(Uri requestUri)
        {
            return (HttpWebRequest)WebRequestCreator.ClientHttp.Create(requestUri);
        }

        protected virtual void DispatchCallback<T>(Action<CloudOperationResponse<T>> callback, CloudOperationResponse<T> response)
        {
            if (callback != null)
            {
                if (this.Dispatcher != null)
                    this.Dispatcher.BeginInvoke(() => callback(response));
                else
                    callback(response);
            }
        }

        private void OnGetResponse<T>(IAsyncResult asyncResult, HttpStatusCode successStatusCode, Func<HttpWebResponse, T> responseMapper, Action<CloudOperationResponse<T>> callback)
        {
            try
            {
                var request = (HttpWebRequest)asyncResult.AsyncState;
                var httpResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
                CloudOperationResponse<T> serviceResponse;

                if (httpResponse.StatusCode == successStatusCode)
                {
                    serviceResponse = new CloudOperationResponse<T>(
                        true,
                        httpResponse.StatusCode,
                        responseMapper != null ? responseMapper(httpResponse) : default(T),
                        string.Empty);
                }
                else
                {
                    var contentResponse = httpResponse.GetResponseString();
                    serviceResponse = new CloudOperationResponse<T>(
                        false,
                        httpResponse.StatusCode,
                        default(T),
                        !string.IsNullOrWhiteSpace(contentResponse) ? contentResponse : string.Format(CultureInfo.InvariantCulture, "{0}: There was an error while performing the request.", httpResponse.StatusCode));
                }

                this.DispatchCallback(callback, serviceResponse);
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse)exception.Response;
                this.DispatchCallback(callback, new CloudOperationResponse<T>(false, response.StatusCode, default(T), Helpers.ParseXmlWebException(exception)));
            }
            catch (SerializationException exception)
            {
                this.DispatchCallback(callback, new CloudOperationResponse<T>(false, (HttpStatusCode)(-1), default(T), exception.Message));
            }
        }
    }
}
