using System;
using System.IO;
using UnityEngine;

namespace ScriptEditor
{
    /// <summary>
    /// File tool class
    /// </summary>
    public static class ScriptEditorFileTool
    {
        public static string GetProjectFolder()
        {
            return Path.GetDirectoryName(Application.dataPath);
        }

        public static string GetRelativePath(string path1, string path2)
        {
            Uri uri1 = new Uri(path1 + "/");
            Uri uri2 = new Uri(path2 + "/");
                
            return uri1.MakeRelativeUri(uri2).ToString();
        }

        public static string GetAbsoluteFileName(string relativePath, string productName)
        {
            return new FileInfo(Path.Combine(GetProjectFolder(), relativePath) + "/" + productName + ".sln").FullName;
        }
    }
}