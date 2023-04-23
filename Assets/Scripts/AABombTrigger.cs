using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class AABombTrigger : MonoBehaviour
{
    public Animator anim;

    private void OnTriggerEnter(Collider other)
    {
        anim.SetTrigger("EnemyHit");
    }

    private void OnTriggerExit(Collider other)
    {
        anim.SetTrigger("EnemyHit");
    }
}
