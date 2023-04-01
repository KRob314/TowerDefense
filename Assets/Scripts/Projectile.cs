using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public abstract class Projectile : MonoBehaviour
{
    [HideInInspector]
    public float damage;
    public float groundDamage;
    public float airDamage;
    
    public float armorDamage = 0;

    [HideInInspector]
    public float speed;

    [HideInInspector]
    public Enemy targetEnemy;

    public void Setup(float damage, float speed, Enemy targetEnemy)
    {
        this.damage = damage;
        this.speed = speed;
        this.targetEnemy = targetEnemy;
        OnSetup();
    }

    protected abstract void OnSetup();
}
