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

    using Microsoft.WindowsAzure.StorageClient;

    [CLSCompliant(false)]
    public class RoleRow : TableServiceEntity
    {
        private string applicationName;
        private string roleName;
        private string userName;
        
        // applicationName + userName is partitionKey
        // roleName is rowKey
        public RoleRow(string applicationName, string roleName, string userName)
        {
            SecUtility.CheckParameter(ref applicationName, true, true, true, Constants.MaxTableApplicationNameLength, "applicationName");
            SecUtility.CheckParameter(ref roleName, true, true, true, TableStorageRoleProvider.MaxTableRoleNameLength, "roleName");
            SecUtility.CheckParameter(ref userName, true, false, true, Constants.MaxTableUsernameLength, "userName");
            
            PartitionKey = SecUtility.CombineToKey(applicationName, userName);
            RowKey = SecUtility.Escape(roleName);
            this.ApplicationName = applicationName;
            this.RoleName = roleName;
            this.UserName = userName;
        }

        public RoleRow()
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

        public string RoleName
        {
            get
            {
                return this.roleName;
            }

            set
            {
                if (value == null)
                {
                    throw new ArgumentException("To ensure string values are always updated, this implementation does not allow null as a string value.");
                }

                this.roleName = value;
                this.RowKey = SecUtility.Escape(this.RoleName);
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
    }
}