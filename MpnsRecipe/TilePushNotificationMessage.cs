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

namespace Microsoft.WindowsPhone.Samples.Notifications
{
    using System;
    using System.Globalization;
    using System.Text;
    using System.Web;
    using Microsoft.WindowsPhone.Samples.Notifications.Properties;

    /// <summary>
    /// Represents a tile push notification message.
    /// </summary>
    /// <remarks>
    /// Every phone application has one assigned 'tile' – a visual, dynamic
    /// representation of the application or its content. A tile displays in
    /// the Start screen if the end user has pinned it.
    /// This class members are thread safe.
    /// </remarks>
    public sealed class TilePushNotificationMessage : PushNotificationMessage
    {
        #region Constants

        /// <value>Tile push notification message maximum payload size.</value>
        public const int MaxPayloadSize = MaxMessageSize - TileMessageHeadersSize;

        /// <value>The minimum <see cref="TilePushNotificationMessage.Count"/> value.</value>
        public const int MinCount = 0;

        /// <value>The maximum <see cref="TilePushNotificationMessage.Count"/> value.</value>
        public const int MaxCount = 99;        

        /// <value>Windows phone target.</value>
        private const string WindowsPhoneTarget = "token";

        /// <value>A well formed structure of the tile notification message.</value>
        private const string PayloadString = 
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
                "<wp:Tile ID=\"{0}\">" +
                    "<wp:BackgroundImage>{1}</wp:BackgroundImage>" +
                    "<wp:Count{2}>{3}</wp:Count>" +
                    "<wp:Title{4}>{5}</wp:Title>" +
                    "<wp:BackBackgroundImage{6}>{7}</wp:BackBackgroundImage>" +
                    "<wp:BackTitle{8}>{9}</wp:BackTitle>" +
                    "<wp:BackContent{10}>{11}</wp:BackContent>" +
                "</wp:Tile> " +
            "</wp:Notification>";
    
        /// <value>Calculated tile message headers size.</value>
        /// <remarks>This should ne updated if changing the protocol.</remarks>
        private const int TileMessageHeadersSize = 146;

        #endregion

        #region Fields

        /// <value>The secondary tile uri and query string.</value>
        private string secondaryTile;

        /// <value>The phone's local path, or a remote path for the background image.</value>
        private Uri backgroundImageUri;

        /// <value>An integer value to be displayed in the tile.</value>
        private int count = MinCount;

        /// <value>The title text that should be displayed in the tile.</value>
        private string title;

        /// <value>The phone's local path, or a remote path for the back background image.</value>
        private Uri backBackgroundImageUri;

        /// <value>The title text that should be displayed in the back of the tile.</value>
        private string backTitle;

        /// <value>The title text content that  should be displayed in the back of the tile.</value>
        private string backContent;

        /// <value>Clear the count field?</value>
        private bool clearCount;

        /// <value>Clear the title field?</value>
        private bool clearTitle;

        /// <value>Clear the backgrouond image for the back of the tile?</value>
        private bool clearBackBackgroundImageUri;

        /// <value>Clear the title field on the back of the tile?</value>
        private bool clearBackTitle;

