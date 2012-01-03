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
    using System.Collections.Generic;
    using System.Net.Http;

    public class StorageServicesConfig
    {
        private const int DefaultMaximmumResponseSize = 1024 * 1024;
        private const int DefaultSasExpirationSize = 15;

        private Func<HttpRequestMessage, bool> authorizeQueuesAccess;
        private Func<HttpRequestMessage, bool> authorizeTablesAccess;
        private Func<HttpRequestMessage, bool> authorizeBlobsAccess;
        private Func<HttpRequestMessage, bool> authenticateRequest;
        private Func<HttpRequestMessage, string> mapUserName;

        private int containerSasExpirationTime;
        private int blobsSasExpirationTime;
        private int windowsAzureStorageMaximmumResponseSize;

        public StorageServicesConfig()
        {
            this.DelegatingHandlers = new Type[] { };
        }

        public CloudStorageAccount CloudStorageAccount { get; set; }

        public int WindowsAzureStorageMaximumResponseSize
        {
            get
            {
                if (this.windowsAzureStorageMaximmumResponseSize == 0)
                    this.WindowsAzureStorageMaximumResponseSize = DefaultMaximmumResponseSize;

                return this.windowsAzureStorageMaximmumResponseSize;
            }

            set
            {
                this.windowsAzureStorageMaximmumResponseSize = value;
            }
        }

        public int ContainerSasExpirationTime
        {
            get
            {
                if (this.containerSasExpirationTime == 0)
                    this.ContainerSasExpirationTime = DefaultSasExpirationSize;

                return this.containerSasExpirationTime;
            }

            set
             {
                 this.containerSasExpirationTime = value;
             }
        }

        public int BlobsSasExpirationTime
        {
            get
            {
                if (this.blobsSasExpirationTime == 0)
                    this.BlobsSasExpirationTime = DefaultSasExpirationSize;

                return this.blobsSasExpirationTime;
            }

            set
            {
                this.blobsSasExpirationTime = value;
            }
        }

        public Func<HttpRequestMessage, bool> AuthorizeTablesAccess
        {
            get
            {
                return this.authorizeTablesAccess ?? (this.AuthorizeTablesAccess = DefaultAnonymousAccess);
            }

            set { this.authorizeTablesAccess = value; }
        }

        public Func<HttpRequestMessage, bool> AuthorizeQueuesAccess
        {
            get
            {
                return this.authorizeQueuesAccess ?? (this.AuthorizeQueuesAccess = DefaultAnonymousAccess);
            }

            set { this.authorizeQueuesAccess = value; }
        }

        public Func<HttpRequestMessage, bool> AuthorizeBlobsAccess
        {
            get
            {
                return this.authorizeBlobsAccess ?? (this.AuthorizeBlobsAccess = DefaultAnonymousAccess);
            }

            set { this.authorizeBlobsAccess = value; }
        }

        public Func<HttpRequestMessage, bool> AuthenticateRequest
        {
            get
            {
                if (this.authenticateRequest == null)
                    this.AuthenticateRequest = DefaultAnonymousAccess;

                return this.authenticateRequest;
            }

            set { this.authenticateRequest = value; }
        }

        public Func<HttpRequestMessage, string> MapUserName
        {
            get
            {
                if (this.mapUserName == null)
                    this.MapUserName = DefaultUsernameMapper;

                return this.mapUserName;
            }

            set { this.mapUserName = value; }
        }

        public IEnumerable<Type> DelegatingHandlers { get; set; }

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
