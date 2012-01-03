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

namespace Microsoft.WindowsAzure.Samples.Common.Providers
{
    using System;

    public class TimeProvider : ITimeProvider
    {
        private static TimeProvider currentTimeProvider;

        static TimeProvider()
        {
            Current = new UtcTimeProvider();
        }

        public static ITimeProvider Current
        {
            get
            {
                return currentTimeProvider;
            }

            set
            {
                if (value is TimeProvider)
                {
                    currentTimeProvider = (TimeProvider)value;
                }
                else
                {
                    var timeProvider = new TimeProvider { CurrentTimeProviderImplementation = value };
                    currentTimeProvider = timeProvider;
                }
            }
        }

        public ITimeProvider CurrentTimeProviderImplementation { get; set; }

        public DateTime CurrentDateTime
        {
            get
            {
                return this.CurrentTimeProviderImplementation.CurrentDateTime;
            }
        }
    }
}