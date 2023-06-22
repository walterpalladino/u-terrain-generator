using System.Collections;
using System.Collections.Generic;
using UnityEngine;



public enum UTTerrainMask
{
    None,
    Circular,
    Square,
    Custom
}

[System.Serializable]
public struct UTTerrainLayer
{
    public int textureIndex;
    //public float startingHeight;
    public float startHeight;
    public float endHeight;
    public float overlap;
}

[System.Serializable]
public struct UTTerrainCliff
{
    public bool enabled;
    public int textureIndex;
    public float minAngle;
    public float overlap;
}

[RequireComponent(typeof(Terrain))]
public class UTTerrainGenerator : MonoBehaviour, IGenerator
{


    [Header("Terrain Settings")]
    [SerializeField] bool autoUpdate;
    public bool AutoUpdate { get => autoUpdate; }
    [SerializeField] bool addToExistingTerrain;
    //[SerializeField] Vector3 mapDimensions = new Vector3(500, 128, 500);
    //public int mapChunkSize = 512;


    [Header("Noise Settings")]
    public float noiseScale = 256.0f;

    [Range(1, 8)]
    public int octaves = 1;

    [Range(0.0f, 1.0f)]
    public float persistance;
    public float lacunarity;

    public int seed;
    public Vector2 offset;


    [Header("Mask")]
    public UTTerrainMask mapMaskMode = UTTerrainMask.None;
    //[Range(-16f, 16f)]
    public float maskMarginOffset = 0;

    public AnimationCurve terrainCurve = new AnimationCurve();

    [Header("Layers Settings")]
    public UTTerrainLayer[] layers;
    public UTTerrainCliff cliffs;



    [Header("Filters")]
    [Range(0, 10)]
    [SerializeField] int filterQty;
    [Range(0.01f, 1.0f)]
    [SerializeField] float filterWeight = 0.01f;


    [Header("Terraces")]
    [SerializeField] bool createTerraces;
    [SerializeField][Range(1, 128)] int terraces = 16;


    [Header("FXs")]
    //Help for Island / Beaches / smooth mountain sides
    [Tooltip("Help for Island / Beaches / smooth mountain sides")]
    [SerializeField]
    float softExp = 1.0f;



