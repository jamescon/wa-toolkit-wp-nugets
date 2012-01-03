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

    public class CloudStorageContext
    {
        private static readonly CloudStorageContext Instance = new CloudStorageContext();

        private ICloudStorageClientResolver resolver;

        public static CloudStorageContext Current
        {
            get { return Instance; }
        }

        [CLSCompliant(false)]
        public ICloudStorageClientResolver Resolver
        {
            get
            {
                if (this.resolver == null)
                    throw new InvalidOperationException("The resolver cannot be used since it was not set yet.");

                return this.resolver;
            }

            set
            {
                this.resolver = value;
            }
        }
    }
}
