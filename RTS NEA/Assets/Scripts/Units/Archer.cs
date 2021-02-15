using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Archer : Unit
{
    public override float speed { get => 12f; }
    public override float stoppingDistance { get => 3f; }
    public override float turnSpeed { get => 4f; }
    public override float turnRadius { get => 3f; }
    public override float meleeAttackSpeed { get => 0.8f; }
    public override float meleeRange { get => 4f; }
    public override int meleeDamage { get => 2; }
    public override float meleeAccuracy { get => 0.7f; }
    public override int maxHealth { get => 70; }

    public float rangedAttackSpeed = 3f;
    public float rangedRange = 50f;
    public float accuracy = 0.7f;
    public int rangedDamage = 15;
    
}