    private Terrain terrain;
    private float[,,] splatmapData;



    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }

    //  IGenerator implementation
    public void Clear()
    {
        terrain = GetComponent<Terrain>();

        //  Clear the heightmap
        float[,] heightMap;
        heightMap = new float[terrain.terrainData.heightmapResolution, terrain.terrainData.heightmapResolution];
        for (int x = 0; x < terrain.terrainData.heightmapResolution; x++)
        {
            for (int z = 0; z < terrain.terrainData.heightmapResolution; z++)
            {
                heightMap[x, z] = 0;
            }
        }
        terrain.terrainData.SetHeights(0, 0, heightMap);

        //  Update the splatmap based on the heights
        splatmapData = GenerateSplatMap(terrain.terrainData);
        terrain.terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    public void Generate()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData);
    }

    private TerrainData GenerateTerrain(TerrainData terrainData)
    {

        float[,] noiseMap = GenerateHeightMap(terrainData.heightmapResolution, terrainData.heightmapResolution, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Debug.Log("Created noiseMap : " + noiseMap.GetLength(0) + " x " + noiseMap.GetLength(1));

        //  Apply noise reduction if required
        for (int f = 0; f < filterQty; f++)
        {
            noiseMap = FilterNoiseMap(noiseMap, filterWeight);
        }

        if (createTerraces)
        {
            noiseMap = GenerateTerraces(noiseMap, terraces);
        }

        //  Check for masks
        if (mapMaskMode == UTTerrainMask.Circular)
        {
            noiseMap = TerrainMaskUtils.ApplyCircularMask(noiseMap, terrainData.heightmapResolution, maskMarginOffset);
        }
        else if (mapMaskMode == UTTerrainMask.Square)
        {
            noiseMap = TerrainMaskUtils.ApplySquareMask(noiseMap, terrainData.heightmapResolution, maskMarginOffset);
        }
        else if (mapMaskMode == UTTerrainMask.Custom)
        {
            noiseMap = TerrainMaskUtils.ApplyCustomMask(noiseMap, terrainData.heightmapResolution, maskMarginOffset, terrainCurve);
        }

        //  Set the terrain heightmap
        if (addToExistingTerrain) {
            float[,] heights = terrainData.GetHeights(0,0,terrainData.heightmapResolution, terrainData.heightmapResolution);
            for (int z = 0; z < terrainData.heightmapResolution; z++)
            {
                for (int x = 0; x < terrainData.heightmapResolution; x++)
                {
                    noiseMap[x, z] += heights[x, z];
                }
            }
        }
        //  Array of heightmap samples to set (values range from 0 to 1, array indexed as [y,x]).
        terrainData.SetHeights(0, 0, noiseMap);

        //  Generate splat maps for textures
        splatmapData = GenerateSplatMap(terrainData);
        terrainData.SetAlphamaps(0, 0, splatmapData);

        return terrainData;
    }


    float[,] GenerateHeightMap(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        Debug.Log("mapWidth : " + mapWidth + " mapDepth : " + mapDepth + " seed : " + seed + " scale : " + scale +
    "ocatces : " + octaves + " persistence : " + persistance + " lacunarity : " + lacunarity + " offset : " + offset);

        System.Random prng = new System.Random(seed);

        float offsetX = offset.x;
        float offsetY = offset.y;

        offsetX += prng.Next(-100000, 100000);
        offsetY += prng.Next(-100000, 100000);


        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];


        if (scale <= 0.0f)
        {
            scale = 0.0001f;
        }

        //  Used once all the values are calculated to normalize the result to be sure we only get a 0-1 values
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                float sampleX;
                float sampleY;
                float perlinNoise;

                for (int o = 0; o < octaves; o++)
                {
                    sampleX = (x + offsetX) / scale * frequency;
                    sampleY = (y + offsetY) / scale * frequency;

                    // generate noise value using PerlinNoise
                    perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;

                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseHeight = (noiseHeight + 1.0f) / 2.0f;

                noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 1.0f);

                //  Help for Island / Beaches / smooth mountain sides
                noiseHeight = Mathf.Pow(noiseHeight, softExp);
                
                //  White noise
                sampleX = (x + offsetX) / 16;
                sampleY = (y + offsetY) / 16;
                perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinNoise * 0.025f;
                
                noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 1.0f);

                noiseMap[x, y] = noiseHeight;// * heightScale;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);

        return noiseMap;
    }


    
