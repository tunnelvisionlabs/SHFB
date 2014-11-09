// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change history:
// 02/09/2012 - EFW - Updated WriteParameter() so that it notes optional parameters indicated by
// OptionalAttribute alone (no default value).
// 11/30/2012 - EFW - Added updates based on changes submitted by ComponentOne to fix crashes caused by
// obfuscated member names.
// 11/20/2013 - EFW - Cleaned up the code and removed unused members.  Added code to apply visibility settings
// to property getters and setters.  Added code to write out type data for the interop attributes that are
// converted to type metadata.
// 08/06/2014 - EFW - Added code to write out values for literal (constant) fields.

using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Linq;
using System.Xml;

using System.Compiler;

using Microsoft.Ddue.Tools.Reflection;

namespace Microsoft.Ddue.Tools
{
    /// <summary>
    /// This class is used to write out information gained from managed reflection
    /// </summary>
    public class ManagedReflectionWriter : ApiVisitor
    {
        #region Private data members
        //=====================================================================

        private XmlWriter writer;
        private ApiNamer namer;

        private HashSet<string> assemblyNames;
        private Dictionary<TypeNode, List<TypeNode>> descendantIndex;
        private Dictionary<Interface, List<TypeNode>> implementorIndex;

        private List<Namespace> parsedNamespaces;
        private List<TypeNode> parsedTypes;
        private List<Member> parsedMembers;

        private Dictionary<string, List<MRefBuilderCallback>> startTagCallbacks, endTagCallbacks;

        #endregion

        #region Properties
        //=====================================================================

        /// <summary>
        /// This read-only property returns the API namer being used
        /// </summary>
        public ApiNamer ApiNamer
        {
            get { return namer; }
        }

        /// <summary>
        /// This read-only property returns an enumerable list of namespaces found
        /// </summary>
        public IEnumerable<Namespace> Namespaces
        {
            get { return parsedNamespaces; }
        }

        /// <summary>
        /// This read-only property returns an enumerable list of the types found
        /// </summary>
        public IEnumerable<TypeNode> Types
        {
            get { return parsedTypes; }
        }

        /// <summary>
        /// This read-only property returns an enumerable list of the members found
        /// </summary>
        public IEnumerable<Member> Members
        {
            get { return parsedMembers; }
        }
        #endregion

        #region Constructor
        //=====================================================================

        /// <summary>
        /// Constructor
        /// </summary>
        /// <param name="output">The text writer to which the output is written</param>
        /// <param name="namer">The API namer to use</param>
        /// <param name="resolver">The assembly resolver to use</param>
        /// <param name="filter">The API filter to use</param>
        public ManagedReflectionWriter(TextWriter output, ApiNamer namer, AssemblyResolver resolver,
          ApiFilter filter) : base(resolver, filter)
        {
            assemblyNames = new HashSet<string>();
            descendantIndex = new Dictionary<TypeNode, List<TypeNode>>();
            implementorIndex = new Dictionary<Interface, List<TypeNode>>();

            parsedNamespaces = new List<Namespace>();
            parsedTypes = new List<TypeNode>();
            parsedMembers = new List<Member>();

            startTagCallbacks = new Dictionary<string, List<MRefBuilderCallback>>();
            endTagCallbacks = new Dictionary<string, List<MRefBuilderCallback>>();

            XmlWriterSettings settings = new XmlWriterSettings();
            settings.Indent = true;
            writer = XmlWriter.Create(output, settings);

            this.namer = namer;
        }
        #endregion

        #region Dispose and visitor method overrides
        //=====================================================================

        /// <inheritdoc />
        protected override void Dispose(bool disposing)
        {
            if(disposing)
                writer.Close();

            base.Dispose(disposing);
        }

        /// <inheritdoc />
        protected override void VisitNamespaces(NamespaceList spaces)
        {
            // Construct an assembly catalog
            foreach(AssemblyNode assembly in this.Assemblies)
                assemblyNames.Add(assembly.StrongName);

            // Catalog type hierarchy and interface implementors
            foreach(var ns in spaces)
                foreach(var type in ns.Types)
                    if(base.ApiFilter.IsExposedType(type))
                    {
                        if(type.NodeType == NodeType.Class)
                            this.PopulateDescendantIndex(type);

                        this.PopulateImplementorIndex(type);
                    }

            // Start the document
            writer.WriteStartDocument();
            writer.WriteStartElement("reflection");

            // Write assembly info
            writer.WriteStartElement("assemblies");

            foreach(AssemblyNode assembly in this.Assemblies)
                this.WriteAssembly(assembly);

            writer.WriteEndElement();

            // Start API info
            writer.WriteStartElement("apis");

            this.StartElementCallbacks("apis", spaces);

            // Write out information for each namespace.  The overall list of namespaces is part of the document
            // model XSL transformation and isn't generated here.
            base.VisitNamespaces(spaces);

            // Finish API info
            this.EndElementCallbacks("apis", spaces);

            writer.WriteEndElement();

            // Finish document
            writer.WriteEndElement();
            writer.WriteEndDocument();
        }

        /// <inheritdoc />
        protected override void VisitNamespace(Namespace space)
        {
            parsedNamespaces.Add(space);

            this.WriteNamespace(space);
            base.VisitNamespace(space);
        }

        /// <inheritdoc />
        protected override void VisitType(TypeNode type)
        {
            parsedTypes.Add(type);

            this.WriteType(type);
            base.VisitType(type);
        }

        /// <inheritdoc />
        protected override void VisitMember(Member member)
        {
            parsedMembers.Add(member);

            writer.WriteStartElement("api");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("id", namer.GetMemberName(member).TranslateToValidXmlValue());

            this.StartElementCallbacks("api", member);

            this.WriteMember(member);

            this.EndElementCallbacks("api", member);

            writer.WriteEndElement();
        }
        #endregion

        #region Tag callback methods
        //=====================================================================

        /// <summary>
        /// Register a start tag callback
        /// </summary>
        /// <param name="name">The name of the element to which the callback relates</param>
        /// <param name="callback">The callback to invoke</param>
        public void RegisterStartTagCallback(string name, MRefBuilderCallback callback)
        {
            List<MRefBuilderCallback> current;

            if(!startTagCallbacks.TryGetValue(name, out current))
            {
                current = new List<MRefBuilderCallback>();
                startTagCallbacks.Add(name, current);
            }

            current.Add(callback);
        }

        /// <summary>
        /// Register an end tag callback
        /// </summary>
        /// <param name="name">The name of the element to which the callback relates</param>
        /// <param name="callback">The callback to invoke</param>
        public void RegisterEndTagCallback(string name, MRefBuilderCallback callback)
        {
            List<MRefBuilderCallback> current;

            if(!endTagCallbacks.TryGetValue(name, out current))
            {
                current = new List<MRefBuilderCallback>();
                endTagCallbacks.Add(name, current);
            }

            current.Add(callback);
        }

