using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class TerrainGeneratorNoiseUtils {

    public static float[,] GenerateNoiseMap (int mapWidth, int mapDepth, float scale, float radius)
    {


        float angle = 30 * Mathf.Deg2Rad;

        float xOffset = 2.0f * radius * Mathf.Cos(angle);
        float zOffset = radius + radius * Mathf.Sin(angle);


        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        if (scale <= 0.0f) {
            scale = 0.0001f;
        }

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                // calculate sample indices based on the coordinates and the scale

                float xCell = x;
                float yCell = y;


                xCell = xCell * xOffset;
                if (y % 2 == 1)
                {
                    xCell += xOffset / 2.0f;
                }
               yCell = yCell * zOffset;


                float sampleX = xCell / scale;
                float sampleY = yCell / scale;

                // generate noise value using PerlinNoise
                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);

                noiseMap[x, y] = perlinNoise;
            }
        }

        return noiseMap;
    }
    /*
    public static float[,] GenerateNoiseMap(int mapWidth, int mapDepth, float scale, int octaves, float persistance, float lacunarity)
    {
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

                for (int o = 0; o < octaves; o++)
                {
                    // calculate sample indices based on the coordinates and the scale
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    // generate noise value using PerlinNoise
                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }
        }

        //  Normalize the obtained values
        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }
         
        return noiseMap;
    }
*/
    public static float[,] GenerateNoiseMap(int mapWidth, int mapDepth, int seed, float scale, float radius, int octaves, float persistance, float lacunarity, Vector2 offset)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        octaves = 1;
        System.Random prng = new System.Random(seed);
        Vector2[] octaveOffsets = new Vector2[octaves];
        for (int o = 0; o < octaves; o++) {
            float offsetX = prng.Next(-100000, 100000) + offset.x;
            float offsetY = prng.Next(-100000, 100000) + offset.y;
            octaveOffsets[o] = new Vector2(offsetX, offsetY);
        }

        if (scale <= 0.0f)
        {
            scale = 0.0001f;
        }

        //  Used once all the values are calculated to normalize the result to be sure we only get a 0-1 values
        float maxNoiseHeight = float.MinValue;
        float minNoiseHeight = float.MaxValue;

        //  Used to center the point when scaling the map
        float halfWidth = mapWidth / 2.0f;
        float halfDepth = mapDepth / 2.0f;

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                for (int o = 0; o < octaves; o++)
                {
                    // calculate sample indices based on the coordinates and the scale
                    float sampleX = (x - halfWidth) / scale * frequency + octaveOffsets[o].x;
                    float sampleY = (y - halfDepth) / scale * frequency + octaveOffsets[o].y;

                    // generate noise value using PerlinNoise
                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseMap[x, y] = noiseHeight;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;
            }
        }

        //  Normalize the obtained values
        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, y] = Mathf.InverseLerp(minNoiseHeight, maxNoiseHeight, noiseMap[x, y]);
            }
        }

        return noiseMap;
    }



    public static float[,] GenerateTerracedNoiseMap(int mapWidth, int mapDepth, int seed, float scale, float radius, int octaves, float persistance, float lacunarity, Vector2 offset, int maxSteps)
    {

        //  Terrain values should be from 0.0 to 1.0
        //  Base on steps
        float stepHeight = 1.0f / (float)maxSteps;

        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];


        if (scale <= 0.0f)
        {
            scale = 0.0001f;
        }
        scale = 16.0f;

        //  Used to center the point when scaling the map
        float halfWidth = mapWidth / 2.0f;
        float halfDepth = mapDepth / 2.0f;

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float sampleX = (x - halfWidth) / scale + offset.x ;
                float sampleY = (y - halfDepth) / scale + offset.y ;

                // generate noise value using PerlinNoise
                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) ;

                perlinNoise += .5f / 1.0f * Mathf.PerlinNoise(2 * sampleX, 2 * sampleY);
                perlinNoise += .25f / 1.0f * .5f * Mathf.PerlinNoise(4 * sampleX, 4 * sampleY);
                perlinNoise += .125f / 1.0f * .5f * Mathf.PerlinNoise(8 * sampleX, 8 * sampleY);
                //perlinNoise += .0625f / 1.0f * .5f * Mathf.PerlinNoise(16 * sampleX, 16 * sampleY);


                //perlinNoise += stepHeight / 2.0f;
                perlinNoise = (int)(perlinNoise * maxSteps) * stepHeight / (1.0f + 0.5f + 0.25f + .125f);

                noiseMap[x, y] = perlinNoise;
                Debug.Log("Value : " + perlinNoise);
            }

        }

        return noiseMap;
    }

