using System.Collections;
using System.Collections.Generic;
using TreeEditor;
using Unity.VisualScripting;
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
    private UTTreesData treesSpawnData;

    [Header("Bushes Settings")]
    [SerializeField]
    private UTBushesData bushesData;

    [Header("Grass Settings")]
    [SerializeField]
    private UTGrassData grassData;

    [Header("Rocks Settings")]
    [SerializeField]
    private UTRocksData rocksData;



    [Header("General Settings")]
    [SerializeField] int randomSeed = 0;
    [SerializeField] LayerMask terrainIgnoreLayers;



    //  Private lists to store data to spawn
    List<UTSpawnObject> treesList = new List<UTSpawnObject>();
    List<UTSpawnObject> rocksList = new List<UTSpawnObject>();
    List<UTSpawnObject> bushesList = new List<UTSpawnObject>();
    List<UTSpawnObject> grassList = new List<UTSpawnObject>();


    Terrain terrain;



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

        //  Clear all Rocks, Grass
        UTTerrainUtils.ClearAllDetailLayers(terrain);

        UpdateTreesTemplates(terrain);
        UpdateDetailTemplates(terrain);

        PlaceTerrainObjects(terrain);


#if UNITY_EDITOR
        UnityEditor.EditorUtility.ClearProgressBar();
