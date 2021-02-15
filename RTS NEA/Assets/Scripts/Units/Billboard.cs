using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Billboard : MonoBehaviour
{
    Transform mainCamera;

    private void Awake()
    {
        mainCamera = GameObject.Find("Main Camera").transform;
    }

    void LateUpdate()
    {
        transform.LookAt(transform.position + mainCamera.forward);
    }
}
