﻿#define USE
#if USE
using System;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using UnityEditorInternal;
using System.Diagnostics;
using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using System.Reflection;

namespace UniFSharp
{
    [CustomEditor(typeof(Transform))]
    public class FSharpTransformInspector : UnityEditor.Editor
    {
        // inspector vars
        static Type decoratedEditorType;
        UnityEditor.Editor EDITOR_INSTANCE;


        public override void OnInspectorGUI()
        {
            if (targets == null || targets.Length == 0)
            {
                base.DrawDefaultInspector();
                return;
            }
            if (decoratedEditorType == null) decoratedEditorType = System.Reflection.Assembly.GetAssembly(typeof(UnityEditor.Editor)).GetType("UnityEditor.TransformInspector", true);
            if (!EDITOR_INSTANCE) EDITOR_INSTANCE = UnityEditor.Editor.CreateEditor(targets, decoratedEditorType);

            GUILayout.BeginVertical();

            //////// INTERNAL GUI ////////
            EDITOR_INSTANCE.OnInspectorGUI();

            // RIGHT PADDING
            GUILayout.EndVertical(); 

            /*  int controlID = GUIUtility.GetControlID(FocusType.Passive);
            if (Event.current.Equals(Event.KeyboardEvent("W")))
            {   Debug.Log(Event.current.GetTypeForControl(controlID));
            }
            if (Input.GetKey(KeyCode.C))
            {   Debug.Log(Event.current.GetTypeForControl(controlID));
            }
            if (Event.current.type == EventType.Repaint)
            {   var os = SnapMod.sceneKeyCode;
                SnapMod.sceneKeyCode = KeyCode.None;
                // if (os != KeyCode.None ) SnapMod. modifierKeysChanged_KEYS();
            }*/


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
                    switch (FSharpOptionStorage.GetOptions().assemblySearch)
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
#endif


