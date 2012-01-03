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

namespace Microsoft.WindowsAzure.Samples.Phone.Storage
{
    using System;
    using System.Globalization;
    using System.IO;
    using System.Net;
    using System.Xml.Linq;

    public static class Helpers
    {
        internal const string DataServicesMetadataNamespace = "http://schemas.microsoft.com/ado/2007/08/dataservices/metadata";

        internal static readonly XName LowerCaseDataServiceMessageXName = XName.Get("message", DataServicesMetadataNamespace);
        internal static readonly XName UpperCaseDataServiceMessageXName = XName.Get("Message", DataServicesMetadataNamespace);
        internal static readonly XName DataServiceInnerErrorXName = XName.Get("innererror", DataServicesMetadataNamespace);

        internal static readonly XName LowerCaseMessageXName = XName.Get("message");
        internal static readonly XName UpperCaseMessageXName = XName.Get("Message");
        internal static readonly XName InnerErrorXName = XName.Get("innererror");

        public static string ParseDataServiceException(Exception exception)
        {
            if (exception == null)
                return string.Empty;

            try
            {
                var baseException = exception.GetBaseException();
                var document = XDocument.Parse(baseException.Message);

                return ParseXmlExceptionDocument(document.Root).Message;
            }
            catch
            {
                return exception.GetBaseException().Message;
            }
        }

        public static string ParseXmlWebException(WebException webException)
        {
            if (webException == null)
                return string.Empty;

            try
            {
                var stream = webException.Response.GetResponseStream();
                stream.Position = 0;

                var streamReader = new StreamReader(stream);
                var document = XDocument.Parse(streamReader.ReadToEnd());

                return ParseXmlExceptionDocument(document.Root).Message;
            }
            catch
            {
                return ParseWebException(webException);
            }
        }

        public static string ParseWebException(WebException webException)
        {
            if (webException == null)
                return string.Empty;

            var responseContent = webException.Response.GetResponseString();

            if (!string.IsNullOrWhiteSpace(responseContent))
                return responseContent;

            var response = webException.Response as HttpWebResponse;
            if (response != null)
                return response.StatusCode.ToString();

            return webException.GetBaseException().Message;
        }

        internal static WebHeaderCollection ParseQueryString(string queryString)
        {
            var res = new WebHeaderCollection();
            if (queryString != null)
            {
                int num = queryString.Length;

                for (int i = 0; i < num; i++)
                {
                    var startIndex = i;
                    var num4 = -1;
                    while (i < num)
                    {
                        char ch = queryString[i];
                        if (ch == '=')
                        {
                            if (num4 < 0)
                                num4 = i;
                        }
                        else if (ch == '&')
                        {
                            break;
                        }

                        i++;
                    }

                    var str = string.Empty;
                    string str2;
                    if (num4 >= 0)
                    {
                        str = queryString.Substring(startIndex, num4 - startIndex);
                        str2 = queryString.Substring(num4 + 1, (i - num4) - 1);
                    }
                    else
                    {
                        str2 = queryString.Substring(startIndex, i - startIndex);
                    }

                    res[str.Replace("?", string.Empty)] = HttpUtility.UrlDecode(str2);
                }
            }

            return res;
        }

        internal static T PaseEnum<T>(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return default(T);

            try
            {
                return (T)Enum.Parse(typeof(T), value, true);
            }
            catch (ArgumentException)
            {
                return default(T);
            }
        }

        internal static DateTime ParseDateTimeUtc(string value)
        {
            if (string.IsNullOrWhiteSpace(value))
                return DateTime.MaxValue;

            DateTime date;
            if (!DateTime.TryParse(value, CultureInfo.InvariantCulture, DateTimeStyles.AdjustToUniversal, out date))
                date = DateTime.MaxValue;

            return date.ToUniversalTime();
        }

        private static Exception ParseXmlExceptionDocument(XElement errorElement)
        {
            switch (errorElement.Name.LocalName)
            {
                case "error":
                case "Error":
                case "innererror":
                    var innerException = errorElement.Element(InnerErrorXName) != null
                                        ? ParseXmlExceptionDocument(errorElement.Element(InnerErrorXName))
                                        : errorElement.Element(DataServiceInnerErrorXName) != null
                                            ? ParseXmlExceptionDocument(errorElement.Element(DataServiceInnerErrorXName))
                                            : null;

                    var errorMessage = string.Empty;

                    if (errorElement.Element(LowerCaseMessageXName) != null)
                        errorMessage = errorElement.Element(LowerCaseMessageXName).Value;
                    else if (errorElement.Element(UpperCaseMessageXName) != null)
                        errorMessage = errorElement.Element(UpperCaseMessageXName).Value;
                    else if (errorElement.Element(LowerCaseDataServiceMessageXName) != null)
                        errorMessage = errorElement.Element(LowerCaseDataServiceMessageXName).Value;
                    else if (errorElement.Element(UpperCaseDataServiceMessageXName) != null)
                        errorMessage = errorElement.Element(UpperCaseDataServiceMessageXName).Value;

                    return new Exception(errorMessage, innerException);

                default:
                    return null;
            }
        }
    }
}
