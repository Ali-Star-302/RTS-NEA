using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    /// <summary> Generates a mesh using the mesh data class </summary>
    public static MeshData GenerateMesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve)
    {
        int meshWidth = heightMap.GetLength(0);
        int meshHeight = heightMap.GetLength(1);
        float topLeftX = (meshWidth - 1) / -2f;
        float topLeftZ = (meshHeight - 1) / 2f;

        MeshData meshData = new MeshData(meshWidth, meshHeight);
        int vertexIndex = 0;

        for (int z = 0; z < meshHeight; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                meshData.vertices[vertexIndex] = new Vector3(topLeftX + x, heightCurve.Evaluate(heightMap[z, x]) * heightScale, topLeftZ - z);
                meshData.uvs[vertexIndex] = new Vector2(x / (float)meshWidth, z / (float)meshHeight);

                if (x < meshWidth - 1 && z < meshHeight - 1)
                {
                    meshData.AddTriangle(vertexIndex, vertexIndex + meshWidth + 1, vertexIndex + meshWidth);
                    meshData.AddTriangle(vertexIndex + meshWidth + 1, vertexIndex, vertexIndex + 1);
                }

                vertexIndex++;
            }
        }

        return meshData;
    }
}