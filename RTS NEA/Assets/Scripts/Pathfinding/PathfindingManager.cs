using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System;

public class PathfindingManager : MonoBehaviour
{
    Queue<PathData> pathQueue = new Queue<PathData>();
    PathData currentPathData;

    static PathfindingManager instance;
    Pathfinding pathfinding;

    bool processing;

    void Awake()
    {
        instance = this;
        pathfinding = GetComponent<Pathfinding>();
    }

    ///<summary> Allows a path to be requested </summary>
    public static void RequestPath(Vector3 pathStart, Vector3 pathEnd, Action<Vector3[], bool> callback)
    {
        PathData path = new PathData(pathStart, pathEnd, callback);
        instance.pathQueue.Enqueue(path);
        instance.ProcessNextPath();
    }

    void ProcessNextPath()
    {
        if (!processing && pathQueue.Count > 0)
        {
            currentPathData = pathQueue.Dequeue();
            processing = true;
            pathfinding.BeginPathfinding(currentPathData.pathStart, currentPathData.pathEnd);
        }
    }

    public void FinishedProcessingPath(Vector3[] path, bool success)
    {
        currentPathData.callback(path, success);
        processing = false;
        ProcessNextPath();
    }

    ///<summary> Class which holds all data needed to create a path </summary>
    class PathData
    {
        public Vector3 pathStart;
        public Vector3 pathEnd;
        public Action<Vector3[], bool> callback;

        public PathData(Vector3 _start, Vector3 _end, Action<Vector3[], bool> _callback)
        {
            pathStart = _start;
            pathEnd = _end;
            callback = _callback;
        }
    }
}
