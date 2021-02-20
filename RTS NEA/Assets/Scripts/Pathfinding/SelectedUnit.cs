using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    Vector3 targetPosition;
    public Vector3 centreOfGroup;
    Vector3 gizmoPos;
    Unit unitScript;

    void Start()
    {
        unitScript = GetComponent<Unit>();
    }

    void Update()
    {
        MouseInput();
    }


    ///<summary> Checks for the player clicking somewhere and calls the subroutine to begin the process of moving to that point </summary>
    void MouseInput()
    {
        if (Input.GetMouseButtonDown(1) && Input.GetKey(KeyCode.LeftControl))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);
                
                //Offsets the archer from the unit by its specified range so it can do a ranged attack
                if (unitScript.GetType().ToString() == "Archer")
                {
                    unitScript.gameObject.GetComponent<Archer>().arrowTarget = targetPosition;
                    gizmoPos = targetPosition;
                    Vector3 unitVector = (targetPosition - transform.position).normalized;
                    float unitRange = unitScript.gameObject.GetComponent<Archer>().rangedRange;
                    targetPosition = targetPosition - (unitRange * unitVector);
                    unitScript.gameObject.GetComponent<Archer>().rangedAttacking = true;

                    targetPosition = targetPosition + CalculateOffset(true);
                    if (Vector3.Distance(unitScript.transform.position, targetPosition) > 10)
                        StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(true)));
                }
                else
                {
                    unitScript.attacking = true;
                    if (Input.GetKey(KeyCode.C))
                        StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(true))); //Calls the UpdatePath method from the Unit script, passing in the target position with an offset
                    else
                        StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(false)));
                }
            }
        }
        else if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                targetPosition = new Vector3(targetPosition.x, targetPosition.y, targetPosition.z);

                if (unitScript.GetType().ToString() == "Archer")
                    unitScript.gameObject.GetComponent<Archer>().rangedAttacking = false;
                
                unitScript.attacking = false;

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

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(gizmoPos, new Vector3(0.3f, 0.3f, 0.3f));
    }
}
