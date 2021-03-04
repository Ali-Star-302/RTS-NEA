using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPrefab;
    public GridManager gridManager;
    public int mapSize = 1;
    int chunkSize;

    GameObject[] terrainObjects;
    

    void Awake()
    {

        mapSize = GenerationValues.GetMapSize();

        if (GenerationValues.GetSeed() == 0)
            GenerationUtilities.GenerateRandomSeed();

        chunkSize = GenerationValues.GetChunkSize();
        Vector3 placementPos;
        Vector3 newPlacementPos;
        terrainObjects = new GameObject[mapSize*mapSize];

        if (mapSize == 1)
        {
            placementPos = new Vector3(0, 0, 0);
            terrainObjects[0] = Instantiate(terrainPrefab, placementPos, Quaternion.identity);
        }
        else
        {
            placementPos = new Vector3(((mapSize / 2f) - 0.5f) * -chunkSize, 0, ((mapSize / 2f) - 0.5f) * chunkSize); //Finds the top left centre point where the first chunk will be generated
            newPlacementPos = placementPos;
            int meshCounter = 0;

            for (int z = 1; z <= mapSize; z++)
            {
                for (int x = 1; x <= mapSize;x++)
                {
                    
                    if (x == 1 && z == 1) //When its top left use the placement position as its position
                    {
                        terrainObjects[0] = Instantiate(terrainPrefab, new Vector3(placementPos.x, 0, placementPos.z), Quaternion.identity);
                    }
                    else
                    {
                        terrainObjects[meshCounter] = Instantiate(terrainPrefab, new Vector3(newPlacementPos.x, 0, newPlacementPos.z), Quaternion.identity);
                    }
                    terrainObjects[meshCounter].name = "Terrain Mesh " + meshCounter;
                    newPlacementPos = new Vector3(placementPos.x + (x * 128), 0, newPlacementPos.z); //When iterating horizontally this is the maths which finds the positiona
                    terrainObjects[meshCounter].GetComponent<MapGeneration>().StartGeneration();
                    meshCounter++;
                }
                newPlacementPos = new Vector3(placementPos.x, 0, placementPos.z - (z * 128)); //When a vertical iteration is done this finds the new location
            }
        }

        gridManager.StartGridCreation();
    }
}
