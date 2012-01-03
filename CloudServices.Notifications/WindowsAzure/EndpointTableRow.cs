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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Notifications.WindowsAzure
{
    using System;
    using System.Data.Services.Common;    
    using Microsoft.WindowsAzure.Samples.Common.Storage;
    
    [DataServiceEntity]
    [DataServiceKey(new[] { "PartitionKey", "RowKey" })]
    public class EndpointTableRow : Endpoint, ITableServiceEntity
    {
        public string PartitionKey
        {
            get { return this.ApplicationId; }
            set { this.ApplicationId = value; }
        }

        public string RowKey
        {
            get { return this.DeviceId; }
            set { this.DeviceId = value; }
        }

        public virtual DateTime Timestamp { get; set; }
    }
}
