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
    using System.Diagnostics;

    public static class Log
    {
        internal static void Write(EventKind eventKind, string message, params object[] args)
        {
            switch (eventKind)
            {
                case EventKind.Error:
                case EventKind.Critical:
                    Trace.TraceError(message, args);
                    break;
                case EventKind.Warning:
                    Trace.TraceWarning(message, args);
                    break;
                case EventKind.Information:
                case EventKind.Verbose:
                    Trace.TraceInformation(message, args);
                    break;
            }
        }
    }
}