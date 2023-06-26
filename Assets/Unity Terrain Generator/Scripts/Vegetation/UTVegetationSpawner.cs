using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.Analytics;
using UnityEngine.UIElements;
using static UnityEditor.Experimental.AssetDatabaseExperimental.AssetDatabaseCounters;
using static UnityEditor.Progress;

public enum UTHit
{
    NO_HIT,
    UNKNOWN_HIT,
    TERRAIN_HIT,
    SPECIAL_AREA_HIT,
    PLACEHOLDER_HIT
}

[RequireComponent(typeof(Terrain))]
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


    [Header("Random Settings")]
    [SerializeField]
    private int randomSeed = 0;

    [SerializeField]
    [Range(1, 5)]
    private int maxTriesToLocateObjects = 3;


    private List<UTSpawnObject> treesList = new List<UTSpawnObject>();
    private List<UTSpawnObject> rocksList = new List<UTSpawnObject>();
    private List<UTSpawnObject> bushesList = new List<UTSpawnObject>();
    private List<UTSpawnObject> grassList = new List<UTSpawnObject>();


    Terrain terrain;

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
    }

    // Update is called once per frame
    void Update()
    {

    }

    //
    public void Generate()
    {

#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Generating Vegetation...", "...", 0.0f);
#endif
        CheckRequiredLayers();

        InitData();

        Random.InitState(randomSeed);
        NoiseGenerator.Init();

        treesList = new List<UTSpawnObject>();
        rocksList = new List<UTSpawnObject>();
        bushesList = new List<UTSpawnObject>();
        grassList = new List<UTSpawnObject>();

        CollectTerrain();

        SpawnObjects();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }

    public void Clear()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Clear Vegetation", "Working...", 0.0f);
#endif

        CollectTerrain();
        InitData();

#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Clear Vegetation", "Clearing Trees...", 0.50f);
#endif
        //  Clear all Trees
        UTTerrainUtils.ClearTrees(terrain);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Clear Vegetation", "Clearing Detail Layers...", 0.750f);
#endif
        //  Clear all Rocks, Grass
        UTTerrainUtils.ClearAllDetailLayers(terrain);

