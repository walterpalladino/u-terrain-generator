using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTGrassData
{
    public bool enabled = true;
    [Range(0.0f, 1.0f)]
    public float presence = 0.5f;
    [HideInInspector]
    public int maxQuantity = 1000;
    public UTGrassTemplate[] templates;
    [Range(0.0f, 90.0f)]
    public float maxSlope = 20.0f;
    public float minAltitude = 0.0f;
    public float maxAltitude = 100.0f;

}
