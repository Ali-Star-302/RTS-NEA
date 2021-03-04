using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Arrow : MonoBehaviour
{
    public int team;
    public Vector3 targetPosition;
    public LayerMask groundMask;
    public LayerMask selectableMask;
    [HideInInspector]
    public int damage;
    public float accuracy;

    Rigidbody rb;
    bool hasDamaged;

    void Start()
    {
        rb = GetComponent<Rigidbody>();
        targetPosition = new Vector3(Random.Range(targetPosition.x - accuracy, targetPosition.x + accuracy), Random.Range(targetPosition.y - accuracy, targetPosition.y + accuracy), Random.Range(targetPosition.z - accuracy, targetPosition.z + accuracy));
        rb.velocity = ProjectileVelocity(targetPosition);
    }
    
    void Update()
    {
        if (GetComponent<Rigidbody>())
            transform.rotation = Quaternion.LookRotation(rb.velocity);

        foreach (Collider col in Physics.OverlapSphere(transform.position, 0.5f, selectableMask))
        {
            if (col.gameObject.tag == "Unit" && !hasDamaged)
            {
                Unit enemyUnit = col.gameObject.GetComponent<Unit>();
                if (enemyUnit.team != team)
                {
                    enemyUnit.Damage(damage);
                    hasDamaged = true;
                }
            }
        }
    }

    public void Constructor(int _team, Vector3 _targetPos, int _damage, float _accuracy)
    {
        team = _team;
        targetPosition = _targetPos;
        damage = _damage;
        accuracy = _accuracy;
    }

    Vector3 ProjectileVelocity(Vector3 target)
    {
        Vector3 direction = target - transform.position;
        float height = direction.y;
        direction.y = 0; //Ensure only horizontal vectors are used
        float distance = direction.magnitude;
        direction.y = distance;
        distance += height;
        float velocity = Mathf.Sqrt(distance * Physics.gravity.magnitude);
        return velocity * direction.normalized;
    }

    void OnCollisionEnter(Collision col)
    {
        //Ensures it doesn't collider with other projectiles
        if (col.gameObject.layer == gameObject.layer)
            return;

        //Ignores the collision if it is a friendly unit
        if (col.gameObject.GetComponent<Unit>())
        {
            if (col.gameObject.GetComponent<Unit>().team == team)
            {
                Physics.IgnoreCollision(col.gameObject.GetComponent<Collider>(), gameObject.GetComponent<Collider>(), true);
                return;
            }
        }
        

        //Destroys the rigidbody and collider once collided with something
        Destroy(rb);
        Destroy(GetComponent<Collider>());

        //If the collider is on selectable layer set the parent to be the collider object
        if (col.gameObject.layer == 11)
        {
            transform.SetParent(col.transform);
            Destroy(gameObject, 1.5f);  //Destroys the arrow after 1.5 seconds of being stuck to something selectable
        }
        else
            Destroy(gameObject, 1f);    //Destroys the arrow after 1 second of colliding with something
    }

    private void OnDrawGizmos()
    {
        Gizmos.color = Color.red;
        Gizmos.DrawWireSphere(transform.position, 0.5f);
    }
}
