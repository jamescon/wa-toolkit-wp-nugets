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
    using System.IO.IsolatedStorage;

    /// <summary>
    /// Provides a class for storing a SimpleWebToken to isolatedStorage
    /// </summary>
    public class SimpleWebTokenStore
    {
        private const string SimpleWebTokenSettingKeyName = "Microsoft.WindowsAzure.Samples.Phone.Identity.AccessControl.SimpleWebTokenStore";
        private const long ExpirationBuffer = 10;

        private IsolatedStorageSettings isolatedStore;

        private SimpleWebToken token;

        /// <summary>
        /// Gets or sets the configured SimpleWebToken
        /// </summary>
        public SimpleWebToken SimpleWebToken
        {
            get
            {
                if (null == this.token)
                {
                    if (this.Settings.Contains(SimpleWebTokenSettingKeyName))
                    {
                        this.token = new SimpleWebToken(this.Settings[SimpleWebTokenSettingKeyName] as string);
                    }
                }

                return this.token;
            }

            set
            {
                if (null == value && this.Settings.Contains(SimpleWebTokenSettingKeyName))
                {
                    this.Settings.Remove(SimpleWebTokenSettingKeyName);
                }
                else
                {
                    if (value != null)
                    {
                        this.Settings[SimpleWebTokenSettingKeyName] = value.RawToken;
                    }
                }

                this.token = value;
            }
        }

        private IsolatedStorageSettings Settings
        {
            get
            {
                return this.isolatedStore ?? (this.isolatedStore = IsolatedStorageSettings.ApplicationSettings);
            }
        }

        /// <summary>
        /// Checks if the Simple Web token currenlty in the store is valid.
        /// </summary>
        /// <remarks>Returns true if there is a SimpleWebToken in the store and it has not expired,
        /// otherwise retruns false</remarks>
        public bool IsValid()
        {
            var simpleWebToken = this.SimpleWebToken;
            return simpleWebToken != null && simpleWebToken.ExpiresOn > DateTime.UtcNow.AddSeconds(ExpirationBuffer);
        }
    }
}
