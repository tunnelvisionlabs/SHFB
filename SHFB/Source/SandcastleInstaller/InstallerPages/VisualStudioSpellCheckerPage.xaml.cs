﻿//===============================================================================================================
// System  : Sandcastle Guided Installation
// File    : VisualStudioSpellCheckerPage.cs
// Author  : Eric Woodruff
// Updated : 09/02/2013
// Compiler: Microsoft Visual C#
//
// This file contains a page containing information about the Visual Studio Spell Checker
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice and all copyright notices must remain intact in all applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.1.0.3  09/02/2013  EFW  Created the code
//===============================================================================================================

using System.Windows;
using System.Windows.Documents;

namespace Sandcastle.Installer.InstallerPages
{
    /// <summary>
    /// This page contains information about the Visual Studio Spell Checker
    /// </summary>
    public partial class VisualStudioSpellCheckerPage : BasePage
    {
        #region Properties
        //=====================================================================

        /// <inheritdoc />
        public override string PageTitle
        {
            get { return "Visual Studio Spell Checker"; }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public VisualStudioSpellCheckerPage()
        {
            InitializeComponent();

            // Handle hyperlink clicks using the default handler
            fdDocument.AddHandler(Hyperlink.ClickEvent, new RoutedEventHandler(Utility.HyperlinkClick));
        }
        #endregion
    }
}
