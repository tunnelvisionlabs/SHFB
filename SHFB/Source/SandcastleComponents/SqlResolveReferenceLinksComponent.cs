﻿//===============================================================================================================
// System  : Sandcastle Help File Builder Components
// File    : SqlResolveReferenceLinksComponent.cs
// Author  : Eric Woodruff  (Eric@EWoodruff.us)
// Updated : 03/14/2013
// Compiler: Microsoft Visual C#
//
// This is a version of the ResolveReferenceLinksComponent2 that stores the MSDN content IDs and the framework
// targets in persistent SQL database tables.
//
// This code is published under the Microsoft Public License (Ms-PL).  A copy of the license should be
// distributed with the code.  It can also be found at the project website: http://SHFB.CodePlex.com.  This
// notice, the author's name, and all copyright notices must remain intact in all applications, documentation,
// and source files.
//
// Version     Date     Who  Comments
// ==============================================================================================================
// 1.9.7.0  01/14/2013  EFW  Created the code
//===============================================================================================================

using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Globalization;
using System.Reflection;
using System.Windows.Forms;
using System.Xml.XPath;

using Microsoft.Ddue.Tools;
using Microsoft.Ddue.Tools.Targets;

using SandcastleBuilder.Components.Targets;
using SandcastleBuilder.Components.UI;

namespace SandcastleBuilder.Components
{
    /// <summary>
    /// This is a version of the <c>ResolveReferenceLinksComponent2</c> that stores the MSDN content IDs and the
    /// framework targets in persistent SQL databases.
    /// </summary>
    public class SqlResolveReferenceLinksComponent : ResolveReferenceLinksComponent2
    {
        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembler">A reference to the build assembler.</param>
        /// <param name="configuration">The configuration information</param>
        /// <remarks>This component is obsolete and will be removed in a future release.</remarks>
        public SqlResolveReferenceLinksComponent(BuildAssembler assembler, XPathNavigator configuration) :
          base(assembler, configuration)
        {
            Assembly asm = Assembly.GetExecutingAssembly();
            FileVersionInfo fvi = FileVersionInfo.GetVersionInfo(asm.Location);

            base.WriteMessage(MessageLevel.Info, String.Format(CultureInfo.InvariantCulture,
                "\r\n    [{0}, version {1}]\r\n    Sql Resolve Reference Links Component.  {2}\r\n" +
                "    http://SHFB.CodePlex.com", fvi.ProductName, fvi.ProductVersion, fvi.LegalCopyright));
        }
        #endregion

        #region Method overrides
        //=====================================================================

        /// <summary>
        /// This is overridden to allow use of an Sql backed MSDN content ID cache
        /// </summary>
        /// <param name="configuration">The component configuration</param>
        /// <returns>An MSDN resolver instance</returns>
        protected override MsdnResolver CreateMsdnResolver(XPathNavigator configuration)
        {
            MsdnResolver resolver;
            IDictionary<string, string> cache = null;
            int localCacheSize;

            if(BuildComponent.Data.ContainsKey(SharedMsdnContentIdCacheId))
                cache = BuildComponent.Data[SharedMsdnContentIdCacheId] as IDictionary<string, string>;

            // If the shared cache already exists, return an instance that uses it.  It is assumed that all
            // subsequent instances will use the same cache.
            if(cache != null)
                return new MsdnResolver(cache, true);

            XPathNavigator node = configuration.SelectSingleNode("msdnContentIdCache");

            // If a <cache> element is not specified, use the default resolver
            if(node == null)
                resolver = base.CreateMsdnResolver(configuration);
            else
            {
                node = configuration.SelectSingleNode("sqlCache");
                string connectionString = node.GetAttribute("connectionString", String.Empty);

                // If a connection string is not defined, use the default resolver
                if(String.IsNullOrWhiteSpace(connectionString))
                    resolver = base.CreateMsdnResolver(configuration);
                else
                {
                    string cacheSize = node.GetAttribute("msdnLocalCacheSize", String.Empty);

                    if(String.IsNullOrWhiteSpace(cacheSize) || !Int32.TryParse(cacheSize, out localCacheSize))
                        localCacheSize = 2500;

                    // Load or create the cache database and the resolver.  The resolver will dispose of the
                    // dictionary when it is disposed of since it implements IDisposable.
                    resolver = new MsdnResolver(new SqlDictionary<string>(connectionString, "ContentIds",
                        "TargetKey", "ContentId") { LocalCacheSize = localCacheSize }, false);

                    int cacheCount = resolver.MsdnContentIdCache.Count;

                    if(cacheCount == 0)
                    {
                        // Log a diagnostic message since looking up all IDs can significantly slow the build
                        base.WriteMessage(MessageLevel.Diagnostic, "The SQL MSDN content ID cache in '" +
                            connectionString + "' does not exist yet.  All IDs will be looked up in this " +
                            "build which will slow it down.");
                    }
                    else
                        base.WriteMessage(MessageLevel.Info, "{0} cached MSDN content ID entries exist", cacheCount);

                    BuildComponent.Data[SharedMsdnContentIdCacheId] = resolver.MsdnContentIdCache;
                }
            }

            return resolver;
        }

