using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    GridScript grid;

    void Awake()
    {
        grid = GetComponent<GridScript>();
    }

    ///<summary> Finds a path using the A* algorithm </summary>
    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromPosition(request.pathStart);
        Node targetNode = grid.GetNodeFromPosition(request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openList = new Heap<Node>(grid.GetMaxSize); //Nodes that need to be checked
            HashSet<Node> closedList = new HashSet<Node>(); //Nodes that have been checked
            openList.Add(startNode);

            while (openList.Count > 0)
            {
                Node currentNode = openList.RemoveFirst();
                closedList.Add(currentNode);

                if (currentNode == targetNode)
                {
                    pathSuccess = true;
                    break;
                }

                foreach (Node _node in grid.GetNeighbouringNodes(currentNode))
                {
                    //Skip node if its not walkable or is already in the closed set
                    if (!_node.walkable || closedList.Contains(_node))
                        continue;

                    //Update neighbour variables if there is a quicker path or if its already been checked
                    int updatedScoreToNeighbour = currentNode.gScore + GetDistance(currentNode, _node) + _node.movementPenalty;
                    if (updatedScoreToNeighbour < _node.gScore || !openList.Contains(_node))
                    {
                        _node.gScore = updatedScoreToNeighbour;
                        _node.hScore = GetDistance(_node, targetNode);
                        _node.parent = currentNode;

                        if (!openList.Contains(_node))
                            openList.Add(_node);
                        else
                            openList.UpdateItem(_node);
                    }
                }
            }
        }
        //If a path is found the path is reversed and simplified
        if (pathSuccess)
        {
            waypoints = CreateFinalPath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));

    }

    ///<summary> Creates the finished path from the start and end node </summary>
    Vector3[] CreateFinalPath(Node startNode, Node targetNode)
    {
        List<Node> startingPath = new List<Node>();
        Node currentNode = targetNode;

        //Iteratively adds the nodes to a list and goes to the parent of that node (works backwards from the target to the startNode)
        while (currentNode != startNode)
        {
            startingPath.Add(currentNode);
            currentNode = currentNode.parent;
        }

        List<Vector3> simplifiedPath = new List<Vector3>(); //New list which holds the simplified path
        Vector2 oldGradient = Vector2.zero;

        //Iterates through the points in the path finding whether the gradient/direction between two nodes has changed
        for (int i = 1; i < startingPath.Count; i++)
        {
            Vector2 newGradient = new Vector2(startingPath[i - 1].gridX - startingPath[i].gridX, startingPath[i - 1].gridZ - startingPath[i].gridZ);
            if (newGradient != oldGradient)
                simplifiedPath.Add(startingPath[i - 1].worldPosition);

            oldGradient = newGradient;
        }

        Vector3[] finalPath = simplifiedPath.ToArray();
        Array.Reverse(finalPath);

        return finalPath;
    }

    ///<summary> Uses formula to find the distance between 2 nodes </summary>
    int GetDistance(Node a, Node b) 
    {
        int xDistance = Mathf.Abs(a.gridX - b.gridX);
        int yDistance = Mathf.Abs(a.gridZ - b.gridZ);

        return 14 * Mathf.Min(xDistance, yDistance) + 10 * Mathf.Abs(xDistance - yDistance);
    }
}
