﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Utilities
// File    : PresentationStyleSettings.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 10/26/2012
// Note    : Copyright 2012, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a class that is used to contain settings information for a specific presentation style
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.   This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.9.6.0  10/24/2012  EFW  Created the code
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;

using SandcastleBuilder.Utils.BuildComponent;

namespace SandcastleBuilder.Utils.PresentationStyle
{
    /// <summary>
    /// This class is used to contain settings for a specific presentation style
    /// </summary>
    public sealed class PresentationStyleSettings
    {
        #region Private data members
        //=====================================================================

        private string parentFilePath;

        private List<ContentFiles> contentFiles;
        private List<TransformComponentArgument> transformComponentArgs;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the presentation style ID
        /// </summary>
        public string Id { get; private set; }

        /// <summary>
        /// This read-only property returns the presentation style title
        /// </summary>
        public string Title { get; private set; }

        /// <summary>
        /// This read-only property returns the presentation style base path
        /// </summary>
        public string BasePath { get; private set; }

        /// <summary>
        /// This read-only property returns the presentation style description
        /// </summary>
        public string Description { get; private set; }

        /// <summary>
        /// This read-only property returns the help file formats supported by the presentation style
        /// </summary>
        public HelpFileFormat HelpFileFormats { get; private set; }

        /// <summary>
        /// This read-only property returns an enumerable list of help content file locations
        /// </summary>
        public IEnumerable<ContentFiles> ContentFiles
        {
            get { return contentFiles; }
        }

        /// <summary>
        /// This read-only property returns the path in which BuildAssembler resource item files are stored
        /// </summary>
        public string ResourceItemsPath { get; private set; }

        /// <summary>
        /// This read-only property returns the path in which help file builder resource item files are stored
        /// </summary>
        public string ToolResourceItemsPath { get; private set; }

        /// <summary>
        /// This read-only property returns the document model transformation file and its parameters
        /// </summary>
        public TransformationFile DocumentModelTransformation { get; private set; }

        /// <summary>
        /// This read-only property returns the intermediate TOC transformation file and its parameters
        /// </summary>
        public TransformationFile IntermediateTocTransformation { get; private set; }

        /// <summary>
        /// This read-only property returns the BuildAssembler configuration filename for conceptual builds
        /// </summary>
        public string ConceptualBuildConfiguration { get; private set; }

        /// <summary>
        /// This read-only property returns the BuildAssembler configuration filename for reference builds
        /// </summary>
        public string ReferenceBuildConfiguration { get; private set; }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Private constructor
        /// </summary>
        /// <param name="parentFilePath">The parent file's path used to resolve relative paths within the
        /// presentation style.</param>
        private PresentationStyleSettings(string parentFilePath)
        {
            this.parentFilePath = parentFilePath;

            contentFiles = new List<ContentFiles>();
            transformComponentArgs = new List<TransformComponentArgument>();
        }
        #endregion

        #region Methods used to convert from XML
        //=====================================================================