        /// <summary>
        /// This is overridden to create a target dictionary that utilizes an SQL database for persistence
        /// </summary>
        /// <param name="configuration">The configuration element for the target dictionary</param>
        /// <returns>A simple dictionary if no <c>connectionString</c> attribute is found or a SQL backed target
        /// dictionary if the attribute is found.</returns>
        public override TargetDictionary CreateTargetDictionary(XPathNavigator configuration)
        {
            TargetDictionary td = null;
            string connectionString, groupId, attrValue;
            int frameworkCacheSize, projectCacheSize;
            bool cacheProject, isProjectData;

            var parent = configuration.Clone();
            parent.MoveToParent();

            var cache = parent.SelectSingleNode("sqlCache");

            connectionString = cache.GetAttribute("connectionString", String.Empty);

            attrValue = cache.GetAttribute("frameworkLocalCacheSize", String.Empty);
            frameworkCacheSize = Convert.ToInt32(attrValue);

            attrValue = cache.GetAttribute("projectLocalCacheSize", String.Empty);
            projectCacheSize = Convert.ToInt32(attrValue);

            attrValue = cache.GetAttribute("cacheProject", String.Empty);
            cacheProject = Convert.ToBoolean(attrValue);

            groupId = configuration.GetAttribute("groupId", String.Empty);
            isProjectData = groupId.StartsWith("Project_", StringComparison.OrdinalIgnoreCase);

            // If no connection is specified or if it is project data and we aren't caching it, use the simple
            // target dictionary.
            if(String.IsNullOrWhiteSpace(connectionString) || (isProjectData && !cacheProject))
                td = base.CreateTargetDictionary(configuration);
            else
            {
                try
                {
                    td = new SqlTargetDictionary(this, configuration, connectionString, groupId,
                        isProjectData ? projectCacheSize : frameworkCacheSize, isProjectData);
                }
                catch(Exception ex)
                {
                    base.WriteMessage(MessageLevel.Error, BuildComponentUtilities.GetExceptionMessage(ex));
                }
            }

            return td;
        }

        /// <summary>
        /// This is overridden to report the persistent cache information
        /// </summary>
        public override void UpdateMsdnContentIdCache()
        {
            if(base.MsdnResolver != null)
            {
                var cache = base.MsdnResolver.MsdnContentIdCache as SqlDictionary<string>;

                // Only report if we own the cache (it won't have been disposed off yet)
                if(cache != null && !cache.IsDisposed)
                {
                    if(base.MsdnResolver.CacheItemsAdded)
                        base.WriteMessage(MessageLevel.Diagnostic, "New MSDN content ID cache size: {0} entries",
                            cache.Count);

                    base.WriteMessage(MessageLevel.Diagnostic, "MSDN content ID SQL local cache flushed {0} " +
                        "time(s).  Current SQL local cache usage: {1} of {2}.", cache.LocalCacheFlushCount,
                        cache.CurrentLocalCacheCount, cache.LocalCacheSize);
                }
            }

            base.UpdateMsdnContentIdCache();
        }
        #endregion

        #region Static configuration method for use with SHFB
        //=====================================================================

        /// <summary>
        /// This static method is used by the Sandcastle Help File Builder to let the component perform its own
        /// configuration.
        /// </summary>
        /// <param name="currentConfig">The current configuration XML fragment</param>
        /// <returns>A string containing the new configuration XML fragment</returns>
        public static string ConfigureComponent(string currentConfig)
        {
            using(var dlg = new SqlResolveReferenceLinksConfigDlg(currentConfig))
            {
                if(dlg.ShowDialog() == DialogResult.OK)
                    currentConfig = dlg.Configuration;
            }

            return currentConfig;
        }
        #endregion
    }
}
