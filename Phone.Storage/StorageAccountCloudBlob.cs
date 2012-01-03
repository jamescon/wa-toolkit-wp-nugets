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
    using System.Net;

    public class StorageAccountCloudBlob : CloudBlobBase
    {
        public StorageAccountCloudBlob(Uri uri, string name, ICloudBlobContainer container)
            : base(uri, name, container)
        {
        }

        protected override Uri BaseOperationUri
        {
            get { return this.Uri; }
        }

        public override void GetSharedAccessSignature(Action<CloudOperationResponse<Uri>> callback)
        {
            throw new NotSupportedException("This operation is not supported in the StorageAccountCloudBlob implementation.");
        }

        protected override void OnSendingRequest(HttpWebRequest request, int contentLength)
        {
            if (request != null)
            {
                // Add x-ms-version header.
                request.Headers["x-ms-version"] = "2011-08-18";

                // Sign the request to add the authorization header.
                this.Credentials.SignRequest(request, contentLength);
            }
        }
    }
}