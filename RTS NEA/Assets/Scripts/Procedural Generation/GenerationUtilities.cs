using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerationUtilities
{
    public static TerrainRegion[] terrainRegions;
    public static AnimationCurve meshHeightCurve;

    ///<summary> Creates a texture which has colours determined by the region of terrain </summary>
    public static Texture2D CreateColourTexture(float[,] heightMap)
    {
        int mapLength = heightMap.GetLength(0); //Number of rows
        int mapWidth = heightMap.GetLength(1); //Number of columns

        Color[] colourMap = new Color[mapLength * mapWidth]; //Needs to be a 2D image flattened to a 1D array for the mapTexture.SetPixels part
        for (int z = 0; z < mapLength; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int currentPos = x * mapWidth + z; //Finds the current position in a 1D array with 2D array coordinates
                float currentHeight = heightMap[z, x]; //Sets current height in the loop equal to the one stored in the height map

                TerrainRegion _terrainRegion = ChooseTerrainRegion(currentHeight);

                colourMap[currentPos] = _terrainRegion.colour;
            }
        }

        //Sets the pixels of the texture to be colours determined by the colour map
        Texture2D mapTexture = new Texture2D(mapWidth, mapLength);
        //mapTexture.filterMode = FilterMode.Point;
        mapTexture.wrapMode = TextureWrapMode.Clamp;
        mapTexture.SetPixels(colourMap);
        mapTexture.Apply();

        return mapTexture;
    }

    ///<summary> Loops through the terrain regions and returns the first one above the given height value </summary>
    static TerrainRegion ChooseTerrainRegion(float terrainHeight)
    {
        foreach (TerrainRegion terrainRegion in terrainRegions)
        {
            if (terrainHeight < terrainRegion.height)
            {
                return terrainRegion;
            }
        }
        return terrainRegions[terrainRegions.Length - 1];
    }

    /// <summary> Generates and returns a mesh </summary>
    public static Mesh GenerateMesh(float[,] heightMap, float heightScale, AnimationCurve heightCurve, Vector3 position)
    {
        int meshWidth = heightMap.GetLength(0);
        int meshHeight = heightMap.GetLength(1);

        //These values are used to find the top left point i.e. the first point of the mesh
        float minimumX = (meshWidth - 1) / -2f;         //Formula to find the point furthest to the left from the centre
        float maximumZ = (meshHeight - 1) / 2f;         //Formula to find the point furthest to the top from the centre

        Vector3[] vertices = new Vector3[meshWidth * meshHeight]; //List of the points of all vertices in the mesh
        int[] triangles = new int[(meshWidth - 1) * (meshHeight - 1) * 6]; //List of all triangles that make up the mesh
        Vector2[] uvs = new Vector2[meshWidth * meshHeight]; //List of the texture coordinates of the mesh

        int triangleIndex = 0;
        int vertexIndex = 0;

        for (int z = 0; z < meshHeight; z++)
        {
            for (int x = 0; x < meshWidth; x++)
            {
                vertices[vertexIndex] = new Vector3(minimumX + x, heightCurve.Evaluate(heightMap[x, z]) * heightScale, maximumZ - z); //Sets the x, y and z of all the vertices that will make up the mesh
                uvs[vertexIndex] = new Vector2(x / (float)meshWidth, z / (float)meshHeight); //Sets the uv of the vertex to be a percentage of the whole mesh

                /*if (x % 5 == 0 && z % 5 == 0)
                {
                    GameObject cube = GameObject.CreatePrimitive(PrimitiveType.Cube);
                    cube.transform.position = new Vector3(minimumX + x + position.x, heightCurve.Evaluate(heightMap[x, z]) * heightScale, maximumZ - z + position.z);
                }*/

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

    public static void GenerateRandomSeed()
    {
        //Sets up the seed which can then be accessed by each of the terrain tiles
        System.Random random = new System.Random();
        GenerationValues.SetSeed(random.Next(int.MinValue, int.MaxValue));
    }
}
