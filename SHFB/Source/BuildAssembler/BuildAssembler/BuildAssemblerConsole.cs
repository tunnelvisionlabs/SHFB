// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change history:
// 02/16/2012 - EFW - Added support for setting a verbosity level.  Messages with a log level below
// the current verbosity level are ignored.
// 01/12/2013 - EFW - Moved the execution code into the BuildAssembler class to allow for parallel execution
// 12/28/2013 - EFW - Added MSBuild task support

using System;
using System.ComponentModel.Composition;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Sandcastle.Core;
using Sandcastle.Core.CommandLine;
using Sandcastle.Core.BuildAssembler;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This controls the BuildAssembler process
    /// </summary>
    class BuildAssemblerConsole
    {
        [Export(typeof(BuildAssemblerCore))]
        public static BuildAssemblerCore BuildAssembler { get; private set; }

        /// <summary>
        /// Main program entry point
        /// </summary>
        /// <param name="args">The command line arguments</param>
        /// <returns>Zero on success or one on failure</returns>
        public static int Main(string[] args)
        {
            int exitCode = 0;

            ConsoleApplication.WriteBanner();

            #region Read command line arguments, and setup config

            // Specify options
            OptionCollection options = new OptionCollection();
            options.Add(new SwitchOption("?", "Show this help page."));
            options.Add(new StringOption("config", "Specify a configuration file.", "configFilePath"));

            // Process options
            ParseArgumentsResult results = options.ParseArguments(args);

            // Process help option
            if(results.Options["?"].IsPresent)
            {
                Console.WriteLine("BuildAssembler [options] manifestFilename");
                options.WriteOptionSummary(Console.Out);
                return 1;
            }

            // check for invalid options
            if(!results.Success)
            {
                results.WriteParseErrors(Console.Out);
                return 1;
            }

            // Check for manifest
            if(results.UnusedArguments.Count != 1)
            {
                Console.WriteLine("You must supply exactly one manifest file.");
                return 1;
            }

            string manifest = results.UnusedArguments[0];

            // Load the configuration file
            XPathDocument configuration;

            try
            {
                if(results.Options["config"].IsPresent)
                    configuration = ConsoleApplication.GetConfigurationFile((string)results.Options["config"].Value);
                else
                    configuration = ConsoleApplication.GetConfigurationFile();
            }
            catch(IOException e)
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, "The specified configuration file could not " +
                    "be loaded. The error message is: {0}", e.Message);
                return 1;
            }
            catch(XmlException e)
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, "The specified configuration file is not " +
                    "well-formed. The error message is: {0}", e.Message);
                return 1;
            }
            #endregion

            // Create a build assembler instance to do the work.  Messages are logged to the console logger.
            BuildAssembler = new BuildAssemblerCore((lvl, msg) => ConsoleApplication.WriteMessage(lvl, msg));

            try
            {
                // Execute it using the given configuration and manifest
                BuildAssembler.Execute(configuration, manifest);
            }
            catch(Exception ex)
            {
                // Ignore aggregate exceptions where the inner exception is OperationCanceledException.
                // These are the result of of logging an error message.
                if(!(ex is AggregateException) || !(ex.InnerException is OperationCanceledException))
                {
                    System.Diagnostics.Debug.WriteLine(ex);
                    ConsoleApplication.WriteMessage(LogLevel.Error, ex.GetExceptionMessage());
                }

                exitCode = 1;
            }
            finally
            {
                BuildAssembler.Dispose();
            }

            return exitCode;
        }
    }
}
