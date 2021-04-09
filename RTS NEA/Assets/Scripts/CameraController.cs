using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class CameraController : MonoBehaviour
{
    public static CameraController cameraInstance;

    public Transform followTransform;
    public Transform cameraTransform;

    public float normalSpeed;
    public float fastSpeed;
    public float movementSpeed;
    public float movementTime;
    public float rotationAmount;
    public Vector3 zoomAmount;
    public float maxZoom;
    public float minZoom;

    public Vector3 newPosition;
    public Quaternion newRotation;
    public Vector3 newZoom;

    public Vector3 rotateStartPosition, rotateCurrentPosition;

    float verticalOffset;
    float mapSize;
    TerrainManager terrainManager;

    void Start()
    {
        terrainManager = GameObject.Find("Terrain Manager").GetComponent<TerrainManager>();
        mapSize = (terrainManager.mapSize * GenerationValues.GetChunkSize())/2;
        verticalOffset = transform.position.y;
        cameraInstance = this;
        newPosition = transform.position;
        newRotation = transform.rotation;
        newZoom = cameraTransform.localPosition;
    }

    void LateUpdate() //Uses late update to ensure the camera is moved after all other code has been executed, reducing potential jitteriness
    {
        MouseInput();
        KeyboardInput();

        //Ensures the camera isn't too far or too close
        newZoom.y = Mathf.Clamp(newZoom.y, minZoom, maxZoom);

        //Clamps the camera position to just outside the map's bounds
        Vector3 position = transform.position;
        position.x = Mathf.Clamp(transform.position.x, -(mapSize + 10), mapSize + 10);
        position.z = Mathf.Clamp(transform.position.z, -(mapSize + 10), mapSize + 10);
        transform.position = position;

        //If slightly outside the map's bounds it will snap the camera back inside if the movement key is released
        if (transform.position.x > mapSize + 5 && !Input.GetButton("Horizontal"))
        {
            newPosition = new Vector3(mapSize - 5, transform.position.y, transform.position.z);
        }
        else if (transform.position.x < -(mapSize + 5) && !Input.GetButton("Horizontal"))
        {
            newPosition = new Vector3(-(mapSize - 5), transform.position.y, transform.position.z);
        }
        else if (transform.position.z > mapSize + 5 && !Input.GetButton("Vertical"))
        {
            newPosition = new Vector3(transform.position.x, transform.position.y, mapSize - 5);
        }
        else if (transform.position.z < -(mapSize + 5) && !Input.GetButton("Vertical"))
        {
            newPosition = new Vector3(transform.position.x, transform.position.y, -(mapSize - 5));
        }
    }

    void MouseInput()
    {
        //Zoom with scroll wheel
        if(Input.mouseScrollDelta.y != 0)
        {
            newZoom += Input.mouseScrollDelta.y * zoomAmount * 2;
        }
    }


    void KeyboardInput()
    {
        //Reduces movement speed when outside the map's bounds
        if (transform.position.x > mapSize || transform.position.x < -mapSize || transform.position.z > mapSize || transform.position.z < -mapSize)
        {
            movementSpeed = normalSpeed/4f;
        }
        else if (Input.GetKey(KeyCode.LeftShift)) //Fast movement mode when pressing left shift
        {
            movementSpeed = fastSpeed;
        }
        else
        {
            movementSpeed = normalSpeed;
        }

        newPosition += Input.GetAxis("Vertical") * transform.forward * movementSpeed;
        newPosition += Input.GetAxis("Horizontal") * transform.right * movementSpeed;

        //Zoom
        if (Input.GetKey(KeyCode.R))
        {
            newZoom += zoomAmount;
        }
        if (Input.GetKey(KeyCode.F))
        {
            newZoom -= zoomAmount;
        }

        //Update position, zoom and rotation
        transform.position = Vector3.Lerp(transform.position, newPosition, Time.deltaTime * movementTime);
        transform.rotation = Quaternion.Lerp(transform.rotation, newRotation, Time.deltaTime * movementTime);
        cameraTransform.localPosition = Vector3.Lerp(cameraTransform.localPosition, newZoom, Time.deltaTime * movementTime);
    }
}
