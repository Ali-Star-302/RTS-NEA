using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    Color defaultColour;
    Vector3 targetPosition;
    public Vector3 centreOfGroup;

    Unit unitScript;

    void Start()
    {
        unitScript = GetComponent<Unit>();

        defaultColour = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.green;
    }

    void Update()
    {
        MouseInput();
    }


    ///<summary> Checks for the player clicking somewhere and calls the subroutine to begin the process of moving to that point </summary>
    void MouseInput()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                targetPosition = new Vector3(targetPosition.x, targetPosition.y + 3, targetPosition.z);

                if (Input.GetKey(KeyCode.C))
                    StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(true))); //Calls the UpdatePath method from the Unit script, passing in the target position with an offset
                else
                    StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(false)));
            }
        }
    }

    ///<summary> Offsets unit away from the centre of the group </summary>
    Vector3 CalculateOffset(bool closeTogether)
    {
        Vector3 baseOffset = transform.position - centreOfGroup;

        //If close mode is on they are moved closer together
        if (closeTogether && baseOffset.sqrMagnitude > 45)
            return baseOffset.normalized * 3;
        else
            return baseOffset;
    }

    void OnDestroy()
    {
        GetComponent<Renderer>().material.color = defaultColour;
    }
}
