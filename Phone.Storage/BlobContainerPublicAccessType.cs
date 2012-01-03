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
    /// <summary>
    /// Specifies the level of public access that is allowed on the container.
    /// </summary>
    public enum BlobContainerPublicAccessType
    {
        /// <summary>
        /// No public access. Only the account owner can read resources in this container.
        /// </summary>
        Off = 0,

        /// <summary>
        /// Container-level public access. Anonymous clients can read container and blob data.
        /// </summary>
        Container = 1,

        /// <summary>
        /// Blob-level public access. Anonymous clients can read only blob data within this container.
        /// </summary>
        Blob = 2
    }
}
