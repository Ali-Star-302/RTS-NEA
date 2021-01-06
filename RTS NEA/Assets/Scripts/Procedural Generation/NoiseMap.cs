using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{
    ///<summary> Returns an array of floats with a given size, each float representing a value of perlin noise </summary>
    public float[,] GenerateNoiseMap(int mapSize, float scale, float _offsetX, float _offsetZ)
    {
        //int seed = GenerationValues.GetSeed();
        //System.Random random = new System.Random(1);
        //random.Next(-10000, 10000);
        
        float[,] noiseMap = new float[mapSize, mapSize];

        for (int z = 0; z < mapSize; z++) //Loops through length and width assigning perlin noise value
        {
            for (int x = 0; x < mapSize; x++)
            {
                float currentX = (x + _offsetX) / scale;
                float currentZ = (z + _offsetZ) / scale;

                noiseMap[x, z] = Mathf.PerlinNoise(currentX, currentZ);

                /*if (x == 0 && z == 0 && _offsetX == 0 && _offsetZ == 0)
                {
                    Debug.Log("Middle tl: " + currentX + ", " + currentZ);
                    Debug.Log("Perlin:" + Mathf.PerlinNoise(currentX, currentZ));
                }
                else if (x == 0 && z == 0 && _offsetX == 128 && _offsetZ == 0)
                {
                    Debug.Log("Right tl: " + currentX + ", " + currentZ);
                    Debug.Log("Perlin:" + Mathf.PerlinNoise(currentX, currentZ));
                }*/
            }
        }
        
        return noiseMap;
    }
}
