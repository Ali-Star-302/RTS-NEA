using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    ///<summary> Returns an array of floats with a given size, each float representing a value of perlin noise </summary>
    public float[,] GenerateNoiseMap(int mapLength, int mapWidth, float scale)
    {
        int offset = Random.Range(0, 10000);
        float[,] noiseMap = new float[mapLength, mapWidth];
        for (int z = 0; z < mapLength; z++) //Loops through length and width assigning perlin noise value
        {
            for (int x = 0; x < mapWidth; x++)
            {
                float currentX = x / scale;
                float currentZ = z / scale;
                noiseMap[x, z] = Mathf.PerlinNoise(currentX + offset, currentZ + offset);
            }
        }
        return noiseMap;
    }
}
