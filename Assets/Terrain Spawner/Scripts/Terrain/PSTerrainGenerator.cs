using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PSTerrainGenerator : MonoBehaviour
{

    [Header("Terrain Settings")]
    [SerializeField]
    private bool terrainGenerationEnabled = true;

    [SerializeField]
    private GameObject terrainTilesParent;
    [SerializeField]
    [Range(4, 16)]
    public int terrainWidth = 4;
    [SerializeField]
    [Range(4, 16)]
    public int terrainDepth = 4;
    [SerializeField]
    public float terrainTileSize = 100.0f;


    [Header("Flat Terrain")]
    [SerializeField]
    private GameObject[] terrainTilesFlat;

    [SerializeField]
    private bool terrainAddHills = true;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float hillsPresence = 0.5f;
    [SerializeField]
    private GameObject[] terrainTilesHill;

    [Header("River Tiles")]
    [SerializeField]
    private bool terrainAddRivers = true;
    //    [Range(0.0f, 1.0f)]
    //    [SerializeField]
    //    private float riversPresence = 0.5f;
    [SerializeField]
    private GameObject[] terrainTilesRiver;
    [SerializeField]
    private bool terrainAddRiverTerminatorAtStart = true;
    [SerializeField]
    private bool terrainAddRiverTerminatorAtEnd = true;
    [SerializeField]
    private GameObject terrainTilesRiverEnd;


    [Header("Lake Tiles")]
    [SerializeField]
    private bool terrainAddLakes = true;
    [Range(0.0f, 1.0f)]
    [SerializeField]
    private float lakesPresence = 0.5f;
    [SerializeField]
    private GameObject[] terrainTilesLakes;


    [Header("Border Tiles")]
    [SerializeField]
    private bool terrainAddBorders = true;
    [SerializeField]
    private GameObject[] terrainTilesBorders;


    public GameObject[] terrainTiles;
    int maxHills;
    int hillsAdded;
    int maxLakes;
    int lakesAdded;


    private PSTerrain[] terrainObjects;
    [SerializeField]
    public Vector3 terrainMin = new Vector3(Mathf.Infinity, Mathf.Infinity, Mathf.Infinity);
    [SerializeField]
    public Vector3 terrainMax = new Vector3(-Mathf.Infinity, -Mathf.Infinity, -Mathf.Infinity);


    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;


    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public void Generate()
    {
        Random.InitState(randomSeed);

        if (terrainGenerationEnabled)
        {
            PlaceTerrainTiles();
            AddBoundaries();
        }

        terrainObjects = FindObjectsOfType<PSTerrain>();
        Debug.Log("Found : " + terrainObjects.Length + " terrain objects in the scene.");
        terrainMin = new Vector3(0, 0, 0);
        terrainMax = new Vector3(terrainWidth * terrainTileSize, 0, terrainDepth * terrainTileSize);

        Debug.Log(" from " + terrainMin + " to " + terrainMax);

    }

    private void AddBoundaries()
    {
        //  Instantiate Boundary gameObject
        GameObject go = new GameObject();
        go.transform.parent = terrainTilesParent.transform;
        go.name = "Boundaries";

        //  Add 4 colliders to close the area
        BoxCollider bcX0 = go.AddComponent<BoxCollider>();
        bcX0.center = new Vector3(0.0f, 40.0f, terrainDepth * terrainTileSize / 2.0f);
        bcX0.size = new Vector3(1.0f, 100.0f, terrainDepth * terrainTileSize);

        BoxCollider bcX1 = go.AddComponent<BoxCollider>();
        bcX1.center = new Vector3(terrainWidth * terrainTileSize, 40.0f, terrainDepth * terrainTileSize / 2.0f);
        bcX1.size = new Vector3(1.0f, 100.0f, terrainDepth * terrainTileSize);

        BoxCollider bcZ0 = go.AddComponent<BoxCollider>();
        bcZ0.center = new Vector3(terrainWidth * terrainTileSize / 2.0f, 40.0f, 0.0f);
        bcZ0.size = new Vector3(terrainWidth * terrainTileSize, 100.0f, 1.0f);

        BoxCollider bcZ1 = go.AddComponent<BoxCollider>();
        bcZ1.center = new Vector3(terrainWidth * terrainTileSize / 2.0f, 40.0f, terrainDepth * terrainTileSize);
        bcZ1.size = new Vector3(terrainWidth * terrainTileSize, 100.0f, 1.0f);
    }

    private void PlaceTerrainTiles()
    {
        Debug.Log("Starting terrain generation.");

        terrainTiles = new GameObject[terrainWidth * terrainDepth];

        AddRivers();
        AddLakes();

        maxHills = (!terrainAddHills) ? 0 : (int)(hillsPresence * (terrainWidth * terrainDepth));
        Debug.Log("Max Hills to place : " + maxHills);
        hillsAdded = 0;
        //int maxRivers = (!terrainAddHills) ? 0 : (int)(hillsPresence * (terrainWidth * terrainDepth));
        maxLakes = (!terrainAddLakes) ? 0 : (int)(lakesPresence * (terrainWidth * terrainDepth));
        Debug.Log("Max Lakes to place : " + maxLakes);
        lakesAdded = 0;

        for (int z = 0; z < terrainDepth; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                if (terrainTiles[x * terrainWidth + z] == null)
                {
                    AddTerrainTile(x, z);
                }
            }
        }

        AddBorders();

        Debug.Log("Completed terrain generation.");
    }

    private void AddBorders()
    {
        if (!terrainAddBorders)
        {
            return;
        }

        int z ;
        int x ;

        z = -1;
        for (x = -1; x <= terrainWidth; x ++)
        {
            AddBorderTileAt(x, z);
        }
        z = terrainDepth;
        for (x = -1; x <= terrainWidth; x++)
        {
            AddBorderTileAt(x, z);
        }

        x = -1;
        for (z = 0; z < terrainWidth; z++)
        {
            AddBorderTileAt(x, z);
        }
        x = terrainWidth;
        for (z = 0; z < terrainDepth; z++)
        {
            AddBorderTileAt(x, z);
        }

    }

    private void AddBorderTileAt(int x, int z)
    {
        Vector3 terrainPosition = new Vector3(x * terrainTileSize, 0, z * terrainTileSize);

        GameObject terrainPrefab;

        int idx = Random.Range(0, terrainTilesBorders.Length);
        terrainPrefab = terrainTilesBorders[idx];

        GameObject instance = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
        instance.transform.parent = terrainTilesParent.transform;
    }

    private void AddRivers()
    {
        if (!terrainAddRivers)
        {
            return;
        }

        int z = 0;
        int x = Random.Range(1, terrainDepth - 1);

        int tileOffset = 0;

        int length = (terrainWidth > terrainTilesRiver.Length) ? terrainTilesRiver.Length : terrainWidth;
        Debug.Log("length : " + length);
        if (length < terrainTilesRiver.Length)
        {
            tileOffset = Random.Range(0, terrainTilesRiver.Length - length);
        }
        Debug.Log("tileOffset : " + tileOffset);


        GameObject riverEndPrefab = terrainTilesRiverEnd;

        if (terrainAddRiverTerminatorAtStart)
        {
            Vector3 riverStartPosition = new Vector3(x * terrainTileSize, 0, 0);
            GameObject riverStartInstance = Instantiate(riverEndPrefab, riverStartPosition, Quaternion.identity);
            riverStartInstance.transform.parent = terrainTilesParent.transform;
        }

        if (terrainAddRiverTerminatorAtStart)
        {
            Vector3 riverEndPosition = new Vector3(x * terrainTileSize, 0, (z + length) * terrainTileSize);
            GameObject riverEndInstance = Instantiate(riverEndPrefab, riverEndPosition, Quaternion.identity);
            riverEndInstance.transform.parent = terrainTilesParent.transform;
        }


        for (int count = 0; count < length; count++)
        {
            Debug.Log("tileIdx : " + (tileOffset + count));
            GameObject riverPrefab = terrainTilesRiver[tileOffset + count];
            Vector3 riverPosition = new Vector3(x * terrainTileSize, 0, (z + count) * terrainTileSize);

            GameObject instance = Instantiate(riverPrefab, riverPosition, Quaternion.identity);
            instance.transform.parent = terrainTilesParent.transform;

            terrainTiles[x * terrainWidth + (z + count)] = instance;
        }

    }

    private void AddLakes()
    {
        if (!terrainAddLakes)
        {
            return;
        }


        for (int z = 0; z < terrainDepth; z++)
        {
            for (int x = 0; x < terrainWidth; x++)
            {
                if (terrainTiles[x * terrainWidth + z] == null)
                {
                    AddLakeTile(x, z);
                }
            }
        }

    }

    private void AddLakeTile(int x, int z)
    {

        Vector3 terrainPosition = new Vector3(x * terrainTileSize, 0, z * terrainTileSize);

        GameObject terrainPrefab;

        int orientation = Random.Range(0, 3);

        if ((Random.Range(0.0f, 1.0f) < lakesPresence) && (lakesAdded < maxLakes))
        {
            int idx = Random.Range(0, terrainTilesLakes.Length);
            terrainPrefab = terrainTilesLakes[idx];
            lakesAdded++;
            Debug.Log("Added Lake : " + lakesAdded + " of : " + maxLakes);


            GameObject instance = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
            instance.transform.parent = terrainTilesParent.transform;
            instance.transform.Rotate(0, orientation * 90.0f, 0);

            Vector3 offset = Vector3.zero;
            if (orientation == 2 || orientation == 3)
            {
                offset.x = terrainTileSize;
            }
            if (orientation == 1 || orientation == 2)
            {
                offset.z = terrainTileSize;
            }
            instance.transform.position = instance.transform.position + offset;

            terrainTiles[x * terrainWidth + z] = instance;

        }

    }

    private void AddTerrainTile(int x, int z)
    {

        Vector3 terrainPosition = new Vector3(x * terrainTileSize, 0, z * terrainTileSize);

        GameObject terrainPrefab;

        int orientation = Random.Range(0, 3);

        if (terrainAddHills && (Random.Range(0.0f, 1.0f) < hillsPresence) && (hillsAdded < maxHills))
        {
            int idx = Random.Range(0, terrainTilesFlat.Length);
            terrainPrefab = terrainTilesHill[idx];
            hillsAdded++;
            Debug.Log("Added Hill : " + hillsAdded + " of : " + maxHills);
        }
        else
        {
            int idx = Random.Range(0, terrainTilesFlat.Length);
            terrainPrefab = terrainTilesFlat[idx];
        }

        GameObject instance = Instantiate(terrainPrefab, terrainPosition, Quaternion.identity);
        instance.transform.parent = terrainTilesParent.transform;
        instance.transform.Rotate(0, orientation * 90.0f, 0);

        Vector3 offset = Vector3.zero;
        if (orientation == 2 || orientation == 3)
        {
            offset.x = terrainTileSize;
        }
        if (orientation == 1 || orientation == 2)
        {
            offset.z = terrainTileSize;
        }
        instance.transform.position = instance.transform.position + offset;

        //ChangeLayersRecursively(instance, "Terrain");

        terrainTiles[x * terrainWidth + z] = instance;

    }
    /*
    public static void ChangeLayersRecursively(GameObject go, string name)
    {
        go.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in go.transform)
        {
            ChangeLayersRecursively(child.gameObject, name);
        }
    }
    */
    public void Clear()
    {

        if (Application.isPlaying)
        {


        }
        else
        {

            if (terrainGenerationEnabled)
            {
                while (terrainTilesParent.transform.childCount != 0)
                {
                    DestroyImmediate(terrainTilesParent.transform.GetChild(0).gameObject);
                }
            }

        }
    }


    public bool CheckAt(Vector3 position, out Vector3 hitPosition)
    {

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~LayerMask.NameToLayer("Terrain")))
        {
            Debug.Log("Hit : " + hit.collider.gameObject.layer);
            hitPosition = hit.point;
            return true;
        }
        else
        {
            Debug.Log("Hit Nothing");
            hitPosition = Vector3.zero;
            return false;
        }

    }

}
