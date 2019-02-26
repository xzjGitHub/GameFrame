using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class AssetBundleManager
{
    private AssetBundleManager() { }

    private static AssetBundleManager instance;
    public static AssetBundleManager Instance
    {
        get
        {
            if(instance == null)
            {
                instance = new AssetBundleManager();
            }
            return instance;
        }
    }



}

