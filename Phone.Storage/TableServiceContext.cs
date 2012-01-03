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
    using System.Security;
    using Microsoft.WindowsAzure.Samples.Data.Services.Client;

    [CLSCompliant(false)]
    [SecurityCritical]
    public class TableServiceContext : DataServiceContext
    {
        public TableServiceContext(Uri tablesBaseUri, IStorageCredentials credentials)
            : base(tablesBaseUri)
        {
            this.StorageCredentials = credentials;

            this.SendingRequest += this.OnSendingRequest;
            this.IgnoreMissingProperties = true;
            this.MergeOption = MergeOption.PreserveChanges;
        }

        public IStorageCredentials StorageCredentials { get; internal set; }

        public void AddTable(TableServiceSchema table)
        {
            if (table == null)
                throw new ArgumentNullException("table", "The table to add cannot be null");

            if (string.IsNullOrWhiteSpace(table.TableName))
                throw new ArgumentException("You need to provide a valid name for the new table", "table");

            this.AddObject("Tables", table);
        }

        public void RemoveTable(TableServiceSchema table)
        {
            if (table == null)
                throw new ArgumentNullException("table", "The table to remove cannot be null");

            this.AttachTo("Tables", table);
            this.DeleteObject(table);
        }

        [SecuritySafeCritical]
        protected virtual void OnSendingRequest(object sender, SendingRequestEventArgs e)
        {
            e.RequestHeaders["x-ms-version"] = "2011-08-18";

            this.StorageCredentials.SignRequestLite(e.RequestHeaders, e.RequestUri);
        }
    }
}
