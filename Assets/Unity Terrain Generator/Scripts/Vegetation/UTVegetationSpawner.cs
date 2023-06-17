using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;
using static UnityEditor.Progress;

public enum UTHit
{
    NO_HIT,
    UNKNOWN_HIT,
    TERRAIN_HIT,
    SPECIAL_AREA_HIT,
    PLACEHOLDER_HIT
}

public class UTVegetationSpawner : MonoBehaviour, IGenerator
{


    [Header("Tree Settings")]
    [SerializeField]
    private UTSpawnData treesData;

    [Header("Bushes Settings")]
    [SerializeField]
    private UTSpawnData bushesData;

    [Header("Grass Settings")]
    [SerializeField]
    private UTSpawnData grassData;

    [Header("Rocks Settings")]
    [SerializeField]
    private UTSpawnData rocksData;

    /*
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
    */
    /*
    [Header("Grass Settings")]
    [SerializeField]
    private bool grassEnabled = true;
    [SerializeField]
    private GameObject[] grasses;
//    [SerializeField]
//    private GameObject grassParent;
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
    */

    /*
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
    */

    /*
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
    */



    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;

    [SerializeField]
    [Range(1, 5)]
    private int maxTriesToLocateObjects = 3;


    //public PSTerrainGenerator terrainGenerator;

    //private Dictionary<string, GameObject> treesDictionary = new Dictionary<string, GameObject>();
    //private Dictionary<string, GameObject> rocksDictionary = new Dictionary<string, GameObject>();
    //private Dictionary<string, GameObject> bushesDictionary = new Dictionary<string, GameObject>();
    //private Dictionary<string, GameObject> grassDictionary = new Dictionary<string, GameObject>();

    private List<UTSpawnObject> treesList = new List<UTSpawnObject>();
    private List<UTSpawnObject> rocksList = new List<UTSpawnObject>();
    private List<UTSpawnObject> bushesList = new List<UTSpawnObject>();
    private List<UTSpawnObject> grassList = new List<UTSpawnObject>();


    [Header("Terrain Settings")]
    [SerializeField]
    Terrain[] terrains;

    [SerializeField] float terrainTileSize = 10;

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




    // Start is called before the first frame update
    void Start()
    {
        //InitData();
        //CollectObjects();
    }

    private void InitData()
    {
        treesData.spawnDataType = UTSpawnDataType.Tree;
        treesData.layerName = "Trees";

    }

    // Update is called once per frame
    void Update()
    {

    }


    public void Generate()
    {

        InitData();

        CheckRequiredLayers();

        Random.InitState(randomSeed);
        NoiseGenerator.Init();

        treesList = new List<UTSpawnObject>();
        rocksList = new List<UTSpawnObject>();
        bushesList = new List<UTSpawnObject>();
        grassList = new List<UTSpawnObject>();

        CollectTerrain();

        SpawnObjects();
    }

    public void Clear()
    {

        InitData();
        CollectTerrain();

        foreach (Terrain terrain in terrains)
        {
            //            List<TreeInstance> treeInstanceCollection = new List<TreeInstance>(terrain.terrainData.treeInstances);
            //          foreach(TreeInstance treeInstance in treeInstanceCollection){
            //        }

            //  Clear all trees
            terrain.terrainData.treeInstances = new TreeInstance[0];

            //  Clear all details (grass, etc)
            // Get all of layer zero.
            var map = terrain.terrainData.GetDetailLayer(
                0, 0,
                terrain.terrainData.detailWidth, terrain.terrainData.detailHeight,
                0);

            // For each pixel in the detail map...
            for (var y = 0; y < terrain.terrainData.detailHeight; y++)
            {
                for (var x = 0; x < terrain.terrainData.detailWidth; x++)
                {
                    map[x, y] = 0;
                }
            }

            // Assign the modified map back.
            terrain.terrainData.SetDetailLayer(0, 0, 0, map);
        }

        if (Application.isPlaying)
        {
            /*
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
            */
        }
        else
        {
            /*
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
            */
        }
    }

    /*
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
    */

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

