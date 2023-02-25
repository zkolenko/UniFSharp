using System.Collections.Generic;
using System.IO;
using UnityEngine;
using System.Linq;
using UnityEditorInternal;
using System.Text;
using System.Xml.Linq;
using System;

namespace UniFSharp
{
    public static class FSharpProject
    {
        public static string GetProjectGuid()
        {
            ChekProjectFile();
            var fsprojXDoc = XDocument.Load(FSharpOption.assemblyFileNamePath);
            var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
            var projectGuid = fsprojXDoc.Root
                        .Elements(XName.Get(ns + "PropertyGroup"))
                        .Elements(XName.Get(ns + "ProjectGuid"))
                        .Select(x => x.Value)
                        .DefaultIfEmpty("")
                        .First();
            return projectGuid;
        }

        public static void ChekProjectFile()
        {
            var assembliePath = FSharpOption.assemblyFileNamePath;
            if (File.Exists(assembliePath)) return;

            using (var template = new StreamReader(FSharpOption.templateAssembly , new UTF8Encoding(false)))
            {
                using (var sw = File.CreateText(assembliePath))
                {
                    sw.Write(template.ReadToEnd());
                    sw.Flush();
                }
            }
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


        public static string OpenFileNameSelector()
        {
            var editor = ScriptEditorUtility.GetExternalScriptEditor();
            if (editor.Contains("Visual Studio"))
            {
                return FSharpOption.solutionFileName;
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

        public static IEnumerable<string> GetAllFSharpScriptAssets()
        {
            var res = Directory.GetFiles(Application.dataPath, FSharpOption.fsExtensionWildcard, SearchOption.AllDirectories);
            return res;
        }

        static public IEnumerable<string> GetApplicationDlls(bool autoConnect)
        {
            var appDlls = FSharpOption.autoConnectApplicationDll;
            var dllApplication = Directory.GetFiles(FSharpOption.unityApplicationllPath, "*.dll", SearchOption.AllDirectories)
                .Where(x => {
                    var res = appDlls.Contains(Path.GetFileName(x).ToLower());
                    return (autoConnect) ? res : !res;
                });
            return dllApplication;
        }
        static public IEnumerable<string> GetAssemblieDlls(bool autoConnect)
        {
            var assDlls = FSharpOption.autoConnectAssembliesDll;
            var dllAssemblies = Directory.GetFiles(FSharpOption.unityAssemblePath, "*.dll", SearchOption.AllDirectories)
                .Where(x => {
                    var res = assDlls.Contains(Path.GetFileName(x).ToLower());
                    return (autoConnect) ? res : !res;
                });
            return dllAssemblies;
        }

        public static void UpdateProjectFile()
        {
            ChekProjectFile();

            var assembliePath = FSharpOption.assemblyFileNamePath;
            var fsprojXDoc = XDocument.Load(assembliePath);
            var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
            var option = FSharpOptionStorage.GetOptions();

            var guid = "{" + UnityEditor.VisualStudioIntegration.SolutionGuidGenerator.GuidForProject(FSharpOption.assemblyFileName).ToString() + "}";
            // update 
            FSharpScriptAssetPostprocessor.getXDocElements(fsprojXDoc, ns, "PropertyGroup").ToList()
                    .ForEach(p =>
                    {
                        p.Elements(XName.Get(ns + "ProjectGuid")).ToList().ForEach(e => e.Value = guid);
                        p.Elements(XName.Get(ns + "RootNamespace")).ToList().ForEach(e => e.Value = option.rootName);
                        p.Elements(XName.Get(ns + "AssemblyName")).ToList().ForEach(e => e.Value = FSharpOption.assemblieName);
                        p.Elements(XName.Get(ns + "TargetFramework")).ToList().ForEach(e => e.Value = AliasNameAttribute.ToAliasName<NetFramework, int>(option.netFramework));
                        p.Elements(XName.Get(ns + "OutputPath")).ToList().ForEach(e => e.Value = FSharpOption.fsharpBinPath);
                        p.Elements(XName.Get(ns + "DocumentationFile")).ToList().ForEach(e => e.Value = FSharpOption.fsharpBinPath + @"\DocumentationFile.xml");
                    });
            fsprojXDoc.Save(assembliePath);
            
            var f = File.OpenWrite(assembliePath);
            f.Flush();
            f.Close();
        }
    }
}

