using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Linq;
using System;

namespace UniFSharp
{
    public static class FSharpProject
    {
        const string xmlSourceFileDataEnter = "    <Compile Include=\"";
        const string xlmSourceFileDataEnding = "\" />\n";


        public static string GetProjectGuid(ProjectFileType projectFileType)
        {
            var fsprojFileName = GetFSharpProjectFileName(projectFileType);
            var vsFsprojPath = GetFSharpProjectPath(fsprojFileName);
            if (File.Exists(vsFsprojPath) == false)
            {
                CreateFSharpProjectFile(projectFileType);
            }
            var fsprojXDoc = XDocument.Load(vsFsprojPath);
            var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
            var projectGuid = fsprojXDoc.Root
                        .Elements(XName.Get(ns + "PropertyGroup"))
                        .Elements(XName.Get(ns + "ProjectGuid"))
                        .Select(x => x.Value)
                        .DefaultIfEmpty("")
                        .First();
            return projectGuid;
        }




        public static string Surround(string s)
        {
            var res = Path.DirectorySeparatorChar.ToString() + s + Path.DirectorySeparatorChar.ToString();
            return res;
        }


        public static string GetJudgmentRelativePath(string pathName)
        {
            var basePath = GetProjectRootPath();
            var res = PathUtil.GetRelativePath(basePath, PathUtil.GetAbsolutePath(basePath, pathName));
            res = PathUtil.GetDirectoryName(res);
            res = Surround(res);
            return res;
        }

        public static bool ContainsEditorFolder(string pathName)
        {
            var directory = GetJudgmentRelativePath(pathName);
            return PathUtil.ReplaceDirSepFromAltSep(directory).Contains(Surround("Editor"));
        }



        public static string OpenFileNameSelector()
        {
            var editor = ScriptEditorUtility.GetExternalScriptEditor();
            if (editor.Contains("Visual Studio"))
            {
                return FSharpBuildTools.vsSolutionFileName;
            }
            else
            {
                throw new System.Exception("allow only Visual Studio");
            }
        }
        public static string GetProjectRootPath()
        {
            var res = PathUtil.GetUpDirectory(1, Application.dataPath);
            res = PathUtil.ReplaceDirSepFromAltSep(res);
            res = PathUtil.AppendDirSep(res);
            return res;
        }
        public static string GetFSharpProjectPath(string fsprojFileName)
        {
            return GetProjectRootPath() + fsprojFileName;
        }
        public static string GetFSharpBinPath()
        {
            return GetProjectRootPath() + FSharpBuildTools.unityFsharpBinPath;
        }

        public static string GetFSharpProjectTemplateFilePath(ProjectFileType projectFileType)
        {
            string version = FSharpBuildToolsWindow.FSharpOption.vsVersion.ToString();
            if (projectFileType == ProjectFileType.VisualStudioNormal)
            {
                version = FSharpBuildToolsWindow.FSharpOption.vsVersion.ToString();
            }
            else if (projectFileType == ProjectFileType.VisualStudioEditor)
            {
                version = FSharpBuildToolsWindow.FSharpOption.vsVersion.ToString();
            }
            var res = FSharpBuildTools.templatePath + version;
            res = PathUtil.ReplaceDirSepFromAltSep(res);
            res = PathUtil.AppendDirSep(res);
            return res;
        }

