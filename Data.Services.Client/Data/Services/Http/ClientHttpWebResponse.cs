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

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Samples.Data.Services.Client
{
    using System;
    using System.Diagnostics;
    using System.IO;

    internal sealed class ClientHttpWebResponse : Microsoft.WindowsAzure.Samples.Data.Services.Client.HttpWebResponse
    {
        private System.Net.HttpWebResponse innerResponse;

        private ClientWebHeaderCollection headerCollection;

        private ClientHttpWebRequest request;

        internal ClientHttpWebResponse(System.Net.HttpWebResponse innerResponse, ClientHttpWebRequest request)
        {
            Debug.Assert(innerResponse != null, "innerResponse can't be null.");
            this.innerResponse = innerResponse;
            this.request = request;
            int statusCode = (int)this.innerResponse.StatusCode;
            if (statusCode > (int)HttpStatusCodeRange.MaxValue || statusCode < (int)HttpStatusCodeRange.MinValue)
            {
                throw WebException.CreateInternal("HttpWebResponse.NormalizeResponseStatus");
            }
        }

        #region Properties.

        public override long ContentLength
        {
            get
            {
                return this.innerResponse.ContentLength;
            }
        }

        public override string ContentType
        {
            get
            {
                return this.innerResponse.ContentType;
            }
        }

        public override Microsoft.WindowsAzure.Samples.Data.Services.Client.WebHeaderCollection Headers
        {
            get
            {
                if (this.headerCollection == null)
                {
                    this.headerCollection = new ClientWebHeaderCollection(this.innerResponse.Headers);
                }

                return this.headerCollection;
            }
        }

        public override Microsoft.WindowsAzure.Samples.Data.Services.Client.HttpWebRequest Request
        {
            get
            {
                return this.request;
            }
        }

        public override Microsoft.WindowsAzure.Samples.Data.Services.Client.HttpStatusCode StatusCode
        {
            get
            {
                return (Microsoft.WindowsAzure.Samples.Data.Services.Client.HttpStatusCode)(int)this.innerResponse.StatusCode;
            }
        }

        #endregion Properties.

        public override void Close()
        {
            this.innerResponse.Close();
        }

        public override string GetResponseHeader(string headerName)
        {
            return this.innerResponse.Headers[headerName];
        }

        public override Stream GetResponseStream()
        {
            return this.innerResponse.GetResponseStream();
        }

        protected override void Dispose(bool disposing)
        {
            ((IDisposable)this.innerResponse).Dispose();
        }
    }
}
