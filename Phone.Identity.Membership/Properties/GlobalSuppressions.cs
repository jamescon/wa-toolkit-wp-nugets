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

[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1020:AvoidNamespacesWithFewTypes", Scope = "namespace", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.Converters", Justification = "This namespace only contains value converters that are intentionally separate from the main namespace.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#ViewModel", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#OnLogOnTextBoxKeyUp(System.Object,System.Windows.Input.KeyEventArgs)", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#OnRegisterTextBoxKeyUp(System.Object,System.Windows.Input.KeyEventArgs)", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#OnChangeCurrentModeButtonClick(System.Object,System.Windows.RoutedEventArgs)", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#OnSubmitMembershipOperationButtonClick(System.Object,System.Windows.RoutedEventArgs)", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#OnLoaded(System.Object,System.Windows.RoutedEventArgs)", Justification = "This method is binded to an event in the XAML")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#SubmitMembershipOperation(System.Action)", Justification = "This method is called from another private method.")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipTokenStore.#MembershipToken", Justification = "This property is intended to be binded")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1822:MarkMembersAsStatic", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipTokenStore.#RememberMe", Justification = "This property is intended to be binded")]
[assembly: System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1823:AvoidUnusedPrivateFields", Scope = "member", Target = "Microsoft.WindowsAzure.Samples.Phone.Identity.Membership.MembershipSignIn.#MembershipSignInControl", Justification = "This field is needed to be able to bind it to another property using the ElementName.")]
