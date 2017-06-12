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
        static string OUTPUTPATH = Application.streamingAssetsPath + "/Resoucrces";

        static string[] assetsPath = new string[] {
            "/Res/Map/" , ".unity"
        };

        public static void Build()
        {
            DelBundles();
            DelBundleNames();
            SetBundleConfig();
            SetBundleNames();
            BuildBundles();
        }

        static void DelBundles()
        {
            if (Directory.Exists(OUTPUTPATH))
            {
                Directory.Delete(OUTPUTPATH, true);
            }
            Directory.CreateDirectory(OUTPUTPATH);
        }

        static void DelBundleNames()
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
                string[] files = Directory.GetFiles(Application.dataPath + assetsPath[i * 2], "*" + assetsPath[i * 2 + 1]);
                foreach (string file in files)
                {
                    UnityEngine.Object obj;
                    string assetPath = "Assets" + file.Replace(Application.dataPath , "");
                    string extenName = System.IO.Path.GetExtension(file).ToLower();
                    string fileName = System.IO.Path.GetFileNameWithoutExtension(file).ToLower();
                    if (string.IsNullOrEmpty(extenName) || extenName == ".meta")
                    {
                        continue;
                    }
                    GTResourceUnit bundle = new GTResourceUnit();
                    switch (extenName)
                    {
                        case ".unity":
                            {
                                bundle.AssetName = fileName;
                                bundle.AssetBundleName = fileName + ".unity3d";
                                bundle.Path = assetPath;
                                bundle.GUID = AssetDatabase.AssetPathToGUID(assetPath);
                            }
                            break;
                        case ".xml":
                        case ".txt":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = fileName + extenName + ".assetbundle";
                                bundle.Path = assetPath;
                                bundle.GUID = AssetDatabase.AssetPathToGUID(bundle.Path);
                            }
                            break;
                        case ".prefab":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = GTTools.GetParentPathName(assetPath) + ".pre.assetbundle";
                                bundle.Path = assetPath;
                                bundle.GUID = AssetDatabase.AssetPathToGUID(bundle.Path);
                            }
                            break;
                        case ".mp3":
                            {
                                bundle.AssetName = fileName + extenName;
                                bundle.AssetBundleName = fileName + extenName + ".assetbundle";
                                bundle.Path = assetPath;
                                bundle.GUID = AssetDatabase.AssetPathToGUID(bundle.Path);
                            }
                            break;
                        case ".png":
                            {
                                if (assetPath.Contains("Image"))
                                {
                                    bundle.AssetName = fileName + extenName;
                                    bundle.AssetBundleName = GTTools.GetParentPathName(assetPath) + ".atlas.assetbundle";
                                    bundle.Path = assetPath;
                                    bundle.GUID = AssetDatabase.AssetPathToGUID(bundle.Path);
                                }
                                if (assetPath.Contains("T_Background"))
                                {
                                    bundle.AssetName = fileName + extenName;
                                    bundle.AssetBundleName = fileName + ".tex.assetbundle";
                                    bundle.GUID = AssetDatabase.AssetPathToGUID(bundle.Path);
                                }
                            }
                            break;

                    }
                    if (string.IsNullOrEmpty(bundle.AssetName))
                    {
                        continue;
                    }
                    bundle.AssetBundleName = bundle.AssetBundleName.ToLower();
                    list.Add(bundle);
                }
            }

            list.Sort((a1, a2) => { return a1.AssetName.CompareTo(a2.AssetName); });
            GTResourceManager.Instance.Units.Clear();
            foreach (var current in list)
            {
                GTResourceManager.Instance.Units[current.Path] = current;
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

        static void SetBundleNames()
        {
            Dictionary<string, GTResourceUnit> units = GTResourceManager.Instance.Units;
            foreach (var current in units)
            {
                GTResourceUnit bundle = current.Value;
                var assetImporter = AssetImporter.GetAtPath(bundle.Path);
                if (assetImporter != null)
                {
                    assetImporter.assetBundleName = bundle.AssetBundleName.ToLower();
                }
            }
        }

        static void BuildBundles()
        {
            BuildPipeline.BuildAssetBundles(OUTPUTPATH, BuildAssetBundleOptions.UncompressedAssetBundle,EditorUserBuildSettings.activeBuildTarget);
        }
    }
}
