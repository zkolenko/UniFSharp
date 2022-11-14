namespace UniFSharp
{
    public class FSharpOption
    {
        public bool foldoutIDE = true;
        public VsVersion vsVersion = VsVersion.Vs2022;

        //fsproj
        public bool foldoutFsprojDetail = false;
        //"#TargetFrameworkVersion#"
        public NetFramework netFramework = NetFramework.Net48;

        //#AssemblyName#
        public string assemblyName = "AssemblyFSharp";

        //"#RootNamespace#"
        public string rootNameSpace = "AssemblyFSharp";

        //#AssemblyName#
        public string assemblyNameEditor = "AssemblyFSharpEditor";

        //"#RootNamespace#"
        public string rootNameSpaceEditor = "AssemblyFSharpEditor";

        // Build Log Console Outoput
        public bool foldoutOther = true;
        public bool buildLogConsoleOutput = true;

        // AssemblySearch
        public AssemblySearch assemblySearch = AssemblySearch.Simple;
        
        public bool addCSharpProjectsToAssembly = false; 
    }

    public enum VsVersion
    {
        [AliasName("15.0")] Vs2019 = 0,
        [AliasName("17.0")] Vs2022 = 1
    }

    public enum NetFramework
    {
        [AliasName("net4.8")] Net48 = 0
    }
    public enum AssemblySearch
    {
        [AliasName("Simple")] Simple = 0,
        [AliasName("F# Compiler Service")] CompilerService = 1
    }
}