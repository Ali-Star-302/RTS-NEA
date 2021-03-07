using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TerrainEditor : MonoBehaviour
{
    public float mapScale, heightScale;
    public int mapSize;
    public TerrainRegion[] terrainRegions;
    public AnimationCurve meshHeightCurve;

    private void Awake()
    {
        GenerationUtilities.terrainRegions = terrainRegions;
        GenerationUtilities.meshHeightCurve = meshHeightCurve;
        GenerationValues.SetMapSize(mapSize);
        GenerationValues.SetMapScale(mapScale);
        GenerationValues.SetHeightScale(heightScale);
    }

    public Texture2D UpdateMapPreview()
    {
        float[,] noiseMap = NoiseMap.GenerateNoiseMap(GenerationValues.GetMapSize() * GenerationValues.GetChunkSize()+1, GenerationValues.GetMapScale(), 0,0, "Bob");
        return GenerationUtilities.CreateColourTexture(noiseMap);
    }
}
