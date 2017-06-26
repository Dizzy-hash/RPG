using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.SceneManagement;

public class ResourceManager
{
    public delegate void LoadSceneComplete();
    public delegate void LoadPrefabComplete(GameObject o);
    public delegate void LoadSoundbComplete(AudioClip a);
    public delegate void LoadConfigComplete(string str);
    public delegate void LoadPictureComplete(Texture2D t);

    public delegate void LoadProgress(float p);

    IEnumerator LoadAssets(string strName , LoadProgress loadProgress , LoadSceneComplete loadComplete)
    {
        WWW www = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath("StreamingAssets"), 0);
        yield return www;
        List<AssetBundle> assetBundleList = new List<AssetBundle>();
        AssetBundle assetBundle = www.assetBundle;
        assetBundleList.Add(assetBundle);
        AssetBundleManifest abManifest = assetBundle.LoadAsset<AssetBundleManifest>("AssetBundleManifest");
        string[] assetDepends = abManifest.GetAllDependencies(strName.ToLower());
        for(int i = 0; i < assetDepends.Length; i ++)
        {
            WWW dependWWW = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath(assetDepends[i]), 0);
            yield return dependWWW;
            assetBundleList.Add(dependWWW.assetBundle);
            if(loadProgress != null) loadProgress((i + 1) * 0.02f);
        }

        WWW asset = WWW.LoadFromCacheOrDownload(VersionManager.GetResPath(strName.ToLower()), 0);
        while (!asset.isDone)
        {
            yield return null;
            loadProgress(assetDepends.Length * 0.2f + asset.progress * 0.3f);
        }
        yield return asset;
        AssetBundle assetAB = asset.assetBundle;
        assetBundleList.Add(assetAB);
        float p = assetDepends.Length * 0.2f + 0.3f;

        AsyncOperation asyn = SceneManager.LoadSceneAsync(strName);
        while (!asyn.isDone)
        {
            yield return null;
            loadProgress(p + asyn.progress * (1 - p));
        }

        if (loadComplete != null) loadComplete();

        foreach (var ab in assetBundleList)
        {
            if (ab != null)
            {
                ab.Unload(false);
            }
        }
    }
}
