//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// File    : MefProviderOptions.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/15/2014
// Note    : Copyright 2014, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains the class used to contain the MEF provider configuration settings
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
//===============================================================================================================
// 12/08/2014  EFW  Created the code
//===============================================================================================================

using System;
using System.ComponentModel.Composition;

using Microsoft.VisualStudio.Settings;
using Microsoft.VisualStudio.Shell;
using Microsoft.VisualStudio.Shell.Settings;

namespace SandcastleBuilder.Package
{
    /// <summary>
    /// This class is used to contain the MEF provider configuration options
    /// </summary>
    /// <remarks>Settings are stored in the Visual Studio settings for the current user.  These are separate from the
    /// main package options but are editable using the package options page.  Since these are not directly related to
    /// the package, we don't want to force it to load just to access these few settings.</remarks>
    [Export]
    internal sealed class MefProviderOptions
    {
        private readonly SVsServiceProvider _serviceProvider;

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the settings collection path
        /// </summary>
        private string CollectionPath
        {
            get
            {
                return @"SHFB\MEF Provider\IntelliSense Features";
            }
        }

        /// <summary>
        /// This is used to get or set whether or not the extended XML comments completion source options are
        /// enabled.
        /// </summary>
        /// <value>This is true by default</value>
        public bool EnableExtendedXmlCommentsCompletion{ get; set; }

        /// <summary>
        /// This is used to get or set whether or not the MAML and XML comments element Go To Definition and tool
        /// tip option is enabled.
        /// </summary>
        /// <value>This is true by default</value>
        public bool EnableGoToDefinition { get; set; }

        /// <summary>
        /// Related to the above, if enabled, any XML comments <c>cref</c> attribute value will allow Go To
        /// Definition and tool tip info.
        /// </summary>
        /// <value>True by default, except in Visual Studio 2015 (which provides tool tip and
        /// Go To Definition support for <c>cref</c> attribute values already).</value>
        public bool EnableGoToDefinitionInCRef { get; set; }

        /// <summary>
        /// Gets the default value for the <see cref="EnableGoToDefinitionInCRef"/> setting for the current version of
        /// Visual Studio.
        /// </summary>
        /// <value>
        /// <para><see langword="true"/> if Roslyn is installed (which supports this feature natively).</para>
        /// <para>-or-</para>
        /// <para><see langword="false"/> if Roslyn is not installed, so this extension must provide the feature.</para>
        /// </value>
        private bool DefaultEnableGoToDefinitionInCRef
        {
            get
            {
                if (IntelliSense.RoslynHacks.RoslynUtilities.IsRoslynInstalled(_serviceProvider) ?? false)
                    return false;

                return true;
            }
        }

        #endregion

        #region Constructor
        //=====================================================================

        [ImportingConstructor]
        public MefProviderOptions(SVsServiceProvider serviceProvider)
        {
            if (serviceProvider == null)
                throw new ArgumentNullException("serviceProvider");

            _serviceProvider = serviceProvider;

            if(!LoadConfiguration())
                ResetConfiguration(false);
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// This is used to load the MEF provider configuration settings
        /// </summary>
        /// <returns>True if loaded successfully or false if the settings collection does not exist</returns>
        /// <remarks>The settings are loaded using the <see cref="ShellSettingsManager"/> from the
        /// <see cref="CollectionPath"/> collection.</remarks>
        private bool LoadConfiguration()
        {
            ShellSettingsManager settingsManager = new ShellSettingsManager(_serviceProvider);
            SettingsStore settingsStore = settingsManager.GetReadOnlySettingsStore(SettingsScope.UserSettings);
            if (!settingsStore.CollectionExists(CollectionPath))
                return false;

            EnableExtendedXmlCommentsCompletion = settingsStore.GetBoolean(CollectionPath, "EnableExtendedXmlCommentsCompletion", true);
            EnableGoToDefinition = settingsStore.GetBoolean(CollectionPath, "EnableGoToDefinition", true);
            EnableGoToDefinitionInCRef = settingsStore.GetBoolean(CollectionPath, "EnableGoToDefinitionInCRef", DefaultEnableGoToDefinitionInCRef);
            return true;
        }

        /// <summary>
        /// This is used to save the MEF provider configuration settings
        /// </summary>
        /// <remarks>The settings are saved using the <see cref="ShellSettingsManager"/> to the
        /// <see cref="CollectionPath"/> collection.</remarks>
        public bool SaveConfiguration()
        {
            ShellSettingsManager settingsManager = new ShellSettingsManager(_serviceProvider);
            WritableSettingsStore settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (!settingsStore.CollectionExists(CollectionPath))
                settingsStore.CreateCollection(CollectionPath);

            settingsStore.SetBoolean(CollectionPath, "EnableExtendedXmlCommentsCompletion", EnableExtendedXmlCommentsCompletion);
            settingsStore.SetBoolean(CollectionPath, "EnableGoToDefinition", EnableGoToDefinition);
            settingsStore.SetBoolean(CollectionPath, "EnableGoToDefinitionInCRef", EnableGoToDefinitionInCRef);
            return true;
        }

        /// <summary>
        /// This is used to reset the configuration to its default state
        /// </summary>
        /// <param name="deleteConfigurationFile">True to delete the configuration settings collection if it exists,
        /// false to just set the default values</param>
        public void ResetConfiguration(bool deleteConfigurationFile)
        {
            EnableExtendedXmlCommentsCompletion = EnableGoToDefinition = true;
            EnableGoToDefinitionInCRef = DefaultEnableGoToDefinitionInCRef;

            ShellSettingsManager settingsManager = new ShellSettingsManager(_serviceProvider);
            WritableSettingsStore settingsStore = settingsManager.GetWritableSettingsStore(SettingsScope.UserSettings);
            if (deleteConfigurationFile && settingsStore.CollectionExists(CollectionPath))
                settingsStore.DeleteCollection(CollectionPath);
        }
        #endregion
    }
}
