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
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Handlers;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Helpers;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Security;

    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = true)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class QueuesProxyService
    {
        private static readonly AzureQueuesProxyHandler Proxy = new AzureQueuesProxyHandler();

        [WebInvoke(Method = "POST", UriTemplate = "{*path}"), OperationContract, AuthorizeQueuesAccess, CLSCompliant(false)]
        public HttpResponseMessage HandlePost(HttpRequestMessage request, string path)
        {
            if (request == null)
                throw Extensions.StorageException(HttpStatusCode.BadRequest, Resource.RequestCannotBeNullErrorMessage, Resource.RequestCannotBeNullErrorMessage);

            request.Properties[StorageProxyHandler.RequestedPathPropertyName] = path;
            return Proxy.ProcessRequest(request);
        }

        [WebInvoke(Method = "PUT", UriTemplate = "{*path}"), OperationContract, AuthorizeQueuesAccess, CLSCompliant(false)]
        public HttpResponseMessage HandlePut(HttpRequestMessage request, string path)
        {
            if (request == null)
                throw Extensions.StorageException(HttpStatusCode.BadRequest, Resource.RequestCannotBeNullErrorMessage, Resource.RequestCannotBeNullErrorMessage);

            request.Properties[StorageProxyHandler.RequestedPathPropertyName] = path;
            return Proxy.ProcessRequest(request);
        }

        [WebInvoke(Method = "GET", UriTemplate = "{*path}"), OperationContract, AuthorizeQueuesAccess, CLSCompliant(false)]
        public HttpResponseMessage HandleGet(HttpRequestMessage request, string path)
        {
            if (request == null)
                throw Extensions.StorageException(HttpStatusCode.BadRequest, Resource.RequestCannotBeNullErrorMessage, Resource.RequestCannotBeNullErrorMessage);

            request.Properties[StorageProxyHandler.RequestedPathPropertyName] = path;
            return Proxy.ProcessRequest(request);
        }

        [WebInvoke(Method = "DELETE", UriTemplate = "{*path}"), OperationContract, AuthorizeQueuesAccess, CLSCompliant(false)]
        public HttpResponseMessage HandleDelete(HttpRequestMessage request, string path)
        {
            if (request == null)
                throw Extensions.StorageException(HttpStatusCode.BadRequest, Resource.RequestCannotBeNullErrorMessage, Resource.RequestCannotBeNullErrorMessage);

            request.Properties[StorageProxyHandler.RequestedPathPropertyName] = path;
            return Proxy.ProcessRequest(request);
        }

        [WebInvoke(Method = "HEAD", UriTemplate = "{*path}"), OperationContract, AuthorizeQueuesAccess, CLSCompliant(false)]
        public HttpResponseMessage HandleMerge(HttpRequestMessage request, string path)
        {
            if (request == null)
                throw Extensions.StorageException(HttpStatusCode.BadRequest, Resource.RequestCannotBeNullErrorMessage, Resource.RequestCannotBeNullErrorMessage);

            request.Properties[StorageProxyHandler.RequestedPathPropertyName] = path;
            return Proxy.ProcessRequest(request);
        }
    }
}