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

namespace Microsoft.WindowsAzure.Samples.CloudServices.Identity.Membership
{
    using System;
    using System.Net;
    using System.Net.Http;
    using System.ServiceModel;
    using System.ServiceModel.Activation;
    using System.ServiceModel.Web;
    using System.Web.Security;
    using Microsoft.ApplicationServer.Http.Dispatcher;
    using Microsoft.Net.Http;
    using Properties;

    [ServiceContract]
    [ServiceBehavior(IncludeExceptionDetailInFaults = false)]
    [AspNetCompatibilityRequirements(RequirementsMode = AspNetCompatibilityRequirementsMode.Required)]
    public class MembershipService
    {
        private readonly MembershipProvider membershipProvider;

        public MembershipService()
            : this(Membership.Provider)
        {
        }

        [CLSCompliant(false)]
        public MembershipService(MembershipProvider membershipProvider)
        {
            this.membershipProvider = membershipProvider ?? Membership.Provider;
        }

        [WebInvoke(UriTemplate = "logon", Method = "POST")]
        public HttpResponseMessage LogOnUser(LogOn logOn)
        {
            if ((logOn == null) || string.IsNullOrWhiteSpace(logOn.UserName) || string.IsNullOrWhiteSpace(logOn.Password))
            {
                throw WebException(Resources.InvalidCredentialsMessage, HttpStatusCode.BadRequest);
            }

            bool isValidUser;
            try
            {
                isValidUser = this.membershipProvider.ValidateUser(logOn.UserName, logOn.Password);
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }

            if (!isValidUser)
            {
                throw WebException(Resources.InvalidCredentialsMessage, HttpStatusCode.Unauthorized);
            }

            var token = this.GenerateMembershipToken(logOn.UserName);
            return new HttpResponseMessage(HttpStatusCode.OK) { Content = new StringContent(token) };
        }

        [Authenticate, WebGet(UriTemplate = "username")]
        public HttpResponseMessage UserName(HttpRequestMessage message)
        {
            try
            {
                var username = message.User().Identity.Name;

                if (username != null)
                {
                    var user = this.membershipProvider.GetUser(username, false);
                    if (user != null)
                    {
                        var response = new HttpResponseMessage(HttpStatusCode.OK)
                            { Content = new StringContent(user.UserName) };
                        return response;
                    }
                }

                return new HttpResponseMessage(HttpStatusCode.NotFound);
            }
            catch (Exception exception)
            {
                throw WebException(exception.Message, HttpStatusCode.InternalServerError);
            }
        }

        [WebInvoke(UriTemplate = "users", Method = "PUT")]
        public HttpResponseMessage<User> CreateUser(User user)
        {
            MembershipCreateStatus createStatus;

            if ((user == null) || string.IsNullOrWhiteSpace(user.Name) || string.IsNullOrWhiteSpace(user.Email) || string.IsNullOrWhiteSpace(user.Password))
            {
                throw WebException(Resources.InvalidUserInformation, HttpStatusCode.BadRequest);
            }

            this.membershipProvider.CreateUser(user.Name, user.Password, user.Email, null, null, true, null, out createStatus);
            switch (createStatus)
            {
                case MembershipCreateStatus.Success:
                    return new HttpResponseMessage<User>(user, HttpStatusCode.OK);
                case MembershipCreateStatus.DuplicateUserName:
                    throw WebException(createStatus.ToString(), HttpStatusCode.Conflict);
                case MembershipCreateStatus.DuplicateEmail:
                    throw WebException(createStatus.ToString(), HttpStatusCode.Conflict);
                default:
                    throw WebException(createStatus.ToString(), HttpStatusCode.BadRequest);
            }
        }

        protected virtual string GenerateMembershipToken(string userName)
        {
            var ticket = new FormsAuthenticationTicket(userName, false, int.MaxValue);
            return FormsAuthentication.Encrypt(ticket);
        }

        private static HttpResponseException WebException(string message, HttpStatusCode code)
        {
            var result = new HttpResponseException(new HttpResponseMessage(code) { Content = new StringContent(message) });

            if (code == HttpStatusCode.Unauthorized)
            {
                result.Response.Headers.Add(System.Web.SuppressFormsAuthenticationRedirectModule.SuppressFormsHeaderName, "true");
            }

            return result;
        }
    }
}