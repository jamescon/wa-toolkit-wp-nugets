﻿// ----------------------------------------------------------------------------------
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

namespace Microsoft.WindowsAzure.Samples.Common.Storage
{
    using System;
    using System.IO;
    using StorageClient;
    using StorageClient.Protocol;

    public static class CloudBlobExtensions
    {
        public static void UploadText(this CloudBlob blob, string text, string leaseId)
        {
            string url = blob.Uri.ToString();
            if (blob.ServiceClient.Credentials.NeedsTransformUri)
            {
                url = blob.ServiceClient.Credentials.TransformUri(url);
            }

            var req = BlobRequest.Put(new Uri(url), 90, new BlobProperties(), BlobType.BlockBlob, leaseId, 0);
            using (var writer = new StreamWriter(req.GetRequestStream()))
            {
                writer.Write(text);
            }

            blob.ServiceClient.Credentials.SignRequest(req);
            req.GetResponse().Close();
        }

        public static void AcquireLeaseDuring(this CloudBlob blob, Action<string> action)
        {
            var leaseId = blob.AcquireLease();
            action(leaseId);
            blob.UploadText("text", "text");
            blob.ReleaseLease(leaseId);
        }
        
        public static string AcquireLease(this CloudBlob blob)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var req = BlobRequest.Lease(
                transformedUri,
                90, // timeout (in seconds)
                LeaseAction.Acquire, // as opposed to "break" "release" or "renew"
                null); // name of the existing lease, if any
            blob.ServiceClient.Credentials.SignRequest(req);
            using (var response = req.GetResponse())
            {
                return response.Headers["x-ms-lease-id"];
            }
        }

        public static void ReleaseLease(this CloudBlob blob, string leaseId)
        {
            DoLeaseOperation(blob, leaseId, LeaseAction.Release);
        }

        public static void RenewLease(this CloudBlob blob, string leaseId)
        {
            DoLeaseOperation(blob, leaseId, LeaseAction.Renew);
        }

        public static void BreakLease(this CloudBlob blob)
        {
            DoLeaseOperation(blob, null, LeaseAction.Break);
        }

        private static void DoLeaseOperation(CloudBlob blob, string leaseId, LeaseAction action)
        {
            var creds = blob.ServiceClient.Credentials;
            var transformedUri = new Uri(creds.TransformUri(blob.Uri.ToString()));
            var req = BlobRequest.Lease(transformedUri, 90, action, leaseId);
            creds.SignRequest(req);
            req.GetResponse().Close();
        }
    }
}