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

    Vector3 CalculateOffset(bool closeTogether) //Needs improvement
    {
        Vector3 baseOffset = transform.position - centreOfGroup;

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
