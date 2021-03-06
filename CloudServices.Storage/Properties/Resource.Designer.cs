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

//------------------------------------------------------------------------------
// <auto-generated>
//     This code was generated by a tool.
//     Runtime Version:4.0.30319.235
//
//     Changes to this file may cause incorrect behavior and will be lost if
//     the code is regenerated.
// </auto-generated>
//------------------------------------------------------------------------------

namespace Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties {
    using System;
    
    
    /// <summary>
    ///   A strongly-typed resource class, for looking up localized strings, etc.
    /// </summary>
    // This class was auto-generated by the StronglyTypedResourceBuilder
    // class via a tool like ResGen or Visual Studio.
    // To add or remove a member, edit your .ResX file then rerun ResGen
    // with the /str option, or rebuild your VS project.
    [global::System.CodeDom.Compiler.GeneratedCodeAttribute("System.Resources.Tools.StronglyTypedResourceBuilder", "4.0.0.0")]
    [global::System.Diagnostics.DebuggerNonUserCodeAttribute()]
    [global::System.Runtime.CompilerServices.CompilerGeneratedAttribute()]
    internal class Resource {
        
        private static global::System.Resources.ResourceManager resourceMan;
        
        private static global::System.Globalization.CultureInfo resourceCulture;
        
        [global::System.Diagnostics.CodeAnalysis.SuppressMessageAttribute("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        internal Resource() {
        }
        
        /// <summary>
        ///   Returns the cached ResourceManager instance used by this class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Resources.ResourceManager ResourceManager {
            get {
                if (object.ReferenceEquals(resourceMan, null)) {
                    global::System.Resources.ResourceManager temp = new global::System.Resources.ResourceManager("Microsoft.WindowsAzure.Samples.CloudServices.Storage.Properties.Resource", typeof(Resource).Assembly);
                    resourceMan = temp;
                }
                return resourceMan;
            }
        }
        
        /// <summary>
        ///   Overrides the current thread's CurrentUICulture property for all
        ///   resource lookups using this strongly typed resource class.
        /// </summary>
        [global::System.ComponentModel.EditorBrowsableAttribute(global::System.ComponentModel.EditorBrowsableState.Advanced)]
        internal static global::System.Globalization.CultureInfo Culture {
            get {
                return resourceCulture;
            }
            set {
                resourceCulture = value;
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The Storage Account setting cannot be null.
        /// </summary>
        internal static string CloudStorageAccountNullArgumentErrorMessage {
            get {
                return ResourceManager.GetString("CloudStorageAccountNullArgumentErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Argument comp must be sas for this operation.
        /// </summary>
        internal static string CompMustBeSasArgumentErrorMessage {
            get {
                return ResourceManager.GetString("CompMustBeSasArgumentErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Parameter configureAction cannot be null.
        /// </summary>
        internal static string ConfigureActionArgumentNullErrorMessage {
            get {
                return ResourceManager.GetString("ConfigureActionArgumentNullErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Container can not be null..
        /// </summary>
        internal static string ContainerCannotBeNullErrorMessage {
            get {
                return ResourceManager.GetString("ContainerCannotBeNullErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The containerName cannot be null..
        /// </summary>
        internal static string ContainerNameNullArgumentErrorMessage {
            get {
                return ResourceManager.GetString("ContainerNameNullArgumentErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to The connection with Windows Azure has been closed before the result could be read.
        /// </summary>
        internal static string HttpClientDisposedErrorString {
            get {
                return ResourceManager.GetString("HttpClientDisposedErrorString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Container Access Mode is invalid.
        /// </summary>
        internal static string InvalidPublicAccessModeArgumentErrorMessage {
            get {
                return ResourceManager.GetString("InvalidPublicAccessModeArgumentErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to You have specified an ACL operation but not sent the required HTTP header.
        /// </summary>
        internal static string PublicAccessNotSpecifiedErrorMessage {
            get {
                return ResourceManager.GetString("PublicAccessNotSpecifiedErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Request can not be null..
        /// </summary>
        internal static string RequestCannotBeNullErrorMessage {
            get {
                return ResourceManager.GetString("RequestCannotBeNullErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Called service with null HttpRequestMessage.
        /// </summary>
        internal static string RequestNullErrorMessage {
            get {
                return ResourceManager.GetString("RequestNullErrorMessage", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to Response can not be null..
        /// </summary>
        internal static string ResponseNullArgumentErrorString {
            get {
                return ResourceManager.GetString("ResponseNullArgumentErrorString", resourceCulture);
            }
        }
        
        /// <summary>
        ///   Looks up a localized string similar to There was an error when sending data to Windows Azure Storage.
        /// </summary>
        internal static string WindowsAzureStorageExceptionStringMessage {
            get {
                return ResourceManager.GetString("WindowsAzureStorageExceptionStringMessage", resourceCulture);
            }
        }
    }
}
