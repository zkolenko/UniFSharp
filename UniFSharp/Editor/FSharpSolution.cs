using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Collections.Generic;
using System.Xml.Linq;
using UnityEngine;
using System.Linq;

namespace UniFSharp
{

    public sealed class FSharpSolution
    {

        public static string CreateSolutionFile(string pathName, string resourceFile, string normalProjectGuid, string editorProjectGuid)
        {
            using (var sr = new StreamReader(resourceFile + FSharpBuildTools.txtExtension, new UTF8Encoding(false)))
            {
                using (var sw = File.CreateText(pathName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(pathName);
                    var slnGuidValue = Guid.NewGuid().ToString();
                    string csharpProjects = "";
                    if (FSharpBuildToolsWindow.FSharpOption.addCSharpProjectsToAssembly)
                    {
                        csharpProjects = GetAllCSharpProjects(slnGuidValue);
                    }

                    var sln = Regex.Replace(sr.ReadToEnd(), "#SolutionGuid#", slnGuidValue)
                                .Replace("#NormalProjectGuid#", normalProjectGuid)
                                .Replace("#EditorProjectGuid#", editorProjectGuid)
                                .Replace("#CSharpProjects#", csharpProjects);
                    sw.Write(sln);
                    sw.Flush();
                    sw.Close();
                    return fileName;
                }
            }
        }

        public static string GetAllCSharpProjects(string slnGuidValue)
        {
            var csharpProjects = Directory.GetFiles(FSharpBuildTools.unityProjectPath, "*.csproj", SearchOption.AllDirectories);
            string res = "";
            foreach (var path in csharpProjects)
            {
                var fileNameProject = Path.GetFileName(path);
                var nameProject = fileNameProject.Replace(".csproj", "");
                if (nameProject.Contains("Assembly-CSharp-Editor-firstpass")) continue;

                var fsprojXDoc = XDocument.Load(path);
                var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
                var projectGuid = fsprojXDoc.Root
                            .Elements(XName.Get(ns + "PropertyGroup"))
                            .Elements(XName.Get(ns + "ProjectGuid"))
                            .Select(x => x.Value)
                            .DefaultIfEmpty("")
                            .First();

                var project = "Project(\"{" + slnGuidValue + "}\") = \"" + nameProject + "\", \"" + fileNameProject + "\", \"" + projectGuid + "\" EndProject\n";
                res += project;
            }
            return res;
        }

        public static void CreateVisualStudioSolutionFile()
        {
            var vsSolutionFile = FSharpProject.GetProjectRootPath() + FSharpBuildTools.vsSolutionFileName;
            if (File.Exists(vsSolutionFile) == false)
            {
                UpdateVisualStudioSolutionFile();
            }
        }
        public static void UpdateVisualStudioSolutionFile()
        {
            var vsSolutionFile = FSharpProject.GetProjectRootPath() + FSharpBuildTools.vsSolutionFileName;
            var normalProjectGuid = FSharpProject.GetProjectGuid(ProjectFileType.VisualStudioNormal);
            var editorProjectGuid = FSharpProject.GetProjectGuid(ProjectFileType.VisualStudioEditor);
            var vsSolutionPath = FSharpProject.GetProjectPath(FSharpBuildTools.vsSolutionFileName);
            var templatePath = PathUtil.AppendDirSep(FSharpBuildTools.templatePath + FSharpBuildToolsWindow.FSharpOption.vsVersion.ToString());
            CreateSolutionFile(vsSolutionPath, (templatePath + FSharpBuildTools.vsSolutionFileName), normalProjectGuid, editorProjectGuid);
        }
    }
}