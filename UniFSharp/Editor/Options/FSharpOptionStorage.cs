using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{
    public class FSharpOptionStorage : ScriptableObject
    {
        [SerializeField]
        public VsVersion vsVersion = VsVersion.Vs2022;
        [SerializeField]
        public NetFramework netFramework = NetFramework.Net48;
        [SerializeField]
        public string rootName = "FSharp";

        // Build Log Console Outoput
        [SerializeField]
        public bool buildLogConsoleOutput = true;
        [SerializeField]
        public AutoBuild autoBuild = AutoBuild.None;

        [SerializeField]
        public AssemblySearch assemblySearch = AssemblySearch.Simple;
        [SerializeField]
        public List<string> applicationDlls = new List<string>();
        [SerializeField]
        public List<string> assemblieDlls = new List<string>();
        [SerializeField]
        public List<string> assetDlls = new List<string>();

        public static FSharpOptionStorage GetOptions()
        {
            string path = FSharpOption.projectRootPath + "FSharpOptions.asset";
            if (File.Exists(path))
            {
                var result = AssetDatabase.LoadAssetAtPath<FSharpOptionStorage>(path);
                return result;
            }
            else
            {
                FSharpOptionStorage asset = ScriptableObject.CreateInstance<FSharpOptionStorage>();
                AssetDatabase.CreateAsset(asset, path);
                AssetDatabase.SaveAssets();
                return asset;
            }
        }

    }

    public static class FSharpOption
    {
        public const string ToolName = "UniFSharp";
        public const string fsExtension = ".fs";
        public const string txtExtension = ".txt";
        public const string fsExtensionWildcard = "*.fs";
        public const string txtExtensionWildcard = "*.txt";
        public const string assemblieName = "Assemblie-FSharp";
        public const string solutionFileName = "Solution-FSharp.sln";
        public const string assemblyFileName = "Assembly-FSharp.fsproj";
        public static string assemblyFileNamePath = FSharpProject.GetProjectRootPath() + assemblyFileName;
        public static string solutionFileNamePath = FSharpProject.GetProjectRootPath() + solutionFileName;
        public static string unityApplicationllPath = PathUtil.ReplaceDirSepFromAltSep(UnityEditor.EditorApplication.applicationContentsPath + @"\Managed\");
        public static string unityAssemblePath = Directory.GetCurrentDirectory() + @"\Library\ScriptAssemblies";
        public static string unityProjectPath = Directory.GetCurrentDirectory();
        public static string unityAssetsPath = Directory.GetCurrentDirectory() + @"\Assets";
        public const string fsharpBinPath = projectRootPath + @"Dll";

        public const string projectRootPath = @"Assets\" + ToolName + @"\";
        public static string projectRootAbsolutePath = Directory.GetCurrentDirectory() + @"\" + projectRootPath;
        public const string fsharpIconPath = projectRootPath + @"Texture\fsharp_icon.png";

        public static List<string> autoConnectApplicationDll = new List<string> { "unityeditor.dll", "unityengine.dll", "unityeditor.coremodule.dll", "unityengine.coremodule.dll" };
        public static List<string> autoConnectAssembliesDll = new List<string> { "assembly-csharp.dll", "assembly-csharp-editor.dll" };

        // TEMPLATES
        public static string templatePath = FSharpProject.GetProjectRootPath() + projectRootPath + @"Template\";
        public static string templateAssembly = templatePath + @"VisualStudio\Assembly.fsproj.txt";
        public static string templateSolution = templatePath + @"VisualStudio\Solution.sln.txt";
        public static string fsharpScriptTemplatePath = templatePath + @"FSharpScript\";
    }
    public enum FSharpCore
    {
        [AliasName("FSharp.Core.4.0")] FSharpCore4 = 0,
        [AliasName("FSharp.Core.5.0")] FSharpCore5 = 1,
        [AliasName("FSharp.Core.6.0")] FSharpCore6 = 2,
        [AliasName("FSharp.Core.7.0")] FSharpCore7 = 3
    }
    public enum NetFramework
    {
        [AliasName("net4.8")] Net48 = 0,
        [AliasName("netstandard2.0")] Netstandard20 = 1,
        [AliasName("netstandard2.1")] Netstandard21 = 2
    }
    public enum VsVersion
    {
        [AliasName("Visual studio 2017")] Vs2017 = 0,
        [AliasName("Visual studio 2019")] Vs2019 = 1,
        [AliasName("Visual studio 2022")] Vs2022 = 2
    }
    public enum AssemblySearch
    {
        [AliasName("Simple")] Simple = 0,
        [AliasName("F# Compiler Service")] CompilerService = 1
    }
    public enum AutoBuild
    {
        [AliasName("None")] None = 0,
        [AliasName("Debug")] Debug = 1,
        [AliasName("Release")] Release = 2
    }


    [AttributeUsage(AttributeTargets.Field, Inherited = false, AllowMultiple = false)]
    public class AliasNameAttribute : Attribute
    {
        string value;
        public AliasNameAttribute(string aliasName)
        {
            value = aliasName;
        }
        public string AliasName { get { return value; } }

        public static string ToAliasName<TEnum, U>(TEnum value) where TEnum : System.Enum
        {
            //var result = value.GetType().GetField(value.ToString()).GetCustomAttributes(typeof(AliasNameAttribute), false).Cast<AliasNameAttribute>().Where(x => x != null).Single();
            var v1 = value.GetType();
            var v2 = v1.GetField(value.ToString());
            var v3 = v2.GetCustomAttributes(typeof(AliasNameAttribute), false);
            var v4 = v3.Cast<AliasNameAttribute>();
            var v5 = v4.Where(x => x != null);
            try
            {
                var v6 = v5.Single();
                return v6.AliasName;
            }
            catch (Exception)
            {
                throw new ArgumentException("AliasNameAttribute is not found.");
            }
        }
        public static string ToAliasName<TEnum, U>(U value) where TEnum : System.Enum
        {
            var res = (TEnum)Enum.ToObject(typeof(TEnum), value);
            return ToAliasName<TEnum, U>(res);
        }
    }



}