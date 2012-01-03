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

namespace Microsoft.WindowsPhone.Samples.Notifications
{
    using System;
    using System.Net;

    /// <summary>
    /// Extends the <see cref="HttpWebResponse"/> type with methods for translating push notification specific status codes strings to strong typed enumeration.
    /// </summary>
    internal static class HttpWebResponseExtensions
    {
        /// <summary>
        /// Gets the Notification Status code as <see cref="NotificationStatus"/> enumeration.
        /// </summary>
        /// <param name="response">The http web response instance.</param>
        /// <returns>Correlate enumeration value.</returns>
        public static NotificationStatus GetNotificationStatus(this HttpWebResponse response)
        {
            return response.GetStatus<NotificationStatus>(PushNotificationMessage.Headers.NotificationStatus);
        }

        /// <summary>
        /// Gets the Device Connection Status code as <see cref="NotificationStatus"/> enumeration.
        /// </summary>
        /// <param name="response">The http web response instance.</param>
        /// <returns>Correlate enumeration value.</returns>
        public static DeviceConnectionStatus GetDeviceConnectionStatus(this HttpWebResponse response)
        {
            return response.GetStatus<DeviceConnectionStatus>(PushNotificationMessage.Headers.DeviceConnectionStatus);
        }

        /// <summary>
        /// Gets the Subscription Status code as <see cref="NotificationStatus"/> enumeration.
        /// </summary>
        /// <param name="response">The http web response instance.</param>
        /// <returns>Correlate enumeration value.</returns>
        public static SubscriptionStatus GetSubscriptionStatus(this HttpWebResponse response)
        {
            return response.GetStatus<SubscriptionStatus>(PushNotificationMessage.Headers.SubscriptionStatus);
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Usage", "CA1801:ReviewUnusedParameters", MessageId = "def", Justification = "The def parameter is left in order to inferr the proper type for this")]
        private static T GetStatus<T>(this HttpWebResponse response, string header) where T : struct
        {
            var statusString = response.Headers[header];
            T status;
            if (Enum.TryParse(statusString, out status))
            {
                return status;                
            }

            return default(T);
        }
    }
}
