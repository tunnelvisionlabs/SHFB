//===============================================================================================================
// System  : Sandcastle Help File Builder Components
// File    : PostTransformComponent.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 10/14/2012
// Note    : Copyright 2006-2012, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a build component that is used to insert a logo image at the top of each help topic.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
//===============================================================================================================
// 1.3.3.0  11/23/2006  EFW  Created the code
// 1.4.0.0  01/31/2007  EFW  Added placement options for logo.  Made changes to support custom presentation
//                           styles.  Reworked version info code to improve performance when used with very
//                           large documentation builds.
// 1.5.0.0  06/19/2007  EFW  Various additions and updates for the June CTP
// 1.6.0.1  10/30/2007  EFW  Fixed the logo placement for the VS2005 style
// 1.6.0.3  06/20/2007  EFW  Fixed bug that caused code blocks with an unknown or unspecified language to always
//                           be hidden.
// 1.6.0.7  03/24/2008  EFW  Updated to handle multiple assembly versions. Updated to support use in conceptual
//                           builds.
// 1.7.0.0  06/01/2008  EFW  Removed language filter support for Hana and Prototype due to changes in the way the
//                           transformations implement it.
// 1.9.0.0  06/06/2010  EFW  Replaced outputPath element with an outputPaths element that supports multiple help
//                           file format output locations.
// 1.9.3.4  02/21/2012  EFW  Merged changes from Don Fehr for VS2010 style
// 1.9.6.0  10/14/2012  EFW  Moved version info and Prototype language filter stuff into the Sandcastle XSL
//                           transformations and removed it from here.  Move the code block relaetd code into
//                           the CodeBlockComponent's new event handler.
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Diagnostics;
using System.Globalization;
using System.IO;
using System.Reflection;
using System.Windows.Forms;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;

namespace SandcastleBuilder.Components
{
    /// <summary>
    /// This build component is used to insert a logo image at the top of each help topic if wanted.
    /// </summary>
    /// <example>
    /// <code lang="xml" title="Example configuration">
    /// &lt;!-- Post-transform component configuration.  This must
    ///      appear after the TransformComponent.  See also:
    ///      CodeBlockComponent. --&gt;
    /// &lt;component type="SandcastleBuilder.Components.PostTransformComponent"
    ///   assembly="C:\SandcastleComponents\SandcastleBuilder.Components.dll" &gt;
    ///     &lt;!-- Base output paths for the files (required).  These should
    ///          match the parent folder of the output path of the HTML files
    ///          (see each of the SaveComponent instances below). --&gt;
    ///     &lt;outputPaths&gt;
    ///       &lt;path value="Output\HtmlHelp1\" /&gt;
    ///       &lt;path value="Output\MSHelp2\" /&gt;
    ///       &lt;path value="Output\MSHelpViewer\" /&gt;
    ///       &lt;path value="Output\Website\" /&gt;
    ///     &lt;/outputPaths&gt;
    ///
    ///     &lt;!-- Logo image file (optional).  Filename is required.  The
    ///          height, width, altText, placement, and alignment attributes
    ///          are optional. --&gt;
    ///     &lt;logoFile filename="Logo.jpg" height="64" width="64"
    ///        altText="Test Logo" placement="left" alignment="left" /&gt;
    /// &lt;/component&gt;
    /// </code>
    /// </example>
    public class PostTransformComponent : BuildComponent
    {
        #region Logo placement enumerations
        //=====================================================================

        /// <summary>
        /// This enumeration defines the logo placement options
        /// </summary>
        public enum LogoPlacement
        {
            /// <summary>Place the logo to the left of the header text
            /// (the default).</summary>
            Left,
            /// <summary>Place the logo to the right of the header text.</summary>
            Right,
            /// <summary>Place the logo above the header text.</summary>
            Above
        }