        private static IEnumerable<string> AllDirectoryName(string _path, string _targetPath)
        {
            var path = PathUtil.ReplaceDirSepFromAltSep(_path);
            path = PathUtil.AppendDirSep(path);
            path = PathUtil.GetAbsolutePath(Application.dataPath, path);
            path = PathUtil.ReplaceDirSepFromAltSep(path);
            var result = Directory.GetFiles(path, FSharpBuildTools.fsExtensionWildcard, SearchOption.AllDirectories);
            var result1 = result.Where(x => x != _targetPath).ToArray();
            var result2 = result1.Select(x => PathUtil.ReplaceDirSepFromAltSep(PathUtil.GetDirectoryName(x))).ToArray();
            var result3 = result2.Append(PathUtil.ReplaceDirSepFromAltSep(Application.dataPath)).ToArray();
            var result4 = result3.Distinct().ToArray();
            return result4;
        }
        private static string GetNewFolderName(string _path, IEnumerable<string> allDirectories)
        {
            var path = PathUtil.ReplaceDirSepFromAltSep(_path);
            var beforePath = path;
            path = PathUtil.RemoveDirSep(PathUtil.GetUpDirectory(1, path));
            if (allDirectories.Any(x => x == path))
            {
                return beforePath;
            }
            else
            {
                return GetNewFolderName(path, allDirectories);
            }
        }
        public static bool FsharpScriptCreatable(string _targetPath)
        {
            string[] allDirectories = AllDirectoryName(Application.dataPath, _targetPath).ToArray();

            var path = PathUtil.GetAbsolutePath(Application.dataPath, _targetPath);
            path = PathUtil.ReplaceDirSepFromAltSep(path);
            if (allDirectories.Any(x => x == path))
            {
                return true;
            }
            else
            {
                var v0 = allDirectories.Select(x => PathUtil.GetRelativePath(Application.dataPath, x)).ToArray();
                var v1 = v0.Where(x => x != "").ToArray();
                var v2 = v1.Select(x => x.Split(Path.AltDirectorySeparatorChar)).ToArray();
                var v3 = v2.SelectMany(x => x).ToArray();
                var v4 = v3.Distinct().ToArray();
                string[] directories = v4;

                var targetPath = path;
                var relativePath = PathUtil.GetRelativePath(GetNewFolderName(path, allDirectories), targetPath);
                string[] folderNames = relativePath.Split(Path.AltDirectorySeparatorChar).Distinct().ToArray();
                var exist = directories.Any(x => folderNames.Any(y => y == x));
                return exist == false;
            }
        }
        public static string GetFSharpProjectFileName(ProjectFileType projectFileType)
        {
            if (projectFileType == ProjectFileType.VisualStudioNormal)
            {
                return FSharpBuildTools.vsNormalFsprojFileName;
            }
            else
            {
                return FSharpBuildTools.vsEditorFsprojFileName;
            }
        }

        public static string GetFSharpProjectFilePath(ProjectFileType projectFileType)
        {
            string fsharpPrjectFileName = GetFSharpProjectFileName(projectFileType);
            return GetFSharpProjectPath(fsharpPrjectFileName);
        }

        public static IEnumerable<string> GetAllFSharpScriptAssets(ProjectFileType projectFileType)
        {
            var basePath = GetProjectRootPath();
            switch (projectFileType)
            {
                case ProjectFileType.VisualStudioNormal:
                    var res = Directory.GetFiles(Application.dataPath, FSharpBuildTools.fsExtensionWildcard, SearchOption.AllDirectories)
                        .Where(pathName => ContainsEditorFolder(pathName) == false);
                    return res;
                case ProjectFileType.VisualStudioEditor:
                    res = Directory.GetFiles(Application.dataPath, FSharpBuildTools.fsExtensionWildcard, SearchOption.AllDirectories)
                        .Where(pathName => ContainsEditorFolder(pathName));
                    return res;
                default:
                    return new string[0];
            }
        }


        public static string GetFSharpItems(ProjectFileType projectFileType)
        {
            var files = GetAllFSharpScriptAssets(projectFileType);
            var items = "";
            foreach (var file in files)
            {
                var basePath = Application.dataPath;
                var absolutePath = PathUtil.GetAbsolutePath(basePath, file);
                if (File.Exists(absolutePath))
                {
                    var relativePath = PathUtil.GetRelativePath(basePath, absolutePath);
                    items += xmlSourceFileDataEnter + PathUtil.ReplaceDirSepFromAltSep(relativePath) + xlmSourceFileDataEnding;
                }
            }
            return items;
        }

