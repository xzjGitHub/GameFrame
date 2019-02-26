using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public abstract class MonoBase: MonoBehaviour
{
    public abstract void ProcessEvent(MsgBase msg);

}

