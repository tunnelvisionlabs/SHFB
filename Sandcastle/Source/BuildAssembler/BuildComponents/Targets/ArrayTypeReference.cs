﻿// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change History
// 12/26/2012 - EFW - Moved the classes into the Targets namespace
// 12/30/2012 - EFW - Cleaned up the code and marked the class as serializable

using System;

namespace Microsoft.Ddue.Tools.Targets
{
    /// <summary>
    /// This represents an array type reference
    /// </summary>
    [Serializable]
    public class ArrayTypeReference : TypeReference
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the element type
        /// </summary>
        public TypeReference ElementType { get; private set; }

        /// <summary>
        /// This read-only property returns the array rank
        /// </summary>
        public int Rank { get; private set; }

        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="elementType">The element type</param>
        /// <param name="rank">The array rank</param>
        internal ArrayTypeReference(TypeReference elementType, int rank)
        {
            if(elementType == null)
                throw new ArgumentNullException("elementType");

            if(rank <= 0)
                throw new ArgumentOutOfRangeException("rank");

            this.ElementType = elementType;
            this.Rank = rank;
        }
        #endregion
    }
}
