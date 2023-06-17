using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseGenerator
{

    static float seedX = 0.0f ;
    static float seedY = 0.0f;

    //  Call it after reseed the random generator
    public static void Init()
    {
        seedX = Random.Range(-1000000, 1000000);
        seedY = Random.Range(-1000000, 1000000);
    }

    /// scale : The scale of the "perlin noise" view
    /// octaves : Number of iterations (the more there is, the more detailed the terrain will be)
    /// persistance : The higher it is, the rougher the terrain will be (this value should be between 0 and 1 excluded)
    /// lacunarity : The higher it is, the more "feature" the terrain will have (should be strictly positive)
    public static float GetNoiseAt(float x, float y, float scale, int octaves, float persistance, float lacunarity)
    {
        float perlinValue = 0f;
        float amplitude = 1f;
        float frequency = 1f;

        //persistance = 0.75f;
        //lacunarity = 2.0f;

        for (int i = 0; i < octaves; i++)
        {
            float sampleX = x / scale * frequency + seedX;
            float sampleY = y / scale * frequency + seedY;

            // Get the perlin value at that octave and add it to the sum
            //perlinValue += Mathf.PerlinNoise(x * frequency, z * frequency) * amplitude;
            perlinValue += Mathf.PerlinNoise(sampleX, sampleY) * amplitude ;

            // Decrease the amplitude and the frequency
            amplitude *= persistance;
            frequency *= lacunarity;
        }

        // Return the noise value in 0.0f and 1.0f range
        //return Mathf.Clamp(perlinValue, 0.0f, 1.0f);
        float maxValue = 1;
        for(int i = 1; i < octaves; i++)
        {
            maxValue += Mathf.Pow(persistance, i); 
        }
        //Debug.Log("Perlin Value : " + perlinValue + " / Max Value : " + maxValue);
        perlinValue = perlinValue / maxValue;
        return Mathf.Clamp(perlinValue, 0.0f, 1.0f);
    }
}
