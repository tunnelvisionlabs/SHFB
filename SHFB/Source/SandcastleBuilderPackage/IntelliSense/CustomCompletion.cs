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
    using Microsoft.VisualStudio.Language.Intellisense;
    using Microsoft.VisualStudio.Text;
    using ImageSource = System.Windows.Media.ImageSource;

    /// <summary>
    /// Represents a completion item, including the icon, insertion text, and display text, in a CompletionSet.
    /// </summary>
    /// <remarks>
    /// This class extends the <see cref="Completion"/> class by allowing the <see cref="Completion.InsertionText"/>
    /// to contain a pipe character (<c>|</c>) to represent the placement of the caret after the completion is
    /// inserted into the editor.
    /// </remarks>
    internal sealed class CustomCompletion : Completion, ICustomCommit
    {
        private readonly ICompletionSession _session;

        /// <summary>
        /// Initializes a new instance of the <see cref="CustomCompletion"/> class with
        /// the specified text and description.
        /// </summary>
        /// <param name="session">The completion session.</param>
        /// <param name="displayText">The text that is to be displayed by an IntelliSense presenter.</param>
        /// <param name="insertionText">The text that is to be inserted into the buffer if this completion is committed.</param>
        /// <param name="description">A description that can be displayed with the display text of the completion.</param>
        /// <param name="iconSource">The icon.</param>
        /// <param name="iconAutomationText">The text to be used as the automation name for the icon.</param>
        /// <exception cref="ArgumentNullException">If <paramref name="session"/> is <see langword="null"/>.</exception>
        public CustomCompletion(ICompletionSession session, string displayText, string insertionText, string description, ImageSource iconSource, string iconAutomationText)
            : base(displayText, insertionText, description, iconSource, iconAutomationText)
        {
            if (session == null)
                throw new ArgumentNullException("session");

            _session = session;
        }

        /// <inheritdoc/>
        public void Commit()
        {
            if (!_session.SelectedCompletionSet.SelectionStatus.IsSelected)
                return;

            ITrackingSpan applicableTo = _session.SelectedCompletionSet.ApplicableTo;
            using (ITextEdit edit = applicableTo.TextBuffer.CreateEdit())
            {
                // the insertion text is inserted without the | character (if any)
                string insertionText = InsertionText.Replace("|", "");
                edit.Replace(applicableTo.GetSpan(edit.Snapshot), insertionText);
                ITextSnapshot applied = edit.Apply();

                // the original position of the | character determines the placement of the caret
                int pipeOffset = InsertionText.IndexOf('|');
                if (pipeOffset >= 0)
                {
                    SnapshotPoint startPoint = applicableTo.GetStartPoint(applied);
                    SnapshotPoint caretPoint = startPoint + pipeOffset;
                    _session.TextView.Caret.MoveTo(caretPoint, PositionAffinity.Predecessor);
                }
            }
        }
    }
}
