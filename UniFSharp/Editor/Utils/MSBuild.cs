using System;
using System.Diagnostics;
using System.IO;
using System.Text;
using UniFSharp;
using UnityEditor;

namespace UniFSharp
{

    public class MSBuild
    {
        static void InitOutputDir(string outputDirPath)
        {
            if (Directory.Exists(outputDirPath) == false)
            {
                Directory.CreateDirectory(outputDirPath);
            }
            else
            {
                //var files = Directory.GetFiles(outputDirPath);
                //foreach (var file in files)
                //{
                //    File.Delete(file);
                //}
            }
        }


        static string GetAargs(string _projectFilePath, string _outputDirPath, bool isDebug)
        {
            var projectFilePath = PathUtil.ReplaceDirAltSepFromSep(_projectFilePath);
            var outputDirPath = PathUtil.ReplaceDirAltSepFromSep(_outputDirPath);
            var args = new StringBuilder();
            var result = args.AppendFormat("\"{0}\"", projectFilePath)
                .AppendFormat(" /p:Configuration={0}", isDebug ? "Debug" : "Release")
                //.AppendFormat(" /p:OutputPath=\"{0}\"", outputDirPath)
                .Append(" /p:OptionExplicit=true")
                .Append(" /p:OptionCompare=binary")
                .Append(" /p:OptionStrict=true")
                .Append(" /p:OptionInfer=true")
                .Append(" /p:BuildProjectReferences=false")
                //.AppendFormat(" /p:DebugType={0}", isDebug ? "full" : "pdbonly")
                //.AppendFormat(" /p:DebugSymbols={0}", isDebug ? "true" : "false")
                //.AppendFormat(" /p:VisualStudioVersion={0}", "17.0") // TODO : 
                .AppendFormat(" /l:FileLogger,Microsoft.Build;logfile={0}", String.Format("{0}/{1}.log", outputDirPath, isDebug ? "DebugBuild" : "ReleaseBuild"))
                .Append(" /t:Clean;Rebuild")
                .ToString();
            return result;
        }




        public static int Execute(string projectFilePath, string outputDirPath, bool isDebug, DataReceivedEventHandler outputDataReceivedEventHandler, DataReceivedEventHandler errorDataReceivedEventHandler)
        {
            using (var p = new Process())
            {
                InitOutputDir(outputDirPath);
                p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = "cmd.exe";
                p.StartInfo.Arguments = @"/c dotnet build " + (GetAargs(projectFilePath, outputDirPath, isDebug));

                if (outputDataReceivedEventHandler != null || errorDataReceivedEventHandler != null)
                {
                    p.StartInfo.UseShellExecute = false;
                    p.StartInfo.CreateNoWindow = true;
                    p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                    if (outputDataReceivedEventHandler != null)
                    {
                        p.StartInfo.RedirectStandardOutput = true;
                        p.OutputDataReceived += outputDataReceivedEventHandler;
                    }
                    if (errorDataReceivedEventHandler != null)
                    {
                        p.StartInfo.RedirectStandardError = true;
                        p.ErrorDataReceived += errorDataReceivedEventHandler;
                    }
                }
                if (p.Start())
                {
                    if (outputDataReceivedEventHandler != null)
                    {
                        p.BeginOutputReadLine();
                    }
                    if (errorDataReceivedEventHandler != null)
                    {
                        p.BeginErrorReadLine();
                    }
                    p.WaitForExit();
                    return p.ExitCode;
                }
                else
                {
                    return p.ExitCode;
                }
            }
        }

    }

}