// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change history:
// 02/16/2012 - EFW - Added Diagnostic message level type for diagnostic messages.  This allows messages
// to appear regardless of the verbosity level.
// 10/14/2012 - EFW - Added support for topic ID and message parameters in the message logging methods.
// 01/05/2012 - EFW - Made the WriteMessage method public so that subcomponents with a reference to a build
// component can log messages easily.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This is the base class for all build components
    /// </summary>
    public abstract class BuildComponent : IDisposable
    {
        #region Private data members
        //=====================================================================

        private BuildAssembler assembler;

        private static Dictionary<string, object> data = new Dictionary<string, object>();

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns a reference to the build assembler instance using the component
        /// </summary>
        public BuildAssembler BuildAssembler
        {
            get { return assembler; }
        }

        /// <summary>
        /// This read-only property returns a static dictionary that can be used to store information shared
        /// between build components.
        /// </summary>
        protected static Dictionary<string, object> Data
        {
            get { return data; }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="assembler">The build assembler instance using this component</param>
        /// <param name="configuration">The component configuration</param>
        protected BuildComponent(BuildAssembler assembler, XPathNavigator configuration)
        {
            this.assembler = assembler;
            WriteMessage(MessageLevel.Info, "Instantiating component.");
        }
        #endregion

        #region IDisposable implementation
        //=====================================================================

        /// <summary>
        /// This handles garbage collection to ensure proper disposal of the build component if not done
        /// explicity with <see cref="Dispose()"/>.
        /// </summary>
        ~BuildComponent()
        {
            this.Dispose(false);
        }

        /// <summary>
        /// This implements the Dispose() interface to properly dispose of the build component.
        /// </summary>
        /// <overloads>There are two overloads for this method.</overloads>
        public void Dispose()
        {
            this.Dispose(true);
            GC.SuppressFinalize(this);
        }

        /// <summary>
        /// This can be overridden by derived classes to add their own disposal code if necessary.
        /// </summary>
        /// <param name="disposing">Pass true to dispose of the managed and unmanaged resources or false to just
        /// dispose of the unmanaged resources.</param>
        protected virtual void Dispose(bool disposing)
        {
            // Nothing to dispose of in this one
        }
        #endregion

        #region Abstract methods
        //=====================================================================

        /// <summary>
        /// This abstract method must be overridden to apply the build component's changes to the specified
        /// document.
        /// </summary>
        /// <param name="document">The document that the build component can modify</param>
        /// <param name="key">The key that uniquely identifies the document</param>
        public abstract void Apply(XmlDocument document, string key);
        #endregion

        #region Component messaging methods
        //=====================================================================

        /// <summary>
        /// This can be used to raise the <see cref="Microsoft.Ddue.Tools.BuildAssembler.ComponentEvent"/> event
        /// with the specified event arguments.
        /// </summary>
        /// <param name="e">The event arguments.  This can be <see cref="EventArgs.Empty"/> or a derived event
        /// arguments class containing information to pass to the event handlers.</param>
        protected void OnComponentEvent(EventArgs e)
        {
            assembler.OnComponentEvent(this.GetType(), e);
        }

        /// <summary>
        /// This can be used to report a message
        /// </summary>
        /// <param name="level">The message level</param>
        /// <param name="message">The message to report</param>
        /// <param name="args">An optional list of arguments to format into the message</param>
        public void WriteMessage(MessageLevel level, string message, params object[] args)
        {
            if(level != MessageLevel.Ignore)
                assembler.WriteMessage(this.GetType(), level, null, (args.Length == 0) ? message :
                    String.Format(CultureInfo.CurrentCulture, message, args));
        }

        /// <summary>
        /// This can be used to report a message for a specific topic ID
        /// </summary>
        /// <param name="key">The topic key related to the message</param>
        /// <param name="level">The message level</param>
        /// <param name="message">The message to report</param>
        /// <param name="args">An optional list of arguments to format into the message</param>
        /// <remarks>This is useful for warning and error messages as the topic ID will be included even when
        /// the message level is set to warnings or higher.  In such cases, the informational messages containing
        /// the "building topic X" messages are suppressed.</remarks>
        public void WriteMessage(string key, MessageLevel level, string message, params object[] args)
        {
            if(level != MessageLevel.Ignore)
                assembler.WriteMessage(this.GetType(), level, key, (args.Length == 0) ? message :
                    String.Format(CultureInfo.CurrentCulture, message, args));
        }
        #endregion
    }
}
