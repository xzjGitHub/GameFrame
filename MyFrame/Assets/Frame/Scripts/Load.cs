using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;

public class Load:UIBase
{

    private void Awake()
    {
        msgIds = new ushort[]
        {


        };
        RegisterSelf(this, msgIds);
        UIManager.Instance.GetGameObject("").GetComponent<UIBehaviour>().AddButtonLister(ButtonClck);
    }


    public void ButtonClck()
    {

    }
}

