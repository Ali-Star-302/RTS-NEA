using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingManager : MonoBehaviour
{
    static PathfindingManager thisInstance;

    Queue<PathData> pathQueue = new Queue<PathData>();
    PathData currentPathData;
    Pathfinding pathfinding;
    bool processing;

    void Awake()
    {
        thisInstance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    ///<summary> Gets a new path which is put in the path queue </summary>
    public static void GetPath(Vector3 pathStart, Vector3 pathEnd, Unit unitInstance)
    {
        PathData path = new PathData(pathStart, pathEnd, unitInstance);
        thisInstance.pathQueue.Enqueue(path);
        thisInstance.ProcessNextPath();
    }

    ///<summary> Sends the finished path back to the unit which requested a path and moves on to the next path </summary>
    public void FinishedPath(Vector3[] path, bool success)
    {
        currentPathData.unitInstance.GetComponent<Unit>().PathFound(path, success);
        processing = false;
        ProcessNextPath();
    }

    ///<summary> If available, the next path in the queue is processed </summary>
    void ProcessNextPath()
    {
        if (pathQueue.Count > 0 && !processing)
        {
            currentPathData = pathQueue.Dequeue();
            processing = true;
            StartCoroutine(pathfinding.FindPath(currentPathData.pathStart, currentPathData.pathEnd)); //Begins the process of finding a path for the specific unit
        }
    }
}
