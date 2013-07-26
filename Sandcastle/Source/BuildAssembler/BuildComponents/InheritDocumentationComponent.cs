// ------------------------------------------------------------------------------------------------
// <copyright file="InheritDocumentationComponent.cs" company="Microsoft">
//      Copyright � Microsoft Corporation.
//      This source file is subject to the Microsoft Permissive License.
//      See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
//      All other rights reserved.
// </copyright>
// <summary>Contains code that indexes XML comments files for <inheritdoc /> tags, reflection files
// for API information and produces a new XML comments file containing the inherited documentation
// for use by Sandcastle.
// </summary>
// ------------------------------------------------------------------------------------------------

// Change History
// 12/21/2012 - EFW - Moved this component into the main BuildComponents project and removed the CopyComponents
// project as this was the only thing in it.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools.Commands;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// InheritDocumentationComponent class.
    /// </summary>
    public class InheritDocumentationComponent : CopyComponent
    {
        #region Private members
        //=====================================================================

        // XPathExpression for API name.
        private static XPathExpression apiNameExpression = XPathExpression.Compile("string(apidata/@name)");

        // XPathExpression for API group.
        private static XPathExpression apiGroupExpression = XPathExpression.Compile("string(apidata/@group)");

        // XPathExpression for API subgroup.
        private static XPathExpression apiSubgroupExpression = XPathExpression.Compile("string(apidata/@subgroup)");

        // XPathExpression for API ancestors.
        private static XPathExpression typeExpression = XPathExpression.Compile("family/ancestors/type/@api");

        // XPathExpression for API type interface implementations.
        private static XPathExpression interfaceImplementationExpression = XPathExpression.Compile("implements/type/@api");

        // XPathExpression for API containers.
        private static XPathExpression containerTypeExpression = XPathExpression.Compile("string(containers/type/@api)");

        // XPathExpression for override members.
        private static XPathExpression overrideMemberExpression = XPathExpression.Compile("overrides/member/@api");

        // XPathExpression for API member interface implementaions.
        private static XPathExpression interfaceImplementationMemberExpression = XPathExpression.Compile("implements/member/@api");

        // XPathExpression for <inheritdoc /> nodes.
        private static XPathExpression inheritDocExpression = XPathExpression.Compile("//inheritdoc");

        // XPathExpression that looks for example, filterpriority, preliminary, remarks, returns, summary, threadsafety and value nodes.
        private static XPathExpression tagsExpression = XPathExpression.Compile("example|filterpriority|preliminary|remarks|returns|summary|threadsafety|value");

        // XPathExpression for source nodes.
        private static XPathExpression sourceExpression;

        // Document to be parsed.
        private XmlDocument sourceDocument;

        // A cache for comment files.
        private IndexedCache commentsIndex;

        // A cache for reflection files.
        private IndexedCache reflectionIndex;
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Creates an instance of InheritDocumentationComponent class.
        /// </summary>
        /// <param name="parent">The parent build component</param>
        /// <param name="configuration">Configuration section to be parsed.</param>
        /// <param name="data">A dictionary object with string as key and object as value.</param>
        public InheritDocumentationComponent(BuildComponent parent, XPathNavigator configuration,
          IDictionary<string, object> data) : base(parent, configuration, data)
        {
            // Get the copy command
            XPathNavigator copyNode = configuration.SelectSingleNode("copy");

            if(copyNode == null)
                parent.WriteMessage(MessageLevel.Error, "A copy element is required to define the indexes " +
                    "from which to obtain comments and reflection information");

            // Get the comments info
            string sourceName = copyNode.GetAttribute("name", string.Empty);

            if(String.IsNullOrEmpty(sourceName))
                parent.WriteMessage(MessageLevel.Error, "Each copy command must specify an index to copy from");

            // Get the reflection info
            string reflectionName = copyNode.GetAttribute("use", String.Empty);

            if(String.IsNullOrEmpty(reflectionName))
                parent.WriteMessage(MessageLevel.Error, "Each copy command must specify an index to get " +
                    "reflection information from");

            this.commentsIndex = (IndexedCache)data[sourceName];
            this.reflectionIndex = (IndexedCache)data[reflectionName];
        }
        #endregion

        #region Methods
        //=====================================================================

        /// <summary>
        /// Deletes the specified node and logs the message.
        /// </summary>
        /// <param name="inheritDocNodeNavigator">navigator for inheritdoc node</param>
        /// <param name="key">Id of the topic specified</param>
        public void DeleteNode(XPathNavigator inheritDocNodeNavigator, string key)
        {
            base.ParentBuildComponent.WriteMessage(MessageLevel.Info, "Comments not found for topic: {0}", key);
            inheritDocNodeNavigator.DeleteSelf();
        }

        /// <summary>
        /// Implement inheritDocumentation.
        /// </summary>
        /// <param name="document">document to be parsed</param>
        /// <param name="key">Id pf the topic specified</param>
        public override void Apply(XmlDocument document, string key)
        {
            // default selection filter set not to inherit <overloads>.
            sourceExpression = XPathExpression.Compile("*[not(local-name()='overloads')]");
            this.sourceDocument = document;
            this.InheritDocumentation(key);
        }

        /// <summary>
        /// Inherit the documentation.
        /// </summary>
        /// <param name="key">Id of the topic specified</param>
        public void InheritDocumentation(string key)
        {
            foreach(XPathNavigator inheritDocNodeNavigator in this.sourceDocument.CreateNavigator().Select(inheritDocExpression))
            {
                inheritDocNodeNavigator.MoveToParent();

                XPathNodeIterator iterator = (XPathNodeIterator)inheritDocNodeNavigator.CreateNavigator().Evaluate(tagsExpression);

                // do not inherit the comments if the tags specified in tagsExpression are already present.
                if(iterator.Count != 0)
                {
                    inheritDocNodeNavigator.MoveTo(this.sourceDocument.CreateNavigator().SelectSingleNode(inheritDocExpression));
                    inheritDocNodeNavigator.DeleteSelf();
                    continue;
                }

                inheritDocNodeNavigator.MoveTo(this.sourceDocument.CreateNavigator().SelectSingleNode(inheritDocExpression));

                // Inherit from the specified API [id=cref].
                string cref = inheritDocNodeNavigator.GetAttribute("cref", string.Empty);

                if(!string.IsNullOrEmpty(cref))
                {
                    XPathNavigator contentNodeNavigator = this.commentsIndex[cref];

                    // if no comments were found for the specified api, delete the <inheritdoc /> node,
                    // otherwise update the <inheritdoc /> node with the comments from the specified api.
                    if(contentNodeNavigator == null)
                        this.DeleteNode(inheritDocNodeNavigator, cref);
                    else
                    {
                        this.UpdateNode(inheritDocNodeNavigator, contentNodeNavigator);

                        if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count != 0)
                            this.InheritDocumentation(cref);
                    }
                }
                else
                {
                    XPathNavigator reflectionNodeNavigator = this.reflectionIndex[key];

                    // no reflection information was found for the api, so delete <inheritdoc /> node.
                    if(reflectionNodeNavigator == null)
                    {
                        this.DeleteNode(inheritDocNodeNavigator, key);
                        continue;
                    }

                    string group = (string)reflectionNodeNavigator.Evaluate(apiGroupExpression);
                    string subgroup = (string)reflectionNodeNavigator.Evaluate(apiSubgroupExpression);

                    if(group == "type")
                    {
                        // Inherit from base types
                        XPathNodeIterator typeNodeIterator = (XPathNodeIterator)reflectionNodeNavigator.Evaluate(typeExpression);
                        this.GetComments(typeNodeIterator, inheritDocNodeNavigator);

                        // no <inheritdoc /> nodes were found, so continue with next iteration. Otherwise inherit from interface implementation types.
                        if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count == 0)
                            continue;

                        // Inherit from interface implementation types
                        XPathNodeIterator interfaceNodeIterator = (XPathNodeIterator)reflectionNodeNavigator.Evaluate(interfaceImplementationExpression);
                        this.GetComments(interfaceNodeIterator, inheritDocNodeNavigator);
                    }
                    else if(group == "member")
                    {
                        // constructors do not have override member information in reflection files, so search all the base types for a matching signature.
                        if(subgroup == "constructor")
                        {
                            string name = (string)reflectionNodeNavigator.Evaluate(apiNameExpression);
                            string typeApi = (string)reflectionNodeNavigator.Evaluate(containerTypeExpression);

                            // no container type api was found, so delete <inheritdoc /> node.
                            if(string.IsNullOrEmpty(typeApi))
                            {
                                this.DeleteNode(inheritDocNodeNavigator, key);
                                continue;
                            }

                            reflectionNodeNavigator = this.reflectionIndex[typeApi];

                            // no reflection information for container type api was found, so delete <inheritdoc /> node.
                            if(reflectionNodeNavigator == null)
                            {
                                this.DeleteNode(inheritDocNodeNavigator, key);
                                continue;
                            }

                            XPathNodeIterator containerIterator = reflectionNodeNavigator.Select(typeExpression);

                            foreach(XPathNavigator containerNavigator in containerIterator)
                            {
                                string constructorId = string.Format(CultureInfo.InvariantCulture, "M:{0}.{1}", containerNavigator.Value.Substring(2), name.Replace('.', '#'));
                                XPathNavigator contentNodeNavigator = this.commentsIndex[constructorId];

                                if(contentNodeNavigator == null)
                                    continue;

                                this.UpdateNode(inheritDocNodeNavigator, contentNodeNavigator);

                                if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count == 0)
                                    break;

                                inheritDocNodeNavigator.MoveTo(this.sourceDocument.CreateNavigator().SelectSingleNode(inheritDocExpression));
                            }
                        }
                        else
                        {
                            // Inherit from override members.
                            XPathNodeIterator memberNodeIterator = (XPathNodeIterator)reflectionNodeNavigator.Evaluate(overrideMemberExpression);
                            this.GetComments(memberNodeIterator, inheritDocNodeNavigator);

                            if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count == 0)
                                continue;

                            // Inherit from interface implementations members.
                            XPathNodeIterator interfaceNodeIterator = (XPathNodeIterator)reflectionNodeNavigator.Evaluate(interfaceImplementationMemberExpression);
                            this.GetComments(interfaceNodeIterator, inheritDocNodeNavigator);
                        }
                    }

                    // no comments were found, so delete <iheritdoc /> node.
                    if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count != 0)
                        this.DeleteNode(inheritDocNodeNavigator, key);
                }
            }
        }

        /// <summary>
        /// Updates the node replacing inheritdoc node with comments found.
        /// </summary>
        /// <param name="inheritDocNodeNavigator">Navigator for inheritdoc node</param>
        /// <param name="contentNodeNavigator">Navigator for content</param>
        public void UpdateNode(XPathNavigator inheritDocNodeNavigator, XPathNavigator contentNodeNavigator)
        {
            // retrieve the selection filter if specified.
            string selectValue = inheritDocNodeNavigator.GetAttribute("select", string.Empty);

            if(!string.IsNullOrEmpty(selectValue))
                sourceExpression = XPathExpression.Compile(selectValue);

            inheritDocNodeNavigator.MoveToParent();

            if(inheritDocNodeNavigator.LocalName != "comments" && inheritDocNodeNavigator.LocalName != "element")
                sourceExpression = XPathExpression.Compile(inheritDocNodeNavigator.LocalName);
            else
                inheritDocNodeNavigator.MoveTo(this.sourceDocument.CreateNavigator().SelectSingleNode(inheritDocExpression));

            XPathNodeIterator sources = (XPathNodeIterator)contentNodeNavigator.CreateNavigator().Evaluate(sourceExpression);
            inheritDocNodeNavigator.DeleteSelf();

            // append the source nodes to the target node
            foreach(XPathNavigator source in sources)
                inheritDocNodeNavigator.AppendChild(source);
        }

        /// <summary>
        /// Gets the comments for inheritdoc node.
        /// </summary>
        /// <param name="iterator">Iterator for API information</param>
        /// <param name="inheritDocNodeNavigator">Navigator for inheritdoc node</param>
        public void GetComments(XPathNodeIterator iterator, XPathNavigator inheritDocNodeNavigator)
        {
            foreach(XPathNavigator navigator in iterator)
            {
                XPathNavigator contentNodeNavigator = this.commentsIndex[navigator.Value];

                if(contentNodeNavigator == null)
                    continue;

                this.UpdateNode(inheritDocNodeNavigator, contentNodeNavigator);

                if(this.sourceDocument.CreateNavigator().Select(inheritDocExpression).Count == 0)
                    break;

                inheritDocNodeNavigator.MoveTo(this.sourceDocument.CreateNavigator().SelectSingleNode(inheritDocExpression));
            }
        }
        #endregion
    }
}
