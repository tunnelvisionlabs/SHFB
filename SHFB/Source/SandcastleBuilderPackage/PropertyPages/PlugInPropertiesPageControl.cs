﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Visual Studio Package
// File    : PlugInPropertiesPageControl.cs
// Author  : Eric Woodruff
// Updated : 03/07/2013
// Note    : Copyright 2011-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This user control is used to edit the Plug-In category properties
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.9.3.0  03/27/2011  EFW  Created the code
// 1.9.6.0  10/28/2012  EFW  Updated for use in the standalone GUI
//===============================================================================================================

using System;
using System.Globalization;
using System.Reflection;
using System.Runtime.InteropServices;
using System.Windows.Forms;

using Microsoft.Build.Evaluation;

#if !STANDALONEGUI
using SandcastleBuilder.Package.Nodes;
using SandcastleBuilder.Package.Properties;
#endif
using SandcastleBuilder.Utils.PlugIn;

namespace SandcastleBuilder.Package.PropertyPages
{
    /// <summary>
    /// This is used to edit the Plug-In category project properties
    /// </summary>
    [Guid("8FB53BCE-82A8-4207-9DB6-7D30696C780C")]
    public partial class PlugInPropertiesPageControl : BasePropertyPage
    {
        #region Private data members
        //=====================================================================

