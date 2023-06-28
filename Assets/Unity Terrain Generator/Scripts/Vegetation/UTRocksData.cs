using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTRocksData
{
    public bool enabled = true;
    [Range(0.0f, 1.0f)]
    public float presence = 0.5f;
    [HideInInspector]
    public int maxQuantity = 1000;
    public UTRockTemplate[] templates;
    [Range(1, 10)]
    public int groupSize = 1;
    [Range(1.0f, 15.0f)]
    public float groupRadius = 1.0f;
    [Range(0.5f, 25.0f)]
    public float freeRadius = 0.5f;
    [Range(0.0f, 90.0f)]
    public float maxSlope = 20.0f;
    public float minAltitude = 0.0f;
    public float maxAltitude = 100.0f;
    [Range(0.0f, 1.0f)]
    public float sizeVariation = 0.0f;

}
