using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using static UnityEngine.GraphicsBuffer;

[CustomEditor(typeof(UTTerrainGenerator))]
public class UTTerrainGeneratorEditor : Editor
{
    void OnSceneGUI()
    {
        UTTerrainGenerator generator = (UTTerrainGenerator)target;
    }

    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        UTTerrainGenerator generator = (UTTerrainGenerator)target;

        if (DrawDefaultInspector())
        {
            if (generator.AutoUpdate)
            {
                generator.Clear();
                generator.Generate();
            }
        }

        if (GUILayout.Button("Generate"))
        {
            generator.Clear();
            generator.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
        }

        //EditorGUILayout.HelpBox("Test", MessageType.Info, false);
    }
}
