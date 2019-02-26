using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class HunkAssetRes: MsgBase
{
    public string sceneName;

    public string bundleName;

    public string resName;

    public ushort backMsgId;

    public bool isSingle;


    public HunkAssetRes(bool tempSingle
        ,string sceneName,string bundleName,string resName,ushort tempBackId)
    {
        this.isSingle = tempSingle;
        this.sceneName = sceneName;
        this.bundleName = bundleName;
        this.resName = resName;
        this.backMsgId = tempBackId;
    }
}

public class HunkAssetBack: MsgBase
{
    public UnityEngine.Object[] value;

    public HunkAssetBack()
    {
        this.msgId = 0;
        this.value = null;
    }

    public void Change(ushort msgId,params UnityEngine.Object[] temvalue)
    {
        this.msgId = msgId;
        this.value = temvalue;
    }

    public void Change(ushort msgId)
    {
        this.msgId = msgId;
    }

    public void Change(params UnityEngine.Object[] tempvalue)
    {
        this.value = tempvalue;
    }
}


