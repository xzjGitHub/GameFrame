using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;
using System.IO;

public class AssetBundleEditor{

    [MenuItem("Itools/BuildAsset")]
    public static void BuildAssetBundle()
    {
        string path = IPathTools.GetAssetBundlePath();

        Debug.LogError(path);

        if (Directory.Exists(path))
            Directory.Delete(path);

        Directory.CreateDirectory(path);

        BuildPipeline.BuildAssetBundles(path,0,EditorUserBuildSettings.activeBuildTarget);

        AssetDatabase.Refresh();
    }

    [MenuItem("Itools/MakeAssetBundle")]
    public static void MakeAssetBundle()
    {
        AssetDatabase.RemoveUnusedAssetBundleNames();

        string path = Application.dataPath + "/Art/Scenes/";

        DirectoryInfo directoryInfo = new DirectoryInfo(path);
        FileSystemInfo[] fileSystemInfos = directoryInfo.GetFileSystemInfos();
        for (int i = 0; i < fileSystemInfos.Length; i++)
        {
            FileSystemInfo fileSystemInfo = fileSystemInfos[i];
            if (fileSystemInfo is DirectoryInfo)
            {
                string tempPath = Path.Combine(path,fileSystemInfo.Name);
                ScencesOverView(tempPath);
            }
        }

        AssetDatabase.Refresh();
    }

    /// <summary>
    /// 遍历目录
    /// </summary>
    /// <param name="scencesPath"></param>
    public static void ScencesOverView(string scencesPath)
    {
        string textFileName = "Record.txt";

        string tempPath = scencesPath + textFileName;

        FileStream fs = new FileStream(tempPath,FileMode.OpenOrCreate);

        StreamWriter sw = new StreamWriter(fs);

        //存储对应关系
        Dictionary<string, string> readDict = new Dictionary<string, string>();

        ChangeHead(scencesPath,readDict);

        sw.WriteLine(readDict.Count);

        foreach (string key in readDict.Keys)
        {
            sw.Write(key);
            sw.Write(" ");
            sw.Write(readDict[key]);
            sw.WriteLine();
        }

        sw.Close();
        fs.Close();
    }

    /// <summary>
    /// 截取相对路径 H:\MyFrame\Assets\Art\Scenes
    /// </summary>
    public static void ChangeHead(string fullPath,Dictionary<string ,string> theWiter)
    {
        int tempCount = fullPath.IndexOf("Assets");
        int tempLength = fullPath.Length;
        //Assets\Art\Scenes
        string replacePath = fullPath.Substring(tempCount, tempLength - tempCount);
        DirectoryInfo dir = new DirectoryInfo(fullPath);
        if (dir != null)
        {
            ListFiles(dir,replacePath,theWiter);
        }
        else
        {
            Debug.LogError("dir is not exit");
        }
    }

    /// <summary>
    /// 遍历功能文件夹
    /// </summary>
    /// <param name="fullPath"></param>
    /// <param name="theWiter"></param>
    public static void ListFiles(FileSystemInfo info, string replacePath, Dictionary<string, string> theWiter)
    {
        if (info == null)
            return;
        DirectoryInfo dir = info as DirectoryInfo;
        FileSystemInfo[] fileSystemInfos = dir.GetFileSystemInfos();
        for (int i = 0; i < fileSystemInfos.Length; i++)
        {
            FileInfo file = fileSystemInfos[i] as FileInfo;
            if (file != null)
            {
                ChangeMark(file,replacePath,theWiter);
            }
            else
            {
                ListFiles(fileSystemInfos[i],replacePath,theWiter);
            }
        }
    }

    public static string FixedPath(string path)
    {
        string tempPath= path.Replace("\\","/");
        return tempPath;
    }

    public static string GetBundlePath(FileInfo fileInfo,string replacePath)
    {
        string tempPath = fileInfo.FullName;
        tempPath = FixedPath(tempPath);
        int assetCount = tempPath.IndexOf(replacePath);
        assetCount += replacePath.Length + 1;
        int nameCount = tempPath.IndexOf(fileInfo.Name);
        int tempCount = replacePath.LastIndexOf("/");
        string secenceHead = replacePath.Substring(tempCount+1,replacePath.Length-tempCount-1);
        int tempLenght = nameCount - assetCount;
        if (tempLenght > 0)
        {
            string subString = tempPath.Substring(assetCount, tempPath.Length - assetCount);
            string[] result = subString.Split("/".ToCharArray());
            return secenceHead + "/" + result[0];
        }
        else
        {
            return secenceHead;
        }
    }

    public static void ChangeMark(FileInfo fileInfo,string replacePath,Dictionary<string,string> theWiter)
    {
        if (fileInfo.Extension == ".meta")
        {
            return;
        }
        string markString = GetBundlePath(fileInfo,replacePath);
        ChangeAssetMark(fileInfo,markString,theWiter);
    }

    public static void ChangeAssetMark(FileInfo info,string mark,Dictionary<string,string> theWiter)
    {
        string fullPath = info.FullName;

        int assetCount = fullPath.IndexOf("Assets");
        string assetPath = fullPath.Substring(assetCount, fullPath.Length - assetCount);

        AssetImporter assetImporter = AssetImporter.GetAtPath(assetPath);
        assetImporter.assetBundleName = mark;
        if (info.Extension == ".unity")
        {
            assetImporter.assetBundleVariant = "u3d";
        }
        else
        {
            assetImporter.assetBundleVariant = "ld";
        }

        string modelName = "";
        string[] subMark = mark.Split("/".ToCharArray());
        if (subMark.Length > 1)
        {
            modelName = subMark[1];
        }
        else
        {
            modelName = mark;
        }

        string modelPath = mark.ToLower() + "." + assetImporter.assetBundleVariant;
        if (!theWiter.ContainsKey(modelName))
        {
            theWiter.Add(modelName,modelPath);
        }
    }
}
