using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SelectedUnit : MonoBehaviour
{
    Color defaultColour;
    Vector3 targetPosition;
    public Vector3 centreOfGroup;

    void Start()
    {
        defaultColour = GetComponent<Renderer>().material.color;
        GetComponent<Renderer>().material.color = Color.green;
    }

    private void Update()
    {
        if (Input.GetMouseButtonDown(1))
        {
            RaycastHit hit;
            Ray ray = Camera.main.ScreenPointToRay(Input.mousePosition);
            if (Physics.Raycast(ray, out hit))
            {
                targetPosition = hit.point;
                targetPosition = new Vector3(targetPosition.x, targetPosition.y + 3, targetPosition.z);
                if ((centreOfGroup - transform.position).sqrMagnitude > 3)
                    this.gameObject.SendMessage("UpdatePath", targetPosition + CalculateOffset());
                else
                    this.gameObject.SendMessage("UpdatePath", targetPosition);
            }
        }
    }

    Vector3 CalculateOffset() //needs improvement
    {
        Vector3 baseOffset = transform.position - centreOfGroup;
        if (baseOffset.sqrMagnitude > 45)
            return baseOffset.normalized * 3;
        else
            return baseOffset;
    }

    private void OnDestroy()
    {
        GetComponent<Renderer>().material.color = defaultColour;
    }
}
