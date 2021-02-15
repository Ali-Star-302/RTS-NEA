using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Cavalry : Unit
{
    public override float speed { get => 15f; }
    public override float stoppingDistance { get => 10f; }
    public override float turnSpeed { get => 1f; }
    public override float turnRadius { get => 8f; }
    public override float meleeAttackSpeed { get => 1.5f; }
    public override float meleeRange { get => 6f; }
    public override int meleeDamage { get => 10; }
    public override float meleeAccuracy { get => 0.6f; }
    public override int maxHealth { get => 90; }
}
