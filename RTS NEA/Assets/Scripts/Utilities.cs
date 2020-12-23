using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static Texture2D whiteTexture;

    public static Texture2D WhiteTexture
    {
        get
        {
            if (whiteTexture == null)
            {
                whiteTexture = new Texture2D(1, 1);
                whiteTexture.SetPixel(0, 0, Color.white);
                whiteTexture.Apply();
            }

            return whiteTexture;
        }
    }

    public static void DrawRectangle(Rect rect, Color color)
    {
        GUI.color = color;
        GUI.DrawTexture(rect, WhiteTexture);
        GUI.color = Color.white;
    }

    public static void DrawRectangleBorder(Rect rect, float thickness, Color color)
    {
        // Top
        Utilities.DrawRectangle(new Rect(rect.xMin, rect.yMin, rect.width, thickness), color);
        // Left
        Utilities.DrawRectangle(new Rect(rect.xMin, rect.yMin, thickness, rect.height), color);
        // Right
        Utilities.DrawRectangle(new Rect(rect.xMax - thickness, rect.yMin, thickness, rect.height), color);
        // Bottom
        Utilities.DrawRectangle(new Rect(rect.xMin, rect.yMax - thickness, rect.width, thickness), color);
    }

    public static Rect GetScreenRectangle(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        // Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;
        // Calculate corners
        var topLeft = Vector3.Min(screenPosition1, screenPosition2);
        var bottomRight = Vector3.Max(screenPosition1, screenPosition2);
        // Create Rect
        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
    {
        Vector3 v1 = Camera.main.ScreenToViewportPoint(screenPosition1);
        Vector3 v2 = Camera.main.ScreenToViewportPoint(screenPosition2);
        Vector3 min = Vector3.Min(v1, v2);
        Vector3 max = Vector3.Max(v1, v2);
        min.z = camera.nearClipPlane;
        max.z = camera.farClipPlane;

        Bounds bounds = new Bounds();
        bounds.SetMinMax(min, max);
        return bounds;
    }

    /*public static Vector3 CalculateCentreOfGroup(List<GameObject> _selectedUnits) //calculates average of positions of a group of units
    {
        if (_selectedUnits.Count < 1)
            return Vector3.zero;
        Vector3 sum = Vector3.zero;
        Vector3 average;

        foreach (GameObject g in _selectedUnits)
        {
            sum += g.gameObject.transform.position;
        }
        average = sum / _selectedUnits.Count;

        return new Vector3(average.x, 0, average.z);
    }

    public static void UpdateCentreOfGroup(List<GameObject> _selectedUnits)
    {
        Vector3 centre = CalculateCentreOfGroup(_selectedUnits);
        foreach (GameObject g in _selectedUnits)
        {
            g.GetComponent<SelectedUnit>().centreOfGroup = centre;
        }
    }*/
}
