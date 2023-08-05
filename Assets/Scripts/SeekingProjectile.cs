using System.Collections;
using System.Collections.Generic;
using UnityEngine;
using UnityEngine.UI;

public class SeekingProjectile : Projectile
{
    [Header("References")]
    public Transform trans;

    private Vector3 targetPosition;

    protected override void OnSetup() { }

    // Start is called before the first frame update
    void Start() { }

    // Update is called once per frame
    void Update()
    {
        if (targetEnemy != null)
        {
            targetPosition = targetEnemy.projectileSeekPoint.position;
        }

        //point towards the target position
        trans.forward = (targetPosition - trans.position).normalized;

        //move towards the target position
        trans.position = Vector3.MoveTowards(
            trans.position,
            targetPosition,
            speed * Time.deltaTime
        );

        //if we have reached the target position
        if (trans.position == targetPosition)
        {
            if (targetEnemy != null)
                targetEnemy.TakeDamage(damage);

            Destroy(gameObject);
        }
    }
}
