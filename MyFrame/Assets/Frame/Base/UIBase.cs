using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class UIBase:MonoBase
{

    public ushort[] msgIds;

    public override void ProcessEvent(MsgBase msg)
    {
        
    }

    public void RegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        UIManager.Instance.RegisterMsg(mono, msgs);
    }

    public void UnRegisterSelf(MonoBase mono, params ushort[] msgs)
    {
        UIManager.Instance.UnRegister(mono, msgs);
    }

    public void SendMsg(MsgBase msg)
    {
        UIManager.Instance.SendMsg(msg);
    }

    private void OnDestroy()
    {
        if (msgIds != null)
        {
            UnRegisterSelf(this, msgIds);
        }
    }
}
