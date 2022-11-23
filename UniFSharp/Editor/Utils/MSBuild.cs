using System;
using System.Diagnostics;
using System.IO;
using System.Text;
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
        }


        static string GetAargs(string _projectFilePath, string _outputDirPath, bool isDebug)
        {
            var projectFilePath = PathUtil.ReplaceDirAltSepFromSep(_projectFilePath);
            var outputDirPath = PathUtil.ReplaceDirAltSepFromSep(_outputDirPath);
            var args = new StringBuilder();
            var result = args.AppendFormat("\"{0}\"", projectFilePath)
                .AppendFormat(" /p:Configuration={0}", isDebug ? "Debug" : "Release")
                .Append(" /p:OptionExplicit=true")
                .Append(" /p:OptionCompare=binary")
                .Append(" /p:OptionStrict=true")
                .Append(" /p:OptionInfer=true")
                .Append(" /p:DebugType=embedded")
                .AppendFormat(" /p:DebugSymbols={0}", (isDebug)?"true" : "false")
                .Append(" /p:BuildProjectReferences=false")
                .AppendFormat(" /l:FileLogger,Microsoft.Build;logfile={0}", String.Format("{0}/{1}.log", outputDirPath, isDebug ? "DebugBuild" : "ReleaseBuild"))
                .Append(" /t:Clean;Rebuild")
                .ToString();
            return result;
        }


        public static void ExecuteMSBuild(bool isdebug)
        {
            var option = FSharpOption.GetOptions();

            string outputAssemblyPath = FSharpOption.fsharpBinPath;
            if (option.buildLogConsoleOutput == false)
            {
                var handler = new DataReceivedEventHandler((x, e) => { });
                MSBuild.Execute(FSharpOption.assemblyFileNamePath(), outputAssemblyPath, isdebug, handler, handler);
            }
            else
            {
                var outputHandler = new DataReceivedEventHandler((x, e) =>
                {
                    if (e != null && System.String.IsNullOrEmpty(e.Data) == false)
                    {
                        UnityEngine.Debug.Log(e.Data);
                    }
                });
                var errorHandler = new DataReceivedEventHandler((x, e) =>
                {
                    if (e != null && System.String.IsNullOrEmpty(e.Data) == false)
                    {
                        UnityEngine.Debug.Log(e.Data);
                    }
                });
                MSBuild.Execute(FSharpOption.assemblyFileNamePath(), outputAssemblyPath, isdebug, outputHandler, errorHandler);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }

        public static int Execute(string projectFilePath, string outputDirPath, bool isDebug, DataReceivedEventHandler outputDataReceivedEventHandler, DataReceivedEventHandler errorDataReceivedEventHandler)
        {
            using (var p = new Process())
            {
                InitOutputDir(outputDirPath);

                var filename = "cmd.exe";
                var arguments = @"/c dotnet build " + (GetAargs(projectFilePath, outputDirPath, isDebug));

                p.StartInfo.StandardOutputEncoding = Encoding.GetEncoding(866);
                p.StartInfo.WindowStyle = ProcessWindowStyle.Hidden;
                p.StartInfo.CreateNoWindow = true;
                p.StartInfo.UseShellExecute = true;
                p.StartInfo.FileName = filename;
                p.StartInfo.Arguments = arguments;

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