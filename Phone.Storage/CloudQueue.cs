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
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Text;
    using System.Windows.Threading;

    public class CloudQueue : CloudClientBase, ICloudQueue
    {
        private const int MaximumClearRetries = 10;

        public CloudQueue(Uri uri, string name, IStorageCredentials credentials, Dispatcher dispatcher)
            : base(dispatcher)
        {
            if (uri == null)
                throw new ArgumentNullException("uri", "The queue uri cannot be null.");

            if (string.IsNullOrWhiteSpace(name))
                throw new ArgumentException("The queue name cannot be null, empty or white space.", "name");

            if (credentials == null)
                throw new ArgumentNullException("credentials", "The storage credentials cannot be null.");

            this.Uri = uri;
            this.Name = name;
            this.Credentials = credentials;
            this.Metadata = new Dictionary<string, string>();
        }

        public string Name { get; internal set; }

        public Uri Uri { get; internal set; }

        public IDictionary<string, string> Metadata { get; internal set; }

        public IStorageCredentials Credentials { get; internal set; }

        private Uri BaseUri
        {
            get
            {
                var uriString = this.Uri.AbsoluteUri;
                var baseUri = uriString.Substring(0, uriString.LastIndexOf(this.Name, StringComparison.OrdinalIgnoreCase));
                return new Uri(baseUri);
            }
        }

        public void AddMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message", "The message cannot be null.");

            var requestUri = BuildUri(this.Uri, "messages", string.Empty);
            var request = this.ResolveRequest(requestUri);

            request.Method = "POST";

            var xmlData = string.Format(CultureInfo.InvariantCulture, "<QueueMessage><MessageText>{0}</MessageText></QueueMessage>", Convert.ToBase64String(message.AsBytes));

            this.SendRequest(request, Encoding.UTF8.GetBytes(xmlData), HttpStatusCode.Created, r => true, callback);
        }

        public void Clear(Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages", this.Uri.AbsoluteUri);

            this.InnerClear(serviceOperation, callback, 0);
        }

        public void Create(Action<CloudOperationResponse<bool>> callback)
        {
            var request = this.ResolveRequest(this.Uri);
            request.Method = "PUT";

            // Add metadata for the specified queue.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(request, HttpStatusCode.Created, r => true, callback);
        }

        public void CreateIfNotExist(Action<CloudOperationResponse<bool>> callback)
        {
            this.Create(
                r =>
                {
                    if (r.Success)
                        callback(r);
                    else if ((r.StatusCode == HttpStatusCode.NoContent) || (r.StatusCode == HttpStatusCode.Conflict))
                        callback(new CloudOperationResponse<bool>(true, r.StatusCode, false, r.ErrorMessage));
                    else
                        callback(r);
                });
        }

        public void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            var request = this.ResolveRequest(this.Uri);
            request.Method = "DELETE";

            this.SendRequest(request, HttpStatusCode.NoContent, r => true, callback);
        }

        public void DeleteMessage(CloudQueueMessage message, Action<CloudOperationResponse<bool>> callback)
        {
            if (message == null)
                throw new ArgumentNullException("message");

            if (string.IsNullOrWhiteSpace(message.Id))
                throw new ArgumentNullException("message", "The message id cannot be null nor empty.");

            if (string.IsNullOrWhiteSpace(message.PopReceipt))
                throw new ArgumentNullException("message", "The message must be retrieved from a queue in order to delete it.");

            var requestUri = BuildUri(this.Uri, BuilUriPath("messages", message.Id), string.Concat("popreceipt=", HttpUtility.UrlEncode(message.PopReceipt)));
            var request = this.ResolveRequest(requestUri);
            request.Method = "DELETE";

            this.SendRequest(request, HttpStatusCode.NoContent, r => true, callback);
        }

        public void Exists(Action<CloudOperationResponse<bool>> callback)
        {
            var queryString = string.Concat("comp=list&prefix=", this.Name, "&", GenerateIncrementalSeedQueryString());
            var requestUri = BuildUri(this.BaseUri, string.Empty, queryString);

            var request = this.ResolveRequest(requestUri);
            request.Method = "GET";

            this.SendRequest(request, HttpStatusCode.OK, this.CloudQueueExistsMapper, callback);
        }

        public void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}?comp=metadata", this.Uri);

            var request = this.ResolveRequest(new Uri(serviceOperation));
            request.Method = "HEAD";

            // Add metadata for the specified queue.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(
                request,
                HttpStatusCode.OK,
                r => r.Headers.GetMetadataHeaders(),
                r =>
                {
                    if (r.Success)
                        this.Metadata = r.Response;

                    callback(new CloudOperationResponse<bool>(r.Success, r.StatusCode, r.Success, r.ErrorMessage));
                });
        }

        public void GetMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, false, null, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Success, c.StatusCode, c.Response != null ? c.Response.SingleOrDefault() : null, c.ErrorMessage)));
        }

        public void GetMessage(TimeSpan visibilityTimeout, Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, false, visibilityTimeout, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Success, c.StatusCode, c.Response != null ? c.Response.SingleOrDefault() : null, c.ErrorMessage)));
        }

        public void GetMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, false, null, callback);
        }

        public void GetMessages(int messageCount, TimeSpan visibilityTimeout, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, false, visibilityTimeout, callback);
        }

        public void PeekMessage(Action<CloudOperationResponse<CloudQueueMessage>> callback)
        {
            this.InnerGetMessages(1, true, null, c => callback(new CloudOperationResponse<CloudQueueMessage>(c.Success, c.StatusCode, c.Response != null ? c.Response.SingleOrDefault() : null, c.ErrorMessage)));
        }

        public void PeekMessages(int messageCount, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            this.InnerGetMessages(messageCount, true, null, callback);
        }

        public void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}?comp=metadata", this.Uri);

            var request = this.ResolveRequest(new Uri(serviceOperation));
            request.Method = "PUT";

            // Add metadata for the specified queue.
            request.Headers.AddMetadataHeaders(this.Metadata);

            this.SendRequest(request, HttpStatusCode.NoContent, r => true, callback);
        }

        protected static IEnumerable<CloudQueueMessage> CloudQueueMessagesMapper(HttpWebResponse response)
        {
            var serializer = new DataContractSerializer(typeof(IEnumerable<CloudQueueMessage>), "QueueMessagesList", string.Empty);

            return serializer.ReadObject(response.GetResponseStream()) as IEnumerable<CloudQueueMessage>;
        }

        protected bool CloudQueueExistsMapper(HttpWebResponse response)
        {
            var serializer = new DataContractSerializer(typeof(InnerCloudQueueListResponse));

            return ((InnerCloudQueueListResponse)serializer.ReadObject(response.GetResponseStream()))
                .Queues
                .Select(q => new CloudQueue(q.Uri, q.Name, this.Credentials, this.Dispatcher))
                .Any(q => q.Name.Equals(this.Name, StringComparison.OrdinalIgnoreCase));
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

        private void InnerClear(string serviceOperation, Action<CloudOperationResponse<bool>> callback, int retryCounter)
        {
            if (retryCounter > MaximumClearRetries)
            {
                var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "Clear operation was not completed due to an internal time out. Please retry to finish the operation");
                this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, HttpStatusCode.RequestTimeout, false, exceptionMessage));
            }
            else
            {
                var request = this.ResolveRequest(new Uri(serviceOperation));
                request.Headers["x-ms-version"] = "2011-08-18";
                request.Method = "DELETE";

                this.Credentials.SignRequest(request, 0);
                request.BeginGetResponse(
                    ar =>
                    {
                        try
                        {
                            var response = (HttpWebResponse)request.EndGetResponse(ar);

                            if (response.StatusCode == HttpStatusCode.NoContent)
                            {
                                this.DispatchCallback(callback, new CloudOperationResponse<bool>(true, HttpStatusCode.NoContent, true, string.Empty));
                            }
                            else
                            {
                                if (response.StatusCode == HttpStatusCode.InternalServerError &&
                                    response.StatusDescription.Equals("OperationTimedOut", StringComparison.Ordinal))
                                {
                                    this.InnerClear(serviceOperation, callback, ++retryCounter);
                                }
                                else
                                {
                                    var exceptionMessage = string.Format(CultureInfo.InvariantCulture, "An error occurred while clearing all the messages from the queue '{0}'", this.Name);
                                    this.DispatchCallback(callback, new CloudOperationResponse<bool>(true, response.StatusCode, false, exceptionMessage));
                                }
                            }
                        }
                        catch (WebException exception)
                        {
                            var response = (HttpWebResponse)exception.Response;
                            this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, response.StatusCode, false, Helpers.ParseXmlWebException(exception)));
                        }
                        catch (SerializationException exception)
                        {
                            this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, exception.Message));
                        }
                    },
                    null);
            }
        }

        private void InnerGetMessages(int messageCount, bool peek, TimeSpan? visibilitytimeout, Action<CloudOperationResponse<IEnumerable<CloudQueueMessage>>> callback)
        {
            if (messageCount < 0 || messageCount > 32)
                throw new ArgumentException("The number of messages to be retrieved from the queue must be greater than zero and less than 32", "messageCount");

            if (visibilitytimeout.HasValue && (visibilitytimeout.Value.TotalHours > 2 || visibilitytimeout.Value.TotalSeconds < 0))
                throw new ArgumentException("The visibility timeout must be greater than zero and less than 7200 seconds (2 hours)", "visibilitytimeout");

            var visibilityQueryString = visibilitytimeout.HasValue ? string.Concat("visibilitytimeout=", visibilitytimeout.Value.TotalSeconds) : string.Empty;
            var serviceOperation = string.Format(CultureInfo.InvariantCulture, "{0}/messages?numofmessages={1}{2}{3}", this.Uri, messageCount, peek ? "&peekonly=true" : string.Empty, visibilityQueryString);

            // Adding an incremental seed to avoid a cached response.
            serviceOperation = string.Concat(serviceOperation, "&incrementalSeed=", DateTime.UtcNow.Ticks);

            var request = this.ResolveRequest(new Uri(serviceOperation));
            request.Method = "GET";

            this.SendRequest(
                request,
                HttpStatusCode.OK,
                CloudQueueMessagesMapper,
                c => callback(new CloudOperationResponse<IEnumerable<CloudQueueMessage>>(c.StatusCode == HttpStatusCode.NotFound || c.Success, c.StatusCode, c.Response, c.ErrorMessage)));
        }
    }
}
