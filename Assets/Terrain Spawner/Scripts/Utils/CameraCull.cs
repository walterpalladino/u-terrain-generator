using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


/*
 * Add this script to the camera to set several values for culling objects based on the layer
 */

public class CameraCull : MonoBehaviour
{

    [SerializeField] float terrainDistanceCulling = 1000.0f;    //  16 - Terrain
    [SerializeField] float treesDistanceCulling = 500.0f;       //  17 - Trees
    [SerializeField] float vegetationDistanceCulling = 200.0f;  //  18 - Vegetation
    [SerializeField] float rocksDistanceCulling = 200.0f;       //  19 - Rocks
    [SerializeField] float buildingsDistanceCulling = 200.0f;   //  20 - Buildings
    [SerializeField] float grassDistanceCulling = 50.0f;        //  21 - Grass


    // Start is called before the first frame update
    void Start()
    {
        Init();
    }


    // Update is called once per frame
    void Update()
    {
        
    }


    private void Init()
    {
        CheckRequiredLayers();

        Camera camera = GetComponent<Camera>();
        float[] distances = new float[32];

        distances[LayerMask.NameToLayer("Terrain")] = terrainDistanceCulling;
        distances[LayerMask.NameToLayer("Trees")] = treesDistanceCulling;
        distances[LayerMask.NameToLayer("Vegetation")] = vegetationDistanceCulling;
        distances[LayerMask.NameToLayer("Rocks")] = rocksDistanceCulling;
        distances[LayerMask.NameToLayer("Buildings")] = buildingsDistanceCulling;
        distances[LayerMask.NameToLayer("Grass")] = grassDistanceCulling;

        camera.layerCullDistances = distances;
    }


    /*

Layers Required:

16 - Terrain
17 - Trees
18 - Vegetation
19 - Rocks
20 - Buildings
21 - Grass


*/
    private void CheckRequiredLayers()
    {
        LayerUtils.CreateLayer("Terrain", 16);
        LayerUtils.CreateLayer("Trees", 17);
        LayerUtils.CreateLayer("Vegetation", 18);
        LayerUtils.CreateLayer("Rocks", 19);
        LayerUtils.CreateLayer("Buildings", 20);
        LayerUtils.CreateLayer("Grass", 21);
    }


}
