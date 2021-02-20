using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Pikeman : Unit
{
    public override float speed { get => 9f; }
    public override float stoppingDistance { get => 5f; }
    public override float turnSpeed { get => 3f; }
    public override float turnRadius { get => 5f; }
    public override float meleeAttackSpeed { get => 0.8f; } //Higher is slower
    public override float meleeRange { get => 7f; }
    public override int meleeDamage { get => 5; }
    public override float meleeAccuracy { get => 0.7f; }
    public override float maxHealth { get => 100; }
}
