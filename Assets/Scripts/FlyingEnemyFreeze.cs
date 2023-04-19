using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using System.Linq;

public class FlyingEnemyFreeze : FlyingEnemy
{
    private ParticleSystem particleSystem = null;
    private bool hasUsedAbility = false;

    protected override void Start()
    {
        particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Pause();

        base.Start();

        //Set target position to the last corner in the path:
        targetPosition = GroundEnemy.path.corners[GroundEnemy.path.corners.Length - 1];

        //But make the Y position equal to the one we were given at start:
        targetPosition.y = trans.position.y;
    }

    // Update is called once per frame
    void Update()
    {
        //Move towards the target position:
        trans.position = Vector3.MoveTowards(
            trans.position,
            targetPosition,
            movespeed * Time.deltaTime
        );

        //Leak if we've reached the target position:
        if (trans.position == targetPosition)
        {
            Leak();
        }

        // Debug.Log("Enemy position z: " + trans.position.z);
        //if (!hasUsedAbility)
        //{
        System.Random rnd = new System.Random();
        var randomNum = rnd.Next(1, 50);
        foreach (var towerItem in _towers)
        {
            //if (!towerItem.Value.isActive)
            //return;
            //Debug.Log("tower position: " + towerItem.Key.z);



            if (
                towerItem.Key.z >= trans.position.z - 10
                && towerItem.Key.z <= trans.position.z + 10
                && !particleSystem.isEmitting
            )
            {
                particleSystem.Play();
            }

            if (
                towerItem.Key.x >= -30
                && towerItem.Key.x <= 30
                && towerItem.Key.z >= trans.position.z - 5
                && towerItem.Key.z <= trans.position.z + 5
            )
            {
                // if (randomNum < 5)
                //{
                if ((towerItem.Value is DualFiringAATower) == false)
                {
                    //towerItem.Value.isActive = false;
                    towerItem.Value.SetInactive();
                    //hasUsedAbility = true;
                    //Debug.Log("freeze tower");
                }
                //}
            }
        }
        //}
    }
}
