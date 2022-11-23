using System;
using System.IO;


namespace UniFSharp
{
    public static class PathUtil
    {
        public static string GetDirectoryName(string path)
        {
            var directoryName = Path.GetDirectoryName(path);
            return (directoryName == null) ? "" : directoryName;
        }
        public static string ReplaceDirSepFromAltSep(string path)
        {
            if (path == null)
            {
                return path;
            }
            else
            {
                return path.Replace(Path.AltDirectorySeparatorChar, Path.DirectorySeparatorChar);
            }
        }
        public static string ReplaceDirAltSepFromSep(string path)
        {
            if (path == null)
            {
                return path;
            }
            else
            {
                return path.Replace(Path.DirectorySeparatorChar, Path.AltDirectorySeparatorChar);
            }
        }
        public static string AppendDirSep(string _path)
        {
            string path = ReplaceDirSepFromAltSep(_path);
            string path_ = ReplaceDirSepFromAltSep(GetDirectoryName(path));
            if (path != path_)
            {
                return path + Path.DirectorySeparatorChar;
            }
            else { return path; }
        }
        public static string RemoveDirSep(string _path)
        {
            string r = ReplaceDirSepFromAltSep(_path);
            if (r.Substring(r.Length - 1, 1) == ("" + Path.DirectorySeparatorChar))
            {
                return r.Substring(0, r.Length - 1);
            }
            else
            {
                return r;
            }
        }
        public static string GetCurrentFolderName(string _path)
        {
            string path = ReplaceDirSepFromAltSep(GetDirectoryName(_path));
            int index = path.LastIndexOf(Path.DirectorySeparatorChar);
            if (index > 0)
            {
                return path.Substring(index + 1, _path.Length - index - 1);
            }
            else { throw new Exception("NotFound"); }
        }

        private static string GetUpDirectoryRec(string _filePath, int n)
        {
            if (n - 1 < 0)
            {
                return _filePath;
            }
            else
            {
                string filePath = ReplaceDirSepFromAltSep(_filePath);
                int index = filePath.LastIndexOf("" + Path.DirectorySeparatorChar);
                if (index < 0)
                {
                    return filePath;
                }
                else
                {
                    var dinfo = Directory.GetParent(filePath);
                    string result = (dinfo == null) ? "" : dinfo.FullName;
                    if (n - 1 == 0)
                    {
                        return result;
                    }
                    else
                    {
                        return GetUpDirectoryRec(result, (n - 1));
                    }
                }
            }
        }
        public static string GetUpDirectory(int count, string filePath)
        {
            var res = GetUpDirectoryRec(filePath, count);
            return RemoveDirSep(ReplaceDirSepFromAltSep(res));
        }

        public static string GetRelativePath(string _basePath, string _targetPath)
        {
            var basePath = ReplaceDirSepFromAltSep(_basePath);
            var targetPath = ReplaceDirSepFromAltSep(_targetPath);
            basePath = basePath.Replace("%", "%25");
            var filePath = targetPath.Replace("%", "%25");
            var u1 = new Uri(basePath);
            var u2 = new Uri(targetPath);
            var relativeUri = u1.MakeRelativeUri(u2);
            var relativePath = Uri.UnescapeDataString(relativeUri.ToString());
            return relativePath.Replace("%25", "%");
        }
        public static string GetAbsolutePath(string _basePath, string _targetPath)
        {
            var basePath = ReplaceDirSepFromAltSep(_basePath);
            var targetPath = ReplaceDirSepFromAltSep(_targetPath);
            basePath = basePath.Replace("%", "%25");
            var filePath = targetPath.Replace("%", "%25");
            var u1 = new Uri(basePath);
            var u2 = new Uri(u1, targetPath);
            var absolutePath = (u2.LocalPath).Replace("%25", "%");
            return absolutePath;
        }
    } 
}