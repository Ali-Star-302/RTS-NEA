using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class Unit : MonoBehaviour
{
    const float gravity = -9.81f;

    public Transform target;
    public Transform groundCheck;
    public virtual float speed { get; set; }
    public virtual float stoppingDistance { get; set; }
    public virtual float turnSpeed { get; set; }
    public virtual float turnRadius { get; set; }
    public virtual float meleeAttackSpeed { get; set; }
    public virtual float meleeRange { get; set; }
    public virtual int meleeDamage { get; set; }
    public virtual float meleeAccuracy { get; set; }
    public virtual int maxHealth { get; set; }

    public int team;
    public int health;
    public HealthManager healthManager;
    public bool selected;

    bool followingPath;
    bool displayPathGizmos;
    float defaultSpeed;
    int groundMask;
    int selectableMask;
    Path path;
    GridManager gridScript;
    bool unshowHealthIsRunning;
    bool showHealth;
    List<GameObject> enemiesInRange = new List<GameObject>();

    Vector3 pathTarget;

    void Awake()
    {
        health = maxHealth;
        defaultSpeed = speed;
        gridScript = GameObject.Find("A*").GetComponent<GridManager>();
        groundMask = ~LayerMask.GetMask("Selectable");
        selectableMask = LayerMask.GetMask("Selectable");

        healthManager.SetMaxHealth(maxHealth);
    }

    void Start()
    {
        StartCoroutine(MeleeAttack());
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

        if (health <= 0)
            Death();
        
        healthManager.gameObject.SetActive(selected || showHealth);
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

    public virtual IEnumerator MeleeAttack()
    {
        while (true)
        {
            foreach (Collider col in Physics.OverlapSphere(transform.position, meleeRange, selectableMask))
            {
                if (col.gameObject.tag == "Unit")
                {
                    if (col.gameObject.GetComponent<Unit>().team != team)
                    {
                        col.gameObject.GetComponent<Unit>().Damage(meleeDamage);
                    }
                }
            }

            yield return new WaitForSeconds(meleeAttackSpeed);
        }
    }

    public void Damage(int damage)
    {
        health -= damage;
        healthManager.UpdateHealthSlider(health);
        healthManager.gameObject.SetActive(true);

        if (!unshowHealthIsRunning)
            StartCoroutine(UnshowHealth());
    }

    IEnumerator UnshowHealth()
    {
        unshowHealthIsRunning = true;

        yield return new WaitForSeconds(3);
        healthManager.gameObject.SetActive(true);
        unshowHealthIsRunning = false;
    }

    void Death()
    {
        StopAllCoroutines();
        Debug.Log(gameObject.name + " died");
        GameObject.Find("Player").GetComponent<UnitSelection>().BroadcastMessage("Deselect", this.gameObject);
        Destroy(this.gameObject);
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

    private void OnDrawGizmosSelected()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, meleeRange);
    }
}
