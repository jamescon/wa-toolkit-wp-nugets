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

namespace Microsoft.WindowsAzure.Samples.Phone.Notifications
{
    using System;
    using System.Collections.ObjectModel;
    using System.ComponentModel;

    using Microsoft.Phone.Notification;

    public sealed class PushContext : INotifyPropertyChanged
    {
        #region Fields

        private static readonly PushContext CurrentContext = new PushContext();

        private readonly PushConfig config = new PushConfig();

        private bool isConnected;

        #endregion

        #region Ctor

        internal PushContext()
        {
        }

        #endregion

        #region Events

        public event EventHandler<PushContextErrorEventArgs> Error;

        public event EventHandler<HttpNotificationEventArgs> RawNotification;

        public event PropertyChangedEventHandler PropertyChanged = delegate { };

        #endregion

        #region Properties

        public static PushContext Current
        {
            get { return CurrentContext; }
        }

        public PushConfig Configuration
        {
            get { return this.config; }
        }

        public bool IsConnected
        {
            get
            {
                return this.isConnected;
            }

            set
            {
                if (this.isConnected != value)
                {
                    this.isConnected = value;
                    this.NotifyPropertyChanged("IsConnected");
                }
            }
        }

        public HttpNotificationChannel NotificationChannel { get; private set; }

        public bool IsPushEnabled
        {
            get { return Helpers.GetIsolatedStorageSetting<bool>("PushContext.IsPushEnabled"); }
            set { Helpers.SetIsolatedStorageSetting("PushContext.IsPushEnabled", value); }
        }

        #endregion

        #region Public Methods

        public void Configure(Action<PushConfig> configureContext)
        {
            if (configureContext != null)
            {
                configureContext(this.config);

                if (string.IsNullOrWhiteSpace(this.config.ServiceName))
                    throw new ArgumentException("The service name name must be configured");

                if (string.IsNullOrWhiteSpace(this.config.ChannelName))
                    throw new ArgumentException("The channel name name must be configured");
            }
        }

        public IPushClient ResolvePushClient()
        {
            return new PushClient(this.Configuration.EndpointsServiceUri, this.Configuration.SignRequest, this.Configuration.ApplicationId, this.Configuration.DeviceId, this.Configuration.Dispatcher);
        }

        public void Connect(Action<HttpNotificationChannel> callback)
        {
            callback = callback ?? (r => { });

            if (this.IsConnected)
            {
                callback(this.NotificationChannel);
            }
            else
            {
                try
                {
                    // First, try to pick up an existing channel.
                    this.NotificationChannel = HttpNotificationChannel.Find(this.config.ChannelName);
                    if (this.NotificationChannel == null)
                    {
                        this.CreateChannel(callback);
                    }
                    else
                    {
                        this.PrepareChannel(callback);
                    }

                    this.UpdateNotificationBindings();
                    this.IsConnected = true;
                }
                catch (Exception ex)
                {
                    this.OnError(ex);
                }
            }
        }

        public void Disconnect()
        {
            if (!this.IsConnected)
            {
                return;
            }

            try
            {
                if (this.NotificationChannel != null)
                {
                    this.UnbindFromTileNotifications();
                    this.UnbindFromToastNotifications();
                    this.UnsubscribeToNotificationEvents();
                    this.NotificationChannel.Close();
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
            finally
            {
                this.NotificationChannel = null;
                this.IsConnected = false;
            }
        }

        #endregion

        #region Privates

        /// <summary>
        /// Create channel, subscribe to channel events and open the channel.
        /// </summary>
        private void CreateChannel(Action<HttpNotificationChannel> prepared)
        {
            // Create a new channel.
            this.NotificationChannel = new HttpNotificationChannel(this.config.ChannelName, this.config.ServiceName);

            // Register to UriUpdated event. This occurs when channel successfully opens.
            this.NotificationChannel.ChannelUriUpdated += (s, e) => this.Dispatch(() => this.PrepareChannel(prepared));

            // Trying to Open the channel.
            this.NotificationChannel.Open();
        }

        private void PrepareChannel(Action<HttpNotificationChannel> prepared)
        {
            try
            {
                prepared(this.NotificationChannel);
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void SubscribeToNotificationEvents()
        {
            if (this.NotificationChannel != null)
                this.NotificationChannel.HttpNotificationReceived += this.OnRawNotificationReceived;
        }

        private void UnsubscribeToNotificationEvents()
        {
            if (this.NotificationChannel != null)
                this.NotificationChannel.HttpNotificationReceived -= this.OnRawNotificationReceived;
        }

        private void OnRawNotificationReceived(object sender, HttpNotificationEventArgs args)
        {
            this.Dispatch(() =>
            {
                var rawNotification = this.RawNotification;
                if (rawNotification != null)
                    rawNotification(this, args);
            });
        }

        private void OnError(Exception exception)
        {
            var error = this.Error;
            if (error != null)
                error(this, new PushContextErrorEventArgs(exception));
        }

        private void BindToTileNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && !this.NotificationChannel.IsShellTileBound)
                {
                    var listOfAllowedDomains = new Collection<Uri>(this.config.AllowedDomains);
                    this.NotificationChannel.BindToShellTile(listOfAllowedDomains);
                }
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void BindToToastNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && !this.NotificationChannel.IsShellToastBound)
                    this.NotificationChannel.BindToShellToast();
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UnbindFromTileNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && this.NotificationChannel.IsShellTileBound)
                    this.NotificationChannel.UnbindToShellTile();
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UnbindFromToastNotifications()
        {
            try
            {
                if (this.NotificationChannel != null && this.NotificationChannel.IsShellToastBound)
                    this.NotificationChannel.UnbindToShellToast();
            }
            catch (Exception ex)
            {
                this.OnError(ex);
            }
        }

        private void UpdateNotificationBindings()
        {
            if (this.IsPushEnabled)
            {
                this.BindToTileNotifications();
                this.BindToToastNotifications();
                this.SubscribeToNotificationEvents();
            }
            else
            {
                this.UnbindFromTileNotifications();
                this.UnbindFromToastNotifications();
                this.UnsubscribeToNotificationEvents();
            }
        }

        private void NotifyPropertyChanged(string propertyName)
        {
            this.PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }

        private void Dispatch(Action a)
        {
            if (this.config.Dispatcher != null)
                this.config.Dispatcher.BeginInvoke(a);
            else
                a.Invoke();
        }

        #endregion
    }
}
