using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Editor;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text.Classification;
using Microsoft.VisualStudio.Text.Editor;
using Microsoft.VisualStudio.Text.Operations;
using Microsoft.VisualStudio.TextManager.Interop;
using Microsoft.VisualStudio.Utilities;

namespace SandcastleBuilder.Package.GoToDefinition
{
    [Export(typeof(IVsTextViewCreationListener))]
    [ContentType("Roslyn Languages")]
    [ContentType("csharp")]
    [ContentType("xml")]
    [TextViewRole(PredefinedTextViewRoles.Document)]
    internal sealed class GoToDefinitionTextViewCreationListener : IVsTextViewCreationListener
    {
        private readonly IVsEditorAdaptersFactoryService _editorAdaptersFactoryService;

        [ImportingConstructor]
        public GoToDefinitionTextViewCreationListener(SVsServiceProvider serviceProvider, IVsEditorAdaptersFactoryService editorAdaptersFactoryService, ITextStructureNavigatorSelectorService textStructureNavigatorSelectorService, IClassifierAggregatorService classifierAggregatorService, MefProviderOptions mefProviderOptions)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");
            if (editorAdaptersFactoryService == null)
                throw new ArgumentNullException("editorAdaptersFactoryService");
            if (textStructureNavigatorSelectorService == null)
                throw new ArgumentNullException("textStructureNavigatorSelectorService");
            if (classifierAggregatorService == null)
                throw new ArgumentNullException("classifierAggregatorService");
            if (mefProviderOptions == null)
                throw new ArgumentNullException("mefProviderOptions");

            ServiceProvider = serviceProvider;
            _editorAdaptersFactoryService = editorAdaptersFactoryService;
            TextStructureNavigatorSelectorService = textStructureNavigatorSelectorService;
            ClassifierAggregatorService = classifierAggregatorService;
            MefProviderOptions = mefProviderOptions;
        }

        public SVsServiceProvider ServiceProvider
        {
            get;
            private set;
        }

        public ITextStructureNavigatorSelectorService TextStructureNavigatorSelectorService
        {
            get;
            private set;
        }

        public IClassifierAggregatorService ClassifierAggregatorService
        {
            get;
            private set;
        }

        public MefProviderOptions MefProviderOptions
        {
            get;
            private set;
        }

        public void VsTextViewCreated(IVsTextView textViewAdapter)
        {
            // only hook when necessary
            if (!MefProviderOptions.EnableGoToDefinition)
                return;

            ITextView textView = _editorAdaptersFactoryService.GetWpfTextView(textViewAdapter);
            if (textView == null)
                return;

            var filter = new GoToDefinitionCommandFilter(textViewAdapter, textView, this);
            filter.Enabled = true;
            textView.Properties.AddProperty(typeof(GoToDefinitionCommandFilter), filter);
        }
    }
}
