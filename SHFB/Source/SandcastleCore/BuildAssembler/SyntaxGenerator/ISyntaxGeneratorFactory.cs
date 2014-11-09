﻿//===============================================================================================================
// System  : Sandcastle Tools - Sandcastle Tools Core Class Library
// File    : ISyntaxGeneratorFactory.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/17/2013
// Note    : Copyright 2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a factory interface for syntax generator components
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
// ==============================================================================================================
// 12/17/2013  EFW  Created the code
//===============================================================================================================

namespace Sandcastle.Core.BuildAssembler.SyntaxGenerator
{
    /// <summary>
    /// This interface defines the factory method for syntax generators
    /// </summary>
    /// <remarks>Although not required, syntax generators are non-shared and instances are created as needed.</remarks>
    public interface ISyntaxGeneratorFactory
    {
        /// <summary>
        /// This read-only property is implemented to return the default and localized resource item file
        /// location.
        /// </summary>
        /// <remarks><para>If it returns null or an empty string, it is assumed the syntax generator has no
        /// resource item files.  Otherwise, this will return the folder containing the default and localized
        /// resource item files for things such as the title to use for syntax sections and code examples,
        /// unsupported language feature messages, etc.</para>
        /// 
        /// <para>The resource item files are like those in the presentation styles.  The root folder contains
        /// the default (English) resource item file.  Subfolders, if any, named after each supported language
        /// contain the localized version of the file.  In all cases, the file is named using the language ID
        /// plus a ".xml" extension.</para></remarks>
        string ResourceItemFileLocation { get; }

        /// <summary>
        /// This is implemented to provide a syntax generator factory
        /// </summary>
        /// <returns>A new instance of a syntax generator</returns>
        SyntaxGeneratorCore Create();
    }
}
