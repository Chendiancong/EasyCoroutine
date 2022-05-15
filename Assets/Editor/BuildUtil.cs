using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;

namespace Editor
{
    public static class BuildUtil
    {
        [MenuItem("BuildUtil/BuildAssetBundles")]
        public static void BuildAssetBundls()
        {
            string sourcePath = string.Format("{0}/PackAssets", Application.dataPath);
            string targetPath = Application.streamingAssetsPath;
            if (!Directory.Exists(sourcePath))
                Directory.CreateDirectory(sourcePath);
            List<AssetBundleBuild> list = new List<AssetBundleBuild>();
            WalkDirectoryForAssetBundleBuilds(new DirectoryInfo(sourcePath), list, $"Assets{Path.DirectorySeparatorChar}PackAssets");
            if (Directory.Exists(targetPath))
                Directory.Delete(targetPath, true);
            Directory.CreateDirectory(targetPath);
            foreach (var b in list)
            {
                Debug.Log(b.assetBundleName);
                Debug.Log(b.assetNames[0]);
            }
            BuildPipeline.BuildAssetBundles(
                targetPath,
                list.ToArray(),
                BuildAssetBundleOptions.None,
                BuildTarget.StandaloneWindows
            );
        }

        private static void WalkDirectoryForAssetBundleBuilds(DirectoryInfo dirInfo, List<AssetBundleBuild> list, string basePath)
        {
            foreach (FileInfo f in dirInfo.GetFiles())
            {
                if (f.FullName.EndsWith(".meta"))
                    continue;
                var build = new AssetBundleBuild()
                {
                    assetBundleName = string.Format("{0}.asset", f.Name),
                    assetNames = new string[] { $"{basePath}{Path.DirectorySeparatorChar}{f.Name}" },
                };
                list.Add(build);
            }
            foreach (DirectoryInfo d in dirInfo.GetDirectories())
                WalkDirectoryForAssetBundleBuilds(d, list, basePath + Path.DirectorySeparatorChar + d.Name);
        }
    }
}