using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticTower : TargetingTower
{
    // Start is called before the first frame update
    [Tooltip("Can the tower attack flying enemies?")]
    public bool canAttackFlying = true;
    public bool canAttackGround = true;

    [Tooltip("Quick reference to the root Transform of the tower.")]
    public Transform trans;

    [Tooltip(
        "Reference to the Transform that the projectile should be positioned and rotated with initially."
    )]
    public Transform projectileSpawnPoint;

    [Tooltip("Reference to the Transform that should point towards the enemy.")]
    public Transform aimer;

    [Tooltip("Seconds between each projectile being fired.")]
    public float fireInterval = 0.0f;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab;

    [Tooltip("Damage dealt by each projectile.")]
    public float damage = 4;

    // public float airDamage = 4;
    //public float groundDamage = 4;

    [Tooltip("Units per second travel speed for projectiles.")]
    public float projectileSpeed = 60;

    [HideInInspector]
    protected Enemy targetedEnemy;

    [HideInInspector]
    protected float lastFireTime = Mathf.NegativeInfinity;

    //Methods:

    protected void GetNextTarget()
    {
        targetedEnemy = targeter.GetClosestEnemy(trans.position);
    }

    protected virtual void Fire()
    {
        //Mark the time we fired:
        lastFireTime = Time.time;

        //Spawn projectile prefab at spawn point, using spawn point rotation:
        var proj = Instantiate<Projectile>(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        );

        //Setup the projectile with damage, speed, and target enemy:
        proj.Setup(damage, projectileSpeed, targetedEnemy);
    }

    public void RotateBarrel(float yPos)
    {
        aimer.Rotate(0.0f, yPos, 0.0f, Space.Self);
    }

    //Unity events:
    protected virtual void Update()
    {
        GetNextTarget();

        if (!isActive)
            return;

        if (Time.time > lastFireTime + fireInterval)
        {
            Fire();
        }
    }
}
