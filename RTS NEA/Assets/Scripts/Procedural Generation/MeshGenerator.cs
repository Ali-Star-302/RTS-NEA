using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class MeshGenerator
{
    /// <summary> Generates a mesh using the mesh data class </summary>
    public static Mesh GenerateMesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve)
    {
        int meshWidth = heightMap.GetLength(0);
        int meshHeight = heightMap.GetLength(1);

        //These values are used to find the top left point i.e. the first point of the mesh
        float minimumX = (meshWidth - 1) / -2f;         //Formula to find the point furthest to the left from the centre
        float maximumZ = (meshHeight - 1) / 2f;         //Formula to find the point furthest to the top from the centre

        Vector3[] vertices =  new Vector3[meshWidth * meshHeight]; //List of the points of all vertices in the mesh
        int[] triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6]; //List of all triangles that make up the mesh
        Vector2[] uvs = new Vector2[meshWidth * meshHeight]; //List of the texture coordinates of the mesh

        int triangleIndex = 0;
        int vertexIndex = 0;


        for (int z = 0; z < meshHeight; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                vertices[vertexIndex] = new Vector3(minimumX + x, heightCurve.Evaluate(heightMap[z, x]) * heightScale, maximumZ - z); //Sets the x, y and z of all the vertices that will make up the mesh
                uvs[vertexIndex] = new Vector2(x / (float)meshWidth, z / (float)meshHeight); //Sets the uv of the vertex to be a percentage of the whole mesh

                if (x < meshWidth - 1 && z < meshHeight - 1) //Triangles don't need to be made when the vertex is along the right or bottom
                {
                    AddTriangle(vertexIndex, vertexIndex + meshWidth + 1, vertexIndex + meshWidth, triangles, ref triangleIndex); //Creates a triangle in the pattern: top left, bottom right, bottom left
                    AddTriangle(vertexIndex, vertexIndex + 1, vertexIndex + meshWidth + 1, triangles, ref triangleIndex); //Creates a triangle in the pattern: top left, top right, bottom right
                }
                vertexIndex++;
            }
        }

        //Creates the mesh from the calculated properties
        Mesh mesh = new Mesh();
        mesh.vertices = vertices;
        mesh.uv = uvs;
        mesh.triangles = triangles;
        mesh.RecalculateNormals();
        mesh.RecalculateBounds();

        return mesh;
    }

    /// <summary> Adds a triangle with the vertices at the given points </summary>
    public static void AddTriangle(int pointA, int pointB, int pointC, int[] _triangles, ref int _triangleIndex)
    {
        _triangles[_triangleIndex] = pointA;
        _triangleIndex++;
        _triangles[_triangleIndex] = pointB;
        _triangleIndex++;
        _triangles[_triangleIndex] = pointC;
        _triangleIndex++;
    }
}