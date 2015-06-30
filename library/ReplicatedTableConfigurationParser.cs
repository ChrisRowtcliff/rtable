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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.WindowsAzure.Storage.Blob;

    internal class ReplicatedTableConfigurationParser : IReplicatedTableConfigurationParser
    {
        /// <summary>
        /// Parses the RTable configuration blobs.
        /// Returns the list of views, the list of configured tables and the lease duration.
        /// If null is returned, then the value of tableConfigList/leaseDuration are not relevant.
        /// </summary>
        /// <param name="blobs"></param>
        /// <param name="useHttps"></param>
        /// <param name="tableConfigList"></param>
        /// <param name="leaseDuration"></param>
        /// <returns></returns>
        public List<View> ParseBlob(
                                List<CloudBlockBlob> blobs,
                                bool useHttps,
                                out List<ReplicatedTableConfiguredTable> tableConfigList,
                                out int leaseDuration)
        {
            tableConfigList = null;
            leaseDuration = 0;

            ReplicatedTableConfiguration configuration;
            List<string> eTags;

            QuorumReadResult result = CloudBlobHelpers.TryReadBlobQuorum(
                                                                    blobs,
                                                                    out configuration,
                                                                    out eTags,
                                                                    ReplicatedTableConfiguration.FromJson);
            if (result != QuorumReadResult.Success)
            {
                ReplicatedTableLogger.LogError("Unable to refresh views, result={0}", result);
                return null;
            }


            /**
             * Views:
             */
            var viewList = new List<View>();

            foreach (var entry in configuration.viewMap)
            {
                ReplicatedTableConfigurationStore configurationStore = entry.Value;
                var view = View.InitFromConfigVer2(entry.Key, configurationStore, useHttps);
                view.RefreshTime = DateTime.UtcNow;

                if (view.ViewId <= 0)
                {
                    ReplicatedTableLogger.LogError("ViewId={0} of  ViewName={1} is invalid. Must be >= 1.", view.ViewId, view.Name);
                    continue;
                }

                if (view.IsEmpty)
                {
                    continue;
                }

                // - ERROR!
                if (view.ReadHeadIndex > view.TailIndex)
                {
                    ReplicatedTableLogger.LogError("ReadHeadIndex={0} of  ViewName={1} is out of range. Must be <= {2}", view.ReadHeadIndex, view.Name, view.TailIndex);
                    continue;
                }

                viewList.Add(view);
            }

            if (!viewList.Any())
            {
                return null;
            }


            /**
             * Tables:
             */
            tableConfigList = configuration.tableList.ToList();


            // - lease duration
            leaseDuration = configuration.LeaseDuration;

            return viewList;
        }
    }
}