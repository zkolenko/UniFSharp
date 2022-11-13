using System;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;

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
                    var slnGuidValue = Guid.NewGuid();
                    var sln = Regex.Replace(sr.ReadToEnd(), "#SolutionGuid#", slnGuidValue.ToString())
                                .Replace("#NormalProjectGuid#", normalProjectGuid)
                                .Replace("#EditorProjectGuid#", editorProjectGuid);
                    sw.Write(sln);
                    sw.Flush();
                    sw.Close();
                    return fileName;
                }
            }
        }

        public static void CreateVisualStudioSolutionFile()
        {
            var vsSolutionFile = FSharpProject.GetProjectRootPath() + FSharpBuildTools.vsSolutionFileName;
            if (File.Exists(vsSolutionFile) == false)
            {
                var normalProjectGuid = FSharpProject.GetProjectGuid(ProjectFileType.VisualStudioNormal);
                var editorProjectGuid = FSharpProject.GetProjectGuid(ProjectFileType.VisualStudioEditor);
                var vsSolutionPath = FSharpProject.GetFSharpProjectPath(FSharpBuildTools.vsSolutionFileName);
                var templatePath = PathUtil.AppendDirSep(FSharpBuildTools.templatePath + FSharpBuildToolsWindow.FSharpOption.vsVersion.ToString());
                CreateSolutionFile(vsSolutionPath, (templatePath + FSharpBuildTools.vsSolutionFileName), normalProjectGuid, editorProjectGuid);
            }
        }
    }
}