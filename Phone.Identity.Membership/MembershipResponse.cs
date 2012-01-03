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

namespace Microsoft.WindowsAzure.Samples.Phone.Identity.Membership
{
    using System.Net;

    public class MembershipResponse
    {
        public MembershipResponse(bool success, HttpStatusCode statusCode, string content, string errorMessage)
        {
            this.Success = success;
            this.StatusCode = statusCode;
            this.Content = content;
            this.ErrorMessage = errorMessage;
        }

        public bool Success { get; private set; }

        public HttpStatusCode StatusCode { get; private set; }

        public string Content { get; private set; }

        public string ErrorMessage { get; private set; }
    }
}
