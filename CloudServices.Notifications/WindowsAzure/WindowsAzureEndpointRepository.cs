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
    using System.Collections.Generic;
    using System.Data.Services.Client;
    using System.Linq;
    using Common.Storage;

    public class WindowsAzureEndpointRepository : IEndpointRepository
    {
        protected readonly IAzureTable<EndpointTableRow> Table; 

        private const string EndpointsTableName = "Endpoints";

        public WindowsAzureEndpointRepository(CloudStorageAccount storageAccount)
            : this(new AzureTable<EndpointTableRow>(storageAccount, EndpointsTableName))
        {
        }

        public WindowsAzureEndpointRepository(IAzureTable<EndpointTableRow> table)
        {
            this.Table = table;

            // Create the Push User Endpoints table if does not exist.
            this.Table.CreateIfNotExist();
        }

        private IQueryable<EndpointTableRow> Endpoints
        {
            get
            {
                return this.Table.Query;
            }
        }

        public IEnumerable<Endpoint> All()
        {
            return this.Endpoints.ToList().Select(Endpoint.To<Endpoint>).ToList();
        }

        public IEnumerable<Endpoint> AllThat(Func<Endpoint, bool> filterExpression)
        {
            if (filterExpression == null)
                throw new ArgumentNullException("filterExpression", "The filterExpression cannot be null.");

            try
            {
                return this.Endpoints.Where(filterExpression);
            }
            catch (DataServiceQueryException ex)
            {
                if (!ex.IsTablesNotFoundException())
                    throw;

                return null;
            }
        }

        public Endpoint Find(Func<Endpoint, bool> filterExpression)
        {
            return this.Endpoints.Where(filterExpression).ToList().FirstOrDefault();
        }

        public void InsertOrUpdate(Endpoint endpoint)
        {
            if (endpoint == null)
                throw new ArgumentNullException("endpoint", "Parameter endpoint cannot be null.");

            this.Table.AddOrUpdateEntity(Endpoint.To<EndpointTableRow>(endpoint));
        }

        public void Delete(string applicationId, string deviceId)
        {
            if (applicationId == null)
                throw new ArgumentNullException("applicationId");

            if (deviceId == null)
                throw new ArgumentNullException("deviceId");

            var storedEndpoint = this.Find(e => e.ApplicationId.Equals(applicationId) && e.DeviceId.Equals(deviceId));

            if (storedEndpoint != null)
                this.Table.DeleteEntity(Endpoint.To<EndpointTableRow>(storedEndpoint));
        }
    }
}