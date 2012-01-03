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

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoint.#Microsoft.WindowsAzure.Samples.Common.Storage.ITableServiceEntity.RowKey", Justification = "Hiding on ITableService methods is on purpose and used exclusively by the Storage classes shipped as part of this assembly")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1033:InterfaceMethodsShouldBeCallableByChildTypes", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoint.#Microsoft.WindowsAzure.Samples.Common.Storage.ITableServiceEntity.PartitionKey", Justification = "Hiding on ITableService methods is on purpose and used exclusively by the Storage classes shipped as part of this assembly")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoints.#Put(Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoint)", Justification = "If disposed the HttpMessage is no longer valid for the rest of the handler chains")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoints.#Delete(Microsoft.WindowsAzure.Samples.CloudServices.Notifications.Endpoint)", Justification = "If disposed the HttpMessage is no longer valid for the rest of the handler chains")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.EndpointService.#Repository", Justification = "In fact it's an inmmutable declaration exposed to children for testability")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.WindowsAzure.WindowsAzureEndpointsRepository.#Table", Justification = "In fact it's an inmmutable declaration exposed to children for testability")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Security", "CA2104:DoNotDeclareReadOnlyMutableReferenceTypes", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.WindowsAzure.WindowsAzureEndpointRepository.#Table", Justification = "It it's inmmutable by nature. Do not need to hide or create another property")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.EndpointService.#Delete(System.String,System.String)", Justification = "Breaks the application pipeline")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.EndpointService.#WebException(System.String,System.Net.HttpStatusCode)", Justification = "Breaks the application pipeline")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Reliability", "CA2000:Dispose objects before losing scope", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.CloudServices.Notifications.FuncBasedOperationHandler.#OnHandle(System.Net.Http.HttpRequestMessage)", Justification = "Breaks the application pipeline")]
