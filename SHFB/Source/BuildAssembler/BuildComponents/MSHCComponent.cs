﻿//===============================================================================================================
// System  : Sandcastle Build Components
// File    : MSHCComponent.cs
// Note    : Copyright 2010-2012 Microsoft Corporation
//
// This file contains a modified version of the original MSHCComponent that allows the inclusion of a sortOrder
// attribute on the table of contents file elements.  This allows the sort order of the elements to be defined
// to set the proper placement of the TOC entries when parented to an entry outside of the help file and to
// parent the API content within a conceptual content folder.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice and all copyright notices must remain intact in all applications, documentation, and source files.
//
// Change History
// 02/16/2012 - EFW - Merged my changes into the code
// 09/28/2012 - EFW - Changed "SelfBranded" to "Microsoft.Help.SelfBranded" for Help Viewer 2.0 support.
//                    Removed the ContentType metadata as it's output by the XSL transformations.
//                    Removed header bottom fix up code as it is handled in the XSL transofmrations and script.
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This class is a modified version of the original <c>MSHCComponent</c> that
    /// is used to add MS Help Viewer meta data to the topics.  This version allows
    /// the inclusion of a <c>sortOrder</c> attribute on the table of contents
    /// file elements.  This allows the sort order of the elements to be defined
    /// to set the proper placement of the TOC entries when parented to an entry
    /// outside of the help file and to parent the API content within a conceptual
    /// content folder.
    /// </summary>
    /// <remarks>The <c>sortOrder</c> attributes are optional.  If not found,
    /// standard ordering is applied starting from zero.  If a <c>sortOrder</c>
    /// attribute is found, numbering starts from that value for the associated
    /// topic and increments by one for all subsequent topics until another
    /// <c>sortOrder</c> attribute is encountered or the end of the group is
    /// reached.
    /// </remarks>
    /// <example>
    /// <code lang="xml" title="Example Component Configuration">
    /// &lt;component type="Microsoft.Ddue.Tools.MSHCComponent"
    ///   assembly="%SHFBROOT%\BuildComponents.dll"&gt;
    ///   &lt;data self-branded="true" topic-version="100" toc-file="toc.xml"
    ///   toc-parent="" toc-parent-version="100" locale="en-US" /&gt;
    /// &lt;/component&gt;
    /// </code>
    ///
    /// <code lang="xml" title="Example toc.xml File">
    /// &lt;?xml version="1.0" encoding="utf-8"?&gt;
    /// &lt;topics&gt;
    ///   &lt;!-- Sort our content below that of the parent node's existing sub-topics --&gt;
    ///   &lt;topic id="d4648875-d41a-783b-d5f4-638df39ee413" file="d4648875-d41a-783b-d5f4-638df39ee413" sortOrder="100"&gt;
    ///     &lt;topic id="57f7aedc-17d3-4547-bdf9-5b468a08a1bc" file="57f7aedc-17d3-4547-bdf9-5b468a08a1bc" /&gt;
    ///     &lt;topic id="0e6bbd29-775a-8deb-c4f5-5b1e63349ef1" file="0e6bbd29-775a-8deb-c4f5-5b1e63349ef1" /&gt;
    ///     &lt;topic id="fcdfafc4-7625-f407-d8e9-ec006944e1d7" file="fcdfafc4-7625-f407-d8e9-ec006944e1d7" /&gt;
    ///     &lt;!-- API content (7 namespaces, merged later) goes here and this topic follows it --&gt;
    ///     &lt;topic id="ce37cf86-fd95-49fc-b048-ba7d25d68d87" file="ce37cf86-fd95-49fc-b048-ba7d25d68d87" sortOrder="10"&gt;
    ///   &lt;/topic&gt;
    ///   .
    ///   .
    ///   .
    /// &lt;/topics&gt;
    /// </code>
    /// </example>
    public class MSHCComponent : BuildComponent
    {
        #region Constants definitions
        //=====================================================================

        // EFW - Made all constants classes static.

        // Component tag names in the configuration file
        private static class ConfigurationTag
        {
            public const string Data = "data";
        }

        // Component attribute names in the configuration file
        private static class ConfigurationAttr
        {
            public const string Locale = "locale";
            public const string SelfBranded = "self-branded";
            public const string TopicVersion = "topic-version";
            public const string TocFile = "toc-file";
            public const string TocParent = "toc-parent";
            public const string TocParentVersion = "toc-parent-version";
        }

        // XPath expressions to navigate the TOC file
        private static class TocXPath
        {
            public const string Topics = "/topics";
            public const string Topic = "topic";
        }

        // Attribute names in the TOC file
        private static class TocAttr
        {
            public const string Id = "id";
            public const string SortOrder = "sortOrder";    // EFW - Added sort order
        }

        // Microsoft Help 2.0 namespace info
        private static class Help2Namespace
        {
            public const string Prefix = "MSHelp";
            public const string Uri = "http://msdn.microsoft.com/mshelp";
        }

        // XPath expressions to navigate Microsoft Help 2.0 data in the document
        private static class Help2XPath
        {
            public const string Head = "head";
            public const string Xml = "xml";
            public const string TocTitle = "MSHelp:TOCTitle";
            public const string Attr = "MSHelp:Attr[@Name='{0}']";
            public const string Keyword = "MSHelp:Keyword[@Index='{0}']";

            // !EFW - Added to remove unnecessary Help 2 CSS link element
            public const string HxLink = "link[@href='ms-help://Hx/HxRuntime/HxLink.css']";
        }

        // Microsoft Help 2.0 tag attributes in the document
        private static class Help2Attr
        {
            public const string Value = "Value";
            public const string Term = "Term";
            public const string Title = "Title";
        }

        // Microsoft Help 2.0 attribute values in the document
        private static class Help2Value
        {
            public const string K = "K";
            public const string F = "F";
            public const string Locale = "Locale";
            public const string AssetID = "AssetID";
            public const string DevLang = "DevLang";
            public const string Abstract = "Abstract";
        }

        // Microsoft Help System tags
        private static class MHSTag
        {
            public const string Meta = "meta";
        }

        // Microsoft Help System meta tag attributes
        private static class MHSMetaAttr
        {
            public const string Name = "name";
            public const string Content = "content";
        }

        // Microsoft Help System meta names
        private static class MHSMetaName
        {
            public const string SelfBranded = "Microsoft.Help.SelfBranded";
            public const string Locale = "Microsoft.Help.Locale";
            public const string TopicLocale = "Microsoft.Help.TopicLocale";
            public const string Id = "Microsoft.Help.Id";
            public const string TopicVersion = "Microsoft.Help.TopicVersion";
            public const string TocParent = "Microsoft.Help.TocParent";
            public const string TocParentVersion = "Microsoft.Help.TOCParentTopicVersion";
            public const string TocOrder = "Microsoft.Help.TocOrder";
            public const string Title = "Title";
            public const string Keywords = "Microsoft.Help.Keywords";
            public const string F1 = "Microsoft.Help.F1";
            public const string Category = "Microsoft.Help.Category";
            public const string Description = "Description";
        }

        // Microsoft Help System meta default values 
        private static class MHSDefault
        {
            public const bool SelfBranded = true;
            public const string Locale = "en-us";
            public const string TopicVersion = "100";
            public const string TocParent = "-1";
            public const string TocParentVersion = "100";
            public const string TocFile = "./toc.xml";
            public const string ShortName = "MHS";
        }
        #endregion

        #region TOC information class
        //=====================================================================

        // TOC information of a document
        private class TocInfo
        {
            private string _parent;
            private string _parentVersion;
            private int _order;

            public TocInfo(string parent, string parentVersion, int order)
            {
                _parent = parent;
                _parentVersion = parentVersion;
                _order = order;
            }

            public string Parent { get { return _parent; } }
            public string ParentVersion { get { return _parentVersion; } }
            public int Order { get { return _order; } }
        }
        #endregion

        #region Private data members
        //=====================================================================

        private XmlDocument _document;
        private XmlNode _head;
        private XmlNode _xml;

        private string _locale = String.Empty;
        private bool _selfBranded = MHSDefault.SelfBranded;
        private string _topicVersion = MHSDefault.TopicVersion;
        private string _tocParent = MHSDefault.TocParent;
        private string _tocParentVersion = MHSDefault.TocParentVersion;
        private Dictionary<string, TocInfo> _toc = new Dictionary<string, TocInfo>();
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Creates a new instance of the <see cref="MSHCComponent"/> class.
        /// </summary>
        /// <param name="assembler">The active <see cref="BuildAssembler"/>.</param>
        /// <param name="configuration">The current <see cref="XPathNavigator"/> of the configuration.</param>
        public MSHCComponent(BuildAssembler assembler, XPathNavigator configuration) :
          base(assembler, configuration)
        {
            string tocFile = MHSDefault.TocFile;
            XPathNavigator data = configuration.SelectSingleNode(ConfigurationTag.Data);

            if(data != null)
            {
                string value = data.GetAttribute(ConfigurationAttr.Locale, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    _locale = value;

                value = data.GetAttribute(ConfigurationAttr.SelfBranded, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    _selfBranded = bool.Parse(value);

                value = data.GetAttribute(ConfigurationAttr.TopicVersion, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    _topicVersion = value;

                value = data.GetAttribute(ConfigurationAttr.TocParent, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    _tocParent = value;

                value = data.GetAttribute(ConfigurationAttr.TocParentVersion, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    _tocParentVersion = value;

                value = data.GetAttribute(ConfigurationAttr.TocFile, String.Empty);

                if(!String.IsNullOrEmpty(value))
                    tocFile = value;
            }

            LoadToc(Path.GetFullPath(Environment.ExpandEnvironmentVariables(tocFile)));
        }
        #endregion

        #region Apply method
        //=====================================================================

        /// <summary>
        /// Applies Microsoft Help System transformation to the output document.
        /// </summary>
        /// <param name="document">The <see cref="XmlDocument"/> to apply transformation to.</param>
        /// <param name="key">Topic key of the output document.</param>
        public override void Apply(XmlDocument document, string key)
        {
            _document = document;

            XmlElement html = _document.DocumentElement;
            _head = html.SelectSingleNode(Help2XPath.Head);

            if(_head == null)
            {
                _head = document.CreateElement(Help2XPath.Head);

                if(!html.HasChildNodes)
                    html.AppendChild(_head);
                else
                    html.InsertBefore(_head, html.FirstChild);
            }
            else
            {
                // !EFW - Remove the unnecessary Help 2 CSS link element from the header
                XmlNode hxLink = _head.SelectSingleNode(Help2XPath.HxLink);

                if(hxLink != null)
                    _head.RemoveChild(hxLink);
            }

            // Apply some fix-ups if not branding aware
            if(_head.SelectSingleNode("meta[@name='BrandingAware']") == null)
            {
                ModifyAttribute("id", "mainSection");
                ModifyAttribute("class", "members");
            }

            AddMHSMeta(MHSMetaName.SelfBranded, _selfBranded.ToString().ToLowerInvariant());
            AddMHSMeta(MHSMetaName.TopicVersion, _topicVersion);

            string locale = _locale;
            string id = Guid.NewGuid().ToString();
            _xml = _head.SelectSingleNode(Help2XPath.Xml);

            if(_xml != null)
            {
                XmlNamespaceManager nsmgr = new XmlNamespaceManager(_document.NameTable);

                if(!nsmgr.HasNamespace(Help2Namespace.Prefix))
                    nsmgr.AddNamespace(Help2Namespace.Prefix, Help2Namespace.Uri);

                XmlElement elem = _xml.SelectSingleNode(Help2XPath.TocTitle, nsmgr) as XmlElement;

                if(elem != null)
                    AddMHSMeta(MHSMetaName.Title, elem.GetAttribute(Help2Attr.Title));

                foreach(XmlElement keyword in _xml.SelectNodes(String.Format(CultureInfo.InvariantCulture, Help2XPath.Keyword, Help2Value.K), nsmgr))
                    AddMHSMeta(MHSMetaName.Keywords, keyword.GetAttribute(Help2Attr.Term), true);

                foreach(XmlElement keyword in _xml.SelectNodes(String.Format(CultureInfo.InvariantCulture, Help2XPath.Keyword, Help2Value.F), nsmgr))
                    AddMHSMeta(MHSMetaName.F1, keyword.GetAttribute(Help2Attr.Term), true);

                foreach(XmlElement lang in _xml.SelectNodes(String.Format(CultureInfo.InvariantCulture, Help2XPath.Attr, Help2Value.DevLang), nsmgr))
                    AddMHSMeta(MHSMetaName.Category, Help2Value.DevLang + ":" + lang.GetAttribute(Help2Attr.Value), true);

                elem = _xml.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, Help2XPath.Attr, Help2Value.Abstract), nsmgr) as XmlElement;

                if(elem != null)
                    AddMHSMeta(MHSMetaName.Description, elem.GetAttribute(Help2Attr.Value));

                elem = _xml.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, Help2XPath.Attr, Help2Value.AssetID), nsmgr) as XmlElement;

                if(elem != null)
                    id = elem.GetAttribute(Help2Attr.Value);

                if(String.IsNullOrEmpty(locale))
                {
                    elem = _xml.SelectSingleNode(String.Format(CultureInfo.InvariantCulture, Help2XPath.Attr, Help2Value.Locale), nsmgr) as XmlElement;
                    if(elem != null)
                        locale = elem.GetAttribute(Help2Attr.Value);
                }

                // !EFW - Remove the XML data island as it serves no purpose
                _head.RemoveChild(_xml);
            }

            if(String.IsNullOrEmpty(locale))
                locale = MHSDefault.Locale;

            AddMHSMeta(MHSMetaName.Locale, locale);
            AddMHSMeta(MHSMetaName.TopicLocale, locale);
            AddMHSMeta(MHSMetaName.Id, id);

            if(_toc.ContainsKey(id))
            {
                TocInfo tocInfo = _toc[id];
                AddMHSMeta(MHSMetaName.TocParent, tocInfo.Parent);

                if(tocInfo.Parent != MHSDefault.TocParent)
                    AddMHSMeta(MHSMetaName.TocParentVersion, tocInfo.ParentVersion);

                AddMHSMeta(MHSMetaName.TocOrder, tocInfo.Order.ToString(CultureInfo.InvariantCulture));
            }
        }
        #endregion

        #region Private helper methods
        //=====================================================================

        // Loads TOC structure from a file
        private void LoadToc(string path)
        {
            _toc.Clear();

            using(Stream stream = File.OpenRead(path))
            {
                XPathDocument document = new XPathDocument(stream);
                XPathNavigator navigator = document.CreateNavigator();
                LoadToc(navigator.SelectSingleNode(TocXPath.Topics), _tocParent, _tocParentVersion);
            }
        }

        // Loads TOC structure from an XPathNavigator
        private void LoadToc(XPathNavigator navigator, string parent, string parentVersion)
        {
            // EFW - Reworked to support sortOrder attribute
            int sortOrder = 0, tempOrder;
            XPathNodeIterator interator = navigator.SelectChildren(TocXPath.Topic, String.Empty);

            while(interator.MoveNext())
            {
                XPathNavigator current = interator.Current;
                string id = current.GetAttribute(TocAttr.Id, String.Empty);

                // If a sort order is defined, start from that value
                string order = current.GetAttribute(TocAttr.SortOrder, String.Empty);

                if(!String.IsNullOrEmpty(order) && Int32.TryParse(order, out tempOrder))
                    sortOrder = tempOrder;

                if(!String.IsNullOrEmpty(id))
                {
                    TocInfo info = new TocInfo(parent, parentVersion, sortOrder++);

                    // EFW - Work around a bug in Sandcastle that can result in duplicate IDs
                    // by using the indexer to add the topic rather than Add() which throws
                    // an exception when the duplicate is encountered.
                    _toc[id] = info;

                    LoadToc(current, id, _topicVersion);
                }
            }
        }

        // Adds Microsoft Help System meta data to the output document
        private XmlElement AddMHSMeta(string name, string content)
        {
            return AddMHSMeta(name, content, false);
        }

        // Adds Microsoft Help System meta data to the output document
        private XmlElement AddMHSMeta(string name, string content, bool multiple)
        {
            if(String.IsNullOrEmpty(content))
                return null;

            XmlElement elem = null;

            // !EFW - Bug fix.  Fixed attribute name to find them properly (reported by Don Fehr).
            if(!multiple)
                elem = _document.SelectSingleNode(String.Format(CultureInfo.InvariantCulture,
                    "//meta[@name='{0}']", name)) as XmlElement;

            if(elem == null)
            {
                elem = _document.CreateElement(MHSTag.Meta);
                elem.SetAttribute(MHSMetaAttr.Name, name);
                elem.SetAttribute(MHSMetaAttr.Content, content);
                _head.AppendChild(elem);
            }

            return elem;
        }

        // Modifies an attribute value to prevent conflicts with Microsoft Help System branding
        private void ModifyAttribute(string name, string value)
        {
            XmlNodeList list = _document.SelectNodes(String.Format(CultureInfo.InvariantCulture,
                @"//*[@{0}='{1}']", name, value));

            foreach(XmlElement elem in list)
                elem.SetAttribute(name, value + MHSDefault.ShortName);
        }
        #endregion
    }
}
