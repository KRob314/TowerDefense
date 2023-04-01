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

    private Enemy targetedEnemyNew;

    private float lastFireTimeNew = Mathf.NegativeInfinity;

    //Methods:
    private void AimAtTarget()
    {
        //If the 'aimer' has been set, make it look at the enemy on the Y axis only:
        if (aimer)
        {
            //Get to and from positions, but set both Y values to 0:
            Vector3 to = targetedEnemyNew.trans.position;
            to.y = 0;

            Vector3 from = aimer.position;
            from.y = 0;

            //Get desired rotation to look from the 'from' position to the 'to' position:
            Quaternion desiredRotation = Quaternion.LookRotation(
                (to - from).normalized,
                Vector3.up
            );

            //Slerp current rotation towards the desired rotation:
            aimer.rotation = Quaternion.Slerp(aimer.rotation, desiredRotation, .08f);
        }
    }

    private void GetNextTarget()
    {
        targetedEnemyNew = targeter.GetClosestEnemy(trans.position);
    }

    private void Fire()
    {
        //Mark the time we fired:
        lastFireTimeNew = Time.time;

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
        proj.Setup(damage, projectileSpeed, targetedEnemyNew);
        proj2.Setup(damage, projectileSpeed, targetedEnemyNew);
    }
}
