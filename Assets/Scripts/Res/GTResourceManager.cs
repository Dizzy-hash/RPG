using UnityEngine;
using System.Collections;
using System.Collections.Generic;
using System.Xml;
using System.IO;
using LitJson;

public class GTResourceManager : GTMonoSingleton<GTResourceManager>
{
    public Dictionary<string, GTResourceUnit>   Units    = new Dictionary<string, GTResourceUnit>();//以路径为Key
    public Dictionary<string, GTResourceBundle> Bundles  = new Dictionary<string, GTResourceBundle>();
    public EResourceLoadType                    Type     = EResourceLoadType.TYPE_SOURCE;

    public string GetBundlePersistentPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return Application.persistentDataPath;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return Application.dataPath.Replace("/Assets", string.Empty) + "/AssetBundles";
            default:
                return Application.dataPath.Replace("/Assets", string.Empty) + "/AssetBundles";
        }
    }

    public string GetBundleStreamingPath()
    {

        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return Application.streamingAssetsPath;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return Application.dataPath.Replace("/Assets", string.Empty) + "/AssetBundles";
            default:
                return Application.dataPath.Replace("/Assets", string.Empty) + "/AssetBundles";
        }

    }

    public string GetWWWPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.IPhonePlayer:
            case RuntimePlatform.Android:
                return "file:///";
            default:
                return string.Empty;
        }
    }

    public string GetAssetConfigPath()
    {
        switch (Application.platform)
        {
            case RuntimePlatform.Android:
            case RuntimePlatform.IPhonePlayer:
                return Application.persistentDataPath;
            case RuntimePlatform.WindowsEditor:
            case RuntimePlatform.WindowsPlayer:
                return Application.streamingAssetsPath;
            default:
                return Application.streamingAssetsPath;
        }
    }

    public string GetExtPath()
    {
        string path = string.Empty;
        switch (Application.platform)
        {
            case RuntimePlatform.WindowsEditor:
                path = Application.streamingAssetsPath;
                break;
            case RuntimePlatform.Android:
                path = Application.persistentDataPath;
                break;
            case RuntimePlatform.IPhonePlayer:
                path = Application.persistentDataPath;
                break;
            default:
                path = Application.streamingAssetsPath;
                break;

        }
        return path;
    }

    public string GetDataPath()
    {
        return Application.streamingAssetsPath;
    }

    void LoadConfig()
    {
        string fsPath = GetAssetConfigPath() + "/Asset.xml";
        StreamReader fs = new StreamReader(fsPath);
        XmlDocument doc = new XmlDocument();
        doc.LoadXml(fs.ReadToEnd());
        XmlNodeList list = doc.SelectSingleNode("root").ChildNodes;
        foreach (var current in list)
        {
            XmlElement element = current as XmlElement;
            if (element == null)
            {
                continue;
            }
            GTResourceUnit u = new GTResourceUnit();
            for (int i = 0; i < element.Attributes.Count; i++)
            {
                XmlAttribute attr = element.Attributes[i];
                switch (attr.Name)
                {
                    case "AssetBundleName":
                        u.AssetBundleName = attr.Value;
                        break;
                    case "AssetName":
                        u.AssetName = attr.Value;
                        break;
                    case "Path":
                        u.Path = attr.Value;
                        break;
                    case "GUID":
                        u.GUID = attr.Value;
                        break;
                }
            }
            Units[u.Path] = u;

            GTResourceBundle bundle = null;
            Bundles.TryGetValue(u.AssetBundleName, out bundle);
            if (bundle == null)
            {
                bundle = new GTResourceBundle();
                bundle.AssetBundleName = u.AssetBundleName;
                Bundles.Add(u.AssetBundleName, bundle);
            }
        }
        fs.Dispose();
        fs.Close();
    }

    void LoadBundle<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
    {
        GTResourceUnit unit = null;
        Units.TryGetValue(path, out unit);
        if (unit == null)
        {
            return;
        }
        GTResourceBundle bundle = null;
        Bundles.TryGetValue(unit.AssetBundleName, out bundle);
        if (bundle == null)
        {
            return;
        }
        AddLoadBundleTask<T>(unit.AssetName, bundle, callback, null);
    }

    void LoadSource<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
    {
#if UNITY_EDITOR
        if (string.IsNullOrEmpty(path))
        {
            return;
        }
        T asset = UnityEditor.AssetDatabase.LoadAssetAtPath<T>(path);
        if (asset == null)
        {
            return;
        }
        if (callback != null)
        {
            callback.Invoke(asset);
        }
#endif
    }

    void AddLoadBundleTask<T>(string assetName, GTResourceBundle bundle, System.Action<T> callback, GTResourceTask parentTask) where T : UnityEngine.Object
    {
        GTResourceTask task = new GTResourceTask();
        task.Bundle = bundle;
        task.LoadedDepCount = 0;
        task.Parent = parentTask;
        task.AssetName = assetName;
        StartCoroutine(LoadAsyncBundle(task, callback));
    }

    IEnumerator LoadAsyncManifest(System.Action callback)
    {
        string url = string.Format("{0}{1}/{2}", GetWWWPath(), GetBundleStreamingPath(), "AssetBundles");
        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        AssetBundleManifest maniFest = (AssetBundleManifest)ab.LoadAsset("AssetBundleManifest");
        ab.Unload(false);
        foreach (var current in maniFest.GetAllAssetBundles())
        {
            string[] deps = maniFest.GetAllDependencies(current);
            if (deps == null || deps.Length == 0)
            {
                continue;
            }
            GTResourceBundle bundle = null;
            Bundles.TryGetValue(current, out bundle);
            if (bundle == null)
            {
                continue;
            }
            bundle.Deps.AddRange(deps);
        }
        if (callback != null)
        {
            callback.Invoke();
        }
        www.Dispose();
        yield return null;
    }

    IEnumerator LoadAsyncBundle<T>(GTResourceTask task, System.Action<T> callback) where T : UnityEngine.Object
    {
        GTResourceBundle bundleData = task.Bundle;
        string url = string.Format("{0}{1}/{2}", GetWWWPath(), GetBundlePersistentPath(), bundleData.AssetBundleName);
        WWW www = new WWW(url);
        yield return www;
        if (!string.IsNullOrEmpty(www.error))
        {
            Debug.LogError(www.error);
            yield break;
        }
        AssetBundle ab = www.assetBundle;
        if (ab == null)
        {
            Debug.LogError(string.Format("assetBundle is null with data=" + JsonMapper.ToJson(bundleData)));
            yield break;
        }

        for (int i = 0; i < bundleData.Deps.Count; i++)
        {
            string depAbName = bundleData.Deps[i];
            GTResourceBundle dpBundle = null;
            Bundles.TryGetValue(depAbName, out dpBundle);
            if (dpBundle == null)
            {
                continue;
            }
            AddLoadBundleTask<UnityEngine.Object>(null, dpBundle, null, task);
        }

        AssetBundleRequest req = null;
        if (task.AssetName != null)
        {
            req = ab.LoadAssetAsync<T>(task.AssetName);
        }
        else
        {
            req = ab.LoadAllAssetsAsync();
        }
        while (!req.isDone)
        {
            yield return null;
        }

        if (task.Parent != null)
        {
            task.Parent.LoadedDepCount++;
        }

        while (task.LoadedDepCount < bundleData.Deps.Count)
        {
            yield return null;
        }

        if (req.asset == null)
        {
            Debug.LogError(ab);
            yield break;
        }
        if (callback != null)
        {
            callback.Invoke(req.asset as T);
        }

        ab.Unload(false);
        www.Dispose();
        yield return 0;
    }

    public void LoadAsset<T>(string path, System.Action<T> callback) where T : UnityEngine.Object
    {
        switch (Type)
        {
            case EResourceLoadType.TYPE_BUNDLE:
                LoadBundle(path, callback);
                break;
            case EResourceLoadType.TYPE_SOURCE:
                LoadSource(path, callback);
                break;
        }
    }

    public T Load<T>(string path, bool instance = false) where T : UnityEngine.Object
    {
        T asset = Resources.Load<T>(path) as T;
        if (asset != null && instance)
        {
            return UnityEngine.Object.Instantiate(asset);
        }
        return asset;
    }

    public GameObject Instantiate(string path, Vector3 position, Quaternion rotation)
    {
        GameObject asset = Load<GameObject>(path);
        GameObject go = null;
        if (asset != null)
        {
            go = (GameObject)GameObject.Instantiate(asset, position, rotation);
        }
        return go;
    }

    public GameObject Instantiate(string path)
    {
        GameObject asset = Load<GameObject>(path);
        GameObject go = null;
        if (asset != null)
        {
            go = (GameObject)GameObject.Instantiate(asset);
        }
        return go;
    }

    public void Destroy(GameObject go)
    {
        GameObject.Destroy(go);
    }

    public void DestroyObj(UnityEngine.Object obj)
    {
        if (obj != null)
        {
            UnityEngine.Object.Destroy(obj);
        }
    }

    public void LoadFromStreamingAssets(string sPath, string pPath)
    {
        string targetPath = Application.persistentDataPath + sPath;
        string sourcePath = Application.streamingAssetsPath + pPath;
        if (Application.platform == RuntimePlatform.Android)
        {
            if (File.Exists(targetPath))
            {
                File.Delete(targetPath);
            }
            WWW www = new WWW(sourcePath);
            bool boo = true;
            while (boo)
            {
                if (www.isDone)
                {
                    File.WriteAllBytes(targetPath, www.bytes);
                    boo = false;
                }
            }
        }
    }
}
