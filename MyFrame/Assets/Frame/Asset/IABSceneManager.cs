using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;

using UnityEngine;

public class IABSceneManager
{
    private IABManager m_iABManager;


    private Dictionary<string,string> m_allAsset = new Dictionary<string,string>();

    public IABSceneManager(string sceneName)
    {
        m_iABManager = new IABManager(sceneName);
    }

    public void ReadConfiger(string sceneName)
    {
        string textFileName = "Record.txt";
        string path = IPathTools.GetAssetBundlePath() + "/" + sceneName + textFileName;
        m_iABManager = new IABManager(sceneName);
        ReadConfig(path);
    }

    private void ReadConfig(string path)
    {
        FileStream fs = new FileStream(path,FileMode.Open);

        StreamReader br = new StreamReader(fs);

        string line = br.ReadLine();

        int allCount = int.Parse(line);

        for(int i = 0; i < allCount; i++)
        {
            string tempStr = br.ReadLine();
            string[] tempArr = tempStr.Split(" ".ToArray());
            m_allAsset[tempArr[0]] = tempArr[1];


            Debug.LogError(tempArr[0]);
        }

        br.Close();
        fs.Close();
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName">ScenesOne scenesone.ld  这里的bundleName指的是ScenesOne</param>
    /// <param name="loaderProgress"></param>
    /// <param name="loadAssetBundleCallBack"></param>
    public void LoadAsset(string bundleName,LoaderProgress loaderProgress,
        LoadAssetBundleCallBack loadAssetBundleCallBack)
    {
        if (m_allAsset.ContainsKey(bundleName))
        {
            string tempValue = m_allAsset[bundleName];
            m_iABManager.LoadAssetBundle(tempValue, loaderProgress, loadAssetBundleCallBack);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
        }
    }

    public IEnumerator LoadAssetSys(string bundleName)
    {
        yield return m_iABManager.LoadAssetBundles(bundleName);
    }

    public UnityEngine.Object GetSingleResource(string bundleName, string resName)
    {
        if (m_allAsset.ContainsKey(bundleName))
        {
            return m_iABManager.GetSingleResource(m_allAsset[bundleName], resName);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
            return null;
        }
    }

    public UnityEngine.Object[] GetMutiResource(string bundleName,string resName)
    {
        if(m_allAsset.ContainsKey(bundleName))
        {
            return m_iABManager.GetMutiResources(m_allAsset[bundleName],resName);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
            return null;
        }
    }


    //------卸载已经加载出来的物体-------

    public void DisposeResObj(string bundleName, string resName)
    {
        if(m_allAsset.ContainsKey(bundleName))
        {
            m_iABManager.DisposeSingleResObj(m_allAsset[bundleName], resName);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
        }
    }

    public void DisposeBundleResObj(string bundleName)
    {
        if(m_allAsset.ContainsKey(bundleName))
        {
            m_iABManager.DisposeSingleBundleResObjs(m_allAsset[bundleName]);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
        }
    }

    public void DisposeAllBundleResObj(string bundleName)
    {
        m_iABManager.DisposeAllBundleResObjs();
    }

    //------卸载AB包-------

    public void DisposeBundle(string bundleName)
    {
        if(m_allAsset.ContainsKey(bundleName))
        {
            m_iABManager.DisposeBundle(bundleName);
        }
        else
        {
            Debug.LogError("not contain the bundle:  " + bundleName);
        }
    }

    public void DisAllBundle()
    {
        m_iABManager.DisposeAllBundle();
        m_allAsset.Clear();
    }


    public void DisAllBundleAndRes()
    {
        m_iABManager.DisposeAllBundleAndRes();
        m_allAsset.Clear();
    }

    public void DebugAsset()
    {
        List<string> keys = new List<string>();
        keys.AddRange(m_allAsset.Keys);
        for (int i = 0; i < keys.Count; i++)
        {
            m_iABManager.DebugAsset(m_allAsset[keys[i]]);
        }
    }

    public bool IsLoadFinish(string bundleName)
    {
        if (m_allAsset.ContainsKey(bundleName))
        {
            return m_iABManager.IsLoadFnish(m_allAsset[bundleName]);
        }
        return false;
    }

    public bool IsLoadedAssetBundle(string bundleName)
    {
        if(m_allAsset.ContainsKey(bundleName))
        {
            return m_iABManager.IsLoadAssetBundle(m_allAsset[bundleName]);
        }
        return false;
    }
}

