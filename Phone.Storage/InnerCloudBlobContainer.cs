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
    using System.Runtime.Serialization;

    [DataContract(Name = "Container", Namespace = "")]
    internal class InnerCloudBlobContainer
    {
        public InnerCloudBlobContainer()
        {
            this.Properties = new InnerBlobContainerProperties();
        }

        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Url { get; set; }

        [DataMember]
        public InnerBlobContainerProperties Properties { get; set; }

        public Uri Uri
        {
            get
            {
                if (!string.IsNullOrWhiteSpace(this.Url))
                    return new Uri(this.Url, UriKind.Absolute);

                return null;
            }
        }
    }
}
