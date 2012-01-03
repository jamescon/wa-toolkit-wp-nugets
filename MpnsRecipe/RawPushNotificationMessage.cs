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
    using Microsoft.WindowsPhone.Samples.Notifications.Properties;

    /// <summary>
    /// Represents a raw push notification message.
    /// </summary>
    /// <remarks>
    /// If you do not wish to update the tile or send a toast notification, you can instead
    /// send raw information to your application using a raw notification. If your application
    /// is not currently running, the raw notification is discarded on the Microsoft Push
    /// Notification Service and is not delivered to the device.
    /// This class members are thread safe.
    /// </remarks>
    public sealed class RawPushNotificationMessage : PushNotificationMessage
    {
        #region Constants

        /// <value>Raw push notification message maximum payload size.</value>
        public const int MaxPayloadSize = MaxMessageSize - RawMessageHeadersSize;

        /// <value>Calculated raw message headers size.</value>
        /// <remarks>This should ne updated if changing the protocol.</remarks>
        private const int RawMessageHeadersSize = 116;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="RawPushNotificationMessage"/> class. 
        /// Initializes a new instance of this type with <see cref="MessageSendPriority.Normal"/> send priority.
        /// </summary>
        public RawPushNotificationMessage()
            : this(MessageSendPriority.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="RawPushNotificationMessage"/> class. 
        /// Initializes a new instance of this type.
        /// </summary>
        /// <param name="sendPriority">
        /// The send priority of this message in the MPNS.
        /// </param>
        public RawPushNotificationMessage(MessageSendPriority sendPriority)
            : base(sendPriority)
        {
        }
        #endregion

        #region Properties

        /// <summary>
        /// Gets or sets the message raw data bytes.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Data type is a byte array")]
        public byte[] RawData
        {
            get
            {
                return Payload;
            }

            set
            {
                Payload = value;
            }
        }

        /// <summary>
        /// Raw push notification message class id.
        /// </summary>
        protected override int NotificationClassId
        {
            get { return 3; }
        }
        
        #endregion        

        #region Overrides

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
