using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(PSTown))]
public class PSTownEditor : Editor
{
    public override void OnInspectorGUI()
    {

        PSTown town = (PSTown)target;

        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate"))
        {
            town.Generate();
        }

        if (GUILayout.Button("Clear"))
        {
            town.Clear();
        }

    }

}
