using System.IO;
using UnityEngine;

namespace UniFSharp
{

    public enum ProjectFileType
    {
        VisualStudioNormal,
        VisualStudioEditor
    }

    public static class FSharpBuildTools
    {
        public const string ToolName = "UniFSharp";
        public const string fsExtension = ".fs";
        public const string txtExtension = ".txt";
        public const string fsExtensionWildcard = "*.fs";
        public const string txtExtensionWildcard = "*.txt";
        public const string vsSolutionFileName = "Assembly-FSharp-vs.sln";
        public const string vsNormalFsprojFileName = "Assembly-FSharp-vs.fsproj";
        public const string vsEditorFsprojFileName = "Assembly-FSharp-Editor-vs.fsproj";
        public static string unityEnginedllPath = UnityEditor.EditorApplication.applicationContentsPath + "/Managed/UnityEngine.dll";
        public static string unityAssemblePath = Directory.GetCurrentDirectory() + @"\Library\ScriptAssemblies";
        public static string unityAssetsPath = Application.dataPath;
        public static string unityProjectPath = Directory.GetCurrentDirectory();
        public static string unityFsharpBinPath = @"Assets\" + FSharpBuildToolsWindow.FSharpOption.assemblyName;
        public const string projectRootPath = @"Assets\" + ToolName + @"\";
        public const string projectEditorPath = @"Editor\";
        public const string templatePath = projectRootPath + @"Template\";
        public const string settingsPath = projectRootPath + @"Settings\";
        public const string fsharpScriptTemplatePath = templatePath + @"FSharpScript\";
        public const string fsharpIconPath = projectRootPath + @"Texture\fsharp_icon.png";






    }
}