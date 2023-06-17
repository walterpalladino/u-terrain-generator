using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PSTerrainGenerator))]
public class ProceduralTerrainEditor : Editor
{
    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        PSTerrainGenerator generator = (PSTerrainGenerator)target;

        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate"))
        {

            //ClearMap();
            generator.Clear();
            generator.Generate();
        }

        if (GUILayout.Button("Clear"))
        {

            //ClearMap();
            generator.Clear();
        }


    }

}
