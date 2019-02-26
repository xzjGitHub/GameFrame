using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public delegate void LoadAssetBundleCallBack(string seceName,string bundleName);

//存的是一个bundle包里面的obj
public class AssetResObj
{
    public Dictionary<string,AssetObj> Objs;

    public AssetResObj(string name,AssetObj obj)
    {
        Objs = new Dictionary<string,AssetObj>();
        Objs[name] = obj;
    }

    public void AddResObj(string name,AssetObj obj)
    {
        Objs[name] = obj;
    }

    public void ReleaseAllResObj()
    {
        List<string> keys = new List<string>();
        foreach(var key in Objs.Keys)
        {
            keys.Add(key);
        }
        for(int i = 0; i < keys.Count; i++)
        {
            ReleaseSingleResObj(keys[i]);
        }
    }

    public void ReleaseSingleResObj(string name)
    {
        Objs[name].ReleaseObj();
    }

    public List<UnityEngine.Object> GetResObj(string name)
    {
        return Objs[name].objs;
    }
}

//当个物体 可能包含多个 一般是一个物体由多个物体组成的那种
public class AssetObj
{
    public List<UnityEngine.Object> objs;

    public AssetObj(params UnityEngine.Object[] tempObj)
    {
        objs = new List<UnityEngine.Object>();
        objs.AddRange(tempObj);
    }

    public void ReleaseObj()
    {
        for(int i = 0; i < objs.Count; i++)
        {
            Resources.UnloadAsset(objs[i]);
        }
    }
}



/// <summary>
/// 对场景里面的所有bundle包的管理
/// </summary>
public class IABManager
{
    private Dictionary<string,IABRelationManager> m_loaderHelper = new Dictionary<string,IABRelationManager>();

    private Dictionary<string,AssetResObj> m_loadObjs = new Dictionary<string,AssetResObj>();

    private string m_sceneName;

    public IABManager(string sceneName)
    {
        m_sceneName = sceneName;
    }

    private string[] GetDenpendces(string bundleName)
    {
        return IABManifestLoader.Instance.GetDenpences(bundleName);
    }

    public void LoadAssetBundle(string bundleName,LoaderProgress loaderProgress,
        LoadAssetBundleCallBack loadAssetBundleCallBack)
    {
        if(!m_loaderHelper.ContainsKey(bundleName))
        {
            IABRelationManager iABRelationManager = new IABRelationManager();
            iABRelationManager.Initial(bundleName,loaderProgress);
            m_loaderHelper.Add(bundleName,iABRelationManager);
            loadAssetBundleCallBack(m_sceneName,bundleName);
        }
    }

    public IEnumerator LoadAssetBundleDependeces(string bundleName,string refName,
        LoaderProgress loaderProgress)
    {
        if (!m_loaderHelper.ContainsKey(bundleName))
        {
            IABRelationManager iABRelationManager = new IABRelationManager();
            iABRelationManager.Initial(bundleName, loaderProgress);
            if (refName != null)
            {
                iABRelationManager.AddRefference(refName);
            }
            m_loaderHelper.Add(bundleName, iABRelationManager);
            yield return LoadAssetBundles(bundleName);
        }
        else
        {
            if(refName != null)
            {
                IABRelationManager iABRelationManager = new IABRelationManager();
                iABRelationManager.AddRefference(refName);
            }
        }
    }

    /// <summary>
    /// 加载ab 必须先加载manifest
    /// </summary>
    /// <param name="bundleName"></param>
    /// <returns></returns>
    public IEnumerator LoadAssetBundles(string bundleName)
    {
        while(!IABManifestLoader.Instance.IsLoadFnish())
        {
            yield return null;
        }
        IABRelationManager iABRelation = m_loaderHelper[bundleName];
        string[] denpences = GetDenpendces(bundleName);
        iABRelation.SetDependences(denpences);

        for (int i = 0; i < denpences.Length; i++)
        {
            yield return LoadAssetBundleDependeces(denpences[i], bundleName, iABRelation.GetLoaderProgress());
        }

        yield return iABRelation.LoadAssetBundle();
    }

    public bool IsLoadAssetBundle(string bundleName)
    {
        if(m_loaderHelper.ContainsKey(bundleName))
        {
            return true;
        }
        else
        {
            return false;
        }
    }

    public void DebugAsset(string bundleName)
    {
        if(m_loaderHelper.ContainsKey(bundleName))
        {
            IABRelationManager iABRelationManager = m_loaderHelper[bundleName];
            iABRelationManager.DebugAsset();
        }
        else
        {
            Debug.LogError("m_loaderHelper not exit bundleName: " + bundleName);
        }
    }

    public bool IsLoadFnish(string bundleName)
    {
        if(m_loaderHelper.ContainsKey(bundleName))
        {
            IABRelationManager iABRelationManager = m_loaderHelper[bundleName];
            return iABRelationManager.IsLoadFnish();
        }
        else
        {
            Debug.LogError("m_loaderHelper not exit bundleName: " + bundleName);
            return false;
        }
    }

