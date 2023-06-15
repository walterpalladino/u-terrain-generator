using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class CustomNoise : MonoBehaviour, ICustomNoise
{
    [SerializeField] Texture2D noiseTexture;
    [SerializeField] float textureScale = 1.0f;


    [SerializeField] float lowerNoiseValue;
    [SerializeField] float higherNoiseValue;

    [SerializeField] float flatValue = 2.0f;

    // Start is called before the first frame update
    void Start()
    {
        
    }

    // Update is called once per frame
    void Update()
    {
        
    }


    public float[,] GenerateNoiseMap(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    {

        float offsetX = offset.x;
        float offsetY = offset.y;


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
                    // calculate sample indices based on the coordinates and the scale
                    //float sampleX = x / scale * frequency;
                    //float sampleY = y / scale * frequency;
                    sampleX = (x + offsetX) / scale * frequency;
                    sampleY = (y + offsetY) / scale * frequency;

                    // generate noise value using PerlinNoise
                    perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }

                noiseHeight = Mathf.Clamp(noiseHeight, 0.0f, 2.0f);

                //  White noise
                sampleX = (x + offsetX) / 16;
                sampleY = (y + offsetY) / 16;
                perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) ;
                noiseHeight += perlinNoise * 0.05f;


                noiseMap[x, y] = noiseHeight * heightScale;

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);

        return noiseMap;
    }

    public float[,] GenerateNoiseMapW(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    {

        float offsetX = offset.x;
        float offsetY = offset.y;


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
                /*
                for (int o = 0; o < octaves; o++)
                {
                    float sampleX = (x + offsetX) / scale * frequency;
                    float sampleY = (y + offsetY) / scale * frequency;


                    float perlinNoise = GetNoiseAt(sampleX, sampleY);
                    perlinNoise = perlinNoise * flatValue - flatValue / 2.0f;

                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                */
                
                for (int o = 0; o < octaves; o++)
                {
                    // calculate sample indices based on the coordinates and the scale
                    //float sampleX = x / scale * frequency;
                    //float sampleY = y / scale * frequency;
                    float sampleX = (x + offsetX) / scale * frequency;
                    float sampleY = (y + offsetY) / scale * frequency;

                    // generate noise value using PerlinNoise
                    float perlinNoise = Mathf.PerlinNoise(sampleX, sampleY) * 2.0f - 1.0f;
                    noiseHeight += perlinNoise * amplitude;

                    amplitude *= persistance;
                    frequency *= lacunarity;
                }
                

                noiseMap[x, y] = noiseHeight * heightScale;
                //Debug.Log("Value : " + perlinNoise);

                if (noiseHeight > maxNoiseHeight) maxNoiseHeight = noiseHeight;
                if (noiseHeight < minNoiseHeight) minNoiseHeight = noiseHeight;

            }

        }

        Debug.Log("Min Noise Value: " + minNoiseHeight + " Max Noise Value:" + maxNoiseHeight);
        
/*
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

                        float perlinNoise = noiseMap[y, x];
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
        */
        return noiseMap;
    }

    public float[,] GenerateNoiseMapV1(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale)
    {

        //ProcessNoiseTexture();



        float offsetX = offset.x;
        float offsetY = offset.y;


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
        maxPossibleHeightValue = 0.0f;
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

                    
                    float perlinNoise = GetNoiseAt(sampleX, sampleY);

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

                float perlinNoise = noiseMap[y, x];
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

    private void ProcessNoiseTexture()
    {
        int width = noiseTexture.width;

        lowerNoiseValue = Mathf.Infinity;
        higherNoiseValue = -Mathf.Infinity;

        for (int y = 0; y < noiseTexture.height; y++)
        {
            for (int x = 0; x < noiseTexture.width; x++)
            {
                Color color = noiseTexture.GetPixel(x, y);

                float colorValue = color.grayscale;

                if (colorValue < lowerNoiseValue)
                {
                    lowerNoiseValue = colorValue;
                }
                else if (colorValue > higherNoiseValue)
                {
                    higherNoiseValue = colorValue;
                }
            }
        }

    }

    private float GetNoiseAt(float x, float y)
    {
        /*
        int textureX = Mathf.FloorToInt(x * textureScale);
        int textureY = Mathf.FloorToInt(y * textureScale);

        textureX = textureX % noiseTexture.width;
        textureY = textureY % noiseTexture.height;

        Color color = noiseTexture.GetPixel(textureX, textureY);

        float colorValue = color.grayscale;


        colorValue = (colorValue - lowerNoiseValue) / (higherNoiseValue - lowerNoiseValue);

        return colorValue;*/

        float value = Mathf.PerlinNoise(x, y);
        value = Mathf.Clamp(value, 0.0f, 1.0f);

        //value = Mathf.FloorToInt(value * 100.0f) / 100.0f;

        return value;
    }
}
