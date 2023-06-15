using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public interface ICustomNoise
{
    float[,] GenerateNoiseMap(int mapWidth, int mapDepth, int seed, float scale, int octaves, float persistance, float lacunarity, Vector2 offset, float heightScale, float offsetScale);

}
