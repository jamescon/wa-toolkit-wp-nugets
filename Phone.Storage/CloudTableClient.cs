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
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Windows.Threading;
    using Microsoft.WindowsAzure.Samples.Data.Services.Client;

    [SecuritySafeCritical]
    [CLSCompliant(false)]
    public class CloudTableClient : ICloudTableClient
    {
        private readonly Dispatcher dispatcher;

        [CLSCompliant(false)]
        public CloudTableClient(Uri tablesBaseUri, IStorageCredentials credentials) :
            this(tablesBaseUri, credentials, null)
        {
        }

        [CLSCompliant(false)]
        public CloudTableClient(Uri tablesBaseUri, IStorageCredentials credentials, Dispatcher dispatcher)
        {
            if (tablesBaseUri == null)
                throw new ArgumentNullException("tablesBaseUri", "Table Base Uri cannot be null.");

            if (credentials == null)
                throw new ArgumentNullException("credentials", "Credentials cannot be null.");
            
            this.dispatcher = dispatcher;
            this.TablesBaseUri = tablesBaseUri;
            this.Credentials = credentials;
        }

        public Uri TablesBaseUri { get; internal set; }

        public IStorageCredentials Credentials { get; internal set; }

        public void CreateTable(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            var context = this.GetDataServiceContext();
            context.AddTable(new TableServiceSchema(tableName));
            context.BeginSaveChanges(
                asyncResult =>
                {
                    try
                    {
                        var response = context.EndSaveChanges(asyncResult);
                        var statusCode = (HttpStatusCode)(response.IsBatchResponse ? response.BatchStatusCode : response.SingleOrDefault().StatusCode);

                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(true, statusCode, true, string.Empty));
                    }
                    catch (DataServiceClientException exception)
                    {
                        var statusCode = (HttpStatusCode)exception.StatusCode;
                        string errorMessage = statusCode == HttpStatusCode.NotFound ? "Could not reach the Storage Provider." : Helpers.ParseDataServiceException(exception);
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, statusCode, false, errorMessage));
                    }
                    catch (InvalidOperationException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, Helpers.ParseDataServiceException(exception.GetBaseException())));
                    }
                    catch (NotSupportedException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, exception.Message));
                    }
                },
                null);
        }

        public void CreateTableIfNotExist(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            this.DoesTableExist(
                tableName,
                tableExistsResult =>
                {
                    if (!tableExistsResult.Success || tableExistsResult.Response)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(tableExistsResult.Success, tableExistsResult.StatusCode, false, tableExistsResult.ErrorMessage));
                    }
                    else
                    {
                        this.CreateTable(
                            tableName,
                            tableCreateResult => this.DispatchCallback(callback, new CloudOperationResponse<bool>(tableCreateResult.Success, tableCreateResult.StatusCode, tableCreateResult.Response, tableCreateResult.ErrorMessage)));
                    }
                });
        }

        public void DoesTableExist(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            var context = this.GetDataServiceContext();
            var uri = new Uri(string.Format(CultureInfo.InvariantCulture, "{0}/Tables?$filter=TableName eq '{1}'", context.BaseUri.ToString().TrimEnd('/'), tableName));

            context.BeginExecute<TableServiceSchema>(
                uri,
                asyncResult =>
                {
                    try
                    {
                        var response = context.EndExecute<TableServiceSchema>(asyncResult);
                        var operationResponse = (OperationResponse)response;
                        var statusCode = (HttpStatusCode)operationResponse.StatusCode;

                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(true, statusCode, response.FirstOrDefault() != null, Helpers.ParseDataServiceException(operationResponse.Error)));
                    }
                    catch (DataServiceClientException exception)
                    {
                        var statusCode = (HttpStatusCode)exception.StatusCode;
                        var errorMessage = statusCode == HttpStatusCode.NotFound ? "Could not reach the Storage Provider." : Helpers.ParseDataServiceException(exception);
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, statusCode, false, errorMessage));
                    }
                    catch (InvalidOperationException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, Helpers.ParseDataServiceException(exception.GetBaseException())));
                    }
                    catch (NotSupportedException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, exception.Message));
                    }
                },
                null);
        }

        public void DeleteTable(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            var context = this.GetDataServiceContext();
            context.RemoveTable(new TableServiceSchema(tableName));
            context.BeginSaveChanges(
                asyncResult =>
                {
                    try
                    {
                        var response = context.EndSaveChanges(asyncResult);
                        var statusCode = (HttpStatusCode)(response.IsBatchResponse ? response.BatchStatusCode : response.SingleOrDefault().StatusCode);

                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(true, statusCode, true, string.Empty));
                    }
                    catch (DataServiceClientException exception)
                    {
                        var statusCode = (HttpStatusCode)exception.StatusCode;
                        var errorMessage = statusCode == HttpStatusCode.NotFound ? "Could not reach the Storage Provider." : Helpers.ParseDataServiceException(exception);
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, statusCode, false, errorMessage));
                    }
                    catch (InvalidOperationException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, Helpers.ParseDataServiceException(exception.GetBaseException())));
                    }
                    catch (NotSupportedException exception)
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(false, (HttpStatusCode)(-1), false, exception.Message));
                    }
                },
                null);
        }

        public void DeleteTableIfExist(string tableName, Action<CloudOperationResponse<bool>> callback)
        {
            this.DoesTableExist(
                tableName,
                tableExistsResult =>
                {
                    if (!tableExistsResult.Success || tableExistsResult.Response)
                    {
                        this.DeleteTable(
                            tableName,
                            tableCreateResult => this.DispatchCallback(callback, new CloudOperationResponse<bool>(tableCreateResult.Success, tableCreateResult.StatusCode, tableCreateResult.Response, tableCreateResult.ErrorMessage)));
                    }
                    else
                    {
                        this.DispatchCallback(callback, new CloudOperationResponse<bool>(tableExistsResult.Success, tableExistsResult.StatusCode, false, tableExistsResult.ErrorMessage));
                    }
                });
        }

        public TableServiceContext GetDataServiceContext()
        {
            return new TableServiceContext(this.TablesBaseUri, this.Credentials);
        }

        protected virtual void DispatchCallback(Action<CloudOperationResponse<bool>> callback, CloudOperationResponse<bool> response)
        {
            if (callback != null)
            {
                if (this.dispatcher != null)
                    this.dispatcher.BeginInvoke(() => callback(response));
                else
                    callback(response);
            }
        }
    }
}
