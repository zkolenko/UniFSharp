using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{

    [CustomEditor(typeof(UnityEditor.DefaultAsset))]
    public class FSharpScriptInspector : Editor
    {
        private string code;
        void OnEnable()
        {
            Repaint();
        }
        
        protected override bool ShouldHideOpenButton()
        {
            if (!AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".fs"))
            {
                return base.ShouldHideOpenButton();
            }
            else
            {
                return true;
            }
        }

        protected override void OnHeaderGUI()
        {
            if (!AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".fs"))
            {
                base.OnHeaderGUI();
            }
            else
            {
                base.OnHeaderGUI();
                var rec = EditorGUILayout.BeginHorizontal();
                if (GUI.Button(new Rect(rec.width - 160, 25, 155, 25), "open visual studio"))
                {
                    var path = AssetDatabase.GetAssetPath(Selection.activeObject);
                    var basePath = FSharpProject.GetProjectRootPath();
                    var fileName = PathUtil.GetAbsolutePath(basePath, path);
                    FSharpSolution.OpenExternalVisualStudio(fileName);
                }
                EditorGUILayout.EndHorizontal();

            }

        }
        
        
        public override void OnInspectorGUI()
        {
            GUI.enabled = true;

            if (!AssetDatabase.GetAssetPath(Selection.activeObject).EndsWith(".fs"))
            {
                DrawDefaultInspector();
            }
            else
            {
                EditorGUILayout.BeginHorizontal("box");
                GUIStyle boldtext = new GUIStyle();
                boldtext.fontStyle = FontStyle.Bold;
                boldtext.normal.textColor = Color.white;
                EditorGUILayout.LabelField("Imported F# Script", boldtext);
                EditorGUILayout.EndHorizontal();

                var targetAssetPath = AssetDatabase.GetAssetPath(target);
                if (File.Exists(targetAssetPath))
                {
                    var sr = File.OpenText(targetAssetPath);
                    code = sr.ReadToEnd();
                    sr.Close();

                    GUIStyle myStyle = new GUIStyle();
                    GUIStyle style = EditorStyles.textField;
                    myStyle.border = style.border;
                    myStyle.contentOffset = style.contentOffset;
                    myStyle.normal.background = style.normal.background;
                    myStyle.padding = style.padding;
                    myStyle.wordWrap = true;
                    myStyle.normal.textColor = Color.white;
                    EditorGUILayout.LabelField(code, myStyle);
                }
            }
        }



    }
}