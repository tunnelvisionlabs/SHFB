﻿using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Xml;
using System.Xml.XPath;

using Microsoft.Ddue.Tools.CommandLine;

namespace SegregateByNamespace
{
    public static class Program
    {
        // Fields
        private static XPathExpression apiExpression = XPathExpression.Compile("/*/apis/api");
        private static XPathExpression apiNamespaceExpression = XPathExpression.Compile("string(containers/namespace/@api)");
        private static XPathExpression assemblyNameExpression = XPathExpression.Compile("string(containers/library/@assembly)");
        private static XPathExpression namespaceIdExpression = XPathExpression.Compile("string(@id)");

        // Methods
        public static int Main(string[] args)
        {
            XPathDocument document;

            ConsoleApplication.WriteBanner();

            OptionCollection options = new OptionCollection {
                new SwitchOption("?", "Show this help page."),
                new StringOption("out", "Specify an output directory. If unspecified, output goes to the " +
                    "current directory.", "outputDirectory")
            };

            ParseArgumentsResult result = options.ParseArguments(args);

            if(result.Options["?"].IsPresent)
            {
                Console.WriteLine("SegregateByNamespace [options] reflectionDataFile");
                options.WriteOptionSummary(Console.Out);
                return 0;
            }

            if(!result.Success)
            {
                result.WriteParseErrors(Console.Out);
                return 1;
            }

            if(result.UnusedArguments.Count != 1)
            {
                Console.WriteLine("Specify one reflection data file.");
                return 1;
            }

            string uri = Environment.ExpandEnvironmentVariables(result.UnusedArguments[0]);
            string outputPath = null;

            if(result.Options["out"].IsPresent)
                outputPath = Environment.ExpandEnvironmentVariables((string)result.Options["out"].Value);

            try
            {
                document = new XPathDocument(uri);
            }
            catch(IOException ioEx)
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, String.Format(CultureInfo.CurrentCulture,
                    "An error occured while attempting to access the file '{0}'. The error message is: {1}",
                    uri, ioEx.Message));
                return 1;
            }
            catch(XmlException xmlEx)
            {
                ConsoleApplication.WriteMessage(LogLevel.Error, String.Format(CultureInfo.CurrentCulture,
                    "An exception processing the input file '{0}'. The error message is: {1}", uri,
                    xmlEx.Message));
                return 1;
            }

            WriteNamespaceFiles(document, outputPath);

            return 0;
        }

        private static void WriteNamespaceFiles(XPathDocument source, string outputDir)
        {
            Dictionary<string, object> dictionary3;
            XmlWriter writer;
            string current;

            Dictionary<string, Dictionary<string, object>> dictionary = new Dictionary<string, Dictionary<string, object>>();
            Dictionary<string, XmlWriter> dictionary2 = new Dictionary<string, XmlWriter>();
            XmlWriterSettings settings = new XmlWriterSettings { Indent = true };

            try
            {
                XPathNodeIterator iterator = source.CreateNavigator().Select(apiExpression);

                foreach(XPathNavigator navigator in iterator)
                {
                    current = (string)navigator.Evaluate(apiNamespaceExpression);

                    if(!String.IsNullOrEmpty(current))
                    {
                        String key = (string)navigator.Evaluate(assemblyNameExpression);

                        if(!dictionary.TryGetValue(current, out dictionary3))
                        {
                            dictionary3 = new Dictionary<string, object>();
                            dictionary.Add(current, dictionary3);
                        }

                        if(!dictionary3.ContainsKey(key))
                            dictionary3.Add(key, null);
                    }
                }

                foreach(string currentKey in dictionary.Keys)
                {
                    string filename = currentKey.Substring(2) + ".xml";

                    if(filename == ".xml")
                        filename = "default_namespace.xml";

                    if(outputDir != null)
                        filename = Path.Combine(outputDir, filename);

                    writer = XmlWriter.Create(filename, settings);

                    dictionary2.Add(currentKey, writer);

                    writer.WriteStartElement("reflection");
                    writer.WriteStartElement("assemblies");

                    dictionary3 = dictionary[currentKey];

                    foreach(string assemblyName in dictionary3.Keys)
                    {
                        XPathNavigator navigator2 = source.CreateNavigator().SelectSingleNode(
                            "/*/assemblies/assembly[@name='" + assemblyName + "']");

                        if(navigator2 != null)
                            navigator2.WriteSubtree(writer);
                        else
                            ConsoleApplication.WriteMessage(LogLevel.Error, String.Format(CultureInfo.CurrentCulture,
                                "Input file does not contain node for '{0}' assembly", assemblyName));
                    }

                    writer.WriteEndElement();
                    writer.WriteStartElement("apis");
                }

                foreach(XPathNavigator navigator in iterator)
                {
                    current = (string)navigator.Evaluate(apiNamespaceExpression);

                    if(string.IsNullOrEmpty(current))
                        current = (string)navigator.Evaluate(namespaceIdExpression);

                    writer = dictionary2[current];
                    navigator.WriteSubtree(writer);
                }

                foreach(XmlWriter w in dictionary2.Values)
                {
                    w.WriteEndElement();
                    w.WriteEndElement();
                    w.WriteEndDocument();
                }

                ConsoleApplication.WriteMessage(LogLevel.Info, String.Format(CultureInfo.CurrentCulture,
                    "Wrote information on {0} APIs to {1} files.", iterator.Count, dictionary2.Count));
            }
            finally
            {
                foreach(XmlWriter w in dictionary2.Values)
                    w.Close();
            }
        }
    }
}
