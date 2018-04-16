//  Copyright (c) 2016-present amlovey
//  
#if UNITY_EDITOR
using System;
using System.IO;
using UnityEditor;
using UnityEditor.Callbacks;
using UnityEngine;
using System.Collections.Generic;

namespace APlus
{
    public static class AssetsUsageChecker
    {
        private const string ASSETCHECKERKEY = "ANYPERF_ASSETCHECKERKEY";

        [PostProcessBuild(100)]
        public static void OnPostProcessBuild(BuildTarget target, string pathToBuildTarget)
        {
            var key = GetUniqueAssetCheckerKey();
            EditorPrefs.SetString(GetUniqueAssetCheckerKey(), key);
        }

        public static List<string> Check()
        {
            string editorLogPath = GetEdtiorFilePath();
            if (string.IsNullOrEmpty(editorLogPath) || !File.Exists(editorLogPath))
            {
                return null;
            }

            string logContent = ReadLogContent(editorLogPath);
            if (string.IsNullOrEmpty(logContent))
            {
                return null;
            }

            string[] lines = logContent.Split(new char[] { '\r', '\n' }, StringSplitOptions.RemoveEmptyEntries);

            bool inRegion = false;
            int startIndex = lines.Length;

            for (int i = lines.Length - 1; i >= 0; i--)
            {
                if (lines[i].Trim().EndsWith("sorted by uncompressed size:"))
                {
                    inRegion = true;
                    startIndex = i;
                    break;
                }
            }

            List<string> usedFiles = new List<string>();

            for (int i = startIndex + 1; i < lines.Length; i++)
            {
                if (!inRegion)
                {
                    break;
                }

                if (char.IsDigit(lines[i].Trim(), 0))
                {
                    string assetsPath = GetAssetPath(lines[i]);
                    if (!string.IsNullOrEmpty(assetsPath))
                    {
                        usedFiles.Add(assetsPath);
                    }
                }
                else
                {
                    inRegion = false;
                    break;
                }
            }

            return usedFiles;
        }

        public static string GetUniqueAssetCheckerKey()
        {
#if UNITY_5 && !UNITY_5_6
            return string.Format("{0}_{1}", Application.productName, Application.bundleIdentifier, ASSETCHECKERKEY);
#else
            return string.Format("{0}_{1}", Application.productName, Application.identifier, ASSETCHECKERKEY);
#endif
        }

        private static string GetAssetPath(string log)
        {
            // 0.1 kb	 0.0% Assets/Test/SampleScenes/Scripts/CameraSwitch.cs
            //
            string[] temp = log.Split(new char[] { ' ' }, 4, StringSplitOptions.RemoveEmptyEntries);
            if (temp.Length == 4)
            {
                return temp[3];
            }

            return string.Empty;
        }

        private static string ReadLogContent(string logPath)
        {

#if UNITY_EDITOR_WIN
            // Will happen "IOException: Sharing violation on path" on Windows,
            // so just copy a backup file and read it for workaroud solution.
            //
            string destFile = logPath + "backup";
            File.Copy(logPath, destFile, true);
#else
            string destFile = logPath;
#endif
            string data = string.Empty;

            using (FileStream fs = new FileStream(destFile, FileMode.Open))
            {
                using (StreamReader reader = new StreamReader(fs))
                {
                    data = reader.ReadToEnd();
                }
            }

#if UNITY_EDITOR_WIN
            File.Delete(destFile);
#endif

            return data;
        }

        public static string GetEdtiorFilePath()
        {
            string editorLogPath = "Editor.log";

            if (Application.platform == RuntimePlatform.OSXEditor)
            {
                string unityLogFolder = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.Personal), "Library/Logs/Unity");
                editorLogPath = Path.Combine(unityLogFolder, editorLogPath);
            }
            else if (Application.platform == RuntimePlatform.WindowsEditor)
            {
                editorLogPath = Path.Combine(Environment.ExpandEnvironmentVariables(@"%localappdata%\Unity\Editor\"), editorLogPath);
            }
            else
            {
                Debug.LogError("Not support by A+ Assets Explorer for now!");
                return null;
            }

            return editorLogPath;
        }
    }
}
#endif