//===============================================================================================================
// System  : Sandcastle Help File Builder Plug-Ins
// File    : DeploymentPlugIn.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 03/09/2014
// Note    : Copyright 2007-2014, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a plug-in that can be used to deploy the resulting help file output to a location other
// than the output folder (i.e. a file share, an FTP site, a web server, etc.).
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.5.2.0  09/09/2007  EFW  Created the code
// 1.6.0.5  02/18/2008  EFW  Added support for relative deployment paths
// 1.8.0.0  08/13/2008  EFW  Updated to support the new project format
// 1.8.0.3  07/06/2009  EFW  Added support for Help Viewer deployment
// -------  12/17/2013  EFW  Updated to use MEF for the plug-ins
//          03/09/2014  EFW  Updated to support Open XML file deployment
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Net;
using System.Net.Cache;
using System.Windows.Forms;
using System.Xml.XPath;

using Sandcastle.Core;

using SandcastleBuilder.Utils;
using SandcastleBuilder.Utils.BuildComponent;
using SandcastleBuilder.Utils.BuildEngine;

namespace SandcastleBuilder.PlugIns
{
    /// <summary>
    /// This plug-in class is used to copy the resulting help file output to a location other than the output
    /// folder (i.e. a file share, an FTP site, a web server, etc.).
    /// </summary>
    [HelpFileBuilderPlugInExport("Output Deployment", IsConfigurable = true,
      Version = AssemblyInfo.ProductVersion, Copyright = AssemblyInfo.Copyright,
      Description = "This plug-in is used to deploy the resulting help file output to a location other than " +
        "the output folder (i.e. a file share, a web server, an FTP site, etc.).")]
    public sealed class DeploymentPlugIn : IPlugIn
    {
        #region Private data members
        //=====================================================================

        private List<ExecutionPoint> executionPoints;

        private BuildProcess builder;

        // Plug-in configuration options
        private DeploymentLocation deployHelp1, deployHelp2, deployHelpViewer, deployWebsite, deployOpenXml;
        private bool deleteAfterDeploy, renameMSHA;
        #endregion

        #region IPlugIn implementation
        //=====================================================================

        /// <summary>
        /// This read-only property returns a collection of execution points that define when the plug-in should
        /// be invoked during the build process.
        /// </summary>
        public IEnumerable<ExecutionPoint> ExecutionPoints
        {
            get
            {
                if(executionPoints == null)
                    executionPoints = new List<ExecutionPoint>
                    {
                        // This plug-in has a lower priority as it should execute after all other plug-ins in
                        // case they add other files to the set.
                        new ExecutionPoint(BuildStep.CompilingHelpFile, ExecutionBehaviors.After, 200),
                        new ExecutionPoint(BuildStep.CopyingWebsiteFiles, ExecutionBehaviors.After, 200)
                    };

                return executionPoints;
            }
        }

        /// <summary>
        /// This method is used by the Sandcastle Help File Builder to let the plug-in perform its own
        /// configuration.
        /// </summary>
        /// <param name="project">A reference to the active project</param>
        /// <param name="currentConfig">The current configuration XML fragment</param>
        /// <returns>A string containing the new configuration XML fragment</returns>
        /// <remarks>The configuration data will be stored in the help file builder project</remarks>
        public string ConfigurePlugIn(SandcastleProject project, string currentConfig)
        {
            using(DeploymentConfigDlg dlg = new DeploymentConfigDlg(currentConfig))
            {
                if(dlg.ShowDialog() == DialogResult.OK)
                    currentConfig = dlg.Configuration;
            }

            return currentConfig;
        }

