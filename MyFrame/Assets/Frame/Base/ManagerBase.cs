using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using UnityEngine;


public class EventNode
{
    public MonoBase data;
    public EventNode next;

    public EventNode(MonoBase data)
    {
        this.data = data;
        this.next = null;
    }
}

public class ManagerBase: MonoBase
{

    public Dictionary<ushort,EventNode> eventTree = new Dictionary<ushort,EventNode>();

    /// <summary>
    /// 
    /// </summary>
    /// <param name="mono">要注册得脚本</param>
    /// <param name="msgs">脚本 可以注册多个Msg</param>
    public void RegisterMsg(MonoBase mono,params ushort[] msgs)
    {
        for(int i = 0; i < msgs.Length; i++)
        {
            RegisterMsg(msgs[i],new EventNode(mono));
        }
    }

    public void RegisterMsg(ushort id,EventNode node)
    {
        if(!eventTree.ContainsKey(id))
        {
            eventTree.Add(id,node);
        }
        else
        {
            EventNode temp = eventTree[id];
            while(temp.next != null)
            {
                temp = temp.next;
            }
            temp.next = node;
        }
    }

    public void UnRegister(MonoBase mono,params ushort[] msgs)
    {
        for(int i = 0; i < msgs.Length; i++)
        {
            UnRegister(msgs[i],mono);
        }
    }

    public void UnRegister(ushort msgId,MonoBase mono)
    {
        if(eventTree.ContainsKey(msgId))
        {
            EventNode node = eventTree[msgId];
            //要注销的是头部
            if(node.data == mono)
            {
                EventNode header = node;
                if(header.next != null)
                {
                    header.data = node.next.data;
                    header.next = node.next.next;
                }
                else
                {
                    eventTree.Remove(msgId);
                }
            }
            else
            {
                while(node.next != null && node.next.data != mono)
                {
                    node = node.next;
                }
                if(node.next.next != null)
                {
                    node.next = node.next.next;
                }
                else
                {
                    node.next = null;
                }
            }

        }
    }

    public override void ProcessEvent(MsgBase msg)
    {
        if(!eventTree.ContainsKey(msg.msgId))
        {
            Debug.LogError("eventTree not contains msg: " + msg.msgId);
            return;
        }
        else
        {
            EventNode node = eventTree[msg.msgId];
            do
            {
                node.data.ProcessEvent(msg);
                node = node.next;
            } while(node != null);
        }
    }
}

