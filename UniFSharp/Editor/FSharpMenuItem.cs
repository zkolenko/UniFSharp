using System.Diagnostics;
using System.IO;
using UnityEditor;

namespace UniFSharp
{
    public static class FSharpMenuItem
    {

        public static void ExecuteMSBuild(ProjectFileType projectfiletype, bool isdebug)
        {
            var vsFSharpPrjectPath = FSharpProject.GetFSharpProjectFilePath(projectfiletype);
            if (File.Exists(vsFSharpPrjectPath) == false)
            {
                FSharpProject.CreateFSharpProjectFile(projectfiletype);
            }
            var outputAssemblyPath = FSharpBuildTools.unityFsharpBinPath;
            if (FSharpBuildToolsWindow.FSharpOption.buildLogConsoleOutput == false)
            {
                var handler = new DataReceivedEventHandler((x, e) => { });
                MSBuild.Execute(vsFSharpPrjectPath, outputAssemblyPath, isdebug, handler, handler);
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
                MSBuild.Execute(vsFSharpPrjectPath, outputAssemblyPath, isdebug, outputHandler, errorHandler);
            }
            AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
        }




        [MenuItem(FSharpBuildTools.ToolName + "/Rebuild(Debug)", false, 10)]
        public static void RebuildDebug()
        {
            ExecuteMSBuild(ProjectFileType.VisualStudioNormal, true);
        }
        [MenuItem(FSharpBuildTools.ToolName + "/Rebuild(Release)", false, 11)]
        public static void RebuildRelease()
        {
            ExecuteMSBuild(ProjectFileType.VisualStudioNormal, false);
        }

        [MenuItem(FSharpBuildTools.ToolName + "/", false, 20)]
        static void Separator() { }


        [MenuItem(FSharpBuildTools.ToolName + "/Editor Rebuild(Debug)", false, 30)]
        public static void editorRebuildDebug()
        {
            ExecuteMSBuild(ProjectFileType.VisualStudioEditor, true);
        }

        [MenuItem(FSharpBuildTools.ToolName + "/Editor Rebuild(Release)", false, 31)]
        public static void editorRebuildRelease()
        {
            ExecuteMSBuild(ProjectFileType.VisualStudioEditor, false);
        }


        [MenuItem("Assets/Create/F# Script", false, 80)]
        public static void CreateNewBehaviourScript() { FSharpScriptCreateAsset.CreateFSharpScript("NewBehaviourScript.fs"); }


        [MenuItem("Assets/Create/F# Script+/Module", false, 20)]
        public static void createNewModule() { FSharpScriptCreateAsset.CreateFSharpScript("NewModule.fs"); }


        [MenuItem("Assets/Create/F# Script+/TabWindow", false, 21)]
        public static void createNewTabEditorWindow() { FSharpScriptCreateAsset.CreateFSharpScript("NewTabWindow.fs"); }


        //[MenuItem("Assets/Create/F# Script+/more...", false, 101)]
        //public static void more() { MoreFSharpScriptWindow.ShowWindow(); }

    }
}