    public UnityEngine.Object GetSingleResource(string bundleName,string resName)
    {
        //已经缓存了物体 从缓存里面取
        if(m_loadObjs.ContainsKey(bundleName))
        {
            AssetResObj assetResObj = m_loadObjs[bundleName];
            List<UnityEngine.Object> tempObj = assetResObj.GetResObj(resName);
            if(tempObj != null)
            {
                return tempObj[0];
            }
        }
        else
        {
            //已经加载了该bundle 从Bundle里面取
            if(m_loaderHelper.ContainsKey(bundleName))
            {
                IABRelationManager iABRelationManager = m_loaderHelper[bundleName];
                UnityEngine.Object tempObj = iABRelationManager.GetSingleResource(resName);
                AssetObj assetObj = new AssetObj(tempObj);
                //判断缓存里面是否已经有这个包
                if(m_loadObjs.ContainsKey(bundleName))
                {
                    AssetResObj tempRes = m_loadObjs[bundleName];
                    tempRes.AddResObj(resName,assetObj);
                }
                else
                {
                    AssetResObj tempRes = new AssetResObj(resName,assetObj);
                    m_loadObjs.Add(bundleName,tempRes);
                }
                return tempObj;
            }
            return null;
        }
        return null;
    }


    public UnityEngine.Object[] GetMutiResources(string bundleName,string resName)
    {
        //已经缓存了物体 从缓存里面取
        if(m_loadObjs.ContainsKey(bundleName))
        {
            AssetResObj assetResObj = m_loadObjs[bundleName];
            List<UnityEngine.Object> tempObj = assetResObj.GetResObj(resName);
            if(tempObj != null)
            {
                return tempObj.ToArray();
            }
        }
        else
        {
            //已经加载了该bundle 从Bundle里面取
            if(m_loaderHelper.ContainsKey(bundleName))
            {
                IABRelationManager iABRelationManager = m_loaderHelper[bundleName];
                UnityEngine.Object[] tempObj = iABRelationManager.GetMutiResource(bundleName);
                AssetObj assetObj = new AssetObj(tempObj);
                //判断缓存里面是否已经有这个包
                if(m_loadObjs.ContainsKey(bundleName))
                {
                    AssetResObj tempRes = m_loadObjs[bundleName];
                    tempRes.AddResObj(resName,assetObj);
                }
                else
                {
                    AssetResObj tempRes = new AssetResObj(resName,assetObj);
                    m_loadObjs.Add(bundleName,tempRes);
                }
                return tempObj;
            }
            else
            {
                Debug.LogError("m_loaderHelper not exit bundleName: " + bundleName);
                return null;
            }
        }
        return null;
    }

    /// <summary>
    /// 卸载某个Bundle里面加载出的某个物体
    /// </summary>
    /// <param name="bundleName"></param>
    /// <param name="resName"></param>
    public void DisposeSingleResObj(string bundleName,string resName)
    {
        if(m_loadObjs.ContainsKey(bundleName))
        {
            AssetResObj assetResObj = m_loadObjs[bundleName];
            assetResObj.ReleaseSingleResObj(resName);
        }
    }

    /// <summary>
    /// 卸载当个bundle加载出来的物体
    /// </summary>
    /// <param name="bundleName"></param>
    public void DisposeSingleBundleResObjs(string bundleName)
    {
        if(m_loadObjs.ContainsKey(bundleName))
        {
            AssetResObj assetResObj = m_loadObjs[bundleName];
            assetResObj.ReleaseAllResObj();
        }
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 卸载所有加载出来的物体
    /// </summary>
    public void DisposeAllBundleResObjs()
    {
        List<string> keys = new List<string>();
        keys.AddRange(m_loadObjs.Keys);
        for(int i = 0; i < keys.Count; i++)
        {
            DisposeSingleBundleResObjs(keys[i]);
        }
        m_loadObjs.Clear();
        Resources.UnloadUnusedAssets();
    }

    /// <summary>
    /// 这是卸载ab包 卸载一个 需要处理其依赖关系
    /// </summary>
    /// <param name="bundleName"></param>
    public void DisposeBundle(string bundleName)
    {
        if (m_loadObjs.ContainsKey(bundleName))
        {
            IABRelationManager iABRelationManager = m_loaderHelper[bundleName];
            List<string> dep = iABRelationManager.GetDenpedences();
            for (int i = 0; i < dep.Count; i++)
            {
                if (m_loaderHelper.ContainsKey(dep[i]))
                {
                    IABRelationManager de = m_loaderHelper[dep[i]];
                    if (de.RemoveReffence(bundleName))
                    {
                        DisposeBundle(de.GetBundleName());
                    }
                }
            }
            if (iABRelationManager.GetRefferences().Count == 0)
            {
                iABRelationManager.Dispose();
                m_loaderHelper.Remove(bundleName);
            }
        }
    }

    /// <summary>
    /// 卸载所有的ab包 这里不需要处理依赖关系 直接干掉
    /// </summary>
    public void DisposeAllBundle()
    {
        //卸载所有的ab包 这里不需要处理依赖关系 直接干掉
        List<string> keys = new List<string>();
        keys.AddRange(m_loaderHelper.Keys);
        for(int i = 0; i < keys.Count; i++)
        {
            IABRelationManager iABRelationManager = m_loaderHelper[keys[i]];
            iABRelationManager.Dispose();
        }

        m_loaderHelper.Clear();
    }

    /// <summary>
    /// 卸载所以的ab包和已经加载出来的物体
    /// </summary>
    public void DisposeAllBundleAndRes()
    {
        DisposeAllBundleResObjs();

        DisposeAllBundle();
    }
}

