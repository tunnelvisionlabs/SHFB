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
    using System;
    using System.Collections.Generic;
    using System.Linq;
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;

    /// <summary>
    /// This completion source augments the completion set returned by the C# language
    /// service within XML documentation comments to include custom tags which are implemented
    /// within the latest release of Sandcastle.
    /// </summary>
    internal sealed class CSharpDocumentationCompletionSource : ICompletionSource
    {
        /// <summary>
        /// The text buffer associated with this completion source.
        /// </summary>
        private readonly ITextBuffer _textBuffer;

        /// <summary>
        /// The completion source provider which created this instance.
        /// </summary>
        private readonly CSharpDocumentationCompletionSourceProvider _provider;

        /// <summary>
        /// Initializes a new instance of the <see cref="CSharpDocumentationCompletionSource"/> class
        /// for the specified text buffer and provider.
        /// </summary>
        /// <param name="textBuffer">The text buffer associated with this completion source.</param>
        /// <param name="provider">The completion source provider.</param>
        /// <exception cref="ArgumentNullException">
        /// If <paramref name="textBuffer"/> is <see langword="null"/>.
        /// <para>-or-</para>
        /// <para>If <paramref name="provider"/> is <see langword="null"/>.</para>
        /// </exception>
        public CSharpDocumentationCompletionSource(ITextBuffer textBuffer, CSharpDocumentationCompletionSourceProvider provider)
        {
            if (textBuffer == null)
                throw new ArgumentNullException("textBuffer");
            if (provider == null)
                throw new ArgumentNullException("provider");

            _textBuffer = textBuffer;
            _provider = provider;
        }

        /// <inheritdoc/>
        public void AugmentCompletionSession(ICompletionSession session, IList<CompletionSet> completionSets)
        {
            /* The IntelliSense calculation within comments is rather simple. To detect when the
             * completion is currently located within an XML documentation comment, the following
             * "tricks" are used.
             * 
             * 1. The filtering algorithm in use never removes "!--" (indicating an XML comment)
             *    from the completion set, so it is always listed when completion is invoked. In
             *    addition, the sorting algorithm always places this item first in the completion
             *    set.
             * 2. Since this particular tag is not valid elsewhere in C#, it is easily use to
             *    determine when the caret is inside an XML documentation comment.
             */

            if (completionSets.Count == 0
                || completionSets[0].Completions.Count == 0
                || completionSets[0].Completions[0].DisplayText != "!--")
            {
                // not inside a documentation comment, so leave things alone
                return;
            }

            /* Next, we need to determine if the user pressed "<" or simply pressed Ctrl+Space to
             * invoke code completion. Since the C# IntelliSense provider doesn't include any
             * existing "<" character in the reported ApplicableTo tracking span, we must insert
             * this character when it's not already present. XML itself makes this easy - the "<"
             * character will either appear immediately before the ApplicableTo span or it will
             * not be present.
             */

            ITextSnapshot snapshot = _textBuffer.CurrentSnapshot;
            SnapshotPoint startPoint = completionSets[0].ApplicableTo.GetStartPoint(snapshot);

            string prefix = string.Empty;
            if (startPoint > 0 && snapshot.GetText(startPoint.Position - 1, 1) != "<")
                prefix = "<";

            /* Use the GlyphKeyword glyph for "normal" XML tags, to match the glyphs used by C#
             * for the standard IntelliSense tags. Use the GlyphGroupMacro glyph for other completion
             * items that expand to something other that what the user wrote (e.g. "true" expands to
             * <see langword="true"/> as opposed to <true/>).
             *
             * The descriptions for custom tags is copied from the the Sandcastle XML Comments Guide.
             * Obsolete custom tags are not included.
             */

            var iconSource = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphKeyword, StandardGlyphItem.GlyphItemPublic);
            var macroIconSource = _provider.GlyphService.GetGlyph(StandardGlyphGroup.GlyphGroupMacro, StandardGlyphItem.GlyphItemPublic);
            Completion[] completions =
                {
                    // custom tags implemented by sandcastle
                    new CustomCompletion(session, "event", prefix + "event cref=\"|\">", "This element is used to list events that can be raised by a type's member.", iconSource, ""),
                    new Completion("preliminary", prefix + "preliminary/>", "This element is used to indicate that a particular type or member is preliminary and is subject to change.", iconSource, ""),
                    new Completion("threadsafety", prefix + "threadsafety static=\"true\" instance=\"false\"/>", "This element is used to indicate whether or not a class or structure's static and instance members are safe for use in multi-threaded scenarios.", iconSource, ""),
                    new Completion("note", prefix + "note type=\"note\">", "This element is used to create a note-like section within a topic to draw attention to some important information.", iconSource, ""),
                    new Completion("AttachedEventComments", prefix + "AttachedEventComments>", "This element is used to define the content that should appear on the auto-generated attached event member topic for a given WPF routed event member.", iconSource, ""),
                    new Completion("AttachedPropertyComments", prefix + "AttachedPropertyComments>", "This element is used to define the content that should appear on the auto-generated attached property member topic for a given WPF dependency property member.", iconSource, ""),
                    new CustomCompletion(session, "conceptualLink", prefix + "conceptualLink target=\"|\"/>", "This element is used to create a link to a MAML topic within the See Also section of a topic or an inline link to a MAML topic within one of the other XML comments elements.", iconSource, ""),
                    new Completion("inheritdoc", prefix + "inheritdoc/>", "This element can help minimize the effort required to document complex APIs by allowing common documentation to be inherited from base types/members.", iconSource, ""),
                    new Completion("overloads", prefix + "overloads>", "This element is used to define the content that should appear on the auto-generated overloads topic for a given set of member overloads.", iconSource, ""),
                    new Completion("token", prefix + "token>", "This element represents a replaceable tag within a topic.", iconSource, ""),

                    // language-specific keywords
                    new Completion("null", prefix + "see langword=\"null\"/>", "Inserts the language-specific keyword 'null'.", macroIconSource, ""),
                    new Completion("static", prefix + "see langword=\"static\"/>", "Inserts the language-specific keyword 'static'.", macroIconSource, ""),
                    new Completion("virtual", prefix + "see langword=\"virtual\"/>", "Inserts the language-specific keyword 'virtual'.", macroIconSource, ""),
                    new Completion("true", prefix + "see langword=\"true\"/>", "Inserts the language-specific keyword 'true'.", macroIconSource, ""),
                    new Completion("false", prefix + "see langword=\"false\"/>", "Inserts the language-specific keyword 'false'.", macroIconSource, ""),
                    new Completion("abstract", prefix + "see langword=\"abstract\"/>", "Inserts the language-specific keyword 'abstract'.", macroIconSource, ""),
                    new Completion("async", prefix + "see langword=\"async\"/>", "Inserts the language-specific keyword 'async'.", macroIconSource, ""),
                    new Completion("await", prefix + "see langword=\"await\"/>", "Inserts the language-specific keyword 'await'.", macroIconSource, ""),
                    new Completion("async/await", prefix + "see langword=\"async/await\"/>", "Inserts the language-specific keyword 'async/await'.", macroIconSource, ""),
                };

            /* The augmented completion set is created from the previously existing one (created by the C#
             * language service), and a CompletionSet created for the custom XML tags. The moniker and
             * display name for the additional set are not used, since the augmented set always returns
             * the values reported by the original completion set.
             */

            CompletionSet additionalCompletionSet = new CompletionSet("", "", completionSets[0].ApplicableTo, completions, Enumerable.Empty<Completion>());
            completionSets[0] = new AugmentedCompletionSet(completionSets[0], additionalCompletionSet);
        }

        /// <inheritdoc/>
        public void Dispose()
        {
            // nothing to do for this sealed class
        }
    }
}
