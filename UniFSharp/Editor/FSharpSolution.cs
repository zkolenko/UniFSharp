using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Xml.Linq;
using System.Linq;
using System.Diagnostics;
using System.Runtime.InteropServices;
using Unity.CodeEditor;
using UnityEditor;
using Codice.Utils;
using UniFSharp;
using UnityEngine;

namespace UniFSharp
{

    public sealed class FSharpSolution
    {

        public static string CreateSolutionFile(string pathName, string resourceFile, string projectGuid)
        {
            using (var sr = new StreamReader(resourceFile, new UTF8Encoding(false)))
            {
                using (var sw = File.CreateText(pathName))
                {
                    var fileName = Path.GetFileNameWithoutExtension(pathName);
                    var slnGuidValue = Guid.NewGuid().ToString();
                    string csharpProjects = "";
                    csharpProjects = GetAllCSharpProjects(slnGuidValue);

                    var sln = Regex.Replace(sr.ReadToEnd(), "#SolutionGuid#", slnGuidValue)
                                .Replace("#ProjectGuid#", projectGuid)
                                .Replace("#CSharpProjects#", csharpProjects);
                    sw.Write(sln);
                    sw.Flush();
                    sw.Close();
                    return fileName;
                }
            }
        }

        public static void CreateVisualStudioSolutionFile()
        {
            var vsSolutionFile = FSharpProject.GetProjectRootPath() + FSharpOption.solutionFileName;
            if (File.Exists(vsSolutionFile) == false)
            {
                UpdateVisualStudioSolutionFile();
            }
        }
        public static void UpdateVisualStudioSolutionFile()
        {
            var vsSolutionFile = FSharpProject.GetProjectRootPath() + FSharpOption.solutionFileName;
            var projectGuid = FSharpProject.GetProjectGuid();
            var vsSolutionPath = FSharpOption.solutionFileNamePath();
            CreateSolutionFile(vsSolutionPath, FSharpOption.templateSolution(), projectGuid);
        }

        public static string GetAllCSharpProjects(string slnGuidValue)
        {
            var csharpProjects = Directory.GetFiles(FSharpOption.unityProjectPath, "*.csproj", SearchOption.AllDirectories);
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


        public static void OpenExternalVisualStudio(string fileName = "", int lineNumber = 0)
        {
            var solutionFilePath = PathUtil.ReplaceDirAltSepFromSep(FSharpOption.unityProjectPath) + "/" + FSharpOption.solutionFileName;
            if (File.Exists(solutionFilePath) == false)
            {
                CreateVisualStudioSolutionFile();
            }

            string vsVersion = AliasNameAttribute.ToAliasName<Enum, int>(FSharpOption.GetOptions().vsVersion);
            solutionFilePath = PathUtil.ReplaceDirSepFromAltSep(solutionFilePath);
            fileName = PathUtil.ReplaceDirSepFromAltSep(fileName);

            var p = new System.Diagnostics.Process();
            p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = true;
            p.StartInfo.FileName = "\"" + FSharpOption.projectRootPath + "DTE.exe" + "\"";
            p.StartInfo.Arguments = "\"" + vsVersion + "\" \"" + solutionFilePath + "\" \"" + fileName + "\" \"" + lineNumber + "\"";
            p.Start();
        }
    }
}

