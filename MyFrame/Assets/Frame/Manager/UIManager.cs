using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class UIManager: ManagerBase
{
    public static UIManager Instance;

    private Dictionary<string, GameObject> sonMembers = new Dictionary<string, GameObject>(); 

    private void Awake()
    {
        Instance = null;
    }

    public void RegisterGameObject(string name,GameObject obj)
    {
        if (!sonMembers.ContainsKey(name))
        {
            sonMembers.Add(name, obj);
        }
    }

    public void UnRegisterGameObject(string name)
    {
        if (sonMembers.ContainsKey(name))
        {
            sonMembers.Remove(name);
        }
    }

    public void SendMsg(MsgBase msg)
    {
        if(msg.GetManagerId() == ManagerId.UIManager)
        {
            ProcessEvent(msg);
        }
        else
        {
            MsgCenter.Instance.SendToMsg(msg);
        }
    }

    public GameObject GetGameObject(string name)
    {
        if (sonMembers.ContainsKey(name))
        {
            return sonMembers[name];
        }
        return null;
    }
}
