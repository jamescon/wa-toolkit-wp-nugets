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

    /// <summary>
    /// Push notification message send operation result.
    /// </summary>
    public class MessageSendResult
    {
        private const string ErrorStringTemplate = "Notification error: {0}";

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendResult"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        internal MessageSendResult(PushNotificationMessage associatedMessage, Uri channelUri, WebResponse response)
        {
            this.Timestamp = DateTimeOffset.Now;
            this.AssociatedMessage = associatedMessage;
            this.ChannelUri = channelUri;

            this.InitializeStatusCodes(response as HttpWebResponse);
        }        

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendResult"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        internal MessageSendResult(PushNotificationMessage associatedMessage, Uri channelUri, WebException exception)
            : this(associatedMessage, channelUri, exception.Response)
        {
            Exception = exception;
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendResult"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        internal MessageSendResult(PushNotificationMessage associatedMessage, Uri channelUri, Exception exception)
            : this(associatedMessage, channelUri, response: null)
        {
            Exception = exception;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets the original exception or null.
        /// </summary>
        public Exception Exception { get; private set; }

        /// <summary>
        /// Gets the response time offset.
        /// </summary>
        public DateTimeOffset Timestamp { get; private set; }

        /// <summary>
        /// Gets the associated message.
        /// </summary>
        public PushNotificationMessage AssociatedMessage { get; private set; }

        /// <summary>
        /// Gets the channel URI.
        /// </summary>
        public Uri ChannelUri { get; private set; }

        /// <summary>
        /// Gets the web request status.
        /// </summary>
        public HttpStatusCode StatusCode { get; private set; }

        /// <summary>
        /// Gets the push notification status.
        /// </summary>
        public NotificationStatus NotificationStatus { get; private set; }

        /// <summary>
        /// Gets the device connection status.
        /// </summary>
        public DeviceConnectionStatus DeviceConnectionStatus { get; private set; }

        /// <summary>
        /// Gets the subscription status.
        /// </summary>
        public SubscriptionStatus SubscriptionStatus { get; private set; }

        /// <summary>
        /// Gets the status of the Delivery.
        /// </summary>
        public string LookupDeliveryStatus()
        {
            // See delivery status from http://msdn.microsoft.com/en-us/library/ff941100(v=VS.92).aspx
            if (this.SubscriptionStatus == SubscriptionStatus.Expired)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "The ChannelUri is invalid and is not present on the Push Notification Service");
            }

            if (this.NotificationStatus == NotificationStatus.Dropped && this.DeviceConnectionStatus != DeviceConnectionStatus.Inactive)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "Channel quota exceeded");
            }

            if (this.SubscriptionStatus == SubscriptionStatus.NotApplicable && this.DeviceConnectionStatus == DeviceConnectionStatus.Inactive)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "Device inactive, you may try again in 60 minutes");
            }

            if (this.NotificationStatus == NotificationStatus.QueueFull)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "Device queue full, retry later");
            }

            if (this.NotificationStatus == NotificationStatus.Suppressed)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "The device discarded the message");
            }

            if (this.StatusCode == HttpStatusCode.BadRequest)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "There was an internal error generating the Push Notification message");
            }

            if (this.StatusCode == HttpStatusCode.Unauthorized)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "Sending this notification is unauthorized");
            }

            if (this.StatusCode == HttpStatusCode.MethodNotAllowed)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "HTTP Method not allowed");
            }

            if (this.SubscriptionStatus == SubscriptionStatus.NotApplicable)
            {
                return string.Format(CultureInfo.CurrentCulture, ErrorStringTemplate, "Internal error in the Notification Gateway");
            }

            return "Push Notification accepted by MPNS";
        }

        #endregion

        #region Privates

        private void InitializeStatusCodes(HttpWebResponse response)
        {
            if (response == null)
            {
                this.StatusCode = HttpStatusCode.InternalServerError;
                NotificationStatus = NotificationStatus.NotApplicable;
                DeviceConnectionStatus = DeviceConnectionStatus.NotApplicable;
                SubscriptionStatus = SubscriptionStatus.NotApplicable;
            }
            else
            {
                this.StatusCode = response.StatusCode;
                NotificationStatus = response.GetNotificationStatus();
                DeviceConnectionStatus = response.GetDeviceConnectionStatus();
                SubscriptionStatus = response.GetSubscriptionStatus();
            }
        }
        
        #endregion
    }    
}
