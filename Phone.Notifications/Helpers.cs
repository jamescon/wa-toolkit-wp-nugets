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

namespace Microsoft.WindowsAzure.Samples.Phone.Notifications
{
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;

    internal static class Helpers
    {
        internal static T GetIsolatedStorageSetting<T>(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (!isolatedStorage.ContainsKey(key))
                return default(T);

            return (T)isolatedStorage[key];
        }

        internal static void SetIsolatedStorageSetting(string key, object value)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (isolatedStorage.ContainsKey(key))
                isolatedStorage.Remove(key);

            isolatedStorage.Add(key, value);
        }

        internal static string GetResponseString(this WebResponse response)
        {
            try
            {
                var stream = response.GetResponseStream();
                var reader = new StreamReader(stream);

                return reader.ReadToEnd();
            }
            catch
            {
                return string.Empty;
            }
        }

        internal static string ParseWebException(WebException webException)
        {
            if (webException == null)
                return string.Empty;

            var responseContent = webException.Response.GetResponseString();
            var response = webException.Response as HttpWebResponse;

            if (response != null)
                return string.Format(CultureInfo.InvariantCulture, "{0} {1}", response.StatusCode, responseContent).Trim();

            if (string.IsNullOrWhiteSpace(responseContent))
                return responseContent;

            return webException.GetBaseException().Message;
        }
    }
}