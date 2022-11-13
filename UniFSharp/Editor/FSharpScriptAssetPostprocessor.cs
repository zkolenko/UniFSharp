using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{

    public class FSharpScriptAssetPostprocessor : AssetPostprocessor
    {


        static string[] getXDocCompileIncureds(XDocument fsprojXDoc, string ns, ProjectFileType projectFileType)
        {
            var elements = fsprojXDoc.Root.Elements(XName.Get(ns + "ItemGroup")).Elements(XName.Get(ns + "Compile"));
            var res = elements.Select(x => PathUtil.ReplaceDirSepFromAltSep(x.Attribute(XName.Get("Include")).Value)).ToArray();
            return res;
        }

        static XElement getNewCompileIncludeElement(string ns, string file)
        {
            var res = new XElement(XName.Get(ns + "Compile"), new XAttribute(XName.Get("Include"), file));
            return res;
        }

        static XElement getNewItemGroupCompileIncludeElement(string ns, string file)
        {
            var res = new XElement(XName.Get(ns + "ItemGroup"), new XElement(XName.Get(ns + "Compile"), new XAttribute(XName.Get("Include"), file)));
            return res;
        }
        static XElement[] getXDocComiles(XDocument fsprojXDoc, string ns)
        {
            var res = fsprojXDoc.Root.Elements(XName.Get(ns + "ItemGroup")).Elements(XName.Get(ns + "Compile")).ToArray();
            return res;
        }

        static string[] getNotExitsFiles(string[] compileIncludes, ProjectFileType projectFileType)
        {
            var basePath = FSharpProject.GetProjectRootPath();
            var files = FSharpProject.GetAllFSharpScriptAssets(projectFileType);
            files = files.Select(x => PathUtil.GetRelativePath(basePath, x));
            List<string> res = new List<string>();
            foreach (var _file in files)
            {
                var file = PathUtil.ReplaceDirSepFromAltSep(_file);
                if (compileIncludes.Any(x => x == file) == false)
                {
                    res.Add(file);
                }
            }
            return res.ToArray();
        }


        static void addCompileIncludeFiles(XDocument fsprojXDoc, string ns, string[] compileIncludes, ProjectFileType projectFileType)
        {
            var notExists = getNotExitsFiles(compileIncludes, projectFileType);
            foreach (var file in notExists)
            {
                var newElem = getNewCompileIncludeElement(ns, file);
                var compiles = getXDocComiles(fsprojXDoc, ns);
                if (compiles.Any())
                {
                    XElement[] directoryPoint = compiles.Where(x =>
                    {
                        var includeFile = x.Attribute(XName.Get("Include")).Value;
                        var includeDirectory = PathUtil.ReplaceDirSepFromAltSep(PathUtil.GetDirectoryName(includeFile));
                        var directory = PathUtil.ReplaceDirSepFromAltSep(PathUtil.GetDirectoryName(file));
                        return includeDirectory == directory;
                    }).ToArray();
                    if (directoryPoint.Any())
                    {
                        directoryPoint.Last().AddAfterSelf(newElem);
                    }
                    else
                    {
                        compiles.Last().AddAfterSelf(newElem);
                    }
                }
                else
                {
                    var newItemGroupElem = getNewItemGroupCompileIncludeElement(ns, file);
                    fsprojXDoc.Root.Add(newItemGroupElem);
                }
            }
        }

        static string[] getRemoveFiles(string[] compileIncludes, ProjectFileType projectFileType)
        {
            var basePath = FSharpProject.GetProjectRootPath();
            List<string> res = new List<string>();
            foreach (var _include in compileIncludes)
            {
                var include = PathUtil.ReplaceDirSepFromAltSep(_include);
                var files = FSharpProject.GetAllFSharpScriptAssets(projectFileType);
                files = files.Select(x => PathUtil.GetRelativePath(basePath, x));
                files = files.Select(x => PathUtil.ReplaceDirSepFromAltSep(x));
                if (files.Contains(include) == false)
                {
                    res.Add(include);
                }
            }
            return res.ToArray();

        }


        static void removeCompileIncludeFiles(XDocument fsprojXDoc, string ns, string[] compileIncludes, ProjectFileType projectFileType)
        {
            var removedFiles = getRemoveFiles(compileIncludes, projectFileType);
            foreach (var file in removedFiles)
            {
                var compileItems = (fsprojXDoc.Root.Elements(XName.Get(ns + "ItemGroup")).Elements(XName.Get(ns + "Compile")));
                if (compileItems.Count() == 1 && compileItems.Any(x => x.Attribute(XName.Get("Include")).Value == file))
                {
                    var parent = compileItems.Select(x => x.Parent).First();
                    parent.Remove();
                }
                else
                {
                    compileItems.Where(x => x.Attribute(XName.Get("Include")).Value == file).Remove();
                }
            }
        }

        static void createOrUpdateProject(ProjectFileType projectFileType)
        {
            var fsprojFileName = FSharpProject.GetFSharpProjectFileName(projectFileType);
            var fsprojFilePath = FSharpProject.GetFSharpProjectPath(fsprojFileName);
            if (File.Exists(fsprojFilePath) == false)
            {
                FSharpProject.CreateFSharpProjectFile(projectFileType);
            }
            else
            {
                var fsprojXDoc = XDocument.Load(fsprojFilePath);
                var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
                var compileIncludes = getXDocCompileIncureds(fsprojXDoc, ns, projectFileType);
                addCompileIncludeFiles(fsprojXDoc, ns, compileIncludes, projectFileType);
                removeCompileIncludeFiles(fsprojXDoc, ns, compileIncludes, projectFileType);
                fsprojXDoc.Save(fsprojFilePath);
            }
        }

        static void deleteProject(ProjectFileType projectFileType, string _assetPath)
        {
            var assetPath = PathUtil.ReplaceDirSepFromAltSep(_assetPath);
            var fsprojFileName = FSharpProject.GetFSharpProjectFileName(projectFileType);
            if (File.Exists(fsprojFileName))
            {
                var basePath = FSharpProject.GetProjectRootPath();
                var fsprojXDoc = XDocument.Load(fsprojFileName);
                var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
                var _compileIncludes = fsprojXDoc.Root
                                      .Elements(XName.Get(ns + "ItemGroup"))
                                      .Elements(XName.Get(ns + "Compile"));
                var compileIncludes = _compileIncludes.Select(x => x.Attribute(XName.Get("Include")).Value);
                compileIncludes = compileIncludes.Select(x => PathUtil.ReplaceDirSepFromAltSep(x));
                fsprojXDoc.Root
                    .Elements(XName.Get(ns + "ItemGroup"))
                    .Elements(XName.Get(ns + "Compile"))
                    .Where(x => PathUtil.ReplaceDirSepFromAltSep(x.Attribute(XName.Get("Include")).Value) == assetPath).Remove();
                fsprojXDoc.Save(fsprojFileName);
            }
        }






        static void createOrUpdateEditor()
        {
            createOrUpdateProject(ProjectFileType.VisualStudioEditor);
        }

        static void createOrUpdateNormal()
        {
            createOrUpdateProject(ProjectFileType.VisualStudioNormal);
        }

        static void createOrUpdate()
        {
            createOrUpdateNormal();
            createOrUpdateEditor();
        }

        static string[] filterFSharpScript(string[] x)
        {
            var res = x.Where(assetPath => Path.GetExtension(assetPath) == FSharpBuildTools.fsExtension).ToArray();
            return res;
        }

        static void onImportedAssets(string[] _importedAssets)
        {
            var importedAssets = filterFSharpScript(_importedAssets);
            if (importedAssets.Length > 0)
            {
                createOrUpdate();
            }
            FSharpSolution.CreateVisualStudioSolutionFile();
        }
        static void onDeletedAssets(string[] _deletedAssets)
        {
            var deletedAssets = filterFSharpScript(_deletedAssets);
            foreach (var assetPath in deletedAssets)
            {
                if (FSharpProject.ContainsEditorFolder(assetPath))
                {
                    deleteProject(ProjectFileType.VisualStudioEditor, assetPath);
                }
                else
                {
                    deleteProject(ProjectFileType.VisualStudioNormal, assetPath);
                }
            }
        }

        static void onMovedAssets(string[] _movedAssets)
        {
            var movedAssets = filterFSharpScript(_movedAssets);
            foreach (var assetPath in movedAssets)
            {
                var assetAbsolutePath = (PathUtil.GetAbsolutePath(Application.dataPath, assetPath));
                var fileName = Path.GetFileName(assetAbsolutePath);
                if (FSharpProject.FsharpScriptCreatable(assetAbsolutePath) == false)
                {
                    EditorUtility.DisplayDialog("Warning", "Folder name that contains the F# Script file,\n must be unique in the entire F# Project.\nMove to Assets Folder.", "OK");
                    AssetDatabase.MoveAsset(assetPath, "Assets/" + fileName);
                    AssetDatabase.Refresh(ImportAssetOptions.ForceUpdate);
                }

            }
        }

        static void onMovedFromPathAssets(string[] movedFromPath)
        {
            if (filterFSharpScript(movedFromPath).Any())
            {
                createOrUpdateNormal();
            }

        }

        static void OnPostprocessAllAssets(string[] importedAssets, string[] deletedAssets, string[] movedAssets, string[] movedFromPath)
        {
            onImportedAssets(importedAssets);
            onDeletedAssets(deletedAssets);
            onMovedAssets(movedAssets);
            onMovedFromPathAssets(movedFromPath);
        }

    }
}