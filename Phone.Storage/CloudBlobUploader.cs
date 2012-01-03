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
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Net.Browser;
    using System.Text;
    using System.Windows.Threading;
    using System.Xml;
    using System.Xml.Linq;

    internal class CloudBlobUploader
    {
        private const long ChunkSize = 4194304;

        private readonly Stream fileStream;
        private readonly Uri cloudBlobUri;
        private readonly IDictionary<string, string> metadata;
        private readonly BlobProperties blobProperties;
        private readonly IStorageCredentials credentials;
        private readonly long dataLength;
        private readonly IList<string> blockIds = new List<string>();
        private readonly bool useBlocks;
        private readonly Dispatcher dispatcher;

        private long dataSent;
        private string currentBlockId;
        private Action<CloudOperationResponse<bool>> uploadFinishedCallback;

        public CloudBlobUploader(Stream fileStream, Uri cloudBlobUri, IDictionary<string, string> metadata, BlobProperties blobProperties, IStorageCredentials credentials, Dispatcher dispatcher)
        {
            this.fileStream = fileStream;
            this.cloudBlobUri = cloudBlobUri;
            this.metadata = metadata;
            this.blobProperties = blobProperties;
            this.credentials = credentials;

            this.dataLength = this.fileStream.Length;
            this.dataSent = 0;

            this.dispatcher = dispatcher;

            // Upload the blob in smaller blocks if it's a "big" stream.
            this.useBlocks = (this.dataLength - this.dataSent) > ChunkSize;
        }

        public void Upload(Action<CloudOperationResponse<bool>> callback)
        {
            this.uploadFinishedCallback = callback;
            this.InnerUpload();
        }

        public void InnerUpload()
        {
            var uriBuilder = new UriBuilder(this.cloudBlobUri);

            // Set a timeout query string parameter.
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "timeout=10000" : "&timeout=10000");

            if (this.useBlocks)
            {
                // Encode the block name and add it to the query string.
                this.currentBlockId = Convert.ToBase64String(Encoding.UTF8.GetBytes(Guid.NewGuid().ToString()));
                uriBuilder.Query = string.Format(
                    CultureInfo.InvariantCulture,
                    "{0}&comp=block&blockid={1}",
                    uriBuilder.Query.TrimStart('?'),
                    this.currentBlockId);
            }

            // Make a PUT request to submitt the blob's data.
            var request = this.ResolveRequest(uriBuilder.Uri);
            request.Method = "PUT";

            request.Headers["x-ms-version"] = "2011-08-18";
            request.Headers["x-ms-blob-type"] = "BlockBlob";

            if (!this.useBlocks)
            {
                request.Headers.AddBlobPropertiesHeaders(this.blobProperties);
                request.Headers.AddMetadataHeaders(this.metadata);
            }

            request.BeginGetRequestStream(this.WriteBlobRequestStream, request);
        }

        protected virtual HttpWebRequest ResolveRequest(Uri requestUri)
        {
            return (HttpWebRequest)WebRequestCreator.ClientHttp.Create(requestUri);
        }

        private void WriteBlobRequestStream(IAsyncResult asyncResult)
        {
            var request = (HttpWebRequest)asyncResult.AsyncState;
            var requestStream = request.EndGetRequestStream(asyncResult);
            var buffer = new byte[4096];
            int bytesRead;
            var tempTotal = 0;
            this.fileStream.Position = this.dataSent;

            while (((bytesRead = this.fileStream.Read(buffer, 0, buffer.Length)) != 0) && (tempTotal + bytesRead < ChunkSize))
            {
                requestStream.Write(buffer, 0, bytesRead);
                requestStream.Flush();

                this.dataSent += bytesRead;
                tempTotal += bytesRead;
            }

            requestStream.Close();

            if (this.credentials != null)
                this.credentials.SignRequest(request, tempTotal);

            request.BeginGetResponse(this.ReadBlobHttpResponse, request);
        }

        private void ReadBlobHttpResponse(IAsyncResult asyncResult)
        {
            try
            {
                var request = (HttpWebRequest)asyncResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asyncResult);
                
                if (this.useBlocks)
                    this.blockIds.Add(this.currentBlockId);

                // If there is more data, send another request.
                if (this.dataSent < this.dataLength)
                {
                    this.InnerUpload();
                }
                else
                {
                    this.fileStream.Close();
                    this.fileStream.Dispose();

                    if (this.useBlocks)
                    {
                        // Commit the block list into the blob.
                        this.PutBlockList();
                    }
                    else
                    {
                        if (response.StatusCode == HttpStatusCode.Created)
                            this.DispatchCallback(new CloudOperationResponse<bool>(true, response.StatusCode, true, string.Empty));
                        else
                            this.DispatchCallback(new CloudOperationResponse<bool>(false, response.StatusCode, false, string.Format(CultureInfo.InvariantCulture, "Error uploading blob: {0}", response.StatusDescription)));
                    }
                }
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse)exception.Response;
                this.DispatchCallback(new CloudOperationResponse<bool>(false, response.StatusCode, false, Helpers.ParseXmlWebException(exception)));
            }
        }

        private void PutBlockList()
        {
            var uriBuilder = new UriBuilder(this.cloudBlobUri);
            uriBuilder.Query = string.Format(
                CultureInfo.InvariantCulture,
                "{0}{1}",
                uriBuilder.Query.TrimStart('?'),
                string.IsNullOrWhiteSpace(uriBuilder.Query) ? "comp=blocklist" : "&comp=blocklist");

            var request = this.ResolveRequest(uriBuilder.Uri);
            request.Method = "PUT";

            request.Headers["x-ms-version"] = "2011-08-18";
            request.Headers.AddBlobPropertiesHeaders(this.blobProperties);
            request.Headers.AddMetadataHeaders(this.metadata);

            request.BeginGetRequestStream(this.WriteBlockListRequestStream, request);
        }

        private void WriteBlockListRequestStream(IAsyncResult asyncResult)
        {
            var request = (HttpWebRequest)asyncResult.AsyncState;
            var requestStream = request.EndGetRequestStream(asyncResult);
            var document = new XDocument(new XElement("BlockList", from blockId in this.blockIds select new XElement("Uncommitted", blockId)));
            var writer = XmlWriter.Create(requestStream, new XmlWriterSettings { Encoding = Encoding.UTF8 });

            document.Save(writer);
            writer.Flush();

            long length = requestStream.Length;
            requestStream.Close();

            if (this.credentials != null)
                this.credentials.SignRequest(request, length);

            request.BeginGetResponse(this.ReadBlockListHttpResponse, request);
        }

        private void ReadBlockListHttpResponse(IAsyncResult asyncResult)
        {
            try
            {
                var request = (HttpWebRequest)asyncResult.AsyncState;
                var response = (HttpWebResponse)request.EndGetResponse(asyncResult);

                if (response.StatusCode == HttpStatusCode.Created)
                    this.DispatchCallback(new CloudOperationResponse<bool>(true, response.StatusCode, true, string.Empty));
                else
                    this.DispatchCallback(new CloudOperationResponse<bool>(false, response.StatusCode, false, string.Format(CultureInfo.InvariantCulture, "Error uploading blob: {0}", response.StatusDescription)));
            }
            catch (WebException exception)
            {
                var response = (HttpWebResponse)exception.Response;
                this.DispatchCallback(new CloudOperationResponse<bool>(false, response.StatusCode, false, Helpers.ParseXmlWebException(exception)));
            }
        }

        private void DispatchCallback(CloudOperationResponse<bool> response)
        {
            if (this.uploadFinishedCallback != null)
            {
                if (this.dispatcher != null)
                    this.dispatcher.BeginInvoke(() => this.uploadFinishedCallback(response));
                else
                    this.uploadFinishedCallback(response);
            }
        }
    }
}
