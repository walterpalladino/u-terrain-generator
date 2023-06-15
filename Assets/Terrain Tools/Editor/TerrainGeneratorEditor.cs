using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(TerrainGenerator))]
public class TerrainGeneratorEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        TerrainGenerator mapGenerator = (TerrainGenerator)target;

        if (DrawDefaultInspector())
        {
            if (mapGenerator._autoUpdate)
            {
                mapGenerator.GenerateMap();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            mapGenerator.GenerateMap();
            
        }
    }
}
