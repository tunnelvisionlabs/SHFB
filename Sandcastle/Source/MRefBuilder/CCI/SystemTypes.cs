// Copyright � Microsoft Corporation.
// This source file is subject to the Microsoft Permissive License.
// See http://www.microsoft.com/resources/sharedsource/licensingbasics/sharedsourcelicenses.mspx.
// All other rights reserved.

// Change history:
// 09/10/2012 - EFW - Added support to the TargetPlatform class for using the Frameworks.xml file to load
// framework assembly information.
// 11/22/2013 - EFW - Cleared out the conditional statements

using System.Diagnostics;
using System.IO;
using System.Linq;

using System.Compiler.Metadata;

namespace System.Compiler
{
    public static class SystemAssemblyLocation
    {
        static string location;

        public static string Location
        {
            get
            {
                return location;
            }
            set
            {
                //Debug.Assert(location == null || location == value, string.Format("You attempted to set the mscorlib.dll location to\r\n\r\n{0}\r\n\r\nbut it was already set to\r\n\r\n{1}\r\n\r\nThis may occur if you have multiple projects that target different platforms. Make sure all of your projects target the same platform.\r\n\r\nYou may try to continue, but targeting multiple platforms during the same session is not supported, so you may see erroneous behavior.", value, location));
                location = value;
            }
        }
        public static AssemblyNode ParsedAssembly;
    }

    public static class SystemDataAssemblyLocation
    {
        public static string Location = null;
    }

    public static class SystemXmlAssemblyLocation
    {
        public static string Location = null;
    }

    public static class TargetPlatform
    {
        public static bool DoNotLockFiles;
        public static bool GetDebugInfo;
        public static char GenericTypeNamesMangleChar = '_';

        public static bool UseGenerics
        {
            get
            {
                Version v = TargetPlatform.TargetVersion;
                if (v == null)
                {
                    v = CoreSystemTypes.SystemAssembly.Version;
                    if (v == null)
                        v = typeof(object).Assembly.GetName().Version;
                }
                return v.Major > 1 || v.Minor > 2 || v.Minor == 2 && v.Build >= 3300;
            }
        }

        public static void Clear()
        {
            SystemAssemblyLocation.Location = null;
            SystemDataAssemblyLocation.Location = null;
            SystemXmlAssemblyLocation.Location = null;
            TargetPlatform.DoNotLockFiles = false;
            TargetPlatform.GetDebugInfo = false;
            TargetPlatform.PlatformAssembliesLocation = "";
            SystemTypes.Clear();
        }
        public static System.Collections.IDictionary StaticAssemblyCache
        {
            get { return Reader.StaticAssemblyCache; }
        }

        public static Version TargetVersion = new Version(2, 0, 50727);  // Default for a Whidbey compiler
        public static string TargetRuntimeVersion;

        public static int LinkerMajorVersion
        {
            get
            {
                switch (TargetVersion.Major)
                {
                    case 2: return 8;
                    case 1: return 7;
                    default: return 6;
                }
            }
        }
        public static int LinkerMinorVersion
        {
            get
            {
                return TargetVersion.Minor;
            }
        }

        public static int MajorVersion { get { return TargetVersion.Major; } }
        public static int MinorVersion { get { return TargetVersion.Minor; } }
        public static int Build { get { return TargetVersion.Build; } }

        public static string/*!*/ PlatformAssembliesLocation = String.Empty;
        private static TrivialHashtable assemblyReferenceFor;
        public static TrivialHashtable/*!*/ AssemblyReferenceFor
        {
            get
            {
                if (TargetPlatform.assemblyReferenceFor == null)
                    TargetPlatform.SetupAssemblyReferenceFor();
                //^ assume TargetPlatform.assemblyReferenceFor != null;
                return TargetPlatform.assemblyReferenceFor;
            }
            set
            {
                TargetPlatform.assemblyReferenceFor = value;
            }
        }

