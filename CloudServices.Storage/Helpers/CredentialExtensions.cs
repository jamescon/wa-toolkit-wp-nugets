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
    using System.Collections;
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Globalization;
    using System.Linq;
    using System.Net.Http;
    using System.Net.Http.Headers;
    using System.Text;
    using System.Web;

    internal static class CredentialExtensions
    {
        private const string CanonicalEol = "\n";

        internal static void SignLite(this HttpRequestMessage request, CloudStorageAccount account)
        {
            string str = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);

            var headerName = request.Headers.Where(h => h.Key.Equals("x-ms-date", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(headerName))
                request.Headers.Remove(headerName);
            request.Headers.Add("x-ms-date", str);

            var canonicalizedResource = GetCanonicalizedResourceLite(request.RequestUri, account.Credentials.AccountName);
            var canonicalizedString = new StringBuilder();
            canonicalizedString.Append(str);
            canonicalizedString.Append(CanonicalEol);
            canonicalizedString.Append(canonicalizedResource);

            string signedString = account.Credentials.ComputeHmac(canonicalizedString.ToString());

            headerName = request.Headers.Where(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(headerName))
                request.Headers.Remove(headerName);
            request.Headers.Add("Authorization", string.Format(CultureInfo.InvariantCulture, "SharedKeyLite {0}:{1}", new object[] { account.Credentials.AccountName, signedString }));
        }

        internal static void Sign(this HttpRequestMessage request, CloudStorageAccount account)
        {
            string str = DateTime.UtcNow.ToString("R", CultureInfo.InvariantCulture);
            var headerName = request.Headers.Where(h => h.Key.Equals("x-ms-date", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).FirstOrDefault();

            if (!string.IsNullOrEmpty(headerName))
                request.Headers.Remove(headerName);
            request.Headers.Add("x-ms-date", str);

            var canonicalizedString = new CanonicalizedString(request.Method.ToString());

            if (request.Content == null)
            {
                canonicalizedString.AppendCanonicalizedElement(string.Empty);
                canonicalizedString.AppendCanonicalizedElement(string.Empty);

                if (request.Method == HttpMethod.Put || request.Method == HttpMethod.Post || request.Method == HttpMethod.Delete)
                    canonicalizedString.AppendCanonicalizedElement("0");
                else
                {
                    canonicalizedString.AppendCanonicalizedElement(string.Empty);                    
                }

                canonicalizedString.AppendCanonicalizedElement(string.Empty);
                canonicalizedString.AppendCanonicalizedElement(string.Empty);
            }
            else
            {
                canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentEncoding.ToString());
                canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentLanguage.ToString());
                canonicalizedString.AppendCanonicalizedElement((request.Content.Headers.ContentLength > 0) ? request.Content.Headers.ContentLength.ToString() : string.Empty);

                canonicalizedString.AppendCanonicalizedElement(
                    request.Content.Headers.ContentMD5 != null
                        ? request.Content.Headers.ContentMD5.ToString()
                        : string.Empty);

                canonicalizedString.AppendCanonicalizedElement(request.Content.Headers.ContentType.ToString());
            }

            canonicalizedString.AppendCanonicalizedElement(string.Empty);

            canonicalizedString.AppendCanonicalizedElement(request.Headers.IfModifiedSince.ToString());
            canonicalizedString.AppendCanonicalizedElement(request.Headers.IfMatch.ToString());
            canonicalizedString.AppendCanonicalizedElement(request.Headers.IfNoneMatch.ToString());
            canonicalizedString.AppendCanonicalizedElement(request.Headers.IfUnmodifiedSince.ToString());

            canonicalizedString.AppendCanonicalizedElement(request.Headers.Range != null ? request.Headers.Range.ToString() : string.Empty);

            canonicalizedString.AppendCanonicalizedElement(CanonicalizedHeaders(request.Headers));

            canonicalizedString.AppendCanonicalizedElement(
                GetCanonicalizedResourceVersion2(
                    request.RequestUri, account.Credentials.AccountName).TrimEnd('\n'));

            var stringToSign = canonicalizedString.Value;
            string signedString = account.Credentials.ComputeHmac(stringToSign);

            headerName = request.Headers.Where(h => h.Key.Equals("Authorization", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).FirstOrDefault();
            if (!string.IsNullOrEmpty(headerName))
                request.Headers.Remove(headerName);
            request.Headers.Add("Authorization", string.Format(CultureInfo.InvariantCulture, "SharedKey {0}:{1}", new object[] { account.Credentials.AccountName, signedString }));
        }

        internal static string GetCanonicalizedResourceLite(Uri address, string accountName)
        {
            var builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);

            var parsedQuery = HttpUtility.ParseQueryString(address.Query);
            var compQuery = parsedQuery["comp"] ?? string.Empty;
            if (!string.IsNullOrWhiteSpace(compQuery))
            {
                builder.Append("?comp=");
                builder.Append(compQuery);
            }

            return builder.ToString();
        }

        internal static string GetCanonicalizedResourceVersion2(Uri address, string accountName)
        {
            var builder = new StringBuilder("/");
            builder.Append(accountName);
            builder.Append(address.AbsolutePath);
            var str = new CanonicalizedString(builder.ToString());
            NameValueCollection values = HttpUtility.ParseQueryString(address.Query);
            var values2 = new NameValueCollection();
            foreach (string str2 in values.Keys)
            {
                var list = new ArrayList(values.GetValues(str2));
                list.Sort();
                var builder2 = new StringBuilder();
                foreach (object obj2 in list)
                {
                    if (builder2.Length > 0)
                    {
                        builder2.Append(",");
                    }

                    builder2.Append(obj2.ToString());
                }

                values2.Add((str2 == null) ? str2 : str2.ToLowerInvariant(), builder2.ToString());
            }

            var list2 = new ArrayList(values2.AllKeys);
            list2.Sort();
            foreach (string str3 in list2)
            {
                var builder3 = new StringBuilder(string.Empty);
                builder3.Append(str3);
                builder3.Append(":");
                builder3.Append(values2[str3]);
                str.AppendCanonicalizedElement(builder3.ToString());
            }

            return str.Value;
        }

        internal static string CanonicalizedHeaders(HttpRequestHeaders headers)
        {
            var canonicalizedString = new CanonicalizedString(string.Empty);

            var keyList = 
                headers.Where(h => h.Key.StartsWith("x-ms-", StringComparison.OrdinalIgnoreCase)).Select(header => header.Key).ToList();

            keyList.Sort();
            foreach (string str2 in keyList)
            {
                var builder = new StringBuilder(str2);
                string str3 = ":";
                foreach (string str4 in GetHeaderValues(headers, str2))
                {
                    string str5 = str4.Replace("\r\n", string.Empty);
                    builder.Append(str3);
                    builder.Append(str5);
                    str3 = ",";
                }

                canonicalizedString.AppendCanonicalizedElement(builder.ToString());
            }

            return canonicalizedString.Value.TrimEnd('\n').TrimStart('\n');
        }

        internal static IEnumerable<string> GetHeaderValues(HttpRequestHeaders headers, string headerName)
        {
            var list = new List<string>();
            var values = headers.GetValues(headerName);
            if (values != null)
            {
                list.AddRange(values.Select(value => value.TrimStart(new char[0])));
            }

            return list;
        }
    }
}