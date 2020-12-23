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
    TerrainRegion[] terrainRegions;

    public AnimationCurve meshHeightCurve;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    void Awake()
    {
        _meshRenderer = this.GetComponent<MeshRenderer>();
        _meshFilter = this.GetComponent<MeshFilter>();
        _meshCollider = this.GetComponent<MeshCollider>();

        GenerateTile();
    }

    void Start()
    {
        
    }

    void GenerateTile()
    {
        /*//Finds the length and width based on the size of the plane in the scene
        Vector3[] meshVertices = this._meshFilter.mesh.vertices;
        int mapLength = (int)Mathf.Sqrt(meshVertices.Length);
        int mapWidth = mapLength;*/

        //Finds the array of points based on the perlin noise map
        float[,] heightMap = this.noiseMap.GenerateNoiseMap(mapLength, mapWidth, this.mapScale);

        //Creates the texture from the array of points
        Texture2D mapTexture = CreateColourTexture(heightMap);
        this._meshRenderer.material.mainTexture = mapTexture;

        MeshData _meshData;
        _meshData = MeshGenerator.GenerateMesh(heightMap, heightScale, meshHeightCurve);
        Mesh createdMesh = _meshFilter.sharedMesh = _meshData.CreateMesh();
        _meshRenderer.sharedMaterial.mainTexture = mapTexture;
        _meshCollider.sharedMesh = createdMesh;
    }

    private Texture2D CreateColourTexture(float[,] heightMap)
    {
        int mapLength = heightMap.GetLength(0); //number of rows
        int mapWidth = heightMap.GetLength(1); //number of columns

        Color[] colourMap = new Color[mapLength * mapWidth];
        for (int z = 0; z < mapLength; z++)
        {
            for (int x = 0; x < mapWidth; x++)
            {
                int currentPos = x * mapWidth + z;
                float height = heightMap[x, z]; //Sets height equal to the one stored in the height map

                TerrainRegion _terrainRegion = ChooseTerrainRegion(height);

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

    TerrainRegion ChooseTerrainRegion(float height)
    {
        // for each terrain type, check if the height is lower than the one for the terrain type
        foreach (TerrainRegion terrainRegion in terrainRegions)
        {
            // return the first terrain type whose height is higher than the generated one
            if (height < terrainRegion.height)
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