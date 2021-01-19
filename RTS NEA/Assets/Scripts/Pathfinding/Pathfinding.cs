using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    PathfindingManager pathfindingManager;
    GridScript grid;

    void Awake()
    {
        pathfindingManager = GetComponent<PathfindingManager>();
        grid = GetComponent<GridScript>();
    }

    ///<summary> Finds a path using the A* algorithm </summary>
    public IEnumerator FindPath(Vector3 startPos, Vector3 targetPos)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeFromPosition(startPos);
        Node targetNode = grid.GetNodeFromPosition(targetPos);

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
                    //Only updates the node if it is walkable and isn't already in the closed set
                    if (_node.walkable && !closedList.Contains(_node))
                    {
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
        }
        yield return null;

        //If a path is found the path is reversed and simplified
        if (pathSuccess)
        {
            waypoints = CreateFinalPath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }

        pathfindingManager.FinishedPath(waypoints, pathSuccess); //Sends the completed data to the pathfinding manager

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
