using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TGMask
{
    None,
    Circular,
    Square
}


[System.Serializable]
public struct TGType
{
    public int textureIndex;
    public float startingHeight;
    public float startHeight;
    public float endHeight;
    public float overlap; 
}

[System.Serializable]
public struct TGCliff
{
    public bool enabled;
    public int textureIndex;
    public float minAngle;
    public float overlap;
}

public class TG : MonoBehaviour
{
    //    public int depth = 20;

    //public int width = 256;
    //public int height = 256;

    //public float scale = 20f;


    //public float xOffset = 100;
    //public float zOffset = 100;

    private float xOffset;
    private float zOffset;


    [SerializeField] Vector3 mapDimensions = new Vector3(500, 128, 500);

    public int mapChunkSize = 512;


    public float noiseScale = 256.0f;
    //public float heightmapScale = 48.0f;

    //[Range(0.1f, 2.0f)]
    //public float heightScale = 1.0f;
    [Range(0.0f, 1.0f)]
    public float offsetScale = 0.5f;



    [Range(1, 8)]
    public int octaves = 1;

    [Range(0.0f, 1.0f)]
    public float persistance;
    public float lacunarity;
    //[Range(0.10f, 4.0f)]
    //public float _noiseExponent = 1.0f;

    public int seed;
    public Vector2 offset;

    //    public float _meshHeightMultiplier;
    //    public AnimationCurve _meshHeightCurve;

    public TGMask mapMaskMode = TGMask.None;
    //[Range(-16f, 16f)]
    public float maskMarginOffset = 0;

    [SerializeField]
    bool autoUpdate;
    public bool AutoUpdate { get => autoUpdate; }


    //    public TerrainType[] regions;


    //    public bool _createCliffs = false;
    //    [Range(0.0f, 1.0f)]
    //    public float _cliffLimit = 0.07f;
    //    public Color _cliffColor = Color.magenta;

    public TGType[] regions;

    public TGCliff cliffs;



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



    private void Start()
    {

    }

