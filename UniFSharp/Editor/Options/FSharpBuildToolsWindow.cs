using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{
    public enum CustomSections
    {
        [AliasName("Application dll")] applicationDll = 1,
        [AliasName("Assemblies dll")] asembliesDll = 2,
        [AliasName("Assets dll")] assetsDll = 3,
    }

    public class FSharpBuildToolsWindow : EditorWindow
    {
        struct FileData
        {
            public bool enabled;
            public string path;
            public string root;
            public FileData(bool enabled, string path, string root)
            {
                this.enabled = enabled;
                this.path = path;
                this.root = root;
            } 
            public string GetRelativePath()
            {
                return Path.GetRelativePath(root, path);
            }
            public string GetFileNameWithoutExtension()
            {
                return Path.GetFileNameWithoutExtension(path);
            }
            public string GetName()
            {
                return Path.GetFileName(path);
            }
            public void Update(bool enable, List<string> optionData)
            {
                string _path = path;
                this.enabled = enable;
                if (this.enabled && !optionData.Exists(x => x == _path))
                {
                    optionData.Add(_path);
                } else if (!this.enabled && optionData.Exists(x => x == _path))
                {
                    optionData.Remove(_path);
                }
            }
        }
        static public CustomSections section;
        public GUIStyle preferencesSectionStyle;

        private static FSharpOption option = null;

        Vector2 scrollDllsPosition = Vector2.zero;
        static FileData[] dllApplication = null;
        static FileData[] dllAssemblies = null;
        static FileData[] dllAsset = null;
        static FileData[] csProjects = null;


        [MenuItem("UniFSharp" + "/Option %&O", false, 70)]
        public static void ShowWindow()
        {
            option = FSharpOption.GetOptions();   
            
            var window = UnityEditor.EditorWindow.GetWindow<FSharpBuildToolsWindow>(true, "UniFSharp" + " - F# Build Tools for Unity");
            section = CustomSections.applicationDll;
            var preferencesSectionStyle = new GUIStyle();
            window.preferencesSectionStyle = preferencesSectionStyle;

            var width = EditorPrefs.GetFloat("UnityEditor.PreferencesWindoww", 400);
            var height = EditorPrefs.GetFloat("UnityEditor.PreferencesWindowh", 180);
            window.minSize = new Vector2(width, height);

            // dlls
            dllApplication = FSharpProject.GetApplicationDlls(false)
                .Select(x => {
                    bool exist = option.applicationDlls.Exists(y => y == x);
                    return new FileData(exist, x, FSharpOption.unityApplicationllPath);
                })
                .OrderBy(x => !x.enabled).ThenBy(x => x.GetRelativePath())
                .ToArray();
            option.applicationDlls = option.applicationDlls.Where(x => dllApplication.ToList().Exists(y => y.path == x)).ToList();

            var assDlls = FSharpOption.autoConnectAssembliesDll();
            dllAssemblies = Directory.GetFiles(FSharpOption.unityAssemblePath, "*.dll", SearchOption.AllDirectories)
                .Where(x => !assDlls.Contains(Path.GetFileName(x).ToLower()))
                .Select(x => {
                    bool exist = option.assemblieDlls.Exists(y => y == x);
                    return new FileData(exist, x, FSharpOption.unityAssemblePath);
                })
                .OrderBy(x => !x.enabled).ThenBy(x => x.GetRelativePath())
                .ToArray();
            option.assemblieDlls = option.assemblieDlls.Where(x => dllAssemblies.ToList().Exists(y => y.path == x)).ToList();

            dllAsset = Directory.GetFiles(FSharpOption.unityAssetsPath, "*.dll", SearchOption.AllDirectories)
                .Where(x => !x.Contains(FSharpOption.projectRootAbsolutePath))
                .Select(x => {
                    bool exist = option.assetDlls.Exists(y => y == x);
                    return new FileData(exist, x, FSharpOption.unityAssetsPath);
                })
                .OrderBy(x => !x.enabled).ThenBy(x => x.GetRelativePath())
                .ToArray();
            option.assetDlls = option.assetDlls.Where(x => dllAsset.ToList().Exists(y => y.path == x)).ToList();
        }

        void OnGUI()
        {
            if (option == null) ShowWindow();   
             
            EditorGUILayout.BeginVertical();
            DrawFSharpSection();
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

            EditorGUILayout.BeginHorizontal();
            // option section
            if (GUILayout.Button(new GUIContent("Application dll")))
            {
                section = CustomSections.applicationDll;
            }
            else if (GUILayout.Button(new GUIContent("Assemblies dll")))
            {
                section = CustomSections.asembliesDll;
            }
            else if (GUILayout.Button(new GUIContent("Assets dll")))
            {
                section = CustomSections.assetsDll;
            }
            EditorGUILayout.EndHorizontal();

            EditorGUILayout.Space(5);

            // dll section
            EditorGUILayout.BeginVertical();
            switch (section)
            {
                case CustomSections.applicationDll:
                    DrawDllSection(dllApplication, "Application dlls", option.applicationDlls);
                    break;
                case CustomSections.asembliesDll:
                    DrawDllSection(dllAssemblies, "Assemblie dlls", option.assemblieDlls);
                    break;
                case CustomSections.assetsDll:
                    DrawDllSection(dllAsset, "Assets dlls", option.assetDlls);
                    break;
                default:
                    break;
            }
            EditorGUILayout.EndVertical();

            EditorGUILayout.Space(5);

        }


        TEnum EnumPopupAsAliasName<TEnum>(string label, TEnum selectedValue) where TEnum : Enum
        {
            var v0 = Enum.GetValues(typeof(TEnum));
            var v1 = v0.Cast<TEnum>();
            var v2 = v1.Select(x => new GUIContent(AliasNameAttribute.ToAliasName<Enum, int>(x), AliasNameAttribute.ToAliasName<Enum, int>(x)));
            var displayOptions = v2.ToArray();
            var v3 = Enum.GetValues(typeof(TEnum));
            var v4 = v3.Cast<TEnum>();
            var v5 = v1.Select(x => (int)(x as object));
            var optionValues = v5.ToArray();
            var content = new GUIContent(label);
            var selection = EditorGUILayout.IntPopup(content, (int)(selectedValue as object), displayOptions, optionValues);
            return (TEnum)Enum.ToObject(typeof(TEnum), selection);
        }

        public void DrawFSharpSection()
        {
            GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));

            option.vsVersion = this.EnumPopupAsAliasName<VsVersion>("Visual studio version", option.vsVersion);

            option.netFramework = this.EnumPopupAsAliasName<NetFramework>(".NET Framework", option.netFramework);

            option.rootName = EditorGUILayout.TextField("Root name", option.rootName);

            option.buildLogConsoleOutput = EditorGUILayout.Toggle("Build Log Output", option.buildLogConsoleOutput);

            GUILayout.Box("", GUILayout.Width(this.position.width), GUILayout.Height(1));

        }

        void DrawDllSection(FileData[] dll, string label, List<string> optionData)
        {
            GUIStyle style = new GUIStyle();
            style.alignment = TextAnchor.MiddleCenter;
            style.normal.textColor = Color.green;
            
            EditorGUILayout.LabelField(new GUIContent(label), style);
            EditorGUILayout.BeginVertical(GUILayout.Width(EditorGUIUtility.currentViewWidth));

            scrollDllsPosition = EditorGUILayout.BeginScrollView(scrollDllsPosition, GUILayout.ExpandWidth(true), GUILayout.ExpandHeight(true));
            for (int i = 0; i < dll.Length; i++)
            {
                dll[i].Update(EditorGUILayout.ToggleLeft(dll[i].GetRelativePath(), dll[i].enabled), optionData);
            }
            EditorGUILayout.EndScrollView();

            EditorGUILayout.EndVertical();

        }

        public override void SaveChanges()
        {

            base.SaveChanges();
        }
        
    }
}



