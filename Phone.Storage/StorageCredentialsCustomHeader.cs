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
    using System.Net;

    public class StorageCredentialsCustomHeader : IStorageCredentials
    {
        private readonly string headerName;
        private readonly Func<string> headerValue;

        public StorageCredentialsCustomHeader(string headerName, Func<string> headerValue)
        {
            if (string.IsNullOrWhiteSpace(headerName))
                throw new ArgumentException("The header name cannot be null, empty or white space.", "headerName");

            if (headerValue == null)
                throw new ArgumentNullException("headerValue", "The lambda expression to get the custom header value cannot be null.");

            this.headerName = headerName;
            this.headerValue = headerValue;
        }

        public void SignRequest(WebRequest webRequest, long contentLength)
        {
            if (webRequest == null)
                throw new ArgumentNullException("webRequest");

            webRequest.Headers[this.headerName] = this.headerValue();
        }

        public void SignRequestLite(WebRequest webRequest)
        {
            if (webRequest == null)
                throw new ArgumentNullException("webRequest");

            webRequest.Headers[this.headerName] = this.headerValue();
        }

        public void SignRequestLite(WebHeaderCollection requestHeaders, Uri requestUri)
        {
            if (requestHeaders == null)
                throw new ArgumentNullException("requestHeaders", "The request headers cannot be null.");

            requestHeaders[this.headerName] = this.headerValue();
        }
    }
}
