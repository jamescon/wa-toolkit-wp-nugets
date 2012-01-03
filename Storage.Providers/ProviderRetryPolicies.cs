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
    using System.Data.Services.Client;
    using System.Diagnostics;
    using System.Net;
    using System.Threading;

    /// <summary>
    /// We are using this retry policies for only one purpose: the ASP providers often read data from the server, process it 
    /// locally and then write the result back to the server. The problem is that the row that has been read might have changed 
    /// between the read and write operation. This retry policy is used to retry the whole process in this case.
    /// </summary>
    /// <summary>
    /// Provides definitions for some standard retry policies.
    /// </summary>
    public static class ProviderRetryPolicies
    {
        public static readonly TimeSpan StandardMinBackoff = TimeSpan.FromMilliseconds(100);
        public static readonly TimeSpan StandardMaxBackoff = TimeSpan.FromSeconds(30);
        private static readonly Random Random = new Random();

        /// <summary>
        /// Policy that retries a specified number of times with an exponential backoff scheme
        /// </summary>
        /// <param name="numberOfRetries">The number of times to retry. Should be a non-negative number.</param>
        /// <param name="deltaBackoff">The multiplier in the exponential backoff scheme</param>
        /// <returns></returns>
        /// <remarks>For this retry policy, the minimum amount of milliseconds between retries is given by the 
        /// StandardMinBackoff constant, and the maximum backoff is predefined by the StandardMaxBackoff constant. 
        /// Otherwise, the backoff is calculated as random(2^currentRetry) * deltaBackoff.</remarks>
        public static SecUtility.ProviderRetryPolicy RetryN(int numberOfRetries, TimeSpan deltaBackoff)
        {
            return new SecUtility.ProviderRetryPolicy((Action action) =>
                {
                    RetryNImpl(action, numberOfRetries, StandardMinBackoff, StandardMaxBackoff, deltaBackoff);
                });
        }

        /// <summary>
        /// Policy that retries a specified number of times with an exponential backoff scheme
        /// </summary>
        /// <param name="numberOfRetries">The number of times to retry. Should be a non-negative number</param>
        /// <param name="deltaBackoff">The multiplier in the exponential backoff scheme</param>
        /// <param name="minBackoff">The minimum backoff interval</param>
        /// <param name="minBackoff">The minimum backoff interval</param>
        /// <returns></returns>
        /// <remarks>For this retry policy, the minimum amount of milliseconds between retries is given by the 
        /// minBackoff parameter, and the maximum backoff is predefined by the maxBackoff parameter. 
        /// Otherwise, the backoff is calculated as random(2^currentRetry) * deltaBackoff.</remarks>
        public static SecUtility.ProviderRetryPolicy RetryN(int numberOfRetries, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff)
        {
            return new SecUtility.ProviderRetryPolicy((Action action) =>
                {
                    RetryNImpl(action, numberOfRetries, minBackoff, maxBackoff, deltaBackoff);
                });
        }

        /// <summary>
        /// Policy that does no retries i.e., it just invokes <paramref name="action"/> exactly once
        /// </summary>
        /// <param name="action">The action to retry</param>
        /// <returns>The return value of <paramref name="action"/></returns>
        internal static void NoRetry(Action action)
        {
            action();
        }

        private static void RetryNImpl(Action action, int numberOfRetries, TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff)
        {
            int totalNumberOfRetries = numberOfRetries;
            int backoff;

            if (minBackoff > maxBackoff)
            {
                throw new ArgumentException("The minimum backoff must not be larger than the maximum backoff period.");
            }

            if (minBackoff.TotalMilliseconds < 0)
            {
                throw new ArgumentException("The minimum backoff period must not be negative.");
            }

            do
            {
                try
                {
                    action();
                    break;
                }
                catch (InvalidOperationException e)
                {
                    var dsce = e.InnerException as DataServiceClientException;

                    // precondition failed is the status code returned by the server to indicate that the etag is wrong
                    if (dsce != null)
                    {
                        var status = (HttpStatusCode)dsce.StatusCode;

                        if (status == HttpStatusCode.PreconditionFailed)
                        {
                            if (numberOfRetries == 0)
                            {
                                throw;
                            }

                            backoff = CalculateCurrentBackoff(minBackoff, maxBackoff, deltaBackoff, totalNumberOfRetries - numberOfRetries);
                            Debug.Assert(backoff >= minBackoff.TotalMilliseconds, "backof is smaller than minimum backof");
                            Debug.Assert(backoff <= maxBackoff.TotalMilliseconds, "backof is larger than maximum backof");
                            if (backoff > 0)
                            {
                                Thread.Sleep(backoff);
                            }
                        }
                        else
                        {
                            throw;
                        }
                    }
                    else
                    {
                        throw;
                    }
                }
            }
            while (numberOfRetries-- > 0);
        }

        private static int CalculateCurrentBackoff(TimeSpan minBackoff, TimeSpan maxBackoff, TimeSpan deltaBackoff, int curRetry)
        {
            int backoff;

            if (curRetry > 31)
            {
                backoff = (int)maxBackoff.TotalMilliseconds;
            }

            try
            {
                backoff = Random.Next((1 << curRetry) + 1);
                //// Console.WriteLine("backoff:" + backoff);
                //// Console.WriteLine("index:" + ((1 << curRetry) + 1));
                backoff *= (int)deltaBackoff.TotalMilliseconds;
                backoff += (int)minBackoff.TotalMilliseconds;
            }
            catch (OverflowException)
            {
                backoff = (int)maxBackoff.TotalMilliseconds;
            }

            if (backoff > (int)maxBackoff.TotalMilliseconds)
            {
                backoff = (int)maxBackoff.TotalMilliseconds;
            }

            Debug.Assert(backoff >= (int)minBackoff.TotalMilliseconds, "backoff is smaller than minimum backoff");

            // Console.WriteLine("real backoff:" + backoff);
            return backoff;
        }
    }
}