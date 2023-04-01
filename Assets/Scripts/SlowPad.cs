using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class SlowPad : TargetingTower
{
    void Update()
    {
        if (targeter.TargetsAreAvailable)
        {
            for (int i = 0; i < targeter.enemies.Count; i++)
            {
                Enemy enemy = targeter.enemies[i];
                if (enemy is GroundEnemy)
                {
                    //enemy.TakeDamage(damagePerSecond * Time.deltaTime);
                    enemy.SlowDown(slowDownRate);
                }
            }
        }
    }
}
