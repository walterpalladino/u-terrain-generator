//using Cinemachine;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using UnityEngine;

public class GameManager : MonoBehaviour
{

    [SerializeField]
    GameObject characterPrefab;
    [SerializeField]
    Vector3 characterSpawnPosition;

    PSTerrainGenerator terrainGenerator;
    ProceduralSpawner proceduralSpawner;
    //VirtualCameraSettings virtualCameraSettings;

    [SerializeField]
    bool[] spawnedObjects;
    [SerializeField]
    bool allObjectsSpawned;
    [SerializeField]
    bool spawningObjects;

    GameObject character;

    [SerializeField]
    int xPrev;
    [SerializeField]
    int zPrev;


    // Start is called before the first frame update
    void Start()
    {
        terrainGenerator = FindObjectOfType<PSTerrainGenerator>();
        proceduralSpawner = FindObjectOfType<ProceduralSpawner>();
        //virtualCameraSettings = GetComponent<VirtualCameraSettings>();

        SpawnCharacter();

    }

    // Update is called once per frame
    void Update()
    {
        CheckCharacterMovement();
    }

    private void CheckCharacterMovement()
    {
        //if (allObjectsSpawned || spawningObjects)
        //{
        //    return;
        //}

        int xPos = (int)(character.transform.position.x / terrainGenerator.terrainTileSize);
        int zPos = (int)(character.transform.position.z / terrainGenerator.terrainTileSize);

        if (xPos != xPrev || zPos != zPrev)
        {

            int xStart = (xPos > 0) ? xPos - 1 : xPos;
            int xEnd = (xPos < terrainGenerator.terrainWidth - 1) ? xPos + 1 : xPos;

            int zStart = (zPos > 0) ? zPos - 1 : zPos;
            int zEnd = (zPos < terrainGenerator.terrainDepth - 1) ? zPos + 1 : zPos;
            /*
            for (int z = zStart; z <= zEnd; z++)
            {
                for (int x = xStart; x <= xEnd; x++)
                {
                    proceduralSpawner.ShowObjects(x, z);
                    //if (!spawnedObjects[x * terrainGenerator.terrainWidth + z])
                    //{
                    //    //  Should spawn objects at this location
                    //    Debug.Log("Should Start an Async Run");
                    //    //Task.Run(async () =>
                    //    //{
                    //        Debug.Log("Start Async Run");
                    //        SpawnObjectsAt(x, z);
                    //    //});
                    //}
                }
            }
            */
            /*
            if (xPos > xPrev)
            {
                proceduralSpawner.HideObjects(xPrev - 1, zPrev);
                proceduralSpawner.HideObjects(xPrev - 1, zPrev - 1);
                proceduralSpawner.HideObjects(xPrev - 1, zPrev + 1);

                proceduralSpawner.ShowObjects(xPrev + 2, zPrev);
                proceduralSpawner.ShowObjects(xPrev + 2, zPrev - 1);
                proceduralSpawner.ShowObjects(xPrev + 2, zPrev + 1);
            }

            if (xPos < xPrev)
            {
                proceduralSpawner.HideObjects(xPrev + 1, zPrev);
                proceduralSpawner.HideObjects(xPrev + 1, zPrev - 1);
                proceduralSpawner.HideObjects(xPrev + 1, zPrev + 1);

                proceduralSpawner.ShowObjects(xPrev - 2, zPrev);
                proceduralSpawner.ShowObjects(xPrev - 2, zPrev - 1);
                proceduralSpawner.ShowObjects(xPrev - 2, zPrev + 1);
            }

            if (zPos > zPrev)
            {
                proceduralSpawner.HideObjects(xPrev,     zPrev - 1);
                proceduralSpawner.HideObjects(xPrev - 1, zPrev - 1);
                proceduralSpawner.HideObjects(xPrev + 1, zPrev - 1);

                proceduralSpawner.ShowObjects(xPrev,     zPrev + 2);
                proceduralSpawner.ShowObjects(xPrev - 1, zPrev + 2);
                proceduralSpawner.ShowObjects(xPrev + 1, zPrev + 2);
            }

            if (zPos < zPrev)
            {
                proceduralSpawner.HideObjects(xPrev,     zPrev + 1);
                proceduralSpawner.HideObjects(xPrev - 1, zPrev + 1);
                proceduralSpawner.HideObjects(xPrev + 1, zPrev + 1);

                proceduralSpawner.ShowObjects(xPrev,     zPrev - 2);
                proceduralSpawner.ShowObjects(xPrev - 1, zPrev - 2);
                proceduralSpawner.ShowObjects(xPrev + 1, zPrev - 2);
            }
            */

            xPrev = xPos;
            zPrev = zPos;
        }

        /*
        int xStart = (xPos > 0) ? xPos - 1 : xPos;
        int xEnd = (xPos < terrainGenerator.terrainWidth - 1) ? xPos + 1 : xPos;

        int zStart = (zPos > 0) ? zPos - 1 : zPos;
        int zEnd = (zPos < terrainGenerator.terrainDepth - 1) ? zPos + 1 : zPos;

        for (int z = zStart; z <= zEnd; z++)
        {
            for (int x = xStart; x <= xEnd; x++)
            {
                //if (!spawnedObjects[x * terrainGenerator.terrainWidth + z])
                //{
                //    //  Should spawn objects at this location
                //    Debug.Log("Should Start an Async Run");
                //    //Task.Run(async () =>
                //    //{
                //        Debug.Log("Start Async Run");
                //        SpawnObjectsAt(x, z);
                //    //});
                //}
            }
        }*/

    }

   

