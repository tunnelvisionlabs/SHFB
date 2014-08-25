//===============================================================================================================
// System  : Sandcastle Help File Builder Utilities
// File    : NamespaceSummaryItemCollection.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 08/24/2014
// Note    : Copyright 2006-2014, Eric Woodruff, All rights reserved
// Compiler: Microsoft Visual C#
//
// This file contains a collection class used to hold the namespace summary item information
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.2.0.0  09/04/2006  EFW  Created the code
// 1.8.0.0  06/30/2008  EFW  Rewrote to support MSBuild project format
// 1.9.3.0  04/07/2011  EFW  Made the constructor and from/to XML members public so that it can be used from the
//                           VSPackage.
// 1.9.9.0  11/30/2013  EFW  Merged changes from Stazzz to support namespace grouping
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Text;
using System.Xml;

namespace SandcastleBuilder.Utils
{
    /// <summary>
    /// This collection class is used to hold the namespace summary items for a project
    /// </summary>
    /// <remarks>Namespaces that appear in the assemblies but not in this list are documented by default and
    /// will appear without a namespace summary.</remarks>
    public class NamespaceSummaryItemCollection : BindingList<NamespaceSummaryItem>
    {
        #region Private data members
        //=====================================================================

        private SandcastleProject projectFile;
        private bool isDirty;
        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This is used to get a reference to the project that owns the collection
        /// </summary>
        public SandcastleProject Project
        {
            get { return projectFile; }
        }

        /// <summary>
        /// This is used to get or set the dirty state of the collection
        /// </summary>
        public bool IsDirty
        {
            get
            {
                foreach(NamespaceSummaryItem nsi in this)
                    if(nsi.IsDirty)
                        return true;

                return isDirty;
            }
            set
            {
                foreach(NamespaceSummaryItem nsi in this)
                    nsi.IsDirty = value;

                isDirty = value;
            }
        }

        /// <summary>
        /// Indexer.  This can be used to retrieve the summary information for the specified namespace
        /// </summary>
        /// <param name="name">The namespace for which to search</param>
        /// <returns>The namespace summary information if found or null if not found</returns>
        public NamespaceSummaryItem this[string name]
        {
            get
            {
                if(name == null || name.Length == 0)
                    name = "(global)";

                foreach(NamespaceSummaryItem nsi in this)
                    if(nsi.Name == name)
                        return nsi;

                return null;
            }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="project">The project that owns the collection</param>
        public NamespaceSummaryItemCollection(SandcastleProject project)
        {
            projectFile = project;
        }
        #endregion

        #region Sort the collection
        //=====================================================================

        /// <summary>
        /// This is used to sort the namespace items by name
        /// </summary>
        public void Sort()
        {
            ((List<NamespaceSummaryItem>)this.Items).Sort(
                (x, y) => Comparer<string>.Default.Compare(x.Name, y.Name) );
        }
        #endregion

        #region Read/write namespace summary items from/to XML
        //=====================================================================

        /// <summary>
        /// This is used to load existing namespace summary items from the project file
        /// </summary>
        /// <param name="namespaceSummaries">The namespace summary items</param>
        /// <remarks>The information is stored as an XML fragment</remarks>
        public void FromXml(string namespaceSummaries)
        {
            XmlTextReader xr = null;
            string name, groupValue;
            bool isDocumented, isGroup;

            try
            {
                xr = new XmlTextReader(namespaceSummaries, XmlNodeType.Element,
                    new XmlParserContext(null, null, null, XmlSpace.Default));
                xr.MoveToContent();

                while(!xr.EOF)
                {
                    if(xr.NodeType == XmlNodeType.Element && xr.Name == "NamespaceSummaryItem")
                    {
                        name = xr.GetAttribute("name");
                        isDocumented = Convert.ToBoolean(xr.GetAttribute("isDocumented"),
                            CultureInfo.InvariantCulture);

                        groupValue = xr.GetAttribute("isGroup");
                        isGroup = !String.IsNullOrWhiteSpace(groupValue) && Convert.ToBoolean(groupValue,
                           CultureInfo.InvariantCulture);

                        this.Add(name, isGroup, isDocumented, xr.ReadString());
                    }

                    xr.Read();
                }
            }
            finally
            {
                if(xr != null)
                    xr.Close();

                isDirty = false;
            }
        }

        /// <summary>
        /// This is used to write the namespace summary info to an XML fragment ready for storing in the project
        /// file.
        /// </summary>
        /// <returns>The XML fragment containing the namespace summary info</returns>
        public string ToXml()
        {
            using(var ms = new MemoryStream(10240))
            {
                using(var xw = new XmlTextWriter(ms, new UTF8Encoding(false)))
                {
                    xw.Formatting = Formatting.Indented;

                    foreach(NamespaceSummaryItem nsi in this)
                    {
                        xw.WriteStartElement("NamespaceSummaryItem");
                        xw.WriteAttributeString("name", nsi.Name);

                        if(nsi.IsGroup)
                            xw.WriteAttributeString("isGroup", nsi.IsGroup.ToString());

                        xw.WriteAttributeString("isDocumented", nsi.IsDocumented.ToString());
                        xw.WriteString(nsi.Summary);
                        xw.WriteEndElement();
                    }

                    xw.Flush();

                    return Encoding.UTF8.GetString(ms.ToArray());
                }
            }
        }
        #endregion

        #region Add/create a new namespace summary item
        //=====================================================================

        /// <summary>
        /// Add a new item to the collection
        /// </summary>
        /// <param name="name">The namespace name</param>
        /// <param name="isGroup">True if this is a grouping namespace, false if this is normal namespace</param>
        /// <param name="isDocumented">True for documented, false for not documented</param>
        /// <param name="summary">The summary text</param>
        /// <returns>The <see cref="NamespaceSummaryItem" /> added to the project.  If the namespace already
        /// exists in the collection, the existing item is returned.</returns>
        /// <remarks>The <see cref="NamespaceSummaryItem" /> constructor is internal so that we control creation
        /// of the items and can associate them with the project.</remarks>
        public NamespaceSummaryItem Add(string name, bool isGroup, bool isDocumented, string summary)
        {
            NamespaceSummaryItem item = this[name];

            if(item == null)
            {
                item = new NamespaceSummaryItem(name, isGroup, isDocumented, summary, projectFile);
                base.Add(item);
            }

            return item;
        }

        /// <summary>
        /// Create a temporary item that isn't part of the project
        /// </summary>
        /// <param name="name">The namespace name</param>
        /// <param name="isGroup">True if this is a grouping namespace, false if this is normal namespace</param>
        /// <returns>The <see cref="NamespaceSummaryItem" /> that can later be added to the project if necessary</returns>
        /// <exception cref="ArgumentException">This is thrown if the given namespace already exists in the
        /// collection.</exception>
        public NamespaceSummaryItem CreateTemporaryItem(string name, bool isGroup)
        {
            NamespaceSummaryItem item = this[name];

            if(item != null)
                throw new ArgumentException("The given namespace exists in the collection and cannot be a " +
                    "temporary item", "name");

            item = new NamespaceSummaryItem(name, isGroup, !String.IsNullOrEmpty(name), String.Empty, projectFile);

            return item;
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <summary>
        /// This is overridden to mark the collection as dirty when it changes
        /// </summary>
        /// <param name="e">The event arguments</param>
        protected override void OnListChanged(ListChangedEventArgs e)
        {
            isDirty = true;
            base.OnListChanged(e);
        }
        #endregion
    }
}
