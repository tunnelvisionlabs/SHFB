// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

using System;
using System.Collections.Generic;
using System.Configuration;
using System.Xml;
using System.Xml.XPath;
using System.Xml.Xsl;

using System.Reflection;

namespace Microsoft.Ddue.Tools {

	public class IfThenComponent : BuildComponent {

		private XPathExpression condition;

		private IEnumerable<BuildComponent> true_branch = new List<BuildComponent>();

		private IEnumerable<BuildComponent> false_branch = new List<BuildComponent>();

		public IfThenComponent (BuildAssembler assembler, XPathNavigator configuration) : base(assembler, configuration) {
			
			// get the condition
			XPathNavigator if_node = configuration.SelectSingleNode("if");
			if (if_node == null) throw new ConfigurationErrorsException("You must specify a condition using the <if> element.");
			string condition_xpath = if_node.GetAttribute("condition", String.Empty);
			if (String.IsNullOrEmpty(condition_xpath)) throw new ConfigurationErrorsException();
			condition = XPathExpression.Compile(condition_xpath);

			// construct the true branch
			XPathNavigator then_node = configuration.SelectSingleNode("then");
			if (then_node != null) true_branch = BuildAssembler.LoadComponents(then_node);

			// construct the false branch
			XPathNavigator else_node = configuration.SelectSingleNode("else");
			if (else_node != null) false_branch = BuildAssembler.LoadComponents(else_node);

            // keep a pointer to the context for future use
            context = assembler.Context;

		}

		private BuildContext context;

		public override void Apply (XmlDocument document, string key) {

            // set up the test
			context["key"] = key;
			XPathExpression test = condition.Clone();
			test.SetContext(context.XsltContext);

			// evaluate the condition
			bool result = (bool) document.CreateNavigator().Evaluate(test);

			// on the basis of the condition, execute either the true or the false branch
			if (result) {
				foreach (BuildComponent component in true_branch) {
					component.Apply(document, key);
				}
			} else {
				foreach (BuildComponent component in false_branch) {
					component.Apply(document, key);
				}
			}

		}

        protected override void Dispose(bool disposing) {
            if (disposing) {
                foreach (BuildComponent component in true_branch) {
                    component.Dispose();
                }
                foreach (BuildComponent component in false_branch) {
                    component.Dispose();
                }
            }
            base.Dispose(disposing);
        }

	}


}