        /// <value>Clear the content field on the back of the tile?</value>
        private bool clearBackContent;

        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TilePushNotificationMessage"/> class. 
        /// Initializes a new instance of this type with <see cref="MessageSendPriority.Normal"/> send priority.
        /// </summary>
        public TilePushNotificationMessage()
            : this(MessageSendPriority.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="TilePushNotificationMessage"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="sendPriority">
        /// The send priority of this message in the MPNS.
        /// </param>
        public TilePushNotificationMessage(MessageSendPriority sendPriority)
            : base(sendPriority)
        {
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the URI and query string of the secondary tile as specified by the Windows Phone client.        
        /// <example>/GameList.xaml</example>
        /// <example>/GameList.xaml?sort=byName</example>
        /// </summary>
        /// <remarks>
        /// Null or empty string refers the main tile.
        /// </remarks>
        public string SecondaryTile
        {
            get
            {
                return this.secondaryTile;
            }

            set
            {
                SafeSet(ref this.secondaryTile, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Gets or sets the phone's local path, or a remote path for the background image.
        /// </summary>
        /// <remarks>
        /// If the uri references a remote resource, the maximum allowed size of the tile
        /// image is 80 KB, with a maximum download time of 15 seconds.
        /// </remarks>
        public Uri BackgroundImageUri
        {
            get
            {
                return this.backgroundImageUri;
            }

            set
            {
                SafeSet(ref this.backgroundImageUri, value);
            }
        }

        /// <summary>
        /// Gets or sets an integer value from 1 to 99 to be displayed in the tile, or 0 to clear count.
        /// </summary>
        public int Count
        {
            get
            {
                return this.count;
            }

            set
            {
                if (value < MinCount || value > MaxCount)
                {
                    throw new ArgumentOutOfRangeException(string.Format(CultureInfo.InvariantCulture, Resources.CountValueIsNotValid, value, MinCount, MaxCount));
                }

                SafeSet(ref this.count, value);
            }
        }

        /// <summary>
        /// Gets or sets the title text should be displayed in the tile. Null keeps the existing title.
        /// </summary>
        /// <remarks>
        /// The Title must fit a single line of text and should not be wider than the actual tile.
        /// Imperatively a good number of letters would be 18-20 characters long.
        /// </remarks>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                SafeSet(ref this.title, value);
            }
        }

        /// <summary>
        /// Gets or sets the phone's local path, or a remote path for the back background image.
        /// </summary>
        /// <remarks>
        /// If the uri references a remote resource, the maximum allowed size of the tile
        /// image is 80 KB, with a maximum download time of 15 seconds.
        /// </remarks>
        public Uri BackBackgroundImageUri
        {
            get
            {
                return this.backBackgroundImageUri;
            }

            set
            {
                SafeSet(ref this.backBackgroundImageUri, value);
            }
        }

        /// <summary>
        /// Gets or sets the title on the back of the tile.
        /// </summary>
        public string BackTitle
        {
            get
            {
                return this.backTitle;
            }

            set
            {
                SafeSet(ref this.backTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets the content on the back of the tile.
        /// </summary>
        public string BackContent {
            get
            {
                return this.backContent;
            }

            set
            {
                this.SafeSet(ref this.backContent, value);
            }
        }

        /// <summary>
        /// Gets or sets the flag to clear the count field.
        /// </summary>
        public bool ClearCount
        {
            get
            {
                return this.clearCount;
            }

            set
            {
                this.SafeSet(ref this.clearCount, value);
            }
        }

        /// <summary>
        /// Gets or sets the flag to clear the title field.
        /// </summary>
        public bool ClearTitle {
            get
            {
                return this.clearTitle;
            }

            set
            {
                this.SafeSet(ref this.clearTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets the flag to clear the Back Background image field.
        /// </summary>
        public bool ClearBackBackgroundImageUri
        {
            get
            {
                return this.clearBackBackgroundImageUri;
            }

            set
            {
                this.SafeSet(ref this.clearBackBackgroundImageUri, value);
            }
        }

        /// <summary>
        /// Gets or sets the flag to clear the back title field.
        /// </summary>
        public bool ClearBackTitle
        {
            get
            {
                return this.clearBackTitle;
            }

            set
            {
                this.SafeSet(ref this.clearBackTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets the flag to clear the back content field.
        /// </summary>
        public bool ClearBackContent
        {
            get
            {
                return this.clearBackContent;
            }

            set
            {
                this.SafeSet(ref this.clearBackContent, value);
            }
        }

        /// <summary>
        /// Tile push notification message class id.
        /// </summary>
        protected override int NotificationClassId
        {
            get { return 1; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Create the tile message payload.
        /// </summary>
        /// <returns>The message payload bytes.</returns>
        protected override byte[] OnCreatePayload()
        {
            var imageUri = EncodeUri(this.BackgroundImageUri);
            var backImageUri = EncodeUri(this.backBackgroundImageUri);

            var payloadString = string.Format(
                        CultureInfo.InvariantCulture,
                        PayloadString,
                        this.SecondaryTile,
                        imageUri,
                        ClearField(this.ClearCount),
                        this.Count,
                        ClearField(this.ClearTitle),
                        this.Title,
                        ClearField(this.ClearBackBackgroundImageUri),
                        backImageUri,
                        ClearField(this.ClearBackTitle),
                        this.BackTitle,
                        ClearField(this.ClearBackContent),
                        this.BackContent);

            return Encoding.UTF8.GetBytes(payloadString);
        }

        /// <summary>
        /// Initialize the request with tile specific headers.
        /// </summary>
        /// <param name="request">The message request.</param>
        protected override void OnInitializeRequest(System.Net.HttpWebRequest request)
        {
            if (request != null)
            {
                request.Headers[Headers.WindowsPhoneTarget] = WindowsPhoneTarget;
            }
        }

        protected override void VerifyPayloadSize(byte[] payload)
        {
            if (payload != null && payload.Length > MaxPayloadSize)
            {
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentUICulture, Resources.PayloadSizeIsTooBig, MaxPayloadSize));
            }
        }

        #endregion

        #region Utilities

        private static string ClearField(bool value)
        {
            return value ? " Action=\"Clear\"" : string.Empty;
        }

        private static string EncodeUri(Uri uri)
        {
            string encodedUri = string.Empty;
            if (uri != null)
            {
                if (uri.IsAbsoluteUri && uri.HostNameType == UriHostNameType.Dns)
                {
                    encodedUri = HttpUtility.HtmlEncode(uri.AbsoluteUri);
                }
                else
                {
                    encodedUri = uri.OriginalString;
                }
            }

            return encodedUri;
        }
        
        #endregion
    }
}
