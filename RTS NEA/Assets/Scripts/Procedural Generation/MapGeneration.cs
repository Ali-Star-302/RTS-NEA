using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MapGeneration : MonoBehaviour
{
    [SerializeField]
    NoiseMap noiseMap;

    [SerializeField]
    int mapLength, mapWidth;

    [SerializeField]
    float mapScale, heightScale;

    [SerializeField]
    TerrainRegion[] terrainRegions; //The 1D array of available terrains and their properties

    public AnimationCurve meshHeightCurve;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    void Awake()
    {
        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        GenerateTerrain();
    }

    ///<summary> Creates a terrain mesh using the perlin noise </summary>
    void GenerateTerrain()
    {
        /*//Finds the length and width based on the size of the plane in the scene
        Vector3[] meshVertices = this._meshFilter.mesh.vertices;
        int mapLength = (int)Mathf.Sqrt(meshVertices.Length);
        int mapWidth = mapLength;*/

        //Finds the array of floats based on the perlin noise map
        float[,] heightMap = noiseMap.GenerateNoiseMap(mapLength, mapWidth, this.mapScale);

        //Creates the texture from the array of floats
        Texture2D mapTexture = CreateColourTexture(heightMap);
        _meshRenderer.material.mainTexture = mapTexture;

        MeshData _meshData;
        _meshData = MeshGenerator.GenerateMesh(heightMap, heightScale, meshHeightCurve);
        Mesh createdMesh = _meshFilter.sharedMesh = _meshData.CreateMesh();
        _meshRenderer.sharedMaterial.mainTexture = mapTexture;
        _meshCollider.sharedMesh = createdMesh;
    }

    ///<summary> Creates a texture which has colours determined by the region of terrain </summary>
    private Texture2D CreateColourTexture(float[,] heightMap)
    {
        int mapLength = heightMap.GetLength(0); //Number of rows
        int mapWidth = heightMap.GetLength(1); //Number of columns

        Color[] colourMap = new Color[mapLength * mapWidth]; //DONT UNDERSTAND THIS PART, NEEDS VIDEO HELP
        for (int z = 0; z < mapLength; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int currentPos = x * mapWidth + z;
                float currentHeight = heightMap[x, z]; //Sets current height in the loop equal to the one stored in the height map

                TerrainRegion _terrainRegion = ChooseTerrainRegion(currentHeight); //

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
    TerrainRegion ChooseTerrainRegion(float terrainHeight)
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
}

[System.Serializable]
public class TerrainRegion
{
    public string name;
    public float height;
    public Color colour;
}