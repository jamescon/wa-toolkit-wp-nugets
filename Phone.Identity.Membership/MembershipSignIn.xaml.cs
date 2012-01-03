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

namespace Microsoft.WindowsAzure.Samples.Phone.Identity.Membership
{
    using System;
    using System.Windows;
    using System.Windows.Input;
    using Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.Properties;

    public partial class MembershipSignIn
    {
        private IMembershipClient membershipClient;

        public MembershipSignIn()
        {
            this.InitializeComponent();

            this.ControlMode = MembershipSignInModes.LogOn;
        }

        /// <summary>
        /// Occurs when a response from the Membership service is received after signing in with user's credentials.
        /// </summary>
        public event EventHandler<MembershipRequestCompletedEventArgs> MembershipSignInResponseCompleted;

        /// <summary>
        /// Occurs when a response from the Membership service is received after registering a new user.
        /// </summary>
        public event EventHandler<MembershipRequestCompletedEventArgs> MembershipRegistrationResponseCompleted;

        /// <summary>
        /// Gets or Sets the Membership service endpoint.
        /// </summary>
        public string MembershipServiceEndpoint { get; set; }

        /// <summary>
        /// Gets or Sets the MembershipTokenStore which is used to store the token returned from Membership service.
        /// </summary>
        public MembershipTokenStore MembershipTokenStore { get; set; }

        /// <summary>
        /// Gets or Sets the mode of the MembershipSignIn control.
        /// </summary>
        public MembershipSignInModes ControlMode { get; set; }

        /// <summary>
        /// Gets or Sets the ViewModel for the MembershipSignIn control.
        /// </summary>
        internal MembershipSignInViewModel ViewModel
        {
            get { return this.DataContext as MembershipSignInViewModel; }
            set { this.DataContext = value; }
        }

        protected virtual IMembershipClient ResolveMembershipClient()
        {
            if (this.membershipClient == null)
            {
                if (string.IsNullOrWhiteSpace(this.MembershipServiceEndpoint))
                    throw new ArgumentException("MembershipServiceEndpoint cannot be null, empty or white space.");

                this.membershipClient = new MembershipClient(new Uri(this.MembershipServiceEndpoint), this.Dispatcher);
            }

            return this.membershipClient;
        }

        private void OnLoaded(object sender, RoutedEventArgs e)
        {
            try
            {
                if (this.ViewModel == null)
                    this.ViewModel = new MembershipSignInViewModel(this.MembershipTokenStore, this.ControlMode);
            }
            catch (ArgumentNullException exception)
            {
                MessageBox.Show(exception.Message, Resource.MissingArgumentMessage, MessageBoxButton.OK);
            }
        }

        private void OnSubmitMembershipOperationButtonClick(object sender, RoutedEventArgs e)
        {
            Action operation;
            bool operationEnabled;
            
            if (this.ViewModel.CurrentMode == MembershipSignInModes.LogOn)
            {
                operationEnabled = this.ViewModel.IsLogOnEnabled;
                operation = this.LogOn;
            }
            else
            {
                operationEnabled = this.ViewModel.IsRegisterEnabled;
                operation = this.Register;
            }

            if (operationEnabled)
                this.SubmitMembershipOperation(operation);
            else
                MessageBox.Show("Please fill out all the information with your credentials.", Resource.OperationIncompleteMessage, MessageBoxButton.OK);
        }

        private void OnChangeCurrentModeButtonClick(object sender, RoutedEventArgs e)
        {
            this.ViewModel.CurrentMode = this.ViewModel.CurrentMode == MembershipSignInModes.LogOn ? MembershipSignInModes.Register : MembershipSignInModes.LogOn;
        }

        private void OnLogOnTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == this.LogOnUserName)
                    this.LogOnPassword.Focus();
                else
                    this.LogOnUserName.Focus();
            }
        }

        private void OnRegisterTextBoxKeyUp(object sender, KeyEventArgs e)
        {
            if (e.Key == Key.Enter)
            {
                if (sender == this.RegisterUserName)
                {
                    this.RegisterEmail.Focus();
                }
                else if (sender == this.RegisterEmail)
                {
                    this.RegisterPassword.Focus();
                }
                else if (sender == this.RegisterPassword)
                {
                    this.RegisterConfirmPassword.Focus();
                }
                else
                {
                    this.RegisterUserName.Focus();
                }
            }
        }

        private void SubmitMembershipOperation(Action operation)
        {
            try
            {
                if (operation != null)
                    operation();
            }
            catch (ArgumentNullException exception)
            {
                MessageBox.Show(exception.Message, Resource.ParametersErrorMessage, MessageBoxButton.OK);

                this.ViewModel.IsBusy = false;
                this.ViewModel.Message = exception.Message;
            }
            catch (ArgumentException exception)
            {
                MessageBox.Show(exception.Message, Resource.ParametersErrorMessage, MessageBoxButton.OK);

                this.ViewModel.IsBusy = false;
                this.ViewModel.Message = exception.Message;
            }
            catch (UriFormatException exception)
            {
                MessageBox.Show(exception.Message, Resource.ParametersErrorMessage, MessageBoxButton.OK);

                this.ViewModel.IsBusy = false;
                this.ViewModel.Message = exception.Message;
            }
        }
        
        private void LogOn()
        {
            this.membershipClient = this.ResolveMembershipClient();

            this.ViewModel.IsBusy = true;
            this.ViewModel.Message = Resource.SigningInMessage;

            this.membershipClient.LogOn(
                this.ViewModel.UserName,
                this.ViewModel.Password,
                r =>
                {
                    this.ViewModel.IsBusy = false;

                    if (r.Success)
                    {
                        this.ViewModel.Message = Resource.LogOnSuccessfullyMessage;
                        this.MembershipTokenStore.MembershipToken = r.Content;
                    }
                    else
                        this.ViewModel.Message = r.ErrorMessage;

                    this.RaiseMembershipSignInResponseCompleted(r);
                });
        }

        private void Register()
        {
            this.membershipClient = this.ResolveMembershipClient();

            this.ViewModel.IsBusy = true;
            this.ViewModel.Message = Resource.RegisteringMessage;

            this.membershipClient.Register(
                this.ViewModel.UserName,
                this.ViewModel.Email,
                this.ViewModel.Password,
                this.ViewModel.ConfirmPassword,
                r =>
                {
                    if (r.Success)
                    {
                        this.ViewModel.Message = Resource.RegistrationSuccessfullyMessage;
                        if (this.ControlMode == MembershipSignInModes.LogOnAndRegister)
                            this.ViewModel.CurrentMode = MembershipSignInModes.LogOn;
                    }
                    else
                        this.ViewModel.Message = r.ErrorMessage;

                    this.ViewModel.IsBusy = false;

                    this.RaiseMembershipRegistrationResponseCompleted(r);
                });
        }
        
        private void RaiseMembershipSignInResponseCompleted(MembershipResponse response)
        {
            var membershipSignInResponseCompleted = this.MembershipSignInResponseCompleted;
            if (membershipSignInResponseCompleted != null)
                membershipSignInResponseCompleted(this, new MembershipRequestCompletedEventArgs(response));
        }

        private void RaiseMembershipRegistrationResponseCompleted(MembershipResponse response)
        {
            var membershipRegistrationResponseCompleted = this.MembershipRegistrationResponseCompleted;
            if (membershipRegistrationResponseCompleted != null)
                membershipRegistrationResponseCompleted(this, new MembershipRequestCompletedEventArgs(response));
        }
    }
}
