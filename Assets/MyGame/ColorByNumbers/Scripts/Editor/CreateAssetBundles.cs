using System.IO;
using UnityEditor;
using UnityEngine;

public class CreateAssetBundles
{
    [MenuItem("Assets/Build AssetBundles")]
    static void BuildAllAssetBundles()
    {
        string assetBundleDirectory = Path.Combine(Application.persistentDataPath, "StreamingAssets");
        if (!Directory.Exists(Application.streamingAssetsPath))
        {
            Directory.CreateDirectory(assetBundleDirectory);
        }
        BuildPipeline.BuildAssetBundles(assetBundleDirectory, BuildAssetBundleOptions.None, EditorUserBuildSettings.activeBuildTarget);
    }
}