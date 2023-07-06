using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTBushTemplate
{
    public GameObject mesh = null;

    public float minSize = 1.0f;
    public float maxSize = 2.0f;

    public bool useGpuInstancing = false;

    [HideInInspector]
    public int prefabId;

}
