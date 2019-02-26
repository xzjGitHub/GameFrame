using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public delegate void LoaderProgress(string abName, float progress);

public delegate void LoadFnish(string anName);

public class IABLoader
{

    private string m_abName;
    private string m_abPath;

    private WWW m_www;

    private float m_progress;

    private LoaderProgress m_loaderProgress;
    private LoadFnish m_loadFnish;

    private IABResourceLoader m_resourceLoader;

    public IABLoader(string abName, LoaderProgress loaderProgress,LoadFnish loadFnish)
    {
        m_loaderProgress = loaderProgress;
        m_loadFnish = loadFnish;
        m_abName = abName;
        m_abPath = "";
        m_progress = 0;
        m_loaderProgress = null;
        m_loadFnish = null;
        m_resourceLoader = null;
    }

    public void SetBundleName(string name)
    {
        m_abName = name;
    }

    public void LoadResources(string path)
    {
        m_abPath = path;
    }

    public IEnumerator Load()
    {
        m_abPath = IPathTools.GetAssetBundlePath() + "/" + m_abName;
        Debug.LogError(m_abPath);


        m_www = new WWW(m_abPath);
        while (!m_www.isDone)
        {
            m_progress = m_www.progress;

            if (m_loaderProgress != null)
            {
                m_loaderProgress(m_abName, m_www.progress);
            }

            yield return m_www.progress;
            m_progress = m_www.progress;
        }
        if (m_progress >= 1.0f)
        {
            if (m_loadFnish != null)
            {
                m_loadFnish(m_abName);
            }

            m_resourceLoader = new IABResourceLoader(m_www.assetBundle);
        }
        else
        {
            Debug.LogError("加载异常: "+m_abName);
        }
        m_www = null;
    }

    public void DebugLoader()
    {
        if (m_www != null)
        {
            m_resourceLoader.DebugAllRes();
        }
    }

    /// <summary>
    /// 获取单个资源
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Object GetResource(string name)
    {
        return m_resourceLoader[name];
    }

    /// <summary>
    /// 获取多个资源
    /// </summary>
    /// <param name="name"></param>
    /// <returns></returns>
    public UnityEngine.Object[] GetMutiResource(string name)
    {
        return m_resourceLoader.LoadResources(name);
    }

    /// <summary>
    /// 释放
    /// </summary>
    public void Dispose()
    {
        m_resourceLoader.Dispose();
        m_resourceLoader = null;
    }

    public void UnloadAssetRes(UnityEngine.Object tempObj)
    {
        m_resourceLoader.UnLoadResource(tempObj);
    }
}

