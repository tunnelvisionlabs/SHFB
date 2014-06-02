﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Components
// File    : LanguageSpecificTextComponent.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 04/30/2014
// Note    : Copyright 2014, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a build component that is used to convert span style language-specific text elements to
// the script style elements used in the VS2010 and later styles.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
//    Date     Who  Comments
// ==============================================================================================================
// 04/23/2014  EFW  Created the code
//===============================================================================================================

using System;
using System.Globalization;
using System.Linq;
using System.Xml;
using System.Xml.XPath;

using Sandcastle.Core.BuildAssembler;
using Sandcastle.Core.BuildAssembler.BuildComponent;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This component is used to convert the span style language-specific text elements to the script style
    /// elements used in the VS2010 and later styles.
    /// </summary>
    /// <remarks>An optional <c>scriptFunction</c> configuration element with a <c>name</c> attribute specifying
    /// the JavaScript function to call can be specified.  If omitted, the function name defaults to
    /// "<c>AddLanguageSpecificTextSet</c>".</remarks>
    public class LanguageSpecificTextComponent : BuildComponentCore
    {
        #region Build component factory for MEF
        //=====================================================================

        /// <summary>
        /// This is used to create a new instance of the build component
        /// </summary>
        [BuildComponentExport("Language-Specific Text Component")]
        public sealed class Factory : BuildComponentFactory
        {
            /// <inheritdoc />
            public override BuildComponentCore Create()
            {
                return new LanguageSpecificTextComponent(base.BuildAssembler);
            }
        }
        #endregion

        #region Private data members
        //=====================================================================

        private string scriptFunctionName;
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="buildAssembler">A reference to the build assembler</param>
        protected LanguageSpecificTextComponent(BuildAssemblerCore buildAssembler) : base(buildAssembler)
        {
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <inheritdoc />
        public override void Initialize(XPathNavigator configuration)
        {
            // The <scriptFunction> element is optional.  If not set, it will use the default name
            // "AddLanguageSpecificTextSet".
            var nav = configuration.SelectSingleNode("scriptFunction");

            if(nav != null)
                scriptFunctionName = nav.GetAttribute("name", String.Empty);

            if(String.IsNullOrWhiteSpace(scriptFunctionName))
                scriptFunctionName = "AddLanguageSpecificTextSet";
        }

        /// <inheritdoc />
        public override void Apply(XmlDocument document, string key)
        {
            XmlNodeList langList;
            XmlElement spanElement, scriptElement;
            string uniqueId, langParam, idRoot = String.Format(CultureInfo.InvariantCulture,
                "LST{0:X}_", key.GetHashCode());
            int sequence = 0;

            foreach(XmlNode lstNode in document.SelectNodes("//span[@class='languageSpecificText']"))
            {
                langList = lstNode.SelectNodes("span[@class]");

                // The language span count should match the total element count.  If not, we'll let it pass
                // through as is.
                if(langList.Count > 0 && langList.Count == lstNode.ChildNodes.Count)
                {
                    uniqueId = idRoot + sequence.ToString(CultureInfo.InvariantCulture);
                    langParam = String.Join("|", langList.OfType<XmlNode>().Select(
                        n => String.Concat(n.Attributes["class"].Value, "=", n.InnerText)));

                    spanElement = document.CreateElement("span");
                    spanElement.SetAttribute("id", uniqueId);

                    // Set the inner text to an empty string so that it is written out with full opening and
                    // closing elements.
                    spanElement.InnerText = String.Empty;

                    scriptElement = document.CreateElement("script");
                    scriptElement.SetAttribute("type", "text/javascript");
                    scriptElement.InnerText = String.Format(CultureInfo.InvariantCulture, "{0}(\"{1}?{2}\");",
                        scriptFunctionName, uniqueId, langParam);

                    lstNode.ParentNode.InsertAfter(scriptElement, lstNode);
                    lstNode.ParentNode.ReplaceChild(spanElement, lstNode);

                    sequence++;
                }
            }
        }
        #endregion
    }
}
