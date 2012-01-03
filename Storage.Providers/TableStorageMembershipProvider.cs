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
    using System.Collections.Generic;
    using System.Collections.Specialized;
    using System.Configuration.Provider;
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Globalization;
    using System.Linq;
    using System.Net;
    using System.Security;
    using System.Security.Cryptography;
    using System.Text;
    using System.Text.RegularExpressions;
    using System.Web;
    using System.Web.Security;
    using Microsoft.WindowsAzure;
    using Microsoft.WindowsAzure.StorageClient;

    public class TableStorageMembershipProvider : MembershipProvider
    {
        #region Member variables and constants

        private const int MaxTablePasswordSize = 128;
        private const int MaxTableEmailLength = 256;
        private const int MaxTablePasswordQuestionLength = 256;
        private const int MaxTablePasswordAnswerLength = 128;
        private const int MaxFindUserSize = 10000;

        // this is the absolute minimum password size when generating new password
        // the number is chosen so that it corresponds to the SQL membership provider implementation
        private const int MinGeneratedPasswordSize = 14;
        private const int NumRetries = 3;

        private readonly object lockObject = new object();

        // retry policies are used sparingly throughout this class because we often want to be
        // very conservative when it comes to security-related functions
        private readonly RetryPolicy tableRetryPolicy = RetryPolicies.Retry(NumRetries, TimeSpan.FromSeconds(1));

        // member variables shared between most providers
        private string applicationName;
        private string tableName;
        private CloudTableClient tableStorage;

        // membership provider specific member variables
        private bool enablePasswordRetrieval;
        private bool enablePasswordReset;
        private bool requiresQuestionAndAnswer;
        private bool requiresUniqueEmail;
        private int maxInvalidPasswordAttempts;
        private int passwordAttemptWindow;
        private int minRequiredPasswordLength;
        private int minRequiredNonalphanumericCharacters;
        private string passwordStrengthRegularExpression;
        private MembershipPasswordFormat passwordFormat;

        #endregion

        #region Properties

        /// <summary>
        /// The app name is not used in this implementation. 
        /// </summary>
        public override string ApplicationName
        {
            get { return this.applicationName; }

            set
            {
                lock (this.lockObject)
                {
                    SecUtility.CheckParameter(ref value, true, true, true, Constants.MaxTableApplicationNameLength, "ApplicationName");
                    this.applicationName = value;
                }
            }
        }

        public override bool EnablePasswordRetrieval
        {
            get
            {
                return this.enablePasswordRetrieval;
            }
        }

        public override bool EnablePasswordReset
        {
            get
            {
                return this.enablePasswordReset;
            }
        }

        public override bool RequiresQuestionAndAnswer
        {
            get
            {
                return this.requiresQuestionAndAnswer;
            }
        }

        public override bool RequiresUniqueEmail
        {
            get
            {
                return this.requiresUniqueEmail;
            }
        }

        public override MembershipPasswordFormat PasswordFormat
        {
            get
            {
                return this.passwordFormat;
            }
        }

        public override int MaxInvalidPasswordAttempts
        {
            get
            {
                return this.maxInvalidPasswordAttempts;
            }
        }

        public override int PasswordAttemptWindow
        {
            get
            {
                return this.passwordAttemptWindow;
            }
        }

        public override int MinRequiredPasswordLength
        {
            get
            {
                return this.minRequiredPasswordLength;
            }
        }

        public override int MinRequiredNonAlphanumericCharacters
        {
            get
            {
                return this.minRequiredNonalphanumericCharacters;
            }
        }

        public override string PasswordStrengthRegularExpression
        {
            get
            {
                return this.passwordStrengthRegularExpression;
            }
        }

        #endregion

        #region Public methods

        /// <summary>
        /// Initializes the membership provider. This is the only function that cannot be accessed
        /// in parallel by multiple applications. The function reads the properties of the 
        /// provider specified in the Web.config file and stores them in member variables.
        /// </summary>
        public override void Initialize(string name, NameValueCollection config)
        {
            if (config == null)
            {
                throw new ArgumentNullException("config");
            }

            if (string.IsNullOrEmpty(name))
            {
                name = "TableStorageMembershipProvider";
            }

            if (string.IsNullOrEmpty(config["description"]))
            {
                config.Remove("description");
                config.Add("description", "Table storage-based membership provider");
            }

            base.Initialize(name, config);

            bool allowInsecureRemoteEndpoints = ProvidersConfiguration.GetBooleanValue(config, "allowInsecureRemoteEndpoints", false);
            this.enablePasswordRetrieval = ProvidersConfiguration.GetBooleanValue(config, "enablePasswordRetrieval", false);
            this.enablePasswordReset = ProvidersConfiguration.GetBooleanValue(config, "enablePasswordReset", true);
            this.requiresQuestionAndAnswer = ProvidersConfiguration.GetBooleanValue(config, "requiresQuestionAndAnswer", true);
            this.requiresUniqueEmail = ProvidersConfiguration.GetBooleanValue(config, "requiresUniqueEmail", true);
            this.maxInvalidPasswordAttempts = ProvidersConfiguration.GetIntValue(config, "maxInvalidPasswordAttempts", 5, false, 0);
            this.passwordAttemptWindow = ProvidersConfiguration.GetIntValue(config, "passwordAttemptWindow", 10, false, 0);
            this.minRequiredPasswordLength = ProvidersConfiguration.GetIntValue(config, "minRequiredPasswordLength", 7, false, MaxTablePasswordSize);
            this.minRequiredNonalphanumericCharacters = ProvidersConfiguration.GetIntValue(config, "minRequiredNonalphanumericCharacters", 1, true, MaxTablePasswordSize);

            this.passwordStrengthRegularExpression = config["passwordStrengthRegularExpression"];
            if (this.passwordStrengthRegularExpression != null)
            {
                this.passwordStrengthRegularExpression = this.passwordStrengthRegularExpression.Trim();
                if (this.passwordStrengthRegularExpression.Length != 0)
                {
                    try
                    {
                        Regex testIfRegexIsValid = new Regex(this.passwordStrengthRegularExpression);
                    }
                    catch (ArgumentException e)
                    {
                        throw new ProviderException(e.Message, e);
                    }
                }
            }
            else
            {
                this.passwordStrengthRegularExpression = string.Empty;
            }

            if (this.minRequiredNonalphanumericCharacters > this.minRequiredPasswordLength)
            {
                throw new HttpException("The minRequiredNonalphanumericCharacters can not be greater than minRequiredPasswordLength.");
            }

            string strTemp = config["passwordFormat"];
            if (strTemp == null)
            {
                strTemp = "Hashed";
            }

            switch (strTemp)
            {
                case "Clear":
                    this.passwordFormat = MembershipPasswordFormat.Clear;
                    break;
                case "Encrypted":
                    this.passwordFormat = MembershipPasswordFormat.Encrypted;
                    break;
                case "Hashed":
                    this.passwordFormat = MembershipPasswordFormat.Hashed;
                    break;
                default:
                    throw new ProviderException("Password format specified is invalid.");
            }

            if (this.PasswordFormat == MembershipPasswordFormat.Hashed && this.EnablePasswordRetrieval)
            {
                throw new ProviderException("Configured settings are invalid: Hashed passwords cannot be retrieved. Either set the password format to different type, or set supportsPasswordRetrieval to false.");
            }
            
            // Table storage-related properties
            this.applicationName = ProvidersConfiguration.GetStringValueWithGlobalDefault(config, "applicationName", ProvidersConfiguration.DefaultProviderApplicationNameConfigurationString, ProvidersConfiguration.DefaultProviderApplicationName, false);
            this.tableName = ProvidersConfiguration.GetStringValueWithGlobalDefault(config, "membershipTableName", ProvidersConfiguration.DefaultMembershipTableNameConfigurationString, ProvidersConfiguration.DefaultMembershipTableName, false);

            config.Remove("allowInsecureRemoteEndpoints");
            config.Remove("enablePasswordRetrieval");
            config.Remove("enablePasswordReset");
            config.Remove("requiresQuestionAndAnswer");
            config.Remove("requiresUniqueEmail");
            config.Remove("maxInvalidPasswordAttempts");
            config.Remove("passwordAttemptWindow");
            config.Remove("passwordFormat");
            config.Remove("minRequiredPasswordLength");
            config.Remove("minRequiredNonalphanumericCharacters");
            config.Remove("passwordStrengthRegularExpression");
            config.Remove("applicationName");
            config.Remove("membershipTableName");

            // Throw an exception if unrecognized attributes remain
            if (config.Count > 0)
            {
                string attr = config.GetKey(0);
                if (!string.IsNullOrEmpty(attr))
                {
                    throw new ProviderException("Unrecognized attribute: " + attr);
                }
            }

            CloudStorageAccount account = null;
            try
            {
                account = ProvidersConfiguration.GetStorageAccount(ProvidersConfiguration.DefaultStorageConfigurationString);

                SecUtility.CheckAllowInsecureEndpoints(allowInsecureRemoteEndpoints, account.Credentials, account.TableEndpoint);
                this.tableStorage = account.CreateCloudTableClient();
                this.tableStorage.RetryPolicy = this.tableRetryPolicy;
                TableStorageExtensionMethods.CreateTableIfNotExist<MembershipRow>(this.tableStorage, this.tableName);
            }
            catch (SecurityException)
            {
                throw;
            }
            catch (Exception e)
            {
                string exceptionDescription = ProvidersConfiguration.GetInitExceptionDescription(account.Credentials, account.TableEndpoint, "table storage configuration");
                string membershipTableName = (this.tableName == null) ? "no membership table name specified" : this.tableName;
                var logMessage = "Could not create or find membership table: " + membershipTableName + "!"
                                 + Environment.NewLine + exceptionDescription + Environment.NewLine + e.Message
                                 + Environment.NewLine + e.StackTrace;
             
                Log.Write(EventKind.Error, logMessage);
                throw new ProviderException(
                                            "Could not create or find membership table. The most probable reason for this is that " +
                                            "the storage endpoints are not configured correctly. Please look at the configuration settings " +
                                            "in your .cscfg and Web.config files. More information about this error " +
                                            "can be found in the logs when running inside the hosting environment or in the output " +
                                            "window of Visual Studio.",
                                            e);
            }
        }

        /// <summary>
        /// Returns true if the username and password match an exsisting user.
        /// This implementation does not update a user's login time and does
        /// not raise corresponding Web events
        /// </summary>
        public override bool ValidateUser(string username, string password)
        {
            if (SecUtility.ValidateParameter(ref username, true, true, true, Constants.MaxTableUsernameLength) &&
                SecUtility.ValidateParameter(ref password, true, true, false, MaxTablePasswordSize) &&
                this.CheckPassword(username, password, true, true))
            {
                return true;
            }
            else
            {
                return false;
            }
        }

        /// <summary>
        /// Get a user based on the username parameter.
        /// If the userIsOnline parameter is set the lastActivity flag of the user
        /// is changed in the data store
        /// </summary>
        public override MembershipUser GetUser(object providerUserKey, bool userIsOnline)
        {
            if (providerUserKey == null)
            {
                throw new ArgumentNullException("providerUserKey");
            }

            if (providerUserKey.GetType() != typeof(Guid))
            {
                throw new ArgumentException("Provided key is not a Guid!");
            }

            Guid key = (Guid)providerUserKey;

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                // we need an IQueryable here because we do a Top(2) in the ProcessGetUserQuery() 
                // and cast it to DataServiceQuery object in this function
                // this does not work when we use IEnumerable as a type here
                IQueryable<MembershipRow> query = from user in queryObj
                                                  where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                                  user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                                  user.UserId == key &&
                                                  user.ProfileIsCreatedByProfileProvider == false
                                                  select user;
                return this.ProcessGetUserQuery(svc, query, userIsOnline);
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a user based on his/her username.
        /// The userIsOnline parameter determines whether to update the lastActivityDate of 
        /// the user in the data store
        /// </summary>
        public override MembershipUser GetUser(string username, bool userIsOnline)
        {
            SecUtility.CheckParameter(
                            ref username,
                            true,
                            false,
                            true,
                            Constants.MaxTableUsernameLength,
                            "username");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                // we need an IQueryable here because we do a Top(2) in the ProcessGetUserQuery() 
                // and cast it to DataServiceQuery object in this function
                // this does not work when we use IEnumerable as a type here
                IQueryable<MembershipRow> query = from user in queryObj
                                                  where user.PartitionKey == SecUtility.CombineToKey(this.applicationName, username) &&
                                                  user.ProfileIsCreatedByProfileProvider == false
                                                  select user;
                return this.ProcessGetUserQuery(svc, query, userIsOnline);
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a collection of all the users.
        /// </summary>
        public override MembershipUserCollection GetAllUsers(int pageIndex, int pageSize, out int totalRecords)
        {
            if (pageIndex < 0)
            {
                throw new ArgumentException("The page index cannot be negative.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("The page size can only be a positive integer.");
            }

            long upperBound = ((long)pageIndex * pageSize) + pageSize - 1;
            if (upperBound > int.MaxValue)
            {
                throw new ArgumentException("pageIndex and pageSize are too big.");
            }

            totalRecords = 0;
            MembershipUserCollection users = new MembershipUserCollection();
            TableServiceContext svc = this.CreateDataServiceContext();
            try
            {
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                var query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();

                IEnumerable<MembershipRow> allUsers = query.Execute();
                List<MembershipRow> allUsersSorted = new List<MembershipRow>(allUsers);

                // the result should already be sorted because the user name is part of the primary key
                // we have to sort anyway because we have encoded the user names in the key
                // this is also why we cannot use the table stoage pagination mechanism here and need to retrieve all elements
                // for every page
                allUsersSorted.Sort();

                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize;
                MembershipRow row;
                for (int i = startIndex; i < endIndex && i < allUsersSorted.Count; i++)
                {
                    row = allUsersSorted.ElementAt<MembershipRow>(i);
                    users.Add(
                        new MembershipUser(
                            this.Name,
                            row.UserName,
                            row.UserId,
                            row.Email,
                            row.PasswordQuestion,
                            row.Comment,
                            row.IsApproved,
                            row.IsLockedOut,
                            row.CreateDateUtc.ToLocalTime(),
                            row.LastLoginDateUtc.ToLocalTime(),
                            row.LastActivityDateUtc.ToLocalTime(),
                            row.LastPasswordChangedDateUtc.ToLocalTime(),
                            row.LastLockoutDateUtc.ToLocalTime()));
                }
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Changes a users password. We don't use retries in this highly security-related function. 
        /// All errors are exposed to the user of this function.
        /// </summary>
        public override bool ChangePassword(string username, string oldPassword, string newPassword)
        {
            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");
            SecUtility.CheckParameter(ref oldPassword, true, true, false, MaxTablePasswordSize, "oldPassword");
            SecUtility.CheckParameter(ref newPassword, true, true, false, MaxTablePasswordSize, "newPassword");

            try
            {
                string salt = null;
                MembershipRow member;
                TableServiceContext svc = this.CreateDataServiceContext();

                if (!CheckPassword(svc, username, oldPassword, false, false, out member))
                {
                    return false;
                }

                salt = member.PasswordSalt;
                int format = member.PasswordFormat;

                if (newPassword.Length < this.MinRequiredPasswordLength)
                {
                    throw new ArgumentException("The new password is to short.");
                }

                int count = 0;

                for (int i = 0; i < newPassword.Length; i++)
                {
                    if (!char.IsLetterOrDigit(newPassword, i))
                    {
                        count++;
                    }
                }

                if (count < this.MinRequiredNonAlphanumericCharacters)
                {
                    throw new ArgumentException("The new password does not have enough non-alphanumeric characters!");
                }

                if (this.PasswordStrengthRegularExpression.Length > 0)
                {
                    if (!Regex.IsMatch(newPassword, this.PasswordStrengthRegularExpression))
                    {
                        throw new ArgumentException("The new password does not match the specified password strength regular expression.");
                    }
                }

                string pass = this.EncodePassword(newPassword, (int)format, salt);
                if (pass.Length > MaxTablePasswordSize)
                {
                    throw new ArgumentException("Password is too long!");
                }

                var e = new ValidatePasswordEventArgs(username, newPassword, false);
                OnValidatingPassword(e);

                if (e.Cancel)
                {
                    if (e.FailureInformation != null)
                    {
                        throw e.FailureInformation;
                    }
                    else
                    {
                        throw new ArgumentException("Password validation failure!");
                    }
                }

                member.Password = pass;
                member.PasswordSalt = salt;
                member.PasswordFormat = format;
                member.LastPasswordChangedDateUtc = DateTime.UtcNow;
                svc.UpdateObject(member);
                svc.SaveChanges();

                return true;
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Creates a new user and stores it in the membership table. We do not use retry policies in this 
        /// highly security-related function. All error conditions are directly exposed to the user.
        /// </summary>
        public override MembershipUser CreateUser(
            string username,
            string password,
            string email,
            string passwordQuestion,
            string passwordAnswer,
            bool isApproved,
            object providerUserKey,
            out MembershipCreateStatus status)
        {
            if (!SecUtility.ValidateParameter(ref password, true, true, false, MaxTablePasswordSize))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string salt = GenerateSalt();
            string pass = this.EncodePassword(password, (int)this.passwordFormat, salt);
            if (pass.Length > MaxTablePasswordSize)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            string encodedPasswordAnswer;
            if (passwordAnswer != null)
            {
                passwordAnswer = passwordAnswer.Trim();
            }

            if (!string.IsNullOrEmpty(passwordAnswer))
            {
                if (passwordAnswer.Length > MaxTablePasswordSize)
                {
                    status = MembershipCreateStatus.InvalidAnswer;
                    return null;
                }

                encodedPasswordAnswer = this.EncodePassword(passwordAnswer.ToLowerInvariant(), (int)this.passwordFormat, salt);
            }
            else
            {
                encodedPasswordAnswer = passwordAnswer;
            }

            if (!SecUtility.ValidateParameter(ref encodedPasswordAnswer, this.RequiresQuestionAndAnswer, true, false, MaxTablePasswordSize))
            {
                status = MembershipCreateStatus.InvalidAnswer;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref username, true, true, true, Constants.MaxTableUsernameLength))
            {
                status = MembershipCreateStatus.InvalidUserName;
                return null;
            }

            if (!SecUtility.ValidateParameter(
                                               ref email,
                                               this.RequiresUniqueEmail,
                                               this.RequiresUniqueEmail,
                                               false,
                                               Constants.MaxTableUsernameLength))
            {
                status = MembershipCreateStatus.InvalidEmail;
                return null;
            }

            if (!SecUtility.ValidateParameter(ref passwordQuestion, this.RequiresQuestionAndAnswer, true, false, Constants.MaxTableUsernameLength))
            {
                status = MembershipCreateStatus.InvalidQuestion;
                return null;
            }

            if (providerUserKey != null)
            {
                if (!(providerUserKey is Guid))
                {
                    status = MembershipCreateStatus.InvalidProviderUserKey;
                    return null;
                }
            }

            if (!this.EvaluatePasswordRequirements(password))
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            ValidatePasswordEventArgs e = new ValidatePasswordEventArgs(username, password, true);
            OnValidatingPassword(e);

            if (e.Cancel)
            {
                status = MembershipCreateStatus.InvalidPassword;
                return null;
            }

            // Check whether a user with the same email address already exists.
            // The danger here is (as we don't have transaction support here) that 
            // there are overlapping requests for creating two users with the same email 
            // address at the same point in time. 
            // A solution for this would be to have a separate table for email addresses.
            // At this point here in the code we would try to insert this user's email address into the 
            // table and thus check whether the email is unique (the email would be the primary key of the 
            // separate table). There are quite some problems
            // associated with that. For example, what happens if the user creation fails etc., stale data in the 
            // email table etc.
            // Another solution is to already insert the user at this point and then check at the end of this 
            // funcation whether the email is unique. 
            if (this.RequiresUniqueEmail && !this.IsUniqueEmail(email))
            {
                status = MembershipCreateStatus.DuplicateEmail;
                return null;
            }

            if (!this.IsUniqueUserName(username))
            {
                status = MembershipCreateStatus.DuplicateUserName;
                return null;
            }

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                var newUser = new MembershipRow(this.applicationName, username);
                if (providerUserKey == null)
                {
                    providerUserKey = Guid.NewGuid();
                }

                newUser.UserId = (Guid)providerUserKey;
                newUser.Password = pass;
                newUser.PasswordSalt = salt;
                newUser.Email = email ?? string.Empty;
                newUser.PasswordQuestion = passwordQuestion ?? string.Empty;
                newUser.PasswordAnswer = encodedPasswordAnswer ?? string.Empty;
                newUser.IsApproved = isApproved;
                newUser.PasswordFormat = (int)this.passwordFormat;
                DateTime now = DateTime.UtcNow;
                newUser.CreateDateUtc = now;
                newUser.LastActivityDateUtc = now;
                newUser.LastPasswordChangedDateUtc = now;
                newUser.LastLoginDateUtc = now;
                newUser.IsLockedOut = false;

                svc.AddObject(this.tableName, newUser);
                svc.SaveChanges();

                status = MembershipCreateStatus.Success;
                return new MembershipUser(
                                               this.Name,
                                               username,
                                               providerUserKey,
                                               email,
                                               passwordQuestion,
                                               null,
                                               isApproved,
                                               false,
                                               now.ToLocalTime(),
                                               now.ToLocalTime(),
                                               now.ToLocalTime(),
                                               now.ToLocalTime(),
                                               ProvidersConfiguration.MinSupportedDateTime);
            }
            catch (InvalidOperationException ex)
            {
                if (ex.InnerException is DataServiceClientException && (ex.InnerException as DataServiceClientException).StatusCode == (int)HttpStatusCode.Conflict)
                {
                    // in this case, some membership providers update the last activity time of the user
                    // we don't do this in this implementation because it would add another roundtrip
                    status = MembershipCreateStatus.DuplicateUserName;
                    return null;
                }
                else if (ex.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Cannot add user to membership data store because of problems when accessing the data store.", ex);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Deletes the user from the membership table.
        /// This implementation ignores the deleteAllRelatedData argument
        /// </summary>
        public override bool DeleteUser(string username, bool deleteAllRelatedData)
        {
            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                var user = new MembershipRow(this.applicationName, username);
                svc.AttachTo(this.tableName, user, "*");
                svc.DeleteObject(user);
                svc.SaveChangesWithRetries();
                return true;
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    var dsce = e.InnerException as DataServiceClientException;

                    if (dsce.StatusCode == (int)HttpStatusCode.NotFound)
                    {
                        return false;
                    }
                    else
                    {
                        throw new ProviderException("Error accessing the data source.", e);
                    }
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Retrieves a username based on a matching email.
        /// </summary>
        public override string GetUserNameByEmail(string email)
        {
            SecUtility.CheckParameter(ref email, false, false, false, MaxTableEmailLength, "email");

            string nonNullEmail = email ?? string.Empty;

            try
            {
                DataServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                var query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.Email == nonNullEmail &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();

                IEnumerable<MembershipRow> allUsers = query.Execute();
                if (allUsers == null)
                {
                    return null;
                }

                var allUsersList = new List<MembershipRow>(allUsers);
                if (allUsersList == null || allUsersList.Count < 1)
                {
                    return null;
                }

                if (allUsersList.Count > 1 && this.RequiresUniqueEmail)
                {
                    throw new ProviderException("No unique email address!");
                }

                MembershipRow firstMatch = allUsersList.ElementAt(0);
                return string.IsNullOrEmpty(firstMatch.Email) ? null : firstMatch.Email;
            }
            catch (InvalidOperationException e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Updates a user. The username will not be changed. We explicitly don't use a large retry policy statement between 
        /// reading the user data and updating the user data. 
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1725:ParameterNamesShouldMatchBaseDeclaration",
                                                         MessageId = "0#", Justification = "Code clarity.")]
        public override void UpdateUser(MembershipUser updatedUser)
        {
            if (updatedUser == null)
            {
                throw new ArgumentNullException("updatedUser");
            }

            try
            {
                string temp = updatedUser.UserName;
                SecUtility.CheckParameter(ref temp, true, true, true, Constants.MaxTableUsernameLength, "username");
                temp = updatedUser.Email;
                SecUtility.CheckParameter(ref temp, this.RequiresUniqueEmail, this.RequiresUniqueEmail, false, MaxTableEmailLength, "Email");
                updatedUser.Email = temp;

                MembershipRow member = null;
                if (this.RequiresUniqueEmail && !this.IsUniqueEmail(updatedUser.Email, out member) &&
                    member != null && member.UserName != updatedUser.UserName)
                {
                    throw new ProviderException("Not a unique email address!");
                }

                TableServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                var query = (from user in queryObj
                             where user.PartitionKey == SecUtility.CombineToKey(this.applicationName, updatedUser.UserName) &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();

                IEnumerable<MembershipRow> allUsers = query.Execute();

                if (allUsers == null)
                {
                    throw new ProviderException("Cannot update user. User not found.");
                }

                var allUsersList = new List<MembershipRow>(allUsers);
                if (allUsersList == null || allUsersList.Count != 1)
                {
                    throw new ProviderException("No or no unique user to update.");
                }

                MembershipRow userToUpdate = allUsersList.ElementAt(0);
                userToUpdate.Email = updatedUser.Email;
                userToUpdate.Comment = updatedUser.Comment ?? string.Empty;
                userToUpdate.IsApproved = updatedUser.IsApproved;
                userToUpdate.LastLoginDateUtc = updatedUser.LastLoginDate.ToUniversalTime();
                userToUpdate.LastActivityDateUtc = updatedUser.LastActivityDate.ToUniversalTime();

                svc.UpdateObject(userToUpdate);
                svc.SaveChangesWithRetries();
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        public virtual string GeneratePassword()
        {
            return Membership.GeneratePassword(
                      this.MinRequiredPasswordLength < MinGeneratedPasswordSize ? MinGeneratedPasswordSize : this.MinRequiredPasswordLength,
                      this.MinRequiredNonAlphanumericCharacters);
        }

        /// <summary>
        /// Reset the password of a user. No retry policies are used in this function.
        /// </summary>
        public override string ResetPassword(string username, string answer)
        {
            if (!this.EnablePasswordReset)
            {
                throw new NotSupportedException("Membership provider is configured to not allow password resets!");
            }

            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                MembershipRow member = this.GetUserFromTable(svc, username);
                if (member == null)
                {
                    throw new ProviderException(string.Format(CultureInfo.InstalledUICulture, "Couldn't find a unique user with the name {0}.", username));
                }

                if (member.IsLockedOut)
                {
                    throw new MembershipPasswordException(string.Format(CultureInfo.InstalledUICulture, "The user {0} is currently locked out!", username));
                }

                int format = member.PasswordFormat;
                string salt = member.PasswordSalt;
                string encodedPasswordAnswer;

                if (answer != null)
                {
                    answer = answer.Trim();
                }

                if (!string.IsNullOrEmpty(answer))
                {
                    encodedPasswordAnswer = this.EncodePassword(answer.ToLowerInvariant(), format, salt);
                }
                else
                {
                    encodedPasswordAnswer = answer;
                }

                SecUtility.CheckParameter(ref encodedPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MaxTablePasswordSize, "passwordAnswer");

                string newPassword = this.GeneratePassword();
                var e = new ValidatePasswordEventArgs(username, newPassword, false);
                OnValidatingPassword(e);
                if (e.Cancel)
                {
                    if (e.FailureInformation != null)
                    {
                        throw e.FailureInformation;
                    }
                    else
                    {
                        throw new ProviderException("Password validation failed.");
                    }
                }

                DateTime now = DateTime.UtcNow;
                Exception ex = null;
                if (encodedPasswordAnswer == null || encodedPasswordAnswer == member.PasswordAnswer)
                {
                    member.Password = this.EncodePassword(newPassword, (int)format, salt);
                    member.LastPasswordChangedDateUtc = now;
                    if (member.FailedPasswordAnswerAttemptCount > 0 && encodedPasswordAnswer != null)
                    {
                        member.FailedPasswordAnswerAttemptCount = 0;
                        member.FailedPasswordAnswerAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                    }
                }
                else
                {
                    if (now > member.FailedPasswordAnswerAttemptWindowStartUtc.Add(TimeSpan.FromMinutes(this.PasswordAttemptWindow)))
                    {
                        member.FailedPasswordAnswerAttemptWindowStartUtc = now;
                        member.FailedPasswordAnswerAttemptCount = 1;
                    }
                    else
                    {
                        member.FailedPasswordAnswerAttemptWindowStartUtc = now;
                        member.FailedPasswordAnswerAttemptCount++;
                    }

                    if (member.FailedPasswordAnswerAttemptCount >= this.MaxInvalidPasswordAttempts)
                    {
                        member.IsLockedOut = true;
                        member.LastLockoutDateUtc = now;
                    }

                    ex = new MembershipPasswordException("Wrong password answer.");
                }

                svc.UpdateObject(member);
                svc.SaveChanges();

                if (ex != null)
                {
                    throw ex;
                }

                return newPassword;
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Unlock a user
        /// </summary>
        public override bool UnlockUser(string userName)
        {
            SecUtility.CheckParameter(ref userName, true, true, true, Constants.MaxTableUsernameLength, "username");

            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                MembershipRow member = this.GetUserFromTable(svc, userName);
                if (member == null)
                {
                    return false;
                }

                member.IsLockedOut = false;
                member.FailedPasswordAttemptCount = 0;
                member.FailedPasswordAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                member.FailedPasswordAnswerAttemptCount = 0;
                member.FailedPasswordAnswerAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                member.LastLockoutDateUtc = ProvidersConfiguration.MinSupportedDateTime;
                svc.UpdateObject(member);
                svc.SaveChangesWithRetries();

                return true;
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Find users based on their email addresses.
        /// The emailToMatch must be a complete email string like abc@def.com or can contain a '%' character at the end.
        /// A '%' character at the end implies that arbitrary characters can follow. 
        /// Supporting additional searches right now would be very expensive because the filtering would have to be done on the 
        /// client side.
        /// </summary>
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Maintainability", "CA1506:AvoidExcessiveClassCoupling", Justification = "Predefined interface.")]
        public override MembershipUserCollection FindUsersByEmail(string emailToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            SecUtility.CheckParameter(ref emailToMatch, false, false, false, Constants.MaxTableUsernameLength, "emailToMatch");

            if (pageIndex < 0)
            {
                throw new ArgumentException("Page index must be a non-negative integer.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Page size must be a positive integer.");
            }

            if (emailToMatch == null)
            {
                emailToMatch = string.Empty;
            }

            bool startswith = false;
            if (emailToMatch.Contains('%'))
            {
                if (emailToMatch.IndexOf('%') != emailToMatch.Length - 1)
                {
                    throw new ArgumentException("The TableStorageMembershipProvider only supports search strings that contain '%' as the last character!");
                }

                emailToMatch = emailToMatch.Substring(0, emailToMatch.Length - 1);
                startswith = true;
            }

            long upperBound = ((long)pageIndex * pageSize) + pageSize - 1;
            if (upperBound > int.MaxValue)
            {
                throw new ArgumentException("Cannot return so many elements!");
            }

            MembershipUserCollection users = new MembershipUserCollection();
            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                CloudTableQuery<MembershipRow> query;
                if (startswith && string.IsNullOrEmpty(emailToMatch))
                {
                    query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();
                }
                else if (startswith)
                {
                    // so far, the table storage service does not support StartsWith; thus, we retrieve all users whose email is "larger" than the one 
                    // specified and do the comparison locally
                    // this can result in significant overhead
                    query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false &&
                                   user.Email.CompareTo(emailToMatch) >= 0
                             select user).AsTableServiceQuery();
                }
                else
                {
                    query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false &&
                                   user.Email == emailToMatch
                             select user).AsTableServiceQuery();
                }

                IEnumerable<MembershipRow> allUsers = query.Execute();

                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize;
                int i = 0;
                List<MembershipRow> allUsersList = new List<MembershipRow>(allUsers);
                allUsersList.Sort(new EmailComparer());
                MembershipRow row;
                bool userMatches = true;
                for (i = startIndex; i < endIndex && i < allUsersList.Count && userMatches; i++)
                {
                    row = allUsersList.ElementAt<MembershipRow>(i);
                    Debug.Assert(emailToMatch != null, "email to detach is null");
                    if (startswith && !string.IsNullOrEmpty(emailToMatch))
                    {
                        if (!row.Email.StartsWith(emailToMatch, StringComparison.Ordinal))
                        {
                            userMatches = false;
                            continue;
                        }
                    }

                    users.Add(new MembershipUser(
                                                 this.Name,
                                                 row.UserName,
                                                 row.UserId,
                                                 row.Email,
                                                 row.PasswordQuestion,
                                                 row.Comment,
                                                 row.IsApproved,
                                                 row.IsLockedOut,
                                                 row.CreateDateUtc.ToLocalTime(),
                                                 row.LastLoginDateUtc.ToLocalTime(),
                                                 row.LastActivityDateUtc.ToLocalTime(),
                                                 row.LastPasswordChangedDateUtc.ToLocalTime(),
                                                 row.LastLockoutDateUtc.ToLocalTime()));
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Find users by their names.
        /// The usernameToMatch must be the complete username like frank or can contain a '%' character at the end.
        /// A '%' character at the end implies that arbitrary characters can follow. 
        /// Supporting additional searches right now would be very expensive because the filtering would have to be done on the 
        /// client side; i.e., all users would have to be retrieved in order to do the filtering.
        /// IMPORTANT: because of this decision, user names must not contain a % character when using this function.
        /// </summary>
        public override MembershipUserCollection FindUsersByName(string usernameToMatch, int pageIndex, int pageSize, out int totalRecords)
        {
            SecUtility.CheckParameter(ref usernameToMatch, true, true, false, Constants.MaxTableUsernameLength, "usernameToMatch");

            if (pageIndex < 0)
            {
                throw new ArgumentException("Page index must be a non-negative integer.");
            }

            if (pageSize < 1)
            {
                throw new ArgumentException("Page size must be a positive integer.");
            }

            bool startswith = false;
            if (usernameToMatch.Contains('%'))
            {
                if (usernameToMatch.IndexOf('%') != usernameToMatch.Length - 1)
                {
                    throw new ArgumentException("The TableStorageMembershipProvider only supports search strings that contain '%' as the last character!");
                }

                usernameToMatch = usernameToMatch.Substring(0, usernameToMatch.Length - 1);
                startswith = true;
            }

            long upperBound = ((long)pageIndex * pageSize) + pageSize - 1;
            if (upperBound > int.MaxValue)
            {
                throw new ArgumentException("Cannot return so many elements!");
            }

            var users = new MembershipUserCollection();
            try
            {
                TableServiceContext svc = this.CreateDataServiceContext();
                DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

                CloudTableQuery<MembershipRow> query;
                if (startswith && string.IsNullOrEmpty(usernameToMatch))
                {
                    query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();
                }
                else if (startswith)
                {
                    // note that we cannot include the usernameToMatch in the query over the partition key because the partitionkey is escaped, which destroys
                    // the sorting order
                    // and yes, we get all users here whose username is larger than the usernameToMatch because StartsWith is not supported in the current
                    // table storage service
                    query = (from user in queryObj
                             where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                                   user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                                   user.UserName.CompareTo(usernameToMatch) >= 0 &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();
                }
                else
                {
                    query = (from user in queryObj
                             where user.PartitionKey == SecUtility.CombineToKey(this.applicationName, usernameToMatch) &&
                                   user.ProfileIsCreatedByProfileProvider == false
                             select user).AsTableServiceQuery();
                }

                IEnumerable<MembershipRow> allUsers = query.Execute();

                int startIndex = pageIndex * pageSize;
                int endIndex = startIndex + pageSize;
                int i;
                var allUsersList = new List<MembershipRow>(allUsers);

                // default sorting is by user name (not the escaped version in the partition key)
                allUsersList.Sort();
                bool userMatches = true;
                for (i = startIndex; i < endIndex && i < allUsersList.Count && userMatches; i++)
                {
                    MembershipRow row = allUsersList.ElementAt<MembershipRow>(i);
                    if (startswith && !string.IsNullOrEmpty(usernameToMatch))
                    {
                        if (!row.UserName.StartsWith(usernameToMatch, StringComparison.Ordinal))
                        {
                            userMatches = false;
                            continue;
                        }
                    }

                    users.Add(
                        new MembershipUser(
                                                 this.Name,
                                                 row.UserName,
                                                 row.UserId,
                                                 row.Email,
                                                 row.PasswordQuestion,
                                                 row.Comment,
                                                 row.IsApproved,
                                                 row.IsLockedOut,
                                                 row.CreateDateUtc.ToLocalTime(),
                                                 row.LastLoginDateUtc.ToLocalTime(),
                                                 row.LastActivityDateUtc.ToLocalTime(),
                                                 row.LastPasswordChangedDateUtc.ToLocalTime(),
                                                 row.LastLockoutDateUtc.ToLocalTime()));
                }
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }

            totalRecords = users.Count;
            return users;
        }

        /// <summary>
        /// Get the number of users that are currently online
        /// </summary>
        /// <returns></returns>
        public override int GetNumberOfUsersOnline()
        {
            TableServiceContext svc = this.CreateDataServiceContext();
            DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

            DateTime thresh = DateTime.UtcNow.Subtract(TimeSpan.FromMinutes(Membership.UserIsOnlineTimeWindow));
            var query = (from user in queryObj
                         where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                               user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                               user.LastActivityDateUtc > thresh &&
                               user.ProfileIsCreatedByProfileProvider == false
                         select user).AsTableServiceQuery();

            IEnumerable<MembershipRow> allUsers = query.Execute();
            if (allUsers == null)
            {
                return 0;
            }

            List<MembershipRow> allUsersList = new List<MembershipRow>(allUsers);
            if (allUsersList == null)
            {
                return 0;
            }

            return allUsersList.Count;
        }

        /// <summary>
        /// Change the password answer for a user.
        /// </summary>
        public override bool ChangePasswordQuestionAndAnswer(string username, string password, string newPasswordQuestion, string newPasswordAnswer)
        {
            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");
            SecUtility.CheckParameter(ref password, true, true, false, MaxTablePasswordSize, "password");

            try
            {
                MembershipRow member;
                TableServiceContext svc = this.CreateDataServiceContext();
                if (!CheckPassword(svc, username, password, false, false, out member))
                {
                    return false;
                }

                SecUtility.CheckParameter(ref newPasswordQuestion, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MaxTablePasswordQuestionLength, "newPasswordQuestion");
                string encodedPasswordAnswer;
                if (newPasswordAnswer != null)
                {
                    newPasswordAnswer = newPasswordAnswer.Trim();
                }

                SecUtility.CheckParameter(ref newPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MaxTablePasswordAnswerLength, "newPasswordAnswer");
                if (!string.IsNullOrEmpty(newPasswordAnswer))
                {
                    encodedPasswordAnswer = this.EncodePassword(newPasswordAnswer.ToLowerInvariant(), member.PasswordFormat, member.PasswordSalt);
                }
                else
                {
                    encodedPasswordAnswer = newPasswordAnswer;
                }

                SecUtility.CheckParameter(ref encodedPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MaxTablePasswordAnswerLength, "newPasswordAnswer");

                member.PasswordQuestion = newPasswordQuestion;
                member.PasswordAnswer = encodedPasswordAnswer;

                svc.UpdateObject(member);
                svc.SaveChanges();

                return true;
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        /// <summary>
        /// Gets the password of a user given the provided password answer
        /// </summary>
        public override string GetPassword(string username, string answer)
        {
            if (!this.EnablePasswordRetrieval)
            {
                throw new NotSupportedException("Membership provider is configured to reject password retrieval.");
            }

            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");

            try
            {
                if (answer != null)
                {
                    answer = answer.Trim();
                }

                string encodedPasswordAnswer;
                DataServiceContext svc = this.CreateDataServiceContext();
                MembershipRow member = this.GetUserFromTable(svc, username);
                if (member == null)
                {
                    throw new ProviderException("Couldn't find a unique user with that name.");
                }

                if (member.IsLockedOut)
                {
                    throw new MembershipPasswordException("User is locked out.");
                }

                if (string.IsNullOrEmpty(answer))
                {
                    encodedPasswordAnswer = answer;
                }
                else
                {
                    encodedPasswordAnswer = this.EncodePassword(answer.ToLowerInvariant(), member.PasswordFormat, member.PasswordSalt);
                }

                SecUtility.CheckParameter(ref encodedPasswordAnswer, this.RequiresQuestionAndAnswer, this.RequiresQuestionAndAnswer, false, MaxTablePasswordAnswerLength, "passwordAnswer");

                Exception ex = null;
                if (this.RequiresQuestionAndAnswer)
                {
                    DateTime now = DateTime.UtcNow;
                    if (string.IsNullOrEmpty(member.PasswordAnswer) || encodedPasswordAnswer != member.PasswordAnswer)
                    {
                        ex = new MembershipPasswordException("Password answer is invalid.");
                        if (now > member.FailedPasswordAnswerAttemptWindowStartUtc.Add(TimeSpan.FromMinutes(this.PasswordAttemptWindow)))
                        {
                            member.FailedPasswordAnswerAttemptWindowStartUtc = now;
                            member.FailedPasswordAnswerAttemptCount = 1;
                        }
                        else
                        {
                            member.FailedPasswordAnswerAttemptWindowStartUtc = now;
                            member.FailedPasswordAnswerAttemptCount++;
                        }

                        if (member.FailedPasswordAnswerAttemptCount >= this.MaxInvalidPasswordAttempts)
                        {
                            member.IsLockedOut = true;
                            member.LastLockoutDateUtc = now;
                        }
                    }
                    else
                    {
                        if (member.FailedPasswordAnswerAttemptCount > 0)
                        {
                            member.FailedPasswordAnswerAttemptCount = 0;
                            member.FailedPasswordAnswerAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                        }
                    }
                }

                svc.UpdateObject(member);
                svc.SaveChanges();
                if (ex != null)
                {
                    throw ex;
                }

                return this.UnEncodePassword(member.Password, member.PasswordFormat);
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException)
                {
                    throw new ProviderException("Error accessing the data source.", e);
                }
                else
                {
                    throw;
                }
            }
        }

        #endregion

        #region Private helper methods

        private static string GenerateSalt()
        {
            var buf = new byte[16];
            (new RNGCryptoServiceProvider()).GetBytes(buf);

            return Convert.ToBase64String(buf);
        }

        private TableServiceContext CreateDataServiceContext()
        {
            return this.tableStorage.GetDataServiceContext();
        }

        private MembershipUser ProcessGetUserQuery(TableServiceContext svc, IQueryable<MembershipRow> query, bool updateLastActivityDate)
        {
            if (query == null)
            {
                throw new ArgumentNullException("query");
            }

            // if no user is found, we return null
            MembershipUser res = null;

            // the GetUser query should return at most 1 result, we do a Take(2) to detect error conditions
            query = query.Take(2);

            IEnumerable<MembershipRow> queryResult = query.AsTableServiceQuery().Execute();
            if (queryResult == null)
            {
                return null;
            }

            var l = new List<MembershipRow>(queryResult);
            if (l.Count == 0)
            {
                return null;
            }
            else if (l.Count > 1)
            {
                throw new ProviderException("Non-unique primary keys!");
            }
            else
            {
                MembershipRow row = l.First();
                if (updateLastActivityDate)
                {
                    row.LastActivityDateUtc = DateTime.UtcNow;
                }

                res = new MembershipUser(
                                         this.Name,
                                         row.UserName,
                                         row.UserId,
                                         row.Email,
                                         row.PasswordQuestion,
                                         row.Comment,
                                         row.IsApproved,
                                         row.IsLockedOut,
                                         row.CreateDateUtc.ToLocalTime(),
                                         row.LastLoginDateUtc.ToLocalTime(),
                                         row.LastActivityDateUtc.ToLocalTime(),
                                         row.LastPasswordChangedDateUtc.ToLocalTime(),
                                         row.LastLockoutDateUtc.ToLocalTime());

                if (updateLastActivityDate)
                {
                    svc.UpdateObject(row);
                    svc.SaveChangesWithRetries();
                }
            }

            return res;
        }

        private bool IsUniqueUserName(string userName)
        {
            TableServiceContext svc = this.CreateDataServiceContext();
            DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

            return !(from user in queryObj
                     where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                           user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                           user.UserName.Equals(userName, StringComparison.OrdinalIgnoreCase) &&
                           user.ProfileIsCreatedByProfileProvider == false
                     select user).AsTableServiceQuery().Execute().Any();
        }

        private bool IsUniqueEmail(string email)
        {
            MembershipRow member;
            return IsUniqueEmail(email, out member);
        }

        private bool IsUniqueEmail(string email, out MembershipRow member)
        {
            member = null;
            SecUtility.ValidateParameter(ref email, true, true, true, ProvidersConfiguration.MaxStringPropertySizeInChars);

            TableServiceContext svc = this.CreateDataServiceContext();
            DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

            var query = (from user in queryObj
                         where user.PartitionKey.CompareTo(SecUtility.EscapedFirst(this.applicationName)) > 0 &&
                               user.PartitionKey.CompareTo(SecUtility.NextComparisonString(SecUtility.EscapedFirst(this.applicationName))) < 0 &&
                               user.Email == email &&
                               user.ProfileIsCreatedByProfileProvider == false
                         select user).AsTableServiceQuery();

            IEnumerable<MembershipRow> allUsers = query.Execute();
            if (allUsers == null)
            {
                return true;
            }

            IEnumerator<MembershipRow> e = allUsers.GetEnumerator();

            // e.Reset() throws a not implemented exception
            // according to the spec, the enumerator is at the beginning of the collections after a call to GetEnumerator()
            if (!e.MoveNext())
            {
                return true;
            }
            else
            {
                member = e.Current;
            }

            return false;
        }

        private bool CheckPassword(string username, string password, bool updateLastLoginActivityDate, bool failIfNotApproved)
        {
            MembershipRow member = null;
            return CheckPassword(username, password, updateLastLoginActivityDate, failIfNotApproved, out member);
        }

        private bool CheckPassword(string username, string password, bool updateLastLoginActivityDate, bool failIfNotApproved, out MembershipRow member)
        {
            return CheckPassword(null, username, password, updateLastLoginActivityDate, failIfNotApproved, out member);
        }

        private bool CheckPassword(DataServiceContext svc, string username, string password, bool updateLastLoginActivityDate, bool failIfNotApproved, out MembershipRow member)
        {
            bool createContextAndWriteState = false;
            try
            {
                if (svc == null)
                {
                    svc = this.CreateDataServiceContext();
                    createContextAndWriteState = true;
                }

                member = this.GetUserFromTable(svc, username);
                if (member == null)
                {
                    return false;
                }

                if (member.IsLockedOut)
                {
                    return false;
                }

                if (!member.IsApproved && failIfNotApproved)
                {
                    return false;
                }

                DateTime now = DateTime.UtcNow;
                string encodedPasswd = this.EncodePassword(password, member.PasswordFormat, member.PasswordSalt);

                bool isPasswordCorrect = member.Password.Equals(encodedPasswd);

                if (isPasswordCorrect && member.FailedPasswordAttemptCount == 0 && member.FailedPasswordAnswerAttemptCount == 0)
                {
                    if (createContextAndWriteState)
                    {
                        svc.UpdateObject(member);
                        svc.SaveChanges();
                    }

                    return true;
                }

                if (!isPasswordCorrect)
                {
                    if (now > member.FailedPasswordAttemptWindowStartUtc.Add(TimeSpan.FromMinutes(this.PasswordAttemptWindow)))
                    {
                        member.FailedPasswordAttemptWindowStartUtc = now;
                        member.FailedPasswordAttemptCount = 1;
                    }
                    else
                    {
                        member.FailedPasswordAttemptWindowStartUtc = now;
                        member.FailedPasswordAttemptCount++;
                    }

                    if (member.FailedPasswordAttemptCount >= this.MaxInvalidPasswordAttempts)
                    {
                        member.IsLockedOut = true;
                        member.LastLockoutDateUtc = now;
                    }
                }
                else
                {
                    if (member.FailedPasswordAttemptCount > 0 || member.FailedPasswordAnswerAttemptCount > 0)
                    {
                        member.FailedPasswordAnswerAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                        member.FailedPasswordAnswerAttemptCount = 0;
                        member.FailedPasswordAttemptWindowStartUtc = ProvidersConfiguration.MinSupportedDateTime;
                        member.FailedPasswordAttemptCount = 0;
                        member.LastLockoutDateUtc = ProvidersConfiguration.MinSupportedDateTime;
                    }
                }

                if (isPasswordCorrect && updateLastLoginActivityDate)
                {
                    member.LastActivityDateUtc = now;
                    member.LastLoginDateUtc = now;
                }

                if (createContextAndWriteState)
                {
                    svc.UpdateObject(member);
                    svc.SaveChanges();
                }

                return isPasswordCorrect;
            }
            catch (Exception e)
            {
                if (e.InnerException is DataServiceClientException && (e.InnerException as DataServiceClientException).StatusCode == (int)HttpStatusCode.PreconditionFailed)
                {
                    // this element was changed between read and writes
                    Log.Write(EventKind.Warning, "A membership element has been changed between read and writes.");
                    member = null;
                    return false;
                }
                else
                {
                    throw new ProviderException("Error accessing the data store!", e);
                }
            }
        }

        private TimeSpan PasswordAttemptWindowAsTimeSpan()
        {
            return new TimeSpan(0, this.PasswordAttemptWindow, 0);
        }

        private MembershipRow GetUserFromTable(DataServiceContext svc, string username)
        {
            SecUtility.CheckParameter(ref username, true, true, true, Constants.MaxTableUsernameLength, "username");

            DataServiceQuery<MembershipRow> queryObj = svc.CreateQuery<MembershipRow>(this.tableName);

            var query = (from user in queryObj
                         where user.PartitionKey == SecUtility.CombineToKey(this.applicationName, username) &&
                               user.ProfileIsCreatedByProfileProvider == false
                         select user).AsTableServiceQuery();

            IEnumerable<MembershipRow> allUsers = query.Execute();
            if (allUsers == null)
            {
                return null;
            }

            IEnumerator<MembershipRow> e = allUsers.GetEnumerator();
            if (e == null)
            {
                return null;
            }

            // e.Reset() throws a not implemented exception
            // according to the spec, the enumerator is at the beginning of the collections after a call to GetEnumerator()
            if (!e.MoveNext())
            {
                return null;
            }

            MembershipRow ret = e.Current;
            if (e.MoveNext())
            {
                throw new ProviderException("Duplicate elements for primary keys application and user name.");
            }

            return ret;
        }

        private bool EvaluatePasswordRequirements(string password)
        {
            if (password.Length < this.MinRequiredPasswordLength)
            {
                return false;
            }

            int count = 0;
            for (int i = 0; i < password.Length; i++)
            {
                if (!char.IsLetterOrDigit(password, i))
                {
                    count++;
                }
            }

            if (count < this.MinRequiredNonAlphanumericCharacters)
            {
                return false;
            }

            if (this.PasswordStrengthRegularExpression.Length > 0)
            {
                if (!Regex.IsMatch(password, this.PasswordStrengthRegularExpression))
                {
                    return false;
                }
            }

            return true;
        }

        private string EncodePassword(string pass, int passwordFormat, string salt)
        {
            if (passwordFormat == 0)
            { // MembershipPasswordFormat.Clear
                return pass;
            }

            byte[] bIn = Encoding.Unicode.GetBytes(pass);
            byte[] convertedSalt = Convert.FromBase64String(salt);
            byte[] all = new byte[convertedSalt.Length + bIn.Length];
            byte[] result;

            Buffer.BlockCopy(convertedSalt, 0, all, 0, convertedSalt.Length);
            Buffer.BlockCopy(bIn, 0, all, convertedSalt.Length, bIn.Length);
            if (passwordFormat == 1)
            { // MembershipPasswordFormat.Hashed
                HashAlgorithm s = HashAlgorithm.Create(Membership.HashAlgorithmType);
                result = s.ComputeHash(all);
            }
            else
            {
                result = EncryptPassword(all);
            }

            return Convert.ToBase64String(result);
        }

        private string UnEncodePassword(string pass, int passwordFormat)
        {
            switch (passwordFormat)
            {
                case 0: // MembershipPasswordFormat.Clear:
                    return pass;
                case 1: // MembershipPasswordFormat.Hashed:
                    throw new ProviderException("Hashed password cannot be decrypted.");
                default:
                    byte[] bIn = Convert.FromBase64String(pass);
                    byte[] result = DecryptPassword(bIn);
                    if (result == null)
                    {
                        return null;
                    }

                    return Encoding.Unicode.GetString(result, 16, result.Length - 16);
            }
        }

        #endregion
    }
}
