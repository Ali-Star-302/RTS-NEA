using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainManager : MonoBehaviour
{
    public GameObject terrainPrefab;
    public int mapSize = 1;
    int chunkSize;

    GameObject[] terrainObjects;
    

    void Awake()
    {
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
                    
                    if (x == 1 && z == 1)
                    {
                        terrainObjects[0] = Instantiate(terrainPrefab, new Vector3(placementPos.x, 0, placementPos.z), Quaternion.identity);
                    }
                    else
                    {
                        terrainObjects[meshCounter] = Instantiate(terrainPrefab, new Vector3(newPlacementPos.x, 0, newPlacementPos.z), Quaternion.identity);
                    }
                    terrainObjects[meshCounter].name = "Terrain Mesh " + meshCounter;
                    newPlacementPos = new Vector3(placementPos.x + (x * 128), 0, newPlacementPos.z);
                    meshCounter++;
                }
                newPlacementPos = new Vector3(placementPos.x, 0, placementPos.z - (z * 128));
            }
        }
    }
}
