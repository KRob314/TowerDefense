using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class StaticProjectile : Projectile
{
    [Header("References")]
    public Transform trans;

    [Tooltip("Radius of the explosion.")]
    public float explosionRadius = 25;

    [Tooltip("Layer mask to use when detecting enemies the explosion will affect.")]
    public LayerMask enemyLayerMask;

    private Vector3 targetPosition;
    private float toXPos = 0f;
    private float toZPos = 100f;
    private float maxBounds = 99f;

    protected override void OnSetup() { }

    // Start is called before the first frame update
    void Start()
    {
        transform.Rotate(90f, 0, 0);
    }

    // Update is called once per frame
    void Update()
    {
        targetPosition = new Vector3(trans.position.x, 0, toZPos);

        Vector3 currentPosition = Vector3.MoveTowards(
            transform.position,
            targetPosition,
            speed * Time.deltaTime
        );

        transform.position = currentPosition;

        if (transform.position.x > maxBounds || transform.position.x < -maxBounds)
        {
            Destroy(gameObject);
        }

        if (transform.position.z > maxBounds || transform.position.z < -maxBounds)
        {
            Destroy(gameObject);
        }
    }

    private void OnTriggerEnter(Collider other)
    {
        if (!other.CompareTag("Enemy"))
            return;

        Debug.Log("collision");

        var enemy = other.GetComponent<Enemy>();
        enemy.TakeDamage(damage);

        /*
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
            */

        /*
        if (other.CompareTag("Powerup"))
        {
            hasPowerup = true;
            Destroy(other.gameObject);
            StartCoroutine(PowerupCountdownRoutine());
            powerupIndicator.gameObject.SetActive(true);

            for (int i = 0; i < powerUpPrefabs.Length; i++)
            {
                Instantiate(
                    powerUpPrefabs[i],
                    transform.position,
                    powerUpPrefabs[i].transform.rotation
                );
            }
        }

        */
    }
}