    //  TODO : Check if boundary changes when changing the terrain position
    private void CollectTerrain()
    {

        terrains = FindObjectsOfType<Terrain>();
        Debug.Log("Found : " + terrains.Length + " terrain objects in the scene.");

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        foreach (Terrain terrain in terrains)
        {
            //Debug.Log(terrain.terrainData.size);
            Vector3 terrainMin = terrain.terrainData.bounds.min;
            Vector3 terrainMax = terrain.terrainData.bounds.max;

            terrainMin.y = 0;
            terrainMax.y = terrain.terrainData.size.y;

            min = Vector3.Min(min, terrainMin);
            max = Vector3.Max(max, terrainMax);
        }

        this.terrainMin = min;
        this.terrainMax = max;

        //
        float mapSizeX = terrainMax.x - terrainMin.x;
        float mapSizeY = terrainMax.y - terrainMin.y;
        float mapSizeZ = terrainMax.z - terrainMin.z;

        terrainCenter = new Vector3(mapSizeX / 2.0f, mapSizeY / 2.0f, mapSizeZ / 2.0f);
        terrainCenter += terrainMin;

        Debug.Log(" from " + terrainMin + " to " + terrainMax);
    }


    private void SpawnObjects()
    {

        foreach (Terrain terrain in terrains)
        {

            Vector3 terrainMin = terrain.terrainData.bounds.min;
            Vector3 terrainMax = terrain.terrainData.bounds.max;

            terrainMin.y = 0;
            terrainMax.y = terrain.terrainData.size.y;

            Vector3 terrainSize = terrain.terrainData.size;

            Vector3 terrainCenter = terrainSize / 2.0f;
            terrainCenter += terrainMin;

            PlaceTerrainObjects(terrain);

        }

    }


    /*
    private void SpawnObjects()
    {

        foreach (Terrain terrain in terrains)
        {

            Vector3 terrainMin = terrain.terrainData.bounds.min;
            Vector3 terrainMax = terrain.terrainData.bounds.max;

            terrainMin.y = 0;
            terrainMax.y = terrain.terrainData.size.y;

            Vector3 terrainSize = terrain.terrainData.size;

            Vector3 terrainCenter = terrainSize / 2.0f;
            terrainCenter += terrainMin;

            int chunksX = (int)(terrainSize.x) / (int)(terrainTileSize);
            int chunksZ = (int)(terrainSize.z) / (int)(terrainTileSize);

            for (int z = 0; z < chunksZ; z++)
            {
                for (int x = 0; x < chunksX; x++)
                {
                    PlaceTerrainObjects(terrain, x, z, terrainTileSize);
                }
            }

        }

    }
*/

    /*
private void SpawnObjects()
{

    float mapSizeX = terrainMax.x - terrainMin.x;
    float mapSizeY = terrainMax.y - terrainMin.y;
    float mapSizeZ = terrainMax.z - terrainMin.z;

    terrainCenter = new Vector3(mapSizeX / 2.0f, mapSizeY / 2.0f, mapSizeZ / 2.0f);
    terrainCenter += terrainMin;

    int chunksX = (int)(mapSizeX) / (int)(terrainTileSize);
    int chunksZ = (int)(mapSizeZ) / (int)(terrainTileSize);

    for (int z = 0; z < chunksZ; z++)
    {
        for (int x = 0; x < chunksX; x++)
        {
            PlaceTerrainObjects(x, z, terrainTileSize);
        }
    }

}
*/

    public void PlaceTerrainObjects(Terrain terrain)
    {
        Debug.Log("Placing objects on terrain : " + terrain.gameObject.name);

        //  Place Trees
        GenerateTerrainObjects(terrain, treesData, ref treesList);
        Debug.Log("Trees to instantiate : " + treesList.Count);
        InstantiateTreesOnTerrain(terrain, treesList);

        //  Place Rocks
        //  Place Bushes
        //  Place Grass
        GenerateGrass(terrain, grassData, ref grassList);
        Debug.Log("Grass to instantiate : " + grassList.Count);
    }

    /*
    public void PlaceTerrainObjects(Terrain terrain, int x, int z, float terrainTileSize)
    {

        float xMin = x * terrainTileSize + terrainMin.x;
        float zMin = z * terrainTileSize + terrainMin.z;
        float xMax = (x + 1) * terrainTileSize + terrainMin.x;
        float zMax = (z + 1) * terrainTileSize + terrainMin.z;

        Debug.Log("Placing objects on area : " + xMin + "/" + zMin + " to " + xMax + "/" + zMax);

        GenerateTerrainObjects(treesData, ref treesList, xMin, zMin, xMax, zMax);
        Debug.Log("Objects to instantiate : " + treesList.Count);
        InstantiateTreesOnTerrain(terrain, treesList);

        //PlaceTrees(xMin, zMin, xMax, zMax);
        //PlaceRocks(xMin, zMin, xMax, zMax);
        //PlaceBushes(xMin, zMin, xMax, zMax);

        //PlaceGrass(xMin, zMin, xMax, zMax);
    }
*/

