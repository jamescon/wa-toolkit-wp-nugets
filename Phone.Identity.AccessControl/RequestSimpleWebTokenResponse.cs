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
    using System.IO;
    using System.Net;
    using System.Runtime.Serialization;
    using System.Runtime.Serialization.Json;
    using System.Text;
    using System.Xml;

    /// <summary>
    /// Contains the data returned in a RequestSimpleWebTokenResponse
    /// </summary>
    [DataContract]
    public class RequestSimpleWebTokenResponse
    {
        private const string WsSecuritySecExtNamespace = "http://docs.oasis-open.org/wss/2004/01/oasis-200401-wss-wssecurity-secext-1.0.xsd";

        private const string BinarySecurityTokenName = "BinarySecurityToken";

        /// <summary>
        /// The raw string value of the security token contained in the RequestSimpleWebTokenResponse
        /// </summary>
        [DataMember(Name = "securityToken")]
        public string SecurityToken { get; set; }

        /// <summary>
        /// The uri which uniquely identifies the type of token contained in the RequestSimpleWebTokenResponse
        /// </summary>
        [DataMember(Name = "tokenType")]
        public string TokenType { get; set; }

        /// <summary>
        /// The expiration time of the token in the RequestSimpleWebTokenResponse
        /// </summary>
        [DataMember(Name = "expires")]
        public long Expires { get; set; }

        /// <summary>
        /// The creation time of the token in the RequestSimpleWebTokenResponse
        /// </summary>
        [DataMember(Name = "created")]
        public long Created { get; set; }

        internal static RequestSimpleWebTokenResponse FromJson(string jsonRequestSecurityTokenService)
        {
            RequestSimpleWebTokenResponse returnToken;

            using (var memoryStream = new MemoryStream(Encoding.UTF8.GetBytes(jsonRequestSecurityTokenService)))
            {
                var serializer = new DataContractJsonSerializer(typeof(RequestSimpleWebTokenResponse));
                returnToken = serializer.ReadObject(memoryStream) as RequestSimpleWebTokenResponse;

                returnToken.SecurityToken = HttpUtility.HtmlDecode(returnToken.SecurityToken);

                using (var sr = new StringReader(returnToken.SecurityToken))
                {
                    var reader = XmlReader.Create(sr);
                    reader.MoveToContent();
                    var binaryToken = reader.ReadElementContentAsString(BinarySecurityTokenName, WsSecuritySecExtNamespace);
                    var tokenBytes = Convert.FromBase64String(binaryToken);
                    returnToken.SecurityToken = Encoding.UTF8.GetString(tokenBytes, 0, tokenBytes.Length);
                }
            }

            return returnToken;
        }
    }
}