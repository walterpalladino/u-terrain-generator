using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(BezierPath))]
public class BezierPathEditor : Editor
{
    public override void OnInspectorGUI()
    {
        BezierPath bezierPath = (BezierPath)target;

        if (DrawDefaultInspector())
        {
        }

        if (GUILayout.Button("Generate"))
        {

            //ClearMap();
            bezierPath.Clear();
            bezierPath.Generate();
        }

        if (GUILayout.Button("Clear"))
        {

            //ClearMap();
            bezierPath.Clear();
        }


    }

}
