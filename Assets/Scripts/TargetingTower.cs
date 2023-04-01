using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class TargetingTower : Tower
{
    public Targeter targeter;
    public int range = 45;
    public float damagePerSecond = 0;
    public float slowDownRate = 2.0f;

    protected virtual void Start()
    {
        targeter.SetRange(range);
    }
}
