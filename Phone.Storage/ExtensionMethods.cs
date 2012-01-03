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
    using System.Collections.Generic;

    public static class ExtensionMethods
    {
        private static readonly Dictionary<string, TableServiceContext> tableServiceContexts = new Dictionary<string, TableServiceContext>();

        [CLSCompliant(false)]
        public static TableServiceContext ResolveTableServiceContext(this ICloudStorageClientResolver resolver, string contextName)
        {
            if (tableServiceContexts.ContainsKey(contextName))
                return tableServiceContexts[contextName];

            var context = resolver.CreateTableServiceContext();
            tableServiceContexts[contextName] = context;
            return context;
        }
    }
}
