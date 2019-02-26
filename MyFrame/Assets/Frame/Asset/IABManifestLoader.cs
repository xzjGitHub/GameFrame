using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class IABManifestLoader
{
    private static IABManifestLoader m_instance;
    public static IABManifestLoader Instance
    {
        get
        {
            if (m_instance == null)
            {
                m_instance = new IABManifestLoader();
            }
            return m_instance;
        }
    }

    private string m_manifestPath;
    private bool m_isLoadFnish;

    public AssetBundleManifest m_bundleManifest;
    public AssetBundle m_manifestLoader;

    public IABManifestLoader()
    {
        m_bundleManifest = null;
        m_manifestLoader = null;
        m_isLoadFnish = false;

        m_manifestPath = IPathTools.GetAssetBundlePath()+ "/Windows";

    //    Debug.LogError("m_manifestPath: " + m_manifestPath);
    }

    public void SetMainfestPath(string path)
    {
        m_manifestPath = path;
    }

    public bool IsLoadFnish()
    {
        return m_isLoadFnish;
    }

    public IEnumerator LoadManifest()
    {
        WWW maniFest = new WWW(m_manifestPath);
        yield return maniFest;
        if (!string.IsNullOrEmpty(maniFest.error))
        {
            Debug.LogError("加载manifest error: "+ maniFest.error);
        }
        else
        {
            if (maniFest.progress >= 1.0)
            {
                m_isLoadFnish = true;
                m_manifestLoader = maniFest.assetBundle;
                m_bundleManifest = m_manifestLoader.LoadAsset("AssetBundleManifest") as AssetBundleManifest;

               //string[] s= m_bundleManifest.GetAllAssetBundles();

               // for (int i = 0; i < s.Length; i++)
               //     Debug.LogError(s[i]);
            }
        }
    }

    public string[] GetDenpences(string bundleName)
    {
        return m_bundleManifest.GetAllDependencies(bundleName);
    }

    public void UnLoadManifest()
    {
        m_manifestLoader.Unload(true);
    }

}

