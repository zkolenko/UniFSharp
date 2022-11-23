using System.IO;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using UnityEditor;
using UnityEngine;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using UnityEngine.UIElements;
using UnityEngine.PlayerLoop;
using UnityEditor.PackageManager.UI;
using UnityEditor.UIElements;
using UnityEditor.Overlays;
using UnityEditor.SearchService;
using UnityEditor.Graphs;
using UnityEditor.U2D;
using UnityEditorInternal;

namespace UniFSharp
{

    //[CustomEditor(typeof(UnityEditor.DragAndDrop))]
    [CustomEditor(typeof(UnityEngine.Transform))]
    public class TransformInspector : Editor
    {
        void OnEnable()
        {
            Repaint();
        }

        public override void OnInspectorGUI()
        {

            EditorGUILayout.BeginVertical();
            (this.target as Transform).localPosition = EditorGUILayout.Vector3Field("Local Position", (this.target as Transform).localPosition);
            (this.target as Transform).localRotation = Quaternion.Euler(EditorGUILayout.Vector3Field("Local Rotation", (this.target as Transform).localRotation.eulerAngles));
            (this.target as Transform).localScale = EditorGUILayout.Vector3Field("Local Scale", (this.target as Transform).localScale);
            EditorGUILayout.EndVertical();


            if (DragAndDrop.objectReferences.Length > 0 && AssetDatabase.GetAssetPath(DragAndDrop.objectReferences[0]).EndsWith(".fs"))
            {
                var id = EditorGUIUtility.GetControlID(FocusType.Passive);
                Event currentEvent = Event.current;

                switch (currentEvent.type)
                {
                    case EventType.Layout:
                        EditorGUIUtility.hotControl = id;
                        break;
                    case EventType.DragUpdated:
                    case EventType.DragPerform:
                        var didAcceptDrag = false;
                        
                        DragAndDrop.visualMode = DragAndDropVisualMode.Copy;
                        if (currentEvent.type == EventType.DragPerform)
                        {
                            didAcceptDrag = true;
                            DragAndDrop.activeControlID = 0;
                            DragAndDropFile(DragAndDrop.objectReferences);
                        }
                        else
                            DragAndDrop.activeControlID = id;
                        if (didAcceptDrag)
                        {
                            DragAndDrop.AcceptDrag();
                        }
                        break;
                }
            }
        }

        void DragAndDropFile(IEnumerable<UnityEngine.Object> draggedObjects)
        {
            if (draggedObjects == null) return;

            var dropTarget = this.target as Transform;

            foreach (var draggedObject in draggedObjects)
            {

                var outputPath = FSharpOption.fsharpBinPath;
                if (!Directory.Exists(outputPath))
                {
                    EditorUtility.DisplayDialog("Warning", "F# Assembly is not found.\nPlease Build.", "OK");
                    break;
                }


                var notfound = true;
                foreach (var dll in Directory.GetFiles(outputPath, "*.dll"))
                {
                    var fileName = Path.GetFileName(dll);
                    if (fileName == "FSharp.Core.dll") continue;

                    var assem = Assembly.LoadFrom(dll);
                    IEnumerable<Type> behaviors = null;
                    switch (FSharpOption.GetOptions().assemblySearch)
                    {
                        case AssemblySearch.Simple:
                            var @namespace = GetNameSpace(AssetDatabase.GetAssetPath(draggedObject));
                            var typeName = GetTypeName(AssetDatabase.GetAssetPath(draggedObject));
                            behaviors = assem.GetTypes().Where(type => typeof(MonoBehaviour).IsAssignableFrom(type) && type.FullName == @namespace + typeName);
                            break;
                        case AssemblySearch.CompilerService:
                            var types = GetTypes(AssetDatabase.GetAssetPath(draggedObject));
                            behaviors = assem.GetTypes().Where(type => typeof(MonoBehaviour).IsAssignableFrom(type) && types.Contains(type.FullName));
                            break;
                        default:
                            break;
                    }

                    if (behaviors != null && behaviors.Any())
                    {
                        DragAndDrop.AcceptDrag();
                        foreach (var behavior in behaviors)
                        {
                            dropTarget.gameObject.AddComponent(behavior);
                            notfound = false;
                        }
                    }
                }

                if (notfound)
                {
                    EditorUtility.DisplayDialog("Warning", "MonoBehaviour is not found in the F # assembly.", "OK");
                    return;
                }
            }
        }

        private string GetNameSpace(string path)
        {
            var @namespace = "";
            using (var sr = new StreamReader(path, new UTF8Encoding(false)))
            {
                var text = sr.ReadToEnd();
                string pattern = @"(?<![/]{2,})[\x01-\x7f]*namespace[\s]*(?<ns>.*?)\n";

                var re = new Regex(pattern, RegexOptions.IgnoreCase | RegexOptions.Singleline);
                foreach (Match m in re.Matches(text))
                {
                    @namespace = m.Groups["ns"].Value.Trim() != "" ? m.Groups["ns"].Value.Trim() + "." : "";
                    break;
                }
            }
            return @namespace;
        }

        private string GetTypeName(string path)
        {
            var typeName = "";
            using (var sr = new StreamReader(path, new UTF8Encoding(false)))
            {
                var text = sr.ReadToEnd();
                string pattern = @"(?<![/]{2,}\s{0,})type[\s]*(?<type>.*?)(?![^\({0,}])";
                var re = new Regex(pattern);
                foreach (Match m in re.Matches(text))
                {
                    typeName = m.Groups["type"].Value.Trim();
                    break;
                }
            }
            return typeName;
        }

        private string[] GetTypes(string path)
        {
            var path2 = PathUtil.GetAbsolutePath(Application.dataPath, path);
            var p = new Process();
            p.StartInfo.FileName = FSharpOption.projectRootPath + @"GN_merge.exe";
            p.StartInfo.Arguments = path2 + " " + "DEBUG";
            p.StartInfo.CreateNoWindow = true;
            p.StartInfo.UseShellExecute = false;
            p.StartInfo.RedirectStandardOutput = true;
            p.Start();
            p.WaitForExit();
            var outputString = p.StandardOutput.ReadToEnd();
            var types = outputString.Split(new string[] { Environment.NewLine }, StringSplitOptions.RemoveEmptyEntries);
            return types;
        }
    }
}
