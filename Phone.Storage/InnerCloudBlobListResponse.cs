﻿// ----------------------------------------------------------------------------------
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
    using System.Collections.Generic;
    using System.Runtime.Serialization;

    [DataContract(Name = "EnumerationResults", Namespace = "")]
    internal class InnerCloudBlobListResponse
    {
        public InnerCloudBlobListResponse()
        {
            this.Blobs = new List<InnerCloudBlob>();
        }

        [DataMember]
        public string Delimiter { get; set; }

        [DataMember]
        public IEnumerable<InnerCloudBlob> Blobs { get; set; }

        [DataMember]
        public string NextMarker { get; set; }
    }
}