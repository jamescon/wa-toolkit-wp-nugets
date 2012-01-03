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

namespace Microsoft.WindowsAzure.Samples.Common.JobEngine.Conditions
{
    using System;
    using System.Threading;

    public static class Schedule
    {
        public static Func<bool> Every(TimeSpan interval)
        {
            return new ScheduleCondition(interval).TickFunc;
        }

        public static Func<bool> Every(int milliseconds)
        {
            return new ScheduleCondition(TimeSpan.FromMilliseconds(milliseconds)).TickFunc;
        }

        private class ScheduleCondition : IDisposable
        {
            private readonly TimeSpan interval;
            private readonly System.Timers.Timer timer;
            private readonly AutoResetEvent signal = new AutoResetEvent(false);

            public ScheduleCondition(TimeSpan interval)
            {
                this.interval = interval;
                this.timer = new System.Timers.Timer(this.interval.TotalMilliseconds);
                this.timer.Elapsed += (sender, arg) => this.signal.Set();
                this.timer.Start();
            }

            public void Dispose()
            {
                timer.Dispose();
                signal.Dispose();
            }

            public Func<bool> TickFunc
            {
                get
                {
                    return () =>
                        {
                            this.signal.WaitOne();
                            return true;
                        };
                }
            }
        }
    }
}