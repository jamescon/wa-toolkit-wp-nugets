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
    #region Namespaces.

    using System;
    using System.Collections;
    using System.Collections.Generic;
    using System.Diagnostics;

    #endregion Namespaces.

    internal static class AtomMaterializerInvoker
    {
        internal static IEnumerable<T> EnumerateAsElementType<T>(IEnumerable source)
        {
            return AtomMaterializer.EnumerateAsElementType<T>(source);
        }

        internal static List<TTarget> ListAsElementType<T, TTarget>(object materializer, IEnumerable<T> source) where T : TTarget
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            return AtomMaterializer.ListAsElementType<T, TTarget>((AtomMaterializer)materializer, source);
        }

        internal static bool ProjectionCheckValueForPathIsNull(
            object entry,
            Type expectedType,
            object path)
        {
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return AtomMaterializer.ProjectionCheckValueForPathIsNull((AtomEntry)entry, expectedType, (ProjectionPath)path);
        }

        internal static IEnumerable ProjectionSelect(
            object materializer,
            object entry,
            Type expectedType,
            Type resultType,
            object path,
            Func<object, object, Type, object> selector)
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return AtomMaterializer.ProjectionSelect((AtomMaterializer)materializer, (AtomEntry)entry, expectedType, resultType, (ProjectionPath)path, selector);
        }

        internal static AtomEntry ProjectionGetEntry(object entry, string name)
        {
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            return AtomMaterializer.ProjectionGetEntry((AtomEntry)entry, name);
        }

        internal static object ProjectionInitializeEntity(
            object materializer,
            object entry,
            Type expectedType,
            Type resultType,
            string[] properties,
            Func<object, object, Type, object>[] propertyValues)
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            return AtomMaterializer.ProjectionInitializeEntity((AtomMaterializer)materializer, (AtomEntry)entry, expectedType, resultType, properties, propertyValues);
        }

        internal static object ProjectionValueForPath(object materializer, object entry, Type expectedType, object path)
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            Debug.Assert(path.GetType() == typeof(ProjectionPath), "path.GetType() == typeof(ProjectionPath)");
            return AtomMaterializer.ProjectionValueForPath((AtomMaterializer)materializer, (AtomEntry)entry, expectedType, (ProjectionPath)path);
        }

        internal static object DirectMaterializePlan(object materializer, object entry, Type expectedEntryType)
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            return AtomMaterializer.DirectMaterializePlan((AtomMaterializer)materializer, (AtomEntry)entry, expectedEntryType);
        }

        internal static object ShallowMaterializePlan(object materializer, object entry, Type expectedEntryType)
        {
            Debug.Assert(materializer.GetType() == typeof(AtomMaterializer), "materializer.GetType() == typeof(AtomMaterializer)");
            Debug.Assert(entry.GetType() == typeof(AtomEntry), "entry.GetType() == typeof(AtomEntry)");
            return AtomMaterializer.ShallowMaterializePlan((AtomMaterializer)materializer, (AtomEntry)entry, expectedEntryType);
        }
    }
}
