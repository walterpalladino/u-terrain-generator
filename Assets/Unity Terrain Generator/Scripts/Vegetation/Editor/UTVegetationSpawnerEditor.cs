using System.Collections;
using System.Collections.Generic;
using Palmmedia.ReportGenerator.Core;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UTVegetationSpawner))]
public class UTVegetationSpawnerEditor : Editor
{
    void OnSceneGUI()
    {
        UTVegetationSpawner generator = (UTVegetationSpawner)target;
    }

    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        UTVegetationSpawner generator = (UTVegetationSpawner)target;

        if (DrawDefaultInspector())
        {
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


    }

}