#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif

    }
    

    private void InitData()
    {

        terrain.gameObject.layer = LayerMask.NameToLayer("Terrain");

        treesData.spawnDataType = UTSpawnDataType.Tree;
        treesData.layerName = "Trees";

        grassData.spawnDataType = UTSpawnDataType.GrassTexture;
        grassData.layerName = "Grass";

        rocksData.spawnDataType = UTSpawnDataType.DetailMesh;
        rocksData.layerName = "Rocks";
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
        UTLayerUtils.CreateLayer("Terrain", 16);
        UTLayerUtils.CreateLayer("Trees", 17);
        UTLayerUtils.CreateLayer("Vegetation", 18);
        UTLayerUtils.CreateLayer("Rocks", 19);
        UTLayerUtils.CreateLayer("Buildings", 20);
        UTLayerUtils.CreateLayer("Grass", 21);
        //UTLayerUtils.CreateLayer("Special Area", 22);
    }

    //  TODO : Check if boundary changes when changing the terrain position
    private void CollectTerrain()
    {

        terrain = GetComponent<Terrain>();

        Vector3 min = Vector3.positiveInfinity;
        Vector3 max = Vector3.negativeInfinity;

        //Debug.Log(terrain.terrainData.size);
        terrainMin = terrain.terrainData.bounds.min;
        terrainMax = terrain.terrainData.bounds.max;

        terrainMin.y = 0;
        terrainMax.y = terrain.terrainData.size.y;


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


        Vector3 terrainMin = terrain.terrainData.bounds.min;
        Vector3 terrainMax = terrain.terrainData.bounds.max;

        terrainMin.y = 0;
        terrainMax.y = terrain.terrainData.size.y;

        Vector3 terrainSize = terrain.terrainData.size;

        Vector3 terrainCenter = terrainSize / 2.0f;
        terrainCenter += terrainMin;

        PlaceTerrainObjects(terrain);


    }


    

    public void PlaceTerrainObjects(Terrain terrain)
    {
        Debug.Log("Placing objects on terrain : " + terrain.gameObject.name);


        //  Place Rocks
        GenerateRocks(terrain, rocksData, ref rocksList);
        Debug.Log("Rocks to instantiate : " + rocksList.Count);
        Debug.Log("terrain.terrainData.detailWidth " + terrain.terrainData.detailWidth + " / terrain.terrainData.detailHeight " + terrain.terrainData.detailHeight);
        InstantiateRocksOnTerrain(terrain, rocksList);

        //  Place Bushes

        //  Place Grass
        GenerateGrass(terrain, grassData, ref grassList);
        Debug.Log("Grass to instantiate : " + grassList.Count);
        Debug.Log("terrain.terrainData.detailWidth " + terrain.terrainData.detailWidth + " / terrain.terrainData.detailHeight " + terrain.terrainData.detailHeight);
        InstantiateGrassOnTerrain(terrain, grassList);


        //  Place Trees
        GenerateTrees(terrain, treesData, ref treesList);
        Debug.Log("Trees to instantiate : " + treesList.Count);
        InstantiateTreesOnTerrain(terrain, treesList);

    }

    private void InstantiateGrassOnTerrain(Terrain terrain, List<UTSpawnObject> grassList)
    {

#if UNITY_EDITOR
        int count = 0;
#endif

        DetailPrototype[] details = terrain.terrainData.detailPrototypes;
        //Debug.Log("Found details numbers: " + details.Length);
        for (int i = 0; i < details.Length; i++)
        //foreach(DetailPrototype detail in details)
        {
            //Debug.Log("Processing details : " + i);

            DetailPrototype detail = details[i];
            if (detail.renderMode == DetailRenderMode.GrassBillboard)
            {

                int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];
                foreach (UTSpawnObject spawnObject in grassList)
                {

                    if (spawnObject.prefabIndex == i)
                    {

                        float x = spawnObject.position.x * (float)terrain.terrainData.detailWidth / (float)terrain.terrainData.size.x;
                        float z = spawnObject.position.z * (float)terrain.terrainData.detailHeight / (float)terrain.terrainData.size.z;

                        map[(int)z, (int)x] = 1;

#if UNITY_EDITOR
                        count++;
                        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Grass...", $"Grass {count} of {grassList.Count} Instantiated.", (float)count / (float)treesList.Count);
#endif

                    }

                }

                terrain.terrainData.SetDetailLayer(0, 0, i, map);

            }
        }


    }

    private void InstantiateRocksOnTerrain(Terrain terrain, List<UTSpawnObject> rocksList)
    {

#if UNITY_EDITOR
        int count = 0;
#endif

        DetailPrototype[] details = terrain.terrainData.detailPrototypes;
        //Debug.Log("Found details numbers: " + details.Length);
        for (int i = 0; i < details.Length; i++)
        //foreach(DetailPrototype detail in details)
        {
            //Debug.Log("Processing details : " + i);

            DetailPrototype detail = details[i];
            if (detail.renderMode == DetailRenderMode.VertexLit)
            {

                int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];
                foreach (UTSpawnObject spawnObject in rocksList)
                {

                    if (spawnObject.prefabIndex == i)
                    {

                        float x = spawnObject.position.x * (float)terrain.terrainData.detailWidth / (float)terrain.terrainData.size.x;
                        float z = spawnObject.position.z * (float)terrain.terrainData.detailHeight / (float)terrain.terrainData.size.z;

                        map[(int)z, (int)x] = 1;

#if UNITY_EDITOR
                        count++;
                        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Rocks...", $"Rocks {count} of {rocksList.Count} Instantiated.", (float)count / (float)treesList.Count);
#endif

                    }

                }

                terrain.terrainData.SetDetailLayer(0, 0, i, map);

            }
        }


    }

   
    //  Trees
    private void GenerateTrees(Terrain terrain, UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects)
    {
        if (objectsData.enabled)
        {

            objectsData.maxQuantity = (int)(terrain.terrainData.size.x * terrain.terrainData.size.z / objectsData.freeRadius * objectsData.presence);

            int count = 0;
            int groupObjectCount = 0;
            Vector3 centerGroupPosition = Vector3.zero;
            int prefabIdx = 0;

            while (count < objectsData.maxQuantity)
            {
                count++;

                if (groupObjectCount == 0) {
                    groupObjectCount = objectsData.groupSize;
                    if (groupObjectCount == 0) {
                        groupObjectCount = 1;
                    }

                    // Get the group center position
                    centerGroupPosition = GetCenterGroupPosition(terrain, objectsData);
                    //                    centerGroupPosition = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                    //  Every tree in the group will be the same prefab
                    //  TODO : Add validation for existing index
                    prefabIdx = objectsData.prefabsIds[Random.Range(0, objectsData.prefabsIds.Length)];
                }

                groupObjectCount--;

                Vector3 position = GetPositionInGroup(centerGroupPosition, objectsData.groupRadius);

                float height;
                if (!CheckValidPosition(terrain,position,objectsData, out height)) {
                    continue;
                }
                position.y = height - objectsData.sinkBottom;

                if (CheckPositionOverlap(instantiatedObjects, objectsData, position)) {
                    continue;
                }
                /*
                GameObject hitGameObject;
                float height;
                Vector3 normal;
                UTHit psHit = CheckAt( position, out height, out normal, out hitGameObject);
                if (psHit != UTHit.TERRAIN_HIT) {
                    continue;
                }
                position.y = height;
                */
//                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                //float height = terrain.terrainData.GetHeight((int)position.z, (int)position.x);
                //float height = terrain.SampleHeight(position);
                //if (height > 0) Debug.Log(height);
//                if (height < objectsData.minAltitude || height > objectsData.maxAltitude) continue;
                /*
                float normalizedX = position.x / (float)terrain.terrainData.size.x;
                float normalizedY = position.z / (float)terrain.terrainData.size.z;

                float steepness = terrain.terrainData.GetSteepness(normalizedY, normalizedX);
                */
//                float stepness = UTTerrainUtils.GetStepness(terrain, position);
//                if (stepness > objectsData.maxSlope)
//                {
                    //Debug.Log(steepness);
                    //Debug.Log("rejected by steepness");
                    //count++;
//                    continue;
//                }

                if ((Random.value) > objectsData.presence)
                {
                    //Debug.Log("rejected by presence");
                    //count++;
                    continue;
                }
                //Debug.Log(count);

                UTSpawnObject spawnObject = new UTSpawnObject();

                spawnObject.prefabIndex = prefabIdx;
                spawnObject.rotation = Random.Range(0, 359);
                spawnObject.position = position;
                spawnObject.scale = Random.Range(1.0f - objectsData.sizeVariation, 1.0f + objectsData.sizeVariation);

                instantiatedObjects.Add(spawnObject);

                //count++;


#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Trees...", $"Tree {count} of {objectsData.maxQuantity} Created.", (float)count / (float)objectsData.maxQuantity);
#endif

//                if (count >= objectsData.maxQuantity) break;
            }
        }

    }

    

    private void InstantiateTreesOnTerrain(Terrain terrain, List<UTSpawnObject> treesList)
    {

        List<TreeInstance> treeInstanceCollection = new List<TreeInstance>(terrain.terrainData.treeInstances);
#if UNITY_EDITOR
        int count = 0;
#endif
        foreach (UTSpawnObject spawnObject in treesList)
        {
            TreeInstance treeInstance = new TreeInstance();

            treeInstance.heightScale = 1.0f;
            treeInstance.widthScale = 1.0f;

            treeInstance.prototypeIndex = spawnObject.prefabIndex;

            //  Position need to be normalized (scaled from 0 to 1) based on the terrain width and height
            Vector3 normalizedPosition = new Vector3(spawnObject.position.x / terrain.terrainData.size.x, spawnObject.position.y / terrain.terrainData.size.y, spawnObject.position.z / terrain.terrainData.size.z);
            treeInstance.position = normalizedPosition;
            treeInstance.rotation = spawnObject.rotation * Mathf.Deg2Rad;

            treeInstance.color = Color.white;
            treeInstance.lightmapColor = Color.white;

            treeInstanceCollection.Add(treeInstance);

            ClearCircularArea(terrain, spawnObject.position, treesData.freeRadius);

#if UNITY_EDITOR
            count++;
            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Trees...", $"Tree {count} of {treesList.Count} Instantiated.", (float)count / (float)treesList.Count);
#endif

        }

        terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), false);
        //terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), true);

        terrain.Flush();
    }


    //  Grass
    private void GenerateGrass(Terrain terrain, UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        int control = 0;
        int maxCount = terrain.terrainData.detailWidth * terrain.terrainData.detailHeight;

        if (objectsData.enabled)
        {
            while (true)
            {
                Vector3 position = Vector3.zero;

                //position = new Vector3(Random.Range(0, terrain.terrainData.detailWidth), 0, Random.Range(0, terrain.terrainData.detailHeight));
                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                //XZ world position
                //Vector3 wPos = terrain.DetailToWorld(position.z, position.x);

                float height = terrain.SampleHeight(position);

                if (height < objectsData.minAltitude || height > objectsData.maxAltitude) continue;

                float stepness = UTTerrainUtils.GetStepness(terrain, position);
                if (stepness > objectsData.maxSlope)
                {
                    //Debug.Log(steepness);
                    control++;
                    continue;
                }
                /*
                //  Check trees overlaps
                if (CheckPositionOverlap(treesList, objectsData, position))
                {
                    control++;
                    continue;
                }
                */

                if (Random.value > objectsData.presence)
                {
                    control++;
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
                control++;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Grass...", $"Grass {count} of {objectsData.maxQuantity} Created.", (float)count / (float)objectsData.maxQuantity);
#endif
                if (count >= objectsData.maxQuantity) break;
                if (control >= maxCount) break;

            }
        }
    }

    //  Rocks
    private void GenerateRocks(Terrain terrain, UTSpawnData objectsData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        int control = 0;
        int maxCount = terrain.terrainData.detailWidth * terrain.terrainData.detailHeight;

        if (objectsData.enabled)
        {
            while (true)
            {
                Vector3 position = Vector3.zero;

                //position = new Vector3(Random.Range(0, terrain.terrainData.detailWidth), 0, Random.Range(0, terrain.terrainData.detailHeight));
                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                //XZ world position
                //Vector3 wPos = terrain.DetailToWorld(position.z, position.x);

                float height = terrain.SampleHeight(position);

                if (height < objectsData.minAltitude || height > objectsData.maxAltitude) continue;

                float stepness = UTTerrainUtils.GetStepness(terrain, position);
                if (stepness > objectsData.maxSlope)
                {
                    //Debug.Log(steepness);
                    control++;
                    continue;
                }

                if (Random.value > objectsData.presence)
                {
                    control++;
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
                control++;

#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Rocks...", $"Rocks {count} of {objectsData.maxQuantity} Created.", (float)count / (float)objectsData.maxQuantity);
#endif
                if (count >= objectsData.maxQuantity) break;
                if (control >= maxCount) break;

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


    /*

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
        }

        return UTHit.NO_HIT;
    }
*/

    /*
    private bool GetItemPosition(Vector3 centerGroup, float xMin, float zMin, float xMax, float zMax, float groupRadius, float maxSlope, float minAltitude, float maxAltitude, float freeRadius, int maxTries, out Vector3 position)
    {
        position = Vector3.zero;

        float height = 0;

        int tryCount = 0;
        bool validPosition = false;

        while (!validPosition && tryCount++ < maxTries)
        {
            //  Obtain a position
            position = GetPositionInGroup(centerGroup, groupRadius);
            Debug.Log("testing position : " + position);
            validPosition = CheckValidPosition(position, maxSlope, minAltitude, maxAltitude, out height);
            position.y = height;
        }

        return validPosition;
    }*/

    private Vector3 GetPositionInGroup(Vector3 centerGroup, float groupRadius)
    {
        Vector3 position;

        position = new Vector3(Random.Range(centerGroup.x - groupRadius, centerGroup.x + groupRadius), 0, Random.Range(centerGroup.z - groupRadius, centerGroup.z + groupRadius));
        
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, groupRadius);
        position = centerGroup + distance * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        
        return position;
    }

    /*
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
        
         //       if (CheckPlaceholderAt(position))
         //       {
         //           return false;
         //       }
        
        //  Check max slope
        if (Vector3.Angle(Vector3.up, normal) > maxSlope)
        {
            return false;
        }

        if (CheckOverlap(position, treesData.freeRadius))
        {
            return false;
        }

        //        if (clearGrass)
        //{
        //  If overlaps any grass remove the grass
        //CleanGrassInArea(position, freeRadius);
        //}

        //  Return the position
        return true;

    }
    */


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

    /*
    public static void ChangeLayersRecursively(GameObject go, string name)
    {
        go.layer = LayerMask.NameToLayer(name);
        foreach (Transform child in go.transform)
        {
            ChangeLayersRecursively(child.gameObject, name);
        }
    }*/


    private Vector3 GetCenterGroupPosition(Terrain terrain, UTSpawnData objectsData)
    {
        /*
        float x = (int)terrain.terrainData.size.x / objectsData.groupRadius;
        x = x + objectsData.groupRadius + objectsData.groupRadius / 2;

        float z = (int)terrain.terrainData.size.z / objectsData.groupRadius;
        z = z + objectsData.groupRadius + objectsData.groupRadius / 2;
        */
        Vector3 centerGroupPosition = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

        float areaRadius = (objectsData.groupRadius + objectsData.freeRadius);

        centerGroupPosition.x = (int)centerGroupPosition.x / (int)areaRadius;
        centerGroupPosition.x = centerGroupPosition.x * (int)areaRadius + (int)areaRadius / 2;

        centerGroupPosition.z = (int)centerGroupPosition.z / (int)areaRadius;
        centerGroupPosition.z = centerGroupPosition.z * (int)areaRadius + (int)areaRadius / 2;

        return centerGroupPosition;
    }

    private bool CheckPositionOverlap(List<UTSpawnObject> instantiatedObjects, UTSpawnData objectsData, Vector3 position)
    {
        foreach (UTSpawnObject spawnObject in instantiatedObjects)
        {
            if (Vector3.Distance(position, spawnObject.position) < 2.0f * objectsData.freeRadius)
            {
                return true;
            }
        }

        return false;
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

            if (LayerMask.LayerToName(hit.collider.gameObject.layer).Equals("Terrain"))
            {
                height = hit.point.y;
                normal = hit.normal;

                return UTHit.TERRAIN_HIT;
            }
            else
            {
                return UTHit.UNKNOWN_HIT;
            }
        }

        return UTHit.NO_HIT;
    }

    private bool CheckValidPosition(Terrain terrain, Vector3 position, UTSpawnData spawnData, out float height)
    {
        height = 0;

        //  Check values are in terrain boundaries
        if ((position.x < terrain.terrainData.bounds.min.x) ||
            (position.x > terrain.terrainData.bounds.max.x) ||
            (position.z < terrain.terrainData.bounds.min.z) ||
            (position.z > terrain.terrainData.bounds.max.z))
        {
            //Debug.Log(position);
            return false;
        }

        Vector3 normal;
        GameObject hitGameObject;

        UTHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //  Check if the position is directly over terrain
        if (psHit != UTHit.TERRAIN_HIT)
        {
            //Debug.Log("Not Terrain Hit " + hitGameObject.name);
            return false;
        }
        position.y = height;

        //  Check Min and Max Altitude
        if (height < spawnData.minAltitude || height > spawnData.maxAltitude)
        {
            //Debug.Log(height);
            //Debug.Log(position);
            return false;
        }

        //  Check max slope
        float stepness = UTTerrainUtils.GetStepness(terrain, position);
        if (stepness > spawnData.maxSlope)
        {
            //Debug.Log(steepness);
            //Debug.Log("rejected by steepness");
            return false;
        }

        //  Return a valid position
        return true;

    }


    private void ClearArea(Terrain terrain, Vector3 center, float width, float height)
    {
    }

    private void ClearCircularArea (Terrain terrain, Vector3 position, float radius) {

        DetailPrototype[] details = terrain.terrainData.detailPrototypes;
        
        for (int i = 0; i < details.Length; i++)
        {
            int[,] map = terrain.terrainData.GetDetailLayer(0, 0, terrain.terrainData.detailWidth, terrain.terrainData.detailHeight, i);

            Vector2Int detailSpaceCenter = new Vector2Int(
                (int)(position.z * (float)terrain.terrainData.detailHeight / (float)terrain.terrainData.size.z),
                (int)(position.x * (float)terrain.terrainData.detailWidth / (float)terrain.terrainData.size.x));

            int detailSpaceRadius = (int)(radius * (float)terrain.terrainData.detailWidth / (float)terrain.terrainData.size.x);

            for (int x = detailSpaceCenter.x - detailSpaceRadius; x < detailSpaceCenter.x + detailSpaceRadius; x++)
            {
                for (int y = detailSpaceCenter.y - detailSpaceRadius; y < detailSpaceCenter.y + detailSpaceRadius; y++)
                {

                    if (x < 0 ||
                        x >= terrain.terrainData.detailWidth ||
                        y < 0 ||
                        y >= terrain.terrainData.detailHeight) {
                        continue;
                    }

                    if (Vector2Int.Distance(new Vector2Int(x, y), detailSpaceCenter) <= detailSpaceRadius)
                    {
                        map[x, y] = 0;
                    }
                }
            }

            terrain.terrainData.SetDetailLayer(0, 0, i, map);
        }
    }


}

