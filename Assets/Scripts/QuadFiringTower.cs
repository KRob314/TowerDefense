using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class QuadFiringTower : FiringTower
{
    [Tooltip(
        "Reference to the Transform that the projectile should be positioned and rotated with initially."
    )]
    public Transform projectileSpawnPoint2;
    public Transform projectileSpawnPoint3;
    public Transform projectileSpawnPoint4;

    public Transform barrel1;
    public Transform barrel2;
    public Transform barrel3;
    public Transform barrel4;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab2;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab3;

    [Tooltip("Reference to the projectile prefab that should be fired.")]
    public Projectile projectilePrefab4;

    private Enemy targetedEnemyNew;

    private float lastFireTimeNew = Mathf.NegativeInfinity;
    private float lastFireTime1 = Mathf.NegativeInfinity;
    private float lastFireTime2 = Mathf.NegativeInfinity;
    private float lastFireTime3 = Mathf.NegativeInfinity;
    private float lastFireTime4 = Mathf.NegativeInfinity;

    private int lastRoundFired = 0;

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

        var proj3 = Instantiate<Projectile>(
            projectilePrefab3,
            projectileSpawnPoint3.position,
            projectileSpawnPoint3.rotation
        );

        var proj4 = Instantiate<Projectile>(
            projectilePrefab4,
            projectileSpawnPoint4.position,
            projectileSpawnPoint4.rotation
        );

        //Setup the projectile with damage, speed, and target enemy:
        proj.Setup(damage, projectileSpeed, targetedEnemyNew);
        proj2.Setup(damage, projectileSpeed, targetedEnemyNew);
        proj3.Setup(damage, projectileSpeed, targetedEnemyNew);
        proj4.Setup(damage, projectileSpeed, targetedEnemyNew);
    }

    private void FireRound1()
    {
        lastRoundFired = 1;
        lastFireTimeNew = Time.time;
        lastFireTime1 = Time.time;

        var proj = Instantiate<Projectile>(
            projectilePrefab,
            projectileSpawnPoint.position,
            projectileSpawnPoint.rotation
        );

        proj.Setup(damage, projectileSpeed, targetedEnemyNew);
    }

    private void FireRound2()
    {
        lastRoundFired = 2;
        lastFireTimeNew = Time.time;
        lastFireTime2 = Time.time;
        var proj2 = Instantiate<Projectile>(
            projectilePrefab2,
            projectileSpawnPoint2.position,
            projectileSpawnPoint2.rotation
        );

        proj2.Setup(damage, projectileSpeed, targetedEnemyNew);
    }

    private void FireRound3()
    {
        lastRoundFired = 3;
        lastFireTimeNew = Time.time;
        lastFireTime3 = Time.time;
        var proj3 = Instantiate<Projectile>(
            projectilePrefab3,
            projectileSpawnPoint3.position,
            projectileSpawnPoint3.rotation
        );
        proj3.Setup(damage, projectileSpeed, targetedEnemyNew);
    }

    private void FireRound4()
    {
        lastRoundFired = 4;
        lastFireTimeNew = Time.time;
        lastFireTime4 = Time.time;
        var proj4 = Instantiate<Projectile>(
            projectilePrefab4,
            projectileSpawnPoint4.position,
            projectileSpawnPoint4.rotation
        );
        proj4.Setup(damage, projectileSpeed, targetedEnemyNew);
    }

    private void RotateBarrels()
    {
        var barrelPosition1Temp = barrel1.position;

        barrel1.position = barrel2.position;
        barrel2.position = barrel3.position;
        barrel3.position = barrel4.position;
        barrel4.position = barrelPosition1Temp;
    }

    //Unity events:
    protected override void Update()
    {
        if (targetedEnemyNew != null) //If there is a targeted enemy
        {
            //If the enemy is dead or is not in range anymore, get a new target:
            if (
                !targetedEnemyNew.alive
                || Vector3.Distance(trans.position, targetedEnemyNew.trans.position) > range
            )
            {
                GetNextTarget();
            }
            else //If the enemy is alive and in range,
            {
                if (canAttackFlying || targetedEnemyNew is GroundEnemy)
                {
                    //Aim at the enemy:
                    AimAtTarget();

                    //Check if it's time to fire again:
                    if (Time.time > lastFireTimeNew + fireInterval)
                    {
                        if (lastRoundFired == 0 || lastRoundFired == 4)
                            FireRound1();
                        else if (lastRoundFired == 1)
                            FireRound2();
                        else if (lastRoundFired == 2)
                            FireRound3();
                        else if (lastRoundFired == 3)
                            FireRound4();

                        RotateBarrels();
                    }

                    /*
                    if (Time.time > lastFireTimeNew + fireInterval)
                    {
                        if (Time.time > lastFireTime1 + fireInterval)
                        {
                            FireRound1();
                        }

                        if (Time.time > lastFireTime2 + fireInterval2)
                        {
                            FireRound2();
                        }

                        if (Time.time > lastFireTime3 + fireInterval3)
                        {
                            FireRound3();
                        }

                        if (Time.time > lastFireTime4 + fireInterval4)
                        {
                            FireRound4();
                        }
                    }*/
                }
            }
        }
        //Else if there is no targeted enemy and there are targets available
        else if (targeter.TargetsAreAvailable)
            GetNextTarget();
    }
}
