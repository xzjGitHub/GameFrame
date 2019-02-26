using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MsgCenter: MonoBase
{
    public static MsgCenter Instance;

    private void Awake()
    {
        Instance = this;

        gameObject.AddComponent<UIManager>();
    }

    public override void ProcessEvent(MsgBase msg)
    {

    }

    public void SendToMsg(MsgBase msg)
    {
        AnasysisMsg(msg);
    }

    private void AnasysisMsg(MsgBase msg)
    {
        ManagerId id = msg.GetManagerId();
        switch(id)
        {
            case ManagerId.GameManager:
                break;
            case ManagerId.UIManager:
                break;
            case ManagerId.AudioManafger:
                break;
            case ManagerId.NPCManager:
                break;
            case ManagerId.CharacterManager:
                break;
            case ManagerId.AssetManager:
                break;
            case ManagerId.NetManager:
                break;
        }
    }

}

