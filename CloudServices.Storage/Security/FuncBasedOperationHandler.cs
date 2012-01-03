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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage.Security
{
    using System.Net.Http;
    using System.Web;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    internal class FuncBasedOperationHandler : HttpOperationHandler<HttpRequestMessage, HttpRequestMessage>
    {
        private readonly FuncBasedFilterAttribute filterAttribute;

        public FuncBasedOperationHandler(FuncBasedFilterAttribute filterAttribute)
            : base("response")
        {
            this.filterAttribute = filterAttribute;
        }

        protected override HttpRequestMessage OnHandle(HttpRequestMessage input)
        {
            if (this.filterAttribute.Filter(input))
                return input;

            // HACK: Prevent ASP.NET Forms Authentication to redirect the user to the login page.
            // This thread-safe approach adds a header with the suppression to be read on the 
            // OnEndRequest event of the pipelien. In order to fully support the supression you should have the ASP.NET Module
            // that does this (SuppressFormsAuthenticationRedirectModule).  
            var response = new HttpResponseMessage(System.Net.HttpStatusCode.Unauthorized);
            response.Headers.Add(SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");

            throw new HttpResponseException(response);
        }
    }
}
