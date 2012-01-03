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
    using System.Collections.Generic;
    using System.Net.Http;

    public class NotificationServiceConfig
    {
        private Func<HttpRequestMessage, bool> authenticateRequest;
        private Func<HttpRequestMessage, bool> authorizeManagementRequest;
        private Func<HttpRequestMessage, bool> authorizeRegistrationRequest;
        private Func<HttpRequestMessage, string> mapUsername;

        public NotificationServiceConfig()
        {
            this.DelegatingHandlers = new Type[] { };
        }

        public Func<HttpRequestMessage, bool> AuthenticateRequest
        {
            get
            {
                return this.authenticateRequest ?? (this.authenticateRequest = DefaultAnonymousAccess);
            }

            set { this.authenticateRequest = value; }
        }

        public Func<HttpRequestMessage, bool> AuthorizeManagementRequest
        {
            get
            {
                return this.authorizeManagementRequest ?? (this.authorizeManagementRequest = DefaultAnonymousAccess);
            }

            set { this.authorizeManagementRequest = value; }
        }

        public Func<HttpRequestMessage, bool> AuthorizeRegistrationRequest
        {
            get
            {
                return this.authorizeRegistrationRequest ?? (this.authorizeRegistrationRequest = DefaultAnonymousAccess);
            }

            set { this.authorizeRegistrationRequest = value; }
        }

        public Func<HttpRequestMessage, string> MapUsername
        {
            get
            {
                if (this.mapUsername == null)
                    this.MapUsername = DefaultUsernameMapper;

                return this.mapUsername;
            }

            set { this.mapUsername = value; }
        }

        public IEnumerable<Type> DelegatingHandlers { get; set; }

        public IEndpointRepository StorageProvider { get; set; }

        private static bool DefaultAnonymousAccess(HttpRequestMessage message)
        {
            // By default, return always true (anonymous user)
            return true;
        }

        private static string DefaultUsernameMapper(HttpRequestMessage message)
        {
            // By default, return an empty username (anonymous user)
            return string.Empty;
        }
    }
}
