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
    using System.Runtime.Serialization;
    using Microsoft.WindowsPhone.Samples.Notifications.Properties;

    /// <summary>
    /// Represents errors that occur during push notification message send operation.
    /// </summary>
    [Serializable]
    public class MessageSendException : Exception
    {
        public MessageSendException()
        {
        }

        public MessageSendException(string message)
            : base(message)
        {
        }

        public MessageSendException(string message, Exception innerException)
            : base(message, innerException)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="MessageSendException"/> class. 
        /// </summary>
        /// <param name="result">
        /// The send operation result.
        /// </param>
        /// <param name="innerException">
        /// An inner exception causes this error.
        /// </param>
        public MessageSendException(MessageSendResult result, Exception innerException)
            : base(Resources.FailedToSendMessage, innerException)
        {
            this.Result = result;
        }

        protected MessageSendException(SerializationInfo info, StreamingContext context)
            : base(info, context)
        {
        }

        /// <summary>
        /// Gets the message send result.
        /// </summary>
        public MessageSendResult Result { get; private set; }

        public override void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            base.GetObjectData(info, context);
        }
    }
}
