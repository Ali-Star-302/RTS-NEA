using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class GenerationValues
{
    public static int seed;

    public static int GetChunkSize()
    {
        return 128;
    }

    public static int GetSeed()
    {
        return seed;
    }
}
