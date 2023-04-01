using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class Enemy : MonoBehaviour
{
    [Header("References")]
    public Transform trans;
    public Transform projectileSeekPoint;

    [Header("Stats")]
    public float maxHealth;
    public int maxLifes = 0; // + 1
    private int remainingLifes;

    public float healthGainPerLevel;

    [HideInInspector]
    public float health;

    [HideInInspector]
    public bool alive = true;

    [Tooltip("Units moved per second.")]
    public float movespeed = 22;
    private float movespeedBase = 22;

    //Methods:
    public void SlowDown(float slowDownRate)
    {
        //Debug.Log("slowDown()");
        var moveSpeedTemp = movespeedBase / slowDownRate;

        //Debug.Log(movespeed);
        //Debug.Log(movespeedBase);
        //Debug.Log(moveSpeedTemp);

        if (movespeed > moveSpeedTemp)
        {
            movespeed = moveSpeedTemp;
        }
    }

    public void NormalSpeed()
    {
        movespeed = movespeedBase;
    }

    public void TakeDamage(float amount)
    {
        //Only proceed if damage taken is more than 0:
        if (amount > 0)
        {
            //Reduce health by 'amount' but don't go under 0:
            health = Mathf.Max(health - amount, 0);

            //If all health is lost,
            if (health <= 0)
            {
                if (remainingLifes > 0)
                {
                    //Debug.Log("remaining lifes: " + remainingLifes);
                    ReduceOneLife();
                }
                else
                {
                    //Debug.Log("die");
                    Die();
                }
            }
        }
    }

    private void ReduceOneLife()
    {
        health = maxHealth;
        remainingLifes -= 1;

        if (remainingLifes == 0)
        {
            projectileSeekPoint.GetComponent<Renderer>().material.color = Color.red;
        }
        else
        {
            projectileSeekPoint.GetComponent<Renderer>().material.color = Color.white;
        }

        Vector3 objectScale = projectileSeekPoint.localScale;
        projectileSeekPoint.localScale = new Vector3(
            objectScale.x,
            objectScale.y - 2,
            objectScale.z
        );
    }

    public void Die()
    {
        if (alive)
        {
            alive = false;
            remainingLifes = 0;
            Destroy(gameObject);
        }
    }

    public void Leak()
    {
        Player.remainingLives -= 1;
        Destroy(gameObject);
    }

    //Unity events:
    protected virtual void Start()
    {
        remainingLifes = maxLifes;
        movespeedBase = movespeed;
        maxHealth = maxHealth + (healthGainPerLevel * (Player.level - 1));
        health = maxHealth;
    }
}