    private void InstantiateTreesOnTerrain(Terrain terrain, List<UTSpawnObject> treesList)
    {

        List<TreeInstance> treeInstanceCollection = new List<TreeInstance>(terrain.terrainData.treeInstances);

        foreach (UTSpawnObject spawnObject in treesList)
        {
            TreeInstance treeInstance = new TreeInstance();

            treeInstance.heightScale = 1.0f;
            treeInstance.widthScale = 1.0f;

            treeInstance.prototypeIndex = spawnObject.prefabIndex;
            //treeInstance.position = spawnObject.position;
            treeInstance.position = new Vector3(spawnObject.position.x / terrain.terrainData.size.x, spawnObject.position.y, spawnObject.position.z / terrain.terrainData.size.z);
            treeInstance.rotation = spawnObject.rotation * Mathf.Deg2Rad;

            treeInstance.color = Color.white;
            treeInstance.lightmapColor = Color.white;

            treeInstanceCollection.Add(treeInstance);
        }

        //terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), false);
        terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), true);
    }

    private void GenerateTerrainObjects(Terrain terrain, UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        if (objectsData.enabled)
        {
            while (true)
            {

                Vector3 position = Vector3.zero;

                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                float height = terrain.terrainData.GetHeight((int)position.x, (int)position.z);
                //if (height > 0) Debug.Log(height);
                if (height < objectsData.minAltitude || height > objectsData.maxAltitude) continue;

                float steepness = terrain.terrainData.GetSteepness(position.x / (float)terrain.terrainData.size.x,
                                                               position.z / (float)terrain.terrainData.size.z);
                if (steepness > objectsData.maxSlope)
                {
                    //Debug.Log(steepness);
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();

                int idx = objectsData.prefabsIds[Random.Range(0, objectsData.prefabsIds.Length)];
                //  TODO : Add validation for existing index
                spawnObject.prefabIndex = idx;
                spawnObject.rotation = Random.Range(0, 359);
                spawnObject.position = position;
                spawnObject.scale = Random.Range(1.0f - objectsData.sizeVariation, 1.0f + objectsData.sizeVariation);

                instantiatedObjects.Add(spawnObject);

                count++;

                if (count >= objectsData.maxQuantity) break;
            }
        }

    }

    private void GenerateGrass(Terrain terrain, UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        if (objectsData.enabled)
        {
            while (true)
            {
                /*
                Vector3 position = Vector3.zero;

                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                float height = terrain.terrainData.GetHeight((int)position.x, (int)position.z);
                //if (height > 0) Debug.Log(height);
                if (height < objectsData.minAltitude || height > objectsData.maxAltitude) continue;

                float steepness = terrain.terrainData.GetSteepness(position.x / (float)terrain.terrainData.size.x,
                                                               position.z / (float)terrain.terrainData.size.z);
                if (steepness > objectsData.maxSlope)
                {
                    //Debug.Log(steepness);
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();

                int idx = objectsData.prefabsIds[Random.Range(0, objectsData.prefabsIds.Length)];
                //  TODO : Add validation for existing index
                spawnObject.prefabIndex = idx;
                spawnObject.rotation = Random.Range(0, 359);
                spawnObject.position = position;
                spawnObject.scale = Random.Range(1.0f - objectsData.sizeVariation, 1.0f + objectsData.sizeVariation);

                instantiatedObjects.Add(spawnObject);
                */
                count++;

                if (count >= objectsData.maxQuantity) break;
            }
        }
    }

    /*
    private void GenerateTerrainObjects(UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects, float xMin, float zMin, float xMax, float zMax)
    {

        if (objectsData.enabled)
        {

            int groupsQty = (int)((xMax - xMin) * (zMax - zMin) / (2 * objectsData.groupRadius) / (2 * objectsData.groupRadius));
            //Debug.Log("To create for this group : " + groupsQty);
            //  For every group
            for (int n = 0; n < groupsQty; n++)
            {
                //Debug.Log("Group : " + n);
                //  Get the group center position
                Vector3 centerGroup;
                if (!GetGridRandomPosition(xMin, zMin, xMax, zMax, out centerGroup))
                {
                    continue;
                }
                //Debug.Log("centerGroup : " + centerGroup);
                //  Place the elements of the group
                for (int t = 0; t < objectsData.groupSize; t++)
                {
                    //Debug.Log(" Actual from objectsData.groupSize : "+t);
                    Vector3 position;
                    if (GetItemPosition(centerGroup, xMin, zMin, xMax, zMax, objectsData.groupRadius, objectsData.maxSlope, objectsData.minAltitude, objectsData.maxAltitude, objectsData.freeRadius, maxTriesToLocateObjects, out position))
                    {
                        //Debug.Log("Object position : " + position);
                        //  Value based on the terrain mask
                        float noiseValue = NoiseGenerator.GetNoiseAt(position.x, position.z, noiseScale, noiseOcatves, noisePersistance, noiseLacunarity);
                        //  Random value to compare
                        float randomValue = Random.Range(0.0f, 1.0f);
                        //  The lower the chances on the terrain and the presence the lower the chance to display an element
                        if (randomValue <= noiseValue * objectsData.presence)
                        {

                            //  Randomize the orientation
                           // Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);
                            
                            //GameObject prefab = GetRandomPrefab(objectsData.prefabs);

                            //if (prefab != null)
                            //{
                              //  GameObject instance = Instantiate(prefab, position, orientation);
                                //instance.transform.parent = go.transform;

                                //ChangeLayersRecursively(instance, objectsData.layerName);

                                //instantiatedObjects.Add(instance);
                            //}
                            

                            UTSpawnObject spawnObject = new UTSpawnObject();

                            int idx = objectsData.prefabsIds[Random.Range(0, objectsData.prefabsIds.Length)];
                            //  TODO : Add validation for existing index
                            spawnObject.prefabIndex = idx;
                            spawnObject.rotation = Random.Range(0, 359);
                            spawnObject.position = position;

                            instantiatedObjects.Add(spawnObject);
                        }
                    }

                }

            }
            //Debug.Log("Placed " + treesPlaced + " trees of " + treesGroupsQty + " groups of " + treeGroupSize + " tress.");
        }

    }
*/
    /*
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
                Vector3 centerGroup;
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
                        if (randomValue <= noiseValue * treePresence)
                        {

                            //  Randomize the orientation
                            Quaternion orientation = Quaternion.AngleAxis(Random.Range(0, 359), Vector3.up);

                            GameObject prefab = GetRandomPrefab((spawnInformation == null) ? trees : spawnInformation.treesPrefabs);

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

    */

    private bool GetGridRandomPosition(float minX, float minZ, float maxX, float maxZ, out Vector3 position)
    {

        position = new Vector3(Random.Range(minX, maxX), 0, Random.Range(minZ, maxZ));

        Vector3 normal;
        GameObject hitGameObject;

        float height;
        UTHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != UTHit.TERRAIN_HIT)
        {
            return false;
        }
        position.y = height;

        return true;
    }




    private UTHit CheckAt(Vector3 position, out float height, out Vector3 normal, out GameObject hitGameObject)
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

                return UTHit.PLACEHOLDER_HIT;
            }
            else
            //            if (hit.collider.gameObject.GetComponent<PSTerrain>() != null)
            if (LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return UTHit.TERRAIN_HIT;
            }
            //  Check terrain meshes using LOD
            //if (hit.collider.gameObject.name.Contains("LOD0") && hit.collider.transform.parent.gameObject.GetComponent<PSTerrain>() != null)
            if (hit.collider.gameObject.name.Contains("LOD0") && LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return UTHit.TERRAIN_HIT;
            }
            /*
            else
            {
                return PSHit.UNKNOWN_HIT;
            }
            */
        }

        return UTHit.NO_HIT;
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
            Debug.Log("testing position : " + position);
            validPosition = CheckValidPosition(position, maxSlope, minAltitude, maxAltitude, out height);
            position.y = height;
        }

        return validPosition;
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

        UTHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //        if (!GetTerrainHeight(position, out height, out normal))
        if (psHit != UTHit.TERRAIN_HIT)
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

        if (CheckOverlap(position, treesData.freeRadius))
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



    private bool CheckOverlap(Vector3 position, float freeRadius)
    {
        /*
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
        */
        return false;
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


    public static void ChangeLayersRecursively(GameObject go, string name)
    {
        go.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in go.transform)
        {
            ChangeLayersRecursively(child.gameObject, name);
        }
    }

}

