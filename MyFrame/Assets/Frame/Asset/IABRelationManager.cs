using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class IABRelationManager
{
    //依赖
    private List<string> m_depenceBundles;

    //被依赖
    private List<string> m_referBundles;

    private IABLoader m_abLoader;

    private bool m_loadFnish;

    private string m_bundleName;

    private LoaderProgress m_loaderProgress;

    public IABRelationManager()
    {
        m_depenceBundles = new List<string>();
        m_referBundles = new List<string>();
    }

    public LoaderProgress GetLoaderProgress()
    {
        return m_loaderProgress;
    }

    public string GetBundleName()
    {
        return m_bundleName;
    }

    public void Initial(string bundleName, LoaderProgress loaderProgress)
    {
        m_loadFnish = false;
        m_bundleName = bundleName;
        m_loaderProgress = loaderProgress;
        m_abLoader = new IABLoader(bundleName,loaderProgress, BundleLoadFnish);
    }

    public void BundleLoadFnish(string bundleName)
    {
        m_loadFnish = true;
    }

    public bool IsLoadFnish()
    {
        return m_loadFnish;
    }


    public void AddRefference(string bundleName)
    {
        m_referBundles.Add(bundleName);
    }

    /// <summary>
    /// 
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns>表示是否释放了自己</returns>
    public bool RemoveReffence(string bundleName)
    {
        m_referBundles.Remove(bundleName);
        //没有被其他包所依赖 需要释放自己
        if (m_referBundles.Count == 0)
        {
            Dispose();
            return true;
        }
        return false;
    }

    public List<string> GetRefferences()
    {
        return m_referBundles;
    }


    public void SetDependences(string[] denpence)
    {
        if (denpence.Length > 0)
        {
            m_depenceBundles.AddRange(denpence);
        }
    }

    public void Removeependences(string bundleName)
    {
        m_depenceBundles.Remove(bundleName);
    }

    public List<string> GetDenpedences()
    {
        return m_depenceBundles;
    }

    public IEnumerator LoadAssetBundle()
    {
        yield return m_abLoader.Load();
    }


    public UnityEngine.Object GetSingleResource(string resName)
    {
        return m_abLoader.GetResource(resName);
    }

    public UnityEngine.Object[] GetMutiResource(string bundleName)
    {
        return m_abLoader.GetMutiResource(bundleName);
    }

    public void DebugAsset()
    {
        m_abLoader.DebugLoader();
    }

    public void Dispose()
    {
        m_abLoader.Dispose();
    }
}

