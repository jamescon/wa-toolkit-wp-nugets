﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Samples.Storage.Providers
{
    using System;
    using System.Collections.Specialized;
    using System.Configuration;
    using System.Globalization;
    using System.Text;

    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.ServiceRuntime;

    public static class ProvidersConfiguration
    {
        public const string DefaultMembershipTableName = "secMembership";
        public const string DefaultRoleTableName = "secRole";
        public const string DefaultSessionTableName = "secSession";

        internal const string CsConfigStringPrefix = "CSConfigName";
        internal const string DefaultMembershipTableNameConfigurationString = "DefaultMembershipTableName";
        internal const string DefaultRoleTableNameConfigurationString = "DefaultRoleTableName";
        internal const string DefaultSessionTableNameConfigurationString = "DefaultSessionTableName";
        internal const string DefaultSessionContainerNameConfigurationString = "DefaultSessionContainerName";
        internal const string DefaultProfileContainerNameConfigurationString = "DefaultProfileContainerName";
        internal const string DefaultProviderApplicationNameConfigurationString = "DefaultProviderApplicationName";

        internal const string DefaultSessionContainerName = "sessionprovidercontainer";
        internal const string DefaultProfileContainerName = "profileprovidercontainer";
        internal const string DefaultProviderApplicationName = "appname";

        ////internal static readonly string DefaultTableStorageEndpointConfigurationString = "TableStorageEndpoint";
        ////internal static readonly string DefaultAccountNameConfigurationString = "AccountName";
        ////internal static readonly string DefaultAccountSharedKeyConfigurationString = "AccountSharedKey";
        ////internal static readonly string DefaultBlobStorageEndpointConfigurationString = "BlobStorageEndpoint";

        internal static readonly string DefaultStorageConfigurationString = "DataConnectionString";

        internal static readonly DateTime MinSupportedDateTime = DateTime.FromFileTime(0).ToUniversalTime().AddYears(200);
        internal static readonly int MaxStringPropertySizeInBytes = 64 * 1024;
        internal static readonly int MaxStringPropertySizeInChars = MaxStringPropertySizeInBytes / 2;

        internal static CloudStorageAccount GetStorageAccount(string settingName)
        {
            try
            {
                return CloudStorageAccount.FromConfigurationSetting(settingName);
            }
            catch (InvalidOperationException)
            {
                CloudStorageAccount.SetConfigurationSettingPublisher((configName, configSetter) =>
                {
                    string value = ConfigurationManager.AppSettings[configName];
                    if (RoleEnvironment.IsAvailable)
                    {
                        value = RoleEnvironment.GetConfigurationSettingValue(configName);
                    }

                    configSetter(value);
                });

                return CloudStorageAccount.FromConfigurationSetting(settingName);
            }
        }

        internal static string GetConfigurationSetting(string configurationString, string defaultValue)
        {
            return GetConfigurationSetting(configurationString, defaultValue, false);
        }

        /// <summary>
        /// Gets a configuration setting from application settings in the Web.config or App.config file. 
        /// When running in a hosted environment, configuration settings are read from the settings specified in 
        /// .cscfg files (i.e., the settings are read from the fabrics configuration system).
        /// </summary>
        internal static string GetConfigurationSetting(string configurationString, string defaultValue, bool throwIfNull)
        {
            if (string.IsNullOrEmpty(configurationString))
            {
                throw new ArgumentException("The parameter configurationString cannot be null or empty.");
            }

            string ret = null;

            // first, try to read from appsettings
            ret = TryGetAppSetting(configurationString);

            // settings in the csc file overload settings in Web.config
            if (RoleEnvironment.IsAvailable)
            {
                string cscRet = TryGetConfigurationSetting(configurationString);
                if (!string.IsNullOrEmpty(cscRet))
                {
                    ret = cscRet;
                }

                // if there is a csc config name in the app settings, this config name even overloads the 
                // setting we have right now
                string refWebRet = TryGetAppSetting(CsConfigStringPrefix + configurationString);
                if (!string.IsNullOrEmpty(refWebRet))
                {
                    cscRet = TryGetConfigurationSetting(refWebRet);
                    if (!string.IsNullOrEmpty(cscRet))
                    {
                        ret = cscRet;
                    }
                }
            }

            // if we could not retrieve any configuration string set return value to the default value
            if (string.IsNullOrEmpty(ret) && defaultValue != null)
            {
                ret = defaultValue;
            }

            if (string.IsNullOrEmpty(ret) && throwIfNull)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "Cannot find configuration string {0}.", configurationString));
            }

            return ret;
        }

        internal static string GetConfigurationSettingFromNameValueCollection(NameValueCollection config, string valueName)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (valueName == null)
            {
                throw new ArgumentNullException("valueName");
            }

            string settingValue = config[valueName];

            if (RoleEnvironment.IsAvailable)
            {
                // settings in the hosting configuration are stronger than settings in app config
                string cscRet = TryGetConfigurationSetting(valueName);
                if (!string.IsNullOrEmpty(cscRet))
                {
                    settingValue = cscRet;
                }

                // if there is a csc config name in the app settings, this config name even overloads the 
                // setting we have right now
                string refWebRet = config[CsConfigStringPrefix + valueName];
                if (!string.IsNullOrEmpty(refWebRet))
                {
                    cscRet = TryGetConfigurationSetting(refWebRet);
                    if (!string.IsNullOrEmpty(cscRet))
                    {
                        settingValue = cscRet;
                    }
                }
            }

            return settingValue;
        }

        internal static bool GetBooleanValue(NameValueCollection config, string valueName, bool defaultValue)
        {
            string settingValue = GetConfigurationSettingFromNameValueCollection(config, valueName);

            if (string.IsNullOrEmpty(settingValue))
            {
                return defaultValue;
            }

            bool result;
            if (bool.TryParse(settingValue, out result))
            {
                return result;
            }
            else
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value must be boolean (true or false) for property '{0}'.", valueName));
            }
        }

        internal static int GetIntValue(NameValueCollection config, string valueName, int defaultValue, bool zeroAllowed, int maxValueAllowed)
        {
            string settingValue = GetConfigurationSettingFromNameValueCollection(config, valueName);

            if (string.IsNullOrEmpty(settingValue))
            {
                return defaultValue;
            }

            int result;
            if (!int.TryParse(settingValue, out result))
            {
                if (zeroAllowed)
                {
                    throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value must be a non-negative 32-bit integer for property '{0}'.", valueName));
                }

                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value must be a positive 32-bit integer for property '{0}'.", valueName));
            }

            if (zeroAllowed && result < 0)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value must be a non-negative 32-bit integer for property '{0}'.", valueName));
            }

            if (!zeroAllowed && result <= 0)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value must be a positive 32-bit integer for property '{0}'.", valueName));
            }

            if (maxValueAllowed > 0 && result > maxValueAllowed)
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The value '{0}' can not be greater than '{1}'.", valueName, maxValueAllowed.ToString(CultureInfo.InstalledUICulture)));
            }

            return result;
        }

        internal static string GetStringValue(NameValueCollection config, string valueName, string defaultValue, bool nullAllowed)
        {
            string settingValue = GetConfigurationSettingFromNameValueCollection(config, valueName);

            if (string.IsNullOrEmpty(settingValue) && nullAllowed)
            {
                return null;
            }
            else if (string.IsNullOrEmpty(settingValue) && defaultValue != null)
            {
                return defaultValue;
            }
            else if (string.IsNullOrEmpty(settingValue))
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The parameter '{0}' must not be empty.", valueName));
            }

            return settingValue;
        }
        
        internal static string GetStringValueWithGlobalDefault(NameValueCollection config, string valueName, string defaultConfigString, string defaultValue, bool nullAllowed)
        {
            string settingValue = GetConfigurationSettingFromNameValueCollection(config, valueName);

            if (string.IsNullOrEmpty(settingValue))
            {
                settingValue = GetConfigurationSetting(defaultConfigString, null);
            }

            if (string.IsNullOrEmpty(settingValue) && nullAllowed)
            {
                return null;
            }
            else if (string.IsNullOrEmpty(settingValue) && defaultValue != null)
            {
                return defaultValue;
            }
            else if (string.IsNullOrEmpty(settingValue))
            {
                throw new ConfigurationErrorsException(string.Format(CultureInfo.InstalledUICulture, "The parameter '{0}' must not be empty.", valueName));
            }

            return settingValue;
        }

        internal static string GetInitExceptionDescription(StorageCredentials credentials, Uri tableBaseUri, Uri blobBaseUri)
        {
            var builder = new StringBuilder();
            builder.Append(GetInitExceptionDescription(credentials, tableBaseUri, "table storage configuration"));
            builder.Append(GetInitExceptionDescription(credentials, blobBaseUri, "blob storage configuration"));
            return builder.ToString();
        }

        internal static string GetInitExceptionDescription(StorageCredentials info, Uri baseUri, string desc)
        {
            var builder = new StringBuilder();
            builder.Append("The reason for this exception is typically that the endpoints are not correctly configured. " + Environment.NewLine);
            if (info == null)
            {
                builder.Append("The current " + desc + " is null. Please specify a table endpoint!" + Environment.NewLine);
            }
            else
            {
                builder.Append("The current " + desc + " is: " + baseUri + Environment.NewLine);
                builder.Append("Please also make sure that the account name and the shared key are specified correctly. This information cannot be shown here because of security reasons.");
            }

            return builder.ToString();
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes",
            Justification = "Make sure that no error condition prevents environment from reading service configuration.")]
        internal static string TryGetAppSetting(string configName)
        {
            string ret = null;
            try
            {
                ret = ConfigurationManager.AppSettings[configName];
            }
            catch (Exception)
            {
                // some exception happened when accessing the app settings section
                // most likely this is because there is no app setting file
                // this is not an error because configuration settings can also be located in the cscfg file, and explicitly 
                // all exceptions are captured here
                return null;
            }

            return ret;
        }

        private static string TryGetConfigurationSetting(string configName)
        {
            string ret;
            try
            {
                ret = RoleEnvironment.GetConfigurationSettingValue(configName);
            }
            catch (RoleEnvironmentException)
            {
                return null;
            }

            return ret;
        }
    }
}