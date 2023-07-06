using System.Collections;
using System.Collections.Generic;
using UnityEngine;


[System.Serializable]
public class UTGrassTemplate
{
    public Texture2D texture = null;
    public Color healthyColor = Color.white;
    public Color dryColor = Color.white;

    public float minSize = 1.0f;
    public float maxSize = 2.0f;

    [HideInInspector]
    public int prefabId;
    
}
