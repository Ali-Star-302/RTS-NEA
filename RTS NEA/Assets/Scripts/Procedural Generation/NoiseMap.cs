using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    public float[,] GenerateNoiseMap(int mapLength, int mapWidth, float scale)
    {
        float[,] noiseMap = new float[mapLength, mapWidth];
        for (int z = 0; z < mapLength; z++) //Loops through length and width assigning perlin noise value
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentX = x / scale;
                float currentZ = z / scale;

                noiseMap[x, z] = Mathf.PerlinNoise(currentX, currentZ);
            }
        }
        return noiseMap;
    }
}
