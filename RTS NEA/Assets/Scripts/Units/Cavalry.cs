using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavalry : Unit
{
    public override float speed { get => 15f; }
    public override float stoppingDistance { get => 7f; }
    public override float turnSpeed { get => 4f; }
    public override float turnRadius { get => 3f; }
    public override float meleeAttackSpeed { get => 1.2f; } //Higher is slower
    public override float meleeRange { get => 6f; }
    public override int meleeDamage { get => 6; }
    public override float meleeAccuracy { get => 0.6f; }
    public override float maxHealth { get => 90; }

    protected override void MeleeAttack()
    {
        meleeAttackCounter += Time.deltaTime;

        if (meleeAttackCounter >= meleeAttackSpeed)
        {
            foreach (Collider col in Physics.OverlapSphere(transform.position, meleeRange, selectableMask))
            {
                if (col.gameObject.tag == "Unit" && col.gameObject != this.gameObject)
                {
                    Unit enemyUnit = col.gameObject.GetComponent<Unit>();
                    if (enemyUnit.team != team)
                    {
                        Rigidbody rb = GetComponent<Rigidbody>();
                        if (enemyUnit.GetType().ToString() == unitCounter)
                        {
                            enemyUnit.Damage(Mathf.Clamp(meleeDamage * 2f * (rb.velocity.magnitude + 1),meleeDamage * 2f, 40f)); //Multiplies damage by 2 and the velocity + 1 (so that its never 0), also clamps it so damage isn't insanely high
                            //Debug.Log("Counter: " + Mathf.Clamp(meleeDamage * 2f * (rb.velocity.magnitude + 1), meleeDamage * 2f, 40f));
                        }
                        else
                        {
                            enemyUnit.Damage(Mathf.Clamp(meleeDamage * (rb.velocity.magnitude + 1),meleeDamage,20f));
                            //Debug.Log("Normal: " + Mathf.Clamp(meleeDamage * (rb.velocity.magnitude + 1), meleeDamage, 20f));
                        }

                    }
                }
            }
            meleeAttackCounter = 0;
        }
    }
}
