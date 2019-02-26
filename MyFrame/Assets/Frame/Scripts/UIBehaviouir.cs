using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using UnityEngine;
using UnityEngine.Events;

public class UIBehaviour:MonoBehaviour
{

    private void Awake()
    {
        UIManager.Instance.RegisterGameObject(gameObject.name, gameObject);
    }

    public void AddButtonLister(UnityAction action)
    {

    }

    public void RemoveButtonLister(UnityAction action)
    {

    }
}