        /// <summary>
        /// Invoke the callbacks for the given start tag
        /// </summary>
        /// <param name="name">The start tag for which to invoke callbacks</param>
        /// <param name="info">The information to pass to the callbacks</param>
        private void StartElementCallbacks(string name, object info)
        {
            List<MRefBuilderCallback> callbacks;

            if(startTagCallbacks.TryGetValue(name, out callbacks))
                foreach(MRefBuilderCallback callback in callbacks)
                    callback(writer, info);
        }

        /// <summary>
        /// Invoke the callbacks for the given end tag
        /// </summary>
        /// <param name="name">The end tag for which to invoke callbacks</param>
        /// <param name="info">The information to pass to the callbacks</param>
        private void EndElementCallbacks(string name, object info)
        {
            List<MRefBuilderCallback> callbacks;

            if(endTagCallbacks.TryGetValue(name, out callbacks))
                foreach(MRefBuilderCallback callback in callbacks)
                    callback(writer, info);
        }
        #endregion

        #region General helper methods
        //=====================================================================

        /// <summary>
        /// This is used to check that the given character is a valid XML character
        /// </summary>
        /// <param name="c">The character to check</param>
        /// <returns>True if valid, false if not</returns>
        private static bool IsValidXmlChar(char c)
        {
            if(c < 0x20)
                return (c == 0x9 || c == 0xa);

            return (c <= 0xd7ff || (0xe000 <= c && c <= 0xfffd));
        }

        /// <summary>
        /// This is used to get an enumerable list of exposed interfaces
        /// </summary>
        /// <param name="contracts">The list of interfaces</param>
        /// <returns>An enumerable list of exposed interfaces</returns>
        private IEnumerable<Interface> GetExposedInterfaces(InterfaceList contracts)
        {
            foreach(var contract in contracts)
                if(this.ApiFilter.IsDocumentedInterface(contract))
                    yield return contract;
        }

        /// <summary>
        /// This is used to get a list of exposed implemented members
        /// </summary>
        /// <param name="members">An enumerable list of members to check</param>
        /// <returns>An enumerable list containing just the exposed, implemented members</returns>
        private IEnumerable<Member> GetExposedImplementedMembers(IEnumerable<Member> members)
        {
            foreach(Member member in members)
                if(this.ApiFilter.IsExposedMember(member))
                    yield return member;
        }

        /// <summary>
        /// This is used to get an enumerable list of the exposed attributes on a member
        /// </summary>
        /// <param name="attributes">The general attributes</param>
        /// <param name="securityAttributes">The security attributes</param>
        /// <returns>An enumerable list of the exposed attributes on the member</returns>
        private IEnumerable<AttributeNode> GetExposedAttributes(AttributeList attributes,
          SecurityAttributeList securityAttributes)
        {
            if(attributes == null)
                throw new ArgumentNullException("attributes");

            if(securityAttributes == null)
                throw new ArgumentNullException("securityAttributes");

            foreach(var attribute in attributes)
            {
                if(attribute == null)
                    throw new InvalidOperationException("Null attribute found");

                if(this.ApiFilter.IsExposedAttribute(attribute))
                    yield return attribute;
            }

            foreach(var securityAttribute in securityAttributes)
            {
                if(securityAttribute == null)
                    throw new InvalidOperationException("Null security attribute found");

                AttributeList permissionAttributes = securityAttribute.PermissionAttributes;
                
                if(permissionAttributes == null)
                    continue;

                foreach(var permissionAttribute in permissionAttributes)
                {
                    // Saw an example where this was null; ildasm shows no permission attribute, so skip it
                    if(permissionAttribute == null)
                        continue;

                    if(this.ApiFilter.IsExposedAttribute(permissionAttribute))
                        yield return permissionAttribute;
                }
            }
        }

        /// <summary>
        /// Get an enumerable list of applied fields from an enumeration type
        /// </summary>
        /// <param name="enumeration">The enumeration from which to get the fields</param>
        /// <param name="value">The value to use in determining the applied fields</param>
        /// <returns>An enumerable list of fields from the enumeration that appear in the value</returns>
        private static IEnumerable<Field> GetAppliedFields(EnumNode enumeration, long value)
        {
            FieldList list = new FieldList();
            MemberList members = enumeration.Members;

            foreach(var member in members)
            {
                if(member.NodeType != NodeType.Field)
                    continue;

                Field field = (Field)member;

                if(field.DefaultValue != null)
                {
                    long fieldValue = Convert.ToInt64(field.DefaultValue.Value, CultureInfo.InvariantCulture);

                    // If a single field matches, return it.  Otherwise return all fields that are in value.
                    if(fieldValue == value)
                        return new[] { field };

                    if((fieldValue & value) == fieldValue)
                        list.Add(field);
                }
            }

            return list;
        }

        /// <summary>
        /// This is used to create an index of parent types and their descendants
        /// </summary>
        /// <param name="type">The descendant type to add to the index</param>
        private void PopulateDescendantIndex(TypeNode child)
        {
            TypeNode parent = child.BaseType;

            if(parent != null)
            {
                // Unspecialize the parent so we see specialized types as children
                parent = parent.GetTemplateType();

                // Get the list of children for that parent (i.e. the sibling list)
                List<TypeNode> siblings;

                if(!descendantIndex.TryGetValue(parent, out siblings))
                {
                    siblings = new List<TypeNode>();
                    descendantIndex[parent] = siblings;
                }

                // Add the type in question to the sibling list
                siblings.Add(child);
            }
        }

        /// <summary>
        /// This is used to create an index of interfaces and the types that implement them
        /// </summary>
        /// <param name="type">The type to add to the index</param>
        private void PopulateImplementorIndex(TypeNode type)
        {
            List<TypeNode> implementors;

            foreach(var i in this.GetExposedInterfaces(type.Interfaces))
            {
                var contract = i;

                // Get the unspecialized form of the interface
                if(contract.IsGeneric)
                    contract = (Interface)contract.GetTemplateType();

                // Get the list of implementors
                if(!implementorIndex.TryGetValue(contract, out implementors))
                {
                    implementors = new List<TypeNode>();
                    implementorIndex[contract] = implementors;
                }

                // Add the type to it
                implementors.Add(type);
            }
        }
        #endregion

        #region Writer methods for assemblies
        //=====================================================================

