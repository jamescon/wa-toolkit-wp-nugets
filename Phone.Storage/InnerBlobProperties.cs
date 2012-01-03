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
    using System.Runtime.Serialization;

    [DataContract(Namespace = "")]
    internal class InnerBlobProperties
    {
        [DataMember]
        public string BlobType { get; internal set; }

        [DataMember(Name = "Cache-Control")]
        public string CacheControl { get; set; }

        [DataMember(Name = "Content-Encoding")]
        public string ContentEncoding { get; set; }

        [DataMember(Name = "Content-Language")]
        public string ContentLanguage { get; set; }

        [DataMember(Name = "Content-MD5")]
        public string ContentMD5 { get; set; }

        [DataMember(Name = "Content-Type")]
        public string ContentType { get; set; }

        [DataMember(Name = "Etag")]
        public string ETag { get; internal set; }

        [DataMember(Name = "Last-Modified")]
        public string LastModifiedUtc { get; internal set; }

        [DataMember(Name = "LeaseStatus")]
        public string LeaseStatus { get; internal set; }

        [DataMember(Name = "Content-Length")]
        public long Length { get; internal set; }
    }
}
