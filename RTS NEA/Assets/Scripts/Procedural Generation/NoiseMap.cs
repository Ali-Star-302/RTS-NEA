using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class NoiseMap : MonoBehaviour
{

    ///<summary> Returns an array of floats with a given size, each float representing a value of perlin noise </summary>
    public static float[,] GenerateNoiseMap(int chunkSize, float scale, float _offsetX, float _offsetZ, string name)
    {
        System.Random random = new System.Random(GenerationValues.GetSeed());
        int randomOffset = random.Next(1000, 100000); //Stops weird tiling when the offset is near 0

        float[,] noiseMap = new float[chunkSize, chunkSize];
        /*GameObject parentObj = new GameObject();
        parentObj.name = "Cubes " + name;*/

        for (int z = 0; z < chunkSize; z++) //Loops through length and width assigning perlin noise value
        {
            for (int x = 0; x < chunkSize; x++)
            {
                float currentX = (_offsetX + x) / scale;
                float currentZ = (_offsetZ - z) / scale;

                /*if (x % 5 == 0 && z % 5 == 0)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(_offsetX + x, Mathf.PerlinNoise(currentX + 200, currentZ + 200) * 20, _offsetZ - z);
                    cube.name = "Cube " + name + ": " + x + ", " + z;
                    cube.transform.parent = parentObj.transform;
                }*/

                noiseMap[x, z] = Mathf.PerlinNoise(currentX + randomOffset, currentZ + randomOffset);
            }
        }
        return noiseMap;
    }
}
