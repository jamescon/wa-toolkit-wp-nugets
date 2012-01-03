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

// This file is used by Code Analysis to maintain SuppressMessage 
// attributes that are applied to this project.
// Project-level suppressions either have no target or are given 
// a specific target and scoped to a namespace, type, member, etc.
//
// To add a suppression to this file, right-click the message in the 
// Error List, point to "Suppress Message(s)", and click 
// "In Project Suppression File".
// You do not need to add suppressions to this file manually.

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.CloudTableClient.#GetDataServiceContext()", Justification = "This class matches a similar interface to the CloudTableClient class (http://msdn.microsoft.com/library/ee758601) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1024:UsePropertiesWhereAppropriate", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.ICloudTableClient.#GetDataServiceContext()", Justification = "This class matches a similar interface to the CloudTableClient class (http://msdn.microsoft.com/library/ee758601) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lite", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.IStorageCredentials.#SignRequestLite(System.Net.WebRequest)", Justification = "This class matches a similar interface to the StorageCredentials class (http://msdn.microsoft.com/library/ee758688) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1704:IdentifiersShouldBeSpelledCorrectly", MessageId = "Lite", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.IStorageCredentials.#SignRequestLite(System.Net.WebHeaderCollection,System.Uri)", Justification = "This class matches a similar interface to the StorageCredentials class (http://msdn.microsoft.com/library/ee758688) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.StorageCredentialsAccountAndKey.#GetCanonicalizedResourceVersion2(System.Uri,System.String)", Justification = "This string requires to be lowercase for consistency.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Globalization", "CA1308:NormalizeStringsToUppercase", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.StorageCredentialsAccountAndKey.#AddCanonicalizedHeaders(System.Net.WebHeaderCollection,Microsoft.WindowsAzure.Samples.Phone.Storage.CanonicalizedString)", Justification = "This string requires to be lowercase for consistency.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.CloudQueue", Justification = "This class matches the CloudQueue class (http://msdn.microsoft.com/en-us/library/ee758641) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1711:IdentifiersShouldNotHaveIncorrectSuffix", Scope = "type", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.ICloudQueue", Justification = "This class matches the CloudQueue class (http://msdn.microsoft.com/en-us/library/ee758641) in the Microsoft.WindowsAzure.StorageClient library.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1819:PropertiesShouldNotReturnArrays", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Storage.CloudQueueMessage.#AsBytes", Justification = "This class is a DataContract and matches the CloudQueueMessage class (http://msdn.microsoft.com/library/microsoft.windowsazure.storageclient.cloudqueuemessage_members.aspx) in the Microsoft.WindowsAzure.StorageClient library.")]
