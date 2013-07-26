﻿// Copyright © Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change History
// 12/26/2012 - EFW - Moved the classes into the Targets namespace
// 12/30/2012 - EFW - Cleaned up the code and marked the class as serializable

using System;
using System.Collections.Generic;

namespace Microsoft.Ddue.Tools.Targets
{
    /// <summary>
    /// This represents a property target
    /// </summary>
    [Serializable]
    public class PropertyTarget : ProcedureTarget
    {
        #region Properties
        //=====================================================================

        public IList<Parameter> Parameters { get; private set; }

        public TypeReference ReturnType { get; private set; }

        #endregion

        #region Constructor
        //=====================================================================

        internal PropertyTarget(IList<Parameter> parameters, TypeReference returnType)
        {
            this.Parameters = parameters;
            this.ReturnType = returnType;
        }
        #endregion
    }
}
