using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HolisticTerrain : MonoBehaviour
{

    [System.Serializable]
    public class SplathHeights
    {
        public int textureIndex;
        public int startimgHeight;
    }

    public SplathHeights[] splatHeights;


    // Start is called before the first frame update
    void Start()
    {
        Terrain terrain = GetComponent<Terrain>();

        TerrainData terrainData = terrain.terrainData;
//        TerrainData terrainData = Terrain.activeTerrain.terrainData;
        float[,,] splatmapData = new float[terrainData.alphamapWidth, terrainData.alphamapHeight, terrainData.alphamapLayers];
        
        for (int y=0; y < terrainData.alphamapHeight; y++)
        {
            for (int x = 0; x < terrainData.alphamapWidth; x++)
            {
                float terrainHeight = terrainData.GetHeight(y, x);

                float[] splat = new float[splatHeights.Length];

                for (int i = 0; i < splatHeights.Length; i++)
                {
                    if (i == splatHeights.Length -1)
                    {
                        splat[i] = 1;
                    }
                    else
                    if (terrainHeight >= splatHeights[i].startimgHeight && terrainHeight <= splatHeights[i + 1].startimgHeight)
                    {
                        splat[i] = 1;
                    }
                }

                for (int j = 0; j < splatHeights.Length; j++)
                {
                    splatmapData[x, y, j] = splat[j];
                }
            }

        }

        terrainData.SetAlphamaps(0, 0, splatmapData);
    }

    // Update is called once per frame
    void Update()
    {
        
    }
}
