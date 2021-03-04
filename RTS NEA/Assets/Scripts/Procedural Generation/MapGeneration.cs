using System.Collections;
using System.Collections.Generic;
using UnityEngine;

[RequireComponent(typeof(MeshFilter), typeof(MeshRenderer), typeof(MeshCollider))]
public class MapGeneration : MonoBehaviour
{
    public TerrainRegion[] terrainRegions; //The 1D array of available terrains and their properties
    public AnimationCurve meshHeightCurve;

    MeshRenderer _meshRenderer;
    MeshFilter _meshFilter;
    MeshCollider _meshCollider;

    int chunkSize;

    public void StartGeneration()
    {
        chunkSize = GenerationValues.GetChunkSize();

        _meshRenderer = GetComponent<MeshRenderer>();
        _meshFilter = GetComponent<MeshFilter>();
        _meshCollider = GetComponent<MeshCollider>();

        GenerateTerrain();
    }

    ///<summary> Creates a terrain mesh using the perlin noise </summary>
    void GenerateTerrain()
    {
        float offsetX = gameObject.transform.position.x - chunkSize/2;
        float offsetZ = gameObject.transform.position.z + chunkSize/2;

        //Finds the array of floats based on the perlin noise map
        float[,] heightMap = NoiseMap.GenerateNoiseMap(chunkSize+1, GenerationValues.GetMapScale(), offsetX, offsetZ, gameObject.name); //Chunk size needs to be 1 bigger as the number of vertices is 1 less than the size

        //Creates the texture from the array of floats
        Texture2D mapTexture = GenerationUtilities.CreateColourTexture(heightMap);
        _meshRenderer.material.mainTexture = mapTexture;

        Mesh createdMesh = GenerationUtilities.GenerateMesh(heightMap, GenerationValues.GetHeightScale(), meshHeightCurve, transform.position);
        _meshFilter.sharedMesh = createdMesh;
        _meshRenderer.sharedMaterial.mainTexture = mapTexture;
        _meshCollider.sharedMesh = createdMesh;
    }
}

[System.Serializable]
public class TerrainRegion
{
    public string name;
    public float height;
    public Color colour;
}