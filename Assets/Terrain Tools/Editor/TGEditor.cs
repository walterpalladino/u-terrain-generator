using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;



[CustomEditor(typeof(TG))]
public class TGEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        TG terrainGenerator = (TG)target;

        if (DrawDefaultInspector())
        {
            if (terrainGenerator.AutoUpdate)
            {
                terrainGenerator.Generate();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            terrainGenerator.Generate();

        }
    }
}

