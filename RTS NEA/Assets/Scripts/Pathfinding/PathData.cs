using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class PathData
{
    ///<summary> Class which holds all data needed to create a path </summary>
    public Vector3 pathStart;
    public Vector3 pathEnd;
    public Unit unitInstance; //Holds the instance of the unit script so the specific instance's methods can be called

    public PathData(Vector3 _start, Vector3 _end, Unit _unitInstance)
    {
        pathStart = _start;
        pathEnd = _end;
        unitInstance = _unitInstance;
    }
}
