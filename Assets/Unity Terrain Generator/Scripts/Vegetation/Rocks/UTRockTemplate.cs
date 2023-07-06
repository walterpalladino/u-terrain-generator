using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTRockTemplate
{
    public GameObject mesh = null;

    public float minSize = 1.0f;
    public float maxSize = 2.0f;

    public bool useGpuInstancing = true;

    [HideInInspector]
    public int prefabId;

}
