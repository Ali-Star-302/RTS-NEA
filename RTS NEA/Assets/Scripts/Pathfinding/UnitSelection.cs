using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class UnitSelection : MonoBehaviour
{
    public List<GameObject> selectedUnits;
    public List<GameObject> selectedEnemyUnits;

    bool isBoxSelecting = false;
    Vector3 position1;
    Vector3 position2;
    RaycastHit hit;

    //Mesh variables
    public MeshCollider selectionBox = new MeshCollider();
    Mesh selectionMesh;
    Vector2[] boxCorners;
    Vector3[] vertices;

    //Layer mask variables
    int grassMask = 1 << 10;
    int roadMask = 1 << 9;
    int groundMask;

    private void Awake()
    {
        //Sets the ground mask to be the grass or road masks
        groundMask = grassMask | roadMask;
    }

    void Update()
    {
        UnitSelect();
    }

    ///<summary> Checks for all the methods in which a unit can be selected </summary>
    void UnitSelect()
    {
        if (Input.GetMouseButtonDown(0))
        {
            position1 = Input.mousePosition;
        }
        if (Input.GetMouseButton(0))
        {
            //Checks if the player is dragging and not just clicking
            if ((position1 - Input.mousePosition).magnitude > 40)
                isBoxSelecting = true;
        }

        if (Input.GetMouseButtonUp(0))
        {
            //Selecting a single unit
            if (isBoxSelecting == false)
            {
                Ray ray = Camera.main.ScreenPointToRay(position1);
                //Selectable layer mask
                int selectableMask = 1 << 11;

                if (Physics.Raycast(ray, out hit, 5000f, selectableMask))
                    IndividualSelect();
                else
                {
                    //Clears selected units when no unit is clicked and shift isn't pressed
                    if (!Input.GetKey(KeyCode.LeftShift))
                        DeselectAll();
                }
                UpdateCentreOfGroup();
            }
            //Box selecting units
            else
            {
                BoxSelect();

                //Deselect all units when left shift isnt pressed
                if (!Input.GetKey(KeyCode.LeftShift))
                    DeselectAll();

                Destroy(selectionBox, 0.02f);
            }
            isBoxSelecting = false;
        }

        if (selectedUnits.Count > 0)
            UpdateCentreOfGroup();
    }

    /// <summary> Selects a single unit or adds single units to the selected units list </summary>
    void IndividualSelect()
    {
        //Selecting multiple units individually with left shift
        if (Input.GetKey(KeyCode.LeftShift))
        {
            if (selectedUnits.Contains(hit.transform.gameObject) || selectedEnemyUnits.Contains(hit.transform.gameObject))
                Deselect(hit.transform.gameObject);
            else
                Select(hit.transform.gameObject);
        }
        else
        {
            //Clears selected units and selects a new one
            if (selectedUnits.Contains(hit.transform.gameObject))
                DeselectAll();
            else
            {
                DeselectAll();
                Select(hit.transform.gameObject);
            }
        }
    }

    /// <summary> Selects all units within a box mesh created from the on-screen rectangle </summary>
    void BoxSelect()
    {
        vertices = new Vector3[4];
        int i = 0;
        position2 = Input.mousePosition;
        boxCorners = GetBoundingBox(position1, position2);

        foreach (Vector2 corner in boxCorners)
        {
            Ray ray = Camera.main.ScreenPointToRay(corner);

            if (Physics.Raycast(ray, out hit, 50000.0f, groundMask))
            {
                vertices[i] = new Vector3(hit.point.x, 0, hit.point.z);
                Debug.DrawLine(Camera.main.ScreenToWorldPoint(corner), hit.point, Color.red, 1.0f);
            }
            i++;
        }

        //Generate the mesh which encapsulates the units
        selectionMesh = GenerateBoxMesh(vertices);
        selectionBox = gameObject.AddComponent<MeshCollider>();
        selectionBox.sharedMesh = selectionMesh;
        selectionBox.convex = true;
        selectionBox.isTrigger = true;
    }

    /// <summary> Deselects all units </summary>
    void DeselectAll()
    {
        foreach (GameObject unit in selectedUnits)
        {
            unit.GetComponent<Unit>().selected = false;

            if (unit.GetComponent<SelectedUnit>())
                Destroy(unit.GetComponent<SelectedUnit>());
        }
        selectedUnits.Clear();

        foreach (GameObject unit in selectedEnemyUnits)
        {
            unit.GetComponent<Unit>().selected = false;
        }
        selectedEnemyUnits.Clear();
    }

    /// <summary> Deselects the given unit </summary>
    void Deselect(GameObject unit)
    {
        unit.GetComponent<Unit>().selected = false;

        if (unit.GetComponent<SelectedUnit>())
            Destroy(unit.GetComponent<SelectedUnit>());
        if (unit.GetComponent<Unit>().team == 1)
            selectedUnits.Remove(unit);
        else
            selectedEnemyUnits.Remove(unit);
    }

    /// <summary> Selects the given unit if it is not already selected </summary>
    void Select(GameObject unit)
    {
        unit.GetComponent<Unit>().selected = true;

        if (unit.GetComponent<Unit>().team != 1)
        {
            selectedEnemyUnits.Add(unit);
            return;
        }
        
        if (!unit.GetComponent<SelectedUnit>())
        {
            unit.AddComponent<SelectedUnit>();
        }
        if (!selectedUnits.Contains(unit))
            selectedUnits.Add(unit);
    }

    /// <summary> Calculates average vector3 of positions of a group of units </summary>
    Vector3 CalculateCentreOfGroup()
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
        int _numberOfUnits = selectedUnits.Count;

        for (int i = 0; i < _numberOfUnits; i++)
        {
            selectedUnits[i].GetComponent<SelectedUnit>().centreOfGroup = centre;

            selectedUnits[i].GetComponent<SelectedUnit>().numberOfUnits = _numberOfUnits;
            selectedUnits[i].GetComponent<SelectedUnit>().unitIndex = i;
        }
    }

    /// <summary> Creates a bounding box (4 boxCorners in order) from the start and end mouse position </summary>
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

        Vector2[] boxCorners = { newP1, newP2, newP3, newP4 };
        return boxCorners;

    }

    /// <summary> Generates a mesh from the bottom 4 boxCorners defined in the bounding box </summary>
    Mesh GenerateBoxMesh(Vector3[] boxCorners)
    {
        Vector3[] vertices = new Vector3[8];
        int[] triangles = { 0, 1, 2, 2, 1, 3, 4, 6, 0, 0, 6, 2, 6, 7, 2, 2, 7, 3, 7, 5, 3, 3, 5, 1, 5, 0, 1, 1, 4, 0, 4, 5, 6, 6, 5, 7 }; //Order of triangle formation from points

        for (int i = 0; i < 4; i++)
        {
            vertices[i] = boxCorners[i];
        }

        //Makes the corners of the top face 100 units above the corners of the bottom face
        for (int j = 4; j < 8; j++)
        {
            vertices[j] = boxCorners[j - 4] + Vector3.up * 100;
        }

        Mesh selectionMesh = new Mesh();
        selectionMesh.vertices = vertices;
        selectionMesh.triangles = triangles;

        return selectionMesh;
    }

    void OnGUI()
    {
        //Draws box when dragging
        if (isBoxSelecting)
        {
            var rect = Utilities.GetScreenRectangle(position1, Input.mousePosition);
            Utilities.DrawRectangle(rect, new Color(0.65f, 0.65f, 0.95f, 0.15f));
            Utilities.DrawRectangleBorder(rect, 1, new Color(0.65f, 0.65f, 0.75f));
        }
    }

    private void OnTriggerEnter(Collider col)
    {
        if (col.gameObject.tag == "Unit")
        {
            if (col.gameObject.GetComponent<Unit>().team != 1)
                return;

            Select(col.gameObject);
            UpdateCentreOfGroup();
        }
        
    }
}
