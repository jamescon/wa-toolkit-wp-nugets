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

// Copyright 2010 Microsoft Corporation
//
// Licensed under the Apache License, Version 2.0 (the "License"); you may not use this file except in compliance with the License. 
// You may obtain a copy of the License at
//
// http://www.apache.org/licenses/LICENSE-2.0
//
// Unless required by applicable law or agreed to in writing, software distributed under the License is distributed on an 
// "AS IS" BASIS, WITHOUT WARRANTIES OR CONDITIONS OF ANY KIND, either express or implied. 
// See the License for the specific language governing permissions and limitations under the License.

namespace Microsoft.WindowsAzure.Samples.Data.Services.Client
{
    using System;
    using System.Collections.Generic;
    using System.Diagnostics;
    using System.Reflection;

    internal static class TypeSystem
    {
        private static readonly Dictionary<MethodInfo, string> ExpressionMethodMap;

        private static readonly Dictionary<string, string> ExpressionVBMethodMap;

        private static readonly Dictionary<PropertyInfo, MethodInfo> PropertiesAsMethodsMap;

        private const string VisualBasicAssemblyFullName = "Microsoft.VisualBasic, Version=2.0.5.0, Culture=neutral, PublicKeyToken=" + AssemblyRef.MicrosoftSilverlightPublicKeyToken;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1810:InitializeReferenceTypeStaticFieldsInline", Justification = "Cleaner code")]
        static TypeSystem()
        {
            const int ExpectedCount = 22;

            ExpressionMethodMap = new Dictionary<MethodInfo, string>(ExpectedCount, EqualityComparer<MethodInfo>.Default);
            ExpressionMethodMap.Add(typeof(string).GetMethod("Contains", new Type[] { typeof(string) }), @"substringof");
            ExpressionMethodMap.Add(typeof(string).GetMethod("EndsWith", new Type[] { typeof(string) }), @"endswith");
            ExpressionMethodMap.Add(typeof(string).GetMethod("StartsWith", new Type[] { typeof(string) }), @"startswith");
            ExpressionMethodMap.Add(typeof(string).GetMethod("IndexOf", new Type[] { typeof(string) }), @"indexof");
            ExpressionMethodMap.Add(typeof(string).GetMethod("Replace", new Type[] { typeof(string), typeof(string) }), @"replace");
            ExpressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int) }), @"substring");
            ExpressionMethodMap.Add(typeof(string).GetMethod("Substring", new Type[] { typeof(int), typeof(int) }), @"substring");
            ExpressionMethodMap.Add(typeof(string).GetMethod("ToLower", Type.EmptyTypes), @"tolower");
            ExpressionMethodMap.Add(typeof(string).GetMethod("ToUpper", Type.EmptyTypes), @"toupper");
            ExpressionMethodMap.Add(typeof(string).GetMethod("Trim", Type.EmptyTypes), @"trim");
            ExpressionMethodMap.Add(typeof(string).GetMethod("Concat", new Type[] { typeof(string), typeof(string) }, null), @"concat");   
            ExpressionMethodMap.Add(typeof(string).GetProperty("Length", typeof(int)).GetGetMethod(), @"length");

            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod(), @"day");
            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod(), @"hour");
            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod(), @"month");
            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod(), @"minute");
            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod(), @"second");
            ExpressionMethodMap.Add(typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod(), @"year");

            ExpressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(double) }), @"round");
            ExpressionMethodMap.Add(typeof(Math).GetMethod("Round", new Type[] { typeof(decimal) }), @"round");
            ExpressionMethodMap.Add(typeof(Math).GetMethod("Floor", new Type[] { typeof(double) }), @"floor");

            ExpressionMethodMap.Add(typeof(Math).GetMethod("Ceiling", new Type[] { typeof(double) }), @"ceiling");

            Debug.Assert(ExpressionMethodMap.Count == ExpectedCount, "expressionMethodMap.Count == ExpectedCount");

            ExpressionVBMethodMap = new Dictionary<string, string>(EqualityComparer<string>.Default);

            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Trim", @"trim");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Len", @"length");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.Mid", @"substring");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.UCase", @"toupper");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.Strings.LCase", @"tolower");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Year", @"year");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Month", @"month");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Day", @"day");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Hour", @"hour");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Minute", @"minute");
            ExpressionVBMethodMap.Add("Microsoft.VisualBasic.DateAndTime.Second", @"second");

            Debug.Assert(ExpressionVBMethodMap.Count == 11, "expressionVBMethodMap.Count == 11");

            PropertiesAsMethodsMap = new Dictionary<PropertyInfo, MethodInfo>(EqualityComparer<PropertyInfo>.Default);
            PropertiesAsMethodsMap.Add(
                typeof(string).GetProperty("Length", typeof(int)), 
                typeof(string).GetProperty("Length", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Day", typeof(int)), 
                typeof(DateTime).GetProperty("Day", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Hour", typeof(int)), 
                typeof(DateTime).GetProperty("Hour", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Minute", typeof(int)), 
                typeof(DateTime).GetProperty("Minute", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Second", typeof(int)), 
                typeof(DateTime).GetProperty("Second", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Month", typeof(int)),
                typeof(DateTime).GetProperty("Month", typeof(int)).GetGetMethod());
            PropertiesAsMethodsMap.Add(
                typeof(DateTime).GetProperty("Year", typeof(int)), 
                typeof(DateTime).GetProperty("Year", typeof(int)).GetGetMethod());

            Debug.Assert(PropertiesAsMethodsMap.Count == 7, "propertiesAsMethodsMap.Count == 7");
        }

        internal static bool TryGetQueryOptionMethod(MethodInfo mi, out string methodName)
        {
            return ExpressionMethodMap.TryGetValue(mi, out methodName) ||
                (mi.DeclaringType.Assembly.FullName == VisualBasicAssemblyFullName &&
                 ExpressionVBMethodMap.TryGetValue(mi.DeclaringType.FullName + "." + mi.Name, out methodName));
        }

        internal static bool TryGetPropertyAsMethod(PropertyInfo pi, out MethodInfo mi)
        {
            return PropertiesAsMethodsMap.TryGetValue(pi, out mi);
        }

        internal static Type GetElementType(Type seqType)
        {
            Type ienum = FindIEnumerable(seqType);
            if (ienum == null) 
            {
                return seqType;
            }

            return ienum.GetGenericArguments()[0];
        }

        internal static bool IsPrivate(PropertyInfo pi)
        {
            MethodInfo mi = pi.GetGetMethod() ?? pi.GetSetMethod();
            if (mi != null)
            {
                return mi.IsPrivate;
            }

            return true;
        }

        internal static Type FindIEnumerable(Type seqType)
        {
            if (seqType == null || seqType == typeof(string))
            {
                return null;
            }

            if (seqType.IsArray)
            {
                return typeof(IEnumerable<>).MakeGenericType(seqType.GetElementType());
            }

            if (seqType.IsGenericType)
            {
                foreach (Type arg in seqType.GetGenericArguments())
                {
                    Type ienum = typeof(IEnumerable<>).MakeGenericType(arg);
                    if (ienum.IsAssignableFrom(seqType))
                    {
                        return ienum;
                    }
                }
            }

            Type[] ifaces = seqType.GetInterfaces();
            if (ifaces.Length > 0)
            {
                foreach (Type iface in ifaces)
                {
                    Type ienum = FindIEnumerable(iface);
                    if (ienum != null)
                    {
                        return ienum;
                    }
                }
            }

            if (seqType.BaseType != null && seqType.BaseType != typeof(object))
            {
                return FindIEnumerable(seqType.BaseType);
            }

            return null;
        }
    }
}
