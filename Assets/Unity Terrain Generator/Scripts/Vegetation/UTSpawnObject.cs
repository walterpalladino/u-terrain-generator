using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UTSpawnObject 
{
    public int prefabIndex = 0;

    public Texture2D texture = null;
    public Color color1 = Color.white;
    public Color color2 = Color.white;
    public GameObject mesh = null;
    public Vector3 position = Vector3.zero;

    public float scale = 1.0f;
    public float minSize = 1.0f;
    public float maxSize = 2.0f;

    public float rotation = 0.0f;

    public float freeRadius = 0.0f;
}
