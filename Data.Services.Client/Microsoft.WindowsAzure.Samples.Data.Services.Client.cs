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
    using System.ComponentModel;
    using System.Globalization;
    using System.Resources;
    using System.Threading;

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TextResDescriptionAttribute : DescriptionAttribute
    {
        private bool replaced = false;

        public TextResDescriptionAttribute(string description)
            : base(description)
        {
        }

        public override string Description
        {
            get
            {
                if (!this.replaced)
                {
                    this.replaced = true;
                    DescriptionValue = TextRes.GetString(base.Description);
                }

                return base.Description;
            }
        }
    }

    [AttributeUsage(AttributeTargets.All)]
    internal sealed class TextResCategoryAttribute : CategoryAttribute
    {
        public TextResCategoryAttribute(string category)
            : base(category)
        {
        }

        protected override string GetLocalizedString(string value)
        {
            return TextRes.GetString(value);
        }
    }

    internal sealed class TextRes
    {
        internal const string BatchStreamMissingBoundary = "BatchStream_MissingBoundary";
        internal const string BatchStreamContentExpected = "BatchStream_ContentExpected";
        internal const string BatchStreamContentUnexpected = "BatchStream_ContentUnexpected";
        internal const string BatchStreamGetMethodNotSupportedInChangeset = "BatchStream_GetMethodNotSupportedInChangeset";
        internal const string BatchStreamInvalidBatchFormat = "BatchStream_InvalidBatchFormat";
        internal const string BatchStreamInvalidDelimiter = "BatchStream_InvalidDelimiter";
        internal const string BatchStreamMissingEndChangesetDelimiter = "BatchStream_MissingEndChangesetDelimiter";
        internal const string BatchStreamInvalidHeaderValueSpecified = "BatchStream_InvalidHeaderValueSpecified";
        internal const string BatchStreamInvalidContentLengthSpecified = "BatchStream_InvalidContentLengthSpecified";
        internal const string BatchStreamOnlyGETOperationsCanBeSpecifiedInBatch = "BatchStream_OnlyGETOperationsCanBeSpecifiedInBatch";
        internal const string BatchStreamInvalidOperationHeaderSpecified = "BatchStream_InvalidOperationHeaderSpecified";
        internal const string BatchStreamInvalidHttpMethodName = "BatchStream_InvalidHttpMethodName";
        internal const string BatchStreamMoreDataAfterEndOfBatch = "BatchStream_MoreDataAfterEndOfBatch";
        internal const string BatchStreamInternalBufferRequestTooSmall = "BatchStream_InternalBufferRequestTooSmall";
        internal const string BatchStreamInvalidMethodHeaderSpecified = "BatchStream_InvalidMethodHeaderSpecified";
        internal const string BatchStreamInvalidHttpVersionSpecified = "BatchStream_InvalidHttpVersionSpecified";
        internal const string BatchStreamInvalidNumberOfHeadersAtOperationStart = "BatchStream_InvalidNumberOfHeadersAtOperationStart";
        internal const string BatchStreamMissingOrInvalidContentEncodingHeader = "BatchStream_MissingOrInvalidContentEncodingHeader";
        internal const string BatchStreamInvalidNumberOfHeadersAtChangeSetStart = "BatchStream_InvalidNumberOfHeadersAtChangeSetStart";
        internal const string BatchStreamMissingContentTypeHeader = "BatchStream_MissingContentTypeHeader";
        internal const string BatchStreamInvalidContentTypeSpecified = "BatchStream_InvalidContentTypeSpecified";
        internal const string BatchExpectedContentType = "Batch_ExpectedContentType";
        internal const string BatchExpectedResponse = "Batch_ExpectedResponse";
        internal const string BatchIncompleteResponseCount = "Batch_IncompleteResponseCount";
        internal const string BatchUnexpectedContent = "Batch_UnexpectedContent";
        internal const string ContextBaseUri = "Context_BaseUri";
        internal const string ContextCannotConvertKey = "Context_CannotConvertKey";
        internal const string ContextTrackingExpectsAbsoluteUri = "Context_TrackingExpectsAbsoluteUri";
        internal const string ContextLinkResourceInsertFailure = "Context_LinkResourceInsertFailure";
        internal const string ContextInternalError = "Context_InternalError";
        internal const string ContextBatchExecuteError = "Context_BatchExecuteError";
        internal const string ContextEntitySetName = "Context_EntitySetName";
        internal const string ContextMissingEditLinkInResponseBody = "Context_MissingEditLinkInResponseBody";
        internal const string ContextMissingSelfLinkInResponseBody = "Context_MissingSelfLinkInResponseBody";
        internal const string ContextMissingEditMediaLinkInResponseBody = "Context_MissingEditMediaLinkInResponseBody";
        internal const string ContentEntityWithoutKey = "Content_EntityWithoutKey";
        internal const string ContentEntityIsNotEntityType = "Content_EntityIsNotEntityType";
        internal const string ContextEntityNotContained = "Context_EntityNotContained";
        internal const string ContextEntityAlreadyContained = "Context_EntityAlreadyContained";
        internal const string ContextDifferentEntityAlreadyContained = "Context_DifferentEntityAlreadyContained";
        internal const string ContextDidNotOriginateAsync = "Context_DidNotOriginateAsync";
        internal const string ContextAsyncAlreadyDone = "Context_AsyncAlreadyDone";
        internal const string ContextOperationCanceled = "Context_OperationCanceled";
        internal const string ContextNoLoadWithInsertEnd = "Context_NoLoadWithInsertEnd";
        internal const string ContextNoRelationWithInsertEnd = "Context_NoRelationWithInsertEnd";
        internal const string ContextNoRelationWithDeleteEnd = "Context_NoRelationWithDeleteEnd";
        internal const string ContextRelationAlreadyContained = "Context_RelationAlreadyContained";
        internal const string ContextRelationNotRefOrCollection = "Context_RelationNotRefOrCollection";
        internal const string ContextAddLinkCollectionOnly = "Context_AddLinkCollectionOnly";
        internal const string ContextAddRelatedObjectCollectionOnly = "Context_AddRelatedObjectCollectionOnly";
        internal const string ContextAddRelatedObjectSourceDeleted = "Context_AddRelatedObjectSourceDeleted";
        internal const string ContextSetLinkReferenceOnly = "Context_SetLinkReferenceOnly";
        internal const string ContextNoContentTypeForMediaLink = "Context_NoContentTypeForMediaLink";
        internal const string ContextBatchNotSupportedForMediaLink = "Context_BatchNotSupportedForMediaLink";
        internal const string ContextUnexpectedZeroRawRead = "Context_UnexpectedZeroRawRead";
        internal const string ContextVersionNotSupported = "Context_VersionNotSupported";
        internal const string ContextChildResourceExists = "Context_ChildResourceExists";
        internal const string ContextEntityNotMediaLinkEntry = "Context_EntityNotMediaLinkEntry";
        internal const string ContextMLEWithoutSaveStream = "Context_MLEWithoutSaveStream";
        internal const string ContextSetSaveStreamOnMediaEntryProperty = "Context_SetSaveStreamOnMediaEntryProperty";
        internal const string ContextSetSaveStreamWithoutEditMediaLink = "Context_SetSaveStreamWithoutEditMediaLink";
        internal const string CollectionNullCollectionReference = "Collection_NullCollectionReference";
        internal const string ClientTypeMissingOpenProperty = "ClientType_MissingOpenProperty";
        internal const string ClienttypeMultipleOpenProperty = "Clienttype_MultipleOpenProperty";
        internal const string ClientTypeMissingProperty = "ClientType_MissingProperty";
        internal const string ClientTypeKeysMustBeSimpleTypes = "ClientType_KeysMustBeSimpleTypes";
        internal const string ClientTypeKeysOnDifferentDeclaredType = "ClientType_KeysOnDifferentDeclaredType";
        internal const string ClientTypeMissingMimeTypeProperty = "ClientType_MissingMimeTypeProperty";
        internal const string ClientTypeMissingMediaEntryProperty = "ClientType_MissingMediaEntryProperty";
        internal const string ClientTypeNoSettableFields = "ClientType_NoSettableFields";
        internal const string ClientTypeMultipleImplementationNotSupported = "ClientType_MultipleImplementationNotSupported";
        internal const string ClientTypeNullOpenProperties = "ClientType_NullOpenProperties";
        internal const string ClientTypeCollectionOfNonEntities = "ClientType_CollectionOfNonEntities";
        internal const string ClientTypeAmbiguous = "ClientType_Ambiguous";
        internal const string ClientTypeUnsupportedType = "ClientType_UnsupportedType";
        internal const string DataServiceExceptionGeneralError = "DataServiceException_GeneralError";
        internal const string DeserializeGetEnumerator = "Deserialize_GetEnumerator";
        internal const string DeserializeCurrent = "Deserialize_Current";
        internal const string DeserializeMixedTextWithComment = "Deserialize_MixedTextWithComment";
        internal const string DeserializeExpectingSimpleValue = "Deserialize_ExpectingSimpleValue";
        internal const string DeserializeNotApplicationXml = "Deserialize_NotApplicationXml";
        internal const string DeserializeMismatchAtomLinkLocalSimple = "Deserialize_MismatchAtomLinkLocalSimple";
        internal const string DeserializeMismatchAtomLinkFeedPropertyNotCollection = "Deserialize_MismatchAtomLinkFeedPropertyNotCollection";
        internal const string DeserializeMismatchAtomLinkEntryPropertyIsCollection = "Deserialize_MismatchAtomLinkEntryPropertyIsCollection";
        internal const string DeserializeUnknownMimeTypeSpecified = "Deserialize_UnknownMimeTypeSpecified";
        internal const string DeserializeExpectedEmptyMediaLinkEntryContent = "Deserialize_ExpectedEmptyMediaLinkEntryContent";
        internal const string DeserializeContentPlusPropertiesNotAllowed = "Deserialize_ContentPlusPropertiesNotAllowed";
        internal const string DeserializeNoLocationHeader = "Deserialize_NoLocationHeader";
        internal const string DeserializeServerException = "Deserialize_ServerException";
        internal const string DeserializeMissingIdElement = "Deserialize_MissingIdElement";
        internal const string EpmClientTypePropertyIsComplex = "EpmClientType_PropertyIsComplex";
        internal const string EpmClientTypePropertyIsPrimitive = "EpmClientType_PropertyIsPrimitive";
        internal const string EpmSourceTreeInvalidSourcePath = "EpmSourceTree_InvalidSourcePath";
        internal const string EpmSourceTreeDuplicateEpmAttrsWithSameSourceName = "EpmSourceTree_DuplicateEpmAttrsWithSameSourceName";
        internal const string EpmSourceTreeInaccessiblePropertyOnType = "EpmSourceTree_InaccessiblePropertyOnType";
        internal const string EpmTargetTreeInvalidTargetPath = "EpmTargetTree_InvalidTargetPath";
        internal const string EpmTargetTreeAttributeInMiddle = "EpmTargetTree_AttributeInMiddle";
        internal const string EpmTargetTreeDuplicateEpmAttrsWithSameTargetName = "EpmTargetTree_DuplicateEpmAttrsWithSameTargetName";
        internal const string EntityPropertyMappingEpmAttribute = "EntityPropertyMapping_EpmAttribute";
        internal const string EntityPropertyMappingTargetNamespaceUriNotValid = "EntityPropertyMapping_TargetNamespaceUriNotValid";
        internal const string HttpProcessUtilityContentTypeMissing = "HttpProcessUtility_ContentTypeMissing";
        internal const string HttpProcessUtilityMediaTypeMissingValue = "HttpProcessUtility_MediaTypeMissingValue";
        internal const string HttpProcessUtilityMediaTypeRequiresSemicolonBeforeParameter = "HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter";
        internal const string HttpProcessUtilityMediaTypeRequiresSlash = "HttpProcessUtility_MediaTypeRequiresSlash";
        internal const string HttpProcessUtilityMediaTypeRequiresSubType = "HttpProcessUtility_MediaTypeRequiresSubType";
        internal const string HttpProcessUtilityMediaTypeUnspecified = "HttpProcessUtility_MediaTypeUnspecified";
        internal const string HttpProcessUtilityEncodingNotSupported = "HttpProcessUtility_EncodingNotSupported";
        internal const string HttpProcessUtilityEscapeCharWithoutQuotes = "HttpProcessUtility_EscapeCharWithoutQuotes";
        internal const string HttpProcessUtilityEscapeCharAtEnd = "HttpProcessUtility_EscapeCharAtEnd";
        internal const string HttpProcessUtilityClosingQuoteNotFound = "HttpProcessUtility_ClosingQuoteNotFound";
        internal const string MaterializeFromAtomCountNotPresent = "MaterializeFromAtom_CountNotPresent";
        internal const string MaterializeFromAtomCountFormatError = "MaterializeFromAtom_CountFormatError";
        internal const string MaterializeFromAtomTopLevelLinkNotAvailable = "MaterializeFromAtom_TopLevelLinkNotAvailable";
        internal const string MaterializeFromAtomCollectionKeyNotPresentInLinkTable = "MaterializeFromAtom_CollectionKeyNotPresentInLinkTable";
        internal const string MaterializeFromAtomGetNestLinkForFlatCollection = "MaterializeFromAtom_GetNestLinkForFlatCollection";
        internal const string SerializerNullKeysAreNotSupported = "Serializer_NullKeysAreNotSupported";
        internal const string UtilEmptyString = "Util_EmptyString";
        internal const string UtilEmptyArray = "Util_EmptyArray";
        internal const string UtilNullArrayElement = "Util_NullArrayElement";
        internal const string ALinqUnsupportedExpression = "ALinq_UnsupportedExpression";
        internal const string ALinqCouldNotConvert = "ALinq_CouldNotConvert";
        internal const string ALinqMethodNotSupported = "ALinq_MethodNotSupported";
        internal const string ALinqUnaryNotSupported = "ALinq_UnaryNotSupported";
        internal const string ALinqBinaryNotSupported = "ALinq_BinaryNotSupported";
        internal const string ALinqConstantNotSupported = "ALinq_ConstantNotSupported";
        internal const string ALinqTypeBinaryNotSupported = "ALinq_TypeBinaryNotSupported";
        internal const string ALinqConditionalNotSupported = "ALinq_ConditionalNotSupported";
        internal const string ALinqParameterNotSupported = "ALinq_ParameterNotSupported";
        internal const string ALinqMemberAccessNotSupported = "ALinq_MemberAccessNotSupported";
        internal const string ALinqLambdaNotSupported = "ALinq_LambdaNotSupported";
        internal const string ALinqNewNotSupported = "ALinq_NewNotSupported";
        internal const string ALinqMemberInitNotSupported = "ALinq_MemberInitNotSupported";
        internal const string ALinqListInitNotSupported = "ALinq_ListInitNotSupported";
        internal const string ALinqNewArrayNotSupported = "ALinq_NewArrayNotSupported";
        internal const string ALinqInvocationNotSupported = "ALinq_InvocationNotSupported";
        internal const string ALinqQueryOptionsOnlyAllowedOnLeafNodes = "ALinq_QueryOptionsOnlyAllowedOnLeafNodes";
        internal const string ALinqCantExpand = "ALinq_CantExpand";
        internal const string ALinqCantCastToUnsupportedPrimitive = "ALinq_CantCastToUnsupportedPrimitive";
        internal const string ALinqCantNavigateWithoutKeyPredicate = "ALinq_CantNavigateWithoutKeyPredicate";
        internal const string ALinqCanOnlyApplyOneKeyPredicate = "ALinq_CanOnlyApplyOneKeyPredicate";
        internal const string ALinqCantTranslateExpression = "ALinq_CantTranslateExpression";
        internal const string ALinqTranslationError = "ALinq_TranslationError";
        internal const string ALinqCantAddQueryOption = "ALinq_CantAddQueryOption";
        internal const string ALinqCantAddDuplicateQueryOption = "ALinq_CantAddDuplicateQueryOption";
        internal const string ALinqCantAddAstoriaQueryOption = "ALinq_CantAddAstoriaQueryOption";
        internal const string ALinqCantAddQueryOptionStartingWithDollarSign = "ALinq_CantAddQueryOptionStartingWithDollarSign";
        internal const string ALinqCantReferToPublicField = "ALinq_CantReferToPublicField";
        internal const string ALinqQueryOptionsOnlyAllowedOnSingletons = "ALinq_QueryOptionsOnlyAllowedOnSingletons";
        internal const string ALinqQueryOptionOutOfOrder = "ALinq_QueryOptionOutOfOrder";
        internal const string ALinqCannotAddCountOption = "ALinq_CannotAddCountOption";
        internal const string ALinqCannotAddCountOptionConflict = "ALinq_CannotAddCountOptionConflict";
        internal const string ALinqProjectionOnlyAllowedOnLeafNodes = "ALinq_ProjectionOnlyAllowedOnLeafNodes";
        internal const string ALinqProjectionCanOnlyHaveOneProjection = "ALinq_ProjectionCanOnlyHaveOneProjection";
        internal const string ALinqProjectionMemberAssignmentMismatch = "ALinq_ProjectionMemberAssignmentMismatch";
        internal const string ALinqExpressionNotSupportedInProjectionToEntity = "ALinq_ExpressionNotSupportedInProjectionToEntity";
        internal const string ALinqExpressionNotSupportedInProjection = "ALinq_ExpressionNotSupportedInProjection";
        internal const string ALinqCannotConstructKnownEntityTypes = "ALinq_CannotConstructKnownEntityTypes";
        internal const string ALinqCannotCreateConstantEntity = "ALinq_CannotCreateConstantEntity";
        internal const string ALinqPropertyNamesMustMatchInProjections = "ALinq_PropertyNamesMustMatchInProjections";
        internal const string ALinqCanOnlyProjectTheLeaf = "ALinq_CanOnlyProjectTheLeaf";
        internal const string ALinqCannotProjectWithExplicitExpansion = "ALinq_CannotProjectWithExplicitExpansion";
        internal const string DSKAttributeMustSpecifyAtleastOnePropertyName = "DSKAttribute_MustSpecifyAtleastOnePropertyName";
        internal const string HttpWebInternal = "HttpWeb_Internal";
        internal const string HttpWebInternalArgument = "HttpWeb_InternalArgument";
        internal const string HttpWebRequestAborted = "HttpWebRequest_Aborted";
        internal const string DataServiceCollectionLoadRequiresTargetCollectionObserved = "DataServiceCollection_LoadRequiresTargetCollectionObserved";
        internal const string DataServiceCollectionCannotStopTrackingChildCollection = "DataServiceCollection_CannotStopTrackingChildCollection";
        internal const string DataServiceCollectionDataServiceQueryCanNotBeEnumerated = "DataServiceCollection_DataServiceQueryCanNotBeEnumerated";
        internal const string DataServiceCollectionOperationForTrackedOnly = "DataServiceCollection_OperationForTrackedOnly";
        internal const string DataServiceCollectionCannotDetermineContextFromItems = "DataServiceCollection_CannotDetermineContextFromItems";
        internal const string DataServiceCollectionInsertIntoTrackedButNotLoadedCollection = "DataServiceCollection_InsertIntoTrackedButNotLoadedCollection";
        internal const string DataServiceCollectionMultipleLoadAsyncOperationsAtTheSameTime = "DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime";
        internal const string DataServiceCollectionLoadAsyncNoParamsWithoutParentEntity = "DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity";
        internal const string DataServiceCollectionLoadAsyncRequiresDataServiceQuery = "DataServiceCollection_LoadAsyncRequiresDataServiceQuery";
        internal const string DataBindingDataServiceCollectionArgumentMustHaveEntityType = "DataBinding_DataServiceCollectionArgumentMustHaveEntityType";
        internal const string DataBindingCollectionPropertySetterValueHasObserver = "DataBinding_CollectionPropertySetterValueHasObserver";
        internal const string DataBindingCollectionChangedUnknownAction = "DataBinding_CollectionChangedUnknownAction";
        internal const string DataBindingBindingOperationDetachedSource = "DataBinding_BindingOperation_DetachedSource";
        internal const string DataBindingBindingOperationArrayItemNull = "DataBinding_BindingOperation_ArrayItemNull";
        internal const string DataBindingBindingOperationArrayItemNotEntity = "DataBinding_BindingOperation_ArrayItemNotEntity";
        internal const string DataBindingUtilUnknownEntitySetName = "DataBinding_Util_UnknownEntitySetName";
        internal const string DataBindingEntityAlreadyInCollection = "DataBinding_EntityAlreadyInCollection";
        internal const string DataBindingNotifyPropertyChangedNotImpl = "DataBinding_NotifyPropertyChangedNotImpl";
        internal const string DataBindingComplexObjectAssociatedWithMultipleEntities = "DataBinding_ComplexObjectAssociatedWithMultipleEntities";
        internal const string AtomParserFeedUnexpected = "AtomParser_FeedUnexpected";
        internal const string AtomParserPagingLinkOutsideOfFeed = "AtomParser_PagingLinkOutsideOfFeed";
        internal const string AtomParserManyFeedCounts = "AtomParser_ManyFeedCounts";
        internal const string AtomParserFeedCountNotUnderFeed = "AtomParser_FeedCountNotUnderFeed";
        internal const string AtomParserUnexpectedContentUnderExpandedLink = "AtomParser_UnexpectedContentUnderExpandedLink";
        internal const string AtomMaterializerCannotAssignNull = "AtomMaterializer_CannotAssignNull";
        internal const string AtomMaterializerDuplicatedNextLink = "AtomMaterializer_DuplicatedNextLink";
        internal const string AtomMaterializerEntryIntoCollectionMismatch = "AtomMaterializer_EntryIntoCollectionMismatch";
        internal const string AtomMaterializerEntryToAccessIsNull = "AtomMaterializer_EntryToAccessIsNull";
        internal const string AtomMaterializerEntryToInitializeIsNull = "AtomMaterializer_EntryToInitializeIsNull";
        internal const string AtomMaterializerProjectEntityTypeMismatch = "AtomMaterializer_ProjectEntityTypeMismatch";
        internal const string AtomMaterializerLinksMissingHref = "AtomMaterializer_LinksMissingHref";
        internal const string AtomMaterializerPropertyMissing = "AtomMaterializer_PropertyMissing";
        internal const string AtomMaterializerPropertyMissingFromEntry = "AtomMaterializer_PropertyMissingFromEntry";
        internal const string AtomMaterializerPropertyNotExpectedEntry = "AtomMaterializer_PropertyNotExpectedEntry";
        internal const string DataServiceQueryEnumerationNotSupportedInSL = "DataServiceQuery_EnumerationNotSupportedInSL";
        internal const string DataServiceStateCollectionNotInContext = "DataServiceState_CollectionNotInContext";

        private static TextRes loader = null;
        private ResourceManager resources;

        internal TextRes()
        {
            this.resources = new System.Resources.ResourceManager("Microsoft.WindowsAzure.Samples.Data.Services.Client", this.GetType().Assembly);
        }

        public static ResourceManager Resources
        {
            get
            {
                return GetLoader().resources;
            }
        }

        private static CultureInfo Culture
        {
            get { return null; }
        }

        public static string GetString(string name, params object[] args)
        {
            TextRes sys = GetLoader();
            if (sys == null)
                return null;
            string res = sys.resources.GetString(name, TextRes.Culture);

            if (args != null && args.Length > 0)
            {
                for (int i = 0; i < args.Length; i++)
                {
                    string value = args[i] as string;
                    if (value != null && value.Length > 1024)
                    {
                        args[i] = value.Substring(0, 1024 - 3) + "...";
                    }
                }

                return string.Format(CultureInfo.CurrentCulture, res, args);
            }
            else
            {
                return res;
            }
        }

        public static string GetString(string name)
        {
            TextRes sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetString(name, TextRes.Culture);
        }

        public static string GetString(string name, out bool usedFallback)
        {
            usedFallback = false;
            return GetString(name);
        }

        public static object GetObject(string name)
        {
            TextRes sys = GetLoader();
            if (sys == null)
                return null;
            return sys.resources.GetObject(name, TextRes.Culture);
        }

        private static TextRes GetLoader()
        {
            if (loader == null)
            {
                TextRes sr = new TextRes();
                Interlocked.CompareExchange(ref loader, sr, null);
            }

            return loader;
        }
    }
}
