using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;


public class MsgBase
{

    public ushort msgId;

    public MsgBase()
    {
        msgId = 0;
    }

    public MsgBase(ushort tempMsg)
    {
        msgId = tempMsg;
    }

    public ManagerId GetManagerId()
    {
        return (ManagerId)(msgId/FrameTools.MsgSpan*FrameTools.MsgSpan);
    }
}

