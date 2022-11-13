using System.IO;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{

    [CustomEditor(typeof(UnityEngine.Object), true)]
    public class FSharpScriptInspector : Editor
    {
        private string code;
        void OnEnable()
        {
            Repaint();
        }

        public override void OnInteractivePreviewGUI(Rect r, GUIStyle background)
        {
            base.OnInteractivePreviewGUI(r, background);
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
                if (!Directory.Exists(targetAssetPath) && File.Exists(targetAssetPath))
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