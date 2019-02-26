using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public enum ManagerId
{
    GameManager = 0,
    UIManager = 3000,
    AudioManafger=3000*2,
    NPCManager=3000*3,
    CharacterManager=3000*4,
    AssetManager=3000*5,
    NetManager=3000*6
}

public class FrameTools
{
    public static int MsgSpan = 3000;
}

