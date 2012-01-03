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

    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties;

    public class StorageServicesContext
    {
        private static readonly StorageServicesContext Instance = new StorageServicesContext();

        private readonly StorageServicesConfig config = new StorageServicesConfig();

        public static StorageServicesContext Current
        {
            get { return Instance; }
        }

        public StorageServicesConfig Configuration
        {
            get { return this.config; }
        }

        public void Configure(Action<StorageServicesConfig> configureAction)
        {
            if (configureAction == null)
                throw new ArgumentException(Resource.ConfigureActionArgumentNullErrorMessage, "configureAction");
            
            configureAction(this.config);
        }
    }
}
