using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FiringTower : TargetingTower
{
    public ParticleSystem explosionPrefab;

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
    public Enemy targetedEnemy;

    [HideInInspector]
    protected float lastFireTime = Mathf.NegativeInfinity;

    //Methods:
    protected virtual void AimAtTarget()
    {
        //If the 'aimer' has been set, make it look at the enemy on the Y axis only:
        if (aimer)
        {
            //Get to and from positions, but set both Y values to 0:
            Vector3 to = targetedEnemy.trans.position;
            to.y = 0;

            Vector3 from = aimer.position;
            from.y = 0;

            //Get desired rotation to look from the 'from' position to the 'to' position:
            Quaternion desiredRotation = Quaternion.LookRotation(
                (to - from).normalized,
                Vector3.up
            );

            //Debug.Log("desiredRotation: " + desiredRotation);

            //Slerp current rotation towards the desired rotation:
            aimer.rotation = Quaternion.Slerp(aimer.rotation, desiredRotation, .08f);
        }
    }

    protected void GetNextTarget()
    {
        targetedEnemy = targeter.GetClosestEnemy(trans.position);

        if (targetedEnemy == null)
        {
            if (explosionPrefab != null)
            {
                explosionPrefab.Pause();
            }
        }
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

        if (explosionPrefab != null)
        {
            explosionPrefab.Play();
        }
    }

    //Unity events:
    protected virtual void Update()
    {
        if (!isActive)
            return;

        if (targetedEnemy != null) //If there is a targeted enemy
        {
            //If the enemy is dead or is not in range anymore, get a new target:
            if (
                !targetedEnemy.alive
                || Vector3.Distance(trans.position, targetedEnemy.trans.position) > range
            )
            {
                GetNextTarget();
            }
            else //If the enemy is alive and in range,
            {
                if (
                    (canAttackFlying && targetedEnemy is FlyingEnemy)
                    || (canAttackGround && targetedEnemy is GroundEnemy)
                )
                {
                    //Aim at the enemy:
                    AimAtTarget();

                    //Check if it's time to fire again:
                    if (Time.time > lastFireTime + fireInterval)
                    {
                        Fire();
                    }
                }
            }
        }
        //Else if there is no targeted enemy and there are targets available
        else if (targeter.TargetsAreAvailable)
            GetNextTarget();
    }

    protected override void Start()
    {
        targeter.SetRange(range);

        if (explosionPrefab != null)
            explosionPrefab.Pause();
    }
}
