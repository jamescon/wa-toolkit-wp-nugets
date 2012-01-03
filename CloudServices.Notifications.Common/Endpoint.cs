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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Notifications
{
    using System;

    public class Endpoint
    {
        public virtual string ApplicationId { get; set; }

        public virtual string DeviceId { get; set; }

        public string ChannelUri { get; set; }

        public string UserId { get; set; }

        public string Expiry { get; set; }

        public static T To<T>(Endpoint endpoint) where T : Endpoint
        {
            if (endpoint.GetType() == typeof(T))
                return endpoint as T;

            var destination = Activator.CreateInstance<T>();

            destination.ApplicationId = endpoint.ApplicationId;
            destination.ChannelUri = endpoint.ChannelUri;
            destination.DeviceId = endpoint.DeviceId;
            destination.Expiry = endpoint.Expiry;
            destination.UserId = endpoint.UserId;

            return destination;
        }
    }
}
