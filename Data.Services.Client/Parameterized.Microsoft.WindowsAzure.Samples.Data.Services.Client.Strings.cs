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

    internal static class Strings
    {
        internal static string BatchStream_MissingBoundary
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamMissingBoundary);
            }
        }

        internal static string BatchStream_GetMethodNotSupportedInChangeset
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamGetMethodNotSupportedInChangeset);
            }
        }

        internal static string BatchStream_InvalidBatchFormat
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamInvalidBatchFormat);
            }
        }

        internal static string BatchStream_MissingEndChangesetDelimiter
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamMissingEndChangesetDelimiter);
            }
        }

        internal static string BatchStream_OnlyGETOperationsCanBeSpecifiedInBatch
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamOnlyGETOperationsCanBeSpecifiedInBatch);
            }
        }

        internal static string BatchStream_InvalidOperationHeaderSpecified
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamInvalidOperationHeaderSpecified);
            }
        }

        internal static string BatchStream_MoreDataAfterEndOfBatch
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamMoreDataAfterEndOfBatch);
            }
        }

        internal static string BatchStream_InternalBufferRequestTooSmall
        {
            get
            {
                return TextRes.GetString(TextRes.BatchStreamInternalBufferRequestTooSmall);
            }
        }

        internal static string Batch_IncompleteResponseCount
        {
            get
            {
                return TextRes.GetString(TextRes.BatchIncompleteResponseCount);
            }
        }

        internal static string Context_BaseUri
        {
            get
            {
                return TextRes.GetString(TextRes.ContextBaseUri);
            }
        }

        internal static string Context_TrackingExpectsAbsoluteUri
        {
            get
            {
                return TextRes.GetString(TextRes.ContextTrackingExpectsAbsoluteUri);
            }
        }

        internal static string Context_LinkResourceInsertFailure
        {
            get
            {
                return TextRes.GetString(TextRes.ContextLinkResourceInsertFailure);
            }
        }

        internal static string Context_BatchExecuteError
        {
            get
            {
                return TextRes.GetString(TextRes.ContextBatchExecuteError);
            }
        }

        internal static string Context_EntitySetName
        {
            get
            {
                return TextRes.GetString(TextRes.ContextEntitySetName);
            }
        }

        internal static string Context_MissingEditLinkInResponseBody
        {
            get
            {
                return TextRes.GetString(TextRes.ContextMissingEditLinkInResponseBody);
            }
        }

        internal static string Context_MissingSelfLinkInResponseBody
        {
            get
            {
                return TextRes.GetString(TextRes.ContextMissingSelfLinkInResponseBody);
            }
        }

        internal static string Context_MissingEditMediaLinkInResponseBody
        {
            get
            {
                return TextRes.GetString(TextRes.ContextMissingEditMediaLinkInResponseBody);
            }
        }

        internal static string Content_EntityWithoutKey
        {
            get
            {
                return TextRes.GetString(TextRes.ContentEntityWithoutKey);
            }
        }

        internal static string Content_EntityIsNotEntityType
        {
            get
            {
                return TextRes.GetString(TextRes.ContentEntityIsNotEntityType);
            }
        }

        internal static string Context_EntityNotContained
        {
            get
            {
                return TextRes.GetString(TextRes.ContextEntityNotContained);
            }
        }

        internal static string Context_EntityAlreadyContained
        {
            get
            {
                return TextRes.GetString(TextRes.ContextEntityAlreadyContained);
            }
        }

        internal static string Context_DifferentEntityAlreadyContained
        {
            get
            {
                return TextRes.GetString(TextRes.ContextDifferentEntityAlreadyContained);
            }
        }

        internal static string Context_DidNotOriginateAsync
        {
            get
            {
                return TextRes.GetString(TextRes.ContextDidNotOriginateAsync);
            }
        }

        internal static string Context_AsyncAlreadyDone
        {
            get
            {
                return TextRes.GetString(TextRes.ContextAsyncAlreadyDone);
            }
        }

        internal static string Context_OperationCanceled
        {
            get
            {
                return TextRes.GetString(TextRes.ContextOperationCanceled);
            }
        }

        internal static string Context_NoLoadWithInsertEnd
        {
            get
            {
                return TextRes.GetString(TextRes.ContextNoLoadWithInsertEnd);
            }
        }

        internal static string Context_NoRelationWithInsertEnd
        {
            get
            {
                return TextRes.GetString(TextRes.ContextNoRelationWithInsertEnd);
            }
        }

        internal static string Context_NoRelationWithDeleteEnd
        {
            get
            {
                return TextRes.GetString(TextRes.ContextNoRelationWithDeleteEnd);
            }
        }

        internal static string Context_RelationAlreadyContained
        {
            get
            {
                return TextRes.GetString(TextRes.ContextRelationAlreadyContained);
            }
        }

        internal static string Context_RelationNotRefOrCollection
        {
            get
            {
                return TextRes.GetString(TextRes.ContextRelationNotRefOrCollection);
            }
        }

        internal static string Context_AddLinkCollectionOnly
        {
            get
            {
                return TextRes.GetString(TextRes.ContextAddLinkCollectionOnly);
            }
        }

        internal static string Context_AddRelatedObjectCollectionOnly
        {
            get
            {
                return TextRes.GetString(TextRes.ContextAddRelatedObjectCollectionOnly);
            }
        }

        internal static string Context_AddRelatedObjectSourceDeleted
        {
            get
            {
                return TextRes.GetString(TextRes.ContextAddRelatedObjectSourceDeleted);
            }
        }

        internal static string Context_SetLinkReferenceOnly
        {
            get
            {
                return TextRes.GetString(TextRes.ContextSetLinkReferenceOnly);
            }
        }

        internal static string Context_BatchNotSupportedForMediaLink
        {
            get
            {
                return TextRes.GetString(TextRes.ContextBatchNotSupportedForMediaLink);
            }
        }

        internal static string Context_UnexpectedZeroRawRead
        {
            get
            {
                return TextRes.GetString(TextRes.ContextUnexpectedZeroRawRead);
            }
        }

        internal static string Context_ChildResourceExists
        {
            get
            {
                return TextRes.GetString(TextRes.ContextChildResourceExists);
            }
        }

        internal static string Context_EntityNotMediaLinkEntry
        {
            get
            {
                return TextRes.GetString(TextRes.ContextEntityNotMediaLinkEntry);
            }
        }

        internal static string Context_SetSaveStreamWithoutEditMediaLink
        {
            get
            {
                return TextRes.GetString(TextRes.ContextSetSaveStreamWithoutEditMediaLink);
            }
        }

        internal static string ClientType_MultipleImplementationNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ClientTypeMultipleImplementationNotSupported);
            }
        }

        internal static string ClientType_CollectionOfNonEntities
        {
            get
            {
                return TextRes.GetString(TextRes.ClientTypeCollectionOfNonEntities);
            }
        }

        internal static string DataServiceException_GeneralError
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceExceptionGeneralError);
            }
        }

        internal static string Deserialize_GetEnumerator
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeGetEnumerator);
            }
        }

        internal static string Deserialize_MixedTextWithComment
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeMixedTextWithComment);
            }
        }

        internal static string Deserialize_ExpectingSimpleValue
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeExpectingSimpleValue);
            }
        }

        internal static string Deserialize_NotApplicationXml
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeNotApplicationXml);
            }
        }

        internal static string Deserialize_MismatchAtomLinkLocalSimple
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeMismatchAtomLinkLocalSimple);
            }
        }

        internal static string Deserialize_ExpectedEmptyMediaLinkEntryContent
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeExpectedEmptyMediaLinkEntryContent);
            }
        }

        internal static string Deserialize_ContentPlusPropertiesNotAllowed
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeContentPlusPropertiesNotAllowed);
            }
        }

        internal static string Deserialize_NoLocationHeader
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeNoLocationHeader);
            }
        }

        internal static string Deserialize_MissingIdElement
        {
            get
            {
                return TextRes.GetString(TextRes.DeserializeMissingIdElement);
            }
        }

        internal static string HttpProcessUtility_ContentTypeMissing
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityContentTypeMissing);
            }
        }

        internal static string HttpProcessUtility_MediaTypeMissingValue
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityMediaTypeMissingValue);
            }
        }

        internal static string HttpProcessUtility_MediaTypeRequiresSemicolonBeforeParameter
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityMediaTypeRequiresSemicolonBeforeParameter);
            }
        }

        internal static string HttpProcessUtility_MediaTypeRequiresSlash
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityMediaTypeRequiresSlash);
            }
        }

        internal static string HttpProcessUtility_MediaTypeRequiresSubType
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityMediaTypeRequiresSubType);
            }
        }

        internal static string HttpProcessUtility_MediaTypeUnspecified
        {
            get
            {
                return TextRes.GetString(TextRes.HttpProcessUtilityMediaTypeUnspecified);
            }
        }

        internal static string MaterializeFromAtom_CountNotPresent
        {
            get
            {
                return TextRes.GetString(TextRes.MaterializeFromAtomCountNotPresent);
            }
        }

        internal static string MaterializeFromAtom_CountFormatError
        {
            get
            {
                return TextRes.GetString(TextRes.MaterializeFromAtomCountFormatError);
            }
        }

        internal static string MaterializeFromAtom_TopLevelLinkNotAvailable
        {
            get
            {
                return TextRes.GetString(TextRes.MaterializeFromAtomTopLevelLinkNotAvailable);
            }
        }

        internal static string MaterializeFromAtom_CollectionKeyNotPresentInLinkTable
        {
            get
            {
                return TextRes.GetString(TextRes.MaterializeFromAtomCollectionKeyNotPresentInLinkTable);
            }
        }

        internal static string MaterializeFromAtom_GetNestLinkForFlatCollection
        {
            get
            {
                return TextRes.GetString(TextRes.MaterializeFromAtomGetNestLinkForFlatCollection);
            }
        }

        internal static string Util_EmptyString
        {
            get
            {
                return TextRes.GetString(TextRes.UtilEmptyString);
            }
        }

        internal static string Util_EmptyArray
        {
            get
            {
                return TextRes.GetString(TextRes.UtilEmptyArray);
            }
        }

        internal static string Util_NullArrayElement
        {
            get
            {
                return TextRes.GetString(TextRes.UtilNullArrayElement);
            }
        }

        internal static string ALinq_TypeBinaryNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqTypeBinaryNotSupported);
            }
        }

        internal static string ALinq_ConditionalNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqConditionalNotSupported);
            }
        }

        internal static string ALinq_ParameterNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqParameterNotSupported);
            }
        }

        internal static string ALinq_LambdaNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqLambdaNotSupported);
            }
        }

        internal static string ALinq_NewNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqNewNotSupported);
            }
        }

        internal static string ALinq_MemberInitNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqMemberInitNotSupported);
            }
        }

        internal static string ALinq_ListInitNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqListInitNotSupported);
            }
        }

        internal static string ALinq_NewArrayNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqNewArrayNotSupported);
            }
        }

        internal static string ALinq_InvocationNotSupported
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqInvocationNotSupported);
            }
        }

        internal static string ALinq_QueryOptionsOnlyAllowedOnLeafNodes
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqQueryOptionsOnlyAllowedOnLeafNodes);
            }
        }

        internal static string ALinq_CantExpand
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCantExpand);
            }
        }

        internal static string ALinq_CantNavigateWithoutKeyPredicate
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCantNavigateWithoutKeyPredicate);
            }
        }

        internal static string ALinq_CanOnlyApplyOneKeyPredicate
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCanOnlyApplyOneKeyPredicate);
            }
        }

        internal static string ALinq_CantAddQueryOption
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCantAddQueryOption);
            }
        }

        internal static string ALinq_QueryOptionsOnlyAllowedOnSingletons
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqQueryOptionsOnlyAllowedOnSingletons);
            }
        }

        internal static string ALinq_CannotAddCountOption
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCannotAddCountOption);
            }
        }

        internal static string ALinq_CannotAddCountOptionConflict
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCannotAddCountOptionConflict);
            }
        }

        internal static string ALinq_ProjectionOnlyAllowedOnLeafNodes
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqProjectionOnlyAllowedOnLeafNodes);
            }
        }

        internal static string ALinq_ProjectionCanOnlyHaveOneProjection
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqProjectionCanOnlyHaveOneProjection);
            }
        }

        internal static string ALinq_CannotConstructKnownEntityTypes
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCannotConstructKnownEntityTypes);
            }
        }

        internal static string ALinq_CannotCreateConstantEntity
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCannotCreateConstantEntity);
            }
        }

        internal static string ALinq_CanOnlyProjectTheLeaf
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCanOnlyProjectTheLeaf);
            }
        }

        internal static string ALinq_CannotProjectWithExplicitExpansion
        {
            get
            {
                return TextRes.GetString(TextRes.ALinqCannotProjectWithExplicitExpansion);
            }
        }

        internal static string DSKAttribute_MustSpecifyAtleastOnePropertyName
        {
            get
            {
                return TextRes.GetString(TextRes.DSKAttributeMustSpecifyAtleastOnePropertyName);
            }
        }

        internal static string HttpWebRequest_Aborted
        {
            get
            {
                return TextRes.GetString(TextRes.HttpWebRequestAborted);
            }
        }

        internal static string DataServiceCollection_LoadRequiresTargetCollectionObserved
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionLoadRequiresTargetCollectionObserved);
            }
        }

        internal static string DataServiceCollection_CannotStopTrackingChildCollection
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionCannotStopTrackingChildCollection);
            }
        }

        internal static string DataServiceCollection_DataServiceQueryCanNotBeEnumerated
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionDataServiceQueryCanNotBeEnumerated);
            }
        }

        internal static string DataServiceCollection_OperationForTrackedOnly
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionOperationForTrackedOnly);
            }
        }

        internal static string DataServiceCollection_CannotDetermineContextFromItems
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionCannotDetermineContextFromItems);
            }
        }

        internal static string DataServiceCollection_InsertIntoTrackedButNotLoadedCollection
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionInsertIntoTrackedButNotLoadedCollection);
            }
        }

        internal static string DataServiceCollection_MultipleLoadAsyncOperationsAtTheSameTime
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionMultipleLoadAsyncOperationsAtTheSameTime);
            }
        }

        internal static string DataServiceCollection_LoadAsyncNoParamsWithoutParentEntity
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionLoadAsyncNoParamsWithoutParentEntity);
            }
        }

        internal static string DataServiceCollection_LoadAsyncRequiresDataServiceQuery
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceCollectionLoadAsyncRequiresDataServiceQuery);
            }
        }

        internal static string DataBinding_BindingOperation_DetachedSource
        {
            get
            {
                return TextRes.GetString(TextRes.DataBindingBindingOperationDetachedSource);
            }
        }

        internal static string AtomParser_FeedUnexpected
        {
            get
            {
                return TextRes.GetString(TextRes.AtomParserFeedUnexpected);
            }
        }

        internal static string AtomParser_PagingLinkOutsideOfFeed
        {
            get
            {
                return TextRes.GetString(TextRes.AtomParserPagingLinkOutsideOfFeed);
            }
        }

        internal static string AtomParser_ManyFeedCounts
        {
            get
            {
                return TextRes.GetString(TextRes.AtomParserManyFeedCounts);
            }
        }

        internal static string AtomParser_FeedCountNotUnderFeed
        {
            get
            {
                return TextRes.GetString(TextRes.AtomParserFeedCountNotUnderFeed);
            }
        }

        internal static string AtomParser_UnexpectedContentUnderExpandedLink
        {
            get
            {
                return TextRes.GetString(TextRes.AtomParserUnexpectedContentUnderExpandedLink);
            }
        }

        internal static string AtomMaterializer_DuplicatedNextLink
        {
            get
            {
                return TextRes.GetString(TextRes.AtomMaterializerDuplicatedNextLink);
            }
        }

        internal static string AtomMaterializer_LinksMissingHref
        {
            get
            {
                return TextRes.GetString(TextRes.AtomMaterializerLinksMissingHref);
            }
        }

        internal static string DataServiceQuery_EnumerationNotSupportedInSL
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceQueryEnumerationNotSupportedInSL);
            }
        }

        internal static string DataServiceState_CollectionNotInContext
        {
            get
            {
                return TextRes.GetString(TextRes.DataServiceStateCollectionNotInContext);
            }
        }

        internal static string BatchStream_ContentExpected(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamContentExpected, p0);
        }

        internal static string BatchStream_ContentUnexpected(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamContentUnexpected, p0);
        }

        internal static string BatchStream_InvalidDelimiter(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidDelimiter, p0);
        }

        internal static string BatchStream_InvalidHeaderValueSpecified(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidHeaderValueSpecified, p0);
        }

        internal static string BatchStream_InvalidContentLengthSpecified(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidContentLengthSpecified, p0);
        }

        internal static string BatchStream_InvalidHttpMethodName(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidHttpMethodName, p0);
        }

        internal static string BatchStream_InvalidMethodHeaderSpecified(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidMethodHeaderSpecified, p0);
        }

        internal static string BatchStream_InvalidHttpVersionSpecified(object p0, object p1)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidHttpVersionSpecified, p0, p1);
        }

        internal static string BatchStream_InvalidNumberOfHeadersAtOperationStart(object p0, object p1)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidNumberOfHeadersAtOperationStart, p0, p1);
        }

        internal static string BatchStream_MissingOrInvalidContentEncodingHeader(object p0, object p1)
        {
            return TextRes.GetString(TextRes.BatchStreamMissingOrInvalidContentEncodingHeader, p0, p1);
        }

        internal static string BatchStream_InvalidNumberOfHeadersAtChangeSetStart(object p0, object p1)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidNumberOfHeadersAtChangeSetStart, p0, p1);
        }

        internal static string BatchStream_MissingContentTypeHeader(object p0)
        {
            return TextRes.GetString(TextRes.BatchStreamMissingContentTypeHeader, p0);
        }

        internal static string BatchStream_InvalidContentTypeSpecified(object p0, object p1, object p2, object p3)
        {
            return TextRes.GetString(TextRes.BatchStreamInvalidContentTypeSpecified, p0, p1, p2, p3);
        }

        internal static string Batch_ExpectedContentType(object p0)
        {
            return TextRes.GetString(TextRes.BatchExpectedContentType, p0);
        }

        internal static string Batch_ExpectedResponse(object p0)
        {
            return TextRes.GetString(TextRes.BatchExpectedResponse, p0);
        }

        internal static string Batch_UnexpectedContent(object p0)
        {
            return TextRes.GetString(TextRes.BatchUnexpectedContent, p0);
        }

        internal static string Context_CannotConvertKey(object p0)
        {
            return TextRes.GetString(TextRes.ContextCannotConvertKey, p0);
        }

        internal static string Context_InternalError(object p0)
        {
            return TextRes.GetString(TextRes.ContextInternalError, p0);
        }

        internal static string Context_NoContentTypeForMediaLink(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ContextNoContentTypeForMediaLink, p0, p1);
        }

        internal static string Context_VersionNotSupported(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ContextVersionNotSupported, p0, p1);
        }

        internal static string Context_MLEWithoutSaveStream(object p0)
        {
            return TextRes.GetString(TextRes.ContextMLEWithoutSaveStream, p0);
        }

        internal static string Context_SetSaveStreamOnMediaEntryProperty(object p0)
        {
            return TextRes.GetString(TextRes.ContextSetSaveStreamOnMediaEntryProperty, p0);
        }

        internal static string Collection_NullCollectionReference(object p0, object p1)
        {
            return TextRes.GetString(TextRes.CollectionNullCollectionReference, p0, p1);
        }

        internal static string ClientType_MissingOpenProperty(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ClientTypeMissingOpenProperty, p0, p1);
        }

        internal static string Clienttype_MultipleOpenProperty(object p0)
        {
            return TextRes.GetString(TextRes.ClienttypeMultipleOpenProperty, p0);
        }

        internal static string ClientType_MissingProperty(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ClientTypeMissingProperty, p0, p1);
        }

        internal static string ClientType_KeysMustBeSimpleTypes(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeKeysMustBeSimpleTypes, p0);
        }

        internal static string ClientType_KeysOnDifferentDeclaredType(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeKeysOnDifferentDeclaredType, p0);
        }

        internal static string ClientType_MissingMimeTypeProperty(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ClientTypeMissingMimeTypeProperty, p0, p1);
        }

        internal static string ClientType_MissingMediaEntryProperty(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeMissingMediaEntryProperty, p0);
        }

        internal static string ClientType_NoSettableFields(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeNoSettableFields, p0);
        }

        internal static string ClientType_NullOpenProperties(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeNullOpenProperties, p0);
        }

        internal static string ClientType_Ambiguous(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ClientTypeAmbiguous, p0, p1);
        }

        internal static string ClientType_UnsupportedType(object p0)
        {
            return TextRes.GetString(TextRes.ClientTypeUnsupportedType, p0);
        }

        internal static string Deserialize_Current(object p0, object p1)
        {
            return TextRes.GetString(TextRes.DeserializeCurrent, p0, p1);
        }

        internal static string Deserialize_MismatchAtomLinkFeedPropertyNotCollection(object p0)
        {
            return TextRes.GetString(TextRes.DeserializeMismatchAtomLinkFeedPropertyNotCollection, p0);
        }

        internal static string Deserialize_MismatchAtomLinkEntryPropertyIsCollection(object p0)
        {
            return TextRes.GetString(TextRes.DeserializeMismatchAtomLinkEntryPropertyIsCollection, p0);
        }

        internal static string Deserialize_UnknownMimeTypeSpecified(object p0)
        {
            return TextRes.GetString(TextRes.DeserializeUnknownMimeTypeSpecified, p0);
        }

        internal static string Deserialize_ServerException(object p0)
        {
            return TextRes.GetString(TextRes.DeserializeServerException, p0);
        }

        internal static string EpmClientType_PropertyIsComplex(object p0)
        {
            return TextRes.GetString(TextRes.EpmClientTypePropertyIsComplex, p0);
        }

        internal static string EpmClientType_PropertyIsPrimitive(object p0)
        {
            return TextRes.GetString(TextRes.EpmClientTypePropertyIsPrimitive, p0);
        }

        internal static string EpmSourceTree_InvalidSourcePath(object p0, object p1)
        {
            return TextRes.GetString(TextRes.EpmSourceTreeInvalidSourcePath, p0, p1);
        }

        internal static string EpmSourceTree_DuplicateEpmAttrsWithSameSourceName(object p0, object p1)
        {
            return TextRes.GetString(TextRes.EpmSourceTreeDuplicateEpmAttrsWithSameSourceName, p0, p1);
        }

        internal static string EpmSourceTree_InaccessiblePropertyOnType(object p0, object p1)
        {
            return TextRes.GetString(TextRes.EpmSourceTreeInaccessiblePropertyOnType, p0, p1);
        }

        internal static string EpmTargetTree_InvalidTargetPath(object p0)
        {
            return TextRes.GetString(TextRes.EpmTargetTreeInvalidTargetPath, p0);
        }

        internal static string EpmTargetTree_AttributeInMiddle(object p0)
        {
            return TextRes.GetString(TextRes.EpmTargetTreeAttributeInMiddle, p0);
        }

        internal static string EpmTargetTree_DuplicateEpmAttrsWithSameTargetName(object p0, object p1, object p2, object p3)
        {
            return TextRes.GetString(TextRes.EpmTargetTreeDuplicateEpmAttrsWithSameTargetName, p0, p1, p2, p3);
        }

        internal static string EntityPropertyMapping_EpmAttribute(object p0)
        {
            return TextRes.GetString(TextRes.EntityPropertyMappingEpmAttribute, p0);
        }

        internal static string EntityPropertyMapping_TargetNamespaceUriNotValid(object p0)
        {
            return TextRes.GetString(TextRes.EntityPropertyMappingTargetNamespaceUriNotValid, p0);
        }

        internal static string HttpProcessUtility_EncodingNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.HttpProcessUtilityEncodingNotSupported, p0);
        }

        internal static string HttpProcessUtility_EscapeCharWithoutQuotes(object p0)
        {
            return TextRes.GetString(TextRes.HttpProcessUtilityEscapeCharWithoutQuotes, p0);
        }

        internal static string HttpProcessUtility_EscapeCharAtEnd(object p0)
        {
            return TextRes.GetString(TextRes.HttpProcessUtilityEscapeCharAtEnd, p0);
        }

        internal static string HttpProcessUtility_ClosingQuoteNotFound(object p0)
        {
            return TextRes.GetString(TextRes.HttpProcessUtilityClosingQuoteNotFound, p0);
        }

        internal static string Serializer_NullKeysAreNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.SerializerNullKeysAreNotSupported, p0);
        }

        internal static string ALinq_UnsupportedExpression(object p0)
        {
            return TextRes.GetString(TextRes.ALinqUnsupportedExpression, p0);
        }

        internal static string ALinq_CouldNotConvert(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCouldNotConvert, p0);
        }

        internal static string ALinq_MethodNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.ALinqMethodNotSupported, p0);
        }

        internal static string ALinq_UnaryNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.ALinqUnaryNotSupported, p0);
        }

        internal static string ALinq_BinaryNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.ALinqBinaryNotSupported, p0);
        }

        internal static string ALinq_ConstantNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.ALinqConstantNotSupported, p0);
        }

        internal static string ALinq_MemberAccessNotSupported(object p0)
        {
            return TextRes.GetString(TextRes.ALinqMemberAccessNotSupported, p0);
        }

        internal static string ALinq_CantCastToUnsupportedPrimitive(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantCastToUnsupportedPrimitive, p0);
        }

        internal static string ALinq_CantTranslateExpression(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantTranslateExpression, p0);
        }

        internal static string ALinq_TranslationError(object p0)
        {
            return TextRes.GetString(TextRes.ALinqTranslationError, p0);
        }

        internal static string ALinq_CantAddDuplicateQueryOption(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantAddDuplicateQueryOption, p0);
        }

        internal static string ALinq_CantAddAstoriaQueryOption(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantAddAstoriaQueryOption, p0);
        }

        internal static string ALinq_CantAddQueryOptionStartingWithDollarSign(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantAddQueryOptionStartingWithDollarSign, p0);
        }

        internal static string ALinq_CantReferToPublicField(object p0)
        {
            return TextRes.GetString(TextRes.ALinqCantReferToPublicField, p0);
        }

        internal static string ALinq_PropertyNamesMustMatchInProjections(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ALinqPropertyNamesMustMatchInProjections, p0, p1);
        }

        internal static string ALinq_QueryOptionOutOfOrder(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ALinqQueryOptionOutOfOrder, p0, p1);
        }

        internal static string ALinq_ProjectionMemberAssignmentMismatch(object p0, object p1, object p2)
        {
            return TextRes.GetString(TextRes.ALinqProjectionMemberAssignmentMismatch, p0, p1, p2);
        }

        internal static string ALinq_ExpressionNotSupportedInProjectionToEntity(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ALinqExpressionNotSupportedInProjectionToEntity, p0, p1);
        }

        internal static string ALinq_ExpressionNotSupportedInProjection(object p0, object p1)
        {
            return TextRes.GetString(TextRes.ALinqExpressionNotSupportedInProjection, p0, p1);
        }

        internal static string HttpWeb_Internal(object p0)
        {
            return TextRes.GetString(TextRes.HttpWebInternal, p0);
        }

        internal static string HttpWeb_InternalArgument(object p0, object p1)
        {
            return TextRes.GetString(TextRes.HttpWebInternalArgument, p0, p1);
        }

        internal static string DataBinding_DataServiceCollectionArgumentMustHaveEntityType(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingDataServiceCollectionArgumentMustHaveEntityType, p0);
        }

        internal static string DataBinding_CollectionPropertySetterValueHasObserver(object p0, object p1)
        {
            return TextRes.GetString(TextRes.DataBindingCollectionPropertySetterValueHasObserver, p0, p1);
        }

        internal static string DataBinding_CollectionChangedUnknownAction(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingCollectionChangedUnknownAction, p0);
        }

        internal static string DataBinding_BindingOperation_ArrayItemNull(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingBindingOperationArrayItemNull, p0);
        }

        internal static string DataBinding_BindingOperation_ArrayItemNotEntity(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingBindingOperationArrayItemNotEntity, p0);
        }

        internal static string DataBinding_Util_UnknownEntitySetName(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingUtilUnknownEntitySetName, p0);
        }

        internal static string DataBinding_EntityAlreadyInCollection(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingEntityAlreadyInCollection, p0);
        }

        internal static string DataBinding_NotifyPropertyChangedNotImpl(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingNotifyPropertyChangedNotImpl, p0);
        }

        internal static string DataBinding_ComplexObjectAssociatedWithMultipleEntities(object p0)
        {
            return TextRes.GetString(TextRes.DataBindingComplexObjectAssociatedWithMultipleEntities, p0);
        }

        internal static string AtomMaterializer_CannotAssignNull(object p0, object p1)
        {
            return TextRes.GetString(TextRes.AtomMaterializerCannotAssignNull, p0, p1);
        }

        internal static string AtomMaterializer_EntryIntoCollectionMismatch(object p0, object p1)
        {
            return TextRes.GetString(TextRes.AtomMaterializerEntryIntoCollectionMismatch, p0, p1);
        }

        internal static string AtomMaterializer_EntryToAccessIsNull(object p0)
        {
            return TextRes.GetString(TextRes.AtomMaterializerEntryToAccessIsNull, p0);
        }

        internal static string AtomMaterializer_EntryToInitializeIsNull(object p0)
        {
            return TextRes.GetString(TextRes.AtomMaterializerEntryToInitializeIsNull, p0);
        }

        internal static string AtomMaterializer_ProjectEntityTypeMismatch(object p0, object p1, object p2)
        {
            return TextRes.GetString(TextRes.AtomMaterializerProjectEntityTypeMismatch, p0, p1, p2);
        }

        internal static string AtomMaterializer_PropertyMissing(object p0)
        {
            return TextRes.GetString(TextRes.AtomMaterializerPropertyMissing, p0);
        }

        internal static string AtomMaterializer_PropertyMissingFromEntry(object p0, object p1)
        {
            return TextRes.GetString(TextRes.AtomMaterializerPropertyMissingFromEntry, p0, p1);
        }

        internal static string AtomMaterializer_PropertyNotExpectedEntry(object p0, object p1)
        {
            return TextRes.GetString(TextRes.AtomMaterializerPropertyNotExpectedEntry, p0, p1);
        }
    }
}
