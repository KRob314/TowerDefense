using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualFiringAATower : FiringTower
{
    [Tooltip(
        "Reference to the Transform that the projectile should be positioned and rotated with initially."
    )]
    public Transform projectileSpawnPoint2;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab2;

    // private float lastFireTimeNew = Mathf.NegativeInfinity;
    private Quaternion defaultAimerRotation;
    private Vector3 defaultAimerPosition;

    //Methods:

    protected override void Fire()
    {
        //Mark the time we fired:
        lastFireTime = Time.time;

        //Spawn projectile prefab at spawn point, using spawn point rotation:
        var proj = Instantiate<Projectile>(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        );

        var proj2 = Instantiate<Projectile>(
            projectilePrefab2,
            projectileSpawnPoint2.position,
            projectileSpawnPoint2.rotation
        );

        //Setup the projectile with damage, speed, and target enemy:
        proj.Setup(damage, projectileSpeed, targetedEnemy);
        proj2.Setup(damage, projectileSpeed, targetedEnemy);
    }

    //Unity events:
    protected override void Update()
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
        {
            GetNextTarget();
        }
        else if (!targeter.TargetsAreAvailable)
        {
            if (aimer && aimer.rotation != defaultAimerRotation)
            {
                Vector3 from = aimer.position;

                //Get desired rotation to look from the 'from' position to the 'to' position:
                Quaternion desiredRotation = Quaternion.LookRotation(
                    (defaultAimerPosition - from).normalized,
                    Vector3.up
                );

                //Slerp current rotation towards the desired rotation:
                aimer.rotation = defaultAimerRotation;
                //aimer.rotation = Quaternion.Slerp(aimer.rotation, desiredRotation, .02f);
                //aimer.position = defaultAimerPosition;
            }
        }
    }

    protected override void Start()
    {
        targeter.SetRange(range);
        defaultAimerRotation = aimer.rotation;
        defaultAimerPosition = aimer.position;
    }
}
