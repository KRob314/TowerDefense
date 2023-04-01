using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class ArcingProjectile : Projectile
{
    public Transform trans;

    [Tooltip("Layer mask to use when detecting enemies the explosion will affect.")]
    public LayerMask enemyLayerMask;

    [Tooltip("Radius of the explosion.")]
    public float explosionRadius = 25;

    [Tooltip(
        "Curve that should go from value 0 to 1 over 1 second. Defines the curve of the projectile."
    )]
    public AnimationCurve curve;

    //Position we're aiming to travel to. Will always have a Y value of 0.
    private Vector3 targetPosition;

    //Our position when we spawned.
    private Vector3 initialPosition;

    //Total distance we'll travel from initial position to target position, not counting the Y axis.
    private float xzDistanceToTravel;

    //Time.time at which we spawned.
    private float spawnTime;
    private float FractionOfDistanceTraveled
    {
        get
        {
            float timeSinceSpawn = Time.time - spawnTime;
            float timeToReachDestination = xzDistanceToTravel / speed;
            return timeSinceSpawn / timeToReachDestination;
        }
    }

    protected override void OnSetup()
    {
        //Set initial position to our current position, and target position to the target enemy position:
        initialPosition = trans.position;
        targetPosition = targetEnemy.trans.position;
        //Make sure the target position is always at a Y of 0:
        targetPosition.y = 0;
        //Calculate the total distance we'll need to travel on the X and Z axes:
        xzDistanceToTravel = Vector3.Distance(
            new Vector3(trans.position.x, targetPosition.y, trans.position.z),
            targetPosition
        );

        //Mark the Time.time we spawned at:
        spawnTime = Time.time;
    }

    void Update()
    {
        Vector3 currentPosition = trans.position;
        currentPosition.y = 0;

        currentPosition = Vector3.MoveTowards(
            currentPosition,
            targetPosition,
            speed * Time.deltaTime
        );
        currentPosition.y = Mathf.Lerp(
            initialPosition.y,
            targetPosition.y,
            curve.Evaluate(FractionOfDistanceTraveled)
        );
        trans.position = currentPosition;

        if (currentPosition == targetPosition)
            Explode();
    }

    void Explode()
    {
        Collider[] enemyColliders = Physics.OverlapSphere(
            trans.position,
            explosionRadius,
            enemyLayerMask.value
        );

        for (int i = 0; i < enemyColliders.Length; i++)
        {
            var enemy = enemyColliders[i].GetComponent<Enemy>();

            if (enemy != null)
            {
                float distToEnemy = Vector3.Distance(trans.position, enemy.trans.position);
                float damageToDeal =
                    damage * (1 - Mathf.Clamp(distToEnemy / explosionRadius, 0f, 1f));
                enemy.TakeDamage(damageToDeal);
            }
        }

        Destroy(gameObject);
    }
}
