//===============================================================================================================
// System  : Sandcastle Help File Builder MSBuild Tasks
// File    : MRefBuilderTask.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 12/06/2013
// Note    : Copyright 2008-2013, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains the MSBuild task used to generate the MRefBuilder response file and execute MRefBuilder.exe
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.8.0.0  07/11/2008  EFW  Created the code
// 1.9.9.0  11/30/2013  EFW  Removed the deprecated DocumentInternals property
//===============================================================================================================

using System;
using System.IO;

using Microsoft.Build.Framework;
using Microsoft.Build.Utilities;

namespace SandcastleBuilder.Utils.MSBuild
{
    /// <summary>
    /// This task is used to generate the MRefBuilder response file and run MRefBuilder.exe
    /// </summary>
    public class MRefBuilder : ToolTask
    {
        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the tool name (MRefBuilder.exe)
        /// </summary>
        protected override string ToolName
        {
            get { return "MRefBuilder.exe"; }
        }

        /// <summary>
        /// This is overridden to force all standard error info to be logged
        /// </summary>
        protected override MessageImportance StandardErrorLoggingImportance
        {
            get { return MessageImportance.High; }
        }

        /// <summary>
        /// This is overridden to force all standard output info to be logged
        /// </summary>
        protected override MessageImportance  StandardOutputLoggingImportance
        {
            get { return MessageImportance.High; }
        }

        /// <summary>
        /// This is used to pass in the path to the Sandcastle tools
        /// </summary>
        [Required]
        public string SandcastlePath { get; set; }

        /// <summary>
        /// This is used to pass in the working folder where the files are located
        /// </summary>
        [Required]
        public string WorkingFolder { get; set; }

        /// <summary>
        /// This is used to pass in the assemblies to reflect over
        /// </summary>
        [Required]
        public ITaskItem[] Assemblies { get; set; }

        /// <summary>
        /// This is used to pass in the resolved references
        /// </summary>
        /// <value>References are optional.</value>
        public ITaskItem[] References { get; set; }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <summary>
        /// Validate the parameters
        /// </summary>
        /// <returns>True if the parameters are valid, false if not</returns>
        protected override bool ValidateParameters()
        {
            if(this.Assemblies == null || this.Assemblies.Length == 0)
            {
                Log.LogError(null, "MBT0001", "MBT0001", "SHFB", 0, 0, 0, 0,
                  "At least one assembly (.dll or .exe) is required for MRefBuilder to parse.");
                return false;
            }

            return true;
        }

        /// <summary>
        /// This returns the full path to the tool
        /// </summary>
        /// <returns>The full path to the tool</returns>
        protected override string GenerateFullPathToTool()
        {
            return Path.Combine(this.SandcastlePath, "ProductionTools\\" + this.ToolName);
        }

        /// <summary>
        /// Generate the command line parameters
        /// </summary>
        /// <returns>The command line parameters</returns>
        protected override string GenerateCommandLineCommands()
        {
            return base.GetResponseFileSwitch("MRefBuilder.rsp");
        }

        /// <summary>
        /// Generate the response file commands
        /// </summary>
        /// <returns>An empty string</returns>
        /// <remarks>Rather than letting the tool task create the response file, we'll create it ourself in the
        /// working folder so that we can look at it if the build fails.</remarks>
        protected override string GenerateResponseFileCommands()
        {
            try
            {
                using(StreamWriter sw = new StreamWriter("MRefBuilder.rsp"))
                {
                    sw.WriteLine("/config:MRefBuilder.config");
                    sw.WriteLine("/out:reflection.org");

                    if(this.References != null)
                        foreach(ITaskItem item in this.References)
                            sw.WriteLine("/dep:{0}", item.ItemSpec);

                    foreach(ITaskItem item in this.Assemblies)
                        sw.WriteLine(item.ItemSpec);
                }
            }
            catch(Exception ex)
            {
                System.Diagnostics.Debug.WriteLine(ex.ToString());
                Log.LogError(null, "MBT0002", "MBT0002", "SHFB", 0, 0, 0, 0,
                  "Unable to generate response file.  Reason: {0}", ex);
            }

            return String.Empty;
        }

        /// <summary>
        /// This is overridden to return the working folder for the build
        /// </summary>
        /// <returns>The working folder for the build</returns>
        protected override string GetWorkingDirectory()
        {
            return Path.GetFullPath(this.WorkingFolder);
        }
        #endregion
    }
}
