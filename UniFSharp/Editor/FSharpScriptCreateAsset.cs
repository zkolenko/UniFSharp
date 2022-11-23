using System.IO;
using System.Text.RegularExpressions;
using System.Text;
using UnityEditor;
using UnityEditor.ProjectWindowCallback;
using UnityEngine;
using System;

namespace UniFSharp
{
    public class FSharpScriptCreateAsset : EndNameEditAction
    {
        public override void Action(int instanceId, string pathName, string resourceFile)
        {
            StreamWriter sw;
            using (var sr = new StreamReader(resourceFile, new UTF8Encoding(false)))
            {
                using (sw = File.CreateText(pathName))
                {
                    var filename = Path.GetFileNameWithoutExtension(pathName).Replace(" ", "");
                    var text = Regex.Replace(sr.ReadToEnd(), "#ClassName#", filename)
                        .Replace("#ModuleName#", filename)
                        .Replace("#NameSpace#", FSharpOption.GetOptions().rootName)
                        .Replace("#Guid#", System.Guid.NewGuid().ToString());
                    sw.Write(text);
                    AssetDatabase.ImportAsset(pathName);
                    var uo = AssetDatabase.LoadAssetAtPath(pathName, typeof(UnityEngine.Object));
                    ProjectWindowUtil.ShowCreatedAsset(uo);
                }
            }
        }

        static public void CreateFSharpScript(string fileName)
        {
            var tempFilePath = FSharpOption.fsharpScriptTemplatePath() + fileName + FSharpOption.txtExtension;
            CreateScript(fileName, tempFilePath);
        }
        static public void CreateScript(string defaultName, string templatePath)
        {
            string directoryName;
            var assetPath = AssetDatabase.GetAssetPath(Selection.activeObject);
            if (String.IsNullOrEmpty(Path.GetExtension(assetPath)))
            {
                directoryName = assetPath;
            }
            else
            {
                directoryName = PathUtil.GetDirectoryName(assetPath);
            }
            Texture2D icon = (Texture2D)AssetDatabase.LoadAssetAtPath(FSharpOption.fsharpIconPath, typeof(Texture2D));
            ProjectWindowUtil.StartNameEditingIfProjectWindowExists(0, ScriptableObject.CreateInstance<FSharpScriptCreateAsset>(), defaultName, icon, templatePath);
        }
    }
}
