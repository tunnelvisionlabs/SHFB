//===============================================================================================================
// System  : Sandcastle Help File Builder Plug-Ins
// File    : ScriptSharpPlugIn.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/12/2013
// Note    : Copyright 2008-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a plug-in designed to modify the reflection information file produced after running
// MRefBuilder on assemblies produced by the Script# compiler so that it is suitable for use in producing a help
// file.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.6.0.5  01/25/2008  EFW  Created the code
// 1.8.0.0  07/14/2008  EFW  Updated for use with MSBuild project format
// 1.9.9.0  12/04/2013  EFW  Updated for use with the new visibility settings in MRefBuilder.config.
//===============================================================================================================

using System;
using System.Diagnostics;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

using SandcastleBuilder.Utils;
using SandcastleBuilder.Utils.BuildEngine;
using SandcastleBuilder.Utils.PlugIn;

namespace SandcastleBuilder.PlugIns
{
    /// <summary>
    /// This plug-in class is designed to modify the reflection information file produced after running
    /// MRefBuilder on assemblies produced by the Script# compiler so that it is suitable for use in
    /// producing a help file.
    /// </summary>
    public class ScriptSharpPlugIn : IPlugIn
    {
        #region Private data members
        //=====================================================================

        private ExecutionPointCollection executionPoints;

        private BuildProcess builder;
        #endregion

        #region IPlugIn implementation
        //=====================================================================

        /// <summary>
        /// This read-only property returns a friendly name for the plug-in
        /// </summary>
        public string Name
        {
            get { return "Script# Reflection File Fixer"; }
        }

        /// <summary>
        /// This read-only property returns the version of the plug-in
        /// </summary>
        public Version Version
        {
            get
            {
                // Use the assembly version
                Assembly asm = Assembly.GetExecutingAssembly();
                FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

                return new Version(fvi.ProductVersion);
            }
        }

        /// <summary>
        /// This read-only property returns the copyright information for the plug-in
        /// </summary>
        public string Copyright
        {
            get
            {
                // Use the assembly copyright
                Assembly asm = Assembly.GetExecutingAssembly();
                AssemblyCopyrightAttribute copyright = (AssemblyCopyrightAttribute)Attribute.GetCustomAttribute(
                    asm, typeof(AssemblyCopyrightAttribute));

                return copyright.Copyright + "\r\nScript# is Copyright \xA9 2007-2013 Nikhil Kothari, " +
                    "All Rights Reserved";
            }
        }

        /// <summary>
        /// This read-only property returns a brief description of the plug-in
        /// </summary>
        public string Description
        {
            get
            {
                return "This plug-in is used to modify the reflection information file produced after " +
                    "running MRefBuilder on assemblies produced by the Script# compiler so that it is " +
                    "suitable for use in producing a help file.";
            }
        }

        /// <summary>
        /// This plug-in does not support configuration
        /// </summary>
        /// <seealso cref="ConfigurePlugIn"/>
        public bool SupportsConfiguration
        {
            get { return false; }
        }

        /// <summary>
        /// This plug-in runs in partial builds
        /// </summary>
        public bool RunsInPartialBuild
        {
            get { return true; }
        }