#endif
    }


    public void Clear()
    {
#if UNITY_EDITOR
        UnityEditor.EditorUtility.DisplayCancelableProgressBar("Clear Vegetation", "Working...", 0.0f);
#endif

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
        terrain = GetComponent<Terrain>();
        terrain.gameObject.layer = LayerMask.NameToLayer("Terrain");
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


    private void UpdateDetailTemplates(Terrain terrain)
    {

        //  Add detail layers for: Grass, Rocks, Bushes
        //  Add grass layers
        //  Add rocks layers
        //  Add bushes layers

        DetailPrototype[] detailPrototypes;
        detailPrototypes = new DetailPrototype[grassData.templates.Length + bushesData.templates.Length + rocksData.templates.Length];
        int idx = 0;

        //  Grass
        for (int i = 0; i < grassData.templates.Length; i++)
        {

            detailPrototypes[idx] = new DetailPrototype();
            detailPrototypes[idx].prototype = null;
            detailPrototypes[idx].usePrototypeMesh = false;
            detailPrototypes[idx].prototypeTexture = grassData.templates[i].texture;

            detailPrototypes[idx].healthyColor = grassData.templates[i].healthyColor;
            detailPrototypes[idx].dryColor = grassData.templates[i].dryColor;

            detailPrototypes[idx].renderMode = DetailRenderMode.GrassBillboard;

            if (grassData.templates[i].minSize > grassData.templates[i].maxSize)
            {
                detailPrototypes[idx].minHeight = grassData.templates[i].minSize;
                detailPrototypes[idx].minWidth = grassData.templates[i].minSize;

                detailPrototypes[idx].maxHeight = grassData.templates[i].maxSize;
                detailPrototypes[idx].maxWidth = grassData.templates[i].maxSize;
            }
            else
            {
                detailPrototypes[idx].minHeight = grassData.templates[i].maxSize;
                detailPrototypes[idx].minWidth = grassData.templates[i].maxSize;

                detailPrototypes[idx].maxHeight = grassData.templates[i].minSize;
                detailPrototypes[idx].maxWidth = grassData.templates[i].minSize;
            }

            grassData.templates[i].prefabId = idx;

            idx++;
        }

        //  Bushes
        for (int i = 0; i < bushesData.templates.Length; i++)
        {
            detailPrototypes[idx] = new DetailPrototype();
            detailPrototypes[idx].prototype = bushesData.templates[i].mesh;

            detailPrototypes[idx].usePrototypeMesh = true;
            detailPrototypes[idx].prototypeTexture = null;

            detailPrototypes[idx].useInstancing = bushesData.templates[i].useGpuInstancing;
            detailPrototypes[idx].renderMode = DetailRenderMode.VertexLit;

            if (rocksData.templates[i].minSize > bushesData.templates[i].maxSize)
            {
                detailPrototypes[idx].minHeight = bushesData.templates[i].minSize;
                detailPrototypes[idx].minWidth = bushesData.templates[i].minSize;

                detailPrototypes[idx].maxHeight = bushesData.templates[i].maxSize;
                detailPrototypes[idx].maxWidth = bushesData.templates[i].maxSize;
            }
            else
            {
                detailPrototypes[idx].minHeight = bushesData.templates[i].maxSize;
                detailPrototypes[idx].minWidth = bushesData.templates[i].maxSize;

                detailPrototypes[idx].maxHeight = bushesData.templates[i].minSize;
                detailPrototypes[idx].maxWidth = bushesData.templates[i].minSize;
            }

            bushesData.templates[i].prefabId = idx;

            idx++;
        }

        //  Rocks
        for (int i = 0; i < rocksData.templates.Length; i++)
        {
            detailPrototypes[idx] = new DetailPrototype();
            detailPrototypes[idx].prototype = rocksData.templates[i].mesh;

            detailPrototypes[idx].usePrototypeMesh = true;
            detailPrototypes[idx].prototypeTexture = null;

            detailPrototypes[idx].useInstancing = rocksData.templates[i].useGpuInstancing;
            detailPrototypes[idx].renderMode = DetailRenderMode.VertexLit;

            if (rocksData.templates[i].minSize > rocksData.templates[i].maxSize)
            {
                detailPrototypes[idx].minHeight = rocksData.templates[i].minSize;
                detailPrototypes[idx].minWidth = rocksData.templates[i].minSize;

                detailPrototypes[idx].maxHeight = rocksData.templates[i].maxSize;
                detailPrototypes[idx].maxWidth = rocksData.templates[i].maxSize;
            }
            else
            {
                detailPrototypes[idx].minHeight = rocksData.templates[i].maxSize;
                detailPrototypes[idx].minWidth = rocksData.templates[i].maxSize;

                detailPrototypes[idx].maxHeight = rocksData.templates[i].minSize;
                detailPrototypes[idx].maxWidth = rocksData.templates[i].minSize;
            }

            rocksData.templates[i].prefabId = idx;

            idx++;
        }

        terrain.terrainData.detailPrototypes = detailPrototypes;
    }

    private void UpdateTreesTemplates(Terrain terrain)
    {
        //  Refresh terrain trees list
        TreePrototype[] newTreePrototypes;
        newTreePrototypes = new TreePrototype[treesSpawnData.treeTemplates.Length];
        int tindex = 0;
        foreach (UTTreeTemplate treeTemplate in treesSpawnData.treeTemplates)
        {
            newTreePrototypes[tindex] = new TreePrototype();
            newTreePrototypes[tindex].prefab = treeTemplate.mesh;
            treeTemplate.prefabId = tindex;
            tindex++;
        }
        terrain.terrainData.treePrototypes = newTreePrototypes;
        
        terrain.terrainData.RefreshPrototypes();
    }

    public void PlaceTerrainObjects(Terrain terrain)
    {
        Debug.Log("Placing objects on terrain : " + terrain.gameObject.name);
        Debug.Log("terrain.terrainData.detailWidth " + terrain.terrainData.detailWidth + " / terrain.terrainData.detailHeight " + terrain.terrainData.detailHeight);

        //  Place Rocks
        GenerateRocks(terrain, rocksData, ref rocksList);
        Debug.Log("Rocks to instantiate : " + rocksList.Count);

        //  Place Bushes
        GenerateBushes(terrain, bushesData, ref bushesList);
        Debug.Log("Bushes to instantiate : " + bushesList.Count);
        
        //  Place Grass
        GenerateGrass(terrain, grassData, ref grassList);
        Debug.Log("Grass to instantiate : " + grassList.Count);

        List<UTSpawnObject> objects = new List<UTSpawnObject>(rocksList);
        objects.AddRange(new List<UTSpawnObject>(grassList));
        objects.AddRange(new List<UTSpawnObject>(bushesList));

        //  Grass, Rocks, Bushes
        InstantiateDetailMeshesOnTerrain(terrain, objects);

        //  Place Trees
        GenerateTrees(terrain, treesSpawnData, ref treesList);
        Debug.Log("Trees to instantiate : " + treesList.Count);
        InstantiateTreesOnTerrain(terrain, treesList);

    }




    
    private void InstantiateDetailMeshesOnTerrain(Terrain terrain, List<UTSpawnObject> objectsList) {

#if UNITY_EDITOR
        int count = 0;
#endif

        DetailPrototype[] details = terrain.terrainData.detailPrototypes;

        for (int i = 0; i < details.Length; i++)
        {
            DetailPrototype detail = details[i];

            //  We are clearing the layer and re fill it
            int[,] map = new int[terrain.terrainData.detailWidth, terrain.terrainData.detailHeight];

            foreach (UTSpawnObject spawnObject in objectsList)
            {
                if (spawnObject.prefabIndex == i)
                {
                    float x = spawnObject.position.x * (float)terrain.terrainData.detailWidth / (float)terrain.terrainData.size.x;
                    float z = spawnObject.position.z * (float)terrain.terrainData.detailHeight / (float)terrain.terrainData.size.z;

                    map[(int)z, (int)x] = 1;

#if UNITY_EDITOR
                    count++;
                    UnityEditor.EditorUtility.DisplayCancelableProgressBar("Instantiating Details...", $"Added {count} of {objectsList.Count}.", (float)count / (float)objectsList.Count);
#endif

                }
            }
            terrain.terrainData.SetDetailLayer(0, 0, i, map);
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

            treeInstance.heightScale = spawnObject.scale;
            treeInstance.widthScale = spawnObject.scale;

            treeInstance.prototypeIndex = spawnObject.prefabIndex;

            //  Position need to be normalized (scaled from 0 to 1) based on the terrain width and height
            Vector3 normalizedPosition = new Vector3(spawnObject.position.x / terrain.terrainData.size.x, spawnObject.position.y / terrain.terrainData.size.y, spawnObject.position.z / terrain.terrainData.size.z);
            treeInstance.position = normalizedPosition;
            treeInstance.rotation = spawnObject.rotation * Mathf.Deg2Rad;

            Color color = Color.Lerp(spawnObject.color1, spawnObject.color2, Random.Range(0.0f, 1.0f));
            treeInstance.color = color;
            treeInstance.lightmapColor = Color.white;

            treeInstanceCollection.Add(treeInstance);

            ClearCircularArea(terrain, spawnObject.position, spawnObject.freeRadius);

#if UNITY_EDITOR
            count++;
            UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Trees...", $"Tree {count} of {treesList.Count} Instantiated.", (float)count / (float)treesList.Count);
#endif

        }

        //  We will not snap to the heightmap so we can sunk it a bit. Good for slopes.
        terrain.terrainData.SetTreeInstances(treeInstanceCollection.ToArray(), false);

        terrain.Flush();

    }


    //  Trees
    private void GenerateTrees(Terrain terrain, UTTreesData treesData, ref List<UTSpawnObject> instantiatedObjects)
    {
        if (treesData.enabled)
        {

            treesData.maxQuantity = (int)(terrain.terrainData.size.x * terrain.terrainData.size.z * treesData.presence);

            int count = 0;
            int groupObjectCount = 0;
            Vector3 centerGroupPosition = Vector3.zero;
            int prefabIdx = 0;

            while (count < treesData.maxQuantity)
            {
                count++;

                if (groupObjectCount == 0) {

                    //  Every tree in the group will be the same prefab
                    prefabIdx = Random.Range(0, treesData.treeTemplates.Length);

                    groupObjectCount = treesData.treeTemplates[prefabIdx].groupSize;
                    if (groupObjectCount == 0) {
                        groupObjectCount = 1;
                    }

                    // Get the group center position
                    centerGroupPosition = GetCenterGroupPosition(terrain, treesData.treeTemplates[prefabIdx].groupRadius, treesData.treeTemplates[prefabIdx].freeRadius);
                }

                groupObjectCount--;

                Vector3 position = GetPositionInGroup(centerGroupPosition, treesData.treeTemplates[prefabIdx].groupRadius);

                float height;
                if (!CheckValidPosition(terrain,position, treesData.treeTemplates[prefabIdx].minAltitude, treesData.treeTemplates[prefabIdx].maxAltitude, treesData.treeTemplates[prefabIdx].maxSlope, out height)) {
                    continue;
                }
                position.y = height - treesData.treeTemplates[prefabIdx].sinkBottom;

                if (CheckPositionOverlap(instantiatedObjects, treesData.treeTemplates[prefabIdx].freeRadius, position)) {
                    continue;
                }

                if ((Random.value) > treesData.presence)
                {
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();

                spawnObject.prefabIndex = prefabIdx;
                spawnObject.rotation = Random.Range(0, 359);
                spawnObject.position = position;
                spawnObject.scale = Random.Range(treesData.treeTemplates[prefabIdx].minSize, treesData.treeTemplates[prefabIdx].maxSize);
                spawnObject.freeRadius = treesData.treeTemplates[prefabIdx].freeRadius;

                spawnObject.color1 = treesData.treeTemplates[prefabIdx].color1;
                spawnObject.color2 = treesData.treeTemplates[prefabIdx].color2;

                instantiatedObjects.Add(spawnObject);


#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Trees...", $"Tree {count} of {treesData.maxQuantity} Created.", (float)count / (float)treesData.maxQuantity);
#endif

            }
        }

    }

    


    //  Grass
    private void GenerateGrass(Terrain terrain, UTGrassData grassData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        int maxCount = terrain.terrainData.detailWidth * terrain.terrainData.detailHeight;

        if (grassData.enabled)
        {
            while (count < maxCount)
            {
                count++;

                Vector3 position = Vector3.zero;

                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

                float height = terrain.SampleHeight(position);

                if (height < grassData.minAltitude || height > grassData.maxAltitude) continue;

                float stepness = UTTerrainUtils.GetStepness(terrain, position);
                if (stepness > grassData.maxSlope)
                {
                    continue;
                }

                if (Random.value > grassData.presence)
                {
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();

                int idx = Random.Range(0, grassData.templates.Length);

                spawnObject.prefabIndex = grassData.templates[idx].prefabId;
                spawnObject.texture = grassData.templates[idx].texture;
                spawnObject.position = position;

                instantiatedObjects.Add(spawnObject);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Grass...", $"Grass {count} of {grassData.maxQuantity} Created.", (float)count / (float)grassData.maxQuantity);
#endif

            }
        }
    }


    //  Rocks
    private void GenerateRocks(Terrain terrain, UTRocksData rocksData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        int maxCount = terrain.terrainData.detailWidth * terrain.terrainData.detailHeight;

        int groupObjectCount = 0;
        Vector3 centerGroupPosition = Vector3.zero;


        if (rocksData.enabled)
        {
            while (count < maxCount)
            {
                count++;

                if (groupObjectCount == 0)
                {

                    groupObjectCount = rocksData.groupSize;
                    if (groupObjectCount == 0)
                    {
                        groupObjectCount = 1;
                    }

                    // Get the group center position
                    centerGroupPosition = GetCenterGroupPosition(terrain, rocksData.groupRadius, rocksData.freeRadius);
                }

                groupObjectCount--;

                Vector3 position = GetPositionInGroup(centerGroupPosition, rocksData.groupRadius);

                float height = terrain.SampleHeight(position);

                if (height < rocksData.minAltitude || height > rocksData.maxAltitude) continue;

                float stepness = UTTerrainUtils.GetStepness(terrain, position);
                if (stepness > rocksData.maxSlope)
                {
                    continue;
                }

                if (Random.value > rocksData.presence)
                {
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();

                int idx = Random.Range(0, rocksData.templates.Length);

                spawnObject.prefabIndex = rocksData.templates[idx].prefabId;
                spawnObject.mesh = rocksData.templates[idx].mesh;
                spawnObject.rotation = Random.Range(0, 359);
                spawnObject.position = position;            

                instantiatedObjects.Add(spawnObject);


#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Rocks...", $"Rocks {count} of {rocksData.maxQuantity} Created.", (float)count / (float)rocksData.maxQuantity);
#endif

            }
        }
    }


    //  Bushes
    private void GenerateBushes(Terrain terrain, UTBushesData bushesData, ref List<UTSpawnObject> instantiatedObjects)
    {
        int count = 0;

        int maxCount = terrain.terrainData.detailWidth * terrain.terrainData.detailHeight;

        if (bushesData.enabled)
        {
            while (count < maxCount)
            {
                count++;
                Vector3 position = Vector3.zero;

                position = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));


                float height = terrain.SampleHeight(position);

                if (height < bushesData.minAltitude || height > bushesData.maxAltitude) continue;

                float stepness = UTTerrainUtils.GetStepness(terrain, position);
                if (stepness > bushesData.maxSlope)
                {
                    continue;
                }

                if (Random.value > bushesData.presence)
                {
                    continue;
                }

                UTSpawnObject spawnObject = new UTSpawnObject();
                int idx = Random.Range(0, bushesData.templates.Length);

                spawnObject.prefabIndex = bushesData.templates[idx].prefabId;
                spawnObject.mesh = bushesData.templates[idx].mesh;
                spawnObject.position = position;

                instantiatedObjects.Add(spawnObject);

#if UNITY_EDITOR
                UnityEditor.EditorUtility.DisplayCancelableProgressBar("Spawning Bushes...", $"Bushes {count} of {bushesData.maxQuantity} Created.", (float)count / (float)bushesData.maxQuantity);
#endif

            }
        }
    }

        

    private Vector3 GetPositionInGroup(Vector3 centerGroup, float groupRadius)
    {
        Vector3 position;

        //position = new Vector3(Random.Range(centerGroup.x - groupRadius, centerGroup.x + groupRadius), 0, Random.Range(centerGroup.z - groupRadius, centerGroup.z + groupRadius));
        
        float angle = Random.Range(0, 2 * Mathf.PI);
        float distance = Random.Range(0, groupRadius);
        position = centerGroup + distance * new Vector3(Mathf.Cos(angle), 0, Mathf.Sin(angle));
        
        return position;
    }

   

    private Vector3 GetCenterGroupPosition(Terrain terrain, float groupRadius, float freeRadius)
    {
        Vector3 centerGroupPosition = new Vector3(Random.Range(0, terrain.terrainData.size.x), 0, Random.Range(0, terrain.terrainData.size.z));

        float areaRadius = (groupRadius + freeRadius);

        centerGroupPosition.x = (int)centerGroupPosition.x / (int)areaRadius;
        centerGroupPosition.x = centerGroupPosition.x * (int)areaRadius + (int)areaRadius / 2;

        centerGroupPosition.z = (int)centerGroupPosition.z / (int)areaRadius;
        centerGroupPosition.z = centerGroupPosition.z * (int)areaRadius + (int)areaRadius / 2;

        return centerGroupPosition;
    }

    private bool CheckPositionOverlap(List<UTSpawnObject> instantiatedObjects, float freeRadius, Vector3 position)
    {
        foreach (UTSpawnObject spawnObject in instantiatedObjects)
        {
            if (Vector3.Distance(position, spawnObject.position) < 2.0f * freeRadius)
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


    private bool CheckValidPosition(Terrain terrain, Vector3 position, float minAltitude, float maxAltitude, float maxSlope, out float height)
    {
        height = 0;

        //  Check values are in terrain boundaries
        if ((position.x < terrain.terrainData.bounds.min.x) ||
            (position.x > terrain.terrainData.bounds.max.x) ||
            (position.z < terrain.terrainData.bounds.min.z) ||
            (position.z > terrain.terrainData.bounds.max.z))
        {
            return false;
        }

        Vector3 normal;
        GameObject hitGameObject;

        UTHit psHit = CheckAt(position, out height, out normal, out hitGameObject);

        //  Check if the position is directly over terrain
        if (psHit != UTHit.TERRAIN_HIT)
        {
            return false;
        }
        position.y = height;

        //  Check Min and Max Altitude
        if (height < minAltitude || height > maxAltitude)
        {
            return false;
        }

        //  Check max slope
        float stepness = UTTerrainUtils.GetStepness(terrain, position);
        if (stepness > maxSlope)
        {
            return false;
        }

        //  Return a valid position
        return true;

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

