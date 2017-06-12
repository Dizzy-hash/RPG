using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.IO;
using System.Text;

public class GameUpdateState : GameBaseState
{
    private List<string> NeedDownFiles = new List<string>();
    private int NeedDownCount = 0;
    private bool NeedUpdateLocalVersionFile = false;

    public UILoading mLoadingWindow;

    public override void Enter()
    {
        base.Enter();
        mLoadingWindow = (UILoading)GTWindowManager.Instance.OpenWindow(EWindowID.UI_LOADING);
        CheckVersion();
    }

    public override void Execute()
    {

    }

    public override void Exit()
    {

    }

    public void CheckVersion()
    {
        mLoadingWindow.UpdateDesc("检查资源版本...");
        return;
        CopyLocalVersionFileToPersistent();
    }


    void CopyLocalVersionFileToPersistent()
    {
        if (true)
        {
            GTCoroutinueManager.Instance.StartCoroutine(DownLoad(VersionManager.LocalResUrl + "ConfigAssets.zip", delegate (WWW configZip)
            {
                ZipFile.UnZip(VersionManager.LatestResUrl, configZip.bytes);
                LoadVersion();
            }));
        }
        else
        {
            LoadVersion();
        }
    }

    void LoadVersion()
    {
        if (File.Exists(VersionManager.LatestResUrl + VersionManager.LatestVersionFile))
        {
            GTCoroutinueManager.Instance.StartCoroutine(DownLoad("file://" + VersionManager.LatestResUrl + VersionManager.LatestVersionFile, delegate (WWW latestVersion)
            {
                ParseVersionFile(latestVersion.bytes, VersionManager.LatestResVersion);
                LoadVersion_();
            }));
        }
        else
        {
            LoadVersion_();
        }
    }

    void LoadVersion_()
    {
        GTCoroutinueManager.Instance.StartCoroutine(DownLoad("file://" + VersionManager.LatestResUrl + VersionManager.VersionFile, delegate (WWW localVersion)
        {
            ParseVersionFile(localVersion.bytes, VersionManager.LocalResVersion);
            GTCoroutinueManager.Instance.StartCoroutine(this.DownLoad(VersionManager.ServerResUrl + VersionManager.VersionFile + "?" + VersionManager.Resource_Version, delegate (WWW serverVersion)
            {
                ParseVersionFile(serverVersion.bytes, VersionManager.ServerResVersion);
                if (System.Convert.ToInt32(VersionManager.LocalResVersion["VersionNumber"]) > System.Convert.ToInt32(VersionManager.ServerResVersion["VersionNumber"]))
                {
                   
                }
                else
                {

                    CompareVersion();
                    NeedDownCount = NeedDownFiles.Count;
                    if (NeedDownCount > 0)
                    {

                    }
                    DownLoadRes();
                }
            }));
        }));
    }

    void DownLoadRes()
    {
        if (NeedDownFiles.Count == 0)
        {
            UpdateLocalVersionFile();
            return;
        }

        string file = NeedDownFiles[0];
        NeedDownFiles.RemoveAt(0);

        GTCoroutinueManager.Instance.StartCoroutine(this.DownLoad(VersionManager.ServerResUrl + file + "?" + VersionManager.UpdateResVersion[file], delegate (WWW w)
        {
            ReplaceLocalRes(VersionManager.LatestResUrl + file, w.bytes);
            DownLoadRes();
        }));
    }

    void UpdateLocalVersionFile()
    {
        if (NeedUpdateLocalVersionFile)
        {
            StringBuilder versions = new StringBuilder();
            foreach (var item in VersionManager.ServerResVersion)
            {
                versions.Append(item.Key).Append("|").Append(item.Value).Append("\n");
            }
            SaveVersionFile(VersionManager.LatestResUrl + VersionManager.VersionFile, versions);

            StringBuilder versions_ = new StringBuilder();
            foreach (var item in VersionManager.LatestResVersion)
            {
                if (!VersionManager.UpdateResVersion.ContainsKey(item.Key))
                {
                    versions_.Append(item.Key).Append("|").Append(item.Value).Append("\n");
                }
            }
            foreach (var item in VersionManager.UpdateResVersion)
            {
                versions_.Append(item.Key).Append("|").Append(item.Value).Append("\n");
            }
            SaveVersionFile(VersionManager.LatestResUrl + VersionManager.LatestVersionFile, versions_);
        }

       
    }

    void ReplaceLocalRes(string path, byte[] data)
    {
        Debug.Log(path);
        path = path.Replace("file://", "");
        string directoryName = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream stream = File.Create(path);
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
    }

    void SaveVersionFile(string path, StringBuilder versions)
    {
        Debug.Log(path);
        path = path.Replace("file://", "");
        string directoryName = path.Substring(0, path.LastIndexOf("/"));
        if (!Directory.Exists(directoryName))
        {
            Directory.CreateDirectory(directoryName);
        }
        if (File.Exists(path))
        {
            File.Delete(path);
        }
        FileStream stream = new FileStream(path, FileMode.Create);
        byte[] data = Encoding.ASCII.GetBytes(versions.ToString());
        stream.Write(data, 0, data.Length);
        stream.Flush();
        stream.Close();
    }

    void CompareVersion()
    {
        foreach (var version in VersionManager.ServerResVersion)
        {
            string fileName = version.Key;
            string serverMd5 = version.Value.Replace("\r", string.Empty);
            if (fileName == "VersionNumber" || fileName == "FileCount" || fileName == "Version.txt") continue;
            if (!VersionManager.LocalResVersion.ContainsKey(fileName))
            {
                NeedDownFiles.Add(fileName);
                VersionManager.UpdateResVersion.Add(fileName, serverMd5);
                Debug.Log("new fileName " + fileName);
            }
            else
            {
                string localMd5;
                if (VersionManager.LocalResVersion.TryGetValue(fileName, out localMd5)) localMd5 = localMd5.Replace("\r", string.Empty);
                if (!serverMd5.Equals(localMd5))
                {
                    NeedDownFiles.Add(fileName);
                    VersionManager.UpdateResVersion.Add(fileName, serverMd5);
                    Debug.Log("fileName " + fileName + " MD5 -s " + serverMd5 + " -c " + localMd5);
                }
            }
        }
        NeedUpdateLocalVersionFile = NeedDownFiles.Count > 0;
    }

    void ParseVersionFile(byte[] bytes, Dictionary<string, string> dict)
    {
        dict.Clear();
        string content = Encoding.ASCII.GetString(bytes);
        if (content == null || content.Length == 0)
        {
            return;
        }
        string[] items = content.Split('\n');
        foreach (string item in items)
        {
            string[] info = item.Split('|');
            if (info != null && info.Length == 2)
            {
                if (info[0].IndexOf("VersionNumber") != -1)
                {
                    dict.Add("VersionNumber", info[1]);
                }
                else
                {
                    if (dict.ContainsKey(info[0]))
                    {
                        Debug.Log("有相同的Key " + info[0]);
                    }
                    else
                    {
                        dict.Add(info[0], info[1]);
                    }
                }
            }
        }
    }

    IEnumerator DownLoad(string url, HandleFinishDownload finishFun)
    {
        WWW www = new WWW(url);
        yield return www;
        if (finishFun != null)
        {
            finishFun(www);
        }
        www.Dispose();
    }

    public delegate void HandleFinishDownload(WWW www);
}
