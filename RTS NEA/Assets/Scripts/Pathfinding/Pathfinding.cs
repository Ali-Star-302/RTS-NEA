using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class Pathfinding : MonoBehaviour
{
    GridScript grid;

    void Awake()
    {
        grid = this.GetComponent<GridScript>();
    }

    public void FindPath(PathRequest request, Action<PathResult> callback)
    {
        Vector3[] waypoints = new Vector3[0];
        bool pathSuccess = false;

        Node startNode = grid.GetNodeInWorld(request.pathStart);
        Node targetNode = grid.GetNodeInWorld(request.pathEnd);

        if (startNode.walkable && targetNode.walkable)
        {
            Heap<Node> openList = new Heap<Node>(grid.MaxSize); //nodes that need to be checked
            HashSet<Node> closedList = new HashSet<Node>(); //nodes that have been checked
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

                foreach (Node n in grid.GetNeighbouringNodes(currentNode))
                {
                    if (!n.walkable || closedList.Contains(n))
                    { //skip node if its not walkable or is already in the closed set
                        continue;
                    }

                    //update neighbour variables if there is a quicker path or its already been checked
                    int newMovementCostToNeighbour = currentNode.gScore + GetDistance(currentNode, n) + n.movementPenalty;
                    if (newMovementCostToNeighbour < n.gScore || !openList.Contains(n))
                    {
                        n.gScore = newMovementCostToNeighbour;
                        n.hScore = GetDistance(n, targetNode);
                        n.parent = currentNode;

                        if (!openList.Contains(n))
                            openList.Add(n);
                        else
                            openList.UpdateItem(n);
                    }
                }
            }
        }
        if (pathSuccess)
        {
            waypoints = RetracePath(startNode, targetNode);
            pathSuccess = waypoints.Length > 0;
        }
        callback(new PathResult(waypoints, pathSuccess, request.callback));

    }

    Vector3[] RetracePath(Node start, Node end) //goes backwards through path using the nodes' parents
    {
        List<Node> path = new List<Node>();
        Node currentNode = end;

        while (currentNode != start)
        {
            path.Add(currentNode);
            currentNode = currentNode.parent;
        }
        Vector3[] waypoints = SimplifyPath(path);
        Array.Reverse(waypoints);
        return waypoints;
    }

    Vector3[] SimplifyPath(List<Node> path) //gets rid of unnecessary waypoints, only keeping ones where the unit must change direction
    {
        List<Vector3> waypoints = new List<Vector3>();
        Vector2 directionOld = Vector2.zero;

        for (int i = 1; i < path.Count; i++)
        {
            Vector2 directionNew = new Vector2(path[i - 1].gridX - path[i].gridX, path[i - 1].gridY - path[i].gridY);
            if (directionNew != directionOld)
            {
                waypoints.Add(path[i - 1].worldPosition);
            }
            directionOld = directionNew;
        }
        return waypoints.ToArray();
    }

    int GetDistance(Node a, Node b) //uses maths formula to find distance between 2 nodes
    {
        int xDistance = Mathf.Abs(a.gridX - b.gridX);
        int yDistance = Mathf.Abs(a.gridZ - b.gridZ);

        return 14 * Mathf.Min(xDistance, yDistance) + 10 * Mathf.Abs(xDistance - yDistance);
    }
}
