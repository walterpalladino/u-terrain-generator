using System.Collections;
using System.Collections.Generic;
using System.IO;
using Unity.Burst.CompilerServices;
using Unity.VisualScripting;
using UnityEditor;
using UnityEngine;


[CustomEditor(typeof(UTRoadGenerator))]
public class UTRoadGeneratorEditor : Editor
{

    void OnSceneGUI()
    {
        UTRoadGenerator generator = (UTRoadGenerator)target;

        /*
        Event e = Event.current;

        if (e.type == EventType.MouseDown && e.button == 2)
        {
            Debug.Log("Middle Mouse was pressed");


            Vector3 mousePos = e.mousePosition;
            float ppp = EditorGUIUtility.pixelsPerPoint;
            mousePos.y = Camera.current.pixelHeight - mousePos.y * ppp;
            mousePos.x *= ppp;
//            SceneView.currentDrawingSceneView.camera
            Ray ray = Camera.current.ScreenPointToRay(mousePos);
            RaycastHit hit;

            if (Physics.Raycast(ray, out hit))
            {
                Debug.Log("Hit : " + hit.collider.gameObject.name + " at : " + hit.point);
            }
        }*/

        Event e = Event.current;
        //int controlID = GUIUtility.GetControlID(this.GetHashCode(), FocusType.Passive);

        if (e.type == EventType.MouseDown)
        {
            if (e.button == 0)
            {
                Ray worldRay = HandleUtility.GUIPointToWorldRay(e.mousePosition);
                RaycastHit hitInfo;

                if (Physics.Raycast(worldRay, out hitInfo))
                {
                    //Undo.RegisterUndo(target, "Add Path Node");
                    //((Path)target).AddNode(hitInfo.point);
                    Debug.Log("Hit : " + hitInfo.collider.gameObject.name + " at : " + hitInfo.point);

                    if (hitInfo.collider.gameObject.GetComponent<UTRoadGenerator>() != null) {
                        generator.AddNodeAt(hitInfo.point);
                    }

                }
                
                e.Use();
            }
        }

        if (Event.current.type == EventType.Layout)
        {
            HandleUtility.AddDefaultControl(0);
        }
    }


    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        UTRoadGenerator generator = (UTRoadGenerator)target;
        
        if (DrawDefaultInspector())
        {
            /*
            if (generator.AutoUpdate)
            {
                generator.Clear();
                generator.Generate();
            }
            */
        }
        
        if (GUILayout.Button("Generate"))
        {
            generator.Generate();
        }
        /*
        if (GUILayout.Button("Add River"))
        {
            generator.AddRiver();
        }
        */
        if (GUILayout.Button("Clear"))
        {
            generator.Clear();
        }

        //EditorGUILayout.HelpBox("Test", MessageType.Info, false);
    }
}
