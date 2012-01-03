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
    using System.Globalization;
    using System.Net;
    using System.Threading;
    using System.Windows;
    using System.Windows.Controls;
    using System.Windows.Input;
    using System.Windows.Navigation;
    using Microsoft.Phone.Controls;
    using Microsoft.WindowsAzure.Samples.Phone.Identity.AccessControl.Properties;

    /// <summary>
    /// This control is used to aquire a token from ACS using passive protocals between ACS and the Identity Provider.
    /// </summary>
    public partial class AccessControlServiceSignIn
    {
        private readonly object navigatingToIdentityProviderLock = new object();
        private readonly object setBrowserVisibleLock = new object();

        private Uri identityProviderDiscoveryService;
        private bool navigatingToIdentityProvider;

        private IdentityProviderInformation selectedIdentityProvider;
        private bool setBrowserVisible;

        /// <summary>
        /// Initializes a new instance of the <see cref="AccessControlServiceSignIn"/> class. 
        /// </summary>
        public AccessControlServiceSignIn()
        {
            this.InitializeComponent();
        }

        /// <summary>
        /// Occurs when a Simple Web Token token is issued by ACS and is ready to be presented to the application.
        /// </summary>
        public event EventHandler<RequestSimpleWebTokenResponseCompletedEventArgs> RequestSimpleWebTokenResponseCompleted;

        /// <summary>
        /// Occurs when the user selects an identity provider to log in with.
        /// </summary>
        public event EventHandler<IdentityProviderInformationEventArgs> NavigatingToIdentityProvider;

        /// <summary>
        /// Gets whether there is at least one state that the control can navigate back from.
        /// <value>True if a least one state can be undone, or false otherwise.</value>
        /// </summary>
        public bool CanGoBack
        {
            get { return this.navigatingToIdentityProvider; }
        }

        /// <summary>
        /// Gets and Sets the SimpleWebTokenStore which is used to store
        /// the SimpleWebToken generated from RequestSimpleWebTokenResponse returned from ACS
        /// </summary>
        public SimpleWebTokenStore SimpleWebTokenStore { get; set; }

        /// <summary>
        /// Gets and Sets the Realm
        /// </summary>
        public string Realm { get; set; }

        /// <summary>
        /// Gets and Sets the ServiceNamespace
        /// </summary>
        public string ServiceNamespace { get; set; }

        /// <summary>
        /// Initiates an asynchronous request which prompts user to sign into an identity provider, from the list returned by the
        /// call to the discover service returns a simple web token via the RequestSimpleWebTokenResponseCompleted event handler. 
        /// </summary>
        /// <remarks>
        /// Initiates a token request from ACS following these steps:
        /// 1) Get the list of configured Identity Providers from ACS by calling the discovery service
        /// 2) Once the user selects their identity provider, navigate to the sign in page of the provider
        /// 3) Using the WebBrowser control to complete the passive token request complete
        /// 4) Get the token
        /// 5) If a SimpleWebTokenStore is specified, set the token.
        /// 6) return the token using the RequestSimpleWebTokenResponseCompleted callback
        /// </remarks>
        /// <param name="newIdentityProviderDiscoveryService">The Identity provider discovery service from the ACS managment portal.</param>
        public void GetSimpleWebToken(Uri newIdentityProviderDiscoveryService)
        {
            this.identityProviderDiscoveryService = newIdentityProviderDiscoveryService;

            this.IdentityProviderListRefresh(this.identityProviderDiscoveryService);
        }

        /// <summary>
        /// Gets an SWT from the chosen identity provider
        /// </summary>
        public void GetSimpleWebToken()
        {
            if (string.IsNullOrWhiteSpace(this.Realm))
            {
                throw new InvalidOperationException("Realm was not set");
            }

            if (string.IsNullOrWhiteSpace(this.ServiceNamespace))
            {
                throw new InvalidOperationException("ServiceNamespace was not set");
            }

            if (null == this.SimpleWebTokenStore)
            {
                throw new InvalidOperationException("SimpleWebTokenStore was not set");
            }

            var identityProviderDiscoveryUri = new Uri(
                string.Format( 
                    CultureInfo.InvariantCulture,
                    "https://{0}.accesscontrol.windows.net/v2/metadata/IdentityProviders.js?protocol=javascriptnotify&realm={1}&version=1.0",
                    this.ServiceNamespace,
                    HttpUtility.UrlEncode(this.Realm)),
                UriKind.Absolute);

            this.GetSimpleWebToken(identityProviderDiscoveryUri);
        }

        /// <summary>
        /// Performs a backward navigation action, transitioning the control to a previous state. 
        /// <remarks>
        /// If the control is in a state that it cannot go backwards, no action is taken.
        /// </remarks>
        /// </summary>
        public void GoBack()
        {
            lock (this.navigatingToIdentityProviderLock)
            {
                if (this.navigatingToIdentityProvider)
                {
                    this.ShowProgressBar(string.Empty);
                    this.IdentityProviderListRefresh(this.identityProviderDiscoveryService);
                    this.navigatingToIdentityProvider = false;
                }
            }
        }

        private static void DisplayErrorMessageFromException(Exception e)
        {
            if (null != e)
            {
                DisplayErrorMessage(e.Message);
            }
        }

        private static void DisplayErrorMessage(string message)
        {
            MessageBox.Show(message);
        }

        private void IdentityProviderListSelectionChanged(object sender, SelectionChangedEventArgs e)
        {
            var identityProvider = this.IdentityProviderList.SelectedItem as IdentityProviderInformation;

            this.NavigateToIdentityProvider(identityProvider);

            // Reset to default value
            this.IdentityProviderList.SelectedIndex = -1;
        }

        private void NavigateToIdentityProvider(IdentityProviderInformation identityProvider)
        {
            if (null != identityProvider)
            {
                this.ShowProgressBar(string.Format(CultureInfo.InvariantCulture, Resource.DefaultErrorStringFormat, Resource.ContactingMessage, identityProvider.Name));

                if (null != this.NavigatingToIdentityProvider)
                {
                    this.NavigatingToIdentityProvider(this, new IdentityProviderInformationEventArgs(identityProvider));
                }

                lock (this.navigatingToIdentityProviderLock)
                {
                    this.navigatingToIdentityProvider = true;
                    this.BrowserSigninControl.Navigated += this.SignInWebBrowserControlNavigated;
                    this.BrowserSigninControl.Navigating += this.SignInWebBrowserControlNavigating;
                    this.BrowserSigninControl.ScriptNotify += this.SignInWebBrowserControlScriptNotify;
                    this.selectedIdentityProvider = identityProvider;
                    this.BrowserSigninControl.NavigateToString("<html><head><title></title></head><body></body></html>");
                }
            }
        }

        private void IdentityProviderListRefresh(Uri newIdentityProviderDiscoveryService)
        {
            var jsonClient = new JsonIdentityProviderDiscoveryClient();
            jsonClient.GetIdentityProviderListCompleted += this.IdentityProviderListRefreshCompleted;
            jsonClient.GetIdentityProviderListAsync(newIdentityProviderDiscoveryService);
        }

        private void IdentityProviderListRefreshCompleted(object sender, IdentityProviderListEventArgs e)
        {
            if (null == e.Error)
            {
                this.IdentityProviderList.ItemsSource = e.Result;
                this.ShowIdentityProviderSelection();
            }
            else
            {
                DisplayErrorMessage(
                    string.Format(
                        CultureInfo.InvariantCulture,
                        Resource.DefaultErrorStringFormat,
                        Resource.RetrievingErrorMessage, 
                        e.Error.Message));
            }
        }

        private void SignInWebBrowserControlScriptNotify(object sender, NotifyEventArgs e)
        {
            this.BrowserSigninControl.ScriptNotify -= this.SignInWebBrowserControlScriptNotify;

            RequestSimpleWebTokenResponse rswt = null;
            Exception exception = null;
            try
            {
                this.ShowProgressBar(Resource.SigningInMessage);

                rswt = RequestSimpleWebTokenResponse.FromJson(e.Value);

                if (null == rswt)
                {
                    DisplayErrorMessage(Resource.FailedReadingRSTRMessage);
                    exception = new InvalidOperationException("Failed to get a valid RequestSimpleWebTokenResponse");
                }
                else
                {
                    var simpleWebToken = new SimpleWebToken(rswt.SecurityToken);
                    if (null != this.SimpleWebTokenStore)
                    {
                        this.SimpleWebTokenStore.SimpleWebToken = simpleWebToken;
                    }
                }
            }
            catch (Exception ex)
            {
                DisplayErrorMessageFromException(ex);
                exception = ex;
            }

            if (null != this.RequestSimpleWebTokenResponseCompleted)
            {
                this.RequestSimpleWebTokenResponseCompleted(this, new RequestSimpleWebTokenResponseCompletedEventArgs(rswt, exception));
            }
        }
        
        private void SignInWebBrowserControlNavigated(object sender, NavigationEventArgs e)
        {
            // Navigate if in an empty page, and a idp hrd url is available
            if (null == e.Uri && null != this.selectedIdentityProvider)
            {
                this.BrowserSigninControl.Navigate(new Uri(this.selectedIdentityProvider.LoginUrl));
            }
            else if (null != e.Uri && string.IsNullOrEmpty(e.Uri.ToString()) && null != this.selectedIdentityProvider)
            {
                // This is a patch to make the library work under Windows Phone 7.1
                this.BrowserSigninControl.Navigate(new Uri(this.selectedIdentityProvider.LoginUrl));
            }
            else
            {
                if (this.navigatingToIdentityProvider)
                {
                    lock (this.setBrowserVisibleLock)
                    {
                        this.setBrowserVisible = true;
                    }

                    var show = new Thread(() =>
                    {
                        Thread.CurrentThread.Join(250);

                        lock (setBrowserVisibleLock)
                        {
                            if (this.setBrowserVisible && this.navigatingToIdentityProvider)
                            {
                                Dispatcher.BeginInvoke(this.ShowBrowser);
                            }
                        }
                    });

                    show.Start();
                }
            }
        }

        private void SignInWebBrowserControlNavigating(object sender, NavigatingEventArgs e)
        {
            lock (this.setBrowserVisibleLock)
            {
                this.setBrowserVisible = false;
                this.ShowProgressBar(null);
            }
        }

        private void HideAll()
        {
            this.identityProviderDiscovery.Visibility = Visibility.Collapsed;
            this.BrowserSigninControl.Visibility = Visibility.Collapsed;
            this.progressBar.Visibility = Visibility.Collapsed;
        }

        private void ShowBrowser()
        {
            this.HideAll();
            this.BrowserSigninControl.Visibility = Visibility.Visible;
        }

        private void ShowIdentityProviderSelection()
        {
            this.HideAll();
            this.identityProviderDiscovery.Visibility = Visibility.Visible;
        }

        private void ShowProgressBar(string message)
        {
            this.HideAll();
            if (null != message)
            {
                this.progressBarLabel.Text = message;
            }

            this.progressBar.Visibility = Visibility.Visible;
        }

        private void TextBlockMouseLeftButtonDown(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock)sender).Opacity = .5;
        }

        private void TextBlockMouseLeftButtonUp(object sender, MouseButtonEventArgs e)
        {
            ((TextBlock)sender).Opacity = 1;
        }

        private void TextBlockMouseLeave(object sender, MouseEventArgs e)
        {
            ((TextBlock)sender).Opacity = 1;
        }
    }
}