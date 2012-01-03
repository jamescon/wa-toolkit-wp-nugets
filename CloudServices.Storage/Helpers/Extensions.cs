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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage.Helpers
{
    using System;
    using System.Globalization;
    using System.Net;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using Microsoft.ApplicationServer.Http.Dispatcher;

    public static class Extensions
    {
        private const string ErrorResponse = "<?xml version=\"1.0\" encoding=\"utf-8\" standalone=\"yes\" ?><error {0}><code>{1}</code><message xml:lang=\"en-US\">{2}</message></error>";
        private const string DataServiceNamespace = "xmlns=\"http://schemas.microsoft.com/ado/2007/08/dataservices/metadata\"";

        [CLSCompliant(false)]
        public static HttpContent CloneContent(this HttpContent requestContent, string content)
        {
            if (requestContent == null)
            {
                throw new ArgumentNullException("requestContent");
            }

            var result = new StringContent(content);

            result.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            result.Headers.Expires = requestContent.Headers.Expires;
            result.Headers.LastModified = requestContent.Headers.LastModified;

            return result;
        }

        [CLSCompliant(false)]
        public static HttpResponseException StorageException(HttpStatusCode code, string error, string detail)
        {
            var response = new HttpResponseMessage(code);

            var errorMessage = string.Format(CultureInfo.InvariantCulture, ErrorResponse, DataServiceNamespace, error, detail);
            response.ReasonPhrase = error;
            response.Content = new StringContent(errorMessage);
            response.Content.Headers.ContentType = new MediaTypeHeaderValue("application/atom+xml");
            return new HttpResponseException(response);
        }
    }
}
