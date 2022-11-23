using GluonGui.WorkspaceWindow.Views.WorkspaceExplorer;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Xml;
using System.Xml.Linq;
using UnityEditor;
using UnityEngine;

namespace UniFSharp
{

    public class FSharpScriptAssetPostprocessor : AssetPostprocessor
    {


        public static XElement getXDocElement(XDocument fsprojXDoc, string ns, string elementName, string label)
        {
            var elements = getXDocElements(fsprojXDoc, ns, elementName, label);
            if (elements.Count() > 0)
            {
                return elements.Take(1).Single();
            }
            else
            {
                var res = getNewItemGroupElement(ns, label);
                fsprojXDoc.Root.Add(res);
                return res;
            }
        }
        public static IEnumerable<XElement> getXDocElements(XDocument fsprojXDoc, string ns, string elementName, string label = null)
        {
            var elements = fsprojXDoc.Root.Elements(XName.Get(ns + elementName)).ToArray();
            if (label != null)
            {
                elements = elements.Where(x =>
                {
                    var labelAttribute = x.Attribute(XName.Get("Label"));
                    return (labelAttribute != null) ? labelAttribute.Value.ToLower().Contains(label.ToLower()) : false;
                }).ToArray();
            }
            return elements;
        }
        public static IEnumerable<XElement> getXDocCompiles(XDocument fsprojXDoc, string ns, string label = null)
        {
            var elements = getXDocElements(fsprojXDoc, ns, "ItemGroup", label);
            elements = elements.Elements(XName.Get(ns + "Compile")).ToArray();
            return elements;
        }
        public static string[] getXDocCompileIncludes(XDocument fsprojXDoc, string ns, string label = null)
        {
            var elements = getXDocCompiles(fsprojXDoc, ns, label);
            var res = elements.Select(x => PathUtil.ReplaceDirSepFromAltSep(x.Attribute(XName.Get("Include")).Value)).ToArray();
            return res;
        }

        static XElement getNewCompileIncludeElement(string ns, string file)
        {
            var res = new XElement(XName.Get(ns + "Compile"), new XAttribute(XName.Get("Include"), file));
            return res;
        }

        public static XElement getNewItemGroupElement(string ns, string label = null)
        {
            if (label == null)
            {
                var res = new XElement(XName.Get(ns + "ItemGroup"));
                return res;
            }
            else
            {
                var res = new XElement(XName.Get(ns + "ItemGroup"), new XAttribute(XName.Get("Label"), label));
                return res;
            }
        }
        static XElement getNewItemGroupCompileIncludeElement(string ns, string file, string label = null)
        {
            if (label == null)
            {
                var res = new XElement(XName.Get(ns + "ItemGroup"), new XElement(XName.Get(ns + "Compile"), new XAttribute(XName.Get("Include"), file)));
                return res;
            }
            else
            {
                var res = new XElement(XName.Get(ns + "ItemGroup"), new XAttribute(XName.Get("Label"), label), new XElement(XName.Get(ns + "Compile"), new XAttribute(XName.Get("Include"), file)));
                return res;
            }
        }

        static string[] getNotExitsFiles(string[] compileIncludes)
        {
            var basePath = FSharpProject.GetProjectRootPath();
            var files = FSharpProject.GetAllFSharpScriptAssets();
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


        public static void addCompileIncludeFiles(XDocument fsprojXDoc, string ns, string[] compileIncludes, string label = null)
        {
            var notExists = getNotExitsFiles(compileIncludes);
            XElement itemGroup = getXDocElements(fsprojXDoc, ns, "ItemGroup", label).DefaultIfEmpty(null).FirstOrDefault(x => x != null);
            if (itemGroup == null)
            {
                itemGroup = getNewItemGroupElement(ns, label);
                fsprojXDoc.Root.Add(itemGroup);
            }
            foreach (var file in notExists)
            {
                var newElem = getNewCompileIncludeElement(ns, file);
                var compiles = getXDocCompiles(fsprojXDoc, ns, label);
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
                    itemGroup.AddFirst(newElem);
                }
            }
        }

        static string[] getRemoveFiles(string[] compileIncludes)
        {
            var basePath = FSharpProject.GetProjectRootPath();
            List<string> res = new List<string>();
            foreach (var _include in compileIncludes)
            {
                var include = PathUtil.ReplaceDirSepFromAltSep(_include);
                var files = FSharpProject.GetAllFSharpScriptAssets();
                files = files.Select(x => PathUtil.GetRelativePath(basePath, x));
                files = files.Select(x => PathUtil.ReplaceDirSepFromAltSep(x));
                if (files.Contains(include) == false)
                {
                    res.Add(include);
                }
            }
            return res.ToArray();

        }


