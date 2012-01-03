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

namespace Microsoft.WindowsAzure.Samples.Phone.Notifications
{
    using System;
    using System.Collections.Generic;
    using System.Net;
    using System.Windows.Threading;

    public class PushConfig
    {
        public PushConfig()
        {
            this.AllowedDomains = new List<Uri>();
        }

        public string ChannelName { get; set; }

        public string ServiceName { get; set; }

        public IList<Uri> AllowedDomains { get; private set; }

        public Dispatcher Dispatcher { get; set; }

        public Action<WebRequest> SignRequest { get; set; }

        public Uri EndpointsServiceUri { get; set; }

        public string ApplicationId { get; set; }

        public string DeviceId { get; set; }
    }
}
