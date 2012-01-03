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
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.IO;
    using System.Linq;
    using System.Net;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public enum EventKind
    {
        Critical,

        Error,

        Warning,

        Information,

        Verbose
    }

    internal class BlobProvider
    {
        private static readonly TimeSpan Timeout = TimeSpan.FromSeconds(30);
        private static readonly RetryPolicy RetryPolicy = RetryPolicies.Retry(3, TimeSpan.FromSeconds(1));
        private const string PathSeparator = "/";

        private readonly CloudBlobClient blobClient;
        private readonly string containerName;
        private readonly object lockObject = new object();
        private CloudBlobContainer blobContainer;

        internal BlobProvider(StorageCredentials info, Uri baseUri, string containerName)
        {
            this.containerName = containerName;
            this.blobClient = new CloudBlobClient(baseUri.ToString(), info);
        }

        internal string ContainerUrl
        {
            get
            {
                return string.Join(PathSeparator, new[] { this.blobClient.BaseUri.AbsolutePath, this.containerName });
            }
        }

        public IEnumerable<IListBlobItem> ListBlobs(string folder)
        {
            CloudBlobContainer container = this.GetContainer();
            try
            {
                return container.ListBlobs().Where(blob => blob.Uri.PathAndQuery.StartsWith(folder, StringComparison.Ordinal));
            }
            catch (InvalidOperationException se)
            {
                Log.Write(EventKind.Error, Resource.EnumeratingFolderErrorMessageTemplate, this.ContainerUrl + PathSeparator + folder, se.Message);
                throw;
            }
        }

        internal bool GetBlobContentsWithoutInitialization(string blobName, Stream outputStream, out BlobProperties properties)
        {
            Debug.Assert(outputStream != null, "output strem must not be null");

            CloudBlobContainer container = this.GetContainer();

            try
            {
                var blob = container.GetBlobReference(blobName);

                blob.DownloadToStream(outputStream);

                properties = blob.Properties;
                Log.Write(EventKind.Information, Resource.BlobDownloadMessage, this.blobClient.BaseUri + PathSeparator + this.containerName + PathSeparator + blobName);
                return true;
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException is WebException)
                {
                    var webEx = ex.InnerException as WebException;
                    var resp = webEx.Response as HttpWebResponse;

                    if (resp.StatusCode == HttpStatusCode.NotFound)
                    {
                        properties = null;
                        return false;
                    }
                    else
                    {
                        throw;
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        internal MemoryStream GetBlobContent(string blobName, out BlobProperties properties)
        {
            var blobContent = new MemoryStream();
            properties = this.GetBlobContent(blobName, blobContent);
            blobContent.Seek(0, SeekOrigin.Begin);
            return blobContent;
        }

        internal BlobProperties GetBlobContent(string blobName, Stream outputStream)
        {
            if (string.IsNullOrEmpty(blobName))
            {
                return null;
            }

            CloudBlobContainer container = this.GetContainer();
            try
            {
                var blob = container.GetBlobReference(blobName);

                blob.DownloadToStream(outputStream);

                BlobProperties properties = blob.Properties;
                Log.Write(EventKind.Information, Resource.BlobDownloadMessage, this.ContainerUrl + PathSeparator + blobName);
                return properties;
            }
            catch (InvalidOperationException sc)
            {
                Log.Write(EventKind.Error, Resource.BlobDownloadErrorMessateTemplate, this.ContainerUrl + PathSeparator + blobName, sc.Message);
                throw;
            }
        }

        internal bool UploadStream(string blobName, Stream output)
        {
            CloudBlobContainer container = this.GetContainer();
            try
            {
                output.Position = 0; // Rewind to start
                Log.Write(EventKind.Information, Resource.BlobUploadingMessage, this.ContainerUrl + PathSeparator + blobName);

                var blob = container.GetBlockBlobReference(blobName);

                blob.UploadFromStream(output);

                return true;
            }
            catch (InvalidOperationException se)
            {
                Log.Write(EventKind.Error, Resource.BlobUploadErrorMessageTemplate, this.ContainerUrl + PathSeparator + blobName, se.Message);
                throw;
            }
        }

        internal bool DeleteBlob(string blobName)
        {
            CloudBlobContainer container = this.GetContainer();
            try
            {
                container.GetBlobReference(blobName).Delete();

                return true;
            }
            catch (InvalidOperationException se)
            {
                Log.Write(EventKind.Error, Resource.BlobDeleteErrorMessageTemplate, this.ContainerUrl + PathSeparator + blobName, se.Message);
                throw;
            }
        }

        internal bool DeleteBlobsWithPrefix(string prefix)
        {
            bool ret = true;

            var e = this.ListBlobs(prefix);
            if (e == null)
            {
                return true;
            }

            var props = e.GetEnumerator();

            while (props.MoveNext())
            {
                if (props.Current != null)
                {
                    if (!this.DeleteBlob(props.Current.Uri.ToString()))
                    {
                        // ignore this; it is possible that another thread could try to delete the blob
                        // at the same time
                        ret = false;
                    }
                }
            }

            return ret;
        }

        private CloudBlobContainer GetContainer()
        {
            // we have to make sure that only one thread tries to create the container
            lock (this.lockObject)
            {
                if (this.blobContainer != null)
                {
                    return this.blobContainer;
                }

                try
                {
                    var container = new CloudBlobContainer(this.containerName, this.blobClient);
                    var requestModifiers = new BlobRequestOptions()
                    {
                        Timeout = Timeout,
                        RetryPolicy = RetryPolicy
                    };

                    container.CreateIfNotExist(requestModifiers);

                    this.blobContainer = container;

                    return this.blobContainer;
                }
                catch (InvalidOperationException se)
                {
                    Log.Write(EventKind.Error, "Error creating container {0}: {1}", this.ContainerUrl, se.Message);
                    throw;
                }
            }
        }
    }
}
