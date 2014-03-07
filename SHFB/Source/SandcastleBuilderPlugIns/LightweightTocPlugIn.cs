﻿namespace SandcastleBuilder.PlugIns
{
    using System;
    using System.Collections.Generic;
    using System.Collections.ObjectModel;
    using System.Linq;
    using System.Text;
    using System.Windows.Forms;
    using System.Xml;
    using System.Xml.Linq;
    using System.Xml.XPath;
    using Sandcastle.Core;
    using SandcastleBuilder.Utils;
    using SandcastleBuilder.Utils.BuildComponent;
    using SandcastleBuilder.Utils.BuildEngine;
    using Directory = System.IO.Directory;
    using File = System.IO.File;
    using Path = System.IO.Path;
    using PropertyInfo = System.Reflection.PropertyInfo;

    [HelpFileBuilderPlugInExport("Lightweight TOC",
        Version = AssemblyInfo.ProductVersion,
        Copyright = "Copyright 2014 Sam Harwell",
        Description = "This plug-in embeds a table of contents in the website output similar " +
            "to the current MSDN \"lightweight\" style.")]
    public class LightweightTocPlugIn : IPlugIn
    {
        private static readonly ReadOnlyCollection<ExecutionPoint> _executionPoints =
            new List<ExecutionPoint>
            {
                new ExecutionPoint(BuildStep.ExtractingHtmlInfo, ExecutionBehaviors.After),
                new ExecutionPoint(BuildStep.CopyStandardHelpContent, ExecutionBehaviors.After),
            }.AsReadOnly();

        private BuildProcess _builder;

        /// <inheritdoc />
        public IEnumerable<ExecutionPoint> ExecutionPoints
        {
            get
            {
                return _executionPoints;
            }
        }

        /// <inheritdoc />
        public string ConfigurePlugIn(SandcastleProject project, string currentConfig)
        {
            MessageBox.Show("This plug-in has no configurable settings", "Lightweight TOC Plug-In",
                MessageBoxButtons.OK, MessageBoxIcon.Information);

            return currentConfig;
        }

        /// <inheritdoc />
        public void Initialize(BuildProcess buildProcess, XPathNavigator configuration)
        {
            _builder = buildProcess;

            var metadata = (HelpFileBuilderPlugInExportAttribute)this.GetType().GetCustomAttributes(
                typeof(HelpFileBuilderPlugInExportAttribute), false).First();

            _builder.ReportProgress("{0} Version {1}\r\n{2}", metadata.Id, metadata.Version, metadata.Copyright);
        }

        /// <inheritdoc />
        public void Execute(ExecutionContext context)
        {
            if ((_builder.CurrentProject.HelpFileFormat & HelpFileFormats.Website) == 0)
                return;

            if (context.BuildStep == BuildStep.CopyStandardHelpContent)
            {
                ReplaceIndexFiles();
                return;
            }

            // load the web TOC generated by SandcastleHtmlExtract
            XDocument webtoc = XDocument.Load(Path.Combine(_builder.WorkingFolder, "WebTOC.xml"));
            XDocument contentMetadata = XDocument.Load(Path.Combine(_builder.WorkingFolder, "_ContentMetadata_.xml"));
            XElement tocroot = contentMetadata.XPathSelectElements("/metadata/topic").First();
            XElement defaultTopic = contentMetadata.XPathSelectElements("/metadata/topic[keyword[@index='NamedUrlIndex' and text()='DefaultPage']]").FirstOrDefault();

            // remove the Id attribute from all nodes that contain a Url attribute
            foreach (XElement element in webtoc.XPathSelectElements("//node()[@Id and @Url]"))
            {
                element.Attribute("Id").Remove();
            }

            // generate the TOC fragments
            Directory.CreateDirectory(Path.Combine(_builder.WorkingFolder, "Output", "Website", "toc"));
            List<XElement> elements = new List<XElement>(webtoc.XPathSelectElements("//node()"));
            foreach (XElement element in elements)
            {
                XDocument pageChildren = new XDocument(new XDeclaration("1.0", "utf-8", null));
                XElement copy = new XElement(element);
                pageChildren.Add(copy);
                foreach (XElement child in copy.Elements())
                {
                    if (!child.HasElements)
                        continue;

                    child.SetAttributeValue("HasChildren", true);
                    child.RemoveNodes();
                }

                string uri = null;
                if (copy.Attribute("Url") != null)
                    uri = copy.Attribute("Url").Value;
                else if (copy.Attribute("Id") != null)
                    uri = copy.Attribute("Id").Value;
                else if (copy.Name.LocalName == "HelpTOC")
                    uri = "roottoc.html";
                else
                    throw new NotImplementedException();

                XmlWriterSettings writerSettings = new XmlWriterSettings();
                writerSettings.Indent = true;
                writerSettings.IndentChars = "  ";
                writerSettings.Encoding = Encoding.UTF8;

                string fileId = Path.GetFileNameWithoutExtension(uri.Substring(uri.LastIndexOf('/') + 1));

                if (element.HasElements)
                {
                    using (XmlWriter writer = XmlWriter.Create(Path.Combine(_builder.WorkingFolder, "Output", "Website", "toc", fileId + ".xml"), writerSettings))
                    {
                        pageChildren.WriteTo(writer);
                    }
                }

                if (copy.Attribute("Url") == null)
                    continue;

                // generate the lightweight TOC pane
                XElement current = element;
                IEnumerable<XElement> parents = current.XPathSelectElements("parent::HelpTOCNode/ancestor::HelpTOCNode");
                XElement parent = current.XPathSelectElement("parent::HelpTOCNode");
                IEnumerable<XElement> siblings = current.Parent.Elements("HelpTOCNode");
                IEnumerable<XElement> children = current.Elements("HelpTOCNode");

                XElement tocnav = new XElement("div", new XAttribute("id", "tocnav"));

                // the documentation root
                tocnav.Add(GenerateTocRoot(tocroot, defaultTopic, parent == null && !children.Any()));

                // all the ancestors *except* the immediate parent, always collapsed by default
                foreach (XElement ancestor in parents)
                    tocnav.Add(GenerateTocAncestors(ancestor));

                // the immediate parent is expanded if the current node has no children
                if (parent != null)
                {
                    bool expanded = !current.HasElements;
                    int level = expanded ? 1 : 0;
                    tocnav.Add(GenerateTocAncestors(parent, level, expanded));
                }

                // the siblings of the current node are shown if the parent is expanded, otherwise only the current node is shown
                foreach (XElement sibling in siblings)
                {
                    bool showSiblings;
                    int level;
                    if (parent == null && !current.HasElements)
                    {
                        showSiblings = true;
                        level = 1;
                    }
                    else
                    {
                        showSiblings = !current.HasElements;
                        level = current.HasElements || parent == null ? 1 : 2;
                    }

                    tocnav.Add(GenerateTocSiblings(current, sibling, level, showSiblings));
                }

                // the children of the current node, if any exist, are always shown by default
                foreach (XElement child in children)
                {
                    tocnav.Add(GenerateTocChildren(child));
                }

                XmlWriterSettings htmlWriterSettings = writerSettings.Clone();
                htmlWriterSettings.OmitXmlDeclaration = true;
                htmlWriterSettings.ConformanceLevel = ConformanceLevel.Fragment;

                PropertyInfo outputMethod = typeof(XmlWriterSettings).GetProperty("OutputMethod");
                outputMethod.SetValue(htmlWriterSettings, XmlOutputMethod.Html, null);

                XElement leftNav =
                    new XElement("div",
                        new XAttribute("class", "OH_leftNav"),
                        new XAttribute("id", "LeftNav"),
                        tocnav);

                string resizeIncreaseTooltip = "Expand";
                string resizeResetTooltip = "Minimize";
                XElement resizeUi =
                    new XElement("div",
                        new XAttribute("id", "TocResize"),
                        new XAttribute("class", "OH_TocResize"),
                        new XElement("img",
                            new XAttribute("id", "ResizeImageIncrease"),
                            new XAttribute("src", "../icons/open.gif"),
                            new XAttribute("onclick", "onIncreaseToc()"),
                            new XAttribute("alt", resizeIncreaseTooltip),
                            new XAttribute("title", resizeIncreaseTooltip)),
                        new XElement("img",
                            new XAttribute("id", "ResizeImageReset"),
                            new XAttribute("src", "../icons/close.gif"),
                            new XAttribute("style", "display:none"),
                            new XAttribute("onclick", "onResetToc()"),
                            new XAttribute("alt", resizeResetTooltip),
                            new XAttribute("title", resizeResetTooltip)));

                StringBuilder stringBuilder = new StringBuilder();
                using (XmlWriter writer = XmlWriter.Create(stringBuilder, htmlWriterSettings))
                {
                    leftNav.WriteTo(writer);
                    resizeUi.WriteTo(writer);
                }

                string path = Path.Combine(_builder.WorkingFolder, @"Output\Website", current.Attribute("Url").Value);
                string outputFile = File.ReadAllText(path, Encoding.UTF8);

                // left nav
                string leftNavBeforeText = "<div class=\"OH_outerContent\"";
                outputFile = outputFile.Replace(leftNavBeforeText, stringBuilder.ToString() + leftNavBeforeText);

                // initial style
                string outerContentId = " id=\"OuterContent\"";
                string initialStyle = " style=\"margin-left: 280px;\"";
                outputFile = outputFile.Replace(outerContentId, outerContentId + initialStyle);

                string outerDivClass = " class=\"OH_outerDiv\"";
                initialStyle = " style=\"padding: 35px 20px 0 20px;\"";
                outputFile = outputFile.Replace(outerDivClass, outerDivClass + initialStyle);

                // jquery
                if (outputFile.IndexOf("/jquery-") < 0)
                {
                    string jqueryScript = @"<script src=""//code.jquery.com/jquery-1.11.0.min.js""></script>";
                    outputFile = outputFile.Replace("</body>", jqueryScript + "</body>");
                }

                string script = "<script type=\"text/javascript\">$(document).ready(function () {DocumentReady();});</script>";
                outputFile = outputFile.Replace("</body>", script + "</body>");

                File.WriteAllText(path, outputFile, Encoding.UTF8);
            }

            // generate the index page
            string startPageId = defaultTopic.Attribute("id").Value;
            string startPageTitle = defaultTopic.Element("title").Value;

            StringBuilder indexPageFormat = new StringBuilder();
            indexPageFormat.AppendLine("<!DOCTYPE html>");
            indexPageFormat.AppendLine(@"<html lang=""en-US"">");
            indexPageFormat.AppendLine(@"<head>");
            indexPageFormat.AppendLine(@"	<meta charset=""UTF-8"">");
            indexPageFormat.AppendLine(@"	<meta http-equiv=""refresh"" content=""1;url=html/{0}.htm"">");
            indexPageFormat.AppendLine(@"	<script type=""text/javascript"">");
            indexPageFormat.AppendLine(@"		window.location.replace(""html/{0}.htm"")");
            indexPageFormat.AppendLine(@"	</script>");
            indexPageFormat.AppendLine(@"	<title>Page Redirection</title>");
            indexPageFormat.AppendLine(@"</head>");
            indexPageFormat.AppendLine(@"<body>");
            indexPageFormat.AppendLine(@"	If you are not redirected automatically, follow the <a href='html/{0}.htm'>link to {1}</a>");
            indexPageFormat.AppendLine(@"</body>");
            indexPageFormat.AppendLine(@"</html>");
            string indexPage = string.Format(indexPageFormat.ToString(), startPageId, startPageTitle);
            string indexPath = Path.Combine(_builder.WorkingFolder, "Output", "Website", "index.htm");
            File.WriteAllText(indexPath, indexPage, Encoding.UTF8);
        }

        private void ReplaceIndexFiles()
        {
            // delete the unnecessary index and TOC pages
            string websitePath = Path.Combine(_builder.WorkingFolder, "Output", "Website");
            File.Delete(Path.Combine(websitePath, "index.aspx"));
            File.Delete(Path.Combine(websitePath, "index.html"));
            File.Delete(Path.Combine(websitePath, "index.php"));
            File.Delete(Path.Combine(websitePath, "TOC.css"));
            File.Delete(Path.Combine(websitePath, "TOC.js"));

            File.Move(Path.Combine(websitePath, "index.htm"), Path.Combine(websitePath, "index.html"));
        }

        private XElement[] GenerateTocRoot(XElement rootTopicMetadata, XElement defaultTopicMetadata, bool expanded)
        {
            string glyphClass = expanded ? "toc_expanded" : "toc_collapsed";

            string file;
            if (defaultTopicMetadata != null)
                file = defaultTopicMetadata.Attribute("id").Value + ".htm";
            else
                file = rootTopicMetadata.Attribute("file").Value + ".htm";

            string tocTitle = rootTopicMetadata.Element("title").Value;

            XElement result =
                new XElement("div",
                    new XAttribute("class", "toclevel0"),
                    new XAttribute("data-toclevel", "0"),
                    new XAttribute("style", "padding-left: 0px;"),
                    new XElement("a",
                        new XAttribute("class", glyphClass),
                        new XAttribute("onclick", "javascript: Toggle(this);"),
                        new XAttribute("href", "#")),
                    new XElement("a",
                        new XAttribute("data-tochassubtree", "true"),
                        new XAttribute("href", file),
                        new XAttribute("title", tocTitle),
                        new XAttribute("tocid", "roottoc"),
                        new XText(tocTitle)));

            if (expanded)
                result.SetAttributeValue("data-childrenloaded", "true");

            return new[] { result };
        }

        private XElement[] GenerateTocAncestors(XElement ancestor, int level = 0, bool expanded = false)
        {
            int paddingLeft = level * 13;
            string glyphClass = expanded ? "toc_expanded" : "toc_collapsed";

            string file;
            string tocid;
            if (ancestor.Attribute("Url") != null)
            {
                file = Path.GetFileName(ancestor.Attribute("Url").Value);
                tocid = Path.GetFileNameWithoutExtension(file);
            }
            else
            {
                file = "#";
                if (ancestor.Attribute("Id") != null)
                    tocid = ancestor.Attribute("Id").Value;
                else
                    tocid = Path.GetFileNameWithoutExtension(file);
            }

            string tocTitle = ancestor.Attribute("Title").Value;

            XElement result =
                new XElement("div",
                    new XAttribute("class", "toclevel" + level),
                    new XAttribute("data-toclevel", level),
                    new XAttribute("style", "padding-left: " + paddingLeft + "px;"),
                    new XElement("a",
                        new XAttribute("class", glyphClass),
                        new XAttribute("onclick", "javascript: Toggle(this);"),
                        new XAttribute("href", "#")),
                    new XElement("a",
                        new XAttribute("data-tochassubtree", "true"),
                        new XAttribute("href", file),
                        new XAttribute("title", tocTitle),
                        new XAttribute("tocid", tocid),
                        new XText(tocTitle)));

            if (expanded)
                result.SetAttributeValue("data-childrenloaded", true);

            return new[] { result };
        }

        private XElement[] GenerateTocSiblings(XElement current, XElement sibling, int level, bool showSiblings)
        {
            int paddingLeft = 13 * level;

            string targetId;
            string targetTocId;
            if (sibling.Attribute("Url") != null)
            {
                targetId = sibling.Attribute("Url").Value;
                targetTocId = Path.GetFileNameWithoutExtension(targetId);
            }
            else
            {
                targetId = "#";
                if (sibling.Attribute("Id") != null)
                    targetTocId = sibling.Attribute("Id").Value;
                else
                    targetTocId = "#";
            }

            string currentId;
            if (current.Attribute("Url") != null)
                currentId = current.Attribute("Url").Value;
            else
                currentId = "#";

            string file = Path.GetFileName(targetId);
            string tocTitle = sibling.Attribute("Title").Value;
            string styleClassSuffix = (targetId == currentId) ? " current" : string.Empty;

            if (targetId != currentId && !showSiblings)
                return new XElement[0];

            XElement glyphElement;
            if (sibling.HasElements)
            {
                bool expanded = targetId == currentId;
                string glyphClass = expanded ? "toc_expanded" : "toc_collapsed";
                glyphElement =
                    new XElement("a",
                        new XAttribute("class", glyphClass),
                        new XAttribute("onclick", "javascript: Toggle(this);"),
                        new XAttribute("href", "#"));
            }
            else
            {
                glyphElement =
                    new XElement("span",
                        new XAttribute("class", "toc_empty"));
            }

            XElement result =
                new XElement("div",
                    new XAttribute("class", "toclevel" + level + styleClassSuffix),
                    new XAttribute("data-toclevel", level),
                    new XAttribute("style", "padding-left: " + paddingLeft + "px;"),
                    glyphElement,
                    new XElement("a",
                        new XAttribute("data-tochassubtree", sibling.HasElements),
                        new XAttribute("href", file),
                        new XAttribute("title", tocTitle),
                        new XAttribute("tocid", targetTocId),
                        new XText(tocTitle)));

            if (sibling.HasElements && targetId == currentId)
                result.SetAttributeValue("data-childrenloaded", true);

            return new[] { result };
        }

        private XElement[] GenerateTocChildren(XElement child)
        {
            int level = 2;
            int paddingLeft = 26;

            // some items in the TOC do not have actual pages associated with them
            string file;
            string tocid;
            if (child.Attribute("Url") != null)
            {
                file = Path.GetFileName(child.Attribute("Url").Value);
                tocid = Path.GetFileNameWithoutExtension(file);
            }
            else
            {
                file = "#";
                if (child.Attribute("Id") != null)
                    tocid = child.Attribute("Id").Value;
                else
                    tocid = "#";
            }

            string tocTitle = child.Attribute("Title").Value;

            XElement glyphElement;
            if (child.HasElements)
            {
                glyphElement =
                    new XElement("a",
                        new XAttribute("class", "toc_collapsed"),
                        new XAttribute("onclick", "javascript: Toggle(this);"),
                        new XAttribute("href", "#"));
            }
            else
            {
                glyphElement =
                    new XElement("span",
                        new XAttribute("class", "toc_empty"));
            }

            XElement result =
                new XElement("div",
                    new XAttribute("class", "toclevel" + level),
                    new XAttribute("data-toclevel", level),
                    new XAttribute("style", "padding-left: " + paddingLeft + "px; display: block;"),
                    glyphElement,
                    new XElement("a",
                        new XAttribute("data-tochassubtree", child.HasElements),
                        new XAttribute("href", file),
                        new XAttribute("title", tocTitle),
                        new XAttribute("tocid", tocid),
                        new XText(tocTitle)));

            return new[] { result };
        }

        /// <inheritdoc />
        public void Dispose()
        {
            Dispose(true);
            GC.SuppressFinalize(this);
        }

        protected virtual void Dispose(bool disposing)
        {
        }
    }
}
