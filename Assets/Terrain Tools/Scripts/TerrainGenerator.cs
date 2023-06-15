using System;
using System.Collections;
using System.Collections.Generic;
using UnityEngine;


public enum TerrainMaskMode
{
    None,
    Circular,
    Square
}


[System.Serializable]
public struct TerrainGeneratorType
{
    public int textureIndex;
    public float startingHeight;
}



public class TerrainGenerator : MonoBehaviour
{
    //    public int depth = 20;

    //public int width = 256;
    //public int height = 256;

    //public float scale = 20f;


    //public float xOffset = 100;
    //public float zOffset = 100;

    public float _radius = 0.5f;
    private float xOffset;
    private float zOffset;


    public int _mapChunkSize = 96;


    public float _noiseScale;
    public float _heightmapScale = 1.0f;

    [Range(0.5f, 2.0f)]
    public float _heightScale = 1.0f;
    [Range(0.0f, 1.0f)]
    public float _offsetScale = 0.5f;



    [Range(1, 8)]
    public int _octaves = 1;

    [Range(0.0f, 1.0f)]
    public float _persistance;
    public float _lacunarity;
    [Range(0.10f, 4.0f)]
    public float _noiseExponent = 1.0f;

    public int _seed;
    public Vector2 _offset;

//    public float _meshHeightMultiplier;
//    public AnimationCurve _meshHeightCurve;

    public TerrainMaskMode mapMaskMode = TerrainMaskMode.None;
    [Range(-16f, 16f)]
    public float _maskMarginOffset = 0;

    public bool _autoUpdate;

    //    public TerrainType[] _regions;


    //    public bool _createCliffs = false;
    //    [Range(0.0f, 1.0f)]
    //    public float _cliffLimit = 0.07f;
    //    public Color _cliffColor = Color.magenta;

    public TerrainGeneratorType[] _regions;

    private Terrain terrain;
    private float[,,] splatmapData;

    private void Start()
    {

    }

    public void GenerateMap()
    {
        terrain = GetComponent<Terrain>();
        terrain.terrainData = GenerateTerrain(terrain.terrainData, _heightmapScale);
        
    }

    public static float[,] GenerateNoiseData(int mapWidth, int mapDepth, float scale, Vector2 offset)
    {
        // create an empty noise map with the mapDepth and mapWidth coordinates
        float[,] noiseMap = new float[mapDepth, mapWidth];

        for (int z = 0; z < mapDepth; z++) {

            for (int x = 0; x < mapWidth; x++)
            {
                noiseMap[x, z] = Mathf.PerlinNoise(x, z) * scale;
            }

        }

        return noiseMap;
    }

    private TerrainData GenerateTerrain(TerrainData terrainData, float terrainHeight)
    {

        float angle = 30 * Mathf.Deg2Rad;

        xOffset = 2.0f * _radius * Mathf.Cos(angle);
        zOffset = _radius + _radius * Mathf.Sin(angle);

        Debug.Log(Mathf.Sin(angle));
        Debug.Log(xOffset);
        Debug.Log(zOffset);



        //        terrainData.heightmapResolution = width + 1;
        terrainData.heightmapResolution = _mapChunkSize + 1;
        terrainData.size = new Vector3(_mapChunkSize, terrainHeight, _mapChunkSize);

        //terrainData.heightmapHeight = _verticalScale;

        //        float[,] noiseMap = NoiseUtils.GenerateNoiseMap(_mapChunkSize, _mapChunkSize, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset);

        //
        //        float[,] noiseMap = NoiseUtils.GenerateNoiseMapFixed2(_mapChunkSize, _mapChunkSize, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _noiseExponent, _offset);
//        float[,] noiseMap = TerrainGeneratorNoiseUtils.GenerateTerracedNoiseMapFixed(_mapChunkSize, _mapChunkSize, _seed, _noiseScale, _radius, _octaves, _persistance, _lacunarity, _offset, _heightScale, _offsetScale);
        float[,] noiseMap = TerrainGeneratorNoiseUtils.GenerateTerracedNoiseMapFixed(_mapChunkSize+1, _mapChunkSize+1, _seed, _noiseScale, _octaves, _persistance, _lacunarity, _offset, _heightScale, _offsetScale);
        //        float[,] noiseMap = GenerateNoiseData(_mapChunkSize, _mapChunkSize, depth, _offset);


        if (mapMaskMode == TerrainMaskMode.Circular)
        {
            noiseMap = TerrainMaskUtils.ApplyCircularMask(noiseMap, _mapChunkSize, _maskMarginOffset);
        }
        else if (mapMaskMode == TerrainMaskMode.Square)
        {
            noiseMap = TerrainMaskUtils.ApplySquareMask(noiseMap, _mapChunkSize, _maskMarginOffset);
        }



        terrainData.SetHeights(0, 0, noiseMap);
        //terrainData.SetHeights(0, 0, GenerateHeights());

        splatmapData = GenerateSplatMap(terrainData);
        terrainData.SetAlphamaps(0, 0, splatmapData);

        return terrainData;
    }

    private float[,,] GenerateSplatMap(TerrainData terrainData)
    {

        splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];

        for (int z = 0; z < terrainData.alphamapHeight; z++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(z, x);

                float[] splat = new float[_regions.Length];

                int splatmapDataIndex = 0;
                
                for (int i=0; i < splat.Length; i++)
                {
                    /*
                    if (i == _regions.Length -1)
                    {
                        splat[i] = 1;
                    }
                    else
                    if (terrainHeight >= _regions[i].startingHeight * (float)depth
                     && terrainHeight <= _regions[i+1].startingHeight * (float)depth)
                    {
                        for (int k=0; k < i; k++)
                        {
                            splat[k] = 0.0f;
                        }
                        splat[i] = 1;
                    }*/

                    if (terrainHeight >= _regions[i].startingHeight * (float)_heightmapScale)
                    {
                        splatmapDataIndex = i;
                    }


                }

                /*
                for (int j=0; j < _regions.Length; j++)
                {
//                    splat[j] = splat[j] / _regions.Length;

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

    private void OnValidate()
    {
        if (_lacunarity < 1) _lacunarity = 1;
        if (_octaves < 0) _octaves = 0;
    }

}