//    public static float[,] GenerateTerracedNoiseMapFixed(int mapWidth, int mapDepth, int seed, float scale, float radius, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    public static float[,] GenerateTerracedNoiseMapFixed(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    {

        float offsetX = offset.x ;
        float offsetY = offset.y ;


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

        //  Max value : 1.0f + 0.5f + 0.25f + 0.125f + 0.0625f = 1.9375f
        //  Min value : 0.0f
        float maxPossibleHeightValue = 1.0f + 0.5f + 0.25f + 0.125f + 0.0625f;
        maxPossibleHeightValue    = 0.0f;
        for (int o = 0; o < octaves; o++)
        {
            maxPossibleHeightValue += 1.0f / Mathf.Pow(2, o);
        }
        Debug.Log("maxPossibleHeightValue : " + maxPossibleHeightValue);

        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {

                float amplitude = 1;
                float frequency = 1;
                float noiseHeight = 0;

                //                float sampleX = x / scale + offsetX;
                //                float sampleY = y / scale + offsetY;

                /*
                float sampleX = (x + offsetX) / scale;
                float sampleY = (y + offsetY) / scale;

                // generate noise value using PerlinNoise
                float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);
                
                perlinNoise += .5f * Mathf.PerlinNoise(2 * sampleX, 2 * sampleY);
                perlinNoise += .25f * Mathf.PerlinNoise(4 * sampleX, 4 * sampleY);
                perlinNoise += .125f * Mathf.PerlinNoise(8 * sampleX, 8 * sampleY);
                perlinNoise += .0625f * Mathf.PerlinNoise(16 * sampleX, 16 * sampleY);
                */

                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (x + offsetX) / scale * frequency;
                    float sampleY = (y + offsetY) / scale * frequency;


                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY);

                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                /*
                for (int o = 0; o < octaves; o++)
                {
                    // calculate sample indices based on the coordinates and the scale
                    float sampleX = x / scale * frequency;
                    float sampleY = y / scale * frequency;

                    // generate noise value using PerlinNoise
                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                */

                    noiseMap[x, y] = noiseHeight * heightScale;
                //Debug.Log("Value : " + perlinNoise);

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);

        maxNoiseHeight = float.MinValue;
        minNoiseHeight = float.MaxValue;
        
        //  Normalize the obtained values
        for (int y = 0; y < mapDepth; y++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                //float perlinNoise = noiseMap[y, x] + stepHeight / 2.0f;
                //perlinNoise = (int)(perlinNoise * maxSteps) * stepHeight / maxHeightValue;
                //noiseMap[y, x] = perlinNoise;

                //noiseMap[y, x] = noiseMap[y, x] / maxPossibleHeightValue ;

                float perlinNoise = noiseMap[y, x] ;
                perlinNoise /= maxPossibleHeightValue;
                perlinNoise -= offsetScale;
                perlinNoise *= heightScale;
                perlinNoise = Mathf.Clamp(perlinNoise, -0.5f, 0.5f);
                perlinNoise += offsetScale;
                noiseMap[y, x] = perlinNoise;

                if (noiseMap[y, x] > maxNoiseHeight) maxNoiseHeight = noiseMap[y, x];
                if (noiseMap[y, x] < minNoiseHeight) minNoiseHeight = noiseMap[y, x];
            }
        }

        Debug.Log("After --> Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);
        
        return noiseMap;
    }
}
