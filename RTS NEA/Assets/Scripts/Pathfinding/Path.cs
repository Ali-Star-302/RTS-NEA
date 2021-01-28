using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public int decelerationPoint;
    public Vector3[] waypoints;
    public Line[] turnBoundaries; //Array of lines which are where the unit begins to turn
    
    ///<summary> Path class used for the when the unit follows the path </summary>
    public Path(Vector3[] _waypoints, Vector3 startPosition, float turnRadius, float stoppingDistance)
    {
        waypoints = _waypoints;
        turnBoundaries = new Line[waypoints.Length];

        Vector2 previousPoint = new Vector2(startPosition.x, startPosition.z); //Ensures the previous point is the start point at the start of the path
        for (int i = 0; i < waypoints.Length; i++)
        {
            Vector2 currentLocation = new Vector2(waypoints[i].x, waypoints[i].z);
            Vector2 currentDirection = (currentLocation - previousPoint).normalized;
            Vector2 turnLocation;

            //When the current location is the target, the unit doesn't need to turn
            if (i == turnBoundaries.Length - 1)
                turnLocation = currentLocation;
            else
                turnLocation = currentLocation - currentDirection * turnRadius;

            turnBoundaries[i] = new Line(turnLocation, previousPoint - currentDirection * turnRadius);
            previousPoint = turnLocation;
        }

        float distanceFromTarget = 0;
        //Iterates from the target waypoint backwards summing the distance between each waypoint, finding the index where the unit should start decelerating
        for (int i = waypoints.Length-1; i > 0; i--)
        {
            distanceFromTarget += Vector3.Distance(waypoints[i], waypoints[i - 1]);
            if (stoppingDistance < distanceFromTarget)
            {
                decelerationPoint = i;
                break;
            }
        }
    }


    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in waypoints)
        {
            Gizmos.DrawCube(p + Vector3.up, Vector3.one);
        }

        Gizmos.color = Color.white;
        foreach (Line l in turnBoundaries)
        {
            l.DrawWithGizmos(10);
        }
    }
}
