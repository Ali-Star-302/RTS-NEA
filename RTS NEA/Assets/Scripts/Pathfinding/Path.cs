using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Path
{
    public Vector3[] lookPoints;
    public Line[] turnBoundaries;
    public int targetIndex;
    public int decelerateIndex;

    public Path(Vector3[] waypoints, Vector3 startPos, float turnDst, float stoppingDst)
    {
        lookPoints = waypoints;
        turnBoundaries = new Line[lookPoints.Length];
        targetIndex = turnBoundaries.Length - 1;

        Vector2 previousPoint = new Vector2(startPos.x, startPos.z);
        for (int i = 0; i < lookPoints.Length; i++)
        {
            Vector2 currentPoint = new Vector2(lookPoints[i].x, lookPoints[i].z);
            Vector2 dirToCurrentPoint = (currentPoint - previousPoint).normalized;
            Vector2 turnBoundaryPoint = (i == targetIndex) ?currentPoint : currentPoint - dirToCurrentPoint * turnDst;
            turnBoundaries[i] = new Line(turnBoundaryPoint, previousPoint - dirToCurrentPoint * turnDst);
            previousPoint = turnBoundaryPoint;
        }

        float distanceFromTarget = 0;
        for (int i = lookPoints.Length-1; i > 0; i--)
        {
            distanceFromTarget += Vector3.Distance(lookPoints[i], lookPoints[i - 1]);
            if (distanceFromTarget > stoppingDst)
            {
                decelerateIndex = i;
                break;
            }
        }
    }

    public void DrawWithGizmos()
    {
        Gizmos.color = Color.black;
        foreach (Vector3 p in lookPoints)
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
