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

// This file contains an implementation of a session state provider that uses both blob and table storage.
namespace Microsoft.WindowsAzure.Samples.Storage.Providers
{
    using System;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Web;
    using System.Web.SessionState;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class TableStorageSessionStateProvider : SessionStateStoreProviderBase
    {
        #region Member variables and constants

        private const int NumRetries = 3;

        private static readonly object ThisLock = new object();
        private readonly RetryPolicy tableRetryPolicy = RetryPolicies.Retry(NumRetries, TimeSpan.FromSeconds(1));
        private readonly SecUtility.ProviderRetryPolicy providerRetryPolicy = ProviderRetryPolicies.RetryN(NumRetries, TimeSpan.FromSeconds(1));

        private string applicationName;
        private string tableName;
        private string containerName;
        private CloudTableClient tableStorage;
        private BlobProvider blobProvider;

        #endregion

        #region public methods

        public override void Initialize(string name, NameValueCollection config)
        {
            // Verify that config isn't null
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            // Assign the provider a default name if it doesn't have one
            if (string.IsNullOrEmpty(name))
            {
                name = "TableServiceSessionStateProvider";
            }

            // Add a default "description" attribute to config if the
            // attribute doesn't exist or is empty
            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Session state provider using table storage");
            }

            // Call the base class's Initialize method
            base.Initialize(name, config);

            bool allowInsecureRemoteEndpoints = ProvidersConfiguration.GetBooleanValue(config, "allowInsecureRemoteEndpoints", false);

            // structure storage-related properties
            this.applicationName = ProvidersConfiguration.GetStringValueWithGlobalDefault(
                config,
                "applicationName",
                ProvidersConfiguration.DefaultProviderApplicationNameConfigurationString,
                ProvidersConfiguration.DefaultProviderApplicationName,
                false);

            this.tableName = ProvidersConfiguration.GetStringValueWithGlobalDefault(
                config,
                "sessionTableName",
                ProvidersConfiguration.DefaultSessionTableNameConfigurationString,
                ProvidersConfiguration.DefaultSessionTableName,
                false);

            this.containerName = ProvidersConfiguration.GetStringValueWithGlobalDefault(
                config,
                "containerName",
                ProvidersConfiguration.DefaultSessionContainerNameConfigurationString,
                ProvidersConfiguration.DefaultSessionContainerName,
                false);

            if (!SecUtility.IsValidContainerName(this.containerName))
            {
                throw new ProviderException("The provider configuration for the TableStorageSessionStateProvider does not contain a valid container name. " +
                                            "Please refer to the documentation for the concrete rules for valid container names." +
                                            "The current container name is: " + this.containerName);
            }

            config.Remove("allowInsecureRemoteEndpoints");
            config.Remove("containerName");
            config.Remove("applicationName");
            config.Remove("sessionTableName");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!string.IsNullOrEmpty(attr)) throw new ProviderException("Unrecognized attribute: " + attr);
            }

            CloudStorageAccount account = null;
            try
            {
                account = ProvidersConfiguration.GetStorageAccount(ProvidersConfiguration.DefaultStorageConfigurationString);
                SecUtility.CheckAllowInsecureEndpoints(allowInsecureRemoteEndpoints, account.Credentials, account.TableEndpoint);
                SecUtility.CheckAllowInsecureEndpoints(allowInsecureRemoteEndpoints, account.Credentials, account.BlobEndpoint);

                this.tableStorage = account.CreateCloudTableClient();
                this.tableStorage.RetryPolicy = this.tableRetryPolicy;

                lock (ThisLock)
                {
                    TableStorageExtensionMethods.CreateTableIfNotExist<SessionRow>(this.tableStorage, this.tableName);
                }

                this.blobProvider = new BlobProvider(account.Credentials, account.BlobEndpoint, this.containerName);
            }
            catch (SecurityException)
            {
                throw;
            }
            catch (Exception e)
            {
                // catch InvalidOperationException as well as StorageException
                string exceptionDescription = ProvidersConfiguration.GetInitExceptionDescription(account.Credentials, account.TableEndpoint, account.BlobEndpoint);
                string sessionTableName = (this.tableName == null) ? "no session table name specified" : this.tableName;
                string sessionContainerName = (this.containerName == null)
                                                  ? "no container name specified"
                                                  : this.containerName;

                Log.Write(
                    EventKind.Error,
                    "Initialization of data service structures (tables and/or blobs) failed!" + exceptionDescription + Environment.NewLine + "Configured blob container: " + sessionContainerName + Environment.NewLine + "Configured table name: " + sessionTableName + Environment.NewLine + e.Message + Environment.NewLine + e.StackTrace);

                throw new ProviderException(
                                            "Initialization of data service structures (tables and/or blobs) failed!" +
                                            "The most probable reason for this is that " +
                                            "the storage endpoints are not configured correctly. Please look at the configuration settings " +
                                            "in your .cscfg and Web.config files. More information about this error " +
                                            "can be found in the logs when running inside the hosting environment or in the output "
                                            + "window of Visual Studio.",
                                            e);
            }

            Debug.Assert(this.blobProvider != null, "Blob provider is null");
        }

        public override SessionStateStoreData CreateNewStoreData(HttpContext context, int timeout)
        {
            Debug.Assert(context != null, "Http context is null");
            return new SessionStateStoreData(
                new SessionStateItemCollection(), SessionStateUtility.GetSessionStaticObjects(context), timeout);
        }

        public override void CreateUninitializedItem(HttpContext context, string id, int timeout)
        {
            Debug.Assert(context != null, "Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");
            if (timeout < 0)
            {
                throw new ArgumentException("Parameter timeout must be a non-negative integer!");
            }

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                var session = new SessionRow(id, this.applicationName)
                    {
                        Lock = 0,
                        Initialized = false,
                        Id = id,
                        Timeout = timeout,
                        ExpiresUtc = DateTime.UtcNow.AddMinutes(timeout)
                    };

                // no lock
                svc.AddObject(this.tableName, session);
                svc.SaveChangesWithRetries();
            }
            catch (InvalidOperationException e)
            {
                throw new ProviderException("Error accessing the data store.", e);
            }
        }

        public override SessionStateStoreData GetItem(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actions)
        {
            Debug.Assert(context != null, "Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            return GetSession(context, id, out locked, out lockAge, out lockId, out actions, false);
        }

        public override SessionStateStoreData GetItemExclusive(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actions)
        {
            Debug.Assert(context != null, "Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            return GetSession(context, id, out locked, out lockAge, out lockId, out actions, true);
        }

        public override void SetAndReleaseItemExclusive(
            HttpContext context, string id, SessionStateStoreData item, object lockId, bool newItem)
        {
            Debug.Assert(context != null, "Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            this.providerRetryPolicy(() =>
            {
                TableServiceContext svc = CreateDataServiceContext();
                SessionRow session;

                if (!newItem)
                {
                    session = GetSession(id, svc);
                    if (session == null || session.Lock != (int)lockId)
                    {
                        Debug.Assert(false, "Error");
                        return;
                    }
                }
                else
                {
                    session = new SessionRow(id, this.applicationName);
                    session.Lock = 1;
                    session.LockDateUtc = DateTime.UtcNow;
                }

                session.Initialized = true;
                Debug.Assert(session.Timeout >= 0, "Session timeout is < 0");
                session.Timeout = item.Timeout;
                session.ExpiresUtc = DateTime.UtcNow.AddMinutes(session.Timeout);
                session.Locked = false;

                // yes, we always create a new blob here
                session.BlobName = GetBlobNamePrefix(id) + Guid.NewGuid().ToString("N");
                
                // Serialize the session and write the blob
                byte[] items, statics;
                SerializeSession(item, out items, out statics);
                string serializedItems = Convert.ToBase64String(items);
                string serializedStatics = Convert.ToBase64String(statics);
                MemoryStream output = new MemoryStream();
                StreamWriter writer = new StreamWriter(output);

                try
                {
                    writer.WriteLine(serializedItems);
                    writer.WriteLine(serializedStatics);
                    writer.Flush();

                    // for us, it shouldn't matter whether newItem is set to true or false
                    // because we always create the entire blob and cannot append to an 
                    // existing one
                    this.blobProvider.UploadStream(session.BlobName, output);
                    writer.Close();
                    output.Close();
                }
                catch (Exception e)
                {
                    if (!newItem)
                    {
                        ReleaseItemExclusive(svc, session, lockId);
                    }

                    throw new ProviderException("Error accessing the data store.", e);
                }
                finally
                {
                    if (writer != null)
                    {
                        writer.Close();
                    }

                    if (output != null)
                    {
                        output.Close();
                    }
                }

                if (newItem)
                {
                    svc.AddObject(this.tableName, session);
                    svc.SaveChangesWithRetries();
                }
                else
                {
                    // Unlock the session and save changes
                    ReleaseItemExclusive(svc, session, lockId);
                }
            });
        }

        public override void ReleaseItemExclusive(HttpContext context, string id, object lockId)
        {
            Debug.Assert(context != null, "Http context is null");
            Debug.Assert(lockId != null, "Lock Id is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                SessionRow session = GetSession(id, svc);
                ReleaseItemExclusive(svc, session, lockId);
            }
            catch (InvalidOperationException e)
            {
                throw new ProviderException("Error accessing the data store!", e);
            }
        }

        public override void ResetItemTimeout(HttpContext context, string id)
        {
            Debug.Assert(context != null, "Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            this.providerRetryPolicy(() =>
            {
                TableServiceContext svc = CreateDataServiceContext();
                SessionRow session = GetSession(id, svc);
                session.ExpiresUtc = DateTime.UtcNow.AddMinutes(session.Timeout);
                svc.UpdateObject(session);
                svc.SaveChangesWithRetries();
            });
        }

        public override void RemoveItem(HttpContext context, string id, object lockId, SessionStateStoreData item)
        {
            Debug.Assert(context != null, "Http context is null");
            Debug.Assert(lockId != null, "Lock Id is null");
            Debug.Assert(this.blobProvider != null, "Blob provider is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                SessionRow session = GetSession(id, svc);
                if (session == null)
                {
                    Debug.Assert(false, "Error");
                    return;
                }

                if (session.Lock != (int)lockId)
                {
                    Debug.Assert(false, "Error");
                    return;
                }

                svc.DeleteObject(session);
                svc.SaveChangesWithRetries();
            }
            catch (InvalidOperationException e)
            {
                throw new ProviderException("Error accessing the data store!", e);
            }

            // delete associated blobs
            try
            {
                IEnumerable<IListBlobItem> e = this.blobProvider.ListBlobs(this.GetBlobNamePrefix(id));
                if (e == null)
                {
                    return;
                }

                IEnumerator<IListBlobItem> props = e.GetEnumerator();
                if (props == null)
                {
                    return;
                }

                while (props.MoveNext())
                {
                    if (props.Current != null)
                    {
                        if (!this.blobProvider.DeleteBlob(props.Current.Uri.ToString()))
                        {
                            // ignore this; it is possible that another thread could try to delete the blob
                            // at the same time
                        }
                    }
                }
            }
            catch (Exception e)
            {
                throw new ProviderException("Error accessing blob storage.", e);
            }
        }

        public override bool SetItemExpireCallback(SessionStateItemExpireCallback expireCallback)
        {
            // This provider doesn't support expiration callbacks
            // so simply return false here
            return false;
        }

        public override void InitializeRequest(HttpContext context)
        {
            // no specific logic for initializing requests in this provider
        }

        public override void EndRequest(HttpContext context)
        {
            // no specific logic for ending requests in this provider
        }

        // nothing can be done here because there might be session managers at different machines involved in
        // handling sessions
        public override void Dispose()
        {
        }

        #endregion

        #region Helper methods

        private static void SerializeSession(SessionStateStoreData store, out byte[] items, out byte[] statics)
        {
            bool hasItems = store.Items != null && store.Items.Count > 0;
            bool hasStaticObjects = store.StaticObjects != null && store.StaticObjects.Count > 0 && !store.StaticObjects.NeverAccessed;
            items = null;
            statics = new byte[0];

            using (MemoryStream stream1 = new MemoryStream())
            {
                using (BinaryWriter writer1 = new BinaryWriter(stream1))
                {
                    writer1.Write(hasItems);
                    if (hasItems)
                    {
                        ((SessionStateItemCollection)store.Items).Serialize(writer1);
                    }

                    items = stream1.ToArray();
                }
            }

            if (hasStaticObjects)
            {
                throw new ProviderException("Static objects are not supported in this provider because of security-related hosting constraints.");
            }
        }

        private static SessionStateStoreData DeserializeSession(byte[] items, byte[] statics, int timeout)
        {
            SessionStateItemCollection itemCol = null;

            using (MemoryStream stream1 = new MemoryStream(items))
            {
                using (BinaryReader reader1 = new BinaryReader(stream1))
                {
                    bool hasItems = reader1.ReadBoolean();
                    if (hasItems)
                    {
                        itemCol = SessionStateItemCollection.Deserialize(reader1);
                    }
                    else
                    {
                        itemCol = new SessionStateItemCollection();
                    }
                }
            }

            if (HttpContext.Current != null && HttpContext.Current.Application != null &&
                HttpContext.Current.Application.StaticObjects != null && HttpContext.Current.Application.StaticObjects.Count > 0)
            {
                throw new ProviderException("This provider does not support static session objects because of security-related hosting constraints.");
            }

            if (statics != null && statics.Count() > 0)
            {
                throw new ProviderException("This provider does not support static session objects because of security-related hosting constraints.");
            }

            return new SessionStateStoreData(itemCol, null, timeout);
        }

        private static void ReleaseItemExclusive(TableServiceContext svc, SessionRow session, object lockId)
        {
            if ((int)lockId != session.Lock)
            {
                // obviously that can happen, but let's see when at least in Debug mode
                Debug.Assert(false, "Error");
                return;
            }

            session.ExpiresUtc = DateTime.UtcNow.AddMinutes(session.Timeout);
            session.Locked = false;
            svc.UpdateObject(session);
            svc.SaveChangesWithRetries();
        }

        private SessionRow GetSession(string id)
        {
            DataServiceContext svc = this.CreateDataServiceContext();
            return GetSession(id, svc);
        }
        
        private SessionRow GetSession(string id, DataServiceContext context)
        {
            Debug.Assert(context != null, "Http context is null");
            Debug.Assert(id != null && id.Length <= ProvidersConfiguration.MaxStringPropertySizeInChars, "Id is null or longer than allowed");

            try
            {
                DataServiceQuery<SessionRow> queryObj = context.CreateQuery<SessionRow>(this.tableName);
                var query = (from session in queryObj
                             where session.PartitionKey == SecUtility.CombineToKey(this.applicationName, id)
                             select session).AsTableServiceQuery();
                IEnumerable<SessionRow> sessions = query.Execute();

                // enumerate the result and store it in a list
                var sessionList = new List<SessionRow>(sessions);
                if (sessionList != null && sessionList.Count() == 1)
                {
                    return sessionList.First();
                }
                else if (sessionList != null && sessionList.Count() > 1)
                {
                    throw new ProviderException("Multiple sessions with the same name!");
                }
                else
                {
                    return null;
                }
            }
            catch (Exception e)
            {
                throw new ProviderException("Error accessing storage.", e);
            }
        }

        private TableServiceContext CreateDataServiceContext()
        {
            return this.tableStorage.GetDataServiceContext();
        }

        // we don't use the retry policy itself in this function because out parameters are not well handled by 
        // retry policies
        private SessionStateStoreData GetSession(
            HttpContext context,
            string id,
            out bool locked,
            out TimeSpan lockAge,
            out object lockId,
            out SessionStateActions actions,
            bool exclusive)
        {
            Debug.Assert(context != null, "The Http context is null");
            SecUtility.CheckParameter(ref id, true, true, false, ProvidersConfiguration.MaxStringPropertySizeInChars, "id");

            SessionRow session = null;

            int curRetry = 0;
            bool retry = false;

            // Assign default values to out parameters
            locked = false;
            lockId = null;
            lockAge = TimeSpan.Zero;
            actions = SessionStateActions.None;

            do
            {
                retry = false;
                try
                {
                    TableServiceContext svc = this.CreateDataServiceContext();
                    session = GetSession(id, svc);

                    // Assign default values to out parameters
                    locked = false;
                    lockId = null;
                    lockAge = TimeSpan.Zero;
                    actions = SessionStateActions.None;

                    // if the blob does not exist, we return null
                    // ASP.NET will call the corresponding method for creating the session
                    if (session == null)
                    {
                        return null;
                    }

                    if (session.Initialized == false)
                    {
                        Debug.Assert(session.Locked == false, "The session is locked");
                        actions = SessionStateActions.InitializeItem;
                        session.Initialized = true;
                    }

                    session.ExpiresUtc = DateTime.UtcNow.AddMinutes(session.Timeout);
                    if (exclusive)
                    {
                        if (!session.Locked)
                        {
                            if (session.Lock == int.MaxValue)
                            {
                                session.Lock = 0;
                            }
                            else
                            {
                                session.Lock++;
                            }

                            session.LockDateUtc = DateTime.UtcNow;
                        }

                        lockId = session.Lock;
                        locked = session.Locked;
                        session.Locked = true;
                    }

                    lockAge = DateTime.UtcNow.Subtract(session.LockDateUtc);
                    lockId = session.Lock;

                    if (locked == true)
                    {
                        return null;
                    }

                    // let's try to write this back to the data store
                    // in between, someone else could have written something to the store for the same session
                    // we retry a number of times; if all fails, we throw an exception
                    svc.UpdateObject(session);
                    svc.SaveChangesWithRetries();
                }
                catch (InvalidOperationException e)
                {
                    // precondition fails indicates problems with the status code
                    if (e.InnerException is DataServiceClientException && (e.InnerException as DataServiceClientException).StatusCode == (int)HttpStatusCode.PreconditionFailed)
                    {
                        retry = true;
                    }
                    else
                    {
                        throw new ProviderException("Error accessing the data store.", e);
                    }
                }
            } while (retry && curRetry++ < NumRetries);

            // ok, now we have successfully written back our state
            // we can now read the blob 
            // note that we do not need to care about read/write locking when accessing the 
            // blob because each time we write a new session we create a new blob with a different name
            SessionStateStoreData result = null;
            MemoryStream stream = null;
            StreamReader reader = null;
            try
            {
                try
                {
                    BlobProperties properties;
                    stream = this.blobProvider.GetBlobContent(session.BlobName, out properties);
                }
                catch (Exception e)
                {
                    throw new ProviderException("Couldn't read session blob!", e);
                }

                reader = new StreamReader(stream);
                if (actions == SessionStateActions.InitializeItem)
                {
                    // Return an empty SessionStateStoreData                    
                    result = new SessionStateStoreData(
                        new SessionStateItemCollection(),
                        SessionStateUtility.GetSessionStaticObjects(context),
                        session.Timeout);
                }
                else
                {
                    // Read Items, StaticObjects, and Timeout from the file
                    byte[] items = Convert.FromBase64String(reader.ReadLine());
                    byte[] statics = Convert.FromBase64String(reader.ReadLine());
                    int timeout = session.Timeout;
                    
                    // Deserialize the session
                    result = DeserializeSession(items, statics, timeout);
                }
            }
            finally
            {
                if (stream != null)
                {
                    stream.Close();
                }

                if (reader != null)
                {
                    reader.Close();
                }
            }

            return result;
        }

        private string GetBlobNamePrefix(string id)
        {
            return string.Format(CultureInfo.InstalledUICulture, "{0}{1}", id, this.applicationName);
        }

        #endregion
    }
}
