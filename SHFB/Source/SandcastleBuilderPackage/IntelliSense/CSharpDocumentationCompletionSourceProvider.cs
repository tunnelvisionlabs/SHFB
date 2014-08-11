﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// File    : CSharpDocumentationCompletionSourceProvider.cs
// Author  : Sam Harwell  (sam@tunnelvisionlabs.com)
// Updated : 03/21/2014
// Note    : Copyright 2014, Sam Harwell, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a provider for creating the augmented XML comments completion source
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
// ==============================================================================================================
// 03/21/2014  EFW  Added the code to the help file builder package
//===============================================================================================================

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Utilities;
using SandcastleBuilder.Package.IntelliSense.RoslynHacks;

namespace SandcastleBuilder.Package.IntelliSense
{
    /// <summary>
    /// This implementation of <see cref="ICompletionSourceProvider"/> is responsible for creating the
    /// <see cref="CSharpDocumentationCompletionSource"/> completion source to augment IntelliSense suggestion
    /// lists within C# XML documentation comments.
    /// </summary>
    /// <remarks>The <c>[Order(After = "default")]</c> metadata ensures that the C# language service has already
    /// computed its IntelliSense suggestions before the <see cref="CSharpDocumentationCompletionSource"/> is
    /// checked.  This allows the documentation completion source to <em>augment</em> the existing
    /// <see cref="CompletionSet"/> rather than provide a new <see cref="CompletionSet"/> which would be
    /// displayed in a separate tab in the completion dropdown UI.</remarks>
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("CSharp")]
    [Order(After = "default")]
    [Name("Sandcastle XML Comments Completion Source Provider")]
    internal sealed class CSharpDocumentationCompletionSourceProvider : ICompletionSourceProvider
    {
        /// <summary>
        /// Gets the <see cref="IGlyphService"/> instance which provides standard icons for
        /// <see cref="Completion"/> instances within the IntelliSense suggestion lists.
        /// </summary>
        [Import]
        internal IGlyphService GlyphService { get; private set; }

        /// <summary>
        /// Gets the global <see cref="SVsServiceProvider"/> provided by the IDE through MEF,
        /// which provides access to other IDE services.
        /// </summary>
        [Import]
        internal SVsServiceProvider ServiceProvider { get; private set; }

        /// <inheritdoc />
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            // disable the custom completion source if Roslyn is installed
            if (RoslynUtilities.IsRoslynInstalled(ServiceProvider) ?? false)
                return null;

            return new CSharpDocumentationCompletionSource(textBuffer, this);
        }
    }
}
