using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class DualFiringTower : FiringTower
{
    [Tooltip(
        "Reference to the Transform that the projectile should be positioned and rotated with initially."
    )]
    public Transform projectileSpawnPoint2;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab2;

    //private float lastFireTime = Mathf.NegativeInfinity;

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

        /*
 
         if (targetedEnemyNew is GroundEnemy)
         {
             Debug.Log("ground enemy");
             proj.Setup(groundDamage, projectileSpeed, targetedEnemyNew);
             proj2.Setup(groundDamage, projectileSpeed, targetedEnemyNew);
         }
         else if (targetedEnemyNew is FlyingEnemy)
         {
             Debug.Log("fly enemy");
             proj.Setup(airDamage, projectileSpeed, targetedEnemyNew);
             proj2.Setup(airDamage, projectileSpeed, targetedEnemyNew);
         }
 */
        proj.Setup(damage, projectileSpeed, targetedEnemy);
        proj2.Setup(damage, projectileSpeed, targetedEnemy);
    }

    protected override void Update()
    {
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
}
