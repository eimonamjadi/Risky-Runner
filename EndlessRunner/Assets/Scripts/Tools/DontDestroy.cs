using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DontDestroy : MonoBehaviour
{
    [HideInInspector] public int ID;

    private void Start()
    {
        DontDestroy[] list = FindObjectsOfType<DontDestroy>();
        foreach(DontDestroy d in list)
        {
            //if (d.ID == ID) Destroy()
        }
    }
}
