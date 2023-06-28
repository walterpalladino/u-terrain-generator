using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTSpawnTemplate
{
    public bool enabled = true;
    [System.NonSerialized]
    public string layerName;
    public GameObject mesh = null;
    public Texture2D texture = null;
    public int prefabId;
    [System.NonSerialized]
    public UTSpawnDataType spawnDataType = UTSpawnDataType.Tree;
    [Range(0.0f, 1.0f)]
    public float presence = 0.5f;
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
    public int maxQuantity = 1000;
    [Range(0.0f, 1.0f)]
    public float sizeVariation = 0.0f;

}