using System.Collections;
using UnityEngine;

public class Unit : MonoBehaviour
{
    const float minPathUpdateTime = 0.2f;
    const float pathUpdateMoveThreshold = 0.5f;

    public Transform target;
    public float speed = 15;
    public float turnSpeed = 3f;
    public float turnDst = 5;
    public float stoppingDst = 10f;
    public int groupingPenalty;

    public bool selected = false;
    bool followingPath;

    Path path;
    bool displayPathGizmos;
    public const float gravity = -9.81f;
    float defaultSpeed;

    GridScript gridScript;

    private void Awake()
    {
        defaultSpeed = speed;
        gridScript = GameObject.Find("A*").GetComponent<GridScript>();
    }

    private void Update()
    {
        if (Input.GetKeyDown("x") && displayPathGizmos == true)
            displayPathGizmos = false;
        else if (Input.GetKeyDown("x") && displayPathGizmos == false)
            displayPathGizmos = true;

        GetComponent<Rigidbody>().velocity += new Vector3(0, gravity * Time.deltaTime, 0);
    }

    public void OnPathFound(Vector3[] waypoints, bool pathSuccessful)
    {
        if (pathSuccessful)
        {
            path = new Path(waypoints, transform.position,turnDst, stoppingDst);
            StopCoroutine("FollowPath");
            StartCoroutine("FollowPath");
        }
    }
    IEnumerator UpdatePath(Vector3 target)
    {
        //Waits for the editor to load properly
        if (Time.timeSinceLevelLoad < 0.3f)
        {
            yield return new WaitForSeconds(0.3f);
        }

        PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));   //NEEDS REWRITING AND COMMENTS

        float sqrMoveThreshold = pathUpdateMoveThreshold * pathUpdateMoveThreshold;
        Vector3 targetPosOld = target;

        while (true)
        {
            yield return new WaitForSeconds(minPathUpdateTime);
            if ((target - targetPosOld).sqrMagnitude > sqrMoveThreshold)
            {
                PathRequestManager.RequestPath(new PathRequest(transform.position, target, OnPathFound));
                targetPosOld = target;
            }
        }
    }

    IEnumerator FollowPath()
    {
        followingPath = true;
        int pathIndex = 0;
        transform.LookAt(new Vector3(path.lookPoints[0].x,transform.position.y, path.lookPoints[0].z));

        float speedPercent = 1;

        while (followingPath)
        {
            Vector2 pos2D = new Vector2(transform.position.x,transform.position.z);
            while (path.turnBoundaries[pathIndex].HasCrossedLine(pos2D))
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
                speed = defaultSpeed - (gridScript.GetNodeInWorld(transform.position).movementPenalty)/2; //reduces speed from the default speed depending on the current nodes movement penalty

                if (pathIndex >= path.decelerateIndex && stoppingDst > 0)
                {
                    speedPercent = Mathf.Clamp01(path.turnBoundaries[path.targetIndex].DistanceFromPoint(pos2D) / stoppingDst);
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