        /// <summary>
        /// This enumeration defines the logo alignment options when placement
        /// is set to <b>Above</b>.
        /// </summary>
        public enum LogoAlignment
        {
            /// <summary>Left-align the logo (the default).</summary>
            Left,
            /// <summary>Right-align the logo.</summary>
            Right,
            /// <summary>Center the logo.</summary>
            Center
        }
        #endregion

        #region Private data members
        //=====================================================================

        // Output folder paths
        private List<string> outputPaths;

        // Logo properties
        private string logoFilename, logoAltText, alignment;
        private int logoHeight, logoWidth;
        private LogoPlacement placement;
        private bool logoFileCopied;
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembler">A reference to the build assembler.</param>
        /// <param name="configuration">The configuration information</param>
        /// <exception cref="ConfigurationErrorsException">This is thrown if
        /// an error is detected in the configuration.</exception>
        public PostTransformComponent(BuildAssembler assembler,
          XPathNavigator configuration) : base(assembler, configuration)
        {
            XPathNavigator nav;
            string attr;

            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            base.WriteMessage(MessageLevel.Info, "\r\n    [{0}, version {1}]\r\n    Post-Transform Component. " +
                "{2}\r\n    http://SHFB.CodePlex.com", fvi.ProductName, fvi.ProductVersion, fvi.LegalCopyright);

            outputPaths = new List<string>();

            // This element is obsolete, if found, tell the user to edit and save the configuration
            nav = configuration.SelectSingleNode("outputPath");

            if(nav != null)
                throw new ConfigurationErrorsException("The PostTransformComponent configuration contains an " +
                    "obsolete <outputPath> element.  Please edit the configuration to update it with the new " +
                    "<outputPaths> element.");

            // Get the output paths
            foreach(XPathNavigator path in configuration.Select("outputPaths/path"))
            {
                attr = path.GetAttribute("value", String.Empty);

                if(attr[attr.Length - 1] != '\\')
                    attr += @"\";

                if(!Directory.Exists(attr))
                    throw new ConfigurationErrorsException("The output path '" + attr + "' must exist");

                outputPaths.Add(attr);
            }

            if(outputPaths.Count == 0)
                throw new ConfigurationErrorsException("You must specify at least one <path> element in the " +
                    "<outputPaths> element");

            // The logo element is optional.  The file must exist if specified.
            nav = configuration.SelectSingleNode("logoFile");

            if(nav != null)
            {
                logoFilename = nav.GetAttribute("filename", String.Empty);

                if(!String.IsNullOrEmpty(logoFilename))
                {
                    if(!File.Exists(logoFilename))
                        throw new ConfigurationErrorsException("The logo file '" + logoFilename + "' must exist");

                    logoAltText = nav.GetAttribute("altText", String.Empty);

                    attr = nav.GetAttribute("height", String.Empty);

                    if(!String.IsNullOrEmpty(attr))
                        if(!Int32.TryParse(attr, out logoHeight))
                            throw new ConfigurationErrorsException("The logo height must be an integer value");

                    attr = nav.GetAttribute("width", String.Empty);

                    if(!String.IsNullOrEmpty(attr))
                        if(!Int32.TryParse(attr, out logoWidth))
                            throw new ConfigurationErrorsException("The logo width must be an integer value");

                    // Ignore them if negative
                    if(logoHeight < 0)
                        logoHeight = 0;

                    if(logoWidth < 0)
                        logoWidth = 0;

                    // Placement and alignment are optional
                    attr = nav.GetAttribute("placement", String.Empty);

                    if(!String.IsNullOrEmpty(attr))
                        placement = (LogoPlacement)Enum.Parse(typeof(LogoPlacement), attr, true);
                    else
                        placement = LogoPlacement.Left;

                    attr = nav.GetAttribute("alignment", String.Empty);

                    if(!String.IsNullOrEmpty(attr))
                        alignment = attr;
                    else
                        alignment = "left";
                }
            }
        }
        #endregion

        #region Apply the component
        //=====================================================================