    //
    private void SpawnCharacter()
    {
        spawnedObjects = new bool[terrainGenerator.terrainWidth * terrainGenerator.terrainDepth];

        Vector3 hitPosition;
        if (terrainGenerator.CheckAt(characterSpawnPosition, out hitPosition))
        {
            characterSpawnPosition.y = hitPosition.y + 1.0f ;
        }

        character = Instantiate(characterPrefab, characterSpawnPosition, Quaternion.identity);

        Player2World(characterSpawnPosition, out xPrev, out zPrev);


        for (int z = 0; z < terrainGenerator.terrainDepth; z++)
        {
            for (int x = 0; x < terrainGenerator.terrainWidth; x++)
            {
                //proceduralSpawner.HideObjects(x, z);
            }
        }    

        //proceduralSpawner.ShowObjects(xPrev, zPrev);
        for (int z = zPrev - 1; z <= zPrev + 1; z++)
        {
            for (int x = xPrev - 1; x <= xPrev + 1; x++)
            {
                //proceduralSpawner.ShowObjects(x, z);
            }
        }

        /*
        int xPos = (int)(characterSpawnPosition.x / terrainGenerator.terrainTileSize);
        int zPos = (int)(characterSpawnPosition.z / terrainGenerator.terrainTileSize);

        int xStart = (xPos > 0) ? xPos - 1 : xPos;
        int xEnd = (xPos < terrainGenerator.terrainWidth - 1) ? xPos + 1 : xPos;

        int zStart = (zPos > 0) ? zPos - 1 : zPos;
        int zEnd = (zPos < terrainGenerator.terrainDepth - 1) ? zPos + 1 : zPos;

        for (int z = zStart; z <= zEnd; z++)
        {
            for (int x = xStart; x <= xEnd; x++)
            {
                SpawnObjectsAt(x, z);
            }
        }*/
    }

    private void SpawnObjectsAt(int x, int z)
    {
        spawningObjects = true;
        Debug.Log("Generate objects for : " + x + " / " + z);
        //  Spawn Objects
        //proceduralSpawner.PlaceObjects(x, z);
        spawnedObjects[x * terrainGenerator.terrainWidth + z] = true;
        spawningObjects = false;
    }

    private void Player2World(Vector3 playerPosition, out int x, out int z)
    {
        x = (int)(character.transform.position.x / terrainGenerator.terrainTileSize);
        z = (int)(character.transform.position.z / terrainGenerator.terrainTileSize);
    }

    
}
