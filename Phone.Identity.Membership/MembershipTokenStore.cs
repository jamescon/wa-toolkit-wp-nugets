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
    public class MembershipTokenStore
    {
        private const string MembershipTokenStoreKey = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipTokenStore";
        private const string UserNameKey = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.UserName";
        private const string RememberMeKey = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.RememberMe";

        public string MembershipToken
        {
            get
            {
                return Helpers.GetValue<string>(MembershipTokenStoreKey, this.RememberMe);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Helpers.RemoveKey(MembershipTokenStoreKey);
                else
                    Helpers.SetValue(MembershipTokenStoreKey, value, this.RememberMe);
            }
        }

        public string UserName
        {
            get
            {
                return Helpers.GetValue<string>(UserNameKey, this.RememberMe);
            }

            set
            {
                if (string.IsNullOrWhiteSpace(value))
                    Helpers.RemoveKey(UserNameKey);
                else
                    Helpers.SetValue(UserNameKey, value, this.RememberMe);
            }
        }

        public bool RememberMe
        {
            get
            {
                return Helpers.GetValue<bool>(RememberMeKey);
            }

            set
            {
                // Every time the RememberMe flag is changed, the UserName and
                // MembershipToken properties must be set again with their current
                // values to ensure that they are stored in the right location 
                // (IsolatedStorage or ApplicationState)
                var userName = this.UserName;
                var membershipToken = this.MembershipToken;

                Helpers.SetValue(RememberMeKey, value);

                this.UserName = userName;
                this.MembershipToken = membershipToken;
            }
        }

        public bool ContainsValidMembershipToken()
        {
            return !string.IsNullOrWhiteSpace(this.MembershipToken);
        }
    }
}
