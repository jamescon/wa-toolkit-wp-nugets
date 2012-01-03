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
    using System.Runtime.Serialization;
    using System.Windows.Media.Imaging;

    /// <summary>
    /// DataContract for IdentityProviderInformation returned by the Identity Provider Discover Service
    /// </summary>
    [DataContract]
    public class IdentityProviderInformation
    {
        private BitmapImage image;

        /// <summary>
        /// The display name for the identity provider.
        /// </summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary>
        /// The url used for Login to the identity provider.
        /// </summary>
        [DataMember]
        public string LoginUrl { get; set; }

        /// <summary>
        /// The url used for Logout from the identity provider.
        /// </summary>
        [DataMember]
        public string LogoutUrl { get; set; }
        
        /// <summary>
        /// The url that is used to retrieve the image for the identity provider
        /// </summary>
        [DataMember]
        public string ImageUrl { get; set; }

        /// <summary>
        /// A list fo email address suffixes configured for the identity provider.
        /// </summary>
        [DataMember]
        public string[] EmailAddressSuffixes { get; set; }

        /// <summary>
        /// The image populated by calling LoadImageFromImageUrl
        /// </summary>
        public BitmapImage Image
        {
            get
            {
                return this.image;
            }
        }

        /// <summary>
        /// Retieves the image from ImageUrl
        /// </summary>
        /// <returns>The image from the url as a BitmapImage</returns>
        public BitmapImage LoadImageFromImageUrl()
        {
            this.image = null;

            if (string.IsNullOrWhiteSpace(this.ImageUrl))
            {
                var imageBitmap = new BitmapImage();
                var uriString = this.ImageUrl;
                if (uriString != null)
                {
                    var imageUrlUri = new Uri(uriString);
                    imageBitmap.UriSource = imageUrlUri;
                }

                this.image = imageBitmap;
            }

            return this.image;
        }
    }
}
