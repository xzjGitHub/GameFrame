using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class IABResourceLoader:IDisposable
{

    private AssetBundle m_abRes;

    public IABResourceLoader(AssetBundle ab)
    {
        m_abRes = ab;
    }

    /// <summary>
    /// 加载单个资源 
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public UnityEngine.Object this[string resName]
    {
        get
        {
            if(this.m_abRes == null || !this.m_abRes.Contains(resName))
            {
                Debug.LogError("resource not contain");
                return null;
            }
            return m_abRes.LoadAsset(resName);
        }
    }

    /// <summary>
    /// 加载多个资源
    /// </summary>
    /// <param name="resName"></param>
    /// <returns></returns>
    public UnityEngine.Object[] LoadResources(string resName)
    {
        if(this.m_abRes == null || !this.m_abRes.Contains(resName))
        {
            Debug.LogError("resource not contain");
            return null;
        }
        return m_abRes.LoadAssetWithSubAssets(resName);
    }

    /// <summary>
    /// 卸载单个资源
    /// </summary>
    /// <param name="resObj"></param>
    public void UnLoadResource(UnityEngine.Object resObj)
    {
        Resources.UnloadAsset(resObj);
    }

    /// <summary>
    /// 释放AB包
    /// </summary>
    public void Dispose()
    {
        if (m_abRes != null)
        {
            m_abRes.Unload(false);
        }
        else
        {
            Debug.LogError("m_abRes is null");
        }
    }

    public void DebugAllRes()
    {
        string[] tempAb = m_abRes.GetAllAssetNames();
        for (int i = 0; i < tempAb.Length; i++)
        {
            Debug.Log("abres contain asset name: " + tempAb[i]);
        }
    }
}
