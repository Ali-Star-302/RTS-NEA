using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public override float speed { get => 12f; }
    public override float stoppingDistance { get => 3f; }
    public override float turnSpeed { get => 4f; }
    public override float turnRadius { get => 3f; }
    public override float meleeAttackSpeed { get => 0.8f; } //Higher is slower
    public override float meleeRange { get => 4f; }
    public override int meleeDamage { get => 2; }
    public override float meleeAccuracy { get => 0.7f; }
    public override float maxHealth { get => 70; }

    public float rangedAttackSpeed;
    public float rangedRange;
    public float rangedAccuracy;
    public int rangedDamage;
    public bool rangedAttacking;
    public GameObject arrowPrefab;
    public Vector3 arrowTarget;

    protected float rangedAttackCounter;

    override public void Update()
    {
        if (Input.GetKeyDown("x") && displayPathGizmos == true)
            displayPathGizmos = false;
        else if (Input.GetKeyDown("x") && displayPathGizmos == false)
            displayPathGizmos = true;

        //If the unit is off the ground it applies gravity
        if (!Physics.CheckSphere(groundCheck.position, 0.5f, groundMask))
        {
            GetComponent<Rigidbody>().velocity += new Vector3(0, gravity * Time.deltaTime, 0);
        }

        if (health <= 0)
            Death();

        if (rangedAttacking && !followingPath)
            RangedAttack();
        else if (attacking)
            MeleeAttack();

        //Increments timer if the health is shown
        if (showHealth)
            showHealthCounter += Time.deltaTime;

        //Once three seconds have passed without damage the health bar is not shown
        if (showHealthCounter >= 3f)
        {
            showHealth = false;
            showHealthCounter = 0;
            attacking = false;
        }

        healthManager.gameObject.SetActive(selected || showHealth);
        if (team == 0)
            selectionCircle.gameObject.SetActive(selected);
        else
            selectionCircle.gameObject.SetActive(false);
    }

    void RangedAttack()
    {
        transform.LookAt(arrowTarget);
        attacking = false;

        rangedAttackCounter += Time.deltaTime;

        if (rangedAttackCounter >= rangedAttackSpeed)
        {
            GameObject arrow = Instantiate(arrowPrefab, transform.position + transform.forward + Vector3.up, Quaternion.identity);
            arrow.GetComponent<Arrow>().Constructor(team, arrowTarget, rangedDamage, rangedAccuracy);
            rangedAttackCounter = 0;
        }
    }
}
