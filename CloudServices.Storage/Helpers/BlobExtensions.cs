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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage.Helpers
{
    using System;
    using Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties;

    public static class BlobExtensions
    {
        public static SasCloudBlobContainer ToModel(this StorageClient.CloudBlobContainer container)
        {
            if (container == null)
            {
                throw new ArgumentNullException("container", Resource.ContainerCannotBeNullErrorMessage);
            }

            return new SasCloudBlobContainer
            {
                Name = container.Name,
                Url = container.Uri.AbsoluteUri,
                Properties = new SasCloudBlobContainerProperties
                {
                    ETag = container.Properties.ETag,
                    LastModifiedUtc = container.Properties.LastModifiedUtc
                }
            };
        }
    }
}
