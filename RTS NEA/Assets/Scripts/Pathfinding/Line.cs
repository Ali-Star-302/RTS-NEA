using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Line
{
    const float verticalLineGradient = 100000f;

    bool approachSide;
    float gradient;
    float yIntercept;
    float perpendicularGradient;
    Vector2 linePoint1;
    Vector2 linePoint2;

    public Line(Vector2 pointOnLine, Vector2 pointPerpendicularToLine)
    {
        float deltaX = pointOnLine.x - pointPerpendicularToLine.x; //Difference in x of 
        float deltaY = pointOnLine.y - pointPerpendicularToLine.y;

        if (deltaX == 0)
            perpendicularGradient = verticalLineGradient;
        else
            perpendicularGradient = deltaY / deltaX;

        if (perpendicularGradient == 0)
            gradient = verticalLineGradient;
        else
            gradient = -1 / perpendicularGradient;

        yIntercept = pointOnLine.y - gradient * pointOnLine.x;
        linePoint1 = pointOnLine;
        linePoint2 = pointOnLine + new Vector2(1, gradient);

        approachSide = false;
        approachSide = GetSide(pointPerpendicularToLine);
    }

    bool GetSide(Vector2 p)
    {
        return (p.x - linePoint1.x) * (linePoint2.y - linePoint1.y) > (p.y - linePoint1.y) * (linePoint2.x - linePoint1.x);
    }

    public bool HasCrossedLine(Vector2 p)
    {
        return GetSide(p) != approachSide;
    }

    public float DistanceFromPoint(Vector2 p)
    {
        float yInterceptPerpendicular = p.y - perpendicularGradient * p.x;
        float intersectX = (yInterceptPerpendicular - yIntercept) / (gradient - perpendicularGradient);
        float intersectY = gradient * intersectX + yIntercept;
        return Vector2.Distance(p, new Vector2(intersectX, intersectY));
    }

    public void DrawWithGizmos(float length)
    {
        Vector3 lineDir = new Vector3(1, 0, gradient).normalized;
        Vector3 lineCentre = new Vector3(linePoint1.x, 0, linePoint1.y) + Vector3.up;
        Gizmos.DrawLine(lineCentre - lineDir * length / 2f, lineCentre + lineDir * length / 2f);
    }
}
