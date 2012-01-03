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

namespace Microsoft.WindowsAzure.Samples.Phone.Identity.AccessControl
{
    using System;

    /// <summary>
    /// Provides data for the AccessControlServiceSignIn control RequestSimpleWebTokenResponseCompleted event
    /// </summary>
    public class RequestSimpleWebTokenResponseCompletedEventArgs : EventArgs
    {
        internal RequestSimpleWebTokenResponseCompletedEventArgs(RequestSimpleWebTokenResponse requestSimpleWebTokenResponse, Exception originalError)
        {
            this.Error = originalError;
            this.RequestSimpleWebTokenResponse = requestSimpleWebTokenResponse;
        }

        /// <summary>
        /// Gets any exception thrown during while requesting the security token.
        /// </summary>
        /// <remarks>If no error occur the null is returned.</remarks>
        public Exception Error { get; private set; }

        /// <summary>
        ///  Gets the RequestSimpleWebTokenResponse returned while requesting the security token.
        /// </summary>
        /// <remarks>If an error occur the null is returned.</remarks>
        public RequestSimpleWebTokenResponse RequestSimpleWebTokenResponse { get; private set; }
    }
}
