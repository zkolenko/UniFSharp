using System.Diagnostics;
using System.IO;
using Unity.CodeEditor;
using UnityEditor;

namespace UniFSharp
{
    public static class FSharpMenuItem
    {

        [MenuItem(FSharpOption.ToolName + "/Rebuild(Debug)", false, 10)]
        public static void RebuildDebug()
        {
            MSBuild.ExecuteMSBuild(true);
        }
        [MenuItem(FSharpOption.ToolName + "/Rebuild(Release)", false, 11)]
        public static void RebuildRelease()
        {
            MSBuild.ExecuteMSBuild(false);
        }

        [MenuItem(FSharpOption.ToolName + "/Update Solution", false, 30)]
        public static void updateSolution()
        {
            FSharpSolution.UpdateVisualStudioSolutionFile();
            FSharpProject.UpdateProjectFile();
            FSharpScriptAssetPostprocessor.updateCompileIncludeDlls(true,true,true);
            FSharpScriptAssetPostprocessor.createOrUpdateProject();
        }
        [MenuItem(FSharpOption.ToolName + "/Open F# Project", false, 50)]
        public static void openProject()
        {
            FSharpSolution.OpenExternalVisualStudio();
        }

        [MenuItem("Assets/Create/F# Script", false, 80)]
        public static void CreateNewBehaviourScript() { FSharpScriptCreateAsset.CreateFSharpScript("NewBehaviourScript.fs"); }


        [MenuItem("Assets/Create/F# Script+/Module", false, 20)]
        public static void createNewModule() { FSharpScriptCreateAsset.CreateFSharpScript("NewModule.fs"); }


        [MenuItem("Assets/Create/F# Script+/TabWindow", false, 21)]
        public static void createNewTabEditorWindow() { FSharpScriptCreateAsset.CreateFSharpScript("NewTabWindow.fs"); }


    }
}
