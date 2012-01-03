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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Net.Http;
    using System.Security.Principal;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web;
    using ApplicationServer.Http.Dispatcher;
    using Properties;

    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class EndpointService
    {
        protected readonly IEndpointRepository Repository;
        private readonly Func<HttpRequestMessage, string> mapUsernameDelegate;

        public EndpointService()
            : this(NotificationServiceContext.Current.Configuration.StorageProvider, NotificationServiceContext.Current.Configuration.MapUsername)
        {
        }

        public EndpointService(IEndpointRepository repository)
            : this(repository, NotificationServiceContext.Current.Configuration.MapUsername)
        {
        }

        public EndpointService(IEndpointRepository repository, Func<HttpRequestMessage, string> mapUsernameDelegate)
        {
            if (repository == null)
                throw new ArgumentNullException("repository");

            this.Repository = repository;
            this.mapUsernameDelegate = mapUsernameDelegate;
        }

        [WebGet(UriTemplate = ""), AuthenticateEndpoint, AuthorizeManagementEndpoint]
        public IEnumerable<Endpoint> GetAll()
        {
            try
            {
                return this.Repository.All();
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }
        }

        [WebGet(UriTemplate = "{applicationId}/{deviceId}"), AuthenticateEndpoint, AuthorizeManagementEndpoint]
        public HttpResponseMessage<Endpoint> Get(string applicationId, string deviceId)
        {
            try
            {
                var endpoint = this.Repository.Find(e => e.ApplicationId == applicationId && e.DeviceId == deviceId);

                if (endpoint == null)
                    return new HttpResponseMessage<Endpoint>(null, HttpStatusCode.NotFound);

                return new HttpResponseMessage<Endpoint>(endpoint, HttpStatusCode.OK);
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }
        }

        [WebInvoke(UriTemplate = "", Method = "PUT"), AuthenticateEndpoint, AuthorizeRegistrationEndpoint]
        public HttpResponseMessage<Endpoint> Put(HttpRequestMessage<Endpoint> message)
        {
            if (message == null)
                throw WebException(Resources.ErrorParameterEndpointCannotBeNull, HttpStatusCode.BadRequest);

            try
            {
                var readTask = message.Content.ReadAsAsync();
                readTask.Wait();
                var endpoint = readTask.Result;

                // Set the username under which the app will store the channel
                endpoint.UserId = this.mapUsernameDelegate(message);

                this.Repository.InsertOrUpdate(endpoint);
                return new HttpResponseMessage<Endpoint>(endpoint, HttpStatusCode.Accepted);
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }
        }

        [WebInvoke(UriTemplate = "{applicationId}/{deviceId}", Method = "DELETE"), AuthenticateEndpoint, AuthorizeRegistrationEndpoint]
        public HttpResponseMessage Delete(string applicationId, string deviceId)
        {
            if (string.IsNullOrWhiteSpace(applicationId))
                throw WebException(Resources.ErrorParameterApplicationIdCannotBeNull, HttpStatusCode.BadRequest);

            if (string.IsNullOrWhiteSpace(deviceId))
                throw WebException(Resources.ErrorParameterDeviceIdCannotBeNull, HttpStatusCode.BadRequest);

            try
            {
                this.Repository.Delete(applicationId, deviceId);
                return new HttpResponseMessage { StatusCode = HttpStatusCode.Accepted };
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }
        }

        public virtual HttpContext RequestContext()
        {
            return HttpContext.Current;
        }

        private static HttpResponseException WebException(string message, HttpStatusCode code)
        {
            return new HttpResponseException(new HttpResponseMessage(code) { Content = new StringContent(message) });
        }
    }
}