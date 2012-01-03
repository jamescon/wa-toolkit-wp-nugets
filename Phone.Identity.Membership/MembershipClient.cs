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
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Net.Browser;
    using System.Runtime.Serialization.Json;
    using System.Windows.Threading;

    using Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.Properties;

    public class MembershipClient : IMembershipClient
    {
        private const string LogOnOperation = "/logon";
        private const string GetUserNameOperation = "/username";
        private const string UsersResource = "/users";

        private readonly Uri membershipServiceUri;
        private readonly Dispatcher dispatcher;

        public MembershipClient(Uri membershipServiceUri)
            : this(membershipServiceUri, null)
        {
        }

        public MembershipClient(Uri membershipServiceUri, Dispatcher dispatcher)
        {
            if (membershipServiceUri == null)
                throw new ArgumentNullException("membershipServiceUri", Resource.MembershipServiceUriNullErrorMessage);

            this.membershipServiceUri = membershipServiceUri;
            this.dispatcher = dispatcher;
        }

        public void Register(string userName, string email, string password, string confirmPassword, Action<MembershipResponse> callback)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(Resource.UserNameNullErrorMessage, "userName");

            if (string.IsNullOrWhiteSpace(email))
                throw new ArgumentException(Resource.EmailNullErrorMessage, "email");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(Resource.PasswordNullErrorMessage, "password");

            if (string.IsNullOrWhiteSpace(confirmPassword))
                throw new ArgumentException(Resource.PasswordConfirmationNullErrorMessage, "confirmPassword");

            if (!password.Equals(confirmPassword, StringComparison.Ordinal))
                throw new ArgumentException(Resource.PasswordAndConfirmationDoNotMatchErrorMessage);

            var requestUri = ConcatPath(this.membershipServiceUri, UsersResource);
            var request = this.ResolveRequest(requestUri);
            request.Method = "PUT";

            this.SendRequest(request, new User { Name = userName, Email = email, Password = password }, callback);
        }

        public void LogOn(string userName, string password, Action<MembershipResponse> callback)
        {
            if (string.IsNullOrWhiteSpace(userName))
                throw new ArgumentException(Resource.UserNameNullErrorMessage, "userName");

            if (string.IsNullOrWhiteSpace(password))
                throw new ArgumentException(Resource.PasswordNullErrorMessage, "password");

            var requestUri = ConcatPath(this.membershipServiceUri, LogOnOperation);
            var request = this.ResolveRequest(requestUri);
            request.Method = "POST";

            this.SendRequest(request, new LogOn { UserName = userName, Password = password }, callback);
        }

        public void GetUserName(string membershipToken, Action<MembershipResponse> callback)
        {
            if (string.IsNullOrWhiteSpace(membershipToken))
                throw new ArgumentException(Resource.MembershipTokenNullErrorMessage, "membershipToken");

            var requestUri = ConcatPath(this.membershipServiceUri, GetUserNameOperation);
            var request = this.ResolveRequest(requestUri);
            request.Method = "GET";
            request.Headers["Authorization"] = string.Format(CultureInfo.InvariantCulture, "Membership {0}", membershipToken);

            request.BeginGetResponse(asyncResult => this.OnGetResponse(request, asyncResult, callback), request);
        }

        protected virtual HttpWebRequest ResolveRequest(Uri requestUri)
        {
            return (HttpWebRequest)WebRequestCreator.ClientHttp.Create(requestUri);
        }

        protected virtual void DispatchCallback(Action<MembershipResponse> callback, MembershipResponse response)
        {
            if (callback != null)
            {
                if (this.dispatcher != null)
                    this.dispatcher.BeginInvoke(() => callback(response));
                else
                    callback(response);
            }
        }

        private static Uri ConcatPath(Uri uri, string path)
        {
            var builder = new UriBuilder(uri);
            builder.Path = string.Concat(builder.Path.TrimEnd('/'), path);

            return builder.Uri;
        }

        private void SendRequest<T>(HttpWebRequest request, T body, Action<MembershipResponse> callback)
        {
            byte[] bodyContent;
            using (var stream = new MemoryStream())
            {
                var serializer = new DataContractJsonSerializer(typeof(T));
                serializer.WriteObject(stream, body);

                bodyContent = stream.ToArray();
            }

            request.ContentType = "application/json";
            request.Accept = "application/json";
            request.BeginGetRequestStream(
                ar =>
                {
                    var postStream = request.EndGetRequestStream(ar);

                    postStream.Write(bodyContent, 0, bodyContent.Length);
                    postStream.Close();

                    request.BeginGetResponse(asyncResult => this.OnGetResponse(request, asyncResult, callback), request);
                },
            request);
        }

        private void OnGetResponse(HttpWebRequest request, IAsyncResult asyncResult, Action<MembershipResponse> callback)
        {
            try
            {
                var httpResponse = (HttpWebResponse)request.EndGetResponse(asyncResult);
                MembershipResponse membershipResponse;
                var contentResponse = httpResponse.GetResponseString();

                if (httpResponse.StatusCode == HttpStatusCode.OK)
                {
                    membershipResponse = new MembershipResponse(
                        true,
                        httpResponse.StatusCode,
                        contentResponse,
                        string.Empty);
                }
                else
                {
                    membershipResponse = new MembershipResponse(
                        false,
                        httpResponse.StatusCode,
                        contentResponse,
                        "There was an error in the operation performed againts the Membership service.");
                }

                this.DispatchCallback(callback, membershipResponse);
            }
            catch (WebException webException)
            {
                var response = (HttpWebResponse)webException.Response;
                this.DispatchCallback(callback, new MembershipResponse(false, response.StatusCode, string.Empty, Helpers.ParseWebException(webException)));
            }
        }
    }
}