        /// <summary>
        /// This read-only property returns a collection of execution points that define when the plug-in should
        /// be invoked during the build process.
        /// </summary>
        public ExecutionPointCollection ExecutionPoints
        {
            get
            {
                if(executionPoints == null)
                {
                    executionPoints = new ExecutionPointCollection();

                    executionPoints.Add(new ExecutionPoint(BuildStep.GenerateReflectionInfo,
                        ExecutionBehaviors.Before));
                }

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
            MessageBox.Show("This plug-in has no configurable settings",
                "Script# Reflection File Fixer Plug-In", MessageBoxButtons.OK, MessageBoxIcon.Information);

            return currentConfig;
        }

        /// <summary>
        /// This method is used to initialize the plug-in at the start of the build process
        /// </summary>
        /// <param name="buildProcess">A reference to the current build process</param>
        /// <param name="configuration">The configuration data that the plug-in should use to initialize itself</param>
        public void Initialize(BuildProcess buildProcess, XPathNavigator configuration)
        {
            builder = buildProcess;

            builder.ReportProgress("{0} Version {1}\r\n{2}", this.Name, this.Version, this.Copyright);
        }

        /// <summary>
        /// This method is used to execute the plug-in during the build process
        /// </summary>
        /// <param name="context">The current execution context</param>
        public void Execute(Utils.PlugIn.ExecutionContext context)
        {
            if(this.ModifyMRefBuilderConfig())
                this.ModifyGenerateRefInfoProject();
        }
        #endregion

        #region IDisposable implementation
        //=====================================================================

        /// <summary>
        /// This handles garbage collection to ensure proper disposal of the plug-in if not done explicit with
        /// <see cref="Dispose()"/>.
        /// </summary>
        ~ScriptSharpPlugIn()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// This implements the Dispose() interface to properly dispose of the plug-in object
        /// </summary>
        /// <overloads>There are two overloads for this method.</overloads>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This can be overridden by derived classes to add their own disposal code if necessary
        /// </summary>
        /// <param name="disposing">Pass true to dispose of the managed and unmanaged resources or false to just
        /// dispose of the unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Nothing to dispose of in this one
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// This is used to modify the MRefBuilder.config file for use with Script#
        /// </summary>
        private bool ModifyMRefBuilderConfig()
        {
            XmlDocument config;
            XmlNode filter, type;
            XmlAttribute attr;
            string configFile;

            configFile = builder.WorkingFolder + "MRefBuilder.config";

            // If the configuration file doesn't exist we have nothing to do.  However, it could be that some
            // other plug-in has bypassed it so only issue a warning.
            if(!File.Exists(configFile))
            {
                builder.ReportWarning("SSP0001", "The MRefBuilder configuration file '{0}' could not be " +
                    "found.  The Script# plug-in did not run successfully.", configFile);
                return false;
            }

            builder.ReportProgress("Adding Script# attributes to MRefBuilder.config");

            // Add the required Script# attributes to the attribute filter
            config = new XmlDocument();
            config.Load(configFile);

            filter = config.SelectSingleNode("configuration/dduetools/attributeFilter/namespace[@name='System']");

            if(filter == null)
                throw new BuilderException("SSP0002", "Unable to locate attribute filter for the System " +
                    "namespace in the configuration file");

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "AttachedPropertyAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "GlobalMethodsAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "IgnoreNamespaceAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "IntrinsicPropertyAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "NonScriptableAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "PreserveCaseAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            type = config.CreateElement("type");
            attr = config.CreateAttribute("name");
            attr.Value = "RecordAttribute";
            type.Attributes.Append(attr);
            attr = config.CreateAttribute("required");
            attr.Value = "true";
            type.Attributes.Append(attr);
            filter.AppendChild(type);

            config.Save(configFile);

            return true;
        }

        /// <summary>
        /// This is used to modify the GenerateRefInfo.proj file for use with Script#
        /// </summary>
        private void ModifyGenerateRefInfoProject()
        {
            XmlNamespaceManager nsm;
            XmlDocument project;
            XmlNode target, task;
            XmlAttribute attr;
            string projectFile;

            projectFile = builder.WorkingFolder + "GenerateRefInfo.proj";

            // If the project doesn't exist we have nothing to do.  However, it could be that some other plug-in
            // has bypassed it so only issue a warning.
            if(!File.Exists(projectFile))
            {
                builder.ReportWarning("SSP0003", "The reflection information generation project '{0}' could " +
                    "not be found.  The Script# plug-in did not run successfully.", projectFile);
                return;
            }

            builder.ReportProgress("Adding Script# AfterGenerateRefInfo tasks to GenerateRefInfo.proj");

            // Add the transform that fixes up the Script# elements to the AfterGenerateRefInfo project target.
            // Note that we use a customized version that adds a <scriptSharp /> element to each API node so that
            // our custom JavaScript syntax generator applies the casing rules to the member names.
            project = new XmlDocument();
            project.Load(projectFile);
            nsm = new XmlNamespaceManager(project.NameTable);
            nsm.AddNamespace("MSBuild", project.DocumentElement.NamespaceURI);

            target = project.SelectSingleNode("//MSBuild:Target[@Name='AfterGenerateRefInfo']", nsm);

            if(target == null)
                throw new BuilderException("SSP0004", "Unable to locate AfterGenerateRefInfo target " +
                    "in project file");

            task = project.CreateElement("Message", nsm.LookupNamespace("MSBuild"));
            attr = project.CreateAttribute("Text");
            attr.Value = "Fixing up Script# elements...";
            task.Attributes.Append(attr);
            target.AppendChild(task);

            task = project.CreateElement("Copy", nsm.LookupNamespace("MSBuild"));
            attr = project.CreateAttribute("SourceFiles");
            attr.Value = "reflection.org";
            task.Attributes.Append(attr);

            attr = project.CreateAttribute("DestinationFiles");
            attr.Value = "scriptsharp.org";
            task.Attributes.Append(attr);
            target.AppendChild(task);

            task = project.CreateElement("Microsoft.Ddue.Tools.MSBuild.XslTransform", nsm.LookupNamespace("MSBuild"));
            attr = project.CreateAttribute("WorkingFolder");
            attr.Value = "$(WorkingFolder)";
            task.Attributes.Append(attr);

            attr = project.CreateAttribute("Transformations");
            attr.Value = Path.Combine(builder.SandcastleFolder, @"ProductionTransforms\FixScriptSharp.xsl");
            task.Attributes.Append(attr);

            attr = project.CreateAttribute("InputFile");
            attr.Value = "scriptsharp.org";
            task.Attributes.Append(attr);

            attr = project.CreateAttribute("OutputFile");
            attr.Value = "reflection.org";
            task.Attributes.Append(attr);

            target.AppendChild(task);

            project.Save(projectFile);
        }
        #endregion
    }
}
