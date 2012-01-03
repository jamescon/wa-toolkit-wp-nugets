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
    using System.IO;
    using System.Net;

    public class SharedAccessSignatureServiceCloudBlob : CloudBlobBase
    {
        private readonly ISharedAccessSignatureServiceClient sasServiceClient;
        
        private Uri containerSharedAccessSignatureUri;

        public SharedAccessSignatureServiceCloudBlob(Uri uri, string name, ICloudBlobContainer container, ISharedAccessSignatureServiceClient sasServiceClient)
            : base(uri, name, container)
        {
            if (sasServiceClient == null)
                throw new ArgumentNullException("sasServiceClient", "The Shared Access Signature service client cannot be null.");

            this.sasServiceClient = sasServiceClient;
        }

        public override Uri Uri
        {
            get
            {
                if (this.containerSharedAccessSignatureUri != null)
                    return BuildUriAndCleanQuery(this.containerSharedAccessSignatureUri, BuilUriPath(this.Name));

                return base.Uri;
            }
        }

        protected override Uri BaseOperationUri
        {
            get
            {
                if (this.containerSharedAccessSignatureUri != null)
                    return BuildUri(this.containerSharedAccessSignatureUri, BuilUriPath(this.Name), string.Empty);

                return base.Uri;
            }
        }

        public override void UploadFromStream(Stream source, Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSas<bool>(() => base.UploadFromStream(source, callback), r => this.DispatchCallback(callback, r));
        }

        public override void Delete(Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSas<bool>(() => base.Delete(callback), r => this.DispatchCallback(callback, r));
        }

        public override void FetchAttributes(Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSas<bool>(() => base.FetchAttributes(callback), r => this.DispatchCallback(callback, r));
        }

        public override void SetMetadata(Action<CloudOperationResponse<bool>> callback)
        {
            this.ExecuteWithSas<bool>(() => base.SetMetadata(callback), r => this.DispatchCallback(callback, r));
        }

        public override void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback)
        {
            this.sasServiceClient.GetBlobSharedAccessSignature(this.Container.Name, this.Name, callback);
        }

        protected override void OnSendingRequest(HttpWebRequest request, int contentLength)
        {
            if (request != null)
            {
                // Add x-ms-version header.
                request.Headers["x-ms-version"] = "2011-08-18";
            }
        }

        private void ExecuteWithSas<T>(Action validSasCallback, Action<CloudOperationResponse<T>> invalidSasCallback)
        {
            if (!this.containerSharedAccessSignatureUri.IsSasExpired())
                validSasCallback();
            else
            {
                this.Container.GetSharedAccessSignature(
                    r =>
                    {
                        if (r.Success)
                        {
                            this.containerSharedAccessSignatureUri = r.Response;
                            validSasCallback();
                        }
                        else
                            invalidSasCallback(new CloudOperationResponse<T>(r.Success, r.StatusCode, default(T), r.ErrorMessage));
                    });
            }
        }
    }
}