float[,] GenerateHeightMapV1(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        Debug.Log("mapWidth : " + mapWidth + " mapDepth : " + mapDepth + " seed : " + seed + " scale : " + scale +
    "ocatces : " + octaves + " persistence : " + persistance + " lacunarity : " + lacunarity + " offset : " + offset);

        System.Random prng = new System.Random(seed);

        float offsetX = offset.x;
        float offsetY = offset.y;

        offsetX += prng.Next(-100000, 100000);
        offsetY += prng.Next(-100000, 100000);


        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];


        if (scale <= 0.0f)
        {
            scale = 0.0001f;
        }

        //  Used once all the values are calculated to normalize the result to be sure we only get a 0-1 values
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                float sampleX;
                float sampleY;
                float perlinNoise;

                float maxValue = 0;

                for (int o = 0; o < octaves; o++)
                {
                    maxValue += amplitude;

                    sampleX = (x + offsetX) / scale * frequency;
                    sampleY = (y + offsetY) / scale * frequency;

                    // generate noise value using PerlinNoise
                    //perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    perlinNoise = Mathf.PerlinNoise(x * frequency, y * frequency) ;
                    //perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);

                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseHeight /= maxValue;

                /*
                //noiseHeight = (noiseHeight + 1.0f) / 2.0f;

                //noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 1.0f);

                //  Help for Island / Beaches / smooth mountain sides
                noiseHeight = Mathf.Pow(noiseHeight, softExp);
                
                //  White noise
                sampleX = (x + offsetX) / 16;
                sampleY = (y + offsetY) / 16;
                perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinNoise * 0.025f;

                noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 1.0f);
                */


                noiseMap[x, y] = noiseHeight;// * heightScale;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);

        return noiseMap;
    }     
     
     


    float[,] FilterNoiseMap(float[,] noiseMap, float filterWeight)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] filteredNoiseMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];

        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {

                if (x == 0 || y == 0 || x == noiseMap.GetLength(0) - 1 || y == noiseMap.GetLength(1) - 1)
                {
                    filteredNoiseMap[x, y] = noiseMap[x, y];
                }
                else
                {

                    float value = 0;
                    value += noiseMap[x - 1, y - 1] * filterWeight + noiseMap[x, y - 1] * filterWeight + noiseMap[x + 1, y - 1] * filterWeight;
                    value += noiseMap[x - 1, y] * filterWeight + noiseMap[x, y] + noiseMap[x + 1, y] * filterWeight;
                    value += noiseMap[x - 1, y + 1] * filterWeight + noiseMap[x, y + 1] * filterWeight + noiseMap[x + 1, y + 1] * filterWeight;
                    value /= (1 + 8 * filterWeight);

                    /*
                    float value = noiseMap[x, y];
                    value = ((int)(value * 32.0f)) / 32.0f;
                    */

                    filteredNoiseMap[x, y] = value;
                }

            }
        }
        return filteredNoiseMap;
    }

    private float[,] GenerateTerraces(float[,] noiseMap, int terraces)
    {
        float[,] steppedNoiseMap = new float[noiseMap.GetLength(0), noiseMap.GetLength(1)];

        for (int y = 0; y < noiseMap.GetLength(1); y++)
        {
            for (int x = 0; x < noiseMap.GetLength(0); x++)
            {

                if (x == 0 || y == 0 || x == noiseMap.GetLength(0) - 1 || y == noiseMap.GetLength(1) - 1)
                {
                    steppedNoiseMap[x, y] = Mathf.Floor(noiseMap[x, y] * terraces) / terraces;
                }
                else
                {


                    List<float> values = new List<float>();

                    values.Add(Mathf.Floor(noiseMap[x - 1, y - 1] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x, y - 1] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x + 1, y - 1] * terraces) / terraces);

                    values.Add(Mathf.Floor(noiseMap[x - 1, y] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x, y] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x + 1, y] * terraces) / terraces);

                    values.Add(Mathf.Floor(noiseMap[x - 1, y + 1] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x, y + 1] * terraces) / terraces);
                    values.Add(Mathf.Floor(noiseMap[x + 1, y + 1] * terraces) / terraces);



                    if (values[3] == values[5])
                    {
                        if (values[1] == values[7])
                        {
                            if (values[1] == values[3])
                            {
                                values[4] = values[1];
                            }
                        }
                        else
                        {
                            values[4] = values[3];
                        }
                    }
                    else
                    {
                        if (values[1] == values[7])
                        {
                            values[4] = values[1];
                        }
                    }


                    steppedNoiseMap[x, y] = values[4];
                }

            }
        }

        return steppedNoiseMap;
    }



    private float[,,] GenerateSplatMapV1(TerrainData terrainData)
    {

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(z, x);

                float[] splat = new float[layers.Length];

                int splatmapDataIndex = 0;

                for (int i = 0; i < splat.Length; i++)
                {
                    /*
                    if (i == regions.Length -1)
                    {
                        splat[i] = 1;
                    }
                    else
                    if (terrainHeight >= regions[i].startHeight * (float)depth
                     && terrainHeight <= regions[i+1].startHeight * (float)depth)
                    {
                        for (int k=0; k < i; k++)
                        {
                            splat[k] = 0.0f;
                        }
                        splat[i] = 1;
                    }*/

                    if (terrainHeight >= layers[i].startHeight)
                    {
                        splatmapDataIndex = i;
                    }


                }

                /*
                for (int j=0; j < regions.Length; j++)
                {
//                    splat[j] = splat[j] / regions.Length;

                    splatmapData[x, z, j] = splat[j];
                }
                */
                splatmapData[x, z, 0] = 0;
                splatmapData[x, z, 1] = 0;
                splatmapData[x, z, 2] = 0;
                splatmapData[x, z, 3] = 0;
                splatmapData[x, z, 4] = 0;

                splatmapData[x, z, splatmapDataIndex] = 1;

            }

        }

        return splatmapData;


    }


    private float[,,] GenerateSplatMap(TerrainData terrainData)
    {
        Debug.Log("GenerateSplatMap for : " + terrainData.alphamapHeight + " x " + terrainData.alphamapWidth);
        Debug.Log("cliffs.enabled " + cliffs.enabled);

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        //  Asumming squared sections
        float scaleFactor = (float)terrainData.heightmapResolution / (float)terrainData.alphamapWidth;


        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {


                //float[] splat = new float[regions.Length];

                float[] splat = new float[layers.Length + (cliffs.enabled ? 1 : 0)];
                float steepness = GetSteepnessAtPoint(terrainData, x, z);
                //Debug.Log(steepness);

                if (cliffs.enabled && steepness >= cliffs.minAngle)
                {
                    //Debug.Log(steepness);
                    splat[splat.Length - 1] = 1.0f;
                }
                else
                {


                    float terrainHeight = terrainData.GetHeight((int)(z * scaleFactor), (int)(x * scaleFactor));

                    for (int i = 0; i < layers.Length; i++)
                    {
                        float thisNoise = Mathf.PerlinNoise(x * 0.05f, z * 0.05f);
                        thisNoise = RemapValue(thisNoise, 0.0f, 1.0f, 0.5f, 1.0f);

                        float thisHeightStart = layers[i].startHeight;
                        float thisHeightEnd = layers[i].endHeight;

                        thisHeightStart *= thisNoise;
                        thisHeightEnd *= thisNoise;

                        if (terrainHeight >= thisHeightStart && terrainHeight <= thisHeightEnd)
                        {
                            splat[i] = 1;
                        }
                    }

                }
                splat = Normalize(splat);

                for (int j = 0; j < splat.Length; j++)
                {
                    splatmapData[x, z, j] = splat[j];
                }
            }

        }

        return splatmapData;
    }

    float GetSteepnessAtPoint(TerrainData terrainData, float worldX, float worldZ)
    {
        float x = worldX / terrainData.alphamapWidth;
        float z = worldZ / terrainData.alphamapHeight;

        return terrain.terrainData.GetSteepness(z, x);
    }

    float RemapValue(float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }


    Vector3 NormalAtPoint(TerrainData terrainData, float worldX, float worldZ)
    {
        float x = worldX / terrainData.size.x;
        float z = worldZ / terrainData.size.z;

        return terrain.terrainData.GetInterpolatedNormal(x, z);
    }

    float[] Normalize(float[] values)
    {
        float[] normalizedvalues = new float[values.Length];

        float total = 0.0f;
        for (int i = 0; i < values.Length; i++)
        {
            total += values[i];
        }
        for (int i = 0; i < values.Length; i++)
        {
            normalizedvalues[i] = values[i] / total;
        }

        return normalizedvalues;
    }

    float[,] GetHeightMap(Terrain terrain, bool resetTerrain)
    {
        if (!resetTerrain)
        {
            return terrain.terrainData.GetHeights(0, 0, terrain.terrainData.heightmapResolution,
                                                terrain.terrainData.heightmapResolution);
        }
        else
            return new float[terrain.terrainData.heightmapResolution,
                             terrain.terrainData.heightmapResolution];

    }


    public void AddRiver() {

        terrain = GetComponent<Terrain>();
        TerrainData terrainData = terrain.terrainData;
        float[,] heights = terrainData.GetHeights(0, 0, terrainData.heightmapResolution,
                                                terrainData.heightmapResolution);

        Vector2Int startPoint = new Vector2Int(Random.Range(0, terrainData.heightmapResolution),
            Random.Range(0, terrainData.heightmapResolution));

        Vector2Int endPoint = new Vector2Int(Random.Range(0, terrainData.heightmapResolution),
            Random.Range(0, terrainData.heightmapResolution));

        //  TODO : Add validation when creates starts end end points

        float startPointHeight = terrainData.GetHeight(startPoint.x, startPoint.y);
        float endPointHeight = terrainData.GetHeight(endPoint.x, endPoint.y);

        float distance = Vector2.Distance(startPoint, endPoint);
        Debug.Log("River distance : " + distance);
        float step = 1.0f / distance;

        for (float t = 0.0f; t < 1.0f; t += step) {
            float x = startPoint.x + (endPoint.x - startPoint.x) * t;
            float y = startPoint.y + (endPoint.y - startPoint.y) * t;

            Debug.Log("River running at : " + x + " / " + y + " for t : " + t );

            float actualHeight = heights[(int)x, (int)y];

            heights[(int)x, (int)y] = 0.0f;
        }

        terrainData.SetHeights(0, 0, heights);

        //  Generate splat maps for textures
        splatmapData = GenerateSplatMap(terrainData);
        terrainData.SetAlphamaps(0, 0, splatmapData);
    }
}
