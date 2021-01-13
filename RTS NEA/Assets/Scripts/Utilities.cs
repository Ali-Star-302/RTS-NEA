using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public static class Utilities
{
    static Texture2D texture;

    public static Texture2D BasicTexture
    {
        get
        {
            if (texture == null)
            {
                texture = new Texture2D(1, 1);
                texture.SetPixel(0, 0, Color.white);
                texture.Apply();
            }

            return texture;
        }
    }

    ///<summary> Draws a basic rectangle to the GUI on screen </summary>
    public static void DrawRectangle(Rect rectangle, Color colour)
    {
        GUI.color = colour;
        GUI.DrawTexture(rectangle, BasicTexture);
        GUI.color = Color.white;
    }

    ///<summary> Creates the top, left, right and bottom borders of the main rectangle from smaller rectangles </summary>
    public static void DrawRectangleBorder(Rect rectangle, float width, Color colour)
    {
        DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, rectangle.width, width), colour);
        DrawRectangle(new Rect(rectangle.xMin, rectangle.yMin, width, rectangle.height), colour);
        DrawRectangle(new Rect(rectangle.xMax - width, rectangle.yMin, width, rectangle.height), colour);
        DrawRectangle(new Rect(rectangle.xMin, rectangle.yMax - width, rectangle.width, width), colour);
    }

    
    ///<summary> Returns a rectangle created from on screen coordinates </summary>
    public static Rect GetScreenRectangle(Vector3 screenPosition1, Vector3 screenPosition2)
    {
        //Move origin from bottom left to top left
        screenPosition1.y = Screen.height - screenPosition1.y;
        screenPosition2.y = Screen.height - screenPosition2.y;

        //Calculate corners
        Vector3 topLeft = Vector3.Min(screenPosition1, screenPosition2);
        Vector3 bottomRight = Vector3.Max(screenPosition1, screenPosition2);

        return Rect.MinMaxRect(topLeft.x, topLeft.y, bottomRight.x, bottomRight.y);
    }

    /*public static Bounds GetViewportBounds(Camera camera, Vector3 screenPosition1, Vector3 screenPosition2)
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
    }*/
}