        /// <summary>
        /// This method is used to initialize the plug-in at the start of the build process
        /// </summary>
        /// <param name="buildProcess">A reference to the current build process</param>
        /// <param name="configuration">The configuration data that the plug-in should use to initialize itself</param>
        /// <exception cref="BuilderException">This is thrown if the plug-in configuration is not valid</exception>
        public void Initialize(BuildProcess buildProcess, XPathNavigator configuration)
        {
            XPathNavigator root, msHelpViewer;
            string value;

            builder = buildProcess;

            var metadata = (HelpFileBuilderPlugInExportAttribute)this.GetType().GetCustomAttributes(
                typeof(HelpFileBuilderPlugInExportAttribute), false).First();

            builder.ReportProgress("{0} Version {1}\r\n{2}", metadata.Id, metadata.Version, metadata.Copyright);

            root = configuration.SelectSingleNode("configuration");
            value = root.GetAttribute("deleteAfterDeploy", String.Empty);

            if(!String.IsNullOrEmpty(value))
                deleteAfterDeploy = Convert.ToBoolean(value, CultureInfo.InvariantCulture);

            if(root.IsEmptyElement)
                throw new BuilderException("ODP0001", "The Output Deployment plug-in has not been " +
                    "configured yet");

            deployHelp1 = DeploymentLocation.FromXPathNavigator(root, "help1x");
            deployHelp2 = DeploymentLocation.FromXPathNavigator(root, "help2x");
            deployHelpViewer = DeploymentLocation.FromXPathNavigator(root, "helpViewer");
            deployWebsite = DeploymentLocation.FromXPathNavigator(root, "website");
            deployOpenXml = DeploymentLocation.FromXPathNavigator(root, "openXml");

            msHelpViewer = root.SelectSingleNode("deploymentLocation[@id='helpViewer']");

            if(msHelpViewer == null || !Boolean.TryParse(msHelpViewer.GetAttribute("renameMSHA",
              String.Empty).Trim(), out renameMSHA))
                renameMSHA = false;

            // At least one deployment location must be defined
            if(deployHelp1.Location == null && deployHelp2.Location == null &&
              deployHelpViewer.Location == null && deployWebsite.Location == null && deployOpenXml.Location == null)
                throw new BuilderException("ODP0002", "The output deployment plug-in must have at least " +
                    "one configured deployment location");

            // Issue a warning if the deployment location is null and the associated help file format is active
            if(deployHelp1.Location == null &&
              (builder.CurrentProject.HelpFileFormat & HelpFileFormats.HtmlHelp1) != 0)
                builder.ReportWarning("ODP0003", "HTML Help 1 will be generated but not deployed due to " +
                    "missing deployment location information");

            if(deployHelp2.Location == null &&
              (builder.CurrentProject.HelpFileFormat & HelpFileFormats.MSHelp2) != 0)
                builder.ReportWarning("ODP0003", "MS Help 2 will be generated but not deployed due to " +
                    "missing deployment location information");

            if(deployHelpViewer.Location == null &&
              (builder.CurrentProject.HelpFileFormat & HelpFileFormats.MSHelpViewer) != 0)
                builder.ReportWarning("ODP0003", "MS Help Viewer will be generated but not deployed due " +
                    "to missing deployment location information");

            if(deployWebsite.Location == null &&
              (builder.CurrentProject.HelpFileFormat & HelpFileFormats.Website) != 0)
                builder.ReportWarning("ODP0003", "Website will be generated but not deployed due to " +
                    "missing deployment location information");

            if(deployOpenXml.Location == null &&
              (builder.CurrentProject.HelpFileFormat & HelpFileFormats.OpenXml) != 0)
                builder.ReportWarning("ODP0003", "Open XML will be generated but not deployed due to " +
                    "missing deployment location information");
        }

        /// <summary>
        /// This method is used to execute the plug-in during the build process
        /// </summary>
        /// <param name="context">The current execution context</param>
        public void Execute(ExecutionContext context)
        {
            // Deploy each of the selected help file formats
            if(builder.CurrentFormat == HelpFileFormats.HtmlHelp1)
            {
                builder.ReportProgress("Deploying HTML Help 1 file");
                this.DeployOutput(builder.Help1Files, deployHelp1);
            }

            if(builder.CurrentFormat == HelpFileFormats.MSHelp2)
            {
                builder.ReportProgress("Deploying MS Help 2 files");
                this.DeployOutput(builder.Help2Files, deployHelp2);
            }

            if(builder.CurrentFormat == HelpFileFormats.MSHelpViewer)
            {
                builder.ReportProgress("Deploying MS Help Viewer files");
                this.DeployOutput(builder.HelpViewerFiles, deployHelpViewer);
            }

            if(builder.CurrentFormat == HelpFileFormats.Website)
            {
                builder.ReportProgress("Deploying website files");
                this.DeployOutput(builder.WebsiteFiles, deployWebsite);
            }

            if(builder.CurrentFormat == HelpFileFormats.OpenXml)
            {
                builder.ReportProgress("Deploying Open XML files");
                this.DeployOutput(builder.OpenXmlFiles, deployOpenXml);
            }
        }
        #endregion

