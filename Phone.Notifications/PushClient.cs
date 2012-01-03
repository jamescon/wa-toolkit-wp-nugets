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

namespace Microsoft.WindowsAzure.Samples.Phone.Notifications
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization.Json;
    using System.ServiceModel.Security;
    using System.Windows.Threading;

    public class PushClient : IPushClient
    {
        private readonly Uri endpointsServiceUri;
        private readonly Action<WebRequest> signRequestDelegate;
        private readonly Dispatcher dispatcher;
        private readonly string applicationId;
        private readonly string deviceId;

        public PushClient(Uri endpointsServiceUri, Action<WebRequest> signRequestDelegate, string applicationId, string deviceId)
            : this(endpointsServiceUri, signRequestDelegate, applicationId, deviceId, null)
        {
        }

        public PushClient(Uri endpointsServiceUri, Action<WebRequest> signRequestDelegate, string applicationId, string deviceId, Dispatcher dispatcher)
        {
            if (endpointsServiceUri == null)
                throw new ArgumentNullException("endpointsServiceUri");

            if (signRequestDelegate == null)
                throw new ArgumentNullException("signRequestDelegate");

            if (string.IsNullOrWhiteSpace(applicationId))
                throw new ArgumentNullException("applicationId");

            if (string.IsNullOrWhiteSpace(deviceId))
                throw new ArgumentNullException("deviceId");

            this.endpointsServiceUri = endpointsServiceUri;
            this.signRequestDelegate = signRequestDelegate;
            this.applicationId = applicationId;
            this.deviceId = deviceId;
            this.dispatcher = dispatcher;
        }

        public void Connect(Action<PushRegistrationResponse> callback)
        {
            EventHandler<PushContextErrorEventArgs> errorHandler = null;
            errorHandler =
                (s, e) =>
                {
                    PushContext.Current.Error -= errorHandler;
                    this.DispatchCallback(callback, new PushRegistrationResponse(false, e.Exception.Message));
                };

            PushContext.Current.Error += errorHandler;
            PushContext.Current.Connect(
                c =>
                {
                    PushContext.Current.Error -= errorHandler;

                    var endpoint = new Endpoint
                    {
                        ApplicationId = this.applicationId,
                        DeviceId = this.deviceId,
                        ChannelUri = c.ChannelUri.ToString()
                    };

                    this.PutEndpoint(endpoint, callback);
                });
        }

        public void Disconnect(Action<PushRegistrationResponse> callback)
        {
            this.DeleteEndpoint(callback);

            PushContext.Current.Disconnect();
        }

        protected virtual void DispatchCallback(Action<PushRegistrationResponse> callback, PushRegistrationResponse response)
        {
            if (callback != null)
            {
                if (this.dispatcher != null)
                    this.dispatcher.BeginInvoke(() => callback(response));
                else
                    callback(response);
            }
        }

        protected virtual HttpWebRequest ResolveRequest(Uri requestUri)
        {
            return (HttpWebRequest)WebRequestCreator.ClientHttp.Create(requestUri);
        }

        private void PutEndpoint(Endpoint endpoint, Action<PushRegistrationResponse> callback)
        {
            var request = this.ResolveRequest(this.endpointsServiceUri);
            request.Method = "PUT";
            request.ContentType = "application/json";
            request.Accept = "application/json";

            byte[] body;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(Endpoint));
                serializer.WriteObject(stream, endpoint);

                body = stream.ToArray();
            }

            try
            {
                this.signRequestDelegate(request);
                request.BeginGetRequestStream(
                    ar =>
                    {
                        var postStream = request.EndGetRequestStream(ar);

                        postStream.Write(body, 0, body.Length);
                        postStream.Close();

                        request.BeginGetResponse(
                            asyncResult =>
                            {
                                try
                                {
                                    var response = request.EndGetResponse(asyncResult) as HttpWebResponse;
                                    if ((response != null) && (response.StatusCode == HttpStatusCode.Accepted))
                                    {
                                        this.DispatchCallback(callback, new PushRegistrationResponse(true, string.Empty));
                                    }
                                    else
                                    {
                                        this.DispatchCallback(callback, new PushRegistrationResponse(false, "The push notification channel coud not be registered in the Endpoints service."));
                                    }
                                }
                                catch (WebException webException)
                                {
                                    this.DispatchCallback(callback, new PushRegistrationResponse(false, Helpers.ParseWebException(webException)));
                                }
                            },
                        null);
                    },
                request);
            }
            catch (ArgumentNullException exception)
            {
                this.DispatchCallback(callback, new PushRegistrationResponse(false, exception.Message));
            }
            catch (MessageSecurityException exception)
            {
                this.DispatchCallback(callback, new PushRegistrationResponse(false, exception.Message));
            }
        }

        private void DeleteEndpoint(Action<PushRegistrationResponse> callback)
        {
            var builder = new UriBuilder(this.endpointsServiceUri);
            builder.Path += string.Format(CultureInfo.InvariantCulture, "/{0}/{1}", this.applicationId, this.deviceId);

            var request = this.ResolveRequest(builder.Uri);
            request.Method = "DELETE";

            try
            {
                this.signRequestDelegate(request);
                request.BeginGetResponse(
                    asyncResult =>
                    {
                        try
                        {
                            var response = request.EndGetResponse(asyncResult) as HttpWebResponse;
                            if ((response != null) && (response.StatusCode == HttpStatusCode.Accepted))
                            {
                                this.DispatchCallback(callback, new PushRegistrationResponse(true, string.Empty));
                            }
                            else
                            {
                                this.DispatchCallback(callback, new PushRegistrationResponse(false, "The push notification channel coud not be unregistered in the Endpoints service."));
                            }
                        }
                        catch (WebException webException)
                        {
                            this.DispatchCallback(callback, new PushRegistrationResponse(false, Helpers.ParseWebException(webException)));
                        }
                    },
                null);
            }
            catch (ArgumentNullException exception)
            {
                this.DispatchCallback(callback, new PushRegistrationResponse(false, exception.Message));
            }
            catch (MessageSecurityException exception)
            {
                this.DispatchCallback(callback, new PushRegistrationResponse(false, exception.Message));
            }
        }
    }
}