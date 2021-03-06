﻿// ----------------------------------------------------------------------------------
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
    using System.Web.Routing;
    using Microsoft.ApplicationServer.Http;

    public static class RouteExtensions
    {
        public static void MapAuthenticationServiceRoute(this RouteCollection routes, string prefix, params Type[] handlers)
        {
            var configuration = new HttpConfiguration().AddDelegatingHandlers(handlers).AddRequestHandlers();
            routes.MapServiceRoute<MembershipService>(prefix, configuration);
        }
    }
}
