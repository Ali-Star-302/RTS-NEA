using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float gravity = -9.81f;

    public Transform target;
    public Transform groundCheck;
    protected float speed = 15f;
    protected float stoppingDistance = 5f;
    protected float turnSpeed = 3f;
    protected float turnRadius = 5f;
    protected bool selected = false;

    bool followingPath;
    bool displayPathGizmos;
    float defaultSpeed;
    int groundMask;
    Path path;
    GridManager gridScript;

    Vector3 pathTarget;

    void Awake()
    {
        defaultSpeed = speed;
        gridScript = GameObject.Find("A*").GetComponent<GridManager>();
        groundMask = ~LayerMask.GetMask("Selectable");
    }

    void Update()
    {
        if (Input.GetKeyDown("x") && displayPathGizmos == true)
            displayPathGizmos = false;
        else if (Input.GetKeyDown("x") && displayPathGizmos == false)
            displayPathGizmos = true;

        //If the unit is off the ground it applies gravity
        if (!Physics.CheckSphere(groundCheck.position,0.5f, groundMask))
            GetComponent<Rigidbody>().velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }

    ///<summary> Updates the path starting from its new position to the target </summary>
    public IEnumerator UpdatePath(Vector3 target)
    {
        PathfindingManager.GetPath(transform.position, target, this);

        Vector3 previousTarget = target;

        while (true)
        {
            yield return new WaitForSeconds(100f); //Ensures it doesn't update path every frame

            //If the difference between the new and old target is big enough, the path is updated
            if ((target - previousTarget).sqrMagnitude > 0.25f)
            {
                pathTarget = target;
                PathfindingManager.GetPath(transform.position, target, this);
                previousTarget = target;
            }
        }
    }

    ///<summary> Creates a path from the given waypoints and runs FollowPath </summary>
    public void PathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position,turnRadius, stoppingDistance);

            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    ///<summary> Follows the path that has been created </summary>
    IEnumerator FollowPath()
    {
        int pathIndex = 0;
        followingPath = true;
        transform.LookAt(new Vector3(path.waypoints[0].x,transform.position.y, path.waypoints[0].z));

        float speedPercent = 1;

        while (followingPath)
        {
            while (path.turnBoundaries[pathIndex].HasCrossedLine(new Vector2(transform.position.x, transform.position.z)))
            {
                //Stops following if the end of the path is reached
                if (pathIndex == path.turnBoundaries.Length - 1)
                {
                    followingPath = false;
                    break;
                }
                else
                    pathIndex++;
            }

            if (followingPath)
            {
                speed = defaultSpeed - (gridScript.GetNodeFromPosition(transform.position).movementPenalty)/2; //Reduces speed from the default speed depending on the current node's movement penalty

                //When the unit passes the deceleration point it slows down
                if (pathIndex >= path.decelerationPoint)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.turnBoundaries.Length - 1].DistanceFromPoint(new Vector2(transform.position.x, transform.position.z)) / stoppingDistance);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(path.waypoints[pathIndex].x, transform.position.y, path.waypoints[pathIndex].z) - transform.position);
                transform.rotation = Quaternion.Lerp(transform.rotation, targetRotation, Time.deltaTime * turnSpeed);
                transform.Translate(Vector3.forward * Time.deltaTime * speed * speedPercent, Space.Self);
            }
            yield return null;
        }
    }

    public void OnDrawGizmos()
    {
        if (!displayPathGizmos)
            return;

        if (path != null)
        {
            path.DrawWithGizmos();
        }
    }
}
