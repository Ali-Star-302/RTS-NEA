using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public List<GameObject> selectedUnits;

    bool isSelecting = false;
    Vector3 pos1;
    Vector3 pos2;
    RaycastHit hit;

    //Mesh variables
    public MeshCollider selectionBox = new MeshCollider();
    Mesh selectionMesh;
    Vector2[] corners;
    Vector3[] verts;

    int grassMask = 1 << 10;
    int roadMask = 1 << 9;
    int groundMask;

    private void Awake()
    {
        groundMask = grassMask | roadMask;
    }

    void Update()
    {
        if (Input.GetMouseButtonDown(0))
        {
            pos1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            //Checks if the player is dragging and not just clicking
            if ((pos1 - Input.mousePosition).magnitude > 40)
            {
                isSelecting = true;
            }
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Selecting a single unit
            if (isSelecting == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(pos1);
                //Selectable layer mask
                int selectableMask = 1 << 11;

                if (Physics.Raycast(ray, out hit, 5000f, selectableMask))
                {
                    //Selecting multiple units individually
                    if (Input.GetKey(KeyCode.LeftShift))
                    {
                        if (selectedUnits.Contains(hit.transform.gameObject))
                            Deselect(hit.transform.gameObject);
                        else
                            Select(hit.transform.gameObject);
                    }
                    else
                    {
                        //Clears selected units and selects a new one
                        if (selectedUnits.Contains(hit.transform.gameObject))
                        {
                            DeselectAll();
                        }
                        else
                        {
                            DeselectAll();
                            Select(hit.transform.gameObject);
                        }
                    }
                }
                else
                {
                    //Clears selected units when no unit is clicked and shift isn't pressed
                    if (!Input.GetKey(KeyCode.LeftShift))
                    {
                        DeselectAll();
                    }
                }
                UpdateCentreOfGroup();
            }
            //Box selecting units
            else
            {
                verts = new Vector3[4];
                int i = 0;
                pos2 = Input.mousePosition;
                corners = GetBoundingBox(pos1, pos2);

                foreach (Vector2 corner in corners)
                {
                    Ray ray = Camera.main.ScreenPointToRay(corner);

                    if (Physics.Raycast(ray, out hit, 50000.0f, groundMask))
                    {
                        verts[i] = new Vector3(hit.point.x, 0, hit.point.z);
                        Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 1.0f);
                    }
                    i++;
                }

                //generate the mesh
                selectionMesh = GenerateBoxMesh(verts);
                selectionBox = this.gameObject.AddComponent<MeshCollider>();
                selectionBox.sharedMesh = selectionMesh;
                selectionBox.convex = true;
                selectionBox.isTrigger = true;

                //Deselect all units when left shift isnt pressed
                if (!Input.GetKey(KeyCode.LeftShift))
                {
                    DeselectAll();
                }
                
                Destroy(selectionBox, 0.02f);
            }
            isSelecting = false;
        }

        if (selectedUnits.Count > 0)
        {
            UpdateCentreOfGroup();
        }
    }

    void DeselectAll()
    {
        foreach (GameObject g in selectedUnits)
        {
            if (g.GetComponent<SelectedUnit>())
                Destroy(g.GetComponent<SelectedUnit>());
        }
        selectedUnits.Clear();
    }

    void Deselect(GameObject g)
    {
        if (g.GetComponent<SelectedUnit>())
            Destroy(g.GetComponent<SelectedUnit>());
        selectedUnits.Remove(g);
    }

    void Select(GameObject g)
    {
        SelectedUnit s;
        if (!g.GetComponent<SelectedUnit>())
        {
            s = g.AddComponent<SelectedUnit>();
        }
        if (!selectedUnits.Contains(g))
            selectedUnits.Add(g);
    }

    Vector3 CalculateCentreOfGroup() //calculates average of positions of a group of units
    {
        if (selectedUnits.Count < 1)
            return Vector3.zero;
        Vector3 sum = Vector3.zero;
        Vector3 average;

        foreach (GameObject g in selectedUnits)
        {
            sum += g.gameObject.transform.position;
        }
        average = sum / selectedUnits.Count;

        return new Vector3(average.x, 0, average.z);
    }

    void UpdateCentreOfGroup()
    {
        Vector3 centre = CalculateCentreOfGroup();
        foreach (GameObject g in selectedUnits)
        {
            g.GetComponent<SelectedUnit>().centreOfGroup = centre;
        }
    }

    //Creates a bounding box (4 corners in order) from the start and end mouse position
    Vector2[] GetBoundingBox(Vector2 p1, Vector2 p2)
    {
        Vector2 newP1;
        Vector2 newP2;
        Vector2 newP3;
        Vector2 newP4;

        //Swaps positions of points
        if (p1.x < p2.x) //If p1 is to the left of p2
        {
            if (p1.y > p2.y) //If p1 is above p2
            {
                newP1 = p1;
                newP2 = new Vector2(p2.x, p1.y);
                newP3 = new Vector2(p1.x, p2.y);
                newP4 = p2;
            }
            else //If p1 is below p2
            {
                newP1 = new Vector2(p1.x, p2.y);
                newP2 = p2;
                newP3 = p1;
                newP4 = new Vector2(p2.x, p1.y);
            }
        }
        else //If p1 is to the right of p2
        {
            if (p1.y > p2.y)//If p1 is above p2
            {
                newP1 = new Vector2(p2.x, p1.y);
                newP2 = p1;
                newP3 = p2;
                newP4 = new Vector2(p1.x, p2.y);
            }
            else//If p1 is below p2
            {
                newP1 = p2;
                newP2 = new Vector2(p1.x, p2.y);
                newP3 = new Vector2(p2.x, p1.y);
                newP4 = p1;
            }

        }

        Vector2[] corners = { newP1, newP2, newP3, newP4 };
        return corners;

    }

    //Generates a mesh from the bottom 4 corners defined in the bounding box
    Mesh GenerateBoxMesh(Vector3[] corners)
    {
        Vector3[] verts = new Vector3[8];
        int[] tris = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //Order of triangle formation from points

        for (int i = 0; i < 4; i++)
        {
            verts[i] = corners[i];
        }

        for (int j = 4; j < 8; j++)
        {
            verts[j] = corners[j - 4] + Vector3.up * 100.0f;
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = verts;
        selectionMesh.triangles = tris;

        return selectionMesh;
    }

    void OnGUI()
    {
        //Draws box when dragging
        if (isSelecting)
        {
            var rect = Utilities.GetScreenRectangle(pos1, Input.mousePosition);
            Utilities.DrawRectangle(rect, new Color(0.8f, 0.8f, 0.95f, 0.25f));
            Utilities.DrawRectangleBorder(rect, 2, new Color(0.8f, 0.8f, 0.95f));
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Unit")
        {
            Select(col.gameObject);
        }
        UpdateCentreOfGroup();
    }
}
