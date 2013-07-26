//=============================================================================
// System  : Sandcastle Build Components
// File    : ResolveConceptualLinksComponent.cs
// Note    : Copyright 2010-2012 Microsoft Corporation
//
// This file contains a modified version of the original
// ResolveConceptualLinksComponent that allows the use of inner text from the
// <link> tag and also allows the use of anchor references (#anchorName) in the
// link target.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy
// of the license should be distributed with the code.  It can also be found
// at the project website: http://Sandcastle.CodePlex.com.   This notice and
// all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Change History
// 02/16/2012 - EFW - Merged my changes into the code
//=============================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text.RegularExpressions;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This is a modified version of the original <c>ResolveConceptualLinksComponent</c> that is used
    /// to resolve links to conceptual topics.
    /// </summary>
    /// <remarks>This version contains the following improvements and fixes:
    /// <list type="bullet">
    ///   <item>Broken links use the <b>None</b> style rather than the
    /// <b>Index</b> style so that it is apparant that they do not work.</item>
    ///   <item>The inner text from the conceptual link is used if specified.</item>
    ///   <item>On broken links, when the <c>showBrokenLinkText</c> option
    /// is true and there is no inner text, the target value is displayed.</item>
    ///   <item>Conceptual link targets can include an optional anchor name
    /// from within the target such as "#Name" (see examples below).</item>
    ///   <item>Unnecessary whitespace is removed from the link text.</item>
    ///   <item>If the companion file contains a <c>&lt;linkText&gt;</c>
    /// element and no inner text is specified, its value will be used for the
    /// link text rather than the title.  This allows for a shorter title or
    /// description to use as the default link text.</item>
    /// </list></remarks>
    /// <example>
    /// On links without inner text, if the companion file contains a
    /// <c>linkText</c> element, that text will be used.  If not, the title
    /// is used.
    ///
    /// <code lang="xml" title="Example Links">
    /// <![CDATA[<!-- Link with inner text -->
    /// <link xlink:href="3ab3113f-984b-19ac-7812-990192aca5b0">Click Here</link>
    /// <!-- Link with anchor reference -->
    /// <link xlink:href="3ab3113f-984b-19ac-7812-990192aca5b1#SubTopic" />
    /// <!-- Link with inner text and an anchor reference -->
    /// <link xlink:href="3ab3113f-984b-19ac-7812-990192aca5b1#PropA">PropertyA</link>]]>
    /// </code>
    /// 
    /// <code lang="xml" title="Example configuration">
    /// &lt;!-- Resolve conceptual links --&gt;
    /// &lt;component type="Microsoft.Ddue.Tools.ResolveConceptualLinksComponent"
    ///   assembly="%DXROOT%\ProductionTools\BuildComponents.dll"
    ///   showBrokenLinkText="true"&gt;
    ///     &lt;targets base="xmlComp" type="local" /&gt;
    /// &lt;/component&gt;
    /// </code>
    /// </example>
    public class ResolveConceptualLinksComponent : BuildComponent
    {
        #region Private data members
        //=====================================================================

        private Dictionary<string, TargetInfo> cache;
        private TargetDirectoryCollection targetDirectories;
        private bool showBrokenLinkText;

        private static XPathExpression conceptualLinks = XPathExpression.Compile("//conceptualLink");

        private static Regex validGuid = new Regex(
            "^[0-9a-fA-F]{8}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{4}-[0-9a-fA-F]{12}$");

        private static int cacheSize = 1000;
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembler">A reference to the build assembler.</param>
        /// <param name="configuration">The configuration information</param>
        public ResolveConceptualLinksComponent(BuildAssembler assembler,
          XPathNavigator configuration) : base(assembler, configuration)
        {
            TargetDirectory targetDirectory;
            XPathExpression urlExp, textExp, linkTextExp;
            LinkType linkType = LinkType.None;
            string attribute, basePath;

            targetDirectories = new TargetDirectoryCollection();
            cache = new Dictionary<string, TargetInfo>(cacheSize);

            attribute = configuration.GetAttribute("showBrokenLinkText", String.Empty);

            if(!String.IsNullOrEmpty(attribute))
                showBrokenLinkText = Convert.ToBoolean(attribute, CultureInfo.InvariantCulture);

            foreach(XPathNavigator navigator in configuration.Select("targets"))
            {
                basePath = navigator.GetAttribute("base", String.Empty);

                if(String.IsNullOrEmpty(basePath))
                    base.WriteMessage(MessageLevel.Error, "Every targets " +
                        "element must have a base attribute that specifies " +
                        "the path to a directory of target metadata files.");

                basePath = Environment.ExpandEnvironmentVariables(basePath);
                if(!Directory.Exists(basePath))
                    base.WriteMessage(MessageLevel.Error, String.Format(
                        CultureInfo.InvariantCulture, "The specified target " +
                        "metadata directory '{0}' does not exist.", basePath));

                attribute = navigator.GetAttribute("url", String.Empty);

                if(String.IsNullOrEmpty(attribute))
                    urlExp = XPathExpression.Compile("concat(/metadata/topic/@id,'.htm')");
                else
                    urlExp = this.CompileXPathExpression(attribute);

                attribute = navigator.GetAttribute("text", String.Empty);

                if(String.IsNullOrEmpty(attribute))
                    textExp = XPathExpression.Compile("string(/metadata/topic/title)");
                else
                    textExp = this.CompileXPathExpression(attribute);

                // EFW - Added support for linkText option
                attribute = navigator.GetAttribute("linkText", String.Empty);

                if(String.IsNullOrEmpty(attribute))
                    linkTextExp = XPathExpression.Compile("string(/metadata/topic/linkText)");
                else
                    linkTextExp = this.CompileXPathExpression(attribute);

                attribute = navigator.GetAttribute("type", String.Empty);

                if(String.IsNullOrEmpty(attribute))
                    base.WriteMessage(MessageLevel.Error, "Every targets " +
                        "element must have a type attribute that specifies " +
                        "what kind of link to create to targets found in " +
                        "that directory.");

                try
                {
                    linkType = (LinkType)Enum.Parse(typeof(LinkType), attribute, true);
                }
                catch(ArgumentException)
                {
                    base.WriteMessage(MessageLevel.Error, String.Format(CultureInfo.InvariantCulture,
                        "'{0}' is not a valid link type.", attribute));
                }

                targetDirectory = new TargetDirectory(basePath, urlExp, textExp, linkTextExp, linkType);
                targetDirectories.Add(targetDirectory);
            }

            base.WriteMessage(MessageLevel.Info, String.Format(CultureInfo.InvariantCulture,
                "Collected {0} targets directories.", targetDirectories.Count));
        }
        #endregion

        #region Apply method
        //=====================================================================

        /// <summary>
        /// This is implemented to resolve the conceptual links
        /// </summary>
        /// <param name="document">The XML document with which to work.</param>
        /// <param name="key">The key (member name) of the item being
        /// documented.</param>
        public override void Apply(XmlDocument document, string key)
        {
            ConceptualLinkInfo info;
            TargetInfo targetInfo;
            LinkType linkType;
            string url, text;

            foreach(XPathNavigator navigator in BuildComponentUtilities.ConvertNodeIteratorToArray(
              document.CreateNavigator().Select(conceptualLinks)))
            {
                info = ConceptualLinkInfo.Create(navigator);
                url = text = null;
                linkType = LinkType.None;

                if(validGuid.IsMatch(info.Target))
                {
                    targetInfo = this.GetTargetInfoFromCache(info.Target.ToLower(CultureInfo.InvariantCulture));

                    if(targetInfo == null)
                    {
                        // EFW - Removed linkType = Index, broken links should use the None style.
                        text = this.BrokenLinkDisplayText(info.Target, info.Text);
                        base.WriteMessage(MessageLevel.Warn, String.Format(CultureInfo.InvariantCulture,
                            "Unknown conceptual link target '{0}'.", info.Target));
                    }
                    else
                    {
                        url = targetInfo.Url;

                        // EFW - Append the anchor if one was specified
                        if(!String.IsNullOrEmpty(info.Anchor))
                            url += info.Anchor;

                        // EFW - Use the link text if specified
                        if(!String.IsNullOrEmpty(info.Text))
                            text = info.Text;
                        else
                            text = targetInfo.Text;

                        linkType = targetInfo.LinkType;
                    }
                }
                else
                {
                    // EFW - Removed linkType = Index, broken links should use the None style.
                    text = this.BrokenLinkDisplayText(info.Target, info.Text);
                    base.WriteMessage(MessageLevel.Warn, String.Format(CultureInfo.InvariantCulture,
                        "Invalid conceptual link target '{0}'.", info.Target));
                }

                XmlWriter writer = navigator.InsertAfter();

                switch(linkType)
                {
                    case LinkType.None:
                        writer.WriteStartElement("span");
                        writer.WriteAttributeString("class", "nolink");
                        break;

                    case LinkType.Local:
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", url);
                        break;

                    case LinkType.Index:
                        writer.WriteStartElement("mshelp", "link", "http://msdn.microsoft.com/mshelp");
                        writer.WriteAttributeString("keywords", info.Target.ToLower(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("tabindex", "0");
                        break;

                    case LinkType.Id:
                        writer.WriteStartElement("a");
                        writer.WriteAttributeString("href", String.Format(CultureInfo.InvariantCulture,
                            "ms-xhelp:///?Id={0}", info.Target));
                        break;
                }

                writer.WriteString(text);
                writer.WriteEndElement();
                writer.Close();

                navigator.DeleteSelf();
            }
        }
        #endregion

        #region Helper methods
        //=====================================================================

        /// <summary>
        /// Determine what to display for broken links
        /// </summary>
        /// <param name="target">The target key</param>
        /// <param name="text">The link text</param>
        /// <returns>The text to display for the broken link</returns>
        private string BrokenLinkDisplayText(string target, string text)
        {
            // EFW - If true but text is empty, use the target
            if(showBrokenLinkText && !String.IsNullOrEmpty(text))
                return text;

            return String.Format(CultureInfo.InvariantCulture, "[{0}]", target);
        }

        /// <summary>
        /// Compile an XPath expression and report an error if it fails
        /// </summary>
        /// <param name="xpath">The XPath expression to compile.</param>
        /// <returns>The compiled XPath expression.</returns>
        private XPathExpression CompileXPathExpression(string xpath)
        {
            XPathExpression expression = null;

            try
            {
                expression = XPathExpression.Compile(xpath);
            }
            catch(ArgumentException argEx)
            {
                base.WriteMessage(MessageLevel.Error, String.Format(CultureInfo.InvariantCulture,
                    "'{0}' is not a valid XPath expression. The error message is: {1}", xpath, argEx.Message));
            }
            catch(XPathException xpathEx)
            {
                base.WriteMessage(MessageLevel.Error, String.Format(CultureInfo.InvariantCulture,
                    "'{0}' is not a valid XPath expression. The error message is: {1}", xpath, xpathEx.Message));
            }

            return expression;
        }

        /// <summary>
        /// Get target info
        /// </summary>
        /// <param name="target">The target for which to get info</param>
        /// <returns>The target info object if found or null if not found</returns>
        private TargetInfo GetTargetInfoFromCache(string target)
        {
            TargetInfo targetInfo;

            if(!cache.TryGetValue(target, out targetInfo))
            {
                targetInfo = targetDirectories.GetTargetInfo(target + ".cmp.xml");

                if(cache.Count >= cacheSize)
                    cache.Clear();

                cache.Add(target, targetInfo);
            }

            return targetInfo;
        }
        #endregion
    }
}
