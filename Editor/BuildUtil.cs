using System.IO;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.Text.RegularExpressions;

namespace EasyCoroutine.Editor
{
    public static class BuildUtil
    {
#if EASYCOROUTINE_EDITOR
        [MenuItem("BuildUtil/BuildAssetBundles")]
#endif
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

        private static Regex m_escapeFileExtension = new Regex(@"\.[a-z]+$", RegexOptions.IgnoreCase);
        private static void WalkDirectoryForAssetBundleBuilds(DirectoryInfo dirInfo, List<AssetBundleBuild> list, string basePath)
        {
            foreach (FileInfo f in dirInfo.GetFiles())
            {
                if (f.FullName.EndsWith(".meta"))
                    continue;
                var build = new AssetBundleBuild()
                {
                    assetBundleName = string.Format("{0}.asset", m_escapeFileExtension.Replace(f.Name, "")),
                    assetNames = new string[] { $"{basePath}{Path.DirectorySeparatorChar}{f.Name}" },
                };
                list.Add(build);
            }
            foreach (DirectoryInfo d in dirInfo.GetDirectories())
                WalkDirectoryForAssetBundleBuilds(d, list, basePath + Path.DirectorySeparatorChar + d.Name);
        }

#if EASYCOROUTINE_EDITOR
        [MenuItem("BuildUtil/ScanManifestFile")]
#endif
        public static void ScanManifestFile()
        {
            var reg = new Regex(@"[^\/\\]+$");
            var matcher = reg.Match(Application.streamingAssetsPath);
            var manifestPath = Path.Combine(Application.streamingAssetsPath, matcher.ToString());
            Debug.Log(manifestPath);
            AssetBundle bundle = null;
            try
            {
                bundle = AssetBundle.LoadFromFile(Path.Combine(Application.streamingAssetsPath, matcher.ToString()));
                if (bundle) {
                    foreach (var str in bundle.GetAllAssetNames())
                        Debug.Log(str);
                }
                var manifest = bundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
                foreach (var name in manifest.GetAllAssetBundles())
                    Debug.Log($"{name},{manifest.GetAssetBundleHash(name)}");
            }
            finally
            {
                bundle?.Unload(true);
            }
        }
    }
}