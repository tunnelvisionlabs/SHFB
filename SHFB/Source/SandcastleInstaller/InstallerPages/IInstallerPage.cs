﻿//===============================================================================================================
// System  : Sandcastle Guided Installation
// File    : IInstallerPage.cs
// Author  : Eric Woodruff
// Updated : 12/28/2013
// Compiler: Microsoft Visual C#
//
// This file contains an interface definition used to implement an installer page
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice and all copyright notices must remain intact in all applications, documentation, and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.0.0.0  02/05/2011  EFW  Created the code
// 1.1.0.0  03/05/2012  EFW  Converted to use WPF
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.Windows.Controls;
using System.Xml.Linq;

namespace Sandcastle.Installer.InstallerPages
{
    /// <summary>
    /// This interface defines the methods required to implement an installer page
    /// </summary>
    public interface IInstallerPage
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This is used to get or set a reference to the installer that owns the page
        /// </summary>
        IInstaller Installer { get; set; }

        /// <summary>
        /// This read-only property returns the page title
        /// </summary>
        string PageTitle { get; }

        /// <summary>
        /// This is used to get or set the Sandcastle release version
        /// </summary>
        /// <remarks>The installer will set this when it loads the page</remarks>
        string SandcastleVersion { get; set; }

        /// <summary>
        /// This read-only property is used to determine if the user can continue on from this step to the next
        /// step in the installation process.
        /// </summary>
        bool CanContinue { get; }

        /// <summary>
        /// This read-only property returns a reference to the control
        /// </summary>
        Control Control { get; }

        /// <summary>
        /// This read-only property returns the required .NET Framework version required by the tool installed by
        /// the page.
        /// </summary>
        /// <remarks>Return the minimum .NET Framework version required or null if no version is required.</remarks>
        Version RequiredFrameworkVersion { get; }

        /// <summary>
        /// This read-only property is used to get an enumerable list of <see cref="CompletionAction"/> instances
        /// that should be offered when the guided installation has completed.
        /// </summary>
        IEnumerable<CompletionAction> CompletionActions { get ; }

        /// <summary>
        /// This read-only property is used to find out if a reboot should be suggested after the guided
        /// installation completes.
        /// </summary>
        bool SuggestReboot { get; }
        #endregion

        #region Methods
        //=====================================================================

        /// <summary>
        /// This method is used to initialize the page and set any necessary configuration options
        /// </summary>
        /// <param name="configuration">The page's configuration element from the configuration file</param>
        void Initialize(XElement configuration);

        /// <summary>
        /// This method is used to handle tasks that should occur when the page is shown
        /// </summary>
        void ShowPage();

        /// <summary>
        /// This method is used to handle tasks that should occur when the page is hidden
        /// </summary>
        void HidePage();
        #endregion
    }
}
