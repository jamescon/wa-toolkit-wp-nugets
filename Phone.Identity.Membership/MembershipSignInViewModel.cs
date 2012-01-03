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
    using System.ComponentModel;
    using Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.Properties;

    public class MembershipSignInViewModel : INotifyPropertyChanged
    {
        private readonly MembershipTokenStore membershipTokenStore;
        private readonly MembershipSignInModes controlMode;
        private MembershipSignInModes currentMode;
        private string password;
        private string confirmPassword;
        private string email;
        private bool isBusy;
        private string message;

        public MembershipSignInViewModel(MembershipTokenStore membershipTokenStore, MembershipSignInModes controlMode)
        {
            if (membershipTokenStore == null)
                throw new ArgumentNullException("membershipTokenStore", Resource.MembershipTokenStoreErrorMessage);

            this.membershipTokenStore = membershipTokenStore;
            this.controlMode = controlMode;

            // In case both operations LogOn and Registered are enabled, the default operation is LogOn.
            this.CurrentMode = (this.controlMode == MembershipSignInModes.LogOnAndRegister) ? MembershipSignInModes.LogOn : this.controlMode;
        }

        public event PropertyChangedEventHandler PropertyChanged;

        public MembershipSignInModes CurrentMode
        {
            get
            {
                return this.currentMode;
            }

            set
            {
                if (this.currentMode != value)
                {
                    this.currentMode = value;
                    this.NotifyPropertyChanged("CurrentMode");
                    this.NotifyPropertyChanged("CurrentOperation");
                }
            }
        }

        public string CurrentOperation
        {
            get
            {
                return this.CurrentMode == MembershipSignInModes.LogOn ? "register" : "log on";
            }
        }

        public string UserName
        {
            get
            {
                return this.membershipTokenStore.UserName;
            }

            set
            {
                if (this.membershipTokenStore.UserName != value)
                {
                    this.membershipTokenStore.UserName = value;
                    this.NotifyPropertyChanged("UserName");
                    this.NotifyPropertyChanged("IsLogOnEnabled");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
                else
                    this.membershipTokenStore.UserName = value;
            }
        }

        public bool RememberMe
        {
            get
            {
                return this.membershipTokenStore.RememberMe;
            }

            set
            {
                if (this.membershipTokenStore.RememberMe != value)
                {
                    this.membershipTokenStore.RememberMe = value;
                    this.NotifyPropertyChanged("RememberMe");
                }
                else
                    this.membershipTokenStore.RememberMe = value;
            }
        }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (this.password != value)
                {
                    this.password = value;
                    this.NotifyPropertyChanged("Password");
                    this.NotifyPropertyChanged("IsLogOnEnabled");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
            }
        }

        public string Email
        {
            get
            {
                return this.email;
            }

            set
            {
                if (this.email != value)
                {
                    this.email = value;
                    this.NotifyPropertyChanged("Email");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
            }
        }

        public string ConfirmPassword
        {
            get
            {
                return this.confirmPassword;
            }

            set
            {
                if (this.confirmPassword != value)
                {
                    this.confirmPassword = value;
                    this.NotifyPropertyChanged("ConfirmPassword");
                    this.NotifyPropertyChanged("IsRegisterEnabled");
                }
            }
        }

        public string Message
        {
            get
            {
                return this.message;
            }

            set
            {
                if (this.message != value)
                {
                    this.message = value;
                    this.NotifyPropertyChanged("Message");
                }
            }
        }

        public bool IsBusy
        {
            get
            {
                return this.isBusy;
            }

            set
            {
                if (this.isBusy != value)
                {
                    this.isBusy = value;
                    this.NotifyPropertyChanged("IsBusy");
                    this.NotifyPropertyChanged("IsIdle");
                }
            }
        }

        public bool IsIdle
        {
            get
            {
                return !this.IsBusy;
            }
        }

        public bool IsLogOnEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserName) && !string.IsNullOrWhiteSpace(this.Password);
            }
        }

        public bool IsRegisterEnabled
        {
            get
            {
                return !string.IsNullOrWhiteSpace(this.UserName)
                    && !string.IsNullOrWhiteSpace(this.Email)
                    && !string.IsNullOrWhiteSpace(this.Password)
                    && !string.IsNullOrWhiteSpace(this.ConfirmPassword);
            }
        }

        protected virtual void NotifyPropertyChanged(string propertyName)
        {
            var propertyChanged = this.PropertyChanged;
            if (propertyChanged != null)
                propertyChanged(this, new PropertyChangedEventArgs(propertyName));
        }
    }
}
