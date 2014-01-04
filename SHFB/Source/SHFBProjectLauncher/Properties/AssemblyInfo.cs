﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Project Launcher
// File    : AssemblyInfo.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/29/2013
// Note    : Copyright 2011-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// Sandcastle Help File Builder project launcher attributes.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
// ==============================================================================================================
// 08/02/2006  EFW  Created the code
//===============================================================================================================

using System;
using System.Reflection;

//
// General Information about an assembly is controlled through the following set of attributes.  Change these
// attribute values to modify the information associated with an assembly.
//
[assembly: AssemblyTitle("Sandcastle Help File Builder Project Launcher")]
[assembly: AssemblyDescription("This utility is used to open Sandcastle Help File Builder project files " +
    "using either the standalone GUI or Visual Studio.")]

[assembly: CLSCompliant(true)]

// This sets the version for the MSI file in the WiX project which only supports a Major value between 0 and 255.
[assembly: AssemblyFileVersion(AssemblyInfo.InstallerVersion)]

// See AssemblyInfoShared.cs for the shared attributes common to all projects in the solution.