    public void Generate()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData, mapDimensions);
    }

    public static float[,] GenerateNoiseData(int mapWidth, int mapDepth, float scale, Vector2 offset)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int z = 0; z < mapDepth; z++)
        {

            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, z] = Mathf.PerlinNoise(x, z) * scale;
            }

        }

        return noiseMap;
    }

    private TerrainData GenerateTerrain(TerrainData terrainData, Vector3 dimensions)
    {
        /*
        float angle = 30 * Mathf.Deg2Rad;

        xOffset = 2.0f * radius * Mathf.Cos(angle);
        zOffset = radius + radius * Mathf.Sin(angle);

        Debug.Log(Mathf.Sin(angle));
        Debug.Log(xOffset);
        Debug.Log(zOffset);
        */

        int heightmapResolution = mapChunkSize + 1;
        //        terrainData.heightmapResolution = width + 1;
        terrainData.heightmapResolution = heightmapResolution;
        //terrainData.size = new Vector3(heightmapResolution, terrainHeight, heightmapResolution);
        terrainData.size = dimensions;

        //terrainData.heightmapHeight = _verticalScale;

        //        float[,] noiseMap = NoiseUtils.GenerateNoiseMap(mapChunkSize, mapChunkSize, _seed, noiseScale, octaves, _persistance, _lacunarity, _offset);

        //
        //        float[,] noiseMap = NoiseUtils.GenerateNoiseMapFixed2(mapChunkSize, mapChunkSize, _seed, noiseScale, octaves, _persistance, _lacunarity, _noiseExponent, _offset);
        //        float[,] noiseMap = TerrainGeneratorNoiseUtils.GenerateTerracedNoiseMapFixed(mapChunkSize, mapChunkSize, _seed, noiseScale, radius, octaves, _persistance, _lacunarity, _offset, heightScale, offsetScale);

        //////// OK        float[,] noiseMap = TerrainGeneratorNoiseUtils.GenerateTerracedNoiseMapFixed(heightmapResolution, heightmapResolution, _seed, noiseScale, octaves, _persistance, _lacunarity, _offset, heightScale, offsetScale);
        //        float[,] noiseMap = GenerateNoiseData(mapChunkSize, mapChunkSize, depth, _offset);

        //        float[,] noiseMap = customNoise.GenerateNoiseMap(heightmapResolution, heightmapResolution, _seed, noiseScale, octaves, _persistance, _lacunarity, _offset, heightmapScale, offsetScale);

        //  THIS WORKS
        //float[,] noiseMap = GenerateNoiseMap(heightmapResolution, heightmapResolution, seed, noiseScale, octaves, persistance, lacunarity, offset, heightmapScale, offsetScale);

        float[,] noiseMap = GenerateHeightMap(heightmapResolution, heightmapResolution, seed, noiseScale, octaves, persistance, lacunarity, offset);
        Debug.Log("Created noiseMap : " + noiseMap.GetLength(0) + " x " + noiseMap.GetLength(1));



        for (int f = 0; f < filterQty; f++)
        {
            noiseMap = FilterNoiseMap(noiseMap, filterWeight);
        }

        if (createTerraces)
        {
            noiseMap = GenerateTerraces(noiseMap, terraces);
        }


        if (mapMaskMode == TGMask.Circular)
        {
            noiseMap = TerrainMaskUtils.ApplyCircularMask(noiseMap, heightmapResolution, maskMarginOffset);
        }
        else if (mapMaskMode == TGMask.Square)
        {
            noiseMap = TerrainMaskUtils.ApplySquareMask(noiseMap, heightmapResolution, maskMarginOffset);
        }


        //  Array of heightmap samples to set (values range from 0 to 1, array indexed as [y,x]).
        terrainData.SetHeights(0, 0, noiseMap);
        //terrainData.SetHeights(0, 0, GenerateHeights());

        splatmapData = GenerateSplatMap(terrainData);
        terrainData.SetAlphamaps(0, 0, splatmapData);

        return terrainData;
    }

    private float[,,] GenerateSplatMapV1(TerrainData terrainData)
    {

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(z, x);

                float[] splat = new float[regions.Length];

                int splatmapDataIndex = 0;

                for (int i = 0; i < splat.Length; i++)
                {
                    /*
                    if (i == regions.Length -1)
                    {
                        splat[i] = 1;
                    }
                    else
                    if (terrainHeight >= regions[i].startingHeight * (float)depth
                     && terrainHeight <= regions[i+1].startingHeight * (float)depth)
                    {
                        for (int k=0; k < i; k++)
                        {
                            splat[k] = 0.0f;
                        }
                        splat[i] = 1;
                    }*/

                    if (terrainHeight >= regions[i].startingHeight)
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

    float[] normalize(float[] values)
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

    float remapValue (float value, float sMin, float sMax, float mMin, float mMax)
    {
        return (value - sMin) * (mMax - mMin) / (sMax - sMin) + mMin;
    }


    Vector3 normalAtPoint (TerrainData terrainData, float worldX, float worldZ)
    {
        float x = worldX / terrainData.size.x;
        float z = worldZ / terrainData.size.z;

        return terrain.terrainData.GetInterpolatedNormal(x, z);
    }

    float GetSteepnessAtPoint(TerrainData terrainData, float worldX, float worldZ)
    {
        float x = worldX / terrainData.alphamapWidth;
        float z = worldZ / terrainData.alphamapHeight;

        return terrain.terrainData.GetSteepness(z, x);
    }

    private float[,,] GenerateSplatMapOK(TerrainData terrainData)
    {
        Debug.Log("cliffs.enabled " + cliffs.enabled);
        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {

                float[] splat = new float[regions.Length + (cliffs.enabled ? 1 : 0)];

                float steepness = GetSteepnessAtPoint(terrainData, x, z);
                Debug.Log(steepness);

                if (cliffs.enabled && steepness >= cliffs.minAngle)
                {
                    Debug.Log(steepness);
                    splat[splat.Length - 1] = 1.0f;
                }
                else
                {
                    float terrainHeight = terrainData.GetHeight(z, x);

                    for (int i = 0; i < regions.Length; i++)
                    {

                        float thisNoise = Mathf.PerlinNoise(x * 0.05f, z * 0.05f);
                        thisNoise = remapValue(thisNoise, 0.0f, 1.0f, 0.5f, 1.0f);

                        float thisHeightStart = regions[i].startingHeight - regions[i].overlap;
                        thisHeightStart *= thisNoise;

                        float nextHeightStart = 0;

                        if (i != regions.Length - 1)
                        {
                            nextHeightStart = regions[i + 1].startingHeight + regions[i + 1].overlap;
                            nextHeightStart *= thisNoise;
                        }

                        if (i == regions.Length - 1 && terrainHeight >= thisHeightStart)
                        {
                            splat[i] = 1;
                        }
                        else if (terrainHeight >= thisHeightStart && terrainHeight <= nextHeightStart)
                        {
                            splat[i] = 1;
                        }

                    }
                }


                splat = normalize(splat);

                for (int j = 0; j < splat.Length; j++)
                {
                    splatmapData[x, z, j] = splat[j];
                }
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
        float scaleFactor = (float)mapChunkSize / (float)terrainData.alphamapWidth;


        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {


                //float[] splat = new float[regions.Length];

                float[] splat = new float[regions.Length + (cliffs.enabled ? 1 : 0)];
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

                    for (int i = 0; i < regions.Length; i++)
                    {
                        float thisNoise = Mathf.PerlinNoise(x * 0.05f, z * 0.05f);
                        thisNoise = remapValue(thisNoise, 0.0f, 1.0f, 0.5f, 1.0f);

                        float thisHeightStart = regions[i].startHeight;
                        float thisHeightEnd = regions[i].endHeight;

                        thisHeightStart *= thisNoise;
                        thisHeightEnd *= thisNoise;

                        if (terrainHeight >= thisHeightStart && terrainHeight <= thisHeightEnd)
                        {
                            splat[i] = 1;
                        }
                    }

                }
                splat = normalize(splat);

                for (int j = 0; j < splat.Length; j++)
                {
                    splatmapData[x, z, j] = splat[j];
                }
            }

        }

        return splatmapData;
    }
    /*
    
     public static Color[] GenerateColorMap(FTGRegion[] regions, int chunkSize, float[,] noiseMap, Vector3 dimensions, bool createCliffs = false, float cliffLimit = 0.07f, Color? cliffColor = null)
    {

        Color[] colorMap = new Color[chunkSize * chunkSize];

        int currentRegion = 0;


        for (int y = 0; y < chunkSize; y++)
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float currentHeight = noiseMap[y, x] * dimensions.y;

                colorMap[y * chunkSize + x] = regions[0].color;
                currentRegion = 0;

                for (int r = 0; r < regions.Length; r++)
                {
                    Color color = regions[r].color;
                    //float overlap = 0;

                    if (currentHeight >= regions[r].startHeight && currentHeight <= regions[r].endHeight)
                    {

    //colorMap[y * chunkSize + x] = regions[r].color;
    colorMap[y * chunkSize + x] = color;
                        currentRegion = r;

                        break;
                    }
                }

                if (createCliffs && regions[currentRegion].doCliff)
{
    if ((y < chunkSize - 1) && (x < chunkSize - 1))
    {
        if (Mathf.Abs(currentHeight - noiseMap[x, y + 1]) > cliffLimit)
        {
            colorMap[y * chunkSize + x] = cliffColor ?? Color.magenta;
        }
        else if (Mathf.Abs(currentHeight - noiseMap[x + 1, y]) > cliffLimit)
        {
            colorMap[y * chunkSize + x] = cliffColor ?? Color.magenta;
        }
        else if (Mathf.Abs(currentHeight - noiseMap[x + 1, y + 1]) > cliffLimit)
        {
            colorMap[y * chunkSize + x] = cliffColor ?? Color.magenta;
        }
    }

}
            }
        }

        return colorMap;

    }
     
     */
    /*
    private float[,,] GenerateSplatMapH(TerrainData terrainData)
    {

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(z, x);

                float[] splat = new float[regions.Length];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (i == regions.Length - 1)
                    {
                        splat[i] = 1;
                    }
                    else
                    if (terrainHeight >= regions[i].startingHeight * (float)heightmapScale && terrainHeight <= regions[i + 1].startingHeight * (float)heightmapScale)
                    {
                        splat[i] = 1;
                    }
                }

                for (int j = 0; j < regions.Length; j++)
                {
                    splatmapData[x, z, j] = splat[j];
                }

            }
        }

        return splatmapData;
    }*/

    private float[,,] GenerateSplatMapH2(TerrainData terrainData)
    {

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(z, x);

                float[] splat = new float[regions.Length];

                for (int i = 0; i < regions.Length; i++)
                {
                    if (terrainHeight >= regions[i].startingHeight)
                    {
                        splat[i] = 1;
                    }
                }

                for (int j = 0; j < regions.Length; j++)
                {
                    splatmapData[x, z, j] = splat[j];
                }

            }
        }

        return splatmapData;
    }

    private void OnValidate()
    {
        if (lacunarity < 1) lacunarity = 1;
        if (octaves < 0) octaves = 0;
    }


    float[,] GenerateNoiseMap(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    {

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
        //scale = 16.0f;


        //  Used once all the values are calculated to normalize the result to be sure we only get a 0-1 values
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;
        /*
        //  Max value : 1.0f + 0.5f + 0.25f + 0.125f + 0.0625f = 1.9375f
        //  Min value : 0.0f
        float maxPossibleHeightValue = 1.0f + 0.5f + 0.25f + 0.125f + 0.0625f;
        maxPossibleHeightValue = 0.0f;
        for (int o = 0; o < octaves; o++)
        {
            maxPossibleHeightValue += 1.0f / Mathf.Pow(2, o);
        }
        Debug.Log("maxPossibleHeightValue : " + maxPossibleHeightValue);
        */
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


                if (noiseHeight <= 0)
                {
                    noiseHeight /= 2.0f;
                }
                noiseHeight += 0.25f;


                noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 2.0f);
                
                //  White noise
                sampleX = (x + offsetX) / 16;
                sampleY = (y + offsetY) / 16;
                perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                noiseHeight += perlinNoise * 0.025f;
                


                noiseMap[x, y] = noiseHeight * heightScale;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);

        return noiseMap;
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
}
