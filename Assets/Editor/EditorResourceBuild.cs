using UnityEngine;
using System.Collections;
using System.Xml;
using System.IO;
using System.Collections.Generic;
using System;
using UnityEditor;

namespace EDT
{
    public class EditorResourceBuild
    {
        static string[] assetsPath = new string[] {
            "/ResourceAssets/Config/" , "*.xml",
            "/ResourceAssets/Config/" , "*.txt",
            "/ResourceAssets/Curve/" , "*.prefab",
            "/ResourceAssets/Effect/" , "*.prefab",
            "/ResourceAssets/Guis/" , "*.prefab",
            "/ResourceAssets/Icons/" , "*.png",
            "/ResourceAssets/Icons/" , "*.jpg",
            "/ResourceAssets/Map/" , "*.unity",
            "/ResourceAssets/Model/" , "*.prefab",
            "/ResourceAssets/Sound/" , "*.map3",
            "/ResourceAssets/Sound/" , "*.wav"
        };

        static BuildTarget buildTarget;

        [MenuItem("BuildAssets/BuildWin")]
        static public void BuildWin()
        {
            buildTarget = BuildTarget.StandaloneWindows64;
            Build();
        }

        [MenuItem("BuildAssets/BuildAndroid")]
        static public void BuildAndroid()
        {
            buildTarget = BuildTarget.Android;
            Build();
        }

        [MenuItem("BuildAssets/BuildIos")]
        static public void BuildIos()
        {
            buildTarget = BuildTarget.iOS;
            Build();
        }

        static void Build()
        {
            DelBundles();
            DelBundleNames();
            SetBundleConfig();
            SetBundleNames();
            BuildBundles();
        }

        
        static void DelBundles()
        {
            if (Directory.Exists(Application.streamingAssetsPath + "/resourceassets"))
            {
                Directory.Delete(Application.streamingAssetsPath + "/resourceassets", true);
            }
            Directory.CreateDirectory(Application.streamingAssetsPath + "/resourceassets");
        }

        [MenuItem("BuildAssets/Clear")]
        static public void DelBundleNames()
        {
            string[] bundles = AssetDatabase.GetAllAssetBundleNames();
            for (var i = 0; i < bundles.Length; i++)
            {
                AssetDatabase.RemoveAssetBundleName(bundles[i], true);
            }
        }

        static void SetBundleConfig()
        {
            List<GTResourceUnit> list = new List<GTResourceUnit>();

            for (int i = 0; i < assetsPath.Length / 2; i ++)
            {
                string[] files = Directory.GetFiles(Application.dataPath + assetsPath[i * 2], assetsPath[i * 2 + 1] , SearchOption.AllDirectories);
                foreach (string file in files)
                {
                    string filePath = file.Replace("\\" , "/");
                    string assetPath = filePath.Replace(Application.dataPath + "/", "");
                  
                    string extenName = System.IO.Path.GetExtension(filePath);
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(filePath);

                    assetPath = assetPath.Replace(fileName + extenName, "");

                    if (string.IsNullOrEmpty(extenName) || extenName == ".meta")
                    {
                        continue;
                    }
                 
                    GTResourceUnit bundle = new GTResourceUnit();
                    bundle.GUID = AssetDatabase.AssetPathToGUID("Assets/" + assetPath + fileName + extenName);
                    bundle.FilePath = "Assets" + filePath.Replace(Application.dataPath, "");

                    switch (extenName)
                    {
                        case ".unity":
                            {
                                bundle.AssetName = fileName;   
                                bundle.AssetBundleName = fileName + ".unity3d";
                                bundle.Path = assetPath;
                                SetDepends(bundle.FilePath , bundle.depends);
                            }
                            break;
                        case ".xml":
                        case ".txt":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = "ConfigAssets.assetbundle";
                                bundle.Path = "resourceassets/";
                            }
                            break;
                        case ".prefab":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = fileName + ".assetbundle";
                                bundle.Path = assetPath;
                                SetDepends(bundle.FilePath , bundle.depends);
                            }
                            break;
                        case ".mp3":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = fileName + ".assetbundle";
                                bundle.Path = assetPath;
                            }
                            break;
                        case ".png":
                        case ".jpg":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = "Icons.assetbundle";
                                bundle.Path = "resourceassets/";
                            }
                            break;
                        default:
                            {
                                continue;
                            }

                    }

                    bundle.AssetBundleName = bundle.AssetBundleName.ToLower();
                    list.Add(bundle);
                }
            }

            list.Sort((a1, a2) => { return a1.AssetName.CompareTo(a2.AssetName); });
            GTResourceManager.Instance.Units.Clear();
            foreach (var current in list)
            {
                GTResourceManager.Instance.Units[current.FilePath] = current;
            }

            XmlDocument doc = new XmlDocument();
            XmlNode root = doc.CreateElement("root");
            doc.AppendChild(root);
            foreach (var current in list)
            {
                GTResourceUnit bundle = current;
                XmlElement child = doc.CreateElement("row");
                root.AppendChild(child);
                child.SetAttribute("AssetName", bundle.AssetName);
                child.SetAttribute("AssetBundleName", bundle.AssetBundleName);
                child.SetAttribute("Path", bundle.Path);
                child.SetAttribute("GUID", bundle.GUID);
                for (int i = 0; i < bundle.depends.Count; i ++)
                {
                    XmlElement dp = doc.CreateElement("depends");
                    child.AppendChild(dp);
                    dp.SetAttribute("Depends" , bundle.depends[i]);
                }
            }

            string refName = Application.streamingAssetsPath + "/Asset.xml";
            FileStream fs = null;
            if (!File.Exists(refName))
            {
                fs = File.Create(refName);
            }
            doc.Save(refName);
            if (fs != null)
            {
                fs.Flush();
                fs.Dispose();
                fs.Close();
            }
        }

        static void SetDepends(string filePath , List<string> depends)
        {
            string[] dps = AssetDatabase.GetDependencies(filePath);
            for (int i = 0; i < dps.Length; i++)
            {
                if (dps[i] == filePath || dps[i].Contains(".cs")) continue;
                AssetImporter importer = AssetImporter.GetAtPath(dps[i]);
                string dpName = AssetDatabase.AssetPathToGUID(dps[i]);
                if(!depends.Contains(dpName))depends.Add(dpName);
                importer.assetBundleName = "resourceassets/alldependencies/" + dpName;
                SetDepends(dps[i] , depends);
            }
        }

        static void SetBundleNames()
        {
            Dictionary<string, GTResourceUnit> units = GTResourceManager.Instance.Units;
            foreach (var current in units)
            {
                GTResourceUnit bundle = current.Value;
                var assetImporter = AssetImporter.GetAtPath(bundle.FilePath);
                if (assetImporter != null)
                {
                    assetImporter.assetBundleName = bundle.Path + bundle.AssetBundleName.ToLower();
                }                
            }
        }

        static void BuildBundles()
        {
            BuildPipeline.BuildAssetBundles(Application.streamingAssetsPath , BuildAssetBundleOptions.UncompressedAssetBundle,EditorUserBuildSettings.activeBuildTarget);
            AssetDatabase.Refresh();
        }
    }
}