        private readonly static string[]/*!*/ FxAssemblyNames =
          new string[]{"Accessibility", "CustomMarshalers", "IEExecRemote", "IEHost", "IIEHost", "ISymWrapper", 
                    "Microsoft.JScript", "Microsoft.VisualBasic", "Microsoft.VisualBasic.Vsa", "Microsoft.VisualC",
                    "Microsoft.Vsa", "Microsoft.Vsa.Vb.CodeDOMProcessor", "mscorcfg", "Regcode", "System",
                    "System.Configuration.Install", "System.Data", "System.Design", "System.DirectoryServices",
                    "System.Drawing", "System.Drawing.Design", "System.EnterpriseServices", 
                    "System.Management", "System.Messaging", "System.Runtime.Remoting", "System.Runtime.Serialization.Formatters.Soap",
                    "System.Security", "System.ServiceProcess", "System.Web", "System.Web.Mobile", "System.Web.RegularExpressions",
                    "System.Web.Services", "System.Windows.Forms", "System.Xml", "TlbExpCode", "TlbImpCode", "cscompmgd",
                    "vjswfchtml", "vjswfccw", "VJSWfcBrowserStubLib", "vjswfc", "vjslibcw", "vjslib", "vjscor", "VJSharpCodeProvider"};
        private readonly static string[]/*!*/ FxAssemblyToken =
          new string[]{"b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b77a5c561934e089",
                    "b03f5f7f11d50a3a", "b77a5c561934e089", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", 
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b77a5c561934e089", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b77a5c561934e089", "b77a5c561934e089", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a",
                    "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a", "b03f5f7f11d50a3a"};
        private readonly static string[]/*!*/ FxAssemblyVersion1 =
          new string[]{"1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0",
                    "7.0.3300.0", "7.0.3300.0", "7.0.3300.0", "7.0.3300.0",
                    "7.0.3300.0", "7.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0",
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0",
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0",
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", 
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0",
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "7.0.3300.0",
                    "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "1.0.3300.0", "7.0.3300.0"};
        private readonly static string[]/*!*/ FxAssemblyVersion1_1 =
          new string[]{"1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0",
                    "7.0.5000.0", "7.0.5000.0", "7.0.5000.0", "7.0.5000.0",
                    "7.0.5000.0", "7.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0",
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0",
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", 
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", 
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0",
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "7.0.5000.0",
                    "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "1.0.5000.0", "7.0.5000.0"};
        private static string[]/*!*/ FxAssemblyVersion2Build3600 =
          new string[]{"2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0",
                    "8.0.1200.0", "8.0.1200.0", "8.0.1200.0", "8.0.1200.0",
                    "8.0.1200.0", "8.0.1200.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0",
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0",
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", 
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", 
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0",
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "8.0.1200.0",
                    "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "2.0.3600.0", "7.0.5000.0"};
        private static string[]/*!*/ FxAssemblyVersion2 =
          new string[]{"2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0",
                    "8.0.0.0", "8.0.0.0", "8.0.0.0", "8.0.0.0",
                    "8.0.0.0", "8.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0",
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0",
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", 
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", 
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0",
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "8.0.0.0",
                    "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0", "2.0.0.0"};

        private static void SetupAssemblyReferenceFor()
        {
            Version version = TargetPlatform.TargetVersion;
            if (version == null) version = typeof(object).Module.Assembly.GetName().Version;
            TargetPlatform.SetTo(version);
        }

        public static void SetTo(Version/*!*/ version)
        {
            if(version == null)
                throw new ArgumentNullException("version");

            if(version.Major == 1)
            {
                if (version.Minor == 0 && version.Build == 3300) TargetPlatform.SetToV1();
                else if (version.Minor == 0 && version.Build == 5000) TargetPlatform.SetToV1_1();
                else if (version.Minor == 1 && version.Build == 9999) TargetPlatform.SetToPostV1_1(TargetPlatform.PlatformAssembliesLocation);
            }
            else if (version.Major == 2)
            {
                if (version.Minor == 0 && version.Build == 3600) TargetPlatform.SetToV2Beta1();
                else TargetPlatform.SetToV2();
            }
            else
                TargetPlatform.SetToV1();
        }
        public static void SetTo(Version/*!*/ version, string/*!*/ platformAssembliesLocation)
        {
            if(version == null)
                throw new ArgumentNullException("version");

            if(platformAssembliesLocation == null)
                throw new ArgumentNullException("platformAssembliesLocation");

            if(version.Major == 1)
            {
                if (version.Minor == 0 && version.Build == 3300) TargetPlatform.SetToV1(platformAssembliesLocation);
                else if (version.Minor == 0 && version.Build == 5000) TargetPlatform.SetToV1_1(platformAssembliesLocation);
                else if (version.Minor == 1 && version.Build == 9999) TargetPlatform.SetToPostV1_1(platformAssembliesLocation);
            }
            else if (version.Major == 2)
            {
                if (version.Minor == 0 && version.Build == 3600) TargetPlatform.SetToV2Beta1(platformAssembliesLocation);
                else TargetPlatform.SetToV2(platformAssembliesLocation);
            }
            else
                TargetPlatform.SetToV1(platformAssembliesLocation);
        }
        public static void SetToV1()
        {
            TargetPlatform.SetToV1(TargetPlatform.PlatformAssembliesLocation);
        }
        public static void SetToV1(string platformAssembliesLocation)
        {
            TargetPlatform.TargetVersion = new Version(1, 0, 3300);
            TargetPlatform.TargetRuntimeVersion = "v1.0.3705";
            if (platformAssembliesLocation == null || platformAssembliesLocation.Length == 0)
                platformAssembliesLocation = TargetPlatform.PlatformAssembliesLocation = Path.Combine(Path.GetDirectoryName(typeof(object).Module.Assembly.Location), "..\\v1.0.3705");
            else
                TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.InitializeStandardAssemblyLocationsWithDefaultValues(platformAssembliesLocation);
            TrivialHashtable assemblyReferenceFor = new TrivialHashtable(46);
            for (int i = 0, n = TargetPlatform.FxAssemblyNames.Length; i < n; i++)
            {
                string name = TargetPlatform.FxAssemblyNames[i];
                string version = TargetPlatform.FxAssemblyVersion1[i];
                string token = TargetPlatform.FxAssemblyToken[i];
                AssemblyReference aref = new AssemblyReference(name + ", Version=" + version + ", Culture=neutral, PublicKeyToken=" + token);
                aref.Location = platformAssembliesLocation + "\\" + name + ".dll";
                //^ assume name != null;
                assemblyReferenceFor[Identifier.For(name).UniqueIdKey] = aref;
            }
            TargetPlatform.assemblyReferenceFor = assemblyReferenceFor;
        }
        public static void SetToV1_1()
        {
            TargetPlatform.SetToV1_1(TargetPlatform.PlatformAssembliesLocation);
        }
        public static void SetToV1_1(string/*!*/ platformAssembliesLocation)
        {
            TargetPlatform.TargetVersion = new Version(1, 0, 5000);
            TargetPlatform.TargetRuntimeVersion = "v1.1.4322";
            if (platformAssembliesLocation == null || platformAssembliesLocation.Length == 0)
                platformAssembliesLocation = TargetPlatform.PlatformAssembliesLocation = Path.Combine(Path.GetDirectoryName(typeof(object).Module.Assembly.Location), "..\\v1.1.4322");
            else
                TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.InitializeStandardAssemblyLocationsWithDefaultValues(platformAssembliesLocation);
            TrivialHashtable assemblyReferenceFor = new TrivialHashtable(46);
            for (int i = 0, n = TargetPlatform.FxAssemblyNames.Length; i < n; i++)
            {
                string name = TargetPlatform.FxAssemblyNames[i];
                string version = TargetPlatform.FxAssemblyVersion1_1[i];
                string token = TargetPlatform.FxAssemblyToken[i];
                AssemblyReference aref = new AssemblyReference(name + ", Version=" + version + ", Culture=neutral, PublicKeyToken=" + token);
                aref.Location = platformAssembliesLocation + "\\" + name + ".dll";
                //^ assume name != null;
                assemblyReferenceFor[Identifier.For(name).UniqueIdKey] = aref;
            }
            TargetPlatform.assemblyReferenceFor = assemblyReferenceFor;
        }
        public static void SetToV2()
        {
            TargetPlatform.SetToV2(TargetPlatform.PlatformAssembliesLocation);
        }
        public static void SetToV2(string platformAssembliesLocation)
        {
            TargetPlatform.TargetVersion = new Version(2, 0, 50727);
            TargetPlatform.TargetRuntimeVersion = "v2.0.50727";
            TargetPlatform.GenericTypeNamesMangleChar = '`';
            if (platformAssembliesLocation == null || platformAssembliesLocation.Length == 0)
                platformAssembliesLocation = TargetPlatform.PlatformAssembliesLocation = Path.Combine(Path.GetDirectoryName(typeof(object).Module.Assembly.Location), "..\\v2.0.50727");
            else
                TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.InitializeStandardAssemblyLocationsWithDefaultValues(platformAssembliesLocation);
            TrivialHashtable assemblyReferenceFor = new TrivialHashtable(46);
            for (int i = 0, n = TargetPlatform.FxAssemblyNames.Length; i < n; i++)
            {
                string name = TargetPlatform.FxAssemblyNames[i];
                string version = TargetPlatform.FxAssemblyVersion2[i];
                string token = TargetPlatform.FxAssemblyToken[i];
                AssemblyReference aref = new AssemblyReference(name + ", Version=" + version + ", Culture=neutral, PublicKeyToken=" + token);
                aref.Location = platformAssembliesLocation + "\\" + name + ".dll";
                //^ assume name != null;
                assemblyReferenceFor[Identifier.For(name).UniqueIdKey] = aref;
            }
            TargetPlatform.assemblyReferenceFor = assemblyReferenceFor;
        }
        public static void SetToV2Beta1()
        {
            TargetPlatform.SetToV2Beta1(TargetPlatform.PlatformAssembliesLocation);
        }
        public static void SetToV2Beta1(string/*!*/ platformAssembliesLocation)
        {
            TargetPlatform.TargetVersion = new Version(2, 0, 3600);
            TargetPlatform.GenericTypeNamesMangleChar = '!';
            string dotNetDirLocation = null;
            if (platformAssembliesLocation == null || platformAssembliesLocation.Length == 0)
            {
                DirectoryInfo dotNetDir = new FileInfo(new Uri(typeof(object).Module.Assembly.Location).LocalPath).Directory.Parent;
                dotNetDirLocation = dotNetDir.FullName;
                if (dotNetDirLocation != null) dotNetDirLocation = dotNetDirLocation.ToUpper(System.Globalization.CultureInfo.InvariantCulture);
                DateTime creationTime = DateTime.MinValue;
                foreach (DirectoryInfo subdir in dotNetDir.GetDirectories("v2.0*"))
                {
                    if (subdir == null) continue;
                    if (subdir.CreationTime < creationTime) continue;
                    FileInfo[] mscorlibs = subdir.GetFiles("mscorlib.dll");
                    if (mscorlibs != null && mscorlibs.Length == 1)
                    {
                        platformAssembliesLocation = subdir.FullName;
                        creationTime = subdir.CreationTime;
                    }
                }
            }
            else
                TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            if (dotNetDirLocation != null && (platformAssembliesLocation == null || platformAssembliesLocation.Length == 0))
            {
                int pos = dotNetDirLocation.IndexOf("FRAMEWORK");
                if (pos > 0 && dotNetDirLocation.IndexOf("FRAMEWORK64") < 0)
                {
                    dotNetDirLocation = dotNetDirLocation.Replace("FRAMEWORK", "FRAMEWORK64");
                    if (Directory.Exists(dotNetDirLocation))
                    {
                        DirectoryInfo dotNetDir = new DirectoryInfo(dotNetDirLocation);
                        DateTime creationTime = DateTime.MinValue;
                        foreach (DirectoryInfo subdir in dotNetDir.GetDirectories("v2.0*"))
                        {
                            if (subdir == null) continue;
                            if (subdir.CreationTime < creationTime) continue;
                            FileInfo[] mscorlibs = subdir.GetFiles("mscorlib.dll");
                            if (mscorlibs != null && mscorlibs.Length == 1)
                            {
                                platformAssembliesLocation = subdir.FullName;
                                creationTime = subdir.CreationTime;
                            }
                        }
                    }
                }
            }
            TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.InitializeStandardAssemblyLocationsWithDefaultValues(platformAssembliesLocation);
            TrivialHashtable assemblyReferenceFor = new TrivialHashtable(46);
            for (int i = 0, n = TargetPlatform.FxAssemblyNames.Length; i < n; i++)
            {
                string name = TargetPlatform.FxAssemblyNames[i];
                string version = TargetPlatform.FxAssemblyVersion2Build3600[i];
                string token = TargetPlatform.FxAssemblyToken[i];
                AssemblyReference aref = new AssemblyReference(name + ", Version=" + version + ", Culture=neutral, PublicKeyToken=" + token);
                aref.Location = platformAssembliesLocation + "\\" + name + ".dll";
                //^ assume name != null;
                assemblyReferenceFor[Identifier.For(name).UniqueIdKey] = aref;
            }
            TargetPlatform.assemblyReferenceFor = assemblyReferenceFor;
        }

        /// <summary>
        /// Use this to set the target platform to a platform with a superset of the platform assemblies in version 1.1, but
        /// where the public key tokens and versions numbers are determined by reading in the actual assemblies from
        /// the supplied location. Only assemblies recognized as platform assemblies in version 1.1 will be unified.
        /// </summary>
        public static void SetToPostV1_1(string/*!*/ platformAssembliesLocation)
        {
            TargetPlatform.PlatformAssembliesLocation = platformAssembliesLocation;
            TargetPlatform.TargetVersion = new Version(1, 1, 9999);
            TargetPlatform.TargetRuntimeVersion = "v1.1.9999";
            TargetPlatform.InitializeStandardAssemblyLocationsWithDefaultValues(platformAssembliesLocation);
            TargetPlatform.assemblyReferenceFor = new TrivialHashtable(46);
            string[] dlls = Directory.GetFiles(platformAssembliesLocation, "*.dll");
            foreach (string dll in dlls)
            {
                if (dll == null) continue;
                string assemName = Path.GetFileNameWithoutExtension(dll);
                int i = Array.IndexOf(TargetPlatform.FxAssemblyNames, assemName);
                if (i < 0) continue;
                AssemblyNode assem = AssemblyNode.GetAssembly(Path.Combine(platformAssembliesLocation, dll));
                if (assem == null) continue;
                TargetPlatform.assemblyReferenceFor[Identifier.For(assem.Name).UniqueIdKey] = new AssemblyReference(assem);
            }
        }
        private static void InitializeStandardAssemblyLocationsWithDefaultValues(string platformAssembliesLocation)
        {
            SystemAssemblyLocation.Location = platformAssembliesLocation + "\\mscorlib.dll";

            if (SystemDataAssemblyLocation.Location == null)
                SystemDataAssemblyLocation.Location = platformAssembliesLocation + "\\system.data.dll";

            if (SystemXmlAssemblyLocation.Location == null)
                SystemXmlAssemblyLocation.Location = platformAssembliesLocation + "\\system.xml.dll";
        }

        //!EFW
        /// <summary>
        /// Load target framework settings and assembly details
        /// </summary>
        /// <param name="platformType">The platform type</param>
        /// <param name="version">The framework version</param>
        public static void SetFrameworkInformation(string platformType, string version)
        {
            var fd = Microsoft.Ddue.Tools.Frameworks.FrameworkDictionary.LoadSandcastleFrameworkDictionary();

            var fs = fd.FrameworkMatching(platformType, new Version(version), true);

            if(fs == null)
                throw new InvalidOperationException(String.Format("Unable to locate information for the " +
                    "framework version '{0} {1}' or a suitable redirected version on this system",
                    platformType, version));

            var coreLocation = fs.AssemblyLocations.First(l => l.IsCoreLocation);

            if(coreLocation == null)
                throw new InvalidOperationException(String.Format("A core framework location has not been " +
                    "defined for the framework '{0} {1}'", platformType, version));

            TargetPlatform.TargetVersion = fs.Version;
            TargetPlatform.TargetRuntimeVersion = "v" + fs.Version.ToString();
            TargetPlatform.GenericTypeNamesMangleChar = '`';
            TargetPlatform.PlatformAssembliesLocation = coreLocation.Path;

            // Set references to the common core framework assemblies
            var ad = fs.FindAssembly("mscorlib");

            if(ad != null)
                SystemAssemblyLocation.Location = ad.Filename;

            ad = fs.FindAssembly("System.Data");

            if(ad != null)
                SystemDataAssemblyLocation.Location = ad.Filename;

            ad = fs.FindAssembly("System.Xml");

            if(ad != null)
                SystemXmlAssemblyLocation.Location = ad.Filename;

            // Load references to all the other framework assemblies
            var allAssemblies = fs.AllAssemblies.ToList();

            TrivialHashtable assemblyReferenceFor = new TrivialHashtable(allAssemblies.Count);

            // Loading mscorlib causes a reset of the reference cache and other info so we must ignore it.
            foreach(var asm in allAssemblies)
                if(!asm.Name.Equals("mscorlib", StringComparison.OrdinalIgnoreCase) && File.Exists(asm.Filename))
                {
                    AssemblyReference aref = new AssemblyReference(asm.ToString());
                    aref.Location = asm.Filename;
                    assemblyReferenceFor[Identifier.For(asm.Name).UniqueIdKey] = aref;
                }

            TargetPlatform.assemblyReferenceFor = assemblyReferenceFor;
        }

    }

    public static class CoreSystemTypes
    {
        internal static bool Initialized;

        internal static bool IsInitialized { get { return Initialized; } }

        //system assembly (the basic runtime)
        public static AssemblyNode/*!*/ SystemAssembly;

        //Special base types
        public static Class/*!*/ Object;
        public static Class/*!*/ String;
        public static Class/*!*/ ValueType;
        public static Class/*!*/ Enum;
        public static Class/*!*/ MulticastDelegate;
        public static Class/*!*/ Array;
        public static Class/*!*/ Type;
        public static Class/*!*/ Delegate;
        public static Class/*!*/ Exception;
        public static Class/*!*/ Attribute;

        //primitive types
        public static Struct/*!*/ Boolean;
        public static Struct/*!*/ Char;
        public static Struct/*!*/ Int8;
        public static Struct/*!*/ UInt8;
        public static Struct/*!*/ Int16;
        public static Struct/*!*/ UInt16;
        public static Struct/*!*/ Int32;
        public static Struct/*!*/ UInt32;
        public static Struct/*!*/ Int64;
        public static Struct/*!*/ UInt64;
        public static Struct/*!*/ Single;
        public static Struct/*!*/ Double;
        public static Struct/*!*/ IntPtr;
        public static Struct/*!*/ UIntPtr;
        public static Struct/*!*/ DynamicallyTypedReference;

        //Classes need for System.TypeCode
        public static Class/*!*/ DBNull;
        public static Struct/*!*/ DateTime;
        public static Struct/*!*/ Decimal;

        //Special types
        public static Class/*!*/ IsVolatile;
        public static Struct/*!*/ Void;
        public static Struct/*!*/ ArgIterator;
        public static Struct/*!*/ RuntimeFieldHandle;
        public static Struct/*!*/ RuntimeMethodHandle;
        public static Struct/*!*/ RuntimeTypeHandle;
        public static Struct/*!*/ RuntimeArgumentHandle;

        //Special attributes    
        public static EnumNode SecurityAction;

        static CoreSystemTypes()
        {
            CoreSystemTypes.Initialize(TargetPlatform.DoNotLockFiles, TargetPlatform.GetDebugInfo);
        }

        public static void Clear()
        {
            lock (Module.GlobalLock)
            {
                if (Reader.StaticAssemblyCache != null)
                {
                    foreach (AssemblyNode cachedAssembly in new System.Collections.ArrayList(Reader.StaticAssemblyCache.Values))
                    {
                        if (cachedAssembly != null) cachedAssembly.Dispose();
                    }
                    Reader.StaticAssemblyCache.Clear();
                }
                //Dispose the system assemblies in case they were not in the static cache. It is safe to dispose an assembly more than once.
                if (CoreSystemTypes.SystemAssembly != null && CoreSystemTypes.SystemAssembly != AssemblyNode.Dummy)
                {
                    CoreSystemTypes.SystemAssembly.Dispose();
                    CoreSystemTypes.SystemAssembly = null;
                }
                CoreSystemTypes.ClearStatics();
                CoreSystemTypes.Initialized = false;
                TargetPlatform.AssemblyReferenceFor = new TrivialHashtable(0);
            }
        }
        public static void Initialize(bool doNotLockFile, bool getDebugInfo)
        {
            if (CoreSystemTypes.Initialized) CoreSystemTypes.Clear();
            if (SystemAssembly == null)
                SystemAssembly = CoreSystemTypes.GetSystemAssembly(doNotLockFile, getDebugInfo);
            if (SystemAssembly == null) throw new InvalidOperationException(ExceptionStrings.InternalCompilerError);
            if (TargetPlatform.TargetVersion == null)
            {
                TargetPlatform.TargetVersion = SystemAssembly.Version;
                if (TargetPlatform.TargetVersion == null)
                    TargetPlatform.TargetVersion = typeof(object).Module.Assembly.GetName().Version;
            }
            if (TargetPlatform.TargetVersion != null)
            {
                if (TargetPlatform.TargetVersion.Major > 1 || TargetPlatform.TargetVersion.Minor > 1 ||
                  (TargetPlatform.TargetVersion.Minor == 1 && TargetPlatform.TargetVersion.Build == 9999))
                {
                    if (SystemAssembly.IsValidTypeName(StandardIds.System, Identifier.For("Nullable`1")))
                        TargetPlatform.GenericTypeNamesMangleChar = '`';
                    else if (SystemAssembly.IsValidTypeName(StandardIds.System, Identifier.For("Nullable!1")))
                        TargetPlatform.GenericTypeNamesMangleChar = '!';
                    else if (TargetPlatform.TargetVersion.Major == 1 && TargetPlatform.TargetVersion.Minor == 2)
                        TargetPlatform.GenericTypeNamesMangleChar = (char)0;
                }
            }
            // This must be done in the order: Object, ValueType, Char, String
            // or else some of the generic type instantiations don't get filled
            // in correctly. (String ends up implementing IEnumerable<string>
            // instead of IEnumerable<char>.)
            Object = (Class)GetTypeNodeFor("System", "Object", ElementType.Object);
            ValueType = (Class)GetTypeNodeFor("System", "ValueType", ElementType.Class);
            Char = (Struct)GetTypeNodeFor("System", "Char", ElementType.Char);
            String = (Class)GetTypeNodeFor("System", "String", ElementType.String);
            Enum = (Class)GetTypeNodeFor("System", "Enum", ElementType.Class);
            MulticastDelegate = (Class)GetTypeNodeFor("System", "MulticastDelegate", ElementType.Class);
            Array = (Class)GetTypeNodeFor("System", "Array", ElementType.Class);
            Type = (Class)GetTypeNodeFor("System", "Type", ElementType.Class);
            Boolean = (Struct)GetTypeNodeFor("System", "Boolean", ElementType.Boolean);
            Int8 = (Struct)GetTypeNodeFor("System", "SByte", ElementType.Int8);
            UInt8 = (Struct)GetTypeNodeFor("System", "Byte", ElementType.UInt8);
            Int16 = (Struct)GetTypeNodeFor("System", "Int16", ElementType.Int16);
            UInt16 = (Struct)GetTypeNodeFor("System", "UInt16", ElementType.UInt16);
            Int32 = (Struct)GetTypeNodeFor("System", "Int32", ElementType.Int32);
            UInt32 = (Struct)GetTypeNodeFor("System", "UInt32", ElementType.UInt32);
            Int64 = (Struct)GetTypeNodeFor("System", "Int64", ElementType.Int64);
            UInt64 = (Struct)GetTypeNodeFor("System", "UInt64", ElementType.UInt64);
            Single = (Struct)GetTypeNodeFor("System", "Single", ElementType.Single);
            Double = (Struct)GetTypeNodeFor("System", "Double", ElementType.Double);
            IntPtr = (Struct)GetTypeNodeFor("System", "IntPtr", ElementType.IntPtr);
            UIntPtr = (Struct)GetTypeNodeFor("System", "UIntPtr", ElementType.UIntPtr);
            DynamicallyTypedReference = (Struct)GetTypeNodeFor("System", "TypedReference", ElementType.DynamicallyTypedReference);
            Delegate = (Class)GetTypeNodeFor("System", "Delegate", ElementType.Class);
            Exception = (Class)GetTypeNodeFor("System", "Exception", ElementType.Class);
            Attribute = (Class)GetTypeNodeFor("System", "Attribute", ElementType.Class);
            DBNull = (Class)GetTypeNodeFor("System", "DBNull", ElementType.Class);
            DateTime = (Struct)GetTypeNodeFor("System", "DateTime", ElementType.ValueType);
            Decimal = (Struct)GetTypeNodeFor("System", "Decimal", ElementType.ValueType);
            ArgIterator = (Struct)GetTypeNodeFor("System", "ArgIterator", ElementType.ValueType);
            IsVolatile = (Class)GetTypeNodeFor("System.Runtime.CompilerServices", "IsVolatile", ElementType.Class);
            Void = (Struct)GetTypeNodeFor("System", "Void", ElementType.Void);
            RuntimeFieldHandle = (Struct)GetTypeNodeFor("System", "RuntimeFieldHandle", ElementType.ValueType);
            RuntimeMethodHandle = (Struct)GetTypeNodeFor("System", "RuntimeMethodHandle", ElementType.ValueType);
            RuntimeTypeHandle = (Struct)GetTypeNodeFor("System", "RuntimeTypeHandle", ElementType.ValueType);
            RuntimeArgumentHandle = (Struct)GetTypeNodeFor("System", "RuntimeArgumentHandle", ElementType.ValueType);
            SecurityAction = GetTypeNodeFor("System.Security.Permissions", "SecurityAction", ElementType.ValueType) as EnumNode;
            CoreSystemTypes.Initialized = true;
            CoreSystemTypes.InstantiateGenericInterfaces();

            object dummy = TargetPlatform.AssemblyReferenceFor; //Force selection of target platform

            if(dummy == null)
                return;
        }
        private static void ClearStatics()
        {
            //Special base types
            Object = null;
            String = null;
            ValueType = null;
            Enum = null;
            MulticastDelegate = null;
            Array = null;
            Type = null;
            Delegate = null;
            Exception = null;
            Attribute = null;

            //primitive types
            Boolean = null;
            Char = null;
            Int8 = null;
            UInt8 = null;
            Int16 = null;
            UInt16 = null;
            Int32 = null;
            UInt32 = null;
            Int64 = null;
            UInt64 = null;
            Single = null;
            Double = null;
            IntPtr = null;
            UIntPtr = null;
            DynamicallyTypedReference = null;

            //Special types
            DBNull = null;
            DateTime = null;
            Decimal = null;
            RuntimeArgumentHandle = null;
            ArgIterator = null;
            RuntimeFieldHandle = null;
            RuntimeMethodHandle = null;
            RuntimeTypeHandle = null;
            IsVolatile = null;
            Void = null;
            SecurityAction = null;
        }
        private static void InstantiateGenericInterfaces()
        {
            if (TargetPlatform.TargetVersion != null && (TargetPlatform.TargetVersion.Major < 2 && TargetPlatform.TargetVersion.Minor < 2)) return;
            InstantiateGenericInterfaces(String);
            InstantiateGenericInterfaces(Boolean);
            InstantiateGenericInterfaces(Char);
            InstantiateGenericInterfaces(Int8);
            InstantiateGenericInterfaces(UInt8);
            InstantiateGenericInterfaces(Int16);
            InstantiateGenericInterfaces(UInt16);
            InstantiateGenericInterfaces(Int32);
            InstantiateGenericInterfaces(UInt32);
            InstantiateGenericInterfaces(Int64);
            InstantiateGenericInterfaces(UInt64);
            InstantiateGenericInterfaces(Single);
            InstantiateGenericInterfaces(Double);
            InstantiateGenericInterfaces(DBNull);
            InstantiateGenericInterfaces(DateTime);
            InstantiateGenericInterfaces(Decimal);
        }
        private static void InstantiateGenericInterfaces(TypeNode type)
        {
            if (type == null) return;
            InterfaceList interfaces = type.Interfaces;
            for (int i = 0, n = interfaces == null ? 0 : interfaces.Count; i < n; i++)
            {
                InterfaceExpression ifaceExpr = interfaces[i] as InterfaceExpression;
                if (ifaceExpr == null) continue;
                if (ifaceExpr.Template == null) { Debug.Assert(false); continue; }
                TypeNodeList templArgs = ifaceExpr.TemplateArguments;
                for (int j = 0, m = templArgs.Count; j < m; j++)
                {
                    InterfaceExpression ie = templArgs[j] as InterfaceExpression;
                    if (ie != null)
                        templArgs[j] = ie.Template.GetGenericTemplateInstance(type.DeclaringModule, ie.ConsolidatedTemplateArguments);
                }
                interfaces[i] = (Interface)ifaceExpr.Template.GetGenericTemplateInstance(type.DeclaringModule, ifaceExpr.ConsolidatedTemplateArguments);
            }
        }

        private static AssemblyNode/*!*/ GetSystemAssembly(bool doNotLockFile, bool getDebugInfo)
        {
            AssemblyNode result = SystemAssemblyLocation.ParsedAssembly;
            if (result != null)
            {
                result.TargetRuntimeVersion = TargetPlatform.TargetRuntimeVersion;
                result.MetadataFormatMajorVersion = 1;
                result.MetadataFormatMinorVersion = 1;
                result.LinkerMajorVersion = 8;
                result.LinkerMinorVersion = 0;
                return result;
            }
            if (SystemAssemblyLocation.Location == null || SystemAssemblyLocation.Location.Length == 0)
                SystemAssemblyLocation.Location = typeof(object).Module.Assembly.Location;
            result = (AssemblyNode)(new Reader(SystemAssemblyLocation.Location, null, doNotLockFile, getDebugInfo, true, false)).ReadModule();
            if (result == null && TargetPlatform.TargetVersion != null && TargetPlatform.TargetVersion == typeof(object).Module.Assembly.GetName().Version)
            {
                SystemAssemblyLocation.Location = typeof(object).Module.Assembly.Location;
                result = (AssemblyNode)(new Reader(SystemAssemblyLocation.Location, null, doNotLockFile, getDebugInfo, true, false)).ReadModule();
            }
            if (result == null)
            {
                result = new AssemblyNode();
                System.Reflection.AssemblyName aname = typeof(object).Module.Assembly.GetName();
                result.Name = aname.Name;
                result.Version = TargetPlatform.TargetVersion;
                result.PublicKeyOrToken = aname.GetPublicKeyToken();
            }
            return result;
        }
        private static TypeNode/*!*/ GetTypeNodeFor(string/*!*/ nspace, string/*!*/ name, ElementType typeCode)
        {
            TypeNode result = null;
            if (SystemAssembly == null)
                Debug.Assert(false);
            else
                result = SystemAssembly.GetType(Identifier.For(nspace), Identifier.For(name));
            if (result == null) result = CoreSystemTypes.GetDummyTypeNode(SystemAssembly, nspace, name, typeCode);
            result.typeCode = typeCode;
            return result;
        }

        internal static TypeNode/*!*/ GetDummyTypeNode(AssemblyNode declaringAssembly, string/*!*/ nspace, string/*!*/ name, ElementType typeCode)
        {
            TypeNode result = null;
            switch (typeCode)
            {
                case ElementType.Object:
                case ElementType.String:
                case ElementType.Class:
                    if (name.Length > 1 && name[0] == 'I' && char.IsUpper(name[1]))
                        result = new Interface();
                    else if (name == "MulticastDelegate" || name == "Delegate")
                        result = new Class();
                    else if (name.EndsWith("Callback") || name.EndsWith("Delegate") || name == "ThreadStart" || name == "FrameGuardGetter" || name == "GuardThreadStart")
                        result = new DelegateNode();
                    else
                        result = new Class();
                    break;
                default:
                    if (name == "CciMemberKind")
                        result = new EnumNode();
                    else
                        result = new Struct();
                    break;
            }
            result.Name = Identifier.For(name);
            result.Namespace = Identifier.For(nspace);
            result.DeclaringModule = declaringAssembly;
            return result;
        }
    }

    public static class SystemTypes
    {
        private static bool Initialized;

        //system assembly (the basic runtime)
        public static AssemblyNode/*!*/ SystemAssembly
        {
            get { return CoreSystemTypes.SystemAssembly; }
            set { CoreSystemTypes.SystemAssembly = value; }
        }

        //Special base types
        public static Class/*!*/ Object { get { return CoreSystemTypes.Object; } }
        public static Class/*!*/ String { get { return CoreSystemTypes.String; } }
        public static Class/*!*/ ValueType { get { return CoreSystemTypes.ValueType; } }
        public static Class/*!*/ Enum { get { return CoreSystemTypes.Enum; } }
        public static Class/*!*/ Delegate { get { return CoreSystemTypes.Delegate; } }
        public static Class/*!*/ MulticastDelegate { get { return CoreSystemTypes.MulticastDelegate; } }
        public static Class/*!*/ Array { get { return CoreSystemTypes.Array; } }
        public static Class/*!*/ Type { get { return CoreSystemTypes.Type; } }
        public static Class/*!*/ Exception { get { return CoreSystemTypes.Exception; } }
        public static Class/*!*/ Attribute { get { return CoreSystemTypes.Attribute; } }

        //primitive types
        public static Struct/*!*/ Boolean { get { return CoreSystemTypes.Boolean; } }
        public static Struct/*!*/ Char { get { return CoreSystemTypes.Char; } }
        public static Struct/*!*/ Int8 { get { return CoreSystemTypes.Int8; } }
        public static Struct/*!*/ UInt8 { get { return CoreSystemTypes.UInt8; } }
        public static Struct/*!*/ Int16 { get { return CoreSystemTypes.Int16; } }
        public static Struct/*!*/ UInt16 { get { return CoreSystemTypes.UInt16; } }
        public static Struct/*!*/ Int32 { get { return CoreSystemTypes.Int32; } }
        public static Struct/*!*/ UInt32 { get { return CoreSystemTypes.UInt32; } }
        public static Struct/*!*/ Int64 { get { return CoreSystemTypes.Int64; } }
        public static Struct/*!*/ UInt64 { get { return CoreSystemTypes.UInt64; } }
        public static Struct/*!*/ Single { get { return CoreSystemTypes.Single; } }
        public static Struct/*!*/ Double { get { return CoreSystemTypes.Double; } }
        public static Struct/*!*/ IntPtr { get { return CoreSystemTypes.IntPtr; } }
        public static Struct/*!*/ UIntPtr { get { return CoreSystemTypes.UIntPtr; } }
        public static Struct/*!*/ DynamicallyTypedReference { get { return CoreSystemTypes.DynamicallyTypedReference; } }

        // Types required for a complete rendering
        // of binary attribute information
        public static Class/*!*/ AttributeUsageAttribute;
        public static Class/*!*/ ConditionalAttribute;
        public static Class/*!*/ DefaultMemberAttribute;
        public static Class/*!*/ InternalsVisibleToAttribute;
        public static Class/*!*/ ObsoleteAttribute;

        // Types required to render arrays
        public static Interface/*!*/ GenericICollection;
        public static Interface/*!*/ GenericIEnumerable;
        public static Interface/*!*/ GenericIList;
        public static Interface/*!*/ ICloneable;
        public static Interface/*!*/ ICollection;
        public static Interface/*!*/ IEnumerable;
        public static Interface/*!*/ IList;

        //Special types
        public static Struct/*!*/ ArgIterator { get { return CoreSystemTypes.ArgIterator; } }
        public static Class/*!*/ IsVolatile { get { return CoreSystemTypes.IsVolatile; } }
        public static Struct/*!*/ Void { get { return CoreSystemTypes.Void; } }
        public static Struct/*!*/ RuntimeFieldHandle { get { return CoreSystemTypes.RuntimeTypeHandle; } }
        public static Struct/*!*/ RuntimeMethodHandle { get { return CoreSystemTypes.RuntimeTypeHandle; } }
        public static Struct/*!*/ RuntimeTypeHandle { get { return CoreSystemTypes.RuntimeTypeHandle; } }
        public static Struct/*!*/ RuntimeArgumentHandle { get { return CoreSystemTypes.RuntimeArgumentHandle; } }

        //Special attributes    
        public static Class/*!*/ AllowPartiallyTrustedCallersAttribute;
        public static Class/*!*/ AssemblyCompanyAttribute;
        public static Class/*!*/ AssemblyConfigurationAttribute;
        public static Class/*!*/ AssemblyCopyrightAttribute;
        public static Class/*!*/ AssemblyCultureAttribute;
        public static Class/*!*/ AssemblyDelaySignAttribute;
        public static Class/*!*/ AssemblyDescriptionAttribute;
        public static Class/*!*/ AssemblyFileVersionAttribute;
        public static Class/*!*/ AssemblyFlagsAttribute;
        public static Class/*!*/ AssemblyInformationalVersionAttribute;
        public static Class/*!*/ AssemblyKeyFileAttribute;
        public static Class/*!*/ AssemblyKeyNameAttribute;
        public static Class/*!*/ AssemblyProductAttribute;
        public static Class/*!*/ AssemblyTitleAttribute;
        public static Class/*!*/ AssemblyTrademarkAttribute;
        public static Class/*!*/ AssemblyVersionAttribute;
        public static Class/*!*/ ClassInterfaceAttribute;
        public static Class/*!*/ CLSCompliantAttribute;
        public static Class/*!*/ ComImportAttribute;
        public static Class/*!*/ ComRegisterFunctionAttribute;
        public static Class/*!*/ ComSourceInterfacesAttribute;
        public static Class/*!*/ ComUnregisterFunctionAttribute;
        public static Class/*!*/ ComVisibleAttribute;
        public static Class/*!*/ DebuggableAttribute;
        public static Class/*!*/ DebuggerHiddenAttribute;
        public static Class/*!*/ DebuggerStepThroughAttribute;
        public static EnumNode DebuggingModes;
        public static Class/*!*/ DllImportAttribute;
        public static Class/*!*/ FieldOffsetAttribute;
        public static Class/*!*/ FlagsAttribute;
        public static Class/*!*/ GuidAttribute;
        public static Class/*!*/ ImportedFromTypeLibAttribute;
        public static Class/*!*/ InAttribute;
        public static Class/*!*/ IndexerNameAttribute;
        public static Class/*!*/ InterfaceTypeAttribute;
        public static Class/*!*/ MethodImplAttribute;
        public static Class/*!*/ NonSerializedAttribute;
        public static Class/*!*/ OptionalAttribute;
        public static Class/*!*/ OutAttribute;
        public static Class/*!*/ ParamArrayAttribute;
        public static Class/*!*/ RuntimeCompatibilityAttribute;
        public static Class/*!*/ SatelliteContractVersionAttribute;
        public static Class/*!*/ SerializableAttribute;
        public static Class/*!*/ SecurityAttribute;
        public static Class/*!*/ SecurityCriticalAttribute;
        public static Class/*!*/ SecurityTransparentAttribute;
        public static Class/*!*/ SecurityTreatAsSafeAttribute;
        public static Class/*!*/ STAThreadAttribute;
        public static Class/*!*/ StructLayoutAttribute;
        public static Class/*!*/ SuppressMessageAttribute;
        public static Class/*!*/ SuppressUnmanagedCodeSecurityAttribute;
        public static EnumNode SecurityAction;

        //Classes need for System.TypeCode
        public static Class/*!*/ DBNull;
        public static Struct/*!*/ DateTime;
        public static Struct/*!*/ Decimal { get { return CoreSystemTypes.Decimal; } }
        public static Struct/*!*/ TimeSpan;

        //Classes and interfaces used by the Framework
        public static Class/*!*/ Activator;
        public static Class/*!*/ AppDomain;
        public static Class/*!*/ ApplicationException;
        public static Class/*!*/ ArgumentException;
        public static Class/*!*/ ArgumentNullException;
        public static Class/*!*/ ArgumentOutOfRangeException;
        public static Class/*!*/ ArrayList;
        public static DelegateNode/*!*/ AsyncCallback;
        public static Class/*!*/ Assembly;
        public static Class/*!*/ CodeAccessPermission;
        public static Class/*!*/ CollectionBase;
        public static Class/*!*/ CultureInfo;
        public static Class/*!*/ DictionaryBase;
        public static Struct/*!*/ DictionaryEntry;
        public static Class/*!*/ DuplicateWaitObjectException;
        public static Class/*!*/ Environment;
        public static Class/*!*/ EventArgs;
        public static Class/*!*/ ExecutionEngineException;
        public static Struct/*!*/ GenericArraySegment;
        public static Class/*!*/ GenericArrayToIEnumerableAdapter;
        public static Class/*!*/ GenericDictionary;
        public static Interface/*!*/ GenericIComparable;
        public static Interface/*!*/ GenericIComparer;
        public static Interface/*!*/ GenericIDictionary;
        public static Interface/*!*/ GenericIEnumerator;
        public static Struct/*!*/ GenericKeyValuePair;
        public static Class/*!*/ GenericList;
        public static Struct/*!*/ GenericNullable;
        public static Class/*!*/ GenericQueue;
        public static Class/*!*/ GenericSortedDictionary;
        public static Class/*!*/ GenericStack;
        public static Class/*!*/ GC;
        public static Struct/*!*/ Guid;
        public static Class/*!*/ __HandleProtector;
        public static Struct/*!*/ HandleRef;
        public static Class/*!*/ Hashtable;
        public static Interface/*!*/ IASyncResult;
        public static Interface/*!*/ IComparable;
        public static Interface/*!*/ IDictionary;
        public static Interface/*!*/ IComparer;
        public static Interface/*!*/ IDisposable;
        public static Interface/*!*/ IEnumerator;
        public static Interface/*!*/ IFormatProvider;
        public static Interface/*!*/ IHashCodeProvider;
        public static Interface/*!*/ IMembershipCondition;
        public static Class/*!*/ IndexOutOfRangeException;
        public static Class/*!*/ InvalidCastException;
        public static Class/*!*/ InvalidOperationException;
        public static Interface/*!*/ IPermission;
        public static Interface/*!*/ ISerializable;
        public static Interface/*!*/ IStackWalk;
        public static Class/*!*/ Marshal;
        public static Class/*!*/ MarshalByRefObject;
        public static Class/*!*/ MemberInfo;
        public static Struct/*!*/ NativeOverlapped;
        public static Class/*!*/ Monitor;
        public static Class/*!*/ NotSupportedException;
        public static Class/*!*/ NullReferenceException;
        public static Class/*!*/ OutOfMemoryException;
        public static Class/*!*/ ParameterInfo;
        public static Class/*!*/ Queue;
        public static Class/*!*/ ReadOnlyCollectionBase;
        public static Class/*!*/ ResourceManager;
        public static Class/*!*/ ResourceSet;
        public static Class/*!*/ SerializationInfo;
        public static Class/*!*/ Stack;
        public static Class/*!*/ StackOverflowException;
        public static Class/*!*/ Stream;
        public static Struct/*!*/ StreamingContext;
        public static Class/*!*/ StringBuilder;
        public static Class/*!*/ StringComparer;
        public static EnumNode StringComparison;
        public static Class/*!*/ SystemException;
        public static Class/*!*/ Thread;
        public static Class/*!*/ WindowsImpersonationContext;

        static SystemTypes()
        {
            SystemTypes.Initialize(TargetPlatform.DoNotLockFiles, TargetPlatform.GetDebugInfo);
        }

        public static void Clear()
        {
            lock (Module.GlobalLock)
            {
                CoreSystemTypes.Clear();
                SystemTypes.ClearStatics();
                SystemTypes.Initialized = false;
            }
        }
        public static void Initialize(bool doNotLockFile, bool getDebugInfo)
        {
            if (SystemTypes.Initialized)
            {
                SystemTypes.Clear();
                CoreSystemTypes.Initialize(doNotLockFile, getDebugInfo);
            }
            else if (!CoreSystemTypes.Initialized)
            {
                CoreSystemTypes.Initialize(doNotLockFile, getDebugInfo);
            }

            if (TargetPlatform.TargetVersion == null)
            {
                TargetPlatform.TargetVersion = SystemAssembly.Version;
                if (TargetPlatform.TargetVersion == null)
                    TargetPlatform.TargetVersion = typeof(object).Module.Assembly.GetName().Version;
            }
            //TODO: throw an exception when the result is null

            AttributeUsageAttribute = (Class)GetTypeNodeFor("System", "AttributeUsageAttribute", ElementType.Class);
            ConditionalAttribute = (Class)GetTypeNodeFor("System.Diagnostics", "ConditionalAttribute", ElementType.Class);
            DefaultMemberAttribute = (Class)GetTypeNodeFor("System.Reflection", "DefaultMemberAttribute", ElementType.Class);
            InternalsVisibleToAttribute = (Class)GetTypeNodeFor("System.Runtime.CompilerServices", "InternalsVisibleToAttribute", ElementType.Class);
            ObsoleteAttribute = (Class)GetTypeNodeFor("System", "ObsoleteAttribute", ElementType.Class);

            GenericICollection = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "ICollection", 1, ElementType.Class);
            GenericIEnumerable = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IEnumerable", 1, ElementType.Class);
            GenericIList = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IList", 1, ElementType.Class);
            ICloneable = (Interface)GetTypeNodeFor("System", "ICloneable", ElementType.Class);
            ICollection = (Interface)GetTypeNodeFor("System.Collections", "ICollection", ElementType.Class);
            IEnumerable = (Interface)GetTypeNodeFor("System.Collections", "IEnumerable", ElementType.Class);
            IList = (Interface)GetTypeNodeFor("System.Collections", "IList", ElementType.Class);

            AllowPartiallyTrustedCallersAttribute = (Class)GetTypeNodeFor("System.Security", "AllowPartiallyTrustedCallersAttribute", ElementType.Class);
            AssemblyCompanyAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyCompanyAttribute", ElementType.Class);
            AssemblyConfigurationAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyConfigurationAttribute", ElementType.Class);
            AssemblyCopyrightAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyCopyrightAttribute", ElementType.Class);
            AssemblyCultureAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyCultureAttribute", ElementType.Class);
            AssemblyDelaySignAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyDelaySignAttribute", ElementType.Class);
            AssemblyDescriptionAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyDescriptionAttribute", ElementType.Class);
            AssemblyFileVersionAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyFileVersionAttribute", ElementType.Class);
            AssemblyFlagsAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyFlagsAttribute", ElementType.Class);
            AssemblyInformationalVersionAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyInformationalVersionAttribute", ElementType.Class);
            AssemblyKeyFileAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyKeyFileAttribute", ElementType.Class);
            AssemblyKeyNameAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyKeyNameAttribute", ElementType.Class);
            AssemblyProductAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyProductAttribute", ElementType.Class);
            AssemblyTitleAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyTitleAttribute", ElementType.Class);
            AssemblyTrademarkAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyTrademarkAttribute", ElementType.Class);
            AssemblyVersionAttribute = (Class)GetTypeNodeFor("System.Reflection", "AssemblyVersionAttribute", ElementType.Class);
            ClassInterfaceAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ClassInterfaceAttribute", ElementType.Class);
            CLSCompliantAttribute = (Class)GetTypeNodeFor("System", "CLSCompliantAttribute", ElementType.Class);
            ComImportAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ComImportAttribute", ElementType.Class);
            ComRegisterFunctionAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ComRegisterFunctionAttribute", ElementType.Class);
            ComSourceInterfacesAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ComSourceInterfacesAttribute", ElementType.Class);
            ComUnregisterFunctionAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ComUnregisterFunctionAttribute", ElementType.Class);
            ComVisibleAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ComVisibleAttribute", ElementType.Class);
            DebuggableAttribute = (Class)GetTypeNodeFor("System.Diagnostics", "DebuggableAttribute", ElementType.Class);
            DebuggerHiddenAttribute = (Class)GetTypeNodeFor("System.Diagnostics", "DebuggerHiddenAttribute", ElementType.Class);
            DebuggerStepThroughAttribute = (Class)GetTypeNodeFor("System.Diagnostics", "DebuggerStepThroughAttribute", ElementType.Class);
            DebuggingModes = DebuggableAttribute == null ? null : DebuggableAttribute.GetNestedType(Identifier.For("DebuggingModes")) as EnumNode;
            DllImportAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "DllImportAttribute", ElementType.Class);
            FieldOffsetAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "FieldOffsetAttribute", ElementType.Class);
            FlagsAttribute = (Class)GetTypeNodeFor("System", "FlagsAttribute", ElementType.Class);
            Guid = (Struct)GetTypeNodeFor("System", "Guid", ElementType.ValueType);
            GuidAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "GuidAttribute", ElementType.Class);
            ImportedFromTypeLibAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "ImportedFromTypeLibAttribute", ElementType.Class);
            InAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "InAttribute", ElementType.Class);
            IndexerNameAttribute = (Class)GetTypeNodeFor("System.Runtime.CompilerServices", "IndexerNameAttribute", ElementType.Class);
            InterfaceTypeAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "InterfaceTypeAttribute", ElementType.Class);
            MethodImplAttribute = (Class)GetTypeNodeFor("System.Runtime.CompilerServices", "MethodImplAttribute", ElementType.Class);
            NonSerializedAttribute = (Class)GetTypeNodeFor("System", "NonSerializedAttribute", ElementType.Class);
            OptionalAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "OptionalAttribute", ElementType.Class);
            OutAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "OutAttribute", ElementType.Class);
            ParamArrayAttribute = (Class)GetTypeNodeFor("System", "ParamArrayAttribute", ElementType.Class);
            RuntimeCompatibilityAttribute = (Class)GetTypeNodeFor("System.Runtime.CompilerServices", "RuntimeCompatibilityAttribute", ElementType.Class);
            SatelliteContractVersionAttribute = (Class)GetTypeNodeFor("System.Resources", "SatelliteContractVersionAttribute", ElementType.Class);
            SerializableAttribute = (Class)GetTypeNodeFor("System", "SerializableAttribute", ElementType.Class);
            SecurityAttribute = (Class)GetTypeNodeFor("System.Security.Permissions", "SecurityAttribute", ElementType.Class);
            SecurityCriticalAttribute = (Class)GetTypeNodeFor("System.Security", "SecurityCriticalAttribute", ElementType.Class);
            SecurityTransparentAttribute = (Class)GetTypeNodeFor("System.Security", "SecurityTransparentAttribute", ElementType.Class);
            SecurityTreatAsSafeAttribute = (Class)GetTypeNodeFor("System.Security", "SecurityTreatAsSafeAttribute", ElementType.Class);
            STAThreadAttribute = (Class)GetTypeNodeFor("System", "STAThreadAttribute", ElementType.Class);
            StructLayoutAttribute = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "StructLayoutAttribute", ElementType.Class);
            SuppressMessageAttribute = (Class)GetTypeNodeFor("System.Diagnostics.CodeAnalysis", "SuppressMessageAttribute", ElementType.Class);
            SuppressUnmanagedCodeSecurityAttribute = (Class)GetTypeNodeFor("System.Security", "SuppressUnmanagedCodeSecurityAttribute", ElementType.Class);
            SecurityAction = GetTypeNodeFor("System.Security.Permissions", "SecurityAction", ElementType.ValueType) as EnumNode;
            DBNull = (Class)GetTypeNodeFor("System", "DBNull", ElementType.Class);
            DateTime = (Struct)GetTypeNodeFor("System", "DateTime", ElementType.ValueType);
            TimeSpan = (Struct)GetTypeNodeFor("System", "TimeSpan", ElementType.ValueType);
            Activator = (Class)GetTypeNodeFor("System", "Activator", ElementType.Class);
            AppDomain = (Class)GetTypeNodeFor("System", "AppDomain", ElementType.Class);
            ApplicationException = (Class)GetTypeNodeFor("System", "ApplicationException", ElementType.Class);
            ArgumentException = (Class)GetTypeNodeFor("System", "ArgumentException", ElementType.Class);
            ArgumentNullException = (Class)GetTypeNodeFor("System", "ArgumentNullException", ElementType.Class);
            ArgumentOutOfRangeException = (Class)GetTypeNodeFor("System", "ArgumentOutOfRangeException", ElementType.Class);
            ArrayList = (Class)GetTypeNodeFor("System.Collections", "ArrayList", ElementType.Class);
            AsyncCallback = (DelegateNode)GetTypeNodeFor("System", "AsyncCallback", ElementType.Class);
            Assembly = (Class)GetTypeNodeFor("System.Reflection", "Assembly", ElementType.Class);
            CodeAccessPermission = (Class)GetTypeNodeFor("System.Security", "CodeAccessPermission", ElementType.Class);
            CollectionBase = (Class)GetTypeNodeFor("System.Collections", "CollectionBase", ElementType.Class);
            CultureInfo = (Class)GetTypeNodeFor("System.Globalization", "CultureInfo", ElementType.Class);
            DictionaryBase = (Class)GetTypeNodeFor("System.Collections", "DictionaryBase", ElementType.Class);
            DictionaryEntry = (Struct)GetTypeNodeFor("System.Collections", "DictionaryEntry", ElementType.ValueType);
            DuplicateWaitObjectException = (Class)GetTypeNodeFor("System", "DuplicateWaitObjectException", ElementType.Class);
            Environment = (Class)GetTypeNodeFor("System", "Environment", ElementType.Class);
            EventArgs = (Class)GetTypeNodeFor("System", "EventArgs", ElementType.Class);
            ExecutionEngineException = (Class)GetTypeNodeFor("System", "ExecutionEngineException", ElementType.Class);
            GenericArraySegment = (Struct)GetGenericRuntimeTypeNodeFor("System", "ArraySegment", 1, ElementType.ValueType);
            GenericDictionary = (Class)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "Dictionary", 2, ElementType.Class);
            GenericIComparable = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IComparable", 1, ElementType.Class);
            GenericIComparer = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IComparer", 1, ElementType.Class);
            GenericIDictionary = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IDictionary", 2, ElementType.Class);
            GenericIEnumerator = (Interface)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "IEnumerator", 1, ElementType.Class);
            GenericKeyValuePair = (Struct)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "KeyValuePair", 2, ElementType.ValueType);
            GenericList = (Class)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "List", 1, ElementType.Class);
            GenericNullable = (Struct)GetGenericRuntimeTypeNodeFor("System", "Nullable", 1, ElementType.ValueType);
            GenericQueue = (Class)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "Queue", 1, ElementType.Class);
            GenericSortedDictionary = (Class)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "SortedDictionary", 2, ElementType.Class);
            GenericStack = (Class)GetGenericRuntimeTypeNodeFor("System.Collections.Generic", "Stack", 1, ElementType.Class);
            GC = (Class)GetTypeNodeFor("System", "GC", ElementType.Class);
            __HandleProtector = (Class)GetTypeNodeFor("System.Threading", "__HandleProtector", ElementType.Class);
            HandleRef = (Struct)GetTypeNodeFor("System.Runtime.InteropServices", "HandleRef", ElementType.ValueType);
            Hashtable = (Class)GetTypeNodeFor("System.Collections", "Hashtable", ElementType.Class);
            IASyncResult = (Interface)GetTypeNodeFor("System", "IAsyncResult", ElementType.Class);
            IComparable = (Interface)GetTypeNodeFor("System", "IComparable", ElementType.Class);
            IComparer = (Interface)GetTypeNodeFor("System.Collections", "IComparer", ElementType.Class);
            IDictionary = (Interface)GetTypeNodeFor("System.Collections", "IDictionary", ElementType.Class);
            IDisposable = (Interface)GetTypeNodeFor("System", "IDisposable", ElementType.Class);
            IEnumerator = (Interface)GetTypeNodeFor("System.Collections", "IEnumerator", ElementType.Class);
            IFormatProvider = (Interface)GetTypeNodeFor("System", "IFormatProvider", ElementType.Class);
            IHashCodeProvider = (Interface)GetTypeNodeFor("System.Collections", "IHashCodeProvider", ElementType.Class);
            IMembershipCondition = (Interface)GetTypeNodeFor("System.Security.Policy", "IMembershipCondition", ElementType.Class);
            IndexOutOfRangeException = (Class)GetTypeNodeFor("System", "IndexOutOfRangeException", ElementType.Class);
            InvalidCastException = (Class)GetTypeNodeFor("System", "InvalidCastException", ElementType.Class);
            InvalidOperationException = (Class)GetTypeNodeFor("System", "InvalidOperationException", ElementType.Class);
            IPermission = (Interface)GetTypeNodeFor("System.Security", "IPermission", ElementType.Class);
            ISerializable = (Interface)GetTypeNodeFor("System.Runtime.Serialization", "ISerializable", ElementType.Class);
            IStackWalk = (Interface)GetTypeNodeFor("System.Security", "IStackWalk", ElementType.Class);
            Marshal = (Class)GetTypeNodeFor("System.Runtime.InteropServices", "Marshal", ElementType.Class);
            MarshalByRefObject = (Class)GetTypeNodeFor("System", "MarshalByRefObject", ElementType.Class);
            MemberInfo = (Class)GetTypeNodeFor("System.Reflection", "MemberInfo", ElementType.Class);
            Monitor = (Class)GetTypeNodeFor("System.Threading", "Monitor", ElementType.Class);
            NativeOverlapped = (Struct)GetTypeNodeFor("System.Threading", "NativeOverlapped", ElementType.ValueType);
            NotSupportedException = (Class)GetTypeNodeFor("System", "NotSupportedException", ElementType.Class);
            NullReferenceException = (Class)GetTypeNodeFor("System", "NullReferenceException", ElementType.Class);
            OutOfMemoryException = (Class)GetTypeNodeFor("System", "OutOfMemoryException", ElementType.Class);
            ParameterInfo = (Class)GetTypeNodeFor("System.Reflection", "ParameterInfo", ElementType.Class);
            Queue = (Class)GetTypeNodeFor("System.Collections", "Queue", ElementType.Class);
            ReadOnlyCollectionBase = (Class)GetTypeNodeFor("System.Collections", "ReadOnlyCollectionBase", ElementType.Class);
            ResourceManager = (Class)GetTypeNodeFor("System.Resources", "ResourceManager", ElementType.Class);
            ResourceSet = (Class)GetTypeNodeFor("System.Resources", "ResourceSet", ElementType.Class);
            SerializationInfo = (Class)GetTypeNodeFor("System.Runtime.Serialization", "SerializationInfo", ElementType.Class);
            Stack = (Class)GetTypeNodeFor("System.Collections", "Stack", ElementType.Class);
            StackOverflowException = (Class)GetTypeNodeFor("System", "StackOverflowException", ElementType.Class);
            Stream = (Class)GetTypeNodeFor("System.IO", "Stream", ElementType.Class);
            StreamingContext = (Struct)GetTypeNodeFor("System.Runtime.Serialization", "StreamingContext", ElementType.ValueType);
            StringBuilder = (Class)GetTypeNodeFor("System.Text", "StringBuilder", ElementType.Class);
            StringComparer = (Class)GetTypeNodeFor("System", "StringComparer", ElementType.Class);
            StringComparison = GetTypeNodeFor("System", "StringComparison", ElementType.ValueType) as EnumNode;
            SystemException = (Class)GetTypeNodeFor("System", "SystemException", ElementType.Class);
            Thread = (Class)GetTypeNodeFor("System.Threading", "Thread", ElementType.Class);
            WindowsImpersonationContext = (Class)GetTypeNodeFor("System.Security.Principal", "WindowsImpersonationContext", ElementType.Class);

            SystemTypes.Initialized = true;
            object dummy = TargetPlatform.AssemblyReferenceFor; //Force selection of target platform
            if (dummy == null) return;
        }
        private static void ClearStatics()
        {
            AttributeUsageAttribute = null;
            ConditionalAttribute = null;
            DefaultMemberAttribute = null;
            InternalsVisibleToAttribute = null;
            ObsoleteAttribute = null;

            GenericICollection = null;
            GenericIEnumerable = null;
            GenericIList = null;
            ICloneable = null;
            ICollection = null;
            IEnumerable = null;
            IList = null;

            //Special attributes    
            AllowPartiallyTrustedCallersAttribute = null;
            AssemblyCompanyAttribute = null;
            AssemblyConfigurationAttribute = null;
            AssemblyCopyrightAttribute = null;
            AssemblyCultureAttribute = null;
            AssemblyDelaySignAttribute = null;
            AssemblyDescriptionAttribute = null;
            AssemblyFileVersionAttribute = null;
            AssemblyFlagsAttribute = null;
            AssemblyInformationalVersionAttribute = null;
            AssemblyKeyFileAttribute = null;
            AssemblyKeyNameAttribute = null;
            AssemblyProductAttribute = null;
            AssemblyTitleAttribute = null;
            AssemblyTrademarkAttribute = null;
            AssemblyVersionAttribute = null;
            ClassInterfaceAttribute = null;
            CLSCompliantAttribute = null;
            ComImportAttribute = null;
            ComRegisterFunctionAttribute = null;
            ComSourceInterfacesAttribute = null;
            ComUnregisterFunctionAttribute = null;
            ComVisibleAttribute = null;
            DebuggableAttribute = null;
            DebuggerHiddenAttribute = null;
            DebuggerStepThroughAttribute = null;
            DebuggingModes = null;
            DllImportAttribute = null;
            FieldOffsetAttribute = null;
            FlagsAttribute = null;
            GuidAttribute = null;
            ImportedFromTypeLibAttribute = null;
            InAttribute = null;
            IndexerNameAttribute = null;
            InterfaceTypeAttribute = null;
            MethodImplAttribute = null;
            NonSerializedAttribute = null;
            OptionalAttribute = null;
            OutAttribute = null;
            ParamArrayAttribute = null;
            RuntimeCompatibilityAttribute = null;
            SatelliteContractVersionAttribute = null;
            SerializableAttribute = null;
            SecurityAttribute = null;
            SecurityCriticalAttribute = null;
            SecurityTransparentAttribute = null;
            SecurityTreatAsSafeAttribute = null;
            STAThreadAttribute = null;
            StructLayoutAttribute = null;
            SuppressMessageAttribute = null;
            SuppressUnmanagedCodeSecurityAttribute = null;
            SecurityAction = null;

            //Classes need for System.TypeCode
            DBNull = null;
            DateTime = null;
            TimeSpan = null;

            //Classes and interfaces used by the Framework
            Activator = null;
            AppDomain = null;
            ApplicationException = null;
            ArgumentException = null;
            ArgumentNullException = null;
            ArgumentOutOfRangeException = null;
            ArrayList = null;
            AsyncCallback = null;
            Assembly = null;
            CodeAccessPermission = null;
            CollectionBase = null;
            CultureInfo = null;
            DictionaryBase = null;
            DictionaryEntry = null;
            DuplicateWaitObjectException = null;
            Environment = null;
            EventArgs = null;
            ExecutionEngineException = null;
            GenericArraySegment = null;
            GenericArrayToIEnumerableAdapter = null;
            GenericDictionary = null;
            GenericIComparable = null;
            GenericIComparer = null;
            GenericIDictionary = null;
            GenericIEnumerator = null;
            GenericKeyValuePair = null;
            GenericList = null;
            GenericNullable = null;
            GenericQueue = null;
            GenericSortedDictionary = null;
            GenericStack = null;
            GC = null;
            Guid = null;
            __HandleProtector = null;
            HandleRef = null;
            Hashtable = null;
            IASyncResult = null;
            IComparable = null;
            IDictionary = null;
            IComparer = null;
            IDisposable = null;
            IEnumerator = null;
            IFormatProvider = null;
            IHashCodeProvider = null;
            IMembershipCondition = null;
            IndexOutOfRangeException = null;
            InvalidCastException = null;
            InvalidOperationException = null;
            IPermission = null;
            ISerializable = null;
            IStackWalk = null;
            Marshal = null;
            MarshalByRefObject = null;
            MemberInfo = null;
            NativeOverlapped = null;
            Monitor = null;
            NotSupportedException = null;
            NullReferenceException = null;
            OutOfMemoryException = null;
            ParameterInfo = null;
            Queue = null;
            ReadOnlyCollectionBase = null;
            ResourceManager = null;
            ResourceSet = null;
            SerializationInfo = null;
            Stack = null;
            StackOverflowException = null;
            Stream = null;
            StreamingContext = null;
            StringBuilder = null;
            StringComparer = null;
            StringComparison = null;
            SystemException = null;
            Thread = null;
            WindowsImpersonationContext = null;
        }

        private static AssemblyNode/*!*/ GetSystemDataAssembly(bool doNotLockFile, bool getDebugInfo)
        {
            System.Reflection.AssemblyName aName = typeof(System.Data.IDataReader).Module.Assembly.GetName();
            Identifier SystemDataId = Identifier.For(aName.Name);
            AssemblyReference aref = (AssemblyReference)TargetPlatform.AssemblyReferenceFor[SystemDataId.UniqueIdKey];
            if (aref == null)
            {
                aref = new AssemblyReference();
                aref.Name = aName.Name;
                aref.PublicKeyOrToken = aName.GetPublicKeyToken();
                aref.Version = TargetPlatform.TargetVersion;
                TargetPlatform.AssemblyReferenceFor[SystemDataId.UniqueIdKey] = aref;
            }
            if (SystemDataAssemblyLocation.Location == null || SystemDataAssemblyLocation.Location.Length == 0)
                SystemDataAssemblyLocation.Location = typeof(System.Data.IDataReader).Module.Assembly.Location;
            if (aref.assembly == null) aref.Location = SystemDataAssemblyLocation.Location;
            return aref.assembly = AssemblyNode.GetAssembly(aref);
        }

        private static AssemblyNode/*!*/ GetSystemXmlAssembly(bool doNotLockFile, bool getDebugInfo)
        {
            System.Reflection.AssemblyName aName = typeof(System.Xml.XmlNode).Module.Assembly.GetName();
            Identifier SystemXmlId = Identifier.For(aName.Name);
            AssemblyReference aref = (AssemblyReference)TargetPlatform.AssemblyReferenceFor[SystemXmlId.UniqueIdKey];
            if (aref == null)
            {
                aref = new AssemblyReference();
                aref.Name = aName.Name;
                aref.PublicKeyOrToken = aName.GetPublicKeyToken();
                aref.Version = TargetPlatform.TargetVersion;
                TargetPlatform.AssemblyReferenceFor[SystemXmlId.UniqueIdKey] = aref;
            }
            if (SystemXmlAssemblyLocation.Location == null || SystemXmlAssemblyLocation.Location.Length == 0)
                SystemXmlAssemblyLocation.Location = typeof(System.Xml.XmlNode).Module.Assembly.Location;
            if (aref.assembly == null) aref.Location = SystemXmlAssemblyLocation.Location;
            return aref.assembly = AssemblyNode.GetAssembly(aref);
        }

        private static TypeNode/*!*/ GetGenericRuntimeTypeNodeFor(string/*!*/ nspace, string/*!*/ name, int numParams, ElementType typeCode)
        {
            if (TargetPlatform.GenericTypeNamesMangleChar != 0) name = name + TargetPlatform.GenericTypeNamesMangleChar + numParams;

            return SystemTypes.GetTypeNodeFor(nspace, name, typeCode);
        }
        private static TypeNode/*!*/ GetTypeNodeFor(string/*!*/ nspace, string/*!*/ name, ElementType typeCode)
        {
            TypeNode result = null;
            if (SystemAssembly == null)
                Debug.Assert(false);
            else
                result = SystemAssembly.GetType(Identifier.For(nspace), Identifier.For(name));
            if (result == null) result = CoreSystemTypes.GetDummyTypeNode(SystemAssembly, nspace, name, typeCode);
            result.typeCode = typeCode;
            return result;
        }
    }
}
