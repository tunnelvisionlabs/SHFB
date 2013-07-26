﻿//===============================================================================================================
// System  : Sandcastle Guided Installation
// File    : AssemblyInfo.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 10/06/2012
// Compiler: Microsoft Visual C#
//
// Sandcastle Guided Installation application attributes.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice and all copyright notices must remain intact in all applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.0.0.0  02/05/2011  EFW  Created the code
// 1.1.0.0  03/05/2012  EFW  Converted to use WPF
// 1.1.0.1  10/06/2012  EFW  Merged SHFB installer pages into the main project
//===============================================================================================================

using System;
using System.Reflection;
using System.Resources;
using System.Runtime.InteropServices;

// Resources contained within the assembly are English
[assembly: NeutralResourcesLanguageAttribute("en")]

//
// General Information about an assembly is controlled through the following
// set of attributes. Change these attribute values to modify the information
// associated with an assembly.
//
[assembly: AssemblyProduct("Sandcastle Guided Installation")]
[assembly: AssemblyTitle("Sandcastle Guided Installation")]
[assembly: AssemblyDescription("This utility is used to guide you through the installation of the various " +
    "tools needed to create a working setup to build help files with the Microsoft Sandcastle tools.")]
[assembly: AssemblyConfiguration("")]
[assembly: AssemblyCompany("Eric Woodruff")]
[assembly: AssemblyCopyright("Copyright \xA9 2011-2012, Eric Woodruff, All Rights Reserved")]
[assembly: AssemblyTrademark("Eric Woodruff, All Rights Reserved")]
[assembly: AssemblyCulture("")]
[assembly: CLSCompliant(true)]
[assembly: ComVisible(false)]

//
// Version information for an assembly consists of the following four values:
//
//      Major Version
//      Minor Version
//      Build Number
//      Revision
//
// You can specify all the values or you can default the Revision and Build Numbers
// by using the '*' as shown below:

[assembly: AssemblyVersion("1.1.0.1")]  // NOTE: Update app.manifest with this version number when it changes