        /// <summary>
        /// Write out information for an assembly
        /// </summary>
        /// <param name="assembly">The assembly to write</param>
        private void WriteAssembly(AssemblyNode assembly)
        {
            writer.WriteStartElement("assembly");

            this.WriteStringAttribute("name", assembly.Name);

            // Basic assembly data
            writer.WriteStartElement("assemblydata");
            this.WriteStringAttribute("version", assembly.Version.ToString());
            this.WriteStringAttribute("culture", assembly.Culture.ToString());

            byte[] key = assembly.PublicKeyOrToken;

            writer.WriteStartAttribute("key");
            writer.WriteBinHex(key, 0, key.Length);
            writer.WriteEndAttribute();

            this.WriteStringAttribute("hash", assembly.HashAlgorithm.ToString());
            writer.WriteEndElement();

            // Assembly attribute data
            this.WriteAttributes(assembly.Attributes, assembly.SecurityAttributes);

            writer.WriteEndElement();
        }
        #endregion

        #region Writer methods for namespaces
        //=====================================================================

        /// <summary>
        /// Write out information for a namespace
        /// </summary>
        /// <param name="space">The namespace to write</param>
        private void WriteNamespace(Namespace space)
        {
            writer.WriteStartElement("api");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("id", namer.GetNamespaceName(space).TranslateToValidXmlValue());

            this.StartElementCallbacks("api", space);

            this.WriteApiData(space);
            this.WriteNamespaceElements(space);

            this.EndElementCallbacks("api", space);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a list of namespace elements
        /// </summary>
        /// <param name="space">The namespace for which to write out elements</param>
        private void WriteNamespaceElements(Namespace space)
        {
            TypeNodeList types = space.Types;

            if(types.Count != 0)
            {
                writer.WriteStartElement("elements");

                foreach(var type in types)
                {
                    // Skip hidden types but if a type is not exposed and has exposed members we must add it
                    if(base.ApiFilter.IsExposedType(type) || base.ApiFilter.HasExposedMembers(type))
                    {
                        writer.WriteStartElement("element");

                        // !EFW - Change from ComponentOne
                        writer.WriteAttributeString("api", namer.GetTypeName(type).TranslateToValidXmlValue());

                        writer.WriteEndElement();
                    }
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out a namespace reference
        /// </summary>
        /// <param name="space">The namespace to reference</param>
        private void WriteNamespaceReference(Namespace space)
        {
            writer.WriteStartElement("namespace");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("api", namer.GetNamespaceName(space).TranslateToValidXmlValue());

            writer.WriteEndElement();
        }
        #endregion

        #region Writer methods for types
        //=====================================================================

        /// <summary>
        /// Write out information for a type
        /// </summary>
        /// <param name="type">The type to write</param>
        private void WriteType(TypeNode type)
        {
            writer.WriteStartElement("api");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("id", namer.GetTypeName(type).TranslateToValidXmlValue());

            this.StartElementCallbacks("api", type);

            this.WriteApiData(type);
            this.WriteTypeData(type);

            switch(type.NodeType)
            {
                case NodeType.Class:
                case NodeType.Struct:
                    this.WriteGenericParameters(type.TemplateParameters);
                    this.WriteInterfaces(type.Interfaces);
                    this.WriteTypeElements(type);
                    break;

                case NodeType.Interface:
                    this.WriteGenericParameters(type.TemplateParameters);
                    this.WriteInterfaces(type.Interfaces);
                    this.WriteImplementors((Interface)type);
                    this.WriteTypeElements(type);
                    break;

                case NodeType.DelegateNode:
                    DelegateNode handler = (DelegateNode)type;

                    this.WriteGenericParameters(handler.TemplateParameters);
                    this.WriteParameters(handler.Parameters);
                    this.WriteReturnValue(handler.ReturnType);
                    break;

                case NodeType.EnumNode:
                    this.WriteEnumerationData((EnumNode)type);
                    this.WriteTypeElements(type);
                    break;
            }

            this.WriteTypeContainers(type);
            this.WriteAttributes(type.Attributes, type.SecurityAttributes);

            this.EndElementCallbacks("api", type);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out type data for a type
        /// </summary>
        /// <param name="type">The type for which to write type data</param>
        private void WriteTypeData(TypeNode type)
        {
            writer.WriteStartElement("typedata");

            // Data for all types
            this.WriteStringAttribute("visibility", this.ApiFilter.GetVisibility(type));
            this.WriteBooleanAttribute("abstract", type.IsAbstract, false);
            this.WriteBooleanAttribute("sealed", type.IsSealed, false);
            this.WriteBooleanAttribute("serializable", (type.Flags & TypeFlags.Serializable) != 0);

            // Interop attribute data.  In code, these are attributes.  However, the compiler converts the
            // attribute data to type metadata so we don't see them in the regular attribute list.  The metadata
            // for the attributes is too complex to reconstruct so writing the info out as type data is much
            // simpler and can be handled by the syntax components as needed.
            if(this.ApiFilter.IncludeAttributes)
            {
                // ComImportAttribute
                if((type.Flags & TypeFlags.Import) != 0)
                    this.WriteBooleanAttribute("comimport", true);

                // StructLayoutAttribute.  If layout kind, class size, or packing size are set, assume it is
                // used.  We ignore character set as it may vary by compiler (default is Auto but C# sets it to
                // ANSI).  Structures are marked with a sequential layout and those with no members get a size of
                // one so we'll ignore the attribute in those cases too since the user didn't add them.
                if(((type.Flags & TypeFlags.LayoutMask) != 0 || type.ClassSize != 0 || type.PackingSize != 0) &&
                  (!(type is Struct) || type.ClassSize > 1))
                {
                    switch(type.Flags & TypeFlags.LayoutMask)
                    {
                        case TypeFlags.AutoLayout:
                            this.WriteStringAttribute("layout", "auto");
                            break;

                        case TypeFlags.SequentialLayout:
                            this.WriteStringAttribute("layout", "sequential");
                            break;

                        case TypeFlags.ExplicitLayout:
                            this.WriteStringAttribute("layout", "explicit");
                            break;
                    }

                    if(type.ClassSize != 0)
                        this.WriteStringAttribute("size", type.ClassSize.ToString(CultureInfo.InvariantCulture));

                    if(type.PackingSize != 0)
                        this.WriteStringAttribute("pack", type.PackingSize.ToString(CultureInfo.InvariantCulture));

                    // Equivalent to CharSet but it's been "format" forever so we won't change it now
                    switch(type.Flags & TypeFlags.StringFormatMask)
                    {
                        case TypeFlags.AnsiClass:
                            this.WriteStringAttribute("format", "ansi");
                            break;

                        case TypeFlags.UnicodeClass:
                            this.WriteStringAttribute("format", "unicode");
                            break;

                        case TypeFlags.AutoClass:
                            this.WriteStringAttribute("format", "auto");
                            break;
                    }
                }
            }

            this.StartElementCallbacks("typedata", type);
            this.EndElementCallbacks("typedata", type);

            writer.WriteEndElement();

            // For classes, recored base type
            if(type is Class)
                this.WriteHierarchy(type);
        }

        /// <summary>
        /// Write out the elements for a type
        /// </summary>
        /// <param name="type">The type for which to write out elements</param>
        private void WriteTypeElements(TypeNode type)
        {
            MemberDictionary members = new MemberDictionary(type, this.ApiFilter);

            if(members.Count != 0)
            {
                writer.WriteStartElement("elements");
                this.StartElementCallbacks("elements", members);

                foreach(Member member in members)
                {
                    writer.WriteStartElement("element");

                    Member template = member.GetTemplateMember();

                    // !EFW - Change from ComponentOne
                    writer.WriteAttributeString("api", namer.GetMemberName(template).TranslateToValidXmlValue());

                    bool write = false;

                    // Inherited, specialized generics get a displayed target different from the target.  We also
                    // write out their info since it can't be looked up anywhere.
                    if(!member.DeclaringType.IsStructurallyEquivalentTo(template.DeclaringType))
                    {
                        // !EFW - Change from ComponentOne
                        writer.WriteAttributeString("display-api", namer.GetMemberName(member).TranslateToValidXmlValue());
                        write = true;
                    }

                    // If a member is from a type in a dependency assembly, write out its info, since it can't be
                    // looked up in this file.
                    if(!assemblyNames.Contains(member.DeclaringType.DeclaringModule.ContainingAssembly.StrongName))
                        write = true;

                    if(write)
                        this.WriteMember(member);

                    writer.WriteEndElement();
                }

                this.EndElementCallbacks("elements", members);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out the hierarchy for a type
        /// </summary>
        /// <param name="type">The type for which to write the hierarchy</param>
        private void WriteHierarchy(TypeNode type)
        {
            writer.WriteStartElement("family");

            // Write ancestors
            writer.WriteStartElement("ancestors");

            for(TypeNode ancestor = type.BaseType; ancestor != null; ancestor = ancestor.BaseType)
                this.WriteTypeReference(ancestor);

            writer.WriteEndElement();

            // Write descendants
            if(descendantIndex.ContainsKey(type))
            {
                // Yes, it's misspelled but it's been that way for years and changing it would break all the
                // presentation styles so we'll leave it alone.
                writer.WriteStartElement("descendents");

                foreach(TypeNode descendant in descendantIndex[type])
                    this.WriteTypeReference(descendant);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out implementors of an interface
        /// </summary>
        /// <param name="contract"></param>
        private void WriteImplementors(Interface contract)
        {
            List<TypeNode> implementors;

            if(!implementorIndex.TryGetValue(contract, out implementors))
                return;

            if(implementors != null && implementors.Count != 0)
            {
                writer.WriteStartElement("implementors");

                this.StartElementCallbacks("implementors", implementors);

                foreach(TypeNode implementor in implementors)
                    this.WriteTypeReference(implementor);

                writer.WriteEndElement();

                this.EndElementCallbacks("implementors", implementors);
            }
        }

        /// <summary>
        /// Write out enumeration data
        /// </summary>
        /// <param name="enumeration">The enumeration for which to write out data</param>
        private void WriteEnumerationData(EnumNode enumeration)
        {
            TypeNode underlying = enumeration.UnderlyingType;

            if(underlying.FullName != "System.Int32")
            {
                writer.WriteStartElement("enumerationbase");
                this.WriteTypeReference(underlying);
                writer.WriteEndElement();
            }
        }
        #endregion

        #region Writer methods for members
        //=====================================================================

        /// <summary>
        /// Write out information for a member
        /// </summary>
        /// <param name="member">The member to write out</param>
        public void WriteMember(Member member)
        {
            this.WriteMember(member, member.DeclaringType);
        }

        /// <summary>
        /// Write out information for a member
        /// </summary>
        /// <param name="member">The member to write out</param>
        /// <param name="type">The declaring type of the member</param>
        private void WriteMember(Member member, TypeNode type)
        {
            this.WriteApiData(member);
            this.WriteMemberData(member);

            SecurityAttributeList securityAttributes = new SecurityAttributeList();

            switch(member.NodeType)
            {
                case NodeType.Field:
                    Field field = (Field)member;

                    this.WriteFieldData(field);
                    this.WriteReturnValue(field.Type);

                    // Write enumeration and literal (constant) field values
                    if(field.DeclaringType.NodeType == NodeType.EnumNode)
                    {
                        this.WriteLiteral(new Literal(field.DefaultValue.Value,
                            ((EnumNode)field.DeclaringType).UnderlyingType), false);
                    }
                    else
                        if(field.IsLiteral)
                            this.WriteLiteral(new Literal(field.DefaultValue.Value, field.Type), false);
                    break;

                case NodeType.Method:
                    Method method = (Method)member;

                    this.WriteProcedureData(method, method.OverriddenMember);

                    // Write the templates node with either the generic template parameters or the specialized
                    // template arguments.
                    if(method.TemplateArguments != null)
                        this.WriteSpecializedTemplateArguments(method.TemplateArguments);
                    else
                        this.WriteGenericParameters(method.TemplateParameters);

                    this.WriteParameters(method.Parameters);
                    this.WriteReturnValue(method.ReturnType);
                    this.WriteImplementedMembers(method.GetImplementedMethods());

                    if(method.SecurityAttributes != null)
                        securityAttributes = method.SecurityAttributes;
                    break;

                case NodeType.Property:
                    Property property = (Property)member;

                    this.WritePropertyData(property);
                    this.WriteParameters(property.Parameters);
                    this.WriteReturnValue(property.Type);
                    this.WriteImplementedMembers(property.GetImplementedProperties());
                    break;

                case NodeType.Event:
                    Event trigger = (Event)member;

                    this.WriteEventData(trigger);
                    this.WriteImplementedMembers(trigger.GetImplementedEvents());
                    break;

                case NodeType.InstanceInitializer:
                case NodeType.StaticInitializer:
                    Method constructor = (Method)member;

                    this.WriteParameters(constructor.Parameters);
                    break;
            }

            this.WriteMemberContainers(type);
            this.WriteAttributes(member.Attributes, securityAttributes);
        }

        /// <summary>
        /// Write member data such as visibility, etc.
        /// </summary>
        /// <param name="member">The member for which to write data</param>
        private void WriteMemberData(Member member)
        {
            writer.WriteStartElement("memberdata");

            this.WriteStringAttribute("visibility", this.ApiFilter.GetVisibility(member));
            this.WriteBooleanAttribute("static", member.IsStatic, false);
            this.WriteBooleanAttribute("special", member.IsSpecialName, false);

            // Nothing is done regarding overloads.  That is a document model concept and may need to be
            // tweaked after versioning.  Overloads are handled by the document model XSL transformations.

            this.WriteBooleanAttribute("default", member.IsDefaultMember(), false);

            this.StartElementCallbacks("memberdata", member);
            this.EndElementCallbacks("memberdata", member);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out field data
        /// </summary>
        /// <param name="field">The field for which to write data</param>
        private void WriteFieldData(Field field)
        {
            writer.WriteStartElement("fielddata");

            this.WriteBooleanAttribute("literal", field.IsLiteral);
            this.WriteBooleanAttribute("initonly", field.IsInitOnly);
            this.WriteBooleanAttribute("volatile", field.IsVolatile, false);
            this.WriteBooleanAttribute("serialized", (field.Flags & FieldFlags.NotSerialized) == 0);

            // FieldOffsetAttribute.  In code, this is an attribute.  However, the compiler converts the
            // attribute to metadata so we don't see it in the regular attribute list.  The metadata for the
            // attribute is too complex to reconstruct so writing the info out as field data is much simpler
            // and can be handled by the syntax components as needed.
            if(this.ApiFilter.IncludeAttributes && (field.Offset != 0 ||
              (field.DeclaringType.Flags & TypeFlags.ExplicitLayout) != 0))
                this.WriteStringAttribute("offset", field.Offset.ToString(CultureInfo.InvariantCulture));

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out specialized template arguments
        /// </summary>
        /// <param name="templateArguments">The template arguments to write</param>
        private void WriteSpecializedTemplateArguments(TypeNodeList templateArguments)
        {
            if(templateArguments != null && templateArguments.Count != 0)
            {
                writer.WriteStartElement("templates");

                foreach(var ta in templateArguments)
                    this.WriteTypeReference(ta);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out implemented members
        /// </summary>
        /// <param name="members">An enumerable list of implemented members</param>
        private void WriteImplementedMembers(IEnumerable<Member> members)
        {
            if(members.Count() != 0)
            {
                var exposedMembers = this.GetExposedImplementedMembers(members);

                if(exposedMembers.Count() != 0)
                {
                    writer.WriteStartElement("implements");

                    foreach(var member in exposedMembers)
                        this.WriteMemberReference(member);

                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Write out property data
        /// </summary>
        /// <param name="property">The property for which to write out data</param>
        private void WritePropertyData(Property property)
        {
            string propertyVisibility = this.ApiFilter.GetVisibility(property);

            Method getter = property.Getter, setter = property.Setter;

            Method accessor = getter;

            if(accessor == null)
                accessor = setter;

            // Procedure data
            this.WriteProcedureData(accessor, property.OverriddenMember);

            // Property data
            writer.WriteStartElement("propertydata");

            if(getter != null)
                if(this.ApiFilter.IsVisible(getter))
                {
                    this.WriteBooleanAttribute("get", true);

                    string getterVisibility = this.ApiFilter.GetVisibility(getter);

                    if(getterVisibility != propertyVisibility)
                        this.WriteStringAttribute("get-visibility", getterVisibility);
                }
                else
                    getter = null;

            if(setter != null)
                if(this.ApiFilter.IsVisible(setter))
                {
                    this.WriteBooleanAttribute("set", true);

                    string setterVisibility = this.ApiFilter.GetVisibility(setter);

                    if(setterVisibility != propertyVisibility)
                        this.WriteStringAttribute("set-visibility", setterVisibility);
                }
                else
                    setter = null;

            writer.WriteEndElement();

            if(getter != null)
            {
                writer.WriteStartElement("getter");

                this.WriteStringAttribute("name", "get_" + property.Name.Name);
                this.WriteAttributes(getter.Attributes, getter.SecurityAttributes);

                writer.WriteEndElement();
            }

            if(setter != null)
            {
                writer.WriteStartElement("setter");

                this.WriteStringAttribute("name", "set_" + property.Name.Name.ToString());
                this.WriteAttributes(setter.Attributes, setter.SecurityAttributes);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out event data
        /// </summary>
        /// <param name="trigger">The event for which to write data</param>
        private void WriteEventData(Event trigger)
        {
            Method adder = trigger.HandlerAdder, remover = trigger.HandlerRemover, caller = trigger.HandlerCaller;

            this.WriteProcedureData(adder, trigger.OverriddenMember);

            writer.WriteStartElement("eventdata");

            if(adder != null)
                this.WriteBooleanAttribute("add", true);

            if(remover != null)
                this.WriteBooleanAttribute("remove", true);

            if(caller != null)
                this.WriteBooleanAttribute("call", true);

            writer.WriteEndElement();

            if(adder != null)
            {
                writer.WriteStartElement("adder");

                this.WriteStringAttribute("name", "add_" + trigger.Name.Name);
                this.WriteAttributes(adder.Attributes, adder.SecurityAttributes);

                writer.WriteEndElement();
            }

            if(remover != null)
            {
                writer.WriteStartElement("remover");

                this.WriteStringAttribute("name", "remove_" + trigger.Name.Name);
                this.WriteAttributes(remover.Attributes, remover.SecurityAttributes);

                writer.WriteEndElement();
            }

            writer.WriteStartElement("eventhandler");
            this.WriteTypeReference(trigger.HandlerType);
            writer.WriteEndElement();

            // Handlers should always be delegates but I have seen a case where one is not, so check for this
            DelegateNode handler = trigger.HandlerType as DelegateNode;

            if(handler != null)
            {
                ParameterList parameters = handler.Parameters;

                if(parameters != null && parameters.Count == 2 && parameters[0].Type.FullName == "System.Object")
                {
                    writer.WriteStartElement("eventargs");
                    this.WriteTypeReference(parameters[1].Type);
                    writer.WriteEndElement();
                }
            }
        }

        /// <summary>
        /// Write out member containers
        /// </summary>
        /// <param name="type">The declaring type of the member</param>
        private void WriteMemberContainers(TypeNode type)
        {
            writer.WriteStartElement("containers");

            this.WriteLibraryReference(type.DeclaringModule);
            this.WriteNamespaceReference(type.GetNamespace());
            this.WriteTypeReference(type);

            writer.WriteEndElement();
        }
        #endregion

        #region Writer methods for various common elements
        //=====================================================================

        /// <summary>
        /// Write out API data such as the group, subgroup, etc.
        /// </summary>
        /// <param name="api"></param>
        private void WriteApiData(Member api)
        {
            writer.WriteStartElement("apidata");

            string name = api.Name.Name, group = null, subgroup = null, subsubgroup = null;

            if(api.NodeType == NodeType.Namespace)
                group = "namespace";
            else
            {
                TypeNode type = api as TypeNode;

                if(type != null)
                {
                    group = "type";

                    name = type.GetUnmangledNameWithoutTypeParameters();

                    switch(api.NodeType)
                    {
                        case NodeType.Class:
                            subgroup = "class";
                            break;

                        case NodeType.Struct:
                            subgroup = "structure";
                            break;

                        case NodeType.Interface:
                            subgroup = "interface";
                            break;

                        case NodeType.EnumNode:
                            subgroup = "enumeration";
                            break;

                        case NodeType.DelegateNode:
                            subgroup = "delegate";
                            break;
                    }
                }
                else
                {
                    group = "member";

                    switch(api.NodeType)
                    {
                        case NodeType.Field:
                            subgroup = "field";
                            break;

                        case NodeType.Property:
                            subgroup = "property";
                            break;

                        case NodeType.InstanceInitializer:
                        case NodeType.StaticInitializer:
                            subgroup = "constructor";
                            break;

                        case NodeType.Method:
                            subgroup = "method";

                            if(api.IsSpecialName && name.StartsWith("op_", StringComparison.Ordinal))
                            {
                                subsubgroup = "operator";
                                name = name.Substring(3);
                            }
                            break;

                        case NodeType.Event:
                            subgroup = "event";
                            break;
                    }

                    // Name of EIIs is just interface member name
                    int dotIndex = name.LastIndexOf('.');

                    if(dotIndex > 0)
                        name = name.Substring(dotIndex + 1);
                }
            }

            this.WriteStringAttribute("name", name);
            this.WriteStringAttribute("group", group);

            if(subgroup != null)
                this.WriteStringAttribute("subgroup", subgroup);

            if(subsubgroup != null)
                this.WriteStringAttribute("subsubgroup", subsubgroup);

            this.StartElementCallbacks("apidata", api);
            this.EndElementCallbacks("apidata", api);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a type reference
        /// </summary>
        /// <param name="type">The type to reference</param>
        public void WriteTypeReference(TypeNode type)
        {
            if(type == null)
                throw new ArgumentNullException("type");

            this.WriteStartTypeReference(type);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a member reference
        /// </summary>
        /// <param name="member">The member to reference</param>
        public void WriteMemberReference(Member member)
        {
            if(member == null)
                throw new ArgumentNullException("member");

            writer.WriteStartElement("member");

            Member template = member.GetTemplateMember();

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("api", namer.GetMemberName(template).TranslateToValidXmlValue());

            if(!member.DeclaringType.IsStructurallyEquivalentTo(template.DeclaringType))
            {
                // !EFW - Change from ComponentOne
                writer.WriteAttributeString("display-api", namer.GetMemberName(member).TranslateToValidXmlValue());
            }

            this.WriteTypeReference(member.DeclaringType);
            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out attributes
        /// </summary>
        /// <param name="attributes">The standard attributes to write</param>
        /// <param name="securityAttributes">The security attributes to write</param>
        protected void WriteAttributes(AttributeList attributes, SecurityAttributeList securityAttributes)
        {
            var exposed = this.GetExposedAttributes(attributes, securityAttributes);

            if(exposed.Count() != 0)
            {
                writer.WriteStartElement("attributes");

                foreach(var attribute in exposed)
                {
                    writer.WriteStartElement("attribute");

                    this.WriteTypeReference(attribute.Type);

                    foreach(var expression in attribute.Expressions)
                        this.WriteExpression(expression);

                    writer.WriteEndElement();
                }

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out an expression
        /// </summary>
        /// <param name="expression">The expression to write</param>
        protected void WriteExpression(Expression expression)
        {
            if(expression.NodeType == NodeType.Literal)
            {
                writer.WriteStartElement("argument");
                this.WriteLiteral((Literal)expression);
                writer.WriteEndElement();
            }
            else
                if(expression.NodeType == NodeType.NamedArgument)
                {
                    NamedArgument assignment = (NamedArgument)expression;
                    Literal value = (Literal)assignment.Value;

                    writer.WriteStartElement("assignment");

                    this.WriteStringAttribute("name", assignment.Name.Name);
                    this.WriteLiteral(value);

                    writer.WriteEndElement();
                }
        }

        /// <summary>
        /// Write out generic template parameters
        /// </summary>
        /// <param name="templateParameters">The template parameters to write</param>
        private void WriteGenericParameters(TypeNodeList templateParameters)
        {
            if(templateParameters != null && templateParameters.Count != 0)
            {
                writer.WriteStartElement("templates");

                foreach(var gp in templateParameters)
                    this.WriteGenericParameter(gp);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out a generic template parameter
        /// </summary>
        /// <param name="templateParameter">The template parameter to write</param>
        private void WriteGenericParameter(TypeNode templateParameter)
        {
            ITypeParameter itp = (ITypeParameter)templateParameter;

            writer.WriteStartElement("template");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("name", templateParameter.Name.Name.TranslateToValidXmlValue());

            // Evaluate constraints
            bool reference = ((itp.TypeParameterFlags & TypeParameterFlags.ReferenceTypeConstraint) > 0),
                value = ((itp.TypeParameterFlags & TypeParameterFlags.ValueTypeConstraint) > 0),
                constructor = ((itp.TypeParameterFlags & TypeParameterFlags.DefaultConstructorConstraint) > 0),
                contravariant = ((itp.TypeParameterFlags & TypeParameterFlags.Contravariant) > 0),
                covariant = ((itp.TypeParameterFlags & TypeParameterFlags.Covariant) > 0);

            InterfaceList interfaces = templateParameter.Interfaces;
            TypeNode parent = templateParameter.BaseType;

            // No need to show inheritance from ValueType if value flag is set
            if(value && parent != null && parent.FullName == "System.ValueType")
                parent = null;

            if(parent != null || interfaces.Count > 0 || reference || value || constructor)
            {
                writer.WriteStartElement("constrained");

                if(reference)
                    this.WriteBooleanAttribute("ref", true);

                if(value)
                    this.WriteBooleanAttribute("value", true);

                if(constructor)
                    this.WriteBooleanAttribute("ctor", true);

                if(parent != null)
                    this.WriteTypeReference(parent);

                this.WriteInterfaces(interfaces);
                writer.WriteEndElement();
            }

            if(covariant || contravariant)
            {
                writer.WriteStartElement("variance");

                if(contravariant)
                    this.WriteBooleanAttribute("contravariant", true);

                if(covariant)
                    this.WriteBooleanAttribute("covariant", true);

                writer.WriteEndElement();
            }

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out interfaces
        /// </summary>
        /// <param name="contracts">The interfaces to write out</param>
        private void WriteInterfaces(InterfaceList contracts)
        {
            var implementedContracts = GetExposedInterfaces(contracts);

            if(implementedContracts.Count() != 0)
            {
                writer.WriteStartElement("implements");

                this.StartElementCallbacks("implements", implementedContracts);

                foreach(var contract in implementedContracts)
                    this.WriteTypeReference(contract);

                writer.WriteEndElement();

                this.EndElementCallbacks("implements", implementedContracts);
            }
        }

        /// <summary>
        /// Write out a library reference
        /// </summary>
        /// <param name="module">The module to reference</param>
        private void WriteLibraryReference(Module module)
        {
            AssemblyNode assembly = module.ContainingAssembly;

            writer.WriteStartElement("library");

            this.WriteStringAttribute("assembly", assembly.Name);
            this.WriteStringAttribute("module", module.Name);
            this.WriteStringAttribute("kind", module.Kind.ToString());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a literal
        /// </summary>
        /// <param name="literal">The literal to write out</param>
        private void WriteLiteral(Literal literal)
        {
            this.WriteLiteral(literal, true);
        }

        /// <summary>
        /// Write out a literal optionally including the type
        /// </summary>
        /// <param name="literal">The literal to write out</param>
        /// <param name="showType">True to show the type, false to not show it</param>
        private void WriteLiteral(Literal literal, bool showType)
        {
            TypeNode type = literal.Type;
            Object value = literal.Value;

            if(showType)
                this.WriteTypeReference(type);

            if(value == null)
                writer.WriteElementString("nullValue", String.Empty);
            else
                if(type.NodeType == NodeType.EnumNode)
                {
                    EnumNode enumeration = (EnumNode)type;

                    writer.WriteStartElement("enumValue");

                    foreach(var field in GetAppliedFields(enumeration, Convert.ToInt64(value, CultureInfo.InvariantCulture)))
                    {
                        writer.WriteStartElement("field");

                        // !EFW - Change from ComponentOne
                        writer.WriteAttributeString("name", field.Name.Name.TranslateToValidXmlValue());
                        writer.WriteEndElement();
                    }

                    writer.WriteEndElement();
                }
                else
                    if(type.FullName == "System.Type")
                    {
                        writer.WriteStartElement("typeValue");
                        this.WriteTypeReference((TypeNode)value);
                        writer.WriteEndElement();
                    }
                    else
                    {
                        string text = value.ToString();

                        if(!text.All(c => IsValidXmlChar(c)))
                            text = String.Empty;

                        writer.WriteElementString("value", text);
                    }
        }

        /// <summary>
        /// Write out parameters
        /// </summary>
        /// <param name="parameters">The parameters to write out</param>
        private void WriteParameters(ParameterList parameters)
        {
            if(parameters.Count != 0)
            {
                writer.WriteStartElement("parameters");

                foreach(var p in parameters)
                    WriteParameter(p);

                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out a parameter
        /// </summary>
        /// <param name="parameter">The parameter to write out</param>
        private void WriteParameter(Parameter parameter)
        {
            writer.WriteStartElement("parameter");

            // !EFW - Change from ComponentOne
            writer.WriteAttributeString("name", parameter.Name.Name.TranslateToValidXmlValue());

            if(parameter.IsIn)
                this.WriteBooleanAttribute("in", true);

            if(parameter.IsOut)
                this.WriteBooleanAttribute("out", true);

            if(parameter.Attributes != null && parameter.Attributes.Any(a => a != null &&
              a.Type.FullName == "System.ParamArrayAttribute"))
                this.WriteBooleanAttribute("params", true);

            // !EFW - Support optional parameters noted by OptionalAttribute alone (no default value)
            if(parameter.IsOptional)
                this.WriteBooleanAttribute("optional", true);

            this.WriteTypeReference(parameter.Type);

            if(parameter.IsOptional && parameter.DefaultValue != null)
                this.WriteExpression(parameter.DefaultValue);

            if(parameter.Attributes != null && parameter.Attributes.Count != 0)
                this.WriteAttributes(parameter.Attributes, new SecurityAttributeList());

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out procedure data such as being abstract, virtual, etc.
        /// </summary>
        /// <param name="method">The method for which to write procedure data</param>
        /// <param name="overrides">Member data for the overridden member or null if not an override</param>
        private void WriteProcedureData(Method method, Member overrides)
        {
            writer.WriteStartElement("proceduredata");

            this.WriteBooleanAttribute("abstract", method.IsAbstract, false);
            this.WriteBooleanAttribute("virtual", method.IsVirtual);
            this.WriteBooleanAttribute("final", method.IsFinal, false);
            this.WriteBooleanAttribute("varargs", method.CallingConvention == CallingConventionFlags.VarArg, false);

            if(method.IsPrivate && method.IsVirtual)
                this.WriteBooleanAttribute("eii", true);

            // Interop attribute data.  In code, these are attributes.  However, the compiler converts the
            // attribute data to type metadata so we don't see them in the regular attribute list.  The metadata
            // for the attributes is too complex to reconstruct so writing the info out as procedure data is much
            // simpler and can be handled by the syntax components as needed.
            if(this.ApiFilter.IncludeAttributes)
            {
                // PInvoke methods may set this too.  Syntax components should omit the attribute if PInvoke
                // information is available.
                if((method.ImplFlags & MethodImplFlags.PreserveSig) != 0)
                    this.WriteBooleanAttribute("preservesig", true);

                if(method.PInvokeModule != null)
                {
                    this.WriteStringAttribute("module", method.PInvokeModule.Name);

                    if(!String.IsNullOrEmpty(method.PInvokeImportName) && method.PInvokeImportName != method.Name.Name)
                        this.WriteStringAttribute("entrypoint", method.PInvokeImportName);

                    switch(method.PInvokeFlags & PInvokeFlags.CallingConvMask)
                    {
                        case PInvokeFlags.CallConvCdecl:
                            this.WriteStringAttribute("callingconvention", "cdecl");
                            break;

                        case PInvokeFlags.CallConvFastcall:
                            this.WriteStringAttribute("callingconvention", "fastcall");
                            break;

                        case PInvokeFlags.CallConvStdcall:
                            this.WriteStringAttribute("callingconvention", "stdcall");
                            break;

                        case PInvokeFlags.CallConvThiscall:
                            this.WriteStringAttribute("callingconvention", "thiscall");
                            break;
                    }

                    if((method.PInvokeFlags & PInvokeFlags.CharSetMask) != 0)
                        switch(method.PInvokeFlags & PInvokeFlags.CharSetMask)
                        {
                            case PInvokeFlags.CharSetAns:
                                this.WriteStringAttribute("charset", "ansi");
                                break;

                            case PInvokeFlags.CharSetUnicode:
                                this.WriteStringAttribute("charset", "unicode");
                                break;

                            case PInvokeFlags.CharSetAuto:
                                this.WriteStringAttribute("charset", "auto");
                                break;
                        }

                    if((method.PInvokeFlags & PInvokeFlags.BestFitDisabled) != 0)
                        this.WriteBooleanAttribute("bestfitmapping", false);

                    if((method.PInvokeFlags & PInvokeFlags.NoMangle) != 0)
                        this.WriteBooleanAttribute("exactspelling", true);

                    if((method.PInvokeFlags & PInvokeFlags.ThrowOnUnmappableCharEnabled) != 0)
                        this.WriteBooleanAttribute("throwonunmappablechar", true);

                    if((method.PInvokeFlags & PInvokeFlags.SupportsLastError) != 0)
                        this.WriteBooleanAttribute("setlasterror", true);
                }
            }

            writer.WriteEndElement();

            if(overrides != null)
            {
                writer.WriteStartElement("overrides");
                this.WriteMemberReference(overrides);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write a starting type reference element based on the type node
        /// </summary>
        /// <param name="type">The type for which to write a starting type reference</param>
        private void WriteStartTypeReference(TypeNode type)
        {
            switch(type.NodeType)
            {
                case NodeType.ArrayType:
                    ArrayType array = type as ArrayType;
                    writer.WriteStartElement("arrayOf");
                    writer.WriteAttributeString("rank", array.Rank.ToString(CultureInfo.InvariantCulture));
                    this.WriteTypeReference(array.ElementType);
                    break;

                case NodeType.Reference:
                    Reference reference = type as Reference;
                    writer.WriteStartElement("referenceTo");
                    this.WriteTypeReference(reference.ElementType);
                    break;

                case NodeType.Pointer:
                    Pointer pointer = type as Pointer;
                    writer.WriteStartElement("pointerTo");
                    this.WriteTypeReference(pointer.ElementType);
                    break;

                case NodeType.OptionalModifier:
                    TypeModifier optionalModifierClause = type as TypeModifier;
                    this.WriteStartTypeReference(optionalModifierClause.ModifiedType);
                    writer.WriteStartElement("optionalModifier");
                    this.WriteTypeReference(optionalModifierClause.Modifier);
                    writer.WriteEndElement();
                    break;

                case NodeType.RequiredModifier:
                    TypeModifier requiredModifierClause = type as TypeModifier;
                    this.WriteStartTypeReference(requiredModifierClause.ModifiedType);
                    writer.WriteStartElement("requiredModifier");
                    this.WriteTypeReference(requiredModifierClause.Modifier);
                    writer.WriteEndElement();
                    break;

                default:
                    if(type.IsTemplateParameter)
                    {
                        ITypeParameter gtp = (ITypeParameter)type;
                        writer.WriteStartElement("template");

                        // !EFW - Change from ComponentOne
                        writer.WriteAttributeString("name", type.Name.Name.TranslateToValidXmlValue());
                        writer.WriteAttributeString("index", gtp.ParameterListIndex.ToString(CultureInfo.InvariantCulture));
                        writer.WriteAttributeString("api", namer.GetApiName(gtp.DeclaringMember).TranslateToValidXmlValue());
                    }
                    else
                    {
                        writer.WriteStartElement("type");

                        if(type.IsGeneric)
                        {
                            TypeNode template = type.GetTemplateType();

                            // !EFW - Change from ComponentOne
                            writer.WriteAttributeString("api", namer.GetTypeName(template).TranslateToValidXmlValue());
                            this.WriteBooleanAttribute("ref", !template.IsValueType);

                            // Record specialization							
                            TypeNodeList arguments = type.TemplateArguments;

                            if(arguments != null && arguments.Count > 0)
                            {
                                writer.WriteStartElement("specialization");

                                foreach(var arg in arguments)
                                    this.WriteTypeReference(arg);

                                writer.WriteEndElement();
                            }

                        }
                        else
                        {
                            // !EFW - Change from ComponentOne
                            writer.WriteAttributeString("api", namer.GetTypeName(type).TranslateToValidXmlValue());
                            this.WriteBooleanAttribute("ref", !type.IsValueType);
                        }

                        // Record outer types because they may be specialized and otherwise that information
                        // is lost.
                        if(type.DeclaringType != null)
                            this.WriteTypeReference(type.DeclaringType);
                    }
                    break;
            }
        }

        /// <summary>
        /// Write out type containers
        /// </summary>
        /// <param name="type">The type for which to write out containers</param>
        private void WriteTypeContainers(TypeNode type)
        {
            writer.WriteStartElement("containers");

            this.WriteLibraryReference(type.DeclaringModule);
            this.WriteNamespaceReference(type.GetNamespace());

            // For nested types, record outer types
            TypeNode outer = type.DeclaringType;

            if(outer != null)
                this.WriteTypeReference(outer);

            writer.WriteEndElement();
        }

        /// <summary>
        /// Write out a field or return value type
        /// </summary>
        /// <param name="type">The return value type</param>
        private void WriteReturnValue(TypeNode type)
        {
            if(type.FullName != "System.Void")
            {
                writer.WriteStartElement("returns");
                this.WriteTypeReference(type);
                writer.WriteEndElement();
            }
        }

        /// <summary>
        /// Write out a Boolean attribute value
        /// </summary>
        /// <param name="attribute">The attribute name</param>
        /// <param name="value">The attribute value</param>
        private void WriteBooleanAttribute(string attribute, bool value)
        {
            writer.WriteAttributeString(attribute, value ? "true" : "false");
        }

        /// <summary>
        /// Write out a Boolean attribute value if it does not match the given default value
        /// </summary>
        /// <param name="attribute">The attribute name</param>
        /// <param name="value">The attribute value</param>
        /// <param name="defaultValue">The default attribute value</param>
        private void WriteBooleanAttribute(string attribute, bool value, bool defaultValue)
        {
            if(value != defaultValue)
                writer.WriteAttributeString(attribute, value ? "true" : "false");
        }

        /// <summary>
        /// Write out a string attribute value
        /// </summary>
        /// <param name="attribute">The attribute name</param>
        /// <param name="value">The attribute value</param>
        /// <remarks>The value is checked for invalid XML characters.  Any that are found are removed first.</remarks>
        private void WriteStringAttribute(string attribute, string value)
        {
            // !EFW - Change from ComponentOne
            writer.WriteAttributeString(attribute, value.TranslateToValidXmlValue());
        }
        #endregion
    }
}
