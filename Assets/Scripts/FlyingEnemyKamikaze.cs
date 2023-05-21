using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class FlyingEnemyKamikaze : Enemy
{
    //[Tooltip("Units moved per second.")]
    //public float movespeed;

    protected Vector3 targetPosition;

    protected override void Start()
    {
        base.Start();

        float randomXPos = Random.Range(-60, 60);
        float randomYPos = 0;
        float randomZPos = Random.Range(-80, 80);

        targetPosition = new Vector3(randomXPos, 0, randomZPos);

        //Set target position to the last corner in the path:
        //targetPosition = GroundEnemy.path.corners[GroundEnemy.path.corners.Length - 1];

        //But make the Y position equal to the one we were given at start:
        //targetPosition.y = trans.position.y;
    }

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
            Die();
            Debug.Log(trans.position);
            Debug.Log(targetPosition);
            Debug.Log("--");
        }
    }
}
