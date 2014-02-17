//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// Author  : Sam Harwell  (sam@tunnelvisionlabs.com)
// Note    : Copyright 2014, Sam Harwell, All rights reserved
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//===============================================================================================================

namespace SandcastleBuilder.Package.IntelliSense
{
    using System.ComponentModel.Composition;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Utilities;
    using ICompletionSource = Microsoft.VisualStudio.Language.Intellisense.ICompletionSource;
    using ICompletionSourceProvider = Microsoft.VisualStudio.Language.Intellisense.ICompletionSourceProvider;
    using ITextBuffer = Microsoft.VisualStudio.Text.ITextBuffer;

    /// <summary>
    /// This implementation of <see cref="ICompletionSourceProvider"/> is responsible for creating
    /// the <see cref="CSharpDocumentationCompletionSource"/> completion source to augment
    /// IntelliSense suggestion lists within C# XML documentation comments.
    /// </summary>
    /// <remarks>
    /// The <c>[Order(After = "default")]</c> metadata ensures that the C# language service has
    /// already computed its IntelliSense suggestions before the
    /// <see cref="CSharpDocumentationCompletionSource"/> is checked. This allows the documentation
    /// completion source to <em>augment</em> the existing <see cref="CompletionSet"/> rather than
    /// provide a new <see cref="CompletionSet"/> which would be displayed in a separate tab in
    /// the completion dropdown UI.
    /// </remarks>
    [Export(typeof(ICompletionSourceProvider))]
    [ContentType("CSharp")]
    [Order(After = "default")]
    [Name("DocumentationCompletionSourceProvider")]
    internal sealed class CSharpDocumentationCompletionSourceProvider : ICompletionSourceProvider
    {
        /// <summary>
        /// Gets the <see cref="IGlyphService"/> instance which provides standard icons for
        /// <see cref="Completion"/> instances within the IntelliSense suggestion lists.
        /// </summary>
        [Import]
        internal IGlyphService GlyphService
        {
            get;
            private set;
        }

        /// <inheritdoc/>
        public ICompletionSource TryCreateCompletionSource(ITextBuffer textBuffer)
        {
            return new CSharpDocumentationCompletionSource(textBuffer, this);
        }
    }
}
