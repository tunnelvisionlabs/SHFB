// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change history:
// 02/14/2012 - EFW - Made the unsafe code checks consistent across all syntax generators

using System;
using System.Collections.Generic;
using System.Globalization;
using System.Xml.XPath;

namespace Microsoft.Ddue.Tools
{
    public sealed class VisualBasicUsageSyntaxGenerator : SyntaxGeneratorTemplate
    {
        public VisualBasicUsageSyntaxGenerator(XPathNavigator configuration) : base(configuration)
        {
            if(String.IsNullOrEmpty(Language))
                Language = "VisualBasicUsage";
        }

        public override void WriteNamespaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);

            writer.WriteKeyword("Imports");
            writer.WriteString(" ");
            writer.WriteIdentifier(name);
        }

        private void TypeDeclaration(XPathNavigator reflection, SyntaxWriter writer)
        {
            TypeDeclaration(reflection, writer, false);
        }

        private void TypeDeclaration(XPathNavigator reflection, SyntaxWriter writer, bool writeVariance)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);

            writer.WriteKeyword("Dim");
            writer.WriteString(" ");
            writer.WriteParameter("instance");
            writer.WriteString(" ");
            writer.WriteKeyword("As");
            writer.WriteString(" ");
            if(declaringType != null)
            {
                WriteTypeReference(declaringType, writer);
                writer.WriteString(".");
            }
            if(reservedWords.Contains(name))
            {
                writer.WriteString("[");
                writer.WriteIdentifier(name);
                writer.WriteString("]");
            }
            else
            {
                writer.WriteIdentifier(name);
            }
            WriteGenericTemplates(reflection, writer, writeVariance);
        }

        private static void WriteGenericTemplates(XPathNavigator type, SyntaxWriter writer, bool writeVariance)
        {
            XPathNodeIterator templates = type.Select(apiTemplatesExpression);

            if(templates.Count == 0)
                return;
            writer.WriteString("(");
            writer.WriteKeyword("Of");
            writer.WriteString(" ");
            while(templates.MoveNext())
            {
                XPathNavigator template = templates.Current;
                if(templates.CurrentPosition > 1)
                    writer.WriteString(", ");
                if(writeVariance)
                {
                    bool contravariant = (bool)template.Evaluate(templateIsContravariantExpression);
                    bool covariant = (bool)template.Evaluate(templateIsCovariantExpression);

                    if(contravariant)
                    {
                        writer.WriteKeyword("In");
                        writer.WriteString(" ");
                    }
                    if(covariant)
                    {
                        writer.WriteKeyword("Out");
                        writer.WriteString(" ");
                    }
                }

                string name = template.GetAttribute("name", String.Empty);
                writer.WriteString(name);
            }
            writer.WriteString(")");

        }

        private void ParameterDeclaration(string name, XPathNavigator type, SyntaxWriter writer)
        {
            writer.WriteKeyword("Dim");
            writer.WriteString(" ");
            writer.WriteParameter(name);
            writer.WriteString(" ");
            writer.WriteKeyword("As");
            writer.WriteString(" ");

            string typeName = (string)type.Evaluate(apiNameExpression);

            if(reservedWords.Contains(typeName))
            {
                writer.WriteString("[");
                WriteTypeReference(type, writer);
                writer.WriteString("]");
            }
            else
            {
                WriteTypeReference(type, writer);
            }

            writer.WriteLine();
        }

        public override void WriteClassSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            bool isAbstract = (bool)reflection.Evaluate(apiIsAbstractTypeExpression);
            bool isSealed = (bool)reflection.Evaluate(apiIsSealedTypeExpression);

            if(isAbstract && isSealed)
            {
                writer.WriteMessage("UnsupportedStaticClass_" + Language);
            }
            else
            {
                TypeDeclaration(reflection, writer);
            }

        }

        public override void WriteStructureSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            TypeDeclaration(reflection, writer);
        }

        public override void WriteInterfaceSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            TypeDeclaration(reflection, writer, true);  // Need to write variance info for interfaces and delegates
        }

        public override void WriteDelegateSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if(IsUnsupportedUnsafe(reflection, writer))
                return;

            string name = (string)reflection.Evaluate(apiNameExpression);

            writer.WriteKeyword("Dim");
            writer.WriteString(" ");
            writer.WriteParameter("instance");
            writer.WriteString(" ");
            writer.WriteKeyword("As");
            writer.WriteString(" ");
            writer.WriteKeyword("New");
            writer.WriteString(" ");

            if(reservedWords.Contains(name))
            {
                writer.WriteString("[");
                writer.WriteIdentifier(name);
                writer.WriteString("]");
            }
            else
            {
                writer.WriteIdentifier(name);
            }

            WriteGenericTemplates(reflection, writer, true); // Need to write variance info for interfaces and delegates
            writer.WriteString("(");
            writer.WriteKeyword("AddressOf");
            writer.WriteString(" HandlerMethod)");
        }

        public override void WriteEnumerationSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            TypeDeclaration(reflection, writer);
        }

        public override void WriteFieldSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if(IsUnsupportedUnsafe(reflection, writer))
                return;

            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isLiteral = (bool)reflection.Evaluate(apiIsLiteralFieldExpression);
            bool isInitOnly = (bool)reflection.Evaluate(apiIsInitOnlyFieldExpression);
            bool isFamily = (bool)reflection.Evaluate(apiIsFamilyMemberExpression);
            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);
            XPathNavigator fieldType = reflection.SelectSingleNode(apiReturnTypeExpression);

            if(!(isStatic || isFamily))
                ParameterDeclaration("instance", declaringType, writer);
            ParameterDeclaration("value", fieldType, writer);

            // get value
            writer.WriteLine();
            writer.WriteParameter("value");
            writer.WriteString(" = ");
            if(isStatic)
            {
                WriteTypeReference(declaringType, writer);
            }
            else
            {
                if(isFamily)
                {
                    writer.WriteKeyword("Me");
                }
                else
                {
                    writer.WriteParameter("instance");
                }
            }
            writer.WriteString(".");
            writer.WriteIdentifier(name);
            writer.WriteLine();

            // set value
            if(isLiteral || isInitOnly)
                return;
            writer.WriteLine();
            if(isStatic)
            {
                WriteTypeReference(declaringType, writer);
            }
            else
            {
                if(isFamily)
                {
                    writer.WriteKeyword("Me");
                }
                else
                {
                    writer.WriteParameter("instance");
                }
            }
            writer.WriteString(".");
            writer.WriteIdentifier(name);
            writer.WriteString(" = ");
            writer.WriteParameter("value");

        }

        public override void WriteConstructorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if(IsUnsupportedUnsafe(reflection, writer))
                return;

            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);

            // if static, no usage

            WriteParameterDeclarations(reflection, writer);
            writer.WriteLine();
            writer.WriteKeyword("Dim");
            writer.WriteString(" ");
            writer.WriteParameter("instance");
            writer.WriteString(" ");
            writer.WriteKeyword("As New");
            writer.WriteString(" ");

            string typeName = (string)declaringType.Evaluate(apiNameExpression);

            if(reservedWords.Contains(typeName))
            {
                writer.WriteString("[");
                WriteTypeReference(declaringType, writer);
                writer.WriteString("]");
            }
            else
            {
                WriteTypeReference(declaringType, writer);
            }

            WriteMethodParameters(reflection, writer);
        }

        public override void WriteMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if(IsUnsupportedUnsafe(reflection, writer))
                return;
            if(IsUnsupportedVarargs(reflection, writer))
                return;
            base.WriteMethodSyntax(reflection, writer);
        }

        public override void WriteNormalMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            bool isExtension = (bool)reflection.Evaluate(apiIsExtensionMethod);
            if(isExtension)
            {
                WriteExtensionMethodSyntax(reflection, writer);
            }
            else
            {

                //string name = (string)reflection.Evaluate(apiNameExpression);
                XPathNavigator returnType = reflection.SelectSingleNode(apiReturnTypeExpression);
                XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);
                //bool isExplicit = (bool)reflection.Evaluate(apiIsExplicitImplementationExpression);
                bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
                bool isFamily = (bool)reflection.Evaluate(apiIsFamilyMemberExpression);

                if(!(isStatic || isFamily))
                    ParameterDeclaration("instance", declaringType, writer);
                WriteParameterDeclarations(reflection, writer);
                if(returnType != null)
                    ParameterDeclaration("returnValue", returnType, writer);
                writer.WriteLine();

                if(returnType != null)
                {
                    writer.WriteParameter("returnValue");
                    writer.WriteString(" = ");
                }

                WriteMemberName(reflection, writer);
                WriteMethodParameters(reflection, writer);

            }
        }

        // what about generic methods?

        private void WriteMemberName(XPathNavigator reflection, SyntaxWriter writer)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);
            bool isExplicit = (bool)reflection.Evaluate(apiIsExplicitImplementationExpression);
            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isFamily = (bool)reflection.Evaluate(apiIsFamilyMemberExpression);
            bool isDefault = (bool)reflection.Evaluate(apiIsDefaultMemberExpression);
            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);
            if(isExplicit)
            {
                XPathNavigator member = reflection.SelectSingleNode(apiImplementedMembersExpression);
                XPathNavigator contract = member.SelectSingleNode(memberDeclaringTypeExpression);
                writer.WriteKeyword("CType");
                writer.WriteString("(");
                writer.WriteParameter("instance");
                writer.WriteString(", ");
                WriteTypeReference(contract, writer);
                writer.WriteString(").");
                WriteMemberReference(member, writer);
            }
            else
            {
                if(isStatic)
                {
                    WriteTypeReference(declaringType, writer);
                }
                else
                {
                    if(isFamily)
                    {
                        writer.WriteKeyword("Me");
                    }
                    else
                    {
                        writer.WriteString("instance");
                    }
                }
                if(!isDefault)
                {
                    writer.WriteString(".");
                    writer.WriteIdentifier(name);
                }
            }
        }

        private void WriteExtensionMethodSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {

            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator returnType = reflection.SelectSingleNode(apiReturnTypeExpression);
            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

            // extract the first parameter as the extension type
            parameters.MoveNext();
            XPathNavigator extendedType = parameters.Current.SelectSingleNode(parameterTypeExpression);
            string extendedTypeName = (string)parameters.Current.Evaluate(parameterNameExpression);

            // write the declarations
            ParameterDeclaration(extendedTypeName, extendedType, writer);
            WriteParameterDeclarations(parameters.Clone(), writer);
            if(returnType != null)
                ParameterDeclaration("returnValue", returnType, writer);
            writer.WriteLine();

            // write the method invocation
            if(returnType != null)
            {
                writer.WriteParameter("returnValue");
                writer.WriteString(" = ");
            }
            writer.WriteParameter(extendedTypeName);
            writer.WriteString(".");
            writer.WriteIdentifier(name);
            writer.WriteString("(");
            WriteParameters(parameters.Clone(), writer);
            writer.WriteString(")");

        }

        public override void WriteOperatorSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            string name = (string)reflection.Evaluate(apiNameExpression);
            XPathNavigator returnType = reflection.SelectSingleNode(apiReturnTypeExpression);

            // Determine operator identifier and type
            string identifier = null;
            int type = 0;

            if(!(bool)reflection.Evaluate(apiIsUdtReturnExpression))
            {
                switch(name)
                {
                    // unary math operators
                    case "UnaryPlus":
                        identifier = "+";
                        type = -1;
                        break;
                    case "UnaryNegation":
                        identifier = "-";
                        type = -1;
                        break;
                    case "Increment":
                        identifier = "++";
                        type = +1;
                        break;
                    case "Decrement":
                        identifier = "--";
                        type = +1;
                        break;
                    // unary logical operators
                    case "LogicalNot":
                        identifier = "Not";
                        type = -1;
                        break;
                    case "True":
                        identifier = "IsTrue";
                        type = -1;
                        break;
                    case "False":
                        identifier = "IsFalse";
                        type = -1;
                        break;
                    // binary comparison operators
                    case "Equality":
                        identifier = "=";
                        type = 2;
                        break;
                    case "Inequality":
                        identifier = "<>";
                        type = 2;
                        break;
                    case "LessThan":
                        identifier = "<";
                        type = 2;
                        break;
                    case "GreaterThan":
                        identifier = ">";
                        type = 2;
                        break;
                    case "LessThanOrEqual":
                        identifier = "<=";
                        type = 2;
                        break;
                    case "GreaterThanOrEqual":
                        identifier = ">=";
                        type = 2;
                        break;
                    // binary math operators
                    case "Addition":
                        identifier = "+";
                        type = 2;
                        break;
                    case "Subtraction":
                        identifier = "-";
                        type = 2;
                        break;
                    case "Multiply":
                        identifier = "*";
                        type = 2;
                        break;
                    case "Division":
                        identifier = "/";
                        type = 2;
                        break;
                    case "Exponent":
                        identifier = "^";
                        type = 2;
                        break;
                    case "Modulus":
                        identifier = "Mod";
                        type = 2;
                        break;
                    case "IntegerDivision":
                        identifier = @"\";
                        type = 2;
                        break;
                    // binary logical operators
                    case "BitwiseAnd":
                        identifier = "And";
                        type = 2;
                        break;
                    case "BitwiseOr":
                        identifier = "Or";
                        type = 2;
                        break;
                    case "ExclusiveOr":
                        identifier = "Xor";
                        type = 2;
                        break;
                    // bit-array operators
                    case "OnesComplement":
                        identifier = "~";
                        type = -1;
                        break;
                    case "LeftShift":
                        identifier = "<<";
                        type = 2;
                        break;
                    case "RightShift":
                        identifier = ">>";
                        type = 2;
                        break;
                    // concatenation
                    case "Concatenate":
                        identifier = "&";
                        type = 2;
                        break;
                    case "Assign":
                        identifier = "=";
                        type = 2;
                        break;


                    // didn't recognize an operator
                    default:
                        identifier = null;
                        type = 0;
                        break;
                }
            }
            if(identifier == null)
            {
                writer.WriteMessage("UnsupportedOperator_" + Language);
            }
            else
            {

                XPathNodeIterator parameters = reflection.Select(apiParametersExpression);
                if(parameters.Count != Math.Abs(type))
                {
                    writer.WriteMessage("UnsupportedOperator_" + Language);
                    return;
                }   //throw new InvalidOperationException("An operator has the wrong number of parameters.");

                WriteParameterDeclarations(reflection, writer);
                ParameterDeclaration("returnValue", returnType, writer);
                writer.WriteLine();
                writer.WriteParameter("returnValue");
                writer.WriteString(" = ");
                switch(type)
                {
                    case -1:
                        writer.WriteIdentifier(identifier);
                        parameters.MoveNext();
                        writer.WriteParameter((string)parameters.Current.Evaluate(parameterNameExpression));
                        break;
                    case +1:
                        parameters.MoveNext();
                        writer.WriteParameter((string)parameters.Current.Evaluate(parameterNameExpression));
                        writer.WriteIdentifier(identifier);
                        break;
                    case 2:
                        writer.WriteString("(");

                        // parameter 1
                        parameters.MoveNext();
                        writer.WriteParameter((string)parameters.Current.Evaluate(parameterNameExpression));

                        writer.WriteString(" ");
                        writer.WriteIdentifier(identifier);
                        writer.WriteString(" ");

                        // parameter 2
                        parameters.MoveNext();
                        writer.WriteParameter((string)parameters.Current.Evaluate(parameterNameExpression));

                        writer.WriteString(")");
                        break;
                }
            }

        }

        public override void WriteCastSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            XPathNavigator parameter = reflection.SelectSingleNode(apiParametersExpression);
            if(parameter == null)
                return;
            XPathNavigator inputType = parameter.SelectSingleNode(typeExpression);
            XPathNavigator outputType = reflection.SelectSingleNode(apiReturnTypeExpression);
            if((inputType == null) || (outputType == null))
                return;

            ParameterDeclaration("input", inputType, writer);
            ParameterDeclaration("output", outputType, writer);
            writer.WriteLine();
            writer.WriteParameter("output");
            writer.WriteString(" = ");
            writer.WriteKeyword("CType");
            writer.WriteString("(");
            writer.WriteParameter("input");
            writer.WriteString(", ");
            WriteTypeReference(outputType, writer);
            writer.WriteString(")");
        }

        public override void WritePropertySyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            if(IsUnsupportedUnsafe(reflection, writer))
                return;

            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isFamily = (bool)reflection.Evaluate(apiIsFamilyMemberExpression);
            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);
            XPathNavigator propertyType = reflection.SelectSingleNode(apiReturnTypeExpression);
            bool getter = (bool)reflection.Evaluate(apiIsReadPropertyExpression);
            bool setter = (bool)reflection.Evaluate(apiIsWritePropertyExpression);

            if(!(isStatic || isFamily))
                ParameterDeclaration("instance", declaringType, writer);
            WriteParameterDeclarations(reflection, writer);
            ParameterDeclaration("value", propertyType, writer);

            // get value
            if(getter)
            {
                string getVisibility = (string)reflection.Evaluate(apiGetVisibilityExpression);
                if(string.IsNullOrEmpty(getVisibility) || (getVisibility != "assembly" &&
                    getVisibility != "private" && getVisibility != "family and assembly"))
                {
                    writer.WriteLine();
                    writer.WriteParameter("value");
                    writer.WriteString(" = ");
                    WriteMemberName(reflection, writer);
                    WritePropertyParameters(reflection, writer);
                    writer.WriteLine();
                }
            }

            // set value
            if(setter)
            {
                string setVisibility = (string)reflection.Evaluate(apiSetVisibilityExpression);
                if(string.IsNullOrEmpty(setVisibility) || (setVisibility != "assembly" &&
                    setVisibility != "private" && setVisibility != "family and assembly"))
                {
                    writer.WriteLine();
                    WriteMemberName(reflection, writer);
                    WritePropertyParameters(reflection, writer);
                    writer.WriteString(" = ");
                    writer.WriteParameter("value");
                }
            }

        }

        public override void WriteEventSyntax(XPathNavigator reflection, SyntaxWriter writer)
        {
            // !EFW - Added unsafe check
            if(IsUnsupportedUnsafe(reflection, writer))
                return;

            bool isStatic = (bool)reflection.Evaluate(apiIsStaticExpression);
            bool isFamily = (bool)reflection.Evaluate(apiIsFamilyMemberExpression);
            XPathNavigator declaringType = reflection.SelectSingleNode(apiContainingTypeExpression);
            XPathNavigator handler = reflection.SelectSingleNode(apiHandlerOfEventExpression);

            if(!(isStatic | isFamily))
                ParameterDeclaration("instance", declaringType, writer);
            ParameterDeclaration("handler", handler, writer);

            // adder
            writer.WriteLine();
            writer.WriteKeyword("AddHandler");
            writer.WriteString(" ");
            WriteMemberName(reflection, writer);
            writer.WriteString(", ");
            writer.WriteParameter("handler");
            writer.WriteLine();
        }

        private void WriteParameterDeclarations(XPathNavigator reflection, SyntaxWriter writer)
        {
            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);
            if(parameters.Count == 0)
                return;
            WriteParameterDeclarations(parameters, writer);
        }

        private void WriteParameterDeclarations(XPathNodeIterator parameters, SyntaxWriter writer)
        {
            while(parameters.MoveNext())
            {
                XPathNavigator parameter = parameters.Current;
                XPathNavigator type = parameter.SelectSingleNode(parameterTypeExpression);
                string name = (string)parameter.Evaluate(parameterNameExpression);
                ParameterDeclaration(name, type, writer);
            }
        }

        private static void WriteMethodParameters(XPathNavigator reflection, SyntaxWriter writer)
        {
            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

            writer.WriteString("(");

            if(parameters.Count > 0)
                WriteParameters(parameters, writer);

            writer.WriteString(")");
        }

        private static void WritePropertyParameters(XPathNavigator reflection, SyntaxWriter writer)
        {
            XPathNodeIterator parameters = reflection.Select(apiParametersExpression);

            if(parameters.Count == 0)
                return;

            writer.WriteString("(");
            WriteParameters(parameters, writer);
            writer.WriteString(")");
        }

        private static void WriteParameters(XPathNodeIterator parameters, SyntaxWriter writer)
        {
            while(parameters.MoveNext())
            {
                XPathNavigator parameter = parameters.Current;
                string name = (string)parameter.Evaluate(parameterNameExpression);
                writer.WriteParameter(name);

                if(parameters.CurrentPosition < parameters.Count)
                {
                    writer.WriteString(", ");
                    if(writer.Position > maxPosition)
                    {
                        writer.WriteString("_");
                        writer.WriteLine();
                        writer.WriteString("\t");
                    }
                }
            }
        }

        private void WriteTypeReference(XPathNavigator reference, SyntaxWriter writer)
        {
            switch(reference.LocalName)
            {
                case "arrayOf":
                    int rank = Convert.ToInt32(reference.GetAttribute("rank", String.Empty),
                        CultureInfo.InvariantCulture);

                    XPathNavigator element = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(element, writer);
                    writer.WriteString("(");

                    for(int i = 1; i < rank; i++)
                        writer.WriteString(",");

                    writer.WriteString(")");
                    break;
                case "pointerTo":
                    XPathNavigator pointee = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(pointee, writer);
                    writer.WriteString("*");
                    break;
                case "referenceTo":
                    XPathNavigator referee = reference.SelectSingleNode(typeExpression);
                    WriteTypeReference(referee, writer);
                    break;
                case "type":
                    string id = reference.GetAttribute("api", String.Empty);
                    WriteNormalTypeReference(id, writer);
                    XPathNodeIterator typeModifiers = reference.Select(typeModifiersExpression);
                    while(typeModifiers.MoveNext())
                    {
                        WriteTypeReference(typeModifiers.Current, writer);
                    }
                    break;
                case "template":
                    string name = reference.GetAttribute("name", String.Empty);
                    writer.WriteString(name);
                    XPathNodeIterator modifiers = reference.Select(typeModifiersExpression);
                    while(modifiers.MoveNext())
                    {
                        WriteTypeReference(modifiers.Current, writer);
                    }
                    break;
                case "specialization":
                    writer.WriteString("(");
                    writer.WriteKeyword("Of");
                    writer.WriteString(" ");
                    XPathNodeIterator arguments = reference.Select(specializationArgumentsExpression);
                    while(arguments.MoveNext())
                    {
                        if(arguments.CurrentPosition > 1)
                            writer.WriteString(", ");
                        WriteTypeReference(arguments.Current, writer);
                    }
                    writer.WriteString(")");
                    break;
            }
        }

        private static void WriteNormalTypeReference(string reference, SyntaxWriter writer)
        {
            switch(reference)
            {
                case "T:System.Int16":
                    writer.WriteReferenceLink(reference, "Short");
                    break;
                case "T:System.Int32":
                    writer.WriteReferenceLink(reference, "Integer");
                    break;
                case "T:System.Int64":
                    writer.WriteReferenceLink(reference, "Long");
                    break;
                case "T:System.UInt16":
                    writer.WriteReferenceLink(reference, "UShort");
                    break;
                case "T:System.UInt32":
                    writer.WriteReferenceLink(reference, "UInteger");
                    break;
                case "T:System.UInt64":
                    writer.WriteReferenceLink(reference, "ULong");
                    break;
                default:
                    writer.WriteReferenceLink(reference);
                    break;
            }
        }

        private static void WriteMemberReference(XPathNavigator member, SyntaxWriter writer)
        {
            string api = member.GetAttribute("api", String.Empty);
            writer.WriteReferenceLink(api);
        }

        private static HashSet<string> reservedWords = new HashSet<string>
        {
            "Alias",
            "Assembly",
            "Class",
            "Delegate",
            "Function",
            "Handles",
            "Interface",
            "Loop",
            "Module",
            "New",
            "Next",
            "Nothing",
            "Operator",
            "Option",
            "Property",
            "Structure",
        };
    }
}
