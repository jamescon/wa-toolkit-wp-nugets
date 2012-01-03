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

namespace Microsoft.WindowsAzure.Samples.Phone.Identity.AccessControl
{
    using System;
    using System.Collections.Generic;
    using System.Globalization;
    using System.IO;
    using System.Linq;
    using System.Net;
    using System.Runtime.Serialization.Json;
    using System.Text;

    internal class JsonIdentityProviderDiscoveryClient
    {
        internal event EventHandler<IdentityProviderListEventArgs> GetIdentityProviderListCompleted;

        internal void GetIdentityProviderListAsync(Uri identityProviderListServiceEndpoint)
        {
            var webClient = new WebClient();

            webClient.DownloadStringCompleted += this.WebClientDownloadStringCompleted;
            webClient.DownloadStringAsync(identityProviderListServiceEndpoint);
        }

        private void WebClientDownloadStringCompleted(object sender, DownloadStringCompletedEventArgs args)
        {
            IEnumerable<IdentityProviderInformation> identityProviders = null;
            Exception error = args.Error;

            if (null == args.Error)
            {
                try
                {
                    using (var ms = new MemoryStream(Encoding.Unicode.GetBytes(args.Result)))
                    {
                        var serializer =
                            new DataContractJsonSerializer(typeof(IdentityProviderInformation[]));
                        identityProviders = serializer.ReadObject(ms) as IEnumerable<IdentityProviderInformation>;

                        if (identityProviders != null)
                        {
                            IdentityProviderInformation windowsLiveId =
                                identityProviders.FirstOrDefault(
                                    i => i.Name.Equals("Windows Live™ ID", StringComparison.OrdinalIgnoreCase));
                            if (windowsLiveId != null)
                            {
                                string separator = windowsLiveId.LoginUrl.Contains("?") ? "&" : "?";
                                windowsLiveId.LoginUrl = string.Format(
                                    CultureInfo.InvariantCulture, "{0}{1}pcexp=false", windowsLiveId.LoginUrl, separator);
                            }

                            IdentityProviderInformation facebook =
                                identityProviders.FirstOrDefault(
                                    i => i.LoginUrl.StartsWith("https://www.facebook.com", StringComparison.OrdinalIgnoreCase));
                            if (facebook != null)
                            {
                                string separator = facebook.LoginUrl.Contains("?") ? "&" : "?";
                                facebook.LoginUrl = string.Format(
                                    CultureInfo.InvariantCulture, "{0}{1}display=wap", facebook.LoginUrl, separator);
                            }
                        }
                    }
                }
                catch (Exception ex)
                {
                    error = ex;
                }
            }

            if (null != this.GetIdentityProviderListCompleted)
            {
                this.GetIdentityProviderListCompleted(this, new IdentityProviderListEventArgs(identityProviders, error));
            }
        }
    }
}