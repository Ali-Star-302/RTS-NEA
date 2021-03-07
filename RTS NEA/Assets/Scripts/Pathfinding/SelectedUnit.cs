using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    public Vector3 centreOfGroup;
    public int numberOfUnits;
    public int unitIndex;

    Vector3 targetPosition;
    Vector3 gizmoPos;
    Unit unitScript;
    bool inFormation = false;


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
                    /*unitScript.gameObject.GetComponent<Archer>().arrowTarget = targetPosition;
                    gizmoPos = targetPosition;
                    Vector3 unitVector = (targetPosition - transform.position).normalized;
                    float unitRange = unitScript.gameObject.GetComponent<Archer>().rangedRange;
                    targetPosition = targetPosition - (unitRange * unitVector);
                    unitScript.gameObject.GetComponent<Archer>().rangedAttacking = true;

                    if (Vector3.Distance(unitScript.transform.position, targetPosition) > 10)
                        StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(true)));*/


                    /*if (Input.GetKey(KeyCode.C))
                    {
                        unitScript.gameObject.GetComponent<Archer>().arrowTarget = targetPosition;
                        gizmoPos = targetPosition;

                        Vector3 unitVector = (targetPosition - centreOfGroup).normalized;
                        float unitRange = unitScript.gameObject.GetComponent<Archer>().rangedRange;
                        targetPosition = centreOfGroup - (unitRange * unitVector);
                        unitScript.gameObject.GetComponent<Archer>().rangedAttacking = true;

                        if (Vector3.Distance(unitScript.transform.position, targetPosition) > 10)
                            StartCoroutine(unitScript.UpdatePath(GetFormationPosition(targetPosition, numberOfUnits, unitIndex)));
                    }
                    else
                    {
                        unitScript.gameObject.GetComponent<Archer>().arrowTarget = targetPosition;
                        gizmoPos = targetPosition;
                        Vector3 unitVector = (targetPosition - transform.position).normalized;
                        float unitRange = unitScript.gameObject.GetComponent<Archer>().rangedRange;
                        targetPosition = targetPosition - (unitRange * unitVector);
                        unitScript.gameObject.GetComponent<Archer>().rangedAttacking = true;

                        if (Vector3.Distance(unitScript.transform.position, targetPosition) > 10)
                            StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(true)));
                    }*/

                    unitScript.gameObject.GetComponent<Archer>().arrowTarget = targetPosition;
                    gizmoPos = targetPosition;
                    float unitRange = unitScript.gameObject.GetComponent<Archer>().rangedRange;
                    Vector3 unitVector;

                    if (Input.GetKey(KeyCode.C))
                    {
                        unitVector = (targetPosition - centreOfGroup).normalized;
                        targetPosition = targetPosition - (unitRange * unitVector);

                        if (Vector3.Distance(centreOfGroup, targetPosition) > 10 || !inFormation)
                        {
                            StartCoroutine(unitScript.UpdatePath(GetFormationPosition(targetPosition, numberOfUnits, unitIndex)));
                            inFormation = true;
                        }
                    }
                    else
                    {
                        unitVector = (targetPosition - transform.position).normalized;
                        targetPosition = targetPosition - (unitRange * unitVector);

                        if (Vector3.Distance(transform.position, targetPosition) > 10 || inFormation)
                        {
                            StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(false)));
                            inFormation = false;
                        }
                    }

                    gameObject.GetComponent<Archer>().rangedAttacking = true;
                }
                else
                {
                    unitScript.attacking = true;
                    if (Input.GetKey(KeyCode.C))
                        StartCoroutine(unitScript.UpdatePath(GetFormationPosition(targetPosition, numberOfUnits, unitIndex))); //Calls the UpdatePath method from the Unit script, passing in the target position with an offset
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
                    StartCoroutine(unitScript.UpdatePath(GetFormationPosition(targetPosition, numberOfUnits, unitIndex))); //Calls the UpdatePath method from the Unit script, passing in the target position with an offset
                else
                    StartCoroutine(unitScript.UpdatePath(targetPosition + CalculateOffset(false)));
            }
        }
    }

    ///<summary> Offsets unit away from the centre of the group </summary>
    Vector3 CalculateOffset(bool groupTogether)
    {
        Vector3 baseOffset = transform.position - centreOfGroup;

        //If groupTogether mode is on they are moved closer together
        if (groupTogether && baseOffset.sqrMagnitude > 45)
            return baseOffset.normalized * 3;
        else
            return baseOffset;
    }

    public static Vector3 GetFormationPosition(Vector3 target, int unitCount, int unitIndex)
    {
        Vector3 newTarget = target;
        float unitSpacing = 2;
        int unitLine = 0;
        if (unitCount <= 10)
        {
            newTarget = newTarget + (Vector3.left * (((unitCount % 10) / 2 - unitIndex) * unitSpacing));
        }
        else
        {
            unitLine = (int)(unitIndex / 10);
            if (unitIndex % 10 <= unitIndex % 10 / 2)
                newTarget = newTarget + (Vector3.left * unitSpacing * (unitIndex % 10)) + (Vector3.back * unitLine * unitSpacing);
            else
                newTarget = newTarget + (Vector3.right * unitSpacing * (unitIndex % 10)) + (Vector3.back * unitLine * unitSpacing);
        }
        //Debug.Log(unitIndex + ": " + newTarget + ", " + unitLine);

        return newTarget;
    }

    void OnDrawGizmos()
    {
        Gizmos.color = new Color(0, 0, 1, 0.5f);
        Gizmos.DrawCube(gizmoPos, new Vector3(0.3f, 0.3f, 0.3f));
    }
}
