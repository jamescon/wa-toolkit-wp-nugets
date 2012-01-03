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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Identity.Membership
{
    using System;
    using System.Linq;
    using Microsoft.ApplicationServer.Http;
    using Microsoft.Net.Http;

    internal static class ConfigurationExtensions
    {
        internal static HttpConfiguration AddDelegatingHandlers(this HttpConfiguration config, params Type[] handlers)
        {
            if (handlers != null)
                handlers.ToList().ForEach(t => config.MessageHandlers.Add(t));

            return config;
        }

        internal static HttpConfiguration AddRequestHandlers(this HttpConfiguration config)
        {
            var requestHandlers = config.RequestHandlers;

            config.RequestHandlers = (c, e, od) =>
            {
                if (requestHandlers != null)
                    requestHandlers(c, e, od); // Original request handler

                var filterAttribute = od.Attributes.Where(a => typeof(AuthenticateAttribute).IsAssignableFrom(a.GetType())).Cast<AuthenticateAttribute>().ToList();

                filterAttribute.ForEach(a => c.Add(new MembershipAuthenticateUserOperationHandler()));
            };

            return config;
        }
    }
}