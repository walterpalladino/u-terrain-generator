using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTTreesData
{
    public bool enabled = true;
    [Range(0.0f, 1.0f)]
    public float presence = 0.5f;
    [HideInInspector]
    public int maxQuantity = 1000;
    public UTTreeTemplate[] treeTemplates;
}
