using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(SegmentedPath))]
public class SegmentedPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        SegmentedPath path = (SegmentedPath)target;

        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate"))
        {

            //ClearMap();
            path.Clear();
            path.Generate();
        }

        if (GUILayout.Button("Clear"))
        {

            //ClearMap();
            path.Clear();
        }


    }
}
