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

namespace Microsoft.WindowsAzure.Samples.Storage.Providers
{
    using System;
    using System.Configuration.Provider;

    using Microsoft.WindowsAzure.StorageClient;

    [CLSCompliant(false)]
    public class MembershipRow : TableServiceEntity, IComparable
    {
        private string applicationName;
        private string userName;
        private string password;
        private string passwordSalt;
        private string email;
        private string passwordAnswer;
        private string passwordQuestion;
        private string comment;
        private string profileBlobName;

        private DateTime createDate;
        private DateTime lastLoginDate;
        private DateTime lastPasswordChangedDate;
        private DateTime lastLockoutDate;
        private DateTime lastActivityDate;
        private DateTime failedPasswordAttemptWindowStart;
        private DateTime failedPasswordAnswerAttemptWindowStart;
        private DateTime profileLastUpdated;

        // partition key is applicationName + userName
        // rowKey is empty
        public MembershipRow(string applicationName, string userName)
            : base()
        {
            if (string.IsNullOrEmpty(applicationName))
            {
                throw new ProviderException("Partition key cannot be empty!");
            }

            if (string.IsNullOrEmpty(userName))
            {
                throw new ProviderException("RowKey cannot be empty!");
            }

            // applicationName + userName is partitionKey
            // the reasoning behind this is that we want to strive for the best scalability possible 
            // chosing applicationName as the partition key and userName as row key would not give us that because 
            // it would mean that a site with millions of users had all users on a single partition
            // having the applicationName and userName inside the partition key is important for queries as queries
            // for users in a single application are the most frequent 
            // these queries are faster because application name and user name are part of the key
            PartitionKey = SecUtility.CombineToKey(applicationName, userName);
            RowKey = string.Empty;

            this.ApplicationName = applicationName;
            this.UserName = userName;

            this.Password = string.Empty;
            this.PasswordSalt = string.Empty;
            this.Email = string.Empty;
            this.PasswordAnswer = string.Empty;
            this.PasswordQuestion = string.Empty;
            this.Comment = string.Empty;
            this.ProfileBlobName = string.Empty;

            this.CreateDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.LastLoginDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.LastActivityDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.LastLockoutDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.LastPasswordChangedDateUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.FailedPasswordAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.FailedPasswordAnswerAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
            this.ProfileLastUpdatedUtc = ProvidersConfiguration.MinSupportedDateTime;

            this.ProfileIsCreatedByProfileProvider = false;
            this.ProfileSize = 0;
        }

        public MembershipRow()
        {
        }

        public string ApplicationName
        {
            get
            {
                return this.applicationName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.applicationName = value;
                this.PartitionKey = SecUtility.CombineToKey(this.ApplicationName, this.UserName);
            }
        }

        public string UserName
        {
            get
            {
                return this.userName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.userName = value;
                this.PartitionKey = SecUtility.CombineToKey(this.ApplicationName, this.UserName);
            }
        }

        public Guid UserId { get; set; }

        public string Password
        {
            get
            {
                return this.password;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.password = value;
            }
        }

        public int PasswordFormat { get; set; }

        public string PasswordSalt
        {
            get
            {
                return this.passwordSalt;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.passwordSalt = value;
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
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.email = value;
            }
        }

        public string PasswordQuestion
        {
            get
            {
                return this.passwordQuestion;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.passwordQuestion = value;
            }
        }

        public string PasswordAnswer
        {
            get
            {
                return this.passwordAnswer;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.passwordAnswer = value;
            }
        }

        public bool IsApproved { get; set; }

        public bool IsAnonymous { get; set; }

        public bool IsLockedOut { get; set; }

        public DateTime CreateDateUtc
        {
            get
            {
                return this.createDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.createDate);
            }
        }

        public DateTime LastLoginDateUtc
        {
            get
            {
                return this.lastLoginDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.lastLoginDate);
            }
        }

        public DateTime LastPasswordChangedDateUtc
        {
            get
            {
                return this.lastPasswordChangedDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.lastPasswordChangedDate);
            }
        }

        public DateTime LastLockoutDateUtc
        {
            get
            {
                return this.lastLockoutDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.lastLockoutDate);
            }
        }

        public DateTime LastActivityDateUtc
        {
            get
            {
                return this.lastActivityDate;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.lastActivityDate);
            }
        }

        public int FailedPasswordAttemptCount { get; set; }

        public DateTime FailedPasswordAttemptWindowStartUtc
        {
            get
            {
                return this.failedPasswordAttemptWindowStart;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.failedPasswordAttemptWindowStart);
            }
        }

        public int FailedPasswordAnswerAttemptCount { get; set; }

        public DateTime FailedPasswordAnswerAttemptWindowStartUtc
        {
            get
            {
                return this.failedPasswordAnswerAttemptWindowStart;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.failedPasswordAnswerAttemptWindowStart);
            }
        }

        public string Comment
        {
            get
            {
                return this.comment;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.comment = value;
            }
        }

        #region Profile provider related properties

        public DateTime ProfileLastUpdatedUtc
        {
            get
            {
                return this.profileLastUpdated;
            }

            set
            {
                SecUtility.SetUtcTime(value, out this.profileLastUpdated);
            }
        }

        public int ProfileSize { get; set; }

        public string ProfileBlobName
        {
            get
            {
                return this.profileBlobName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value..");
                }

                this.profileBlobName = value;
            }
        }

        public bool ProfileIsCreatedByProfileProvider { get; set; }

        #endregion

        public int CompareTo(object obj)
        {
            if (obj == null)
            {
                return 1;
            }

            var row = obj as MembershipRow;
            if (row == null)
            {
                throw new ArgumentException("The parameter obj is not of type MembershipRow.");
            }

            return string.Compare(this.UserName, row.UserName, StringComparison.Ordinal);
        }
    }
}