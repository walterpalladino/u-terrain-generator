using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public enum UTSpawnDataType
{
    Tree,
    DetailMesh,
    GrassTexture
}


[System.Serializable]
public class UTSpawnData
{
    public bool enabled = true;
    [System.NonSerialized]
    public string layerName;
    public int[] prefabsIds;
    [System.NonSerialized]
    public UTSpawnDataType spawnDataType= UTSpawnDataType.Tree;
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
    [Range(0.0f, 0.5f)]
    public float sizeVariation = 0.0f;
    [Tooltip("Sunks the bottom of the tree. usefull for some meshes ending on flat bottoms.")]
    public float sinkBottom = 0.0f;
}