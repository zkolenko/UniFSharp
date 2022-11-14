using System;
using System.Linq;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{
    public class FSharpBuildToolsWindow : EditorWindow
    {
        //public CustomSections section;
        public GUIStyle preferencesSectionStyle;

        private static FSharpOption fsharpOption = null;
        public static FSharpOption FSharpOption
        {
            get
            {
                if (fsharpOption == null)
                {
                    fsharpOption = SerializerUtil.Load(fsharpOption);
                }
                return fsharpOption;
            }
        }

        public static void SetWindowSize(EditorWindow window)
        {
            var width = EditorPrefs.GetFloat("UnityEditor.PreferencesWindoww", 400);
            var height = EditorPrefs.GetFloat("UnityEditor.PreferencesWindowh", 200);
            window.minSize = new Vector2(width, height);
        }
        static void Initialize()
        {
            fsharpOption = SerializerUtil.Load(FSharpBuildToolsWindow.FSharpOption);
        }


        public static FSharpBuildToolsWindow OpenWindow()
        {
            var window = UnityEditor.EditorWindow.GetWindow<FSharpBuildToolsWindow>(true, "UniFSharp" + " - F# Build Tools for Unity");
            FSharpBuildToolsWindow.Initialize();
            var preferencesSectionStyle = new GUIStyle();
            window.preferencesSectionStyle = preferencesSectionStyle;
            SetWindowSize(window);
            return window;
        }

        [MenuItem("UniFSharp" + "/Option %&O", false, 70)]
        public static void ShowWindow()
        {
            OpenWindow();
        }
        void OnGUI()
        {
            DrawSectionBox();
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

        public void DrawSectionBox()
        {
            var option = FSharpBuildToolsWindow.FSharpOption;
            if (option == null) { option = new FSharpOption(); }
            VsVersion beforeVersion = option.vsVersion;
            option.vsVersion = this.EnumPopupAsAliasName<VsVersion>("Visual Studio", option.vsVersion);
            if (beforeVersion != option.vsVersion)
            {
                SerializerUtil.Save(option);
            }

            GUILayout.Box("", GUILayout.Width(this.position.width - 130), GUILayout.Height(1));

            var beforenetFramework = option.netFramework;
            option.netFramework = this.EnumPopupAsAliasName<NetFramework>(".NET Framework", option.netFramework);
            if (beforenetFramework != option.netFramework)
            {
                SerializerUtil.Save(option);
            }

            var beforeassemblyName = option.assemblyName;
            option.assemblyName = EditorGUILayout.TextField("AssemblyName", option.assemblyName);
            if (beforeassemblyName != option.assemblyName)
            {
                SerializerUtil.Save(option);
            }

            var beforerootNameSpace = option.rootNameSpace;
            option.rootNameSpace = EditorGUILayout.TextField("RootNamespace", option.rootNameSpace);
            if (beforerootNameSpace != option.rootNameSpace)
            {
                SerializerUtil.Save(option);
            }

            var beforeassemblyNameEditor = option.assemblyNameEditor;
            option.assemblyNameEditor = EditorGUILayout.TextField("AssemblyName(Editor)", option.assemblyNameEditor);
            if (beforeassemblyNameEditor != option.assemblyNameEditor)
            {
                SerializerUtil.Save(option);
            }

            var beforerootNameSpaceEditor = option.rootNameSpaceEditor;
            option.rootNameSpaceEditor = EditorGUILayout.TextField("RootNamespace(Editor)", option.rootNameSpaceEditor);
            if (beforerootNameSpaceEditor != option.rootNameSpaceEditor)
            {
                SerializerUtil.Save(option);
            }
            
            var beforeaddCSharpProjectsToAssembly = option.addCSharpProjectsToAssembly;
            option.addCSharpProjectsToAssembly = EditorGUILayout.Toggle("Add CSharp projects", option.addCSharpProjectsToAssembly);
            if (beforeaddCSharpProjectsToAssembly != option.addCSharpProjectsToAssembly)
            {
                SerializerUtil.Save(option);
            }


            GUILayout.Box("", GUILayout.Width(this.position.width - 130), GUILayout.Height(1));
            var beforebuildLogConsoleOutput = option.buildLogConsoleOutput;
            option.buildLogConsoleOutput = EditorGUILayout.Toggle("Build Log Output", option.buildLogConsoleOutput);
            if (beforebuildLogConsoleOutput != option.buildLogConsoleOutput)
            {
                SerializerUtil.Save(option);
            }

            var beforeassemblySearch = option.assemblySearch;
            option.assemblySearch = this.EnumPopupAsAliasName<AssemblySearch>("Assembly Search", option.assemblySearch);
            if (beforeassemblySearch != option.assemblySearch)
            {
                SerializerUtil.Save(option);
            }
        }
    }
}



