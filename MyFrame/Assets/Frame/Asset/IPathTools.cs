using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using System.IO;
using UnityEngine;

public class IPathTools
{
    public static string GetPlatformFolderName(RuntimePlatform runtimePlatform)
    {
        switch(runtimePlatform)
        {
            case RuntimePlatform.Android:
                return "Android";
            case RuntimePlatform.IPhonePlayer:
                return "IOS";
            case RuntimePlatform.WindowsPlayer:
            case RuntimePlatform.WindowsEditor:
                return "Windows";
            case RuntimePlatform.OSXEditor:
            case RuntimePlatform.OSXPlayer:
                return "OSX";
            default:
                return null;
        }
    }

    public static string GetAppFilePath()
    {
        string tempPath = "";
        if(Application.platform == RuntimePlatform.WindowsEditor || Application.platform == RuntimePlatform.OSXEditor)
        {
            tempPath = Application.streamingAssetsPath;
        }
        else
        {
            tempPath = Application.persistentDataPath;
        }

        return tempPath;
    }

    public static string GetAssetBundlePath()
    {
        string platformFolder = GetPlatformFolderName(Application.platform);

        string path = Path.Combine(GetAppFilePath(),platformFolder);

        return path.Replace("\\","/");
    }


    public static string GetMainfestBundlePath()
    {
        string platformFolder = GetPlatformFolderName(Application.platform);

        string path = Path.Combine(GetAppFilePath(),platformFolder);

        return path.Replace("\\","/");
    }
}