        public static string GetDlls(string[] dlls)
        {
            var result = dlls.Select(curentDll =>
            {
                var dllName = Path.GetFileName(curentDll).Replace(".dll", "");
                var item = $"  <Reference Include=\"{dllName}\">\n" +
                            $"    <HintPath>{curentDll}</HintPath>\n" +
                            "    <Private>False</Private>\n" +
                            "  </Reference>\n";
                return item;
            }).Aggregate((x, y) => x + y);
            return result;
        }







        public static string CreateProjectFile(string pathName, string resourceFile, ProjectFileType projectFileType)
        {
            using (var template = new StreamReader(resourceFile + FSharpBuildTools.txtExtension, new UTF8Encoding(false)))
            {
                using (var sw = File.CreateText(pathName))
                {
                    var guidValue = UnityEditor.VisualStudioIntegration.SolutionGuidGenerator.GuidForProject(GetFSharpProjectFileName(projectFileType));
                    var dllUnityEngine = GetDlls(Directory.GetFiles(PathUtil.ReplaceDirSepFromAltSep(UnityEditor.EditorApplication.applicationContentsPath + @"\Managed"), "UnityEngine.dll", SearchOption.TopDirectoryOnly));
                    var dllUnityEditor = GetDlls(Directory.GetFiles(PathUtil.ReplaceDirSepFromAltSep(UnityEditor.EditorApplication.applicationContentsPath + @"\Managed"), "UnityEditor.dll", SearchOption.TopDirectoryOnly));
                    var data = Regex.Replace(template.ReadToEnd(), "#TargetFrameworkVersion#", AliasNameAttribute.ToAliasName<NetFramework, int>(FSharpBuildToolsWindow.FSharpOption.netFramework))
                            .Replace("#RootNamespace#", FSharpBuildToolsWindow.FSharpOption.rootNameSpace)
                            .Replace("#AssemblyName#", FSharpBuildToolsWindow.FSharpOption.assemblyName)
                            .Replace("#RootNamespaceEditor#", FSharpBuildToolsWindow.FSharpOption.rootNameSpaceEditor)
                            .Replace("#AssemblyNameEditor#", FSharpBuildToolsWindow.FSharpOption.assemblyNameEditor)
                            .Replace("#UnityEnginePath#", UnityEditorInternal.InternalEditorUtility.GetEngineAssemblyPath())
                            .Replace("#ProjectGuid#", "{" + guidValue.ToString() + "}")
                            .Replace("#OutputPath#", FSharpBuildTools.unityFsharpBinPath)
                            .Replace("#DocumentationFile#", FSharpBuildTools.unityFsharpBinPath + @"\DocumentationFile")
                            .Replace("#DocumentationFileEditor#", FSharpBuildTools.unityFsharpBinPath + @"\DocumentationFileEditor")
                            .Replace("#FSharpItems#", GetFSharpItems(projectFileType))
                            .Replace("#ApplicationDlls#", dllUnityEngine + dllUnityEditor)
                            .Replace("#ScriptAssembliesDlls#", GetDlls(Directory.GetFiles(PathUtil.ReplaceDirSepFromAltSep(Directory.GetCurrentDirectory() + @"\Library\ScriptAssemblies"), "*.dll", SearchOption.TopDirectoryOnly)));
                    sw.Write(data);
                    sw.Flush();
                    return guidValue.ToString();
                }
            }
        }

        public static string CreateFSharpProjectFile(ProjectFileType projectFileType)
        {
            string fsharpPrjectFileName = GetFSharpProjectFileName(projectFileType);
            var vsFsprojPath = GetFSharpProjectPath(fsharpPrjectFileName);
            if (File.Exists(vsFsprojPath))
            {
                File.Delete(vsFsprojPath);
            }
            var templatePath = GetFSharpProjectTemplateFilePath(projectFileType);
            var projectGUID = CreateProjectFile(vsFsprojPath, templatePath + fsharpPrjectFileName, projectFileType);
            return projectGUID;
        }

    }
}

