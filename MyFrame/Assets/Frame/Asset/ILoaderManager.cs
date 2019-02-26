using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class ILoaderManager:MonoBehaviour
{

    public static ILoaderManager Instance;

    private Dictionary<string,IABSceneManager> m_loadManager = new Dictionary<string,IABSceneManager>();

    private void Awake()
    {

        Instance = this;

        //第一步 加载iabmanifest

          StartCoroutine(IABManifestLoader.Instance.LoadManifest());
          ReadConfiger("ScenesOne");
    }
    public void Test()
    {
        LoadAsset("ScenesOne","ScenesOne",Loader);
    }

    public void Test1()
    {
        UnityEngine.Object obj = GetSingleRes("ScenesOne","ScenesOne","Cube");

        GameObject temp = GameObject.Instantiate(obj) as GameObject;

        temp.transform.localPosition = Vector3.zero;
    }


    private void Loader(string abName,float progress)
    {
        Debug.LogError("加载进度:" + progress);
    }

    //第二步 读取配置文件
    public void ReadConfiger(string sceneName)
    {
        if(!m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = new IABSceneManager(sceneName);
            iABSceneManager.ReadConfiger(sceneName);
            m_loadManager.Add(sceneName,iABSceneManager);
        }
    }

    //加载
    public void LoadAsset(string sceneName,string bundleName,LoaderProgress loaderProgress)
    {
        if(!m_loadManager.ContainsKey(sceneName))
        {
            ReadConfiger(sceneName);
        }
        IABSceneManager iABSceneManager = m_loadManager[sceneName];
        iABSceneManager.LoadAsset(bundleName,loaderProgress,LoadCallBack);
    }

    public void LoadCallBack(string sceneName,string bundleName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            Game.Instance.StartCoroutine(iABSceneManager.LoadAssetSys(bundleName));
        }
        else
        {
            Debug.LogError("加载出错");
        }
    }

    public UnityEngine.Object GetSingleRes(string sceneName,string bundleName,string resName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            return iABSceneManager.GetSingleResource(bundleName,resName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
            return null;
        }
    }

    public UnityEngine.Object[] GetMutiRes(string sceneName,string bundleName,string resName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            return iABSceneManager.GetMutiResource(bundleName,resName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
            return null;
        }
    }

    public void UnLoadResObj(string sceneName,string bundleName,string resName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            iABSceneManager.DisposeResObj(bundleName,resName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
        }
    }

    public void UnloadBundleResObj(string sceneName,string bundleName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            iABSceneManager.DisposeBundleResObj(bundleName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
        }
    }

    public void UnLoadBundleResObj(string sceneName,string bundleName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            iABSceneManager.DisposeAllBundleResObj(bundleName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
        }
    }

    public void UnloadBundle(string sceneName,string bundleName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iABSceneManager = m_loadManager[sceneName];
            iABSceneManager.DisposeBundle(bundleName);
        }
        else
        {
            Debug.LogError("没有找到bundleName: " + bundleName);
        }
    }

    public void UnLoadAllBundle(string sceneName)
    {
        IABSceneManager iABSceneManager = m_loadManager[sceneName];
        iABSceneManager.DisAllBundle();
    }


    public void UnLoadAllBundleAndRes(string sceneName)
    {
        IABSceneManager iABSceneManager = m_loadManager[sceneName];
        iABSceneManager.DisAllBundleAndRes();
    }

    public void DebugAsset(string sceneName)
    {
        IABSceneManager iABSceneManager = m_loadManager[sceneName];
        iABSceneManager.DebugAsset();
    }


    private void OnDestroy()
    {
        m_loadManager.Clear();
    }


    public bool IsLoadingBundleFinish(string sceneName, string bundleName)
    {
        if (m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iAB = m_loadManager[sceneName];
            return iAB.IsLoadFinish(bundleName);
        }
        return false;
    }

    public bool IsLoadedAssetBundle(string sceneName, string bundleName)
    {
        if(m_loadManager.ContainsKey(sceneName))
        {
            IABSceneManager iAB = m_loadManager[sceneName];
            return iAB.IsLoadedAssetBundle(bundleName);
        }
        return false;
    }
}

