using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class HotPlate : TargetingTower
{
    public float damagePerSecondNew = 10;

    void Update()
    {
        if (targeter.TargetsAreAvailable)
        {
            for (int i = 0; i < targeter.enemies.Count; i++)
            {
                Enemy enemy = targeter.enemies[i];
                if (enemy is GroundEnemy)
                {
                    enemy.TakeDamage(damagePerSecondNew * Time.deltaTime);
                }
            }
        }
    }

    void Start()
    {
        slowDownRate = 0f;
    }
}