        #region Deploy project output
        //=====================================================================

        /// <summary>
        /// Deploy the given list of files to the specified location
        /// </summary>
        /// <param name="files">The list of files to deploy</param>
        /// <param name="location">The deployment location</param>
        private void DeployOutput(Collection<string> files, DeploymentLocation location)
        {
            WebClient webClient = null;
            Uri destUri, target = location.Location;
            string rootPath, destFile, destPath;
            int basePathLength = builder.OutputFolder.Length;

            if(target == null)
            {
                builder.ReportProgress("No deployment location defined for this help format");
                return;
            }

            if(files.Count == 0)
            {
                builder.ReportProgress("No files found to deploy");
                return;
            }

            try
            {
                // Determine the path type
                if(!target.IsAbsoluteUri)
                    rootPath = Path.GetFullPath(target.OriginalString);
                else
                    if(target.IsFile || target.IsUnc)
                        rootPath = target.LocalPath;
                    else
                    {
                        // FTP, HTTP, etc.
                        rootPath = target.ToString();
                        webClient = new WebClient();

                        // Initialize the web client
                        if(!location.UserCredentials.UseDefaultCredentials)
                            webClient.Credentials = new NetworkCredential(location.UserCredentials.UserName,
                                location.UserCredentials.Password);

                        webClient.CachePolicy = new RequestCachePolicy(RequestCacheLevel.NoCacheNoStore);

                        if(location.ProxyCredentials.UseProxyServer)
                        {
                            webClient.Proxy = new WebProxy(location.ProxyCredentials.ProxyServer, true);

                            if(!location.ProxyCredentials.Credentials.UseDefaultCredentials)
                                webClient.Proxy.Credentials = new NetworkCredential(
                                    location.ProxyCredentials.Credentials.UserName,
                                    location.ProxyCredentials.Credentials.Password);
                        }
                    }

                foreach(string sourceFile in files)
                {
                    destFile = Path.Combine(rootPath, sourceFile.Substring(basePathLength));

                    // Rename MSHA file?  Note that if renamed and there is more than one, the last one
                    // copied wins.  This really only applies to MS Help Viewer output but it's the only
                    // unique option so we'll do it in all cases for now.
                    if(Path.GetExtension(destFile).Equals(".msha", StringComparison.OrdinalIgnoreCase) && renameMSHA)
                        destFile = Path.Combine(Path.GetDirectoryName(destFile), "HelpContentSetup.msha");

                    if(webClient == null)
                    {
                        builder.ReportProgress("    Deploying {0} to {1}", sourceFile, destFile);

                        destPath = Path.GetDirectoryName(destFile);

                        if(!Directory.Exists(destPath))
                            Directory.CreateDirectory(destPath);

                        File.Copy(sourceFile, destFile, true);
                    }
                    else
                    {
                        destUri = new Uri(destFile);
                        builder.ReportProgress("    Deploying {0} to {1}", sourceFile, destUri);

                        webClient.UploadFile(destUri, sourceFile);
                    }

                    // If not wanted, remove the source file after deployment
                    if(deleteAfterDeploy)
                        File.Delete(sourceFile);
                }
            }
            finally
            {
                if(webClient != null)
                    webClient.Dispose();
            }
        }
        #endregion

        #region IDisposable implementation
        //=====================================================================

        /// <summary>
        /// This implements the Dispose() interface to properly dispose of the plug-in object
        /// </summary>
        public void Dispose()
        {
            // Nothing to dispose of in this one
            GC.SuppressFinalize(this);
        }
        #endregion
    }
}