        static void removeCompileIncludeFiles(XDocument fsprojXDoc, string ns, string[] compileIncludes, string label = null)
        {
            var removedFiles = getRemoveFiles(compileIncludes);
            foreach (var file in removedFiles)
            {
                var compileItems = getXDocCompiles(fsprojXDoc, ns, label);
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
        static void addReferenceIncludeDll(XElement itemGroup, string ns, string[] dllFiles)
        {
            foreach (var dllPath in dllFiles)
            {
                var dllName = Path.GetFileName(dllPath).Replace(".dll", "");
                var privateElement = new XElement(XName.Get(ns + "Private"), false);
                var HitPathElement = new XElement(XName.Get(ns + "HintPath"), dllPath);
                var res = new XElement(XName.Get(ns + "Reference"), new XAttribute(XName.Get("Include"), dllName));
                res.Add(HitPathElement);
                res.Add(privateElement);
                itemGroup.Add(res);
            }
        }


        public static void updateCompileIncludeDlls(bool updateAplicationDll = false, bool updateAssemblieDll = false, bool updateAssetDll = false)
        {
            FSharpProject.ChekProjectFile();

            var fsprojXDoc = XDocument.Load(FSharpOption.assemblyFileNamePath());
            var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
            var option = FSharpOption.GetOptions();

            if (updateAplicationDll)
            {
                // update application dlls
                var applicationDlls = getXDocElement(fsprojXDoc, ns, "ItemGroup", "ApplicationDlls");
                applicationDlls.RemoveNodes();
                var dlls = FSharpProject.GetApplicationDlls(true).ToList();
                dlls.AddRange(option.applicationDlls);
                addReferenceIncludeDll(applicationDlls, ns, dlls.ToArray());
            }
            if (updateAssemblieDll)
            {
                // update assemblie dlls
                var scriptAssembliesDlls = getXDocElement(fsprojXDoc, ns, "ItemGroup", "ScriptAssemblieDlls");
                scriptAssembliesDlls.RemoveNodes();
                var dlls = FSharpProject.GetAssemblieDlls(true).ToList();
                dlls.AddRange(option.assemblieDlls);
                addReferenceIncludeDll(scriptAssembliesDlls, ns, dlls.ToArray());
            }
            if (updateAssetDll)
            {
                // update assemblie dlls
                var scriptAssetDlls = getXDocElement(fsprojXDoc, ns, "ItemGroup", "ScriptAssetDlls");
                scriptAssetDlls.RemoveNodes();
                addReferenceIncludeDll(scriptAssetDlls, ns, option.assetDlls.ToArray());
            }

            fsprojXDoc.Save(FSharpOption.assemblyFileNamePath());
        }

        public static void createOrUpdateProject()
        {
            FSharpProject.ChekProjectFile();

            var fsprojXDoc = XDocument.Load(FSharpOption.assemblyFileNamePath());
            var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
            string labelItems = "FSharpItems";
            string[] compileIncludes = getXDocCompileIncludes(fsprojXDoc, ns, labelItems);
            addCompileIncludeFiles(fsprojXDoc, ns, compileIncludes, labelItems);
            removeCompileIncludeFiles(fsprojXDoc, ns, compileIncludes, labelItems);
            fsprojXDoc.Save(FSharpOption.assemblyFileNamePath());
        }

        static void deleteProject(string _assetPath)
        {
            var assetPath = PathUtil.ReplaceDirSepFromAltSep(_assetPath);
            var fsprojFileName = FSharpOption.assemblyFileName;
            if (File.Exists(fsprojFileName))
            {
                var basePath = FSharpProject.GetProjectRootPath();
                var fsprojXDoc = XDocument.Load(fsprojFileName);
                var ns = "{" + String.Format("{0}", fsprojXDoc.Root.Attribute(XName.Get("xmlns")).Value) + "}";
                var _compileIncludes = getXDocCompiles(fsprojXDoc, ns, "FSharpItems");
                var compileIncludes = _compileIncludes.Select(x => x.Attribute(XName.Get("Include")).Value);
                compileIncludes = compileIncludes.Select(x => PathUtil.ReplaceDirSepFromAltSep(x));
                _compileIncludes.Where(x => PathUtil.ReplaceDirSepFromAltSep(x.Attribute(XName.Get("Include")).Value) == assetPath).Remove();
                fsprojXDoc.Save(fsprojFileName);
            }
        }



        static string[] filterFSharpScript(string[] x)
        {
            var res = x.Where(assetPath => Path.GetExtension(assetPath) == FSharpOption.fsExtension).ToArray();
            return res;
        }

        static void onImportedAssets(string[] _importedAssets)
        {
            var importedAssets = filterFSharpScript(_importedAssets);
            if (importedAssets.Length > 0)
            {
                createOrUpdateProject();
            }
            FSharpSolution.CreateVisualStudioSolutionFile();
        }
        static void onDeletedAssets(string[] _deletedAssets)
        {
            var deletedAssets = filterFSharpScript(_deletedAssets);
            foreach (var assetPath in deletedAssets)
            {
                deleteProject(assetPath);
            }
        }

        static void onMovedAssets(string[] _movedAssets)
        {
            var movedAssets = filterFSharpScript(_movedAssets);
            foreach (var assetPath in movedAssets)
            {
                var assetAbsolutePath = (PathUtil.GetAbsolutePath(Application.dataPath, assetPath));
                var fileName = Path.GetFileName(assetAbsolutePath);
            }
        }

        static void onMovedFromPathAssets(string[] movedFromPath)
        {
            if (filterFSharpScript(movedFromPath).Any())
            {
                createOrUpdateProject();
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