        /// <summary>
        /// This is used to load the settings for a presentation style from an XML element
        /// </summary>
        /// <param name="parentFilePath">The parent presentation style file's path</param>
        /// <param name="style">The XML element containing the settings</param>
        /// <returns>The new presentation style settings item</returns>
        internal static PresentationStyleSettings FromXml(string parentFilePath, XElement style)
        {
            XElement description = style.Descendants("Description").FirstOrDefault(),
                resourceItems = style.Descendants("ResourceItems").FirstOrDefault(),
                toolResourceItems = (resourceItems == null) ? null :
                    resourceItems.Descendants("ToolResourceItems").FirstOrDefault(),
                docModel = style.Descendants("DocumentModelTransformation").FirstOrDefault(),
                toc = style.Descendants("IntermediateTOCTransformation").FirstOrDefault(),
                conceptual = style.Descendants("ConceptualBuildConfiguration").FirstOrDefault(),
                reference = style.Descendants("ReferenceBuildConfiguration").FirstOrDefault();

            if(resourceItems == null)
                throw new InvalidOperationException("ResourceItems element is missing");

            if(toolResourceItems == null)
                throw new InvalidOperationException("ToolResourceItems element is missing");

            if(docModel == null)
                throw new InvalidOperationException("DocumentModelTransformation element is missing");

            if(toc == null)
                throw new InvalidOperationException("IntermediateTOCTransformation element is missing");

            if(conceptual == null)
                throw new InvalidOperationException("ConceptualBuildConfiguration element is missing");

            if(reference == null)
                throw new InvalidOperationException("ReferenceBuildConfiguration element is missing");

            PresentationStyleSettings pss = new PresentationStyleSettings(parentFilePath)
            {
                Id = style.Attribute("ID").Value,
                Title = style.Attribute("Title").Value,
                BasePath = style.Attribute("BasePath").Value,
                Description = (description == null) ? "No description" : description.Value,
                ResourceItemsPath = resourceItems.Attribute("Path").Value,
                ToolResourceItemsPath = toolResourceItems.Attribute("Path").Value,
                DocumentModelTransformation = new TransformationFile(docModel),
                IntermediateTocTransformation = new TransformationFile(toc),
                ConceptualBuildConfiguration = conceptual.Attribute("File").Value,
                ReferenceBuildConfiguration = reference.Attribute("File").Value
            };

            foreach(var format in style.Descendants("SupportedFormats").Descendants("Format"))
                pss.HelpFileFormats |= (HelpFileFormat)Enum.Parse(typeof(HelpFileFormat),
                    format.Attribute("Type").Value, true);

            foreach(var content in style.Descendants("HelpContent").Descendants("Files"))
                pss.contentFiles.Add(new ContentFiles(content.Attribute("Source").Value,
                    (string)content.Attribute("Destination")));

            foreach(var arg in style.Descendants("TransformComponentArguments").Descendants("Argument"))
                pss.transformComponentArgs.Add(new TransformComponentArgument(arg));

            return pss;
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// This is used to resolve a presentation style path
        /// </summary>
        /// <param name="path">The path to resolve</param>
        /// <returns>The resolved path</returns>
        /// <remarks>Environment variables in the path will be expanded.  If relative, the path is converted
        /// to a full path relative to the presentation style's base path.</remarks>
        public string ResolvePath(string path)
        {
            string rootPath;

            path = ResolveEnvironmentVariables(path);

            if(!Path.IsPathRooted(path))
            {
                rootPath = ResolveEnvironmentVariables(this.BasePath);

                if(!Path.IsPathRooted(rootPath))
                    rootPath = Path.Combine(parentFilePath, rootPath);

                path = Path.Combine(rootPath, path);
            }

            return path;
        }

        /// <summary>
        /// This is used to resolve environment variable in a path with the added step of resolving
        /// <c>%DXROOT%</c> and <c>%SHFBROOT%</c> to the paths found by the build component manager if they
        /// do not resolve automatically.
        /// </summary>
        /// <param name="path">The path in which to resolve an environment variable</param>
        /// <returns>The resolved path value</returns>
        private static string ResolveEnvironmentVariables(string path)
        {
            path = Environment.ExpandEnvironmentVariables(path);

            if(!String.IsNullOrEmpty(BuildComponentManager.SandcastlePath) && path.IndexOf("%DXROOT%",
              StringComparison.Ordinal) != -1)
                path = path.Replace("%DXROOT%", FolderPath.TerminatePath(BuildComponentManager.SandcastlePath));

            if(!String.IsNullOrEmpty(BuildComponentManager.HelpFileBuilderFolder) && path.IndexOf("%SHFBROOT%",
              StringComparison.Ordinal) != -1)
                path = path.Replace("%SHFBROOT%", FolderPath.TerminatePath(BuildComponentManager.HelpFileBuilderFolder));

            return path;
        }

        /// <summary>
        /// This is used to copy the presentation style help file content to the given destination folder
        /// </summary>
        /// <param name="destinationBasePath">The destination base path to which the files are copied</param>
        /// <param name="progressReporter">An optional action delegate used to report progress</param>
        public void CopyHelpContent(string destinationBasePath, Action<string, object[]> progressReporter)
        {
            string sourcePath, destPath;

            foreach(var content in contentFiles)
            {
                sourcePath = this.ResolvePath(content.SourcePathWildcard);

                if(content.DestinationFolder == null)
                    destPath = Path.Combine(destinationBasePath,
                        Path.GetFileName(Path.GetDirectoryName(content.SourcePathWildcard)));
                else
                    if(content.DestinationFolder.Length == 0)
                        destPath = destinationBasePath;
                    else
                        destPath = Path.Combine(destinationBasePath, content.DestinationFolder);

                RecursiveCopy(sourcePath, destPath, progressReporter);
            }
        }

        /// <summary>
        /// This copies files from the specified source folder to the specified destination folder.  If any
        /// subfolders are found below the source folder and the wildcard is "*.*", the subfolders are also
        /// copied recursively.
        /// </summary>
        /// <param name="sourcePath">The source path from which to copy</param>
        /// <param name="destPath">The destination path to which to copy</param>
        /// <param name="progressReporter">An optional action delegate used to report progress</param>
        private static void RecursiveCopy(string sourcePath, string destPath, Action<string, object[]> progressReporter)
        {
            if(sourcePath == null)
                throw new ArgumentNullException("sourcePath");

            if(destPath == null)
                throw new ArgumentNullException("destPath");

            int idx = sourcePath.LastIndexOf('\\');

            string dirName = sourcePath.Substring(0, idx), fileSpec = sourcePath.Substring(idx + 1), filename;

            if(destPath[destPath.Length - 1] != '\\')
                destPath += @"\";

            foreach(string name in Directory.EnumerateFiles(dirName, fileSpec))
            {
                filename = destPath + Path.GetFileName(name);

                if(!Directory.Exists(destPath))
                    Directory.CreateDirectory(destPath);

                // All attributes are turned off so that we can delete it later
                File.Copy(name, filename, true);
                File.SetAttributes(filename, FileAttributes.Normal);

                if(progressReporter != null)
                    progressReporter("{0} -> {1}", new[] { name, filename});
            }

            // For "*.*", copy subfolders too
            if(fileSpec == "*.*")
            {
                // Ignore hidden folders as they may be under source control and are not wanted
                foreach(string folder in Directory.EnumerateDirectories(dirName))
                    if((File.GetAttributes(folder) & FileAttributes.Hidden) != FileAttributes.Hidden)
                        RecursiveCopy(folder + @"\*.*", destPath + folder.Substring(dirName.Length + 1) + @"\",
                            progressReporter);
            }
        }
        #endregion
    }
}
