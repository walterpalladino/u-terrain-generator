using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UIElements;

public static class UTTerrainUtils
{

    public static Vector2 GetTerrainPosition(Terrain terrain, Vector3 worldPos)
    {

        TerrainData terrainData = terrain.terrainData;
        Vector3 terrainPos = terrain.transform.position;
        int mapX = (int)(((worldPos.x - terrainPos.x) / terrainData.size.x) * terrainData.alphamapWidth);
        int mapZ = (int)(((worldPos.z - terrainPos.z) / terrainData.size.z) * terrainData.alphamapHeight);
        return new Vector2(mapX, mapZ);
    }

    private static float[,,] GetAlphaMapsForPosition(Terrain terrain, Vector3 worldPos, int size)
    {
        // get the splat data for this cell as a 1x1xN 3d array (where N = number of textures)
        TerrainData terrainData = terrain.terrainData;
        Vector2 converted = UTTerrainUtils.GetTerrainPosition(terrain, worldPos);
        return terrainData.GetAlphamaps((int)converted.x - size / 2, (int)converted.y - size / 2, size, size);
    }

    public static void ClearTrees(Terrain terrain)
    {
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

    public static void ClearAllDetailLayers(Terrain terrain)
    {
        DetailPrototype[] details = terrain.terrainData.detailPrototypes;

        for (int i = 0; i < details.Length; i++)
        {
            UTTerrainUtils.ClearDetailLayer(terrain, i);
        }
    }

    public static void ClearDetailLayer(Terrain terrain, int layerId)
    {

        // Get all of layer zero.
        int[,] map = terrain.terrainData.GetDetailLayer(
                    0, 0,
                    terrain.terrainData.detailWidth, terrain.terrainData.detailHeight,
                    layerId);

        // For each pixel in the detail map...
        for (var y = 0; y < terrain.terrainData.detailHeight; y++)
        {
            for (var x = 0; x < terrain.terrainData.detailWidth; x++)
            {
                map[x, y] = 0;
            }
        }
    }

    public static float GetStepness(Terrain terrain, Vector3 position) {

        float normalizedX = position.x / (float)terrain.terrainData.size.x;
        float normalizedY = position.z / (float)terrain.terrainData.size.z;

        float stepness = terrain.terrainData.GetSteepness(normalizedY, normalizedX);

        return stepness;
    }

}
