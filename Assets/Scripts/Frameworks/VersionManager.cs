using UnityEngine;
using System;
using System.IO;
using System.Collections;
using System.Collections.Generic;
using System.Text;

//  Created by CaoChunYang 

public class VersionManager : GTMonoSingleton<VersionManager> 
{
	static public string Resource_Version = "1";

	static public string VersionFile = "Version.txt";
	static public string LatestVersionFile = "LatestVersion.txt";  //最新资源 版本文件名  用于查找本地最新资源


	static public string LocalResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return "file://" + Application.dataPath + "/StreamingAssets/Resources/";
			#elif UNITY_IPHONE
			return "file://" + Application.dataPath + "/Raw/Resources/";
			#elif UNITY_ANDROID
			return "jar:file://" + Application.dataPath + "!/assets/Resources/";
			#endif
		}
	}

	static public string LatestResUrl{
		get{
			#if UNITY_STANDALONE_WIN || UNITY_EDITOR  
			return Application.persistentDataPath + "/Resources/";   
			#elif UNITY_IPHONE
			return Application.persistentDataPath + "/Resources/";   
			#elif UNITY_ANDROID
			return Application.persistentDataPath + "/Resources/"; 
			#endif
		}
	}

	static public string ServerResUrl = "";     

	[HideInInspector]
	public EventDelegate.Callback CheckVersionFinish;

	static public Dictionary<string , string> LocalResVersion = new Dictionary<string, string>();   //本地所有最新资源 版本  用于远程比较
	static public Dictionary<string , string> LatestResVersion = new Dictionary<string, string>();  //本地所有安装后更新资源 版本 用于查找本地最新资源(未包括本次登录更新)
	static public Dictionary<string , string> ServerResVersion = new Dictionary<string, string>();  //远程最新资源 版本 
	static public Dictionary<string , string> UpdateResVersion = new Dictionary<string, string>();  //本次登录更新资源 版本 用于查找本地最新资源

	static public string GetResPath(string filePath)
	{
		#if UNITY_STANDALONE_WIN || UNITY_EDITOR 
		return "file://" + Application.streamingAssetsPath + "/Resources/" + filePath;
		#else
		if(UpdateResVersion.ContainsKey(filePath) || LatestResVersion.ContainsKey(filePath))
		{
			return "file://" + LatestResUrl + filePath;
		}
		if(LocalResVersion.ContainsKey(filePath))
		{
			return LocalResUrl + filePath; 
		}
		return ServerResUrl + filePath;
		#endif
	}
} 