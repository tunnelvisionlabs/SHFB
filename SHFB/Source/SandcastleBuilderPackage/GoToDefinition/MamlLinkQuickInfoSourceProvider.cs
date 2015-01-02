﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// File    : MamlLinkQuickInfoSourceProvider.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/08/2014
// Note    : Copyright 2014, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains the class that creates the quick info source specific to MAML elements
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
//===============================================================================================================
// 12/01/2014  EFW  Created the code
//===============================================================================================================

using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Language.Intellisense;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Text;
using Microsoft.VisualStudio.Text.Tagging;
using Microsoft.VisualStudio.Utilities;

namespace SandcastleBuilder.Package.GoToDefinition
{
    /// <summary>
    /// This class creates the quick info source specific to MAML elements
    /// </summary>
    [Export(typeof(IQuickInfoSourceProvider))]
    [Name("MAML Link Quick Info Provider")]
    [Order(Before = "Default Quick Info Presenter")]
    [ContentType("xml")]
    internal sealed class MamlLinkQuickInfoSourceProvider : IQuickInfoSourceProvider
    {
        [Import]
        private SVsServiceProvider GlobalServiceProvider = null;

        [Import]
        internal IViewTagAggregatorFactoryService AggregatorFactory { get; set; }

        /// <inheritdoc />
        public IQuickInfoSource TryCreateQuickInfoSource(ITextBuffer textBuffer)
        {
            if(!MefProviderOptions.EnableGoToDefinition)
                return null;

            return new MamlLinkQuickInfoSource(GlobalServiceProvider, textBuffer, this);
        }
    }
}
