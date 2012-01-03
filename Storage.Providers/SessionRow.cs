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

namespace Microsoft.WindowsAzure.Samples.Storage.Providers
{
    using System;

    using Microsoft.WindowsAzure.StorageClient;

    [CLSCompliant(false)]
    public class SessionRow : TableServiceEntity
    {
        private string id;
        private string applicationName;
        private string blobName;
        private DateTime expires;
        private DateTime created;
        private DateTime lockDate;

        // application name + session id is partitionKey
        public SessionRow(string sessionId, string applicationName)
            : base()
        {
            SecUtility.CheckParameter(ref sessionId, true, true, true, ProvidersConfiguration.MaxStringPropertySizeInChars, "sessionId");
            SecUtility.CheckParameter(ref applicationName, true, true, true, Constants.MaxTableApplicationNameLength, "applicationName");

            PartitionKey = SecUtility.CombineToKey(applicationName, sessionId);
            RowKey = string.Empty;

            this.Id = sessionId;
            this.ApplicationName = applicationName;
            this.ExpiresUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.LockDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.CreatedUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.Timeout = 0;
            this.BlobName = string.Empty;
        }

        public SessionRow()
            : base()
        {
        }

        public string Id
        {
            get
            {
                return this.id;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.id = value;
                this.PartitionKey = SecUtility.CombineToKey(this.ApplicationName, this.Id);
            }
        }

        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.applicationName = value;
                this.PartitionKey = SecUtility.CombineToKey(this.ApplicationName, this.Id);
            }
        }

        public int Timeout { get; set; }

        public DateTime ExpiresUtc
        {
            get
            {
                return this.expires;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.expires);
            }
        }

        public DateTime CreatedUtc
        {
            get
            {
                return this.created;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.created);
            }
        }

        public DateTime LockDateUtc
        {
            get
            {
                return this.lockDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.lockDate);
            }
        }

        public bool Locked { get; set; }

        public int Lock { get; set; }

        public string BlobName
        {
            get
            {
                return this.blobName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.blobName = value;
            }
        }

        public bool Initialized { get; set; }
    }
}