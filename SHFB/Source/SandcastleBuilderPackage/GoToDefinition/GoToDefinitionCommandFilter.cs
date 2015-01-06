using System;
using System.Collections.Generic;
using System.Globalization;

using Microsoft.VisualStudio;
using Microsoft.VisualStudio.OLE.Interop;
using Microsoft.VisualStudio.Shell.Interop;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using SandcastleBuilder.Package.IntelliSense.RoslynHacks;

namespace SandcastleBuilder.Package.GoToDefinition
{
    internal sealed class GoToDefinitionCommandFilter : TextViewCommandFilter
    {
        private readonly ITextView _textView;
        private readonly GoToDefinitionTextViewCreationListener _provider;

        public GoToDefinitionCommandFilter(IVsTextView textViewAdapter, ITextView textView, GoToDefinitionTextViewCreationListener provider)
            : base(textViewAdapter)
        {
            _textView = textView;
            _provider = provider;
        }

        protected override bool HandlePreExec(ref Guid commandGroup, uint commandId, OLECMDEXECOPT executionOptions, IntPtr pvaIn, IntPtr pvaOut)
        {
            if (commandGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch ((VSConstants.VSStd97CmdID)commandId)
                {
                case VSConstants.VSStd97CmdID.GotoDefn:
                    return TryGoToDefinition();

                default:
                    break;
                }
            }

            return base.HandlePreExec(ref commandGroup, commandId, executionOptions, pvaIn, pvaOut);
        }

        protected override OLECMDF QueryCommandStatus(ref Guid commandGroup, uint commandId)
        {
            if (commandGroup == typeof(VSConstants.VSStd97CmdID).GUID)
            {
                switch ((VSConstants.VSStd97CmdID)commandId)
                {
                case VSConstants.VSStd97CmdID.GotoDefn:
                    return OLECMDF.OLECMDF_SUPPORTED | OLECMDF.OLECMDF_ENABLED;

                default:
                    break;
                }
            }

            return base.QueryCommandStatus(ref commandGroup, commandId);
        }

        private bool TryGoToDefinition()
        {
            string definitionType;
            SnapshotSpan? currentUnderlineSpan = GetCurrentUnderlineSpan(out definitionType);
            if (!currentUnderlineSpan.HasValue)
                return false;

            string id = currentUnderlineSpan.Value.GetText().Trim();
            this.GoToDefinition(id, definitionType);
            return true;
        }

        private SnapshotSpan? GetCurrentUnderlineSpan(out string underlineSpanType)
        {
            SnapshotPoint caretPosition = _textView.Caret.Position.BufferPosition;
            ITextBuffer caretBuffer = caretPosition.Snapshot.TextBuffer;
            ITextStructureNavigator navigator = _provider.TextStructureNavigatorSelectorService.GetTextStructureNavigator(caretBuffer);
            TextExtent extent = navigator.GetExtentOfWord(caretPosition);
            if (!extent.IsSignificant)
            {
                underlineSpanType = null;
                return null;
            }

            ITextSnapshotLine line = caretPosition.GetContainingLine();
            IClassifier aggregator = _provider.ClassifierAggregatorService.GetClassifier(caretBuffer);
            return ProcessSpans(caretPosition, aggregator.GetClassificationSpans(new SnapshotSpan(line.Start, line.End)), out underlineSpanType);
        }

