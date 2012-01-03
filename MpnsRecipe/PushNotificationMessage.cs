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
    using System.Diagnostics;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Text;
    using Microsoft.WindowsPhone.Samples.Notifications.Properties;

    /// <summary>
    /// Represents a base class for push notification messages.
    /// </summary>
    /// <remarks>
    /// This class members are thread safe.
    /// </remarks>
    public abstract class PushNotificationMessage
    {
        #region Constants

        /// <value>Push notification maximum message size including headers and payload.</value>
        protected const int MaxMessageSize = 1024;

        #endregion

        #region Fields

        /// <value>Synchronizes payload manipulations.</value>
        private readonly object sync = new object();

        /// <value>The payload raw bytes of this message.</value>
        private byte[] payloadByted;

        private MessageSendPriority sendPriority;

        #endregion

        #region Ctor

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNotificationMessage"/> class. 
        /// Initializes a new instance of this type with <see cref="MessageSendPriority.Normal"/> send priority.
        /// </summary>
        protected PushNotificationMessage()
            : this(MessageSendPriority.Normal)
        {
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="PushNotificationMessage"/> class. 
        /// Initializes a new instance of this type with <see cref="MessageSendPriority.Normal"/> send priority.
        /// </summary>
        protected PushNotificationMessage(MessageSendPriority sendPriority)
        {
            this.Id = Guid.NewGuid();
            this.SendPriority = sendPriority;
            this.IsDirty = true;
        }

        #endregion

        #region Properties

        /// <summary>
        /// Gets this message unique ID.
        /// </summary>
        public Guid Id { get; private set; }

        /// <summary>
        /// Gets or sets the send priority of this message in the MPNS.
        /// </summary>
        public MessageSendPriority SendPriority
        {
            get
            {
                return this.sendPriority;
            }

            set
            {
                this.SafeSet(ref this.sendPriority, value);
            }
        }

        /// <summary>
        /// Gets or sets the message payload.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Justification = "Data type is a byte array")]
        protected byte[] Payload
        {
            get
            {
                return this.payloadByted;
            }

            set
            {
                this.SafeSet(ref this.payloadByted, value);
            }
        }

        protected abstract int NotificationClassId
        {
            get;
        }

        /// <summary>
        /// Gets or sets the flag indicating that one of the message properties
        /// has changed, thus the payload should be rebuilt.
        /// </summary>
        private bool IsDirty { get; set; }

        #endregion

        #region Operations

        /// <summary>
        /// Synchronously send this message to the destination address.
        /// </summary>
        /// <remarks>
        /// Note that properties of this instance may be changed by different threads while
        /// sending, but once the payload created, it won't be changed until the next send.
        /// </remarks>
        /// <param name="uri">Destination address uri.</param>
        /// <exception cref="ArgumentNullException">One of the arguments is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Payload size is out of range. For maximum allowed message size see <see cref="MaxMessageSize"/></exception>
        /// <exception cref="MessageSendException">Failed to send message for any reason.</exception>
        /// <returns>The result instance with relevant information for this send operation.</returns>
        public MessageSendResult Send(Uri uri)
        {
            Guard.ArgumentNotNull(uri, "uri");

            // Create payload or reuse cached one.
            var payload = this.GetOrCreatePayload();

            // Create and initialize the request object.
            var request = this.CreateWebRequest(uri, payload);

            var result = this.SendSynchronously(payload, uri, request);
            return result;
        }

        /// <summary>
        /// Asynchronously send this message to the destination address.
        /// </summary>
        /// <remarks>
        /// This method uses the .NET Thread Pool. Use this method to send one or few
        /// messages asynchronously. If you have many messages to send, please consider
        /// of using the synchronous method with custom (external) queue-thread solution.
        /// Note that properties of this instance may be changed by different threads while
        /// sending, but once the payload created, it won't be changed until the next send.
        /// </remarks>
        /// <param name="uri">Destination address uri.</param>
        /// <exception cref="ArgumentNullException">One of the arguments is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Payload size is out of range. For maximum allowed message size see <see cref="MaxMessageSize"/></exception>        
        public void SendAsync(Uri uri)
        {
            this.SendAsync(uri, null, null);
        }

        /// <summary>
        /// Asynchronously send this message to the destination address.
        /// </summary>
        /// <remarks>
        /// This method uses the .NET Thread Pool. Use this method to send one or few
        /// messages asynchronously. If you have many messages to send, please consider
        /// of using the synchronous method with custom (external) queue-thread solution.
        /// Note that properties of this instance may be changed by different threads while
        /// sending, but once the payload created, it won't be changed until the next send.
        /// </remarks>
        /// <param name="uri">Destination address uri.</param>
        /// <param name="messageSent">Message sent callback.</param>
        /// <param name="messageError">Message send error callback.</param>
        /// <exception cref="ArgumentNullException">One of the arguments is null.</exception>
        /// <exception cref="ArgumentOutOfRangeException">Payload size is out of range. For maximum allowed message size see <see cref="MaxMessageSize"/></exception>        
        public void SendAsync(Uri uri, Action<MessageSendResult> messageSent, Action<MessageSendResult> messageError)
        {
            Guard.ArgumentNotNull(uri, "uri");

            // Create payload or reuse cached one.
            var payload = this.GetOrCreatePayload();

            // Create and initialize the request object.
            var request = this.CreateWebRequest(uri, payload);

            this.SendAsynchronously(
                payload,
                uri,
                request,
                messageSent ?? (result => { }),
                messageError ?? (result => { }));
        }

        #endregion

        #region Protected & Virtuals

        /// <summary>
        /// Override to create the message payload.
        /// </summary>
        /// <returns>The message payload bytes.</returns>
        protected virtual byte[] OnCreatePayload()
        {
            return this.payloadByted;
        }

        /// <summary>
        /// Override to initialize the message web request with custom headers.
        /// </summary>
        /// <param name="request">The message web request.</param>
        protected virtual void OnInitializeRequest(HttpWebRequest request)
        {
        }

        /// <summary>
        /// Check the size of the payload and reject it if too big.
        /// </summary>
        /// <param name="payload">Payload raw bytes.</param>
        protected abstract void VerifyPayloadSize(byte[] payload);

        /// <summary>
        /// Safely set oldValue with newValue in case that are different, and raise the dirty flag.
        /// </summary>
        /// <typeparam name="T">The type of the value.</typeparam>
        /// <param name="oldValue">The old value.</param>
        /// <param name="newValue">The new value.</param>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1045:DoNotPassTypesByReference", MessageId = "0#", Justification = "The value must be provided as a reference to be able to modify it atomically")]
        protected void SafeSet<T>(ref T oldValue, T newValue)
        {
            lock (this.sync)
            {
                if (!Equals(oldValue, newValue))
                {
                    oldValue = newValue;
                    this.IsDirty = true;
                }
            }
        }

        #endregion

        #region Diagnostics

        [Conditional("DEBUG")]
        private static void DebugOutput(byte[] payload)
        {
            string payloadString = Encoding.UTF8.GetString(payload);
            Debug.WriteLine(payloadString);
        }

        #endregion

        #region Privates

        /// <summary>
        /// Synchronously send this message to the destination uri.
        /// </summary>
        /// <param name="payload">The message payload bytes.</param>
        /// <param name="uri">The message destination uri.</param>
        /// <param name="request">Initialized Web request instance.</param>
        /// <returns>The result instance with relevant information for this send operation.</returns>
        private MessageSendResult SendSynchronously(byte[] payload, Uri uri, HttpWebRequest request)
        {
            try
            {
                // Get the request stream.
                using (var requestStream = request.GetRequestStream())
                {
                    // Start to write the payload to the stream.
                    requestStream.Write(payload, 0, payload.Length);

                    // Switch to receiving the response from MPNS.
                    using (var response = (HttpWebResponse)request.GetResponse())
                    {
                        var result = new MessageSendResult(this, uri, response);
                        if (response.StatusCode != HttpStatusCode.OK)
                            throw new InvalidOperationException(string.Format(CultureInfo.CurrentCulture, Resources.ServerErrorStatusCode, response.StatusCode));

                        return result;
                    }
                }
            }
            catch (WebException ex)
            {
                return new MessageSendResult(this, uri, ex);
            }
            catch (ProtocolViolationException ex)
            {
                return new MessageSendResult(this, uri, ex);
            }
            catch (InvalidOperationException ex)
            {
                return new MessageSendResult(this, uri, ex);
            }
            catch (NotSupportedException ex)
            {
                return new MessageSendResult(this, uri, ex);
            }
            catch (IOException ex)
            {
                return new MessageSendResult(this, uri, ex);
            }
        }

        /// <summary>
        /// Asynchronously send this message to the destination uri using the HttpWebRequest context.
        /// </summary>
        /// <param name="payload">The message payload bytes.</param>
        /// <param name="uri">The message destination uri.</param>
        /// <param name="request">Initialized Web request instance.</param>
        /// <param name="sent">Message sent callback.</param>
        /// <param name="error">Message send error callback.</param>
        /// <returns>The result instance with relevant information for this send operation.</returns>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1031:DoNotCatchGeneralExceptionTypes", Justification = "Exceptions are being wrapped in a custom exception and rethrown.")]
        private void SendAsynchronously(byte[] payload, Uri uri, HttpWebRequest request, Action<MessageSendResult> sent, Action<MessageSendResult> error)
        {
            try
            {
                // Get the request stream asynchronously.
                request.BeginGetRequestStream(
                    requestAsyncResult =>
                    {
                        try
                        {
                            using (var requestStream = request.EndGetRequestStream(requestAsyncResult))
                            {
                                // Start writing the payload to the stream.
                                requestStream.Write(payload, 0, payload.Length);
                            }

                            // Switch to receiving the response from MPNS asynchronously.
                            request.BeginGetResponse(
                                responseAsyncResult =>
                                {
                                    try
                                    {
                                        using (var response = (HttpWebResponse)request.EndGetResponse(responseAsyncResult))
                                        {
                                            var result = new MessageSendResult(this, uri, response);
                                            if (response.StatusCode == HttpStatusCode.OK)
                                            {
                                                sent(result);
                                            }
                                            else
                                            {
                                                error(result);
                                            }
                                        }
                                    }
                                    catch (Exception ex3)
                                    {
                                        error(new MessageSendResult(this, uri, ex3));
                                    }
                                },
                                null);
                        }
                        catch (Exception ex2)
                        {
                            error(new MessageSendResult(this, uri, ex2));
                        }
                    },
                    null);
            }
            catch (Exception ex1)
            {
                error(new MessageSendResult(this, uri, ex1));
            }
        }

        /// <summary>
        /// Create a payload and verify its size.
        /// </summary>
        /// <returns>Payload raw bytes.</returns>
        private byte[] GetOrCreatePayload()
        {
            if (this.IsDirty)
            {
                lock (this.sync)
                {
                    if (this.IsDirty)
                    {
                        var payload = this.OnCreatePayload() ?? new byte[0];
                        DebugOutput(payload);
                        this.VerifyPayloadSize(payload);

                        this.payloadByted = payload;

                        this.IsDirty = false;
                    }
                }
            }

            return this.payloadByted;
        }

        private HttpWebRequest CreateWebRequest(Uri uri, byte[] payload)
        {
            var request = (HttpWebRequest)WebRequest.Create(uri);
            request.Method = WebRequestMethods.Http.Post;
            request.ContentType = "text/xml; charset=utf-8";
            request.ContentLength = payload.Length;
            request.Headers[Headers.MessageId] = this.Id.ToString();

            // Batching interval is composed of the message priority and the message class id.
            int batchingInterval = ((int)this.SendPriority * 10) + this.NotificationClassId;
            request.Headers[Headers.BatchingInterval] = batchingInterval.ToString(CultureInfo.CurrentCulture);

            this.OnInitializeRequest(request);

            return request;
        }

        #endregion

        /// <summary>
        /// Well known push notification message web request headers.
        /// </summary>
        internal static class Headers
        {
            public const string MessageId = "X-MessageID";
            public const string BatchingInterval = "X-NotificationClass";
            public const string NotificationStatus = "X-NotificationStatus";
            public const string DeviceConnectionStatus = "X-DeviceConnectionStatus";
            public const string SubscriptionStatus = "X-SubscriptionStatus";
            public const string WindowsPhoneTarget = "X-WindowsPhone-Target";
        }
    }
}