        /// <summary>
        /// This is implemented to perform the post-transformation tasks.
        /// </summary>
        /// <param name="document">The XML document with which to work.</param>
        /// <param name="key">The key (member name) of the item being
        /// documented.</param>
        public override void Apply(XmlDocument document, string key)
        {
            // Add the logo?
            if(!String.IsNullOrEmpty(logoFilename))
                this.AddLogo(document);
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// This is called to add the logo to the page header area
        /// </summary>
        /// <param name="document">The document to which the logo is added.</param>
        private void AddLogo(XmlDocument document)
        {
            XmlAttribute attr;
            XmlNode div, divHeader, devLangsMenu, bottomTable, memberOptionsMenu, memberFrameworksMenu,
                gradientTable;
            string imgWidth, imgHeight, imgAltText, filename, destFile;

            filename = Path.GetFileName(logoFilename);

            // TODO: Move to constructor and copy if a logo file is specified
            if(!logoFileCopied)
            {
                foreach(string outputPath in outputPaths)
                {
                    destFile = outputPath + @"icons\" + filename;

                    // Copy the logo to the icons folder if not there already.
                    // All attributes are turned off so that we can delete it later.
                    if(!File.Exists(destFile))
                    {
                        if(!Directory.Exists(outputPath + @"icons"))
                            Directory.CreateDirectory(outputPath + @"icons");

                        File.Copy(logoFilename, destFile);
                        File.SetAttributes(destFile, FileAttributes.Normal);
                    }
                }

                logoFileCopied = true;
            }

            imgAltText = (String.IsNullOrEmpty(logoAltText)) ? String.Empty : " alt='" + logoAltText + "'";
            imgWidth = (logoWidth == 0) ? String.Empty : " width='" +
                logoWidth.ToString(CultureInfo.InvariantCulture) + "'";
            imgHeight = (logoHeight == 0) ? String.Empty : " height='" +
                logoHeight.ToString(CultureInfo.InvariantCulture) + "'";

            div = document.SelectSingleNode("//div[@id='control']");

            // Prototype style?
            if(div != null)
            {
                // Wrap the header <div> in a table with the image based on the placement option
                switch(placement)
                {
                    case LogoPlacement.Left:
                        div.InnerXml = String.Format(CultureInfo.InvariantCulture,
                            "<table border='0' width='100%' cellpadding='0' " +
                            "cellspacing='0'><tr><td align='center' " +
                            "style='padding-right: 10px'><img {0}{1}{2}><includeAttribute name='src' " +
                            "item='iconPath'><parameter>{3}</parameter></includeAttribute></img></td>" +
                            "<td valign='top' width='100%'>{4}</td></tr></table>", imgAltText, imgWidth,
                            imgHeight, filename, div.InnerXml);
                        break;

                    case LogoPlacement.Right:
                        div.InnerXml = String.Format(CultureInfo.InvariantCulture,
                            "<table border='0' width='100%' cellpadding='0' " +
                            "cellspacing='0'><tr><td valign='top' " +
                            "width='100%'>{0}</td><td align='center' " +
                            "style='padding-left: 10px'><img {1}{2}{3}><includeAttribute name='src' " +
                            "item='iconPath'><parameter>{4}</parameter></includeAttribute></img>" +
                            "</td></tr></table>", div.InnerXml, imgAltText, imgWidth, imgHeight, filename);
                        break;

                    case LogoPlacement.Above:
                        div.InnerXml = String.Format(CultureInfo.InvariantCulture,
                            "<table border='0' width='100%' cellpadding='0' " +
                            "cellspacing='0'><tr><td align='{0}' " +
                            "style='padding-bottom: 5px'><img {1}{2}{3}><includeAttribute name='src' " +
                            "item='iconPath'><parameter>{4}</parameter></includeAttribute></img></td></tr>" +
                            "<tr><td valign='top' width='100%'>{5}</td></tr>" +
                            "</table>", alignment, imgAltText, imgWidth, imgHeight, filename, div.InnerXml);
                        break;
                }
            }
            else
            {
                // VS2010/VS2005/Hana style
                div = document.SelectSingleNode("//table[@id='topTable']");

                if(div == null)
                {
                    base.WriteMessage(MessageLevel.Error, "Unable to locate 'control' <div> or 'topTable' " +
                        "<table> to insert logo.");
                    return;
                }

                // VS2010 style?
                XmlNode runningHeaderNode = div.SelectSingleNode("//td[@id='runningHeaderColumn']");

                if(runningHeaderNode != null)
                {
                    // TODO: This should change to support all options since we are no longer using branding
                    //       and can do what we like.

                    // LogoPlacement is ignored for the VS2010 style because the style itself 
                    // defines a specific placement for the logo.

                    runningHeaderNode.InnerXml = String.Format(CultureInfo.InvariantCulture,
                        "<img align='right' {0}{1}{2}><includeAttribute name='src' item='iconPath'>" +
                        "<parameter>{3}</parameter></includeAttribute></img>",
                        imgAltText, imgWidth, imgHeight, filename);
                }
                else
                {
                    switch(placement)
                    {
                        case LogoPlacement.Left:
                            // Hana style?
                            if(div.ChildNodes.Count != 1)
                            {
                                // Insert a new row with a cell spanning all rows
                                div.InnerXml = String.Format(CultureInfo.InvariantCulture,
                                    "<tr><td rowspan='4' align='center' style='width: 1px; padding: 0px'>" +
                                    "<img {0}{1}{2}><includeAttribute name='src' item='iconPath'>" +
                                    "<parameter>{3}</parameter></includeAttribute></img></td></tr>{4}",
                                    imgAltText, imgWidth, imgHeight, filename, div.InnerXml);

                                attr = document.CreateAttribute("colspan");
                                attr.Value = "2";
                                div.ChildNodes[4].ChildNodes[0].Attributes.Append(attr);
                            }
                            else
                            {
                                // VS2005 style.  Wrap the top table, dev lang menu and bottom table in a new
                                // table with a new cell on the left containing the logo.
                                divHeader = div.ParentNode;
                                devLangsMenu = div.NextSibling;
                                bottomTable = devLangsMenu.NextSibling;
                                memberOptionsMenu = memberFrameworksMenu = null;

                                if(bottomTable.Attributes["id"].Value == "memberOptionsMenu")
                                {
                                    memberOptionsMenu = bottomTable;
                                    bottomTable = bottomTable.NextSibling;

                                    if(bottomTable.Attributes["id"].Value == "memberFrameworksMenu")
                                    {
                                        memberFrameworksMenu = bottomTable;
                                        bottomTable = bottomTable.NextSibling;
                                    }
                                }

                                if(bottomTable.Attributes["id"].Value == "gradientTable")
                                {
                                    gradientTable = bottomTable;
                                    bottomTable = null;
                                }
                                else
                                    gradientTable = bottomTable.NextSibling;

                                divHeader.InnerXml = String.Format(CultureInfo.InvariantCulture,
                                    "<table cellspacing='0' cellpadding='0'><tr>" +
                                    "<td align='center' style='width: 1px; padding: 0px'>" +
                                    "<img {0}{1}{2}><includeAttribute name='src' item='iconPath'>" +
                                    "<parameter>{3}</parameter></includeAttribute></img></td>" +
                                    "<td>{4}{5}{6}{7}{8}</td></tr></table>{9}", imgAltText, imgWidth, imgHeight,
                                    filename, div.OuterXml, devLangsMenu.OuterXml,
                                    (memberOptionsMenu == null) ? String.Empty : memberOptionsMenu.OuterXml,
                                    (memberFrameworksMenu == null) ? String.Empty : memberFrameworksMenu.OuterXml,
                                    (bottomTable == null) ? String.Empty : bottomTable.OuterXml,
                                    gradientTable.OuterXml);
                            }
                            break;

                        case LogoPlacement.Right:
                            // Hana style?
                            if(div.ChildNodes.Count != 1)
                            {
                                // For this, we add a second cell to the first row that spans three rows
                                div = div.ChildNodes[0];
                                div.InnerXml += String.Format(CultureInfo.InvariantCulture,
                                    "<td rowspan='3' align='center' style='width: 1px; padding: 0px'>" +
                                    "<img {0}{1}{2}><includeAttribute name='src' item='iconPath'>" +
                                    "<parameter>{3}</parameter></includeAttribute></img></td>",
                                    imgAltText, imgWidth, imgHeight, filename);

                                // For Hana, we need to add a colspan attribute to the last row
                                div = div.ParentNode;

                                attr = document.CreateAttribute("colspan");
                                attr.Value = "2";
                                div.ChildNodes[3].ChildNodes[0].Attributes.Append(attr);
                            }
                            else
                            {
                                // VS2005 style.  Wrap the top table, dev lang menu and bottom table in a new
                                // table with a new cell on the right containing the logo.
                                divHeader = div.ParentNode;
                                devLangsMenu = div.NextSibling;
                                bottomTable = devLangsMenu.NextSibling;
                                memberOptionsMenu = memberFrameworksMenu = null;

                                if(bottomTable.Attributes["id"].Value == "memberOptionsMenu")
                                {
                                    memberOptionsMenu = bottomTable;
                                    bottomTable = bottomTable.NextSibling;

                                    if(bottomTable.Attributes["id"].Value == "memberFrameworksMenu")
                                    {
                                        memberFrameworksMenu = bottomTable;
                                        bottomTable = bottomTable.NextSibling;
                                    }
                                }

                                if(bottomTable.Attributes["id"].Value == "gradientTable")
                                {
                                    gradientTable = bottomTable;
                                    bottomTable = null;
                                }
                                else
                                    gradientTable = bottomTable.NextSibling;


                                divHeader.InnerXml = String.Format(CultureInfo.InvariantCulture,
                                    "<table cellspacing='0' cellpadding='0'><tr><td>{4}{5}{6}{7}{8}</td>" +
                                    "<td align='center' style='width: 1px; padding: 0px'>" +
                                    "<img {0}{1}{2}><includeAttribute name='src' item='iconPath'>" +
                                    "<parameter>{3}</parameter></includeAttribute></img></td></tr></table>{9}",
                                    imgAltText, imgWidth, imgHeight, filename, div.OuterXml, devLangsMenu.OuterXml,
                                    (memberOptionsMenu == null) ? String.Empty : memberOptionsMenu.OuterXml,
                                    (memberFrameworksMenu == null) ? String.Empty : memberFrameworksMenu.OuterXml,
                                    (bottomTable == null) ? String.Empty : bottomTable.OuterXml,
                                    gradientTable.OuterXml);
                            }
                            break;

                        case LogoPlacement.Above:
                            // Add a new first row
                            div.InnerXml = String.Format(CultureInfo.InvariantCulture,
                                "<tr><td align='{0}'><img {1}{2}{3}><includeAttribute name='src' " +
                                "item='iconPath'><parameter>{4}</parameter></includeAttribute></img></td></tr>{5}",
                                alignment, imgAltText, imgWidth, imgHeight, filename, div.InnerXml);
                            break;
                    }
                }
            }
        }
        #endregion

        #region Static configuration method for use with SHFB
        //=====================================================================

        /// <summary>
        /// This static method is used by the Sandcastle Help File Builder to
        /// let the component perform its own configuration.
        /// </summary>
        /// <param name="currentConfig">The current configuration XML fragment</param>
        /// <returns>A string containing the new configuration XML fragment</returns>
        public static string ConfigureComponent(string currentConfig)
        {
            using(PostTransformConfigDlg dlg = new PostTransformConfigDlg(currentConfig))
            {
                if(dlg.ShowDialog() == DialogResult.OK)
                    currentConfig = dlg.Configuration;
            }

            return currentConfig;
        }
        #endregion
    }
}
