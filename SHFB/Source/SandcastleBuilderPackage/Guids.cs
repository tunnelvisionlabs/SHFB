﻿//=============================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// File    : Guids.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/29/2011
// Note    : Copyright 2011, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains various GUIDs for the package
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy
// of the license should be distributed with the code.  It can also be found
// at the project website: http://SHFB.CodePlex.com.  This notice, the
// author's name, and all copyright notices must remain intact in all
// applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ============================================================================
// 1.9.3.0  03/18/2011  EFW  Created the code
// 1.9.3.3  12/26/2011  EFW  Added the GUIDs for the new file editors
//=============================================================================

using System;

namespace SandcastleBuilder.Package
{
    /// <summary>
    /// This class contains various GUID values for the package
    /// </summary>
    /// <remarks>The main package and command set GUIDs are generated by Guids.tt</remarks>
    static partial class GuidList
    {
        /// <summary>The package project factory GUID in string form</summary>
        public const string guidSandcastleBuilderProjectFactoryString = "7CF6DF6D-3B04-46f8-A40B-537D21BCA0B4";
        /// <summary>The package project factory GUID</summary>
        public static readonly Guid guidSandcastleBuilderProjectFactory = new Guid(guidSandcastleBuilderProjectFactoryString);

        /// <summary>Content layout editor factory GUID string</summary>
        public const string guidContentLayoutEditorFactoryString = "7AAD2922-72A2-42C1-A077-85F5097A8FA7";

        /// <summary>Resource item editor factory GUID string</summary>
        public const string guidResourceItemEditorFactoryString = "1C79180C-BB93-46D2-B4D3-F22E7015A6F1";

        /// <summary>Site map editor factory GUID string</summary>
        public const string guidSiteMapEditorFactoryString = "DED740F1-EB91-48E3-9A41-4E4942FE53C1";

        /// <summary>Token editor factory GUID string</summary>
        public const string guidTokenEditorFactoryString = "D481FB70-9BF0-4868-9D4C-5DB33C6565E1";
    };
}
