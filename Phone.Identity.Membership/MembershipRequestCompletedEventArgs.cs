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
    using System;

    /// <summary>
    /// Provides data for the events of the MembershipSignIn control. 
    /// </summary>
    public class MembershipRequestCompletedEventArgs : EventArgs
    {
        internal MembershipRequestCompletedEventArgs(MembershipResponse response)
        {
            this.Response = response;
        }

        /// <summary>
        ///  Gets the MembershipResponse returned by the Membership service.
        /// </summary>
        /// <remarks>If an error occur the null is returned.</remarks>
        public MembershipResponse Response { get; private set; }
    }
}
