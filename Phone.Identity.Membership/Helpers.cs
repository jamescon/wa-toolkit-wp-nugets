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

namespace Microsoft.WindowsAzure.Samples.Phone.Identity.Membership
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.IO.IsolatedStorage;
    using System.Net;
    using Microsoft.Phone.Shell;

    internal static class Helpers
    {
        internal static string GetResponseString(this WebResponse response)
        {
            if (response == null)
                return string.Empty;

            try
            {
                var stream = response.GetResponseStream();
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

        internal static T GetValue<T>(string key, bool persisted)
        {
            return persisted && ContainsIsolatedStorageKey(key)
                ? GetIsolatedStorageValue<T>(key)
                : GetApplicationStateValue<T>(key);
        }

        internal static T GetValue<T>(string key)
        {
            return GetValue<T>(key, true);
        }

        internal static void SetValue(string key, object value, bool persisted)
        {
            if (persisted)
                SetIsolatedStorageValue(key, value);
            
            SetApplicationStateValue(key, value);
        }

        internal static void SetValue(string key, object value)
        {
            SetValue(key, value, true);
        }

        internal static void RemoveKey(string key)
        {
            RemoveIsolatedStorageKey(key);
            
            RemoveApplicationStateValue(key);
        }

        private static T GetIsolatedStorageValue<T>(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (!isolatedStorage.ContainsKey(key))
                return default(T);

            return (T)isolatedStorage[key];
        }

        private static void SetIsolatedStorageValue(string key, object value)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (isolatedStorage.ContainsKey(key))
                isolatedStorage.Remove(key);

            isolatedStorage.Add(key, value);
        }

        private static void RemoveIsolatedStorageKey(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;
            if (isolatedStorage.ContainsKey(key))
                isolatedStorage.Remove(key);
        }

        private static bool ContainsIsolatedStorageKey(string key)
        {
            IDictionary<string, object> isolatedStorage = IsolatedStorageSettings.ApplicationSettings;

            return isolatedStorage.ContainsKey(key);
        }

        private static T GetApplicationStateValue<T>(string key)
        {
            if (!PhoneApplicationService.Current.State.ContainsKey(key))
                return default(T);

            return (T)PhoneApplicationService.Current.State[key];
        }

        private static void SetApplicationStateValue(string key, object value)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);

            PhoneApplicationService.Current.State.Add(key, value);
        }

        private static void RemoveApplicationStateValue(string key)
        {
            if (PhoneApplicationService.Current.State.ContainsKey(key))
                PhoneApplicationService.Current.State.Remove(key);
        }
    }
}