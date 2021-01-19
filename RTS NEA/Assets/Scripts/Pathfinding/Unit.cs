using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.5f;
    const float pathDifference = 0.5f;
    float pathDifferenceSqr;

    public Transform target;
    public float speed = 15;
    public float turnSpeed = 3f;
    public float turnDst = 5;
    public float stoppingDst = 10f;
    public int groupingPenalty;
    public Transform groundCheck;

    public bool selected = false;
    bool followingPath;

    Path path;
    bool displayPathGizmos;
    const float gravity = -9.81f;
    float defaultSpeed;
    int groundMask;

    GridScript gridScript;

    void Awake()
    {
        defaultSpeed = speed;
        gridScript = GameObject.Find("A*").GetComponent<GridScript>();
        pathDifferenceSqr = pathDifference * pathDifference;
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

        Vector3 targetPositionOld = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime); //Ensures it doesn't update path every frame

            //If the difference between the new and old target is big enough, the path is updated
            if ((target - targetPositionOld).sqrMagnitude > pathDifferenceSqr)
            {
                PathfindingManager.GetPath(transform.position, target, this);
                targetPositionOld = target;
            }
        }
    }

    ///<summary> Creates a path from the given waypoints and runs FollowPath </summary>
    public void PathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position,turnDst, stoppingDst);

            StopAllCoroutines();
            StartCoroutine(FollowPath());
        }
    }

    ///<summary> Follows the path that has been created </summary>
    IEnumerator FollowPath()
    {
        followingPath = true;
        int pathIndex = 0;
        transform.LookAt(new Vector3(path.lookPoints[0].x,transform.position.y, path.lookPoints[0].z));

        float speedPercent = 1;

        while (followingPath)
        {
            while (path.turnBoundaries[pathIndex].HasCrossedLine(new Vector2(transform.position.x, transform.position.z)))
            {
                if (pathIndex == path.targetIndex)
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

                if (pathIndex >= path.decelerateIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.targetIndex].DistanceFromPoint(new Vector2(transform.position.x, transform.position.z)) / stoppingDst);
                    if (speedPercent < 0.01f)
                    {
                        followingPath = false;
                    }
                }

                Quaternion targetRotation = Quaternion.LookRotation(new Vector3(path.lookPoints[pathIndex].x, transform.position.y, path.lookPoints[pathIndex].z) - transform.position);
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
