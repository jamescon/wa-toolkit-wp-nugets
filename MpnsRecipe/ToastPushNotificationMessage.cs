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
    using System.Net;
    using System.Text;
    using Microsoft.WindowsPhone.Samples.Notifications.Properties;

    /// <summary>
    /// Represents a toast push notification message.
    /// </summary>
    /// <remarks>
    /// Toast notifications are system-wide notifications that do not disrupt
    /// the user workflow or require intervention to resolve. They are displayed
    /// at the top of the screen for ten seconds before disappearing. If the toast
    /// notification is tapped, the application that sent the toast notification
    /// will launch. A toast notification can be dismissed with a flick.
    /// This class members are thread safe.
    /// </remarks>    
    public sealed class ToastPushNotificationMessage : PushNotificationMessage
    {
        #region Constants

        /// <value>Toast push notification message maximum payload size.</value>
        public const int MaxPayloadSize = MaxMessageSize - ToastMessageHeadersSize;

        /// <value>Windows phone target.</value>
        private const string WindowsPhoneTarget = "toast";

        /// <value>A well formed structure of the toast notification message.</value>
        private const string PayloadString =
            "<?xml version=\"1.0\" encoding=\"utf-8\"?>" +
            "<wp:Notification xmlns:wp=\"WPNotification\">" +
                "<wp:Toast>" +
                    "<wp:Text1>{0}</wp:Text1>" +
                    "<wp:Text2>{1}</wp:Text2>" +
                    "<wp:Param>{2}</wp:Param>" +
                "</wp:Toast>" +
            "</wp:Notification>";

        /// <value>Calculated toast message headers size.</value>
        /// <remarks>This should ne updated if changing the protocol.</remarks>
        private const int ToastMessageHeadersSize = 146;

        #endregion

        #region Fields

        /// <value>The bolded string that should be displayed immediately after the application icon.</value>
        private string title;

        /// <value>The non-bolded string that should be displayed immediately after the Title.</value>
        private string subTitle;

        /// <value>The toast notification target page uri.</value>
        private string targetPage;

        #endregion

        #region Ctor
        /// <summary>
        /// Initializes a new instance of the <see cref="TilePushNotificationMessage"/> class. 
        /// Initializes a new instance of this type with <see cref="MessageSendPriority.Normal"/> send priority.
        /// </summary>
        public ToastPushNotificationMessage()
            : this(MessageSendPriority.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="ToastPushNotificationMessage"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="sendPriority">
        /// The send priority of this message in the MPNS.
        /// </param>
        public ToastPushNotificationMessage(MessageSendPriority sendPriority)
            : base(sendPriority)
        {
        }
 
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets a bolded string that should be displayed immediately after the application icon.
        /// </summary>
        public string Title
        {
            get
            {
                return this.title;
            }

            set
            {
                this.SafeSet(ref this.title, value);
            }
        }

        /// <summary>
        /// Gets or sets a non-bolded string that should be displayed immediately after the Title.
        /// </summary>
        public string Subtitle
        {
            get
            {
                return this.subTitle;
            }

            set
            {
                this.SafeSet(ref this.subTitle, value);
            }
        }

        /// <summary>
        /// Gets or sets the URI and query string of the target page should be navigated when clicking on the toast notification at client side.
        /// </summary>
        /// <example>/AppSettings.xaml</example>
        /// <example>/AppSettings.xaml?tab=push</example>
        /// <remarks>
        /// Null or empty string refers the main page.
        /// </remarks>
        public string TargetPage
        {
            get
            {
                return this.targetPage;
            }

            set
            {
                this.SafeSet(ref this.targetPage, value ?? string.Empty);
            }
        }

        /// <summary>
        /// Toast push notification message class id.
        /// </summary>
        protected override int NotificationClassId
        {
            get { return 2; }
        }

        #endregion

        #region Overrides

        /// <summary>
        /// Create the toast message payload.
        /// </summary>
        /// <returns>The message payload bytes.</returns>
        protected override byte[] OnCreatePayload()
        {
            var payloadString = string.Format(CultureInfo.InvariantCulture, PayloadString, this.Title, this.Subtitle, this.TargetPage);            
            return Encoding.UTF8.GetBytes(payloadString);
        }

        /// <summary>
        /// Initialize the request with toast specific headers.
        /// </summary>
        /// <param name="request">The message request.</param>
        protected override void OnInitializeRequest(HttpWebRequest request)
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
                throw new ArgumentOutOfRangeException(string.Format(CultureInfo.CurrentCulture, Resources.PayloadSizeIsTooBig, MaxPayloadSize));
            }
        }
        #endregion
    }
}
