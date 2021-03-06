﻿// azure-rtable ver. 0.9
//
// Copyright (c) Microsoft Corporation
//
// All rights reserved.
//
// MIT License
//
// Permission is hereby granted, free of charge, to any person obtaining a copy of this software and associated documentation files
// (the ""Software""), to deal in the Software without restriction, including without limitation the rights to use, copy, modify,
// merge, publish, distribute, sublicense, and/or sell copies of the Software, and to permit persons to whom the Software is furnished
// to do so, subject to the following conditions:
//
// The above copyright notice and this permission notice shall be included in all copies or substantial portions of the Software.
//
// THE SOFTWARE IS PROVIDED *AS IS*, WITHOUT WARRANTY OF ANY KIND, EXPRESS OR IMPLIED, INCLUDING BUT NOT LIMITED TO THE WARRANTIES OF
// MERCHANTABILITY, FITNESS FOR A PARTICULAR PURPOSE AND NONINFRINGEMENT. IN NO EVENT SHALL THE AUTHORS OR COPYRIGHT HOLDERS BE LIABLE
// FOR ANY CLAIM, DAMAGES OR OTHER LIABILITY, WHETHER IN AN ACTION OF CONTRACT, TORT OR OTHERWISE, ARISING FROM, OUT OF OR IN CONNECTION
// WITH THE SOFTWARE OR THE USE OR OTHER DEALINGS IN THE SOFTWARE.


namespace Microsoft.Azure.Toolkit.Replication
{
    using System.Collections.Generic;
    using System.Linq;
    using System.Collections.ObjectModel;

    public enum ReplicatedTableQuorumReadCode
    {
        NullObject,
        NotFound,
        UpdateInProgress,
        Exception,
        NullOrLowSuccessRate,
        Success,
        BlobsNotInSyncOrTransitioning,
    }

    public class ReplicatedTableQuorumReadResult
    {
        public ReplicatedTableQuorumReadResult(ReplicatedTableQuorumReadCode code, ReadOnlyCollection<ReplicatedTableReadBlobResult> results)
        {
            Code = code;
            Results = results;
        }

        public ReplicatedTableQuorumReadCode Code { get; private set; }

        public ReadOnlyCollection<ReplicatedTableReadBlobResult> Results { get; private set; }

        public override string ToString()
        {
            if (Results == null)
            {
                return "";
            }

            // IMPORTANT: foreach()/LINQ throws when "Results" changes (race condition).
            string msg = null;
            for (int index = 0; index < Results.Count; index++)
            {
                msg += "\n";
                msg += string.Format("Blob #{0} -> {1}", index, Results[index]);
            }

            return string.Format("QuorumReadResult Code: {0}, Message: {1}", Code, msg);
        }
    }
}
