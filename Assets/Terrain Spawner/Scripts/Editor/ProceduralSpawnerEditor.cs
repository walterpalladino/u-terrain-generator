using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEditor;


[CustomEditor(typeof(ProceduralSpawner))]
public class ProceduralSpawnerEditor : Editor
{


    void OnSceneGUI()
    {
        ProceduralSpawner spawner = (ProceduralSpawner)target;
        /*
        Handles.color = Color.yellow;

        PSRoad[] roads = spawner.GetRoads();
        Debug.Log("Roads qty :" + roads.Length);
        for (int r = 0; r < roads.Length; r++)
        {
            PSRoad road = roads[r];
            Debug.Log("Road [" + r + "] : Nodes qty : " + road.Nodes.Length);
            for (int n = 1; n < road.Nodes.Length; n++)
            {
                Handles.DrawLine(road.Nodes[n - 1].transform.position, road.Nodes[n].transform.position);
            }
        }
        */
        /*
        GameObject[] pois = spawner.GetPois();
        if (pois == null)
        {
            return;
        }

        Handles.color = Color.yellow;

        Vector3 start = pois[0].transform.position;
        for (int i = 1; i < pois.Length; i++)
        {
            GameObject poi = pois[i];
            if (poi)
            {
                Handles.DrawLine(start, poi.transform.position);
                start = poi.transform.position;
            }
        }
        */



    }

    public override void OnInspectorGUI()
    {
        //        base.OnInspectorGUI();

        ProceduralSpawner spawner = (ProceduralSpawner)target;

        if (DrawDefaultInspector())
        {
/*            if (spawner.AutoUpdate)
            {

                Clear();

                spawner.Generate();
            }*/
        }

        if (GUILayout.Button("Generate"))
        {

            //ClearMap();
            spawner.Clear();
            spawner.Generate();
        }

        if (GUILayout.Button("Clear"))
        {

            //ClearMap();
            spawner.Clear();
        }


    }

    private void Clear()
    {
/*        GameObject[] actualCells = GameObject.FindGameObjectsWithTag("Hex Cell");

        for (var i = 0; i < actualCells.Length; i++)
        {
            DestroyImmediate(actualCells[i]);
        }
*/
    }

}