        private PlugInConfigurationDictionary currentConfigs;
        private string messageBoxTitle;
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        public PlugInPropertiesPageControl()
        {
            InitializeComponent();

#if !STANDALONEGUI
            messageBoxTitle = Resources.PackageTitle;
#else
            messageBoxTitle = SandcastleBuilder.Utils.Constants.AppName;
#endif
            this.Title = "Plug-Ins";
            this.HelpKeyword = "be2b5b09-cf5f-4fc3-be8c-f6d8a27c3691";

            try
            {
                foreach(string key in PlugInManager.PlugIns.Keys)
                    lbAvailablePlugIns.Items.Add(key);
            }
            catch(ReflectionTypeLoadException loadEx)
            {
                System.Diagnostics.Debug.WriteLine(loadEx.ToString());
                System.Diagnostics.Debug.WriteLine(loadEx.LoaderExceptions[0].ToString());

                MessageBox.Show("Unexpected error loading plug-ins: " + loadEx.LoaderExceptions[0].Message,
                    messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());

                MessageBox.Show("Unexpected error loading plug-ins: " + ex.Message, messageBoxTitle,
                    MessageBoxButtons.OK, MessageBoxIcon.Error);
            }

            if(lbAvailablePlugIns.Items.Count != 0)
                lbAvailablePlugIns.SelectedIndex = 0;
            else
            {
                MessageBox.Show("No valid plug-ins found", messageBoxTitle, MessageBoxButtons.OK,
                    MessageBoxIcon.Information);
                gbAvailablePlugIns.Enabled = gbProjectAddIns.Enabled = false;
            }
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        protected override bool BindControlValue(Control control)
        {
            ProjectProperty projProp;
            int idx;

            lbProjectPlugIns .Items.Clear();

#if !STANDALONEGUI
            if(this.ProjectMgr == null)
                return false;

            currentConfigs = new PlugInConfigurationDictionary(
                ((SandcastleBuilderProjectNode)base.ProjectMgr).SandcastleProject);
            projProp = this.ProjectMgr.BuildProject.GetProperty("PlugInConfigurations");
#else
            if(this.CurrentProject == null)
                return false;

            currentConfigs = new PlugInConfigurationDictionary(this.CurrentProject);
            projProp = this.CurrentProject.MSBuildProject.GetProperty("PlugInConfigurations");
#endif
            if(projProp != null && !String.IsNullOrEmpty(projProp.UnevaluatedValue))
                currentConfigs.FromXml(projProp.UnevaluatedValue);

            foreach(string key in currentConfigs.Keys)
            {
                idx = lbProjectPlugIns.Items.Add(key);
                lbProjectPlugIns.SetItemChecked(idx, currentConfigs[key].Enabled);
            }

            if(lbProjectPlugIns.Items.Count != 0)
                lbProjectPlugIns.SelectedIndex = 0;
            else
                btnConfigure.Enabled = btnDelete.Enabled = false;

            return true;
        }

        /// <inheritdoc />
        protected override bool StoreControlValue(Control control)
        {
#if !STANDALONEGUI
            if(this.ProjectMgr == null)
                return false;

            this.ProjectMgr.SetProjectProperty("PlugInConfigurations", currentConfigs.ToXml());
#else
            if(this.CurrentProject == null)
                return false;

            this.CurrentProject.MSBuildProject.SetProperty("PlugInConfigurations", currentConfigs.ToXml());
#endif
            return true;
        }
        #endregion

        #region Event handlers
        //=====================================================================

        /// <summary>
        /// Update the plug-in details when the selected index changes
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void lbAvailablePlugIns_SelectedIndexChanged(object sender, EventArgs e)
        {
            string key = (string)lbAvailablePlugIns.SelectedItem;

            PlugInInfo info = PlugInManager.PlugIns[key];
            txtPlugInCopyright.Text = info.Copyright;
            txtPlugInVersion.Text = String.Format(CultureInfo.CurrentCulture, "Version {0}", info.Version);
            txtPlugInDescription.Text = info.Description;
        }

        /// <summary>
        /// Update the enabled state of the plug-in based on its checked state
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void lbProjectPlugIns_ItemCheck(object sender, ItemCheckEventArgs e)
        {
            string key = (string)lbProjectPlugIns.Items[e.Index];
            bool newState = (e.NewValue == CheckState.Checked);

            if(currentConfigs[key].Enabled != newState)
            {
                currentConfigs[key].Enabled = newState;
                this.IsDirty = true;
            }
        }

        /// <summary>
        /// Add the selected plug-in to the project with a default configuration
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnAddPlugIn_Click(object sender, EventArgs e)
        {
            string key = (string)lbAvailablePlugIns.SelectedItem;
            int idx = lbProjectPlugIns.FindStringExact(key);

            // Currently, no duplicates are allowed
            if(idx != -1)
                lbProjectPlugIns.SelectedIndex = idx;
            else
                if(PlugInManager.IsSupported(key))
                {
                    idx = lbProjectPlugIns.Items.Add(key);

                    if(idx != -1)
                    {
                        currentConfigs.Add(key, true, null);
                        lbProjectPlugIns.SelectedIndex = idx;
                        lbProjectPlugIns.SetItemChecked(idx, true);
                        btnConfigure.Enabled = btnDelete.Enabled = true;

                        this.IsDirty = true;

                        // Open the configuration dialog to configure it when first added if needed
                        btnConfigure_Click(sender, e);
                    }
                }
                else
                    MessageBox.Show("The selected plug-in's version is not compatible with this version of the " +
                        "help file builder and cannot be used.", messageBoxTitle, MessageBoxButtons.OK,
                        MessageBoxIcon.Error);
        }

        /// <summary>
        /// Edit the selected plug-in's project configuration
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnConfigure_Click(object sender, EventArgs e)
        {
            PlugInConfiguration plugInConfig;
            string newConfig, currentConfig, key = (string)lbProjectPlugIns.SelectedItem;

            if(PlugInManager.IsSupported(key))
            {
                PlugInInfo info = PlugInManager.PlugIns[key];

                if(!info.SupportsConfiguration)
                {
                    if(sender == btnConfigure)
                        MessageBox.Show("The selected plug-in contains no editable configuration information",
                            messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Information);
                    return;
                }

                using(IPlugIn plugIn = info.NewInstance())
                {
                    plugInConfig = currentConfigs[key];
                    currentConfig = plugInConfig.Configuration;
                    newConfig = plugIn.ConfigurePlugIn(currentConfigs.ProjectFile, currentConfig);
                }

                // Only store it if new or if it changed
                if(currentConfig != newConfig)
                {
                    plugInConfig.Configuration = newConfig;
                    this.IsDirty = true;
                }
            }
            else
                MessageBox.Show("The selected plug-in either does not exist or is of a version that is not " +
                    "compatible with this version of the help file builder and cannot be used.",
                    messageBoxTitle, MessageBoxButtons.OK, MessageBoxIcon.Error);
        }

        /// <summary>
        /// Delete the selected plug-in from the project
        /// </summary>
        /// <param name="sender">The sender of the event</param>
        /// <param name="e">The event arguments</param>
        private void btnDelete_Click(object sender, EventArgs e)
        {
            string key = (string)lbProjectPlugIns.SelectedItem;
            int idx = lbProjectPlugIns.SelectedIndex;

            if(currentConfigs.ContainsKey(key))
            {
                currentConfigs.Remove(key);
                this.IsDirty = true;

                lbProjectPlugIns.Items.RemoveAt(idx);

                if(lbProjectPlugIns.Items.Count == 0)
                    btnConfigure.Enabled = btnDelete.Enabled = false;
                else
                    if(idx < lbProjectPlugIns.Items.Count)
                        lbProjectPlugIns.SelectedIndex = idx;
                    else
                        lbProjectPlugIns.SelectedIndex = idx - 1;
            }
        }
        #endregion
    }
}