        private SnapshotSpan? ProcessSpans(SnapshotPoint caretPosition, IList<ClassificationSpan> spans, out string definitionType)
        {
            string elementName = null, attrName = null, spanText;

            foreach(var classification in spans)
            {
                var name = classification.ClassificationType.Classification.ToLowerInvariant();

                // Highlight the span if it matches what we are looking for and it contains the mouse span
                switch(name)
                {
                    case "xml name":
                        elementName = classification.Span.GetText();
                        break;

                    case "xml attribute":
                        attrName = classification.Span.GetText();
                        break;

                    case "xml attribute value":
                        if(classification.Span.Contains(caretPosition))
                        {
                            if((elementName == "image" || elementName == "link") && attrName == "xlink:href")
                            {
                                definitionType = elementName;
                                return classification.Span;
                            }

                            definitionType = null;
                            return null;
                        }
                        break;

                    case "xml text":
                        if(classification.Span.Contains(caretPosition))
                        {
                            spanText = classification.Span.GetText().Trim();

                            switch(elementName)
                            {
                                case "codeEntityReference":
                                    if(spanText.IsCodeEntityReference())
                                    {
                                        definitionType = elementName;
                                        return classification.Span;
                                    }
                                    break;

                                case "codeReference":
                                    if(spanText.IsCodeReferenceId())
                                    {
                                        definitionType = elementName;
                                        return classification.Span;
                                    }
                                    break;

                                case "token":
                                    definitionType = elementName;
                                    return classification.Span;

                                default:
                                    // We only get info about the current line so we may just get some XML text
                                    // if the starting tag is on a prior line.  In such cases, see if the text
                                    // looks like an entity reference or a code reference ID.  If so, offer it as
                                    // a clickable link.  If not, ignore it.
                                    if(String.IsNullOrWhiteSpace(elementName))
                                    {
                                        // Ignore any leading whitespace on the span so that it only underlines
                                        // the text.
                                        string highlightText = classification.Span.GetText();

                                        int offset = highlightText.Length - highlightText.TrimStart().Length;
                                        var textSpan = new SnapshotSpan(classification.Span.Snapshot,
                                            classification.Span.Start.Position + offset,
                                            classification.Span.Length - offset);

                                        if(spanText.IsCodeEntityReference())
                                        {
                                            definitionType = "codeEntityReference";
                                            return textSpan;
                                        }
                                        else
                                            if(spanText.IsCodeReferenceId())
                                            {
                                                definitionType = "codeReference";
                                                return textSpan;
                                            }
                                    }
                                    break;
                            }

                            definitionType = null;
                            return null;
                        }
                        break;

                    default:
                        break;
                }
            }

            definitionType = null;
            return null;
        }

        private void GoToDefinition(string id, string definitionType)
        {
            switch(definitionType)
            {
                case "codeEntityReference":
                    var entitySearcher = new CodeEntitySearcher(_provider.ServiceProvider);

                    if(!entitySearcher.GotoDefinitionFor(id))
                    {
                        Guid clsid = Guid.Empty;
                        int result;
                        var uiShell = _provider.ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

                        if(uiShell != null)
                            uiShell.ShowMessageBox(0, ref clsid, "Unable to navigate to code entity reference " +
                                "definition.", String.Format(CultureInfo.CurrentCulture, "Member ID: {0}\r\n\r\n" +
                                "If valid, the most likely cause is that it is not a member of a C# project " +
                                "within the current solution.  Navigating to members in non-C# projects and " +
                                ".NET Framework or reference assemblies is not supported.", id), String.Empty, 0,
                                OLEMSGBUTTON.OLEMSGBUTTON_OK, OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST,
                                OLEMSGICON.OLEMSGICON_INFO, 0, out result);

                        System.Diagnostics.Debug.WriteLine("Unable to go to declaration for member ID: " + id);
                    }
                    break;

                case "codeReference":
                case "image":
                case "link":
                case "token":
                    var projectFileSearcher = new ProjectFileSearcher(_provider.ServiceProvider, _textView);

                    ProjectFileSearcher.IdType idType;

                    if(!Enum.TryParse(definitionType, true, out idType))
                        idType = ProjectFileSearcher.IdType.Unknown;

                    if(!projectFileSearcher.OpenFileFor(idType, id))
                    {
                        Guid clsid = Guid.Empty;
                        int result;
                        var uiShell = _provider.ServiceProvider.GetService(typeof(SVsUIShell)) as IVsUIShell;

                        if(uiShell != null)
                            uiShell.ShowMessageBox(0, ref clsid, "Unable to open file for element target.",
                                String.Format(CultureInfo.CurrentCulture, "Type: {0}\r\nID: {1}\r\n\r\nIf " +
                                "valid, it may not be a part of a help file builder project within this " +
                                "solution.", definitionType, id), String.Empty, 0, OLEMSGBUTTON.OLEMSGBUTTON_OK,
                                OLEMSGDEFBUTTON.OLEMSGDEFBUTTON_FIRST, OLEMSGICON.OLEMSGICON_INFO, 0, out result);

                        System.Diagnostics.Debug.WriteLine("Unable to go to open file for ID '{0}' ({1}): ", id,
                            definitionType);
                    }
                    break;

                default:
                    break;
            }
        }
    }
}
