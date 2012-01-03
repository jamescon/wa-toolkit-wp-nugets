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
#region Namespaces
    using System;
    using System.Collections.Generic;
    using System.ComponentModel;
    using System.Diagnostics;
    using System.Linq;
    using System.Threading;
    using Microsoft.WindowsAzure.Samples.Data.Services.Client;
#endregion

    internal enum BindingPropertyKind
    {
        BindingPropertyKindComplex,

        BindingPropertyKindEntity,

        BindingPropertyKindCollection
    }

    internal class BindingEntityInfo
    {
        private static readonly object FalseObject = new object();

        private static readonly object TrueObject = new object();

        private static readonly ReaderWriterLockSlim MetadataCacheLock = new ReaderWriterLockSlim();

        private static readonly HashSet<Type> KnownNonEntityTypes = new HashSet<Type>(EqualityComparer<Type>.Default);

        private static readonly Dictionary<Type, object> KnownObservableCollectionTypes = new Dictionary<Type, object>(EqualityComparer<Type>.Default);

        private static readonly Dictionary<Type, BindingEntityInfoPerType> BindingEntityInfos = new Dictionary<Type, BindingEntityInfoPerType>(EqualityComparer<Type>.Default);

        internal static IList<BindingPropertyInfo> GetObservableProperties(Type entityType)
        {
            return GetBindingEntityInfoFor(entityType).ObservableProperties;
        }

        internal static ClientType GetClientType(Type entityType)
        {
            return GetBindingEntityInfoFor(entityType).ClientType;
        }

        internal static string GetEntitySet(
            object target,
            string targetEntitySet)
        {
            Debug.Assert(target != null, "Argument 'target' cannot be null.");
            Debug.Assert(BindingEntityInfo.IsEntityType(target.GetType()), "Argument 'target' must be an entity type.");

            if (!string.IsNullOrEmpty(targetEntitySet))
            {
                return targetEntitySet;
            }
            else
            {
                return BindingEntityInfo.GetEntitySetAttribute(target.GetType());
            }
        }

        internal static bool IsDataServiceCollection(Type collectionType)
        {
            Debug.Assert(collectionType != null, "Argument 'collectionType' cannot be null.");

            MetadataCacheLock.EnterReadLock();
            try
            {
                object resultAsObject;
                if (KnownObservableCollectionTypes.TryGetValue(collectionType, out resultAsObject))
                {
                    return resultAsObject == TrueObject;
                }
            }
            finally
            {
                MetadataCacheLock.ExitReadLock();
            }

            Type type = collectionType;
            bool result = false;

            while (type != null)
            {
                if (type.IsGenericType)
                {
                    Type[] parms = type.GetGenericArguments();

                    if (parms.Length == 1 && IsEntityType(parms[0]))
                    {
                        Type dataServiceCollection = WebUtil.GetDataServiceCollectionOfT(parms);
                        if (dataServiceCollection != null && dataServiceCollection.IsAssignableFrom(type))
                        {
                            result = true;
                            break;
                        }
                    }
                }

                type = type.BaseType;
            }

            MetadataCacheLock.EnterWriteLock();
            try
            {
                if (!KnownObservableCollectionTypes.ContainsKey(collectionType))
                {
                    KnownObservableCollectionTypes[collectionType] = result ? TrueObject : FalseObject;
                }
            }
            finally
            {
                MetadataCacheLock.ExitWriteLock();
            }

            return result;
        }

        internal static bool IsEntityType(Type type)
        {
            Debug.Assert(type != null, "Argument 'type' cannot be null.");

            MetadataCacheLock.EnterReadLock();
            try
            {
                if (KnownNonEntityTypes.Contains(type))
                {
                    return false;
                }
            }
            finally
            {
                MetadataCacheLock.ExitReadLock();
            }

            try
            {
                if (BindingEntityInfo.IsDataServiceCollection(type))
                {
                    return false;
                }

                return ClientType.Create(type).IsEntityType;
            }
            catch (InvalidOperationException)
            {
                MetadataCacheLock.EnterWriteLock();
                try
                {
                    if (!KnownNonEntityTypes.Contains(type))
                    {
                        KnownNonEntityTypes.Add(type);
                    }
                }
                finally
                {
                    MetadataCacheLock.ExitWriteLock();
                }

                return false;
            }
        }

        internal static object GetPropertyValue(object source, string sourceProperty, out BindingPropertyInfo bindingPropertyInfo)
        {
            Type sourceType = source.GetType();

            bindingPropertyInfo = BindingEntityInfo.GetObservableProperties(sourceType)
                                                   .SingleOrDefault(x => x.PropertyInfo.PropertyName == sourceProperty);

            if (bindingPropertyInfo == null)
            {
                return BindingEntityInfo.GetClientType(sourceType)
                                        .GetProperty(sourceProperty, false)
                                        .GetValue(source);
            }
            else
            {
                return bindingPropertyInfo.PropertyInfo.GetValue(source);
            }
        }

        private static BindingEntityInfoPerType GetBindingEntityInfoFor(Type entityType)
        {
            BindingEntityInfoPerType bindingEntityInfo;

            MetadataCacheLock.EnterReadLock();
            try
            {
                if (BindingEntityInfos.TryGetValue(entityType, out bindingEntityInfo))
                {
                    return bindingEntityInfo;
                }
            }
            finally
            {
                MetadataCacheLock.ExitReadLock();
            }

            bindingEntityInfo = new BindingEntityInfoPerType();

            object[] attributes = entityType.GetCustomAttributes(typeof(EntitySetAttribute), true);

            bindingEntityInfo.EntitySet = (attributes.Length == 1) ? ((EntitySetAttribute)attributes[0]).EntitySet : null;
            bindingEntityInfo.ClientType = ClientType.Create(entityType);
            
            foreach (ClientType.ClientProperty p in bindingEntityInfo.ClientType.Properties)
            {
                BindingPropertyInfo bpi = null;
            
                Type propertyType = p.PropertyType;
                
                if (p.CollectionType != null)
                {
                    if (BindingEntityInfo.IsDataServiceCollection(propertyType))
                    {
                        bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.BindingPropertyKindCollection };
                    }
                }
                else
                if (BindingEntityInfo.IsEntityType(propertyType))
                {
                    bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.BindingPropertyKindEntity };
                }
                else
                if (BindingEntityInfo.CanBeComplexProperty(p))
                {
                    bpi = new BindingPropertyInfo { PropertyKind = BindingPropertyKind.BindingPropertyKindComplex };
                }
                
                if (bpi != null)
                {
                    bpi.PropertyInfo = p;
                    
                    if (bindingEntityInfo.ClientType.IsEntityType || bpi.PropertyKind == BindingPropertyKind.BindingPropertyKindComplex)
                    {
                        bindingEntityInfo.ObservableProperties.Add(bpi);
                    }
                }
            }

            MetadataCacheLock.EnterWriteLock();
            try
            {
                if (!BindingEntityInfos.ContainsKey(entityType))
                {
                    BindingEntityInfos[entityType] = bindingEntityInfo;
                }
            }
            finally
            {
                MetadataCacheLock.ExitWriteLock();
            }

            return bindingEntityInfo;
        }

        private static bool CanBeComplexProperty(ClientType.ClientProperty property)
        {
            Debug.Assert(property != null, "property != null");
            if (typeof(INotifyPropertyChanged).IsAssignableFrom(property.PropertyType))
            {
                Debug.Assert(!property.IsKnownType, "Known types do not implement INotifyPropertyChanged.");
                return true;
            }

            return false;
        }

        private static string GetEntitySetAttribute(Type entityType)
        {
            return GetBindingEntityInfoFor(entityType).EntitySet;
        }

        internal class BindingPropertyInfo
        {
            public ClientType.ClientProperty PropertyInfo
            {
                get;
                set;
            }

            public BindingPropertyKind PropertyKind
            {
                get;
                set;
            }
        }

        private sealed class BindingEntityInfoPerType
        {
            private readonly List<BindingPropertyInfo> observableProperties;

            public BindingEntityInfoPerType()
            {
                this.observableProperties = new List<BindingPropertyInfo>();
            }

            public string EntitySet
            {
                get;
                set;
            }

            public ClientType ClientType
            {
                get;
                set;
            }

            public List<BindingPropertyInfo> ObservableProperties
            {
                get
                {
                    return this.observableProperties;
                }
            }
        }

        private sealed class ReaderWriterLockSlim
        {
            private readonly object lockObject = new object();

            internal void EnterReadLock()
            {
                Monitor.Enter(this.lockObject);
            }

            internal void EnterWriteLock()
            {
                Monitor.Enter(this.lockObject);
            }

            internal void ExitReadLock()
            {
                Monitor.Exit(this.lockObject);
            }

            internal void ExitWriteLock()
            {
                Monitor.Exit(this.lockObject);
            }
        }
    }
}
