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
    // Enum for status codes returned during recovery/reconfiguration
    public enum ReconfigurationStatus
    {
        SUCCESS = 0,
        PARTIAL_FAILURE = 1,
        LOCK_FAILURE = 2,
        UNLOCK_FAILURE = 4,
        FAULTY_WRITE_VIEW = 8,
        TABLE_NOT_FOUND = 16,
        FAILURE = 32
    }

    // Enum for actions to take on a given row during repair row.
    public enum RepairRowActionType
    {
        RepairRow,
        SkipRow,
        InvalidRow,
    }

    /// <summary>
    /// Caller can pass to RepairTable API a delegate to filter which rows to repair and which to skip.
    /// </summary>
    /// <param name="row"></param>
    /// <returns></returns>
    public delegate RepairRowActionType RepairRowDelegate(DynamicReplicatedTableEntity row);
}