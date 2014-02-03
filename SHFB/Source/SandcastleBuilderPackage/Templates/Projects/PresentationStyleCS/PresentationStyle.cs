//
// NOTES
// =====
// This project is a simple shell.  The folder structure in the Solution Explorer is a suggested folder
// layout that mimics the existing presentation styles in Sandcastle.  If this assembly will contain
// multiple presentation styles, group the presentation style folders into a subfolder named after the
// presentation style.
//
// Copy the files from an existing presentation style to get a head start or start from scratch by creating
// your own.  When adding files to the folders, be sure to set their Build Action property to "Content" and
// their Copy to Output Folder property to "Copy if newer".
//

using System.Collections.Generic;
using System.IO;
using System.Reflection;

using Sandcastle.Core;
using Sandcastle.Core.PresentationStyle;

// Search for "TODO" to find changes that you need to make to this presentation style template.

namespace $safeprojectname$
{
    /// <summary>
    /// TODO: Set your presentation style's unique ID and description in the export attribute below.
    /// </summary>
    /// <remarks>The <c>PresentationStyleExportAttribute</c> is used to export your presentation style so
    /// that the help file builder finds it and can make use of it.  The example below shows the basic usage
    /// for a common presentation style.  Presentation styles are singletons in nature.  The composition
    /// container will create instances as needed and will dispose of them when the container is disposed
    /// of.</remarks>
    [PresentationStyleExport("$safeprojectname$", "$safeprojectname$", Version = AssemblyInfo.ProductVersion,
      Copyright = AssemblyInfo.Copyright, Description = "$safeprojectname$ custom presentation style")]
    public sealed class $safeprojectname$PresentationStyle : PresentationStyleSettings
    {
        /// <inheritdoc />
        public override string Location
        {
            get { return Path.GetDirectoryName(Assembly.GetExecutingAssembly().Location); }
        }

        /// <summary>
        /// Constructor
        /// </summary>
        public $safeprojectname$PresentationStyle()
        {
            // The base path of the presentation style files relative to the assembly's location.  If your
            // assembly will reside in the same folder as the presentation style content, you can remove this
            // property setting.  If adding multiple presentation styles to the assembly, set this to the name
            // of the subfolder that contains the presentation style content folders.
            this.BasePath = "$safeprojectname$";

            // TODO: Adjust the rest of these properties as needed.

            this.SupportedFormats = HelpFileFormats.HtmlHelp1 | HelpFileFormats.MSHelp2 |
                HelpFileFormats.MSHelpViewer | HelpFileFormats.Website;

            this.SupportsNamespaceGrouping = true;

            // If relative, these paths are relative to the base path
            this.ResourceItemsPath = "Content";
            this.ToolResourceItemsPath = "SHFBContent";

            this.DocumentModelTransformation = new TransformationFile(
                @"%SHFBROOT%\ProductionTransforms\ApplyVSDocModel.xsl", new Dictionary<string, string>
                {
                    { "IncludeAllMembersTopic", "true" },
                    { "IncludeInheritedOverloadTopics", "false" },
                    { "project", "{@ProjectNodeIDOptional}" }
                });

            this.IntermediateTocTransformation = new TransformationFile(
                @"%SHFBROOT%\ProductionTransforms\CreateVSToc.xsl");

            this.ConceptualBuildConfiguration = @"Configuration\SHFBConceptual.config";
            this.ReferenceBuildConfiguration = @"Configuration\SHFBReference.config";

            // Note that UNIX based web servers may be case-sensitive with regard to folder and filenames so
            // match the case of the folder and filenames in the literals to their actual casing on the file
            // system.
            this.ContentFiles.Add(new ContentFiles(this.SupportedFormats, @"icons\*.*"));
            this.ContentFiles.Add(new ContentFiles(this.SupportedFormats, @"scripts\*.*"));
            this.ContentFiles.Add(new ContentFiles(this.SupportedFormats, @"styles\*.*"));

            // By default, this will use the standard web file content from the Sandcastle Help File Builder
            this.ContentFiles.Add(new ContentFiles(HelpFileFormats.Website, "%SHFBROOT%", @"Web\*.*", @".\",
                new[] { ".aspx", ".html", ".htm", ".php" } ));

            this.TransformComponentArguments.Add(new TransformComponentArgument("logoFile", true, true, null,
                "An optional logo file to insert into the topic headers.  Specify the filename only, omit " +
                "the path.  Place the file in your project in an icons\\ folder and set the Build Action to " +
                "Content.  If blank, no logo will appear in the topic headers.  If building website output " +
                "and your web server is case-sensitive, be sure to match the case of the folder name in your " +
                "project with that of the presentation style.  The same applies to the logo filename itself."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("logoHeight", true, true, null,
                "An optional logo height.  If left blank, the actual logo image height is used."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("logoWidth", true, true, null,
                "An optional logo width.  If left blank, the actual logo image width is used."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("logoAltText", true, true, null,
                "Optional logo alternate text.  If left blank, no alternate text is added."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("logoPlacement", true, true,
                 "left", "An optional logo placement.  Specify left, right, or above.  If not specified, the " +
                "default is left."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("logoAlignment", true, true,
                "left", "An optional logo alignment when using the 'above' placement option.  Specify left, " +
                "right, or center.  If not specified, the default is left."));
            this.TransformComponentArguments.Add(new TransformComponentArgument("maxVersionParts", false, true,
                null, "The maximum number of assembly version parts to show in API member topics.  Set to 2, " +
                "3, or 4 to limit it to 2, 3, or 4 parts or leave it blank for all parts including the " +
                "assembly file version value if specified."));
        }
    }
}
