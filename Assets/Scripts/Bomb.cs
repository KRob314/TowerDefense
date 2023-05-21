using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Bomb : TargetingTower
{
    public Transform trans;

    public ParticleSystem explosionPrefab;

    [Tooltip("Layer mask to use when detecting enemies the explosion will affect.")]
    public LayerMask enemyLayerMask;

    [Tooltip("Radius of the explosion.")]
    public float explosionRadius = 25;

    public float secondsToDetonateAfterTrigger = 1.0f;
    private float triggerStartTime = Mathf.NegativeInfinity;
    private float explodeTime = Mathf.NegativeInfinity;
    public float secondsToExplodeAfterTrigger = 0.5f;

    void Update()
    {
        //if (explodeTime == Mathf.NegativeInfinity)
        //{
        if (targeter.TargetsAreAvailable)
        {
            if (triggerStartTime == Mathf.NegativeInfinity)
            {
                triggerStartTime = Time.time;
                var particleSystem = GetComponent<ParticleSystem>();
                particleSystem.Play();
            }
        }

        if (
            triggerStartTime != Mathf.NegativeInfinity
            && (Time.time - triggerStartTime) > secondsToDetonateAfterTrigger
        )
        {
            Explode();
        }
        //}

        // Debug.Log(explodeTime);
        //Debug.Log(Time.time - explodeTime);
        // Debug.Log("--");
        /*
                if (
                    explodeTime != Mathf.NegativeInfinity
                    && (Time.time - explodeTime) > secondsToExplodeAfterTrigger
                )
                {
                    Debug.Log("here");
                }
                */
    }

    protected override void Start()
    {
        var particleSystem = GetComponent<ParticleSystem>();
        particleSystem.Pause();
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
                    damagePerSecond * (1 - Mathf.Clamp(distToEnemy / explosionRadius, 0f, 1f));
                enemy.TakeDamage(damageToDeal);
            }
        }

        explosionPrefab = Instantiate(
            explosionPrefab,
            trans.position,
            Quaternion.LookRotation(Vector3.back)
        );

        Destroy(gameObject);
    }
}
