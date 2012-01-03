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
    using System.Collections.Generic;

    internal class IdentityProviderListEventArgs : EventArgs
    {
        internal IdentityProviderListEventArgs(IEnumerable<IdentityProviderInformation> identityProviders, Exception exception)
        { 
            this.Error = exception;
            this.Result = identityProviders; 
        }

        internal Exception Error { get; private set; }

        internal IEnumerable<IdentityProviderInformation> Result { get; private set; }
    }
}
