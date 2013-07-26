﻿// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change History
// 12/26/2012 - EFW - Moved the classes into the Targets namespace

using System.Collections.Generic;

namespace Microsoft.Ddue.Tools.Targets
{
    public class Target
    {
        /// <summary>
        /// This is used to get or set the target's member ID
        /// </summary>
        public string Id { get; internal set; }

        /// <summary>
        /// This is used to get or set the target's container
        /// </summary>
        public string Container { get; internal set; }

        /// <summary>
        /// This is used to get or set the target's reference topic filename
        /// </summary>
        public string File { get; internal set; }

        /// <summary>
        /// This is used to get or set whether or not the target is an invalid link
        /// </summary>
        public bool IsInvalidLink { get; set; }

        /// <summary>
        /// Add the target to the given collection
        /// </summary>
        /// <param name="targets">The targets dictionary to which this target is added</param>
        /// <remarks>This can be overridden to add dependent targets to the collection as well</remarks>
        public virtual void Add(IDictionary<string, Target> targets)
        {
            targets[this.Id] = this;
        }
    }
}
