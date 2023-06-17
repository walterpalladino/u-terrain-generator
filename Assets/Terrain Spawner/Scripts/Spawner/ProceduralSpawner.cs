using System;
using System.Collections;
using System.Collections.Generic;
using UnityEditor;
using UnityEngine;
using Random = UnityEngine.Random;





public enum PSHit
{
    NO_HIT,
    UNKNOWN_HIT,
    TERRAIN_HIT,
    SPECIAL_AREA_HIT,
    PLACEHOLDER_HIT
}


public class ProceduralSpawner : MonoBehaviour
{

    [Header("Tree Settings")]
    [SerializeField]
    private bool treesEnabled = true;
    [SerializeField]
    private GameObject[] trees;
    [SerializeField]
    private GameObject treesParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float treePresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int treeGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float treeGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float treeFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float treeMaxSlope = 20.0f;
    [SerializeField]
    private float treeMinAltitude = 0.0f;
    [SerializeField]
    private float treeMaxAltitude = 20.0f;


    [Header("Grass Settings")]
    [SerializeField]
    private bool grassEnabled = true;
    [SerializeField]
    private GameObject[] grasses;
    [SerializeField]
    private GameObject grassParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float grassPresence = 0.5f;
    [SerializeField]
    [Range(1, 20)]
    private int grassGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float grassGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.1f, 5.0f)]
    private float grassFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float grassMaxSlope = 30.0f;
    [SerializeField]
    private float grassMinAltitude = 0.0f;
    [SerializeField]
    private float grassMaxAltitude = 100.0f;


    [Header("Bushes Settings")]
    [SerializeField]
    private bool bushesEnabled = true;
    [SerializeField]
    private GameObject[] bushes;
    [SerializeField]
    private GameObject bushesParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float bushesPresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int bushesGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float bushesGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float bushesFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float bushesMaxSlope = 20.0f;
    [SerializeField]
    private float bushesMinAltitude = 0.0f;
    [SerializeField]
    private float bushesMaxAltitude = 100.0f;


    [Header("Rocks Settings")]
    [SerializeField]
    private bool rocksEnabled = true;
    [SerializeField]
    private GameObject[] rocks;
    [SerializeField]
    private GameObject rocksParent;
    [SerializeField]
    [Range(0.0f, 1.0f)]
    private float rocksPresence = 0.5f;
    [SerializeField]
    [Range(1, 10)]
    private int rocksGroupSize = 1;
    [SerializeField]
    [Range(1.0f, 15.0f)]
    private float rocksGroupRadius = 1.0f;
    [SerializeField]
    [Range(0.5f, 5.0f)]
    private float rocksFreeRadius = 0.5f;
    [SerializeField]
    [Range(0.0f, 90.0f)]
    private float rocksMaxSlope = 20.0f;
    [SerializeField]
    private float rocksMinAltitude = 0.0f;
    [SerializeField]
    private float rocksMaxAltitude = 100.0f;




    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;

    [SerializeField]
    [Range(1, 5)]
    private int maxTriesToLocateObjects = 3;


    //public PSTerrainGenerator terrainGenerator;

    private Dictionary<string, GameObject> treesDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> rocksDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> bushesDictionary = new Dictionary<string, GameObject>();
    private Dictionary<string, GameObject> grassDictionary = new Dictionary<string, GameObject>();

    private List<GameObject> treesList = new List<GameObject>();
    private List<GameObject> rocksList = new List<GameObject>();
    private List<GameObject> bushesList = new List<GameObject>();
    private List<GameObject> grassList = new List<GameObject>();


    [Header("Terrain Settings")]
    [SerializeField] float terrainTileSize;

    [SerializeField] Vector3 terrainMin;
    [SerializeField] Vector3 terrainMax;

    [SerializeField] float freeBorderSize;

    [SerializeField] Vector3 terrainCenter;


    [SerializeField] LayerMask terrainIgnoreLayers;

    [SerializeField] float maxDistanceFromCenter = 0.0f;

    [Range(0.0001f, 1000.0f)]
    [SerializeField] float noiseScale = 1.0f;
    [Range(1, 8)]
    [SerializeField] int noiseOcatves = 1;
    [Range(0.0f, 1.0f)]
    [SerializeField] float noisePersistance = 0.5f;
    [Range(1.0f, 16.0f)]
    [SerializeField] float noiseLacunarity = 1.0f;

    //[SerializeField]
    PSTerrain[] terrainObjects;



    // Start is called before the first frame update
    void Start()
    {

        CollectObjects();

    }

    // Update is called once per frame
    void Update()
    {

    }

    public void Generate()
    {

        CheckRequiredLayers();

        Random.InitState(randomSeed);
        NoiseGenerator.Init();

        treesList = new List<GameObject>();
        rocksList = new List<GameObject>();
        bushesList = new List<GameObject>();
        grassList = new List<GameObject>();


        //terrainGenerator = FindObjectOfType<PSTerrainGenerator>();

        CollectTerrainTiles();

        SetObjectsForTerrain();
        //StartCoroutine("SpawnAllObjectsInMap");
    }

    private void CollectTerrainTiles()
    {

        terrainObjects = FindObjectsOfType<PSTerrain>();
        Debug.Log("Found : " + terrainObjects.Length + " terrain objects in the scene.");

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach(PSTerrain terrain in terrainObjects)
        {
            Vector3 terrainMin = terrain.gameObject.GetComponent<MeshRenderer>().bounds.min;
            Vector3 terrainMax = terrain.gameObject.GetComponent<MeshRenderer>().bounds.max;

            min = Vector3.Min(min, terrainMin);
            max = Vector3.Max(max, terrainMax);
        }

        this.terrainMin = min;
        this.terrainMax = max;

        Debug.Log(" from " + terrainMin + " to " + terrainMax);
    }



    /*

    Layers Required:

    16 - Terrain
    17 - Trees
    18 - Vegetation
    19 - Rocks
    20 - Buildings
    21 - Grass
    //22 - Special Area

    */
    private void CheckRequiredLayers()
    {
        LayerUtils.CreateLayer("Terrain", 16);
        LayerUtils.CreateLayer("Trees", 17);
        LayerUtils.CreateLayer("Vegetation", 18);
        LayerUtils.CreateLayer("Rocks", 19);
        LayerUtils.CreateLayer("Buildings", 20);
        LayerUtils.CreateLayer("Grass", 21);
        //LayerUtils.CreateLayer("Special Area", 22);
    }


    private void SetObjectsForTerrain()
    {

        float mapSizeX = terrainMax.x - terrainMin.x;
        float mapSizeY = terrainMax.y - terrainMin.y;
        float mapSizeZ = terrainMax.z - terrainMin.z;

        terrainCenter = new Vector3(mapSizeX / 2.0f, mapSizeY / 2.0f, mapSizeZ / 2.0f);
        terrainCenter += terrainMin; 

        int chunksX = (int)(mapSizeX) / (int)(terrainTileSize) ;
        int chunksZ = (int)(mapSizeZ) / (int)(terrainTileSize) ;

        for (int z = 0; z < chunksZ; z++)
        {
            for (int x = 0; x < chunksX; x++)
            {
                PlaceObjects(x, z, terrainTileSize);
            }
        }

    }

    public void PlaceObjects(int x, int z, float terrainTileSize)
    {

        float xMin = x * terrainTileSize + terrainMin.x;
        float zMin = z * terrainTileSize + terrainMin.z;
        float xMax = (x + 1) * terrainTileSize + terrainMin.x;
        float zMax = (z + 1) * terrainTileSize + terrainMin.z;

        Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);

        PlaceTrees(xMin, zMin, xMax, zMax);
        PlaceRocks(xMin, zMin, xMax, zMax);
        PlaceBushes(xMin, zMin, xMax, zMax);

        PlaceGrass(xMin, zMin, xMax, zMax);
    }


    public static void ChangeLayersRecursively(GameObject go, string name)
    {
        go.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in go.transform)
        {
            ChangeLayersRecursively(child.gameObject, name);
        }
    }

    private void PlaceBushes(float xMin, float zMin, float xMax, float zMax)
    {
        if (bushesEnabled)
        {

            int groupsQty = (int)((xMax - xMin) * (zMax - zMin) / (2 * bushesGroupRadius) / (2 * bushesGroupRadius));

            GameObject go = new GameObject();
            go.transform.parent = bushesParent.transform;
            go.name = "" + xMin + "/" + zMin;


            for (int n = 0; n < groupsQty; n++)
            {

                PSSpawnInformation spawnInformation;

                //  Get the group center position
                Vector3 centerGroup;
                if (!GetGridRandomPosition(xMin, zMin, xMax, zMax, out centerGroup, out spawnInformation))
                {
                    continue;
                }

                for (int t = 0; t < bushesGroupSize; t++)
                {
                    Vector3 position;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, bushesGroupRadius, bushesMaxSlope, bushesMinAltitude, bushesMaxAltitude, bushesFreeRadius, maxTriesToLocateObjects, out position))
                    {

                        //  Value based on the terrain mask
                        float noiseValue = NoiseGenerator.GetNoiseAt(position.x, position.z, noiseScale, noiseOcatves, noisePersistance, noiseLacunarity);
                        //  Random value to compare
                        float randomValue = Random.Range(0.0f, 1.0f);
                        //  The lower the chances on the terrain and the presence the lower the chance to display an element
                        if (randomValue <= noiseValue * bushesPresence)
                        {

                            Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                            GameObject prefab = GetRandomPrefab( (spawnInformation == null) ? bushes : spawnInformation.bushesPrefabs );

                            if (prefab != null)
                            {
                                GameObject instance = Instantiate(prefab, position, orientation);
                                instance.transform.parent = go.transform;

                                ChangeLayersRecursively(instance, "Vegetation");
                            }
                        }

                    }
                }

            }
            //Debug.Log("Placed " + bushesPlaced + " bushes of " + bushesGroupsQty + " groups of " + bushesGroupSize + " bush.");
        }

    }

    private void PlaceRocks(float xMin, float zMin, float xMax, float zMax)
    {
        if (rocksEnabled)
        {

            int groupsQty = (int)((xMax - xMin) * (zMax - zMin) / (2 * rocksGroupRadius) / (2 * rocksGroupRadius));

            GameObject go = new GameObject();
            go.transform.parent = rocksParent.transform;
            go.name = "" + xMin + "/" + zMin;

            for (int n = 0; n < groupsQty; n++)
            {

                PSSpawnInformation spawnInformation;

                //  Get the group center position
                Vector3 centerGroup;
                if (!GetGridRandomPosition(xMin, zMin, xMax, zMax, out centerGroup, out spawnInformation))
                {
                    continue;
                }

                for (int t = 0; t < rocksGroupSize; t++)
                {

                    Vector3 position;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, rocksGroupRadius, rocksMaxSlope, rocksMinAltitude, rocksMaxAltitude, rocksFreeRadius, maxTriesToLocateObjects, out position))
                    {
                        //  Value based on the terrain mask
                        float noiseValue = NoiseGenerator.GetNoiseAt(position.x, position.z, noiseScale, noiseOcatves, noisePersistance, noiseLacunarity);
                        //  Random value to compare
                        float randomValue = Random.Range(0.0f, 1.0f);
                        //  The lower the chances on the terrain and the presence the lower the chance to display an element
                        if (randomValue <= noiseValue * rocksPresence)
                        {

                            Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                            GameObject prefab = GetRandomPrefab((spawnInformation == null) ? rocks : spawnInformation.rocksPrefabs);

                            if (prefab != null)
                            {
                                GameObject instance = Instantiate(prefab, position, orientation);
                                instance.transform.parent = go.transform;

                                ChangeLayersRecursively(instance, "Rocks");
                            }
                        }
                    }

                }

            }
            //Debug.Log("Placed " + rocksPlaced + " rocks of " + rocksGroupsQty + " groups of " + rocksGroupSize + " rocks.");
        }

    }

    private void PlaceTrees(float xMin, float zMin, float xMax, float zMax)
    {
        if (treesEnabled)
        {

            int groupsQty = (int)((xMax - xMin) * (zMax - zMin) / (2 * treeGroupRadius) / (2 * treeGroupRadius));

            GameObject go = new GameObject();
            go.transform.parent = treesParent.transform;
            go.name = "" + xMin + "/" + zMin;

            //  For every group
            for (int n = 0; n < groupsQty; n++)
            {

                PSSpawnInformation spawnInformation;

                //  Get the group center position
                Vector3 centerGroup ;
                if (!GetGridRandomPosition(xMin, zMin, xMax, zMax, out centerGroup, out spawnInformation))
                {
                    continue;
                }

                //  Place the elements of the group
                for (int t = 0; t < treeGroupSize; t++)
                {

                    Vector3 position;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, treeGroupRadius, treeMaxSlope, treeMinAltitude, treeMaxAltitude, treeFreeRadius, maxTriesToLocateObjects, out position))
                    {

                        //  Value based on the terrain mask
                        float noiseValue = NoiseGenerator.GetNoiseAt(position.x, position.z, noiseScale, noiseOcatves, noisePersistance, noiseLacunarity);
                        //  Random value to compare
                        float randomValue = Random.Range(0.0f, 1.0f);
                        //  The lower the chances on the terrain and the presence the lower the chance to display an element
                        if (randomValue <= noiseValue * treePresence) { 

                            //  Randomize the orientation
                            Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                            GameObject prefab = GetRandomPrefab( (spawnInformation == null) ? trees : spawnInformation.treesPrefabs );

                            if (prefab != null)
                            {
                                GameObject instance = Instantiate(prefab, position, orientation);
                                instance.transform.parent = go.transform;

                                ChangeLayersRecursively(instance, "Trees");

                                treesList.Add(instance);
                            }

                        }
                    }

                }

            }
            //Debug.Log("Placed " + treesPlaced + " trees of " + treesGroupsQty + " groups of " + treeGroupSize + " tress.");
        }

    }

    private void PlaceGrass(float xMin, float zMin, float xMax, float zMax)
    {
        if (grassEnabled)
        {

            int groupsQty = (int)((xMax - xMin) * (zMax - zMin) / (2 * grassGroupRadius) / (2 * grassGroupRadius));
            groupsQty *= 8; //  Magic Number
            //Debug.Log("Grass groups to place: " + groupsQty);

            GameObject go = new GameObject();
            go.transform.parent = grassParent.transform;
            go.name = "" + xMin + "/" + zMin;


            int ignoredQty = 0;
            int cantPlaceQty = 0;

            for (int n = 0; n < groupsQty; n++)
            {

                PSSpawnInformation spawnInformation;

                //  Get the group center position
                Vector3 centerGroup;
                if (!GetGridRandomPosition(xMin, zMin, xMax, zMax, out centerGroup, out spawnInformation))
                {
                    continue;
                }

                for (int t = 0; t < grassGroupSize; t++)
                {

                    Vector3 position;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, grassGroupRadius, grassMaxSlope, grassMinAltitude, grassMaxAltitude, grassFreeRadius, maxTriesToLocateObjects, out position))
                    {
                        
                        //  Value based on the terrain mask
                        float noiseValue = NoiseGenerator.GetNoiseAt(position.x, position.z, noiseScale, noiseOcatves, noisePersistance, noiseLacunarity);
                        //  Random value to compare
                        float randomValue = Random.Range(0.0f, 1.0f);
                        //  The lower the chances on the terrain and the presence the lower the chance to display an element
                        if (randomValue <= noiseValue * grassPresence)
                        {
                        
                            Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                            GameObject prefab = GetRandomPrefab( (spawnInformation == null) ? grasses : spawnInformation.grassPrefabs );

                            if (prefab != null)
                            {
                                GameObject instance = Instantiate(prefab, position, orientation);

                                instance.transform.parent = go.transform;

                                ChangeLayersRecursively(instance, "Grass");
                            }
                            
                        }
                        else
                        {
                            ignoredQty++;
                        }
                            
                    }
                    else
                    {
                        cantPlaceQty++;
                    }

                }

            }
            //Debug.Log("Ignored: " + ignoredQty + " / Cant Place: " + cantPlaceQty);
            //Debug.Log("Placed " + grassPlaced + " grass of " + grassGroupsQty + " groups of " + grassGroupSize + " grass.");
        }

    }


    private GameObject GetRandomPrefab(GameObject[] prefabs)
    {
        if (prefabs.Length > 0)
        {
            int idx = Random.Range(0, prefabs.Length);
            return prefabs[idx];
        }
        else
        {
            return null;
        }

    }

    private bool CheckOverlap(Vector3 position, float freeRadius)
    {

        foreach (GameObject go in treesList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in bushesList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in rocksList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        foreach (GameObject go in grassList)
        {
            if (Vector3.Distance(position, go.transform.position) <= freeRadius)
            {
                return true;
            }
        }

        return false;
    }


    /*
    private Vector3 GetRandomPosition()
    {
        Vector3 position = new Vector3(Random.Range(terrainMin.x + freeBorderSize, terrainMax.x - freeBorderSize), 0, Random.Range(terrainMin.z + freeBorderSize, terrainMax.z - freeBorderSize));

        return position;
    }
    */
    /*
        private Vector3 GetGridRandomPosition(float minX, float minZ, float maxX, float maxZ)
        {

            float xSize = (maxX - minX) ;
            float zSize = (maxZ - minZ) ;

            int xGridSize = (int)xSize;
            int zGridSize = (int)zSize;
            //Debug.Log("Grid Size x:" + xGridSize + " z:" + zGridSize);

            float xGridCellSize = (maxX - minX) / xGridSize;
            float zGridCellSize = (maxZ - minZ) / zGridSize;

            //  Select a grid position
            Vector3 position = new Vector3(Random.Range(0, xGridSize), 0, Random.Range(0, zGridSize));

            //  Adjust the position to the center of the grid cell
            position.x = ((int)position.x) + 0.5f;
            position.z = ((int)position.z) + 0.5f;

            //  Scale it based on the cell dimensions
            position.x *= xGridCellSize;
            position.z *= zGridCellSize;

            position.x += minX;
            position.z += minZ;

            return position;
        }
        */


    private bool GetGridRandomPosition(float minX, float minZ, float maxX, float maxZ, out Vector3 position, out PSSpawnInformation spawnInformation)
    {
        spawnInformation = null;

        position = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        Vector3 normal;
        GameObject hitGameObject;

        float height;
        PSHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != PSHit.TERRAIN_HIT)
        {
            return false;
        }
        position.y = height;


        //  Terrain
        if ((hitGameObject.GetComponent<PSTerrain>() != null) && (hitGameObject.GetComponent<PSTerrain>().useSpawnInformation))
        {
            spawnInformation = hitGameObject.GetComponent<PSTerrain>().spawnInformation;
        }

        return true;
    }
    
    private bool GetItemPosition(Vector3 centerGroup, float xMin, float zMin, float xMax, float zMax, float groupRadius, float maxSlope, float minAltitude, float maxAltitude, float freeRadius, int maxTries, out Vector3 position)
    {
        position = Vector3.zero;

        float height = 0;

        int tryCount = 0;
        bool validPosition = false;

        while (!validPosition && tryCount++ < maxTries)
        {
            //  Obtain a position
            position = GetPositionInArea(centerGroup, groupRadius);

            validPosition = CheckValidPosition(position, maxSlope, minAltitude, maxAltitude, out height);
            position.y = height;
        }

        return validPosition;
    }

    private bool CheckValidPosition(Vector3 position, float maxSlope, float minAltitude, float maxAltitude, out float height)
    {
        height = 0;

        //  Check values are in terrain boundaries
        if ((position.x < terrainMin.x + freeBorderSize) || (position.x > terrainMax.x - freeBorderSize) || (position.z < terrainMin.z + freeBorderSize) || (position.z > terrainMax.z - freeBorderSize))
        {
            //Debug.Log(position);
            return false;
        }

        //  Check if there is a maximum distance set and the distance to the center if bigger than that
        float distance = (position - terrainCenter).magnitude;
        if (maxDistanceFromCenter > 0.0f && maxDistanceFromCenter < distance)
        {
            return false;
        }

        Vector3 normal;
        GameObject hitGameObject;

        PSHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != PSHit.TERRAIN_HIT)
        {
            //Debug.Log("Not Terrain Hit " + hitGameObject.name);
            return false;
        }
        position.y = height;


        //  Check Min and Max Altitude
        if (height < minAltitude || height > maxAltitude)
        {
            //Debug.Log(height);
            //Debug.Log(position);
            return false;
        }
        /*
                if (CheckPlaceholderAt(position))
                {
                    return false;
                }
        */
        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        if (CheckOverlap(position, treeFreeRadius))
        {
            return false;
        }
        /*
                if (clearGrass)
                {
                    //  If overlaps any grass remove the grass
                    CleanGrassInArea(position, freeRadius);
                }
        */
        //  Return the position
        return true;

    }

    private Vector3 GetPositionInArea(Vector3 centerGroup, float groupRadius)
    {
        Vector3 position;

        //position = new Vector3(Random.Range(xMin - groupRadius, xMax + groupRadius), 0, Random.Range(zMin - groupRadius, zMax + groupRadius));
        position = new Vector3(Random.Range(centerGroup.x - groupRadius, centerGroup.x + groupRadius), 0, Random.Range(centerGroup.z - groupRadius, centerGroup.z + groupRadius));


        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, groupRadius);
        //position = centerGroup + groupRadius * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        position = centerGroup + distance * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));

        return position;
    }



    private PSHit CheckAt(Vector3 position, out float height, out Vector3 normal, out GameObject hitGameObject)
    {
        hitGameObject = null;
        height = -1;
        normal = Vector3.zero;

        position.y = 10000f;

        Ray ray = new Ray(position, Vector3.down);

        RaycastHit hit;

        if (Physics.Raycast(ray, out hit, Mathf.Infinity, ~terrainIgnoreLayers))

        {
            hitGameObject = hit.collider.gameObject;
            //Debug.Log("Hit object : " + hit.collider.gameObject.name + " with layer : " + LayerMask.LayerToName(hit.collider.gameObject.layer));

            if (hit.collider.gameObject.GetComponent<PSPlaceholder>() != null)
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.PLACEHOLDER_HIT;
            }
            else
            //            if (hit.collider.gameObject.GetComponent<PSTerrain>() != null)
            if (LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.TERRAIN_HIT;
            }
            //  Check terrain meshes using LOD
            //if (hit.collider.gameObject.name.Contains("LOD0") && hit.collider.transform.parent.gameObject.GetComponent<PSTerrain>() != null)
            if (hit.collider.gameObject.name.Contains("LOD0") && LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return PSHit.TERRAIN_HIT;
            }
            /*
            else
            {
                return PSHit.UNKNOWN_HIT;
            }
            */
        }

        return PSHit.NO_HIT;
    }


    public GameObject[] GetPois()
    {
        PSTown[] towns = FindObjectsOfType<PSTown>();

        GameObject[] pois = new GameObject[towns.Length];

        for (int n = 0; n < towns.Length; n++)
        {
            pois[n] = towns[n].gameObject;
        }

        return pois;
    }

    public PSRoad[] GetRoads()
    {
        PSRoad[] roads = GetComponent<PSRoadNetwork>().Roads;
        return roads;
    }

    public void Clear()
    {

        if (Application.isPlaying)
        {

            foreach (Transform child in grassParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in treesParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in bushesParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

            foreach (Transform child in rocksParent.transform)
            {
                GameObject.Destroy(child.gameObject);
            }

        }
        else
        {

            while (grassParent.transform.childCount != 0)
            {
                DestroyImmediate(grassParent.transform.GetChild(0).gameObject);
            }

            while (treesParent.transform.childCount != 0)
            {
                DestroyImmediate(treesParent.transform.GetChild(0).gameObject);
            }

            while (bushesParent.transform.childCount != 0)
            {
                DestroyImmediate(bushesParent.transform.GetChild(0).gameObject);
            }

            while (rocksParent.transform.childCount != 0)
            {
                DestroyImmediate(rocksParent.transform.GetChild(0).gameObject);
            }
        }
    }


    private void CollectObjects()
    {
        foreach (Transform child in treesParent.transform)
        {
            treesDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in rocksParent.transform)
        {
            rocksDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in bushesParent.transform)
        {
            bushesDictionary.Add(child.gameObject.name, child.gameObject);
        }
        foreach (Transform child in grassParent.transform)
        {
            grassDictionary.Add(child.gameObject.name, child.gameObject);
        }
    }

    /*
    public void HideObjects(int x, int z)
    {
        if (x < 0 || z < 0 || x >= terrainGenerator.terrainWidth || z >= terrainGenerator.terrainDepth)
        {
            return;
        }

        string key = "" + x * terrainGenerator.terrainTileSize + "/" + z * terrainGenerator.terrainTileSize;

        treesDictionary[key].SetActive(false);
        rocksDictionary[key].SetActive(false);
        bushesDictionary[key].SetActive(false);
        grassDictionary[key].SetActive(false);
    }

    public void ShowObjects(int x, int z)
    {
        if (x < 0 || z < 0 || x >= terrainGenerator.terrainWidth || z >= terrainGenerator.terrainDepth)
        {
            return;
        }

        string key = "" + x * terrainGenerator.terrainTileSize + "/" + z * terrainGenerator.terrainTileSize;

        treesDictionary[key].SetActive(true);
        rocksDictionary[key].SetActive(true);
        bushesDictionary[key].SetActive(true);
        grassDictionary[key].SetActive(true);
    }
    */
}
