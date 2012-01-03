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
    using System.Text.RegularExpressions;

    internal static class InnerExtensionMethods
    {
        internal const string BlobPublicLevelHeaderName = "x-ms-blob-public-access";
        internal const string MetadataHeaderPrefix = "x-ms-meta-";

        internal static string ToHeaderValue(this BlobContainerPublicAccessType blobContainerPublicAccessType)
        {
            switch (blobContainerPublicAccessType)
            {
                case BlobContainerPublicAccessType.Container:
                    return "container";

                case BlobContainerPublicAccessType.Blob:
                    return "blob";

                default: return string.Empty;
            }
        }

        internal static BlobProperties ToBlobProperties(this InnerBlobProperties blobProperties)
        {
            if (blobProperties == null)
                return new BlobProperties();

            return new BlobProperties
            {
                BlobType = Helpers.PaseEnum<BlobType>(blobProperties.BlobType),
                CacheControl = blobProperties.CacheControl,
                ContentEncoding = blobProperties.ContentEncoding,
                ContentLanguage = blobProperties.ContentLanguage,
                ContentMD5 = blobProperties.ContentMD5,
                ContentType = blobProperties.ContentType,
                ETag = blobProperties.ETag,
                LastModifiedUtc = Helpers.ParseDateTimeUtc(blobProperties.LastModifiedUtc),
                LeaseStatus = Helpers.PaseEnum<LeaseStatus>(blobProperties.LeaseStatus),
                Length = blobProperties.Length,
            };
        }

        internal static BlobContainerProperties ToBlobContainerProperties(this InnerBlobContainerProperties blobContainerProperties)
        {
            if (blobContainerProperties == null)
                return new BlobContainerProperties();

            return new BlobContainerProperties
            {
                ETag = blobContainerProperties.ETag,
                LastModifiedUtc = Helpers.ParseDateTimeUtc(blobContainerProperties.LastModifiedUtc)
            };
        }

        internal static string GetResponseString(this WebResponse response)
        {
            if (response == null)
                return string.Empty;

            try
            {
                var stream = response.GetResponseStream();
                stream.Position = 0;

                var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
            catch (NotSupportedException)
            {
                return string.Empty;
            }
            catch (ArgumentException)
            {
                return string.Empty;
            }
        }

        internal static void AddBlobPropertiesHeaders(this WebHeaderCollection headers, BlobProperties blobProperties)
        {
            if ((headers != null) && (blobProperties != null))
            {
                if (!string.IsNullOrWhiteSpace(blobProperties.ContentType))
                    headers["x-ms-blob-content-type"] = blobProperties.ContentType;

                if (!string.IsNullOrWhiteSpace(blobProperties.ContentMD5))
                    headers["x-ms-blob-content-md5"] = blobProperties.ContentMD5;

                if (!string.IsNullOrWhiteSpace(blobProperties.ContentLanguage))
                    headers["x-ms-blob-content-language"] = blobProperties.ContentLanguage;

                if (!string.IsNullOrWhiteSpace(blobProperties.ContentEncoding))
                    headers["x-ms-blob-content-encoding"] = blobProperties.ContentEncoding;

                if (!string.IsNullOrWhiteSpace(blobProperties.CacheControl))
                    headers["x-ms-blob-cache-control"] = blobProperties.CacheControl;
            }
        }

        internal static void AddContainerPublicAccessTypeHeader(this WebHeaderCollection headers, BlobContainerPublicAccessType blobContainerPublicAccessType)
        {
            if ((headers != null) && (blobContainerPublicAccessType != BlobContainerPublicAccessType.Off))
                headers.Add(BlobPublicLevelHeaderName, blobContainerPublicAccessType.ToHeaderValue());
        }

        internal static void AddMetadataHeaders(this WebHeaderCollection headers, IDictionary<string, string> metadata)
        {
            if ((metadata != null) && (metadata.Keys.Count > 0))
            {
                foreach (var metadataName in metadata.Keys)
                {
                    var metadataHeader = string.Concat(MetadataHeaderPrefix, metadataName);
                    headers.Add(metadataHeader, metadata[metadataName]);
                }
            }
        }

        internal static void Add(this WebHeaderCollection headers, string key, string value)
        {
            headers[key] = value;
        }

        internal static void Add(this WebHeaderCollection headers, HttpRequestHeader key, string value)
        {
            headers[key] = value;
        }

        internal static string GetValue(this WebHeaderCollection headers, string key)
        {
            var headerName = headers.AllKeys.FirstOrDefault(h => h.Equals(key, StringComparison.OrdinalIgnoreCase));
            return string.IsNullOrWhiteSpace(headerName) ? string.Empty : headers[headerName];
        }

        internal static IDictionary<string, string> GetMetadataHeaders(this WebHeaderCollection headers)
        {
            var metadata = new Dictionary<string, string>();
            var metadataHeaders = headers.AllKeys.Where(h => h.StartsWith(MetadataHeaderPrefix, StringComparison.OrdinalIgnoreCase));
            foreach (var metadataHeader in metadataHeaders)
            {
                var key = Regex.Replace(metadataHeader, MetadataHeaderPrefix, string.Empty, RegexOptions.IgnoreCase);
                var value = headers[metadataHeader];
                if (value != null)
                    metadata.Add(key, value);
            }

            return metadata;
        }

        internal static IEnumerable<string> GetValues(this WebHeaderCollection headers, string key)
        {
            var list = new List<string>();
            foreach (var k in headers.AllKeys)
            {
                if (k.Equals(key, StringComparison.OrdinalIgnoreCase))
                    list.Add(headers[k]);
            }

            return list;
        }

        internal static bool IsSasExpired(this Uri sharedAccessSignatureUri)
        {
            if (sharedAccessSignatureUri == null)
                return true;

            var se = Helpers.ParseQueryString(sharedAccessSignatureUri.Query)["se"];
            if (string.IsNullOrWhiteSpace(se))
                return true;

            DateTime expirationDate;
            if (!DateTime.TryParseExact(se, "yyyy-MM-ddTHH:mm:ssZ", CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out expirationDate))
                return true;

            return expirationDate <= DateTime.UtcNow;
        }
    }
}
