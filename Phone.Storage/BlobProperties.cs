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

    public class BlobProperties
    {
        public BlobType BlobType { get; internal set; }
        
        public string CacheControl { get; set; }
        
        public string ContentEncoding { get; set; }
        
        public string ContentLanguage { get; set; }

        public string ContentMD5 { get; set; }

        public string ContentType { get; set; }
 
        public string ETag { get; internal set; }
       
        public DateTime LastModifiedUtc { get; internal set; }
        
        public LeaseStatus LeaseStatus { get; internal set; }
        
        public long Length { get; internal set; }
    }
}
