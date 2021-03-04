using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerationValues //Static class for holding constant values used by range of different generations scripts
{
    static int seed;
    static int mapSize;
    static float mapScale, heightScale;

    public static int GetChunkSize()
    {
        return 128;
    }

    public static int GetSeed()
    {
        return seed;
    }

    public static void SetSeed(int _seed)
    {
        seed = _seed;
    }

    public static int GetMapSize()
    {
        return mapSize;
    }

    public static void SetMapSize(int _mapSize)
    {
        mapSize = _mapSize;
    }

    public static float GetMapScale()
    {
        return mapScale;
    }

    public static void SetMapScale(float _mapScale)
    {
        mapScale = _mapScale;
    }

    public static float GetHeightScale()
    {
        return heightScale;
    }

    public static void SetHeightScale(float _heightScale)
    {
        heightScale = _heightScale;
    }
}
