using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Node : IHeapItem<Node>
{
    public bool walkable;
    public Vector3 worldPosition;
    public int gridX, gridY, gridZ;
    public int movementPenalty;

    public int gScore;
    public int hScore;
    public Node parent;
    int heapIndex;
    float steepness;

    public Node(bool _walkable, Vector3 _worldPos, int _gridX, int _gridY, int _gridZ, int _penalty, float _steepness)
    {
        walkable = _walkable;
        worldPosition = _worldPos;
        gridX = _gridX;
        gridY = _gridY;
        gridZ = _gridZ;
        movementPenalty = _penalty;
        steepness = _steepness;
    }

    public int GetFScore
    {
        get
        {
            return gScore + hScore;
        }
    }

    public int GetHeapIndex
    {
        get
        {
            return heapIndex;
        }
        set
        {
            heapIndex = value;
        }
    }

    public int CompareTo(Node nodeToCompare)
    {
        int compare = GetFScore.CompareTo(nodeToCompare.GetFScore);
        if (compare == 0)
        {
            compare = hScore.CompareTo(nodeToCompare.hScore);
        }
        return -compare;
    }
}
